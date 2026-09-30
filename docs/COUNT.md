# COUNT — ANS `COUNT` counted-string picture + `count-demo`

**Status:** Shipper-ready stub spec (wave20 item **3**; thin amend wave21 **1** ACCEPT-REFILL companion cite)
**Canonical brief:** ANS-shaped `COUNT` (thin `( c-addr -- c-addr u )` counted-string picture mark only); `docs/STRING-LIT.md` (wave13 **4** — string-lit companion); `docs/WORD-BL.md` (wave18 **3** — fixed word-buffer companion); `docs/SOURCE-PAD.md` (wave19 **4** — SOURCE/PAD thin marks companion; not ACCEPT/REFILL); `docs/KERNEL.md` (wave7 **5**); optional `docs/PARSE-NAME.md` (wave17 **2**) / `docs/FILL-MOVE.md` (wave15 **3**) / `docs/WORDS-VOCAB.md` (wave11 **3** — **ENTRY-COUNT / SYN-COUNT / words-count disambiguation only**); explicit WAVE18 / WAVE19 / WAVE20 deferral closed as counted-string picture mark only (not full counted-string heap / live TIB rewrite; wave21 **1** lands ACCEPT/REFILL as thin marks — `docs/ACCEPT-REFILL.md` — not COUNT reopen). **Critical:** host `ENTRY-COUNT` / `SYN-COUNT` / `words-count` are **NOT** ANS `COUNT` — do **not** redefine, alias, or Lab-grep those as this tip’s surface.
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `count.fs`); Linux host REPL; **do not** redefine host `COUNT` / `ENTRY-COUNT` / `SYN-COUNT` / `words-count` that already bind on the load path
**Companions:** `docs/STRING-LIT.md` (thin amend this tip), `docs/WORD-BL.md` (thin amend this tip), `docs/SOURCE-PAD.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip); optional light cite `docs/PARSE-NAME.md` / `docs/FILL-MOVE.md` / `docs/WORDS-VOCAB.md` (WORDS-VOCAB only to **disambiguate** ENTRY-COUNT ≠ ANS COUNT — not a WORDS reopen); `docs/ACCEPT-REFILL.md` (wave21 **1** — ACCEPT/REFILL thin marks companion cite; not COUNT reopen / live TIB rewrite / full input-buffer VM)
**Base tip SHA:** `8309c2f` (wave20 tip2 PASS / #93 WITHIN) / full `8309c2ff46b676cb4cc1e16e7888e20826742c41`

## 1. Purpose

WAVE13 landed string-literal parse stubs (`docs/STRING-LIT.md`); WAVE18 landed a fixed host word-buffer (`docs/WORD-BL.md`); WAVE19 tip **4** landed thin `SOURCE` / `PAD` marks (`docs/SOURCE-PAD.md`). WAVE18 / WAVE19 / WAVE20 explicitly deferred ANS `COUNT` (c-addr -- c-addr u picture; **critical:** host `ENTRY-COUNT` / `SYN-COUNT` / `words-count` are **not** ANS COUNT). This tip lands a **stub** counted-string picture mark only: `COUNT` (or Forth mirror **`count-mark`**) prints `[count] COUNT` (+ optional `addr=` / `u=`) for a **fixed demo counted-string fixture** (host buffer / stub c-addr picture — **not** a live TIB rewrite). Smoke via **`count-demo`**. Empty fixture → `[count] FAIL` reason=empty optional (demo avoids). Prefer Forth mirror **`count-mark`** whenever host `COUNT` collides — and **never** treat host `ENTRY-COUNT` / `SYN-COUNT` / `words-count` as this tip’s surface (do **not** redefine, alias, or Lab-grep those). **Not** ACCEPT/REFILL, not full counted-string heap / BLOCK / escape rewrite, not live TIB rewrite, not WORD/BL / SOURCE/PAD reopen. Companions SOURCE/PAD / STRING-LIT / WORD-BL without promoting any to ACCEPT/REFILL / full input-buffer VM / counted-string heap. Thinnest ANS COUNT surface deferred since wave18/19 GAPS — after tip1 TRUE-FALSE + tip2 WITHIN, before tip4 EXECUTE. Wave21 tip **1** lands thin `ACCEPT` / `REFILL` marks (`docs/ACCEPT-REFILL.md` / Forth mirrors `accept-mark` / `refill-mark`) — sibling thin input marks; **not** a COUNT reopen / full counted-string heap / live TIB rewrite / full input-buffer VM / real line editor.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `COUNT` / `count-mark` | `( c-addr -- c-addr u )` *or* `( -- )` with fixed demo fixture | ANS counted-string picture mark; print `[count] COUNT` (+ optional `addr=<n>` / `u=<n>`); classic ANS picture: take c-addr of a counted string (length byte at c-addr) → return body addr (`c-addr+1`) and length `u` |
| `count-demo` | `( -- )` | See §5 |

Host note: bind bare `COUNT` on the Linux REPL **only if** that name does not collide with a host Forth `COUNT` in the same load path. Prefer Forth mirror **`count-mark`** as the Lab-facing surface when in doubt — **do not** redefine host `COUNT`. **Critical disambiguation:** host `ENTRY-COUNT` (dict entry counter), `SYN-COUNT` (synonym table counter / related host count), and `words-count` (WORDS-VOCAB optional helper = `ENTRY-COUNT @` / host_entry_count) are **NOT** ANS `COUNT`. Do **not** redefine, alias, Lab-grep, or document those as this tip’s surface. Optional `addr=` / `u=` are host ints / stub offsets / length echo only — not a live TIB pointer, not a heap pointer, not ACCEPT/REFILL, not an entry-count. Prefer a **fixed non-empty demo counted-string fixture** so Lab hit is deterministic and empty FAIL is avoided.

## 3. Stub semantics

- **`COUNT` / `count-mark`:** take (or use a fixed demo) counted-string c-addr. Classic ANS picture: byte at `c-addr` is the length `u`; body starts at `c-addr+1`. Print `[count] COUNT` and optionally `addr=<n>` (body addr / stub offset echo — prefer body picture `c-addr+1`, or document stub offset) and/or `u=<n>` (length). **Does not** rewrite the live TIB / interpret input stream / ACCEPT / REFILL / SOURCE/PAD marks / WORD-BL word-buffer / STRING-LIT parse path. Does **not** allocate, bump HERE, open a heap, or create dict entries. Captured values are host ints / fixture echo only.
- **Fixed demo counted-string fixture (document):** e.g. a host buffer holding classic counted-string bytes `{5,'h','e','l','l','o'}` (length byte `5` + body `hello`). Invoke `COUNT` / `count-mark` against that fixture → `[count] COUNT` (+ optional `addr=` of body / stub, `u=5`). Alternate short fixture e.g. `{3,'a','b','c'}` → `u=3` also fine — document which Shipper uses. Prefer non-empty (`u≥1`) so empty FAIL is avoided.
- **Empty fixture:** `[count] FAIL reason=empty` optional (demo **must avoid** — always feed a non-empty counted-string fixture with `u≥1`).
- **Optional push:** if the host stack is easy, push the pictured `( c-addr' u )` (body addr + length); marker alone is enough for Lab OK — do not require a real counted-string heap / TIB rewrite.
- **FAIL:** `[count] FAIL reason=<…>` optional (demo **must avoid**). Prefer not emitting `[count] FAIL` on the happy path. Empty is the only documented optional FAIL reason this tip.
- Storage: fixed demo counted-string fixture / host buffer echo only. **No** live TIB rewrite, no ACCEPT/REFILL, no full counted-string heap / BLOCK / escape rewrite, no WORD/BL reopen, no SOURCE/PAD reopen, no HERE bump, no arena.
- **ENTRY-COUNT / SYN-COUNT / words-count stay untouched:** those remain dict / synonym / WORDS list helpers. This tip’s Lab greps are `[count] COUNT` and `[count-demo] OK` only — **never** Lab-grep `ENTRY-COUNT` / `SYN-COUNT` / `words-count` / `words (` as ANS COUNT success.
- Nest with prior within / true / source / env / body / char / state / word / find / tick / create / allot / synonym / exit / buffer / marker / defer / imm / fill / pick / cell / throw / 2var / colon / control / string stubs OK. `dict-reset` unaffected (no new dict entries from count marks — fixed host fixture, not dictionary).
- Still no full input-buffer VM / live TIB rewrite / real line editor, no full counted-string heap, no linked XT / real DOES> XT / real branch XT / full arena/heap / full Win/Android Forth VM. Tip1 TRUE-FALSE, tip2 WITHIN, tip4 EXECUTE, and wave19 tip1–4 (CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD) stay landed — keep cites; this tip does not reopen them. Thin `ACCEPT` / `REFILL` marks → `docs/ACCEPT-REFILL.md` (wave21 **1**) — sibling thin input marks; not a COUNT reopen. Those stay non-goals / later tips (ACCEPT-REFILL thin marks excepted as mark-only).

## 4. Markers

```
[count] COUNT [addr=<n>] [u=<n>]     # addr=/u= optional; fixed demo counted-string fixture echo
[count] FAIL reason=empty            # demo avoids
[count-demo] OK
[count-demo] FAIL
```

Lab greps `[count-demo] OK` plus at least one `[count] COUNT` (optional `addr=` / `u=` welcome; prefer greppable `u=` matching the fixture length, e.g. `u=5` for `hello`). Demo avoids `[count] FAIL reason=empty`. Prefer not emitting `[count] FAIL` on the happy path. **Do not** Lab-grep `ENTRY-COUNT` / `SYN-COUNT` / `words-count` / `[kernel] words (` as this tip’s COUNT surface.

## 5. `count-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; count marks need no dict entries.
2. Ensure a **non-empty** fixed demo counted-string fixture exists (e.g. length-prefixed `hello` → `u=5`) so empty FAIL is avoided. Fixture may live in a host buffer / stub c-addr picture beside (not replacing) WORD-BL word-buffer / SOURCE-PAD pad slot — document; do **not** require live TIB rewrite.
3. Invoke `COUNT` (or **`count-mark`**) against the fixture → `[count] COUNT` (+ optional `addr=` / `u=`). Prefer `u=5` (or documented fixture length) greppable when `u=` is present.
4. Assert no `[count] FAIL reason=empty` on the happy path. Assert count mark did **not** require ACCEPT/REFILL / live TIB rewrite / full counted-string heap / WORD-BL reopen / SOURCE-PAD reopen / HERE bump (marker-only is enough). Assert host `COUNT` was not redefined when using the Forth mirror. Assert host `ENTRY-COUNT` / `SYN-COUNT` / `words-count` were **not** redefined, aliased, or used as this tip’s Lab surface.
5. Prior `within-demo` / `true-demo` / `source-demo` / `env-demo` / `body-demo` / `char-demo` / `state-demo` / `word-demo` / `find-demo` / `tick-demo` / `recurse-demo` / `eval-demo` / `parse-demo` / `synonym-demo` / `exit-demo` / `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` / `refined-boot-demo` / `host-boot-demo` / `interpret-demo` / `kernel-demo` still OK.
6. `[count-demo] OK`.

`[count] COUNT` (with optional greppable `u=`) is required. Empty FAIL path is not exercised by the demo. Optional `( c-addr u )` push and optional `addr=` / `u=` echo are not all required for Lab OK when `[count] COUNT` is greppable. No ACCEPT/REFILL. No live TIB rewrite. No full counted-string heap. No ENTRY-COUNT / SYN-COUNT / words-count alias.

## 6. Thin amend — companions

### `docs/STRING-LIT.md`

- Companions / Status: add `COUNT.md` (wave20 **3** companion cite); **keep** COMMENT-PARSE / INTERPRET / KERNEL / CREATE-DOES / THROW-CATCH / PARSE-NAME / WORD-BL / CHAR-CHARS / SOURCE-PAD cites — do not wipe wave13 STRING-LIT content.
- Purpose / §3 / non-goals: string-literal parse-until-`"` stays; ANS `COUNT` is the sibling **counted-string picture** mark — **not** an S"/escape heap / STRING-LIT reopen / ACCEPT/REFILL / live TIB rewrite. Do not wipe wave13 string content. Stress: host `ENTRY-COUNT` / `SYN-COUNT` / `words-count` ≠ ANS COUNT.
- Non-goals: ANS `COUNT` → `docs/COUNT.md` (wave20 **3**). Full counted-string heap / BLOCK / escape / ACCEPT/REFILL still out. SOURCE/PAD stay wave19 **4**. WORD-BL stays wave18 **3**.
- Acceptance: Lab smokes `count-demo` (retains `string-demo` + `source-demo` + `word-demo`).
- Cite: `docs/COUNT.md`.

### `docs/WORD-BL.md`

- Companions / Status: add `COUNT.md` (wave20 **3** companion cite); **keep** PARSE-NAME / COMMENT-PARSE / INTERPRET / KERNEL / STRING-LIT / CHAR-CHARS / SOURCE-PAD cites — do not wipe wave18 WORD-BL content.
- Purpose / §3: WORD/BL stay stub token/pad markers into the **fixed word-buffer**; ANS `COUNT` is a sibling **counted-string picture** mark over a fixed demo fixture — **not** a word-buffer share / WORD reopen / interpret splitter rewrite / ACCEPT/REFILL / full counted-string heap. Do not wipe wave18 WORD-BL content. Stress: host `ENTRY-COUNT` / `SYN-COUNT` / `words-count` ≠ ANS COUNT; host `WORDS` stays untouched.
- Non-goals: ANS `COUNT` → `docs/COUNT.md` (wave20 **3**). SOURCE/PAD stay wave19 **4**. Full ACCEPT/REFILL / counted-string heap still later.
- Acceptance: Lab smokes `count-demo` (retains `word-demo` + `source-demo` + `string-demo`).
- Cite: `docs/COUNT.md`.

### `docs/SOURCE-PAD.md`

- Companions / Status: add `COUNT.md` (wave20 **3** companion cite); **keep** WORD-BL / PARSE-NAME / INTERPRET / KERNEL / BUFFER-COLON / STRING-LIT / CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY cites — do not wipe wave19 SOURCE-PAD content.
- Purpose / §3 / non-goals: SOURCE/PAD stay thin input-string / pad-slot marks; ANS `COUNT` is a sibling **counted-string picture** mark that may companion a fixed fixture beside SOURCE/PAD — **not** a SOURCE/PAD reopen / ACCEPT/REFILL / live TIB rewrite / full input-buffer VM / counted-string heap. Do not wipe wave19 SOURCE-PAD content. Stress: host `ENTRY-COUNT` / `SYN-COUNT` / `words-count` ≠ ANS COUNT.
- Non-goals: ANS `COUNT` → `docs/COUNT.md` (wave20 **3**). ACCEPT/REFILL / full input-buffer VM still out. SOURCE/PAD stay on this tip (already landed).
- Acceptance: Lab smokes `count-demo` (retains `source-demo` + `word-demo` + `string-demo`).
- Cite: `docs/COUNT.md`.

### `docs/KERNEL.md`

- Companions: add `COUNT.md` (wave20 **3**); **keep** tip1 TRUE-FALSE + tip2 WITHIN cites and wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites and wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites and wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `COUNT` stub + `count-demo` (cite tip; Forth mirror `count-mark` — ANS counted-string picture mark only; **do not** redefine host `COUNT`; **do not** redefine / alias / Lab-grep host `ENTRY-COUNT` / `SYN-COUNT` / `words-count` as this tip; **not** ACCEPT/REFILL / live TIB rewrite / full counted-string heap; optional `addr=` / `u=` — fixed demo fixture welcome).
- Non-goals: ANS `COUNT` counted-string picture → `docs/COUNT.md`. TRUE/FALSE stay on `TRUE-FALSE.md`. WITHIN stays on `WITHIN.md`. CHAR/CHARS/[CHAR] stay on `CHAR-CHARS.md`. >BODY stays on `TO-BODY.md`. ENVIRONMENT? stays on `ENVIRONMENT-QUERY.md`. SOURCE/PAD stay on `SOURCE-PAD.md`. STRING-LIT / WORD-BL stay on their docs. host `ENTRY-COUNT` / `SYN-COUNT` / `words-count` stay dict/WORDS helpers — **not** ANS COUNT. EXECUTE still later (wave20 **4**).
- Acceptance: Lab smokes `count-demo` (and retains `within-demo` + `true-demo` + `source-demo` + `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `string-demo` + `words-demo` + prior demos).
- Cite: `docs/COUNT.md`.

### Optional — `docs/PARSE-NAME.md`

- Companions: add light `COUNT.md` (wave20 **3**) cite; **keep** COMMENT-PARSE / INTERPRET / STRING-LIT / KERNEL / SYNONYM / WORD-BL / CHAR-CHARS / SOURCE-PAD cites — do not wipe wave17 PARSE content.
- Purpose / non-goals: PARSE/PARSE-NAME stay token-parse markers; ANS `COUNT` is a sibling **counted-string picture** mark — **not** a parse reopen / tokenizer VM / ACCEPT/REFILL. Do not wipe wave17 PARSE content. Stress: host `ENTRY-COUNT` ≠ ANS COUNT.
- Non-goals: ANS `COUNT` → `docs/COUNT.md` (wave20 **3**). WORD/BL stays wave18 **3**. SOURCE/PAD stays wave19 **4**.
- Acceptance: Lab smokes `count-demo` (retains `parse-demo` + `word-demo` + `source-demo`).
- Cite: `docs/COUNT.md`.

### Optional — `docs/FILL-MOVE.md`

- Companions: add light `COUNT.md` (wave20 **3**) cite; **keep** ALLOT-HERE / KERNEL / BUFFER-COLON / CELL-CELLS / PICK-ROLL cites — do not wipe wave15 FILL content.
- Purpose / non-goals: FILL/ERASE/MOVE/CMOVE stay fixed host-buffer stubs; ANS `COUNT` may companion a fixed counted-string fixture in a separate host buffer / stub — **not** a FILL reopen / arena / ALLOCATE / counted-string heap. Do not wipe wave15 FILL content. Stress: host `ENTRY-COUNT` ≠ ANS COUNT; do not redefine kernel `cmove`.
- Non-goals: ANS `COUNT` → `docs/COUNT.md` (wave20 **3**). FILL/ERASE/MOVE/CMOVE stay on this tip (already landed). Full counted-string heap / arena still later.
- Acceptance: Lab smokes `count-demo` (retains `fill-demo`).
- Cite: `docs/COUNT.md`.

### Optional — `docs/WORDS-VOCAB.md` (ENTRY-COUNT disambiguation only)

- Companions / Status: add light `COUNT.md` (wave20 **3**) cite **for disambiguation only**; **keep** KERNEL / INTERPRET / COLON / VARIABLE-CONST / MARKER / SYNONYM / FIND / ENVIRONMENT-QUERY cites — do not wipe wave11 WORDS content; do **not** reopen WORDS / SEARCH-WORDLIST.
- Purpose / §2 / §4: flat `WORDS` / `.words` / optional `words-count` (= `ENTRY-COUNT @` / host_entry_count) stay list-only helpers. **Critical:** `ENTRY-COUNT` / `SYN-COUNT` / `words-count` are **NOT** ANS `COUNT` (wave20 **3** — `docs/COUNT.md` / Forth mirror `count-mark`). Do **not** redefine, alias, or Lab-grep those as the ANS COUNT surface. Do not wipe wave11 WORDS content.
- Non-goals: ANS `COUNT` → `docs/COUNT.md` (wave20 **3**). WORDS/SEARCH-WORDLIST stay deferred beyond list-only. This amend is **disambiguation only** — not a WORDS reopen.
- Acceptance: Lab smokes `count-demo` (retains `words-demo`; does **not** treat `words-count` / `ENTRY-COUNT` as ANS COUNT).
- Cite: `docs/COUNT.md` (ENTRY-COUNT ≠ ANS COUNT).

Do **not** wipe tip1 TRUE-FALSE content or tip2 WITHIN content or wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites or wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites or wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites or wave16 DEFER / MARKER / BUFFER / EXIT cites or wave15 CELL / IMMEDIATE / FILL / PICK prior content. Do **not** amend ARCHITECTURE / IMPLEMENTATION-GAPS / CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / TRUE-FALSE / WITHIN / CONTROL / HOST-PARITY this tip (proposal amends are STRING-LIT + WORD-BL + SOURCE-PAD + KERNEL + optional PARSE-NAME / FILL-MOVE / WORDS-VOCAB only). **Leave `TRUE-FALSE.md` and `WITHIN.md` untouched** (tip1/tip2 byte-copy stays; WITHIN tip2 primary checksum `9067f6978ee78e9e40f586b13666261e` / 17847). Tip5 cites come after 1–4 PASS.

## 7. Non-goals

- Confusing / redefining / aliasing / Lab-grepping host `ENTRY-COUNT` / `SYN-COUNT` / `words-count` as ANS `COUNT`
- `ACCEPT` / `REFILL` thin marks → `docs/ACCEPT-REFILL.md` (wave21 **1**; Forth mirrors `accept-mark` / `refill-mark`); full input-buffer VM / live TIB rewrite / real line editor still out
- Full counted-string heap / `ALLOCATE` / BLOCK / screen strings / escape rewrite (`\"` etc.)
- `SOURCE` / `PAD` reopen (wave19 **4** — already stubbed; keep cites; thin companion amend only)
- `WORD` / `BL` reopen (wave18 **3** — already stubbed; keep cites; thin companion amend only)
- `S"` / `."` / `.(` reopen (wave13 **4** — already stubbed; keep cites; thin companion amend only)
- `EXECUTE` xt-id invoke mark (wave20 **4**); not real XT execute
- Docs cites pass (wave20 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `TRUE` / `FALSE` reopen (wave20 **1** — already stubbed; keep cites; leave TRUE-FALSE.md untouched this tip)
- `WITHIN` reopen (wave20 **2** — already stubbed; keep cites; leave WITHIN.md untouched this tip)
- `CHAR` / `CHARS` / `[CHAR]` reopen (wave19 **1** — already stubbed; keep cites)
- `>BODY` reopen (wave19 **2** — already stubbed; keep cites)
- `ENVIRONMENT?` reopen (wave19 **3** — already stubbed; keep cites)
- `PARSE` / `PARSE-NAME` reopen (wave17 **2** — already stubbed; optional thin companion cite only)
- `FILL` / `ERASE` / `MOVE` / `CMOVE` reopen (wave15 **3** — already stubbed; optional thin companion cite only)
- `WORDS` / SEARCH-WORDLIST reopen (wave11 — list-only; optional WORDS-VOCAB amend is ENTRY-COUNT **disambiguation only**)
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

1. `docs/COUNT.md` present (Research byte-copy OK); `STRING-LIT.md` + `WORD-BL.md` + `SOURCE-PAD.md` + `KERNEL.md` thin amends present (+ optional `PARSE-NAME.md` / `FILL-MOVE.md` / `WORDS-VOCAB.md`); tip1 TRUE-FALSE + tip2 WITHIN cites, wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites, wave18 tip1–4 cites, wave17 tip1–4 cites, wave16 DEFER/MARKER/BUFFER/EXIT cites, and wave15 CELL/IMMEDIATE/COLON/KERNEL prior text retained; host `COUNT` untouched via mirrors; host `ENTRY-COUNT` / `SYN-COUNT` / `words-count` **not** redefined / aliased / Lab-grepped as ANS COUNT; `TRUE-FALSE.md` + `WITHIN.md` byte-copy unchanged (WITHIN tip2 primary `9067f6978ee78e9e40f586b13666261e` / 17847); CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / ARCHITECTURE / IMPLEMENTATION-GAPS byte-copy unchanged.
2. `count-demo` → OK (markers §4; `[count] COUNT` greppable; optional `addr=` / `u=` welcome; prefer `u=` matching fixture; no empty FAIL on happy path; no live TIB rewrite / full counted-string heap; no ENTRY-COUNT / SYN-COUNT / words-count alias). wave21 **1**: `accept-demo` → OK (retains `count-demo` + `source-demo` + `word-demo`; not COUNT reopen). Prior `within-demo` + `true-demo` + `source-demo` + `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `recurse-demo` + `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` + `refined-boot-demo` + `host-boot-demo` + `interpret-demo` + `kernel-demo` still OK.
3. Regression green (wave20 tip1–2 + wave19 tip1–5 + wave18 tip1–5 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `count-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**), `docs/STRING-LIT.md` (wave13 **4** — string-lit companion; COUNT is counted-string picture — not S"/escape heap)
- `docs/WORD-BL.md` (wave18 **3** — fixed word-buffer companion; COUNT is picture mark — not WORD reopen)
- `docs/SOURCE-PAD.md` (wave19 **4** — SOURCE/PAD thin marks companion; COUNT companions fixture without ACCEPT/REFILL / live TIB rewrite)
- `docs/PARSE-NAME.md` (wave17 **2**, optional — token-parse companion)
- `docs/FILL-MOVE.md` (wave15 **3**, optional — fixed host-buffer companion)
- `docs/WORDS-VOCAB.md` (wave11 **3**, optional — **ENTRY-COUNT / SYN-COUNT / words-count ≠ ANS COUNT** disambiguation only)
- `docs/TRUE-FALSE.md` (wave20 **1** — prior tip; keep cites; leave untouched this tip)
- `docs/WITHIN.md` (wave20 **2** — prior tip; keep cites; leave untouched this tip)
- `docs/CHAR-CHARS.md` (wave19 **1** — prior tip; keep cites)
- `docs/TO-BODY.md` (wave19 **2** — prior tip; keep cites)
- `docs/ENVIRONMENT-QUERY.md` (wave19 **3** — prior tip; keep cites)
- `docs/TICK.md` (wave18 **1**), `docs/STATE-COMPILE.md` (wave18 **4**), `docs/FIND.md` (wave18 **2**)
- `forth/tritium/kernel.fs` (count-mark only — do not redefine host COUNT / ENTRY-COUNT / SYN-COUNT / words-count)
- ANS Forth `COUNT` (counted-string picture mark only — `( c-addr -- c-addr u )`; not ACCEPT/REFILL / full counted-string heap / live TIB rewrite; host ENTRY-COUNT ≠ ANS COUNT)
- Explicit deferral: WAVE18-PROPOSAL + WAVE19-PROPOSAL + WAVE20-PROPOSAL (ANS `COUNT` — c-addr picture; host ENTRY-COUNT / SYN-COUNT / words-count ≠ ANS COUNT)
- Base tip: `8309c2f` / `8309c2ff46b676cb4cc1e16e7888e20826742c41` (#93 wave20 tip2 WITHIN PASS)
- Wave20 proposal: `/workspace/tritium-research-docs/WAVE20-PROPOSAL.md`
- `docs/ACCEPT-REFILL.md` (wave21 **1** — ACCEPT/REFILL thin marks companion; not COUNT reopen / live TIB rewrite / full input-buffer VM)
