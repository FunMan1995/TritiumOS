needs num/math lib/psrs lib/str lib/ival lib/match fs/core fs/sh mem/range \
      mem/alloc mem/arena mem/array text/ts text/clip io/search io/stream
unit text/ed

struct Line {
  uint ptr ;
  ushort cnt allocsz ;
}
16 const GRANULARITY \ when we realloc a line, we round up to this.

: fits? ( n line -- f ) A! cnt + A> allocsz <= ;
: #fits fits? not ?abort"not enough line space" ;
: line[] ( line -- a u ) A! ptr A> cnt ;
: printline ( line -- ) line[] rtype nl> ;
: _delchars ( idx n line -- ) >r \ V1=line
  2dup + V1 cnt < if ( idx n )
    r! tuck + V1 line[] rot consume[] rslide-
    r> r> doto cnt swap- |
    else drop r> to cnt then ;
: ?realloc ( arena n line -- ) \ n=bytes to add to line
  2dup fits? if 2drop drop else
    r! cnt + GRANULARITY roundup ( arena newsz ) \ V1=line
    dup V1 to allocsz swap alloc[ allot@ ]alloc ( dst )
    V1 line[] dipover cmove ( dst )
    r> to ptr then ;
: append ( a u line -- )
  2dup #fits A! ptr A> cnt + ( src u dst )
  over A> doto cnt + | swap cmove ;
: split ( idx line -- a u )
  r! line[] rot tuck - max0 dip + | ( a u )
  r> doto cnt over - | ;
: words[] ( a u -- ... n )
  swap >r 0 1 rot 0 do ( ... n lastws? V1=a )
    V1 i + c@ ws? tuck not and if i rot> dip 1+ | then loop
  drop rdrop ;
: wordunder ( a u idx -- a u )
  >r 2dup 2>r words[] begin ( ... n V1=refidx V2=a V3=topidx )
    ?dup while 1- swap ( ... n idx )
    dup V1 > while to V3 repeat
    ( ... n idx ) dip ndrop | else ( ) 0 then ( idx )
  r> over - ( idx u ) r> rot + swap rdrop ;

\ An empty line doesn't point into the content buffer, but it's not null
\ either (to avoid repetitive null-checking logic). It's just a pointer to
\ NULLSTR with a count of 0 and an alloc of zero.
create EMPTY NULLSTR , 0 ,
: initempty ( a -- ) EMPTY swap Line typesz cmove ;

variable edbufs \ First edbuf of the LL
variable curbuf \ pointer to active buf
0 value edstream \ set later down

curbuf ivalmap {
  uint next epos buf lines ;
  \ for app/ed
  uint top gutter visualmode mark ;
  [uchar,0] filename ;
}

: cpos ( pos -- cpos ) $ffff and ;
: lpos ( pos -- lpos ) 16 rshift ;
: eposc epos cpos ;
: eposl epos lpos ;
: joinpos ( lpos cpos -- pos ) $ffff min swap 16 lshift or ;

: line ( idx -- line ) lines mem/array get' ;
: linecnt lines Array.cnt ;
: curline eposl lines mem/array get' ;

create findstr STR_MAXSZ allot0
: edfindnext ( -- )
  epos doto epos 1+ dup | edstream to pos
  findstr edstream search if drop edstream pos then to epos ;
: edfind ( str -- ) findstr strmove edfindnext ;

: bounds ( lpos cpos -- pos )
  linecnt 1- rot min ( cpos lpos )
  dup lines mem/array get' cnt rot min joinpos ;
: cpos! ( cpos -- ) eposl swap bounds to epos ;

: nextword ( -- ) curline line[] eposc wordunder + curline ptr - cpos! ;
: prevword ( -- )
  curline line[] eposc 1- max0 wordunder drop curline ptr - cpos! ;
: wordundercursor ( -- a u )
  curline line[] eposc wordunder ( a u )
  2dup rfind"\0 " if ( a u idx ) nip then ;

: go ( idx -- ) eposc bounds to epos ;
: godown ( n -- ) eposl + max0 go ;
: goleft ( n -- ) eposc swap- max0 cpos! ;
: goright ( n -- ) eposc + cpos! ;
: goup ( n -- ) neg godown ;

: poseol? ( -- f ) eposc curline cnt = ;

:~ ( idx -- ) 1 over lines mem/array insert initempty go ;
: appendline eposl 1+ ~ ;
: insertline eposl ~ ;

: ensureline ( -- )
  linecnt not if 1 lines mem/array append initempty 0 to epos then ;

: dellines ( n -- ) eposl tuck lines mem/array delete ensureline go ;
: delchars ( n -- ) curline eposc rot> _delchars ;
: replchar ( c -- ) poseol? if edstream putc else eposc curline ptr + c! then ;
\ Size in characters, including newlines between lopos and hipos
: posdiff ( lopos hipos -- len )
  0 >r over lpos over lpos swap do ( lo hi V1=res )
    i line cnt 1+ doto V1 + | loop ( lo hi )
  dip cpos | cpos swap- r> + ;
: clipto ( pos -- )
  epos ?swap ( lo hi ) over doto epos swap | ( lo hi oldpos )
  rot> posdiff dup clipset ( oldpos n a )
  swap edstream read# ( oldpos )
  to epos ;
: rangeafter ( epos -- a u ) dup lpos line line[] rot cpos ltrim[] ;
: delto ( pos -- )
  epos ?swap ( lo hi ) over to epos ( lo hi )
  over lpos over lpos = if swap- delchars else ( lo hi )
    \ We join the two "leftovers" in curline and then delete the rest.
    swap cpos curline to cnt ( hi )
    dup rangeafter buf over curline ?realloc ( hi a u )
    curline append ( hi )
    lpos dup line cnt not - ( linehi )
    eposl curline cnt bool + ( linehi linelo )
    tuck - 1+ swap lines mem/array delete ensureline then
  eposl go ;

: empty ( -- ) 0 filename ! 0 to top lines mem/array empty ensureline ;
: newedbuf ( -- ed )
  newarena Line typesz $200 newarray ( arena lines )
  here# >r 0 ( next ) , 0 ( epos ) , swap ( arena ) , ( lines ) ,
  0 ( top ) , 80 ( gutter ) , 0 ( visualmode ) , 0 ( mark ) ,
  STR_MAXSZ allot0 r> ;

\ Edbuf stream
: lastline? eposl linecnt 1- = ;
: _eof? lastline? poseol? and ;
: :readbuf ( n st -- a? read-n )
  _eof? if 2drop 0 exit then
  >r poseol? if drop 1 godown 0 cpos! LF c[] else
    curline cnt eposc - ( n1 n2 )
    curline ptr eposc + ( n1 n2 a )
    rot> min dup doto epos + | then
  epos r> to pos ;

: _append ( a u -- ) buf over curline r! ?realloc ( a u ) r> append ;
: _append+ ( a u -- ) tuck _append doto epos + | ;
: :writebuf ( a u st -- written-n )
  >r eposc curline split []>pool 2>r ( a u V1=sp-a V2=sp-u )
  r! begin ( a u ) \ V3=written-n
    2dup LF rot> cidx while ( a u idx )
    dipover ( a u a sub-u )
    tuck _append+ appendline ( a u sub-u )
    1+ tuck - dip + | ( new-a new-u ) repeat ( a u )
  _append+ r> 2r> ( written-n split-a split-u ) _append
  epos r> to pos ;

' :readbuf ' :writebuf newstream to edstream

\ Buffer
: addedbuf newedbuf dup curbuf ! edbufs llend ! empty ;
addedbuf
: nextedbuf curbuf @ @ ?dup not if edbufs @ then curbuf ! ;

:~ 1+ swap do i c, loop ;
create alphanum '0' '9' ~ 'a' 'z' ~ 'A' 'Z' ~
62 const alphacnt
: alphanum[] alphanum alphacnt ;

: c>edbuf ( c -- edbuf-or-0 )
  alphanum[] cidx not if 0 else
    edbufs begin ( n ed ) @ ?dup while swap ?dup while 1- swap repeat
    ( ed ) else ( n ) drop 0 then then ;

:~ ( ll -- )
  curbuf @! ( oldbuf )
  filename dup c@ if stype else drop ."(no file)" then
  spc> linecnt . spc> ( oldbuf )
  dup curbuf @! = if ."*" then nl> ;
: .edbufs ( -- )
  0 edbufs @ begin ( idx ll ) ?dup while
    dip dup alphanum + c@ emit spc> 1+ |
    dup ~ @ repeat ( idx ) drop ;
: findedbuf ( strpath -- f )
  edbufs @ begin ( s ll )
    ?dup while
    dup curbuf ! over filename s= not while @ repeat
    ( s ll ) drop 1 else ( s ) 0 then nip ;
: empty? ( ll -- f )
  curbuf @! >r
  linecnt 1 = curline cnt not and
  r> curbuf ! ;
: findemptybuf ( -- f )
  edbufs @ begin ( ll )
    dup while dup empty? not while @ repeat
    ( ll ) curbuf ! 1 else ( 0 ) then ;
: ?addedbuf ( -- ) findemptybuf not if addedbuf then ;

: bol $10000 goleft ;
: eol $10000 goright ;
: sl ( -- )
  eposc curline split ( a u )
  appendline edstream write# bol ;
: jl ( -- )
  eposl 1+ linecnt over = if drop exit then ( lpos )
  dup line line[] ( lpos a u )
  rot 1 swap lines mem/array delete ( a u )
  eol epos >r edstream write# r> to epos ;
: edload ( strpath -- )
  dup openpath empty ( strpath file )
  swap filename strmove ( file )
  edstream swap spit 0 to epos ;
: edload<< word edload ;
: ?edload ( strpath -- ) dup findedbuf if drop else ?addedbuf edload then ;
: ?edload<< word ?edload ;
: edsaveto ( file -- )
  doto epos 0 | >r
  dup edstream spit ( file )
  dup truncate close
  r> to epos ;
: edsave ( -- )
  filename dup c@ not ?abort"no file associated to edbuf"
  ."Saving edbuf to " dup stype ."... " idle ( fn )
  ensurefile open edsaveto ."saved" ;
