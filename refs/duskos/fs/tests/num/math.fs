needs tests/harness num/math
testbegin
\test
4 log2 2 #eq
3 log2 1 #eq
3 log2mod 1 #eq 1 #eq
2 log2 1 #eq
1 log2 0 #eq
-1 log2 31 #eq

\test
$1234 8 roundup $1238 #eq
$1238 8 roundup $1238 #eq
$1238 16 roundup $1240 #eq
$1234 8 rounddown $1230 #eq
$1238 8 rounddown $1238 #eq
$1238 16 rounddown $1230 #eq

\test
0 isqrt 0 #eq
1 isqrt 1 #eq
4 isqrt 2 #eq
27 isqrt 5 #eq
2000000 isqrt 1414 #eq
-1 isqrt 65535 #eq

\test
42 abs 42 #eq
-12 abs 12 #eq
testend
