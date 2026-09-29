# EXIT-QUIT — `EXIT` / `QUIT` thin control markers + `exit-demo`

**Status:** Shipper-ready stub spec (wave16 item **4**)
**Canonical brief:** ANS-shaped `EXIT` / `QUIT` (thin control markers only); `docs/COLON.md` (wave9 **4**); `docs/INTERPRET.md` (wave8 **1**); `docs/THROW-CATCH.md` (wave14 **4**); `docs/KERNEL.md` (wave7 **5**); `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**); `docs/BUFFER-COLON.md` (wave16 **3**); `docs/MARKER.md` (wave16 **2**); `docs/DEFER-IS.md` (wave16 **1**); explicit WAVE15 / IMMEDIATE-POSTPONE / WAVE16 deferral closed as mark-only mirrors
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `exit.fs`); Linux host REPL
**Companions:** `docs/COLON.md` (thin amend this tip), `docs/INTERPRET.md` (thin amend this tip), `docs/THROW-CATCH.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip)
**Base tip SHA:** `7bc03f39` (wave16 tip3 CLOSED / #74 BUFFER-COLON) / full `7bc03f391107a884674e67e428463b4906826441`

## 1. Purpose

WAVE15 IMMEDIATE-POSTPONE and WAVE16 explicitly deferred redefining host `EXIT` / `QUIT` (real RS unwind / interpret restart VM). Colon-def, interpret-loop, and CATCH/THROW mark stubs already exist. This tip lands **stub** thin control markers only: `EXIT` prints `[exit] EXIT` (optional `depth=` / colon-frame mark) and does **not** hard-abort the host session; `QUIT` prints `[exit] QUIT` and marks an interpret-reset stub (optional clear of colon-def / catch-depth flags — **prefer mark-only**, document choice). Smoke via **`exit-demo`**. Forth mirrors **`exit-mark`** / **`quit-mark`** so in-tree `exit` early-returns across `kernel.fs` / `rekia.fs` stay the host primitive. **Do not redefine host `exit` / `quit`.** Flag + marker only — **not** a real return-stack unwind, not an interpret-loop restart VM, not RECURSE self-XT. Closes the control-marker deferral before tip5 cites. Serial after BUFFER so Lab demos stay ordered.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `EXIT` / `exit-mark` | `( -- )` | Print `[exit] EXIT` (+ optional `depth=<n>` / colon-frame mark); does **not** hard-abort host session; does **not** walk real RS |
| `QUIT` / `quit-mark` | `( -- )` | Print `[exit] QUIT`; mark interpret-reset stub (prefer mark-only — optional clear of colon-def / catch-depth flags is documented, not required for Lab OK) |
| `exit-demo` | `( -- )` | See §5 |

Host note: bind `EXIT` / `QUIT` on the Linux REPL only when safe; Forth mirrors **`exit-mark`** / **`quit-mark`** are the required Lab surfaces so host Forth / kernel `exit` early-returns and any host `quit` stay untouched. Captured values are host ints / flags only — not a real RS frame or restart VM.

## 3. Stub semantics

- **EXIT:** print `[exit] EXIT` (+ optional `depth=<n>` reflecting colon-def / catch-depth stub, or a colon-frame mark). Does **not** hard-abort the host session, does **not** unwind a real return stack, does **not** skip remaining tokens beyond the mark. Prefer calling from inside a colon-def / demo fixture so optional frame mark is meaningful; outside colon is OK if noframe FAIL is avoided (see below).
- **QUIT:** print `[exit] QUIT`. Marks interpret-reset stub (greppable marker is enough). **Documented choice: prefer mark-only** — do **not** require clearing colon-def / catch-depth flags for Lab OK. Optional clear of those prior-tip flags is allowed when cheap and documented in the Shipper note; Lab greps the QUIT marker either way.
- **Unbalanced / outside colon:** optional `[exit] FAIL reason=noframe` when EXIT is invoked with no open colon-def / frame mark. Demo **avoids** this path (or may show one controlled FAIL then recover — prefer avoid, matching tip1–3 miss/bounds style).
- Storage: flag + optional depth/frame int. **No** real RS unwind, no interpret restart VM, no RECURSE self-XT, no linked XT, **do not** redefine host `exit` / `quit` used across `kernel.fs` / `rekia.fs`.
- Nest with prior buffer / marker / defer / imm / fill / pick / cell / allot / throw / 2var / create / colon / control / string stubs OK. Soft KERNEL `abort` / `(abort")` and wave14 CATCH/THROW mark stubs stay as-is beside these markers.
- Still no real RS unwind / interpret restart, no RECURSE XT, no linked XT, no real DOES> XT, no real branch XT, no full arena/heap, no full Win/Android Forth VM. Those stay non-goals / later tips.

## 4. Markers

```
[exit] EXIT [depth=<n>]              # depth= / colon-frame mark optional
[exit] QUIT                          # interpret-reset stub mark
[exit] FAIL reason=noframe           # optional; demo avoids (or one controlled path)
[exit-demo] OK
[exit-demo] FAIL
```

Lab greps `[exit-demo] OK` plus at least one `[exit] EXIT` and one `[exit] QUIT`. Optional `depth=` on EXIT and optional `reason=noframe` FAIL are not required for Lab OK.

## 5. `exit-demo`

1. Clean slate / `dict-reset` (or cold path); ensure colon-def / catch-depth stubs are in a known state if exposed.
2. Invoke `EXIT` (or `exit-mark`) — prefer inside a colon-def / demo fixture if host makes frame mark easy → `[exit] EXIT` (+ optional `depth=`). Assert host session continues (no hard abort).
3. Invoke `QUIT` (or `quit-mark`) → `[exit] QUIT`. Assert interpret-reset stub mark greppable; prefer mark-only (no required clear of colon-def / catch-depth).
4. Do **not** redefine / shadow host `exit` / `quit` used by kernel/rekia early-returns — mirrors only.
5. Assert no required `[exit] FAIL reason=noframe` on the happy path (demo avoids unbalanced EXIT; optional controlled FAIL path not required for Lab OK).
6. Prior `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` still OK.
7. `[exit-demo] OK`.

EXIT + QUIT markers are required. Optional `depth=` and noframe FAIL are not required for Lab OK. No RECURSE XT. No host `exit` / `quit` redefine.

## 6. Thin amend — companions

### `docs/COLON.md`

- Companions: add `EXIT-QUIT.md` (wave16 **4**); **keep** IMMEDIATE-POSTPONE / INTERPRET / KERNEL / CONTROL / VARIABLE-CONST / CREATE-DOES cites.
- Purpose / §3: colon-def / body-marker stubs stay; EXIT may optionally observe colon-frame mark — **not** a real early-return / RS unwind. Do not wipe wave9/15 colon text.
- Non-goals: `EXIT` / `QUIT` thin control markers → `docs/EXIT-QUIT.md` (wave16 **4**). Still no RECURSE self-XT / linked XT compiler.
- Acceptance: Lab smokes `exit-demo` (retains `colon-demo` + `imm-demo`).
- Cite: `docs/EXIT-QUIT.md`.

### `docs/INTERPRET.md`

- Companions: add `EXIT-QUIT.md` (wave16 **4**); **keep** COLON / COMMENT-PARSE / STRING-LIT / IMMEDIATE-POSTPONE / KERNEL cites.
- Purpose / §3: interpret loop stays; QUIT marks interpret-reset stub only — **not** a real restart VM. Do not wipe wave8–15 interpret text.
- Non-goals: `EXIT` / `QUIT` → `docs/EXIT-QUIT.md` (wave16 **4**). Real nested interpret / EVALUATE still later.
- Acceptance: Lab smokes `exit-demo` (retains `interpret-demo` + `imm-demo` + `colon-demo`).
- Cite: `docs/EXIT-QUIT.md`.

### `docs/THROW-CATCH.md`

- Companions: add `EXIT-QUIT.md` (wave16 **4**); **keep** CONTROL / KERNEL / STRING-LIT / INTERPRET cites.
- Purpose / §3: CATCH/THROW stay mark-only; EXIT/QUIT are sibling thin control markers — **not** a real RS unwind / frame restore (ABORT" polish already optional in throw-demo — not reopened). Soft abort stays. Prefer mark-only QUIT (optional catch-depth clear not required).
- Non-goals: `EXIT` / `QUIT` → `docs/EXIT-QUIT.md` (wave16 **4**). Real exception RS unwind still later.
- Acceptance: Lab smokes `exit-demo` (retains `throw-demo`).
- Cite: `docs/EXIT-QUIT.md`.

### `docs/KERNEL.md`

- Companions: add `EXIT-QUIT.md` (wave16 **4**); **keep** tip1 DEFER + tip2 MARKER + tip3 BUFFER cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `EXIT` / `exit-mark`, `QUIT` / `quit-mark` stubs + `exit-demo` (cite tip; **do not** redefine host `exit` / `quit` used across kernel.fs / rekia.fs; soft `abort` / `(abort")` and CATCH/THROW stay).
- Non-goals: EXIT/QUIT thin control markers → `docs/EXIT-QUIT.md`. BUFFER named-buffer stubs stay on `BUFFER-COLON.md`. MARKER restore-mark stubs stay on `MARKER.md`. DEFER/IS/ACTION-OF stub mirrors stay on `DEFER-IS.md`. Real RS unwind / RECURSE XT / linked XT still later.
- Acceptance: Lab smokes `exit-demo` (and retains `buffer-demo` + `marker-demo` + `defer-demo` + prior demos).
- Cite: `docs/EXIT-QUIT.md`.

Do **not** wipe tip1 DEFER cites / tip2 MARKER cites / tip3 BUFFER cites / wave15 IMMEDIATE/FILL/PICK/CELL / COLON / INTERPRET / THROW-CATCH / KERNEL prior content. Do **not** amend `BUFFER-COLON.md` / `MARKER.md` / `DEFER-IS.md` / `IMMEDIATE-POSTPONE.md` this tip (proposal amends are COLON + INTERPRET + THROW-CATCH + KERNEL only).

## 7. Non-goals

- Redefining host Forth `exit` / `quit` used across `kernel.fs` / `rekia.fs` (mirrors `exit-mark` / `quit-mark` only)
- Real RS unwind / interpret restart VM / frame restore beyond mark-stub
- `RECURSE` real self-XT (no self-XT this tip)
- Linked XT compiler / executing postponed XT / executing bound XT
- Docs cites pass (wave16 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `BUFFER:` named allot buffer (wave16 **3** — already stubbed; do not reopen; keep tip3 cites)
- `MARKER` dictionary restore (wave16 **2** — already stubbed; do not reopen; keep tip2 cites)
- `DEFER` / `IS` / `ACTION-OF` (wave16 **1** — already stubbed; do not reopen; keep tip1 cites)
- IMMEDIATE / POSTPONE (wave15 **4** — already stubbed)
- FILL / ERASE / MOVE / CMOVE (wave15 **3** — already stubbed)
- PICK / ROLL / DEPTH / ?DUP (wave15 **2** — already stubbed)
- CELL / CELLS / ALIGN / ALIGNED (wave15 **1** — already stubbed)
- Real exception RS unwind beyond wave14 THROW-CATCH mark-only (`ABORT"` polish already optional in `throw-demo` — not a new tip)
- Real branch XT / LEAVE jump
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/EXIT-QUIT.md` present (Research byte-copy OK); `COLON.md` + `INTERPRET.md` + `THROW-CATCH.md` + `KERNEL.md` thin amends present (wave9/8/14/7 text, tip1 DEFER cites, tip2 MARKER cites, tip3 BUFFER cites, and wave15 IMMEDIATE/FILL/PICK/CELL cites retained).
2. `exit-demo` → OK (markers §4; EXIT greppable; QUIT greppable; no hard host abort; no required noframe FAIL on happy path). Prior `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` still OK.
3. Regression green (wave16 tip1–3 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `exit-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML.

## 9. Cite

- `docs/COLON.md` (wave9 **4**), `docs/INTERPRET.md` (wave8 **1**), `docs/THROW-CATCH.md` (wave14 **4**), `docs/KERNEL.md` (wave7 **5**)
- `docs/BUFFER-COLON.md` (wave16 **3**), `docs/MARKER.md` (wave16 **2**), `docs/DEFER-IS.md` (wave16 **1**), `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**)
- `docs/FILL-MOVE.md` (wave15 **3**), `docs/PICK-ROLL.md` (wave15 **2**), `docs/CELL-CELLS.md` (wave15 **1**)
- `forth/tritium/kernel.fs` (host `exit` early-returns stay; mirrors only)
- ANS Forth `EXIT` / `QUIT` (mark-only stubs — no RS unwind / interpret restart)
- Explicit deferral: WAVE15-PROPOSAL + IMMEDIATE-POSTPONE non-goal + WAVE16-PROPOSAL (`EXIT`/`QUIT` — thin control markers; do not redefine host `exit`)
- Base tip: `7bc03f39` / `7bc03f391107a884674e67e428463b4906826441` (#74 wave16 tip3 BUFFER-COLON)
- Wave16 proposal: `/workspace/tritium-research-docs/WAVE16-PROPOSAL.md`
