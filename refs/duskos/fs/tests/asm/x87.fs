needs tests/harness arch/core asm/x87
testbegin

0 value c
0 value h
: _#eq 2dup = if 2drop else ."instr: " h here over - cspit[] #eq then ;
: t[ here to h scnt to c ;
: ]t ( ... ) scnt c - here h - over _#eq ( ... n )
  here swap 0 do 1- dup c@ rot _#eq loop drop ;

32bmode

\test two-ways instrs
t[ ST0 ST1 fadd, $d8 $c1 ]t
t[ ST1 ST0 fadd, $dc $c1 ]t
t[ ST0 ST1 fsub, $d8 $e1 ]t
t[ ST1 ST0 fsubp, $de $e9 ]t
t[ ST0 ST1 fmul, $d8 $c9 ]t

\test mem-only instrs
t[ ax 0 d) fild, $db $00 ]t
t[ ax 0 d) fist, $db $10 ]t
t[ $1234 abs) fistp, $db $1d $34 $12 $00 $00 ]t

livemode

isx86? not [if] testend \s [then]
\test from this point, we run assembled code

variable mem

\test FPU div (rounded)
: mydiv ( n divideby -- n )
  [ finit, ] \ our FPU might not be initialized
  swap mem ! [ mem abs) fild, ]
  mem ! [ mem abs) fild, ]
  42 mem !
  [ ST1 ST0 fdivp, ]
  [ mem abs) fistp, ] mem @ ;

11 3 mydiv 4 #eq
10 3 mydiv 3 #eq

testend
