# KERNEL — Minimal dict / find flesh

**Status:** Shipper-ready stub spec (wave7 item **5**)  
**Canonical brief:** `TritiumOS.txt` Phase 2; Dusk `fs/doc/kernel.txt` + `fs/mem/dict.fs` (see `docs/FORTH-BASE-REFERENCES.md`)  
**Sources of truth (code):** `forth/tritium/kernel.fs`; Linux host mirrors for demos  
**Companions:** `docs/GROUPS-NESTED.md` (ENTRY-GIDS), `docs/ARCHITECTURE.md`, `docs/NEURON.md`, `docs/COLON.md` (wave9 **4**), `docs/CONTROL.md` (wave10 **2**); `docs/WORDS-VOCAB.md` (wave11 **3**); `docs/VARIABLE-CONST.md` (wave12 **2**); `docs/COMMENT-PARSE.md` (wave12 **3**); `docs/VALUE-TO.md` (wave13 **1**); `docs/CREATE-DOES.md` (wave13 **3**); `docs/ALLOT-HERE.md` (wave14 **1**); `docs/2VARIABLE.md` (wave14 **3**); `docs/THROW-CATCH.md` (wave14 **4**); `docs/CELL-CELLS.md` (wave15 **1**); `docs/PICK-ROLL.md` (wave15 **2**); `docs/FILL-MOVE.md` (wave15 **3**); `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**); `docs/DEFER-IS.md` (wave16 **1**); `docs/MARKER.md` (wave16 **2**); `docs/BUFFER-COLON.md` (wave16 **3**); `docs/EXIT-QUIT.md` (wave16 **4**); `docs/SYNONYM-ALIAS.md` (wave17 **1**); `docs/PARSE-NAME.md` (wave17 **2**); `docs/EVALUATE-INCLUDE.md` (wave17 **3**); `docs/RECURSE.md` (wave17 **4**); `docs/TICK.md` (wave18 **1**); `docs/FIND.md` (wave18 **2**); `docs/WORD-BL.md` (wave18 **3**); `docs/STATE-COMPILE.md` (wave18 **4**); `docs/CHAR-CHARS.md` (wave19 **1**); `docs/TO-BODY.md` (wave19 **2**)
**See also:** `docs/INTERPRET.md` (wave8 **1** — interpret loop deepen); `docs/COLON.md` (wave9 **4** — colon body/marker stub); `docs/COMMENT-PARSE.md` (wave12 **3** — `\` / `(` comment skip); `docs/PARSE-NAME.md` (wave17 **2** — PARSE/PARSE-NAME token-parse markers); `docs/EVALUATE-INCLUDE.md` (wave17 **3** — EVALUATE/INCLUDE mark-only echo; refined-boot stays); `docs/RECURSE.md` (wave17 **4** — RECURSE mark-only; recurse-mark; not self-XT); `docs/TICK.md` (wave18 **1** — `'` / `[']` name→stub-xt-id mark; tick-mark / bracket-tick; not XT execute); `docs/FIND.md` (wave18 **2** — ANS-ish FIND mark; find-xt / find-mark; host find/findentry/entry-find untouched); `docs/WORD-BL.md` (wave18 **3** — WORD/BL stub markers; word-parse / bl-char; host WORDS untouched; fixed word-buffer only); `docs/STATE-COMPILE.md` (wave18 **4** — STATE/COMPILE, stub markers; state-flag / compile-comma; not linked XT / real STATE cell); `docs/CHAR-CHARS.md` (wave19 **1** — CHAR/CHARS/[CHAR] char-unit mark; char-unit / chars-n / bracket-char; not CHAR+/unicode / ALIGN reopen); `docs/TO-BODY.md` (wave19 **2** — >BODY CREATE-body address mark; to-body; not real DOES> XT / body image / linked XT).

## 1. Purpose

`kernel.fs` already has a flat name table (`entry-create` / `entry-find` / `group-entry-*` / `cold-boot` / soft `abort`). This tip **fleshes** the Dusk-aligned surface so Lab can smoke a dedicated `kernel-demo` without breaking the suite:

- Documented **find** aliases (`findentry` / `find`)
- Tiny **interpret stub** (lookup-only; no full control-flow compiler)
- Explicit **demo** + markers
- Demoes stay green (groups nested, rekia, license, …)

Not a full VM, linked dict, or xcomp.

## 2. Contract (keep + add)

| Word | Stack | Notes |
|------|-------|-------|
| `entry-create` | `( "name" -- )` | Existing; global gid=-1 |
| `entry-find` | `( c-addr u -- i )` | Existing; `-1` miss |
| `findentry` | `( c-addr u -- i )` | **Alias** of `entry-find` (Dusk name) |
| `find` | `( c-addr u -- i )` | Same alias for host smoke |
| `group-entry-find` / `group-entry-create` | (existing) | Used by GROUPS / nested |
| `words` / `.words` | `( -- )` | List table |
| `.words` / `WORDS` / `words` | `( -- )` | List flat names — see `WORDS-VOCAB.md` (wave11 **3**) |
| `words-demo` | `( -- )` | Create ≥2 + list → OK (`WORDS-VOCAB.md`) |
| `VARIABLE` / `CONSTANT` | (see tip) | Named-cell stubs — see `VARIABLE-CONST.md` (wave12 **2**) |
| `var-demo` | `( -- )` | ≥1 VARIABLE + ≥1 CONSTANT → OK (`VARIABLE-CONST.md`) |
| `VALUE` / `TO` | (see tip) | Named mutable-cell stubs — see `VALUE-TO.md` (wave13 **1**) |
| `value-demo` | `( -- )` | VALUE + TO → OK (`VALUE-TO.md`) |
| `CREATE` / `DOES>` | (see tip) | Defining-word stubs — see `CREATE-DOES.md` (wave13 **3**) |
| `create-demo` | `( -- )` | CREATE + DOES> → OK (`CREATE-DOES.md`) |
| `HERE` / `ALLOT` | (see tip) | Dictionary-pointer stubs — see `ALLOT-HERE.md` (wave14 **1**) |
| `allot-demo` | `( -- )` | HERE + ALLOT bump → OK (`ALLOT-HERE.md`) |
| `2VARIABLE` / `2CONSTANT` | (see tip) | Double-cell named stubs — see `2VARIABLE.md` (wave14 **3**) |
| `2var-demo` | `( -- )` | ≥1 2VARIABLE + ≥1 2CONSTANT → OK (`2VARIABLE.md`) |
| `CATCH` / `THROW` | (see tip) | Exception-frame stubs — see `THROW-CATCH.md` (wave14 **4**) |
| `throw-demo` | `( -- )` | CATCH + THROW → OK (`THROW-CATCH.md`) |
| `CELL` / `CELLS` / `ALIGN` / `ALIGNED` | (see tip) | Dictionary-unit stubs — see `CELL-CELLS.md` (wave15 **1**) |
| `cell-demo` | `( -- )` | CELL + CELLS + align round-up → OK (`CELL-CELLS.md`) |
| `PICK` / `ROLL` / `DEPTH` / `?DUP` | (see tip) | Stack-marker stubs — see `PICK-ROLL.md` (wave15 **2**); Forth mirrors `pick-nth` / `roll-nth` / `stack-depth` / `qdup` |
| `pick-demo` | `( -- )` | DEPTH + PICK + ROLL + ?DUP → OK (`PICK-ROLL.md`) |
| `FILL` / `ERASE` / `MOVE` / `CMOVE` | (see tip) | Fixed host-buffer stubs — see `FILL-MOVE.md` (wave15 **3**); Forth mirrors `fill-buf` / `erase-buf` / `move-buf` / `cmove-buf` (do not redefine kernel `cmove`) |
| `fill-demo` | `( -- )` | FILL + ERASE + MOVE/CMOVE → OK (`FILL-MOVE.md`) |
| `IMMEDIATE` / `POSTPONE` | (see tip) | Compile-only flag stubs — see `IMMEDIATE-POSTPONE.md` (wave15 **4**); Forth mirrors `immediate-mark` / `postpone-mark` |
| `imm-demo` | `( -- )` | IMMEDIATE + POSTPONE → OK (`IMMEDIATE-POSTPONE.md`) |
| `DEFER` / `IS` / `ACTION-OF` | (see tip) | Deferred-word stubs — see `DEFER-IS.md` (wave16 **1**); Forth mirrors `defer-create` / `is-bind` / `action-of-xt` (**do not** redefine rekia `defer` / `is`) |
| `defer-demo` | `( -- )` | DEFER + IS + ACTION-OF → OK (`DEFER-IS.md`) |
| `MARKER` / `marker-create` | (see tip) | Dictionary-restore stubs — see `MARKER.md` (wave16 **2**); restore via named marker word or optional `marker-restore` (snapshot mark only — not real forget) |
| `marker-demo` | `( -- )` | MARKER + RESTORE → OK (`MARKER.md`) |
| `BUFFER:` / `buffer-colon` | (see tip) | Named buffer stubs — see `BUFFER-COLON.md` (wave16 **3**); Forth mirror `buffer-colon` (named slot over HERE bump and/or fill cap — **not** an arena; do not redefine FILL host buffer) |
| `buffer-demo` | `( -- )` | BUFFER: + fetch → OK (`BUFFER-COLON.md`) |
| `EXIT` / `exit-mark` | (see tip) | Thin control markers — see `EXIT-QUIT.md` (wave16 **4**); Forth mirrors `exit-mark` / `quit-mark` (**do not** redefine host `exit` / `quit` used across kernel.fs / rekia.fs) |
| `QUIT` / `quit-mark` | (see tip) | Interpret-reset stub mark — see `EXIT-QUIT.md` (wave16 **4**); prefer mark-only |
| `exit-demo` | `( -- )` | EXIT + QUIT → OK (`EXIT-QUIT.md`) |
| `SYNONYM` / `ALIAS` | (see tip) | Name-map stubs — see `SYNONYM-ALIAS.md` (wave17 **1**); Forth mirrors `synonym-map` / `alias-map` (name→name only — **not** FIND rewrite / linked XT) |
| `synonym-demo` | `( -- )` | SYNONYM/ALIAS + resolve → OK (`SYNONYM-ALIAS.md`) |
| `PARSE-NAME` / `parse-name` | (see tip) | Whitespace-delimited stub token markers — see `PARSE-NAME.md` (wave17 **2**); Forth mirrors `parse-name` / `parse-delim` (marker deepen only — **not** full parser VM / WORD/BL rewrite) |
| `PARSE` / `parse-delim` | (see tip) | Delimiter-form stub token markers — see `PARSE-NAME.md` (wave17 **2**) |
| `parse-demo` | `( -- )` | PARSE-NAME + PARSE → OK (`PARSE-NAME.md`) |
| `EVALUATE` / `evaluate-mark` | (see tip) | Mark-only nested-interpret echo — see `EVALUATE-INCLUDE.md` (wave17 **3**); Forth mirrors `evaluate-mark` / `include-mark` (echo only — **not** nested interpret / file VM; **do not** redefine host/poly `include` or refined-boot load path) |
| `INCLUDE` / `include-mark` | (see tip) | Mark-only include echo — see `EVALUATE-INCLUDE.md` (wave17 **3**) |
| `eval-demo` | `( -- )` | EVALUATE + INCLUDE → OK (`EVALUATE-INCLUDE.md`) |
| `RECURSE` / `recurse-mark` | (see tip) | Flag + marker only — see `RECURSE.md` (wave17 **4**); Forth mirror `recurse-mark` (optional `name=` / `depth=` — **not** a self-XT / recursive colon body; **do not** break colon / redefine host `exit`) |
| `recurse-demo` | `( -- )` | RECURSE → OK (`RECURSE.md`) |
| `'` / `tick-mark` | `( "name" -- )` | Name→stub-xt-id mark — see `TICK.md` (wave18 **1**); Forth mirrors `tick-mark` / `bracket-tick` (optional `xt=` / `i=` — **not** XT execute / FIND rewrite / COMPILE,; **do not** redefine host tick / Android wordlists) |
| `[']` / `bracket-tick` | `( "name" -- )` | Compile-time sibling mark — see `TICK.md` (wave18 **1**); flag echo only — does **not** compile XT into body |
| `tick-demo` | `( -- )` | `'` + `[']` → OK (`TICK.md`) |
| `FIND` / `find-xt` | `( c-addr -- )` *or* `( "name" -- )` | ANS-ish find mark — see `FIND.md` (wave18 **2**); Forth mirrors `find-xt` / `find-mark` (optional `xt=` / `flag=<1|-1|0>` / `i=` — **not** SEARCH-WORDLIST / linked dict / execute-through / SYNONYM FIND rewrite; **do not** redefine host `find` / `findentry` / `entry-find`) |
| `find-demo` | `( -- )` | FIND / find-xt → OK (`FIND.md`) |
| `BL` / `bl-char` | `( -- )` *or* `( -- char )` | Blank char mark — see `WORD-BL.md` (wave18 **3**); Forth mirrors `word-parse` / `bl-char` (optional `char=32` / `u=` — **not** interpret splitter rewrite / SOURCE/PAD; **do not** redefine host `WORDS` / any host `WORD`) |
| `WORD` / `word-parse` | `( delim -- )` *or* `( delim -- c-addr )` | Delimiter-form stub token into fixed host word-buffer / pad slot — see `WORD-BL.md` (wave18 **3**); optional `tok=` / `u=` / `addr=` — **not** a heap / interpret rewrite |
| `word-demo` | `( -- )` | WORD + BL → OK (`WORD-BL.md`) |
| `STATE` / `state-flag` | `( -- )` *or* `( -- n )` | Compile-state query mark — see `STATE-COMPILE.md` (wave18 **4**); Forth mirrors `state-flag` / `compile-comma` (optional `flag=<0|1>` / `n=` — interpret=0 / compile=1 picture only; **not** linked XT / FIND-then-compile / real STATE cell / POSTPONE reopen; **do not** redefine host assistant-state / host EXIT) |
| `COMPILE,` / `compile-comma` | `( xt -- )` *or* `( "name" -- )` | Compile-comma mark — see `STATE-COMPILE.md` (wave18 **4**); optional `xt=` / `name=` — does **not** append XT to body list or execute it; optional `[state] compile-only` when colon-def flag set |
| `state-demo` | `( -- )` | STATE + COMPILE, → OK (`STATE-COMPILE.md`) |
| `CHAR` / `char-unit` | `( "c" -- )` *or* `( -- )` | Char-unit mark — see `CHAR-CHARS.md` (wave19 **1**); Forth mirrors `char-unit` / `chars-n` / `bracket-char` (optional `char=` / `u=` — **not** CHAR+/unicode / ALIGN reopen / HERE bump; **do not** redefine host CHAR/CHARS/[CHAR]) |
| `CHARS` / `chars-n` | `( k -- bytes )` *or* `( k -- )` | Char-scale mark — see `CHAR-CHARS.md` (wave19 **1**); Linux SoT char-size=1 → `bytes=n`; does **not** bump HERE |
| `[CHAR]` / `bracket-char` | `( "c" -- )` *or* `( -- )` | Compile-time sibling mark — see `CHAR-CHARS.md` (wave19 **1**); flag echo only — does **not** compile a char literal into a body |
| `char-demo` | `( -- )` | CHAR + CHARS + [CHAR] → OK (`CHAR-CHARS.md`) |
| `>BODY` / `to-body` | `( xt -- addr )` *or* `( "name" -- )` | CREATE-body address mark — see `TO-BODY.md` (wave19 **2**); Forth mirror `to-body` (optional `name=` / `addr=` / `xt=` — **not** real DOES> XT / linked XT / body image / HERE bump; **do not** redefine host `>BODY`) |
| `body-demo` | `( -- )` | >BODY → OK (`TO-BODY.md`) |
| `dict-reset` | `( -- )` | Empty table |
| `cold-boot` | `( -- )` | sysvars + dict-reset + edition default + markers |
| `abort` / `(abort")` | soft | Existing; no hard exit; CATCH/THROW mark stubs → `THROW-CATCH.md` (wave14 **4**); EXIT/QUIT thin control markers → `EXIT-QUIT.md` (wave16 **4**) |
| `interpret-token` | `( c-addr u -- flag )` | **New stub:** `entry-find` ≥0 → true + print hit; else false + soft miss line |
| `kernel-demo` | `( -- )` | See §4 |

Caps: keep `MAX-ENTRIES` ≥ **32** (bump only if nested+demo pressure; document if changed).

## 3. Markers

```
[kernel] cold-boot OK (64-bit edition)   \ or 32-bit
[kernel] interpret-ready
[kernel] created #N
[kernel] find hit #N name=<word>
[kernel] find miss
[kernel-demo] OK
[kernel-demo] FAIL
```

`cold-boot` must remain safe for poly/AppImage load (no infinite abort).

## 4. `kernel-demo`

1. `dict-reset` (or rely on fresh cold-boot path)
2. Create two names via `entry-create` / host equivalent
3. `find` / `findentry` hits both; miss on unknown → miss marker
4. `words` lists them
5. `interpret-token` on a known name → true
6. Print `[kernel-demo] OK`

Linux host SoT preferred (mirror table already used for group entries); Forth words in `kernel.fs` remain the poly contract.

## 5. Non-goals (this tip)

- Full SEARCH-WORDLIST / linked dict — list-only `WORDS` is wave11 **3** (`WORDS-VOCAB.md`)
- Named-cell stubs (`VARIABLE`/`CONSTANT`) → `docs/VARIABLE-CONST.md` (wave12 **2**); VALUE/TO stubs → `docs/VALUE-TO.md` (wave13 **1**); CREATE/DOES> stubs → `docs/CREATE-DOES.md` (wave13 **3**); HERE/ALLOT pointer stubs → `docs/ALLOT-HERE.md` (wave14 **1**); double-cell stubs → `docs/2VARIABLE.md` (wave14 **3**); CATCH/THROW stubs → `docs/THROW-CATCH.md` (wave14 **4**); dictionary-unit stubs (`CELL`/`CELLS`/`ALIGN`/`ALIGNED`) → `docs/CELL-CELLS.md` (wave15 **1**); stack-marker stubs (`PICK`/`ROLL`/`DEPTH`/`?DUP`) → `docs/PICK-ROLL.md` (wave15 **2**; do not redefine host `2dup`/`2drop`/`2swap`); fixed host-buffer stubs (`FILL`/`ERASE`/`MOVE`/`CMOVE`) → `docs/FILL-MOVE.md` (wave15 **3**; do not redefine kernel `cmove`); IMMEDIATE/POSTPONE flag stubs → `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**; flag + name mark only); DEFER/IS/ACTION-OF deferred-word stubs → `docs/DEFER-IS.md` (wave16 **1**; Forth mirrors only — do not redefine rekia `defer`/`is`); MARKER restore-mark stubs → `docs/MARKER.md` (wave16 **2**; snapshot HERE + optional entry-count only — not real forget / arena rewind); BUFFER: named-buffer stubs → `docs/BUFFER-COLON.md` (wave16 **3**; size + offset into fill cap / HERE bump — not an arena / ALLOCATE; do not redefine FILL host buffer); EXIT/QUIT thin control markers → `docs/EXIT-QUIT.md` (wave16 **4**; Forth mirrors only — do not redefine host `exit`/`quit`; mark-only — not real RS unwind / interpret restart); SYNONYM/ALIAS name-map stubs → `docs/SYNONYM-ALIAS.md` (wave17 **1**; Forth mirrors `synonym-map`/`alias-map` — name→name only; not FIND rewrite / linked XT / executing aliased XT); PARSE/PARSE-NAME token-parse markers → `docs/PARSE-NAME.md` (wave17 **2**; Forth mirrors `parse-name`/`parse-delim` — marker deepen only; not full parser VM / WORD/BL rewrite); EVALUATE/INCLUDE mark-only echo → `docs/EVALUATE-INCLUDE.md` (wave17 **3**; Forth mirrors `evaluate-mark`/`include-mark` — echo only; not nested interpret / file VM; do not redefine host/poly `include` or refined-boot load path); RECURSE mark-only stub → `docs/RECURSE.md` (wave17 **4**; Forth mirror `recurse-mark` — flag + marker only; not real self-XT / recursive colon body; do not break colon / redefine host `exit`); TICK name→stub-xt-id mark → `docs/TICK.md` (wave18 **1**; Forth mirrors `tick-mark`/`bracket-tick` — name→stub-xt-id mark only; not XT execute / FIND rewrite / COMPILE,; do not redefine host tick); FIND deepen → `docs/FIND.md` (wave18 **2**; Forth mirrors `find-xt`/`find-mark` — ANS-ish find mark only; not SEARCH-WORDLIST / linked dict / execute-through / SYNONYM FIND rewrite; do not redefine host `find`/`findentry`/`entry-find`); WORD/BL stub markers → `docs/WORD-BL.md` (wave18 **3**; Forth mirrors `word-parse`/`bl-char` — stub token/pad only; not interpret splitter rewrite / SOURCE/PAD / heap; do not redefine host `WORDS`/any host `WORD`); STATE/COMPILE, stub markers → `docs/STATE-COMPILE.md` (wave18 **4**; Forth mirrors `state-flag`/`compile-comma` — query + compile-comma mark only; not linked XT / FIND-then-compile / real STATE cell / POSTPONE reopen; do not redefine host assistant-state / host EXIT); CHAR/CHARS/[CHAR] char-unit stubs → `docs/CHAR-CHARS.md` (wave19 **1**; Forth mirrors `char-unit`/`chars-n`/`bracket-char` — char-unit mark only; not CHAR+/unicode / ALIGN reopen / HERE bump; do not redefine host CHAR; `[CHAR]` flag echo only — does not compile char literal into body); >BODY CREATE-body address mark → `docs/TO-BODY.md` (wave19 **2**; Forth mirror `to-body` — address mark only; not real DOES> XT / linked XT / body image / HERE bump; do not redefine host `>BODY`); full arena / free / linked XT chaining / FLOAT / real exception RS unwind / executing postponed XT / executing bound XT / real RECURSE self-XT still later
- Full colon compiler / real branch XT (minimal `interpret` loop → `docs/INTERPRET.md`; comment skip → `docs/COMMENT-PARSE.md` wave12 **3**; `IF`/`THEN`/`ELSE` stubs → `docs/CONTROL.md`)
- Linked-list ENTRYSZ / forget / units objects (Dusk full)
- Replacing host C#/Kotlin VMs
- Breaking `group-find-nested` / vocab persist (share ENTRY-GIDS)

## 6. Acceptance (Test Lab)

1. `docs/KERNEL.md` present (Research byte-copy OK).
2. `kernel-demo` → OK; `find`/`findentry` miss path greppable; wave11 **3**: `words-demo` → OK; wave12 **2**: `var-demo` → OK; wave12 **3**: `comment-demo` → OK; wave13 **1**: `value-demo` → OK; wave13 **3**: `create-demo` → OK; wave14 **1**: `allot-demo` → OK; wave14 **3**: `2var-demo` → OK; wave14 **4**: `throw-demo` → OK; wave15 **1**: `cell-demo` → OK; wave15 **2**: `pick-demo` → OK; wave15 **3**: `fill-demo` → OK; wave15 **4**: `imm-demo` → OK; wave16 **1**: `defer-demo` → OK; wave16 **2**: `marker-demo` → OK; wave16 **3**: `buffer-demo` → OK; wave16 **4**: `exit-demo` → OK; wave17 **1**: `synonym-demo` → OK; wave17 **2**: `parse-demo` → OK; wave17 **3**: `eval-demo` → OK; wave17 **4**: `recurse-demo` → OK; wave18 **1**: `tick-demo` → OK; wave18 **2**: `find-demo` → OK; wave18 **3**: `word-demo` → OK; wave18 **4**: `state-demo` → OK; wave19 **1**: `char-demo` → OK; wave19 **2**: `body-demo` → OK.
3. Full regression green (esp. group-nested / group-vocab-persist / rekia / s3-reserved / cold path).
4. No merge.

## 7. Cite

- `TritiumOS.txt` Phase 2
- `docs/FORTH-BASE-REFERENCES.md` (Dusk kernel.txt / mem/dict.fs)
- `forth/tritium/kernel.fs`
- `docs/GROUPS-NESTED.md`, `docs/ARCHITECTURE.md`
- `docs/VARIABLE-CONST.md` (wave12 **2**)
- `docs/COMMENT-PARSE.md` (wave12 **3**)
- `docs/VALUE-TO.md` (wave13 **1**)
- `docs/CREATE-DOES.md` (wave13 **3**)
- `docs/ALLOT-HERE.md` (wave14 **1**)
- `docs/2VARIABLE.md` (wave14 **3**)
- `docs/THROW-CATCH.md` (wave14 **4**)
- `docs/CELL-CELLS.md` (wave15 **1**)
- `docs/PICK-ROLL.md` (wave15 **2**)
- `docs/FILL-MOVE.md` (wave15 **3**)
- `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**)
- `docs/DEFER-IS.md` (wave16 **1**)
- `docs/MARKER.md` (wave16 **2**)
- `docs/BUFFER-COLON.md` (wave16 **3**)
- `docs/EXIT-QUIT.md` (wave16 **4**)
- `docs/SYNONYM-ALIAS.md` (wave17 **1**)
- `docs/PARSE-NAME.md` (wave17 **2**)
- `docs/EVALUATE-INCLUDE.md` (wave17 **3**)
- `docs/RECURSE.md` (wave17 **4**)
- `docs/TICK.md` (wave18 **1**)
- `docs/FIND.md` (wave18 **2**)
- `docs/WORD-BL.md` (wave18 **3**)
- `docs/STATE-COMPILE.md` (wave18 **4**)
- `docs/CHAR-CHARS.md` (wave19 **1**)
- `docs/TO-BODY.md` (wave19 **2**)
