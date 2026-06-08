needs lib/str lib/wordtbl lib/fmt lib/bit num/math mem/dict text/ts
unit asm/x86d

\ TODO: endian-proof

: err abort"x86d error" ;
16 value listingsz \ number of lines in a listing
0 value dpc     \ disassembler program counter
0 value opcode  \ base opcode currently being decoded
0 value 16baddr \ are we on a $67 prefix?
0 value 16bdata \ are we on a $66 prefix?
0 value page2?  \ are we on page 2 ($0f)?
0 value jmpaddr \ address targeted by a jump or call
: op$ 0 to page2? 0 to jmpaddr 0 to 16baddr 0 to 16bdata ;
\ 0=8b 1=32b 2=16b
: opsz opcode 1 and dup if 16bdata + then ;
: force8b opcode $fe and to opcode ;
: force32b opcode 1 or to opcode ;
: opdir opcode 2/ 1 and ;
: spitmem ( a u -- ) 0 do c@+ .x1 spc> loop drop ;
: _go ( pos -- ) dup tspos < if drop nl> 2 then tsgo ;
3 wordrefs _ .x1 .x .x2
: _.x? _ opsz wexec ;
: ., ."," ;
create _ ,"{[("
: .[ _ opsz + c@ emit ;
create _ ,"}])"
: .] _ opsz + c@ emit ;

\ operation arguments handlers. We align to 18 chars wide
: dpc1@+ doto dpc dup 1+ | c@ ;
: dpc2@+ doto dpc dup 2+ | wle@ ;
: dpc4@+ doto dpc dup 4+ | le@ ;
3 wordrefs _ dpc1@+ dpc4@+ dpc2@+
: dpc@+ _ opsz wexec ;
: modrmsplit ( modrm -- mod reg rm ) tri 6 rshift | 3 rshift 7 and | 7 and ;
create regnames  ,"ALCLDLBLAHCHDHBHAXCXDXBXSPBPSIDI"
: regs regnames opsz 1 min 16 * + ;
: .reg ( idx -- ) opsz 1 = if ."E" then 2* regs + 2 rtype ;
: .regd ( idx -- ) ."E" 8+ 2* regnames + 2 rtype ;
: ?.SIB ( idx -- idx )
  dup 4 <> if .regd else
    drop dpc1@+ modrmsplit .regd
    dup 4 = if 2drop else
      ."+" .regd ?dup if ."*" pow2 . then then then ;
: .[reg] ( idx -- ) .[ ?.SIB .] ;
: .[reg+b] .[ ?.SIB ."+" dpc1@+ .x1 .] ;
: .[reg+wd] .[ ?.SIB ."+" dpc4@+ .x .] ;
: .mem ( -- ) .[ dpc4@+ .x .] ;
wordtbl[ ( idx -- )
:> dup 5 = if drop .mem else .[reg] then ;
' .[reg+b]
' .[reg+wd]
' .reg
]wordtbl _
: .rm ( mod rm -- ) _ rot wexec ;
: modrm ( -- )
  dpc1@+ modrmsplit ( mod reg rm ) opdir if
    swap .reg ., .rm
    else rot swap .rm ., .reg then ;
: .imm dpc@+ _.x? ;
: .imm8 dpc1@+ .x1 ;
: .AXimm ( -- ) 0 .reg ., .imm ;
: _.addr dup to jmpaddr .x ;
: .rel8 dpc1@+ sex8 dpc + _.addr ;
: .rel dpc4@+ dpc + _.addr ;
\ A modrm where the "reg" field is ignored. We only have rm as dst.
: .modrmsingle ( -- ) dpc1@+ modrmsplit nip .rm ;
\ a modrm where reg=opcode, rm=dst and an immediate is the src
: .modrmimm ( -- ) .modrmsingle ., .imm ;
\ A modrm+imm where dst is wide and imm is sign-extended 8bit
: .modrmimm8 ( -- ) .modrmsingle ., .imm8 ;

\ opcode handlers. signature: ( opcode -- )
alias err _.op ( -- ) \ forward declaration
: unknown drop ."???? " ;
4 const MNEMONICSZ
: .mnemonic ( a -- ) MNEMONICSZ rtype spc> ;
create groups ,"ADD OR  ADC SBB AND SUB XOR CMP "
              ,"ROL ROR RCL RCR SHL SHR ????SAR "
              ,"TEST????NOT NEG MUL IMULDIV IDIV"
              ,"INC DEC ????????????????????????"
              ,"INC DEC CALLCALLJMP JMP PUSH????"
              ,"SLDTSTR LLDTLTR VERRVERW????????"
              ,"SGDTSIDTLGDTLIDTSMSW????LMSW????"
              ,"????????????????BT  BTS BTR BTC "
\ peek following modrm to print the proper op name from group index
: .group ( idx -- )
  32 * groups + dpc c@ modrmsplit drop nip ( tbl idx )
  MNEMONICSZ * + .mnemonic ;
: .opnametbl ( idx tbl -- )
  swap MNEMONICSZ * + ( a ) dup "GRP" c@+ c[]= if
    3 + c@ '1' - .group else .mnemonic then ;

\ one of the 8 regular arithmetic ops in rows 0 to 3
4 wordrefs _ modrm modrm .AXimm err
: _args ( opcode -- ) 7 and _ swap 2/ wexec ;
: ?arireg ( opcode -- ?opcode f ) \ f=should continue handling
  dup 6 and 6 <> if dup 3 rshift groups .opnametbl _args 0 else 1 then ;

\ push/pop in rows 0 and 1
create _ ,"PUSHPOP "
: pushpop01 ( opcode -- )
  dup $0f = if drop 1 to page2? _.op exit then
  1 and ( idx ) _ .opnametbl ;

\ row 0 to 3 have a very similar structure
: row01 ?arireg if pushpop01 then ;
: row23 ?arireg if drop ."SEG  " then ;

\ row 4 and 5 have the same structure
create _ ,"INC DEC PUSHPOP "
: row45 dup 3 rshift 3 and _ .opnametbl ( opcode ) 7 and .regd ;

create _ ,"PSHAPOPABOUNARPLSEG SEG PRFDPRFAPUSHIMULPUSHIMULINS INS OUTSOUTS"
: .imul modrm ., .imm ;
: .imul8 modrm ., .imm8 ;
: .BWD "BDW" 1+ opsz + c@ emit ;
16 wordrefs _args
  noop noop modrm modrm noop noop noop noop
  .imm .imul .imm .imul8 .BWD .BWD .BWD .BWD
: row6 dup $66 = if drop 1 to 16bdata _.op exit then
       dup $67 = if drop 1 to 16baddr _.op exit then
       $f and dup _ .opnametbl _args swap wexec ;

stringlist _ O B Z BE S P L LE
: _.cond ( opcode -- )
  dup 1 and if ."N" then 2/ 7 and _ slistiter c@+ rtype ;
: row7 ."Jc   " _.cond ., page2? if .rel else .rel8 then ;

create _ ,"GRP1GRP1????GRP1TESTTESTXCHGXCHGMOV MOV MOV MOV MOV LEA MOV POP "
8 wordrefs _args
  .modrmimm .modrmimm8 modrm modrm modrm modrm modrm noop
: row8 $f and dup _ .opnametbl _args swap 2/ wexec ;

create _ ,"NOP XCHGXCHGXCHGXCHGXCHGXCHGXCHGCBW CWD CALLWAITPSHFPOPFSAHFLAHF"
: .far dpc4@+ dpc2@+ .x2 .":" .x ;
: .xchg opcode $f and force32b 0 .reg ., .reg ;
16 wordrefs _args
  noop .xchg .xchg .xchg .xchg .xchg .xchg .xchg
  noop noop .far noop noop noop noop noop
: row9 $f and dup _ .opnametbl _args swap wexec ;

create _ ,"MOV MOV MOVSCMPSTESTSTOSLODSSCAS"
: .AXmem ;
8 wordrefs _args .AXmem .AXmem .BWD .BWD .AXimm .BWD .BWD .BWD
: rowA $f and 2/ dup _ .opnametbl _args swap wexec ;

: rowB ."MOV  " dup 3 rshift to opcode 7 and .reg ., .imm ;

create _ ,"GRP2GRP2RET RET LES LDS MOV MOV ENTRLEAVRETFRETFINT INT INTOIRET"
8 wordrefs _args .modrmimm8 noop noop .modrmimm noop noop noop noop
: rowC $f and dup _ .opnametbl _args swap 2/ wexec ;

create _ ,"GRP2GRP2GRP2GRP2AAM AAD     XLATESC ESC ESC ESC ESC ESC ESC ESC "
: .rot1 .modrmsingle ., ."1" ;
: .rotcl .modrmsingle ., ."CL" ;
8 wordrefs _args .rot1 .rotcl noop noop noop noop noop noop
: rowD $f and dup _ .opnametbl _args swap 2/ wexec ;

create _ ,"LOOPLOOPLOOPJCXZIN  IN  OUT OUT CALLJMP JMP JMP IN  IN  OUT OUT "
: .loopc opcode 4 + _.cond ., .rel8 ;
: .AX,DX 0 .reg ., 2 .regd ;
: .DX,AX 2 .regd ., 0 .reg ;
: .AXimm8 ( -- ) 0 .reg ., .imm8 ;
: .imm8AX .imm8 ., 0 .reg ;
$10 wordrefs _args
  .loopc .loopc .rel8 .rel8 .AXimm8 .AXimm8 .imm8AX .imm8AX
  .rel .rel .far .rel8 .AX,DX .AX,DX .DX,AX .DX,AX
: rowE $f and dup _ .opnametbl _args swap wexec ;

create _ ,"LOCK    REP REP HLT CMC GRP3GRP3CLC STC CLI STI CLD STD GRP4GRP5"
: .rep opcode 2 + 1 xor _.cond ;
8 wordrefs _args noop .rep noop .modrmsingle noop noop noop .modrmsingle
: rowF $f and dup _ .opnametbl _args swap 2/ wexec ;

\ opcode table page 1. row based
$10 wordrefs page1
  row01 row01 row23 row23 row45 row45 row6 row7
  row8 row9 rowA rowB rowC rowD rowE rowF

: row0f9 ."SETc " _.cond ., force8b .modrmsingle ;

create _ ,"        LSS BTR LFS LGS MVZXMVZX        GRP8BTC BSF BSR MVSXMVSX"
: .movzx
  dpc1@+ modrmsplit ( mod reg rm )
  swap .reg ., 1 to 16bdata .rm ;
8 wordrefs _args noop noop noop .movzx noop noop noop .movzx
: row0fb $f and dup _ .opnametbl _args swap 2/ wexec ;

$10 wordrefs page2
  unknown unknown unknown unknown unknown unknown unknown unknown
  row7 row0f9 unknown row0fb unknown unknown unknown unknown

create _ page1 , page2 ,
: optbl page2? if page2 else page1 then ;

\ high level words
:realias _.op ( -- )
  optbl dpc1@+ dup to opcode ( tbl opcode )
  tuck 4 rshift wexec ( ) ;

: .op ( -- ) \ decode next opcode and print it
  ts[ op$ dpc dup .x spc> _.op ( origpc )
  28 _go
  jmpaddr dup if ?xt>e then ( origpc entry )
  ?dup if entryname[] rtype drop else dpc over - spitmem then nl> ]ts ;

: dis1 ( a -- ) to dpc .op ;
: dis ( a -- ) \ Disassemble instructions starting at a
  to dpc listingsz 0 do .op loop ;
: disn ( -- ) dpc dis ;
