# TRUE-FALSE — `TRUE` / `FALSE` constant marks + `true-demo`

**Status:** Shipper-ready stub spec (wave20 item **1**; thin amend wave20 **2** WITHIN companion cite; thin amend wave21 **2** BASE-HEX companion cite; thin amend wave21 **4** BITWISE companion cite; thin amend wave22 **1** LSHIFT-RSHIFT companion cite; thin amend wave22 **2** ZERO-EQUALS companion cite; thin amend wave23 **3** U-LESS companion cite; thin amend wave23 **4** ABS-NEGATE companion cite)
**Canonical brief:** ANS-shaped `TRUE` / `FALSE` (thin constant marks only); `docs/KERNEL.md` (wave7 **5**); `docs/CELL-CELLS.md` (wave15 **1** — unit/cell picture companion for optional `u=` / flag echo); `docs/PICK-ROLL.md` (wave15 **2** — stack/?DUP flag-picture companion); optional `docs/CONTROL.md` (wave10 **2**) / `docs/HOST-PARITY.md` (wave8 **4**); `docs/WITHIN.md` (wave20 **2** — range-check mark companion; pairs with this tip's flag picture — not boolean cell / branch XT); explicit WAVE18 / WAVE19 / WAVE20 deferral closed as constant marks only (not real boolean cell rewrite / `0=` / flag algebra deepen / WITHIN); `docs/BITWISE.md` (wave21 **4** — sibling bitwise marks beside TRUE/FALSE constants — NOT boolean-cell rewrite / `0=` reopen / TRUE-FALSE reopen); `docs/LSHIFT-RSHIFT.md` (wave22 **1** — sibling shift marks beside constants — NOT boolean cell / `0=` deepen / TRUE-FALSE reopen / BITWISE reopen); `docs/ZERO-EQUALS.md` (wave22 **2** — sibling flag mark `0=` / optional `0<>` beside constants — NOT boolean cell / TRUE-FALSE reopen; **CRITICAL:** host `0=`/`0<>` untouched; prefer `zero-eq-mark`); `docs/U-LESS.md` (wave23 **3** — sibling unsigned compare flag mark — NOT boolean cell / TRUE-FALSE reopen; prefer `u-less-mark`); `docs/ABS-NEGATE.md` (wave23 **4** — sibling signed magnitude / negate marks — NOT boolean cell / TRUE-FALSE reopen; prefer `abs-mark`/`negate-mark`; do not redefine `true-mark`/`false-mark`)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `true.fs` / `true-false.fs`); Linux host REPL; **do not** redefine host `TRUE` / `FALSE` that already bind on the load path
**Companions:** `docs/KERNEL.md` (thin amend this tip), `docs/CELL-CELLS.md` (thin amend this tip), `docs/PICK-ROLL.md` (thin amend this tip); optional light cite `docs/CONTROL.md` / `docs/HOST-PARITY.md`; `docs/WITHIN.md` (wave20 **2** — thin companion cite from tip2); `docs/BASE-HEX.md` (wave21 **2** — thin companion cite; radix marks — not boolean cell / `0=` / BITWISE reopen); `docs/BITWISE.md` (wave21 **4** — thin companion cite; sibling bitwise marks beside TRUE/FALSE constants — NOT boolean-cell rewrite / `0=` reopen / TRUE-FALSE reopen); `docs/LSHIFT-RSHIFT.md` (wave22 **1** — thin companion cite; sibling shift marks beside constants — NOT boolean cell / `0=` deepen / TRUE-FALSE reopen); `docs/ZERO-EQUALS.md` (wave22 **2** — thin companion cite; sibling flag mark — NOT boolean cell / TRUE-FALSE reopen; host `0=`/`0<>` untouched); `docs/U-LESS.md` (wave23 **3** — thin companion cite; sibling unsigned compare flag mark — NOT boolean cell / TRUE-FALSE reopen / ZERO-EQUALS reopen / WITHIN reopen; prefer `u-less-mark`; do not redefine `true-mark`/`false-mark`); `docs/ABS-NEGATE.md` (wave23 **4** — thin companion cite; sibling signed magnitude / negate marks — NOT boolean cell / TRUE-FALSE reopen / BITWISE reopen / LSHIFT reopen; prefer `abs-mark`/`negate-mark`; do not redefine `true-mark`/`false-mark`)
**Base tip SHA:** `e93c44a` (wave19 tip5 CLOSED / #91 DOCS-CITES) / full `e93c44af21bb3310b3fc5a7351ec2f257b50f2bb`

## 1. Purpose

WAVE12 landed named-cell stubs (`docs/VARIABLE-CONST.md`); WAVE15 landed cell-unit + stack/?DUP flag pictures (`docs/CELL-CELLS.md`, `docs/PICK-ROLL.md`); WAVE18 / WAVE19 / WAVE20 explicitly deferred `TRUE` / `FALSE` (constant marks; not real boolean cell rewrite). This tip lands **stub** constant marks only: `TRUE` (or Forth mirror `true-mark`) prints `[true] TRUE` (+ optional `flag=` / `u=` — classic all-bits-set / `-1` picture welcome); `FALSE` (or Forth mirror `false-mark`) prints `[true] FALSE` (+ optional `flag=` / `u=` — classic `0` picture). Smoke via **`true-demo`**. Forth mirrors **`true-mark` / `false-mark`** so host `TRUE`/`FALSE` stay safe (or a single shared `true-false` helper if Shipper prefers — document). **Not** a real boolean cell / flag algebra rewrite, not `0=` reopen, not WITHIN this tip. Thinnest constant surface deferred since wave18/19 GAPS — flag picture before wave20 tip2 WITHIN / tip3 COUNT / tip4 EXECUTE. Pairs with cell/stack flag pictures without promoting either to a real compare/branch machine. Wave20 tip **2** lands `WITHIN` range-check mark (`docs/WITHIN.md`): sibling range-check that may echo optional `flag=<0|1>` — **not** a TRUE/FALSE reopen / boolean cell rewrite / `0=` deepen / branch XT. Wave21 tip **2** lands `BASE` / `HEX` / `DECIMAL` base marks (`docs/BASE-HEX.md`): sibling **radix** marks — **not** a TRUE/FALSE reopen / boolean cell rewrite / `0=` deepen / BITWISE reopen / number parser. Wave21 tip **4** lands `AND` / `OR` / `XOR` / `INVERT` bitwise marks (`docs/BITWISE.md`): sibling **bitwise** marks over fixed stub-int fixtures (classic `0xFF` AND/OR/XOR/INVERT picture) — **not** a TRUE/FALSE reopen / boolean-cell rewrite / `0=` reopen / LSHIFT/RSHIFT / WITHIN reopen; pairs with this tip's `flag=` / all-bits `-1` vs `0` picture without promoting either; host lowercase `and`/`or` stay untouched. Wave22 tip **1** lands `LSHIFT` / `RSHIFT` shift marks (`docs/LSHIFT-RSHIFT.md`): sibling **shift** marks beside constants (classic `1 LSHIFT 4 → 16` / `16 RSHIFT 4 → 1` picture; optional `u=`) — **not** a TRUE/FALSE reopen / boolean-cell rewrite / `0=` deepen / AND-OR reopen / BITWISE reopen / WITHIN reopen; prefer mirrors `lshift-mark` / `rshift-mark`; **CRITICAL:** host lowercase `lshift`/`rshift` stay untouched. Wave22 tip **2** lands `0=` (optional `0<>`) flag marks (`docs/ZERO-EQUALS.md`): sibling **flag** marks beside constants (classic zero→true / nonzero→false; optional `flag=`) — **not** a TRUE/FALSE reopen / boolean-cell rewrite / LSHIFT reopen / AND-OR reopen / BITWISE reopen / WITHIN reopen; prefer Forth mirror `zero-eq-mark` (optional `zero-ne-mark`); **CRITICAL:** host `0=`/`0<>` stay untouched. Wave23 tip **3** lands `U<` unsigned compare flag mark (`docs/U-LESS.md`): sibling **unsigned compare** beside constants (classic u1 <u u2 → true; optional `flag=`) — **not** a TRUE/FALSE reopen / boolean-cell rewrite / ZERO-EQUALS reopen / WITHIN reopen / COMPARE reopen; prefer Forth mirror `u-less-mark`; **do not** redefine `true-mark`/`false-mark`. Wave23 tip **4** lands `ABS` / `NEGATE` signed magnitude / negate marks (`docs/ABS-NEGATE.md`): sibling **magnitude/negate** beside constants (classic `|n|` / `-n`; optional `n=` / `u=`) — **not** a TRUE/FALSE reopen / boolean-cell rewrite / BITWISE reopen / LSHIFT reopen / ZERO-EQUALS reopen / U-LESS reopen; prefer Forth mirrors `abs-mark` / `negate-mark`; **do not** redefine `true-mark`/`false-mark`.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `TRUE` / `true-mark` | `( -- flag )` *or* `( -- )` | Constant mark; print `[true] TRUE` (+ optional `flag=<n>` / `u=<n>`); classic all-bits-set / `-1` picture welcome |
| `FALSE` / `false-mark` | `( -- flag )` *or* `( -- )` | Constant mark; print `[true] FALSE` (+ optional `flag=<n>` / `u=<n>`); classic `0` picture |
| `true-demo` | `( -- )` | See §5 |

Host note: bind `TRUE` / `FALSE` on the Linux REPL **only if** those names do not collide with host Forth boolean constants in the same load path. Prefer Forth mirrors **`true-mark` / `false-mark`** as the Lab-facing surface when in doubt — **do not** redefine host `TRUE`/`FALSE`. Optional single shared helper `true-false` (document if Shipper prefers one word that takes a side) is acceptable — Lab still greps `[true] TRUE` and `[true] FALSE` markers. Optional `flag=` / `u=` are host ints only — not a live boolean cell, not `0=` algebra, not WITHIN. Prefer printing both TRUE and FALSE markers in the demo so Lab hit is deterministic.

## 3. Stub semantics

- **`TRUE` / `true-mark`:** print `[true] TRUE` and optionally `flag=<n>` and/or `u=<n>`. Classic ANS / common Forth picture: **all-bits-set** / **`-1`** (document if Shipper echoes `flag=1` / `u=1` as a thinner presence mark — greppable `[true] TRUE` is enough for Lab OK; `-1` picture is welcome). **Does not** allocate a boolean cell, rewrite VARIABLE/CONSTANT, deepen `0=` / flag algebra, bump HERE, or execute branch XT. Captured values are host ints / flag echo only.
- **`FALSE` / `false-mark`:** print `[true] FALSE` and optionally `flag=<n>` and/or `u=<n>`. Classic picture: **`0`**. Same constraints as TRUE — marker only.
- **Optional push:** if the host stack is easy, push the pictured value (`-1` / `0` or `1` / `0`); marker alone is enough for Lab OK — do not require a real data-stack boolean cell.
- **FAIL:** `[true] FAIL reason=<…>` optional (demo **must avoid**). Prefer not emitting `[true] FAIL` on the happy path.
- Storage: host constant ints / flag echo only. **No** real boolean cell rewrite, no `0=` deepen, no WITHIN (wave20 **2**), no COMPARE/branch XT, no HERE bump, no arena. Bitwise marks → `docs/BITWISE.md` (wave21 **4** — sibling bitwise marks; not a TRUE/FALSE reopen / boolean cell / `0=`).
- Nest with prior source / env / body / char / state / word / find / tick / create / allot / synonym / exit / buffer / marker / defer / imm / fill / pick / cell / throw / 2var / colon / control / string stubs OK. `dict-reset` unaffected (no new dict entries from constant marks — fixed host constants, not dictionary).
- WITHIN range-check mark → `docs/WITHIN.md` (wave20 **2** — sibling range-check; not a TRUE/FALSE reopen / branch XT). AND/OR/XOR/INVERT bitwise marks → `docs/BITWISE.md` (wave21 **4** — sibling bitwise marks; not a TRUE/FALSE reopen / boolean-cell rewrite / `0=` reopen; host lowercase `and`/`or` stay untouched). LSHIFT/RSHIFT shift marks → `docs/LSHIFT-RSHIFT.md` (wave22 **1** — sibling shift marks beside constants; **not** boolean cell / TRUE-FALSE reopen / BITWISE reopen; host lowercase `lshift`/`rshift` stay untouched). `0=` / optional `0<>` flag marks → `docs/ZERO-EQUALS.md` (wave22 **2** — sibling flag marks beside constants; **not** boolean cell / TRUE-FALSE reopen / LSHIFT reopen / AND-OR reopen; host `0=`/`0<>` stay untouched; prefer `zero-eq-mark`). Still no ANS COUNT reopen (wave20 **3** already stubbed), no EXECUTE reopen (wave20 **4** already stubbed), no real boolean cell rewrite, no linked XT / real DOES> XT / real branch XT / full arena/heap / full Win/Android Forth VM. Wave21 tip1–4 (ACCEPT-REFILL / BASE-HEX / COMPARE / BITWISE) and wave19 tip1–4 stay landed — keep cites; this tip does not reopen them.

## 4. Markers

```
[true] TRUE [flag=<n>] [u=<n>]     # flag=/u= optional; classic all-bits-set / -1 picture welcome
[true] FALSE [flag=<n>] [u=<n>]    # flag=/u= optional; classic 0 picture
[true] FAIL reason=<…>             # demo avoids
[true-demo] OK
[true-demo] FAIL
```

Lab greps `[true-demo] OK` plus at least one `[true] TRUE` and one `[true] FALSE` (optional `flag=` / `u=` welcome). Demo avoids `[true] FAIL`.

## 5. `true-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; constant marks need no dict entries.
2. Invoke `TRUE` (or `true-mark`) → `[true] TRUE` (+ optional `flag=` / `u=` — classic `-1` / all-bits-set picture welcome).
3. Invoke `FALSE` (or `false-mark`) → `[true] FALSE` (+ optional `flag=` / `u=` — classic `0` picture).
4. Assert no `[true] FAIL` on the happy path. Assert constant marks did **not** require a real boolean cell rewrite / `0=` deepen / WITHIN / branch XT (marker-only is enough). Assert host `TRUE`/`FALSE` were not redefined when using the Forth mirrors.
5. Prior `source-demo` / `env-demo` / `body-demo` / `char-demo` / `state-demo` / `word-demo` / `find-demo` / `tick-demo` / `recurse-demo` / `eval-demo` / `parse-demo` / `synonym-demo` / `exit-demo` / `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` / `refined-boot-demo` / `host-boot-demo` / `interpret-demo` / `kernel-demo` still OK.
6. `[true-demo] OK`.

Both `TRUE` and `FALSE` markers are required. Optional `flag=` / `u=` echo is not required for Lab OK when both markers are greppable. No real boolean cell. No `0=` deepen. No WITHIN. No branch XT.

## 6. Thin amend — companions

### `docs/KERNEL.md`

- Companions: add `TRUE-FALSE.md` (wave20 **1**); **keep** wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites and wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites and wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `TRUE` / `FALSE` stubs + `true-demo` (cite tip; Forth mirrors `true-mark` / `false-mark` — constant marks only; **do not** redefine host `TRUE`/`FALSE`; **not** real boolean cell rewrite / `0=` deepen / WITHIN; optional `flag=` / `u=` — classic `-1` / `0` picture welcome).
- Non-goals: `TRUE` / `FALSE` constant marks → `docs/TRUE-FALSE.md`. CHAR/CHARS/[CHAR] stay on `CHAR-CHARS.md`. >BODY stays on `TO-BODY.md`. ENVIRONMENT? stays on `ENVIRONMENT-QUERY.md`. SOURCE/PAD stay on `SOURCE-PAD.md`. CELL/CELLS/ALIGN/ALIGNED stay on `CELL-CELLS.md`. PICK/ROLL/DEPTH/?DUP stay on `PICK-ROLL.md`. WITHIN / COUNT / EXECUTE still later (wave20 **2–4**).
- Acceptance: Lab smokes `true-demo` (and retains `source-demo` + `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `pick-demo` + `cell-demo` + prior demos).
- Cite: `docs/TRUE-FALSE.md`.

### `docs/CELL-CELLS.md`

- Companions / Status: add `TRUE-FALSE.md` (wave20 **1** companion cite); **keep** CHAR-CHARS / ENVIRONMENT-QUERY / ALLOT-HERE / KERNEL / VARIABLE-CONST cites — do not wipe wave15 CELL content.
- Purpose / §3: cell-unit stubs stay; `TRUE` / `FALSE` may echo optional stub `u=` / `flag=` that picture cell-width flag values (classic all-bits-set / `0`) — **not** a CELL/ALIGN reopen / cell-size rewrite / real boolean cell. Do not wipe wave15 / wave19 tip1 / tip3 CELL/CHAR/ENV content.
- Non-goals: `TRUE` / `FALSE` → `docs/TRUE-FALSE.md` (wave20 **1**). CHAR/CHARS/[CHAR] stay on `CHAR-CHARS.md`. ENVIRONMENT? stays on `ENVIRONMENT-QUERY.md`. ALIGN/ALIGNED stay on this tip (already landed).
- Acceptance: Lab smokes `true-demo` (retains `cell-demo` + `char-demo` + `env-demo`).
- Cite: `docs/TRUE-FALSE.md`.

### `docs/PICK-ROLL.md`

- Companions: add `TRUE-FALSE.md` (wave20 **1**); **keep** KERNEL / CELL-CELLS cites — do not wipe wave15 PICK-ROLL content.
- Purpose / §3 / non-goals: stack/?DUP flag pictures stay; `TRUE` / `FALSE` are sibling **constant** flag marks — **not** a PICK/ROLL/?DUP reopen / real threaded stack / `0=` deepen / WITHIN. Do not wipe wave15 PICK-ROLL content. Host `2dup`/`2drop`/`2swap` stay untouched.
- Non-goals: `TRUE` / `FALSE` → `docs/TRUE-FALSE.md` (wave20 **1**). PICK/ROLL/DEPTH/?DUP stay on this tip (already landed). WITHIN still later (wave20 **2**).
- Acceptance: Lab smokes `true-demo` (retains `pick-demo` + `cell-demo`).
- Cite: `docs/TRUE-FALSE.md`.

### Optional — `docs/CONTROL.md`

- Companions: add light `TRUE-FALSE.md` (wave20 **1**) cite; **keep** COLON / INTERPRET / KERNEL / BEGIN-UNTIL / DO-LOOP / CASE-OF / THROW-CATCH cites.
- Purpose / non-goals: IF/THEN/ELSE stay balance-only stubs; `TRUE` / `FALSE` constant marks may picture flag values that companion control `taken=` — **not** a real branch XT / IF reopen / flag algebra. Do not wipe wave10 CONTROL content.
- Non-goals: `TRUE` / `FALSE` → `docs/TRUE-FALSE.md` (wave20 **1**). WITHIN still later (wave20 **2**). Real branch XT still out.
- Acceptance: Lab smokes `true-demo` (retains `control-demo`).
- Cite: `docs/TRUE-FALSE.md`.

### Optional — `docs/HOST-PARITY.md`

- Companions: add light `TRUE-FALSE.md` (wave20 **1**) cite; **keep** KERNEL / BUILD / INSTALL / INTERPRET / ENVIRONMENT-QUERY cites.
- Purpose / non-goals: Win/Android stay CONTRACT-parity; `true-demo` CONTRACT line is acceptable — **not** a full Forth VM / boolean-cell port. Do not wipe wave8 HOST-PARITY content.
- Non-goals: `TRUE` / `FALSE` → `docs/TRUE-FALSE.md` (wave20 **1**). Full Win/Android Forth VM still out.
- Acceptance: Lab smokes `true-demo` (Win/Android: `true-demo CONTRACT` OK).
- Cite: `docs/TRUE-FALSE.md`.

### Wave20 tip **2** companion — `docs/WITHIN.md`

- Companions / Status: WITHIN cites TRUE-FALSE as flag-picture companion; this tip cites WITHIN as sibling range-check mark.
- Purpose: `WITHIN` optional `flag=<0|1>` pairs with TRUE/FALSE constant flag pictures — **not** a TRUE/FALSE reopen / boolean cell rewrite / `0=` deepen / branch XT / IF/THEN reopen.
- Non-goals: `WITHIN` → `docs/WITHIN.md` (wave20 **2**). TRUE/FALSE stay on this tip (already landed).
- Acceptance: Lab smokes `within-demo` (retains `true-demo`).
- Cite: `docs/WITHIN.md`.

### `docs/BASE-HEX.md` (wave21 **2** thin companion cite)

- Companions / Status: add light `BASE-HEX.md` (wave21 **2**) cite; **keep** WITHIN / KERNEL / CELL-CELLS / PICK-ROLL cites — do not wipe wave20 TRUE-FALSE content.
- Purpose: `BASE` / `HEX` / `DECIMAL` are sibling **radix marks** — **not** a TRUE/FALSE reopen / boolean cell rewrite / `0=` deepen / BITWISE reopen / number parser.
- Non-goals: `BASE` / `HEX` / `DECIMAL` → `docs/BASE-HEX.md` (wave21 **2**). TRUE/FALSE stay on this tip (already landed). BITWISE → `docs/BITWISE.md` (wave21 **4**).
- Acceptance: Lab smokes `base-demo` (retains `true-demo` + `within-demo`).
- Cite: `docs/BASE-HEX.md`.

### `docs/BITWISE.md` (wave21 **4** thin companion cite)

- Companions / Status: add light `BITWISE.md` (wave21 **4**) cite; **keep** WITHIN / BASE-HEX / KERNEL / CELL-CELLS / PICK-ROLL / CONTROL / HOST-PARITY cites — do not wipe wave20 TRUE-FALSE content.
- Purpose: `AND` / `OR` / `XOR` / `INVERT` are sibling **bitwise marks** beside TRUE/FALSE constants (classic `0xFF` AND/OR/XOR/INVERT picture on stub ints; optional `u=` / `flag=`) — **not** a TRUE/FALSE reopen / boolean-cell rewrite / `0=` reopen / LSHIFT/RSHIFT / WITHIN reopen. Pairs with this tip's `flag=` / all-bits `-1` vs `0` picture without promoting either. Host lowercase `and`/`or` stay untouched.
- Non-goals: `AND` / `OR` / `XOR` / `INVERT` → `docs/BITWISE.md` (wave21 **4**). TRUE/FALSE stay on this tip (already landed). LSHIFT/RSHIFT → `docs/LSHIFT-RSHIFT.md` (wave22 **1**).
- Acceptance: Lab smokes `bit-demo` (retains `true-demo` + `within-demo` + `base-demo` + `compare-demo`).
- Cite: `docs/BITWISE.md`.

### `docs/LSHIFT-RSHIFT.md` (wave22 **1** thin companion cite)

- Companions / Status: add light `LSHIFT-RSHIFT.md` (wave22 **1**) cite; **keep** BITWISE / WITHIN / BASE-HEX / KERNEL / CELL-CELLS / PICK-ROLL / CONTROL / HOST-PARITY cites — do not wipe wave20 TRUE-FALSE content.
- Purpose: `LSHIFT` / `RSHIFT` are sibling **shift marks** beside TRUE/FALSE constants (classic `1 LSHIFT 4 → 16` / `16 RSHIFT 4 → 1` picture; optional `u=`) — **not** a TRUE/FALSE reopen / boolean-cell rewrite / AND-OR reopen / BITWISE reopen / WITHIN reopen. Host lowercase `lshift`/`rshift` stay untouched.
- Non-goals: `LSHIFT` / `RSHIFT` → `docs/LSHIFT-RSHIFT.md` (wave22 **1**). TRUE/FALSE stay on this tip (already landed). BITWISE stays wave21 **4**. `0=` → `docs/ZERO-EQUALS.md` (wave22 **2**).
- Acceptance: Lab smokes `shift-demo` (retains `true-demo` + `bit-demo` + `within-demo` + `base-demo` + `compare-demo`).
- Cite: `docs/LSHIFT-RSHIFT.md`.

### `docs/ZERO-EQUALS.md` (wave22 **2** thin companion cite)

- Companions / Status: add light `ZERO-EQUALS.md` (wave22 **2**) cite; **keep** LSHIFT-RSHIFT / BITWISE / WITHIN / BASE-HEX / KERNEL / CELL-CELLS / PICK-ROLL / CONTROL / HOST-PARITY cites — do not wipe wave20 TRUE-FALSE content.
- Purpose: `0=` (optional `0<>`) are sibling **flag marks** beside TRUE/FALSE constants (classic zero→true / nonzero→false; optional `flag=`) — **not** a TRUE/FALSE reopen / boolean-cell rewrite / LSHIFT reopen / AND-OR reopen / BITWISE reopen / WITHIN reopen. **CRITICAL:** host `0=`/`0<>` stay untouched; prefer `zero-eq-mark`.
- Non-goals: `0=` / optional `0<>` → `docs/ZERO-EQUALS.md` (wave22 **2**). TRUE/FALSE stay on this tip (already landed). LSHIFT-RSHIFT stays wave22 **1** (leave LSHIFT-RSHIFT.md primary untouched). BITWISE stays wave21 **4**.
- Acceptance: Lab smokes `zero-demo` (retains `true-demo` + `shift-demo` + `bit-demo` + `within-demo` + `base-demo` + `compare-demo`).
- Cite: `docs/ZERO-EQUALS.md`.

### `docs/U-LESS.md` (wave23 **3** thin companion cite)

- Companions / Status: add light `U-LESS.md` (wave23 **3**) cite; **keep** ZERO-EQUALS / LSHIFT-RSHIFT / BITWISE / WITHIN / BASE-HEX / KERNEL cites — do not wipe wave20 TRUE-FALSE content.
- Purpose: `U<` is sibling **unsigned compare flag mark** beside TRUE/FALSE constants — **not** a TRUE/FALSE reopen / boolean cell / ZERO-EQUALS reopen / WITHIN reopen. Prefer `u-less-mark`; do not redefine `true-mark`/`false-mark`.
- Non-goals: `U<` → `docs/U-LESS.md` (wave23 **3**). TRUE/FALSE stay on this tip. Tip4 ABS-NEGATE / tip5 DOCS-CITES still later.
- Acceptance: Lab smokes `uless-demo` (retains `true-demo` + `zero-demo` + `bit-demo` + `hold-demo` + `charplus-demo`).
- Cite: `docs/U-LESS.md`.

### `docs/ABS-NEGATE.md` (wave23 **4** thin companion cite)

- Companions / Status: add light `ABS-NEGATE.md` (wave23 **4**) cite; **keep** U-LESS / ZERO-EQUALS / LSHIFT-RSHIFT / BITWISE / WITHIN / BASE-HEX / KERNEL cites — do not wipe wave20 TRUE-FALSE content.
- Purpose: `ABS` / `NEGATE` are sibling **signed magnitude / negate marks** beside TRUE/FALSE constants (classic `|n|` / `-n`; optional `n=` / `u=`) — **not** a TRUE/FALSE reopen / boolean-cell rewrite / BITWISE reopen / LSHIFT reopen / ZERO-EQUALS reopen / U-LESS reopen. Prefer `abs-mark`/`negate-mark`; **do not** redefine `true-mark`/`false-mark`.
- Non-goals: `ABS` / `NEGATE` → `docs/ABS-NEGATE.md` (wave23 **4**). TRUE/FALSE stay on this tip (already landed). BITWISE stays wave21 **4**. LSHIFT stays wave22 **1**. U-LESS stays wave23 **3**. Tip5 DOCS-CITES still later (wave23 **5**).
- Acceptance: Lab smokes `abs-demo` (retains `true-demo` + `bit-demo` + `shift-demo` + `zero-demo` + `uless-demo` + `hold-demo` + `charplus-demo`).
- Cite: `docs/ABS-NEGATE.md`.

Do **not** wipe tip1 TRUE-FALSE content when tip2 / tip4 / wave22 tip1 / wave22 tip2 amends this file thinly. Do **not** wipe wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites or wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites or wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites or wave16 DEFER / MARKER / BUFFER / EXIT cites or wave15 CELL / IMMEDIATE / FILL / PICK prior content. Tip1 primary amends were KERNEL + CELL-CELLS + PICK-ROLL + optional CONTROL / HOST-PARITY; tip2 thin-amends this file as companion only (checksum CHANGES — expected). Leave CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD / ARCHITECTURE / IMPLEMENTATION-GAPS untouched for tip2. Tip5 cites come after 1–4 PASS.

## 7. Non-goals

- Real boolean cell rewrite / live flag storage / VARIABLE-as-boolean
- `0=` / optional `0<>` flag marks → `docs/ZERO-EQUALS.md` (wave22 **2**; sibling flag marks beside constants — **not** boolean cell / TRUE-FALSE reopen / LSHIFT reopen / AND-OR reopen; host `0=`/`0<>` stay untouched; prefer `zero-eq-mark`)
- `WITHIN` range-check mark → `docs/WITHIN.md` (wave20 **2**; sibling range-check — not a TRUE/FALSE reopen / boolean cell / branch XT)
- `BASE` / `HEX` / `DECIMAL` base marks → `docs/BASE-HEX.md` (wave21 **2**; sibling radix marks — not a TRUE/FALSE reopen / boolean cell / `0=` / BITWISE reopen / number parser; do not break HERE stub base)
- `AND` / `OR` / `XOR` / `INVERT` bitwise marks → `docs/BITWISE.md` (wave21 **4**; sibling bitwise marks beside TRUE/FALSE constants — not a TRUE/FALSE reopen / boolean-cell rewrite / `0=` reopen / LSHIFT/RSHIFT; host lowercase `and`/`or` stay untouched)
- `LSHIFT` / `RSHIFT` shift marks → `docs/LSHIFT-RSHIFT.md` (wave22 **1**; sibling shift marks beside constants — **not** boolean cell / TRUE-FALSE reopen / BITWISE reopen; host lowercase `lshift`/`rshift` stay untouched; leave LSHIFT-RSHIFT.md primary untouched this tip)
- `U<` unsigned compare flag mark → `docs/U-LESS.md` (wave23 **3**; sibling unsigned compare — **not** boolean cell / TRUE-FALSE reopen; prefer `u-less-mark`; leave U-LESS.md primary untouched)
- `ABS` / `NEGATE` signed magnitude / negate marks → `docs/ABS-NEGATE.md` (wave23 **4**; sibling magnitude/negate — **not** boolean cell / TRUE-FALSE reopen / BITWISE reopen / LSHIFT reopen; prefer `abs-mark`/`negate-mark`; do not redefine `true-mark`/`false-mark`)
- ANS `COUNT` c-addr picture (wave20 **3**); host `ENTRY-COUNT` / `SYN-COUNT` / `words-count` are **not** ANS COUNT
- `EXECUTE` xt-id invoke mark (wave20 **4**); not real XT execute
- Docs cites pass (wave20 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `CHAR` / `CHARS` / `[CHAR]` reopen (wave19 **1** — already stubbed; keep cites)
- `>BODY` reopen (wave19 **2** — already stubbed; keep cites)
- `ENVIRONMENT?` reopen (wave19 **3** — already stubbed; keep cites)
- `SOURCE` / `PAD` reopen (wave19 **4** — already stubbed; keep cites); `ACCEPT` / `REFILL` / full input-buffer VM still out
- `CELL` / `CELLS` / `ALIGN` / `ALIGNED` reopen (wave15 — already stubbed; keep cites)
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

1. `docs/TRUE-FALSE.md` present (Research byte-copy OK); `KERNEL.md` + `CELL-CELLS.md` + `PICK-ROLL.md` thin amends present (+ optional `CONTROL.md` / `HOST-PARITY.md`); wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites, wave18 tip1–4 cites, wave17 tip1–4 cites, wave16 DEFER/MARKER/BUFFER/EXIT cites, and wave15 CELL/IMMEDIATE/COLON/KERNEL prior text retained; host `TRUE`/`FALSE` untouched via mirrors; CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD / ARCHITECTURE / IMPLEMENTATION-GAPS byte-copy unchanged.
2. `true-demo` → OK (markers §4; `[true] TRUE` and `[true] FALSE` greppable; optional `flag=` / `u=` welcome — classic `-1` / `0` picture; no FAIL on happy path; no real boolean cell / `0=` deepen / WITHIN / branch XT). Prior `source-demo` + `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `recurse-demo` + `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` + `refined-boot-demo` + `host-boot-demo` + `interpret-demo` + `kernel-demo` still OK.
3. Regression green (wave19 tip1–5 + wave18 tip1–5 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `true-demo CONTRACT` OK). Wave20 **2**: `within-demo` → OK (retains `true-demo`). Wave21 **2**: `base-demo` → OK (retains `true-demo` + `within-demo`). Wave21 **4**: `bit-demo` → OK (retains `true-demo` + `within-demo` + `base-demo` + `compare-demo`; pairs with TRUE/FALSE without boolean-cell rewrite / `0=` reopen; host lowercase `and`/`or` untouched). Wave22 **1**: `shift-demo` → OK (retains `true-demo` + `bit-demo` + `within-demo` + `base-demo` + `compare-demo`; sibling shift marks beside constants — **not** boolean cell; host lowercase `lshift`/`rshift` untouched). Wave22 **2**: `zero-demo` → OK (retains `true-demo` + `shift-demo` + `bit-demo` + `within-demo` + `base-demo` + `compare-demo`; sibling flag marks beside constants — **NOT** boolean cell / TRUE-FALSE reopen; host `0=`/`0<>` untouched; prefer `zero-eq-mark`). Wave23 **3**: `uless-demo` → OK (retains `true-demo` + `zero-demo` + `bit-demo` + `hold-demo` + `charplus-demo`; sibling unsigned compare — **NOT** boolean cell / TRUE-FALSE reopen; prefer `u-less-mark`). Wave23 **4**: `abs-demo` → OK (retains `true-demo` + `bit-demo` + `shift-demo` + `zero-demo` + `uless-demo` + `hold-demo` + `charplus-demo`; sibling magnitude/negate — **NOT** boolean cell / TRUE-FALSE reopen; prefer `abs-mark`/`negate-mark`; do not redefine `true-mark`/`false-mark`).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**), `docs/CELL-CELLS.md` (wave15 **1** — unit/cell picture companion for optional `u=` / flag echo)
- `docs/PICK-ROLL.md` (wave15 **2** — stack/?DUP flag-picture companion; TRUE/FALSE are constant marks — not PICK/?DUP reopen)
- `docs/CONTROL.md` (wave10 **2**, optional), `docs/HOST-PARITY.md` (wave8 **4**, optional)
- `docs/VARIABLE-CONST.md` (wave12 **2** — named-cell sibling; TRUE/FALSE are constant marks — not VARIABLE/CONSTANT reopen)
- `docs/CHAR-CHARS.md` (wave19 **1** — prior tip; keep cites)
- `docs/TO-BODY.md` (wave19 **2** — prior tip; keep cites)
- `docs/ENVIRONMENT-QUERY.md` (wave19 **3** — prior tip; keep cites)
- `docs/SOURCE-PAD.md` (wave19 **4** — prior tip; keep cites)
- `docs/TICK.md` (wave18 **1**), `docs/STATE-COMPILE.md` (wave18 **4**), `docs/FIND.md` (wave18 **2**)
- `docs/DEFER-IS.md` (wave16 **1**), `docs/SYNONYM-ALIAS.md` (wave17 **1**)
- `docs/COLON.md` (wave9 **4**), `docs/CREATE-DOES.md` (wave13 **3**)
- `forth/tritium/kernel.fs` (true-mark / false-mark only — do not redefine host TRUE/FALSE)
- ANS Forth `TRUE` / `FALSE` (constant marks only — not real boolean cell rewrite / `0=` deepen / WITHIN)
- Explicit deferral: WAVE18-PROPOSAL + WAVE19-PROPOSAL + WAVE20-PROPOSAL (`TRUE` / `FALSE` — constant marks; not boolean cell rewrite)
- Base tip: `e93c44a` / `e93c44af21bb3310b3fc5a7351ec2f257b50f2bb` (#91 wave19 tip5 DOCS-CITES CLOSED)
- `docs/WITHIN.md` (wave20 **2** — range-check mark companion; pairs with this tip's flag picture)
- `docs/BASE-HEX.md` (wave21 **2** — radix marks companion; not boolean cell / `0=` / BITWISE reopen)
- `docs/BITWISE.md` (wave21 **4** — sibling bitwise marks beside TRUE/FALSE constants — NOT boolean-cell rewrite / `0=` reopen / TRUE-FALSE reopen; host lowercase `and`/`or` stay untouched)
- `docs/LSHIFT-RSHIFT.md` (wave22 **1** — sibling shift marks beside constants — NOT boolean cell / TRUE-FALSE reopen / BITWISE reopen; host lowercase `lshift`/`rshift` stay untouched)
- `docs/ZERO-EQUALS.md` (wave22 **2** — sibling flag mark beside constants — NOT boolean cell / TRUE-FALSE reopen; **CRITICAL:** host `0=`/`0<>` untouched; prefer `zero-eq-mark`)
- `docs/U-LESS.md` (wave23 **3** — sibling unsigned compare flag mark; **not** boolean cell / TRUE-FALSE reopen; prefer `u-less-mark`; do not redefine `true-mark`/`false-mark`)
- `docs/ABS-NEGATE.md` (wave23 **4** — sibling signed magnitude / negate marks; **not** boolean cell / TRUE-FALSE reopen / BITWISE reopen / LSHIFT reopen; prefer `abs-mark`/`negate-mark`; do not redefine `true-mark`/`false-mark`)
- Wave20 proposal: `/workspace/tritium-research-docs/WAVE20-PROPOSAL.md`
