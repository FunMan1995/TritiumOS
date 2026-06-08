needs arch/core hal/instr num/math hal/vreg
needsasm
unit lib/bit
arch<< lib/bit.fs

code bitmask ( bit -- mask ) S) &) !, 1 i) @, S) &) <<, exit,

: bit? ( n bit -- f ) bitmask and bool ;
: bit1! ( n bit -- n ) bitmask or ;
: bit0! ( n bit -- n ) bitmask invand ;
: bitsplit ( n bit -- hi lo )
  2dup tuck rshift swap lshift ( n shift hi )
  rot> 32 swap- tuck lshift swap rshift ;

: bitflag 1 swap lshift const ;
: addr, W) &) ;
: do@, ( halop sz shift -- ) ?dup if i) >>, then pow2 1- i) &, drop ;
: do!, ( halop sz shift -- )
    over pow2 1- over lshift inv i) &, ( op sz shift )
    PSP) S>) @+,
    swap pow2 1- i) S>) &, ?dup if i) S>) <<, then
    S) &) |, drop ;
: bitfield ( shift sz "name" -- )
  swap 2dup ['] do!, bind> bind> ( sz shift set )
  rot> ['] do@, bind> bind>
  ['] addr, 0 0 getset, ;
code sex8 ( n -- n ) 24 i) <<, 24 i) signed) >>, exit,
code sex16 ( n -- n ) 16 i) <<, 16 i) signed) >>, exit,

:~ ( n n -- ) S) &) &nf, ifnz, swap i) |, [compile] then ;
: swapbits8,
  S) &) !, $ff inv i) &,
  $80 $01 ~ $40 $02 ~ $20 $04 ~ $10 $08 ~
  $08 $10 ~ $04 $20 ~ $02 $40 ~ $01 $80 ~ ;
code swapbits8 swapbits8, exit,
