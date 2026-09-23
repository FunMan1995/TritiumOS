\ D.R.E.N.A. — Trit Intelligence Engine (Dynamic Recursive Evolving Neural Architecture)
\ Neuron layout: header(S0|S1|S2|S3) | id | link-count | connected ids…
\ S3 low 2 bits: 0=RANDOM 1=ADDRESS_FOLD 2=CONNECTED 3=RESERVED (leave alone)
\ Research tip: drena-rewire writes advanced S3; ADDRESS_FOLD uses φ(addr,addr')

\ === Header helpers (unpack-header → s0 s1 s2 s3 with s3 on top) ===
: header>s0 ( h -- s0 )
  unpack-header drop drop drop ;
: header>s3 ( h -- s3 )
  unpack-header >r drop drop drop r> ;
: header>mode ( h -- m ) header>s3 s3-mode ;

\ Rewrite S3 mode bits; preserve S0..S2. Does NOT touch RESERVED (3).
: set-s3-mode ( mode n-addr -- )
  >r
  dup 3 = if drop r> drop exit then   \ leave S3=11 alone
  r@ neuron-header unpack-header drop  ( mode s0 s1 s2 )
  rot pack-neuron-header               ( h )
  r> ! ;

\ === Neuron record ===
: neuron-header ( n-addr -- h ) @ ;
: neuron-id ( n-addr -- id ) cell+ @ ;
: neuron-link-count ( n-addr -- n ) 2 cells + @ ;
: neuron-links-base ( n-addr -- addr ) 3 cells + ;

: .neuron-header ( h -- )
  unpack-header ( s0 s1 s2 s3 )
  ." S3=" dup . ." (mode=" dup s3-mode dup . ."=" mode-name ." ) "
  drop
  ." S2=" . ." S1=" . ." S0=" . cr ;

: .neuron ( n-addr -- )
  dup ." Neuron@ " . cr
  dup neuron-header ."   header: " .neuron-header
  dup neuron-id ."   id: " . cr
  dup neuron-link-count dup ."   links(" . ." ): "
  >r neuron-links-base
  r> 0 ?do dup i cells + @ . loop drop cr ;

: valid-trit? ( t -- f ) dup -1 = over 0 = or swap 1 = or ;
: valid-header? ( h -- f )
  header>mode 0 3 within ;
: valid-neuron? ( n-addr -- f )
  dup neuron-header valid-header? swap neuron-link-count 0 256 within and ;
: validate-neuron ( n-addr -- )
  dup valid-neuron? 0= if ." INVALID NEURON! " .neuron abort then
  drop ." neuron stable & valid" cr ;

variable next-id  1 next-id !

: make-neuron ( id mode -- n-addr )
  here >r
  swap                         ( mode id )
  >r                           ( mode | id )
  0 0 0 rot                    ( s0 s1 s2 mode )
  pack-neuron-header ,         \ header
  r> ,                         \ id
  0 ,                          \ link-count
  r> ;

: neuron-add-connection ( connected-id n-addr -- )
  2dup neuron-link-count cells swap neuron-links-base + !
  dup neuron-link-count 1+ swap 2 cells + ! ;

\ φ(addr, addr') — pure-math mix for ADDRESS_FOLD target pick (TritiumOS.txt §3.2)
: phi-fold ( addr addr' -- influence )
  xor dup 13 lshift xor dup 7 rshift xor $7fff and ;

: fold-target ( src-id candidate -- target )
  2dup phi-fold nip 1 max ;   \ deterministic remap; keep ≥1

: drena-spawn ( variation -- neuron )   \ variation = initial S3 mode
  next-id @ dup 1 next-id +!
  swap make-neuron
  dup validate-neuron
  dup ." [DRENA] spawned neuron id=" neuron-id
  ." mode=" dup neuron-header header>mode mode-name cr ;

\ Link: when S3=ADDRESS_FOLD, pick target via φ(src,dst); CONNECTED uses dst as-is.
: drena-link ( src-neuron dst-id -- )
  over >r
  r@ neuron-header header>mode
  1 = if                       \ ADDRESS_FOLD
    r@ neuron-id over fold-target
    ." [DRENA] ADDRESS_FOLD φ(" r@ neuron-id . ." ," over . ." )->" dup . cr
    nip
  then
  r@ neuron-add-connection
  ." [DRENA] linked " r@ neuron-id . ." -> " . cr
  r> drop ;

\ Advance S3: RANDOM → ADDRESS_FOLD → CONNECTED. Leave RESERVED alone.
: drena-rewire ( neuron -- )
  dup neuron-header header>mode
  dup 3 = if drop ." [DRENA] rewire skipped (S3 RESERVED)" cr drop exit then
  dup 2 >= if drop ." [DRENA] rewire already CONNECTED" cr drop exit then
  1+                          ( neuron new-mode )
  2dup swap set-s3-mode
  ." [DRENA] rewire S3 -> " mode-name
  ." (header written)" cr
  drop ;

: drena-group ( label-addr -- group-id )
  ." [DRENA] group: " type cr 42 ;
: drena-join ( neuron group -- )
  ." [DRENA] join neuron to group " . . cr ;

: .neuron-graph ( n-addr -- )
  dup .neuron
  dup neuron-link-count 0 ?do
    i cells over neuron-links-base + @ ."   connected-to: " . cr
  loop drop ;

: drena-init ( -- )
  1 next-id !
  ." [DRENA] Trit intelligence engine initialized" cr ;

drena-init
." Trit intelligence engine (DRENA data blocks) loaded. Ready for neuromorphic compute." cr
