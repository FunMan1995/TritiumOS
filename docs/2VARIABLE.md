# 2VARIABLE — `2VARIABLE` / `2CONSTANT` double-cell stubs + `2var-demo`

**Status:** Shipper-ready stub spec (wave14 item **3**)
**Canonical brief:** ANS-shaped `2VARIABLE` / `2CONSTANT` (thin stub); `docs/VARIABLE-CONST.md` (wave12 **2**), `docs/VALUE-TO.md` (wave13 **1**), `docs/KERNEL.md` (wave7 **5**)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `var.fs` / `2var.fs`); Linux host REPL
**Companions:** `docs/VARIABLE-CONST.md` (thin amend this tip), `docs/VALUE-TO.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip), `docs/ALLOT-HERE.md` (wave14 **1**)
**Base tip SHA:** `71c8986` (wave14 tip2 CLOSED / #63 UNLOOP-J) / full `71c8986b31d30e06c8cce440e920bd82f69653ab`

## 1. Purpose

`VARIABLE` / `CONSTANT` (wave12 **2**) and `VALUE` / `TO` (wave13 **1**) named single-cell stubs exist. This tip **deepens** the cell family with **2VARIABLE / 2CONSTANT stubs**: `2VARIABLE <name>` creates a named double-cell (or two-slot) entry (init 0,0), `2CONSTANT <name>` creates a named double-constant from a two-cell stub value, prints greppable `[2var]` markers, and smokes via **`2var-demo`**. Optional `2@` / `2!` markers only if cheap. Same host cell-map pattern as VARIABLE/VALUE — **not** a real double-cell heap or ALLOT of 2 cells beyond tip1 pointer stubs. Builds on wave12 VARIABLE-CONST / wave13 VALUE-TO; optional use of tip1 HERE/ALLOT markers if Shipper wires a 2-cell bump — not required for Lab OK.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `2VARIABLE` / `2var-create` | `( "name" -- )` | Create named double-cell stub; init **0 0**; print `[2var] 2VARIABLE name=<name>` |
| `2CONSTANT` / `2const-create` | `( d "name" -- )` *or* `( n1 n2 "name" -- )` | Create named double-constant stub; print `[2var] 2CONSTANT name=<name> lo=<a> hi=<b>` (or `value=` pair) |
| `2@` / `2var-fetch` (optional) | `( "name" -- )` *or* `( i -- d )` | If wired: fetch double stub; print `[2var] 2@ name=<name> lo=<a> hi=<b>`; else skip |
| `2!` / `2var-store` (optional) | `( d "name" -- )` *or* `( n1 n2 "name" -- )` | If wired: store into 2VARIABLE stub; print `[2var] 2! name=<name> lo=<a> hi=<b>`; else skip |
| `2var-find` | `( c-addr u -- i )` | Optional; thin wrapper of `entry-find` for 2var/2const entries |
| `2var-demo` | `( -- )` | See §5 |

Host note: bind `2VARIABLE` / `2CONSTANT` on Linux REPL; Forth mirrors `2var-create` / `2const-create` if host Forth names collide (host Forth often has real `2VARIABLE`/`2CONSTANT`). Optional `2@`/`2!` only when host already has cell ops — **do not** invent a real double-cell heap / ALLOT-of-2-cells arena this tip (pointer stubs → `docs/ALLOT-HERE.md`).

## 3. Stub semantics

- **2VARIABLE** → `entry-create` (or host mirror) named double-cell / two-slot entry; stash stub pair = **0, 0**; dict presence greppable via `WORDS` / `entry-find` / find hit. Same host map pattern as VARIABLE — two ints (or one tagged double slot) is enough.
- **2CONSTANT** → create named entry holding immutable stub pair `(lo, hi)` / `(n1, n2)` from TOS (or demo-supplied args); on exec may print/push the pair stub — no requirement to push real stack doubles if markers alone are greppable.
- Init: 2VARIABLE always starts at 0,0; 2CONSTANT takes the pair at create time.
- Optional `2@`/`2!`: only if already present / cheap on host; print `[2var] 2@` / `[2var] 2!` markers. No requirement to ALLOT two cells via tip1 pointer — host map is enough. Optional tip1 bump (e.g. `ALLOT` 2 cells) is fine if Shipper wires it; **not** required for Lab OK.
- Unrecognized / missing name on optional fetch → `[2var] FAIL` reason=miss (demo must avoid).
- Nest with prior VARIABLE / VALUE / CONSTANT / colon / control / loop / allot stubs OK; `dict-reset` clears 2var/2const stubs along with var/value.
- Still no FLOAT, THROW/CATCH, real DOES> XT, full Win/Android Forth VM — those → later tips / non-goals.

## 4. Markers

```
[2var] 2VARIABLE name=<name>
[2var] 2CONSTANT name=<name> lo=<a> hi=<b>     # or value= pair form
[2var] 2@ name=<name> lo=<a> hi=<b>            # optional
[2var] 2! name=<name> lo=<a> hi=<b>            # optional
[2var] FAIL reason=<…>
[2var-demo] OK
[2var-demo] FAIL
```

Lab greps `[2var-demo] OK` plus at least one `[2var] 2VARIABLE name=` and one `[2var] 2CONSTANT name=`; dict presence via `WORDS` / find hit / entry-count greppable (N ≥ prior + 2 after creates). Optional `2@`/`2!` not required for Lab OK.

## 5. `2var-demo`

1. Clean slate / `dict-reset` if available.
2. `2VARIABLE dv` (or fixture name) → `[2var] 2VARIABLE name=dv`; find/WORDS shows `dv`.
3. `1 2 2CONSTANT dc` (or demo-supplied pair then `2CONSTANT dc`) → `[2var] 2CONSTANT name=dc lo=1 hi=2` (or equivalent value= pair); find/WORDS shows `dc`.
4. Optional: `2@` / `2!` on `dv` if wired → markers; else skip.
5. Assert dict presence greppable (entry-count / WORDS / find hits both new names).
6. Prior `unloop-demo` / `allot-demo` / `leave-demo` / `do-loop-demo` / `loop-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `create-demo` / `string-demo` / `colon-demo` still OK.
7. `[2var-demo] OK`.

2VARIABLE + 2CONSTANT create markers and dict presence are required. Optional `2@`/`2!` and tip1 2-cell ALLOT bump are not required for Lab OK.

## 6. Thin amend — companions

### `docs/VARIABLE-CONST.md`

- Companions: add `2VARIABLE.md` (wave14 **3**).
- Purpose / §3: point double-cell named entries (`2VARIABLE` / `2CONSTANT`) at this tip (same host cell-map pattern; two-slot / double stub). Still no FLOAT / full double heap.
- Non-goals: `2VARIABLE` / `2CONSTANT` → `docs/2VARIABLE.md` (wave14 **3**); keep FLOAT / real double-cell heap / THROW out.
- Cite: `docs/2VARIABLE.md`.

### `docs/VALUE-TO.md`

- Companions: add `2VARIABLE.md` (wave14 **3**).
- Purpose / §3: cell-family deepen continues with double-cell stubs → this tip (VALUE remains single mutable cell; 2VARIABLE is two-slot sibling).
- Non-goals: `2VARIABLE` / `2CONSTANT` → `docs/2VARIABLE.md` (wave14 **3**); keep FLOAT / THROW / real double heap out.
- Cite: `docs/2VARIABLE.md`.

### `docs/KERNEL.md`

- Companions: add `2VARIABLE.md` (wave14 **3**).
- Words table: add `2VARIABLE` / `2CONSTANT` stubs + `2var-demo` (cite tip; host two-slot map — no full double heap).
- Non-goals: point double-cell stubs to `docs/2VARIABLE.md`. Full arena / THROW / FLOAT still later.
- Acceptance: Lab smokes `2var-demo`.
- Cite: `docs/2VARIABLE.md`.

## 7. Non-goals

- Real double-cell heap / ALLOT of 2 cells beyond tip1 pointer stub (`docs/ALLOT-HERE.md` — bump optional, not required)
- FLOAT / floating-word surface
- THROW / CATCH / ABORT" (wave14 **4** candidate)
- Real DOES> XT chaining / threaded child runtime body
- Docs cites pass (wave14 **5**)
- Real crypto / network fleet
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/2VARIABLE.md` present (Research byte-copy OK); `VARIABLE-CONST.md` + `VALUE-TO.md` + `KERNEL.md` thin amends present.
2. `2var-demo` → OK (markers §4); prior `unloop-demo` + `allot-demo` + `leave-demo` + `do-loop-demo` + `loop-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `create-demo` + `string-demo` + `colon-demo` still OK.
3. Regression green (wave14 **1–2** + wave13 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `2var-demo CONTRACT` OK).
5. No merge. Stay out of Mango.

## 9. Cite

- `docs/VARIABLE-CONST.md` (wave12 **2**), `docs/VALUE-TO.md` (wave13 **1**), `docs/KERNEL.md` (wave7 **5**), `docs/ALLOT-HERE.md` (wave14 **1**), `docs/UNLOOP-J.md` (wave14 **2**)
- `forth/tritium/kernel.fs`
- ANS Forth `2VARIABLE` / `2CONSTANT` (stub only); optional `2@` / `2!` (stub only); Dusk named-cell surface (double stub only)
- Base tip: `71c8986` / `71c8986b31d30e06c8cce440e920bd82f69653ab`
- Wave14 proposal: `/workspace/tritium-research-docs/WAVE14-PROPOSAL.md`
