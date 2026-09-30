# BASE-HEX — `BASE` / `HEX` / `DECIMAL` base marks + `base-demo`

**Status:** Shipper-ready stub spec (wave21 item **2**; thin amend wave22 **3** TO-NUMBER companion cite)
**Canonical brief:** ANS-shaped `BASE` / `HEX` / `DECIMAL` (thin radix marks only); `docs/KERNEL.md` (wave7 **5**); `docs/ENVIRONMENT-QUERY.md` (wave19 **3** — query-mark companion; optional radix echo welcome — **not** a full ANS ENVIRONMENT? table reopen); optional `docs/CELL-CELLS.md` (wave15 **1** — cell/unit picture companion) / `docs/TRUE-FALSE.md` (wave20 **1** — constant-mark companion) / `docs/HOST-PARITY.md` (wave8 **4**); explicit WAVE21 deferral closed as base marks only (not pictured numeric `#`/`HOLD`/`<#`/`#>`/`#S`; **must not** redefine or bump HERE stub base `$1000 _here !`); `docs/TO-NUMBER.md` (wave22 **3** — sibling thin number-parse mark; **not** BASE reopen; HERE stub untouched).
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `base.fs` / `hex.fs` / `base-hex.fs`); Linux host REPL; **do not** redefine host `BASE` / `HEX` / `DECIMAL` that already bind on the load path; **do not** redefine, alias, or bump `_here` / `HERE` / `here-at`
**Companions:** `docs/KERNEL.md` (thin amend this tip), `docs/ENVIRONMENT-QUERY.md` (thin amend this tip); optional light cite `docs/CELL-CELLS.md` / `docs/TRUE-FALSE.md` / `docs/HOST-PARITY.md`; `docs/TO-NUMBER.md` (wave22 **3** — thin companion cite from tip3; sibling thin parse — **not** BASE reopen; HERE stub untouched)
**Base tip SHA:** `9e9987e` (wave21 tip1 PASS / #97 ACCEPT-REFILL) / full `9e9987e69f9580884d88ce32b3389db11cc921d6`

## 1. Purpose

WAVE14 tip **1** landed `HERE` / `ALLOT` as a dictionary-pointer stub (`docs/ALLOT-HERE.md`) with stub base commonly `$1000 _here !` (`allot-demo`). WAVE19 tip **3** landed thin `ENVIRONMENT?` query mark (`docs/ENVIRONMENT-QUERY.md`) — optional stub values may picture units; **not** a full ANS env table. WAVE21 tip **1** landed thin `ACCEPT` / `REFILL` marks (`docs/ACCEPT-REFILL.md`) beside SOURCE-PAD + COUNT. WAVE18 / WAVE19 / WAVE20 / WAVE21 explicitly deferred `BASE` / `HEX` / `DECIMAL` as a number-parser / radix surface. This tip lands **stub base marks only**: `BASE` (or Forth mirror **`base-mark`**) prints `[base] BASE` (+ optional `u=` / `rad=` — classic decimal **`10`** picture welcome as the default); `HEX` (or Forth mirror **`hex-mark`**) prints `[base] HEX` (+ optional `rad=16`); `DECIMAL` (or Forth mirror **`decimal-mark`**) prints `[base] DECIMAL` (+ optional `rad=10`). Optional `OCTAL` (`rad=8`) is **out** of this tip unless Shipper finds it free — **do not require** it. Smoke via **`base-demo`**. Demo avoids FAIL. Forth mirrors **`base-mark` / `hex-mark` / `decimal-mark`** so a host `BASE` variable (and host `HEX` / `DECIMAL`) stay safe. **Critical:** this tip’s `BASE` is the **numeric radix mark only** and must **NOT** redefine, alias, or bump `_here` / `HERE` / `here-at` / the HERE stub base at `$1000`. Prefer Forth mirror **`base-mark`** whenever host `BASE` collides. **Not** a real number parser, not `>NUMBER`, not pictured numeric `#`/`HOLD`/`<#`/`#>`/`#S`. Optional env query string may echo radix — **not** a full ANS ENVIRONMENT? table reopen. Builds beside ENVIRONMENT-QUERY + KERNEL without promoting either. Independent of tip1 ACCEPT-REFILL and tip3 COMPARE. Wave22 tip **3** lands `>NUMBER` thin number-parse mark (`docs/TO-NUMBER.md`): sibling **thin parse** beside radix marks — **not** a BASE/HEX/DECIMAL reopen / pictured numeric `#`/`HOLD`/`<#`/`#>`/`#S` / HERE stub base bump; prefer `to-number-mark`; HERE stub `$1000` stays untouched.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `BASE` / `base-mark` | `( -- u )` *or* `( -- )` *or* `( u -- )` picture | Thin radix mark; print `[base] BASE` (+ optional `u=<n>` / `rad=<n>`); classic decimal **`10`** picture welcome as the default |
| `HEX` / `hex-mark` | `( -- )` | Thin HEX mark; print `[base] HEX` (+ optional `rad=16`) |
| `DECIMAL` / `decimal-mark` | `( -- )` | Thin DECIMAL mark; print `[base] DECIMAL` (+ optional `rad=10`) |
| `base-demo` | `( -- )` | See §5 |

Host note: bind bare `BASE` / `HEX` / `DECIMAL` on the Linux REPL **only if** those names do not collide with a host Forth `BASE` variable / `HEX` / `DECIMAL` in the same load path. Prefer Forth mirrors **`base-mark` / `hex-mark` / `decimal-mark`** as the Lab-facing surface when in doubt — **do not** redefine host `BASE` / `HEX` / `DECIMAL`. **Critical:** do **not** redefine, alias, or bump `_here` / `HERE` / `here-at` (wave14 **1** HERE stub base `$1000 _here !` stays landed — this tip’s BASE is **numeric radix**, not dictionary-pointer base). Optional `u=` / `rad=` are host ints / radix echo only — not a live number parser, not `>NUMBER`, not pictured numeric output, not a HERE bump. Prefer printing BASE (decimal `10` picture) + HEX (`rad=16`) + DECIMAL (`rad=10`) in the demo so Lab hit is deterministic and FAIL is avoided. Optional `OCTAL` / `octal-mark` (`rad=8`) is **out** — do not require.

## 3. Stub semantics

- **`BASE` / `base-mark`:** picture classic ANS BASE (current numeric radix) — print `[base] BASE` and optionally `u=<n>` and/or `rad=<n>`. Classic default picture: **decimal `10`** (welcome; greppable `u=10` / `rad=10` when present). **Does not** rewrite the number parser, deepen `>NUMBER`, implement pictured numeric `#`/`HOLD`/`<#`/`#>`/`#S`, allocate, or bump HERE. **Does not** redefine / alias / store into `_here` / `HERE` / `here-at`. Captured values are host ints / radix echo only.
- **`HEX` / `hex-mark`:** picture classic HEX (set radix 16) — print `[base] HEX` and optionally `rad=16`. Marker only — does not rewrite parser / pictured numeric / HERE.
- **`DECIMAL` / `decimal-mark`:** picture classic DECIMAL (set radix 10) — print `[base] DECIMAL` and optionally `rad=10`. Marker only.
- **Optional push / store:** if the host stack / a host BASE cell is easy, picture / echo the radix value (`10` / `16`); marker alone is enough for Lab OK — do **not** require a live BASE variable rewrite that collides with HERE, and do **not** store into `_here`.
- **FAIL:** `[base] FAIL reason=<…>` optional (demo **must avoid**). Prefer not emitting `[base] FAIL` on the happy path. No required FAIL reason this tip — unknown radix / parser miss stay out of the demo.
- Storage: host radix ints / flag echo only. **No** real number parser rewrite, no `>NUMBER`, no pictured numeric `#`/`HOLD`/`<#`/`#>`/`#S`, no HERE / `_here` / `here-at` redefine / bump, no arena, no full ANS ENVIRONMENT? table reopen.
- Nest with prior accept / exec / count / within / true / source / env / body / char / state / word / find / tick / create / allot / synonym / exit / buffer / marker / defer / imm / fill / pick / cell / throw / 2var / colon / control / string stubs OK. `dict-reset` unaffected (no new dict entries from base marks — fixed host radix echo). Assert prior `allot-demo` still OK (HERE stub base untouched).
- Still no pictured numeric `#`/`HOLD`/`<#`/`#>`/`#S` this tip (thin `>NUMBER` → wave22 **3** `docs/TO-NUMBER.md` — sibling; **not** BASE reopen), no HERE stub base bump, no tip3 COMPARE / tip4 BITWISE reopen this doc, no linked XT / real DOES> XT / real branch XT / full arena/heap / full Win/Android Forth VM. Wave21 tip1 ACCEPT-REFILL + wave20 tip1–4 + wave19 tip1–4 stay landed — keep cites. Wave22 tip **3** TO-NUMBER is sibling thin parse — keep cites; do not reopen BASE. Optional `OCTAL` out.

## 4. Markers

```
[base] BASE [u=<n>] [rad=<n>]     # u=/rad= optional; classic decimal 10 picture welcome
[base] HEX [rad=16]               # rad= optional; prefer rad=16 when present
[base] DECIMAL [rad=10]           # rad= optional; prefer rad=10 when present
[base] FAIL reason=<…>            # demo avoids
[base-demo] OK
[base-demo] FAIL
```

Lab greps `[base-demo] OK` plus at least one `[base] BASE` (optional `u=` / `rad=` welcome; classic decimal `10` picture welcome), one `[base] HEX` (optional `rad=16` welcome), and one `[base] DECIMAL` (optional `rad=10` welcome). Demo avoids `[base] FAIL`. Prefer not emitting `[base] FAIL` on the happy path. **Do not** Lab-grep `_here` / `HERE` / `here-at` / `[allot]` as this tip’s BASE surface (those stay wave14 **1**). **Do not** Lab-grep `[env]` / full ANS ENVIRONMENT? table as BASE-HEX (optional radix echo via env query string is companion cite only — not a table reopen).

## 5. `base-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; base marks need no dict entries.
2. Invoke `BASE` (or **`base-mark`**) → `[base] BASE` (+ optional `u=` / `rad=` — classic decimal `10` picture welcome). Prefer `u=10` or `rad=10` greppable when present.
3. Invoke `HEX` (or **`hex-mark`**) → `[base] HEX` (+ optional `rad=16`). Prefer `rad=16` greppable when present.
4. Invoke `DECIMAL` (or **`decimal-mark`**) → `[base] DECIMAL` (+ optional `rad=10`). Prefer `rad=10` greppable when present.
5. Assert no `[base] FAIL` on the happy path. Assert base marks did **not** require a real number parser / `>NUMBER` / pictured numeric `#`/`HOLD`/`<#`/`#>`/`#S` / HERE bump / `_here` redefine / full ANS ENVIRONMENT? table reopen (marker-only is enough). Assert host `BASE` / `HEX` / `DECIMAL` were not redefined when using the Forth mirrors. Assert `_here` / `HERE` / `here-at` were **not** redefined / aliased / bumped. Assert prior `allot-demo` / `env-demo` / `accept-demo` still OK.
6. Prior `accept-demo` / `exec-demo` / `count-demo` / `within-demo` / `true-demo` / `source-demo` / `env-demo` / `body-demo` / `char-demo` / `state-demo` / `word-demo` / `find-demo` / `tick-demo` / `recurse-demo` / `eval-demo` / `parse-demo` / `synonym-demo` / `exit-demo` / `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` / `refined-boot-demo` / `host-boot-demo` / `interpret-demo` / `kernel-demo` still OK.
7. `[base-demo] OK`.

`[base] BASE` + `[base] HEX` + `[base] DECIMAL` markers are required. FAIL path is not exercised by the demo. Optional `u=` / `rad=` echo is not all required for Lab OK when BASE + HEX + DECIMAL lines are greppable. No real number parser. No `>NUMBER`. No pictured numeric. No HERE stub base bump. No full ANS ENVIRONMENT? table reopen. Optional OCTAL not required.

## 6. Thin amend — companions

### `docs/KERNEL.md`

- Companions: add `BASE-HEX.md` (wave21 **2**); **keep** wave21 tip1 ACCEPT-REFILL cites and wave20 tip1–4 TRUE-FALSE / WITHIN / COUNT / EXECUTE cites and wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites and wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites and wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `BASE` / `base-mark`, `HEX` / `hex-mark`, `DECIMAL` / `decimal-mark` stubs + `base-demo` (cite tip; Forth mirrors `base-mark` / `hex-mark` / `decimal-mark` — radix marks only; **do not** redefine host `BASE` / `HEX` / `DECIMAL`; **do not** redefine / alias / bump `_here` / `HERE` / `here-at`; **not** number parser / `>NUMBER` / pictured numeric `#`/`HOLD`/`<#`/`#>`/`#S`; optional `u=` / `rad=` — classic decimal `10` / HEX `16` / DECIMAL `10` picture welcome; optional OCTAL out).
- Non-goals: `BASE` / `HEX` / `DECIMAL` base marks → `docs/BASE-HEX.md`. ACCEPT/REFILL stay on `ACCEPT-REFILL.md`. SOURCE/PAD stay on `SOURCE-PAD.md`. ENVIRONMENT? stays on `ENVIRONMENT-QUERY.md`. HERE/ALLOT stay on `ALLOT-HERE.md` (HERE stub base `$1000` untouched). COMPARE still later (wave21 **3**). BITWISE still later (wave21 **4**). Tip5 DOCS-CITES still later (wave21 **5**).
- Acceptance: Lab smokes `base-demo` (and retains `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `source-demo` + `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `allot-demo` + prior demos).
- Cite: `docs/BASE-HEX.md`.

### `docs/ENVIRONMENT-QUERY.md`

- Companions / Status: add `BASE-HEX.md` (wave21 **2** companion cite); **keep** KERNEL / CELL-CELLS / FIND / WORDS-VOCAB / HOST-PARITY / CHAR-CHARS / TO-BODY cites — do not wipe wave19 ENVIRONMENT-QUERY content.
- Purpose / §3 / non-goals: ENVIRONMENT? stays thin query mark against fixed demo query set; `BASE` / `HEX` / `DECIMAL` are sibling **radix marks** — optional env query string may echo radix (e.g. a documented stub name) — **not** a full ANS ENVIRONMENT? table reopen / SEARCH-WORDLIST / wordlist rewrite / number parser / `>NUMBER`. Do not wipe wave19 ENVIRONMENT-QUERY content.
- Non-goals: `BASE` / `HEX` / `DECIMAL` → `docs/BASE-HEX.md` (wave21 **2**). Full ANS ENVIRONMENT? table still out. ENVIRONMENT? stays on this tip (already landed).
- Acceptance: Lab smokes `base-demo` (retains `env-demo` + `accept-demo` + `cell-demo`).
- Cite: `docs/BASE-HEX.md`.

### Optional — `docs/CELL-CELLS.md`

- Companions / Status: add light `BASE-HEX.md` (wave21 **2**) cite; **keep** CHAR-CHARS / ENVIRONMENT-QUERY / TRUE-FALSE / WITHIN / ALLOT-HERE / KERNEL cites — do not wipe wave15 CELL content.
- Purpose / §3 / non-goals: cell-unit stubs stay; `BASE` / `HEX` / `DECIMAL` are sibling **radix marks** — **not** a CELL/ALIGN reopen / cell-size rewrite / HERE stub base bump / number parser. Do not wipe wave15 CELL content. Stress: HERE stub base `$1000` stays wave14 **1** — tip2 BASE is numeric radix only.
- Non-goals: `BASE` / `HEX` / `DECIMAL` → `docs/BASE-HEX.md` (wave21 **2**). ALIGN/ALIGNED / CELL stay on this tip (already landed).
- Acceptance: Lab smokes `base-demo` (retains `cell-demo` + `allot-demo` + `env-demo`).
- Cite: `docs/BASE-HEX.md`.

### Optional — `docs/TRUE-FALSE.md`

- Companions / Status: add light `BASE-HEX.md` (wave21 **2**) cite; **keep** WITHIN / KERNEL / CELL-CELLS / PICK-ROLL / CONTROL / HOST-PARITY cites — do not wipe wave20 TRUE-FALSE content.
- Purpose / §3 / non-goals: TRUE/FALSE stay constant marks; `BASE` / `HEX` / `DECIMAL` are sibling **radix marks** — **not** a boolean cell rewrite / `0=` deepen / WITHIN reopen / number parser. Do not wipe wave20 TRUE-FALSE content.
- Non-goals: `BASE` / `HEX` / `DECIMAL` → `docs/BASE-HEX.md` (wave21 **2**). TRUE/FALSE stay on this tip (already landed). BITWISE still later (wave21 **4**).
- Acceptance: Lab smokes `base-demo` (retains `true-demo` + `within-demo`).
- Cite: `docs/BASE-HEX.md`.

### Optional — `docs/HOST-PARITY.md`

- Companions: add light `BASE-HEX.md` (wave21 **2**) cite; **keep** ENVIRONMENT-QUERY / TRUE-FALSE / KERNEL / BUILD / INSTALL cites.
- Non-goals: `BASE` / `HEX` / `DECIMAL` base marks only → `docs/BASE-HEX.md` (wave21 **2**); `base-demo CONTRACT` acceptable; full number parser / Win/Android Forth VM still out.
- Acceptance: Lab smokes `base-demo` (Win/Android: `base-demo CONTRACT` OK).
- Cite: `docs/BASE-HEX.md`.

### `docs/TO-NUMBER.md` (wave22 **3** thin companion cite)

- Companions / Status: BASE-HEX cites TO-NUMBER as thin number-parse sibling; TO-NUMBER cites BASE-HEX as radix-mark companion (**not** BASE reopen).
- Purpose: TO-NUMBER sibling thin parse — **NOT** BASE reopen; HERE stub untouched (`$1000 _here !`); prefer `to-number-mark`.
- Non-goals: `>NUMBER` → `docs/TO-NUMBER.md` (wave22 **3**). BASE/HEX/DECIMAL stay on this tip (already landed). Pictured numeric `#`/`HOLD`/`<#`/`#>`/`#S` still out.
- Acceptance: Lab smokes `number-demo` (retains `base-demo` + `allot-demo` + `env-demo`).
- Cite: `docs/TO-NUMBER.md`.

Do **not** wipe wave21 tip1 ACCEPT-REFILL content or wave20 tip1–4 TRUE-FALSE / WITHIN / COUNT / EXECUTE content or wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites or wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites or wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites or wave16 DEFER / MARKER / BUFFER / EXIT cites or wave15 CELL / IMMEDIATE / FILL / PICK prior content. Do **not** amend ARCHITECTURE / IMPLEMENTATION-GAPS / ACCEPT-REFILL primary this tip (proposal amends are KERNEL + ENVIRONMENT-QUERY + optional CELL-CELLS / TRUE-FALSE / HOST-PARITY only). **Leave `ACCEPT-REFILL.md` primary untouched** (prefer no cite amend). **Leave `ARCHITECTURE.md` and `IMPLEMENTATION-GAPS.md` untouched.** Tip5 cites come after 1–4 PASS.

## 7. Non-goals

- Real number parser rewrite / tokenizer number path
- `>NUMBER` thin number-parse mark → `docs/TO-NUMBER.md` (wave22 **3**; sibling thin parse — **not** BASE reopen; HERE stub untouched; prefer `to-number-mark`)
- Pictured numeric output (`#` / `HOLD` / `<#` / `#>` / `#S`)
- Redefining / aliasing / bumping `_here` / `HERE` / `here-at` / HERE stub base `$1000 _here !` (wave14 **1** — already stubbed; keep cites; **BASE is numeric radix only**)
- Optional `OCTAL` / `octal-mark` (out unless Shipper finds free — **do not require**)
- Full ANS `ENVIRONMENT?` table reopen (wave19 **3** — already stubbed; keep cites; thin companion amend only — optional radix echo welcome, not table reopen)
- `ACCEPT` / `REFILL` reopen (wave21 **1** — already stubbed; leave ACCEPT-REFILL.md primary untouched)
- ANS `COMPARE` string mark (wave21 **3**); host `cstr=` ≠ ANS COMPARE
- `AND` / `OR` / `XOR` / `INVERT` bitwise marks (wave21 **4**); not boolean cell rewrite
- Docs cites pass (wave21 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `TRUE` / `FALSE` reopen (wave20 **1** — already stubbed; keep cites; optional thin companion cite only)
- `WITHIN` reopen (wave20 **2** — already stubbed; keep cites)
- `COUNT` reopen (wave20 **3** — already stubbed; keep cites)
- `EXECUTE` reopen (wave20 **4** — already stubbed; keep cites)
- `SOURCE` / `PAD` reopen (wave19 **4** — already stubbed; keep cites)
- `CELL` / `CELLS` / `ALIGN` / `ALIGNED` reopen (wave15 — already stubbed; keep cites; optional thin companion cite only)
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

1. `docs/BASE-HEX.md` present (Research byte-copy OK); `KERNEL.md` + `ENVIRONMENT-QUERY.md` thin amends present (+ optional `CELL-CELLS.md` / `TRUE-FALSE.md` / `HOST-PARITY.md`); wave21 tip1 ACCEPT-REFILL cites, wave20 tip1–4 TRUE-FALSE / WITHIN / COUNT / EXECUTE cites, wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites, wave18 tip1–4 cites, wave17 tip1–4 cites, wave16 DEFER/MARKER/BUFFER/EXIT cites, and wave15 CELL/IMMEDIATE/COLON/KERNEL prior text retained; host `BASE` / `HEX` / `DECIMAL` untouched via mirrors; `_here` / `HERE` / `here-at` **not** redefined / bumped; `ACCEPT-REFILL.md` primary untouched; `ARCHITECTURE.md` + `IMPLEMENTATION-GAPS.md` byte-copy unchanged.
2. `base-demo` → OK (markers §4; `[base] BASE` greppable; `[base] HEX` greppable; `[base] DECIMAL` greppable; optional `u=` / `rad=` welcome; classic decimal `10` / HEX `rad=16` / DECIMAL `rad=10` picture welcome; no FAIL on happy path; no pictured numeric / HERE stub base bump / full ANS ENVIRONMENT? table reopen; wave22 **3** `number-demo` → OK retains `base-demo` — TO-NUMBER sibling thin parse, **not** BASE reopen). Prior `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `source-demo` + `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `recurse-demo` + `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` + `refined-boot-demo` + `host-boot-demo` + `interpret-demo` + `kernel-demo` still OK.
3. Regression green (wave21 tip1 + wave20 tip1–5 + wave19 tip1–5 + wave18 tip1–5 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `base-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**)
- `docs/ENVIRONMENT-QUERY.md` (wave19 **3** — query-mark companion; optional radix echo welcome — **not** a full ANS ENVIRONMENT? table reopen)
- `docs/CELL-CELLS.md` (wave15 **1**, optional — cell/unit picture companion; not CELL/ALIGN reopen / HERE stub base bump)
- `docs/TRUE-FALSE.md` (wave20 **1**, optional — constant-mark companion; not boolean cell rewrite)
- `docs/HOST-PARITY.md` (wave8 **4**, optional — `base-demo CONTRACT` parity welcome)
- `docs/ALLOT-HERE.md` (wave14 **1** — HERE/ALLOT pointer stubs; HERE stub base `$1000 _here !` stays untouched — BASE is numeric radix only)
- `docs/ACCEPT-REFILL.md` (wave21 **1** — prior tip; keep cites; leave primary untouched this tip)
- `docs/WITHIN.md` (wave20 **2** — prior tip; keep cites)
- `docs/COUNT.md` (wave20 **3** — prior tip; keep cites)
- `docs/EXECUTE.md` (wave20 **4** — prior tip; keep cites)
- `docs/SOURCE-PAD.md` (wave19 **4** — prior tip; keep cites)
- `docs/CHAR-CHARS.md` (wave19 **1** — prior tip; keep cites)
- `docs/TO-BODY.md` (wave19 **2** — prior tip; keep cites)
- `forth/tritium/kernel.fs` (base-mark / hex-mark / decimal-mark only — do not redefine host BASE/HEX/DECIMAL; do not redefine/bump `_here`/HERE/here-at)
- ANS Forth `BASE` / `HEX` / `DECIMAL` (base marks only — numeric radix picture; thin `>NUMBER` → wave22 **3**; not pictured numeric; not HERE stub base)
- `docs/TO-NUMBER.md` (wave22 **3** — sibling thin number-parse mark; **not** BASE reopen; HERE stub untouched; prefer `to-number-mark`)
- Explicit deferral: WAVE19-PROPOSAL + WAVE20-PROPOSAL + WAVE21-PROPOSAL + WAVE22-PROPOSAL (`BASE` / `HEX` / `DECIMAL` — base marks; thin `>NUMBER` → wave22 **3**; do not break HERE stub base)
- Base tip: `9e9987e` / `9e9987e69f9580884d88ce32b3389db11cc921d6` (#97 wave21 tip1 ACCEPT-REFILL PASS)
- Wave21 proposal: `/workspace/tritium-research-docs/WAVE21-PROPOSAL.md`
