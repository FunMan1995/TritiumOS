needs lib/struct io/stream
unit io/blk

struct Buf {
  uint blk no dirty ;
  [void,0] buf( ;
}

extends Stream struct Blk {
  uint rbuf wbuf blksz ;
  xt writeblk readblk ;
}

: bread ( buf -- ) A! no A> buf( A> blk readblk ;
: bwrite ( buf -- ) A! no A> buf( A> blk writeblk ;
: bflush ( buf -- ) A! dirty if 0 A> to dirty A> bwrite then ;
: bload ( sec buf -- )
  2dup no = if 2drop else dup bflush tuck to no bread then ;
: bwindow ( pos buf -- ?a n-or-0 )
  r! blk blksz /mod ( off relsec V1=buf )
  dup r@ blk blksz * r@ blk size < if
    r@ bload ( off )
    r@ buf( over + r> blk blksz rot -
    else 2drop rdrop 0 then ;
: bplace ( n buf -- a? n-or-0 )
  r! blk pos r@ bwindow dup if ( n a n V1=blk )
    rot min r> blk incposk else ( n 0 ) nip rdrop then ;
: ?binvalidate ( n buf ) A! no = if -1 A> to no then ;

: blkcnt A! size A> blksz /+ ;
: swapbufs ( blk -- ) dup wbuf bflush A! rbuf A> wbuf A> to rbuf A> to wbuf ;
: splitpos ( blk -- off relsec ) A! pos A> blksz /mod ;
: ?rbuf ( tgtblkno blk -- buf ) tuck wbuf no = if wbuf else rbuf then ;
: ?wbuf ( tgtblkno blk -- buf )
  r! wbuf no over = if drop else
    r@ rbuf no = if r@ swapbufs then then
  r> wbuf ;
: window ( pos wr? blk -- ?a n )
  >r over r@ blksz / ( pos wr? tgtblkno V1=blk )
  swap if
    r> ?wbuf r! bwindow dup bool r> to dirty
    else r> ?rbuf bwindow then ;
: _readbuf ( n blk -- ?a n ) dupbi pos | blksz / swap ?rbuf bplace ;
: _writebuf ( a n blk -- written-n )
  r! splitpos swap not if ( a n tgtblkno )
    over r@ blksz >= if \ whole block write
      dup r@ rbuf ?binvalidate
      dup r@ wbuf ?binvalidate
      dup r@ wbuf no = if -1 r@ wbuf to no then
      nip swap r@ writeblk r@ blksz
      r> incposk exit then then ( a n tgtblkno V1=blk )
  r@ ?wbuf bplace dup if ( a a n )
    1 r> wbuf to dirty r! cmove r> else ( a 0 ) nip rdrop then ;
: _flush wbuf bflush ;
: newbuf ( sz -- a ) 0 -1 0 3 n,@ swap allot ;
: newblk ( readxt writext blksz blkcnt -- blk )
  \ We have to allocate in *front* of the struct
  over bi newbuf | newbuf ( ... buf1 buf2 )
  ['] _readbuf ['] _writebuf newstream >r ( rxt wxt blksz cnt buf1 buf2 V1=blk )
  2>r dup -1 <> if over * then V1 to size 2r> ( rxt wxt blksz buf1 buf2 )
  5 n,@ drop ( )
  r@ dup rbuf to blk r@ dup wbuf to blk
  ['] _flush r@ to flush r> ;
