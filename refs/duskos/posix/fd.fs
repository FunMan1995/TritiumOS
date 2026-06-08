\ Utilities around POSIX file descriptors.
\ See README for API docs.

extends Blk struct FDBlk { uint fd ; }
: _seek ( sec self -- ) bi blksz | fd rot> * swap fdseek ;
: _readblk ( sec dst self -- )
  >r swap r@ _seek r@ blksz r> fd fdread drop ;
: _writeblk ( sec src self -- )
  >r swap r@ _seek r@ blksz r> fd fdwrite drop ;
: newfddrive ( blksz imgsz fd -- drv )
  >r 2>r ['] _readblk ['] _writeblk
  2r> ( blksz imgsz ) over / newblk ( blk ) r> ( fd ) , ;
: fdopen# ( str write? -- size fd )
  fdopen ?dup not if abort"fdopen failed" then ;
: mountImage ( imgname -- drv ) 1 fdopen# 512 rot> newfddrive ;

\ wrap IO around a read FD and a write FD
extends Stream struct FDStream { uint readfd writefd ; }
create _buf $100 allot
: _mangleres ( fdres -- n ) dup -1 = if 1+ then ;
: _readbuf ( n hdl -- a? read-n )
  _buf rot $100 min rot readfd fdread _mangleres dup if _buf swap then ;
: _writebuf ( a n stream -- written-n ) writefd fdwrite _mangleres ;
: newfdio ( readfd writefd -- stream )
  ['] _readbuf ['] _writebuf newstream rot , swap , ;

0 1 newfdio const stdio
