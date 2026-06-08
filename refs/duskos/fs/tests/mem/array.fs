needs tests/harness mem/array
testbegin

\test initialize
4 3 newarray const arr
arr cnt 0 #eq

\test append
1 arr append 42 swap !
arr cnt 1 #eq
0 arr get 42 #eq

\test trigger realloc
3 arr append
54 swap !+ 102 swap !+ 12 swap !
arr cnt 4 #eq
create expected 42 , 54 , 102 , 12 ,
arr ptr expected 4 []= #true

\test insert
1 2 arr insert 123 swap !
arr cnt 5 #eq
create expected 42 , 54 , 123 , 102 , 12 ,
arr ptr expected 5 []= #true

\test delete
2 1 arr delete
arr cnt 3 #eq
create expected 42 , 102 , 12 ,
arr ptr expected 3 []= #true
3 1 arr delete \ we auto-adjust 3 to 2
arr cnt 1 #eq
0 arr get 42 #eq

\test insert that triggers realloc
4 3 newarray const arr
1 arr append 42 swap !
3 0 arr insert 44 swap !+ 46 swap !+ 48 swap !
create expected 44 , 46 , 48 , 42 ,
arr ptr expected 4 []= #true

testend
