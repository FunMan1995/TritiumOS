\ R.E.K.I.A. — Pure-math refiner → runnable Forth (TritiumOS.txt §4 / docs/REKIA.md)
\ rekiA-to-forth writes evolve/forth/refined/<label>.fs then include via host hooks.

: contract-trit ( t influence -- t' )
  swap dup 0 = if nip exit then
  swap 0 > if
    dup 0 > if 1- else dup 0 < if 1+ then then
  else drop then ;

: contract-nibble ( nib influence -- nib' )
  >r trit-pair@
  r@ contract-trit swap r@ contract-trit swap
  r> drop trit-pair>nibble ;

: extract-trit-signature ( n-addr -- s0 s1 s2 )
  neuron-header unpack-header drop ;

: rekiA-extract ( n-addr -- s0 s1 s2 influence )
  dup >r r@ extract-trit-signature r> neuron-link-count ;

: rekiA-one-step ( s0 s1 s2 influence -- s0' s1' s2' influence )
  >r r@ contract-nibble r@ contract-nibble r@ contract-nibble r> ;

: rekiA-contract ( s0 s1 s2 influence -- s0' s1' s2' )
  4 0 do rekiA-one-step loop drop ;

defer platform-write-refined
defer platform-include-refined
: (noop-write) 2drop 2drop ;
: (noop-include) 2drop ;
' (noop-write) is platform-write-refined
' (noop-include) is platform-include-refined

create refined-src 160 allot
create refined-path 96 allot
create label-buf 32 allot
variable _s0  variable _s1  variable _s2

: append-num ( n addr -- )
  swap s>d <# #s #> rot +place ;

: rekiA-label-group ( s0 s1 s2 -- c-addr u )
  + +
  dup 0 = if drop s" stable-core" exit then
  0 > if s" positive-flow" else s" negative-drift" then ;

: rekiA-to-forth ( s0 s1 s2 id -- )
  >r
  decode-trit swap decode-trit + swap decode-trit +
  s" evolve/forth/refined/refined-" refined-path place
  r@ refined-path append-num
  s" .fs" refined-path +place
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


defer platform-assistant-state!
: (noop-state!) ;
' (noop-state!) is platform-assistant-state!

: assistant-state! ( -- )
  platform-assistant-state!
  ." [REKIA] assistant-state! -> evolve/assistant-state.trit" cr ;

: rekiA-refine ( neuron-addr -- )
  dup >r
  r@ validate-neuron
  r@ rekiA-extract
  rekiA-contract
  _s2 ! _s1 ! _s0 !
  _s0 @ _s1 @ _s2 @ rekiA-label-group
  label-buf place
  label-buf drena-group drop
  _s0 @ _s1 @ _s2 @ r@ neuron-id
  rekiA-to-forth
  ." [REKIA] refine complete (label+file)" cr
  graph-save
  assistant-state!
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

: s3-reserved-demo ( -- )
  ." [s3-reserved-demo] spawn mode=3 (RESERVED) then rewire — must stay 3" cr
  3 drena-spawn >r
  r@ neuron-header header>mode ." before=" . cr
  r@ drena-rewire
  r@ neuron-header header>mode ." after=" . cr
  r> drop ;



: grow-step-demo ( -- )
  ." [grow-step-demo] spawn0→grow→step; RESERVED→grow child→step skipped" cr
  0 drena-spawn >r
  r@ ." [grow-step-demo] parent0 id=" neuron-id . cr
  r@ drena-grow >r
  r@ ." [grow-step-demo] child0 id=" neuron-id .
  ." mode=" r@ neuron-header header>mode . cr
  ." [grow-step-demo] step (child mode0 → rewire)..." cr
  drena-step
  r@ neuron-header header>mode ." after-step mode=" . cr
  r> drop r> drop
  ." [grow-step-demo] RESERVED path:" cr
  3 drena-spawn >r
  r@ neuron-header header>mode ." before=" . cr
  r@ drena-grow >r
  r@ neuron-header header>mode ." child-mode=" . cr
  ." [grow-step-demo] step on RESERVED child..." cr
  drena-step
  r@ neuron-header header>mode ." after=" . cr
  r> drop r> drop
  ." [grow-step-demo] done — grow+step OK; RESERVED child mode=3; step skipped" cr ;

: persist-demo ( -- )
  ." [persist-demo] refine then persist graph+state" cr
  rekia-demo
  ." [persist-demo] wrote evolve/user-graph.trit + evolve/assistant-state.trit" cr ;

: graph-status ( -- )
  ." [graph-status] (host reports load of user-graph + refined)" cr
  platform-graph-load ;

." R.E.K.I.A. refiner math engine loaded. Pure math -> Forth." cr

