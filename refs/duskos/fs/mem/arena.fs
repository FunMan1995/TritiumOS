needs mem/alloc
unit mem/arena

$1000 const ARENASZ

\ An arena buffer is a 4b LL link followed by a ARENASZ buffer

: newbuf ( -- buf ) sys[ here# 0 , ARENASZ allot ]alloc ;
: buf[] ( buf -- a u ) 4+ ARENASZ ;
: nextbuf ( buf -- buf ) dup @ ?dup if nip else newbuf tuck swap ! then ;

\ An arena is a 4b link to root buffer followed by 4b link to current buffer
3 4* adder root
4 4* adder curbuf

: reset ( -- )
  CURALLOC @ dup root @ tuck swap curbuf ! ( buf )
  buf[] over HERE ! + HEREMAX ! ;
: ensurenext ( -- ) CURALLOC @ curbuf @ nextbuf drop ;
: arenanewhere ( -- a u )
  CURALLOC @ curbuf dup @ nextbuf tuck swap ! buf[] ;

: newarena ( -- arena )
  newbuf ['] arenanewhere over buf[] newallocator ( buf alloc ) over , swap , ;
