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
variable ENTRY-COUNT  0 ENTRY-COUNT !

: entry-name[] ( i -- c-addr ) NAMELEN * ENTRY-NAMES + ;
: entry-id[]   ( i -- addr )   cells ENTRY-IDS + ;

: entry-name-clear ( i -- )
  entry-name[] NAMELEN bl fill ;

: dict-reset ( -- )
  0 ENTRY-COUNT !
  MAX-ENTRIES 0 do i entry-name-clear loop
  MAX-ENTRIES 0 do 0 i entry-id[] ! loop ;

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

\ entry-create ( "name" -- )
\ Parse next word; allocate slot + sequential id if new.
\ Table full → warn; existing name → report index. No redefine.
: entry-create ( "name" -- )
  bl word count
  2dup entry-find dup 0< 0= if
    nip nip
    ." [kernel] entry exists #" . cr exit
  then drop
  ENTRY-COUNT @ MAX-ENTRIES >= if
    2drop ." [kernel] dict full" cr exit
  then
  ENTRY-COUNT @ >r
  ( c-addr u ) r@ entry-name!
  r@ 1+ r@ entry-id[] !        \ simple id = 1-based index
  1 ENTRY-COUNT +!
  r> drop
  ." [kernel] created #" ENTRY-COUNT @ . cr ;

\ .words / words ( -- )  list names in the tiny table
: .words ( -- )
  ." [kernel] words (" ENTRY-COUNT @ . ." ): "
  ENTRY-COUNT @ 0 ?do
    i entry-name[] NAMELEN name-trim type space
  loop cr ;
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

\ === Next steps (from refs) ===
\ - Grow dict toward Dusk units / linked entries (mem/dict.fs)
\ - Minimal interpret loop + findentry (kernel.txt)
\ - Layer units for groups (drena-group etc.)
\ - Use struct for neuron records
\ - Make R.E.K.I.A. a code emitter like comp/c.fs

\ Include the basic trit words (from poly or forth/)
\ include trit.fs     \ (adjust path when bundled)

." Tritium kernel loaded (minimal dict)." cr
