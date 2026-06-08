needs tests/harness lib/wordtbl
testbegin
: foo 42 ;
wordtbl[
:> 54 ;
' foo
:> 102 ;
]wordtbl mytbl

mytbl 0 wexec 54 #eq
: x mytbl 1 wexec ;
x 42 #eq
: x mytbl 3 wexec ;
expectabort x
testend
