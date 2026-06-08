needs lib/str lib/struct
unit io/stream

struct Stream {
  uint pos size ;
  xt readbuf writebuf flush close resize ;
}

: ioerr abort"I/O error" ;
: ?ioerr if ioerr then ;

: write ( a n st -- written-n )
  >r r! begin dup while ( a n V1=st V2=n )
    2dup V1 writebuf ?dup while
    ( a n written-n ) consume[] repeat then ( a n )
  r> swap- nip rdrop ;
: write# ( a n st -- ) over >r write r> <> ?ioerr ;
: read ( a n st -- read-n )
  >r r! begin dup while ( a n V1=st V2=n )
    2dup V1 readbuf ?dup while ( a n dst src read-n )
    r! rot swap cmove r> consume[] ( a n )
  repeat ( a n dst ) drop then ( a n )
  r> swap- nip rdrop ;
: read# ( a n st -- ) over >r read r> <> ?ioerr ;
: getc ( st -- c ) 1 swap readbuf if c@ else EOF then ;
: putc ( c st -- ) swap c[] rot writebuf not ?ioerr ;
: gets ( n st -- str ) over dup 1+ strallot r! c!+ rot> read# r> ;
: puts ( str st -- ) swap c@+ rot write# ;

128 const BUFSZ
create buf BUFSZ allot
variable curst
variable readsz
: next< ( -- ?a u ) buf readsz @ curst @ read dup if buf swap then ;
:~  ( w st -- st )
  ['] next< NEXTIN< @! >r
  curst @! ?dup if
    >r INPTR @ 0 INSZ @! []>r
    ( w ) execute r>[] INSZ ! INPTR ! r>
  else
    INPTR @ >r 0 INSZ @! >r
    ( w ) execute r> INSZ ! r> INPTR ! 0 then ( st )
  curst @!
  r> NEXTIN< ! ;
: exec< BUFSZ readsz ! ~ close ;
: exec1< 1 readsz ! ~ drop ;
: interpretstream ['] interpret swap exec< ;

: spitn ( n dst st -- )
  2>r begin ( n ) \ V1=dst V2=st
    ?dup while
    dup V2 readbuf ?dup while ( n a read-n )
    tuck V1 write# - repeat ( n ) drop then ( )
  2rdrop ;
: spit -1 rot> spitn ;
: spitclose tuck spit close ;
: spitcloseboth 2dup spit close close ;

: seek ( n st -- ) A! size min max0 A> to pos ;
: seekrel ( n st -- ) A! pos + A> seek ;
: rewind1 ( st -- ) -1 swap seekrel ;
: maxn ( st -- n ) A! size A> pos - max0 ;
: incposk ( n st -- n ) doto pos over + | ;
: truncate ( st -- ) A! pos A> resize ;
: ?grow ( sz st -- ) 2dup size > if resize else 2drop then ;

: newstream ( 'rbuf 'wbuf -- stream )
  2>r ['] 2drop ['] flush ['] drop r> r> 0 0 7 n,@ ;

:> 2drop 0 ; :> drop nip ; newstream const nullstream
' ioerr :> drop tuck cmoveallot ; newstream const herestream
' ioerr :> drop tuck rtype ; newstream const console
0 value a 0 value u
:> drop a u rot min ; ' ioerr newstream const _
: fillstream to u to a _ ;
