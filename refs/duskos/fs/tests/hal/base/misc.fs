needs tests/harness
testbegin
\test simple expression
code test1 ( a b -- a-b )
  PSP) @!,
  PSP) -,
  nip, exit,
54 12 test1 42 #eq

\test use RSP as local variables
code test2 ( -- n )
  dup, -8 rs+,
  42 i) @, RSP) !,
  5 i) @, RSP) 4 +) !,
  RSP) @,
  RSP) 4 +) +,
  8 rs+, exit,
test2 47 #eq

\test simple expression with RSP
code test3 ( -- n )
  dup, -4 rs+,
  2 i) @, RSP) !,
  3 i) @, RSP) *,
  1 i) +,
  4 rs+, exit,
test3 7 #eq

\test expression involving push/popping intermediate results
code test4 ( -- n ) \ 2 * 3 + 2
  dup,
  3 i) @,
  -1 i) +,
  dup,
  2 i) @,
  dup,
  3 i) @,
  PSP) *, nip,
  PSP) +, nip,
  exit,
test4 8 #eq
\test variable op width
here# ,"hello" ( a )
code test5 ( n -- c )
  dup,
  ( a ) i) @,
  PSP) +, nip,
  W) 8b) @,
  exit,
0 test5 'h' #eq
1 test5 'e' #eq
 
\test a rewrite of ptrset() from test.c for more precise testing
code test6 ( -- n )
  dup, -8 rs+,
  42 i) @, RSP) !,
  RSP) &) @, RSP) 4 +) !,
  54 i) @,
  RSP) 4 +) A>) @, A) !,
  RSP) @,
  8 rs+, exit,
test6 54 #eq
 
\test &). this returns item "idx" from PSP
code test7 ( ... idx -- n )
  2 i) <<, PSP) &) +, W) @,
  exit,

42 12 123 0 test7 123 #eq
1 test7 12 #eq
2 test7 42 #eq
2drop drop

\test @+, with A>)
create foo 42 , 54 ,
code test28 ( -- n2 n1 )
  dup, foo i) @,
  W) A>) @+,
  A) &) @!,
  dup, A) @,
  exit,

test28 54 #eq 42 #eq

\test 16-bit le@
create data $ff c, $cafe wbe, \ disaligned
code foo ( -- n ) dup, data 1+ m) 16b) le@, exit,
foo $feca #eq

\test 16-bit be@
code foo ( -- n ) dup, data 1+ m) 16b) be@, exit,
foo $cafe #eq

\test 16-bit le!
code foo ( n -- ) data 1+ m) 16b) le!, drop, exit,
$dead foo data 1+ wbe@ $adde #eq

\test 16-bit be!
code foo ( n -- ) data 1+ m) 16b) be!, drop, exit,
$dead foo data 1+ wbe@ $dead #eq

\test 32-bit le@
create data $ff c, $cafebabe be, \ disaligned
code foo ( -- n ) dup, data 1+ m) le@, exit,
foo $bebafeca #eq

\test 32-bit be@
code foo ( -- n ) dup, data 1+ m) be@, exit,
foo $cafebabe #eq

\test 32-bit le!
code foo ( n -- ) data 1+ m) le!, drop, exit,
$deadbeef foo data 1+ be@ $efbeadde #eq

\test 32-bit be!
code foo ( n -- ) data 1+ m) be!, drop, exit,
$deadbeef foo data 1+ be@ $deadbeef #eq

\test le@ with big +)
\ under ARM, this resulted in miscompilation
code foo ( -- n ) 1 litn W) data +) be@, exit,
foo $deadbeef #eq

\test le! with big +)
code foo ( n -- ) 1 i) A>) @, A) data +) be!, drop, exit,
$cafebabe foo data 1+ be@ $cafebabe #eq
testend
