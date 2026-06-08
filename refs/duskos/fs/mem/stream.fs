needs mem/reuse io/stream
unit mem/stream

extends Stream struct MemStream { uint bufptr ; }

: range ( st -- a u ) A! bufptr A> size ;
: writtenrange ( st -- a u ) A! bufptr A> pos ;
: eof? maxn not ;
: rewind ( st -- ) 0 swap seek ;
: :readbuf ( n st -- a? n )
  r! maxn min ( n ) \ V1=st
  dup if r@ pos r@ bufptr + swap then ( ?a n )
  r> doto pos over + | ;
: :writebuf ( a n st -- n )
  r! maxn min ( a n ) \ V1=st
  tuck r@ pos r@ bufptr + ( n a n dst )
  swap cmove
  r> doto pos over + | ;
: _close ( st -- ) bufptr free ;
: newmemstream ( a u -- st )
  ['] :readbuf ['] :writebuf newstream >r ( a u V1=st )
  swap , r@ to size ['] _close r@ to close r> ;
: newmemstreambuf ( sz -- st ) dup ?reuse swap newmemstream ;
