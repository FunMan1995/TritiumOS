needs tests/harness hal/float num/float

hasfloats? not [if] ."no floats, skipping tests\n" \s [then]

testbegin

\test float parsing in interpret mode
float"123" f>n 123 #eq
float"123.456" 1000 n>f f* f>n 123456 #eq

\test float parsing in compile mode
: foo float"1.4" ;
42 n>f foo f/ f>n 30 #eq

\test f.
42 n>f float"1.6" f* 2 exec>str f. "67.20" #s=

testend
