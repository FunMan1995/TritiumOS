needs lib/wordtbl mem/dict text/ts asm/riscv lib/str
unit asm/riscvd

\ Disassembler Program Counter
0 value dpc
16 value listingsz

\ This value should remain at 0 during regular use.
\ The test harness sets this to a test buffer to simulate
\ disassembling from 0, which make automatic checking easier.
0 value dpc_offset

\ Field encoding and decoding logic
: fieldextract ( instr this -- extractedField ) A! @ rshift A> 4+ @ and ;

0 value fieldptr
: _fieldshft@ ( -- shft ) fieldptr @ ;
: _fieldmsk@  ( -- shiftedmsk ) fieldptr 4+ @ _fieldshft@ lshift ;
: _fieldinvmsk  ( n -- nmskd ) _fieldmsk@ invand ;

: _fieldinsert ( val msk n -- val msk )
  _fieldshft@ lshift _fieldmsk@ and ( val msk nshftmskd )
  rot ( msk nshftmskd val ) or ( msk val )
  swap ( val msk ) _fieldmsk@ or ( val msk ) ;
: fieldinsert ( val msk n this -- instr msk ) to fieldptr _fieldinsert ;

: opfield ( mask rshift )
  here# rot> , , dup ['] fieldextract bind ['] fieldinsert bind ;

$7f     0 opfield opc@ opc!
$1f     7 opfield rd@ rd!
$7     12 opfield fun3@ fun3!
$1f    15 opfield rs1@ rs1!
$1f    20 opfield rs2@ rs2!
$1f    20 opfield shamt@ shamt!
$7f    25 opfield fun7@ fun7!
$fff   20 opfield _Iimm@ Iimm!
$7f    25 opfield SBimmHi@ SBimmHi!
$1f     7 opfield SBimmLo@ SBimmLo!
$fffff 12 opfield UJimm@ UJimm!

: SBimm@ ( instr - sbimm ) dup SBimmHi@ 5 lshift swap SBimmLo@ or ;

\ Destination register number and value of AUIPC and LUI instructions
\ are saved for extended address ranged decoding of subsequent itype
\ (jalr, addi) instruction

0 value saved_reg.num
0 value saved_reg.val
0 value saved_reg.ttl

: save_reg ( regnum regval -- )
  to saved_reg.val
  to saved_reg.num
  \ Set time-to-live to 2. It will be decremented each subsequent instruction.
  2 to saved_reg.ttl ;

\ Do we have this register's value?
: saved_reg? ( regnum -- regval? f )
  case
    \ we always know the value of xZERO.
    xZERO = of 0 1 endof
    \ given register is saved and not yet expired.
    saved_reg.num = saved_reg.ttl 0 > and of saved_reg.val 1 endof
    drop 0
  endcase ;

\ Print stuffs

stringlist _ ZERO RA RSP PSP W A S SYSVARS \
 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 Acmp Bcmp 30 31
: .reg ( n -- ) 'x' emit _ slistiter stype ;
: .$x  ( n -- ) '$' emit .x ;
: .,spc ',' emit spc> ;
: .imm ( n -- ) dup . spc> '(' emit .$x ')' emit ;

\ Print address as symbol
: .addr ( a -- )
  ."->" spc>
  dup dpc_offset + ?xt>e \ attempt word look-up
  ?dup if nip entryname[] rtype
  else .$x then
  spc> ;

\ Immediate decoding logic

\ Sign extend 12-bit number
code sex12 ( n -- n ) 20 i) <<, 20 i) signed) >>, exit,

\ Sign extend 20-bit number
code sex20 ( n -- n ) 12 i) <<, 12 i) signed) >>, exit,

\ S-type immediate decode
: Simm@ SBimm@ sex12 ;

\ I-type immediate decode
: Iimm@ _Iimm@ sex12 ;

\ B-type immediate decode
: _  ( n -- n )
  dup $800 and 1 lshift ( n n1 )
  over 1 and 11 lshift ( n n1 n2 )
  or ( n n3 )
  swap $7fe and ( n3 n4 )
  or ( n5 )
  sex12 ( n ) ;
: Bimm@ SBimm@ _ ;

\ J-type immediate decode
\ Signed left shift: negative numbers shift right.
: slshft ( n u - n ) dup 0< if neg rshift else lshift then ;
: msk_shft ( n shft msk -- n ) rot and swap slshft ;
: _ ( n -- n )
  dup 12 $ff msk_shft ( n n1 )
  over 3 $100 msk_shft ( n n1 n2 )
  or ( n n3 )
  over -8 $7fe00 msk_shft ( n n3 n4 )
  or ( n n5 )
  swap 1 $80000 msk_shft ( n5 n6 )
  or ( n )
  sex20 ( n ) ;
: Jimm@ UJimm@ _ ;

\ U-type immediate decode
: Uimm@ UJimm@ 12 lshift ;

\ Decoders according to instruction type (R-Type, I-Type...)

\ e.g. add x3, x1, x2
: dec_rtype ( instr -- )
  dup rd@ .reg .,spc
  dup rs1@ .reg .,spc
  rs2@ .reg ;

\ e.g. mv x20, x10
: dec_mv ( instr -- )
  dup rd@ .reg .,spc
  rs1@ .reg ;

\ e.g. addi x11, x10, $100
: dec_itype ( instr -- )
  dup rd@ .reg .,spc ( instr )
  dup rs1@ dup .reg .,spc ( instr rs1 )
  swap Iimm@ ( rs1 imm_lo )
  dup . spc> ( rs1 imm_lo )
  swap ( imm_lo rs1 )
  saved_reg? if + .addr else drop then ;

\ e.g. jalr xRA, x10[$100]
: dec_itype_base_rel ( instr -- )
  dup rd@ .reg .,spc ( instr )
  dup rs1@ dup .reg ( instr rs1 )
  swap Iimm@ ( rs1 imm_lo )
  dup '[' emit . ']' emit spc> ( rs1 imm_lo )
  swap ( imm_lo rs1 )
  saved_reg? if + .addr else drop then ;

\ e.g. srli x2, x1, 4
: dec_shft_itype ( instr -- )
  dup rd@ .reg .,spc
  dup rs1@ .reg .,spc
  shamt@ . ;

\ e.g. sb x10, x11[12]
: dec_stype ( instr -- )
  dup rs2@ .reg .,spc
  dup rs1@ .reg '[' emit Simm@ . ']' emit ;

\ e.g. beq x2, x1, $1000
: dec_btype ( instr -- )
  dup rs1@ .reg .,spc
  dup rs2@ .reg .,spc
  Bimm@ dup . spc> dpc + .addr ;

\ e.g. lui x1, $10000000
: dec_utype ( instr -- )
  dup rd@ dup .reg .,spc ( instr rd )
  swap Uimm@ dup .imm ( rd imm )
  save_reg ;

\ e.g. auipc xRA, $10000000
: dec_auipc ( instr -- )
  dup rd@ dup .reg .,spc ( instr rd )
  swap Uimm@ dup .imm ( rd imm )
  dpc + save_reg ;

\ e.g. jal x1, $20000000
: dec_jtype ( instr -- )
  dup rd@ .reg .,spc
  Jimm@ dup . spc> dpc + .addr ;

: dec_noargs ( instr -- ) drop ;

: iname, ( s -- )
  c@+ ( s+1 n ) dup 7 > ?abort"Name too long." ( s+1 n )
  here "       " s, 1+ ( srcs+1 n dsts+1 )
  swap cmove ;

\ fltr( <opfield> <opfield> ...)fltr, builds an instruction filter row,
\ i.e. a mask-value-name-decoder tuple. The opfields between the
\ brackets define the filter's bit fields and values.
: fltr( ( -- 0 0 ) 0 0 ;
: )fltr, ( name v m "decoder" -- ) , , iname, ' , ;

\ instruction filter table entries
create iftbl_start
"lui" fltr( $37 opc! )fltr, dec_utype
"auipc" fltr( $17 opc! )fltr, dec_auipc
"jal" fltr( $6F opc! )fltr, dec_jtype
"ret" fltr( xZERO rd! xRA rs1! 0 Iimm! 0 fun3! $67 opc! )fltr, dec_noargs
"jalr" fltr( 0 fun3! $67 opc! )fltr, dec_itype_base_rel
"beq" fltr( 0 fun3! $63 opc! )fltr, dec_btype
"bne" fltr( 1 fun3! $63 opc! )fltr, dec_btype
"blt" fltr( 4 fun3! $63 opc! )fltr, dec_btype
"bge" fltr( 5 fun3! $63 opc! )fltr, dec_btype
"bltu" fltr( 6 fun3! $63 opc! )fltr, dec_btype
"bgeu" fltr( 7 fun3! $63 opc! )fltr, dec_btype
"lb" fltr( 0 fun3! $03 opc! )fltr, dec_itype_base_rel
"lh" fltr( 1 fun3! $03 opc! )fltr, dec_itype_base_rel
"lw" fltr( 2 fun3! $03 opc! )fltr, dec_itype_base_rel
"lbu" fltr( 4 fun3! $03 opc! )fltr, dec_itype_base_rel
"lhu" fltr( 5 fun3! $03 opc! )fltr, dec_itype_base_rel
"sb" fltr( 0 fun3! $23 opc! )fltr, dec_stype
"sh" fltr( 1 fun3! $23 opc! )fltr, dec_stype
"sw" fltr( 2 fun3! $23 opc! )fltr, dec_stype
"nop" fltr( xZERO rd! xZERO rs1! 0 Iimm! 0 fun3! $13 opc! )fltr, dec_noargs
"mv" fltr( 0 Iimm! 0 fun3! $13 opc! )fltr, dec_mv
"addi" fltr( 0 fun3! $13 opc! )fltr, dec_itype
"slti" fltr( 2 fun3! $13 opc! )fltr, dec_itype
"sltiu" fltr( 3 fun3! $13 opc! )fltr, dec_itype
"xori" fltr( 4 fun3! $13 opc! )fltr, dec_itype
"ori" fltr( 6 fun3! $13 opc! )fltr, dec_itype
"andi" fltr( 7 fun3! $13 opc! )fltr, dec_itype
"slli" fltr( 1 fun3! $13 opc! )fltr, dec_shft_itype
"srli" fltr( $00 fun7! 5 fun3! $13 opc! )fltr, dec_shft_itype
"srai" fltr( $20 fun7! 5 fun3! $13 opc! )fltr, dec_shft_itype
"add" fltr( $00 fun7! 0 fun3! $33 opc! )fltr, dec_rtype
"sub" fltr( $20 fun7! 0 fun3! $33 opc! )fltr, dec_rtype
"sll" fltr( $00 fun7! 1 fun3! $33 opc! )fltr, dec_rtype
"slt" fltr( $00 fun7! 2 fun3! $33 opc! )fltr, dec_rtype
"sltu" fltr( $00 fun7! 3 fun3! $33 opc! )fltr, dec_rtype
"xor" fltr( $00 fun7! 4 fun3! $33 opc! )fltr, dec_rtype
"srl" fltr( $00 fun7! 5 fun3! $33 opc! )fltr, dec_rtype
"sra" fltr( $20 fun7! 5 fun3! $33 opc! )fltr, dec_rtype
"or" fltr( $00 fun7! 6 fun3! $33 opc! )fltr, dec_rtype
"and" fltr( $00 fun7! 7 fun3! $33 opc! )fltr, dec_rtype
"mul" fltr( $01 fun7! 0 fun3! $33 opc! )fltr, dec_rtype
"mulh" fltr( $01 fun7! 1 fun3! $33 opc! )fltr, dec_rtype
"mulhsu" fltr( $01 fun7! 2 fun3! $33 opc! )fltr, dec_rtype
"mulhu" fltr( $01 fun7! 3 fun3! $33 opc! )fltr, dec_rtype
"div" fltr( $01 fun7! 4 fun3! $33 opc! )fltr, dec_rtype
"divu" fltr( $01 fun7! 5 fun3! $33 opc! )fltr, dec_rtype
"rem" fltr( $01 fun7! 6 fun3! $33 opc! )fltr, dec_rtype
"remu" fltr( $01 fun7! 7 fun3! $33 opc! )fltr, dec_rtype
"fence.t" fltr( $833 Iimm! 0 rs1! 0 fun3! 0 rd! $0f opc! )fltr, dec_noargs
"pause" fltr( $010 Iimm! 0 rs1! 0 fun3! 0 rd! $0f opc! )fltr, dec_noargs
"ecall" fltr( $000 Iimm! 0 rs1! 0 fun3! 0 rd! $73 opc! )fltr, dec_noargs
"ebreak" fltr( $001 Iimm! 0 rs1! 0 fun3! 0 rd! $73 opc! )fltr, dec_noargs
here value iftbl_end

0 value if_row
: if_row.msk ( -- v ) if_row @ ;
: if_row.val ( -- v ) if_row 4+ @ ;
: if_row.name ( -- s ) if_row 8+ ;
: if_row.decoder ( -- v ) if_row 16 + @ ;
: if_row.next ( -- ) if_row 20 + to if_row ;
: if_row.match? ( instr -- f ) if_row.msk and if_row.val = ;

: decode_instr ( instr -- )
  iftbl_start to if_row
  begin ( instr )
    dup if_row.match? if ( instr )
      if_row.name stype ( instr )
      if_row.decoder execute
      exit
    then ( instr )
    if_row iftbl_end = ( instr f )
  if_row.next until ( instr )
  drop
  ."???    " ;

: .op ( -- )
  ts[
    dpc .x spc> dpc dpc_offset + @ ( instr )
    dup decode_instr ( instr )
    50 tsgo .x nl>
    \ decrement saved register time-to-live until 0
    saved_reg.ttl ?dup if 1- to saved_reg.ttl then
    dpc 4+ to dpc
  ]ts ;

: dis1 ( a -- ) to dpc .op ;
: dis ( a -- ) to dpc listingsz 0 do .op loop ;
: disn ( -- ) dpc dis ;

