needs asm/x86
unit asm/x87

\ we use the "special registers" b5:3 field of opmod for recognition
consts $20 ST0 $21 ST1 $22 ST2 $23 ST3 $24 ST4 $25 ST5 $26 ST6 $27 ST7
: st? $20 and bool ;
: st0? ST0 = ;

: ?argerr ?abort"invalid arguments" ;
: reverse ( dst src opcode -- src opcode )
  rot st0? not ?argerr 16 rshift dup not ?argerr ;

: twoways ( dst src opcode -- )
  over st0? if nip else reverse then ( arg opcode )
  swap regid@ or wbe, ;

: _ does> twoways ; map< _
  $d8c0dcc0 fadd,  $dec0 faddp, \
  $d8e0dce8 fsub,  $dee8 fsubp, \
  $d8c8dcc8 fmul,  $dec8 fmulp, \
  $d8f0dcf8 fdiv,  $def8 fdivp,

: memonly ( opmod regid opc -- ) c, 8* or modrm, ;
: _ does> $db memonly ; map< _ 0 fild, 1 fisttp, 2 fist, 3 fistp,
: _ does> $d9 memonly ; map< _ 0 fld, 2 fst, 3 fstp, 5 fldcw, 7 fnstcw,
: fstcw, $9b c, fnstcw, ;

: _ does> wbe, ; map< _ $d9f6 fdecstp, $d9f7 fincstp, $dbe3 fninit,
: finit, $9b c, fninit, ;

: ffree, ( st -- ) dup st? not ?argerr regid@ $ddc0 or wbe, ;
