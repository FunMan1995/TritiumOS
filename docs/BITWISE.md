# BITWISE — `AND` / `OR` / `XOR` / `INVERT` bitwise marks + `bit-demo`

**Status:** Shipper-ready stub spec (wave21 item **4**; thin amend wave22 **1** LSHIFT-RSHIFT companion cite; thin amend wave22 **2** ZERO-EQUALS companion cite; thin amend wave23 **4** ABS-NEGATE companion cite)
**Canonical brief:** ANS-shaped `AND` / `OR` / `XOR` / `INVERT` (thin bitwise marks only); `docs/TRUE-FALSE.md` (wave20 **1** — TRUE/FALSE constant-picture companion; flag= / all-bits `-1` vs `0`); `docs/KERNEL.md` (wave7 **5**); `docs/CELL-CELLS.md` (wave15 **1** — cell-sized int picture companion); optional `docs/PICK-ROLL.md` (wave15 **2**) / `docs/WITHIN.md` (wave20 **2** — range-check companion; **not** WITHIN reopen) / `docs/HOST-PARITY.md` (wave8 **4**); `docs/LSHIFT-RSHIFT.md` (wave22 **1** — sibling shift marks beside BITWISE — **not** AND/OR/XOR/INVERT reopen / BITWISE reopen; **CRITICAL:** host lowercase `lshift`/`rshift` untouched); `docs/ZERO-EQUALS.md` (wave22 **2** — sibling flag marks beside BITWISE — **not** AND-OR reopen / BITWISE reopen / boolean cell; **CRITICAL:** host `0=`/`0<>` untouched; prefer `zero-eq-mark`); `docs/ABS-NEGATE.md` (wave23 **4** — sibling signed magnitude / negate marks beside BITWISE — **not** AND-OR reopen / BITWISE reopen / boolean cell; prefer `abs-mark`/`negate-mark`; NEGATE ≠ INVERT; do not redefine `and-mark`/`or-mark`/`xor-mark`/`invert-mark`); explicit WAVE19 / WAVE20 / WAVE21 deferral closed as bitwise marks only (not real boolean cell rewrite / LSHIFT/RSHIFT reopen-as-BITWISE / WITHIN reopen / real flag algebra; **critical:** host lowercase `and` / `or` are host Forth primitives — do **not** redefine).
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `bit.fs` / `bitwise.fs`); Linux host REPL; **do not** redefine host lowercase `and` / `or` (edition mask / interpret delimiters / other host uses); **do not** redefine host `AND` / `OR` / `XOR` / `INVERT` that already bind on the load path — prefer Forth mirrors
**Companions:** `docs/TRUE-FALSE.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip), `docs/CELL-CELLS.md` (thin amend this tip); optional light cite `docs/PICK-ROLL.md` / `docs/WITHIN.md` / `docs/HOST-PARITY.md`; `docs/LSHIFT-RSHIFT.md` (wave22 **1** — thin companion cite; sibling shift marks — **not** AND/OR/XOR/INVERT reopen / BITWISE reopen); `docs/ZERO-EQUALS.md` (wave22 **2** — thin companion cite; sibling flag marks — **not** AND-OR reopen / BITWISE reopen; host `0=`/`0<>` untouched); `docs/ABS-NEGATE.md` (wave23 **4** — thin companion cite; sibling signed magnitude / negate marks — **not** BITWISE reopen / boolean cell; prefer `abs-mark`/`negate-mark`; NEGATE ≠ INVERT; do not redefine `and-mark`/`or-mark`/`xor-mark`/`invert-mark`)
**Base tip SHA:** `7abca1b1` (wave21 tip3 PASS / #99 COMPARE) / full `7abca1b1ad4910a2dc5b7220a088ba3e4f150cde`

## 1. Purpose

WAVE15 landed cell-unit + stack/?DUP flag pictures (`docs/CELL-CELLS.md`, `docs/PICK-ROLL.md`); WAVE20 tip **1** landed `TRUE` / `FALSE` constant marks (`docs/TRUE-FALSE.md` — classic all-bits-set / `-1` vs `0` picture); WAVE20 tip **2** landed `WITHIN` range-check mark (`docs/WITHIN.md` — **not** bitwise). WAVE18 / WAVE19 / WAVE20 / WAVE21 explicitly deferred `AND` / `OR` / `XOR` / `INVERT` (bitwise marks beside TRUE/FALSE constant picture; **not** real boolean cell rewrite / `0=` deepen / LSHIFT/RSHIFT). This tip lands **stub** bitwise marks only: `AND` (or Forth mirror **`and-mark`**) prints `[bit] AND` (+ optional `u=` / `flag=` for a small fixed fixture — e.g. classic `0xFF AND` picture on stub ints); `OR` / **`or-mark`** prints `[bit] OR`; `XOR` / **`xor-mark`** prints `[bit] XOR`; `INVERT` / **`invert-mark`** prints `[bit] INVERT`. Smoke via **`bit-demo`**. Prefer Forth mirrors **`and-mark` / `or-mark` / `xor-mark` / `invert-mark`** whenever host `AND`/`OR`/`XOR`/`INVERT` collide — and **CRITICAL:** host kernel already uses lowercase **`and` / `or`** as host Forth primitives (e.g. edition mask / interpret delimiters) — do **NOT** redefine those. Pairs with wave20 TRUE/FALSE constant picture (`flag=` / all-bits `-1` vs `0`) **without** a boolean-cell rewrite and **without** reopening `0=`. **Not** LSHIFT/RSHIFT this tip, not WITHIN reopen (wave20 range-check stays mark-only), not real flag algebra, not COMPARE reopen, not BASE-HEX / ACCEPT-REFILL reopen. Closes a bitwise deferral as **thin mark** beside wave20 TRUE-FALSE without promoting either to a real boolean/flag machine. Independent of tip3 COMPARE. Wave22 tip **1** lands `LSHIFT` / `RSHIFT` shift marks (`docs/LSHIFT-RSHIFT.md`): sibling **shift** marks over fixed stub-int fixtures (classic `1 LSHIFT 4 → 16` / `16 RSHIFT 4 → 1` picture; optional `u=`) — **not** an AND/OR/XOR/INVERT reopen / BITWISE reopen / boolean-cell rewrite / WITHIN reopen; prefer Forth mirrors `lshift-mark` / `rshift-mark`; **CRITICAL:** host lowercase `lshift`/`rshift` in `trit.fs` / `drena.fs` stay untouched. Wave22 tip **2** lands `0=` (optional `0<>`) flag marks (`docs/ZERO-EQUALS.md`): sibling **flag** marks (classic zero→true / nonzero→false; optional `flag=`) — **not** an AND/OR/XOR/INVERT reopen / BITWISE reopen / boolean-cell rewrite / LSHIFT reopen / WITHIN reopen; prefer Forth mirror `zero-eq-mark` (optional `zero-ne-mark`); **CRITICAL:** host `0=`/`0<>` stay untouched. Wave23 tip **4** lands `ABS` / `NEGATE` signed magnitude / negate marks (`docs/ABS-NEGATE.md`): sibling **magnitude/negate** marks (classic `|n|` / `-n` pictures; optional `n=` / `u=`) — **not** an AND/OR/XOR/INVERT reopen / BITWISE reopen / boolean-cell rewrite / LSHIFT reopen / ZERO-EQUALS reopen; prefer Forth mirrors `abs-mark` / `negate-mark`; **do not** redefine `and-mark`/`or-mark`/`xor-mark`/`invert-mark`; NEGATE (two’s-complement) ≠ INVERT (ones-complement).

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `AND` / `and-mark` | `( x1 x2 -- x3 )` *or* `( -- )` with fixed demo fixture | Bitwise AND mark; print `[bit] AND` (+ optional `u=<n>` / `flag=<n>`); classic `0xFF AND 0x0F → 0x0F` picture welcome |
| `OR` / `or-mark` | `( x1 x2 -- x3 )` *or* `( -- )` with fixed demo fixture | Bitwise OR mark; print `[bit] OR` (+ optional `u=<n>` / `flag=<n>`); classic `0xFF OR 0x0F → 0xFF` picture welcome |
| `XOR` / `xor-mark` | `( x1 x2 -- x3 )` *or* `( -- )` with fixed demo fixture | Bitwise XOR mark; print `[bit] XOR` (+ optional `u=<n>` / `flag=<n>`); classic `0xFF XOR 0x0F → 0xF0` picture welcome |
| `INVERT` / `invert-mark` | `( x1 -- x2 )` *or* `( -- )` with fixed demo fixture | Bitwise INVERT / ones-complement mark; print `[bit] INVERT` (+ optional `u=<n>` / `flag=<n>`); classic `INVERT 0 → all-bits / -1` (pairs with TRUE) or `INVERT 0xFF` picture welcome |
| `bit-demo` | `( -- )` | See §5 |

Host note: bind bare `AND` / `OR` / `XOR` / `INVERT` on the Linux REPL **only if** those names do not collide with host Forth bitwise words in the same load path. Prefer Forth mirrors **`and-mark` / `or-mark` / `xor-mark` / `invert-mark`** as the Lab-facing surface when in doubt — **do not** redefine host `AND`/`OR`/`XOR`/`INVERT`. **CRITICAL:** host lowercase **`and` / `or`** (kernel.fs edition mask / interpret delimiters / other host uses) are **NOT** this tip’s surface and must **not** be redefined, aliased, shadowed, or Lab-grepped as bitwise success. Optional `u=` / `flag=` are host ints / fixture echo only — not a live boolean cell, not `0=` algebra, not LSHIFT/RSHIFT, not WITHIN. Prefer **fixed demo stub-int fixtures** (classic `0xFF` / `0x0F` / `0` picture) so Lab hit is deterministic and FAIL is avoided.

## 3. Stub semantics

- **`AND` / `and-mark`:** take (or use fixed demo) stub ints `( x1 x2 )`. Classic ANS / common Forth picture: bitwise AND → result `x3 = x1 & x2`. Print `[bit] AND` and optionally `u=<n>` and/or `flag=<n>` (either form Lab-greppable; prefer documenting which Shipper emits). Classic fixture welcome: `0xFF AND 0x0F → u=0x0F` (or decimal `u=15`). **Does not** rewrite TRUE/FALSE constants, deepen `0=` / flag algebra, bump HERE, open a heap, or create dict entries. Captured values are host ints / fixture echo only.
- **`OR` / `or-mark`:** bitwise OR → `x3 = x1 | x2`. Print `[bit] OR` (+ optional `u=` / `flag=`). Classic fixture welcome: `0xFF OR 0x0F → u=0xFF` (or `u=255`). Same constraints as AND — marker only.
- **`XOR` / `xor-mark`:** bitwise XOR → `x3 = x1 ^ x2`. Print `[bit] XOR` (+ optional `u=` / `flag=`). Classic fixture welcome: `0xFF XOR 0x0F → u=0xF0` (or `u=240`). Same constraints — marker only.
- **`INVERT` / `invert-mark`:** bitwise ones-complement → `x2 = ~x1`. Print `[bit] INVERT` (+ optional `u=` / `flag=`). Classic fixture welcome: `INVERT 0 → all-bits-set / -1` (pairs with wave20 TRUE picture) **or** `INVERT 0xFF` (document the pictured width — cell-width ones-complement is welcome; greppable `[bit] INVERT` is enough for Lab OK). Same constraints — marker only. **Does not** reopen TRUE/FALSE / `0=` / boolean cell.
- **Fixed demo stub-int fixtures (document):**
  - **Classic 0xFF picture (required set):** e.g. `a=0xFF` (255), `b=0x0F` (15), `z=0`. Demo must exercise **all four** marks (`AND` / `OR` / `XOR` / `INVERT`) so Lab greps `[bit] AND`, `[bit] OR`, `[bit] XOR`, and `[bit] INVERT`. Optional `u=` / `flag=` welcome on each.
  - **Optional result echo:** AND → `u=15` / `u=0x0F`; OR → `u=255` / `u=0xFF`; XOR → `u=240` / `u=0xF0`; INVERT 0 → `u=-1` / `flag=-1` (all-bits; pairs with TRUE) — document which form Shipper emits. Greppable markers alone are enough for Lab OK when all four `[bit] …` lines appear.
- **Optional push:** if the host stack is easy, push the pictured result; marker alone is enough for Lab OK — do not require a real boolean cell / flag algebra / LSHIFT/RSHIFT.
- **FAIL:** `[bit] FAIL reason=<…>` optional (demo **must avoid**). Prefer not emitting `[bit] FAIL` on the happy path.
- Storage: fixed demo stub ints / host int echo only. **No** real boolean cell rewrite, no `0=` deepen, no LSHIFT/RSHIFT, no WITHIN reopen, no COMPARE reopen, no HERE bump, no arena.
- **Host `and` / `or` stay untouched:** lowercase host `and` / `or` remain kernel Forth primitives (edition mask / interpret delimiters / etc.). This tip’s Lab greps are `[bit] AND` / `[bit] OR` / `[bit] XOR` / `[bit] INVERT` and `[bit-demo] OK` only — **never** Lab-grep bare host `and` / `or` as this tip’s bitwise success.
- Nest with prior compare / base / accept / exec / count / within / true / source / env / body / char / state / word / find / tick / create / allot / synonym / exit / buffer / marker / defer / imm / fill / pick / cell / throw / 2var / colon / control / string stubs OK. `dict-reset` unaffected (no new dict entries from bitwise marks — fixed host fixtures, not dictionary).
- LSHIFT/RSHIFT shift marks → `docs/LSHIFT-RSHIFT.md` (wave22 **1** — sibling shift marks; **not** an AND/OR/XOR/INVERT reopen / BITWISE reopen; host lowercase `lshift`/`rshift` stay untouched). `0=` / optional `0<>` flag marks → `docs/ZERO-EQUALS.md` (wave22 **2** — sibling flag marks; **not** AND-OR reopen / BITWISE reopen / boolean cell / LSHIFT reopen; host `0=`/`0<>` stay untouched; prefer `zero-eq-mark`). `ABS` / `NEGATE` signed magnitude / negate marks → `docs/ABS-NEGATE.md` (wave23 **4** — sibling magnitude/negate; **not** BITWISE reopen / boolean cell; prefer `abs-mark`/`negate-mark`; NEGATE ≠ INVERT; do not redefine and-mark/or-mark/xor-mark/invert-mark). Still no WITHIN reopen, no real flag algebra / boolean cell rewrite, no linked XT / real DOES> XT / real branch XT / full arena/heap / full Win/Android Forth VM. Wave21 tip1 ACCEPT-REFILL + tip2 BASE-HEX + tip3 COMPARE + wave20 tip1–4 TRUE-FALSE / WITHIN / COUNT / EXECUTE + wave19 tip1–4 stay landed — keep cites; this tip does not reopen them. Those stay non-goals / later tips (BITWISE marks excepted as this tip; LSHIFT/RSHIFT excepted as wave22 tip1 sibling).

## 4. Markers

```
[bit] AND [u=<n>] [flag=<n>]       # u=/flag= optional; classic 0xFF AND 0x0F → u=0x0F welcome
[bit] OR [u=<n>] [flag=<n>]        # u=/flag= optional; classic 0xFF OR 0x0F → u=0xFF welcome
[bit] XOR [u=<n>] [flag=<n>]       # u=/flag= optional; classic 0xFF XOR 0x0F → u=0xF0 welcome
[bit] INVERT [u=<n>] [flag=<n>]    # u=/flag= optional; classic INVERT 0 → all-bits / -1 welcome
[bit] FAIL reason=<…>              # demo avoids
[bit-demo] OK
[bit-demo] FAIL
```

Lab greps `[bit-demo] OK` plus greppable **`[bit] AND`**, **`[bit] OR`**, **`[bit] XOR`**, and **`[bit] INVERT`** (optional `u=` / `flag=` welcome on each). Demo avoids `[bit] FAIL`. Prefer not emitting `[bit] FAIL` on the happy path. **Do not** Lab-grep host lowercase `and` / `or` as this tip’s surface. **Do not** Lab-grep `[true]` / `[within]` / `[compare]` / `[base]` as BITWISE success (those stay their own tips).

## 5. `bit-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; bitwise marks need no dict entries.
2. Ensure **fixed demo stub-int fixtures** exist (e.g. `a=0xFF`, `b=0x0F`, `z=0` — or equivalent host ints) so the classic bitwise picture is deterministic. Fixtures may live beside (not replacing) TRUE/FALSE constant picture / CELL unit picture / PICK stack picture / WITHIN range-check — document; do **not** require boolean-cell rewrite / `0=` deepen / LSHIFT/RSHIFT / WITHIN reopen / COMPARE reopen.
3. Invoke `AND` (or **`and-mark`**) against the classic pair → `[bit] AND` (+ optional `u=` / `flag=` — classic `0x0F` / `15` welcome).
4. Invoke `OR` (or **`or-mark`**) → `[bit] OR` (+ optional `u=` / `flag=` — classic `0xFF` / `255` welcome).
5. Invoke `XOR` (or **`xor-mark`**) → `[bit] XOR` (+ optional `u=` / `flag=` — classic `0xF0` / `240` welcome).
6. Invoke `INVERT` (or **`invert-mark`**) against `0` (or documented fixture) → `[bit] INVERT` (+ optional `u=` / `flag=` — classic all-bits / `-1` welcome; pairs with TRUE picture without reopening TRUE-FALSE / `0=`).
7. Assert no `[bit] FAIL` on the happy path. Assert bitwise marks did **not** require a real boolean cell rewrite / `0=` deepen / LSHIFT/RSHIFT / WITHIN reopen / COMPARE reopen / HERE bump / host `and`/`or` redefine (marker-only is enough). Assert host `AND`/`OR`/`XOR`/`INVERT` were not redefined when using the Forth mirrors. Assert host lowercase `and` / `or` were **not** redefined, aliased, shadowed, or used as this tip’s Lab surface.
8. Prior `compare-demo` / `base-demo` / `accept-demo` / `exec-demo` / `count-demo` / `within-demo` / `true-demo` / `source-demo` / `env-demo` / `body-demo` / `char-demo` / `state-demo` / `word-demo` / `find-demo` / `tick-demo` / `recurse-demo` / `eval-demo` / `parse-demo` / `synonym-demo` / `exit-demo` / `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` / `refined-boot-demo` / `host-boot-demo` / `interpret-demo` / `kernel-demo` still OK.
9. `[bit-demo] OK`.

All four markers (`[bit] AND` / `[bit] OR` / `[bit] XOR` / `[bit] INVERT`) are required. Optional `u=` / `flag=` echo is not required for Lab OK when all four markers are greppable. No real boolean cell. No `0=` deepen. No LSHIFT/RSHIFT. No WITHIN reopen. No host `and`/`or` redefine.

## 6. Thin amend — companions

### `docs/TRUE-FALSE.md`

- Companions / Status: add `BITWISE.md` (wave21 **4** companion cite); **keep** WITHIN / BASE-HEX / KERNEL / CELL-CELLS / PICK-ROLL / CONTROL / HOST-PARITY cites — do not wipe wave20 TRUE-FALSE content.
- Purpose / §3 / non-goals: TRUE/FALSE stay constant marks (`flag=` / all-bits `-1` vs `0`); `AND` / `OR` / `XOR` / `INVERT` are sibling **bitwise marks** over fixed stub-int fixtures — **not** a TRUE/FALSE reopen / boolean-cell rewrite / `0=` deepen / LSHIFT/RSHIFT / WITHIN reopen. Do not wipe wave20 TRUE-FALSE content. Stress: pairs with TRUE/FALSE constant picture **without** boolean-cell rewrite and **without** reopening `0=`; host lowercase `and`/`or` stay untouched.
- Non-goals: `AND` / `OR` / `XOR` / `INVERT` → `docs/BITWISE.md` (wave21 **4**). TRUE/FALSE stay on this tip (already landed). WITHIN stays wave20 **2**. BASE-HEX stays wave21 **2**. LSHIFT/RSHIFT still later.
- Acceptance: Lab smokes `bit-demo` (retains `true-demo` + `within-demo` + `base-demo` + `compare-demo`).
- Cite: `docs/BITWISE.md`.

### `docs/KERNEL.md`

- Companions: add `BITWISE.md` (wave21 **4**); **keep** wave21 tip1 ACCEPT-REFILL + tip2 BASE-HEX + tip3 COMPARE cites and wave20 tip1–4 TRUE-FALSE / WITHIN / COUNT / EXECUTE cites and wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites and wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites and wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `AND` / `and-mark`, `OR` / `or-mark`, `XOR` / `xor-mark`, `INVERT` / `invert-mark` stubs + `bit-demo` (cite tip; Forth mirrors `and-mark` / `or-mark` / `xor-mark` / `invert-mark` — bitwise marks only; **CRITICAL:** do **not** redefine host lowercase `and`/`or`; **do not** redefine host `AND`/`OR`/`XOR`/`INVERT`; **not** real boolean cell rewrite / `0=` deepen / LSHIFT/RSHIFT / WITHIN reopen; optional `u=` / `flag=` — classic `0xFF` AND/OR/XOR/INVERT picture on stub ints welcome).
- Non-goals: `AND` / `OR` / `XOR` / `INVERT` bitwise marks → `docs/BITWISE.md`. COMPARE stays on `COMPARE.md`. BASE/HEX/DECIMAL stay on `BASE-HEX.md`. ACCEPT/REFILL stay on `ACCEPT-REFILL.md`. TRUE/FALSE stay on `TRUE-FALSE.md`. WITHIN stays on `WITHIN.md`. host lowercase `and`/`or` stay kernel Forth primitives — **not** this tip. Tip5 DOCS-CITES still later (wave21 **5**). LSHIFT/RSHIFT still later.
- Acceptance: Lab smokes `bit-demo` (and retains `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `source-demo` + `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `string-demo` + `fill-demo` + prior demos).
- Cite: `docs/BITWISE.md`.

### `docs/CELL-CELLS.md`

- Companions / Status: add `BITWISE.md` (wave21 **4** companion cite); **keep** TRUE-FALSE / WITHIN / BASE-HEX / CHAR-CHARS / ENVIRONMENT-QUERY / ALLOT-HERE / KERNEL / VARIABLE-CONST cites — do not wipe wave15 CELL content.
- Purpose / §3 / non-goals: cell-unit stubs stay; `AND` / `OR` / `XOR` / `INVERT` may echo optional stub `u=` / `flag=` that picture **cell-sized** bitwise results on stub ints — **not** a CELL/ALIGN reopen / cell-size rewrite / real boolean cell / `0=` deepen. Do not wipe wave15 / wave19 / wave20 / wave21 tip2 CELL/CHAR/ENV/TRUE/BASE content. Stress: bitwise marks on cell-sized ints — not CELL reopen; host lowercase `and`/`or` stay untouched.
- Non-goals: `AND` / `OR` / `XOR` / `INVERT` → `docs/BITWISE.md` (wave21 **4**). CELL/CELLS/ALIGN/ALIGNED stay on this tip (already landed). TRUE/FALSE stay wave20 **1**. BASE-HEX stays wave21 **2**. LSHIFT/RSHIFT still later.
- Acceptance: Lab smokes `bit-demo` (retains `cell-demo` + `true-demo` + `within-demo` + `base-demo`).
- Cite: `docs/BITWISE.md`.

### Optional — `docs/PICK-ROLL.md`

- Companions / Status: add light `BITWISE.md` (wave21 **4**) cite; **keep** TRUE-FALSE / WITHIN / KERNEL / CELL-CELLS cites — do not wipe wave15 PICK-ROLL content.
- Purpose / §3 / non-goals: stack/?DUP flag pictures stay; `AND` / `OR` / `XOR` / `INVERT` are sibling **bitwise** marks — **not** a PICK/ROLL/?DUP reopen / real threaded stack / `0=` deepen / boolean cell / WITHIN reopen. Do not wipe wave15 PICK-ROLL content. Host `2dup`/`2drop`/`2swap` stay untouched. Host lowercase `and`/`or` stay untouched.
- Non-goals: `AND` / `OR` / `XOR` / `INVERT` → `docs/BITWISE.md` (wave21 **4**). PICK/ROLL/DEPTH/?DUP stay on this tip (already landed). TRUE/FALSE stay wave20 **1**. WITHIN stays wave20 **2**.
- Acceptance: Lab smokes `bit-demo` (retains `pick-demo` + `true-demo` + `within-demo`).
- Cite: `docs/BITWISE.md`.

### Optional — `docs/WITHIN.md`

- Companions / Status: add light `BITWISE.md` (wave21 **4**) cite; **keep** COMPARE / TRUE-FALSE / KERNEL / CONTROL / PICK-ROLL / CELL-CELLS cites — do not wipe wave20 WITHIN content.
- Purpose / §3 / non-goals: WITHIN stays range-check mark (`lo ≤ n < hi`); `AND` / `OR` / `XOR` / `INVERT` are sibling **bitwise** marks — **not** a WITHIN reopen / runtime compare re-exec / IF/THEN reopen / boolean cell / `0=` deepen. Do not wipe wave20 WITHIN content. Stress: WITHIN stays mark-only range-check — **not** reopen; host lowercase `and`/`or` stay untouched; WITHIN and BITWISE are different surfaces (range-check vs bitwise).
- Non-goals: `AND` / `OR` / `XOR` / `INVERT` → `docs/BITWISE.md` (wave21 **4**). WITHIN stays on this tip (already landed). COMPARE stays wave21 **3**. LSHIFT/RSHIFT still later.
- Acceptance: Lab smokes `bit-demo` (retains `within-demo` + `true-demo` + `compare-demo`).
- Cite: `docs/BITWISE.md`.

### Optional — `docs/HOST-PARITY.md`

- Companions: add light `BITWISE.md` (wave21 **4**) cite; **keep** TRUE-FALSE / BASE-HEX / ENVIRONMENT-QUERY / KERNEL / BUILD / INSTALL / INTERPRET cites.
- Purpose / non-goals: Win/Android stay CONTRACT-parity; `bit-demo` CONTRACT line is acceptable — **not** a full Forth VM / boolean-cell / bitwise-ALU port. Do not wipe wave8 HOST-PARITY content. Stress: host lowercase `and`/`or` stay untouched on Linux SoT.
- Non-goals: `AND` / `OR` / `XOR` / `INVERT` → `docs/BITWISE.md` (wave21 **4**). Full Win/Android Forth VM still out. TRUE/FALSE stay wave20 **1**. BASE-HEX stays wave21 **2**.
- Acceptance: Lab smokes `bit-demo` (Win/Android: `bit-demo CONTRACT` OK).
- Cite: `docs/BITWISE.md`.

### `docs/ABS-NEGATE.md` (wave23 **4** thin companion cite)

- Companions / Status: add light `ABS-NEGATE.md` (wave23 **4**) cite; **keep** TRUE-FALSE / LSHIFT-RSHIFT / ZERO-EQUALS / KERNEL / CELL-CELLS / PICK-ROLL / WITHIN / HOST-PARITY cites — do not wipe wave21 BITWISE content.
- Purpose: `ABS` / `NEGATE` are sibling **signed magnitude / negate marks** beside BITWISE (classic `|n|` / `-n`; optional `n=` / `u=`) — **not** an AND/OR/XOR/INVERT reopen / BITWISE reopen / boolean-cell rewrite / LSHIFT reopen / ZERO-EQUALS reopen. Prefer `abs-mark`/`negate-mark`; **do not** redefine `and-mark`/`or-mark`/`xor-mark`/`invert-mark`; NEGATE ≠ INVERT; host lowercase `and`/`or` stay untouched.
- Non-goals: `ABS` / `NEGATE` → `docs/ABS-NEGATE.md` (wave23 **4**). AND/OR/XOR/INVERT stay on this tip (already landed). LSHIFT-RSHIFT stays wave22 **1**. ZERO-EQUALS stays wave22 **2**. Tip5 DOCS-CITES still later (wave23 **5**).
- Acceptance: Lab smokes `abs-demo` (retains `bit-demo` + `shift-demo` + `true-demo` + `zero-demo` + `uless-demo` + `hold-demo` + `charplus-demo`).
- Cite: `docs/ABS-NEGATE.md`.

Do **not** wipe wave21 tip1 ACCEPT-REFILL content or tip2 BASE-HEX content or tip3 COMPARE content or wave20 tip1–4 TRUE-FALSE / WITHIN / COUNT / EXECUTE content or wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites or wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites or wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites or wave16 DEFER / MARKER / BUFFER / EXIT cites or wave15 CELL / IMMEDIATE / FILL / PICK prior content. Do **not** amend ARCHITECTURE / IMPLEMENTATION-GAPS / COMPARE / BASE-HEX / ACCEPT-REFILL primary this tip (proposal amends are TRUE-FALSE + KERNEL + CELL-CELLS + optional PICK-ROLL / WITHIN / HOST-PARITY only). **Leave `COMPARE.md` primary untouched** (tip3 land md5 `dc429e1f010818957353c216adb5c317` / 23322). **Leave `BASE-HEX.md` primary untouched** (tip2 land md5 `d12dd22a8502b321568fa370b6ecdd2e` / 21270). **Leave `ACCEPT-REFILL.md` primary untouched** (tip1 land md5 `6bda5a5170ac71fa3ccf7db28286dd44` / 23948). **Leave `ARCHITECTURE.md` and `IMPLEMENTATION-GAPS.md` untouched.** Tip5 cites come after 1–4 PASS.

## 7. Non-goals

- Redefining / aliasing / shadowing / Lab-grepping host lowercase `and` / `or` as this tip’s bitwise surface
- Real boolean cell rewrite / live flag storage / VARIABLE-as-boolean
- `0=` / optional `0<>` flag marks → `docs/ZERO-EQUALS.md` (wave22 **2**; sibling flag marks — **not** AND-OR reopen / BITWISE reopen / boolean cell / LSHIFT reopen; host `0=`/`0<>` stay untouched; prefer `zero-eq-mark`)
- `ABS` / `NEGATE` signed magnitude / negate marks → `docs/ABS-NEGATE.md` (wave23 **4**; sibling magnitude/negate — **not** BITWISE reopen / boolean cell; prefer `abs-mark`/`negate-mark`; NEGATE ≠ INVERT; do not redefine and-mark/or-mark/xor-mark/invert-mark)
- `LSHIFT` / `RSHIFT` shift marks → `docs/LSHIFT-RSHIFT.md` (wave22 **1**; sibling shift marks — **not** an AND/OR/XOR/INVERT reopen / BITWISE reopen; host lowercase `lshift`/`rshift` stay untouched; leave LSHIFT-RSHIFT.md primary untouched)
- `WITHIN` reopen (wave20 **2** — already stubbed; keep cites; optional thin companion cite only — **not** runtime compare re-exec)
- `TRUE` / `FALSE` reopen (wave20 **1** — already stubbed; keep cites; thin companion amend only — pairs without boolean-cell rewrite)
- `COMPARE` reopen (wave21 **3** — already stubbed; leave COMPARE.md primary untouched; host `cstr=` ≠ ANS COMPARE)
- `BASE` / `HEX` / `DECIMAL` reopen (wave21 **2** — already stubbed; leave BASE-HEX.md primary untouched)
- `ACCEPT` / `REFILL` reopen (wave21 **1** — already stubbed; leave ACCEPT-REFILL.md primary untouched)
- Docs cites pass (wave21 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `COUNT` reopen (wave20 **3** — already stubbed; keep cites)
- `EXECUTE` reopen (wave20 **4** — already stubbed; keep cites)
- `SOURCE` / `PAD` reopen (wave19 **4** — already stubbed; keep cites)
- `CELL` / `CELLS` / `ALIGN` / `ALIGNED` reopen (wave15 — already stubbed; keep cites; bitwise marks on cell-sized ints — not CELL reopen)
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

1. `docs/BITWISE.md` present (Research byte-copy OK); `TRUE-FALSE.md` + `KERNEL.md` + `CELL-CELLS.md` thin amends present (+ optional `PICK-ROLL.md` / `WITHIN.md` / `HOST-PARITY.md`); wave21 tip1 ACCEPT-REFILL + tip2 BASE-HEX + tip3 COMPARE cites, wave20 tip1–4 TRUE-FALSE / WITHIN / COUNT / EXECUTE cites, wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites, wave18 tip1–4 cites, wave17 tip1–4 cites, wave16 DEFER/MARKER/BUFFER/EXIT cites, and wave15 CELL/IMMEDIATE/COLON/KERNEL prior text retained; host `AND`/`OR`/`XOR`/`INVERT` untouched via mirrors; host lowercase `and`/`or` **not** redefined / aliased / shadowed / Lab-grepped as bitwise success; `COMPARE.md` primary untouched (tip3 land md5 `dc429e1f010818957353c216adb5c317` / 23322); `BASE-HEX.md` primary untouched (tip2 land md5 `d12dd22a8502b321568fa370b6ecdd2e` / 21270); `ACCEPT-REFILL.md` primary untouched (tip1 land md5 `6bda5a5170ac71fa3ccf7db28286dd44` / 23948); `ARCHITECTURE.md` + `IMPLEMENTATION-GAPS.md` byte-copy unchanged.
2. `bit-demo` → OK (markers §4; `[bit] AND` / `[bit] OR` / `[bit] XOR` / `[bit] INVERT` greppable; optional `u=` / `flag=` welcome — classic `0xFF` AND/OR/XOR/INVERT picture on stub ints; no FAIL on happy path; no real boolean cell / `0=` deepen / LSHIFT/RSHIFT / WITHIN reopen / COMPARE reopen; no host `and`/`or` redefine). Prior `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `source-demo` + `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `recurse-demo` + `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` + `refined-boot-demo` + `host-boot-demo` + `interpret-demo` + `kernel-demo` still OK.
3. Regression green (wave21 tip1–3 + wave20 tip1–5 + wave19 tip1–5 + wave18 tip1–5 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `bit-demo CONTRACT` OK). Wave22 **1**: `shift-demo` → OK (retains `bit-demo` + `true-demo` + `within-demo` + `base-demo` + `compare-demo`; sibling shift marks — **not** BITWISE reopen; host lowercase `lshift`/`rshift` untouched). Wave22 **2**: `zero-demo` → OK (retains `bit-demo` + `shift-demo` + `true-demo` + `within-demo` + `base-demo` + `compare-demo`; sibling flag marks — **NOT** AND-OR reopen / BITWISE reopen; host `0=`/`0<>` untouched; prefer `zero-eq-mark`). Wave23 **4**: `abs-demo` → OK (retains `bit-demo` + `shift-demo` + `true-demo` + `zero-demo` + `uless-demo` + `hold-demo` + `charplus-demo`; sibling magnitude/negate — **NOT** BITWISE reopen; prefer `abs-mark`/`negate-mark`; NEGATE ≠ INVERT; do not redefine and-mark/or-mark/xor-mark/invert-mark).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**)
- `docs/TRUE-FALSE.md` (wave20 **1** — TRUE/FALSE constant-picture companion; BITWISE pairs with `flag=` / all-bits `-1` vs `0` **without** boolean-cell rewrite / `0=` reopen)
- `docs/CELL-CELLS.md` (wave15 **1** — unit/cell picture companion; bitwise marks on cell-sized ints — not CELL reopen)
- `docs/PICK-ROLL.md` (wave15 **2**, optional — stack/?DUP flag companion; not PICK/?DUP reopen)
- `docs/WITHIN.md` (wave20 **2**, optional — range-check companion; **not** WITHIN reopen / runtime compare re-exec)
- `docs/HOST-PARITY.md` (wave8 **4**, optional — `bit-demo` CONTRACT parity welcome)
- `docs/COMPARE.md` (wave21 **3** — prior tip; keep cites; leave primary untouched this tip; host `cstr=` ≠ ANS COMPARE)
- `docs/BASE-HEX.md` (wave21 **2** — prior tip; keep cites; leave primary untouched this tip)
- `docs/ACCEPT-REFILL.md` (wave21 **1** — prior tip; keep cites; leave primary untouched this tip)
- `docs/COUNT.md` (wave20 **3** — prior tip; keep cites)
- `docs/EXECUTE.md` (wave20 **4** — prior tip; keep cites)
- `docs/SOURCE-PAD.md` (wave19 **4** — prior tip; keep cites)
- `docs/CONTROL.md` (wave10 **2** — IF `taken=` companion; BITWISE is bitwise mark — not IF/THEN reopen / branch XT)
- `forth/tritium/kernel.fs` (and-mark / or-mark / xor-mark / invert-mark only — do not redefine host AND/OR/XOR/INVERT; **do not** redefine/alias/shadow/Lab-grep host lowercase `and`/`or`)
- ANS Forth `AND` / `OR` / `XOR` / `INVERT` (bitwise marks only — not real boolean cell rewrite / `0=` deepen / LSHIFT/RSHIFT / WITHIN reopen; host lowercase `and`/`or` ≠ this tip)
- Explicit deferral: WAVE19-PROPOSAL + WAVE20-PROPOSAL + WAVE21-PROPOSAL (`AND` / `OR` / `XOR` / `INVERT` — bitwise marks; not boolean cell rewrite / `0=` / LSHIFT/RSHIFT)
- `docs/LSHIFT-RSHIFT.md` (wave22 **1** — sibling shift marks beside BITWISE — **not** AND/OR/XOR/INVERT reopen / BITWISE reopen; host lowercase `lshift`/`rshift` stay untouched)
- `docs/ZERO-EQUALS.md` (wave22 **2** — sibling flag marks beside BITWISE — **not** AND-OR reopen / BITWISE reopen / boolean cell; host `0=`/`0<>` untouched; prefer `zero-eq-mark`)
- `docs/ABS-NEGATE.md` (wave23 **4** — sibling signed magnitude / negate marks beside BITWISE — **not** BITWISE reopen / boolean cell; prefer `abs-mark`/`negate-mark`; NEGATE ≠ INVERT; do not redefine and-mark/or-mark/xor-mark/invert-mark)
- Base tip: `7abca1b1` / `7abca1b1ad4910a2dc5b7220a088ba3e4f150cde` (#99 wave21 tip3 COMPARE PASS)
- Wave21 proposal: `/workspace/tritium-research-docs/WAVE21-PROPOSAL.md`
