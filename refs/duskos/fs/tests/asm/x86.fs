needs tests/harness arch/core asm/x86
testbegin
\ The first part of the tests tests particular mechanics within the assembler
\ and can be ran on any arch. The second part tests by running the resulting
\ binary and can only be ran on x86.

32bmode

\test
cx $1c1 #eq
: chk ( opmod disp expected ) rot tuck $fff and #eq bankid@ hbank@ #eq ;
$1234 abs) $1234 $105 chk
dx 0 d) 0 $102 chk
dx 42 d) 42 $142 chk
dx 1242 d) 1242 $182 chk
bp 0 d) 0 $145 chk

0 value c
0 value h
: _#eq 2dup = if 2drop else ."instr: " h here over - cspit[] #eq then ;
: t[ here to h scnt to c ;
: ]t ( ... ) scnt c - here h - over _#eq ( ... n )
  here swap 0 do 1- dup c@ rot _#eq loop drop ;

\test
t[ $f7 $1a dx 0 d) neg, ]t
t[ $f7 $d1 cx not, ]t
t[ $f6 $d5 ch not, ]t
t[ $0f $9e $05 $34 $12 $00 $00 $1234 abs) setle, ]t
t[ $03 $d9 bx cx add, ]t
t[ $03 $19 bx cx 0 d) add, ]t
t[ $03 $59 $2a bx cx 42 d) add, ]t
t[ $09 $19 cx 0 d) bx or, ]t
t[ $09 $1c $24 sp 0 d) bx or, ]t

\test
t[ $81 $eb $34 $12 $00 $00 bx $1234 imm) sub, ]t
t[ $80 $eb $2a bl 42 imm) sub, ]t
t[ $83 $eb $d6 bx -42 imm) sub, ]t
t[ $83 $7d $00 $2a bp 0 d) 42 imm) cmp, ]t

\test
t[ $2d $34 $12 $00 $00 ax $1234 imm) sub, ]t
t[ $2c $2a al 42 imm) sub, ]t

\test
t[ $85 $c0 ax ax test, ]t
t[ $a8 $2a al 42 imm) test, ]t
t[ $a9 $34 $12 $00 $00 ax $1234 imm) test, ]t
t[ $f6 $c3 $2a bl 42 imm) test, ]t
t[ $f7 $c3 $34 $12 $00 $00 bx $1234 imm) test, ]t
t[ $85 $19 bx cx 0 d) test, ]t
t[ $85 $19 cx 0 d) bx test, ]t

\test
t[ $91 ax cx xchg, ]t
t[ $91 cx ax xchg, ]t

\test
t[ $87 $d9 bx cx xchg, ]t
t[ $87 $cb cx bx xchg, ]t \ equivalent to previous
t[ $86 $d9 bl cl xchg, ]t
t[ $87 $0b bx 0 d) cx xchg, ]t
t[ $87 $0b cx bx 0 d) xchg, ]t
t[ $87 $07 di 0 d) ax xchg, ]t
t[ $87 $07 ax di 0 d) xchg, ]t

\test
t[ $d1 $e3 bx 1 imm) shl, ]t
t[ $d3 $e3 bx cl shl, ]t
t[ $c1 $eb $02 bx 2 imm) shr, ]t
t[ $c1 $65 $04 $02 bp 4 d) 2 imm) shl, ]t
t[ $d3 $6d $04 bp 4 d) cl shr, ]t

\test
t[ $58 ax pop, ]t
t[ $8f $05 $34 $12 $00 $00 $1234 abs) pop, ]t

\test
t[ $6a $2a 42 imm) push, ]t
t[ $68 $34 $12 $00 $00 $1234 imm) push, ]t

\test
t[ $e5 $2a ax 42 imm) in, ]t
t[ $ed ax dx in, ]t
t[ $e6 $2a al 42 imm) out, ]t
t[ $ef ax dx out, ]t

\test
t[ $8b $d9 bx cx mov, ]t
t[ $89 $0b bx 0 d) cx mov, ]t
t[ $8b $19 bx cx 0 d) mov, ]t
t[ $bb $34 $12 $00 $00 bx $1234 imm) mov, ]t
t[ $b3 $2a bl 42 imm) mov, ]t
t[ $8b $1d $34 $12 $00 $00 bx $1234 abs) mov, ]t

\test
t[ $8c $cb bx cs mov, ]t
t[ $8e $cb cs bx mov, ]t
t[ $0f $20 $c3 bx cr0 mov, ]t
t[ $0f $22 $d3 cr2 bx mov, ]t

\test
t[ $0f $be $05 $34 $12 $00 $00 ax $1234 abs) byte) movsx, ]t
t[ $0f $b7 $1f bx di 0 d) movzx, ]t
t[ $0f $b7 $d8 bx ax movzx, ]t

\test
t[ $ff $d0 ax callr, ]t
t[ $74 $fe 0 jz, ]t
t[ $0f $84 $fa $00 $00 $00 $100 jz, ]t

\test
t[ $66 $f7 $d1 cx word) not, ]t
t[ $0f $9e $05 $34 $12 $00 $00 $1234 abs) setle, ]t
t[ $66 $03 $d9 bx word) cx add, ]t
t[ $66 $03 $d9 bx cx word) add, ]t
t[ $66 $03 $19 bx word) cx 0 d) add, ]t
t[ $66 $03 $19 bx cx 0 d) word) add, ]t
t[ $66 $bb $34 $12 bx word) $1234 imm) mov, ]t
t[ $66 $c1 $e8 $02 ax word) 2 imm) shr, ]t
t[ $66 $ed ax word) dx in, ]t
t[ $f6 $00 $80 ax 0 d) byte) $80 imm) test, ]t

\test
t[ $8d $44 $24 $2a ax sp 42 d) lea, ]t

\test
realmode
t[ $8a $04 al 0 si+) mov, ]t
ax $10 imm) mov, \ having this instruction here used to bug the following
t[ $8e $d8 ds ax mov, ]t
t[ $8a $16 $34 $12 dl $1234 abs) mov, ]t
32bmode

\test
t[ $44 $8b $c3 rex.r r8 bx mov, ]t
t[ $41 $03 $d8 rex.b bx r8 add, ]t
t[ $49 $03 $d8 rex.w rex.b bx r8 add, ]t
t[ $49 $03 $40 $04 rex.w rex.b ax r8 4 d) add, ]t
t[ $49 $01 $40 $04 rex.w rex.b r8 4 d) ax add, ]t
\ don't encode a "AX shortcut" with REX set
t[ $41 $f7 $c0 $2a $00 $00 $00 rex.b r8 42 imm) test, ]t

\test
t[ $8b $44 $4b $04 ax bx cx 2r+) 4 d) mov, ]t
t[ $8b $0c $28 cx ax bp r+) mov, ]t
t[ $f6 $84 $28 $fb $ff $ff $ff $40 ax bp r+) -5 d) byte) $40 imm) test, ]t

\test
t[ $ad lods, ]t
t[ $66 $ad lodsw, ]t

\test
realmode
t[ $66 $ad lods, ]t
t[ $ad lodsw, ]t
t[ $66 $ff $44 $08 8 si+) dword) inc, ]t
t[ $ff $44 $08 8 si+) inc, ]t  \ 16-bit width because of realmode
t[ $ff $44 $0C 12 si+) word) inc, ]t
t[ $fe $44 $0C 12 si+) byte) inc, ]t
32bmode

livemode

isx86? not [if] testend \s [then]

\test from this point, we run assembled code
code foo1
  si 4 imm) sub,
  si 0 ?d+bp) ax mov,
  ax 42 imm) mov,
  ret,

foo1 42 #eq

\test
here $1234 , ( a )
code foo2
  si 4 imm) sub,
  si 0 ?d+bp) ax mov,
  ax over abs) mov,
  ( a ) abs) -1 imm) test,
  al setnz,
  ret,

foo2 $1201 #eq

\test call in its different forms
0 value mylabel
code foo3
  ' foo1 abs>rel call,
  bx ' foo1 imm) mov,
  bx dup ?bp+, callr,
  forward call, to mylabel
  ret,

\ we test foo3 later

\test shr/shl
code foo4
  mylabel forward!
  si 4 imm) sub,
  si 0 ?d+bp) ax mov,
  ax 42 imm) mov,
  ax 3 imm) shl,
  cl 2 imm) mov,
  ax cl shr,
  ret,

foo4 84 #eq
foo3 84 #eq 42 #eq 42 #eq

\test single operands
code foo5
  si 4 imm) sub,
  si 0 ?d+bp) ax mov,
  ax 42 imm) mov,
  bx 3 imm) mov,
  bx mul,
  ax ax test,
  bl setnz,
  al bl add,
  ret,

foo5 127 #eq

\test push/pop
code foo6
  si 4 imm) sub,
  si 0 ?d+bp) ax mov,
  42 imm) push,
  ax pop,
  ret,

foo6 42 #eq

\test MOV immediate to r/m
code foo7
  si 4 imm) sub,
  si 0 ?d+bp) 42 imm) mov,
  ax si 0 ?d+bp) xchg,
  ret,

foo7 42 #eq

\test ESP+disp (only works in i386)
FAMILY_i386 instrfamily? [if]
code foo8
  si 4 imm) sub,
  si 0 d) ax mov,
  42 imm) push,
  ax sp 0 d) mov,
  sp 4 imm) add,
  ret,

foo8 42 #eq
[then]

\test forward jumps
code foo9 ( n -- n )
  ax 42 imm) cmp,
  forward8 jnz,
    ax inc,
  forward!
  ax 54 imm) cmp,
  forward jnz,
    ax inc,
  forward!
  ret,

12 foo9 12 #eq
42 foo9 43 #eq
54 foo9 55 #eq

\test the assembler used to mis-assemble ops with imm > $80 but < $100.
code foo10 ( n -- n )
  ax $80 imm) add,
  ret,

1 foo10 $81 #eq
testend
