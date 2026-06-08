needs tests/harness
testbegin
\test operands
W) 8b) &) W) &) #eq
W) 16b) &) W) &) #eq
W) &) 32b) W) #eq
W) &) 16b) W) 16b) #eq
W) &) 8b) W) 8b) #eq
42 i) dup &) #eq
W) 4 +) dup 4 +) <> #true

\test +) additions
W) 8 +) 4 +) (slot hbank@ 12 #eq
W) -12 +) 6 +) (slot hbank@ -6 #eq

\test PSP load and store
code test3.1
  PSP) @, exit,
12 49 test3.1 12 #eq 12 #eq

code test3.2
  PSP) !, exit,
93 22 test3.2 22 #eq 22 #eq

\test +n, and -n, basics
code test4.1
  1 W) &) +n, exit,
code test4.2
  13 W) &) -n, exit,
code test4.3
  30 PSP) +n, exit,

1 test4.1 2 #eq
26 test4.2 13 #eq
8 50 test4.3 50 #eq 38 #eq

\test register mangling
code test5.1
  W) &) A>) @,
  A) &) S>) @,
  S) &) W>) @, exit,
33 test5.1 33 #eq

code test5.2
  PSP) &) A>) @,
  4 i) A>) -,
  A) W>) !,
  4 PSP) &) -n,
  exit,
49 test5.2 49 #eq 49 #eq

\test variable reference and dereference
code test6 ( -- n )
  dup, -8 rs+,
  42 i) @, RSP) !,
  RSP) &) @,
  RSP) 4 +) !, \ reference to RS+0 in RS+4
  \ Now, let's dereference
  RSP) 4 +) @, W) @,
  8 rs+, exit,
test6 42 #eq

\test assign and dereference
code test7
  dup, -8 rs+,
  RSP) &) @,
  RSP) 4 +) !, \ reference to RS+0 in RS+4
  \ Now, let's assign-dereference
  54 i) @,
  RSP) 4 +) A>) @, A) !,
  RSP) @,
  8 rs+, exit,
test7 54 #eq

\test absolute memory location
here# 1234 , ( a )
code test8.1 ( -- n )
  dup, m) @, exit,
test8.1 1234 #eq

here# 43 c,
code test8.2
  dup, m) 8b) @, exit,
test8.2 43 #eq

here# 7830 w,
code test8.3
  dup, m) 16b) @, exit,
test8.3 7830 #eq

\test Increase/decrease directly in memory
code test9 ( -- n )
  dup, -4 rs+,
  42 i) @, RSP) !,
  1 RSP) +n,
  RSP) @,
  4 rs+, exit,
test9 43 #eq

\test +) can receive negative offset
code test10 ( a -- [a-4] )
  W) -4 +) @, exit,

create foo 12 , 42 , 54 ,
foo 4 + test10 12 #eq
foo 8 + test10 42 #eq

\test +) can be used with big numbers
code test11 ( off -- n )
  W) foo +) @, exit,

0 test11 12 #eq
4 test11 42 #eq
8 test11 54 #eq

\test This also works with 16b)
create foo 12 w, 0 w, 42 w, 0 w, 54 w, 0 w,
code test12 ( off -- n )
  W) foo +) 16b) @, exit,

0 test12 12 #eq
4 test12 42 #eq
8 test12 54 #eq

\test And 8b) as well
create foo 27 c, 39 c, 11 c,
code test13
  W) foo +) 8b) @, exit,

0 test13 27 #eq
1 test13 39 #eq
2 test13 11 #eq

\test +) can be more than a byte
create foo $100 allot0 42 c,
code test14 ( a -- [a+$100] )
  W) $100 +) 8b) @, exit,

foo test14 42 #eq

\test &) with offset
code test15 ( -- a )
  dup, PSP) 4 +) &) @,
  exit,

3 2 1 test15 @ 2 #eq 2drop drop

\test &) and m)
code test16 ( -- 42 )
  dup, 42 m) &) @,
  exit,

test16 42 #eq

\test number bank system works
code test17 ( -- 42 )
  dup, 20 i) 22 i) @, +,
  exit,

test17 42 #eq

\test +) can compound
code test18 ( a b c d -- a+d )
  PSP) 5 +) 3 +) +, nip, nip, nip, exit,

20 54 12 22 test18 42 #eq

\test W&), A&) and A>)
code test19 ( a b -- n ) \ a + b*b
  PSP) A>) @, nip,
  W) &) *,
  A) &) +,
  exit,

4 5 test19 29 #eq

\test 8b) with A>)
code test22 ( n a -- n )
  PSP) A>) @,
  nip,
  W) 8b) A>) !,
  drop,
  exit,

create foo $12345678 le,
$23456789 foo test22
foo le@ $12345689 #eq

\test small +) &)
code test23 ( a b -- c )
  PSP) A>) @,
  nip,
  A) &) 13 +) ^,
  exit,

60 12 test23 69 #eq
12 60 test23 37 #eq

\test big +) &)
code test24 ( a b -- c )
  PSP) A>) @,
  nip,
  A) &) $fe203 +) ^,
  exit,

5032 92 test24 $ff5f7 #eq
92 5032 test24 $ff1f7 #eq

\test 8b) signed) @,
create data $fd c, $7f c, $12 c,
code test25 ( a -- n )
  W) 8b) signed) @, exit,
data test25 -3 #eq
data 1+ test25 $7f #eq
data 2+ test25 $12 #eq

\test 8b) signed) !,
\ Previously, ARM HAL would be confused by the signed) flag

code test26 ( n a -- )
  PSP) S>) @, W) 8b) signed) S>) !, 2drop, exit,

42 data test26
data c@ 42 #eq
$7f data 1+ c@ #eq
$12 data 2+ c@ #eq

\test 16b) signed) @,
create data $fff0 w, $7fff w, $8fff w, $1234 w,
code test27
  W) 16b) signed) @, exit,
data test27 -16 #eq
data 2+ test27 $7fff #eq
data 4+ test27 $ffff8fff #eq
data 6 + test27 $1234 #eq

\test direct !n,
code test28 ( -- n )
  dup, 13 W) &) !n, exit,
test28 13 #eq

\test indirect !n,
create data 0 ,
code test29
  data i) A>) @,
  17 A) !n, exit,
test29 data @ 17 #eq

\test memory !n,
-1 data !
code test30.1
  7 data m) 8b) !n, exit,
code test30.2
  17 data m) 16b) !n, exit,
code test30.3
  27 data m) 32b) !n, exit,

test30.1 data c@ 07 #eq
test30.2 data w@ 17 #eq
test30.3 data @  27 #eq

\test offset +) !n,
create data $ffff w, $ffff w, $ffff w,
code test31
  data i) A>) @,
  $3afa A) 2 +) 16b) !n,
  exit,

test31
data 0 + w@ $ffff #eq
data 2 + w@ $3afa #eq
data 4 + w@ $ffff #eq

testend
