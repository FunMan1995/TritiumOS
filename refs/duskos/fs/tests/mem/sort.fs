needs tests/harness mem/sort
testbegin

\test sort
create myarray 3 , 7 , 8 , 5 , 2 , 1 , 9 , 5 , 4 ,
myarray 9 sort
create expected 1 , 2 , 3 , 4 , 5 , 5 , 7 , 8 , 9 ,
myarray expected 9 []= #

\test sort@
: pair ( n1 n2 -- a ) here# rot , swap , ;
1 8 pair const foo
2 7 pair const bar
3 6 pair const baz
create myarray bar , baz , foo ,
0 myarray 3 sort@
create expected foo , bar , baz ,
myarray expected 3 []= #
4 myarray 3 sort@
create expected baz , bar , foo ,
myarray expected 3 []= #

testend
