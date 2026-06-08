\ Deflate files of the gzip format
needs comp/c io/stream comp/sig
unit ar/ungz

cc<< ar/puff.c

: _err abort"ungz error" ;
: _assert not if _err then ;

: _skiptonull ( hdl -- ) begin dup getc not until drop ;

\ Take a gzip file from inhdl (IO) and spit the deflated version on outhdl
: ungz ( inhdl outhdl -- err ) >r >r \ V1=out V2=in
  V2 getc $1f = _assert V2 getc $8b = _assert \ ID1+ID2
  V2 getc 8 = _assert \ CM
  V2 getc >r \ V3=FLG
  here 6 V2 read# \ useless stuff
  V3 $04 and if \ FLG.EXTRA
    V2 getc V2 getc 8 lshift or ( xlen )
    here swap V2 read# \ read extra
  then
  V3 $08 and if \ FLG.NAME
    V2 _skiptonull then
  r> $10 and if \ FLG.COMMENT
    V2 _skiptonull then
  r> r> swap puff ;
