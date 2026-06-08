needs tests/harness
testbegin
: boolz, ifz, 1 i) @, exit, [compile] then 0 i) @, exit, ;

\test testz, + ifz,
code test1
  0 i) @, W) &) testz, ifz, 5 i) @, then exit,
329 test1 5 #eq

\test ifnz,
code test2
  9303 i) @, W) &) testz, ifnz, 192 i) @, then exit,
133 test2 192 #eq

\test ?brz,
create data 0 c, 0 c, 0 c, 0 c, 1 c,
code test3
  data i) A>) @,
  begin
    A) 8b) S>) @+,
    S) &) testz, ?brz,
  1 i) A>) -,
  A) &) W>) @,
  exit,
0 test3 data 4+ #eq

\test ?brnz,
code test4
  3 i) S>) @,
  W) &) A>) @,
  begin
  A) &) *,
  1 i) S>) -,
  S) &) testz, ?brnz, exit,
3 test4 81 #eq

\test m) and m) +) testz,
create data 4 , 42 ,
code test5.1
  data m) testz, boolz,
12 test5.1 0 #eq
0 data !
13 test5.1 1 #eq

code test5.2
  data m) 4 +) testz, boolz,
19 test5.2 0 #eq
0 data 4+ !
20 test5.2 1 #eq

\test 8b) m) testz,
create data $ff c, 0 c, $ff c,
code test6
  data 1+ m) 8b) testz, boolz,
12 test6 1 #eq
0 data c! $1 data 1+ c! 0 data 2+ c!
13 test6 0 #eq

\test A) +) 8b) testz,
code test7
  data i) A>) @,
  A) 1 +) 8b) testz, boolz,
19 test7 0 #eq
$ff data c! $00 data 1+ c! $ff data 2+ c!
20 test7 1 #eq

\test 16b) m) testz,
create data $beef w, $0000 w, $cafe w,
code test8
  data 2+ m) 16b) testz, boolz,
12 test8 1 #eq
0 data w! $dead data 2+ w! 0 data 4+ w!
13 test8 0 #eq

\test A) +) 16b) testz,
code test9
  data i) A>) @,
  A) 2 +) 16b) testz, boolz,
19 test9 0 #eq
$beef data w! $0000 data 2+ w! $cafe data 4+ w!
20 test9 1 #eq

\test PSP) testz,
code test10
  PSP) testz, boolz,
32 9 test10 0 #eq 32 #eq
0 10 test10 1 #eq 0 #eq

\test testz, ignores destination
create data $afa , $000 , $fab ,
code test11.1
  data 4+ m) W>) testz, boolz,
code test11.2
  data 4+ m) A>) testz, boolz,
code test11.3
  data 4+ m) S>) testz, boolz,
111 test11.1 1 #eq
112 test11.2 1 #eq
113 test11.3 1 #eq
$ba0 data 4+ !
111 test11.1 0 #eq
112 test11.2 0 #eq
113 test11.3 0 #eq

\test Z flag on i) +,
code test12
  4 i) +, ifz, 5 i) +, then exit,
2 test12 6 #eq
-4 test12 5 #eq

\test Z flag on &) +,
code test13
  PSP) A>) @,
  A) &) +, ifz, 84 i) +, then exit,

38 11 test13 49 #eq 38 #eq
23 -23 test13 84 #eq 23 #eq

\test Z flag on 32b) +,
code test14
  PSP) +, ifz, 84 i) +, then exit,

38 11 test14 49 #eq 38 #eq
23 -23 test14 84 #eq 23 #eq

\test Z flag on i) -,
code test15
  PSP) S>) @,
  5 i) S>) -, boolz,
7 8 test15 0 #eq 7 #eq
5 9 test15 1 #eq 5 #eq

\test Z flag on &) -,
code test16
  PSP) 4 +) A>) @,
  PSP) S>) @,
  A) &) S>) -, boolz,
14 2 8 test16 0 #eq 2 #eq 14 #eq
2 14 9 test16 0 #eq 14 #eq 2 #eq
14 14 10 test16 1 #eq 14 #eq 14 #eq

\test Z flag on 32b) -,
code test17
  PSP) 4 +) S>) @,
  PSP) S>) -, boolz,
14 2 8 test17 0 #eq 2 #eq 14 #eq
2 14 9 test17 0 #eq 14 #eq 2 #eq
14 14 10 test17 1 #eq 14 #eq 14 #eq

\test Z flag on i) swap-,
code test18
  PSP) S>) @,
  5 i) S>) swap-, boolz,
7 8 test15 0 #eq 7 #eq
5 9 test15 1 #eq 5 #eq

\test Z flag on &) swap-,
code test19
  PSP) 4 +) A>) @,
  PSP) S>) @,
  A) &) S>) swap-, boolz,
14 2 8 test16 0 #eq 2 #eq 14 #eq
2 14 9 test16 0 #eq 14 #eq 2 #eq
14 14 10 test16 1 #eq 14 #eq 14 #eq

\test Z flag on 32b) swap-,
code test20
  PSP) 4 +) S>) @,
  PSP) S>) swap-, boolz,
14 2 8 test17 0 #eq 2 #eq 14 #eq
2 14 9 test17 0 #eq 14 #eq 2 #eq
14 14 10 test17 1 #eq 14 #eq 14 #eq

\test Z flag on i) |,
code test21.1
  8 i) W>) |, boolz,
0 test21.1 0 #eq
18 test21.1 0 #eq

code test21.2
  0 i) W>) |, boolz,
0 test21.2 1 #eq
1 test21.2 0 #eq

\test Z flag on &) |,
code test22.1
  8 i) A>) @,
  A) &) |, boolz,
0 test22.1 0 #eq
18 test22.1 0 #eq

code test22.2
  0 i) S>) @,
  S) &) |, boolz,
0 test22.2 1 #eq
1 test22.2 0 #eq

\test Z flag on 32 b) |,
code test23
  PSP) |, boolz,
32 0 test23 0 #eq 32 #eq
0 32 test23 0 #eq 0 #eq
1 2  test23 0 #eq 1 #eq
0 0  test23 1 #eq 0 #eq
\ we assume ^, and &, work too


\test Z flag on i) >>,
code test24 1 i) >>, boolz,
2 test24 0 #eq
1 test24 1 #eq

\test Z flag on i) <<,
code test25 1 i) <<, boolz,
2 test25 0 #eq
0 test25 1 #eq

\test Z flag on &) +n,
code test26
  4 W) &) +n, boolz,
0 test26 0 #eq
-4 test26 1 #eq

\test Z flag on 32b) +n,
code test27
  8 PSP) +n, boolz,
-4 27 test27 0 #eq 4 #eq
-8 19 test27 1 #eq 0 #eq

\test dropz,
code test29
  dropz, boolz,
291 6 test29 0 #eq
292 0 test29 1 #eq

\test Arithmetic words set flags
code test30 ( a b -- Z-for-a&b )
  PSP) S>) @+, S) &) &, boolz,

2 1 test30 1 #eq
2 3 test30 0 #eq
testend
