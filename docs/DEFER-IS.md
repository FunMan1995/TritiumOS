# DEFER-IS — `DEFER` / `IS` / `ACTION-OF` deferred-word stubs + `defer-demo`

**Status:** Shipper-ready stub spec (wave16 item **1**)
**Canonical brief:** ANS-shaped `DEFER` / `IS` / `ACTION-OF` (thin name + bind markers); `docs/CREATE-DOES.md` (wave13 **3**); `docs/VARIABLE-CONST.md` (wave12 **2**); `docs/KERNEL.md` (wave7 **5**); `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**); explicit WAVE15 deferral closed as stub mirrors only
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `defer.fs`); Linux host REPL
**Companions:** `docs/CREATE-DOES.md` (thin amend this tip), `docs/VARIABLE-CONST.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip); `docs/SYNONYM-ALIAS.md` (wave17 **1** name-map sibling)
**Base tip SHA:** `c46cd68` (wave15 tip5 CLOSED / #71 docs cites) / full `c46cd68de93e9e18e6250bacf1f64a81648ead64`

## 1. Purpose

WAVE15 explicitly deferred `DEFER` / `IS` / `ACTION-OF` because rekia already binds live `defer` / `is` for platform hooks. Defining-word surface (CREATE/DOES>, IMMEDIATE/POSTPONE, VARIABLE/VALUE) exists; a deferred-word stub surface does not. This tip lands **stub** markers only: `DEFER <name>` creates a named deferred entry and prints `[defer] DEFER name=`; `IS <name>` (or `' <xt> IS <name>`) binds a stub action/name and prints `[defer] IS name=` (+ optional `xt=`); `ACTION-OF <name>` prints `[defer] ACTION-OF name=` (+ optional bound name/id). Smoke via **`defer-demo`**. Forth mirrors **`defer-create` / `is-bind` / `action-of-xt`** so rekia's live `defer` / `is` platform hooks stay **untouched**. **Not** a real XT vector table, not linked XT execute, not MARKER restore, not BUFFER:. Pairs with wave13 CREATE / wave15 IMMEDIATE defining-word surface. Name-map stubs (`SYNONYM` / `ALIAS`) → `docs/SYNONYM-ALIAS.md` (wave17 **1**; name→name only — **not** a DEFER vector / linked XT).

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `DEFER` / `defer-create` | `( "name" -- )` | Create named deferred entry; print `[defer] DEFER name=<name>` |
| `IS` / `is-bind` | `( "name" -- )` *or* `( xt "name" -- )` / `' <xt> IS <name>` | Bind a stub action/name (or stub xt id) to an existing deferred entry; print `[defer] IS name=<name>` (+ optional `xt=<id>` / `action=<bound>`) |
| `ACTION-OF` / `action-of-xt` | `( "name" -- )` *or* `( "name" -- xt )` | Query/print bound stub for deferred name; print `[defer] ACTION-OF name=<name>` (+ optional `xt=<id>` / `action=<bound>`) |
| `defer-demo` | `( -- )` | See §5 |

Host note: bind `DEFER` / `IS` / `ACTION-OF` on the Linux REPL **only if** those names do not collide with rekia platform hooks in the same load path. Prefer Forth mirrors **`defer-create` / `is-bind` / `action-of-xt`** as the Lab-facing surface when in doubt — **do not redefine** rekia `defer` / `is`. Optional stub xt id is a host int / name string only — not a callable XT.

## 3. Stub semantics

- **DEFER name:** `entry-create` (or host mirror) named deferred entry with unbound stub action (empty / `action=` unset / `xt=0`). Print `[defer] DEFER name=<name>`. Dict presence greppable via `WORDS` / `entry-find` / find hit. No XT vector table.
- **IS name:** resolve an existing deferred entry by name; bind a stub action (literal name string, optional stub xt id, or prior CREATE/colon name). Print `[defer] IS name=<name>` (+ optional `xt=<id>` / `action=<bound>`). **Does not** compile, link, or execute an XT. Re-bind overwrites the stub binding (last wins).
- **ACTION-OF name:** look up deferred entry; print `[defer] ACTION-OF name=<name>` (+ optional `xt=<id>` / `action=<bound>` matching the last `IS`). Optional push of stub xt id if host stack is easy — Lab greps the marker.
- **Missing name / unbound required path:** `[defer] FAIL reason=miss` (demo **must avoid** — always DEFER before IS/ACTION-OF on that name). Optional unbound ACTION-OF → `action=` empty / `xt=0` is OK if documented; prefer demo binds first.
- Storage: name string + optional stub action/name/id on the entry record. **No** XT vector table, no executing bound XT, no MARKER restore, no BUFFER: arena.
- Nest with prior imm / fill / pick / cell / allot / throw / 2var / create / colon / control / string stubs OK. `dict-reset` clears deferred entries as usual.
- Still no linked XT / executing postponed XT, no real DOES> XT, no real branch XT, no full Win/Android Forth VM. MARKER / BUFFER: / EXIT-QUIT landed wave16 **2–4** (keep cites). SYNONYM/ALIAS name-map → `docs/SYNONYM-ALIAS.md` (wave17 **1**; not a DEFER vector). Those other items stay non-goals / later tips.

## 4. Markers

```
[defer] DEFER name=<name>
[defer] IS name=<name> [xt=<id>] [action=<bound>]   # xt=/action= optional
[defer] ACTION-OF name=<name> [xt=<id>] [action=<bound>]
[defer] FAIL reason=miss                            # demo avoids
[defer-demo] OK
[defer-demo] FAIL
```

Lab greps `[defer-demo] OK` plus at least one `[defer] DEFER name=`, one `[defer] IS name=`, and one `[defer] ACTION-OF name=`. Optional `xt=` / `action=` fields are not required for Lab OK when present on IS/ACTION-OF.

## 5. `defer-demo`

1. Clean slate / `dict-reset` (or cold path).
2. `DEFER` a known name (e.g. `widget`) → `[defer] DEFER name=widget`; find/WORDS shows `widget`.
3. `IS` that same name (optionally with a stub xt id / action name) → `[defer] IS name=widget` (+ optional `xt=` / `action=`).
4. `ACTION-OF` that same name → `[defer] ACTION-OF name=widget` (+ optional matching `xt=` / `action=`).
5. Assert no `[defer] FAIL reason=miss` on the happy path. Assert IS/ACTION-OF did **not** require executing a bound XT (marker-only is enough).
6. Prior `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` still OK.
7. `[defer-demo] OK`.

DEFER + IS + ACTION-OF markers are required. Miss FAIL path is not exercised by the demo. Optional `xt=` / `action=` echo is not required for Lab OK.

## 6. Thin amend — companions

### `docs/CREATE-DOES.md`

- Companions: add `DEFER-IS.md` (wave16 **1**); **keep** COLON / VARIABLE-CONST / KERNEL / VALUE-TO / CASE-OF / ALLOT-HERE cites.
- Purpose / §3: deferred-word stubs (`DEFER` / `IS` / `ACTION-OF`) point at this tip — name + bind markers only; **not** a real XT vector / child body. CREATE/DOES> text stays; do not wipe wave13 content.
- Non-goals: `DEFER` / `IS` / `ACTION-OF` → `docs/DEFER-IS.md` (wave16 **1**). Still no real DOES> XT chaining.
- Acceptance: Lab smokes `defer-demo` (retains `create-demo`).
- Cite: `docs/DEFER-IS.md`.

### `docs/VARIABLE-CONST.md`

- Companions: add `DEFER-IS.md` (wave16 **1**); **keep** KERNEL / COLON / WORDS-VOCAB / VALUE-TO / CREATE-DOES / ALLOT-HERE / 2VARIABLE / CELL-CELLS cites.
- Purpose / §3: named cells stay host ints; deferred-word stubs are this tip (name + bind markers), not a VARIABLE body. Do not wipe wave12–15 content.
- Non-goals: `DEFER` / `IS` / `ACTION-OF` → `docs/DEFER-IS.md` (wave16 **1**).
- Cite: `docs/DEFER-IS.md`.

### `docs/KERNEL.md`

- Companions: add `DEFER-IS.md` (wave16 **1**); **keep** IMMEDIATE-POSTPONE / FILL-MOVE / PICK-ROLL / CELL-CELLS and prior cites.
- Words table: add `DEFER` / `IS` / `ACTION-OF` stubs + `defer-demo` (cite tip; Forth mirrors `defer-create` / `is-bind` / `action-of-xt` — **do not** redefine rekia `defer` / `is`).
- Non-goals: DEFER/IS/ACTION-OF stub mirrors → `docs/DEFER-IS.md`. IMMEDIATE/POSTPONE / buffer / stack / unit stubs stay on wave15 docs. Linked XT / executing bound XT still later.
- Acceptance: Lab smokes `defer-demo` (and retains `imm-demo` + prior demos).
- Cite: `docs/DEFER-IS.md`.

Do **not** wipe tip wave15 / CREATE / VARIABLE prior content. Do **not** amend `WORDS-VOCAB.md` or `IMMEDIATE-POSTPONE.md` this tip (proposal amends are CREATE-DOES + VARIABLE-CONST + KERNEL only).

## 7. Non-goals

- Redefining rekia live `defer` / `is` platform hooks (mirrors only)
- Linked XT / XT vector table / executing a bound XT
- `SYNONYM` / `ALIAS` name-map stubs → `docs/SYNONYM-ALIAS.md` (wave17 **1**; name→name only — not DEFER vector / linked XT)
- `MARKER` dictionary restore (wave16 **2** — already stubbed; keep cites)
- `BUFFER:` named allot buffer (wave16 **3** — already stubbed; keep cites)
- `EXIT` / `QUIT` thin control markers (wave16 **4** — already stubbed; keep cites)
- Docs cites pass (wave16 **5** CLOSED; wave17 **5** after 1–4 PASS)
- `PARSE` / `PARSE-NAME` deepen (wave17 **2** candidate)
- IMMEDIATE / POSTPONE (wave15 **4** — already stubbed; do not reopen)
- FILL / ERASE / MOVE / CMOVE (wave15 **3** — already stubbed)
- PICK / ROLL / DEPTH / ?DUP (wave15 **2** — already stubbed)
- CELL / CELLS / ALIGN / ALIGNED (wave15 **1** — already stubbed)
- Real DOES> XT chaining / threaded child runtime body
- Real branch XT / LEAVE jump
- Full Dusk arena / pool / free / fragmentation model
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live)
- `ABORT"` polish (already optional-wired inside `throw-demo`)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/DEFER-IS.md` present (Research byte-copy OK); `CREATE-DOES.md` + `VARIABLE-CONST.md` + `KERNEL.md` thin amends present (wave13/12/7 text and wave15 cites retained).
2. `defer-demo` → OK (markers §4; DEFER + IS + ACTION-OF greppable; no miss FAIL on happy path). Prior `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` still OK.
3. Regression green (wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `defer-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Rekia `defer` / `is` untouched.

## 9. Cite

- `docs/CREATE-DOES.md` (wave13 **3**), `docs/VARIABLE-CONST.md` (wave12 **2**), `docs/KERNEL.md` (wave7 **5**)
- `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**), `docs/FILL-MOVE.md` (wave15 **3**), `docs/PICK-ROLL.md` (wave15 **2**), `docs/CELL-CELLS.md` (wave15 **1**)
- `forth/tritium/kernel.fs`; rekia live `defer` / `is` (platform hooks — **do not redefine**)
- ANS Forth `DEFER` / `IS` / `ACTION-OF` (name + bind mark only — no XT vector / execute)
- Explicit deferral: WAVE15-PROPOSAL (`DEFER` / `IS` / `ACTION-OF` — rekia binds real hooks; this tip takes stub mirrors only)
- Base tip: `c46cd68` / `c46cd68de93e9e18e6250bacf1f64a81648ead64` (#71 wave15 tip5 docs cites)
- `docs/SYNONYM-ALIAS.md` (wave17 **1**)
- `docs/MARKER.md` (wave16 **2**), `docs/BUFFER-COLON.md` (wave16 **3**), `docs/EXIT-QUIT.md` (wave16 **4**)
- Wave16 proposal: `/workspace/tritium-research-docs/WAVE16-PROPOSAL.md`
- Wave17 proposal: `/workspace/tritium-research-docs/WAVE17-PROPOSAL.md`
