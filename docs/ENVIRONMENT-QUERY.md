# ENVIRONMENT-QUERY — `ENVIRONMENT?` query mark + `env-demo`

**Status:** Shipper-ready stub spec (wave19 item **3**; thin amend wave21 **2** BASE-HEX companion cite; thin amend wave22 **3** TO-NUMBER companion cite; thin amend wave23 **2** HOLD companion cite; thin amend wave24 **3** SHARP-SIGN companion cite)
**Canonical brief:** ANS-shaped `ENVIRONMENT?` (thin query mark only); `docs/KERNEL.md` (wave7 **5**); `docs/CELL-CELLS.md` (wave15 **1** — unit-picture companion for optional stub values); `docs/FIND.md` (wave18 **2** — find-mark companion; not SEARCH-WORDLIST); optional `docs/WORDS-VOCAB.md` (wave11 **3**) / `docs/HOST-PARITY.md` (wave8 **4**); explicit WAVE18 / WAVE19 deferral closed as thin query mark only (not full ANS env query table / WORDLIST / SEARCH-WORDLIST / linked dict); `docs/BASE-HEX.md` (wave21 **2** — radix marks companion; optional env query string may echo radix — **not** a full ANS ENVIRONMENT? table reopen); `docs/TO-NUMBER.md` (wave22 **3** — thin number-parse companion; optional parse-related stub cite — **not** full ANS ENVIRONMENT? table reopen / BASE reopen); `docs/HOLD.md` (wave23 **2** — thin pictured-numeric start companion; optional pictured-numeric stub cite — **not** full ANS ENVIRONMENT? table reopen / BASE reopen / HERE bump)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `env.fs` / `environment-query.fs`); Linux host REPL; **do not** redefine host `ENVIRONMENT?` that already binds on the load path
**Companions:** `docs/KERNEL.md` (thin amend this tip), `docs/CELL-CELLS.md` (thin amend this tip), `docs/FIND.md` (thin amend this tip); optional light cite `docs/WORDS-VOCAB.md` / `docs/HOST-PARITY.md`; `docs/BASE-HEX.md` (wave21 **2** — thin companion cite from tip2; optional radix echo via env query string welcome — not table reopen); `docs/TO-NUMBER.md` (wave22 **3** — thin companion cite from tip3; optional parse-related stub cite — **not** full env table reopen); `docs/HOLD.md` (wave23 **2** — thin companion cite from tip2; optional pictured-numeric stub cite — **not** full env table reopen; prefer `hold-mark`); `docs/SHARP-SIGN.md` (wave24 **3** — optional pictured continue cite; **not** full ANS ENVIRONMENT? reopen / HOLD reopen / HERE bump)
**Base tip SHA:** `d0b5b29` (wave19 tip2 PASS / #88 TO-BODY) / full `d0b5b298da00d0c8f39571d52fdcd4ef86e6223f`

## 1. Purpose

WAVE15 landed cell-unit stubs (`docs/CELL-CELLS.md`); WAVE19 tip **1** landed char-unit stubs (`docs/CHAR-CHARS.md`); WAVE18 landed FIND as an ANS-ish find mark (`docs/FIND.md`); WAVE18 / WAVE19 explicitly deferred `ENVIRONMENT?` (query stub mark; not full ANS env query table). This tip lands a **stub** query mark only: `ENVIRONMENT?` (or Forth mirror `environment-query`) prints `[env] ENVIRONMENT?` (+ optional `query=` / `flag=<0|1>` / `u=`) for a small **fixed demo query set** — document **1–3** allowed query strings (this tip locks: `ADDRESS-UNIT-BITS`, `MAX-CHAR`, and optional host-chosen stub `TRITIUM-STUB`). Hit → greppable `flag=1` (+ optional stub value via `u=`); miss → `[env] ENVIRONMENT? miss` or `flag=0` (demo may show one miss or avoids FAIL). Smoke via **`env-demo`**. Forth mirror **`environment-query`** so host `ENVIRONMENT?` stays safe. **Not** a real env query database, not SEARCH-WORDLIST, not wordlist rewrite, not full ANS env table. Closes GAPS `ENVIRONMENT?` deferral as **thin query mark** before tip4 SOURCE-PAD / tip5 cites. Uses cell/char unit picture for optional stub values without reopening CELL/CHAR. Wave21 tip **2** lands `BASE` / `HEX` / `DECIMAL` base marks (`docs/BASE-HEX.md`): sibling radix marks — optional env query string may echo radix — **not** a full ANS ENVIRONMENT? table reopen / SEARCH-WORDLIST / HERE stub base bump. Wave22 tip **3** lands `>NUMBER` thin number-parse mark (`docs/TO-NUMBER.md`): sibling thin parse — optional env query string may echo a parse-related stub — **not** a full ANS ENVIRONMENT? table reopen / BASE reopen / pictured numeric / HERE stub base bump; prefer `to-number-mark`. Wave23 tip **2** lands `HOLD` thin pictured-numeric start (`docs/HOLD.md`): sibling thin pictured start — optional env query string may echo a pictured-numeric stub — **not** a full ANS ENVIRONMENT? table reopen / BASE reopen / TO-NUMBER reopen / HERE stub base bump / full `#S`/`#>`/`SIGN`; prefer `hold-mark`. Wave24 tip **3** lands thin pictured-numeric **continue** (`docs/SHARP-SIGN.md`) — optional pictured continue cite; **not** full ANS ENVIRONMENT? table reopen / HOLD reopen / HERE bump.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `ENVIRONMENT?` / `environment-query` | `( c-addr u -- )` *or* `( "query" -- )` *or* `( -- )` then parse name | Query mark; print `[env] ENVIRONMENT?` (+ optional `query=<name>` / `flag=<0\|1>` / `u=<n>`); hit → `flag=1` (+ optional stub `u=`); miss → `flag=0` or miss line |
| `env-demo` | `( -- )` | See §5 |

Host note: bind `ENVIRONMENT?` on the Linux REPL **only if** that name does not collide with a host Forth `ENVIRONMENT?` in the same load path. Prefer Forth mirror **`environment-query`** as the Lab-facing surface when in doubt — **do not** redefine host `ENVIRONMENT?`. Optional `query=` / `flag=` / `u=` are host strings / ints only — not a live env database, not a wordlist, not FIND rewrite. Prefer resolving against the **fixed demo query set** (§3) so Lab hit/miss is deterministic.

## 3. Stub semantics

- **Fixed demo query set (document 1–3 allowed strings):**
  1. **`ADDRESS-UNIT-BITS`** — hit → `flag=1` + optional `u=8` (Linux SoT address-unit bits picture; pairs with cell/address unit surface — **not** a CELL reopen).
  2. **`MAX-CHAR`** — hit → `flag=1` + optional `u=127` (classic MAX-CHAR picture; pairs with char-unit surface — **not** a CHAR+/unicode reopen). Host may document `u=255` if preferred; Lab on Linux SoT expects either `127` or `255` when `u=` is present — greppable `flag=1` is enough.
  3. **`TRITIUM-STUB`** (optional host-chosen stub) — hit → `flag=1` + optional `u=1` (presence mark only). Document if Shipper omits this third string and ships only the two ANS-shaped names — Lab still OK with 1–2 hits from the set.
- **`ENVIRONMENT?` / `environment-query`:** take a query string (counted-string fixture / next token / demo fixture). If the string is in the fixed demo query set → print `[env] ENVIRONMENT?` (+ optional `query=<name>` / `flag=1` / `u=<n>`). If not in the set → print `[env] ENVIRONMENT? miss` **or** `[env] ENVIRONMENT?` with `flag=0` (and optional `query=`). **Does not** consult a real ANS env table, SEARCH-WORDLIST, rewrite FIND / WORDS, allocate, bump HERE, or execute. Captured values are host strings / ints / flag echo only.
- **Hit:** greppable `[env] ENVIRONMENT?` with `flag=1` (optional `query=` / `u=` welcome). Prefer at least one hit on `ADDRESS-UNIT-BITS` or `MAX-CHAR` in the demo.
- **Miss:** `[env] ENVIRONMENT? miss` **or** `flag=0`. Demo may show **one** miss (preferred Lab picture — greps miss without FAIL) **or** avoid miss/FAIL entirely. Do not soft-abort the host / kernel-demo path. Prefer not emitting `[env] FAIL` on the happy path.
- Storage: fixed string table (1–3 entries) + optional stub ints. **No** real env query database, no SEARCH-WORDLIST, no wordlist rewrite, no linked dict, no full ANS ENVIRONMENT? table, no HERE bump, no arena.
- Nest with prior body / char / state / word / find / tick / create / allot / synonym / exit / buffer / marker / defer / imm / fill / pick / cell / throw / 2var / colon / control / string stubs OK. `dict-reset` unaffected (no new dict entries from env marks — fixed query set is host table, not dictionary).
- Still no full ANS env table / SEARCH-WORDLIST / wordlist rewrite, no SOURCE/PAD (wave19 **4**), no ACCEPT/REFILL reopen (wave21 **1** landed thin marks), no linked XT / real DOES> XT / real branch XT / full arena/heap / full Win/Android Forth VM. CHAR-CHARS (wave19 **1**) and TO-BODY (wave19 **2**) stay landed — keep cites; this tip does not reopen char-unit or body address. Wave21 tip **2** BASE/HEX/DECIMAL radix marks (`docs/BASE-HEX.md`) are sibling — optional radix echo via env query string welcome; **not** a table reopen. Wave22 tip **3** TO-NUMBER thin parse (`docs/TO-NUMBER.md`) is sibling — optional parse-related stub cite welcome; **not** a table reopen / BASE reopen. Wave23 tip **2** HOLD thin pictured start (`docs/HOLD.md`) is sibling — optional pictured-numeric stub cite welcome; **not** a table reopen / BASE reopen / HERE bump. Those stay non-goals / later tips (BASE-HEX + TO-NUMBER + HOLD excepted as landed).

## 4. Markers

```
[env] ENVIRONMENT? [query=<name>] [flag=<0|1>] [u=<n>]   # query=/flag=/u= optional; hit → flag=1
[env] ENVIRONMENT? miss                                    # optional miss line (demo may show one)
[env] FAIL reason=<…>                                      # demo avoids
[env-demo] OK
[env-demo] FAIL
```

Lab greps `[env-demo] OK` plus at least one `[env] ENVIRONMENT?` with greppable hit (`flag=1` and/or known `query=` from the fixed set). Optional `query=` / `flag=` / `u=` fields are not all required for Lab OK when hit is otherwise greppable. Demo may emit one `[env] ENVIRONMENT? miss` (preferred) or avoid miss/FAIL entirely. Prefer not emitting `[env] FAIL` on the happy path.

## 5. `env-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; env marks need no dict entries (fixed query set is host table).
2. Invoke `ENVIRONMENT?` (or `environment-query`) on a known hit from the fixed set (prefer `ADDRESS-UNIT-BITS` or `MAX-CHAR`) → `[env] ENVIRONMENT?` (+ optional `query=` / `flag=1` / `u=`).
3. Optional second hit: invoke on the other known name (or `TRITIUM-STUB` if shipped) → another `[env] ENVIRONMENT?` with `flag=1`.
4. Optional: invoke on an unknown query string (e.g. `NO-SUCH-ENV`) → `[env] ENVIRONMENT? miss` **or** `flag=0`. Do **not** require `[env] FAIL`.
5. Assert env mark did **not** require a real ANS env table / SEARCH-WORDLIST / wordlist rewrite / FIND reopen / linked dict (marker-only is enough). Assert host `ENVIRONMENT?` was not redefined when using the Forth mirror.
6. Prior `body-demo` / `char-demo` / `state-demo` / `word-demo` / `find-demo` / `tick-demo` / `recurse-demo` / `eval-demo` / `parse-demo` / `synonym-demo` / `exit-demo` / `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` / `refined-boot-demo` / `host-boot-demo` / `interpret-demo` / `kernel-demo` still OK.
7. `[env-demo] OK`.

`ENVIRONMENT?` hit marker is required. Miss line is optional (one `[env] ENVIRONMENT? miss` welcome). Optional `query=` / `flag=` / `u=` echo is not all required for Lab OK when hit is greppable. No real env database. No SEARCH-WORDLIST. No wordlist rewrite. No FIND reopen.

## 6. Thin amend — companions

### `docs/KERNEL.md`

- Companions: add `ENVIRONMENT-QUERY.md` (wave19 **3**); **keep** tip1 CHAR-CHARS + tip2 TO-BODY cites and wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites and wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `ENVIRONMENT?` stub + `env-demo` (cite tip; Forth mirror `environment-query` — query mark only; **do not** redefine host `ENVIRONMENT?`; **not** full ANS env table / SEARCH-WORDLIST / wordlist rewrite / linked dict; fixed demo query set only).
- Non-goals: `ENVIRONMENT?` query mark → `docs/ENVIRONMENT-QUERY.md`. CHAR/CHARS/[CHAR] stay on `CHAR-CHARS.md`. >BODY stays on `TO-BODY.md`. CELL/CELLS/ALIGN/ALIGNED stay on `CELL-CELLS.md`. FIND / TICK / WORD-BL / STATE stay on wave18 docs. SOURCE-PAD still later (wave19 **4**).
- Acceptance: Lab smokes `env-demo` (and retains `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `create-demo` + `allot-demo` + `cell-demo` + prior demos).
- Cite: `docs/ENVIRONMENT-QUERY.md`.

### `docs/CELL-CELLS.md`

- Companions / Status: add `ENVIRONMENT-QUERY.md` (wave19 **3** companion cite); **keep** CHAR-CHARS / ALLOT-HERE / KERNEL / VARIABLE-CONST cites — do not wipe wave15 CELL content.
- Purpose / §3: cell-unit stubs stay; `ENVIRONMENT?` may echo optional stub values that picture address/cell/char units (`ADDRESS-UNIT-BITS` / `MAX-CHAR`) — **not** a CELL/ALIGN reopen / cell-size rewrite / real env table. Do not wipe wave15 / wave19 tip1 CELL/CHAR content.
- Non-goals: `ENVIRONMENT?` → `docs/ENVIRONMENT-QUERY.md` (wave19 **3**). CHAR/CHARS/[CHAR] stay on `CHAR-CHARS.md`. ALIGN/ALIGNED stay on this tip (already landed).
- Acceptance: Lab smokes `env-demo` (retains `cell-demo` + `char-demo`).
- Cite: `docs/ENVIRONMENT-QUERY.md`.

### `docs/FIND.md`

- Companions: add `ENVIRONMENT-QUERY.md` (wave19 **3**); **keep** KERNEL / WORDS-VOCAB / SYNONYM-ALIAS / INTERPRET / TICK cites — do not wipe wave18 FIND content.
- Purpose / §3 / non-goals: FIND stays ANS-ish find mark; `ENVIRONMENT?` is a sibling **query** mark against a fixed demo string set — **not** SEARCH-WORDLIST / FIND rewrite / linked dict / execute-through. Do not wipe wave18 FIND content. Host find aliases stay untouched.
- Non-goals: `ENVIRONMENT?` → `docs/ENVIRONMENT-QUERY.md` (wave19 **3**). SEARCH-WORDLIST / linked dict still later. TICK / WORD-BL / STATE stay on wave18 docs. SOURCE-PAD still later (wave19 **4**).
- Acceptance: Lab smokes `env-demo` (retains `find-demo` + `tick-demo` + `words-demo`).
- Cite: `docs/ENVIRONMENT-QUERY.md`.

### Optional — `docs/WORDS-VOCAB.md`

- Companions: add light `ENVIRONMENT-QUERY.md` (wave19 **3**) cite; **keep** KERNEL / INTERPRET / COLON / VARIABLE-CONST / MARKER / SYNONYM-ALIAS / FIND cites.
- Purpose / non-goals: flat list stays list-only; `ENVIRONMENT?` is a sibling query mark — **not** SEARCH-WORDLIST / wordlist rewrite / WORDS reopen. Do not wipe wave11 / FIND / SYNONYM content.
- Non-goals: `ENVIRONMENT?` → `docs/ENVIRONMENT-QUERY.md` (wave19 **3**). FIND stays wave18 **2**. SEARCH-WORDLIST / linked dict still later.
- Acceptance: Lab smokes `env-demo` (retains `words-demo` + `find-demo`).
- Cite: `docs/ENVIRONMENT-QUERY.md`.

### Optional — `docs/HOST-PARITY.md`

- Companions: add light `ENVIRONMENT-QUERY.md` (wave19 **3**) cite; **keep** KERNEL / BUILD / INSTALL / INTERPRET cites.
- Purpose / non-goals: Win/Android stay CONTRACT-parity; `env-demo` CONTRACT line is acceptable — **not** a full Forth VM / env-table port. Do not wipe wave8 HOST-PARITY content.
- Non-goals: `ENVIRONMENT?` → `docs/ENVIRONMENT-QUERY.md` (wave19 **3**). Full Win/Android Forth VM still out.
- Acceptance: Lab smokes `env-demo` (Win/Android: `env-demo CONTRACT` OK).
- Cite: `docs/ENVIRONMENT-QUERY.md`.

### `docs/BASE-HEX.md` (wave21 **2** thin companion cite)

- Companions / Status: ENVIRONMENT-QUERY cites BASE-HEX as radix-mark companion; BASE-HEX cites ENVIRONMENT-QUERY as query-mark companion (optional radix echo).
- Purpose: optional env query string may echo radix — **not** a full ANS ENVIRONMENT? table reopen / SEARCH-WORDLIST / wordlist rewrite / number parser / `>NUMBER` / HERE stub base bump.
- Non-goals: `BASE` / `HEX` / `DECIMAL` → `docs/BASE-HEX.md` (wave21 **2**). ENVIRONMENT? stays on this tip (already landed). Full ANS ENVIRONMENT? table still out.
- Acceptance: Lab smokes `base-demo` (retains `env-demo`).
- Cite: `docs/BASE-HEX.md`.

### `docs/TO-NUMBER.md` (wave22 **3** thin companion cite)

- Companions / Status: ENVIRONMENT-QUERY cites TO-NUMBER as thin number-parse companion; TO-NUMBER cites ENVIRONMENT-QUERY as optional parse-related stub cite (**not** table reopen).
- Purpose: optional env query string may echo a parse-related stub — **not** a full ANS ENVIRONMENT? table reopen / SEARCH-WORDLIST / BASE reopen / pictured numeric / HERE stub base bump.
- Non-goals: `>NUMBER` → `docs/TO-NUMBER.md` (wave22 **3**). ENVIRONMENT? stays on this tip (already landed). Full ANS ENVIRONMENT? table still out.
- Acceptance: Lab smokes `number-demo` (retains `env-demo` + `base-demo`).
- Cite: `docs/TO-NUMBER.md`.

### `docs/HOLD.md` (wave23 **2** thin companion cite)

- Companions / Status: ENVIRONMENT-QUERY cites HOLD as thin pictured-numeric start companion; HOLD cites ENVIRONMENT-QUERY as optional pictured-numeric stub cite (**not** table reopen).
- Purpose: optional env query string may echo a pictured-numeric stub — **not** a full ANS ENVIRONMENT? table reopen / SEARCH-WORDLIST / BASE reopen / TO-NUMBER reopen / HERE stub base bump / full `#S`/`#>`/`SIGN`.
- Non-goals: `HOLD` → `docs/HOLD.md` (wave23 **2**). ENVIRONMENT? stays on this tip (already landed). Full ANS ENVIRONMENT? table still out.
- Acceptance: Lab smokes `hold-demo` (retains `env-demo` + `number-demo` + `base-demo` + `charplus-demo`).
- Cite: `docs/HOLD.md`.


Do **not** wipe tip1 CHAR-CHARS cites or tip2 TO-BODY cites or wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites or wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites or wave16 DEFER / MARKER / BUFFER / EXIT cites or wave15 CELL / IMMEDIATE / FILL / PICK prior content. Do **not** amend ARCHITECTURE / IMPLEMENTATION-GAPS / TO-BODY / CHAR-CHARS / SOURCE-PAD / DOCS-CITES docs this tip (proposal amends are KERNEL + CELL-CELLS + FIND + optional WORDS-VOCAB / HOST-PARITY only). Leave `TO-BODY.md` and `CHAR-CHARS.md` untouched (tip1/tip2 byte-copy stays). Tip5 cites come after 1–4 PASS.

## 7. Non-goals

- Full ANS `ENVIRONMENT?` query table / WORDLIST / env database
- `SEARCH-WORDLIST` / wordlist stack / linked dict rewrite
- Redefining host `ENVIRONMENT?`
- `SOURCE` / `PAD` thin marks (wave19 **4**); `ACCEPT` / `REFILL` thin marks → `docs/ACCEPT-REFILL.md` (wave21 **1**); full input-buffer VM still out
- `BASE` / `HEX` / `DECIMAL` base marks → `docs/BASE-HEX.md` (wave21 **2**; sibling radix marks — optional env query string may echo radix; **not** a full ANS ENVIRONMENT? table reopen; do not break HERE stub base)
- `>NUMBER` thin number-parse mark → `docs/TO-NUMBER.md` (wave22 **3**; optional parse-related stub cite — **not** full ANS ENVIRONMENT? table reopen / BASE reopen; HERE stub untouched; prefer `to-number-mark`)
- `HOLD` thin pictured-numeric start → `docs/HOLD.md` (wave23 **2**; optional pictured-numeric stub cite — **not** full ANS ENVIRONMENT? table reopen / BASE reopen / HERE bump; prefer `hold-mark`)
- Docs cites pass (wave19 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `CHAR` / `CHARS` / `[CHAR]` reopen (wave19 **1** — already stubbed; keep cites)
- `>BODY` reopen (wave19 **2** — already stubbed; keep cites)
- `CELL` / `CELLS` / `ALIGN` / `ALIGNED` reopen (wave15 — already stubbed; keep cites)
- `FIND` / `'` / `[']` / `WORD` / `BL` / `STATE` / `COMPILE,` reopen (wave18 — already stubbed; keep cites)
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

1. `docs/ENVIRONMENT-QUERY.md` present (Research byte-copy OK); `KERNEL.md` + `CELL-CELLS.md` + `FIND.md` thin amends present (+ optional `WORDS-VOCAB.md` / `HOST-PARITY.md`); tip1 CHAR-CHARS cites, tip2 TO-BODY cites, wave18 tip1–4 cites, wave17 tip1–4 cites, wave16 DEFER/MARKER/BUFFER/EXIT cites, and wave15 CELL/IMMEDIATE/COLON/KERNEL prior text retained; host `ENVIRONMENT?` untouched via mirrors; `TO-BODY.md` + `CHAR-CHARS.md` byte-copy unchanged.
2. `env-demo` → OK (markers §4; `[env] ENVIRONMENT?` greppable hit with `flag=1` and/or known `query=` from fixed set; optional `query=` / `u=` welcome; optional one `[env] ENVIRONMENT? miss` or `flag=0`; no real env table / SEARCH-WORDLIST / wordlist rewrite). Prior `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `recurse-demo` + `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` + `refined-boot-demo` + `host-boot-demo` + `interpret-demo` + `kernel-demo` still OK.
3. Regression green (wave19 tip1–2 + wave18 tip1–5 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `env-demo CONTRACT` OK). Wave21 **2**: `base-demo` → OK (retains `env-demo`; optional radix echo via env query string welcome — not table reopen). Wave22 **3**: `number-demo` → OK (retains `env-demo` + `base-demo`; optional parse-related stub cite — not table reopen).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**), `docs/CELL-CELLS.md` (wave15 **1** — unit-picture companion for optional stub values)
- `docs/FIND.md` (wave18 **2** — find-mark companion; ENVIRONMENT? is query mark — not SEARCH-WORDLIST / FIND rewrite)
- `docs/WORDS-VOCAB.md` (wave11 **3**, optional), `docs/HOST-PARITY.md` (wave8 **4**, optional)
- `docs/CHAR-CHARS.md` (wave19 **1** — prior tip; keep cites; MAX-CHAR pictures char-unit)
- `docs/TO-BODY.md` (wave19 **2** — prior tip; keep cites)
- `docs/TO-NUMBER.md` (wave22 **3** — thin number-parse companion; optional parse-related stub cite — not table reopen)
- `docs/HOLD.md` (wave23 **2** — thin pictured-numeric start companion; optional pictured-numeric stub cite — not table reopen; prefer `hold-mark`)
- `docs/SHARP-SIGN.md` (wave24 **3** — optional pictured continue cite; **not** full ANS ENVIRONMENT? reopen / HERE bump)
- `docs/BASE-HEX.md` (wave21 **2** — radix marks companion; keep cites)
- `docs/TICK.md` (wave18 **1**), `docs/STATE-COMPILE.md` (wave18 **4**), `docs/WORD-BL.md` (wave18 **3**)
- `docs/DEFER-IS.md` (wave16 **1**), `docs/SYNONYM-ALIAS.md` (wave17 **1**)
- `docs/COLON.md` (wave9 **4**), `docs/CREATE-DOES.md` (wave13 **3**)
- `forth/tritium/kernel.fs` (environment-query only — do not redefine host ENVIRONMENT?)
- ANS Forth `ENVIRONMENT?` (query mark only — not full ANS env table / SEARCH-WORDLIST / linked dict)
- Explicit deferral: WAVE18-PROPOSAL + WAVE19-PROPOSAL (`ENVIRONMENT?` — query stub; not env table / SEARCH-WORDLIST)
- Base tip: `d0b5b29` / `d0b5b298da00d0c8f39571d52fdcd4ef86e6223f` (#88 wave19 tip2 TO-BODY PASS)
- Wave19 proposal: `/workspace/tritium-research-docs/WAVE19-PROPOSAL.md`
