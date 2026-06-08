needs lib/psrs mem/range lib/bit lib/macro lib/type comp/sig
unit num/xxhash

consts $9E3779B1 PRIME1 $85EBCA77 PRIME2 $C2B2AE3D PRIME3 \
       $27D4EB2F PRIME4 $165667B1 PRIME5

\ flags used by XXH32 struct
consts 1 LARGE_LEN? 2 READING? 4 CLOSED?

extends Stream struct XXH32 {
  uint total_len ;
  [uint,4] accs ;
  [uchar,16] buf ;
  uchar buf_len flags ;
  ushort _pad ;
}

: _avalanche ( k -- k )
  dup 15 rshift xor
  PRIME2 *
  dup 13 rshift xor
  PRIME3 *
  dup 16 rshift xor ;

: _finalize ( a u k -- k )
  begin over 4 >= while
    rot dup
    le@ PRIME3 * rot +
    17 lrot PRIME4 *
    >r 4 + swap 4 - r>
  repeat

  begin over 1 >= while
    rot dup
    c@ PRIME5 * rot +
    11 lrot PRIME1 *
    >r 1 + swap 1 - r>
  repeat

  \ a and u aren't needed anymore
  rot> 2drop
  _avalanche ;

: _initAccs ( u -- k1 k2 k3 k4 )
  r! [ PRIME1 PRIME2 + litn ] +
  r@ PRIME2 +
  r@
  r> PRIME1 - ;

: _mergeAccs ( k1 k2 k3 k4 -- k )
  18 lrot >r 12 lrot >r 7 lrot >r
  1 lrot r> + r> + r> + ;

:~
  W) &) A>) @,
  PSP) @, W) ' execute
  PRIME2 i) *,
  A) +,
  13 i) lrot,
  PRIME1 i) *,
  A) !,
  4 i) A>) +,
  4 PSP) +n,
  A) &) @!, ;

code _round ~ le@, exit,
code _rounda ~ ale@, exit,

\ condition code for the _consumeLong loop
: _check
  16 i) -, over,
  RSP) S>) @, S) &) >) bool, ; immediate

: _consumeLong ( a u ak -- a )
  rot> over + 15 -
  >r swap
  dup $3 and not if \ use the aligned version if the input is aligned
    begin
      _rounda _rounda _rounda _rounda _check
    until
  else \ unaligned version
    begin
      _round _round _round _round _check
    until
  then rdrop drop ;

: xxh32[] ( a u seed -- u )
  over r! 16 >= if ( a u s R: u )
    rot> 2>r _initAccs
    4 ps[] 2dup swap[] drop
    2r> rot _consumeLong ( k4 ... k1 a R: u )
    >r 4 ps[] swap[] _mergeAccs
    r> swap
  else
    nip PRIME5 +
  then

  r@ +
  r> 15 and swap _finalize
  ;

\ XXH32 related words

\ reset the XXH32 state with a given seed
: reset \ reset ( u xxh32 -- )
  swap >r
  0 over to total_len
  0 over to buf_len
  0 over to flags
  \ we don't cfill buf because it isn't needed
  r> _initAccs 5 roll
  0 4 do
    swap over accs i 1- 4* + !
  1 -loop
  drop ;

: digest ( xxh32 -- u )
  dup flags LARGE_LEN? and if
    r!
    4 0 do V1 accs i 4* + @ loop
    rdrop
    _mergeAccs
  else
    dup accs 2 4* + @ PRIME5 +
  then

  ( xxh k )
  dip tri buf | buf_len | total_len | +
  _finalize ;

: update ( a u xxh32 -- )
  r! doto total_len over + |
  dup 16 >= r@ total_len 16 >= or
  [ LARGE_LEN? 1- litn ] lshift
  r@ doto flags or |

  dup 16 r@ buf_len - < if
    r@ bi buf | buf_len +
    swap r! cmove
    r> r> doto buf_len + |
    exit
  then

  over + ( a bend )
  r@ buf_len if \ non empty buffer, complete first
    over r@ bi buf | buf_len +
    16 r@ buf_len - r! cmove swap r> + swap
    r@ tri buf | buf_len | accs _consumeLong drop
    0 r@ to buf_len
  then

  ( a bend )
  2dup swap- dup 16 >= if
    rot swap ( bend a u )
    r@ accs _consumeLong
  else drop swap then

  2dup > if
    swap over -
    tuck r@ buf swap cmove
    r@ to buf_len
  else 2drop then
  rdrop ;

: _writebuf ( a n stream -- written-n )
  dupbi flags READING? and | flags CLOSED? and or if
    3 ndrop 0
  else
    over dip update |
  then ;

: _readbuf ( n stream -- a? read-n )
  dup flags CLOSED? and if
    2drop 0 exit \ ignore if stream closed
  then

  r! flags READING? and not if
    r@ dupbi digest | accs le!
    doto flags READING? or |
    0 r@ to buf_len
  then
  ( n R: stream )

  4 r@ buf_len - min
  r@ bi accs | buf_len + ( min a R: stream )

  swap r> doto buf_len over + |
  dup not if nip then ;

: _close ( stream -- )
  doto flags CLOSED? or | ;

macro methodSet "['] _%< r@ to %0"
: newXXH32 ( u -- xxh32 )
  alignhere here >r XXH32 typesz allot
  methodSet writebuf
  methodSet readbuf
  methodSet close
  r@ reset r> ;

: xxh32
  newXXH32 >r begin
    16 over readbuf ?dup
  while r@ update repeat
  drop r> digest ;

: xxh32<<
  word openpath swap xxh32 ;

\ Annotations for language interoperability
XXH32 newpointer const XXH32Ptr
annotate ( *void uint uint -- uint ) xxh32[]
annotate ( *Stream uint -- uint ) xxh32
annotate ( uint -- *XXH32 ) newXXH32
annotate ( *void uint *XXH32 -- ) update
annotate ( uint *XXH32 -- ) reset
annotate ( *XXH32 -- uint ) digest
