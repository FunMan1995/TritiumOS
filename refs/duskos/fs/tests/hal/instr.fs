needs tests/harness hal/instr
testbegin

: boolnz, ifz, 0 i) @, [compile] else 1 i) @, [compile] then ;
\test &nf, doesn't change source
code test1 ( n -- n f )
  A) &) !, 3 A) &) &nf,
  boolnz, PSP) A>) -!, exit,
$10 test1 not #true $10 #eq
$12 test1 #true $12 #eq

\test &nf, works on indirect source
code test2 ( n -- n f )
  dup, 3 PSP) &nf, boolnz, exit,
$10 test2 not #true $10 #eq
$12 test2 #true $12 #eq

\test &nf, with big immediates
code test3 ( n -- n f )
  dup, $afa42 PSP) &nf, boolnz, exit,

$ba0bab test3 #true $ba0bab #eq
$f10131 test3 not #true $f10131 #eq

\test W) &) carry?, on empty carry
code test4
  dup, 0 i) +c$, W) &) carry?, exit,
test4 0 #eq

\test W) &) carry?, on empty borrow
code test5
  dup, 0 i) -c$, W) &) carry?, exit,
test5 0 #eq

\test +c, carry value
code test6 ( n -- n )
  -1 i) S>) @,
  W) &) S>) +c$,
  W) &) carry?,
  exit,

0 test6 0 #eq
1 test6 1 #eq
7 test6 1 #eq

\test basic d+
code test7
  PSP) S>) @,
  PSP) 8 +) dir) S>) +c$,
  PSP) 4 +) dir) +c,
  PSP) 4 +) @,
  8 ps+,
  exit,

-1 0 1 0 test7 1 #eq 0 #eq

\test d+ with carry saving
code test8 \ bad d+ copy
  PSP) S>) @,
  PSP) 8 +) dir) S>) +c$,
  A) &) carry?,
  PSP) 4 +) dir) A>) +c$,
  PSP) 4 +) dir) +c,
  PSP) 4 +) @,
  8 ps+,
  exit,

1 2 8 9 test7 1 2 8 9 test8 rot #eq #eq
-1 0 1 0 test8 1 #eq 0 #eq

\test d- with carry saving
code test9
  PSP) S>) @,
  PSP) 8 +) dir) S>) -c$,
  A) &) carry?,
  PSP) 4 +) dir) A>) -c$,
  PSP) 4 +) dir) -c,
  PSP) 4 +) dir) @,
  8 ps+,
  exit,

0 1 -1 0 test9 0 #eq 1 #eq

\test m) +) 16b) carry?,
create data $ff w, $ff w,
code test10
  0 i) +c$, data m) 2 +) 16b) carry?, exit,
test10
data w@ $ff #eq
data 2+ w@ 0 #eq

\test A) +) 8b) carry?,
create data map< c, 4 5 6 7 8
create expected map< c, 0 5 6 1 8
code test11
  data i) A>) @,
  -4 i) S>) @,
  9 i) S>) +c$,
  A) 3 +) 8b) carry?,
  594 i) S>) +c$,
  A) 8b) carry?,
  exit,
test11
data expected 5 c[]= #true

\test PSP) carry?,
code test12 ( x x x x -- x x x x )
  -443891 i) S>) @,
  78532 i) S>) +c$,
  PSP) 8 +) carry?,
  388294 i) S>) +c$,
  PSP) 4 +) carry?,
  $550000 i) S>) +c$,
  PSP) carry?,
  exit,

17 16 15 14 test12 14 #eq 0 #eq 1 #eq 0 #eq

\test d*,
: 2#eq >r #eq r> #eq ;
code test13 ( a b -- hi lo )
  PSP) d*,
  PSP) S>) !,
  exit,

0 0 test13 0 0 2#eq
0 1 test13 0 0 2#eq
1 2 test13 2 0 2#eq
3 3 test13 9 0 2#eq
-1 2 test13 -2 1 2#eq
-5 5 test13 -25 4 2#eq
-5 -5 test13 25 -10 2#eq

\test signed) d*,
code test14 ( a b -- hi lo )
  PSP) signed) d*,
  PSP) S>) !,
  exit,

-1 2 test14 -2 -1 2#eq
-5 5 test14 -25 -1 2#eq
-5 -5 test14 25 0 2#eq

testend
