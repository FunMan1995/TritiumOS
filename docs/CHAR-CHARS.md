# CHAR-CHARS — `CHAR` / `CHARS` / `[CHAR]` char-unit stubs + `char-demo`

**Status:** Shipper-ready stub spec (wave19 item **1**)
**Canonical brief:** ANS-shaped `CHAR` / `CHARS` / `[CHAR]` (thin char-unit markers); `docs/CELL-CELLS.md` (wave15 **1** — cell-unit companion); `docs/KERNEL.md` (wave7 **5**); `docs/STRING-LIT.md` (wave13 **4**); optional `docs/WORD-BL.md` (wave18 **3**) / `docs/PARSE-NAME.md` (wave17 **2**); explicit WAVE18 / WAVE19 deferral closed as char-unit mark only (not CHAR+ / unicode / XCHAR / ALIGN reopen)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `char.fs`); Linux host REPL; **do not** redefine host `CHAR` / `CHARS` / `[CHAR]` that already bind on the load path
**Companions:** `docs/CELL-CELLS.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip), `docs/STRING-LIT.md` (thin amend this tip); optional light cite `docs/WORD-BL.md` / `docs/PARSE-NAME.md`
**Base tip SHA:** `e136713` (wave18 tip5 CLOSED / #86 DOCS-CITES) / full `e13671361caa98d2326ac8ad5542952438a58557`

## 1. Purpose

WAVE15 landed `CELL` / `CELLS` / `ALIGN` / `ALIGNED` as dictionary-unit stubs (`docs/CELL-CELLS.md`); WAVE18 / WAVE19 explicitly deferred `CHAR` / `CHARS` / `[CHAR]` (char-unit companion to CELL; not CHAR+ heap / unicode). This tip lands **stub** char-unit markers only: `CHAR` takes a demo character from the next token / fixture and prints `[char] CHAR` (+ optional `char=<c>` / `u=`); `CHARS` scales a count by char size (**1** on Linux SoT) and prints `[char] CHARS n=<k> bytes=<k>` (scale mark only — does **not** bump HERE); `[CHAR]` is the compile-time sibling mark — prints `[char] [CHAR]` (+ optional `char=` / `u=`) as flag/echo only (does **not** compile a char literal into a body). Smoke via **`char-demo`**. Forth mirrors **`char-unit` / `chars-n` / `bracket-char`** so host `CHAR`/`CHARS`/`[CHAR]` stay safe. **Not** CHAR+ required, not unicode / XCHAR, not ALIGN/ALIGNED reopen, not cell-size rewrite. Pairs with the CELL unit surface the way `CHARS` pairs with `CELLS`.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `CHAR` / `char-unit` | `( "c" -- )` *or* `( -- )` then parse next token | Char-unit mark; print `[char] CHAR` (+ optional `char=<c>` / `u=<ord>`); classic picture `CHAR A` — take first char of next token / fixture |
| `CHARS` / `chars-n` | `( k -- bytes )` *or* `( k -- )` | `bytes = k * char-size`; print `[char] CHARS n=<k> bytes=<k>` when char-size=1. Does **not** bump HERE |
| `[CHAR]` / `bracket-char` | `( "c" -- )` *or* `( -- )` then parse next token | Compile-time sibling mark; print `[char] [CHAR]` (+ optional `char=<c>` / `u=`); flag echo only — does **not** compile a char literal into a body |
| `char-demo` | `( -- )` | See §5 |

Host note: bind `CHAR` / `CHARS` / `[CHAR]` on the Linux REPL **only if** those names do not collide with host Forth char words in the same load path. Prefer Forth mirrors **`char-unit` / `chars-n` / `bracket-char`** as the Lab-facing surface when in doubt — **do not** redefine host char primitives. Char size is a host constant **1** on Linux SoT (document if a non-SoT host uses another size; Lab on Linux SoT expects `bytes=<k>` with `bytes == n`). Optional `char=` / `u=` are host char / ordinal echoes only — not unicode codepoints / XCHAR.

## 3. Stub semantics

- **Char size** is one host constant. Linux SoT: **`1`**. Print scale via `CHARS`; do not probe locale / unicode width / allocate a char. A non-SoT host must document its char size in the marker; Lab on Linux SoT expects `bytes=<k>` equal to `n`.
- **`CHAR` / `char-unit`:** take a demo character from the next token / fixture (classic `CHAR A` — first character of the following whitespace-delimited token, or a fixed demo fixture char). Print `[char] CHAR` and optionally `char=<c>` (printable form, e.g. `A`) and/or `u=<ord>` (ordinal, e.g. `65` for `A`). **Does not** bump HERE, compile into a body, allocate, or rewrite CELL size. Missing / empty next token → `[char] FAIL reason=empty` optional (demo **must avoid**).
- **`CHARS` / `chars-n`:** `bytes = k * char-size`. With char-size=1: `bytes = k`. Print `[char] CHARS n=<k> bytes=<k>` (when char-size=1). `k=0` → `bytes=0` (legal). `k<0` → `[char] FAIL reason=neg` (demo must avoid). **Does not** bump HERE. Optional later composition `k CHARS ALLOT` is **not** required for Lab OK.
- **`[CHAR]` / `bracket-char`:** same char resolve as compile-time sibling. Print `[char] [CHAR]` (+ optional `char=` / `u=`). Flag / char / ordinal echo only — does **not** append a char literal to a colon body, does not set STATE, does not execute. Optional `[char] compile-only` when colon-def flag is set is **not** required this tip.
- Storage: host char / ordinal + scale math only. **No** CHAR+ deepen, no unicode / XCHAR, no ALIGN/ALIGNED reopen, no cell-size rewrite, no HERE bump, no heap.
- Nest with prior cell / allot / string / word / parse / tick / find / state / synonym / exit / buffer / marker / defer / imm / fill / pick / throw / 2var / create / colon / control stubs OK. `dict-reset` unaffected (no new dict entries from char marks).
- Still no CHAR+ / unicode / XCHAR, no ALIGN reopen (wave15 CELL already landed), no TO-BODY (wave19 **2**), no ENVIRONMENT? (wave19 **3**), no SOURCE/PAD (wave19 **4**), no ACCEPT/REFILL, no linked XT / real DOES> XT / real branch XT / full arena/heap / full Win/Android Forth VM. Those stay non-goals / later tips.

## 4. Markers

```
[char] CHAR [char=<c>] [u=<ord>]           # char=/u= optional; classic CHAR A
[char] CHARS n=<k> bytes=<k>               # Linux SoT char-size=1 → bytes=n
[char] [CHAR] [char=<c>] [u=<ord>]         # char=/u= optional; compile-time sibling mark only
[char] FAIL reason=<…>                     # empty / neg (demo avoids)
[char-demo] OK
[char-demo] FAIL
```

Lab greps `[char-demo] OK` plus at least one `[char] CHAR` (optional `char=` / `u=` welcome), one `[char] CHARS n=` with `bytes=`, and one `[char] [CHAR]` (optional `char=` / `u=` welcome). Demo avoids `[char] FAIL reason=empty` / `reason=neg`.

## 5. `char-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; char marks need no dict entries.
2. Invoke `CHAR` (or `char-unit`) on a known demo char (e.g. classic `CHAR A` / fixture `A`) → `[char] CHAR` (+ optional `char=A` / `u=65`).
3. Invoke `CHARS` (or `chars-n`) with a positive `k` (e.g. `3 CHARS` or `3` then `CHARS`) → `[char] CHARS n=3 bytes=3` on Linux SoT (`bytes` must equal `k * char-size`; char-size=1 → `bytes=3`). Assert HERE was **not** bumped (marker-only is enough; optional HERE echo before/after welcome).
4. Invoke `[CHAR]` (or `bracket-char`) on a known demo char (same or another) → `[char] [CHAR]` (+ optional `char=` / `u=`). Assert **no** char literal was compiled into a body (marker alone is enough).
5. Assert no `[char] FAIL reason=empty` / `reason=neg` on the happy path. Assert char marks did **not** require CHAR+ / unicode / ALIGN reopen / cell-size rewrite (marker-only is enough).
6. Prior `state-demo` / `word-demo` / `find-demo` / `tick-demo` / `recurse-demo` / `eval-demo` / `parse-demo` / `synonym-demo` / `exit-demo` / `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` / `refined-boot-demo` / `host-boot-demo` / `interpret-demo` / `kernel-demo` still OK.
7. `[char-demo] OK`.

`CHAR` + `CHARS` + `[CHAR]` markers are required. Empty/neg FAIL paths are not exercised by the demo. Optional `char=` / `u=` echo is not required for Lab OK. No HERE bump. No char literal compiled into body. No CHAR+. No unicode. No ALIGN reopen.

## 6. Thin amend — companions

### `docs/CELL-CELLS.md`

- Companions / Status: add `CHAR-CHARS.md` (wave19 **1** companion cite); **keep** ALLOT-HERE / KERNEL / VARIABLE-CONST cites — do not wipe wave15 CELL content.
- Purpose / §3: cell-unit stubs stay; char-unit companion (`CHAR` / `CHARS` / `[CHAR]`) points at this tip — pairs the way `CHARS` pairs with `CELLS`; **not** an ALIGN/ALIGNED reopen / cell-size rewrite / CHAR+ / unicode.
- Non-goals: `CHAR` / `CHARS` / `[CHAR]` → `docs/CHAR-CHARS.md` (wave19 **1**). ALIGN/ALIGNED stay on this tip (already landed). CHAR+ / unicode still later.
- Acceptance: Lab smokes `char-demo` (retains `cell-demo`).
- Cite: `docs/CHAR-CHARS.md`.

### `docs/KERNEL.md`

- Companions: add `CHAR-CHARS.md` (wave19 **1**); **keep** tip1–4 wave18 TICK / FIND / WORD-BL / STATE-COMPILE cites and wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `CHAR` / `CHARS` / `[CHAR]` stubs + `char-demo` (cite tip; Forth mirrors `char-unit` / `chars-n` / `bracket-char` — char-unit mark only; **do not** redefine host CHAR/CHARS/[CHAR]; **not** CHAR+ / unicode / ALIGN reopen / HERE bump; `[CHAR]` flag echo only — does **not** compile char literal into body).
- Non-goals: CHAR/CHARS/[CHAR] char-unit stubs → `docs/CHAR-CHARS.md`. CELL/CELLS/ALIGN/ALIGNED stay on `CELL-CELLS.md`. TICK / FIND / WORD-BL / STATE stay on wave18 docs. TO-BODY / ENVIRONMENT? / SOURCE-PAD still later (wave19 **2–4**).
- Acceptance: Lab smokes `char-demo` (and retains `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `cell-demo` + prior demos).
- Cite: `docs/CHAR-CHARS.md`.

### `docs/STRING-LIT.md`

- Companions: add `CHAR-CHARS.md` (wave19 **1**); **keep** COMMENT-PARSE / INTERPRET / KERNEL / CREATE-DOES / THROW-CATCH / PARSE-NAME / WORD-BL cites.
- Purpose / §3 / non-goals: string-literal parse-until-`"` stays; CHAR/CHARS/[CHAR] is a sibling **char-unit** surface — **not** S"/escape heap / unicode / CHAR+ / interpret rewrite. Do not wipe wave13 string content.
- Non-goals: `CHAR` / `CHARS` / `[CHAR]` → `docs/CHAR-CHARS.md` (wave19 **1**). Full counted-string heap / BLOCK / escape / unicode still out. PARSE-NAME stays wave17 **2**. WORD-BL stays wave18 **3**.
- Acceptance: Lab smokes `char-demo` (retains `string-demo` + `word-demo` + `parse-demo`).
- Cite: `docs/CHAR-CHARS.md`.

### Optional — `docs/WORD-BL.md`

- Companions: add light `CHAR-CHARS.md` (wave19 **1**) cite; **keep** PARSE-NAME / COMMENT-PARSE / INTERPRET / KERNEL / STRING-LIT cites.
- Purpose / non-goals: WORD/BL stub token/pad stays; BL optional `char=32` echo is a blank-char mark — CHAR/CHARS/[CHAR] is the **char-unit** companion (not a BL reopen / SOURCE/PAD / interpret rewrite). Do not wipe wave18 WORD-BL content.
- Non-goals: `CHAR` / `CHARS` / `[CHAR]` → `docs/CHAR-CHARS.md` (wave19 **1**). SOURCE/PAD / ACCEPT/REFILL still later (wave19 **4** thin marks).
- Acceptance: Lab smokes `char-demo` (retains `word-demo`).
- Cite: `docs/CHAR-CHARS.md`.

### Optional — `docs/PARSE-NAME.md`

- Companions: add light `CHAR-CHARS.md` (wave19 **1**) cite; **keep** COMMENT-PARSE / INTERPRET / STRING-LIT / KERNEL / WORD-BL / SYNONYM cites.
- Purpose / non-goals: PARSE/PARSE-NAME stay token-parse markers; CHAR is a sibling char-unit mark — **not** a parse reopen / tokenizer VM / unicode. Do not wipe wave17 PARSE content.
- Non-goals: `CHAR` / `CHARS` / `[CHAR]` → `docs/CHAR-CHARS.md` (wave19 **1**). WORD-BL stays wave18 **3**.
- Acceptance: Lab smokes `char-demo` (retains `parse-demo` + `word-demo`).
- Cite: `docs/CHAR-CHARS.md`.

Do **not** wipe wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites or wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites or wave16 DEFER / MARKER / BUFFER / EXIT cites or wave15 CELL / IMMEDIATE / FILL / PICK prior content. Do **not** amend ARCHITECTURE / IMPLEMENTATION-GAPS / TO-BODY / ENVIRONMENT / SOURCE-PAD / DOCS-CITES docs this tip (proposal amends are CELL-CELLS + KERNEL + STRING-LIT + optional WORD-BL / PARSE-NAME only). Tip5 cites come after 1–4 PASS.

## 7. Non-goals

- `CHAR+` deepen / char-address arithmetic required
- Unicode / XCHAR / multi-byte char width / locale probe
- `ALIGN` / `ALIGNED` reopen (wave15 CELL already landed — keep cites; this tip is char-unit only)
- Cell-size rewrite / `CELL` / `CELLS` reopen (wave15 — already stubbed; keep cites)
- HERE bump via `CHARS` / composing `k CHARS ALLOT` (optional later; not required)
- Compiling a char literal into a colon body via `[CHAR]`
- `>BODY` CREATE-body address mark (wave19 **2**)
- `ENVIRONMENT?` query stub (wave19 **3**)
- `SOURCE` / `PAD` thin marks (wave19 **4**); `ACCEPT` / `REFILL` / full input-buffer VM still out
- Docs cites pass (wave19 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `WORD` / `BL` reopen (wave18 **3** — already stubbed; keep cites)
- `STATE` / `COMPILE,` reopen (wave18 **4** — already stubbed; keep cites)
- `FIND` / `'` / `[']` reopen (wave18 **1–2** — already stubbed; keep cites)
- `PARSE` / `PARSE-NAME` / `S"` reopen (wave17/13 — already stubbed; keep cites)
- Real DOES> XT chaining / threaded child runtime body
- Real branch XT / LEAVE jump
- Full Dusk arena / pool / free / fragmentation model
- Linked XT / executing postponed XT / real STATE cell
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live — **skip 2DUP-FAMILY**)
- `ABORT"` polish (already optional-wired inside `throw-demo` — **skip**)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/CHAR-CHARS.md` present (Research byte-copy OK); `CELL-CELLS.md` + `KERNEL.md` + `STRING-LIT.md` thin amends present (+ optional `WORD-BL.md` / `PARSE-NAME.md`); wave18 tip1–4 cites, wave17 tip1–4 cites, wave16 DEFER/MARKER/BUFFER/EXIT cites, and wave15 CELL/IMMEDIATE/COLON/KERNEL prior text retained; host CHAR/CHARS/[CHAR] untouched via mirrors.
2. `char-demo` → OK (markers §4; `[char] CHAR` greppable; `[char] CHARS n=` with `bytes=` greppable — Linux SoT `bytes=n`; `[char] [CHAR]` greppable; optional `char=` / `u=` welcome; no empty/neg FAIL on happy path; no HERE bump; no char literal compiled into body). Prior `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `recurse-demo` + `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` + `refined-boot-demo` + `host-boot-demo` + `interpret-demo` + `kernel-demo` still OK.
3. Regression green (wave18 tip1–5 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `char-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/CELL-CELLS.md` (wave15 **1** — cell-unit companion; CHAR pairs with CELL the way CHARS pairs with CELLS)
- `docs/KERNEL.md` (wave7 **5**), `docs/STRING-LIT.md` (wave13 **4**)
- `docs/WORD-BL.md` (wave18 **3**, optional), `docs/PARSE-NAME.md` (wave17 **2**, optional)
- `docs/ALLOT-HERE.md` (wave14 **1** — HERE stay; CHARS does not bump)
- `docs/TICK.md` (wave18 **1** — `[CHAR]` mirrors `[']` as compile-time sibling mark pattern)
- `docs/STATE-COMPILE.md` (wave18 **4**), `docs/FIND.md` (wave18 **2**)
- `docs/FILL-MOVE.md` (wave15 **3**), `docs/PICK-ROLL.md` (wave15 **2**)
- `docs/THROW-CATCH.md` (wave14 **4**), `docs/INTERPRET.md` (wave8 **1**)
- `forth/tritium/kernel.fs` (char-unit / chars-n / bracket-char only — do not redefine host CHAR)
- ANS Forth `CHAR` / `CHARS` / `[CHAR]` (char-unit mark only — not CHAR+ / unicode / compile into body)
- Explicit deferral: WAVE18-PROPOSAL + WAVE19-PROPOSAL (`CHAR` / `CHARS` / `[CHAR]` — char-unit; not CHAR+/unicode / ALIGN reopen)
- Base tip: `e136713` / `e13671361caa98d2326ac8ad5542952438a58557` (#86 wave18 tip5 DOCS-CITES)
- Wave19 proposal: `/workspace/tritium-research-docs/WAVE19-PROPOSAL.md`
