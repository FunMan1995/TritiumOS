\ R.E.K.I.A. — Pure-math refiner → runnable Forth (TritiumOS.txt §4)
\ Acceptance: rekiA-to-forth writes evolve/forth/refined/<label>.fs then include.
\ Hosts without file I/O implement platform-write-refined / platform-include-refined.

: contract-trit ( t influence -- t' )
  swap dup 0 = if nip exit then
  swap 0 > if
    dup 0 > if 1- else dup 0 < if 1+ then then
  else drop then ;

: contract-nibble ( nib influence -- nib' )
  >r trit-pair@
  r@ contract-trit swap r@ contract-trit swap
  r> drop
  trit-pair>nibble ;

: extract-trit-signature ( n-addr -- s0 s1 s2 )
  neuron-header unpack-header drop ;

: rekiA-extract ( n-addr -- s0 s1 s2 influence )
  dup >r
  r@ extract-trit-signature
  r> neuron-link-count ;

: rekiA-one-step ( s0 s1 s2 influence -- s0' s1' s2' influence )
  >r
  r@ contract-nibble
  r@ contract-nibble
  r@ contract-nibble
  r> ;

: rekiA-contract ( s0 s1 s2 influence -- s0' s1' s2' )
  4 0 do rekiA-one-step loop drop ;

\ Host hooks (no-ops in pure Forth; Linux C host performs real write+include)
defer platform-write-refined
defer platform-include-refined
: (noop-write) 2drop 2drop ;
: (noop-include) 2drop ;
' (noop-write) is platform-write-refined
' (noop-include) is platform-include-refined

create refined-src 160 allot
create refined-path 96 allot
create name-buf 32 allot

\ Append unsigned number as decimal digits onto counted string at addr
: append-num ( n addr -- )
  swap s>d <# #s #> ( addr c-addr u )
  rot +place ;

: rekiA-to-forth ( s0 s1 s2 id -- )
  >r
  decode-trit swap decode-trit + swap decode-trit +   ( value )
  \ path evolve/forth/refined/refined-<id>.fs
  s" evolve/forth/refined/refined-" refined-path place
  r@ refined-path append-num
  s" .fs" refined-path +place
  \ source line
  s" : refined-" refined-src place
  r@ refined-src append-num
  s"  ( -- n ) " refined-src +place
  dup refined-src append-num
  s"  ;" refined-src +place
  refined-src count type cr
  refined-src count refined-path count platform-write-refined
  refined-path count platform-include-refined
  ." [REKIA] wrote+include " refined-path count type cr
  drop r> drop ;

: rekiA-refine ( neuron-addr -- )
  dup >r
  r@ validate-neuron
  r@ rekiA-extract
  rekiA-contract
  r@ neuron-id
  rekiA-to-forth
  ." [REKIA] refine complete" cr
  r> drop ;

: rekia-demo ( -- )
  ." [rekia-demo] spawn→rewire→link(φ)→refine" cr
  0 drena-spawn >r
  r@ drena-rewire
  99 r@ drena-link
  r@ drena-rewire
  r@ rekiA-refine
  r> drop
  ." [rekia-demo] done — refined word should be live vocab" cr ;

: rekiA-demo ( -- ) rekia-demo ;

." R.E.K.I.A. refiner math engine loaded. Pure math -> Forth." cr
