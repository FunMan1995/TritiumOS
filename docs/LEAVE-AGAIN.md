# LEAVE-AGAIN — Early-exit + infinite-loop stubs (`LEAVE` / `AGAIN`)

**Status:** Shipper-ready stub spec (wave12 item **4**)
**Canonical brief:** Dusk `LEAVE` / `AGAIN` compile (thin stub); `docs/BEGIN-UNTIL.md` (wave11 **1**), `docs/DO-LOOP.md` (wave12 **1**)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `control.fs` / `loop.fs`); Linux host REPL
**Companions:** `docs/BEGIN-UNTIL.md` (thin amend this tip), `docs/DO-LOOP.md` (thin amend this tip), `docs/CONTROL.md`, `docs/COMMENT-PARSE.md` (wave12 **3**)
**Base tip SHA:** `daa326e` (wave12 tip3 CLOSED / #54) / full `daa326e8296dbe47987160b5028168af10e4debf`

## 1. Purpose

`BEGIN`/`UNTIL`/`WHILE`/`REPEAT` (loop-depth) and `DO`/`LOOP`/`+LOOP`/`I` (do-depth) stubs exist. This tip **thin-deepens** those frames with two stubs:

- `LEAVE` — mark an early-exit on an open BEGIN or DO frame (no branch XT, no token skip).
- `AGAIN` — close a BEGIN-style infinite-loop stub (compile shape of UNTIL-always-false / unconditional back-branch).

Print greppable markers and smoke via **`leave-demo`**. Not a runtime leave/jump, not a WHILE/REPEAT rewrite.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `LEAVE` / `control-leave` | `( -- )` | Requires open BEGIN **or** DO frame; **mark only** (does not pop); print `[leave] LEAVE depth=<n> [frame=begin\|do]` |
| `AGAIN` / `control-again` | `( -- )` | Close BEGIN loop frame (pop loop-depth); print `[loop] AGAIN depth=<n>` |
| `leave-demo` | `( -- )` | See §5 |

Host note: bind `LEAVE` / `AGAIN` on Linux REPL; Forth mirrors `control-leave` / `control-again` if host Forth names collide (host Forth often has real `LEAVE`/`AGAIN`).

## 3. Stub semantics

**Marker namespaces (locked):**

- `LEAVE` prints under **`[leave]`** (same family as `leave-demo`).
- `AGAIN` prints under **`[loop]`** so it sits with `BEGIN` / `UNTIL` / `WHILE` / `REPEAT` (`docs/BEGIN-UNTIL.md`). Do not print AGAIN as `[leave]`.

**Depth families (align, do not invent a third counter):**

- BEGIN loop-depth = existing `BEGIN-UNTIL` counter (`loop-cs-depth` or shared cs). `BEGIN` +1; `UNTIL` / `REPEAT` / **`AGAIN`** −1.
- DO do-depth = existing `DO-LOOP` counter (`do-loop-depth`). `DO` +1; `LOOP` / `+LOOP` −1. `AGAIN` does **not** touch do-depth.
- Shared counter acceptable only if `loop-demo` / `do-loop-demo` / `leave-demo` each still finish at 0.

**`LEAVE`:**

- Legal only when BEGIN loop-depth > 0 **or** do-loop depth > 0. Else `[leave] FAIL` reason=unbalanced (demo must avoid).
- Target: if do-depth > 0, mark the DO frame (ANS-shaped); else mark the open BEGIN frame. If both are open, prefer DO. Print that family's current depth.
- Optional suffix `frame=begin` or `frame=do` (preferred; lab may ignore it).
- **Does not pop** either depth. **Does not** compile or execute a branch XT, skip body tokens, or jump. Optional host flag `leave-pending` on the marked frame is fine; `UNTIL` / `LOOP` / `+LOOP` / `AGAIN` **ignore** it for control flow this tip (they still only balance depth as today).

**`AGAIN`:**

- Legal only when BEGIN loop-depth > 0. Else `[loop] FAIL` reason=unbalanced (same fail family as unbalanced `UNTIL`). An open DO with no BEGIN is **not** enough.
- Pop one BEGIN frame (−1 loop-depth). Print **post-pop** depth: `[loop] AGAIN depth=<n>`.
- Compile-shape of **UNTIL-always-false** (unconditional back-branch / `FALSE UNTIL`): no flag, no `again=` required. Closes the stub frame the way `UNTIL` does. **Does not** re-execute body tokens and does not need a real branch XT.

Nest with `IF`/`THEN`/`ELSE`, `BEGIN`/`UNTIL`/`WHILE`/`REPEAT`, and `DO`/`LOOP`/`+LOOP`/`I` OK if each depth family is 0 at demo end.

## 4. Markers

```
[leave] LEAVE depth=<n> [frame=begin|do]   # frame= optional
[leave] FAIL reason=<…>                    # LEAVE with no open BEGIN or DO
[loop] AGAIN depth=<n>                     # post-pop BEGIN loop-depth
[loop] FAIL reason=<…>                     # AGAIN with no open BEGIN (BEGIN-UNTIL family)
[leave-demo] OK
[leave-demo] FAIL
```

Lab greps `[leave-demo] OK` plus at least one `[leave] LEAVE depth=` and one `[loop] AGAIN depth=`.

## 5. `leave-demo`

1. Clean slate / `dict-reset` if available (resets loop-depth and do-depth).
2. Stream A — LEAVE on BEGIN, existing closer pops: `BEGIN` … `LEAVE` … `UNTIL` → `[loop] BEGIN`, `[leave] LEAVE depth=1` (frame=begin), `[loop] UNTIL`; loop-depth 0. (`LEAVE` must not pop; `UNTIL` does.)
3. Stream B — LEAVE on DO, existing closer pops: `DO` … `LEAVE` … `LOOP` → `[do-loop] DO`, `[leave] LEAVE depth=1` (frame=do), `[do-loop] LOOP`; do-depth 0.
4. Stream C — BEGIN…AGAIN: `BEGIN` … `AGAIN` → `[loop] BEGIN`, `[loop] AGAIN depth=0`; loop-depth 0. No body re-exec.
5. Prior `loop-demo` / `do-loop-demo` / `comment-demo` still OK.
6. `[leave-demo] OK`.

Streams A and C are required (LEAVE + AGAIN). Stream B is required too so both frame families get a LEAVE mark. Depth of each family is 0 before `[leave-demo] OK`.

## 6. Thin amend — companions

### `docs/BEGIN-UNTIL.md`

- Companions: add `LEAVE-AGAIN.md` (wave12 **4**).
- Purpose / §3: point `LEAVE` / `AGAIN` at this tip (no longer an open later-wave bullet).
- Markers note: `AGAIN` reuses `[loop]` — `[loop] AGAIN depth=<n>` — cite `docs/LEAVE-AGAIN.md`. Do not add `AGAIN` to the `loop-demo` required grep.
- Non-goals: `LEAVE` / `AGAIN` stubs → `docs/LEAVE-AGAIN.md` (wave12 **4**).
- Cite: `docs/LEAVE-AGAIN.md`.

### `docs/DO-LOOP.md`

- Companions: add `LEAVE-AGAIN.md` (wave12 **4**).
- §3: strike “`LEAVE` / `AGAIN` (later tips)”; point to wave12 **4**. `LEAVE` may mark an open DO frame but does not pop it (`LOOP` / `+LOOP` still close).
- Non-goals: `LEAVE` / `AGAIN` → `docs/LEAVE-AGAIN.md`.
- Cite: `docs/LEAVE-AGAIN.md`.

## 7. Non-goals

- Real runtime `LEAVE` / jump / branch XT / token skip
- WHILE / REPEAT rewrite (those stubs stay as in `BEGIN-UNTIL`)
- Docs cites pass (wave12 **5**)
- Real crypto / network fleet
- Full Win/Android Forth VM (CONTRACT acceptable)

## 8. Acceptance (Test Lab)

1. `docs/LEAVE-AGAIN.md` present (Research byte-copy OK); `BEGIN-UNTIL.md` + `DO-LOOP.md` thin amends present.
2. `leave-demo` → OK (markers §4); loop-depth and do-depth 0; `loop-demo` + `do-loop-demo` + `comment-demo` still OK.
3. Regression green (wave12 **1–3** + prior demos).
4. Win/Android: CONTRACT acceptable (parity line `leave-demo CONTRACT` OK).
5. No merge. Stay out of Mango.

## 9. Cite

- `docs/BEGIN-UNTIL.md` (wave11 **1**), `docs/DO-LOOP.md` (wave12 **1**), `docs/CONTROL.md` (wave10 **2**), `docs/COMMENT-PARSE.md` (wave12 **3**)
- `forth/tritium/kernel.fs`
- Dusk `LEAVE` / `AGAIN` (stub only)
- Base tip: `daa326e` / `daa326e8296dbe47987160b5028168af10e4debf`
