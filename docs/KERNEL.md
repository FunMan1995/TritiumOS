# KERNEL — Minimal dict / find flesh

**Status:** Shipper-ready stub spec (wave7 item **5**)  
**Canonical brief:** `TritiumOS.txt` Phase 2; Dusk `fs/doc/kernel.txt` + `fs/mem/dict.fs` (see `docs/FORTH-BASE-REFERENCES.md`)  
**Sources of truth (code):** `forth/tritium/kernel.fs`; Linux host mirrors for demos  
**Companions:** `docs/GROUPS-NESTED.md` (ENTRY-GIDS), `docs/ARCHITECTURE.md`, `docs/NEURON.md`, `docs/COLON.md` (wave9 **4**), `docs/CONTROL.md` (wave10 **2**); `docs/WORDS-VOCAB.md` (wave11 **3**); `docs/VARIABLE-CONST.md` (wave12 **2**); `docs/COMMENT-PARSE.md` (wave12 **3**); `docs/VALUE-TO.md` (wave13 **1**); `docs/CREATE-DOES.md` (wave13 **3**); `docs/ALLOT-HERE.md` (wave14 **1**); `docs/2VARIABLE.md` (wave14 **3**); `docs/THROW-CATCH.md` (wave14 **4**); `docs/CELL-CELLS.md` (wave15 **1**); `docs/PICK-ROLL.md` (wave15 **2**); `docs/FILL-MOVE.md` (wave15 **3**)
**See also:** `docs/INTERPRET.md` (wave8 **1** — interpret loop deepen); `docs/COLON.md` (wave9 **4** — colon body/marker stub); `docs/COMMENT-PARSE.md` (wave12 **3** — `\` / `(` comment skip).

## 1. Purpose

`kernel.fs` already has a flat name table (`entry-create` / `entry-find` / `group-entry-*` / `cold-boot` / soft `abort`). This tip **fleshes** the Dusk-aligned surface so Lab can smoke a dedicated `kernel-demo` without breaking the suite:

- Documented **find** aliases (`findentry` / `find`)
- Tiny **interpret stub** (lookup-only; no full control-flow compiler)
- Explicit **demo** + markers
- Demoes stay green (groups nested, rekia, license, …)

Not a full VM, linked dict, or xcomp.

## 2. Contract (keep + add)

| Word | Stack | Notes |
|------|-------|-------|
| `entry-create` | `( "name" -- )` | Existing; global gid=-1 |
| `entry-find` | `( c-addr u -- i )` | Existing; `-1` miss |
| `findentry` | `( c-addr u -- i )` | **Alias** of `entry-find` (Dusk name) |
| `find` | `( c-addr u -- i )` | Same alias for host smoke |
| `group-entry-find` / `group-entry-create` | (existing) | Used by GROUPS / nested |
| `words` / `.words` | `( -- )` | List table |
| `.words` / `WORDS` / `words` | `( -- )` | List flat names — see `WORDS-VOCAB.md` (wave11 **3**) |
| `words-demo` | `( -- )` | Create ≥2 + list → OK (`WORDS-VOCAB.md`) |
| `VARIABLE` / `CONSTANT` | (see tip) | Named-cell stubs — see `VARIABLE-CONST.md` (wave12 **2**) |
| `var-demo` | `( -- )` | ≥1 VARIABLE + ≥1 CONSTANT → OK (`VARIABLE-CONST.md`) |
| `VALUE` / `TO` | (see tip) | Named mutable-cell stubs — see `VALUE-TO.md` (wave13 **1**) |
| `value-demo` | `( -- )` | VALUE + TO → OK (`VALUE-TO.md`) |
| `CREATE` / `DOES>` | (see tip) | Defining-word stubs — see `CREATE-DOES.md` (wave13 **3**) |
| `create-demo` | `( -- )` | CREATE + DOES> → OK (`CREATE-DOES.md`) |
| `HERE` / `ALLOT` | (see tip) | Dictionary-pointer stubs — see `ALLOT-HERE.md` (wave14 **1**) |
| `allot-demo` | `( -- )` | HERE + ALLOT bump → OK (`ALLOT-HERE.md`) |
| `2VARIABLE` / `2CONSTANT` | (see tip) | Double-cell named stubs — see `2VARIABLE.md` (wave14 **3**) |
| `2var-demo` | `( -- )` | ≥1 2VARIABLE + ≥1 2CONSTANT → OK (`2VARIABLE.md`) |
| `CATCH` / `THROW` | (see tip) | Exception-frame stubs — see `THROW-CATCH.md` (wave14 **4**) |
| `throw-demo` | `( -- )` | CATCH + THROW → OK (`THROW-CATCH.md`) |
| `CELL` / `CELLS` / `ALIGN` / `ALIGNED` | (see tip) | Dictionary-unit stubs — see `CELL-CELLS.md` (wave15 **1**) |
| `cell-demo` | `( -- )` | CELL + CELLS + align round-up → OK (`CELL-CELLS.md`) |
| `PICK` / `ROLL` / `DEPTH` / `?DUP` | (see tip) | Stack-marker stubs — see `PICK-ROLL.md` (wave15 **2**); Forth mirrors `pick-nth` / `roll-nth` / `stack-depth` / `qdup` |
| `pick-demo` | `( -- )` | DEPTH + PICK + ROLL + ?DUP → OK (`PICK-ROLL.md`) |
| `FILL` / `ERASE` / `MOVE` / `CMOVE` | (see tip) | Fixed host-buffer stubs — see `FILL-MOVE.md` (wave15 **3**); Forth mirrors `fill-buf` / `erase-buf` / `move-buf` / `cmove-buf` (do not redefine kernel `cmove`) |
| `fill-demo` | `( -- )` | FILL + ERASE + MOVE/CMOVE → OK (`FILL-MOVE.md`) |
| `dict-reset` | `( -- )` | Empty table |
| `cold-boot` | `( -- )` | sysvars + dict-reset + edition default + markers |
| `abort` / `(abort")` | soft | Existing; no hard exit; CATCH/THROW mark stubs → `THROW-CATCH.md` (wave14 **4**) |
| `interpret-token` | `( c-addr u -- flag )` | **New stub:** `entry-find` ≥0 → true + print hit; else false + soft miss line |
| `kernel-demo` | `( -- )` | See §4 |

Caps: keep `MAX-ENTRIES` ≥ **32** (bump only if nested+demo pressure; document if changed).

## 3. Markers

```
[kernel] cold-boot OK (64-bit edition)   \ or 32-bit
[kernel] interpret-ready
[kernel] created #N
[kernel] find hit #N name=<word>
[kernel] find miss
[kernel-demo] OK
[kernel-demo] FAIL
```

`cold-boot` must remain safe for poly/AppImage load (no infinite abort).

## 4. `kernel-demo`

1. `dict-reset` (or rely on fresh cold-boot path)
2. Create two names via `entry-create` / host equivalent
3. `find` / `findentry` hits both; miss on unknown → miss marker
4. `words` lists them
5. `interpret-token` on a known name → true
6. Print `[kernel-demo] OK`

Linux host SoT preferred (mirror table already used for group entries); Forth words in `kernel.fs` remain the poly contract.

## 5. Non-goals (this tip)

- Full SEARCH-WORDLIST / linked dict — list-only `WORDS` is wave11 **3** (`WORDS-VOCAB.md`)
- Named-cell stubs (`VARIABLE`/`CONSTANT`) → `docs/VARIABLE-CONST.md` (wave12 **2**); VALUE/TO stubs → `docs/VALUE-TO.md` (wave13 **1**); CREATE/DOES> stubs → `docs/CREATE-DOES.md` (wave13 **3**); HERE/ALLOT pointer stubs → `docs/ALLOT-HERE.md` (wave14 **1**); double-cell stubs → `docs/2VARIABLE.md` (wave14 **3**); CATCH/THROW stubs → `docs/THROW-CATCH.md` (wave14 **4**); dictionary-unit stubs (`CELL`/`CELLS`/`ALIGN`/`ALIGNED`) → `docs/CELL-CELLS.md` (wave15 **1**); stack-marker stubs (`PICK`/`ROLL`/`DEPTH`/`?DUP`) → `docs/PICK-ROLL.md` (wave15 **2**; do not redefine host `2dup`/`2drop`/`2swap`); fixed host-buffer stubs (`FILL`/`ERASE`/`MOVE`/`CMOVE`) → `docs/FILL-MOVE.md` (wave15 **3**; do not redefine kernel `cmove`); full arena / free / IMMEDIATE / XT chaining / FLOAT / real exception RS unwind still later
- Full colon compiler / real branch XT (minimal `interpret` loop → `docs/INTERPRET.md`; comment skip → `docs/COMMENT-PARSE.md` wave12 **3**; `IF`/`THEN`/`ELSE` stubs → `docs/CONTROL.md`)
- Linked-list ENTRYSZ / forget / units objects (Dusk full)
- Replacing host C#/Kotlin VMs
- Breaking `group-find-nested` / vocab persist (share ENTRY-GIDS)

## 6. Acceptance (Test Lab)

1. `docs/KERNEL.md` present (Research byte-copy OK).
2. `kernel-demo` → OK; `find`/`findentry` miss path greppable; wave11 **3**: `words-demo` → OK; wave12 **2**: `var-demo` → OK; wave12 **3**: `comment-demo` → OK; wave13 **1**: `value-demo` → OK; wave13 **3**: `create-demo` → OK; wave14 **1**: `allot-demo` → OK; wave14 **3**: `2var-demo` → OK; wave14 **4**: `throw-demo` → OK; wave15 **1**: `cell-demo` → OK; wave15 **2**: `pick-demo` → OK; wave15 **3**: `fill-demo` → OK.
3. Full regression green (esp. group-nested / group-vocab-persist / rekia / s3-reserved / cold path).
4. No merge.

## 7. Cite

- `TritiumOS.txt` Phase 2
- `docs/FORTH-BASE-REFERENCES.md` (Dusk kernel.txt / mem/dict.fs)
- `forth/tritium/kernel.fs`
- `docs/GROUPS-NESTED.md`, `docs/ARCHITECTURE.md`
- `docs/VARIABLE-CONST.md` (wave12 **2**)
- `docs/COMMENT-PARSE.md` (wave12 **3**)
- `docs/VALUE-TO.md` (wave13 **1**)
- `docs/CREATE-DOES.md` (wave13 **3**)
- `docs/ALLOT-HERE.md` (wave14 **1**)
- `docs/2VARIABLE.md` (wave14 **3**)
- `docs/THROW-CATCH.md` (wave14 **4**)
- `docs/CELL-CELLS.md` (wave15 **1**)
- `docs/PICK-ROLL.md` (wave15 **2**)
- `docs/FILL-MOVE.md` (wave15 **3**)
