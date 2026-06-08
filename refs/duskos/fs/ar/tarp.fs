needs io/stream num/math lib/str ar/tar fs/core fs/sh
unit ar/tarp

0 value stream
Record typesz const RECORDSZ
create record RECORDSZ allot
: rd ( -- ) record RECORDSZ stream read# ;
: normname ( -- str )
  record zname "./" over 2 s[]= if 2+ then ( zstr )
  z[] []>str ;
: .record normname stype spc> record recordsize . nl> ;
: tarls ( stream -- )
  to stream rd begin
    .record record recordsize ( sz )
    RECORDSZ roundup nullstream stream spitn
    rd record empty? until ;

: untar ( stream -- )
  to stream rd enterdir begin ( )
    .record idle record recordsize ?dup if ( sz )
      walk>r 0 normname ensurepath open ( sz file )
      2dup stream spitn dup truncate close ( sz )
      RECORDSZ mod ?dup if RECORDSZ swap- record swap stream read# then
      r>walk then ( )
    rd record empty? until
  nullstream stream spitclose ;

: octal! ( n a sz -- )
  tuck + 1- swap 0 do ( n a )
    over 7 and '0' + over c! ( n a )
    1- swap 3 rshift swap loop 2drop ;
: zero record RECORDSZ 4/ 0 fill ;
: recwr ( -- )
  "ustar  " c@+ record signature swap cmove
  record checksum 8 SPC cfill
  $1b4 ( o664 ) record omode 7 octal!
  0 record ouid 7 octal!
  0 record ogid 7 octal!
  0 record omtime 11 octal!
  0 record RECORDSZ 0 do c@+ rot + swap loop drop ( n )
  record checksum 6 octal!
  record RECORDSZ stream write# ;
: tarfile ( -- )
  zero .walkpath idle nl>
  walkpath walkname pathcat lowstr c@+ record swap cmove
  walksize record ofilesz 11 octal!
  recwr stream open spitclose
  walksize RECORDSZ mod ?dup if
    zero record RECORDSZ rot - stream write# then ;
: tardir ( -- )
  enterdir begin gotonext while
    walkdir? if walk>r tardir r>walk else tarfile then repeat ;
: tar ( ... n stream -- )
  to stream 0 do ( ... path )
    lookup# walkdir? if tardir else tarfile then loop
  stream close ;
: tar< >r strings< r> tar ;
