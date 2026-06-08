needs tests/harness comp/sig
testbegin

\test annotate
annotate ( uint uint -- uint ) max
"max" findannotated #true
exec>str .type "( uint uint -- uint )" #s=

testend
