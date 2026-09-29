# MARKER — `MARKER` dictionary-restore stubs + `marker-demo`

**Status:** Shipper-ready stub spec (wave16 item **2**)
**Canonical brief:** ANS-shaped `MARKER` (thin snapshot restore mark); `docs/ALLOT-HERE.md` (wave14 **1**); `docs/KERNEL.md` (wave7 **5**); `docs/WORDS-VOCAB.md` (wave11 **3**); `docs/DEFER-IS.md` (wave16 **1**); explicit WAVE15 / WAVE16 deferral closed as restore-mark stub only
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `marker.fs`); Linux host REPL
**Companions:** `docs/ALLOT-HERE.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip), `docs/WORDS-VOCAB.md` (thin amend this tip)
**Base tip SHA:** `ed3b14ba` (wave16 tip1 CLOSED / #72 DEFER-IS) / full `ed3b14ba086d8c2a7d878983f049a7d360aa7b8d`

## 1. Purpose

WAVE15 and WAVE16 explicitly deferred `MARKER` (full dictionary image / arena rewind / real forget-chain). HERE/ALLOT already expose a stub byte-bump pointer; WORDS lists a flat entry table. This tip lands **stub** dictionary-restore markers only: `MARKER <name>` creates a named restore point (HERE bump counter + optional latest-entry index) and prints `[marker] MARKER name= here=` (+ optional `dict=`); executing the named marker word (or `marker-restore <name>`) restores the stub pointer / entry-count and prints `[marker] RESTORE name= here=`. Smoke via **`marker-demo`**. Forth mirror **`marker-create`** (restore via the named marker word, or optional `marker-restore`). Snapshot mark only — **not** a real forget of XT bodies, not a file-backed dict image, not arena rewind. Builds on wave14 HERE/ALLOT without promoting it to a heap. Independent of tip1 DEFER on paper; serial after DEFER so Lab dict demos stay ordered.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `MARKER` / `marker-create` | `( "name" -- )` | Create named restore point capturing current HERE stub (+ optional entry-count / latest-entry index); print `[marker] MARKER name=<name> here=<n>` (+ optional `dict=<count>`) |
| *(named marker)* / `marker-restore` | `( -- )` *or* `( "name" -- )` | Execute the named marker word **or** call `marker-restore <name>` to restore stub HERE / entry-count; print `[marker] RESTORE name=<name> here=<n>` (+ optional `dict=<count>`) |
| `marker-demo` | `( -- )` | See §5 |

Host note: bind `MARKER` on the Linux REPL; Forth mirror **`marker-create`** if host Forth names collide. Prefer restore by **executing the named marker word** (ANS-shaped); optional `marker-restore <name>` is OK when the named-word path is awkward on host. Captured values are host ints only — not a dict image blob.

## 3. Stub semantics

- **MARKER name:** snapshot current HERE bump counter (from `ALLOT-HERE` stub) + optional latest-entry index / `ENTRY-COUNT`. Create a named dict entry whose body (or host side-table) holds that snapshot. Print `[marker] MARKER name=<name> here=<n>` (+ optional `dict=<count>`). Dict presence greppable via `WORDS` / `entry-find` / find hit.
- **RESTORE (named marker / marker-restore):** look up snapshot by name; set HERE stub back to captured `here=`; optionally trim / reset stub entry-count to captured `dict=` (host int only — **do not** free XT bodies or rewrite a file-backed dict). Print `[marker] RESTORE name=<name> here=<n>` (+ optional `dict=<count>` matching the snapshot).
- **Missing name:** `[marker] FAIL reason=miss` (demo **must avoid** — always MARKER before restore on that name).
- Storage: name string + HERE int (+ optional entry-count int). **No** full dictionary image, no arena rewind, no real forget-chain / wordlist prune beyond the stub count, no BUFFER:, no EXIT/QUIT redefine.
- Nest with prior defer / imm / fill / pick / cell / allot / throw / 2var / create / colon / control / string stubs OK. `dict-reset` clears markers as usual.
- Still no real forget of XT bodies, no full arena/heap, no BUFFER:, no EXIT/QUIT redefine, no linked XT, no real DOES> XT, no real branch XT, no full Win/Android Forth VM. Those stay non-goals / later tips.

## 4. Markers

```
[marker] MARKER name=<name> here=<n> [dict=<count>]   # dict= optional
[marker] RESTORE name=<name> here=<n> [dict=<count>]
[marker] FAIL reason=miss                             # demo avoids
[marker-demo] OK
[marker-demo] FAIL
```

Lab greps `[marker-demo] OK` plus at least one `[marker] MARKER name=` (with `here=`) and one `[marker] RESTORE name=` (with `here=`). Optional `dict=` is not required for Lab OK.

## 5. `marker-demo`

1. Clean slate / `dict-reset` (or cold path); optional HERE reset to base if available.
2. Optional: note HERE / create a throwaway named entry so bump or entry-count is non-trivial.
3. `MARKER` a known name (e.g. `ckpt`) → `[marker] MARKER name=ckpt here=<h0>` (+ optional `dict=`).
4. Bump HERE (`ALLOT`) and/or create ≥1 additional named entry so restore has something to undo.
5. Execute named marker `ckpt` (or `marker-restore ckpt`) → `[marker] RESTORE name=ckpt here=<h0>` (+ optional matching `dict=`). Assert HERE (and optional entry-count) matches the snapshot.
6. Assert no `[marker] FAIL reason=miss` on the happy path. Assert restore did **not** require freeing XT bodies / rewriting a dict image (marker + stub ints are enough).
7. Prior `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` still OK.
8. `[marker-demo] OK`.

MARKER + RESTORE markers (with `here=`) are required. Miss FAIL path is not exercised by the demo. Optional `dict=` echo is not required for Lab OK.

## 6. Thin amend — companions

### `docs/ALLOT-HERE.md`

- Companions: add `MARKER.md` (wave16 **2**); **keep** KERNEL / VARIABLE-CONST / CREATE-DOES / CELL-CELLS / FILL-MOVE cites.
- Purpose / §3: HERE bump stays a byte counter; MARKER restore-mark stubs point at this tip (snapshot HERE + optional entry index — **not** arena rewind / real forget). Do not wipe wave14–15 content.
- Non-goals: `MARKER` dictionary restore → `docs/MARKER.md` (wave16 **2**). Full arena / free still later.
- Acceptance: Lab smokes `marker-demo` (retains `allot-demo`).
- Cite: `docs/MARKER.md`.

### `docs/KERNEL.md`

- Companions: add `MARKER.md` (wave16 **2**); **keep** DEFER-IS (wave16 **1**) and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `MARKER` / `marker-create` stubs + `marker-demo` (cite tip; restore via named marker word or optional `marker-restore` — snapshot mark only).
- Non-goals: MARKER restore-mark stubs → `docs/MARKER.md`. DEFER/IS/ACTION-OF stub mirrors stay on `DEFER-IS.md`. Full forget-chain / arena / linked XT still later.
- Acceptance: Lab smokes `marker-demo` (and retains `defer-demo` + prior demos).
- Cite: `docs/MARKER.md`.

### `docs/WORDS-VOCAB.md`

- Companions: add `MARKER.md` (wave16 **2**); **keep** KERNEL / INTERPRET / COLON / VARIABLE-CONST cites.
- Purpose / §4: flat list stays list-only; MARKER may snapshot / restore stub entry-count — **not** a real wordlist prune or SEARCH-WORDLIST. Do not wipe wave11 content.
- Non-goals: `MARKER` restore → `docs/MARKER.md` (wave16 **2**). SEARCH-WORDLIST / linked dict still later.
- Acceptance: Lab smokes `marker-demo` (retains `words-demo`).
- Cite: `docs/MARKER.md`.

Do **not** wipe tip1 DEFER cites / wave15 / ALLOT / KERNEL / WORDS prior content. Do **not** amend `CREATE-DOES.md` / `VARIABLE-CONST.md` / `DEFER-IS.md` this tip (proposal amends are ALLOT-HERE + KERNEL + WORDS-VOCAB only).

## 7. Non-goals

- Full dictionary image / arena rewind / file-backed dict snapshot
- Real forget-chain / wordlist prune beyond stub entry-count restore
- Freeing XT bodies / linked-list ENTRYSZ forget (Dusk full)
- `BUFFER:` named allot buffer (wave16 **3** candidate)
- `EXIT` / `QUIT` redefine (wave16 **4** candidate — mark-only mirrors later)
- Docs cites pass (wave16 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- DEFER / IS / ACTION-OF (wave16 **1** — already stubbed; do not reopen; keep tip1 cites)
- IMMEDIATE / POSTPONE (wave15 **4** — already stubbed)
- FILL / ERASE / MOVE / CMOVE (wave15 **3** — already stubbed)
- PICK / ROLL / DEPTH / ?DUP (wave15 **2** — already stubbed)
- CELL / CELLS / ALIGN / ALIGNED (wave15 **1** — already stubbed)
- Real DOES> XT chaining / threaded child runtime body
- Real branch XT / LEAVE jump
- Full Dusk arena / pool / free / fragmentation model
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live)
- `ABORT"` polish (already optional-wired inside `throw-demo`)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/MARKER.md` present (Research byte-copy OK); `ALLOT-HERE.md` + `KERNEL.md` + `WORDS-VOCAB.md` thin amends present (wave14/7/11 text, tip1 DEFER cites, and wave15 cites retained).
2. `marker-demo` → OK (markers §4; MARKER + RESTORE greppable with `here=`; no miss FAIL on happy path). Prior `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` still OK.
3. Regression green (wave16 tip1 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `marker-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML.

## 9. Cite

- `docs/ALLOT-HERE.md` (wave14 **1**), `docs/KERNEL.md` (wave7 **5**), `docs/WORDS-VOCAB.md` (wave11 **3**)
- `docs/DEFER-IS.md` (wave16 **1**), `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**), `docs/FILL-MOVE.md` (wave15 **3**), `docs/PICK-ROLL.md` (wave15 **2**), `docs/CELL-CELLS.md` (wave15 **1**)
- `forth/tritium/kernel.fs`
- ANS Forth `MARKER` (snapshot mark only — no real forget / dict image)
- Explicit deferral: WAVE15-PROPOSAL + WAVE16-PROPOSAL (`MARKER` — stub restore only; full image/arena/forget out)
- Base tip: `ed3b14ba` / `ed3b14ba086d8c2a7d878983f049a7d360aa7b8d` (#72 wave16 tip1 DEFER-IS)
- Wave16 proposal: `/workspace/tritium-research-docs/WAVE16-PROPOSAL.md`
