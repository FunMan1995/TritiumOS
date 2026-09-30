# RECURSE — `RECURSE` mark-only + `recurse-demo`

**Status:** Shipper-ready stub spec (wave17 item **4**; thin amend wave20 **4** EXECUTE companion cite)
**Canonical brief:** ANS-shaped `RECURSE` (thin flag + marker only); `docs/COLON.md` (wave9 **4**); `docs/EXIT-QUIT.md` (wave16 **4**); `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**); `docs/KERNEL.md` (wave7 **5**); `docs/EVALUATE-INCLUDE.md` (wave17 **3**); `docs/PARSE-NAME.md` (wave17 **2**); `docs/SYNONYM-ALIAS.md` (wave17 **1**); explicit WAVE16 / EXIT-QUIT / WAVE17 deferral closed as mark-only stub (`recurse-mark`; not real self-XT)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `recurse.fs`); Linux host REPL; **do not** break `:` / `;` / `colon-demo`; **do not** redefine host `exit`
**Companions:** `docs/COLON.md` (thin amend this tip), `docs/EXIT-QUIT.md` (thin amend this tip), `docs/IMMEDIATE-POSTPONE.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip); `docs/EXECUTE.md` (wave20 **4** — xt-id invoke mark companion cite; not RECURSE self-XT reopen / real XT execute)
**Base tip SHA:** `3105624e` (wave17 tip3 CLOSED / #79 EVALUATE-INCLUDE) / full `3105624ec26c9f3f25507e1048252ed7f2b8177b`

## 1. Purpose

WAVE16 EXIT-QUIT and WAVE17 explicitly deferred real `RECURSE` (self-XT / recursive colon body). Colon-def, IMMEDIATE/POSTPONE flag stubs, and EXIT/QUIT thin control markers already exist; a Forth-surface **recurse mark** does not. This tip lands a **stub** flag + marker only: `RECURSE` prints `[recurse] RECURSE` (+ optional `name=` of current colon-def stub / `depth=`) and does **not** compile or execute a self-XT. Prefer calling from inside a colon-def / demo fixture so optional name mark is meaningful. Smoke via **`recurse-demo`**. Forth mirror **`recurse-mark`** so Android/host wordlists that mention `recurse` and in-tree colon bodies stay safe. **Do not break colon** (`:` / `;` / `colon-demo` remain green). No real RS unwind beyond wave14/16 marks. Closes WAVE16 + EXIT-QUIT non-goal (“RECURSE real self-XT”) as a thin stub before tip5 cites.

 Sibling xt-id invoke mark → `docs/EXECUTE.md` (wave20 **4**; Forth mirror `execute-mark` — mark only; **not** a RECURSE self-XT reopen / real XT execute).

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `RECURSE` / `recurse-mark` | `( -- )` | Flag + marker only; print `[recurse] RECURSE` (+ optional `name=<colon-def>` / `depth=<n>`); does **not** compile or execute a self-XT |
| `recurse-demo` | `( -- )` | See §5 |

Host note: bind `RECURSE` on the Linux REPL only when safe; Forth mirror **`recurse-mark`** is the required Lab surface so Android/host wordlists that mention `recurse` and in-tree colon bodies stay untouched. Prefer invoking from inside a colon-def / demo fixture so optional `name=` is meaningful. Captured values are host ints / name strings only — not a real self-XT or RS frame.

## 3. Stub semantics

- **RECURSE:** print `[recurse] RECURSE` (+ optional `name=<name>` of the current open colon-def stub, and/or `depth=<n>` reflecting colon-def / catch-depth stub). Does **not** compile a self-XT into the colon body, does not execute the current definition recursively, does not walk a real return stack, does not redefine `:` / `;` / host `exit`. Prefer calling from inside a colon-def / demo fixture so optional name mark is meaningful.
- **Outside colon / no frame:** optional `[recurse] FAIL reason=noframe` when `RECURSE` is invoked with no open colon-def / frame mark. Demo **avoids** this path (matching tip1–3 miss/bounds and EXIT noframe style).
- Storage: flag + optional name string / depth int. **No** self-XT compile/exec, no recursive colon body, no real RS unwind beyond wave14/16 marks, **do not** break colon (`:` / `;` / `colon-demo` stay green), **do not** redefine host `exit`.
- Nest with prior eval / parse / synonym / exit / buffer / marker / defer / imm / fill / pick / cell / allot / throw / 2var / create / colon / control / string stubs OK. Soft KERNEL `abort` / `(abort")`, wave14 CATCH/THROW, and wave16 EXIT/QUIT mark stubs stay as-is beside this marker.
- Still no real self-XT / recursive colon body, no colon-def / `;` break, no host `exit` redefine, no linked XT, no real DOES> XT, no real branch XT, no full arena/heap, no full Win/Android Forth VM. Those stay non-goals / later tips.

## 4. Markers

```
[recurse] RECURSE                     # + optional name=<colon-def> / depth=<n>
[recurse] FAIL reason=noframe         # optional; demo avoids
[recurse-demo] OK
[recurse-demo] FAIL
```

Lab greps `[recurse-demo] OK` plus at least one `[recurse] RECURSE` (optional `name=` / `depth=` welcome). Demo avoids `[recurse] FAIL reason=noframe`. Prior `colon-demo` / `exit-demo` / `imm-demo` / `eval-demo` / `parse-demo` / `synonym-demo` still OK — tip4 does **not** break `:` / `;` / colon body markers.

## 5. `recurse-demo`

1. Clean slate / `dict-reset` (or cold path); ensure colon-def stub is available.
2. Open a colon-def / demo fixture (e.g. `: recurse-fixture` …) so optional `name=` is meaningful.
3. Inside that fixture (or while colon-def flag is set), invoke `RECURSE` (or `recurse-mark`) → `[recurse] RECURSE` (+ optional `name=recurse-fixture` / `depth=`). Assert **no** self-XT compile/exec is required (marker alone is enough).
4. Close the colon-def with `;` as usual. Assert `:` / `;` / `colon-demo` path remains green (tip4 does **not** break colon).
5. Assert no `[recurse] FAIL reason=noframe` on the happy path (demo avoids outside-colon RECURSE).
6. Prior `eval-demo` / `parse-demo` / `synonym-demo` / `exit-demo` / `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` / `refined-boot-demo` / `host-boot-demo` / `interpret-demo` still OK.
7. `[recurse-demo] OK`.

RECURSE marker is required. Optional `name=` / `depth=` and noframe FAIL are not required for Lab OK beyond greppable `[recurse] RECURSE`. No self-XT. No colon break. No host `exit` redefine.

## 6. Thin amend — companions

### `docs/COLON.md`

- Companions: add `RECURSE.md` (wave17 **4**); **keep** EXIT-QUIT / IMMEDIATE-POSTPONE / INTERPRET / KERNEL / CONTROL / VARIABLE-CONST / CREATE-DOES cites.
- Purpose / words / §3: colon-def / body-marker stubs stay; RECURSE may optionally observe colon-def name / depth — **not** a real self-XT / recursive colon body. **Do not break** `:` / `;` / `colon-demo`. Do not wipe wave9/15/16 colon text.
- Words table: add `RECURSE` / `recurse-mark` stubs + `recurse-demo` (cite tip; Forth mirror — flag + marker only).
- Non-goals: `RECURSE` mark-only stub → `docs/RECURSE.md` (wave17 **4**). Still no real self-XT / linked XT compiler. EXIT/QUIT stay on `EXIT-QUIT.md`.
- Acceptance: Lab smokes `recurse-demo` (retains `colon-demo` + `exit-demo` + `imm-demo`).
- Cite: `docs/RECURSE.md`.

### `docs/EXIT-QUIT.md`

- Companions: add `RECURSE.md` (wave17 **4**); **keep** COLON / INTERPRET / THROW-CATCH / KERNEL / BUFFER / MARKER / DEFER / IMMEDIATE cites.
- Purpose / §3 / non-goals: EXIT/QUIT stay mark-only; RECURSE is a sibling thin colon-side marker — **not** a real self-XT / RS unwind beyond wave14/16 marks. Do not wipe wave16 EXIT-QUIT content. Do not redefine host `exit`.
- Non-goals: `RECURSE` mark-only stub → `docs/RECURSE.md` (wave17 **4**). Real self-XT still later. Real RS unwind still later.
- Acceptance: Lab smokes `recurse-demo` (retains `exit-demo` + `colon-demo`).
- Cite: `docs/RECURSE.md`.

### `docs/IMMEDIATE-POSTPONE.md`

- Companions: add `RECURSE.md` (wave17 **4**); **keep** COLON / INTERPRET / KERNEL / FILL / PICK / CELL cites.
- Purpose / §3 / non-goals: IMMEDIATE/POSTPONE stay flag + name mark; RECURSE is a separate colon-side mark-only stub — **not** a linked XT / self-XT / executing postponed XT. Do not wipe wave15 IMMEDIATE-POSTPONE content.
- Non-goals: `RECURSE` mark-only stub → `docs/RECURSE.md` (wave17 **4**). Host `EXIT` / `QUIT` stay on wave16 `EXIT-QUIT.md` (mirrors only). Real self-XT / linked XT still later.
- Acceptance: Lab smokes `recurse-demo` (retains `imm-demo` + `colon-demo`).
- Cite: `docs/RECURSE.md`.

### `docs/KERNEL.md`

- Companions: add `RECURSE.md` (wave17 **4**); **keep** tip1 SYNONYM-ALIAS + tip2 PARSE-NAME + tip3 EVALUATE-INCLUDE cites and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `RECURSE` / `recurse-mark` stubs + `recurse-demo` (cite tip; Forth mirror — flag + marker only; **do not** break colon / redefine host `exit`; **not** a real self-XT / recursive colon body).
- Non-goals: RECURSE mark-only stub → `docs/RECURSE.md`. EVALUATE/INCLUDE stays on wave17 **3**. PARSE/PARSE-NAME stays on wave17 **2**. SYNONYM/ALIAS stays on wave17 **1**. DEFER/IS/ACTION-OF / MARKER / BUFFER: / EXIT/QUIT stay on wave16 docs. Real self-XT / real RS unwind / linked XT still later.
- Acceptance: Lab smokes `recurse-demo` (and retains `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `colon-demo` + prior demos).
- Cite: `docs/RECURSE.md`.

Do **not** wipe tip1 SYNONYM cites or tip2 PARSE cites or tip3 EVALUATE cites or wave16 DEFER / MARKER / BUFFER / EXIT cites or wave15 IMMEDIATE / COLON / EXIT-QUIT / KERNEL prior content. Do **not** amend EVALUATE-INCLUDE / PARSE-NAME / SYNONYM-ALIAS / DOCS-CITES docs this tip (proposal amends are COLON + EXIT-QUIT + IMMEDIATE-POSTPONE + KERNEL only).

## 7. Non-goals

- `EXECUTE` xt-id invoke mark → `docs/EXECUTE.md` (wave20 **4**; Forth mirror `execute-mark` — mark only; **not** a RECURSE self-XT reopen / real XT execute / linked XT)
- Real `RECURSE` self-XT / recursive colon body / compiling current-definition XT
- Breaking colon-def / `;` / `colon-demo` (must remain green)
- Redefining host Forth `exit` / `quit` (wave16 EXIT-QUIT mirrors stay)
- Real RS unwind / interpret restart / frame restore beyond wave14/16 marks
- Linked XT compiler / executing postponed XT / executing bound XT
- Docs cites pass (wave17 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `EVALUATE` / `INCLUDE` reopen (wave17 **3** — already stubbed; keep cites)
- `PARSE` / `PARSE-NAME` reopen (wave17 **2** — already stubbed; keep cites)
- `SYNONYM` / `ALIAS` reopen (wave17 **1** — already stubbed; keep cites)
- `MARKER` / `BUFFER:` / `EXIT` / `QUIT` (wave16 **2–4** — already stubbed; keep cites)
- `DEFER` / `IS` / `ACTION-OF` (wave16 **1** — already stubbed; keep cites)
- IMMEDIATE / POSTPONE / FILL / PICK / CELL (wave15 — already stubbed)
- Real DOES> XT chaining / threaded child runtime body
- Real branch XT / LEAVE jump
- Full Dusk arena / pool / free / fragmentation model
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live — skip 2DUP-FAMILY)
- `ABORT"` polish (already optional-wired inside `throw-demo` — skip)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/RECURSE.md` present (Research byte-copy OK); `COLON.md` + `EXIT-QUIT.md` + `IMMEDIATE-POSTPONE.md` + `KERNEL.md` thin amends present (wave9/16/15/7 text, tip1 SYNONYM cites, tip2 PARSE cites, tip3 EVALUATE cites, and wave16 DEFER/MARKER/BUFFER/EXIT cites retained; colon `:` / `;` / `colon-demo` remain green).
2. `recurse-demo` → OK; wave20 **4**: `exec-demo` → OK (retains `recurse-demo`; not self-XT reopen) (markers §4; RECURSE greppable; optional `name=` / `depth=` welcome; no noframe FAIL on happy path). Prior `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` + `refined-boot-demo` + `host-boot-demo` + `interpret-demo` still OK.
3. Regression green (wave17 tip1–3 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `recurse-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/EXECUTE.md` (wave20 **4** — invoke-mark companion; not RECURSE self-XT reopen)
- `docs/COLON.md` (wave9 **4**), `docs/EXIT-QUIT.md` (wave16 **4**), `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**), `docs/KERNEL.md` (wave7 **5**)
- `docs/EVALUATE-INCLUDE.md` (wave17 **3**), `docs/PARSE-NAME.md` (wave17 **2**), `docs/SYNONYM-ALIAS.md` (wave17 **1**)
- `docs/DEFER-IS.md` (wave16 **1**), `docs/MARKER.md` (wave16 **2**), `docs/BUFFER-COLON.md` (wave16 **3**)
- `docs/FILL-MOVE.md` (wave15 **3**), `docs/PICK-ROLL.md` (wave15 **2**), `docs/CELL-CELLS.md` (wave15 **1**)
- `docs/THROW-CATCH.md` (wave14 **4**), `docs/INTERPRET.md` (wave8 **1**)
- `forth/tritium/kernel.fs` (colon / exit mirrors stay; recurse-mark only)
- ANS Forth `RECURSE` (mark-only stub — no self-XT / recursive colon body)
- Explicit deferral: WAVE16-PROPOSAL + EXIT-QUIT non-goal + WAVE17-PROPOSAL (`RECURSE` — mark-only `recurse-mark`; not real self-XT)
- Base tip: `3105624e` / `3105624ec26c9f3f25507e1048252ed7f2b8177b` (#79 wave17 tip3 EVALUATE-INCLUDE)
- Wave17 proposal: `/workspace/tritium-research-docs/WAVE17-PROPOSAL.md`
