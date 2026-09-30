# ALLOT-HERE — `HERE` / `ALLOT` dictionary-pointer stubs + `allot-demo`

**Status:** Shipper-ready stub spec (wave14 item **1**; thin amend wave19 **2** TO-BODY companion cite; thin amend wave22 **3** TO-NUMBER companion cite — HERE stub reminder; do not bump; thin amend wave23 **2** HOLD companion cite — HERE stub reminder; do not bump)
**Canonical brief:** ANS-shaped `HERE` / `ALLOT` (thin pointer stub); `docs/VARIABLE-CONST.md` (wave12 **2**), `docs/CREATE-DOES.md` (wave13 **3**), `docs/KERNEL.md` (wave7 **5**)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `allot.fs` / `here.fs`); Linux host REPL
**Companions:** `docs/KERNEL.md` (thin amend this tip), `docs/VARIABLE-CONST.md` (thin amend this tip), `docs/CREATE-DOES.md` (thin amend this tip), `docs/CELL-CELLS.md` (wave15 **1**), `docs/FILL-MOVE.md` (wave15 **3**), `docs/MARKER.md` (wave16 **2**), `docs/BUFFER-COLON.md` (wave16 **3**); `docs/TO-BODY.md` (wave19 **2** — CREATE-body address mark companion cite); `docs/TO-NUMBER.md` (wave22 **3** — thin number-parse companion cite; HERE stub base `$1000` reminder — **do not bump**); `docs/BASE-HEX.md` (wave21 **2** — radix marks; numeric radix only — not HERE stub base); `docs/HOLD.md` (wave23 **2** — thin pictured-numeric start companion cite; HERE stub base `$1000` reminder — **do not bump**; pictured buffer must not bump HERE; prefer `hold-mark`)
**Base tip SHA:** `b2c8e7d` (wave13 tip5 CLOSED / #61) / full `b2c8e7d2de370ca822c49bc9f3a8c9f84d77598b`

## 1. Purpose

Named-cell / CREATE surfaces (VARIABLE / CONSTANT / VALUE / CREATE) deferred **ALLOT / HERE**. This tip lands **stub** dictionary-pointer markers: `HERE` reports a host-held pointer (or byte-bump counter) with a greppable `[allot] HERE addr=` line; `ALLOT` bumps that pointer by `n` and prints `[allot] ALLOT n=`. Optional `,` / `C,` cell/char-store stubs may bump + mark if cheap. Smoke via **`allot-demo`**. **Not** a Dusk arena/pool, free/fragmentation model, or linked cell memory — a host int / counter is enough. Builds on wave12–13 named-cell / CREATE surface; DRENA docs already reference HERE allocation. Wave19 tip **2** lands `>BODY` as a sibling CREATE-body **stub offset / echo** (`docs/TO-BODY.md`) — **not** a HERE bump / arena reopen / mapped dictionary image. Wave22 tip **3** lands `>NUMBER` thin number-parse mark (`docs/TO-NUMBER.md`) — sibling thin parse beside BASE-HEX; **HERE stub reminder — do not bump** `_here` / `HERE` / `here-at`; HERE stub base stays `$1000 _here !`; prefer `to-number-mark`; **not** a HERE/ALLOT reopen / arena / BASE reopen / pictured numeric. Wave23 tip **2** lands `HOLD` thin pictured-numeric start (`docs/HOLD.md`) — sibling thin pictured start; **HERE stub reminder — do not bump** `_here` / `HERE` / `here-at`; pictured buffer must not bump HERE; HERE stub base stays `$1000 _here !`; prefer `hold-mark`; **not** a HERE/ALLOT reopen / arena / BASE reopen / TO-NUMBER reopen / full `#S`/`#>`/`SIGN`.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `HERE` / `here-at` | `( -- addr )` *or* `( -- )` | Report current stub pointer; print `[allot] HERE addr=<n>`; push optional if host stack easy |
| `ALLOT` / `allot-bump` | `( n -- )` | Bump stub pointer by `n` (bytes or cells — document choice; **bytes** preferred for markers); print `[allot] ALLOT n=<n>` (+ optional `addr=<new>`) |
| `,` / `comma` (optional) | `( n -- )` | If wired: store cell stub + bump; print `[allot] , value=<n>`; else skip |
| `C,` / `c-comma` (optional) | `( c -- )` | If wired: store char stub + bump; print `[allot] C, value=<c>`; else skip |
| `allot-demo` | `( -- )` | See §5 |

Host note: bind `HERE` / `ALLOT` on Linux REPL; Forth mirrors `here-at` / `allot-bump` if host Forth names collide (host Forth often has real `HERE`/`ALLOT`). Optional `,` / `C,` only when bump + marker are cheap — **do not** invent a real memory arena / free list this tip.

## 3. Stub semantics

- **HERE** → read host-held pointer / bump counter; print `[allot] HERE addr=<n>`. Initial value may be 0 or a fixed stub base (e.g. 0x1000) — document in markers; Lab greps `addr=` presence, not a specific absolute.
- **ALLOT** → add `n` to the stub pointer; print `[allot] ALLOT n=<n>` (optional `addr=<new>` after bump). Negative `n` → `[allot] FAIL` reason=neg (demo must avoid) **or** allow and document — prefer reject-or-skip for stub simplicity.
- Optional `,` / `C,`: bump by cell size / 1 + print value marker. No requirement to retain stored bytes in a real buffer if markers alone are greppable.
- Storage: one host `int` (or `size_t`) is enough — **no** arena, pool, free, fragmentation, or linked cell memory.
- Dictionary-unit words `CELL` / `CELLS` / `ALIGN` / `ALIGNED` → `docs/CELL-CELLS.md` (wave15 **1**). This tip's bump stays a **byte** counter; cell size is a host constant (prefer 8 on 64-bit Linux SoT). `ALIGN` rounds this stub pointer up to a cell boundary only — **not** a real aligned physical image or dictionary.
- Fixed host-buffer `FILL` / `ERASE` / `MOVE` / `CMOVE` → `docs/FILL-MOVE.md` (wave15 **3**). Still **not** an arena / `ALLOCATE` / free — one capped host byte array only.
- Dictionary-restore `MARKER` snapshot (HERE + optional entry index) → `docs/MARKER.md` (wave16 **2**). Snapshot mark only — **not** arena rewind / real forget of XT bodies.
- Named buffer `BUFFER:` slots (size + offset into fill cap and/or HERE bump) → `docs/BUFFER-COLON.md` (wave16 **3**). Named slot only — **not** an arena / ALLOCATE / FILL-buffer redefine.
- Nest with prior VARIABLE / VALUE / CREATE / colon / control / string stubs OK; `dict-reset` may reset the pointer to base (optional; demo may record before/after without requiring reset).
- Still no real DOES> XT chain, UNLOOP/J, THROW/CATCH, 2VARIABLE, full Win/Android Forth VM; those → later wave14 tips / non-goals. CREATE-body address mark → `docs/TO-BODY.md` (wave19 **2**; stub offset / echo — does **not** bump HERE / reopen arena).

## 4. Markers

```
[allot] HERE addr=<n>
[allot] ALLOT n=<n> [addr=<new>]     # addr= optional
[allot] , value=<n>                  # optional
[allot] C, value=<c>                 # optional
[allot] FAIL reason=<…>
[allot-demo] OK
[allot-demo] FAIL
```

Lab greps `[allot-demo] OK` plus at least one `[allot] HERE addr=` and one `[allot] ALLOT n=`.

## 5. `allot-demo`

1. Clean slate / optional pointer reset to base if available.
2. `HERE` → `[allot] HERE addr=<a0>` (record `a0`).
3. `8 ALLOT` (or `8` then `ALLOT`) → `[allot] ALLOT n=8` (optional `addr=<a0+8>`).
4. `HERE` again → `[allot] HERE addr=<a1>` with `a1` ≥ `a0` (bump visible; exact cell-vs-byte math flexible if documented).
5. Optional: `,` / `C,` once each → markers; else skip.
6. Prior `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` still OK.
7. `[allot-demo] OK`.

HERE + ALLOT markers and a visible bump are required. Optional comma words are not required for Lab OK.

## 6. Thin amend — companions

### `docs/KERNEL.md`

- Companions: add `ALLOT-HERE.md` (wave14 **1**).
- Words table: add `HERE` / `ALLOT` stubs + `allot-demo` (cite tip; pointer/bump only — no full memory model).
- Non-goals: strike open “real ALLOT/HERE … still later”; point pointer stubs to `docs/ALLOT-HERE.md`. Full arena / free / XT chaining still later.
- Acceptance: Lab smokes `allot-demo`.
- Cite: `docs/ALLOT-HERE.md`.

### `docs/VARIABLE-CONST.md`

- Companions: add `ALLOT-HERE.md` (wave14 **1**).
- Purpose / §3: strike open “Still no real ALLOT/HERE arena”; point HERE/ALLOT **pointer stubs** at this tip. Still no full arena/pool/free.
- Non-goals: ALLOT / HERE stubs → `docs/ALLOT-HERE.md` (wave14 **1**); keep full arena / linked cell memory out.
- Cite: `docs/ALLOT-HERE.md`.

### `docs/CREATE-DOES.md`

- Companions: add `ALLOT-HERE.md` (wave14 **1**).
- Purpose / §3 / Non-goals: strike open HERE/ALLOT arena as wholly absent; point pointer stubs at this tip. Still no real DOES> XT chaining / full arena.
- Cite: `docs/ALLOT-HERE.md`.

## 7. Non-goals

- Full Dusk arena / pool / free / fragmentation model (this tip = pointer stub only)
- `CELL` / `CELLS` / `ALIGN` / `ALIGNED` dictionary-unit markers → `docs/CELL-CELLS.md` (wave15 **1**); fixed host-buffer FILL/ERASE/MOVE/CMOVE → `docs/FILL-MOVE.md` (wave15 **3**); `MARKER` restore-mark stubs → `docs/MARKER.md` (wave16 **2**); `BUFFER:` named buffer stubs → `docs/BUFFER-COLON.md` (wave16 **3**); IMMEDIATE/POSTPONE already stubbed (wave15 **4**). Full arena stays out
- Real linked cell memory / buffer-backed `,`/`C,` heap
- Real DOES> XT chaining / threaded child runtime body
- `>BODY` CREATE-body address mark → `docs/TO-BODY.md` (wave19 **2**; stub offset / echo only — not HERE bump / arena / body image / DOES> XT)
- `>NUMBER` thin number-parse mark → `docs/TO-NUMBER.md` (wave22 **3**; thin parse only — **HERE stub reminder; do not bump** `_here`/`HERE`/`here-at`; HERE stub base stays `$1000`; prefer `to-number-mark`; not BASE reopen / pictured numeric)
- `HOLD` thin pictured-numeric start → `docs/HOLD.md` (wave23 **2**; thin pictured start only — **HERE stub reminder; do not bump** `_here`/`HERE`/`here-at`; pictured buffer must not bump HERE; HERE stub base stays `$1000`; prefer `hold-mark`; not BASE reopen / TO-NUMBER reopen / full `#S`/`#>`/`SIGN`)
- UNLOOP / J (wave14 **2** candidate)
- 2VARIABLE / 2CONSTANT (wave14 **3** candidate)
- THROW / CATCH / ABORT" (wave14 **4** candidate)
- Docs cites pass (wave14 **5**)
- Real crypto / network fleet
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/ALLOT-HERE.md` present (Research byte-copy OK); `KERNEL.md` + `VARIABLE-CONST.md` + `CREATE-DOES.md` thin amends present.
2. `allot-demo` → OK (markers §4); prior `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` still OK; wave19 **2**: `body-demo` → OK (retains `allot-demo` + `create-demo`; >BODY does not bump HERE). Wave22 **3**: `number-demo` → OK (retains `allot-demo` + `base-demo`; HERE stub base `$1000` untouched — TO-NUMBER must not bump `_here`/`HERE`/`here-at`). Wave23 **2**: `hold-demo` → OK (retains `allot-demo` + `number-demo` + `base-demo` + `charplus-demo`; HERE stub base `$1000` untouched — HOLD must not bump `_here`/`HERE`/`here-at`; pictured buffer must not bump HERE).
3. Regression green (wave13 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `allot-demo CONTRACT` OK).
5. No merge. Stay out of Mango.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**), `docs/VARIABLE-CONST.md` (wave12 **2**), `docs/CREATE-DOES.md` (wave13 **3**), `docs/VALUE-TO.md` (wave13 **1**)
- `forth/tritium/kernel.fs`
- ANS Forth `HERE` / `ALLOT` (stub only); Dusk mem/arena (pointer stub only — full arena deferred); DRENA HERE allocation cites
- Base tip: `b2c8e7d` / `b2c8e7d2de370ca822c49bc9f3a8c9f84d77598b`
- Wave14 proposal: `/workspace/tritium-research-docs/WAVE14-PROPOSAL.md`
- `docs/CELL-CELLS.md` (wave15 **1**)
- `docs/FILL-MOVE.md` (wave15 **3**)
- `docs/MARKER.md` (wave16 **2**)
- `docs/BUFFER-COLON.md` (wave16 **3**)
- `docs/TO-BODY.md` (wave19 **2** — CREATE-body address mark; not HERE bump / arena)
