needs tests/harness hal/float

hasfloats? not [if] ."no floats, skipping tests\n" \s [then]

testbegin

\test some simple arithmetics
42 n>f 12 n>f f* 5 n>f f/ f>n 100 #eq

\test fscale10
42 n>f 3 fscale10 f>n 42000 #eq

testend
