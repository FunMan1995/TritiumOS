needs io/stream io/blk
unit mem/blk

extends Blk struct MemBlk { [void,0] buf( ; }

: )buf ( self -- a ) bi buf( | size + ;
: _addr ( n self -- a ) >r
  V1 blksz * r@ buf( +
  r> )buf over <= ?abort"blk out of range" ;
: _blk@ ( n dst self -- ) >r swap r@ _addr swap ( src dst ) r> blksz cmove ;
: _blk! ( n src self -- ) >r swap r@ _addr ( src dst ) r> blksz cmove ;

: newmemblk ( blksz blkcnt -- blk )
  2>r ['] _blk@ ['] _blk! 2r> newblk dup size allot ;
