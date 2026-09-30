# WITHIN — `WITHIN` range-check mark + `within-demo`

**Status:** Shipper-ready stub spec (wave20 item **2**)
**Canonical brief:** ANS-shaped `WITHIN` (thin range-check mark only); `docs/TRUE-FALSE.md` (wave20 **1** — flag-picture companion); `docs/CONTROL.md` (wave10 **2** — IF `taken=` companion; not IF/THEN reopen); `docs/KERNEL.md` (wave7 **5**); optional `docs/PICK-ROLL.md` (wave15 **2**) / `docs/CELL-CELLS.md` (wave15 **1**); explicit WAVE18 / WAVE19 / WAVE20 deferral closed as range-check mark only (not real branch XT / runtime compare re-exec / IF/THEN reopen)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `within.fs`); Linux host REPL; **do not** redefine host `WITHIN` that already binds on the load path
**Companions:** `docs/KERNEL.md` (thin amend this tip), `docs/CONTROL.md` (thin amend this tip), `docs/TRUE-FALSE.md` (thin amend this tip); optional light cite `docs/PICK-ROLL.md` / `docs/CELL-CELLS.md`
**Base tip SHA:** `6a6a87d` (wave20 tip1 PASS / #92 TRUE-FALSE) / full `6a6a87d2dc573211173f4c67d4d133c30418a0c7`

## 1. Purpose

WAVE10 landed IF/THEN/ELSE as balance-only control stubs (`docs/CONTROL.md`); WAVE15 landed stack/?DUP flag pictures (`docs/PICK-ROLL.md`); WAVE20 tip **1** landed `TRUE` / `FALSE` constant marks (`docs/TRUE-FALSE.md`). WAVE18 / WAVE19 / WAVE20 explicitly deferred `WITHIN` (range-check mark; not real branch XT). This tip lands a **stub** range-check mark only: `WITHIN` (or Forth mirror `within-mark`) prints `[within] WITHIN` (+ optional `n=` / `lo=` / `hi=` / `flag=<0|1>`) for a small fixed demo triple — hit → greppable `flag=1`; miss → `flag=0` or `[within] WITHIN miss` (demo may show one miss or avoids FAIL). Smoke via **`within-demo`**. Forth mirror **`within-mark`** so host `WITHIN` stays safe. **Not** a real branch XT, not IF/THEN/ELSE reopen, not unsigned-wrap rewrite required (document signed picture if used). Pairs with tip1 flag picture without promoting either to a real compare/branch machine. Thinnest range-check surface deferred since wave18/19 GAPS — after constants, before wave20 tip3 COUNT / tip4 EXECUTE.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `WITHIN` / `within-mark` | `( n lo hi -- flag )` *or* `( -- )` with fixed demo triple | Range-check mark; print `[within] WITHIN` (+ optional `n=<n>` / `lo=<lo>` / `hi=<hi>` / `flag=<0\|1>`); classic ANS picture: `lo ≤ n < hi` (signed) → `flag=1`; else `flag=0` |
| `within-demo` | `( -- )` | See §5 |

Host note: bind `WITHIN` on the Linux REPL **only if** that name does not collide with a host Forth `WITHIN` in the same load path. Prefer Forth mirror **`within-mark`** as the Lab-facing surface when in doubt — **do not** redefine host `WITHIN`. Optional `n=` / `lo=` / `hi=` / `flag=` are host ints only — not a live compare machine, not a branch XT, not IF reopen. Prefer a **fixed demo triple** (document 1–2 fixture ranges) so Lab hit is deterministic. Classic stack order `n lo hi` (ANS `n1 n2 n3` with `n2=lo`, `n3=hi`).

## 3. Stub semantics

- **`WITHIN` / `within-mark`:** take (or use a fixed demo) triple `( n lo hi )`. Print `[within] WITHIN` and optionally `n=<n>`, `lo=<lo>`, `hi=<hi>`, and/or `flag=<0|1>`. Classic ANS / common Forth **signed** picture: **hit** when `lo ≤ n < hi` → `flag=1`; **miss** otherwise → `flag=0` (or print `[within] WITHIN miss` as an alternate miss marker — document which Shipper uses). **Does not** compile a branch XT, re-exec a runtime compare loop, reopen IF/THEN/ELSE, deepen `0=` / flag algebra, bump HERE, or allocate. Captured values are host ints / flag echo only.
- **Unsigned wrap:** ANS `WITHIN` also has an unsigned-wrap form (`(u1-u2) < (u3-u2)`). This tip does **not** require unsigned-wrap rewrite — signed picture for the fixed demo triple is enough. If Shipper echoes unsigned wrap, document it; Lab greps `[within] WITHIN` + `flag=` (or miss marker).
- **Fixed demo fixtures (document 1–2):**
  - **Hit:** e.g. `n=5 lo=0 hi=10` → `[within] WITHIN n=5 lo=0 hi=10 flag=1`
  - **Miss (optional):** e.g. `n=15 lo=0 hi=10` → `[within] WITHIN n=15 lo=0 hi=10 flag=0` **or** `[within] WITHIN miss` (demo may show one miss or avoids FAIL)
- **Optional push:** if the host stack is easy, push the pictured flag (`1` / `0`); marker alone is enough for Lab OK — do not require a real data-stack compare/branch machine.
- **FAIL:** `[within] FAIL reason=<…>` optional (demo **must avoid**). Prefer not emitting `[within] FAIL` on the happy path. Miss is **not** FAIL — miss is `flag=0` or `[within] WITHIN miss`.
- Storage: host ints / flag echo only. **No** real branch XT, no runtime compare re-exec, no IF/THEN reopen, no `0=` deepen, no HERE bump, no arena.
- Nest with prior true / source / env / body / char / state / word / find / tick / create / allot / synonym / exit / buffer / marker / defer / imm / fill / pick / cell / throw / 2var / colon / control / string stubs OK. `dict-reset` unaffected (no new dict entries from range-check marks — fixed host fixtures, not dictionary).
- Still no ANS COUNT (wave20 **3**), no EXECUTE mark (wave20 **4**), no real boolean cell / `0=` deepen, no ACCEPT/REFILL, no linked XT / real DOES> XT / real branch XT / full arena/heap / full Win/Android Forth VM. Tip1 TRUE-FALSE and wave19 tip1–4 (CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD) stay landed — keep cites; this tip does not reopen them. Those stay non-goals / later tips.

## 4. Markers

```
[within] WITHIN [n=<n>] [lo=<lo>] [hi=<hi>] [flag=<0|1>]   # n=/lo=/hi=/flag= optional; hit → flag=1
[within] WITHIN miss                                         # optional alternate miss marker (flag=0 preferred)
[within] FAIL reason=<…>                                     # demo avoids
[within-demo] OK
[within-demo] FAIL
```

Lab greps `[within-demo] OK` plus at least one `[within] WITHIN` with hit `flag=1` (optional `n=` / `lo=` / `hi=` welcome). Demo may also show one miss (`flag=0` or `[within] WITHIN miss`) or avoids FAIL. Demo avoids `[within] FAIL`.

## 5. `within-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; range-check marks need no dict entries.
2. Invoke `WITHIN` (or `within-mark`) on the **hit** fixture (e.g. `n=5 lo=0 hi=10`) → `[within] WITHIN` (+ optional `n=` / `lo=` / `hi=` / `flag=1`).
3. Optional: invoke on a **miss** fixture (e.g. `n=15 lo=0 hi=10`) → `[within] WITHIN` with `flag=0` **or** `[within] WITHIN miss`. Demo may skip miss and still PASS — hit alone is enough for Lab OK when `flag=1` is greppable.
4. Assert no `[within] FAIL` on the happy path. Assert range-check mark did **not** require a real branch XT / runtime compare re-exec / IF/THEN reopen / `0=` deepen (marker-only is enough). Assert host `WITHIN` was not redefined when using the Forth mirror.
5. Prior `true-demo` / `source-demo` / `env-demo` / `body-demo` / `char-demo` / `state-demo` / `word-demo` / `find-demo` / `tick-demo` / `recurse-demo` / `eval-demo` / `parse-demo` / `synonym-demo` / `exit-demo` / `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` / `refined-boot-demo` / `host-boot-demo` / `interpret-demo` / `kernel-demo` still OK.
6. `[within-demo] OK`.

Hit `[within] WITHIN` with `flag=1` (or greppable hit marker) is required. Miss path is optional. Optional `n=` / `lo=` / `hi=` echo is not required for Lab OK when hit marker is greppable. No real branch XT. No IF/THEN reopen. No runtime compare re-exec.

## 6. Thin amend — companions

### `docs/KERNEL.md`

- Companions: add `WITHIN.md` (wave20 **2**); **keep** tip1 TRUE-FALSE cite and wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites and wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites and wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `WITHIN` stub + `within-demo` (cite tip; Forth mirror `within-mark` — range-check mark only; **do not** redefine host `WITHIN`; **not** real branch XT / runtime compare re-exec / IF/THEN reopen; optional `n=` / `lo=` / `hi=` / `flag=<0|1>` — signed `lo ≤ n < hi` picture welcome).
- Non-goals: `WITHIN` range-check mark → `docs/WITHIN.md`. TRUE/FALSE stay on `TRUE-FALSE.md`. CHAR/CHARS/[CHAR] stay on `CHAR-CHARS.md`. >BODY stays on `TO-BODY.md`. ENVIRONMENT? stays on `ENVIRONMENT-QUERY.md`. SOURCE/PAD stay on `SOURCE-PAD.md`. CONTROL IF/THEN/ELSE stay on `CONTROL.md`. COUNT / EXECUTE still later (wave20 **3–4**).
- Acceptance: Lab smokes `within-demo` (and retains `true-demo` + `source-demo` + `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `pick-demo` + `cell-demo` + `control-demo` + prior demos).
- Cite: `docs/WITHIN.md`.

### `docs/CONTROL.md`

- Companions / Status: add `WITHIN.md` (wave20 **2** companion cite); **keep** TRUE-FALSE / COLON / INTERPRET / KERNEL / BEGIN-UNTIL / DO-LOOP / CASE-OF / THROW-CATCH cites — do not wipe wave10 CONTROL content.
- Purpose / §3: IF/THEN/ELSE stay balance-only stubs; `WITHIN` range-check mark pairs with tip1 flag picture and control `taken=` — **not** a real branch XT / IF reopen / runtime compare re-exec. Do not wipe wave10 CONTROL content.
- Non-goals: `WITHIN` → `docs/WITHIN.md` (wave20 **2**). TRUE/FALSE stay on `TRUE-FALSE.md` (wave20 **1**). Real branch XT still out.
- Acceptance: Lab smokes `within-demo` (retains `control-demo` + `true-demo`).
- Cite: `docs/WITHIN.md`.

### `docs/TRUE-FALSE.md`

- Companions / Status: add `WITHIN.md` (wave20 **2** companion cite); **keep** KERNEL / CELL-CELLS / PICK-ROLL / CONTROL / HOST-PARITY cites — do not wipe tip1 TRUE-FALSE content.
- Purpose / §3 / non-goals: TRUE/FALSE stay constant marks; `WITHIN` is the sibling **range-check** mark that may echo optional `flag=<0|1>` — **not** a TRUE/FALSE reopen / boolean cell rewrite / `0=` deepen / branch XT. Do not wipe tip1 TRUE-FALSE content. Thin amend only — tip1 primary checksum will CHANGE (expected).
- Non-goals: `WITHIN` → `docs/WITHIN.md` (wave20 **2**). TRUE/FALSE stay on this tip (already landed). COUNT / EXECUTE still later (wave20 **3–4**).
- Acceptance: Lab smokes `within-demo` (retains `true-demo`).
- Cite: `docs/WITHIN.md`.

### Optional — `docs/PICK-ROLL.md`

- Companions: add light `WITHIN.md` (wave20 **2**) cite; **keep** TRUE-FALSE / KERNEL / CELL-CELLS cites — do not wipe wave15 PICK-ROLL content.
- Purpose / non-goals: stack/?DUP flag pictures stay; `WITHIN` is a sibling **range-check** flag mark — **not** a PICK/ROLL/?DUP reopen / real threaded stack / compare machine. Do not wipe wave15 PICK-ROLL content. Host `2dup`/`2drop`/`2swap` stay untouched.
- Non-goals: `WITHIN` → `docs/WITHIN.md` (wave20 **2**). TRUE/FALSE stay on `TRUE-FALSE.md`. PICK/ROLL/DEPTH/?DUP stay on this tip (already landed).
- Acceptance: Lab smokes `within-demo` (retains `pick-demo` + `true-demo`).
- Cite: `docs/WITHIN.md`.

### Optional — `docs/CELL-CELLS.md`

- Companions: add light `WITHIN.md` (wave20 **2**) cite; **keep** TRUE-FALSE / CHAR-CHARS / ENVIRONMENT-QUERY / ALLOT-HERE / KERNEL / VARIABLE-CONST cites — do not wipe wave15 CELL content.
- Purpose / non-goals: cell-unit stubs stay; `WITHIN` optional `flag=` may picture a cell-width flag beside tip1 TRUE/FALSE — **not** a CELL/ALIGN reopen / cell-size rewrite / compare machine. Do not wipe wave15 / wave19 / wave20 tip1 CELL/CHAR/ENV/TRUE content.
- Non-goals: `WITHIN` → `docs/WITHIN.md` (wave20 **2**). TRUE/FALSE stay on `TRUE-FALSE.md`. CELL/CELLS/ALIGN/ALIGNED stay on this tip (already landed).
- Acceptance: Lab smokes `within-demo` (retains `cell-demo` + `true-demo`).
- Cite: `docs/WITHIN.md`.

Do **not** wipe tip1 TRUE-FALSE content or wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites or wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites or wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites or wave16 DEFER / MARKER / BUFFER / EXIT cites or wave15 CELL / IMMEDIATE / FILL / PICK prior content. Do **not** amend ARCHITECTURE / IMPLEMENTATION-GAPS / CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD / DOCS-CITES docs this tip (proposal amends are KERNEL + CONTROL + TRUE-FALSE + optional PICK-ROLL / CELL-CELLS only). Leave CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD / ARCHITECTURE / IMPLEMENTATION-GAPS untouched this tip. Tip5 cites come after 1–4 PASS.

## 7. Non-goals

- Real branch XT / runtime compare re-exec / LEAVE jump
- `IF` / `THEN` / `ELSE` reopen as real branch XT (wave10 — already mark-only; keep cites)
- Unsigned-wrap rewrite required (signed picture for fixed demo triple is enough)
- `0=` / `AND` / `OR` / `INVERT` / flag algebra deepen
- Real boolean cell rewrite (tip1 TRUE-FALSE already stubbed; keep cites — do not wipe)
- ANS `COUNT` c-addr picture (wave20 **3**); host `ENTRY-COUNT` / `SYN-COUNT` / `words-count` are **not** ANS COUNT
- `EXECUTE` xt-id invoke mark (wave20 **4**); not real XT execute
- Docs cites pass (wave20 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `TRUE` / `FALSE` reopen (wave20 **1** — already stubbed; keep cites; thin companion amend only)
- `CHAR` / `CHARS` / `[CHAR]` reopen (wave19 **1** — already stubbed; keep cites)
- `>BODY` reopen (wave19 **2** — already stubbed; keep cites)
- `ENVIRONMENT?` reopen (wave19 **3** — already stubbed; keep cites)
- `SOURCE` / `PAD` reopen (wave19 **4** — already stubbed; keep cites); `ACCEPT` / `REFILL` / full input-buffer VM still out
- `CELL` / `CELLS` / `ALIGN` / `ALIGNED` reopen (wave15 — already stubbed; keep cites)
- `PICK` / `ROLL` / `DEPTH` / `?DUP` reopen (wave15 — already stubbed; keep cites)
- Real DOES> XT chaining / threaded child runtime body
- Full Dusk arena / pool / free / fragmentation model
- Linked XT / executing postponed XT / real STATE cell
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live — **skip 2DUP-FAMILY**)
- `ABORT"` polish (already optional-wired inside `throw-demo` — **skip**)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/WITHIN.md` present (Research byte-copy OK); `KERNEL.md` + `CONTROL.md` + `TRUE-FALSE.md` thin amends present (+ optional `PICK-ROLL.md` / `CELL-CELLS.md`); tip1 TRUE-FALSE content retained (thin companion cite only — tip1 primary checksum CHANGES as expected); wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites, wave18 tip1–4 cites, wave17 tip1–4 cites, wave16 DEFER/MARKER/BUFFER/EXIT cites, and wave15 CELL/IMMEDIATE/COLON/KERNEL prior text retained; host `WITHIN` untouched via mirrors; CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD / ARCHITECTURE / IMPLEMENTATION-GAPS byte-copy unchanged.
2. `within-demo` → OK (markers §4; `[within] WITHIN` hit greppable with `flag=1`; optional `n=` / `lo=` / `hi=`; optional miss `flag=0` or `[within] WITHIN miss`; no FAIL on happy path; no real branch XT / runtime compare re-exec / IF/THEN reopen / `0=` deepen). Prior `true-demo` + `source-demo` + `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `recurse-demo` + `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` + `refined-boot-demo` + `host-boot-demo` + `interpret-demo` + `kernel-demo` still OK.
3. Regression green (wave20 tip1 + wave19 tip1–5 + wave18 tip1–5 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `within-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**), `docs/CONTROL.md` (wave10 **2** — IF `taken=` companion; WITHIN is range-check mark — not IF/THEN reopen)
- `docs/TRUE-FALSE.md` (wave20 **1** — flag-picture companion; WITHIN pairs without promoting to compare/branch machine)
- `docs/PICK-ROLL.md` (wave15 **2**, optional — stack/?DUP flag companion)
- `docs/CELL-CELLS.md` (wave15 **1**, optional — unit/cell picture companion for optional flag echo)
- `docs/CHAR-CHARS.md` (wave19 **1** — prior tip; keep cites)
- `docs/TO-BODY.md` (wave19 **2** — prior tip; keep cites)
- `docs/ENVIRONMENT-QUERY.md` (wave19 **3** — prior tip; keep cites)
- `docs/SOURCE-PAD.md` (wave19 **4** — prior tip; keep cites)
- `docs/TICK.md` (wave18 **1**), `docs/STATE-COMPILE.md` (wave18 **4**), `docs/FIND.md` (wave18 **2**)
- `docs/COLON.md` (wave9 **4**), `docs/BEGIN-UNTIL.md` (wave11 **1**), `docs/DO-LOOP.md` (wave12 **1**)
- `forth/tritium/kernel.fs` (within-mark only — do not redefine host WITHIN)
- ANS Forth `WITHIN` (range-check mark only — not real branch XT / runtime compare re-exec / IF/THEN reopen; signed `lo ≤ n < hi` picture for fixed demo triple)
- Explicit deferral: WAVE18-PROPOSAL + WAVE19-PROPOSAL + WAVE20-PROPOSAL (`WITHIN` — range-check mark; not branch XT)
- Base tip: `6a6a87d` / `6a6a87d2dc573211173f4c67d4d133c30418a0c7` (#92 wave20 tip1 TRUE-FALSE PASS)
- Wave20 proposal: `/workspace/tritium-research-docs/WAVE20-PROPOSAL.md`
