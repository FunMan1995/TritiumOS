# PICK-ROLL — `PICK` / `ROLL` / `DEPTH` / `?DUP` stack stubs + `pick-demo`

**Status:** Shipper-ready stub spec (wave15 item **2**; thin amend wave20 **1** TRUE-FALSE companion cite; thin amend wave20 **2** WITHIN companion cite; thin amend wave21 **4** BITWISE companion cite)
**Canonical brief:** ANS-shaped `PICK` / `ROLL` / `DEPTH` / `?DUP` (thin stack markers); `docs/KERNEL.md` (wave7 **5**); pairs with wave15 **1** `CELL-CELLS` (independent)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `pick.fs`); Linux host REPL
**Companions:** `docs/KERNEL.md` (thin amend this tip only); `docs/TRUE-FALSE.md` (wave20 **1** — constant-mark companion cite; sibling flag picture beside ?DUP — not PICK/?DUP reopen); `docs/WITHIN.md` (wave20 **2** — range-check mark companion cite; sibling flag picture — not PICK/?DUP reopen / compare machine); `docs/BITWISE.md` (wave21 **4** — bitwise marks companion cite; sibling bitwise marks — not PICK/?DUP reopen / boolean cell / `0=` deepen)
**Base tip SHA:** `2e949e7` (wave15 tip1 CLOSED / #67 CELL-CELLS) / full `2e949e7392f911195e1aa8888437f12e28135ac9`

## 1. Purpose

Core stack words never got a `[*demo]` surface. Host `2dup` / `2drop` / `2swap` already live inside `kernel.fs`; this tip does **not** redefine them. It adds **stub** markers only: `DEPTH` reports a host stack count, `PICK` copies the u-th item (tiny host array / TOS window picture), `ROLL` rotates that picture, `?DUP` duplicates TOS when nonzero, and smokes via **`pick-demo`**. Marker + tiny host stack picture only — **not** a real threaded data stack, not a return-stack walk, and not a buffer FILL. Independent of tip1 CELL-CELLS. Forth mirrors `pick-nth` / `roll-nth` / `stack-depth` / `qdup` so existing `2 pick` in `drena.fs` is **not** replaced. Wave20 tip **1** lands `TRUE` / `FALSE` constant marks (`docs/TRUE-FALSE.md`): sibling flag picture beside `?DUP flag=` — **not** a PICK/ROLL/?DUP reopen / real threaded stack / `0=` deepen / WITHIN. Wave20 tip **2** lands `WITHIN` range-check mark (`docs/WITHIN.md`): sibling range-check flag mark — **not** a PICK/ROLL/?DUP reopen / real threaded stack / compare machine / branch XT. Wave21 tip **4** lands `AND` / `OR` / `XOR` / `INVERT` bitwise marks (`docs/BITWISE.md`): sibling **bitwise** marks — **not** a PICK/ROLL/?DUP reopen / real threaded stack / boolean cell / `0=` deepen / WITHIN reopen; host lowercase `and`/`or` stay untouched.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `DEPTH` / `stack-depth` | `( -- n )` *or* `( -- )` | Host stack depth; print `[pick] DEPTH n=<n>` |
| `PICK` / `pick-nth` | `( xu … x0 u -- xu … x0 xu )` *or* stub | Copy the u-th item (0 = TOS); print `[pick] PICK u=<u> x=<x>`; tiny host stack picture OK |
| `ROLL` / `roll-nth` | `( xu … x0 u -- xu-1 … x0 xu )` *or* stub | Rotate u items; print `[pick] ROLL u=<u>`; demo may use a fixed picture |
| `?DUP` / `qdup` | `( x -- 0 \| x x )` *or* stub | If TOS ≠ 0, duplicate; print `[pick] ?DUP flag=<0\|1>` (`1` when duplicated, `0` when TOS was 0) |
| `pick-demo` | `( -- )` | See §5 |

Host note: bind `PICK` / `ROLL` / `DEPTH` / `?DUP` on the Linux REPL; Forth mirrors **`pick-nth` / `roll-nth` / `stack-depth` / `qdup`** so host Forth / in-tree `2 pick` (e.g. `drena.fs`) is not replaced. Do **not** redefine host `2dup` / `2drop` / `2swap` already used inside `kernel.fs`.

## 3. Stub semantics

- **Host picture:** a small host array (or printed TOS window) is enough. Depth is the length of that picture. No real threaded stack, no return-stack walk, no dictionary image.
- **DEPTH:** print `[pick] DEPTH n=<n>` with `n` = current picture length (demo builds a known depth ≥ 3 before PICK/ROLL).
- **PICK u:** `u=0` copies TOS; `u` indexes down the picture. Print `[pick] PICK u=<u> x=<x>` with the copied value. Copy only — stack depth after a real ANS PICK is +0 relative to consuming `u` (picture may show the copy). Lab greps `u=` and `x=`.
- **ROLL u:** rotate so the u-th item becomes TOS (ANS-shaped). Print `[pick] ROLL u=<u>`. Demo may use a **fixed** before/after picture; Lab greps `u=` (optional post-picture is fine).
- **?DUP:** TOS = 0 → `flag=0` (no duplicate); TOS ≠ 0 → `flag=1` (duplicate). Print `[pick] ?DUP flag=<0|1>`. Demo must show **both** paths (zero and nonzero).
- **Underflow / u past depth:** `[pick] FAIL reason=underflow` (demo **must avoid** — always seed enough items and keep `u` in range).
- Nest with prior cell / allot / throw / 2var / create / colon / control / string stubs OK. `dict-reset` need not clear the host picture (demo seeds explicitly).
- Still no FILL/MOVE buffer, IMMEDIATE/POSTPONE, real DOES> XT, real branch XT, full Win/Android Forth VM. TRUE/FALSE constant marks → `docs/TRUE-FALSE.md` (wave20 **1** — sibling flag picture; not a PICK/?DUP reopen / boolean cell rewrite). WITHIN range-check mark → `docs/WITHIN.md` (wave20 **2** — sibling range-check flag; not a PICK/?DUP reopen / compare machine / branch XT). AND/OR/XOR/INVERT bitwise marks → `docs/BITWISE.md` (wave21 **4** — sibling bitwise marks; not a PICK/?DUP reopen / boolean cell / `0=` deepen; host lowercase `and`/`or` stay untouched). Those stay non-goals / later tips.

## 4. Markers

```
[pick] DEPTH n=<n>
[pick] PICK u=<u> x=<x>
[pick] ROLL u=<u>
[pick] ?DUP flag=<0|1>
[pick] FAIL reason=underflow          # demo avoids
[pick-demo] OK
[pick-demo] FAIL
```

Lab greps `[pick-demo] OK` plus `[pick] DEPTH n=`, one `[pick] PICK u=` with `x=`, one `[pick] ROLL u=`, and both `?DUP` flags (`flag=0` and `flag=1`) preferred — at minimum one `?DUP flag=` plus DEPTH/PICK/ROLL.

## 5. `pick-demo`

1. Clean slate / seed a tiny host picture with known values (e.g. three items so depth ≥ 3).
2. `DEPTH` → `[pick] DEPTH n=<n>` with `n` matching the seeded picture (≥ 3).
3. `PICK` with a legal `u` (e.g. `1` or `2`) → `[pick] PICK u=<u> x=<x>` (copied value greppable).
4. `ROLL` with a legal `u` (e.g. `2`) → `[pick] ROLL u=<u>` (fixed picture OK).
5. `?DUP` on nonzero TOS → `[pick] ?DUP flag=1`; then on zero → `[pick] ?DUP flag=0` (both paths preferred).
6. Assert no `[pick] FAIL reason=underflow` on the happy path.
7. Prior `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` still OK.
8. `[pick-demo] OK`.

DEPTH + PICK + ROLL markers and at least one `?DUP flag=` are required. Underflow FAIL path is not exercised by the demo.

## 6. Thin amend — companions

### `docs/KERNEL.md`

- Companions: add `PICK-ROLL.md` (wave15 **2**); **keep** `CELL-CELLS.md` (wave15 **1**) and prior cites.
- Words table: add `PICK` / `ROLL` / `DEPTH` / `?DUP` stubs + `pick-demo` (cite tip; host picture markers only — do not redefine `2dup`/`2drop`/`2swap`; Forth mirrors `pick-nth` / `roll-nth` / `stack-depth` / `qdup`).
- Non-goals: stack-marker stubs → `docs/PICK-ROLL.md`. Dictionary-unit stubs stay → `docs/CELL-CELLS.md`. Full arena / FILL / IMMEDIATE / real XT still later.
- Acceptance: Lab smokes `pick-demo` (and retains `cell-demo`).
- Cite: `docs/PICK-ROLL.md`.

### `docs/TRUE-FALSE.md` (wave20 **1** companion — cited from this tip)

- Companions: PICK-ROLL cites TRUE-FALSE as sibling flag picture; TRUE-FALSE cites PICK-ROLL as stack/?DUP flag companion.
- Purpose: `TRUE` / `FALSE` constant marks pair with `?DUP flag=` picture — **not** a PICK/ROLL/?DUP reopen / real threaded stack / `0=` deepen / WITHIN.
- Cite: `docs/TRUE-FALSE.md`.

### `docs/WITHIN.md` (wave20 **2** companion — cited from this tip)

- Companions: PICK-ROLL cites WITHIN as sibling range-check flag picture; WITHIN optionally cites PICK-ROLL.
- Purpose: `WITHIN` optional `flag=<0|1>` pairs with `?DUP flag=` / TRUE/FALSE pictures — **not** a PICK/ROLL/?DUP reopen / real threaded stack / compare machine / branch XT.
- Cite: `docs/WITHIN.md`.

### `docs/BITWISE.md` (wave21 **4** companion — cited from this tip)

- Companions: PICK-ROLL cites BITWISE as sibling bitwise marks; BITWISE optionally cites PICK-ROLL.
- Purpose: `AND` / `OR` / `XOR` / `INVERT` bitwise marks pair with `?DUP flag=` / TRUE/FALSE pictures — **not** a PICK/ROLL/?DUP reopen / real threaded stack / boolean cell / `0=` deepen / WITHIN reopen. Host lowercase `and`/`or` stay untouched.
- Cite: `docs/BITWISE.md`.

Do **not** wipe CELL-CELLS / ALLOT-HERE / THROW-CATCH / other wave14–15 content. Do **not** wipe wave20 TRUE-FALSE / WITHIN companion cites once landed. Do **not** wipe wave21 BITWISE companion cite once landed.

## 7. Non-goals

- Redefining host `2dup` / `2drop` / `2swap` already used inside `kernel.fs`
- Replacing in-tree `2 pick` / host Forth `PICK` (use mirrors `pick-nth` / `roll-nth` / `stack-depth` / `qdup`)
- Real return-stack walk / threaded data-stack VM
- Buffer-backed FILL / ERASE / MOVE / CMOVE (wave15 **3** candidate)
- IMMEDIATE / POSTPONE (wave15 **4** candidate)
- Docs cites pass (wave15 **5**)
- CELL / CELLS / ALIGN / ALIGNED (wave15 **1** — already stubbed; do not reopen)
- `TRUE` / `FALSE` constant marks → `docs/TRUE-FALSE.md` (wave20 **1**; sibling flag picture — not a PICK/?DUP reopen / real boolean cell / `0=` deepen / WITHIN)
- `AND` / `OR` / `XOR` / `INVERT` bitwise marks → `docs/BITWISE.md` (wave21 **4**; sibling bitwise marks — not a PICK/?DUP reopen / real boolean cell / `0=` deepen / LSHIFT/RSHIFT; host lowercase `and`/`or` stay untouched)
- `WITHIN` range-check mark → `docs/WITHIN.md` (wave20 **2**; sibling range-check flag — not a PICK/?DUP reopen / compare machine / branch XT)
- Real DOES> XT chaining / threaded child runtime body
- Real branch XT / LEAVE jump
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/PICK-ROLL.md` present (Research byte-copy OK); `KERNEL.md` thin amend present (wave15 **1** CELL-CELLS cites and wave14 text retained); wave20 **1** TRUE-FALSE companion cite present; wave21 **4** BITWISE companion cite present.
2. `pick-demo` → OK (markers §4; DEPTH / PICK / ROLL / ?DUP greppable; no underflow FAIL on happy path). Prior `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` still OK; wave20 **1**: `true-demo` → OK (retains `pick-demo` + `cell-demo`); wave20 **2**: `within-demo` → OK (retains `pick-demo` + `true-demo`).
3. Regression green (wave15 tip1 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `pick-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**), `docs/CELL-CELLS.md` (wave15 **1**)
- `docs/TRUE-FALSE.md` (wave20 **1** — constant-mark companion; sibling flag picture beside ?DUP)
- `docs/BITWISE.md` (wave21 **4** — bitwise marks companion; sibling bitwise marks — not PICK/?DUP reopen)
- `docs/WITHIN.md` (wave20 **2** — range-check mark companion; sibling flag picture — not PICK/?DUP reopen)
- `forth/tritium/kernel.fs`, `forth/tritium/drena.fs` (`2 pick` stays — mirrors only)
- ANS Forth `PICK` / `ROLL` / `DEPTH` / `?DUP` (stub markers + host picture only)
- Base tip: `2e949e7` / `2e949e7392f911195e1aa8888437f12e28135ac9` (#67 wave15 tip1 CELL-CELLS)
- Wave15 proposal: `/workspace/tritium-research-docs/WAVE15-PROPOSAL.md`
