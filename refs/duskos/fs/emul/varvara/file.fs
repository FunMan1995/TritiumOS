needs fs/core fs/sh io/stream emul/uxn emul/varvara/core mem/stream
unit emul/varvara/file

\ For each file device (fdev), we have a pointer to a stream. Whenever the
\ "name" port changes, we close the previous handler, set it to 0.

0 value fdev \ points to current $a0 or $b0 device
0 value fstream \ points to the current stream associated to fdev
create fstreams 8 allot0
: fno ( -- 0-or-1 ) CURPORT @ 4 lshift 1 and ;
: fdev$
  fno 4* fstreams + @ to fstream
  CURPORT @ $f0 and uxn devices + to fdev ;
: fstream! ( hdl -- )
  dup if fstream ?abort"fstream was supposed to be closed!" then
  dup to fstream fno 4* fstreams + ! ;
: fstream# fstream dup not ?abort"fstream not set" ;

create _fnbuf STR_MAXSZ allot
: _readfn ( -- str )
  fdev 8 + wbe@ [ uxn+, ] ( zstr )
  z[] dup _fnbuf c! _fnbuf 1+ swap cmove ( ) _fnbuf ;

: success! fdev 2 + wbe! ;
: _read ( uxndst -- res ) [ uxn+, ] fdev $a + wbe@ fstream# read ;
: _write ( uxndst -- res )
  [ uxn+, ] fdev $a + wbe@ fstream# write fstream# flush ;

$1000 const MAXSZ \ it ought to be enough for any listing...
MAXSZ newmemstreambuf const _mf
: _mf$ MAXSZ _mf resize _mf rewind ;
: _statwr ( -- )
  walkdir? if "----" _mf puts else
    walksize $ffff > if "????" _mf puts else
      walksize formathex2 _mf write# then then
  SPC _mf putc walkname _mf puts LF _mf putc ;

:~ ( -- )
  fdev$ _readfn lookup if
    _mf$ _statwr _mf truncate _mf rewind _mf fstream!
    fdev 4 + wbe@ _read then ( res ) success! ;
' ~ $a5 setdeo ' ~ $b5 setdeo

: ?closestream fstream ?dup if close 0 fstream! then ;
:~ ( -- ) fdev$ ?closestream _readfn lookup if removefsnode then ;
' ~ $a6 setdeo ' ~ $b6 setdeo

:~ ( -- ) fdev$ ?closestream ;
' ~ $a9 setdeo ' ~ $b9 setdeo

: _readdir ( -- )
  _mf$ enterdir begin gotonext while _statwr repeat
  _mf truncate _mf rewind _mf fstream! ;
:~ ( -- )
  fdev$ fstream not if
    _readfn lookup not if 0 success! exit then
    walkdir? if _readdir else open fstream! then then ( )
  fdev $c + wbe@ _read success! ;
' ~ $ad setdeo ' ~ $bd setdeo

:~ ( -- )
  fdev$ fstream not if
    _readfn lookup not if _fnbuf ensurefile then
    walkdir? if 0 success! exit else
      open fstream!
      fdev 7 + c@ if fstream size fstream seek then then then ( )
  fdev $e + wbe@ _write success! ;
' ~ $af setdeo ' ~ $bf setdeo
