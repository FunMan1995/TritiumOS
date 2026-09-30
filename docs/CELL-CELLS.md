# CELL-CELLS — `CELL` / `CELLS` / `ALIGN` / `ALIGNED` dictionary-unit stubs + `cell-demo`

**Status:** Shipper-ready stub spec (wave15 item **1**; thin amend wave19 **1** CHAR-CHARS companion cite; thin amend wave19 **3** ENVIRONMENT-QUERY companion cite; thin amend wave20 **1** TRUE-FALSE companion cite)
**Canonical brief:** ANS-shaped `CELL` / `CELLS` / `ALIGN` / `ALIGNED` (thin unit markers); `docs/ALLOT-HERE.md` (wave14 **1**), `docs/KERNEL.md` (wave7 **5**), `docs/VARIABLE-CONST.md` (wave12 **2**)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `cell.fs`); Linux host REPL
**Companions:** `docs/ALLOT-HERE.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip), `docs/VARIABLE-CONST.md` (thin amend this tip); `docs/CHAR-CHARS.md` (wave19 **1** — char-unit companion cite); `docs/ENVIRONMENT-QUERY.md` (wave19 **3** — query-mark companion cite; optional stub values picture address/cell/char units); `docs/TRUE-FALSE.md` (wave20 **1** — constant-mark companion cite; optional `flag=` / `u=` picture cell-width flag values)
**Base tip SHA:** `201d14b` (wave14 tip5 CLOSED / #66 docs cites) / full `201d14bc6818a8e1985f98507b5f9de136ad2291`

## 1. Purpose

Wave14 **1** landed `HERE` / `ALLOT` as a **byte** bump on a host-held pointer (`docs/ALLOT-HERE.md`). Dictionary-unit words never landed: there is no `CELL`, no `CELLS`, and no align-up of that stub pointer. This tip adds **stub** markers only: `CELL` reports a host cell size, `CELLS` scales a count by that size, `ALIGN` rounds the wave14 HERE stub up to a cell boundary, `ALIGNED` rounds a passed (or stub) address the same way, and smokes via **`cell-demo`**. **Not** a real dictionary image, not aligned physical memory, and not an arena. Pairs with wave14 HERE/ALLOT — the byte counter already exists; unit words do not. Named cells (`VARIABLE` / `CONSTANT`) stay host ints; this tip does not allocate them. Wave19 tip **1** lands the char-unit companion (`CHAR` / `CHARS` / `[CHAR]` + `char-demo` — `docs/CHAR-CHARS.md`): pairs with this tip the way `CHARS` pairs with `CELLS` — **not** an ALIGN/ALIGNED reopen / cell-size rewrite / CHAR+ / unicode. Wave19 tip **3** lands `ENVIRONMENT?` query mark (`docs/ENVIRONMENT-QUERY.md`): optional stub values may picture address/cell/char units (`ADDRESS-UNIT-BITS` / `MAX-CHAR`) — **not** a CELL/ALIGN reopen / cell-size rewrite / real env table. Wave20 tip **1** lands `TRUE` / `FALSE` constant marks (`docs/TRUE-FALSE.md`): optional stub `flag=` / `u=` may picture cell-width flag values (classic all-bits-set / `-1` / `0`) — **not** a CELL/ALIGN reopen / cell-size rewrite / real boolean cell.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `CELL` / `cell-size` | `( -- n )` *or* `( -- )` | Host constant; print `[cell] CELL bytes=<n>`; push `n` if the host stack is easy. **Prefer `n=8`** on 64-bit Linux SoT (document if a non-SoT host uses another `n`) |
| `CELLS` / `cells-n` | `( k -- bytes )` *or* `( k -- )` | `bytes = k * cell`; print `[cell] CELLS n=<k> bytes=<k*cell>`. Does **not** bump HERE |
| `ALIGN` / `align-here` | `( -- )` | Round the wave14 HERE stub pointer **up** to a cell boundary and **store** that rounded address back into the stub; print `[cell] ALIGN addr=<rounded>` |
| `ALIGNED` / `aligned-addr` | `( addr -- a-addr )` *or* `( addr -- )` | Round a **passed** stub addr up the same way; print `[cell] ALIGNED addr=<rounded>`. Does **not** write HERE unless Shipper documents an extra store (prefer pure) |
| `cell-demo` | `( -- )` | See §5 |

Host note: bind `CELL` / `CELLS` / `ALIGN` / `ALIGNED` on the Linux REPL; Forth mirrors `cell-size` / `cells-n` / `align-here` / `aligned-addr` if host Forth names collide (host Forth often has real `CELL`/`CELLS`/`ALIGN`/`ALIGNED`). Do **not** redefine in-tree primitives used by `kernel.fs` beyond these new stub words.

## 3. Stub semantics

- **Cell size** is one host constant. Linux SoT (64-bit): **`n=8`**. Print it; do not probe physical page size or allocate a cell. A non-64-bit host must document its `n` in the marker; Lab on Linux SoT expects `bytes=8`.
- **CELLS:** `bytes = k * n`. `k=0` → `bytes=0` (legal). `k<0` → `[cell] FAIL reason=neg` (demo must avoid). No HERE bump. Optional later composition `k CELLS ALLOT` is **not** required for Lab OK.
- **Round-up** (both `ALIGN` and `ALIGNED`), `n` power-of-two (true for 8):

  ```
  aligned = addr                              if addr % n == 0
  aligned = addr + (n - (addr % n))           otherwise
  ```

  Equivalent when `n` is a power of two: `(addr + n - 1) & ~(n - 1)`. `addr=0` stays `0`. Already-aligned input stays unchanged (still print `addr=`).
- **ALIGN** reads the wave14 stub pointer (same host int / counter as `HERE` / `ALLOT`), rounds it, writes it back, prints `[cell] ALIGN addr=<rounded>`. Optional `from=<old>` is fine; Lab greps `addr=`.
- **ALIGNED** rounds the address it is given (literal or current stub). It must not require a mapped dictionary. Negative addr → `[cell] FAIL reason=neg` (demo avoids).
- Demo must **show** a round-up: if HERE is already aligned (often 0 or a fixed base), bump **one byte** with existing `ALLOT` (`1 ALLOT`) or pass `base+1` into `ALIGNED` so the printed addr is strictly greater and a multiple of `n`. Example on Linux SoT with base 0: unaligned `1` → `addr=8`.
- Storage: the existing wave14 host int is enough. **No** arena, pool, free, real aligned physical memory, or dictionary image.
- Nest with prior HERE/ALLOT / VARIABLE / VALUE / CREATE / colon / control / string / throw stubs OK. `dict-reset` may reset the pointer to base (optional).
- Still no FILL/MOVE buffer, IMMEDIATE/POSTPONE, real DOES> XT, real branch XT, full Win/Android Forth VM. PICK/ROLL is wave15 **2** (independent). Char-unit companion → `docs/CHAR-CHARS.md` (wave19 **1**). ENVIRONMENT? query mark → `docs/ENVIRONMENT-QUERY.md` (wave19 **3** — thin query mark; optional stub values may picture units — not a CELL reopen / env table). TRUE/FALSE constant marks → `docs/TRUE-FALSE.md` (wave20 **1** — constant marks; optional `flag=` / `u=` may picture cell-width flags — not a CELL reopen / boolean cell rewrite). Those stay non-goals here.

## 4. Markers

```
[cell] CELL bytes=<n>                  # Linux SoT: bytes=8
[cell] CELLS n=<k> bytes=<k*cell>
[cell] ALIGN addr=<rounded>            # optional from=<old>
[cell] ALIGNED addr=<rounded>          # optional from=<old>
[cell] FAIL reason=<…>                 # neg (demo avoids)
[cell-demo] OK
[cell-demo] FAIL
```

Lab greps `[cell-demo] OK` plus `[cell] CELL bytes=` (Linux SoT: `bytes=8`), one `[cell] CELLS n=` with `bytes=`, one `[cell] ALIGN addr=`, and one `[cell] ALIGNED addr=`.

## 5. `cell-demo`

1. Clean slate / optional pointer reset to the wave14 stub base if available.
2. `CELL` → `[cell] CELL bytes=8` on 64-bit Linux SoT (record `n`).
3. `3 CELLS` (or `3` then `CELLS`) → `[cell] CELLS n=3 bytes=24` when `n=8` (`bytes` must equal `3*n`).
4. Force an unaligned stub: if HERE `a0` is already a multiple of `n`, `1 ALLOT` (byte bump from wave14). Then `ALIGN` → `[cell] ALIGN addr=<a>` with `a` a multiple of `n` and `a` > unaligned input (visible round-up).
5. `ALIGNED` on a passed unaligned stub addr (e.g. `1`, or `a0+1`) → `[cell] ALIGNED addr=<rounded>` (Linux SoT, input `1`, `n=8` → `addr=8`). Do not require HERE to change on this step.
6. Prior `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` still OK.
7. `[cell-demo] OK`.

`CELL` + `CELLS` markers and both align markers with a visible round-up are required. Feeding `CELLS` into `ALLOT` is not required for Lab OK.

## 6. Thin amend — companions

### `docs/ALLOT-HERE.md`

- Companions: add `CELL-CELLS.md` (wave15 **1**).
- §3: byte bump stays byte-counted; dictionary-unit words (`CELL` / `CELLS` / `ALIGN` / `ALIGNED`) point at this tip. ALIGN rounds the stub pointer only — not a real image. Do not delete wave14 HERE/ALLOT text.
- Non-goals: point unit markers at `docs/CELL-CELLS.md`; full arena stays out. FILL/MOVE and IMMEDIATE/POSTPONE stay later.
- Cite: `docs/CELL-CELLS.md`.

### `docs/KERNEL.md`

- Companions: add `CELL-CELLS.md` (wave15 **1**).
- Words table: add `CELL` / `CELLS` / `ALIGN` / `ALIGNED` stubs + `cell-demo` (cite tip; host constant + round-up only — no dictionary image).
- Non-goals: dictionary-unit stubs → `docs/CELL-CELLS.md`. Full arena / FILL / IMMEDIATE / real XT still later.
- Acceptance: Lab smokes `cell-demo`.
- Cite: `docs/CELL-CELLS.md`.

### `docs/VARIABLE-CONST.md`

- Companions: add `CELL-CELLS.md` (wave15 **1**).
- Purpose / §3: named cells stay host ints; cell **size** and align-up of the HERE stub are this tip, not a physical cell allocation.
- Non-goals: `CELL` / `CELLS` / `ALIGN` / `ALIGNED` → `docs/CELL-CELLS.md` (wave15 **1**).
- Cite: `docs/CELL-CELLS.md`.

## 7. Non-goals

- Full Dusk arena / pool / free / fragmentation model (wave14 HERE/ALLOT stays a pointer stub; this tip is unit markers only)
- Real aligned physical memory / dictionary image / page-size probe
- `CHAR` / `CHARS` / `[CHAR]` char-unit stubs → `docs/CHAR-CHARS.md` (wave19 **1**; char-unit companion — not CHAR+/unicode / ALIGN reopen / cell-size rewrite)
- `ENVIRONMENT?` query mark → `docs/ENVIRONMENT-QUERY.md` (wave19 **3**; thin query mark — optional stub values may picture address/cell/char units; not a CELL/ALIGN reopen / real env table / SEARCH-WORDLIST)
- `TRUE` / `FALSE` constant marks → `docs/TRUE-FALSE.md` (wave20 **1**; constant marks — optional `flag=` / `u=` may picture cell-width flag values; not a CELL/ALIGN reopen / real boolean cell / `0=` deepen / WITHIN)
- Buffer-backed FILL / ERASE / MOVE / CMOVE (wave15 **3** — already stubbed; keep cites)
- PICK / ROLL / DEPTH / ?DUP (wave15 **2**; independent)
- IMMEDIATE / POSTPONE (wave15 **4** candidate)
- Docs cites pass (wave15 **5**)
- Real DOES> XT chaining / threaded child runtime body
- Real branch XT / LEAVE jump
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/CELL-CELLS.md` present (Research byte-copy OK); `ALLOT-HERE.md` + `KERNEL.md` + `VARIABLE-CONST.md` thin amends present (wave14 text retained).
2. `cell-demo` → OK (markers §4; Linux SoT `CELL` `bytes=8`; `CELLS` bytes = `k*n`; ALIGN and ALIGNED `addr=` show round-up). Prior `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` still OK; wave19 **1**: `char-demo` → OK (retains `cell-demo`); wave19 **3**: `env-demo` → OK (retains `cell-demo` + `char-demo`); wave20 **1**: `true-demo` → OK (retains `cell-demo` + `char-demo` + `env-demo`).
3. Regression green (wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `cell-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML.

## 9. Cite

- `docs/ALLOT-HERE.md` (wave14 **1**), `docs/KERNEL.md` (wave7 **5**), `docs/VARIABLE-CONST.md` (wave12 **2**), `docs/2VARIABLE.md` (wave14 **3**)
- `forth/tritium/kernel.fs`
- ANS Forth `CELL` / `CELLS` / `ALIGN` / `ALIGNED` (stub only — not a dictionary image)
- Base tip: `201d14b` / `201d14bc6818a8e1985f98507b5f9de136ad2291` (#66 wave14 tip5)
- `docs/CHAR-CHARS.md` (wave19 **1** — char-unit companion; CHAR pairs with CELL the way CHARS pairs with CELLS)
- `docs/ENVIRONMENT-QUERY.md` (wave19 **3** — query-mark companion; optional stub values picture address/cell/char units)
- `docs/TRUE-FALSE.md` (wave20 **1** — constant-mark companion; optional `flag=` / `u=` picture cell-width flag values)
- Wave15 proposal: `/workspace/tritium-research-docs/WAVE15-PROPOSAL.md`
