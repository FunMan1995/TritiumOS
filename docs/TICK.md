# TICK — `'` / `[']` tick markers + `tick-demo`

**Status:** Shipper-ready stub spec (wave18 item **1**; thin amend wave18 **4** STATE-COMPILE companion cite)
**Canonical brief:** ANS-shaped `'` / `[']` (name→stub-xt-id mark only); `docs/KERNEL.md` (wave7 **5**); `docs/DEFER-IS.md` (wave16 **1** — optional `' <xt> IS <name>` form); `docs/IMMEDIATE-POSTPONE.md` (wave15 **4** — POSTPONE name-mark sibling); `docs/CREATE-DOES.md` (wave13 **3**); `docs/COLON.md` (wave9 **4**, optional companion); `docs/STATE-COMPILE.md` (wave18 **4** — stub `xt=` companion for COMPILE,); explicit WAVE17 / WAVE18 deferral closed as tick mark only (not XT execute / FIND rewrite / COMPILE,)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `tick.fs`); Linux host REPL; **do not** redefine host tick / Android wordlists that already bind `'` / `[']`
**Companions:** `docs/KERNEL.md` (thin amend this tip), `docs/DEFER-IS.md` (thin amend this tip), `docs/IMMEDIATE-POSTPONE.md` (thin amend this tip), `docs/CREATE-DOES.md` (thin amend this tip); optional light cite `docs/COLON.md`; optional companion cite `docs/STATE-COMPILE.md` (wave18 **4**)
**Base tip SHA:** `4bdfc39` (wave17 tip5 CLOSED / #81 DOCS-CITES) / full `4bdfc39f3431b157d039f19d85cd681e0aad98a2`

## 1. Purpose

WAVE17 and WAVE18 explicitly deferred `'` / `[']` (tick mark; not XT execute). DEFER already documents an optional `' <xt> IS <name>` form that references tick without a dedicated tip; POSTPONE is a name-mark sibling that must not become a real XT vector. This tip lands **stub** name→stub-xt-id markers only: `'` `<name>` prints `[tick] ' name=` (+ optional `xt=<id>` / `i=<index>`) for a known dict/demo name; `[']` `<name>` prints `[tick] ['] name=` (+ optional `xt=`) as the compile-time sibling mark (flag echo only — does **not** compile an XT into a body). Smoke via **`tick-demo`**. Forth mirrors **`tick-mark` / `bracket-tick`** so host tick / Android wordlists stay safe. **Not** XT execute, not FIND rewrite, not COMPILE,, not STATE deepen. Pairs with wave16 DEFER optional tick form and wave15 POSTPONE name-mark without promoting either to a real XT vector. Unlocks consistent stub `xt=` ids for later FIND / COMPILE, tips.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `'` / `tick-mark` | `( "name" -- )` *or* `( -- )` then parse name | Name→stub-xt-id mark; print `[tick] ' name=<name>` (+ optional `xt=<id>` / `i=<index>`); does **not** execute the looked-up XT |
| `[']` / `bracket-tick` | `( "name" -- )` *or* `( -- )` then parse name | Compile-time sibling mark; print `[tick] ['] name=<name>` (+ optional `xt=<id>`); flag echo only — does **not** compile an XT into a body |
| `tick-demo` | `( -- )` | See §5 |

Host note: bind `'` / `[']` on the Linux REPL **only if** those names do not collide with host Forth tick / Android wordlists in the same load path. Prefer Forth mirrors **`tick-mark` / `bracket-tick`** as the Lab-facing surface when in doubt — **do not** redefine host tick. Optional stub `xt=` / `i=` are host ints / entry indices only — not callable XTs. Prefer looking up a known dict/demo name (CREATE / colon / VARIABLE / DEFER entry, or a greppable demo fixture) so miss FAIL is avoided.

## 3. Stub semantics

- **`'` name:** resolve a known dict/demo name (via `entry-find` / host mirror / demo fixture table). Print `[tick] ' name=<name>` (+ optional `xt=<id>` stub id and/or `i=<index>` entry index). **Does not** execute the looked-up word, push a real XT, FIND-rewrite, or COMPILE,. Captured values are host ints / name strings only.
- **`[']` name:** same name resolve as compile-time sibling. Print `[tick] ['] name=<name>` (+ optional `xt=<id>`). Flag / name / stub-id echo only — does **not** append an XT to a colon body, does not set STATE, does not execute. Optional `[tick] compile-only` when colon-def flag is set is **not** required this tip (STATE/COMPILE, is wave18 tip **4**).
- **Missing name:** `[tick] FAIL reason=miss` (demo **must avoid** — always tick a known existing dict/demo name).
- Storage: name string + optional stub xt id / entry index. **No** XT execute, no FIND rewrite of host `find`/`findentry`/`entry-find`, no COMPILE, / linked XT body append, no STATE query reopen.
- Nest with prior recurse / eval / parse / synonym / exit / buffer / marker / defer / imm / fill / pick / cell / allot / throw / 2var / create / colon / control / string stubs OK. `dict-reset` clears as usual. Soft KERNEL `abort` / `(abort")`, wave14 CATCH/THROW, and wave16 EXIT/QUIT mark stubs stay as-is beside this marker.
- Still no XT execute / FIND deepen (wave18 **2** — landed as mark) / WORD-BL (wave18 **3** — landed as stub) / linked XT via COMPILE, (wave18 **4** STATE-COMPILE lands query + compile-comma mark only — **not** XT body append) / real DOES> XT / real branch XT / full arena/heap / full Win/Android Forth VM. Those stay non-goals / later tips.

## 4. Markers

```
[tick] ' name=<name> [xt=<id>] [i=<index>]   # xt=/i= optional
[tick] ['] name=<name> [xt=<id>]             # xt= optional; compile-time sibling mark only
[tick] FAIL reason=miss                      # demo avoids
[tick-demo] OK
[tick-demo] FAIL
```

Lab greps `[tick-demo] OK` plus at least one `[tick] ' name=` and one `[tick] ['] name=`. Optional `xt=` / `i=` fields are not required for Lab OK when present. Demo avoids `[tick] FAIL reason=miss`.

## 5. `tick-demo`

1. Clean slate / `dict-reset` (or cold path).
2. Ensure a known name exists (e.g. create via `entry-create` / VARIABLE / CREATE / colon stub, or use a greppable existing demo name such as a prior `widget` fixture) so miss FAIL is avoided.
3. Invoke `'` (or `tick-mark`) on that known name → `[tick] ' name=<name>` (+ optional `xt=` / `i=`).
4. Invoke `[']` (or `bracket-tick`) on that same (or another known) name → `[tick] ['] name=<name>` (+ optional `xt=`). Assert **no** XT was compiled into a body (marker alone is enough).
5. Assert no `[tick] FAIL reason=miss` on the happy path. Assert tick did **not** require executing the looked-up XT / FIND rewrite / COMPILE, (marker-only is enough).
6. Prior `recurse-demo` / `eval-demo` / `parse-demo` / `synonym-demo` / `exit-demo` / `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` / `refined-boot-demo` / `host-boot-demo` / `interpret-demo` still OK.
7. `[tick-demo] OK`.

`'` + `[']` markers are required. Miss FAIL path is not exercised by the demo. Optional `xt=` / `i=` echo is not required for Lab OK. No XT execute. No FIND rewrite. No COMPILE,.

## 6. Thin amend — companions

### `docs/KERNEL.md`

- Companions: add `TICK.md` (wave18 **1**); **keep** tip1–4 wave17 SYNONYM / PARSE / EVALUATE / RECURSE cites and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `'` / `[']` stubs + `tick-demo` (cite tip; Forth mirrors `tick-mark` / `bracket-tick` — name→stub-xt-id mark only; **do not** redefine host tick / Android wordlists; **not** XT execute / FIND rewrite / COMPILE,).
- Non-goals: TICK name→stub-xt-id mark → `docs/TICK.md`. RECURSE / EVALUATE / PARSE / SYNONYM stay on wave17 docs. DEFER/IS/ACTION-OF / MARKER / BUFFER: / EXIT/QUIT stay on wave16 docs. FIND deepen / WORD-BL / STATE-COMPILE still later (wave18 **2–4**). XT execute / linked XT still later.
- Acceptance: Lab smokes `tick-demo` (and retains `recurse-demo` + `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `colon-demo` + prior demos).
- Cite: `docs/TICK.md`.

### `docs/DEFER-IS.md`

- Companions: add `TICK.md` (wave18 **1**); **keep** CREATE-DOES / VARIABLE-CONST / KERNEL cites and wave17 SYNONYM + wave16 MARKER / BUFFER / EXIT closed cites (do not wipe).
- Purpose / §2 / §3: deferred-word stubs stay name + bind markers; optional `' <xt> IS <name>` form now points at this tip’s tick mark for the stub xt id — **not** a real XT vector / execute. Do not wipe wave16 DEFER content. Do not require `defer-demo` to call tick (optional form remains optional).
- Non-goals: `'` / `[']` tick mark → `docs/TICK.md` (wave18 **1**). SYNONYM/ALIAS stay wave17 **1**. MARKER / BUFFER: / EXIT-QUIT stay wave16 **2–4**. Linked XT / executing bound XT still later.
- Acceptance: Lab smokes `tick-demo` (retains `defer-demo`).
- Cite: `docs/TICK.md`.

### `docs/IMMEDIATE-POSTPONE.md`

- Companions: add `TICK.md` (wave18 **1**); **keep** COLON / INTERPRET / KERNEL / RECURSE / EXIT cites.
- Purpose / §3 / non-goals: IMMEDIATE/POSTPONE stay flag + name mark; tick is a sibling name→stub-xt-id mark — **not** a linked XT / FIND-then-compile / executing postponed XT / COMPILE,. Do not wipe wave15 IMMEDIATE-POSTPONE content. STATE/COMPILE, deepen stays wave18 tip **4**.
- Non-goals: `'` / `[']` tick mark → `docs/TICK.md` (wave18 **1**). `RECURSE` mark-only stays on `RECURSE.md`. Host `EXIT` / `QUIT` stay on wave16 `EXIT-QUIT.md`. Real STATE/COMPILE, / linked XT still later.
- Acceptance: Lab smokes `tick-demo` (retains `imm-demo` + `recurse-demo` + `colon-demo`).
- Cite: `docs/TICK.md`.

### `docs/CREATE-DOES.md`

- Companions: add `TICK.md` (wave18 **1**); **keep** COLON / VARIABLE-CONST / KERNEL / DEFER / SYNONYM cites.
- Purpose / §3: CREATE/DOES> stay defining-word markers; tick may look up a CREATE name for stub xt-id echo — **not** a real DOES> XT / child body / executing XT. Do not wipe wave13 CREATE-DOES content.
- Non-goals: `'` / `[']` tick mark → `docs/TICK.md` (wave18 **1**). DEFER/IS stay on `DEFER-IS.md`. SYNONYM/ALIAS stay on `SYNONYM-ALIAS.md`. Still no real DOES> XT chaining.
- Acceptance: Lab smokes `tick-demo` (retains `create-demo`).
- Cite: `docs/TICK.md`.

### Optional — `docs/COLON.md`

- Companions: add light `TICK.md` (wave18 **1**) cite; **keep** EXIT-QUIT / IMMEDIATE-POSTPONE / RECURSE / INTERPRET / KERNEL cites.
- Purpose / non-goals: colon-def / body-marker stubs stay; tick may optionally observe a colon-def name as stub xt-id — **not** a real self-XT / COMPILE, / linked XT. **Do not break** `:` / `;` / `colon-demo`. Do not wipe wave9/15/16/17 colon text.
- Non-goals: `'` / `[']` tick mark → `docs/TICK.md` (wave18 **1**). RECURSE / EXIT / IMMEDIATE stay on prior docs.
- Acceptance: Lab smokes `tick-demo` (retains `colon-demo` + `recurse-demo`).
- Cite: `docs/TICK.md`.

Do **not** wipe wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites or wave16 DEFER / MARKER / BUFFER / EXIT cites or wave15 IMMEDIATE / COLON / KERNEL prior content. Do **not** amend PARSE-NAME / EVALUATE-INCLUDE / SYNONYM-ALIAS / RECURSE / EXIT-QUIT / DOCS-CITES docs this tip (proposal amends are KERNEL + DEFER-IS + IMMEDIATE-POSTPONE + CREATE-DOES + optional COLON only).

## 7. Non-goals

- Executing looked-up XT / calling through stub xt id
- Linked XT compile / compiling XT into colon body via `[']`
- FIND deepen / rewriting host `find` / `findentry` / `entry-find` (wave18 **2**)
- `WORD` / `BL` stubs (wave18 **3**)
- `STATE` / `COMPILE,` stubs → `docs/STATE-COMPILE.md` (wave18 **4**; Forth mirrors `state-flag` / `compile-comma` — query + compile-comma mark only; may echo tip1 stub `xt=`; not linked XT / XT body append / real STATE cell)
- Docs cites pass (wave18 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `RECURSE` reopen (wave17 **4** — already mark-only; keep cites)
- `EVALUATE` / `INCLUDE` reopen (wave17 **3** — already stubbed; keep cites)
- `PARSE` / `PARSE-NAME` reopen (wave17 **2** — already stubbed; keep cites)
- `SYNONYM` / `ALIAS` reopen (wave17 **1** — already stubbed; keep cites)
- `MARKER` / `BUFFER:` / `EXIT` / `QUIT` (wave16 **2–4** — already stubbed; keep cites)
- `DEFER` / `IS` / `ACTION-OF` reopen (wave16 **1** — already stubbed; optional tick form cites this tip; keep cites)
- IMMEDIATE / POSTPONE / FILL / PICK / CELL (wave15 — already stubbed)
- Real DOES> XT chaining / threaded child runtime body
- Real branch XT / LEAVE jump
- Full Dusk arena / pool / free / fragmentation model
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live — **skip 2DUP-FAMILY**)
- `ABORT"` polish (already optional-wired inside `throw-demo` — **skip**)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/TICK.md` present (Research byte-copy OK); `KERNEL.md` + `DEFER-IS.md` + `IMMEDIATE-POSTPONE.md` + `CREATE-DOES.md` thin amends present (+ optional `COLON.md`); wave17 tip1–4 cites, wave16 DEFER/MARKER/BUFFER/EXIT cites, and wave15 IMMEDIATE/COLON/KERNEL prior text retained; host tick / Android wordlists untouched via mirrors.
2. `tick-demo` → OK (markers §4; `'` + `[']` greppable; optional `xt=` / `i=` welcome; no miss FAIL on happy path). Prior `recurse-demo` + `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` + `refined-boot-demo` + `host-boot-demo` + `interpret-demo` still OK; wave18 **2–4**: `find-demo` / `word-demo` / `state-demo` → OK (tick stub `xt=` may echo on COMPILE,; not XT execute).
3. Regression green (wave18 tip1–4 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `tick-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**), `docs/DEFER-IS.md` (wave16 **1**), `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**), `docs/CREATE-DOES.md` (wave13 **3**), `docs/COLON.md` (wave9 **4**, optional)
- `docs/RECURSE.md` (wave17 **4**), `docs/EVALUATE-INCLUDE.md` (wave17 **3**), `docs/PARSE-NAME.md` (wave17 **2**), `docs/SYNONYM-ALIAS.md` (wave17 **1**)
- `docs/MARKER.md` (wave16 **2**), `docs/BUFFER-COLON.md` (wave16 **3**), `docs/EXIT-QUIT.md` (wave16 **4**)
- `docs/FILL-MOVE.md` (wave15 **3**), `docs/PICK-ROLL.md` (wave15 **2**), `docs/CELL-CELLS.md` (wave15 **1**)
- `docs/THROW-CATCH.md` (wave14 **4**), `docs/INTERPRET.md` (wave8 **1**)
- `forth/tritium/kernel.fs` (find/entry aliases stay; tick-mark / bracket-tick only)
- ANS Forth `'` / `[']` (name→stub-xt-id mark only — no XT execute / no compile into body)
- Explicit deferral: WAVE17-PROPOSAL + WAVE18-PROPOSAL (`'` / `[']` — tick mark; not XT execute / FIND rewrite / COMPILE,)
- DEFER optional `' <xt> IS <name>` form → pairs with this tip without promoting to XT vector
- `docs/STATE-COMPILE.md` (wave18 **4** — companion cite; stub `xt=` may echo on COMPILE,)
- Base tip: `4bdfc39` / `4bdfc39f3431b157d039f19d85cd681e0aad98a2` (#81 wave17 tip5 DOCS-CITES)
- Wave18 proposal: `/workspace/tritium-research-docs/WAVE18-PROPOSAL.md`
