# PARSE-NAME — `PARSE` / `PARSE-NAME` markers + `parse-demo`

**Status:** Shipper-ready stub spec (wave17 item **2**; thin amend wave19 **1** CHAR-CHARS companion cite)
**Canonical brief:** ANS-shaped `PARSE` / `PARSE-NAME` (thin token-parse markers only); `docs/COMMENT-PARSE.md` (wave12 **3**); `docs/INTERPRET.md` (wave8 **1**); `docs/STRING-LIT.md` (wave13 **4**); `docs/KERNEL.md` (wave7 **5**); `docs/SYNONYM-ALIAS.md` (wave17 **1**); explicit WAVE16/WAVE17 deferral closed as parse-marker deepen only
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `interpret.fs` / `parse.fs`); Linux host REPL
**Companions:** `docs/COMMENT-PARSE.md` (thin amend this tip), `docs/INTERPRET.md` (thin amend this tip), `docs/STRING-LIT.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip), `docs/WORD-BL.md` (wave18 **3**); `docs/CHAR-CHARS.md` (wave19 **1** — char-unit companion cite)
**Base tip SHA:** `b73161b` (wave17 tip1 CLOSED / #77 SYNONYM-ALIAS) / full `b73161b896b62abef640b73eef93e4ae77c1fb0a`

## 1. Purpose

WAVE16/WAVE17 explicitly deferred `PARSE` / `PARSE-NAME` deepen (full parser / tokenizer VM / WORD/BL rewrite). Comment skip (`\` / `(`) and string-literal parse stubs already exist; a whitespace / delimiter **token-parse marker** surface does not. This tip **deepens** on wave12 COMMENT-PARSE with thin markers only: `PARSE-NAME` prints `[parse] PARSE-NAME` (+ optional `tok=` / `u=`) for a whitespace-delimited stub token taken from a demo string/fixture; `PARSE` (delimiter form) prints `[parse] PARSE delim=` (+ optional `tok=`). Smoke via **`parse-demo`**. Forth mirrors **`parse-name` / `parse-delim`** if host names collide. **Not** a full stream parser, not WORD/BL rewrite of the whole interpret path, not S"/escape heap. Builds on comment skip without reopening BLOCK / nested-paren deferrals. Pairs with wave13 STRING-LIT parse-until-delim surface without promoting either to a real tokenizer VM. Wave18 **3** closes the WORD/BL rewrite deferral as **stub markers only** — see `docs/WORD-BL.md` (still not a full interpret splitter rewrite). Wave19 tip **1** lands char-unit companion (`CHAR` / `CHARS` / `[CHAR]` — `docs/CHAR-CHARS.md`) — sibling mark, not a parse reopen / unicode / tokenizer VM.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `PARSE-NAME` / `parse-name` | `( -- c-addr u )` *or* `( -- )` stub | Whitespace-delimited stub token from demo string/fixture; print `[parse] PARSE-NAME` (+ optional `tok=<token>` / `u=<n>`) |
| `PARSE` / `parse-delim` | `( delim -- c-addr u )` *or* `( delim -- )` stub | Delimiter form; print `[parse] PARSE delim=<d>` (+ optional `tok=<token>`) |
| `parse-demo` | `( -- )` | See §5 |

Host note: bind `PARSE-NAME` / `PARSE` on the Linux REPL; Forth mirrors **`parse-name` / `parse-delim`** if host Forth names collide (host Forth may already expose real `PARSE` / `PARSE-NAME`). Prefer fixture/demo-string source over rewriting the live interpret WORD/BL path. Returned `( c-addr u )` is optional — marker alone is enough for Lab.

## 3. Stub semantics

- **PARSE-NAME:** take one whitespace-delimited stub token from a demo string/fixture (or from a short in-demo buffer). Print `[parse] PARSE-NAME` and optionally `tok=<token>` and/or `u=<n>` (char count). **Does not** rewrite interpret WORD/BL, FIND, or the live token stream for all inputs — demo/fixture path is enough.
- **PARSE (delimiter form):** given a delimiter char (or fixture equivalent), take characters until that delim (or end of fixture). Print `[parse] PARSE delim=<d>` and optionally `tok=<token>`. Delim may be shown as a printable char or ordinal — document choice; Lab greps `delim=`.
- **Empty / no-token:** `[parse] FAIL reason=empty` optional (demo **must avoid** — always feed a non-empty fixture with ≥1 whitespace-delimited token and a non-empty PARSE span).
- Storage: stub token string for marker echo only. **No** full tokenizer VM, no nested-paren reopen, no BLOCK comments, no EVALUATE nested interpret, no S"/escape heap rewrite.
- Nest with prior synonym / exit / buffer / marker / defer / imm / fill / pick / cell / allot / throw / 2var / create / colon / control / string / comment / words stubs OK. `dict-reset` unaffected (parse stubs do not create dict words from token text).
- Still no full parser/tokenizer VM, no nested paren deepen, no BLOCK comments, no EVALUATE nested interpret, no RECURSE self-XT, no real DOES> XT, no real branch XT, no full Win/Android Forth VM. `WORD` / `BL` stub markers (not full interpret rewrite) → `docs/WORD-BL.md` (wave18 **3**). Those stay non-goals / later tips (WORD-BL stubs excepted as mark-only).

## 4. Markers

```
[parse] PARSE-NAME                  # + optional tok=<token> / u=<n>
[parse] PARSE delim=<d>             # + optional tok=<token>
[parse] FAIL reason=empty           # demo avoids
[parse-demo] OK
[parse-demo] FAIL
```

Lab greps `[parse-demo] OK` plus at least one `[parse] PARSE-NAME` (optional `tok=` / `u=` welcome) and one `[parse] PARSE delim=` (optional `tok=` welcome). Demo avoids `[parse] FAIL reason=empty`.

## 5. `parse-demo`

1. Clean slate / `dict-reset` (or cold path).
2. Prepare a non-empty demo string/fixture with ≥1 whitespace-delimited token (e.g. `hello world` or `alpha`).
3. `PARSE-NAME` (or `parse-name`) against the fixture → `[parse] PARSE-NAME` (+ optional `tok=hello` / `u=5`).
4. `PARSE` (or `parse-delim`) with a delimiter (e.g. `,` or `|`) against a fixture that yields a non-empty span → `[parse] PARSE delim=,` (+ optional `tok=`).
5. Assert no `[parse] FAIL reason=empty` on the happy path. Assert token body text is **not** required to `entry-create` / FIND-hit (marker-only is enough; no interpret WORD/BL rewrite required).
6. Prior `synonym-demo` / `exit-demo` / `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` still OK.
7. `[parse-demo] OK`.

PARSE-NAME + PARSE delim markers are required. Empty FAIL path is not exercised by the demo. Optional `( c-addr u )` push is not required for Lab OK.

## 6. Thin amend — companions

### `docs/COMMENT-PARSE.md`

- Companions: add `PARSE-NAME.md` (wave17 **2**); **keep** INTERPRET / KERNEL / STRING-LIT / VARIABLE-CONST cites.
- Purpose / §3: comment skip stays `\` EOL + `(` … `)`; PARSE/PARSE-NAME token-parse markers deepen beside it — **not** a full stream parser / nested-paren reopen / BLOCK comments. Do not wipe wave12 comment content.
- Non-goals: `PARSE` / `PARSE-NAME` → `docs/PARSE-NAME.md` (wave17 **2**). Nested paren / BLOCK comments still later. STRING-LIT stays wave13 **4**.
- Acceptance: Lab smokes `parse-demo` (retains `comment-demo`).
- Cite: `docs/PARSE-NAME.md`.

### `docs/INTERPRET.md`

- Status / companions: cite wave17 **2**; add `PARSE-NAME.md`; **keep** COMMENT-PARSE / STRING-LIT / IMMEDIATE-POSTPONE / EXIT-QUIT cites.
- §1 / words: note PARSE/PARSE-NAME stub markers on a demo/fixture path (not WORD/BL rewrite of whole interpret); point to `PARSE-NAME.md`.
- Words table: add `PARSE-NAME` / `PARSE` (+ mirrors) + `parse-demo` (cite tip).
- Markers: one-line pointer to `[parse]` markers.
- Non-goals: PARSE-NAME deepen in scope via this tip; keep full tokenizer VM / EVALUATE nested interpret out.
- Acceptance: Lab smokes `parse-demo` (retains `interpret-demo` / `comment-demo` / `string-demo` / `exit-demo`).
- Cite: `docs/PARSE-NAME.md`.

### `docs/STRING-LIT.md`

- Companions: add `PARSE-NAME.md` (wave17 **2**); **keep** COMMENT-PARSE / INTERPRET / KERNEL / CREATE-DOES / THROW-CATCH cites.
- Purpose / §3: string-literal parse-until-`"` stays; PARSE/PARSE-NAME is a sibling **token/delim marker** surface — **not** S"/escape heap / WORD rewrite. Do not wipe wave13 string content.
- Non-goals: `PARSE` / `PARSE-NAME` → `docs/PARSE-NAME.md` (wave17 **2**). Full counted-string heap / BLOCK / escape still out.
- Acceptance: Lab smokes `parse-demo` (retains `string-demo`).
- Cite: `docs/PARSE-NAME.md`.

### `docs/KERNEL.md`

- Companions: add `PARSE-NAME.md` (wave17 **2**); **keep** tip1 SYNONYM-ALIAS cite and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `PARSE` / `PARSE-NAME` stubs + `parse-demo` (cite tip; Forth mirrors `parse-name` / `parse-delim` — marker deepen only; **not** full parser VM / WORD/BL rewrite).
- Non-goals: PARSE/PARSE-NAME marker deepen → `docs/PARSE-NAME.md`. SYNONYM/ALIAS stays on wave17 **1**. DEFER/IS/ACTION-OF / MARKER / BUFFER: / EXIT/QUIT stay on wave16 docs. Full tokenizer / EVALUATE nested interpret / RECURSE self-XT still later.
- Acceptance: Lab smokes `parse-demo` (and retains `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + prior demos).
- Cite: `docs/PARSE-NAME.md`.

Do **not** wipe tip1 SYNONYM cites or wave16 DEFER / MARKER / BUFFER / EXIT cites or wave15 / COMMENT / STRING / KERNEL prior content. Do **not** amend EVALUATE / RECURSE docs this tip (proposal amends are COMMENT-PARSE + INTERPRET + STRING-LIT + KERNEL only).

## 7. Non-goals

- Full parser / tokenizer VM / WORD/BL rewrite of the whole interpret path — `WORD` / `BL` stub markers only → `docs/WORD-BL.md` (wave18 **3**; still not a full interpret splitter rewrite / SOURCE/PAD)
- Nested paren comment deepen (COMMENT-PARSE remains non-nested stub)
- BLOCK comments / screen comments
- `EVALUATE` / `INCLUDE` nested interpret / file VM (wave17 **3** candidate — mark-only later; refined-boot stays)
- `RECURSE` real self-XT (wave17 **4** candidate — mark-only `recurse-mark` later)
- Docs cites pass (wave17 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `SYNONYM` / `ALIAS` reopen (wave17 **1** — already stubbed; keep cites)
- `MARKER` / `BUFFER:` / `EXIT` / `QUIT` (wave16 **2–4** — already stubbed; keep cites)
- IMMEDIATE / POSTPONE / FILL / PICK / CELL (wave15 — already stubbed)
- S" / escape heap rewrite (STRING-LIT stays marker stubs)
- `CHAR` / `CHARS` / `[CHAR]` char-unit stubs → `docs/CHAR-CHARS.md` (wave19 **1**; not CHAR+/unicode / parse reopen)
- Real DOES> XT chaining / threaded child runtime body
- Real branch XT / LEAVE jump
- Full Dusk arena / pool / free / fragmentation model
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live — skip 2DUP-FAMILY)
- `ABORT"` polish (already optional-wired inside `throw-demo` — skip)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/PARSE-NAME.md` present (Research byte-copy OK); `COMMENT-PARSE.md` + `INTERPRET.md` + `STRING-LIT.md` + `KERNEL.md` thin amends present (wave12/8/13/7 text, tip1 SYNONYM cites, and wave16 DEFER/MARKER/BUFFER/EXIT cites retained).
2. `parse-demo` → OK (markers §4; PARSE-NAME greppable; PARSE delim= greppable; no empty FAIL on happy path); wave18 **3**: `word-demo` → OK (retains `parse-demo`); wave19 **1**: `char-demo` → OK (retains `parse-demo`). Prior `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` still OK.
3. Regression green (wave17 tip1 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `parse-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/COMMENT-PARSE.md` (wave12 **3**), `docs/INTERPRET.md` (wave8 **1**), `docs/STRING-LIT.md` (wave13 **4**), `docs/KERNEL.md` (wave7 **5**)
- `docs/SYNONYM-ALIAS.md` (wave17 **1**)
- `docs/DEFER-IS.md` (wave16 **1**), `docs/MARKER.md` (wave16 **2**), `docs/BUFFER-COLON.md` (wave16 **3**), `docs/EXIT-QUIT.md` (wave16 **4**)
- `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**), `docs/FILL-MOVE.md` (wave15 **3**), `docs/PICK-ROLL.md` (wave15 **2**), `docs/CELL-CELLS.md` (wave15 **1**)
- `forth/tritium/kernel.fs`
- ANS Forth `PARSE` / `PARSE-NAME` (token/delim mark only — no full tokenizer VM / WORD/BL rewrite)
- Explicit deferral: WAVE16-PROPOSAL / WAVE17-PROPOSAL (`PARSE` / `PARSE-NAME` deepen — not full parser VM)
- Base tip: `b73161b` / `b73161b896b62abef640b73eef93e4ae77c1fb0a` (#77 wave17 tip1 SYNONYM-ALIAS)
- `docs/WORD-BL.md` (wave18 **3** — WORD/BL stub markers; closes this tip's WORD/BL rewrite deferral as stubs only)
- `docs/CHAR-CHARS.md` (wave19 **1** — char-unit companion)
- Wave17 proposal: `/workspace/tritium-research-docs/WAVE17-PROPOSAL.md`
