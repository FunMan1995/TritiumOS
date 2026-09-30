# LSHIFT-RSHIFT — `LSHIFT` / `RSHIFT` shift marks + `shift-demo`

**Status:** Shipper-ready stub spec (wave22 item **1**)
**Canonical brief:** ANS-shaped `LSHIFT` / `RSHIFT` (thin shift marks only); `docs/BITWISE.md` (wave21 **4** — AND/OR/XOR/INVERT sibling bitwise marks; **not** AND/OR/XOR/INVERT reopen / BITWISE reopen); `docs/TRUE-FALSE.md` (wave20 **1** — TRUE/FALSE constant-picture companion; flag= / all-bits `-1` vs `0`); `docs/KERNEL.md` (wave7 **5**); optional `docs/CELL-CELLS.md` (wave15 **1** — cell-sized int picture companion) / `docs/PICK-ROLL.md` (wave15 **2**) / `docs/HOST-PARITY.md` (wave8 **4**); explicit WAVE19 / WAVE20 / WAVE21 / WAVE22 deferral closed as **shift marks only** (not real boolean cell rewrite / `0=` deepen / AND-OR reopen / WITHIN reopen / BITWISE reopen; **CRITICAL:** host lowercase `lshift` / `rshift` already live in `forth/trit.fs` + `forth/tritium/drena.fs` — do **not** redefine).
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `shift.fs` / `lshift-rshift.fs`); Linux host REPL; **CRITICAL — do not** redefine host lowercase `lshift` / `rshift` (pack-header / unpack-header / phi-fold and other host uses in `trit.fs` / `drena.fs`); **do not** redefine host `LSHIFT` / `RSHIFT` that already bind on the load path — prefer Forth mirrors; **do not** redefine `AND` / `OR` / `XOR` / `INVERT` / `and-mark` / `or-mark` / `xor-mark` / `invert-mark` (BITWISE stays mark-only)
**Companions:** `docs/BITWISE.md` (thin amend this tip), `docs/TRUE-FALSE.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip); optional light cite `docs/CELL-CELLS.md` / `docs/PICK-ROLL.md` / `docs/HOST-PARITY.md`
**Base tip SHA:** `0cf98da` (wave21 tip5 CLOSED / #101 DOCS-CITES) / full `0cf98dac548d57f6ee515433869926f533af428a`

## 1. Purpose

WAVE15 landed cell-unit + stack/?DUP flag pictures (`docs/CELL-CELLS.md`, `docs/PICK-ROLL.md`); WAVE20 tip **1** landed `TRUE` / `FALSE` constant marks (`docs/TRUE-FALSE.md` — classic all-bits-set / `-1` vs `0` picture); WAVE20 tip **2** landed `WITHIN` range-check mark (`docs/WITHIN.md` — **not** bitwise / shift). WAVE21 tip **4** landed `AND` / `OR` / `XOR` / `INVERT` bitwise marks (`docs/BITWISE.md` — classic `0xFF` AND/OR/XOR/INVERT picture; host lowercase `and`/`or` untouched). WAVE18 / WAVE19 / WAVE20 / WAVE21 / WAVE22 explicitly deferred `LSHIFT` / `RSHIFT` (shift marks beside wave21 BITWISE; **not** real boolean cell rewrite / `0=` deepen / AND-OR reopen / WITHIN reopen). This tip lands **stub** shift marks only: `LSHIFT` (or Forth mirror **`lshift-mark`**) prints `[shift] LSHIFT` (+ optional `u=` — classic fixed stub-int shift-left picture welcome); `RSHIFT` / **`rshift-mark`** prints `[shift] RSHIFT` (+ optional `u=` — classic shift-right picture welcome). Smoke via **`shift-demo`**. Prefer Forth mirrors **`lshift-mark` / `rshift-mark`** whenever host `LSHIFT`/`RSHIFT` collide — and **CRITICAL:** host already uses lowercase **`lshift` / `rshift`** as host Forth primitives in `forth/trit.fs` (e.g. `pack-header` / `unpack-header`) and `forth/tritium/drena.fs` (e.g. `phi-fold`) — do **NOT** redefine those. Pairs with wave21 BITWISE bitwise marks and wave20 TRUE/FALSE constant picture **without** a boolean-cell rewrite, **without** reopening `0=` (tip2), **without** reopening AND/OR/XOR/INVERT / BITWISE, and **without** reopening WITHIN. **Not** tip2 ZERO-EQUALS / tip3 TO-NUMBER / tip4 SEARCH-WORDLIST / tip5 DOCS-CITES. Closes a shift deferral as **thin mark** beside wave21 BITWISE without promoting either to a real boolean/flag/ALU machine. Independent of tip2–5.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `LSHIFT` / `lshift-mark` | `( x1 u -- x2 )` *or* `( -- )` with fixed demo fixture | Shift-left mark; print `[shift] LSHIFT` (+ optional `u=<n>`); classic `1 LSHIFT 4 → 16` / `0x01 << 4 → 0x10` picture welcome |
| `RSHIFT` / `rshift-mark` | `( x1 u -- x2 )` *or* `( -- )` with fixed demo fixture | Shift-right mark; print `[shift] RSHIFT` (+ optional `u=<n>`); classic `16 RSHIFT 4 → 1` / `0x10 >> 4 → 0x01` picture welcome |
| `shift-demo` | `( -- )` | See §5 |

Host note: bind bare `LSHIFT` / `RSHIFT` on the Linux REPL **only if** those names do not collide with host Forth shift words in the same load path. Prefer Forth mirrors **`lshift-mark` / `rshift-mark`** as the Lab-facing surface when in doubt — **do not** redefine host `LSHIFT`/`RSHIFT`. **CRITICAL:** host lowercase **`lshift` / `rshift`** (`trit.fs` pack-header / unpack-header; `drena.fs` phi-fold / other host uses) are **NOT** this tip’s surface and must **not** be redefined, aliased, shadowed, or Lab-grepped as shift success. **Do not** redefine `AND` / `OR` / `XOR` / `INVERT` / `and-mark` / `or-mark` / `xor-mark` / `invert-mark` — BITWISE stays mark-only sibling. Optional `u=` is host int / fixture echo only — not a live boolean cell, not `0=` algebra, not AND/OR/XOR/INVERT reopen, not WITHIN. Prefer **fixed demo stub-int fixtures** (classic `1 << 4 → 16` / `16 >> 4 → 1` picture, or hex `0x01` / `0x10`) so Lab hit is deterministic and FAIL is avoided.

## 3. Stub semantics

- **`LSHIFT` / `lshift-mark`:** take (or use fixed demo) stub ints `( x1 u )`. Classic ANS / common Forth picture: logical (or arithmetic — document) shift left → result `x2 = x1 << u` (cell-width picture; greppable marker is enough). Print `[shift] LSHIFT` and optionally `u=<n>` (Lab-greppable; prefer documenting which Shipper emits). Classic fixture welcome: `1 LSHIFT 4 → u=16` (or hex `u=0x10`). **Does not** rewrite TRUE/FALSE constants, deepen `0=` / flag algebra, reopen AND/OR/XOR/INVERT / BITWISE, bump HERE, open a heap, or create dict entries. Captured values are host ints / fixture echo only.
- **`RSHIFT` / `rshift-mark`:** shift right → `x2 = x1 >> u` (document logical vs arithmetic if Shipper echoes a signed fixture; greppable `[shift] RSHIFT` is enough for Lab OK). Print `[shift] RSHIFT` (+ optional `u=`). Classic fixture welcome: `16 RSHIFT 4 → u=1` (or hex `u=0x01`). Same constraints as LSHIFT — marker only.
- **Fixed demo stub-int fixtures (document):**
  - **Classic shift picture (required set):** e.g. `x=1` (or `0x01`), `u=4`, expected left → `16` / `0x10`; expected right from `16` / `0x10` → `1` / `0x01`. Demo must exercise **both** marks (`LSHIFT` / `RSHIFT`) so Lab greps `[shift] LSHIFT` and `[shift] RSHIFT`. Optional `u=` welcome on each.
  - **Optional result echo:** LSHIFT → `u=16` / `u=0x10`; RSHIFT → `u=1` / `u=0x01` — document which form Shipper emits. Greppable markers alone are enough for Lab OK when both `[shift] …` lines appear.
- **Optional push:** if the host stack is easy, push the pictured result; marker alone is enough for Lab OK — do not require a real boolean cell / flag algebra / AND-OR reopen / `0=` deepen / WITHIN reopen.
- **FAIL:** `[shift] FAIL reason=<…>` optional (demo **must avoid**). Prefer not emitting `[shift] FAIL` on the happy path.
- Storage: fixed demo stub ints / host int echo only. **No** real boolean cell rewrite, no `0=` deepen (tip2), no AND/OR/XOR/INVERT reopen / BITWISE reopen, no WITHIN reopen, no COMPARE reopen, no BASE-HEX / ACCEPT-REFILL reopen, no HERE bump, no arena.
- **Host `lshift` / `rshift` stay untouched:** lowercase host `lshift` / `rshift` remain kernel Forth primitives (`trit.fs` pack-header / unpack-header; `drena.fs` phi-fold; etc.). This tip’s Lab greps are `[shift] LSHIFT` / `[shift] RSHIFT` and `[shift-demo] OK` only — **never** Lab-grep bare host `lshift` / `rshift` as this tip’s shift success. Prefer mirrors whenever host `LSHIFT`/`RSHIFT` or lowercase collide.
- **BITWISE stays:** do **not** redefine `AND` / `OR` / `XOR` / `INVERT` / `and-mark` / `or-mark` / `xor-mark` / `invert-mark`. Shift marks are **siblings** beside bitwise marks — not a BITWISE reopen.
- Nest with prior bit / compare / base / accept / exec / count / within / true / source / env / body / char / state / word / find / tick / create / allot / synonym / exit / buffer / marker / defer / imm / fill / pick / cell / throw / 2var / colon / control / string stubs OK. `dict-reset` unaffected (fixed host fixtures, not dictionary).
- Still no boolean cell / `0=` deepen (tip2), no `>NUMBER` (tip3), no SEARCH-WORDLIST (tip4), no tip5 DOCS-CITES, no AND-OR / WITHIN reopen, no linked XT / real DOES> XT / real branch XT / full arena/heap / full Win/Android Forth VM. Wave21 tip1–5 + wave20 tip1–4 + wave19 tip1–4 stay landed — keep cites; this tip does not reopen them.

## 4. Markers

```
[shift] LSHIFT [u=<n>]       # u= optional; classic 1 LSHIFT 4 → u=16 / u=0x10 welcome
[shift] RSHIFT [u=<n>]       # u= optional; classic 16 RSHIFT 4 → u=1 / u=0x01 welcome
[shift] FAIL reason=<…>      # demo avoids
[shift-demo] OK
[shift-demo] FAIL
```

Lab greps `[shift-demo] OK` plus greppable **`[shift] LSHIFT`** and **`[shift] RSHIFT`** (optional `u=` welcome on each). Demo avoids `[shift] FAIL`. Prefer not emitting `[shift] FAIL` on the happy path. **Do not** Lab-grep host lowercase `lshift` / `rshift` as this tip’s surface. **Do not** Lab-grep `[bit]` / `[true]` / `[within]` / `[compare]` / `[base]` as LSHIFT-RSHIFT success (those stay their own tips). **Do not** Lab-grep `[bit] AND` / `[bit] OR` / `[bit] XOR` / `[bit] INVERT` as this tip’s surface (BITWISE stays wave21 **4**).

## 5. `shift-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; shift marks need no dict entries.
2. Ensure **fixed demo stub-int fixtures** exist (e.g. `x=1` / `0x01`, `u=4`, right-source `16` / `0x10` — or equivalent host ints) so the classic shift picture is deterministic. Fixtures may live beside (not replacing) BITWISE stub-int fixtures / TRUE/FALSE constant picture / CELL unit picture / PICK stack picture / WITHIN range-check — document; do **not** require boolean-cell rewrite / `0=` deepen / AND-OR reopen / BITWISE reopen / WITHIN reopen / COMPARE reopen.
3. Invoke `LSHIFT` (or **`lshift-mark`**) against the classic pair → `[shift] LSHIFT` (+ optional `u=` — classic `16` / `0x10` welcome).
4. Invoke `RSHIFT` (or **`rshift-mark`**) → `[shift] RSHIFT` (+ optional `u=` — classic `1` / `0x01` welcome).
5. Assert no `[shift] FAIL` on the happy path. Assert shift marks did **not** require a real boolean cell rewrite / `0=` deepen / AND/OR/XOR/INVERT reopen / BITWISE reopen / WITHIN reopen / COMPARE reopen / HERE bump / host `lshift`/`rshift` redefine (marker-only is enough). Assert host `LSHIFT`/`RSHIFT` were not redefined when using the Forth mirrors. Assert host lowercase `lshift` / `rshift` were **not** redefined, aliased, shadowed, or used as this tip’s Lab surface. Assert `and-mark` / `or-mark` / `xor-mark` / `invert-mark` were **not** redefined.
6. Prior `bit-demo` / `compare-demo` / `base-demo` / `accept-demo` / `exec-demo` / `count-demo` / `within-demo` / `true-demo` / `source-demo` / `env-demo` / `body-demo` / `char-demo` / `state-demo` / `word-demo` / `find-demo` / `tick-demo` / `string-demo` / `fill-demo` / `pick-demo` / `cell-demo` + earlier demos incl `trit-math-demo` / `fold-demo` still OK (host `lshift`/`rshift` uses in trit-math / fold must remain intact).
7. `[shift-demo] OK`.

Both markers (`[shift] LSHIFT` / `[shift] RSHIFT`) are required. Optional `u=` echo is not required for Lab OK when both markers are greppable. No real boolean cell. No `0=` deepen. No AND-OR reopen. No BITWISE reopen. No WITHIN reopen. No host `lshift`/`rshift` redefine.

## 6. Thin amend — companions

### `docs/BITWISE.md`

- Companions / Status: add `LSHIFT-RSHIFT.md` (wave22 **1** companion cite); **keep** TRUE-FALSE / KERNEL / CELL-CELLS / PICK-ROLL / WITHIN / HOST-PARITY / COMPARE / BASE-HEX / ACCEPT-REFILL cites — do not wipe wave21 BITWISE content.
- Purpose / §3 / non-goals: AND/OR/XOR/INVERT stay bitwise marks; `LSHIFT` / `RSHIFT` are sibling **shift marks** over fixed stub-int fixtures — **not** an AND/OR/XOR/INVERT reopen / BITWISE reopen / boolean-cell rewrite / `0=` deepen / WITHIN reopen. Do not wipe wave21 BITWISE content. Stress: shift marks beside BITWISE — **not** BITWISE reopen; host lowercase `lshift`/`rshift` stay untouched; host lowercase `and`/`or` stay untouched; do **not** redefine `and-mark`/`or-mark`/`xor-mark`/`invert-mark`.
- Non-goals: `LSHIFT` / `RSHIFT` → `docs/LSHIFT-RSHIFT.md` (wave22 **1**). AND/OR/XOR/INVERT stay on this tip (already landed). TRUE/FALSE stay wave20 **1**. WITHIN stays wave20 **2**. `0=` still tip2. Tip5 DOCS-CITES still later (wave22 **5**).
- Acceptance: Lab smokes `shift-demo` (retains `bit-demo` + `true-demo` + `within-demo` + `base-demo` + `compare-demo` + `accept-demo`).
- Cite: `docs/LSHIFT-RSHIFT.md`.

### `docs/TRUE-FALSE.md`

- Companions / Status: add `LSHIFT-RSHIFT.md` (wave22 **1** companion cite); **keep** BITWISE / WITHIN / BASE-HEX / KERNEL / CELL-CELLS / PICK-ROLL / CONTROL / HOST-PARITY cites — do not wipe wave20 TRUE-FALSE content.
- Purpose / §3 / non-goals: TRUE/FALSE stay constant marks (`flag=` / all-bits `-1` vs `0`); `LSHIFT` / `RSHIFT` are sibling **shift marks** beside constants — **not** a TRUE/FALSE reopen / boolean-cell rewrite / `0=` deepen (tip2) / AND-OR reopen / BITWISE reopen / WITHIN reopen. Do not wipe wave20 TRUE-FALSE content. Stress: shift marks beside TRUE/FALSE constants — **not** boolean cell / `0=`; host lowercase `lshift`/`rshift` stay untouched.
- Non-goals: `LSHIFT` / `RSHIFT` → `docs/LSHIFT-RSHIFT.md` (wave22 **1**). TRUE/FALSE stay on this tip (already landed). BITWISE stays wave21 **4**. `0=` still tip2 ZERO-EQUALS. WITHIN stays wave20 **2**.
- Acceptance: Lab smokes `shift-demo` (retains `true-demo` + `bit-demo` + `within-demo` + `base-demo` + `compare-demo`).
- Cite: `docs/LSHIFT-RSHIFT.md`.

### `docs/KERNEL.md`

- Companions: add `LSHIFT-RSHIFT.md` (wave22 **1**); **keep** wave21 tip1–4 ACCEPT-REFILL / BASE-HEX / COMPARE / BITWISE cites and wave20 tip1–4 TRUE-FALSE / WITHIN / COUNT / EXECUTE cites and wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites and wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites and wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `LSHIFT` / `lshift-mark`, `RSHIFT` / `rshift-mark` stubs + `shift-demo` (cite tip; Forth mirrors `lshift-mark` / `rshift-mark` — shift marks only; **CRITICAL:** do **not** redefine host lowercase `lshift`/`rshift`; **do not** redefine host `LSHIFT`/`RSHIFT`; **do not** redefine `AND`/`OR`/`XOR`/`INVERT`/`and-mark`/`or-mark`/`xor-mark`/`invert-mark`; **not** real boolean cell rewrite / `0=` deepen / AND-OR reopen / BITWISE reopen / WITHIN reopen; optional `u=` — classic `1 LSHIFT 4 → 16` / `16 RSHIFT 4 → 1` picture on stub ints welcome).
- Non-goals: `LSHIFT` / `RSHIFT` shift marks → `docs/LSHIFT-RSHIFT.md`. AND/OR/XOR/INVERT stay on `BITWISE.md`. COMPARE stays on `COMPARE.md`. BASE/HEX/DECIMAL stay on `BASE-HEX.md`. ACCEPT/REFILL stay on `ACCEPT-REFILL.md`. TRUE/FALSE stay on `TRUE-FALSE.md`. WITHIN stays on `WITHIN.md`. host lowercase `lshift`/`rshift` stay kernel Forth primitives — **not** this tip. Tip2 ZERO-EQUALS / tip3 TO-NUMBER / tip4 SEARCH-WORDLIST / tip5 DOCS-CITES still later (wave22 **2–5**).
- Acceptance: Lab smokes `shift-demo` (and retains `bit-demo` + `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `source-demo` + `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `string-demo` + `fill-demo` + prior demos incl `trit-math-demo` / `fold-demo`).
- Cite: `docs/LSHIFT-RSHIFT.md`.

### Optional — `docs/CELL-CELLS.md`

- Companions / Status: add light `LSHIFT-RSHIFT.md` (wave22 **1**) cite; **keep** BITWISE / TRUE-FALSE / WITHIN / BASE-HEX / CHAR-CHARS / ENVIRONMENT-QUERY / ALLOT-HERE / KERNEL / VARIABLE-CONST cites — do not wipe wave15 CELL content.
- Purpose / §3 / non-goals: cell-unit stubs stay; `LSHIFT` / `RSHIFT` may echo optional stub `u=` that picture **cell-sized** shift results on stub ints — **not** a CELL/ALIGN reopen / cell-size rewrite / real boolean cell / `0=` deepen / BITWISE reopen. Do not wipe wave15 / wave19 / wave20 / wave21 CELL/CHAR/ENV/TRUE/BASE/BITWISE content. Stress: shift marks on cell-sized ints — not CELL reopen; host lowercase `lshift`/`rshift` stay untouched.
- Non-goals: `LSHIFT` / `RSHIFT` → `docs/LSHIFT-RSHIFT.md` (wave22 **1**). CELL/CELLS/ALIGN/ALIGNED stay on this tip (already landed). BITWISE stays wave21 **4**. TRUE/FALSE stay wave20 **1**.
- Acceptance: Lab smokes `shift-demo` (retains `cell-demo` + `bit-demo` + `true-demo` + `within-demo` + `base-demo`).
- Cite: `docs/LSHIFT-RSHIFT.md`.

### Optional — `docs/PICK-ROLL.md`

- Companions / Status: add light `LSHIFT-RSHIFT.md` (wave22 **1**) cite; **keep** BITWISE / TRUE-FALSE / WITHIN / KERNEL / CELL-CELLS cites — do not wipe wave15 PICK-ROLL content.
- Purpose / §3 / non-goals: stack/?DUP flag pictures stay; `LSHIFT` / `RSHIFT` are sibling **shift** marks — **not** a PICK/ROLL/?DUP reopen / real threaded stack / `0=` deepen / boolean cell / BITWISE reopen / WITHIN reopen. Do not wipe wave15 PICK-ROLL content. Host `2dup`/`2drop`/`2swap` stay untouched. Host lowercase `lshift`/`rshift` stay untouched.
- Non-goals: `LSHIFT` / `RSHIFT` → `docs/LSHIFT-RSHIFT.md` (wave22 **1**). PICK/ROLL/DEPTH/?DUP stay on this tip (already landed). BITWISE stays wave21 **4**. TRUE/FALSE stay wave20 **1**. WITHIN stays wave20 **2**.
- Acceptance: Lab smokes `shift-demo` (retains `pick-demo` + `bit-demo` + `true-demo` + `within-demo`).
- Cite: `docs/LSHIFT-RSHIFT.md`.

### Optional — `docs/HOST-PARITY.md`

- Companions: add light `LSHIFT-RSHIFT.md` (wave22 **1**) cite; **keep** BITWISE / TRUE-FALSE / BASE-HEX / ENVIRONMENT-QUERY / KERNEL / BUILD / INSTALL / INTERPRET cites.
- Purpose / non-goals: Win/Android stay CONTRACT-parity; `shift-demo` CONTRACT line is acceptable — **not** a full Forth VM / boolean-cell / shift-ALU / bitwise-ALU port. Do not wipe wave8 HOST-PARITY content. Stress: host lowercase `lshift`/`rshift` stay untouched on Linux SoT.
- Non-goals: `LSHIFT` / `RSHIFT` → `docs/LSHIFT-RSHIFT.md` (wave22 **1**). Full Win/Android Forth VM still out. BITWISE stays wave21 **4**. TRUE/FALSE stay wave20 **1**. BASE-HEX stays wave21 **2**.
- Acceptance: Lab smokes `shift-demo` (Win/Android: `shift-demo CONTRACT` OK).
- Cite: `docs/LSHIFT-RSHIFT.md`.

Do **not** wipe wave21 tip1–5 / wave20 tip1–4 / wave19 tip1–4 / wave18 tip1–4 / wave17 tip1–4 / wave16 DEFER/MARKER/BUFFER/EXIT / wave15 CELL prior content. Do **not** amend ARCHITECTURE / IMPLEMENTATION-GAPS / ACCEPT-REFILL / BASE-HEX / COMPARE primary this tip (amends are BITWISE + TRUE-FALSE + KERNEL + optional CELL-CELLS / PICK-ROLL / HOST-PARITY only). **Leave `ACCEPT-REFILL.md` untouched** (md5 `6bda5a5170ac71fa3ccf7db28286dd44` / 23948). **Leave `BASE-HEX.md` untouched** (`d12dd22a8502b321568fa370b6ecdd2e` / 21270). **Leave `COMPARE.md` untouched** (`dc429e1f010818957353c216adb5c317` / 23322). **Leave `ARCHITECTURE.md` / `IMPLEMENTATION-GAPS.md` untouched** (`b924e9ce5c1e14efd8e0ae738dde5f51` / `712d77305646b9275cb4e76195f00f38`). **Leave `WAVE22-PROPOSAL.md` / `WAVE22-COS-PASTE.txt` untouched.** Tip5 cites after 1–4 PASS. Skip 2DUP-FAMILY + ABORT" polish.

## 7. Non-goals

- Redefining / aliasing / shadowing / Lab-grepping host lowercase `lshift` / `rshift` as this tip’s shift surface
- Redefining `AND` / `OR` / `XOR` / `INVERT` / `and-mark` / `or-mark` / `xor-mark` / `invert-mark` (BITWISE stays — **not** AND-OR reopen / BITWISE reopen)
- Real boolean cell rewrite / live flag storage / VARIABLE-as-boolean
- `0=` / `0<>` / flag algebra deepen (tip2 ZERO-EQUALS — still deferred; keep TRUE-FALSE / BITWISE cites — do not reopen)
- `WITHIN` reopen (wave20 **2** — already stubbed; keep cites — **not** runtime compare re-exec)
- `TRUE` / `FALSE` reopen (wave20 **1** — already stubbed; keep cites; thin companion amend only — pairs without boolean-cell rewrite)
- `BITWISE` reopen (wave21 **4** — already stubbed; thin companion amend only — shift marks are siblings, not reopen)
- `COMPARE` / `BASE-HEX` / `ACCEPT-REFILL` reopen (wave21 **1–3** — already stubbed; leave those primaries untouched)
- `>NUMBER` / pictured numeric `#`/`HOLD`/`<#`/`#>` (tip3 TO-NUMBER — still deferred; do **not** bump HERE stub `$1000`)
- `SEARCH-WORDLIST` / FIND reopen (tip4 — still deferred)
- Docs cites pass (wave22 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `COUNT` / `EXECUTE` / `SOURCE` / `PAD` reopen (wave20 **3–4** / wave19 **4** — already stubbed; keep cites)
- `CELL` / `CELLS` / `ALIGN` / `ALIGNED` reopen (wave15 — already stubbed; shift marks on cell-sized ints — not CELL reopen)
- `PICK` / `ROLL` / `DEPTH` / `?DUP` reopen (wave15 — already stubbed; keep cites)
- `IF` / `THEN` / `ELSE` reopen as real branch XT (wave10 — already mark-only; keep cites)
- Real DOES> XT chaining / threaded child runtime body
- Real branch XT / LEAVE jump
- Full Dusk arena / pool / free / fragmentation model
- Linked XT / executing postponed XT / real STATE cell
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live — **skip 2DUP-FAMILY**)
- `ABORT"` polish (already optional-wired inside `throw-demo` — **skip**)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/LSHIFT-RSHIFT.md` present (Research byte-copy OK); `BITWISE.md` + `TRUE-FALSE.md` + `KERNEL.md` thin amends present (+ optional `CELL-CELLS.md` / `PICK-ROLL.md` / `HOST-PARITY.md`); wave21 tip1–5 ACCEPT-REFILL / BASE-HEX / COMPARE / BITWISE / DOCS-CITES cites, wave20 tip1–4 TRUE-FALSE / WITHIN / COUNT / EXECUTE cites, wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites, wave18 tip1–4 cites, wave17 tip1–4 cites, wave16 DEFER/MARKER/BUFFER/EXIT cites, and wave15 CELL/IMMEDIATE/COLON/KERNEL prior text retained; host `LSHIFT`/`RSHIFT` untouched via mirrors; host lowercase `lshift`/`rshift` **not** redefined / aliased / shadowed / Lab-grepped as shift success; `AND`/`OR`/`XOR`/`INVERT`/`and-mark`/`or-mark`/`xor-mark`/`invert-mark` **not** redefined; `ACCEPT-REFILL.md` primary untouched (tip1 land md5 `6bda5a5170ac71fa3ccf7db28286dd44` / 23948); `BASE-HEX.md` primary untouched (tip2 land md5 `d12dd22a8502b321568fa370b6ecdd2e` / 21270); `COMPARE.md` primary untouched (tip3 land md5 `dc429e1f010818957353c216adb5c317` / 23322); `ARCHITECTURE.md` + `IMPLEMENTATION-GAPS.md` byte-copy unchanged (wave21 tip5 land md5s `b924e9ce5c1e14efd8e0ae738dde5f51` / `712d77305646b9275cb4e76195f00f38`); `WAVE22-PROPOSAL.md` / `WAVE22-COS-PASTE.txt` untouched.
2. `shift-demo` → OK (markers §4; `[shift] LSHIFT` / `[shift] RSHIFT` greppable; optional `u=` welcome — classic `1 LSHIFT 4 → 16` / `16 RSHIFT 4 → 1` picture on stub ints; no FAIL on happy path; no real boolean cell / `0=` deepen / AND-OR reopen / BITWISE reopen / WITHIN reopen / COMPARE reopen; no host `lshift`/`rshift` redefine). Prior `bit-demo` + `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `source-demo` / `env` / `body` / `char` / `state` / `word` / `find` / `tick` + earlier demos incl `trit-math-demo` / `fold-demo` still OK.
3. Regression green (wave21 tip1–5 + wave20 tip1–5 + wave19 tip1–5 + wave18 tip1–5 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `shift-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**)
- `docs/BITWISE.md` (wave21 **4** — AND/OR/XOR/INVERT sibling bitwise marks; LSHIFT/RSHIFT are sibling **shift** marks — **not** AND/OR/XOR/INVERT reopen / BITWISE reopen; host lowercase `and`/`or` stay untouched)
- `docs/TRUE-FALSE.md` (wave20 **1** — TRUE/FALSE constant-picture companion; shift marks beside constants — **not** boolean-cell rewrite / `0=` reopen)
- `docs/CELL-CELLS.md` (wave15 **1**, optional — unit/cell picture companion; shift marks on cell-sized ints — not CELL reopen)
- `docs/PICK-ROLL.md` (wave15 **2**, optional — stack/?DUP flag companion; not PICK/?DUP reopen)
- `docs/HOST-PARITY.md` (wave8 **4**, optional — `shift-demo` CONTRACT parity welcome)
- `docs/WITHIN.md` (wave20 **2** — prior tip; keep cites; **not** WITHIN reopen)
- `docs/COMPARE.md` (wave21 **3** — prior tip; keep cites; leave primary untouched this tip; host `cstr=` ≠ ANS COMPARE)
- `docs/BASE-HEX.md` (wave21 **2** — prior tip; keep cites; leave primary untouched this tip)
- `docs/ACCEPT-REFILL.md` (wave21 **1** — prior tip; keep cites; leave primary untouched this tip)
- `docs/COUNT.md` (wave20 **3** — prior tip; keep cites)
- `docs/EXECUTE.md` (wave20 **4** — prior tip; keep cites)
- `docs/SOURCE-PAD.md` (wave19 **4** — prior tip; keep cites)
- `docs/CONTROL.md` (wave10 **2** — IF `taken=` companion; LSHIFT/RSHIFT is shift mark — not IF/THEN reopen / branch XT)
- `forth/trit.fs` / `forth/tritium/drena.fs` (host lowercase `lshift` / `rshift` — pack-header / unpack-header / phi-fold; **do not** redefine/alias/shadow/Lab-grep)
- `forth/tritium/kernel.fs` (lshift-mark / rshift-mark only — do not redefine host LSHIFT/RSHIFT; **do not** redefine host lowercase `lshift`/`rshift`; **do not** redefine and-mark/or-mark/xor-mark/invert-mark)
- ANS Forth `LSHIFT` / `RSHIFT` (shift marks only — not real boolean cell rewrite / `0=` deepen / AND-OR reopen / WITHIN reopen; host lowercase `lshift`/`rshift` ≠ this tip)
- Explicit deferral: WAVE19-PROPOSAL + WAVE20-PROPOSAL + WAVE21-PROPOSAL + WAVE22-PROPOSAL (`LSHIFT` / `RSHIFT` — shift marks; not boolean cell rewrite / `0=` / AND-OR reopen)
- Base tip: `0cf98da` / `0cf98dac548d57f6ee515433869926f533af428a` (#101 wave21 tip5 DOCS-CITES CLOSED)
- Wave22 proposal: `/workspace/tritium-research-docs/WAVE22-PROPOSAL.md`
