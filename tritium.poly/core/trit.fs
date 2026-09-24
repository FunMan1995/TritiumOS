\ Tritium trit arithmetic (−1, 0, +1) — Phase 2 vocabulary
\ Spec: TritiumOS.txt §3 + Phase 2 (trit+, trit*, decode-trit, trit-pair@, pack-neuron-header)

: trit-table  -1 , 0 , 1 , ;

: decode-trit ( n -- t )
  3 mod trit-table + @ ;

\ Balanced ternary digit add (result stays in {-1,0,+1})
: trit+ ( t1 t2 -- t3 )
  + dup  2 = if drop -1 exit then
  dup -2 = if drop  1 exit then ;

\ Product of two trits stays in {-1,0,+1}
: trit* ( t1 t2 -- t3 ) * ;

: trit-pair@ ( nibble -- tlo thi )
  dup 3 mod decode-trit
  swap 3 / 3 mod decode-trit ;

: .trit ( t -- )
  dup -1 = if ." -1" exit then
  dup  0 = if ." 0" exit then
  ." +1" ;

: encode-trit ( t -- n )   \ -1→0, 0→1, +1→2
  dup -1 = if drop 0 exit then
  dup  0 = if drop 1 exit then
  drop 2 ;

: trit-pair>nibble ( tlo thi -- nib )
  encode-trit 3 * swap encode-trit + ;

\ Pack 4×4-bit sections into 16-bit header (S0|S1|S2|S3). Spec alias: pack-neuron-header.
: pack-header ( s0 s1 s2 s3 -- h )
  12 lshift swap 8 lshift or swap 4 lshift or or ;

: pack-neuron-header ( s0 s1 s2 s3 -- h ) pack-header ;

: unpack-header ( h -- s0 s1 s2 s3 )
  >r
  r@ $f and
  r@ 4 rshift $f and
  r@ 8 rshift $f and
  r> 12 rshift $f and ;

: s3-mode ( s3 -- m ) 3 and ;  \ 00 RANDOM, 01 ADDRESS_FOLD, 10 CONNECTED, 11 RESERVED
: random-mode ( -- 0 ) 0 ;
: fold-mode   ( -- 1 ) 1 ;
: connected-mode ( -- 2 ) 2 ;
: reserved-mode ( -- 3 ) 3 ;

: mode-name ( m -- )
  dup 0 = if drop ." RANDOM" exit then
  dup 1 = if drop ." ADDRESS_FOLD" exit then
  dup 2 = if drop ." CONNECTED" exit then
  drop ." RESERVED" ;
