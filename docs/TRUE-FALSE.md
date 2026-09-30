# TRUE-FALSE — `TRUE` / `FALSE` constant marks + `true-demo`

**Status:** Shipper-ready stub spec (wave20 item **1**; thin amend wave20 **2** WITHIN companion cite)
**Canonical brief:** ANS-shaped `TRUE` / `FALSE` (thin constant marks only); `docs/KERNEL.md` (wave7 **5**); `docs/CELL-CELLS.md` (wave15 **1** — unit/cell picture companion for optional `u=` / flag echo); `docs/PICK-ROLL.md` (wave15 **2** — stack/?DUP flag-picture companion); optional `docs/CONTROL.md` (wave10 **2**) / `docs/HOST-PARITY.md` (wave8 **4**); `docs/WITHIN.md` (wave20 **2** — range-check mark companion; pairs with this tip's flag picture — not boolean cell / branch XT); explicit WAVE18 / WAVE19 / WAVE20 deferral closed as constant marks only (not real boolean cell rewrite / `0=` / flag algebra deepen / WITHIN)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `true.fs` / `true-false.fs`); Linux host REPL; **do not** redefine host `TRUE` / `FALSE` that already bind on the load path
**Companions:** `docs/KERNEL.md` (thin amend this tip), `docs/CELL-CELLS.md` (thin amend this tip), `docs/PICK-ROLL.md` (thin amend this tip); optional light cite `docs/CONTROL.md` / `docs/HOST-PARITY.md`; `docs/WITHIN.md` (wave20 **2** — thin companion cite from tip2)
**Base tip SHA:** `e93c44a` (wave19 tip5 CLOSED / #91 DOCS-CITES) / full `e93c44af21bb3310b3fc5a7351ec2f257b50f2bb`

## 1. Purpose

WAVE12 landed named-cell stubs (`docs/VARIABLE-CONST.md`); WAVE15 landed cell-unit + stack/?DUP flag pictures (`docs/CELL-CELLS.md`, `docs/PICK-ROLL.md`); WAVE18 / WAVE19 / WAVE20 explicitly deferred `TRUE` / `FALSE` (constant marks; not real boolean cell rewrite). This tip lands **stub** constant marks only: `TRUE` (or Forth mirror `true-mark`) prints `[true] TRUE` (+ optional `flag=` / `u=` — classic all-bits-set / `-1` picture welcome); `FALSE` (or Forth mirror `false-mark`) prints `[true] FALSE` (+ optional `flag=` / `u=` — classic `0` picture). Smoke via **`true-demo`**. Forth mirrors **`true-mark` / `false-mark`** so host `TRUE`/`FALSE` stay safe (or a single shared `true-false` helper if Shipper prefers — document). **Not** a real boolean cell / flag algebra rewrite, not `0=` reopen, not WITHIN this tip. Thinnest constant surface deferred since wave18/19 GAPS — flag picture before wave20 tip2 WITHIN / tip3 COUNT / tip4 EXECUTE. Pairs with cell/stack flag pictures without promoting either to a real compare/branch machine. Wave20 tip **2** lands `WITHIN` range-check mark (`docs/WITHIN.md`): sibling range-check that may echo optional `flag=<0|1>` — **not** a TRUE/FALSE reopen / boolean cell rewrite / `0=` deepen / branch XT.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `TRUE` / `true-mark` | `( -- flag )` *or* `( -- )` | Constant mark; print `[true] TRUE` (+ optional `flag=<n>` / `u=<n>`); classic all-bits-set / `-1` picture welcome |
| `FALSE` / `false-mark` | `( -- flag )` *or* `( -- )` | Constant mark; print `[true] FALSE` (+ optional `flag=<n>` / `u=<n>`); classic `0` picture |
| `true-demo` | `( -- )` | See §5 |

Host note: bind `TRUE` / `FALSE` on the Linux REPL **only if** those names do not collide with host Forth boolean constants in the same load path. Prefer Forth mirrors **`true-mark` / `false-mark`** as the Lab-facing surface when in doubt — **do not** redefine host `TRUE`/`FALSE`. Optional single shared helper `true-false` (document if Shipper prefers one word that takes a side) is acceptable — Lab still greps `[true] TRUE` and `[true] FALSE` markers. Optional `flag=` / `u=` are host ints only — not a live boolean cell, not `0=` algebra, not WITHIN. Prefer printing both TRUE and FALSE markers in the demo so Lab hit is deterministic.

## 3. Stub semantics

- **`TRUE` / `true-mark`:** print `[true] TRUE` and optionally `flag=<n>` and/or `u=<n>`. Classic ANS / common Forth picture: **all-bits-set** / **`-1`** (document if Shipper echoes `flag=1` / `u=1` as a thinner presence mark — greppable `[true] TRUE` is enough for Lab OK; `-1` picture is welcome). **Does not** allocate a boolean cell, rewrite VARIABLE/CONSTANT, deepen `0=` / flag algebra, bump HERE, or execute branch XT. Captured values are host ints / flag echo only.
- **`FALSE` / `false-mark`:** print `[true] FALSE` and optionally `flag=<n>` and/or `u=<n>`. Classic picture: **`0`**. Same constraints as TRUE — marker only.
- **Optional push:** if the host stack is easy, push the pictured value (`-1` / `0` or `1` / `0`); marker alone is enough for Lab OK — do not require a real data-stack boolean cell.
- **FAIL:** `[true] FAIL reason=<…>` optional (demo **must avoid**). Prefer not emitting `[true] FAIL` on the happy path.
- Storage: host constant ints / flag echo only. **No** real boolean cell rewrite, no `0=` / `AND` / `OR` / `INVERT` deepen, no WITHIN (wave20 **2**), no COMPARE/branch XT, no HERE bump, no arena.
- Nest with prior source / env / body / char / state / word / find / tick / create / allot / synonym / exit / buffer / marker / defer / imm / fill / pick / cell / throw / 2var / colon / control / string stubs OK. `dict-reset` unaffected (no new dict entries from constant marks — fixed host constants, not dictionary).
- WITHIN range-check mark → `docs/WITHIN.md` (wave20 **2** — sibling range-check; not a TRUE/FALSE reopen / branch XT). Still no ANS COUNT (wave20 **3**), no EXECUTE mark (wave20 **4**), no `0=` / flag algebra deepen, no real boolean cell, no ACCEPT/REFILL, no linked XT / real DOES> XT / real branch XT / full arena/heap / full Win/Android Forth VM. Wave19 tip1–4 (CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD) stay landed — keep cites; this tip does not reopen them. Those stay non-goals / later tips.

## 4. Markers

```
[true] TRUE [flag=<n>] [u=<n>]     # flag=/u= optional; classic all-bits-set / -1 picture welcome
[true] FALSE [flag=<n>] [u=<n>]    # flag=/u= optional; classic 0 picture
[true] FAIL reason=<…>             # demo avoids
[true-demo] OK
[true-demo] FAIL
```

Lab greps `[true-demo] OK` plus at least one `[true] TRUE` and one `[true] FALSE` (optional `flag=` / `u=` welcome). Demo avoids `[true] FAIL`.

## 5. `true-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; constant marks need no dict entries.
2. Invoke `TRUE` (or `true-mark`) → `[true] TRUE` (+ optional `flag=` / `u=` — classic `-1` / all-bits-set picture welcome).
3. Invoke `FALSE` (or `false-mark`) → `[true] FALSE` (+ optional `flag=` / `u=` — classic `0` picture).
4. Assert no `[true] FAIL` on the happy path. Assert constant marks did **not** require a real boolean cell rewrite / `0=` deepen / WITHIN / branch XT (marker-only is enough). Assert host `TRUE`/`FALSE` were not redefined when using the Forth mirrors.
5. Prior `source-demo` / `env-demo` / `body-demo` / `char-demo` / `state-demo` / `word-demo` / `find-demo` / `tick-demo` / `recurse-demo` / `eval-demo` / `parse-demo` / `synonym-demo` / `exit-demo` / `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` / `refined-boot-demo` / `host-boot-demo` / `interpret-demo` / `kernel-demo` still OK.
6. `[true-demo] OK`.

Both `TRUE` and `FALSE` markers are required. Optional `flag=` / `u=` echo is not required for Lab OK when both markers are greppable. No real boolean cell. No `0=` deepen. No WITHIN. No branch XT.

## 6. Thin amend — companions

### `docs/KERNEL.md`

- Companions: add `TRUE-FALSE.md` (wave20 **1**); **keep** wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites and wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites and wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `TRUE` / `FALSE` stubs + `true-demo` (cite tip; Forth mirrors `true-mark` / `false-mark` — constant marks only; **do not** redefine host `TRUE`/`FALSE`; **not** real boolean cell rewrite / `0=` deepen / WITHIN; optional `flag=` / `u=` — classic `-1` / `0` picture welcome).
- Non-goals: `TRUE` / `FALSE` constant marks → `docs/TRUE-FALSE.md`. CHAR/CHARS/[CHAR] stay on `CHAR-CHARS.md`. >BODY stays on `TO-BODY.md`. ENVIRONMENT? stays on `ENVIRONMENT-QUERY.md`. SOURCE/PAD stay on `SOURCE-PAD.md`. CELL/CELLS/ALIGN/ALIGNED stay on `CELL-CELLS.md`. PICK/ROLL/DEPTH/?DUP stay on `PICK-ROLL.md`. WITHIN / COUNT / EXECUTE still later (wave20 **2–4**).
- Acceptance: Lab smokes `true-demo` (and retains `source-demo` + `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `pick-demo` + `cell-demo` + prior demos).
- Cite: `docs/TRUE-FALSE.md`.

### `docs/CELL-CELLS.md`

- Companions / Status: add `TRUE-FALSE.md` (wave20 **1** companion cite); **keep** CHAR-CHARS / ENVIRONMENT-QUERY / ALLOT-HERE / KERNEL / VARIABLE-CONST cites — do not wipe wave15 CELL content.
- Purpose / §3: cell-unit stubs stay; `TRUE` / `FALSE` may echo optional stub `u=` / `flag=` that picture cell-width flag values (classic all-bits-set / `0`) — **not** a CELL/ALIGN reopen / cell-size rewrite / real boolean cell. Do not wipe wave15 / wave19 tip1 / tip3 CELL/CHAR/ENV content.
- Non-goals: `TRUE` / `FALSE` → `docs/TRUE-FALSE.md` (wave20 **1**). CHAR/CHARS/[CHAR] stay on `CHAR-CHARS.md`. ENVIRONMENT? stays on `ENVIRONMENT-QUERY.md`. ALIGN/ALIGNED stay on this tip (already landed).
- Acceptance: Lab smokes `true-demo` (retains `cell-demo` + `char-demo` + `env-demo`).
- Cite: `docs/TRUE-FALSE.md`.

### `docs/PICK-ROLL.md`

- Companions: add `TRUE-FALSE.md` (wave20 **1**); **keep** KERNEL / CELL-CELLS cites — do not wipe wave15 PICK-ROLL content.
- Purpose / §3 / non-goals: stack/?DUP flag pictures stay; `TRUE` / `FALSE` are sibling **constant** flag marks — **not** a PICK/ROLL/?DUP reopen / real threaded stack / `0=` deepen / WITHIN. Do not wipe wave15 PICK-ROLL content. Host `2dup`/`2drop`/`2swap` stay untouched.
- Non-goals: `TRUE` / `FALSE` → `docs/TRUE-FALSE.md` (wave20 **1**). PICK/ROLL/DEPTH/?DUP stay on this tip (already landed). WITHIN still later (wave20 **2**).
- Acceptance: Lab smokes `true-demo` (retains `pick-demo` + `cell-demo`).
- Cite: `docs/TRUE-FALSE.md`.

### Optional — `docs/CONTROL.md`

- Companions: add light `TRUE-FALSE.md` (wave20 **1**) cite; **keep** COLON / INTERPRET / KERNEL / BEGIN-UNTIL / DO-LOOP / CASE-OF / THROW-CATCH cites.
- Purpose / non-goals: IF/THEN/ELSE stay balance-only stubs; `TRUE` / `FALSE` constant marks may picture flag values that companion control `taken=` — **not** a real branch XT / IF reopen / flag algebra. Do not wipe wave10 CONTROL content.
- Non-goals: `TRUE` / `FALSE` → `docs/TRUE-FALSE.md` (wave20 **1**). WITHIN still later (wave20 **2**). Real branch XT still out.
- Acceptance: Lab smokes `true-demo` (retains `control-demo`).
- Cite: `docs/TRUE-FALSE.md`.

### Optional — `docs/HOST-PARITY.md`

- Companions: add light `TRUE-FALSE.md` (wave20 **1**) cite; **keep** KERNEL / BUILD / INSTALL / INTERPRET / ENVIRONMENT-QUERY cites.
- Purpose / non-goals: Win/Android stay CONTRACT-parity; `true-demo` CONTRACT line is acceptable — **not** a full Forth VM / boolean-cell port. Do not wipe wave8 HOST-PARITY content.
- Non-goals: `TRUE` / `FALSE` → `docs/TRUE-FALSE.md` (wave20 **1**). Full Win/Android Forth VM still out.
- Acceptance: Lab smokes `true-demo` (Win/Android: `true-demo CONTRACT` OK).
- Cite: `docs/TRUE-FALSE.md`.

### Wave20 tip **2** companion — `docs/WITHIN.md`

- Companions / Status: WITHIN cites TRUE-FALSE as flag-picture companion; this tip cites WITHIN as sibling range-check mark.
- Purpose: `WITHIN` optional `flag=<0|1>` pairs with TRUE/FALSE constant flag pictures — **not** a TRUE/FALSE reopen / boolean cell rewrite / `0=` deepen / branch XT / IF/THEN reopen.
- Non-goals: `WITHIN` → `docs/WITHIN.md` (wave20 **2**). TRUE/FALSE stay on this tip (already landed).
- Acceptance: Lab smokes `within-demo` (retains `true-demo`).
- Cite: `docs/WITHIN.md`.

Do **not** wipe tip1 TRUE-FALSE content when tip2 amends this file thinly. Do **not** wipe wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites or wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites or wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites or wave16 DEFER / MARKER / BUFFER / EXIT cites or wave15 CELL / IMMEDIATE / FILL / PICK prior content. Tip1 primary amends were KERNEL + CELL-CELLS + PICK-ROLL + optional CONTROL / HOST-PARITY; tip2 thin-amends this file as companion only (checksum CHANGES — expected). Leave CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD / ARCHITECTURE / IMPLEMENTATION-GAPS untouched for tip2. Tip5 cites come after 1–4 PASS.

## 7. Non-goals

- Real boolean cell rewrite / live flag storage / VARIABLE-as-boolean
- `0=` / `AND` / `OR` / `INVERT` / flag algebra deepen
- `WITHIN` range-check mark → `docs/WITHIN.md` (wave20 **2**; sibling range-check — not a TRUE/FALSE reopen / boolean cell / branch XT)
- ANS `COUNT` c-addr picture (wave20 **3**); host `ENTRY-COUNT` / `SYN-COUNT` / `words-count` are **not** ANS COUNT
- `EXECUTE` xt-id invoke mark (wave20 **4**); not real XT execute
- Docs cites pass (wave20 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `CHAR` / `CHARS` / `[CHAR]` reopen (wave19 **1** — already stubbed; keep cites)
- `>BODY` reopen (wave19 **2** — already stubbed; keep cites)
- `ENVIRONMENT?` reopen (wave19 **3** — already stubbed; keep cites)
- `SOURCE` / `PAD` reopen (wave19 **4** — already stubbed; keep cites); `ACCEPT` / `REFILL` / full input-buffer VM still out
- `CELL` / `CELLS` / `ALIGN` / `ALIGNED` reopen (wave15 — already stubbed; keep cites)
- `PICK` / `ROLL` / `DEPTH` / `?DUP` reopen (wave15 — already stubbed; keep cites)
- `IF` / `THEN` / `ELSE` reopen as real branch XT (wave10 — already mark-only; keep cites)
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

1. `docs/TRUE-FALSE.md` present (Research byte-copy OK); `KERNEL.md` + `CELL-CELLS.md` + `PICK-ROLL.md` thin amends present (+ optional `CONTROL.md` / `HOST-PARITY.md`); wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites, wave18 tip1–4 cites, wave17 tip1–4 cites, wave16 DEFER/MARKER/BUFFER/EXIT cites, and wave15 CELL/IMMEDIATE/COLON/KERNEL prior text retained; host `TRUE`/`FALSE` untouched via mirrors; CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD / ARCHITECTURE / IMPLEMENTATION-GAPS byte-copy unchanged.
2. `true-demo` → OK (markers §4; `[true] TRUE` and `[true] FALSE` greppable; optional `flag=` / `u=` welcome — classic `-1` / `0` picture; no FAIL on happy path; no real boolean cell / `0=` deepen / WITHIN / branch XT). Prior `source-demo` + `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `recurse-demo` + `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` + `refined-boot-demo` + `host-boot-demo` + `interpret-demo` + `kernel-demo` still OK.
3. Regression green (wave19 tip1–5 + wave18 tip1–5 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `true-demo CONTRACT` OK). Wave20 **2**: `within-demo` → OK (retains `true-demo`).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**), `docs/CELL-CELLS.md` (wave15 **1** — unit/cell picture companion for optional `u=` / flag echo)
- `docs/PICK-ROLL.md` (wave15 **2** — stack/?DUP flag-picture companion; TRUE/FALSE are constant marks — not PICK/?DUP reopen)
- `docs/CONTROL.md` (wave10 **2**, optional), `docs/HOST-PARITY.md` (wave8 **4**, optional)
- `docs/VARIABLE-CONST.md` (wave12 **2** — named-cell sibling; TRUE/FALSE are constant marks — not VARIABLE/CONSTANT reopen)
- `docs/CHAR-CHARS.md` (wave19 **1** — prior tip; keep cites)
- `docs/TO-BODY.md` (wave19 **2** — prior tip; keep cites)
- `docs/ENVIRONMENT-QUERY.md` (wave19 **3** — prior tip; keep cites)
- `docs/SOURCE-PAD.md` (wave19 **4** — prior tip; keep cites)
- `docs/TICK.md` (wave18 **1**), `docs/STATE-COMPILE.md` (wave18 **4**), `docs/FIND.md` (wave18 **2**)
- `docs/DEFER-IS.md` (wave16 **1**), `docs/SYNONYM-ALIAS.md` (wave17 **1**)
- `docs/COLON.md` (wave9 **4**), `docs/CREATE-DOES.md` (wave13 **3**)
- `forth/tritium/kernel.fs` (true-mark / false-mark only — do not redefine host TRUE/FALSE)
- ANS Forth `TRUE` / `FALSE` (constant marks only — not real boolean cell rewrite / `0=` deepen / WITHIN)
- Explicit deferral: WAVE18-PROPOSAL + WAVE19-PROPOSAL + WAVE20-PROPOSAL (`TRUE` / `FALSE` — constant marks; not boolean cell rewrite)
- Base tip: `e93c44a` / `e93c44af21bb3310b3fc5a7351ec2f257b50f2bb` (#91 wave19 tip5 DOCS-CITES CLOSED)
- `docs/WITHIN.md` (wave20 **2** — range-check mark companion; pairs with this tip's flag picture)
- Wave20 proposal: `/workspace/tritium-research-docs/WAVE20-PROPOSAL.md`
