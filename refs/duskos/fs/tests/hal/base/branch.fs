needs tests/harness
testbegin

\test Jumps
code test1 ( n -- n ) \ returns 42 if arg >= 10, 54 otherwise
  10 i) >=) if,
    42 i) @, exit,
  then
  54 i) @, exit,
5 test1 54 #eq
15 test1 42 #eq

\test function calls
code dummy ( a b -- a-b )
  PSP) @!,
  PSP) -,
  nip, exit,

code test2 ( n -- n-42 )
  dup,
  42 i) @,
  pushlr, ' dummy execute, poplr,
  exit,
54 test2 12 #eq
 
\test Branching with intermediate results. Check for PS leaks.
create myarray 1 , 2 , 3 , 0 ,
\ Equivalent: int i = 0; int *b = myarray; do ++i; while (*(b++)); return i;
code test3 ( -- n )
  dup, -4 rs+,
  0 i) @, RSP) !, \ i=0
  myarray i) A>) @,
  begin
    1 RSP) +n,
    A) @,
    4 A) &) +n,
    0 i) <>) ?br,
  RSP) @, 4 rs+, exit,
test3 4 #eq
scntneutral#

\test bool, and dir) work
code test4 ( n n -- f )
  PSP) dir) <) bool, nip, exit,

1 2 test4 #true
2 2 test4 not #true
2 1 test4 not #true
 
\test br with &) and +)
code test5 ( w-100 -- w-100 ? ) W) &) 100 +) br,
: success 42 ;
' success 100 - test5 42 #eq drop

testend
