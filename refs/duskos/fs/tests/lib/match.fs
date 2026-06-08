needs tests/harness lib/match
testbegin
\test
'c' 0-9? not #
'9' 0-9? #
'z' A-Za-z? #
'0' A-Za-z? not #
'z' alnum? #
'0' alnum? #

\test
: rfind09 rfind"09" ;
"hello" c@+ rfind09 0 #eq
"foo9bar" c@+ rfind09 1 #eq 3 #eq
testend
