needs lib/struct io/stream drv/timer
unit io/wait

extends Stream struct WaitingStream { uint tgtstream timeoutms ; }

: _readbuf ( n st -- ?a n )
  ticks >r dup timeoutms >r tgtstream 2>r begin ( ) \ V1=ts V2=tmout V3=n V4=st
    V3 V4 readbuf dup not while ( 0 )
    V2 V1 elapsedms? not while drop repeat then ( ?a n )
  2rdrop 2rdrop ;

: _writebuf ( a n st -- n )
  ticks >r dup timeoutms >r tgtstream >r begin ( a n ) \ V1=ts V2=tmout V3=st
    2dup V3 writebuf dup not while
    V2 V1 elapsedms? not while drop repeat then ( a n res )
  nip nip 2rdrop rdrop ;

: _close tgtstream close ;

: newwaitingstream ( timeoutms tgtstream -- stream )
  ['] _readbuf ['] _writebuf newstream
  ['] _close over to close ( tmout tgt st )
  swap , swap , ;
