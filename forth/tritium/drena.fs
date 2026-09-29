\ D.R.E.N.A. — Trit Intelligence Engine
\ Neuron: header|id|link-count|connected-ids
\ Typed links (TritiumOS.txt §3.5 / docs/NEURON.md): src dst type w_lo w_hi s3-mode
\ S3: 0=RANDOM 1=ADDRESS_FOLD 2=CONNECTED 3=RESERVED (never auto-advanced; see docs/ASSUMPTIONS.md)

: header>s0 ( h -- s0 ) unpack-header drop drop drop ;
: header>s3 ( h -- s3 ) unpack-header >r drop drop drop r> ;
: header>mode ( h -- m ) header>s3 s3-mode ;

: set-s3-mode ( mode n-addr -- )
  >r dup 3 = if drop r> drop exit then
  r@ neuron-header unpack-header drop
  rot pack-neuron-header r> ! ;

: neuron-header ( n-addr -- h ) @ ;
: neuron-id ( n-addr -- id ) cell+ @ ;
: neuron-link-count ( n-addr -- n ) 2 cells + @ ;
: neuron-links-base ( n-addr -- addr ) 3 cells + ;

: .neuron-header ( h -- )
  unpack-header
  ." S3=" dup . ." (mode=" dup s3-mode dup . ."=" mode-name ." ) "
  drop ." S2=" . ." S1=" . ." S0=" . cr ;

: .neuron ( n-addr -- )
  dup ." Neuron@ " . cr
  dup neuron-header ."   header: " .neuron-header
  dup neuron-id ."   id: " . cr
  dup neuron-link-count dup ."   links(" . ." ): "
  >r neuron-links-base r> 0 ?do dup i cells + @ . loop drop cr ;

: valid-header? ( h -- f ) header>mode 0 3 within ;
: valid-neuron? ( n-addr -- f )
  dup neuron-header valid-header? swap neuron-link-count 0 256 within and ;
: validate-neuron ( n-addr -- )
  dup valid-neuron? 0= if ." INVALID NEURON! " .neuron abort then
  drop ." neuron stable & valid" cr ;

variable next-id  1 next-id !
variable last-grown  0 last-grown !  \ addr of last spawn/grow (for drena-step)

: make-neuron ( id mode -- n-addr )
  here >r swap >r
  0 0 0 rot pack-neuron-header ,
  r> ,  0 ,  r> ;

: neuron-add-connection ( connected-id n-addr -- )
  2dup neuron-link-count cells swap neuron-links-base + !
  dup neuron-link-count 1+ swap 2 cells + ! ;

\ === address-fold deepen (wave10 item 3 / docs/ADDRESS-FOLD.md) ===
\ Keep rounds=1 mix compatible with prior drena-link / grow-step goldens.

variable phi-fold-rounds
1 phi-fold-rounds !
variable last-fold-influence
0 last-fold-influence !
variable last-fold-target
0 last-fold-target !
variable _fold-src
variable _fold-cand

\ phi-fold ( addr addr' -- influence )
\   xor → (x^=x<<13; x^=x>>7) × rounds → x&=$7fff
: phi-fold ( addr addr' -- influence )
  xor
  phi-fold-rounds @ 1 max 0 ?do
    dup 13 lshift xor dup 7 rshift xor
  loop
  $7fff and
  dup last-fold-influence ! ;

\ phi-fold3 ( a b c -- influence ) = phi-fold(phi-fold(a,b),c)
: phi-fold3 ( a b c -- influence )
  >r phi-fold r> phi-fold ;

\ fold-target ( src-id candidate -- target )  deterministic; target ≥ 1
: fold-target ( src-id candidate -- target )
  _fold-cand !  _fold-src !
  _fold-src @ _fold-cand @ phi-fold
  ." [fold] phi a=" _fold-src @ . ." b=" _fold-cand @ . ." influence=" dup . cr
  1 max
  dup last-fold-target !
  ." [fold] target src=" _fold-src @ . ." cand=" _fold-cand @ . ." -> " dup . cr ;

\ fold-demo ( -- )  golden vectors rounds=1 → [fold-demo] OK
: fold-demo ( -- )
  ." [fold-demo] golden vectors rounds=1" cr
  1 phi-fold-rounds !
  1 2 fold-target
  dup 24771 <> last-fold-influence @ 24771 <> or if
    ." [fold-demo] FAIL" cr drop exit then drop
  7 11 fold-target
  dup 780 <> last-fold-influence @ 780 <> or if
    ." [fold-demo] FAIL" cr drop exit then drop
  100 100 fold-target
  dup 1 <> last-fold-influence @ 0 <> or if
    ." [fold-demo] FAIL" cr drop exit then drop
  ." [fold-demo] OK" cr ;

0 constant LINK-INTRA
1 constant LINK-INTER
2 constant LINK-FOLD-PHI
3 constant LINK-RANDOM

64 constant MAX-LINKS
6 constant LINK-CELLS
create links  MAX-LINKS LINK-CELLS * cells allot
variable link-count  0 link-count !

: link-rec ( idx -- addr ) LINK-CELLS * cells links + ;
: link@ ( idx -- rec ) link-rec ;
: link-src ( rec -- n ) @ ;
: link-dst ( rec -- n ) cell+ @ ;
: link-type@ ( rec -- n ) 2 cells + @ ;
: link-w-lo ( rec -- t ) 3 cells + @ ;
: link-w-hi ( rec -- t ) 4 cells + @ ;
: link-s3@ ( rec -- m ) 5 cells + @ ;

variable _lsrc  variable _ldst  variable _ltype  variable _lw

: link! ( src dst type w -- )
  link-count @ MAX-LINKS >= if 2drop 2drop ." [DRENA] link! full" cr exit then
  _lw !  _ltype !  _ldst !  _lsrc !
  link-count @ link-rec >r
  _lsrc @ r@ !
  _ldst @ r@ cell+ !
  _ltype @ r@ 2 cells + !
  _lw @ trit-pair@                 \ w_lo w_hi
  swap r@ 3 cells + !              \ w_lo
  r@ 4 cells + !                   \ w_hi
  0 r@ 5 cells + !                 \ s3-mode default; set after if known
  r> drop
  1 link-count +!
  ." [DRENA] link! " _lsrc @ . ." -> " _ldst @ . ." type=" _ltype @ . cr ;

\ links-for-neuron: ( neuron -- addr count )
\ addr = links table base; count = number of typed records with src = neuron-id
: links-for-neuron ( neuron -- addr count )
  neuron-id >r  0
  link-count @ 0 ?do
    i link-rec link-src r@ = if 1+ then
  loop
  r> drop  links swap ;

: .link ( idx -- )
  dup link@ >r
  ." link[" . ." ] src=" r@ link-src .
  ." dst=" r@ link-dst .
  ." type=" r@ link-type@ .
  ." w=(" r@ link-w-lo .trit ." ," r@ link-w-hi .trit ." )"
  ." s3=" r> link-s3@ . cr ;

\ --- labeled groups (members + GROUP-<label>/ searchable vocab unit) ---
16 constant MAX-GROUPS
32 constant MAX-MEMBERS
48 constant VOCAB-PFX-SZ
create group-labels  MAX-GROUPS 32 * allot
create group-member-tbl  MAX-GROUPS MAX-MEMBERS * cells allot
create group-n-members   MAX-GROUPS cells allot
create group-vocab-prefixes  MAX-GROUPS VOCAB-PFX-SZ * allot
variable group-count  0 group-count !
variable _gid
variable _nid
variable _gpfxd  variable _gpca  variable _gpu

: group-label-addr ( gid -- c-addr ) 32 * group-labels + ;
: group-member-base ( gid -- addr ) MAX-MEMBERS * cells group-member-tbl + ;
: group-member-count ( gid -- n ) cells group-n-members + @ ;
: group-members ( gid -- addr count )
  dup group-member-base swap group-member-count ;
: group-vocab-prefix-addr ( gid -- c-addr ) VOCAB-PFX-SZ * group-vocab-prefixes + ;

\ Build counted string GROUP-<label>/ and mount as searchable vocab unit (Dusk-style).
\ Keeps the prefix string for display/persist; also registers it in the kernel dict under gid.
: group-register-vocab-unit ( gid -- )
  dup >r
  group-vocab-prefix r@ group-entry-create drop
  ." [DRENA] vocab unit " r> group-vocab-prefix type ."  (searchable)" cr ;

: group-set-vocab-prefix ( gid -- )
  dup group-vocab-prefix-addr _gpfxd !
  dup >r
  group-label-addr count  _gpu !  _gpca !
  _gpu @ 7 + 47 min  _gpfxd @ c!
  _gpfxd @ 1+
  dup [char] G swap c! 1+
  dup [char] R swap c! 1+
  dup [char] O swap c! 1+
  dup [char] U swap c! 1+
  dup [char] P swap c! 1+
  dup [char] - swap c! 1+
  _gpca @ over _gpu @ cmove
  _gpu @ +  [char] / swap c!
  ." [DRENA] vocab prefix " _gpfxd @ count type cr
  r> group-register-vocab-unit ;

: group-vocab-prefix ( gid -- c-addr u )
  group-vocab-prefix-addr count ;

\ group-vocab-add ( c-addr u gid -- )  register a word under the group's vocab unit
: group-vocab-add ( c-addr u gid -- )
  dup >r
  group-entry-create dup 0< if
    drop r> drop ." [DRENA] group-vocab-add failed" cr exit
  then
  ." [DRENA] group-vocab-add #" . ." under gid=" r> . cr ;

\ group-find ( c-addr u gid -- i )  scoped find under GROUP-<label>/ unit
: group-find ( c-addr u gid -- i ) group-entry-find ;

\ --- nested search-order + caps (wave7 item 4 / docs/GROUPS-NESTED.md) ---
\ Walk active gid then LINK-INTER neighbors; max 4 hops / 8 gids; first hit; miss -1.
4 constant NEST-MAX-HOPS
8 constant NEST-MAX-GIDS
create nest-order   NEST-MAX-GIDS cells allot
create nest-depth   NEST-MAX-GIDS cells allot
variable nest-order-n
variable _nwalk-gid  variable _nwalk-d

: (gid-mark?) ( id -- f ) $80000000 and 0<> ;
: (unmark-gid) ( id -- gid ) $7fffffff and ;

: (nid-in-group?) ( nid gid -- f )
  group-members 0 ?do
    dup i cells + @  2 pick = if 2drop -1 unloop exit then
  loop
  2drop 0 ;

: (id->gid) ( id -- gid | -1 )
  dup (gid-mark?) if (unmark-gid) exit then
  group-count @ 0 ?do
    dup i (nid-in-group?) if drop i unloop exit then
  loop
  drop -1 ;

: (nest-has?) ( gid -- f )
  nest-order-n @ 0 ?do
    dup nest-order i cells + @ = if drop -1 unloop exit then
  loop
  drop 0 ;

: (nest-push) ( gid depth -- )
  nest-order-n @ NEST-MAX-GIDS >= if 2drop exit then
  over (nest-has?) if 2drop exit then
  over 0< if 2drop exit then
  nest-order-n @ cells nest-depth + !
  nest-order-n @ cells nest-order + !
  1 nest-order-n +! ;

\ Collect LINK-INTER neighbor gids of _nwalk-gid at depth _nwalk-d+1
: (nest-expand) ( -- )
  link-count @ 0 ?do
    i link-rec link-type@ LINK-INTER = if
      i link-rec link-src (id->gid)
      i link-rec link-dst (id->gid)   \ ga gb
      2dup 0< swap 0< or if 2drop
      else
        over _nwalk-gid @ = if         \ ga == walk → neighbor gb
          nip _nwalk-d @ 1+ (nest-push)
        else
          dup _nwalk-gid @ = if        \ gb == walk → neighbor ga
            drop _nwalk-d @ 1+ (nest-push)
          else
            2drop
          then
        then
      then
    then
  loop ;

: group-search-order ( gid -- addr count )
  0 nest-order-n !
  0 (nest-push)
  0
  begin
    dup nest-order-n @ <
  while
    dup cells nest-depth + @ NEST-MAX-HOPS >= if
      \ skip expand past hop cap
    else
      dup cells nest-order + @ _nwalk-gid !
      dup cells nest-depth + @ _nwalk-d !
      (nest-expand)
    then
    1+
  repeat
  drop
  nest-order nest-order-n @ ;

variable _gnf-ca  variable _gnf-u

: group-find-nested ( c-addr u gid -- i )
  _gnf-u !  _gnf-ca !
  group-search-order 0 ?do
    _gnf-ca @ _gnf-u @  nest-order i cells + @  group-find
    dup 0< 0= if nip unloop exit then
    drop
  loop
  drop -1 ;


: group-label! ( c-addr u gid -- )
  dup _gid !
  group-label-addr place
  ." [DRENA] group-label! gid=" _gid @ . ." -> "
  _gid @ group-label-addr count type cr
  _gid @ group-set-vocab-prefix ;

: (drena-group) ( c-addr u -- group-id )
  group-count @ MAX-GROUPS >= if 2drop -1 exit then
  group-count @ >r
  0 r@ cells group-n-members + !
  r@ group-label!
  1 group-count +!
  ." [DRENA] group id=" r@ . cr
  r> ;

\ Accept counted-string address (label-addr -- group-id)
: drena-group ( label-addr -- group-id ) count (drena-group) ;

\ True if nid already in group's member list
: (group-has-member?) ( nid gid -- f )
  group-members 0 ?do
    dup i cells + @  2 pick = if 2drop -1 unloop exit then
  loop
  2drop 0 ;

: drena-join ( neuron group -- )
  swap neuron-id  _nid !  _gid !
  _gid @ group-member-count MAX-MEMBERS >= if
    ." [DRENA] join full group " _gid @ . cr exit then
  _nid @ _gid @ (group-has-member?) if
    ." [DRENA] join neuron " _nid @ . ." -> group " _gid @ .
    ." (members=" _gid @ group-member-count . ." already)" cr exit then
  _nid @  _gid @ group-member-base  _gid @ group-member-count cells +  !
  1  _gid @ cells group-n-members +  +!
  ." [DRENA] join neuron " _nid @ . ." -> group " _gid @ .
  ." (members=" _gid @ group-member-count . ." )" cr ;


\ group-link! ( group-a group-b -- )  inter-group bridge via LINK-INTER
\ Prefer representative members (first nid of each group). If a group has no
\ members yet, store a marked group-id edge: src/dst = $80000000 | gid so the
\ links table clearly flags a group-group bridge (TritiumOS.txt §3.5).
variable _gla  variable _glb

: (group-rep-or-marker) ( gid -- id )
  dup group-member-count 0> if
    group-member-base @
  else
    $80000000 or
  then ;

: group-link! ( group-a group-b -- )
  _glb !  _gla !
  _gla @ 0< _gla @ group-count @ >= or
  _glb @ 0< _glb @ group-count @ >= or or if
    ." [DRENA] group-link! bad gid" cr exit then
  _gla @ (group-rep-or-marker)
  _glb @ (group-rep-or-marker)
  LINK-INTER 4 link!
  ." [DRENA] group-link! " _gla @ . ." <-> " _glb @ .
  ." type=" LINK-INTER . ." (LINK-INTER)" cr ;

: .group ( gid -- )
  dup ." [DRENA] .group gid=" . cr
  dup ."   label=" group-label-addr count type cr
  dup ."   prefix=" group-vocab-prefix type cr
  dup ."   members(" group-member-count . ." ): "
  group-members 0 ?do dup i cells + @ . loop drop cr ;

\ Edition id width (wave3 item 5): 32 → $ffffffff mask; 64 → full cell
\ Depends on kernel edition@ / 32bit? (loaded before drena).
: id-width ( -- bits ) 32bit? if 32 else 64 then ;
: id-mask ( -- mask ) 32bit? if $ffffffff else -1 then ;
: id-clamp ( n -- n' ) id-mask and ;

: drena-spawn ( variation -- neuron )
  next-id @ id-clamp dup 1+ id-clamp next-id !
  swap make-neuron
  dup validate-neuron
  dup last-grown !
  dup ." [DRENA] spawned neuron id=" neuron-id
  ." mode=" dup neuron-header header>mode mode-name
  ." (id-width=" id-width . ." )" cr ;

variable _dtype
: drena-link ( src-neuron dst-id -- )
  over >r
  LINK-INTRA _dtype !
  r@ neuron-header header>mode
  dup 1 = if
    drop
    r@ neuron-id over fold-target
    ." [DRENA] ADDRESS_FOLD φ(" r@ neuron-id . ." ," over . ." )->" dup . cr
    nip
    LINK-FOLD-PHI _dtype !
  else
    0 = if LINK-RANDOM _dtype ! then
  then
  dup r@ neuron-add-connection
  ." [DRENA] linked " r@ neuron-id . ." -> " dup . cr
  \ also append typed record: src dst type w=0 (neutral trit pair nibble 4 = 0,0 encoded as 1*3+1=4)
  r@ neuron-id swap _dtype @ 4 link!
  r@ neuron-header header>mode
  link-count @ 1- link-rec 5 cells + !   \ stamp s3-mode on last link
  r> drop ;

: drena-rewire ( neuron -- )
  dup neuron-header header>mode
  dup 3 = if drop ." [DRENA] rewire skipped (S3 RESERVED)" cr drop exit then
  dup 2 >= if drop ." [DRENA] rewire already CONNECTED" cr drop exit then
  1+
  2dup swap set-s3-mode
  ." [DRENA] rewire S3 -> " mode-name ." (header written)" cr
  drop ;

\ drena-grow: recursive expansion — child inherits parent S3 mode; link parent→child.
\ Never calls rewire / never advances RESERVED (RESERVED parent → RESERVED child).
: drena-grow ( parent -- child )
  dup >r
  r@ neuron-header header>mode
  drena-spawn                    \ child; last-grown updated
  dup neuron-id r@ swap drena-link
  ." [DRENA] grow parent=" r@ neuron-id . ." -> child=" dup neuron-id .
  ." mode=" dup neuron-header header>mode mode-name cr
  r> drop ;

\ drena-step: one evolution tick (scheduler). Tracks via last-grown.
\ no neuron → spawn mode 0; RESERVED → skip; CONNECTED (mode≥2) → grow; else rewire.
: drena-step ( -- )
  last-grown @ 0= if
    0 drena-spawn drop
    exit
  then
  last-grown @ neuron-header header>mode
  dup 3 = if
    drop ." [DRENA] step skipped (S3 RESERVED)" cr exit
  then
  dup 2 >= if
    drop last-grown @ drena-grow drop exit
  then
  drop
  last-grown @ drena-rewire ;

: .neuron-graph ( n-addr -- )
  dup .neuron
  dup neuron-link-count 0 ?do
    i cells over neuron-links-base + @ ."   connected-to: " . cr
  loop drop ;


\ --- persist graph (evolve/user-graph.trit) ---
\ Host implements platform-graph-save / platform-graph-load
defer platform-graph-save
defer platform-graph-load
: (noop-graph-save) ;
: (noop-graph-load) ;
' (noop-graph-save) is platform-graph-save
' (noop-graph-load) is platform-graph-load

\ Serialize typed links + next-id into host evolve (Linux writes the file)
: graph-save ( -- )
  platform-graph-save
  ." [DRENA] graph-save -> evolve/user-graph.trit" cr ;

: graph-load ( -- )
  platform-graph-load
  ." [DRENA] graph-load <- evolve/user-graph.trit" cr ;

: drena-init ( -- )
  1 next-id !  0 link-count !  0 group-count !  0 last-grown !
  1 phi-fold-rounds !  0 last-fold-influence !  0 last-fold-target !
  MAX-GROUPS 0 ?do 0 i cells group-n-members + ! loop
  ." [DRENA] Trit intelligence engine initialized" cr ;

drena-init
." Trit intelligence engine (DRENA data blocks) loaded. Ready for neuromorphic compute." cr
