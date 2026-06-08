needs lib/struct mem/range mem/reuse
unit mem/array

struct Array { uint ptr cnt elemsz alloc ; }

: _' ( idx array -- a ) A! elemsz * A> ptr + ;
: _inbounds? ( idx array -- f ) cnt < ;
: _#bounds _inbounds? not ?abort"array bounds error" ;
: get' ( idx array -- a ) 2dup _#bounds _' ;
: _'end ( array -- a ) bi cnt | _' ;
: get ( idx array -- n ) get' @ ;
: _?alloc ( cnt array -- )
  tuck A! cnt + A> elemsz * ( array minalloc )
  A> alloc over < if ( array newalloc )
    2* dup A> to alloc A> ptr ( array newalloc ptr )
    swap ?realloc swap to ptr ( )
    else 2drop then ;
: append ( cnt array -- a )
  2dup _?alloc
  dup _'end ( cnt array a )
  rot> doto cnt + | ( a ) ;
: insert ( cnt idx array -- a ) >r \ V1=self
  over V1 _?alloc
  dup V1 _inbounds? if ( cnt idx )
    V1 get' >r dup V1 elemsz * ( cnt by ) \ V2=src
    V1 _'end V2 - ( cnt by u )
    V2 swap rslide+ ( cnt )
    V1 doto cnt + | r> rdrop ( a )
    else drop r> append then ;
: delete ( cnt idx array -- ) >r \ V1=self
  over bool over V1 _inbounds? and if
    2dup + V1 cnt < if ( cnt idx )
      V1 get' dup >r over V1 elemsz * + ( cnt src ) \ V2=dst
      V1 _'end over - ( cnt src u )
      r> swap cmove ( cnt )
      r> doto cnt swap- |
      else nip r> to cnt then
    else 2drop rdrop then ( ) ;
: empty ( array -- ) 0 swap to cnt ;
: newarray ( elemsz allotcnt -- array )
  over * dup ?reuse here >r , 0 , swap , , r> ;
