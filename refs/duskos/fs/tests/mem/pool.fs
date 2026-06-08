needs tests/harness io/stream fs/core mem/pool
testbegin

: readall ( st -- str ) 0 over seek bi Stream.size | gets ;

\test create a small pool
8 4 newmempool const mypool
mypool chunktbl be@ $ffffffff #eq
EOC mypool chaincnt 0 #eq

\test allocate a chunk
mypool allocchunk #true 0 #eq
mypool chunktbl be@ $feffffff #eq
0 mypool ?nextchunk not #true
0 mypool chaincnt 1 #eq

\test grow that chain
0 mypool growchain #true
mypool chunktbl be@ $01feffff #eq
0 mypool ?nextchunk #true 1 #eq
0 mypool chaincnt 2 #eq

\test allocate another chunk
mypool allocchunk #true 2 #eq
mypool chunktbl be@ $01fefeff #eq

\test grow first chain again
0 mypool growchain #true
mypool chunktbl be@ $0103fefe #eq

\test try and fail to grow second chain
2 mypool growchain not #true
mypool chunktbl be@ $0103fefe #eq

\test release first chain
0 mypool releasechain
mypool chunktbl be@ $fffffeff #eq

\test try growing the second chain again
2 mypool growchain #true
mypool chunktbl be@ $feff00ff #eq

\test accomodatesize
31 2 mypool accomodatesize #true
mypool chunktbl be@ $010300fe #eq

\test release the pool
2 mypool releasechain

\test single poolstream
mypool chunktbl be@ $ffffffff #eq
mypool getpoolstream const f1
"hello" f1 puts
f1 readall "hello" #s=
mypool chunktbl be@ $feffffff #eq

\test another poolstream
mypool getpoolstream const f2
"goodbye" f2 puts
mypool chunktbl be@ $fefeffff #eq
f1 readall "hello" #s=
f2 readall "goodbye" #s=

\test grow first stream
"again" f1 puts
f1 readall "helloagain" #s=
mypool chunktbl be@ $02fefeff #eq

\test grow second stream
"hey" f2 puts
f1 readall "helloagain" #s=
f2 readall "goodbyehey" #s=
mypool chunktbl be@ $0203fefe #eq

\test write beyond pool capacity
\ f1 has 10 characters, only 6 more fit
"0123456" c@+ f1 write 6 #eq
f1 readall "helloagain012345" #s=
f2 readall "goodbyehey" #s=
mypool chunktbl be@ $0203fefe #eq

\test close first stream
f1 close
mypool chunktbl be@ $ff03fffe #eq

\test closed streams are reused
mypool getpoolstream f1 #eq
f1 size 0 #eq
"other" f1 puts
f1 readall "other" #s=
f2 readall "goodbyehey" #s=
mypool chunktbl be@ $fe03fffe #eq

\test read past EOF
\ There used to be a nasty PS corruption buf in there
f1 getc EOF #eq

testend
