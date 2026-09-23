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

: make-neuron ( id mode -- n-addr )
  here >r swap >r
  0 0 0 rot pack-neuron-header ,
  r> ,  0 ,  r> ;

: neuron-add-connection ( connected-id n-addr -- )
  2dup neuron-link-count cells swap neuron-links-base + !
  dup neuron-link-count 1+ swap 2 cells + ! ;

: phi-fold ( addr addr' -- influence )
  xor dup 13 lshift xor dup 7 rshift xor $7fff and ;
: fold-target ( src-id candidate -- target )
  2dup phi-fold nip 1 max ;

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

\ --- labeled groups ---
16 constant MAX-GROUPS
create group-labels  MAX-GROUPS 32 * allot
variable group-count  0 group-count !
variable _gid

: group-label-addr ( gid -- c-addr ) 32 * group-labels + ;

: group-label! ( c-addr u gid -- )
  dup _gid !
  group-label-addr place
  ." [DRENA] group-label! gid=" _gid @ . ." -> "
  _gid @ group-label-addr count type cr ;

: (drena-group) ( c-addr u -- group-id )
  group-count @ MAX-GROUPS >= if 2drop -1 exit then
  group-count @ >r
  r@ group-label!
  1 group-count +!
  ." [DRENA] group id=" r@ . cr
  r> ;

\ Accept counted-string address (label-addr -- group-id)
: drena-group ( label-addr -- group-id ) count (drena-group) ;

: drena-join ( neuron group -- )
  swap neuron-id ." [DRENA] join neuron " . ." -> group " . cr ;

: drena-spawn ( variation -- neuron )
  next-id @ dup 1 next-id +!
  swap make-neuron
  dup validate-neuron
  dup ." [DRENA] spawned neuron id=" neuron-id
  ." mode=" dup neuron-header header>mode mode-name cr ;

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
  1 next-id !  0 link-count !  0 group-count !
  ." [DRENA] Trit intelligence engine initialized" cr ;

drena-init
." Trit intelligence engine (DRENA data blocks) loaded. Ready for neuromorphic compute." cr
