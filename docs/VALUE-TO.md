# VALUE-TO — `VALUE` / `TO` named mutable cells + `value-demo`

**Status:** Shipper-ready stub spec (wave13 item **1**)
**Canonical brief:** ANS-shaped `VALUE` / `TO` (thin stub); `docs/VARIABLE-CONST.md` (wave12 **2**), `docs/KERNEL.md` (wave7 **5**)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `var.fs` / `value.fs`); Linux host REPL
**Companions:** `docs/VARIABLE-CONST.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip)
**Base tip SHA:** `8df5974` (wave12 tip5 CLOSED / #56) / full `8df59749768792e97babc98f14e68851a3705c69`

## 1. Purpose

`VARIABLE` / `CONSTANT` named-cell stubs exist (wave12 **2**). This tip **thin-deepens** that surface with **VALUE / TO stubs**: `n VALUE <name>` creates a named mutable cell initialized to `n`, `TO <name>` stores TOS into that cell, prints greppable `[value]` markers, and smokes via **`value-demo`**. Optional `value@` fetch. Not real ALLOT/HERE, DOES>/CREATE, CASE, string lit, or branch XT.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `VALUE` / `value-create` | `( n "name" -- )` *or* `( n -- )` then parse name | Create named mutable cell; init = `n`; print `[value] VALUE name=<name> value=<n>` |
| `TO` / `value-to` | `( n "name" -- )` *or* `( n -- )` then parse name | Store `n` into existing VALUE stub; print `[value] TO name=<name> value=<n>` |
| `value@` (optional) | `( "name" -- )` *or* `( -- n )` after name | If wired: fetch VALUE stub; print `[value] @ name=<name> value=<n>`; else skip — create + TO markers are enough |
| `value-find` | `( c-addr u -- i )` | Optional; thin wrapper of `entry-find` for VALUE entries |
| `value-demo` | `( -- )` | See §5 |

Host note: bind `VALUE` / `TO` on Linux REPL; Forth mirrors `value-create` / `value-to` if host Forth names collide (host Forth often has real `VALUE`/`TO`). Optional `value@` only when host already has cell ops — **do not** invent a HERE/ALLOT arena this tip.

## 3. Stub semantics

- **VALUE** → `entry-create` (or host mirror) named mutable cell; stash stub cell = `n` at create; dict presence greppable via `WORDS` / `entry-find` / find hit. Unlike VARIABLE (always init 0), VALUE takes the init from TOS (or demo-supplied arg). Unlike CONSTANT, the cell stays mutable via `TO`.
- **TO** → store `n` into an existing VALUE named cell; print `[value] TO name=<name> value=<n>`. Missing / non-VALUE name → `[value] FAIL` reason=miss (demo must avoid).
- Optional `value@`: fetch current stub; print `[value] @ name=<name> value=<n>`. No requirement to push a real stack cell if the marker alone is greppable.
- Storage: host-side int map or entry-slot field is enough — **no** ALLOT/HERE arena, no linked cell memory, no DOES>/CREATE defining-word body.
- Nest with prior VARIABLE/CONSTANT / colon / control / loop stubs OK; `dict-reset` clears VALUE stubs along with var/const.
- Still no CASE, string lit, real branch XT, DOES>/CREATE; those → later wave13 tips.

## 4. Markers

```
[value] VALUE name=<name> value=<n>
[value] TO name=<name> value=<n>
[value] @ name=<name> value=<n>     # optional
[value] FAIL reason=<…>
[value-demo] OK
[value-demo] FAIL
```

Lab greps `[value-demo] OK` plus at least one `[value] VALUE name=` with `value=` and one `[value] TO name=` with `value=`; dict presence via `WORDS` / find hit / entry-count greppable (N ≥ prior + 1 after VALUE create).

## 5. `value-demo`

1. Clean slate / `dict-reset` if available.
2. `7 VALUE baz` (or `7` then `VALUE baz`) → `[value] VALUE name=baz value=7`; find/WORDS shows `baz`.
3. `99 TO baz` (or `99` then `TO baz`) → `[value] TO name=baz value=99`.
4. Optional: `value@` on `baz` → `[value] @ name=baz value=99`; else skip.
5. Assert dict presence greppable (find hit / WORDS / entry-count for `baz`).
6. Prior `var-demo` / `leave-demo` / `comment-demo` / `do-loop-demo` still OK.
7. `[value-demo] OK`.

## 6. Thin amend — companions

### `docs/VARIABLE-CONST.md`

- Companions: add `VALUE-TO.md` (wave13 **1**).
- Purpose / §3: strike open “Still no VALUE/TO”; point VALUE/TO stubs at this tip (VALUE ≈ mutable named cell; `TO <name>` stores). Still no DOES>/CREATE.
- Non-goals: VALUE / TO → `docs/VALUE-TO.md` (wave13 **1**); keep DOES> / CREATE-DOES> / real ALLOT/HERE out.
- Cite: `docs/VALUE-TO.md`.

### `docs/KERNEL.md`

- Companions: add `VALUE-TO.md` (wave13 **1**).
- Words table: add `VALUE` / `TO` stubs + `value-demo` (cite tip; no full memory model).
- Non-goals: strike open VALUE/TO from “still later”; point to `docs/VALUE-TO.md`. Real ALLOT/HERE / DOES> still later.
- Acceptance: Lab smokes `value-demo`.
- Cite: `docs/VALUE-TO.md`.

## 7. Non-goals

- Real ALLOT / HERE arena / linked cell memory model
- DOES> / CREATE / CREATE-DOES> (wave13 **3** candidate)
- CASE / OF / ENDOF / ENDCASE (wave13 **2** candidate)
- String literals `S"` / `."` (wave13 **4** candidate)
- Real branch XT / runtime counted re-exec / LEAVE jump
- Docs cites pass (wave13 **5**)
- Real crypto / network fleet
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/VALUE-TO.md` present (Research byte-copy OK); `VARIABLE-CONST.md` + `KERNEL.md` thin amends present.
2. `value-demo` → OK (markers §4); `var-demo` + `leave-demo` + `comment-demo` + `do-loop-demo` still OK.
3. Regression green (wave12 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `value-demo CONTRACT` OK).
5. No merge. Stay out of Mango.

## 9. Cite

- `docs/VARIABLE-CONST.md` (wave12 **2**), `docs/KERNEL.md` (wave7 **5**), `docs/WORDS-VOCAB.md`, `docs/COLON.md`
- `forth/tritium/kernel.fs`
- ANS Forth `VALUE` / `TO` (stub only); Dusk named-cell surface (stub only)
- Base tip: `8df5974` / `8df59749768792e97babc98f14e68851a3705c69`
- Wave13 proposal: `/workspace/tritium-research-docs/WAVE13-PROPOSAL.md`

