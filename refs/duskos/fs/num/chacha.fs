needs lib/bit lib/struct hal/vreg lib/type comp/sig hal/opq
unit num/chacha

create CHACHA_CST ,"expand 32-byte k"

enum _none IETF Legacy XChaCha

extends Stream struct ChaCha {
  [uint,16] wstate ; \ working state
  [uint,16] bstate ; \ base state
  *Stream wrappedstream ;
  uchar rounds variant remaining _pad ;
}

\ a += b ; c^=a ; c <<= n
: _regRound, ( n c b a )
  swap &) over (src dst) +,
  &) over (src dst) ^,
  swap i) swap (src dst) lrot, ;

\ a=S b=W c=A d=R0
\ b is W to remove some xchg W) Reg>)
: _rqround,
  16 R0) W) S) _regRound,
  12 W) R0) A) _regRound,
  08 R0) W) S) _regRound,
  07 W) R0) A) _regRound, ;

\ a=S b=W c=A d=R0
: _load4, ( a b c d -- )
  R2) W>) !,
  W) swap 4* +) R0>) @,
  W) swap 4* +) A>) @,
  swap
  W) swap 4* +) S>) @,
  W) swap 4* +) W>) @, ;

: _store4, ( a b c d -- )
  R2) R1>) @,
  R1) swap 4* +) R0>) !,
  R1) swap 4* +) A>) !,
  R1) swap 4* +) W>) !,
  R1) swap 4* +) S>) !,
  R1) &) @, ;

: qround@,
  4 ndup _load4, _rqround, _store4, ;

\ ---MAIN API---

: round ( chacha -- )
  bi wstate | rounds 2/ 0 do [
    0 4 08 12 qround@,
    1 5 09 13 qround@,
    2 6 10 14 qround@,
    3 7 11 15 qround@,
    0 5 10 15 qround@,
    1 6 11 12 qround@,
    2 7 08 13 qround@,
    3 4 09 14 qround@,
    ] loop drop ;

code _statesum ( a a -- a a )
  64 i) R0>) @, begin
    PSP) A>) @,
    R0) &) A>) +,
    W) &) S>) @,
    R0) &) S>) +,
    S) -4 +) S>) @,
    A) -4 +) dir) S>) +,
  4 i) R0>) -, ?brnz,
  exit,

: blockfn ( chacha -- )
  dup round bi wstate | bstate ( a a )
  _statesum 2drop ;

:~ offsetof bstate + i) +, ;
code symkey 16 ~ exit,
\ Both counter and nonce access are merged to allow legacy and IETF support
code cntrnonce 48 ~ exit,

: _incrcounter ( chacha -- )
  cntrnonce bi le@ 1+ | le! ;

: counter! cntrnonce le! ;

: _loadstate ( chacha -- chacha )
  dupbi bstate | wstate 64 cmove ;

: _lemove ( src dst u -- )
  swap >r r! 4* 0 do dup i + @ V1 i + le! 4 +loop r> 0 fill rdrop ;

: setup ( an ak chacha -- )
  tuck symkey 8 _lemove
  dup variant case
    IETF = of cntrnonce 4+ 3 _lemove endof
    Legacy = of
      dup cntrnonce 4+ 1 0 fill
      cntrnonce 8+ 2 _lemove endof
    XChaCha = of
      2dup cntrnonce 4 _lemove
      dup _loadstate round \ hchacha round
      dupbi wstate 00 + | bstate 16 + 4 _lemove
      dupbi wstate 48 + | bstate 32 + 4 _lemove
      dip 16 + | tuck cntrnonce 8+ 2 _lemove
      dup cntrnonce 4+ 0 swap ! \ prefix the nonce with 4 NUL bytes
      1 swap counter!
     endof
    drop abort"Unsupported variant"
  endcase ;

: _writebuf ( a n stream -- written-n )
  nip nip drop 0 ;

code _xorloop ( a rn st wst cnt -- a rn st )
  W) &) R0>) @, drop,
  PSP) 8 +) A>) @,
  begin
    W) 8b) S>) @+,
    A) 8b) dir) S>) ^,
    1 i) A>) +,
  1 i) R0>) -, ?brnz,
  drop, exit,

: _readbuf ( n stream -- a? read-n )
  tuck wrappedstream readbuf
  dup 0 = if nip then 2dup 2>r rot ( a read-n st )

  \ no blockfn because we reuse the one generated before
  dup remaining ?dup if
    oover min ( a read-n st min )
    >r swap r@ - swap
    dupbi wstate | remaining 64 swap- + r@ _xorloop
    r@ over doto remaining swap- |
    dup remaining not if dup _incrcounter then
    rot r> + rot>
  then ( a read-n st )

  over 6 rshift 0 do
    dup _loadstate blockfn
    dup wstate 64 _xorloop
    rot 64 + rot>
    dup _incrcounter
  loop ( a read-n st )

  over 63 and ?dup if ( a read-n st read-n )
    dup 64 swap- oover to remaining
    over _loadstate blockfn
    dip dup wstate | _xorloop
  then

  ( a read-n st )
  nip 2drop 2r> ;

: _close ( stream -- )
  r! wstate 16 0 fill
  r@ bstate 16 + 12 0 fill
  0 r@ to remaining
  1 r@ counter!
  rdrop ;

: newChaCha ( rounds variant -- chacha )
  alignhere ChaCha typesz allot@
  tuck to variant
  tuck to rounds
  1 over counter!
  0 over to remaining
  0 over to wrappedstream
  CHACHA_CST over bstate 16 cmove
  ['] _writebuf over to writebuf
  ['] _readbuf over to readbuf
  ['] _close over to close ;

: newChaCha20 ( -- chacha )
  20 IETF newChaCha ;

\ Annotations for language interoperability
annotate ( -- *ChaCha ) newChaCha20
annotate ( uint uint -- *ChaCha ) newChaCha
annotate ( *void *void *ChaCha -- ) setup
annotate ( uint *ChaCha -- ) counter!
