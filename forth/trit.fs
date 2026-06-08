\ Tritium trit arithmetic (−1, 0, +1) — stub vocabulary

: trit-table  -1 , 0 , 1 , ;

: decode-trit ( n -- t )
  3 mod trit-table + @ ;

: trit-pair@ ( nibble -- tlo thi )
  dup 3 mod decode-trit
  swap 3 / 3 mod decode-trit ;

: .trit ( t -- )
  dup -1 = if ." -1" exit then
  dup  0 = if ." 0" exit then
  ." +1" ;

: encode-trit ( t -- n )   \ -1→0, 0→1, +1→2
  dup -1 = if 0 exit then
  dup  0 = if 1 exit then
  2 ;

: trit-pair>nibble ( tlo thi -- nib )
  encode-trit 3 * swap encode-trit + ;

: pack-header ( s0 s1 s2 s3 -- h )
  12 lshift swap 8 lshift or swap 4 lshift or or ;

: unpack-header ( h -- s0 s1 s2 s3 )
  dup $f and 
  over  4 rshift $f and 
  over  8 rshift $f and 
  rot 12 rshift $f and ;

: s3-mode ( s3 -- m ) 3 and ;  \ 00 RANDOM, 01 FOLD, 10 CONNECTED, 11 RESERVED
: random-mode ( -- 0 ) 0 ;
: fold-mode   ( -- 1 ) 1 ;
: connected-mode ( -- 2 ) 2 ;
: reserved-mode ( -- 3 ) 3 ;