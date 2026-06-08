needs lib/str lib/struct lib/ival ar/tar fs/core io/stream
unit fs/tar

extends Stream struct TarFile {
  uint tgtblk tgtoff ;
}

: abspos A! pos A> tgtoff + ;
: :readbuf ( n file -- a? n )
  r! maxn min ( n V1=file )
  dup if ( n )
    r@ abspos r@ tgtblk tuck seek ( n blk ) readbuf
    r@ incposk then rdrop ;
: roerr abort"TAR FS is read-only" ;
: :close dup closecursor flush ;
: :initfilestruct ( fs -- )
  ['] :readbuf ['] roerr newstream
  ['] :close swap to close
  storage , 0 , ;

\ We expect a leading "./" in all record names
\ FSIDs are sector numbers

\ The index is a LL of: 4b blkpos 4b filesz 4b dir? Xb path

addrof curfs offsetof walkcontext ivalmapfrom { uint idxptr idxll ; }

Record typesz const RECSZ
create record RECSZ allot

: rdrec ( pos -- ) storage seek record RECSZ storage read# ;
: firstrecord ( -- ) 0 rdrec ;
: nextrecord ( -- )
  record empty? ?abort"end of tar chain"
  record recordsize Record typesz /+ ( sec+ )
  RECSZ * storage pos + rdrec ;

: ?trimslash ( a u -- a u ) dup if 2dup + 1- c@ '/' = if 1- then then ;
: ?trimroot ( a u -- a u ) over c@ '.' = if 2 consume[] then ;

variable tmpll
: buildindex ( -- )
  0 tmpll ! firstrecord begin
    record empty? not while
    tmpll lladd
    storage pos RECSZ - , record recordsize , record dir? ,
    record zname z[] ?trimroot ?trimslash ( zname len )
    dup c, cmoveallot nextrecord repeat
  tmpll @ to idxll ;

: idxpos@ idxptr 4+ @ ;
: idxsz@ idxptr 8+ @ ;
: idxdir@ idxptr 12 + @ ;
: idxpath  idxptr 16 + ;

: :gotoroot ( -- ) idxll to idxptr ;

: prefix? ( -- f )
  walkpath c@ not if 1 exit then
  0 walkpath c@ 1+ idxpath c@ < if
    walkpath idxpath startswith? if
      drop idxpath walkpath c@ 1+ + c@ '/' = then then ;

: :gotonext ( -- f )
  begin
    idxptr if idxptr @ to idxptr then
    idxptr while
    prefix? dup if ( f )
      idxpath c@+ walkpath c@ dup if 1+ then consume[] ( f a u )
      2dup walkname []strmove
      '/' rot> cidx if 2drop 0 then then ( f ) not while repeat
    idxdir@ to walkdir? idxsz@ to walksize 0 to walkmtime 1
    else 0 then ;

: :enterdir ( -- )
  idxll idxptr = if exit then
  walkdir# walkpath dup c@ if "/" strcat then
  walkname strcat walkpath strmove ;

: :initfilestruct ( -- )
  ['] :readbuf ['] roerr newstream drop storage , 0 , ;

: :openfile ( file -- )
  idxpos@ RECSZ + over to tgtoff ( file )
  idxsz@ over to Stream.size ( file )
  0 swap seek ;

: newtarfs ( storage -- fs )
  >r ['] roerr dup dup ['] :openfile ['] :initfilestruct
  ['] :enterdir ['] :gotonext ['] :gotoroot
  0 r> newfs
  dup to curfs buildindex ;
