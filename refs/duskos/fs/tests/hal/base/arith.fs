needs tests/harness
testbegin
\test +, basics
code test1.1
  13 i) S>) @,
  S) &) +,
  exit,
code test1.2
  38 i) +, exit,
2 test1.1 15 #eq
-14 test1.2 24 #eq

\test dir) +,
create tmp $1234 ,
code test2 ( n -- )
  tmp m) dir) +, drop,
  exit,

42 test2 tmp @ $1234 42 + #eq

\test 32b) and m) +,
code test3.1
  PSP) +, exit,
30 120 test3.1 150 #eq 30 #eq

here# 22 ,
code test3.2
  m) +, exit,
11 test3.2 33 #eq

\test 16b) +,
here# dup 47 w, 39 w,
code test4.1
  m) 16b) +, exit,
code test4.2
  m) 2 +) 16b) +, exit,

22 test4.1 69 #eq
25 test4.2 64 #eq

create foo 1020 w,
code test4.3
  392 i) S>) @,
  foo m) 16b) S>) dir) +,
  exit,
test4.3 foo w@ 1412 #eq

create foo $ff00 w,
code test4.4
  $121 i) S>) @,
  foo m) 16b) S>) dir) +,
  exit,
test4.4 foo w@ $21 #eq

\test 8b) +,
create foo 9 c, 12 c,
code test5.1
  foo m) 8b) +, exit,
42 test5.1 51 #eq

code test5.2
  foo m) 8b) 1 +) +, exit,
33 test5.2 45 #eq

code test5.3
  7 i) S>) @,
  foo m) 8b) S>) dir) +,
  exit,
test5.3 foo c@ 16 #eq

code test5.4
  250 i) S>) @,
  foo m) 1 +) 8b) dir) S>) +,
  exit,
test5.4 foo 1+ c@ 6 #eq

\test +, doesn't affect target memory location
here# 42 , here swap , ( pc of *int )
code test6 ( -- n )
  dup,
  ( pc ) i) @,
  W) A>) @,
  A) @,
  1 i) +, \ result in W, not in memory location
  A) +, \ 42+43, not 43+43
  exit,
test6 85 #eq

\test force 32-bit arithmetic on 8b)
code test7 ( a n -- n ) PSP) A>) @+, A) 8b) +, exit,
create data $ff c,
data $101 test7 $200 #eq

\test force 32-bit arithmetic on 8b) and *,
\ i386 used to misassemble this
code test8 ( a n -- n ) PSP) A>) @+, A) 8b) *, exit,
create data 42 c,
data $100 test8 $2a00 #eq

\test 0 i) >>,
code test9 ( n -- n ) 0 i) >>, exit,
42 test9 42 #eq

\test RSP) 4 +) &) +, doesn't leak to PS
code test10 RSP) 4 +) &) +,
scntneutral# \ used to leak on m68k

\test swap-,
code test11 ( a b -- a a-b )
  PSP) swap-, exit,

5 2 test11 3 #eq 5 #eq

\test swap- with dir)
code test12 ( a b -- b-a b )
  PSP) dir) swap-, exit,

2 5 test12 5 #eq 3 #eq

\test swap- with m)
create foo 42 ,
code test13 ( a -- foo-a )
  foo m) swap-, exit,

5 test13 37 #eq foo @ 42 #eq

\test swap- with &)
code test14 ( a b -- a-b )
  PSP) S>) @+, S) &) swap-, exit,

5 2 test14 3 #eq

\test *, with 8b) S>) and dir)
code test15 ( n a -- ) PSP) S>) @, W) 8b) S>) dir) *, 2drop, exit,
create data $1234 le,
8 data test15 data le@ $12a0 #eq

\test /mod, with 8b) and dir) doesn't do a 32-bit op on target address
code test16 ( a n -- n ) PSP) A>) @+, A) 8b) dir) /mod, drop, exit,
create data $2a2a2a2a ,
data 10 test16
data c@ 4 #eq

\test >>, works with signed)
code test17 ( n by -- n )
  PSP) dir) signed) >>, drop, exit,

17 2 test17 4 #eq
-17 2 test17 -5 #eq

\test *, works properly with an alternate dst combined with dir)
create _ 123 ,
code test18 ( n -- )
  S) &) !, 42 i) @, _ m) S>) dir) *, drop, exit,
3 test18 _ @ 369 #eq

\test *, preserves S register
code test19 ( n -- n )
  S) &) !, 2 i) *, S) &) @, exit,

42 test19 42 #eq

\test *, and /mod, with a dest other than W preserves W.
code test20 ( n -- n 6 )
  4 i) A>) @, 3 i) A>) *, 2 i) A>) /mod,
  dup, A) &) @, exit,

42 test20 6 #eq 42 #eq

\test /mod, and dir)
code test21 ( a b -- a/b a%b )
  PSP) dir) /mod, S) &) @, exit,

9 2 test21 1 #eq 4 #eq

create data $12345678 le,
\test 8b *,
code test22 ( n -- n )
  data m) 8b) *, exit,

3 test22 $78 3 * #eq

\test /mod, with S)
code test23 ( n n -- r q )
  PSP) S>) @, S) &) /mod, PSP) S>) !, exit,

2 5 test23 2 #eq 1 #eq


\test |, and 8b) which caused problems on arm
code test24 ( a n -- n|[a] )
  PSP) A>) @+, A) 8b) |, exit,

$34 tmp c! tmp 1+ 3 $12 cfill
tmp $80 test24 $b4 #eq

\test 8b) |, preserves the upper 24-bit of the dest
code test25 ( n a -- n )
  A) &) !, drop, 8 i) <<, A) 8b) |, exit,

create data $2a c,
$1234 data test25 $12342a #eq

\test +n, with 16b) and 8b)
code test8badd ( -- ) \ W is not supposed to be affected
  $10 tmp m) 8b) +n, exit,

0 tmp ! 42 test8badd 42 #eq tmp c@ $10 #eq

code test16badd ( -- )
  $200 tmp m) 16b) +n, exit,

0 tmp ! 42 test16badd 42 #eq tmp w@ $200 #eq

\test operand orders on -, with dir)
code test27 ( a b -- a-b )
  PSP) dir) -, drop, exit,

5 3 test27 2 #eq

\test operand orders on /mod, with dir)
code test28 ( a b -- q r oldW )
  dup, PSP) 4 +) dir) /mod, PSP) S>) !, exit,

5 3 test28 3 #eq 2 #eq 1 #eq

\test <<, dir) and +)
code test29 ( a b -- a<<b )
  dup, PSP) 4 +) dir) <<, 2drop, exit,

$2a 4 test29 $2a0 #eq

\test <<, dir) and 16b)
create foo 42 w, 54 w,
code test30 ( n -- )
  foo m) dir) 16b) <<, drop, exit,

4 test30 foo w@ $2a0 #eq foo 2 + w@ 54 #eq

\test /mod with i)
code test31 ( n -- r q )
  3 i) /mod,
  dup,
  PSP) S>) !,
  exit,

10 test31 3 #eq 1 #eq

\test &). this returns item "idx" from PSP
code test32 ( ... idx -- n )
  2 i) <<, PSP) &) +, W) @,
  exit,

42 12 123 0 test32 123 #eq
1 test32 12 #eq
2 test32 42 #eq
2drop drop

\test 16-bit/8-bit arithmetics are properly upscaled to 32-bit in W/A registers
code test33 ( a n -- n )
  PSP) A>) @+, A) 16b) +, exit,

1 tmp w! tmp $1ffff test33 $20000 #eq

\test dir) with *, which was problematic on i386
code test34 ( a b -- n )
  PSP) dir) *, drop, exit,

4 5 test34 20 #eq

\test *, with A>)
code test35 ( a b -- n )
  W) &) A>) @,
  0 i) @,
  PSP) A>) *,
  A) &) @,
  nip, exit,

4 5 test35 20 #eq

\test i386 didn't allow << and >> with non-const right operand
code test36 ( n n -- n )
  PSP) @!,
  PSP) <<,
  nip, exit,
$42 4 test36 $420 #eq

\test /mod, and signed)
code s/mod PSP) @!, PSP) signed) /mod, PSP) S>) !, exit,

13 4 s/mod 3 #eq 1 #eq
-13 4 s/mod -3 #eq -1 #eq
-13 -4 s/mod 3 #eq -1 #eq
13 -4 s/mod -3 #eq 1 #eq

\test *, and 16b)
code test38 ( a n -- [a]*n ) \ we ignore b31:16 in a
  PSP) A>) @+, A) 16b) *, exit,
2 tmp w! $1234 tmp 2+ w!
tmp 21 test38 42 #eq

\test 16b) signed) +,
create data -2 w,
code test39
  data i) A>) @,
  A) 16b) signed) +, exit,
3 test39 1 #eq

\test 8b) signed) &,
create flag40 -2 c,
code test40
  flag40 m) 8b) signed) &, exit,

30 test40 30 #eq
31 test40 30 #eq
$5740 test40 $5740 #eq
$5741 test40 $5740 #eq

\test 8b) signed) *,
create mul41 -1 c,
code test41
  mul41 i) S>) @,
  S) 8b) signed) W>) *, exit,

5 test41 -5 #eq

\test 16b) signed) swap-,
\ this used to fail on x86
create data -10 w,
code test42
  data m) 16b) signed) swap-, exit,

9 test42 -19 #eq

\test lrot
$87654321 12 lrot $54321876 #eq
$10101010 5 lrot $02020202 #eq
$2c7 9 lrot $2c7 9 lshift #eq

\test i) lrot,
code test43 ( u -- u )
  13 i) lrot, exit,
12345678 test43 12345678 13 lrot #eq

\test m) 8b) dir) lrot,
create data $65 c,
create expected map< c, $2b $59 $ca $56 $b2 $95 $ac $65
code test44 ( -- n )
  3 i) S>) @,
  data m) 8b) dir) S>) lrot,
  dup, data m) 8b) @, exit,

:> 8 0 do test44 expected i + c@ #eq loop ; execute
 
\test &) lrot,
code test45
  17 i) S>) @,
  S) &) lrot,
  exit,

342301 test45 342301 17 lrot #eq
10 test45 10 17 lrot #eq

\test i) rrot,
code test46
  17 i) lrot,
  17 i) rrot,
  exit,

304921 dup test46 #eq
10203013 dup test46 #eq
$812eaf3b dup test46 #eq

\test &) rrot,
code test47
  21 i) S>) @,
  S) &) rrot,
  S) &) lrot,
  exit,

304921 dup test47 #eq
10203013 dup test47 #eq
$812eaf3b dup test47 #eq

\test m) 16b) dir) rrot,
create data $305f w,
code test48 ( n -- )
  data m) 16b) dir) rrot,
  data m) 16b) dir) lrot,
  drop, exit,

:> 16 0 do i test48 data w@ $305f #eq loop ; execute
testend
