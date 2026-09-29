\ TritiumForth kernel base — inspired by Dusk OS / Collapse OS patterns
\ See docs/FORTH-BASE-REFERENCES.md for rationale and pointers to refs/duskos/ and refs/collapseos/
\ Goal: minimal cold-boot + soft abort + tiny in-memory dict, then layer DRENA/REKIA on top.
\ TritiumOS by Draco. Slogan: The line tread between madness and genius.
\
\ Minimal dict modeled on Dusk fs/mem/dict.fs ENTRYSZ / words / entry ideas,
\ kept simple for host/poly load (no infinite loops; abort returns to caller).

\ === ARCH / Edition (32 cyan / 64 magenta) ===
\ Modeled on Dusk ARCH constant.
\ Low byte: edition (32 or 64). High bits for host/target later.
0 value ARCH
: edition@ ( -- n ) ARCH $ff and ;
: 64bit? ( -- f ) edition@ 64 = ;
: 32bit? ( -- f ) edition@ 32 = ;

: set-edition ( n -- ) to ARCH ;
\ On cold boot / first-run the host will set this from edition.trit

\ === Minimal SYSVARS analog (Dusk-style) ===
\ For Tritium: graph root, current neuron/group, REKIA state, soft error flag.
\ Start tiny; grow as needed (see Dusk mem/dict + data.txt).
create SYSVARS 256 allot   \ placeholder size
: sysvar ( offset -- addr ) SYSVARS + ;

\ Example slots (will expand with DRENA/REKIA)
 0 sysvar 'graph-root
 4 sysvar 'current-neuron
 8 sysvar 'current-group
12 sysvar 'soft-error       \ nonzero after abort; cleared by cold-boot / clear-error

: clear-error ( -- ) 0 'soft-error ! ;
: soft-error? ( -- f ) 'soft-error @ 0<> ;

: sysvars-zero ( -- )
  SYSVARS 256 0 fill ;

\ === Basic stack / arith / mem primitives (Dusk kernel requirements) ===
\ (These would be provided by host VM or assembler in real bootstrap.
\  For now we rely on the underlying Forth or host bridge.)
\ Host must supply at minimum: dup drop swap over + - @ ! , c@ c! etc.
\ We will add trit-specific on top.

\ === Tiny in-memory name table (Dusk ENTRYSZ-inspired, simple Forth) ===
\ Layout per entry (fixed slots, not linked list yet):
\   name: NAMELEN chars (blank-padded)
\   id:   cell (xt/id placeholder; host may bind later)
\ Inspired by refs/duskos/fs/mem/dict.fs ENTRYSZ=8 — here we keep a flat table.

32 constant MAX-ENTRIES
16 constant NAMELEN
NAMELEN cell + constant ENTRY-BYTES

create ENTRY-NAMES  MAX-ENTRIES NAMELEN * allot
create ENTRY-IDS    MAX-ENTRIES cells allot
create ENTRY-GIDS   MAX-ENTRIES cells allot   \ -1 = global/flat; >=0 = group-scoped (Dusk-style unit)
variable ENTRY-COUNT  0 ENTRY-COUNT !

: entry-name[] ( i -- c-addr ) NAMELEN * ENTRY-NAMES + ;
: entry-id[]   ( i -- addr )   cells ENTRY-IDS + ;
: entry-gid[]  ( i -- addr )   cells ENTRY-GIDS + ;

: entry-name-clear ( i -- )
  entry-name[] NAMELEN bl fill ;

: dict-reset ( -- )
  0 ENTRY-COUNT !
  MAX-ENTRIES 0 do i entry-name-clear loop
  MAX-ENTRIES 0 do 0 i entry-id[] ! loop
  MAX-ENTRIES 0 do -1 i entry-gid[] ! loop ;

\ entry-name! ( c-addr u i -- )  store name into slot i (truncate/pad to NAMELEN)
: entry-name! ( c-addr u i -- )
  >r r@ entry-name-clear
  NAMELEN min
  r> entry-name[] swap cmove ;

\ name-trim ( c-addr max -- c-addr u )  drop trailing blanks
: name-trim ( c-addr max -- c-addr u )
  begin dup while
    1- 2dup + c@ bl <> if 1+ exit then
  repeat ;

\ cstr= ( c-addr1 u1 c-addr2 u2 -- f )  exact length+bytes compare
: cstr= ( c-addr1 u1 c-addr2 u2 -- f )
  rot over <> if 2drop drop false exit then   \ lengths differ
  ( a1 a2 u )
  0 ?do
    over i + c@  over i + c@ <> if 2drop unloop false exit then
  loop 2drop true ;

\ entry-name= ( c-addr u i -- f )
: entry-name= ( c-addr u i -- f )
  entry-name[] NAMELEN name-trim cstr= ;

\ entry-find ( c-addr u -- i )  index or -1 if missing
: entry-find ( c-addr u -- i )
  ENTRY-COUNT @ 0 ?do
    2dup i entry-name= if 2drop i unloop exit then
  loop
  2drop -1 ;

\ Dusk-aligned aliases (wave7 item 5) — same stack as entry-find
: findentry ( c-addr u -- i ) entry-find ;
: find ( c-addr u -- i ) entry-find ;

\ entry-create-from ( c-addr u -- i )
\ Stack form of entry-create (for demos / host bridges). Returns index or -1.
: entry-create-from ( c-addr u -- i )
  2dup entry-find dup 0< 0= if
    nip nip
    ." [kernel] entry exists #" dup . cr exit
  then drop
  ENTRY-COUNT @ MAX-ENTRIES >= if
    2drop ." [kernel] dict full" cr -1 exit
  then
  ENTRY-COUNT @ >r
  ( c-addr u ) r@ entry-name!
  r@ 1+ r@ entry-id[] !
  -1 r@ entry-gid[] !          \ global / flat
  1 ENTRY-COUNT +!
  ." [kernel] created #" r@ . cr
  r> ;

\ entry-create ( "name" -- )
\ Parse next word; allocate slot + sequential id if new.
\ Table full → warn; existing name → report index. No redefine.
: entry-create ( "name" -- )
  bl word count entry-create-from drop ;

\ .words / words ( -- )  list names in the tiny table
: .words ( -- )
  ." [kernel] words (" ENTRY-COUNT @ . ." ): "
  ENTRY-COUNT @ 0 ?do
    i entry-name[] NAMELEN name-trim type space
  loop cr ;

\ group-entry-find ( c-addr u gid -- i )  scoped find; -1 if missing
\ Only matches entries whose ENTRY-GIDS slot equals gid (Dusk-style unit search).
: group-entry-find ( c-addr u gid -- i )
  >r
  ENTRY-COUNT @ 0 ?do
    i entry-gid[] @ r@ = if
      2dup i entry-name= if 2drop r> drop i unloop exit then
    then
  loop
  2drop r> drop -1 ;

\ group-entry-create ( c-addr u gid -- i )
\ Create (or return existing) name under group gid. Returns index or -1 if full.
variable _gec-gid
: group-entry-create ( c-addr u gid -- i )
  _gec-gid !
  2dup _gec-gid @ group-entry-find dup 0< 0= if
    nip nip
    ." [kernel] group-entry exists #" dup . cr exit
  then drop
  ENTRY-COUNT @ MAX-ENTRIES >= if
    2drop ." [kernel] dict full" cr -1 exit
  then
  ENTRY-COUNT @ >r
  ( c-addr u ) r@ entry-name!
  r@ 1+ r@ entry-id[] !
  _gec-gid @ r@ entry-gid[] !
  1 ENTRY-COUNT +!
  ." [kernel] group-entry #" r@ . ." gid=" _gec-gid @ . cr
  r> ;

: words ( -- ) .words ;

\ === Trit + neuron primitives (Tritium-specific, Phase 2) ===
\ Build on existing forth/trit.fs (include it in boot).
\ Phase 2 primitives live in trit.fs: trit+ trit* pack-neuron-header (alias pack-header)

\ === Cold boot / soft abort (safe for host/poly load) ===
\ Platform HAL provided by host (C# on Win11, Kotlin on komodo/Pixel 9 Pro XL).
\ See docs/SYSTEM-DESIGN-INITIAL-PLATFORMS.md and Dusk posix/vm.c + arch/hal for model.
\ Host must supply: platform-file-read, platform-console-out, platform-compute-hook, etc.
\
\ abort: soft path — set error flag, clear known SYSVARS, print once, RETURN.
\ Does NOT infinite-loop or hard-exit the AppImage/host demo session.
: abort ( -- )
  1 'soft-error !
  0 'graph-root !
  0 'current-neuron !
  0 'current-group !
  ." [kernel] abort" cr
;

\ (abort") ( c-addr u -- )  abort" style: message then soft abort
: (abort") ( c-addr u -- )
  ." [kernel] " type cr abort ;

: cold-boot ( -- )
  \ 1. init SYSVARS zeros + empty dict + clear soft error
  sysvars-zero
  clear-error
  dict-reset
  \ 2. default edition if unset (host may already have called set-edition)
  edition@ 0= if 64 set-edition then
  \ 3. boot line — interpret-ready; no stub abort
  ." [kernel] cold-boot OK ("
  64bit? if ." 64-bit" else ." 32-bit" then ." edition)" cr
  ." [kernel] interpret-ready" cr
;

\ Platform-specific init hooks (implemented in host VM, called from Forth later)
: platform-init ( -- ) ." [HAL] platform init (Win11 or komodo)" cr ;
: platform-evolve-path ( -- c-addr u ) s" evolve/" ;

\ On real bootstrap the host (C# / Kotlin / Linux C) calls cold-boot after loading sources.
\ Poly/AppImage load must remain safe: no endless abort stub.

\ === interpret-token stub (wave7 item 5) — lookup-only; no : / control-flow ===
\ interpret-token ( c-addr u -- flag )
\ Hit: print [kernel] find hit #N name=… → true
\ Miss: print [kernel] find miss → false (soft; no abort)
: interpret-token ( c-addr u -- flag )
  2dup findentry dup 0< if
    drop 2drop
    ." [kernel] find miss" cr
    false
  else
    ( c-addr u i )
    >r
    ." [kernel] find hit #" r@ . ." name=" type cr
    r> drop
    true
  then ;

\ Counted-string fixtures for kernel-demo (host SoT preferred; Forth = poly contract)
create (kd-a) 5 c, char a c, char l c, char p c, char h c, char a c,
create (kd-b) 4 c, char b c, char e c, char t c, char a c,
create (kd-miss) 6 c, char n c, char o c, char s c, char u c, char c c, char h c,

\ kernel-demo ( -- )  dict-reset → create 2 names → find hits + miss → words → interpret-token → OK
: kernel-demo ( -- )
  ." [kernel-demo] dict-reset + two names + find/interpret stub" cr
  dict-reset
  (kd-a) count entry-create-from drop
  (kd-b) count entry-create-from drop
  (kd-a) count findentry dup 0< if
    drop ." [kernel-demo] FAIL" cr exit
  then
  ." [kernel] find hit #" dup . ." name=" (kd-a) count type cr drop
  (kd-b) count find dup 0< if
    drop ." [kernel-demo] FAIL" cr exit
  then
  ." [kernel] find hit #" dup . ." name=" (kd-b) count type cr drop
  (kd-miss) count findentry 0< 0= if
    ." [kernel-demo] FAIL" cr exit
  then
  ." [kernel] find miss" cr
  words
  (kd-a) count interpret-token 0= if
    ." [kernel-demo] FAIL" cr exit
  then
  ." [kernel-demo] OK" cr ;

\ === Next steps (from refs) ===
\ - Grow dict toward Dusk units / linked entries (mem/dict.fs)
\ - Full interpret loop with : / control flow (beyond interpret-token stub)
\ - Group-scoped entries via ENTRY-GIDS + group-entry-find (wave4 item 2) — landed
\ - findentry / find aliases + interpret-token stub (wave7 item 5) — landed
\ - Use struct for neuron records
\ - Make R.E.K.I.A. a code emitter like comp/c.fs

\ Include the basic trit words (from poly or forth/)
\ include trit.fs     \ (adjust path when bundled)

." Tritium kernel loaded (minimal dict)." cr
