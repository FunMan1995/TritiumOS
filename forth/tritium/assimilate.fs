\ Assimilate stub — collective neural puzzle + simti/ASIM (no real crypto / fleet)
\ TritiumOS.txt §5b.2–5b.3 : assimilate-epoch assimilate-fragment
\   assimilate-merge! assimilate-solved? assimilate-balance assimilate-demo
\ Persist path is host-side (evolve/assimilate/); this module keeps in-memory wallet.

\ 1 ASIM = 10^8 simti (Bitcoin : satoshi scale)
100000000 constant SIMTI-PER-ASIM
\ Default stub epoch pool (dev): 10^6 simti
1000000 constant ASIM-STUB-POOL
\ epsilon below this → epoch solved
10 constant ASIM-SOLVE-EPS

variable asim-epoch-id   1 asim-epoch-id !
variable asim-epsilon    1000 asim-epsilon !   \ stub global error ε
variable asim-wallet     0 asim-wallet !       \ local balance in simti
variable asim-pool       ASIM-STUB-POOL asim-pool !
variable asim-frag-next  1 asim-frag-next !
variable asim-last-delta 0 asim-last-delta !
variable _asim-tmp

\ Anti-gaming stub: remember last few proof hashes (duplicate → zero credit)
8 constant ASIM-PROOF-MAX
create asim-proofs  ASIM-PROOF-MAX cells allot
variable asim-proof-n  0 asim-proof-n !

: asim-proof-seen? ( proof -- flag )
  asim-proof-n @ 0 ?do
    i cells asim-proofs + @ over = if drop -1 unloop exit then
  loop drop 0 ;

: asim-proof-remember ( proof -- )
  asim-proof-n @ ASIM-PROOF-MAX >= if drop exit then
  asim-proof-n @ cells asim-proofs + !
  1 asim-proof-n +! ;

: assimilate-epoch ( -- id ) asim-epoch-id @ ;

\ assimilate-fragment ( group links -- frag )
\ Stub: fold group xor links into a fragment id (no real graph merge).
: assimilate-fragment ( group links -- frag )
  xor asim-frag-next @ xor
  1 asim-frag-next +!
  dup ." [ASSIMILATE] fragment=" . ." (group⊕links stub)" cr ;

\ Stub Δε: (frag xor proof) mod 64 + 1 — positive on first merge
: (asim-delta) ( frag proof -- delta )
  xor $3f and 1+ ;

\ Credit: floor( pool × delta / (delta + epsilon_before) ) capped by pool
: (asim-credit) ( delta eps-before -- simti )
  over 0<= if 2drop 0 exit then
  over +                      \ delta  (delta+eps)
  swap asim-pool @ *          \ (d+e)  (pool*delta)
  swap /                      \ credit
  dup asim-pool @ > if drop asim-pool @ then
  dup 0< if drop 0 then ;

: assimilate-merge! ( frag proof -- delta-epsilon )
  \ duplicate proof-hash → zero credit (anti-gaming stub)
  dup asim-proof-seen? if
    2drop 0
    0 asim-last-delta !
    ." [ASSIMILATE] merge! duplicate proof-hash → delta=0 credit=0" cr
    0 exit then
  2dup (asim-delta)           \ frag proof delta
  _asim-tmp !                 \ frag proof   (delta in _asim-tmp)
  nip                         \ proof
  dup asim-proof-remember
  drop
  _asim-tmp @ asim-last-delta !
  asim-epsilon @              \ eps-before
  dup _asim-tmp @             \ eps  eps  delta
  - dup 0< if drop 0 then asim-epsilon !
                              \ eps-before
  _asim-tmp @ swap (asim-credit)   \ credit
  dup asim-wallet +!
  asim-pool @ over - dup 0< if drop 0 then asim-pool !
  ." [ASSIMILATE] merge! delta-eps=" _asim-tmp @ .
  ." credit-simti=" . cr
  _asim-tmp @ ;

: assimilate-solved? ( -- flag )
  asim-epsilon @ ASIM-SOLVE-EPS <= if
    ." [ASSIMILATE] solved? true (eps=" asim-epsilon @ . ." ) → new epoch" cr
    1 asim-epoch-id +!
    1000 asim-epsilon !
    ASIM-STUB-POOL asim-pool !
    0 asim-proof-n !
    -1
  else
    ." [ASSIMILATE] solved? false eps=" asim-epsilon @ . cr
    0
  then ;

: assimilate-balance ( -- )
  asim-wallet @
  dup SIMTI-PER-ASIM /          \ simti  asim
  swap SIMTI-PER-ASIM mod       \ asim  rem-simti
  ." [ASSIMILATE] balance epoch=" assimilate-epoch .
  ." ASIM=" swap . ." simti=" . cr ;

\ --- demo: fragment → merge → credit (standalone; may follow queue-prove!) ---
: assimilate-demo ( -- )
  ." [assimilate-demo] fragment → merge → credit (simti; no crypto)" cr
  1 asim-epoch-id !
  1000 asim-epsilon !
  0 asim-wallet !
  ASIM-STUB-POOL asim-pool !
  1 asim-frag-next !
  0 asim-proof-n !
  0 asim-last-delta !
  7 3 assimilate-fragment >r
  r@ $c0ffee assimilate-merge! drop
  asim-wallet @ 0= if
    ." [assimilate-demo] FAIL — expected simti credit" cr
    r> drop exit then
  \ duplicate proof → zero credit
  r@ $c0ffee assimilate-merge! drop
  assimilate-balance
  ." [assimilate-demo] OK — simti credited (evolve/assimilate/; no crypto)" cr
  r> drop ;

\ --- economy-wire-demo: queue-prove! → assimilate-merge! e2e (wave9 §5b.4) ---
\ Alias queue-assim-demo prints same OK marker.
: economy-wire-demo ( -- )
  ." [economy-wire] enqueue→pull→prove→assimilate-merge (local stub)" cr
  \ reset queue cue
  0 q-count !
  1 q-next-id !
  \ reset assimilate wallet/epoch/proofs
  1 asim-epoch-id !
  1000 asim-epsilon !
  0 asim-wallet !
  ASIM-STUB-POOL asim-pool !
  1 asim-frag-next !
  0 asim-proof-n !
  0 asim-last-delta !
  \ mint non-local job (why=OPTIN, local-ok=0)
  42 77 Q-WHY-OPTIN 0 queue-make >r   \ R: job-id
  r@ 0= if
    ." [economy-wire-demo] FAIL" cr r> drop exit then
  ." [economy-wire] enqueue job=" r@ . cr
  r@ queue-local? if
    ." [economy-wire-demo] FAIL" cr r> drop exit then
  queue-pull dup 0= if
    ." [economy-wire-demo] FAIL" cr r> drop exit then
  drop
  \ stub proof (nonzero)
  r@ $b0b0 queue-prove!               \ score
  dup 0<= if
    ." [economy-wire-demo] FAIL" cr drop r> drop exit then
  ." [economy-wire] prove score=" . cr
  \ fragment from job id ⊕ fixed links
  r@ 1 assimilate-fragment >r         \ R: job  frag
  r@ $b0b0 assimilate-merge!          \ delta
  dup 0<= if
    ." [economy-wire-demo] FAIL" cr drop r> drop r> drop exit then
  asim-wallet @ 0= if
    ." [economy-wire-demo] FAIL" cr drop r> drop r> drop exit then
  ." [economy-wire] merge delta=" . ." credit-ok" cr
  \ optional reward
  r> drop                             \ drop frag; R: job
  r@ queue-reward!
  \ duplicate proof still 0 credit
  r@ 1 assimilate-fragment $b0b0 assimilate-merge! drop
  ." [economy-wire-demo] OK" cr
  r> drop ;

: queue-assim-demo ( -- ) economy-wire-demo ;
