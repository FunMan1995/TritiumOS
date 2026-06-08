needs tests/harness mem/stack
testbegin
3 newstack const s

s empty? #

1 s push
2 s push
3 s push
: _ s push ;
4 expectabort _
s count 3 #eq
s pop 3 #eq
s peek 2 #eq
s peek' @ 2 #eq
s pop 2 #eq
s pop 1 #eq
: _ s pop ;
expectabort _
1 s push
2 s push
s count 2 #eq
s empty
s count 0 #eq
testend
