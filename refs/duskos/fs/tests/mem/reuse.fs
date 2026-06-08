needs tests/harness mem/reuse
testbegin

\test ?reuse
42 ?reuse const var1
54 ?reuse const var2
var1 var2 <> #true
var1 free
12 ?reuse const var3
var3 var1 #eq
12 ?reuse const var4
var4 var1 <> #true
var4 var2 <> #true

\test ?realloc
var2 12 ?realloc var2 #eq
var2 102 ?realloc const var5
var5 var1 <> #true
var5 var2 <> #true
var5 var3 <> #true
var5 var4 <> #true

\test reuse[ ]reuse
reuse[ 42 allot@ ]reuse const var6
var6 free
12 ?reuse dup var6 #eq free
testend
