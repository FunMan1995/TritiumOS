# FILL-MOVE — `FILL` / `ERASE` / `MOVE` / `CMOVE` host-buffer stubs + `fill-demo`

**Status:** Shipper-ready stub spec (wave15 item **3**)
**Canonical brief:** ANS-shaped `FILL` / `ERASE` / `MOVE` / `CMOVE` (thin fixed host-buffer markers); `docs/ALLOT-HERE.md` (wave14 **1**); `docs/KERNEL.md` (wave7 **5**); pairs with wave15 **1** `CELL-CELLS` (optional `CELLS` for `u` — not required for Lab OK)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `fill.fs`); Linux host REPL
**Companions:** `docs/ALLOT-HERE.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip)
**Base tip SHA:** `3587fc2` (wave15 tip2 CLOSED / #68 PICK-ROLL) / full `3587fc274f6dc05221dfa8663ca625c699fddd36`

## 1. Purpose

Wave14 **1** landed `HERE` / `ALLOT` as a byte pointer stub; wave15 **1** added unit markers. There is still no buffer-backed `FILL` / `ERASE` / `MOVE` / `CMOVE` surface. This tip adds **stub** markers over **one fixed host byte buffer** (small cap — document size; prefer **64** bytes): `FILL` writes a char across `u` bytes, `ERASE` clears with char 0, `MOVE` / `CMOVE` copy `u` bytes within (or across regions of) that buffer, and smokes via **`fill-demo`**. **Not** an arena, heap, free list, or real `ALLOCATE`. Does **not** redefine kernel-internal `cmove` used by dict copy in `kernel.fs` / `drena.fs`. Forth mirrors `fill-buf` / `erase-buf` / `move-buf` / `cmove-buf` so host `cmove` stays the primitive. Optional tie to tip1 `CELLS` for `u` — not required for Lab OK. Builds on the wave14 pointer stub without promoting it to a heap.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `FILL` / `fill-buf` | `( addr u char -- )` *or* stub | Fill `u` bytes at `addr` (offset into the fixed host buffer) with `char`; print `[fill] FILL addr= u= char=` |
| `ERASE` / `erase-buf` | `( addr u -- )` *or* stub | Same as FILL with `char=0`; print `[fill] ERASE addr= u=` |
| `MOVE` / `move-buf` | `( from to u -- )` *or* stub | Copy `u` bytes from→to inside the fixed buffer; print `[fill] MOVE from= to= u=` (+ greppable post-image or `bytes=<u>`) |
| `CMOVE` / `cmove-buf` | `( from to u -- )` *or* stub | Byte-copy marker (same host buffer); print `[fill] CMOVE from= to= u=` (+ greppable post-image or `bytes=<u>`). Does **not** replace in-tree `cmove` |
| `fill-demo` | `( -- )` | See §5 |

Host note: bind `FILL` / `ERASE` / `MOVE` / `CMOVE` on the Linux REPL; Forth mirrors **`fill-buf` / `erase-buf` / `move-buf` / `cmove-buf`** so `kernel.fs` / `drena.fs` `cmove` stays the host primitive. Do **not** redefine that kernel-internal `cmove`.

## 3. Stub semantics

- **Fixed host buffer:** one host byte array of **cap = 64** (document if Shipper picks another small size; Lab on Linux SoT expects a documented cap ≤ 256). Addresses are **offsets** into this buffer (`0 .. cap-1`), not heap pointers and not the wave14 HERE stub (optional alias of a region is fine if documented — prefer plain offsets `0..`).
- **FILL addr u char:** write `char` into `buf[addr .. addr+u)`. Print `[fill] FILL addr=<a> u=<u> char=<c>`. `u=0` is a no-op success (still print).
- **ERASE addr u:** FILL with `char=0`. Print `[fill] ERASE addr=<a> u=<u>` (no `char=` required — Lab greps `ERASE`).
- **MOVE / CMOVE from to u:** copy `u` bytes. Print `[fill] MOVE from=<f> to=<t> u=<u>` or `[fill] CMOVE from=<f> to=<t> u=<u>`. Also emit a greppable **post-image** (e.g. hex dump of touched region) **or** `bytes=<u>` on the same / next line. Stub may treat MOVE and CMOVE identically (overlap handling not required for Lab OK); demo should avoid overlapping regions.
- **Bounds:** `addr+u > cap`, `from+u > cap`, `to+u > cap`, or negative → `[fill] FAIL reason=bounds` (demo **must avoid** — keep all ranges inside `0 .. cap`).
- Storage: the fixed host array only. **No** arena, pool, free, fragmentation, real `ALLOCATE`, or dictionary image.
- Optional: `u` may be composed with tip1 `CELLS` (e.g. `2 CELLS` → 16 when cell=8) — **not** required for Lab OK.
- Nest with prior pick / cell / allot / throw / 2var / create / colon / control / string stubs OK. `dict-reset` need not clear the buffer (demo seeds explicitly via FILL/ERASE).
- Still no IMMEDIATE/POSTPONE, real DOES> XT, real branch XT, full Win/Android Forth VM. Those stay non-goals / later tips.

## 4. Markers

```
[fill] FILL addr=<a> u=<u> char=<c>
[fill] ERASE addr=<a> u=<u>              # char 0
[fill] MOVE from=<f> to=<t> u=<u>        # + post-image or bytes=<u>
[fill] CMOVE from=<f> to=<t> u=<u>       # + post-image or bytes=<u>
[fill] FAIL reason=bounds                # demo avoids
[fill-demo] OK
[fill-demo] FAIL
```

Lab greps `[fill-demo] OK` plus `[fill] FILL addr=` with `u=` and `char=`, one `[fill] ERASE addr=` with `u=`, one `[fill] MOVE from=` (or `CMOVE`) with `to=` and `u=`, and a greppable post-image or `bytes=`. Prefer both MOVE and CMOVE once each if cheap.

## 5. `fill-demo`

1. Clean slate / ensure the fixed host buffer exists (cap documented, prefer 64).
2. `FILL` a small region (e.g. `addr=0 u=8 char=65` / `'A'`) → `[fill] FILL addr=0 u=8 char=65` (exact values flexible if greppable).
3. `ERASE` a sub-range or the same region → `[fill] ERASE addr= u=`.
4. Seed source bytes (FILL again), then `MOVE` non-overlapping → `[fill] MOVE from= to= u=` (+ post-image or `bytes=<u>`).
5. `CMOVE` once (non-overlapping) → `[fill] CMOVE from= to= u=` (+ post-image or `bytes=<u>`). If only one of MOVE/CMOVE is wired, the other may alias; Lab needs at least one copy marker.
6. Assert no `[fill] FAIL reason=bounds` on the happy path.
7. Prior `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` still OK.
8. `[fill-demo] OK`.

FILL + ERASE + at least one MOVE-or-CMOVE marker (with post-image or `bytes=`) are required. Bounds FAIL path is not exercised by the demo. Optional `CELLS` composition is not required for Lab OK.

## 6. Thin amend — companions

### `docs/ALLOT-HERE.md`

- Companions: add `FILL-MOVE.md` (wave15 **3**); **keep** `CELL-CELLS.md` (wave15 **1**).
- §3 / Non-goals: pointer stub stays a byte counter; fixed host-buffer FILL/ERASE/MOVE/CMOVE → this tip. Still **not** an arena / ALLOCATE / free. Do not delete wave14 HERE/ALLOT or tip1 CELL text.
- Cite: `docs/FILL-MOVE.md`.

### `docs/KERNEL.md`

- Companions: add `FILL-MOVE.md` (wave15 **3**); **keep** `CELL-CELLS.md` (wave15 **1**) and `PICK-ROLL.md` (wave15 **2**).
- Words table: add `FILL` / `ERASE` / `MOVE` / `CMOVE` stubs + `fill-demo` (cite tip; fixed host buffer only — do not redefine kernel `cmove`; Forth mirrors `fill-buf` / `erase-buf` / `move-buf` / `cmove-buf`).
- Non-goals: buffer-helper stubs → `docs/FILL-MOVE.md`. Stack-marker stubs stay → `docs/PICK-ROLL.md`. Dictionary-unit stubs stay → `docs/CELL-CELLS.md`. Full arena / IMMEDIATE / real XT still later.
- Acceptance: Lab smokes `fill-demo` (and retains `pick-demo` + `cell-demo`).
- Cite: `docs/FILL-MOVE.md`.

Do **not** wipe CELL-CELLS / PICK-ROLL / ALLOT-HERE / THROW-CATCH / other wave14–15 content.

## 7. Non-goals

- Full Dusk arena / pool / free / fragmentation model (this tip = fixed host buffer stub only)
- Real `ALLOCATE` / `FREE` / `RESIZE`
- Redefining kernel-internal `cmove` used by dict copy in `kernel.fs` / `drena.fs` (use mirrors `fill-buf` / `cmove-buf` / `move-buf` / `erase-buf`)
- IMMEDIATE / POSTPONE (wave15 **4** candidate)
- Docs cites pass (wave15 **5**)
- CELL / CELLS / ALIGN / ALIGNED (wave15 **1** — already stubbed; do not reopen)
- PICK / ROLL / DEPTH / ?DUP (wave15 **2** — already stubbed; do not reopen)
- Real DOES> XT chaining / threaded child runtime body
- Real branch XT / LEAVE jump
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/FILL-MOVE.md` present (Research byte-copy OK); `ALLOT-HERE.md` + `KERNEL.md` thin amends present (wave15 **1** CELL-CELLS + wave15 **2** PICK-ROLL cites and wave14 text retained).
2. `fill-demo` → OK (markers §4; FILL / ERASE / MOVE-or-CMOVE greppable; post-image or `bytes=`; no bounds FAIL on happy path). Prior `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` still OK.
3. Regression green (wave15 tip1–2 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `fill-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML.

## 9. Cite

- `docs/ALLOT-HERE.md` (wave14 **1**), `docs/KERNEL.md` (wave7 **5**), `docs/CELL-CELLS.md` (wave15 **1**), `docs/PICK-ROLL.md` (wave15 **2**)
- `forth/tritium/kernel.fs`, `forth/tritium/drena.fs` (`cmove` stays host primitive — mirrors only)
- ANS Forth `FILL` / `ERASE` / `MOVE` / `CMOVE` (stub markers + fixed host buffer only — not an arena)
- Base tip: `3587fc2` / `3587fc274f6dc05221dfa8663ca625c699fddd36` (#68 wave15 tip2 PICK-ROLL)
- Wave15 proposal: `/workspace/tritium-research-docs/WAVE15-PROPOSAL.md`
