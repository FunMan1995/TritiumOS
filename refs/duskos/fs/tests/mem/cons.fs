needs tests/harness mem/cons
testbegin

gc
12 13 cons dup const leakedcons leak
42 43 cons \ hold onto it in PS
44 45 cons consref myref
conscnt 3 #eq
: dofill 0 do 0 0 cons drop loop ;
CONSCNT 4 - dofill
conscnt CONSCNT 1- #eq
54 54 cons iscons? #true \ last cons before GC
conscnt CONSCNT #eq
54 54 cons iscons? #true \ GC should have run
conscnt 4 #eq
carcdr 43 #eq 42 #eq
myref carcdr 45 #eq 44 #eq
leakedcons carcdr 13 #eq 12 #eq

42 ?single #true 42 #eq
42 0 cons ?single #true 42 #eq
1 0 cons 2 swap cons dup ?single not #true #eq
testend
