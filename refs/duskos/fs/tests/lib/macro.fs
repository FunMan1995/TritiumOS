needs tests/harness lib/macro
testbegin
\test macro
42 twice"%< + " 2 4 48 #eq
macro foo "%< - "
42 foo "2" foo "3" 37 #eq
macro foo "%-%< - "
42 foo "ignore" "2" 40 #eq
42 twice"%<%0 + " "2" "4" 42 22 + 44 + #eq

\test macro in a compiled word
macro foo "3 + "
: bar 2 foo 2* ;
bar 10 #eq

\test macro in a macro, right at the end of it
\ Previously, when restoring the previous stream, it wouldn't check when the
\ restored INSZ was zero, resulting in a false "end of stream" signal.
macro foo "42 + "
macro bar "12 foo "
bar 54 #eq scntneutral#
\ When this test was failing, it messed up with RS in a bad way resulting in
\ confusing and catastrophic failures, but I don't think the failure could lead
\ to a "false pass", so I think the test is good.
testend
