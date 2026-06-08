needs lib/struct io/stream
unit mem/pool

struct Pool { uint poolbuf chunktbl chunksz chunkcnt ; }

extends Stream struct PoolStream { uint pool chunkidx ; }

$ff const FREE
$fe const EOC

: newmempool ( chunksz chunkcnt -- pool )
  dup EOC >= ?abort"pool chunksz exceeds limit"
  alignhere 2dup * allot@ ( chunksz cnt buf )
  over allot@ ( chunksz cnt buf tbl )
  here# >r swap , , swap , , ( )
  r@ chunktbl r@ chunkcnt -1 cfill r> ;

: chunk' ( idx pool -- a ) bi poolbuf | chunksz rot * + ;
: tbl' ( idx pool -- a ) chunktbl + ;
: allocchunk ( pool -- ?idx f )
  FREE over bi chunktbl | chunkcnt cidx if ( pool idx )
    tuck swap tbl' EOC swap c! 1 else drop 0 then ;

: notfree# ( n -- n ) dup FREE = ?abort"messed up pool operation" ;
: eoc' ( idx pool -- a )
  begin tuck tbl' dup c@ notfree# EOC <> while ( pool a )
    c@ swap repeat nip ;
: growchain ( idx pool -- f )
  tuck eoc' swap allocchunk if swap c! 1 else drop 0 then ;

: releasechain ( idx pool -- )
  begin tuck tbl' dup c@ FREE rot c! ( pool idx ) swap over EOC = until 2drop ;

: ?nextchunk ( idx pool -- ?idx f )
  tbl' c@ notfree# dup EOC = if drop 0 else 1 then ;

: chaincnt ( idx pool -- n )
  over EOC = if 2drop 0 exit then
  1 rot> begin ( n idx pool ) tuck ?nextchunk while ( n pool idx )
    swap rot 1+ rot> repeat ( n pool ) drop ;

: chainsz ( idx pool -- sz ) tuck chaincnt swap chunksz * ;

: accomodatesize ( sz idx pool -- f )
  >r dup r@ chaincnt rot r@ chunksz /+ ( idx cnt tgtcnt )
  2dup >= if 2drop drop rdrop 1 else
    swap- r> swap 0 do ( idx pool )
      2dup growchain not if break then loop ( idx pool )
    2drop broke? not then ;

: chunkref ( st -- idx pool ) bi chunkidx | pool ;
: seekmem ( st -- ?a n-or-0 )
  dup chunkidx EOC = if drop 0 exit then
  r! maxn if ( ) \ V1=st
    r@ pos r@ pool chunksz /mod ( r q )
    r@ chunkref rot 0 do ( r idx pool )
      tuck ?nextchunk not if break then ( r pool idx ) swap loop
    broke? if ( r pool ) 2drop 0 exit then ( r idx pool )
    chunk' over + ( r a )
    r> pool chunksz rot -
    else rdrop 0 then ;
: _readbuf ( n st -- a? n )
  r! seekmem dup if ( n a n V1=st ) rot min r> incposk else rdrop nip then ;
: _writebuf ( buf n st -- n )
  r! pos over + r@ ?grow r@ seekmem dup if ( buf n a n )
    rot min r! cmove r> else ( buf n 0 ) nip nip then r> incposk ;

: _resize ( sz st -- )
  dup chunkidx EOC = if
    dup pool allocchunk not if 2drop exit then ( sz st idx )
    over to chunkidx then ( sz st )
  2dup chunkref accomodatesize not if ( sz st )
    nip dup chunkref chainsz swap then ( sz st )
  to Stream.size ;

: _close ( st -- )
  dup chunkidx EOC <> if dup chunkref releasechain then ( st )
  0 over to pool
  0 over to size
  0 over to pos
  EOC swap to chunkidx ;

variable streams
: newpoolstream ( pool -- stream )
  streams lladd ['] _readbuf ['] _writebuf newstream >r
  ['] _resize r@ to resize
  ['] _close r@ to close ( pool )
  , EOC , r> ;

: ?unusedstream ( -- st-or-0 )
  streams @ begin dup while dup 4+ pool while @ repeat 4+ then ;
: getpoolstream ( pool -- stream )
  ?unusedstream ?dup if tuck to pool else newpoolstream then ;
