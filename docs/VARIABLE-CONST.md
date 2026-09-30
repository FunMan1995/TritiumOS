# VARIABLE-CONST — `VARIABLE` / `CONSTANT` named-cell stubs + `var-demo`

**Status:** Shipper-ready stub spec (wave12 item **2**; thin amend wave19 **2** TO-BODY companion cite)
**Canonical brief:** Dusk `variable` / `const` (thin stub); `docs/KERNEL.md` (wave7 **5**), `docs/WORDS-VOCAB.md` (wave11 **3**)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `var.fs`); Linux host REPL
**Companions:** `docs/KERNEL.md` (thin amend this tip), `docs/COLON.md` (thin amend this tip), `docs/WORDS-VOCAB.md` (thin amend this tip), `docs/DO-LOOP.md` (wave12 **1**), `docs/VALUE-TO.md` (wave13 **1**), `docs/CREATE-DOES.md` (wave13 **3**), `docs/ALLOT-HERE.md` (wave14 **1**), `docs/2VARIABLE.md` (wave14 **3**), `docs/CELL-CELLS.md` (wave15 **1**), `docs/DEFER-IS.md` (wave16 **1**), `docs/BUFFER-COLON.md` (wave16 **3**); `docs/TO-BODY.md` (wave19 **2** — CREATE-body address mark companion cite)
**Base tip SHA:** `132f99e` (wave12 tip1 CLOSED / #52)

## 1. Purpose

Flat dict + colon body stubs exist; named cells do not. This tip adds **VARIABLE / CONSTANT stubs**: `VARIABLE <name>` creates a named cell entry (init 0), `CONSTANT <name>` creates a named constant from TOS (or an explicit value arg), prints greppable `[var]` markers, and smokes via **`var-demo`**. Optional `@` / `!` on VARIABLE body if already present — **prefer greppable markers without a full memory / HERE model**. HERE/ALLOT pointer stubs → `docs/ALLOT-HERE.md` (wave14 **1**; not full arena). VALUE/TO stubs → `docs/VALUE-TO.md` (wave13 **1**). CREATE/DOES> defining-word stubs → `docs/CREATE-DOES.md` (wave13 **3**). Double-cell `2VARIABLE` / `2CONSTANT` stubs → `docs/2VARIABLE.md` (wave14 **3**). Dictionary-unit size and align-up (`CELL` / `CELLS` / `ALIGN` / `ALIGNED`) → `docs/CELL-CELLS.md` (wave15 **1**; host constant + stub-pointer round-up, not a physical cell). Deferred-word stubs (`DEFER` / `IS` / `ACTION-OF`) → `docs/DEFER-IS.md` (wave16 **1**; name + bind markers — Forth mirrors; rekia `defer`/`is` untouched). Named buffer stubs (`BUFFER:`) → `docs/BUFFER-COLON.md` (wave16 **3**; size + offset into fill cap / HERE bump — not a VARIABLE body / arena).

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `VARIABLE` / `var-create` | `( "name" -- )` | Create named cell stub; init value **0**; print `[var] VARIABLE name=<name>` |
| `CONSTANT` / `const-create` | `( n "name" -- )` *or* `( "name" -- )` with n from TOS | Create named constant stub; print `[var] CONSTANT name=<name> value=<n>` |
| `var-fetch` / `@` (optional) | `( "name" -- )` *or* `( i -- n )` | If wired: fetch VARIABLE stub cell; print `[var] @ name=<name> value=<n>`; else skip — markers from create are enough |
| `var-store` / `!` (optional) | `( n "name" -- )` *or* `( n i -- )` | If wired: store into VARIABLE stub; print `[var] ! name=<name> value=<n>`; else skip |
| `var-find` | `( c-addr u -- i )` | Optional; alias / thin wrapper of `entry-find` for var/const entries |
| `var-demo` | `( -- )` | See §5 |

Host note: bind `VARIABLE` / `CONSTANT` on Linux REPL; Forth mirrors `var-create` / `const-create` if host Forth names collide (host Forth often has real `VARIABLE`/`CONSTANT`). Optional `@`/`!` only when host already has cell ops — HERE/ALLOT pointer stubs → `docs/ALLOT-HERE.md` (wave14 **1**; full arena still out).

## 3. Stub semantics

- **VARIABLE** → `entry-create` (or host mirror) named cell; stash stub cell = **0**; dict presence greppable via `WORDS` / `entry-find` / find hit.
- **CONSTANT** → create named entry that holds immutable stub value `n` (from TOS, or demo-supplied arg); on exec may print/push `value=<n>` stub — no requirement to push real stack cell if marker alone is greppable.
- Init: VARIABLE always starts at 0; CONSTANT takes the value at create time.
- Optional `@`/`!`: only if already present on host / kernel soft ops; print `[var] @` / `[var] !` markers. VARIABLE may stay host-side int map / entry-slot; HERE/ALLOT pointer stubs → `docs/ALLOT-HERE.md` (wave14 **1**; full arena still out).
- Unrecognized / missing name on optional fetch → `[var] FAIL` reason=miss (demo must avoid).
- Nest with prior colon / control / loop / do-loop stubs OK; dict-reset clears var/const stubs.
- VALUE/TO stubs → `docs/VALUE-TO.md` (wave13 **1**). CREATE/DOES> stubs → `docs/CREATE-DOES.md` (wave13 **3**; markers only, no XT child). HERE/ALLOT pointer stubs → `docs/ALLOT-HERE.md` (wave14 **1**); full arena/pool/free still out. Double-cell `2VARIABLE` / `2CONSTANT` stubs → `docs/2VARIABLE.md` (wave14 **3**; same host cell-map, two-slot). Dictionary-unit words → `docs/CELL-CELLS.md` (wave15 **1**): cell size + align of the HERE stub only; named cells stay host ints (no physical cell allocation). Deferred-word stubs → `docs/DEFER-IS.md` (wave16 **1**; name + bind only, not a VARIABLE body / XT vector). Named buffer stubs → `docs/BUFFER-COLON.md` (wave16 **3**; size + offset slot — not a VARIABLE body / arena). Comment-parse → `docs/COMMENT-PARSE.md` (wave12 **3**).

## 4. Markers

```
[var] VARIABLE name=<name>
[var] CONSTANT name=<name> value=<n>
[var] @ name=<name> value=<n>     # optional
[var] ! name=<name> value=<n>     # optional
[var] FAIL reason=<…>
[var-demo] OK
[var-demo] FAIL
```

Lab greps `[var-demo] OK` plus at least one `[var] VARIABLE name=` and one `[var] CONSTANT name=` with `value=`; dict presence via `WORDS` / find hit / entry-count greppable (N ≥ prior + 2 after creates).

## 5. `var-demo`

1. Clean slate / `dict-reset` if available.
2. `VARIABLE foo` (or fixture name) → `[var] VARIABLE name=foo`; find/WORDS shows `foo`.
3. `42 CONSTANT bar` (or `42` then `CONSTANT bar`) → `[var] CONSTANT name=bar value=42`; find/WORDS shows `bar`.
4. Optional: `@` / `!` on `foo` if wired → `[var] @` / `[var] !` markers; else skip.
5. Assert depth/dict presence greppable (entry-count / words N ≥ 2 for the new names, or find hits both).
6. Prior `do-loop-demo` / `words-demo` / `colon-demo` still OK.
7. `[var-demo] OK`.

## 6. Thin amend — companions

### `docs/KERNEL.md`

- Companions: add `VARIABLE-CONST.md` (wave12 **2**).
- Words table: add `VARIABLE` / `CONSTANT` stubs + `var-demo` (cite tip; no full memory model).
- Non-goals: HERE/ALLOT pointer stubs → wave14 **1** (`ALLOT-HERE.md`); full arena / real DOES> XT still later; VALUE/TO → wave13 **1** (`VALUE-TO.md`); this tip is VARIABLE/CONSTANT stubs only.
- Acceptance: Lab smokes `var-demo`.
- Cite: `docs/VARIABLE-CONST.md`.

### `docs/COLON.md`

- Companions: add `VARIABLE-CONST.md` (wave12 **2**).
- Non-goals / cite: named-cell stubs via wave12 **2**; CREATE/DOES> stubs → `docs/CREATE-DOES.md` (wave13 **3**; markers only, no real XT child).
- Cite: `docs/VARIABLE-CONST.md`.

### `docs/WORDS-VOCAB.md`

- Companions: add `VARIABLE-CONST.md` (wave12 **2**).
- Purpose / demo note: VARIABLE/CONSTANT names appear in flat `WORDS` list after create.
- Cite: `docs/VARIABLE-CONST.md`.

### `docs/DO-LOOP.md` (optional one-line)

- Non-goals: strike open `VARIABLE` / `CONSTANT` (wave12 **2**); point to `docs/VARIABLE-CONST.md`.

## 7. Non-goals

- Full ALLOT / HERE arena / pool / free / linked cell memory model (pointer stubs → `docs/ALLOT-HERE.md` wave14 **1**)
- `CELL` / `CELLS` / `ALIGN` / `ALIGNED` unit markers → `docs/CELL-CELLS.md` (wave15 **1**; not a dictionary image)
- `DEFER` / `IS` / `ACTION-OF` deferred-word stubs → `docs/DEFER-IS.md` (wave16 **1**; mirrors only — do not redefine rekia `defer`/`is`)
- `BUFFER:` named buffer stubs → `docs/BUFFER-COLON.md` (wave16 **3**; not a VARIABLE body / arena)
- VALUE / TO stubs: see `docs/VALUE-TO.md` (wave13 **1**); CREATE / DOES> stubs: see `docs/CREATE-DOES.md` (wave13 **3**; no real XT chaining); HERE/ALLOT stubs: see `docs/ALLOT-HERE.md` (wave14 **1**); `2VARIABLE` / `2CONSTANT` stubs: see `docs/2VARIABLE.md` (wave14 **3**); `>BODY` CREATE-body address mark: see `docs/TO-BODY.md` (wave19 **2**; stub offset / echo only — not VARIABLE body / DOES> XT / arena)
- Comment-parse: see `docs/COMMENT-PARSE.md` (wave12 **3**); leave-again: see `docs/LEAVE-AGAIN.md` (wave12 **4**); docs cites (wave12 **5**)
- Real crypto / network fleet
- Full Win/Android Forth VM (CONTRACT acceptable)

## 8. Acceptance (Test Lab)

1. `docs/VARIABLE-CONST.md` present (Research byte-copy OK); `KERNEL.md` + `COLON.md` + `WORDS-VOCAB.md` thin amends present (DO-LOOP one-line optional).
2. `var-demo` → OK (markers §4); `do-loop-demo` + `words-demo` + `colon-demo` still OK; wave19 **2**: `body-demo` → OK (retains `var-demo` + `create-demo`).
3. Regression green (wave12 **1** + wave11 demos).
4. Win/Android: CONTRACT acceptable (parity line `var-demo CONTRACT` OK).
5. No merge. Stay out of Mango.

## 9. Cite

- `docs/KERNEL.md`, `docs/WORDS-VOCAB.md`, `docs/COLON.md`, `docs/DO-LOOP.md` (wave12 **1**)
- `forth/tritium/kernel.fs`
- Dusk `fs/doc/dict.txt` (`variable` / `const` — stub only)
- Base tip: `132f99e`
- `docs/VALUE-TO.md` (wave13 **1**)
- `docs/CREATE-DOES.md` (wave13 **3**)
- `docs/ALLOT-HERE.md` (wave14 **1**)
- `docs/2VARIABLE.md` (wave14 **3**)
- `docs/CELL-CELLS.md` (wave15 **1**)
- `docs/DEFER-IS.md` (wave16 **1**)
- `docs/BUFFER-COLON.md` (wave16 **3**)
- `docs/TO-BODY.md` (wave19 **2**)
