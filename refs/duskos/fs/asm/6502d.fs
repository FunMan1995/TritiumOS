needs asm/label
unit asm/6502d

\ TODO: not working properly. This has been directly copied from Collapse OS
\ but diassembler semantics differ from Dusk. Will adjust and coment later.
\ For now, only some words are used by emul/6502.

\ order below represent "opid", also used in emulator
create OPNAME ,"ORAANDEORADCSTALDACMPSBC" \ 1/5/9/d x8
,"ASLROLLSRRORSTXLDXDECINC" \ 6/a/e x8
,"BITJMPSTYLDYCPYCPX" \ 4/c x6
,"BRKBPLJSRBMIRTIBVCRTSBVSBCCLDYBCSCPYBNECPXBEQ" \ 0 x15
,"PHPCLCPLPSECPHACLIPLASEIDEYTYATAYCLVINYCLDINXSED" \ 8 x16
,"TXATXSTAXTSXDEXNOP" \ a x6
59 value OPCNT $ff value NUL 20 value DISCNT
: >>4 4 rshift ;
: opid. dup OPCNT < if
  3 * OPNAME + 3 rtype else drop ."???" then ;
: words, ( n -- ) create 0 do ' , loop ;
: spcs ( n -- ) 0 do spc> loop ;
: id159d ( opcode -- opid )
  dup $89 = if drop NUL else 5 rshift then ;
create _ map< c, $c $c $d $d $e $e $f $f \
                 $35 $36 $37 $38 $39 NUL $3a NUL \
                 $c NUL $d $d $e $e $f $f
: id6ae dup $80 < if ( ASL/ROL/LSR/ROR )
    dup $1f and $1a = if drop NUL exit then 5 rshift 8+ exit then
  dup 4/ 1- 3 and 8* _ + ( op tbl ) swap >>4 7 and + c@ ;
create _ map< c,
NUL NUL $10 NUL NUL NUL NUL NUL $12 $12 $13 $13 $14 $14 $15 NUL \
NUL NUL $10 NUL $11 NUL $11 NUL $12 NUL $13 $13 $14 NUL $15 NUL
: id4c _ over $8 and if $10 + then swap >>4 + c@ ;
: idnul drop NUL ;
: id0 >>4 dup 8 = if drop NUL exit then dup 8 > if 1- then 22 + ;
: id8 >>4 37 + ;
: id2 $a2 = if $0d else NUL then ;
16 words, _ id0 id159d id2 idnul id4c
  id159d id6ae idnul id8 id159d
  id6ae idnul id4c id159d id6ae idnul
: opid dup $f and 4* _ + @ execute ;
\ 0=inh 1=imm 2=acc 3=zp 4=zp,X 5=zp,Y 6=abs 7=abs,X 8=abs,Y
\ 9=ind 10=ind,X 11=ind,Y 12=rel
create _ map< c, 0  10 1 0 3 3 3 0 0 1 2 0 6 6 6 0 \
                 12 11 0 0 4 4 4 0 0 8 0 0 7 7 7 0 \
                 1  10 1 0 3 3 3 0 0 1 0 0 6 6 6 0 \
                 12 11 0 0 4 4 4 0 0 8 0 0 7 7 7 0
: modeid ( opcode -- id )
  dup $20 = if drop 6 exit then dup $6c = if drop 9 exit then
  dup $be = if drop 8 exit then
  dup $80 and 4/ swap $1f and or _ + c@ ;
: inh. ( a -- a ) 7 spcs ; : byte. c@+ .x1 ;
: $. '$' emit byte. ; : zp. $. 4 spcs ; alias zp. rel.
: imm. '#' emit byte. 4 spcs ;
: $$. '$' emit c@+ swap c@+ .x1 swap .x1 ; : abs. $$. 2 spcs ;
: ind. '(' emit $$. ')' emit ;
: acc. 'A' emit 6 spcs ;
: ,X. ',' emit 'X' emit ;
: ,Y. ',' emit 'Y' emit ;
: zp,X. $. ,X. 2 spcs ; : zp,Y. $. ,Y. 2 spcs ;
: abs,X. $$. ,X. ; : abs,Y. $$. ,Y. ;
: ind,X. '(' emit $. ,X. ')' emit ;
: ind,Y. '(' emit $. ')' emit ,Y. ;
13 words, _ inh. imm. acc. zp. zp,X. zp,Y. abs. abs,X. abs,Y.
  ind. ind,X. ind,Y. rel.
: mode. ( a opcode -- a ) modeid 4* _ + @ execute ;
: op. ( a -- a ) c@+ dup opid dup opid. spc>
  OPCNT < if mode. else drop then ;
: _dump ( a u -- ) 0 do c@+ .x1 spc> loop drop ;
: offset here pc - ;
: dis ( a -- ) DISCNT 0 do
  dup offset - .x2 spc> dup op. spc>
  tuck over - _dump nl> loop drop ;
