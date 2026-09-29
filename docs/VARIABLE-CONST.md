# VARIABLE-CONST — `VARIABLE` / `CONSTANT` named-cell stubs + `var-demo`

**Status:** Shipper-ready stub spec (wave12 item **2**)
**Canonical brief:** Dusk `variable` / `const` (thin stub); `docs/KERNEL.md` (wave7 **5**), `docs/WORDS-VOCAB.md` (wave11 **3**)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `var.fs`); Linux host REPL
**Companions:** `docs/KERNEL.md` (thin amend this tip), `docs/COLON.md` (thin amend this tip), `docs/WORDS-VOCAB.md` (thin amend this tip), `docs/DO-LOOP.md` (wave12 **1**)
**Base tip SHA:** `132f99e` (wave12 tip1 CLOSED / #52)

## 1. Purpose

Flat dict + colon body stubs exist; named cells do not. This tip adds **VARIABLE / CONSTANT stubs**: `VARIABLE <name>` creates a named cell entry (init 0), `CONSTANT <name>` creates a named constant from TOS (or an explicit value arg), prints greppable `[var]` markers, and smokes via **`var-demo`**. Optional `@` / `!` on VARIABLE body if already present — **prefer greppable markers without a full memory / HERE model**. Not real ALLOT, VALUE/TO, or DOES>.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `VARIABLE` / `var-create` | `( "name" -- )` | Create named cell stub; init value **0**; print `[var] VARIABLE name=<name>` |
| `CONSTANT` / `const-create` | `( n "name" -- )` *or* `( "name" -- )` with n from TOS | Create named constant stub; print `[var] CONSTANT name=<name> value=<n>` |
| `var-fetch` / `@` (optional) | `( "name" -- )` *or* `( i -- n )` | If wired: fetch VARIABLE stub cell; print `[var] @ name=<name> value=<n>`; else skip — markers from create are enough |
| `var-store` / `!` (optional) | `( n "name" -- )` *or* `( n i -- )` | If wired: store into VARIABLE stub; print `[var] ! name=<name> value=<n>`; else skip |
| `var-find` | `( c-addr u -- i )` | Optional; alias / thin wrapper of `entry-find` for var/const entries |
| `var-demo` | `( -- )` | See §5 |

Host note: bind `VARIABLE` / `CONSTANT` on Linux REPL; Forth mirrors `var-create` / `const-create` if host Forth names collide (host Forth often has real `VARIABLE`/`CONSTANT`). Optional `@`/`!` only when host already has cell ops — **do not** invent a HERE/ALLOT arena this tip.

## 3. Stub semantics

- **VARIABLE** → `entry-create` (or host mirror) named cell; stash stub cell = **0**; dict presence greppable via `WORDS` / `entry-find` / find hit.
- **CONSTANT** → create named entry that holds immutable stub value `n` (from TOS, or demo-supplied arg); on exec may print/push `value=<n>` stub — no requirement to push real stack cell if marker alone is greppable.
- Init: VARIABLE always starts at 0; CONSTANT takes the value at create time.
- Optional `@`/`!`: only if already present on host / kernel soft ops; print `[var] @` / `[var] !` markers. **No** requirement to back VARIABLE with real ALLOT/HERE this tip — a host-side int map or entry-slot field is enough.
- Unrecognized / missing name on optional fetch → `[var] FAIL` reason=miss (demo must avoid).
- Nest with prior colon / control / loop / do-loop stubs OK; dict-reset clears var/const stubs.
- Still no VALUE/TO, DOES>, real ALLOT/HERE arena; comment-parse → `docs/COMMENT-PARSE.md` (wave12 **3**).

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
- Non-goals: note real ALLOT/HERE / VALUE/TO / DOES> still later; this tip is named-cell stubs only.
- Acceptance: Lab smokes `var-demo`.
- Cite: `docs/VARIABLE-CONST.md`.

### `docs/COLON.md`

- Companions: add `VARIABLE-CONST.md` (wave12 **2**).
- Non-goals / cite: named-cell stubs via wave12 **2**; still no DOES> / CREATE-DOES> body.
- Cite: `docs/VARIABLE-CONST.md`.

### `docs/WORDS-VOCAB.md`

- Companions: add `VARIABLE-CONST.md` (wave12 **2**).
- Purpose / demo note: VARIABLE/CONSTANT names appear in flat `WORDS` list after create.
- Cite: `docs/VARIABLE-CONST.md`.

### `docs/DO-LOOP.md` (optional one-line)

- Non-goals: strike open `VARIABLE` / `CONSTANT` (wave12 **2**); point to `docs/VARIABLE-CONST.md`.

## 7. Non-goals

- Real ALLOT / HERE arena / linked cell memory model
- VALUE / TO / DOES> / CREATE-DOES>
- Comment-parse: see `docs/COMMENT-PARSE.md` (wave12 **3**); leave-again (wave12 **4**); docs cites (wave12 **5**)
- Real crypto / network fleet
- Full Win/Android Forth VM (CONTRACT acceptable)

## 8. Acceptance (Test Lab)

1. `docs/VARIABLE-CONST.md` present (Research byte-copy OK); `KERNEL.md` + `COLON.md` + `WORDS-VOCAB.md` thin amends present (DO-LOOP one-line optional).
2. `var-demo` → OK (markers §4); `do-loop-demo` + `words-demo` + `colon-demo` still OK.
3. Regression green (wave12 **1** + wave11 demos).
4. Win/Android: CONTRACT acceptable (parity line `var-demo CONTRACT` OK).
5. No merge. Stay out of Mango.

## 9. Cite

- `docs/KERNEL.md`, `docs/WORDS-VOCAB.md`, `docs/COLON.md`, `docs/DO-LOOP.md` (wave12 **1**)
- `forth/tritium/kernel.fs`
- Dusk `fs/doc/dict.txt` (`variable` / `const` — stub only)
- Base tip: `132f99e`
