# BUFFER-COLON — `BUFFER:` named buffer stubs + `buffer-demo`

**Status:** Shipper-ready stub spec (wave16 item **3**)
**Canonical brief:** ANS-shaped `BUFFER:` (thin named allot-buffer markers); `docs/ALLOT-HERE.md` (wave14 **1**); `docs/FILL-MOVE.md` (wave15 **3**); `docs/VARIABLE-CONST.md` (wave12 **2**); `docs/KERNEL.md` (wave7 **5**); `docs/MARKER.md` (wave16 **2**); `docs/DEFER-IS.md` (wave16 **1**); explicit WAVE15 / WAVE16 deferral closed as named-buffer stub only
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `buffer.fs`); Linux host REPL
**Companions:** `docs/ALLOT-HERE.md` (thin amend this tip), `docs/FILL-MOVE.md` (thin amend this tip), `docs/VARIABLE-CONST.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip)
**Base tip SHA:** `38b8825a` (wave16 tip2 CLOSED / #73 MARKER) / full `38b8825a6c8954ddabe5416caa5cec4247747d3f`

## 1. Purpose

WAVE15 and WAVE16 explicitly deferred `BUFFER:` (full named allot buffer / arena / heap). HERE/ALLOT already expose a stub byte-bump pointer; FILL/ERASE/MOVE/CMOVE already smoke a **fixed host byte buffer** (cap **64**). This tip lands **stub** named buffer markers only: `n BUFFER: <name>` creates a named buffer slot of size `n` (offset into the fill cap and/or a HERE bump region) and prints `[buffer] BUFFER: name= n=` (+ optional `addr=` / offset); fetching the name prints `[buffer] addr= name=` (or returns the stub offset). Smoke via **`buffer-demo`**. Forth mirror **`buffer-colon`** if host names collide. Named slot over wave14 HERE bump and/or wave15 fixed host buffer — **not** an arena, not `ALLOCATE`/`RESIZE`, not a redefinition of the FILL host buffer. Optional tie to tip1 DEFER / tip2 MARKER not required for Lab OK. Serial after MARKER so Lab dict / buffer demos stay ordered.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `BUFFER:` / `buffer-colon` | `( n "name" -- )` | Create named buffer slot of size `n`; print `[buffer] BUFFER: name=<name> n=<n>` (+ optional `addr=<offset>` into fill cap / HERE region) |
| *(named buffer)* / fetch | `( -- addr )` *or* `( -- )` | Execute / fetch the named buffer → print `[buffer] addr=<offset> name=<name>` (optional push of stub offset if host stack easy) |
| `buffer-demo` | `( -- )` | See §5 |

Host note: bind `BUFFER:` on the Linux REPL; Forth mirror **`buffer-colon`** if host Forth names collide. Captured values are host ints / offsets only — not a heap pointer and not a new arena. Prefer offsets into the wave15 fill cap (`0 .. 63` when cap=64) or a documented HERE-bump region alias.

## 3. Stub semantics

- **BUFFER: name:** take size `n` from TOS (or explicit arg); allocate a **named slot** of `n` bytes as an offset into the wave15 fixed host buffer (cap **64**) and/or a wave14 HERE bump region (document choice; prefer fill-cap offsets so FILL/ERASE can touch the same bytes if desired). Create a named dict entry whose body (or host side-table) holds `{name, n, addr/offset}`. Print `[buffer] BUFFER: name=<name> n=<n>` (+ optional `addr=<offset>`). Dict presence greppable via `WORDS` / `entry-find` / find hit.
- **Fetch / execute named buffer:** look up by name; print `[buffer] addr=<offset> name=<name>` (optional push of stub offset). Lab greps `addr=` + `name=`.
- **Bounds:** `n ≤ 0`, `n` past documented cap, or `addr+n > cap` → `[buffer] FAIL reason=bounds` (demo **must avoid** — keep `n` small and inside cap; e.g. `8 BUFFER: buf` with cap=64).
- Storage: name string + size int + offset int. **No** arena, pool, free, fragmentation, real `ALLOCATE`/`RESIZE`, and **do not** redefine / replace the wave15 FILL host buffer itself — named slots are markers over that cap (or HERE bump), not a second heap.
- Optional: bump HERE by `n` when creating the slot (echo via optional `addr=` from HERE before bump) — **not** required for Lab OK if fill-cap offsets alone are greppable.
- Optional tie to tip1 DEFER / tip2 MARKER restore — **not** required for Lab OK.
- Nest with prior marker / defer / imm / fill / pick / cell / allot / throw / 2var / create / colon / control / string stubs OK. `dict-reset` clears named buffer entries as usual (fill buffer contents need not clear).
- Still no full arena/heap/free, no real ALLOCATE/RESIZE, no redefining FILL host buffer, no real DOES> XT, no EXIT/QUIT redefine, no linked XT, no real branch XT, no full Win/Android Forth VM. Those stay non-goals / later tips.

## 4. Markers

```
[buffer] BUFFER: name=<name> n=<n> [addr=<offset>]   # addr= optional
[buffer] addr=<offset> name=<name>                   # fetch / execute named buffer
[buffer] FAIL reason=bounds                          # demo avoids
[buffer-demo] OK
[buffer-demo] FAIL
```

Lab greps `[buffer-demo] OK` plus at least one `[buffer] BUFFER: name=` (with `n=`) and one `[buffer] addr=` (with `name=`). Optional `addr=` on the BUFFER: create line is not required for Lab OK.

## 5. `buffer-demo`

1. Clean slate / `dict-reset` (or cold path); ensure fill-cap host buffer exists (cap **64** from wave15 FILL-MOVE).
2. `8 BUFFER: buf` (or `8` then `BUFFER: buf` / `buffer-colon`) → `[buffer] BUFFER: name=buf n=8` (+ optional `addr=`). Assert `n=8` is inside cap (demo avoids bounds FAIL).
3. Fetch / execute `buf` → `[buffer] addr=<offset> name=buf` (optional push of stub offset).
4. Optional: a second small named buffer (e.g. `4 BUFFER: tmp`) if cheap — not required for Lab OK.
5. Assert no `[buffer] FAIL reason=bounds` on the happy path. Assert create did **not** require a real arena / ALLOCATE / FILL-buffer redefine (name + size + offset ints are enough).
6. Prior `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` still OK.
7. `[buffer-demo] OK`.

BUFFER: create (with `n=`) + fetch (`addr=` + `name=`) markers are required. Bounds FAIL path is not exercised by the demo. Optional `addr=` on create and optional HERE bump are not required for Lab OK.

## 6. Thin amend — companions

### `docs/ALLOT-HERE.md`

- Companions: add `BUFFER-COLON.md` (wave16 **3**); **keep** MARKER (wave16 **2**) / FILL-MOVE / CELL-CELLS / KERNEL / VARIABLE-CONST / CREATE-DOES cites.
- Purpose / §3: HERE bump stays a byte counter; BUFFER: named slots may optionally bump HERE or alias a fill-cap offset — **not** an arena. Do not wipe wave14–16 tip1–2 content.
- Non-goals: `BUFFER:` named allot buffer → `docs/BUFFER-COLON.md` (wave16 **3**). Full arena / free still later. MARKER restore stays on `MARKER.md`.
- Acceptance: Lab smokes `buffer-demo` (retains `allot-demo` + `marker-demo`).
- Cite: `docs/BUFFER-COLON.md`.

### `docs/FILL-MOVE.md`

- Companions: add `BUFFER-COLON.md` (wave16 **3**); **keep** ALLOT-HERE / KERNEL / CELL-CELLS / PICK-ROLL cites.
- Purpose / §3: fixed host buffer (cap **64**) stays the FILL surface; BUFFER: named slots may use offsets into this same cap — **do not** redefine / replace the FILL host buffer. Do not wipe wave15 FILL text.
- Non-goals: `BUFFER:` named allot buffer → `docs/BUFFER-COLON.md` (wave16 **3**). Real ALLOCATE / arena still later.
- Acceptance: Lab smokes `buffer-demo` (retains `fill-demo`).
- Cite: `docs/BUFFER-COLON.md`.

### `docs/VARIABLE-CONST.md`

- Companions: add `BUFFER-COLON.md` (wave16 **3**); **keep** DEFER-IS (wave16 **1**) / CELL-CELLS / ALLOT-HERE / CREATE-DOES / VALUE-TO cites.
- Purpose / §3: named cells stay VARIABLE/CONSTANT; BUFFER: is a named size+offset slot — **not** a VARIABLE body / arena cell. Do not wipe tip1 DEFER cites.
- Non-goals: `BUFFER:` → `docs/BUFFER-COLON.md` (wave16 **3**). DEFER/IS stay on `DEFER-IS.md`.
- Acceptance: Lab smokes `buffer-demo` (retains `var-demo` + `defer-demo`).
- Cite: `docs/BUFFER-COLON.md`.

### `docs/KERNEL.md`

- Companions: add `BUFFER-COLON.md` (wave16 **3**); **keep** MARKER (wave16 **2**) / DEFER-IS (wave16 **1**) and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `BUFFER:` / `buffer-colon` stubs + `buffer-demo` (cite tip; named slot over HERE bump and/or fill cap — **not** an arena; do not redefine FILL host buffer).
- Non-goals: BUFFER: named-buffer stubs → `docs/BUFFER-COLON.md`. MARKER restore-mark stubs stay on `MARKER.md`. DEFER/IS/ACTION-OF stub mirrors stay on `DEFER-IS.md`. Full arena / ALLOCATE / linked XT still later.
- Acceptance: Lab smokes `buffer-demo` (and retains `marker-demo` + `defer-demo` + prior demos).
- Cite: `docs/BUFFER-COLON.md`.

Do **not** wipe tip1 DEFER cites / tip2 MARKER cites / wave15 IMMEDIATE/FILL/PICK/CELL / ALLOT / KERNEL / VARIABLE-CONST prior content. Do **not** amend `MARKER.md` / `DEFER-IS.md` / `WORDS-VOCAB.md` / `CREATE-DOES.md` this tip (proposal amends are ALLOT-HERE + FILL-MOVE + VARIABLE-CONST + KERNEL only).

## 7. Non-goals

- Full Dusk arena / pool / free / fragmentation model (this tip = named slot stub only)
- Real `ALLOCATE` / `FREE` / `RESIZE`
- Redefining / replacing the wave15 FILL fixed host buffer (named slots may offset into it; FILL surface stays `FILL-MOVE.md`)
- Real DOES> XT chaining / threaded child runtime body
- `EXIT` / `QUIT` redefine (wave16 **4** candidate — mark-only mirrors later)
- Docs cites pass (wave16 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `MARKER` dictionary restore (wave16 **2** — already stubbed; do not reopen; keep tip2 cites)
- `DEFER` / `IS` / `ACTION-OF` (wave16 **1** — already stubbed; do not reopen; keep tip1 cites)
- IMMEDIATE / POSTPONE (wave15 **4** — already stubbed)
- FILL / ERASE / MOVE / CMOVE (wave15 **3** — already stubbed; do not redefine)
- PICK / ROLL / DEPTH / ?DUP (wave15 **2** — already stubbed)
- CELL / CELLS / ALIGN / ALIGNED (wave15 **1** — already stubbed)
- Real branch XT / LEAVE jump
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live)
- `ABORT"` polish (already optional-wired inside `throw-demo`)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/BUFFER-COLON.md` present (Research byte-copy OK); `ALLOT-HERE.md` + `FILL-MOVE.md` + `VARIABLE-CONST.md` + `KERNEL.md` thin amends present (wave14/15/7/12 text, tip1 DEFER cites, tip2 MARKER cites, and wave15 IMMEDIATE/FILL/PICK/CELL cites retained).
2. `buffer-demo` → OK (markers §4; BUFFER: greppable with `name=` + `n=`; fetch greppable with `addr=` + `name=`; no bounds FAIL on happy path). Prior `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` still OK.
3. Regression green (wave16 tip1–2 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `buffer-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML.

## 9. Cite

- `docs/ALLOT-HERE.md` (wave14 **1**), `docs/FILL-MOVE.md` (wave15 **3**), `docs/VARIABLE-CONST.md` (wave12 **2**), `docs/KERNEL.md` (wave7 **5**)
- `docs/MARKER.md` (wave16 **2**), `docs/DEFER-IS.md` (wave16 **1**), `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**), `docs/PICK-ROLL.md` (wave15 **2**), `docs/CELL-CELLS.md` (wave15 **1**)
- `forth/tritium/kernel.fs`
- ANS Forth `BUFFER:` (named size slot only — no arena / ALLOCATE)
- Explicit deferral: WAVE15-PROPOSAL + WAVE16-PROPOSAL (`BUFFER:` — stub named buffer only; full arena/heap out)
- Base tip: `38b8825a` / `38b8825a6c8954ddabe5416caa5cec4247747d3f` (#73 wave16 tip2 MARKER)
- Wave16 proposal: `/workspace/tritium-research-docs/WAVE16-PROPOSAL.md`
