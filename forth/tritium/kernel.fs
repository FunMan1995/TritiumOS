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
variable ENTRY-COUNT  0 ENTRY-COUNT !

: entry-name[] ( i -- c-addr ) NAMELEN * ENTRY-NAMES + ;
: entry-id[]   ( i -- addr )   cells ENTRY-IDS + ;
: entry-gid[]  ( i -- addr )   cells ENTRY-GIDS + ;
: entry-body[] ( i -- addr )   cells ENTRY-BODY + ;
: entry-btoks[] ( i -- addr )  cells ENTRY-BTOKS + ;
: entry-cell[] ( i -- addr )   cells ENTRY-CELLS + ;
: entry-value?[] ( i -- addr ) cells ENTRY-VALUE? + ;
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
\ case-stack stub (wave13 item 2) — sibling of cs; balance-only CASE/OF
variable _case-depth  \ open CASE count


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
  0 _colon-def !
  -1 _colon-idx !
  0 _colon-toks !
  0 _cs-depth !
  0 _cs-side !
  0 _loop-depth !
  0 _do-loop-depth !
  0 _do-loop-index !
  0 _case-depth ! ;

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
\ - Use struct for neuron records
\ - Make R.E.K.I.A. a code emitter like comp/c.fs

\ Include the basic trit words (from poly or forth/)
\ include trit.fs     \ (adjust path when bundled)

." Tritium kernel loaded (minimal dict)." cr
