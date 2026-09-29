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
create ENTRY-BODY   MAX-ENTRIES cells allot   \ 0 = create-only; 1 = body-present (wave9 item 4)
create ENTRY-BTOKS  MAX-ENTRIES cells allot   \ body token count when body-present
create ENTRY-CELLS  MAX-ENTRIES cells allot   \ VARIABLE/CONSTANT stub cell (wave12 item 2)
create ENTRY-VALUE? MAX-ENTRIES cells allot   \ 1 = VALUE stub (wave13 item 1)
create ENTRY-DOES?  MAX-ENTRIES cells allot   \ 1 = DOES> stub marked (wave13 item 3)
create ENTRY-CELLS2 MAX-ENTRIES cells allot   \ 2VARIABLE/2CONSTANT hi stub cell (wave14 item 3)
create ENTRY-2CELL? MAX-ENTRIES cells allot   \ 1 = 2VARIABLE/2CONSTANT stub (wave14 item 3)
create ENTRY-IMM?   MAX-ENTRIES cells allot   \ 1 = IMMEDIATE bit set (wave15 item 4)
create ENTRY-DEFER? MAX-ENTRIES cells allot   \ 1 = deferred-word stub (wave16 item 1)
create ENTRY-DEFER-XT MAX-ENTRIES cells allot \ stub xt id (0 = unbound; not executable)
create ENTRY-DEFER-ACT MAX-ENTRIES NAMELEN * allot \ stub action name (blank = unbound)
create ENTRY-MARKER? MAX-ENTRIES cells allot  \ 1 = MARKER restore stub (wave16 item 2)
create ENTRY-MARKER-HERE MAX-ENTRIES cells allot \ captured HERE bump
create ENTRY-MARKER-DICT MAX-ENTRIES cells allot \ captured ENTRY-COUNT
create ENTRY-BUFFER? MAX-ENTRIES cells allot  \ 1 = BUFFER: named slot (wave16 item 3)
create ENTRY-BUFFER-N MAX-ENTRIES cells allot \ size n of named buffer
create ENTRY-BUFFER-ADDR MAX-ENTRIES cells allot \ offset into fill cap
\ synonym/alias name-map side-table (wave17 item 1) — name→name only; not linked XT
16 constant MAX-SYNONYMS
create SYN-NEW MAX-SYNONYMS NAMELEN * allot
create SYN-OLD MAX-SYNONYMS NAMELEN * allot
variable SYN-COUNT  0 SYN-COUNT !
variable ENTRY-COUNT  0 ENTRY-COUNT !

: entry-name[] ( i -- c-addr ) NAMELEN * ENTRY-NAMES + ;
: entry-id[]   ( i -- addr )   cells ENTRY-IDS + ;
: entry-gid[]  ( i -- addr )   cells ENTRY-GIDS + ;
: entry-body[] ( i -- addr )   cells ENTRY-BODY + ;
: entry-btoks[] ( i -- addr )  cells ENTRY-BTOKS + ;
: entry-cell[] ( i -- addr )   cells ENTRY-CELLS + ;
: entry-value?[] ( i -- addr ) cells ENTRY-VALUE? + ;
: entry-does?[]  ( i -- addr ) cells ENTRY-DOES? + ;
: entry-cell2[]  ( i -- addr ) cells ENTRY-CELLS2 + ;
: entry-2cell?[] ( i -- addr ) cells ENTRY-2CELL? + ;
: entry-imm?[]   ( i -- addr ) cells ENTRY-IMM? + ;
: entry-defer?[] ( i -- addr ) cells ENTRY-DEFER? + ;
: entry-defer-xt[] ( i -- addr ) cells ENTRY-DEFER-XT + ;
: entry-defer-act[] ( i -- c-addr ) NAMELEN * ENTRY-DEFER-ACT + ;
: entry-marker?[] ( i -- addr ) cells ENTRY-MARKER? + ;
: entry-marker-here[] ( i -- addr ) cells ENTRY-MARKER-HERE + ;
: entry-marker-dict[] ( i -- addr ) cells ENTRY-MARKER-DICT + ;
: entry-buffer?[] ( i -- addr ) cells ENTRY-BUFFER? + ;
: entry-buffer-n[] ( i -- addr ) cells ENTRY-BUFFER-N + ;
: entry-buffer-addr[] ( i -- addr ) cells ENTRY-BUFFER-ADDR + ;
: syn-new[] ( i -- c-addr ) NAMELEN * SYN-NEW + ;
: syn-old[] ( i -- c-addr ) NAMELEN * SYN-OLD + ;
\ colon-def stub state (wave9 item 4) — body/marker; not a real compiler
variable _colon-def    \ nonzero while defining after :
variable _colon-idx    \ entry index being defined (-1 none)
variable _colon-toks   \ body tokens accumulated
\ control-stack stub (wave10 item 2) — depth only; no XT patching
variable _cs-depth     \ open IF count
variable _cs-side      \ 0=IF side; 1=ELSE side (stub flip)
\ loop-stack stub (wave11 item 1) — sibling of cs; no XT patching
variable _loop-depth  \ open BEGIN count
\ do-loop stub (wave12 item 1) — sibling of BEGIN loop-depth; no XT patching
variable _do-loop-depth  \ open DO count
variable _do-loop-index  \ stub index for I (start value)
variable _do-loop-outer  \ enclosing-frame index stub for J (wave14 item 2)
\ case-stack stub (wave13 item 2) — sibling of cs; balance-only CASE/OF
variable _case-depth  \ open CASE count
\ CREATE/DOES> stub (wave13 item 3) — latest CREATE index; no XT child body
variable _create-latest  \ index of most recent CREATE; -1 = none
\ IMMEDIATE/POSTPONE stub (wave15 item 4) — latest colon/CREATE index for IMMEDIATE
variable _latest  \ index of most recent colon/CREATE; -1 = none
\ HERE/ALLOT stub pointer (wave14 item 1) — host-held bump counter; bytes; no arena
variable _here
$1000 _here !   \ stub base 0x1000
\ CATCH/THROW stub depth (wave14 item 4) — frame-mark only; no RS unwind
variable _catch-depth  \ open CATCH frame count
\ BUFFER: next free offset into fill cap (wave16 item 3) — named slots only; not an arena
variable _buffer-next
0 _buffer-next !


: entry-name-clear ( i -- )
  entry-name[] NAMELEN bl fill ;

: dict-reset ( -- )
  0 ENTRY-COUNT !
  MAX-ENTRIES 0 do i entry-name-clear loop
  MAX-ENTRIES 0 do 0 i entry-id[] ! loop
  MAX-ENTRIES 0 do -1 i entry-gid[] ! loop
  MAX-ENTRIES 0 do 0 i entry-body[] ! loop
  MAX-ENTRIES 0 do 0 i entry-btoks[] ! loop
  MAX-ENTRIES 0 do 0 i entry-cell[] ! loop
  MAX-ENTRIES 0 do 0 i entry-value?[] ! loop
  MAX-ENTRIES 0 do 0 i entry-does?[] ! loop
  MAX-ENTRIES 0 do 0 i entry-cell2[] ! loop
  MAX-ENTRIES 0 do 0 i entry-2cell?[] ! loop
  MAX-ENTRIES 0 do 0 i entry-imm?[] ! loop
  MAX-ENTRIES 0 do 0 i entry-defer?[] ! loop
  MAX-ENTRIES 0 do 0 i entry-defer-xt[] ! loop
  MAX-ENTRIES 0 do i entry-defer-act[] NAMELEN bl fill loop
  MAX-ENTRIES 0 do 0 i entry-marker?[] ! loop
  MAX-ENTRIES 0 do 0 i entry-marker-here[] ! loop
  MAX-ENTRIES 0 do 0 i entry-marker-dict[] ! loop
  MAX-ENTRIES 0 do 0 i entry-buffer?[] ! loop
  MAX-ENTRIES 0 do 0 i entry-buffer-n[] ! loop
  MAX-ENTRIES 0 do 0 i entry-buffer-addr[] ! loop
  0 SYN-COUNT !
  MAX-SYNONYMS 0 do i syn-new[] NAMELEN bl fill loop
  MAX-SYNONYMS 0 do i syn-old[] NAMELEN bl fill loop
  0 _colon-def !
  -1 _colon-idx !
  0 _colon-toks !
  0 _cs-depth !
  0 _cs-side !
  0 _loop-depth !
  0 _do-loop-depth !
  0 _do-loop-index !
  0 _do-loop-outer !
  0 _case-depth !
  -1 _create-latest !
  -1 _latest !
  $1000 _here !
  0 _catch-depth !
  0 _buffer-next ! ;

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
  0 r@ entry-body[] !          \ create-only until ;
  0 r@ entry-btoks[] !
  0 r@ entry-cell[] !          \ VARIABLE/CONSTANT stub init
  0 r@ entry-cell2[] !         \ 2VARIABLE/2CONSTANT hi stub init
  0 r@ entry-2cell?[] !        \ not a 2-cell stub by default
  0 r@ entry-does?[] !         \ CREATE/DOES> stub clear
  0 r@ entry-imm?[] !          \ IMMEDIATE bit clear
  0 r@ entry-defer?[] !        \ deferred stub clear
  0 r@ entry-defer-xt[] !      \ stub xt unbound
  r@ entry-defer-act[] NAMELEN bl fill
  0 r@ entry-marker?[] !       \ marker stub clear
  0 r@ entry-marker-here[] !
  0 r@ entry-marker-dict[] !
  0 r@ entry-buffer?[] !       \ BUFFER: named slot clear
  0 r@ entry-buffer-n[] !
  0 r@ entry-buffer-addr[] !
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
  0 r@ entry-body[] !
  0 r@ entry-btoks[] !
  0 r@ entry-cell[] !
  1 ENTRY-COUNT +!
  ." [kernel] group-entry #" r@ . ." gid=" _gec-gid @ . cr
  r> ;

: words ( -- ) .words ;
\ WORDS — ANS/Dusk-facing alias of .words (host also binds WORDS; Forth often case-insensitive)
: WORDS ( -- ) .words ;
\ words-count ( -- n )  optional helper = ENTRY-COUNT @
: words-count ( -- n ) ENTRY-COUNT @ ;

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

\ === interpret loop deepen (wave8 item 1) + colon body/marker (wave9 item 4) ===
\ interpret ( c-addr u -- )  whitespace-split → find → exec stub or miss (continue)
\ : / colon-create enter colon-def; body tokens until ; mark body-present.
\ Forth mirror is colon-create / colon-create-from / semicolon; host REPL binds ": … ;".

variable _interp-misses
variable _interp-hits

: ws? ( c -- flag )
  dup bl = if drop true exit then
  dup 9 = if drop true exit then    \ tab
  dup 10 = if drop true exit then   \ lf
  dup 13 = if drop true exit then   \ cr
  drop false ;

: skip-bl ( c-addr u -- c-addr' u' )
  begin
    dup while
    over c@ ws? while
      swap 1+ swap 1-
  repeat then ;

: tok-len ( c-addr u -- n )
  0 >r
  begin
    dup r@ > while
      over r@ + c@ ws? if drop r> exit then
      r> 1+ >r
  repeat drop r> ;

\ colon-body? ( i -- flag )
: colon-body? ( i -- flag )
  entry-body[] @ 0<> ;

\ Finalize open colon-def as create-only (no body) — keeps prior create-only behavior
: colon-abandon ( -- )
  _colon-def @ if
    0 _colon-def !
    -1 _colon-idx !
    0 _colon-toks !
  then ;

\ colon-body-tok ( c-addr u -- )  while colon-def: count token + optional marker
: colon-body-tok ( c-addr u -- )
  _colon-def @ 0= if 2drop exit then
  ." [colon] body + " type cr
  1 _colon-toks +! ;

\ semicolon ( -- )  end colon-def; store body-present + token count
: semicolon ( -- )
  _colon-def @ 0= if
    ." [colon] ; (not in colon-def)" cr exit
  then
  _colon-idx @ dup 0< if drop ." [colon] ; bad idx" cr exit then
  >r
  _colon-toks @ dup 0= if drop 1 then   \ fixed BODY ⇒ at least 1 if empty
  dup r@ entry-btoks[] !
  1 r@ entry-body[] !
  ." [colon] ; name=" r@ entry-name[] NAMELEN name-trim type
  ."  tokens=" . cr
  r> drop
  0 _colon-def !
  -1 _colon-idx !
  0 _colon-toks ! ;

: skip-line-rest ( c-addr u -- c-addr' u' )
  begin
    dup while
    over c@ 10 = over c@ 13 = or 0= while
      swap 1+ swap 1-
  repeat then
  dup if over c@ 13 = if swap 1+ swap 1- then then
  dup if over c@ 10 = if swap 1+ swap 1- then then ;

\ skip-paren-rest ( c-addr u -- c-addr' u' flag )  non-nested; flag true if found ')'
: skip-paren-rest ( c-addr u -- c-addr' u' flag )
  begin
    dup 0= if false exit then
    over c@ [char] ) = if
      swap 1+ swap 1- true exit
    then
    swap 1+ swap 1-
  again ;

variable _str-delim
variable _str-len

\ measure-to-delim ( c-addr u delim -- n flag )
\ n = content length; flag true if delim found.
\ For ASCII 34 ('"'), LF/CR before delim → flag false (unclosed).
: measure-to-delim ( c-addr u delim -- n flag )
  _str-delim !
  0 _str-len !
  begin
    dup 0= if 2drop _str-len @ false exit then
    over c@ _str-delim @ = if
      2drop _str-len @ true exit
    then
    over c@ 10 = over c@ 13 = or
    _str-delim @ 34 = and if
      2drop _str-len @ false exit
    then
    1 _str-len +!
    swap 1+ swap 1-
  again ;

\ advance-past-n+1 ( c-addr u n -- c-addr' u' )  skip n content bytes + 1 delim
: advance-past-n+1 ( c-addr u n -- c-addr' u' )
  1+ >r
  r@ - swap r> + swap ;

: interpret ( c-addr u -- )
  colon-abandon                 \ open : without ; → create-only
  0 _interp-misses !
  0 _interp-hits !
  begin
    skip-bl
    dup 0= if 2drop exit then
    2dup tok-len >r            \ R: toklen
    over r@                    \ c-addr u c-addr toklen
    \ wave12 item 3: comment skip before find/exec
    dup 1 = if
      over c@ 92 = if          \ '\' (ASCII 92) — line comment
        2drop
        r@ - swap r> + swap    \ advance past '\'
        skip-line-rest
        ." [comment] skip line" cr
        again
      then
      over c@ [char] ( = if    \ '(' — paren comment (non-nested)
        2drop
        r@ - swap r> + swap    \ advance past '('
        skip-paren-rest if
          ." [comment] skip paren" cr
        else
          ." [comment] FAIL reason=unclosed" cr
        then
        again
      then
    then
    \ wave13 item 4: stub string-literal parse S" / ." / .( after comment skip
    dup 2 = if
      over c@ [char] S = over 1+ c@ 34 = and if   \ S"
        2drop
        r@ - swap r> + swap
        dup if over c@ bl = if swap 1+ swap 1- then then
        2dup 34 measure-to-delim if
          ." [string] S" [char] " emit ."  length=" dup . cr
          advance-past-n+1
        else
          drop 2drop
          0 0
          ." [string] FAIL reason=unclosed" cr
        then
        again
      then
      over c@ [char] . = over 1+ c@ 34 = and if   \ ."
        2drop
        r@ - swap r> + swap
        dup if over c@ bl = if swap 1+ swap 1- then then
        2dup 34 measure-to-delim if
          ." [string] ." [char] " emit ."  length=" dup . cr
          advance-past-n+1
        else
          drop 2drop
          0 0
          ." [string] FAIL reason=unclosed" cr
        then
        again
      then
      over c@ [char] . = over 1+ c@ [char] ( = and if   \ .(
        2drop
        r@ - swap r> + swap
        dup if over c@ bl = if swap 1+ swap 1- then then
        2dup [char] ) measure-to-delim if
          ." [string] .( length=" dup . cr
          advance-past-n+1
        else
          drop 2drop
          0 0
          ." [string] FAIL reason=unclosed" cr
        then
        again
      then
    then
    2dup find dup 0< if
      drop
      ." [interpret] miss name=" type cr
      1 _interp-misses +!
    else
      >r                       \ R: toklen idx
      ." [interpret] exec #" r@ . ." name=" type cr
      r@ colon-body? if
        ." [colon] run body name="
        r@ entry-name[] NAMELEN name-trim type
        ."  tokens=" r@ entry-btoks[] @ . cr
      then
      r> drop
      1 _interp-hits +!
    then
    ( c-addr u )
    r@ - swap r> + swap
  again ;

: colon-create-from ( c-addr u -- )
  colon-abandon                 \ prior open : stays create-only
  2dup entry-create-from dup 0< if
    drop 2drop exit
  then
  ( c-addr u idx )
  dup _colon-idx !
  dup _latest !
  1 _colon-def !
  0 _colon-toks !
  0 over entry-body[] !
  0 over entry-btoks[] !
  >r
  ." [colon] : " 2dup type cr
  ." [interpret] : created " type cr
  r> drop ;

\ colon-create ( "name" -- )  parse next word; enter colon-def
: colon-create ( "name" -- )
  bl word count colon-create-from ;

\ Fixtures for interpret-demo
create (id-a) 5 c, char a c, char l c, char p c, char h c, char a c,
create (id-b) 4 c, char b c, char e c, char t c, char a c,
\ "alpha beta nosuch" = 17 chars
create (id-src) 17 c,
  char a c, char l c, char p c, char h c, char a c, bl c,
  char b c, char e c, char t c, char a c, bl c,
  char n c, char o c, char s c, char u c, char c c, char h c,

\ Fixtures for colon-demo — "square" / body toks "dup" "*" / exec "square"
create (cd-name) 6 c, char s c, char q c, char u c, char a c, char r c, char e c,
create (cd-t1) 3 c, char d c, char u c, char p c,
create (cd-t2) 1 c, char * c,
create (cd-only) 4 c, char o c, char n c, char l c, char y c,
create (cd-run) 6 c, char s c, char q c, char u c, char a c, char r c, char e c,

\ interpret-demo ( -- )  dict-reset → : two names → interpret hits+miss → OK
: interpret-demo ( -- )
  ." [interpret-demo] dict-reset + colon-create + interpret stream" cr
  dict-reset
  (id-a) count colon-create-from
  (id-b) count colon-create-from
  (id-src) count interpret
  _interp-hits @ 2 < if
    ." [interpret-demo] FAIL" cr exit
  then
  _interp-misses @ 1 <> if
    ." [interpret-demo] FAIL" cr exit
  then
  ." [interpret-demo] OK" cr ;

\ colon-demo ( -- )  body path + create-only path → [colon] run body → OK
: colon-demo ( -- )
  ." [colon-demo] dict-reset + : body ; + interpret + create-only" cr
  dict-reset
  (cd-name) count colon-create-from
  (cd-t1) count colon-body-tok
  (cd-t2) count colon-body-tok
  semicolon
  (cd-name) count findentry dup 0< if
    drop ." [colon-demo] FAIL" cr exit
  then
  colon-body? 0= if
    ." [colon-demo] FAIL" cr exit
  then
  (cd-run) count interpret
  _interp-hits @ 1 < if
    ." [colon-demo] FAIL" cr exit
  then
  \ create-only path (no ;)
  (cd-only) count colon-create-from
  colon-abandon
  (cd-only) count findentry dup 0< if
    drop ." [colon-demo] FAIL" cr exit
  then
  colon-body? if
    ." [colon-demo] FAIL" cr exit
  then
  ." [colon-demo] OK" cr ;

\ === control IF/THEN/ELSE stubs (wave10 item 2) ===
\ Balance-only cs depth; no branch XT patching. Loop stubs → BEGIN-UNTIL (wave11).
\ Forth mirrors are control-if / control-then / control-else (host binds IF/THEN/ELSE).

: control-cs-depth ( -- n ) _cs-depth @ ;

\ control-if ( flag -- )  +1 cs; print [control] IF taken=0|1
: control-if ( flag -- )
  1 _cs-depth +!
  0 _cs-side !
  0= if
    ." [control] IF taken=0" cr
  else
    ." [control] IF taken=1" cr
  then ;

\ control-else ( -- )  flip stub side; requires open IF
: control-else ( -- )
  _cs-depth @ 0= if
    ." [control] FAIL reason=unbalanced" cr exit
  then
  1 _cs-side !
  ." [control] ELSE" cr ;

\ control-then ( -- )  -1 cs; print depth after pop
: control-then ( -- )
  _cs-depth @ 0= if
    ." [control] FAIL reason=unbalanced" cr exit
  then
  -1 _cs-depth +!
  0 _cs-side !
  ." [control] THEN depth=" _cs-depth @ . cr ;

\ Aliases (may collide with host Forth IF/THEN/ELSE — prefer control-* in includes)
\ : IF control-if ;  \ omitted — host Forth uses IF; Linux REPL binds IF

\ control-demo ( -- )  balanced IF…THEN + IF…ELSE…THEN → OK
: control-demo ( -- )
  ." [control-demo] dict-reset + IF…THEN + IF…ELSE…THEN" cr
  dict-reset
  1 control-if
  control-then
  control-cs-depth 0<> if
    ." [control-demo] FAIL" cr exit
  then
  0 control-if
  control-else
  control-then
  control-cs-depth 0<> if
    ." [control-demo] FAIL" cr exit
  then
  ." [control-demo] OK" cr ;


\ === loop BEGIN/UNTIL/WHILE/REPEAT stubs (wave11 item 1) ===
\ Sibling loop-depth; no back-branch XT / DO/LOOP.
\ Forth mirrors are control-begin / control-until / control-while / control-repeat
\ (host binds BEGIN/UNTIL/WHILE/REPEAT).

: loop-cs-depth ( -- n ) _loop-depth @ ;

\ control-begin ( -- )  +1 loop; print [loop] BEGIN depth=<n>
: control-begin ( -- )
  1 _loop-depth +!
  ." [loop] BEGIN depth=" _loop-depth @ . cr ;

\ control-until ( flag -- )  -1 loop; print again=0|1
: control-until ( flag -- )
  _loop-depth @ 0= if
    ." [loop] FAIL reason=unbalanced" cr exit
  then
  -1 _loop-depth +!
  0= if
    ." [loop] UNTIL again=0" cr
  else
    ." [loop] UNTIL again=1" cr
  then ;

\ control-while ( flag -- )  mid-loop gate; depth unchanged
: control-while ( flag -- )
  _loop-depth @ 0= if
    ." [loop] FAIL reason=unbalanced" cr exit
  then
  0= if
    ." [loop] WHILE cont=0" cr
  else
    ." [loop] WHILE cont=1" cr
  then ;

\ control-repeat ( -- )  close WHILE-loop; -1 loop
: control-repeat ( -- )
  _loop-depth @ 0= if
    ." [loop] FAIL reason=unbalanced" cr exit
  then
  -1 _loop-depth +!
  ." [loop] REPEAT" cr ;

\ loop-demo ( -- )  BEGIN…UNTIL + BEGIN…WHILE…REPEAT → OK
: loop-demo ( -- )
  ." [loop-demo] dict-reset + BEGIN…UNTIL + BEGIN…WHILE…REPEAT" cr
  dict-reset
  control-begin
  0 control-until
  loop-cs-depth 0<> if
    ." [loop-demo] FAIL" cr exit
  then
  control-begin
  1 control-while
  control-repeat
  loop-cs-depth 0<> if
    ." [loop-demo] FAIL" cr exit
  then
  ." [loop-demo] OK" cr ;

\ === do-loop DO/LOOP/+LOOP/I stubs (wave12 item 1) ===
\ Sibling _do-loop-depth (independent of BEGIN _loop-depth); no counted re-exec.
\ Forth mirrors are control-do / control-loop / control-plus-loop / control-i
\ (host binds DO/LOOP/+LOOP/I).

: do-loop-depth ( -- n ) _do-loop-depth @ ;

\ control-do ( limit start -- )  +1 do-loop; stash start as I index
: control-do ( limit start -- )
  _do-loop-depth @ 0> if
    _do-loop-index @ _do-loop-outer !
  then
  _do-loop-index !
  drop
  1 _do-loop-depth +!
  ." [do-loop] DO depth=" _do-loop-depth @ . ." index=" _do-loop-index @ . cr ;

\ control-loop ( -- )  close do-loop (+1 step stub); -1 depth
: control-loop ( -- )
  _do-loop-depth @ 0= if
    ." [do-loop] FAIL reason=unbalanced" cr exit
  then
  -1 _do-loop-depth +!
  ." [do-loop] LOOP depth=" _do-loop-depth @ . cr ;

\ control-plus-loop ( n -- )  close do-loop with step stub; -1 depth
: control-plus-loop ( n -- )
  _do-loop-depth @ 0= if
    drop
    ." [do-loop] FAIL reason=unbalanced" cr exit
  then
  -1 _do-loop-depth +!
  ." [do-loop] +LOOP depth=" _do-loop-depth @ . ." step=" . cr ;

\ control-i ( -- )  print stub index; depth unchanged
: control-i ( -- )
  _do-loop-depth @ 0= if
    ." [do-loop] FAIL reason=unbalanced" cr exit
  then
  ." [do-loop] I index=" _do-loop-index @ . cr ;

\ do-loop-demo ( -- )  DO…I…LOOP + DO…I…+LOOP → OK
: do-loop-demo ( -- )
  ." [do-loop-demo] dict-reset + DO…I…LOOP + DO…I…+LOOP" cr
  dict-reset
  10 0 control-do
  control-i
  control-loop
  do-loop-depth 0<> if
    ." [do-loop-demo] FAIL" cr exit
  then
  5 0 control-do
  control-i
  1 control-plus-loop
  do-loop-depth 0<> if
    ." [do-loop-demo] FAIL" cr exit
  then
  ." [do-loop-demo] OK" cr ;


\ === LEAVE / AGAIN stubs (wave12 item 4) ===
\ LEAVE marks open BEGIN or DO frame (prefer DO); does NOT pop.
\ AGAIN pops one BEGIN frame (UNTIL-always-false shape); [loop] marker.
\ Forth mirrors: control-leave / control-again (host binds LEAVE/AGAIN).

\ control-leave ( -- )  mark only; prefer DO if both open
: control-leave ( -- )
  _do-loop-depth @ 0> if
    ." [leave] LEAVE depth=" _do-loop-depth @ . ." frame=do" cr exit
  then
  _loop-depth @ 0> if
    ." [leave] LEAVE depth=" _loop-depth @ . ." frame=begin" cr exit
  then
  ." [leave] FAIL reason=unbalanced" cr ;

\ control-again ( -- )  pop one BEGIN frame; print post-pop depth
: control-again ( -- )
  _loop-depth @ 0= if
    ." [loop] FAIL reason=unbalanced" cr exit
  then
  -1 _loop-depth +!
  ." [loop] AGAIN depth=" _loop-depth @ . cr ;

\ leave-demo ( -- )  BEGIN…LEAVE…UNTIL + DO…LEAVE…LOOP + BEGIN…AGAIN → OK
: leave-demo ( -- )
  ." [leave-demo] dict-reset + BEGIN…LEAVE…UNTIL + DO…LEAVE…LOOP + BEGIN…AGAIN" cr
  dict-reset
  \ Stream A: BEGIN … LEAVE … UNTIL
  control-begin
  control-leave
  0 control-until
  loop-cs-depth 0<> if
    ." [leave-demo] FAIL" cr exit
  then
  \ Stream B: DO … LEAVE … LOOP
  10 0 control-do
  control-leave
  control-loop
  do-loop-depth 0<> if
    ." [leave-demo] FAIL" cr exit
  then
  \ Stream C: BEGIN … AGAIN
  control-begin
  control-again
  loop-cs-depth 0<> if
    ." [leave-demo] FAIL" cr exit
  then
  ." [leave-demo] OK" cr ;

\ === UNLOOP / J stubs (wave14 item 2) ===
\ UNLOOP pops one DO frame (unlike LEAVE mark-only); no LOOP step/re-exec.
\ J prints outer-index stub when do-depth ≥ 2; depth unchanged.
\ Marker namespace: [do-loop] (same family as DO/LOOP/I). Demo banner: [unloop-demo].
\ Forth mirrors: control-unloop / control-j (host binds UNLOOP/J).

\ control-unloop ( -- )  pop one DO frame; print post-pop depth
: control-unloop ( -- )
  _do-loop-depth @ 0= if
    ." [do-loop] FAIL reason=unbalanced" cr exit
  then
  -1 _do-loop-depth +!
  ." [do-loop] UNLOOP depth=" _do-loop-depth @ . cr ;

\ control-j ( -- )  outer-index stub; require depth ≥ 2; no depth change
: control-j ( -- )
  _do-loop-depth @ 2 < if
    ." [do-loop] FAIL reason=no-outer" cr exit
  then
  ." [do-loop] J index=" _do-loop-outer @ . cr ;

\ unloop-demo ( -- )  DO…UNLOOP + nested DO…J…LOOP…LOOP → OK
: unloop-demo ( -- )
  ." [unloop-demo] dict-reset + DO…UNLOOP + DO…DO…J…LOOP…LOOP" cr
  dict-reset
  \ Stream A: DO … UNLOOP (no LOOP)
  10 0 control-do
  control-unloop
  do-loop-depth 0<> if
    ." [unloop-demo] FAIL" cr exit
  then
  \ Stream B: nested DO + J + LOOP closers
  10 0 control-do
  5 1 control-do
  control-j
  control-loop
  control-loop
  do-loop-depth 0<> if
    ." [unloop-demo] FAIL" cr exit
  then
  ." [unloop-demo] OK" cr ;

\ words-demo ( -- )  dict-reset → entry-create alpha+beta → WORDS → count≥2 → OK
\ wave11 item 3 / docs/WORDS-VOCAB.md
: words-demo ( -- )
  ." [words-demo] dict-reset + entry-create alpha + beta + WORDS" cr
  dict-reset
  (kd-a) count entry-create-from drop
  (kd-b) count entry-create-from drop
  WORDS
  words-count 2 < if
    ." [words-demo] FAIL" cr exit
  then
  (kd-a) count entry-find 0< if
    ." [words-demo] FAIL" cr exit
  then
  (kd-b) count entry-find 0< if
    ." [words-demo] FAIL" cr exit
  then
  ." [words-demo] OK" cr ;

\ === VARIABLE / CONSTANT named-cell stubs (wave12 item 2) ===
\ Prefer greppable markers; stub cell via ENTRY-CELLS (no HERE/ALLOT arena).
\ Forth mirrors: var-create / const-create (host binds VARIABLE / CONSTANT).

\ var-create-from ( c-addr u -- i )  named cell stub; init 0
: var-create-from ( c-addr u -- i )
  2dup entry-create-from dup 0< if nip nip exit then
  >r
  0 r@ entry-cell[] !
  ." [var] VARIABLE name=" type cr
  r> ;

\ const-create-from ( n c-addr u -- i )  named constant stub
: const-create-from ( n c-addr u -- i )
  2dup entry-create-from dup 0< if nip nip nip exit then
  >r
  rot r@ entry-cell[] !
  ." [var] CONSTANT name=" type ."  value=" r@ entry-cell[] @ . cr
  r> ;

\ var-create ( "name" -- )  parse + VARIABLE stub
: var-create ( "name" -- )
  bl word count var-create-from drop ;

\ const-create ( n "name" -- )  parse + CONSTANT stub
: const-create ( n "name" -- )
  bl word count const-create-from drop ;

\ var-find ( c-addr u -- i )  thin alias of entry-find
: var-find ( c-addr u -- i ) entry-find ;

\ var-fetch-from ( c-addr u -- )  optional @ stub
: var-fetch-from ( c-addr u -- )
  2dup entry-find dup 0< if
    drop 2drop
    ." [var] FAIL reason=miss" cr exit
  then
  >r 2drop
  ." [var] @ name=" r@ entry-name[] NAMELEN name-trim type
  ."  value=" r@ entry-cell[] @ . cr
  r> drop ;

\ var-store-from ( n c-addr u -- )  optional ! stub
: var-store-from ( n c-addr u -- )
  2dup entry-find dup 0< if
    drop 2drop drop
    ." [var] FAIL reason=miss" cr exit
  then
  >r 2drop
  r@ entry-cell[] !
  ." [var] ! name=" r@ entry-name[] NAMELEN name-trim type
  ."  value=" r@ entry-cell[] @ . cr
  r> drop ;

create (vd-foo) 3 c, char f c, char o c, char o c,
create (vd-bar) 3 c, char b c, char a c, char r c,

\ var-demo ( -- )  dict-reset → VARIABLE foo → CONSTANT bar 42 → find/WORDS → OK
: var-demo ( -- )
  ." [var-demo] dict-reset + VARIABLE foo + CONSTANT bar value=42" cr
  dict-reset
  (vd-foo) count var-create-from drop
  42 (vd-bar) count const-create-from drop
  WORDS
  (vd-foo) count entry-find 0< if
    ." [var-demo] FAIL" cr exit
  then
  (vd-bar) count entry-find 0< if
    ." [var-demo] FAIL" cr exit
  then
  words-count 2 < if
    ." [var-demo] FAIL" cr exit
  then
  ." [var-demo] OK" cr ;


\ === VALUE / TO named mutable-cell stubs (wave13 item 1) ===
\ Prefer greppable markers; stub cell via ENTRY-CELLS (no HERE/ALLOT arena).
\ Forth mirrors: value-create / value-to (host binds VALUE / TO).

\ value-create-from ( n c-addr u -- i )  named mutable cell; init = n
: value-create-from ( n c-addr u -- i )
  2dup entry-create-from dup 0< if nip nip nip exit then
  >r
  rot r@ entry-cell[] !
  1 r@ entry-value?[] !
  ." [value] VALUE name=" type ."  value=" r@ entry-cell[] @ . cr
  r> ;

\ value-to-from ( n c-addr u -- )  store into existing VALUE stub
: value-to-from ( n c-addr u -- )
  2dup entry-find dup 0< if
    drop 2drop drop
    ." [value] FAIL reason=miss" cr exit
  then
  dup entry-value?[] @ 0= if
    drop 2drop drop
    ." [value] FAIL reason=miss" cr exit
  then
  >r 2drop
  r@ entry-cell[] !
  ." [value] TO name=" r@ entry-name[] NAMELEN name-trim type
  ."  value=" r@ entry-cell[] @ . cr
  r> drop ;

\ value-create ( n "name" -- )  parse + VALUE stub
: value-create ( n "name" -- )
  bl word count value-create-from drop ;

\ value-to ( n "name" -- )  parse + TO stub
: value-to ( n "name" -- )
  bl word count value-to-from ;

\ value-find ( c-addr u -- i )  thin alias of entry-find
: value-find ( c-addr u -- i ) entry-find ;

\ value-fetch-from ( c-addr u -- )  optional value@ stub
: value-fetch-from ( c-addr u -- )
  2dup entry-find dup 0< if
    drop 2drop
    ." [value] FAIL reason=miss" cr exit
  then
  dup entry-value?[] @ 0= if
    drop 2drop
    ." [value] FAIL reason=miss" cr exit
  then
  >r 2drop
  ." [value] @ name=" r@ entry-name[] NAMELEN name-trim type
  ."  value=" r@ entry-cell[] @ . cr
  r> drop ;

create (vald-baz) 3 c, char b c, char a c, char z c,

\ value-demo ( -- )  dict-reset → 7 VALUE baz → 99 TO baz → value@ → find/WORDS → OK
: value-demo ( -- )
  ." [value-demo] dict-reset + 7 VALUE baz + 99 TO baz" cr
  dict-reset
  7 (vald-baz) count value-create-from drop
  99 (vald-baz) count value-to-from
  (vald-baz) count value-fetch-from
  WORDS
  (vald-baz) count entry-find 0< if
    ." [value-demo] FAIL" cr exit
  then
  words-count 1 < if
    ." [value-demo] FAIL" cr exit
  then
  ." [value-demo] OK" cr ;


\ === CASE / OF / ENDOF / ENDCASE stubs (wave13 item 2) ===
\ Balance-only case-depth sibling of IF/ELSE/THEN; no OF match / branch XT / token skip.
\ Forth mirrors: control-case / control-of / control-endof / control-endcase
\ (host binds CASE / OF / ENDOF / ENDCASE). Marker namespace is [case] NOT [control].

: case-cs-depth ( -- n ) _case-depth @ ;

\ control-case ( -- )  +1 case; print [case] CASE depth=<n>
: control-case ( -- )
  1 _case-depth +!
  ." [case] CASE depth=" _case-depth @ . cr ;

\ control-of ( -- )  require open CASE; no depth change; print [case] OF
: control-of ( -- )
  _case-depth @ 0= if
    ." [case] FAIL reason=unbalanced" cr exit
  then
  ." [case] OF" cr ;

\ control-endof ( -- )  close OF branch; no depth change; print [case] ENDOF
: control-endof ( -- )
  _case-depth @ 0= if
    ." [case] FAIL reason=unbalanced" cr exit
  then
  ." [case] ENDOF" cr ;

\ control-endcase ( -- )  -1 case; print depth after pop
: control-endcase ( -- )
  _case-depth @ 0= if
    ." [case] FAIL reason=unbalanced" cr exit
  then
  -1 _case-depth +!
  ." [case] ENDCASE depth=" _case-depth @ . cr ;

\ case-demo ( -- )  Stream A single OF + Stream B two OF arms → OK
: case-demo ( -- )
  ." [case-demo] dict-reset + CASE…OF…ENDOF…ENDCASE + two-OF" cr
  dict-reset
  \ Stream A — single OF
  control-case
  control-of
  control-endof
  control-endcase
  case-cs-depth 0<> if
    ." [case-demo] FAIL" cr exit
  then
  \ Stream B — two OF arms
  control-case
  control-of
  control-endof
  control-of
  control-endof
  control-endcase
  case-cs-depth 0<> if
    ." [case-demo] FAIL" cr exit
  then
  ." [case-demo] OK" cr ;

\ === CREATE / DOES> defining-word stubs (wave13 item 3) ===
\ Prefer greppable markers; no real XT chaining / child runtime body / HERE/ALLOT.
\ Forth mirrors: create-entry / does-mark (host binds CREATE / DOES>).

\ create-entry-from ( c-addr u -- i )  named dict entry; presence only
: create-entry-from ( c-addr u -- i )
  2dup entry-create-from dup 0< if nip nip exit then
  >r
  0 r@ entry-does?[] !
  r@ _create-latest !
  r@ _latest !
  ." [create] CREATE name=" type cr
  r> ;

\ does-mark ( -- )  mark does-body stub on latest CREATE
: does-mark ( -- )
  _create-latest @ dup 0< if
    drop
    ." [create] FAIL reason=unbalanced" cr exit
  then
  >r
  1 r@ entry-does?[] !
  ." [create] DOES> name=" r@ entry-name[] NAMELEN name-trim type cr
  -1 _create-latest !
  r> drop ;

\ create-entry ( "name" -- )  parse + CREATE stub
: create-entry ( "name" -- )
  bl word count create-entry-from drop ;

\ create-find ( c-addr u -- i )  thin alias of entry-find
: create-find ( c-addr u -- i ) entry-find ;

create (crd-widget) 6 c, char w c, char i c, char d c, char g c, char e c, char t c,

\ create-demo ( -- )  dict-reset → CREATE widget → DOES> → find/WORDS → OK
: create-demo ( -- )
  ." [create-demo] dict-reset + CREATE widget + DOES>" cr
  dict-reset
  (crd-widget) count create-entry-from drop
  does-mark
  WORDS
  (crd-widget) count entry-find 0< if
    ." [create-demo] FAIL" cr exit
  then
  words-count 1 < if
    ." [create-demo] FAIL" cr exit
  then
  ." [create-demo] OK" cr ;

\ === comment-parse stubs (wave12 item 3) ===
\ Stream skip inside interpret (above). Forth mirrors: comment-line / comment-paren
\ (host binds '\' / '(' on REPL interpret path).

: comment-line ( -- )
  ." [comment] skip line" cr ;

: comment-paren ( -- )
  ." [comment] skip paren" cr ;

\ Fixtures for comment-demo
\ "alpha" / "beta"
create (cm-a) 5 c, char a c, char l c, char p c, char h c, char a c,
create (cm-b) 4 c, char b c, char e c, char t c, char a c,
\ line stream: "alpha \ line comment" + lf  (23 chars: alpha sp \ sp line sp comment + lf = 5+1+1+1+4+1+7+1=21?)
\ a l p h a   \   l i n e   c o m m e n t \n
\ 5 +1 +1 +1 +4 +1 +7 +1 = 21
create (cm-line) 21 c,
  char a c, char l c, char p c, char h c, char a c, bl c,
  92 c, bl c,
  char l c, char i c, char n c, char e c, bl c,
  char c c, char o c, char m c, char m c, char e c, char n c, char t c,
  10 c,
\ paren stream: "alpha ( paren comment ) beta" = 28 chars
\ a l p h a sp ( sp p a r e n sp c o m m e n t sp ) sp b e t a
\ 5+1+1+1+5+1+7+1+1+1+4 = 28
create (cm-paren) 28 c,
  char a c, char l c, char p c, char h c, char a c, bl c,
  char ( c, bl c,
  char p c, char a c, char r c, char e c, char n c, bl c,
  char c c, char o c, char m c, char m c, char e c, char n c, char t c, bl c,
  char ) c, bl c,
  char b c, char e c, char t c, char a c,
\ body strings that must NOT be dict hits
create (cm-line-body) 4 c, char l c, char i c, char n c, char e c,
create (cm-paren-body) 5 c, char p c, char a c, char r c, char e c, char n c,
create (cm-comment) 7 c, char c c, char o c, char m c, char m c, char e c, char n c, char t c,

\ comment-demo ( -- )  dict-reset → known name → mixed streams → OK
: comment-demo ( -- )
  ." [comment-demo] dict-reset + mixed \\ / ( ) streams" cr
  dict-reset
  (cm-a) count entry-create-from drop
  (cm-line) count interpret
  _interp-hits @ 1 < if
    ." [comment-demo] FAIL" cr exit
  then
  (cm-line-body) count entry-find 0< 0= if
    ." [comment-demo] FAIL" cr exit
  then
  (cm-comment) count entry-find 0< 0= if
    ." [comment-demo] FAIL" cr exit
  then
  (cm-paren) count interpret
  _interp-hits @ 2 < if
    ." [comment-demo] FAIL" cr exit
  then
  (cm-paren-body) count entry-find 0< 0= if
    ." [comment-demo] FAIL" cr exit
  then
  (cm-b) count entry-find 0< if
    ." [comment-demo] FAIL" cr exit
  then
  ." [comment-demo] OK" cr ;

\ === string-lit stubs (wave13 item 4) ===
\ Stream parse inside interpret (above). Forth mirrors: string-s / string-dot / string-paren
\ (host binds S" / ." / .( on interpret/stream path; mirrors if host Forth names collide).

: string-s ( -- )
  ." [string] S" [char] " emit cr ;

: string-dot ( -- )
  ." [string] ." [char] " emit cr ;

: string-paren ( -- )
  ." [string] .(" cr ;

\ Fixtures for string-demo
\ S" hello" = 9 chars: S " sp h e l l o "
create (sl-s) 9 c,
  char S c, 34 c, bl c,
  char h c, char e c, char l c, char l c, char o c, 34 c,
\ ." world" = 9 chars
create (sl-dot) 9 c,
  char . c, 34 c, bl c,
  char w c, char o c, char r c, char l c, char d c, 34 c,
\ .( hi) = 6 chars
create (sl-paren) 6 c,
  char . c, char ( c, bl c,
  char h c, char i c, char ) c,
\ body strings that must NOT be dict hits
create (sl-hello) 5 c, char h c, char e c, char l c, char l c, char o c,
create (sl-world) 5 c, char w c, char o c, char r c, char l c, char d c,
create (sl-hi) 2 c, char h c, char i c,

\ string-demo ( -- )  dict-reset → S" / ." / .( streams → OK
: string-demo ( -- )
  ." [string-demo] dict-reset + S" [char] " emit ."  / ." [char] " emit ."  / .(" cr
  dict-reset
  (sl-s) count interpret
  (sl-hello) count entry-find 0< 0= if
    ." [string-demo] FAIL" cr exit
  then
  (sl-dot) count interpret
  (sl-world) count entry-find 0< 0= if
    ." [string-demo] FAIL" cr exit
  then
  (sl-paren) count interpret
  (sl-hi) count entry-find 0< 0= if
    ." [string-demo] FAIL" cr exit
  then
  ." [string-demo] OK" cr ;

\ === HERE / ALLOT dictionary-pointer stubs (wave14 item 1) ===
\ Prefer greppable markers; one cell bump counter (bytes). No arena/pool/free.
\ Forth mirrors: here-at / allot-bump (host binds HERE / ALLOT; host Forth often collides).
\ Optional comma / c-comma when cheap.

: here-at ( -- )
  ." [allot] HERE addr=" _here @ . cr ;

: allot-bump ( n -- )
  dup 0< if
    drop ." [allot] FAIL reason=neg" cr exit
  then
  dup _here +!
  ." [allot] ALLOT n=" . ." addr=" _here @ . cr ;

\ comma ( n -- )  optional cell-store stub + bump by cell
: comma ( n -- )
  cell _here +!
  ." [allot] , value=" . cr ;

\ c-comma ( c -- )  optional char-store stub + bump by 1
: c-comma ( c -- )
  1 _here +!
  ." [allot] C, value=" . cr ;

\ allot-demo ( -- )  HERE → ALLOT 8 → HERE (bump visible) → OK
: allot-demo ( -- )
  ." [allot-demo] HERE + ALLOT 8 + HERE" cr
  dict-reset
  _here @ >r
  here-at
  8 allot-bump
  _here @ r@ > 0= if
    r> drop ." [allot-demo] FAIL" cr exit
  then
  _here @ r> 8 + <> if
    ." [allot-demo] FAIL" cr exit
  then
  here-at
  ." [allot-demo] OK" cr ;


\ === CELL / CELLS / ALIGN / ALIGNED dictionary-unit stubs (wave15 item 1) ===
\ Prefer greppable markers; host cell constant + HERE pointer round-up. No dictionary image.
\ Forth mirrors: cell-size / cells-n / align-here / aligned-addr (host binds CELL / CELLS / ALIGN / ALIGNED).
\ Linux SoT cell size n=8. Pairs with wave14 HERE/ALLOT byte bump.

8 constant _cell-bytes   \ Linux SoT n=8

\ align-up ( addr -- a-addr )  round addr up to cell boundary
: align-up ( addr -- a-addr )
  dup 0< if drop -1 exit then
  dup _cell-bytes mod dup 0= if drop exit then
  _cell-bytes swap - + ;

: cell-size ( -- )
  ." [cell] CELL bytes=" _cell-bytes . cr ;

: cells-n ( k -- )
  dup 0< if
    drop ." [cell] FAIL reason=neg" cr exit
  then
  dup _cell-bytes *
  ." [cell] CELLS n=" swap . ." bytes=" . cr ;

: align-here ( -- )
  _here @ align-up dup 0< if
    drop ." [cell] FAIL reason=neg" cr exit
  then
  dup _here !
  ." [cell] ALIGN addr=" . cr ;

: aligned-addr ( addr -- )
  dup 0< if
    drop ." [cell] FAIL reason=neg" cr exit
  then
  align-up
  ." [cell] ALIGNED addr=" . cr ;

\ cell-demo ( -- )  CELL + CELLS 3 + 1 ALLOT + ALIGN + ALIGNED 1 + OK
: cell-demo ( -- )
  ." [cell-demo] CELL + CELLS + ALIGN + ALIGNED" cr
  dict-reset
  cell-size
  3 cells-n
  \ Force unaligned: HERE base $1000 is cell-aligned, so bump 1 byte
  1 allot-bump
  _here @ >r
  align-here
  _here @ _cell-bytes mod 0= 0= if
    r> drop ." [cell-demo] FAIL" cr exit
  then
  _here @ r> > 0= if
    ." [cell-demo] FAIL" cr exit
  then
  1 aligned-addr
  ." [cell-demo] OK" cr ;



\ === PICK / ROLL / DEPTH / ?DUP stack-marker stubs (wave15 item 2) ===
\ Prefer greppable markers; tiny host stack picture only. Not a real threaded stack.
\ Forth mirrors: pick-nth / roll-nth / stack-depth / qdup (host binds PICK / ROLL / DEPTH / ?DUP).
\ Do NOT redefine host 2dup/2drop/2swap. Do NOT replace in-tree 2 pick (drena.fs) — mirrors only.

8 constant _pick-max
create _pick-pic 8 cells allot
variable _pick-depth
variable _pick-tos-hold   \ scratch for ?DUP / demo

: pick-pic-reset ( -- )
  0 _pick-depth ! ;

: pick-pic-push ( x -- )
  _pick-depth @ _pick-max >= if drop exit then
  _pick-pic _pick-depth @ cells + !
  1 _pick-depth +! ;

: pick-pic-tos@ ( -- x )
  _pick-depth @ 0= if 0 exit then
  _pick-pic _pick-depth @ 1- cells + @ ;

: stack-depth ( -- )
  ." [pick] DEPTH n=" _pick-depth @ . cr ;

: pick-nth ( u -- )
  dup 0< if
    drop ." [pick] FAIL reason=underflow" cr exit
  then
  dup _pick-depth @ >= if
    drop ." [pick] FAIL reason=underflow" cr exit
  then
  \ u=0 → TOS = depth-1; u indexes down from TOS
  dup >r
  _pick-depth @ 1- swap -   \ idx
  _pick-pic swap cells + @
  ." [pick] PICK u=" r> . ." x=" . cr ;

: roll-nth ( u -- )
  dup 0< if
    drop ." [pick] FAIL reason=underflow" cr exit
  then
  dup _pick-depth @ >= if
    drop ." [pick] FAIL reason=underflow" cr exit
  then
  dup 0= if
    ." [pick] ROLL u=" . cr exit
  then
  \ Rotate picture: item at (depth-1-u) → TOS; shift higher slots down one
  dup >r
  _pick-depth @ 1- over -          \ u  idx
  nip                              \ idx   (u saved in R)
  _pick-pic over cells + @         \ idx  val
  swap                             \ val  idx
  begin
    dup _pick-depth @ 1- <
  while
    dup >r                         \ val idx | R: u idx
    1+ _pick-pic swap cells + @    \ val next
    _pick-pic r@ cells + !         \ val      (store next at idx)
    r> 1+                          \ val idx+1
  repeat
  drop                             \ val
  _pick-pic _pick-depth @ 1- cells + !
  ." [pick] ROLL u=" r> . cr ;

: qdup ( -- )
  pick-pic-tos@ dup 0= if
    drop ." [pick] ?DUP flag=0" cr exit
  then
  pick-pic-push
  ." [pick] ?DUP flag=1" cr ;

\ pick-demo ( -- )  seed≥3 + DEPTH + PICK + ROLL + ?DUP both paths + OK
: pick-demo ( -- )
  ." [pick-demo] DEPTH + PICK + ROLL + ?DUP" cr
  pick-pic-reset
  10 pick-pic-push
  20 pick-pic-push
  30 pick-pic-push
  stack-depth
  1 pick-nth
  2 roll-nth
  \ nonzero ?DUP (TOS after ROLL 2 on [10,20,30] → [20,30,10], TOS=10)
  qdup
  \ zero path: push 0 then ?DUP
  0 pick-pic-push
  qdup
  ." [pick-demo] OK" cr ;


\ === FILL / ERASE / MOVE / CMOVE fixed host-buffer stubs (wave15 item 3) ===
\ Prefer greppable markers; one fixed host byte buffer (cap=64). Not an arena/heap.
\ Forth mirrors: fill-buf / erase-buf / move-buf / cmove-buf (host binds FILL / ERASE / MOVE / CMOVE).
\ Do NOT redefine kernel-internal cmove used by dict copy in kernel.fs / drena.fs — mirrors only.
\ Addresses are offsets into the fixed buffer (0 .. cap-1).

64 constant _fill-cap
create _fill-buf 64 allot

: fill-bounds? ( addr u -- flag )  \ true = out of bounds
  over 0< if 2drop -1 exit then
  dup 0< if 2drop -1 exit then
  + _fill-cap > ;

: fill-buf ( addr u char -- )
  >r                                \ addr u | R: char
  2dup fill-bounds? if
    2drop r> drop ." [fill] FAIL reason=bounds" cr exit
  then
  2dup                              \ addr u addr u
  0 ?do                             \ addr u
    over i + _fill-buf + r@ swap c!
  loop
  ." [fill] FILL addr=" swap . ." u=" . ." char=" r> . cr ;

: erase-buf ( addr u -- )
  2dup fill-bounds? if
    2drop ." [fill] FAIL reason=bounds" cr exit
  then
  2dup
  0 ?do
    over i + _fill-buf + 0 swap c!
  loop
  ." [fill] ERASE addr=" swap . ." u=" . cr ;

: move-buf ( from to u -- )
  >r                                \ from to | R: u
  over r@ fill-bounds? if
    2drop r> drop ." [fill] FAIL reason=bounds" cr exit
  then
  dup r@ fill-bounds? if
    2drop r> drop ." [fill] FAIL reason=bounds" cr exit
  then
  r@ 0 ?do
    over i + _fill-buf + c@
    over i + _fill-buf + c!
  loop
  \ from to | R:u
  swap                              \ to from
  ." [fill] MOVE from=" . ." to=" dup . ." u=" r@ . ." bytes=" r@ . cr
  drop r> drop ;

: cmove-buf ( from to u -- )
  >r                                \ from to | R: u
  over r@ fill-bounds? if
    2drop r> drop ." [fill] FAIL reason=bounds" cr exit
  then
  dup r@ fill-bounds? if
    2drop r> drop ." [fill] FAIL reason=bounds" cr exit
  then
  r@ 0 ?do
    over i + _fill-buf + c@
    over i + _fill-buf + c!
  loop
  swap
  ." [fill] CMOVE from=" . ." to=" dup . ." u=" r@ . ." bytes=" r@ . cr
  drop r> drop ;

\ fill-demo ( -- )  FILL + ERASE + MOVE + CMOVE (non-overlap) + OK
: fill-demo ( -- )
  ." [fill-demo] FILL + ERASE + MOVE + CMOVE cap=" _fill-cap . cr
  \ 1) FILL addr=0 u=8 char=65 ('A')
  0 8 65 fill-buf
  \ 2) ERASE addr=0 u=8
  0 8 erase-buf
  \ 3) seed source then MOVE non-overlapping: FILL 0..4 with 'B'=66, MOVE 0->8 u=4
  0 4 66 fill-buf
  0 8 4 move-buf
  \ 4) CMOVE non-overlapping: FILL 16..4 with 'C'=67, CMOVE 16->24 u=4
  16 4 67 fill-buf
  16 24 4 cmove-buf
  ." [fill-demo] OK" cr ;


\ === IMMEDIATE / POSTPONE compile-only stubs (wave15 item 4) ===
\ Prefer greppable markers; flag + name mark only. No linked XT / executing postponed XT.
\ Forth mirrors: immediate-mark / postpone-mark (host binds IMMEDIATE / POSTPONE).

\ immediate-mark-idx ( i -- )  set immediate-bit on entry i
: immediate-mark-idx ( i -- )
  dup 0< if
    drop ." [imm] FAIL reason=miss" cr exit
  then
  dup ENTRY-COUNT @ >= if
    drop ." [imm] FAIL reason=miss" cr exit
  then
  >r
  1 r@ entry-imm?[] !
  ." [imm] IMMEDIATE name=" r@ entry-name[] NAMELEN name-trim type cr
  r> drop ;

\ immediate-mark ( -- )  IMMEDIATE on latest colon/CREATE
: immediate-mark ( -- )
  _latest @ immediate-mark-idx ;

\ immediate-mark-named ( c-addr u -- )  IMMEDIATE on named entry
: immediate-mark-named ( c-addr u -- )
  entry-find immediate-mark-idx ;

\ postpone-mark-from ( c-addr u -- )  parse/mark name only; no XT compile/run
: postpone-mark-from ( c-addr u -- )
  2dup entry-find 0< if
    2drop ." [imm] FAIL reason=miss" cr exit
  then
  ." [imm] POSTPONE name=" type cr
  _colon-def @ if
    ." [imm] compile-only" cr
  then ;

\ postpone-mark ( "name" -- )  parse + POSTPONE stub
: postpone-mark ( "name" -- )
  bl word count postpone-mark-from ;

create (imm-sq) 6 c, char s c, char q c, char u c, char a c, char r c, char e c,

\ imm-demo ( -- )  dict-reset → : square ; → IMMEDIATE → POSTPONE → OK
: imm-demo ( -- )
  ." [imm-demo] dict-reset + : square ; + IMMEDIATE + POSTPONE" cr
  dict-reset
  (imm-sq) count colon-create-from
  s" dup" colon-body-tok
  s" *" colon-body-tok
  semicolon
  (imm-sq) count entry-find 0< if
    ." [imm-demo] FAIL" cr exit
  then
  immediate-mark
  (imm-sq) count postpone-mark-from
  ." [imm-demo] OK" cr ;


\ === 2VARIABLE / 2CONSTANT double-cell stubs (wave14 item 3) ===
\ Prefer greppable markers; two-slot via ENTRY-CELLS + ENTRY-CELLS2 (no double heap).
\ Forth mirrors: 2var-create / 2const-create (host binds 2VARIABLE / 2CONSTANT).
\ Optional 2@ / 2! when cheap.

\ 2var-create-from ( c-addr u -- i )  named double-cell stub; init 0 0
: 2var-create-from ( c-addr u -- i )
  2dup entry-create-from dup 0< if nip nip exit then
  >r
  0 r@ entry-cell[] !
  0 r@ entry-cell2[] !
  1 r@ entry-2cell?[] !
  ." [2var] 2VARIABLE name=" type cr
  r> ;

\ 2const-create-from ( lo hi c-addr u -- i )  named double-constant stub
: 2const-create-from ( lo hi c-addr u -- i )
  2dup entry-create-from dup 0< if nip nip nip nip exit then
  >r
  ( lo hi ) swap r@ entry-cell[] !   \ lo
  r@ entry-cell2[] !                 \ hi
  1 r@ entry-2cell?[] !
  ." [2var] 2CONSTANT name=" type
  ."  lo=" r@ entry-cell[] @ .
  ." hi=" r@ entry-cell2[] @ . cr
  r> ;

\ 2var-create ( "name" -- )  parse + 2VARIABLE stub
: 2var-create ( "name" -- )
  bl word count 2var-create-from drop ;

\ 2const-create ( lo hi "name" -- )  parse + 2CONSTANT stub
: 2const-create ( lo hi "name" -- )
  bl word count 2const-create-from drop ;

\ 2var-find ( c-addr u -- i )  thin alias of entry-find
: 2var-find ( c-addr u -- i ) entry-find ;

\ 2var-fetch-from ( c-addr u -- )  optional 2@ stub
: 2var-fetch-from ( c-addr u -- )
  2dup entry-find dup 0< if
    drop 2drop
    ." [2var] FAIL reason=miss" cr exit
  then
  dup entry-2cell?[] @ 0= if
    drop 2drop
    ." [2var] FAIL reason=miss" cr exit
  then
  >r 2drop
  ." [2var] 2@ name=" r@ entry-name[] NAMELEN name-trim type
  ."  lo=" r@ entry-cell[] @ .
  ." hi=" r@ entry-cell2[] @ . cr
  r> drop ;

\ 2var-store-from ( lo hi c-addr u -- )  optional 2! stub
: 2var-store-from ( lo hi c-addr u -- )
  2dup entry-find dup 0< if
    drop 2drop 2drop
    ." [2var] FAIL reason=miss" cr exit
  then
  dup entry-2cell?[] @ 0= if
    drop 2drop 2drop
    ." [2var] FAIL reason=miss" cr exit
  then
  >r 2drop
  ( lo hi ) swap r@ entry-cell[] !
  r@ entry-cell2[] !
  ." [2var] 2! name=" r@ entry-name[] NAMELEN name-trim type
  ."  lo=" r@ entry-cell[] @ .
  ." hi=" r@ entry-cell2[] @ . cr
  r> drop ;

create (2vd-dv) 2 c, char d c, char v c,
create (2vd-dc) 2 c, char d c, char c c,

\ 2var-demo ( -- )  dict-reset → 2VARIABLE dv → 1 2 2CONSTANT dc → find/WORDS → OK
: 2var-demo ( -- )
  ." [2var-demo] dict-reset + 2VARIABLE dv + 1 2 2CONSTANT dc" cr
  dict-reset
  (2vd-dv) count 2var-create-from drop
  1 2 (2vd-dc) count 2const-create-from drop
  WORDS
  (2vd-dv) count entry-find 0< if
    ." [2var-demo] FAIL" cr exit
  then
  (2vd-dc) count entry-find 0< if
    ." [2var-demo] FAIL" cr exit
  then
  words-count 2 < if
    ." [2var-demo] FAIL" cr exit
  then
  ." [2var-demo] OK" cr ;


\ === CATCH / THROW stubs (wave14 item 4) ===
\ Frame-mark only: CATCH +1 depth; THROW 0 no-op; THROW n≠0 pops one frame.
\ Optional ABORT" reuses string-lit parse path (marker + THROW-equivalent).
\ Soft KERNEL abort / (abort") remain. No real RS unwind / frame restore.
\ Forth mirrors: catch-mark / throw-code / abort-quote (host binds CATCH/THROW/ABORT").

: catch-depth ( -- n ) _catch-depth @ ;

\ catch-mark ( -- )  +1 catch-depth; print post-push depth
: catch-mark ( -- )
  1 _catch-depth +!
  ." [throw] CATCH depth=" _catch-depth @ . cr ;

\ throw-code ( n -- )  n=0 no-op; n≠0 require open CATCH then pop
: throw-code ( n -- )
  dup 0= if
    ." [throw] THROW code=" . cr exit
  then
  _catch-depth @ 0= if
    drop
    ." [throw] FAIL reason=uncaught" cr exit
  then
  -1 _catch-depth +!
  ." [throw] THROW code=" . cr ;

\ abort-quote-from ( c-addr u -- )  optional ABORT" stub; THROW-equivalent code=-2
: abort-quote-from ( c-addr u -- )
  ." [throw] ABORT" [char] " emit space type cr
  -2 throw-code ;

create (td-boom) 4 c, char b c, char o c, char o c, char m c,

\ throw-demo ( -- )  CATCH + THROW 0 + THROW nonzero + optional ABORT" → OK
: throw-demo ( -- )
  ." [throw-demo] dict-reset + CATCH + THROW 0 + THROW 42 + ABORT" [char] " emit ."  boom" [char] " emit cr
  dict-reset
  catch-mark
  0 throw-code
  catch-depth 1 <> if
    ." [throw-demo] FAIL" cr exit
  then
  42 throw-code
  catch-depth 0<> if
    ." [throw-demo] FAIL" cr exit
  then
  catch-mark
  (td-boom) count abort-quote-from
  catch-depth 0<> if
    ." [throw-demo] FAIL" cr exit
  then
  ." [throw-demo] OK" cr ;


\ === DEFER / IS / ACTION-OF deferred-word stubs (wave16 item 1) ===
\ Prefer greppable markers; name + bind mark only. No XT vector / executing bound XT.
\ Forth mirrors: defer-create / is-bind / action-of-xt
\ Do NOT redefine rekia live defer / is platform hooks.

create (df-widget) 6 c, char w c, char i c, char d c, char g c, char e c, char t c,
create (df-bound) 5 c, char b c, char o c, char u c, char n c, char d c,

\ defer-act! ( c-addr u i -- )  store stub action name into entry i
: defer-act! ( c-addr u i -- )
  >r r@ entry-defer-act[] NAMELEN bl fill
  NAMELEN min
  r> entry-defer-act[] swap cmove ;

\ defer-create-from ( c-addr u -- i )  named deferred entry; unbound
: defer-create-from ( c-addr u -- i )
  2dup entry-create-from dup 0< if nip nip exit then
  >r
  1 r@ entry-defer?[] !
  0 r@ entry-defer-xt[] !
  r@ entry-defer-act[] NAMELEN bl fill
  r@ _latest !
  ." [defer] DEFER name=" type cr
  r> ;

\ defer-create ( "name" -- )  parse + DEFER stub
: defer-create ( "name" -- )
  bl word count defer-create-from drop ;

\ is-bind-from ( c-addr u -- )  bind default stub action/xt to deferred name
: is-bind-from ( c-addr u -- )
  2dup entry-find dup 0< if
    drop 2drop
    ." [defer] FAIL reason=miss" cr exit
  then
  dup entry-defer?[] @ 0= if
    drop 2drop
    ." [defer] FAIL reason=miss" cr exit
  then
  >r 2drop
  1 r@ entry-defer-xt[] !
  (df-bound) count r@ defer-act!
  ." [defer] IS name=" r@ entry-name[] NAMELEN name-trim type
  ."  xt=" r@ entry-defer-xt[] @ .
  ." action=" r@ entry-defer-act[] NAMELEN name-trim type cr
  r> drop ;

\ is-bind-xt-from ( xt c-addr u -- )  bind stub xt id + default action
: is-bind-xt-from ( xt c-addr u -- )
  2dup entry-find dup 0< if
    drop 2drop drop
    ." [defer] FAIL reason=miss" cr exit
  then
  dup entry-defer?[] @ 0= if
    drop 2drop drop
    ." [defer] FAIL reason=miss" cr exit
  then
  >r 2drop
  ( xt ) r@ entry-defer-xt[] !
  (df-bound) count r@ defer-act!
  ." [defer] IS name=" r@ entry-name[] NAMELEN name-trim type
  ."  xt=" r@ entry-defer-xt[] @ .
  ." action=" r@ entry-defer-act[] NAMELEN name-trim type cr
  r> drop ;

\ is-bind ( "name" -- )  parse + IS stub (default xt=1 action=bound)
: is-bind ( "name" -- )
  bl word count is-bind-from ;

\ action-of-xt-from ( c-addr u -- )  query/print bound stub; no XT execute
: action-of-xt-from ( c-addr u -- )
  2dup entry-find dup 0< if
    drop 2drop
    ." [defer] FAIL reason=miss" cr exit
  then
  dup entry-defer?[] @ 0= if
    drop 2drop
    ." [defer] FAIL reason=miss" cr exit
  then
  >r 2drop
  ." [defer] ACTION-OF name=" r@ entry-name[] NAMELEN name-trim type
  ."  xt=" r@ entry-defer-xt[] @ .
  ." action=" r@ entry-defer-act[] NAMELEN name-trim type cr
  r> drop ;

\ action-of-xt ( "name" -- )  parse + ACTION-OF stub
: action-of-xt ( "name" -- )
  bl word count action-of-xt-from ;

\ defer-demo ( -- )  dict-reset → DEFER widget → IS → ACTION-OF → OK
: defer-demo ( -- )
  ." [defer-demo] dict-reset + DEFER widget + IS + ACTION-OF" cr
  dict-reset
  (df-widget) count defer-create-from drop
  (df-widget) count entry-find 0< if
    ." [defer-demo] FAIL" cr exit
  then
  (df-widget) count is-bind-from
  (df-widget) count action-of-xt-from
  WORDS
  (df-widget) count entry-find 0< if
    ." [defer-demo] FAIL" cr exit
  then
  ." [defer-demo] OK" cr ;



\ === MARKER dictionary-restore stubs (wave16 item 2) ===
\ Prefer greppable markers; snapshot HERE + optional ENTRY-COUNT. No arena / real forget.
\ Forth mirrors: marker-create / marker-restore (host binds MARKER; restore via named word or marker-restore).

create (mk-ckpt) 4 c, char c c, char k c, char p c, char t c,
create (mk-tmp) 3 c, char t c, char m c, char p c,

\ marker-create-from ( c-addr u -- i )  named restore mark; capture HERE + ENTRY-COUNT first
: marker-create-from ( c-addr u -- i )
  _here @ >r
  ENTRY-COUNT @ >r
  2dup entry-create-from dup 0< if
    nip nip r> drop r> drop exit
  then
  >r
  1 r@ entry-marker?[] !
  r@
  r> drop
  r> over entry-marker-dict[] !
  r> over entry-marker-here[] !
  dup _latest !
  ." [marker] MARKER name=" rot rot type
  ."  here=" dup entry-marker-here[] @ .
  ." dict=" dup entry-marker-dict[] @ . cr ;

\ marker-create ( "name" -- )  parse + MARKER stub
: marker-create ( "name" -- )
  bl word count marker-create-from drop ;

\ marker-restore-from ( c-addr u -- )  restore HERE + optional ENTRY-COUNT from named mark
: marker-restore-from ( c-addr u -- )
  2dup entry-find dup 0< if
    drop 2drop
    ." [marker] FAIL reason=miss" cr exit
  then
  dup entry-marker?[] @ 0= if
    drop 2drop
    ." [marker] FAIL reason=miss" cr exit
  then
  >r 2drop
  r@ entry-marker-here[] @ _here !
  r@ entry-marker-dict[] @ dup 0< if
    drop
  else
    ENTRY-COUNT @ min ENTRY-COUNT !
  then
  ." [marker] RESTORE name=" r@ entry-name[] NAMELEN name-trim type
  ."  here=" r@ entry-marker-here[] @ .
  ." dict=" r@ entry-marker-dict[] @ . cr
  r> drop
  ENTRY-COUNT @ dup 0= if drop -1 else 1- then _latest ! ;

\ marker-restore ( "name" -- )  parse + RESTORE stub
: marker-restore ( "name" -- )
  bl word count marker-restore-from ;

\ marker-demo ( -- )  dict-reset → MARKER ckpt → ALLOT + entry → RESTORE → OK
: marker-demo ( -- )
  ." [marker-demo] dict-reset + MARKER ckpt + ALLOT + RESTORE" cr
  dict-reset
  (mk-ckpt) count marker-create-from drop
  _here @ >r
  8 allot-bump
  (mk-tmp) count entry-create-from drop
  _here @ r@ > 0= if
    r> drop ." [marker-demo] FAIL" cr exit
  then
  (mk-ckpt) count marker-restore-from
  _here @ r> <> if
    ." [marker-demo] FAIL" cr exit
  then
  (mk-ckpt) count entry-find 0< 0= if
    ." [marker-demo] FAIL" cr exit
  then
  (mk-tmp) count entry-find 0< 0= if
    ." [marker-demo] FAIL" cr exit
  then
  ." [marker-demo] OK" cr ;


\ === BUFFER: named buffer-slot stubs (wave16 item 3) ===
\ Prefer greppable markers; named size+offset into fill cap (cap=64). Not an arena / ALLOCATE.
\ Forth mirrors: buffer-colon / buffer-fetch (host binds BUFFER: ; fetch via named word or buffer-fetch).
\ Do NOT redefine the wave15 FILL host buffer — slots are markers over that cap.

variable _bf-n
create (bf-buf) 3 c, char b c, char u c, char f c,

\ buffer-colon-from ( n c-addr u -- i )  named buffer slot of size n; offset bump in fill cap
: buffer-colon-from ( n c-addr u -- i )
  rot _bf-n !                       \ c-addr u ; n saved
  _bf-n @ 0 <= if
    2drop
    ." [buffer] FAIL reason=bounds" cr -1 exit
  then
  _buffer-next @ _bf-n @ + _fill-cap > if
    2drop
    ." [buffer] FAIL reason=bounds" cr -1 exit
  then
  2dup entry-create-from dup 0< if
    nip nip exit
  then
  >r 2drop                          \ | R: idx
  1 r@ entry-buffer?[] !
  _bf-n @ r@ entry-buffer-n[] !
  _buffer-next @ r@ entry-buffer-addr[] !
  _bf-n @ _buffer-next +!
  r@ _latest !
  ." [buffer] BUFFER: name=" r@ entry-name[] NAMELEN name-trim type
  ."  n=" r@ entry-buffer-n[] @ .
  ." addr=" r@ entry-buffer-addr[] @ . cr
  r> ;

\ buffer-colon ( n "name" -- )  parse + BUFFER: stub
: buffer-colon ( n "name" -- )
  bl word count buffer-colon-from drop ;

\ buffer-fetch-from ( c-addr u -- )  fetch/execute named buffer slot
: buffer-fetch-from ( c-addr u -- )
  2dup entry-find dup 0< if
    drop 2drop
    ." [buffer] FAIL reason=miss" cr exit
  then
  dup entry-buffer?[] @ 0= if
    drop 2drop
    ." [buffer] FAIL reason=miss" cr exit
  then
  >r 2drop
  ." [buffer] addr=" r@ entry-buffer-addr[] @ .
  ." name=" r@ entry-name[] NAMELEN name-trim type cr
  r> drop ;

\ buffer-fetch ( "name" -- )  parse + fetch stub
: buffer-fetch ( "name" -- )
  bl word count buffer-fetch-from ;

\ buffer-demo ( -- )  dict-reset → 8 BUFFER: buf → fetch → OK
: buffer-demo ( -- )
  ." [buffer-demo] dict-reset + 8 BUFFER: buf + fetch" cr
  dict-reset
  8 (bf-buf) count buffer-colon-from drop
  (bf-buf) count entry-find 0< if
    ." [buffer-demo] FAIL" cr exit
  then
  (bf-buf) count buffer-fetch-from
  ." [buffer-demo] OK" cr ;


\ === EXIT / QUIT thin control markers (wave16 item 4) ===
\ Prefer greppable markers; flag + marker only. NOT real RS unwind / interpret restart.
\ Forth mirrors: exit-mark / quit-mark — do NOT redefine host exit / quit
\ (in-tree early-returns across kernel.fs / rekia.fs stay the host primitive).

\ exit-mark ( -- )  print [exit] EXIT + optional depth= (catch-depth stub); no hard abort
: exit-mark ( -- )
  ." [exit] EXIT depth=" _catch-depth @ . cr ;

\ quit-mark ( -- )  print [exit] QUIT; interpret-reset stub mark-only (no flag clear)
: quit-mark ( -- )
  ." [exit] QUIT" cr ;

\ exit-demo ( -- )  dict-reset → exit-mark → quit-mark → OK
: exit-demo ( -- )
  ." [exit-demo] dict-reset + exit-mark + quit-mark" cr
  dict-reset
  exit-mark
  quit-mark
  ." [exit-demo] OK" cr ;


\ === SYNONYM / ALIAS name-map stubs (wave17 item 1) ===
\ Prefer greppable markers; name→name side-table only. No linked XT / FIND rewrite / execute-through.
\ Forth mirrors: synonym-map / alias-map / synonym-resolve
\ Host binds SYNONYM / ALIAS; shared map table.

create (sy-widget) 6 c, char w c, char i c, char d c, char g c, char e c, char t c,
create (sy-gadget) 6 c, char g c, char a c, char d c, char g c, char e c, char t c,
create (sy-widget2) 7 c, char w c, char i c, char d c, char g c, char e c, char t c, char 2 c,

\ syn-name! ( c-addr u dest -- )  store name into NAMELEN slot
: syn-name! ( c-addr u dest -- )
  >r r@ NAMELEN bl fill
  NAMELEN min
  r> swap cmove ;

\ synonym-find ( c-addr u -- i )  find new-name in map; -1 if missing
: synonym-find ( c-addr u -- i )
  SYN-COUNT @ 0 ?do
    2dup i syn-new[] NAMELEN name-trim cstr= if
      2drop i unloop exit
    then
  loop
  2drop -1 ;

\ synonym-map-from ( na nu oa ou -- )  record new→old; print SYNONYM marker
: synonym-map-from ( na nu oa ou -- )
  2dup entry-find 0< if
    2drop 2drop
    ." [synonym] FAIL reason=miss" cr exit
  then
  2swap                                   \ oa ou na nu
  2dup synonym-find dup 0< if
    drop
    SYN-COUNT @ MAX-SYNONYMS >= if
      2drop 2drop
      ." [synonym] FAIL reason=miss" cr exit
    then
    SYN-COUNT @ >r
    r@ syn-new[] syn-name!                \ na nu → new slot (consumes)
    r@ syn-old[] syn-name!                \ oa ou → old slot
    1 SYN-COUNT +!
    ." [synonym] SYNONYM new=" r@ syn-new[] NAMELEN name-trim type
    ."  old=" r@ syn-old[] NAMELEN name-trim type cr
    r> drop
  else
    >r                                    \ idx ; stack: oa ou na nu
    2drop                                 \ drop na nu (already mapped)
    r@ syn-old[] syn-name!                \ update old
    ." [synonym] SYNONYM new=" r@ syn-new[] NAMELEN name-trim type
    ."  old=" r@ syn-old[] NAMELEN name-trim type cr
    r> drop
  then ;

\ synonym-map ( "new" "old" -- )  parse two names + SYNONYM stub
: synonym-map ( "new" "old" -- )
  bl word count bl word count synonym-map-from ;

\ alias-map-from ( na nu oa ou -- )  same map; print ALIAS marker
: alias-map-from ( na nu oa ou -- )
  2dup entry-find 0< if
    2drop 2drop
    ." [synonym] FAIL reason=miss" cr exit
  then
  2swap
  2dup synonym-find dup 0< if
    drop
    SYN-COUNT @ MAX-SYNONYMS >= if
      2drop 2drop
      ." [synonym] FAIL reason=miss" cr exit
    then
    SYN-COUNT @ >r
    r@ syn-new[] syn-name!
    r@ syn-old[] syn-name!
    1 SYN-COUNT +!
    ." [synonym] ALIAS new=" r@ syn-new[] NAMELEN name-trim type
    ."  old=" r@ syn-old[] NAMELEN name-trim type cr
    r> drop
  else
    >r
    2drop
    r@ syn-old[] syn-name!
    ." [synonym] ALIAS new=" r@ syn-new[] NAMELEN name-trim type
    ."  old=" r@ syn-old[] NAMELEN name-trim type cr
    r> drop
  then ;

\ alias-map ( "new" "old" -- )  parse two names + ALIAS stub
: alias-map ( "new" "old" -- )
  bl word count bl word count alias-map-from ;

\ synonym-resolve-from ( c-addr u -- )  lookup/print bound old; no XT execute
: synonym-resolve-from ( c-addr u -- )
  2dup synonym-find dup 0< if
    drop 2drop
    ." [synonym] FAIL reason=miss" cr exit
  then
  >r 2drop
  ." [synonym] resolve new=" r@ syn-new[] NAMELEN name-trim type
  ."  old=" r@ syn-old[] NAMELEN name-trim type cr
  r> drop ;

\ synonym-resolve ( "new" -- )  parse + resolve stub
: synonym-resolve ( "new" -- )
  bl word count synonym-resolve-from ;

\ synonym-demo ( -- )  dict-reset → create old → SYNONYM → resolve → ALIAS → OK
: synonym-demo ( -- )
  ." [synonym-demo] dict-reset + SYNONYM widget gadget + resolve" cr
  dict-reset
  (sy-gadget) count entry-create-from drop
  (sy-gadget) count entry-find 0< if
    ." [synonym-demo] FAIL" cr exit
  then
  (sy-widget) count (sy-gadget) count synonym-map-from
  (sy-widget) count synonym-resolve-from
  (sy-widget2) count (sy-gadget) count alias-map-from
  (sy-widget2) count synonym-resolve-from
  ." [synonym-demo] OK" cr ;


\ === Next steps (from refs) ===
\ - Grow dict toward Dusk units / linked entries (mem/dict.fs)
\ - Full colon compiler / real branch XT (beyond IF/THEN/ELSE + loop + do-loop stubs)
\ - Group-scoped entries via ENTRY-GIDS + group-entry-find (wave4 item 2) — landed
\ - findentry / find aliases + interpret-token stub (wave7 item 5) — landed
\ - interpret loop deepen + : create-only (wave8 item 1) — landed
\ - colon body/marker stub (wave9 item 4) — landed
\ - IF/THEN/ELSE control stubs (wave10 item 2) — landed
\ - BEGIN/UNTIL/WHILE/REPEAT loop stubs (wave11 item 1) — landed
\ - DO/LOOP/+LOOP/I do-loop stubs (wave12 item 1) — landed
\ - WORDS / words-demo dict-list smoke (wave11 item 3) — landed
\ - VARIABLE/CONSTANT named-cell stubs (wave12 item 2) — landed
\ - comment-parse \\ / ( ) skip + comment-demo (wave12 item 3) — landed
\ - LEAVE/AGAIN stubs + leave-demo (wave12 item 4) — landed
\ - VALUE/TO named mutable-cell stubs (wave13 item 1) — landed
\ - CASE/OF/ENDOF/ENDCASE stubs (wave13 item 2) — landed
\ - CREATE/DOES> defining-word stubs (wave13 item 3) — landed
\ - S" / ." / .( string-lit stubs + string-demo (wave13 item 4) — landed
\ - HERE/ALLOT pointer stubs + allot-demo (wave14 item 1) — landed
\ - UNLOOP/J stubs + unloop-demo (wave14 item 2) — landed
\ - 2VARIABLE/2CONSTANT stubs + 2var-demo (wave14 item 3) — landed
\ - CATCH/THROW stubs + throw-demo (wave14 item 4) — landed
\ - CELL/CELLS/ALIGN/ALIGNED stubs + cell-demo (wave15 item 1) — landed
\ - PICK/ROLL/DEPTH/?DUP stubs + pick-demo (wave15 item 2) — landed
\ - FILL/ERASE/MOVE/CMOVE stubs + fill-demo (wave15 item 3) — landed
\ - IMMEDIATE/POSTPONE stubs + imm-demo (wave15 item 4) — landed
\ - DEFER/IS/ACTION-OF stubs + defer-demo (wave16 item 1) — landed
\ - MARKER restore-mark stubs + marker-demo (wave16 item 2) — landed
\ - BUFFER: named-buffer stubs + buffer-demo (wave16 item 3) — landed
\ - EXIT/QUIT thin control markers + exit-demo (wave16 item 4) — landed
\ - SYNONYM/ALIAS name-map stubs + synonym-demo (wave17 item 1) — landed
\ - Use struct for neuron records
\ - Make R.E.K.I.A. a code emitter like comp/c.fs

\ Include the basic trit words (from poly or forth/)
\ include trit.fs     \ (adjust path when bundled)

." Tritium kernel loaded (minimal dict)." cr
