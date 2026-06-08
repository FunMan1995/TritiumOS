needs tests/harness mem/stream ar/ungz
testbegin
\ Testing ar/ungz
create _expected ,"Hello from compressed file!"
create _resultbuf $20 allot
"data/tests/hello.gz" openpath ( inio )
dup _resultbuf $20 newmemstream ( inio inio outio )
ungz ( inio err ) 0 #eq close
_resultbuf _expected 27 c[]= #
testend
