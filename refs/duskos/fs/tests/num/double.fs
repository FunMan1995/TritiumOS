needs tests/harness num/double
testbegin

: 2#eq >r #eq r> #eq ;
1 31 lshift const MIN-INT
-1 1 rshift const MAX-INT

\test n>d
 0 n>d  0  0 2#eq
 1 n>d  0  1 2#eq
 2 n>d  0  2 2#eq
-1 n>d -1 -1 2#eq
-2 n>d -1 -2 2#eq
MIN-INT n>d -1 MIN-INT 2#eq
MAX-INT n>d 0 MAX-INT 2#eq

\test dneg
 0 n>d dneg  0 n>d      2#eq
 1 n>d dneg -1 n>d swap 2#eq
-1 n>d dneg  1 n>d swap 2#eq

\test d+ on small integers
 0  0  5  0 d+  0  5 2#eq
-5 -1  0  0 d+ -1 -5 2#eq
 1  0  2  0 d+  0  3 2#eq
 1  0 -2 -1 d+ -1 -1 2#eq
-1 -1  2  0 d+  0  1 2#eq
-1 -1 -2 -1 d+ -1 -3 2#eq
-1 -1  1  0 d+  0  0 2#eq
\test d+ on medium integers
 0  0  0  5 d+  5  0 2#eq
-1  5  0  0 d+  5 -1 2#eq
 0  0  0 -5 d+ -5  0 2#eq
 0 -5 -1  0 d+ -5 -1 2#eq
 0  1  0  2 d+  3  0 2#eq
-1  1  0 -2 d+ -1 -1 2#eq
 0 -1  0  2 d+  1  0 2#eq
 0 -1 -1 -2 d+ -3 -1 2#eq
-1 -1  0  1 d+  0 -1 2#eq

\test d2*
      0 0 d2* 0 0 2#eq
      1 0 d2* 0 2 2#eq
MIN-INT 0 d2* 1 0 2#eq

\test d2/
 0  0 d2/ 0             0 2#eq
 1  0 d2/ 0             0 2#eq
 0  1 d2/ 0       MIN-INT 2#eq
-1 -1 d2/ MAX-INT      -1 2#eq

testend
