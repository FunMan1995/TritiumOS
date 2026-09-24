\ Collective queue stub — local cue only (no fleet crypto / network)
\ TritiumOS.txt §5b.1 / Research: queue-local? queue-enqueue! queue-pull queue-prove!
\ Persist path is host-side (evolve/queue/); this module keeps an in-memory cue.

\ Status: pending | active | proved | rewarded
0 constant Q-PENDING
1 constant Q-ACTIVE
2 constant Q-PROVED
3 constant Q-REWARDED

\ local-failed-why codes (stub)
0 constant Q-WHY-OOM
1 constant Q-WHY-TIMEOUT
2 constant Q-WHY-ED32
3 constant Q-WHY-OPTIN

16 constant MAX-QUEUE
\ job record: id submitter payload why proof status local-ok
7 constant Q-CELLS
create q-jobs  MAX-QUEUE Q-CELLS * cells allot
variable q-count  0 q-count !
variable q-next-id  1 q-next-id !

: q-rec ( idx -- addr ) Q-CELLS * cells q-jobs + ;
: q-id@     ( rec -- n ) @ ;
: q-sub@    ( rec -- n ) cell+ @ ;
: q-pay@    ( rec -- n ) 2 cells + @ ;
: q-why@    ( rec -- n ) 3 cells + @ ;
: q-proof@  ( rec -- n ) 4 cells + @ ;
: q-status@ ( rec -- n ) 5 cells + @ ;
: q-local@  ( rec -- n ) 6 cells + @ ;

: q-id!     ( n rec -- ) ! ;
: q-sub!    ( n rec -- ) cell+ ! ;
: q-pay!    ( n rec -- ) 2 cells + ! ;
: q-why!    ( n rec -- ) 3 cells + ! ;
: q-proof!  ( n rec -- ) 4 cells + ! ;
: q-status! ( n rec -- ) 5 cells + ! ;
: q-local!  ( n rec -- ) 6 cells + ! ;

: q-find-id ( job-id -- idx|-1 )
  q-count @ 0 ?do
    i q-rec q-id@ over = if drop i unloop exit then
  loop drop -1 ;

\ queue-local? ( job -- flag )  can run on this host now?
: queue-local? ( job -- flag )
  q-find-id dup 0< if drop 0 exit then
  q-rec q-local@ 0= if 0 else -1 then ;

\ Internal: allocate slot, return rec addr (or 0 if full)
: (q-alloc) ( -- rec|0 )
  q-count @ MAX-QUEUE >= if 0 exit then
  q-count @ q-rec
  1 q-count +! ;

\ queue-enqueue! ( job -- )  — job is an id already filled, or 0 to mint
\ Stack form for stub mint: submitter payload why local-ok -- job-id
\ For id form: job-id -- (re-enqueue existing pending)
variable _qe-sub  variable _qe-pay  variable _qe-why  variable _qe-lok

: queue-make ( submitter payload why local-ok -- job-id )
  _qe-lok !  _qe-why !  _qe-pay !  _qe-sub !
  (q-alloc) dup 0= if
    drop ." [QUEUE] enqueue full" cr 0 exit then
  >r
  q-next-id @ dup 1+ q-next-id !  r@ q-id!
  _qe-sub @ r@ q-sub!
  _qe-pay @ r@ q-pay!
  _qe-why @ r@ q-why!
  0 r@ q-proof!
  Q-PENDING r@ q-status!
  _qe-lok @ r@ q-local!
  r> q-id@
  dup ." [QUEUE] enqueue! job=" . ." status=pending (local cue)" cr ;

: queue-enqueue! ( job -- )
  \ If job already exists, ensure pending; else treat TOS as payload-only mint
  dup q-find-id dup 0< if
    drop
    \ mint: submitter=1 payload=job why=OPTIN local-ok=0 (non-local)
    1 swap Q-WHY-OPTIN 0 queue-make drop
  else
    nip q-rec >r
    Q-PENDING r@ q-status!
    ." [QUEUE] enqueue! job=" r@ q-id@ . ." status=pending (local cue)" cr
    r> drop
  then ;

\ queue-pull ( -- job|false )  take next pending → active
: queue-pull ( -- job|0 )
  q-count @ 0 ?do
    i q-rec dup q-status@ Q-PENDING = if
      dup Q-ACTIVE swap q-status!
      q-id@
      dup ." [QUEUE] pull job=" . ." status=active" cr
      unloop exit
    then
    drop
  loop
  0 ." [QUEUE] pull empty" cr ;

\ queue-prove! ( job proof -- score )
: queue-prove! ( job proof -- score )
  swap q-find-id dup 0< if
    drop drop ." [QUEUE] prove! unknown job" cr 0 exit then
  q-rec >r
  r@ q-status@ Q-ACTIVE = 0= if
    r> drop drop ." [QUEUE] prove! not active" cr 0 exit then
  dup r@ q-proof!
  Q-PROVED r@ q-status!
  \ stub score: fold proof + job-id (no crypto)
  r@ q-id@ xor $ffff and 1 max
  ." [QUEUE] prove! job=" r@ q-id@ . ." score=" dup . cr
  r> drop ;

\ --- demo: force non-local → enqueue → pull → prove ---
: queue-demo ( -- )
  ." [queue-demo] enqueue non-local job (queue-local?=false), pull, prove" cr
  \ reset cue for clean smoke
  0 q-count !
  1 q-next-id !
  \ submitter=42 payload=99 why=TIMEOUT local-ok=0
  42 99 Q-WHY-TIMEOUT 0 queue-make >r
  r@ queue-local? if
    ." [queue-demo] FAIL — expected queue-local? false" cr
    r> drop exit
  else
    ." [queue-demo] queue-local? = false (forced non-local)" cr
  then
  \ already pending from queue-make; pull + prove
  queue-pull dup 0= if
    ." [queue-demo] FAIL — pull empty" cr r> drop exit then
  drop
  r@ $a5a5 queue-prove! drop
  ." [queue-demo] OK — local cue (evolve/queue/; no fleet crypto)" cr
  r> drop ;
