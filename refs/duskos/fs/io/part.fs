needs io/stream io/blk
unit io/part

extends Blk struct Partition { uint tgtblk first ; }

: reframe
  A! blksz * A> to size A> to first 0 A> to pos
  -1 A> rbuf to no -1 A> wbuf to no ;
:~ ( n dst part -- n dst blk )
  A! size rot tuck A> blksz * <= ?abort"partition out of range" ( dst n )
  A> first + swap A> tgtblk ;
: _readblk ~ readblk ;
: _writeblk ~ writeblk ;
: newpart ( first blkcnt tgtblk -- part )
  >r ['] _readblk ['] _writeblk rot r@ blksz swap
  newblk ( first part ) r> , swap , ;
