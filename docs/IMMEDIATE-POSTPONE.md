# IMMEDIATE-POSTPONE — `IMMEDIATE` / `POSTPONE` compile-only stubs + `imm-demo`

**Status:** Shipper-ready stub spec (wave15 item **4**; thin amend wave17 **4** RECURSE; thin amend wave18 **1** TICK; thin amend wave18 **4** STATE-COMPILE)
**Canonical brief:** ANS-shaped `IMMEDIATE` / `POSTPONE` (thin flag + name markers); `docs/COLON.md` (wave9 **4**); `docs/INTERPRET.md` (wave8 **1**); `docs/KERNEL.md` (wave7 **5**); `docs/EXIT-QUIT.md` (wave16 **4**); `docs/RECURSE.md` (wave17 **4**); `docs/TICK.md` (wave18 **1**); `docs/STATE-COMPILE.md` (wave18 **4**); explicit WAVE13 + WAVE14 deferral closed as flag stub only
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `imm.fs` / `postpone.fs`); Linux host REPL
**Companions:** `docs/COLON.md` (thin amend wave15 **4**), `docs/INTERPRET.md` (thin amend wave15 **4**), `docs/KERNEL.md` (thin amend wave15 **4**), `docs/RECURSE.md` (wave17 **4** — sibling colon-side mark), `docs/TICK.md` (wave18 **1** — sibling name→stub-xt-id mark), `docs/STATE-COMPILE.md` (wave18 **4** — sibling STATE query + COMPILE, mark)
**Base tip SHA:** `ee1d430` (wave15 tip3 CLOSED / #69 FILL-MOVE) / full `ee1d43066a24b262e70f2515313c2a64018c5115`

## 1. Purpose

WAVE13 and WAVE14 explicitly deferred `IMMEDIATE` / `POSTPONE` (and the linked XT compiler). COLON/INTERPRET still treat “immediate / compile state machine” as out of scope beyond the colon-def flag. This tip lands **compile-only stub** markers: `IMMEDIATE` sets an **immediate-bit** on the latest colon/CREATE entry (or a named entry) and prints `[imm] IMMEDIATE name=`; `POSTPONE <name>` prints `[imm] POSTPONE name=` and does **not** compile or run an XT (optional `[imm] compile-only` when the colon-def flag is set). Smoke via **`imm-demo`**. Flag + name mark only — **no** linked XT list, no FIND-then-compile, no executing a postponed XT, no full compile/interpret state machine beyond the existing colon-def flag. Closes the colon/interpret “immediate state” non-goal as a stub before tip5 cites. Forth mirrors `immediate-mark` / `postpone-mark` so host Forth names (if any) are not replaced. Tick mark (`'` / `[']`) → `docs/TICK.md` (wave18 **1**; name→stub-xt-id only — pairs with POSTPONE name-mark without promoting either to a real XT vector). STATE/COMPILE, query + compile-comma mark → `docs/STATE-COMPILE.md` (wave18 **4**; Forth mirrors `state-flag` / `compile-comma` — **not** linked XT / FIND-then-compile / POSTPONE reopen / real STATE cell).

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `IMMEDIATE` / `immediate-mark` | `( -- )` *or* `( "name" -- )` | Set immediate-bit on **latest** colon/CREATE entry (prefer) or on a named entry; print `[imm] IMMEDIATE name=<name>` |
| `POSTPONE` / `postpone-mark` | `( "name" -- )` | Parse/mark name only; print `[imm] POSTPONE name=<name>`; does **not** compile or execute an XT. Optional `[imm] compile-only` when colon-def flag is set |
| `imm-demo` | `( -- )` | See §5 |

Host note: bind `IMMEDIATE` / `POSTPONE` on the Linux REPL; Forth mirrors **`immediate-mark` / `postpone-mark`** so host Forth / in-tree names are not replaced. Prefer acting on the **latest** colon/CREATE entry for `IMMEDIATE` (matches ANS “latest definition”); a named-entry form is OK if documented and Lab-greppable via `name=`.

## 3. Stub semantics

- **Immediate-bit:** one host flag (or bit on the entry record) per dict entry is enough. Set by `IMMEDIATE`; readable later if useful — Lab greps the marker, not a bit dump.
- **IMMEDIATE:** resolve target = latest colon/CREATE entry (or named entry). Set immediate-bit. Print `[imm] IMMEDIATE name=<name>`. No XT compile, no state-machine change beyond the bit.
- **POSTPONE name:** look up / accept the name string; print `[imm] POSTPONE name=<name>`. **Do not** compile an XT, append to a body list, or execute the named word. If colon-def flag is currently set, optionally also print `[imm] compile-only` (Lab-optional — nice-to-have when cheap).
- **Missing name / no latest entry:** `[imm] FAIL reason=miss` (demo **must avoid** — always IMMEDIATE after a named colon/CREATE, and POSTPONE a known name).
- Storage: flag + name string only. **No** linked XT list, no FIND-then-compile, no postponed-XT queue, no EXIT redefinition. Real `RECURSE` self-XT stays out — wave17 **4** lands mark-only `recurse-mark` only (`docs/RECURSE.md`).
- Nest with prior fill / pick / cell / allot / throw / 2var / create / colon / control / string stubs OK. `dict-reset` clears entries (and thus bits) as usual. Wave17 `recurse-demo` nests beside `imm-demo` without breaking colon.
- Still no real compile/interpret state machine beyond colon-def flag + this bit + wave18 **4** STATE query mark, no executing postponed XT, no real RECURSE self-XT (mark-only stub → `RECURSE.md`), no XT execute via tick (mark-only → `TICK.md` wave18 **1**), no linked XT via COMPILE, (mark-only → `STATE-COMPILE.md` wave18 **4**), no real DOES> XT, no real branch XT, no full Win/Android Forth VM. Those stay non-goals / later.

## 4. Markers

```
[imm] IMMEDIATE name=<name>
[imm] POSTPONE name=<name>
[imm] compile-only                    # optional when colon-def flag set
[imm] FAIL reason=miss                # demo avoids
[imm-demo] OK
[imm-demo] FAIL
```

Lab greps `[imm-demo] OK` plus at least one `[imm] IMMEDIATE name=` and one `[imm] POSTPONE name=`. Optional `[imm] compile-only` is not required for Lab OK.

## 5. `imm-demo`

1. Clean slate / `dict-reset` (or cold path).
2. Create a named entry via `:` … `;` **or** `CREATE` (colon preferred) → known `name`.
3. `IMMEDIATE` (latest, or named) → `[imm] IMMEDIATE name=<name>`.
4. `POSTPONE` that same (or another known) name → `[imm] POSTPONE name=<name>`. Optional: while colon-def flag is set, emit `[imm] compile-only` once.
5. Assert no `[imm] FAIL reason=miss` on the happy path. Assert POSTPONE did **not** require an XT compile/run (marker-only is enough).
6. Prior `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` still OK.
7. `[imm-demo] OK`.

IMMEDIATE + POSTPONE markers are required. Miss FAIL path is not exercised by the demo. Optional `compile-only` is not required for Lab OK.

## 6. Thin amend — companions

### `docs/COLON.md`

- Companions: add `IMMEDIATE-POSTPONE.md` (wave15 **4**); **keep** INTERPRET / KERNEL / CONTROL / VARIABLE-CONST / CREATE-DOES cites.
- §7 Non-goals: strike open “Immediate vs compile state machine (beyond colon-def flag)” as wholly absent; point **flag + name mark** stubs at this tip. Still **no** linked XT list / real compile-vs-interpret machine beyond colon-def flag + immediate-bit.
- Acceptance: Lab smokes `imm-demo` (retains `colon-demo`).
- Cite: `docs/IMMEDIATE-POSTPONE.md`.

### `docs/INTERPRET.md`

- Status / Companions: add `IMMEDIATE-POSTPONE.md` (wave15 **4**); **keep** COLON / COMMENT-PARSE / STRING-LIT cites.
- §1 / §5 Non-goals: strike open “Full immediate/compile state machine beyond colon-def flag” as wholly absent; point IMMEDIATE/POSTPONE **flag stubs** at this tip. Still no linked XT compiler / executing postponed XT.
- Words table (optional one-liner): `IMMEDIATE` / `POSTPONE` stubs → `IMMEDIATE-POSTPONE.md`.
- Acceptance: Lab smokes `imm-demo`.
- Cite: `docs/IMMEDIATE-POSTPONE.md`.

### `docs/KERNEL.md`

- Companions: add `IMMEDIATE-POSTPONE.md` (wave15 **4**); **keep** CELL-CELLS / PICK-ROLL / FILL-MOVE and prior cites.
- Words table: add `IMMEDIATE` / `POSTPONE` stubs + `imm-demo` (cite tip; flag + name mark only — Forth mirrors `immediate-mark` / `postpone-mark`).
- Non-goals: IMMEDIATE/POSTPONE flag stubs → `docs/IMMEDIATE-POSTPONE.md`. Buffer / stack / unit stubs stay on tips 1–3 docs. Linked XT / real state machine / executing postponed XT still later.
- Acceptance: Lab smokes `imm-demo` (and retains `fill-demo` + `pick-demo` + `cell-demo`).
- Cite: `docs/IMMEDIATE-POSTPONE.md`.

Do **not** wipe tip1–3 / wave14 / COLON / INTERPRET prior content.

## 7. Non-goals

- Linked XT compiler / FIND-then-compile / XT body append
- Real compile/interpret state machine beyond colon-def flag + immediate-bit
- Executing a postponed XT / postponed-XT queue / runtime compile semantics
- `RECURSE` real self-XT — mark-only stub → `docs/RECURSE.md` (wave17 **4**; Forth mirror `recurse-mark`; still no self-XT)
- `'` / `[']` tick mark → `docs/TICK.md` (wave18 **1**; name→stub-xt-id mark only — not XT execute / FIND rewrite / COMPILE,; pairs with POSTPONE name-mark)
- `STATE` / `COMPILE,` stubs → `docs/STATE-COMPILE.md` (wave18 **4**; Forth mirrors `state-flag` / `compile-comma` — query + compile-comma mark only; not linked XT / FIND-then-compile / POSTPONE reopen / real STATE cell)
- Redefining host `EXIT` / `QUIT` — thin control markers → `docs/EXIT-QUIT.md` (wave16 **4**; mirrors only)
- Docs cites pass (wave15 **5** CLOSED; wave17 **5** CLOSED; wave18 **5** after wave18 1–4 PASS)
- CELL / CELLS / ALIGN / ALIGNED (wave15 **1** — already stubbed; do not reopen)
- PICK / ROLL / DEPTH / ?DUP (wave15 **2** — already stubbed; do not reopen)
- FILL / ERASE / MOVE / CMOVE (wave15 **3** — already stubbed; do not reopen)
- Real DOES> XT chaining / threaded child runtime body
- Real branch XT / LEAVE jump
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/IMMEDIATE-POSTPONE.md` present (Research byte-copy OK); `COLON.md` + `INTERPRET.md` + `KERNEL.md` thin amends present (wave15 **1–3** cites and wave9/8/7 text retained).
2. `imm-demo` → OK (markers §4; IMMEDIATE + POSTPONE greppable; no miss FAIL on happy path). Prior `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` still OK; wave17 **4**: `recurse-demo` → OK (colon remains green); wave18 **1**: `tick-demo` → OK (tick pairs with POSTPONE name-mark; not XT); wave18 **4**: `state-demo` → OK (STATE/COMPILE, mark beside IMMEDIATE; not linked XT).
3. Regression green (wave15 tip1–3 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `imm-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML.

## 9. Cite

- `docs/COLON.md` (wave9 **4**), `docs/INTERPRET.md` (wave8 **1**), `docs/KERNEL.md` (wave7 **5**)
- `docs/FILL-MOVE.md` (wave15 **3**), `docs/PICK-ROLL.md` (wave15 **2**), `docs/CELL-CELLS.md` (wave15 **1**)
- `docs/EXIT-QUIT.md` (wave16 **4**), `docs/RECURSE.md` (wave17 **4**)
- `forth/tritium/kernel.fs`
- ANS Forth `IMMEDIATE` / `POSTPONE` (flag + name mark only — no XT compile/run)
- Explicit deferral: WAVE13-PROPOSAL + WAVE14-PROPOSAL (`IMMEDIATE` / `POSTPONE` / linked XT); THROW-CATCH non-goal called this out as later
- Base tip: `ee1d430` / `ee1d43066a24b262e70f2515313c2a64018c5115` (#69 wave15 tip3 FILL-MOVE)
- Wave15 proposal: `/workspace/tritium-research-docs/WAVE15-PROPOSAL.md`
- `docs/TICK.md` (wave18 **1**)
- `docs/STATE-COMPILE.md` (wave18 **4**)
