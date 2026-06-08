needs tests/harness mem/arena
testbegin

\test
newarena dup bindalloc[ myar[ bindrun1 myar1
myar[ 4 allot@ 4 allot@ ]alloc
4 - #eq

myar[
  CURALLOC @ dup root @ swap curbuf @ #eq
  CURALLOC @ curbuf @ @ 0 #eq
]alloc

\test Try to overstep bounds
myar[
  ARENASZ allot@ ( a ) \ creates a new buf
  CURALLOC @ root @ @ buf[] drop #eq ( )
]alloc
testend
