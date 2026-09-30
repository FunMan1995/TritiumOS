# ZERO-EQUALS — `0=` (optional `0<>`) flag marks + `zero-demo`

**Status:** Shipper-ready stub spec (wave22 item **2**)
**Canonical brief:** ANS-shaped `0=` (thin flag marks only; optional `0<>`); `docs/TRUE-FALSE.md` (wave20 **1** — TRUE/FALSE constant-picture companion; flag= / all-bits `-1` vs `0`); `docs/BITWISE.md` (wave21 **4** — AND/OR/XOR/INVERT sibling bitwise marks; **not** AND-OR reopen / BITWISE reopen); `docs/LSHIFT-RSHIFT.md` (wave22 **1** — sibling shift marks; **not** LSHIFT reopen; leave LSHIFT-RSHIFT.md primary untouched this tip); `docs/KERNEL.md` (wave7 **5**); optional `docs/WITHIN.md` (wave20 **2** — range-check companion; **not** WITHIN reopen) / `docs/PICK-ROLL.md` (wave15 **2**) / `docs/CELL-CELLS.md` (wave15 **1**) / `docs/HOST-PARITY.md` (wave8 **4**); explicit WAVE19 / WAVE20 / WAVE21 / WAVE22 deferral closed as **flag marks only** (not real boolean cell rewrite / LSHIFT reopen / WITHIN reopen / AND-OR reopen; **CRITICAL:** host kernel already uses `0=` / `0<>` extensively as host Forth primitives — do **not** redefine).
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `zero.fs` / `zero-equals.fs`); Linux host REPL; **CRITICAL — do not** redefine host `0=` / `0<>` (control / loop / find / catch / and other host uses across the kernel load path); prefer Forth mirrors; **do not** redefine `TRUE` / `FALSE` / `true-mark` / `false-mark` (TRUE-FALSE stays); **do not** redefine `AND` / `OR` / `XOR` / `INVERT` / `and-mark` / `or-mark` / `xor-mark` / `invert-mark` (BITWISE stays); **do not** redefine `LSHIFT` / `RSHIFT` / `lshift-mark` / `rshift-mark` (LSHIFT-RSHIFT stays mark-only — leave primary untouched)
**Companions:** `docs/TRUE-FALSE.md` (thin amend this tip), `docs/BITWISE.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip); optional light cite `docs/WITHIN.md` / `docs/PICK-ROLL.md` / `docs/CELL-CELLS.md` / `docs/HOST-PARITY.md`
**Base tip SHA:** `4d53ecb` (wave22 tip1 PASS / #102 LSHIFT-RSHIFT) / full `4d53ecb6af6b436ea04e4df72b90bf75db8355a0`

## 1. Purpose

WAVE15 landed cell-unit + stack/?DUP flag pictures; WAVE20 tip **1** landed `TRUE` / `FALSE` constant marks (`docs/TRUE-FALSE.md` — classic `-1` vs `0`); WAVE20 tip **2** landed `WITHIN` range-check mark (**not** zero-equals). WAVE21 tip **4** landed `AND` / `OR` / `XOR` / `INVERT` bitwise marks (`docs/BITWISE.md`; host lowercase `and`/`or` untouched). WAVE22 tip **1** landed `LSHIFT` / `RSHIFT` shift marks (`docs/LSHIFT-RSHIFT.md`; host lowercase `lshift`/`rshift` untouched; **leave LSHIFT-RSHIFT.md primary untouched this tip**). WAVE18–22 deferred `0=` (optional `0<>`) as a flag mark beside TRUE/FALSE + BITWISE + LSHIFT (**not** real boolean cell rewrite; **not** LSHIFT / WITHIN / AND-OR reopen). This tip lands **stub** flag marks only: `0=` (or Forth mirror **`zero-eq-mark`**) prints `[zero] 0=` (+ optional `flag=` — classic zero→true / nonzero→false); optional `0<>` / **`zero-ne-mark`** prints `[zero] 0<>` (+ optional `flag=`) — **do not require** `0<>` unless free. Smoke via **`zero-demo`**. Prefer **`zero-eq-mark`** whenever host `0=` collides — **CRITICAL:** host kernel already uses **`0=` / `0<>`** extensively (control / loop / find / catch / etc.) — do **NOT** redefine those. Pairs with TRUE/FALSE + BITWISE + tip1 LSHIFT **without** boolean-cell rewrite and **without** reopening LSHIFT / WITHIN / AND-OR / BITWISE. **Not** tip3 TO-NUMBER / tip4 SEARCH-WORDLIST / tip5 DOCS-CITES. Independent of tip3–5.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `0=` / `zero-eq-mark` | `( x -- flag )` *or* `( -- )` with fixed demo fixture | Zero-equals flag mark; print `[zero] 0=` (+ optional `flag=<n>`); classic `0 → flag=true/-1/1`; nonzero → `flag=false/0` picture welcome |
| `0<>` / `zero-ne-mark` | `( x -- flag )` *or* `( -- )` with fixed demo fixture | **Optional** zero-not-equals flag mark; print `[zero] 0<>` (+ optional `flag=<n>`); classic nonzero→true / zero→false; **do not require** unless free |
| `zero-demo` | `( -- )` | See §5 |

Host note: bind bare `0=` / `0<>` on the Linux REPL **only if** those names do not collide with host Forth flag words in the same load path. Prefer Forth mirrors **`zero-eq-mark`** (and optional **`zero-ne-mark`**) as the Lab-facing surface when in doubt — **do not** redefine host `0=`/`0<>`. **CRITICAL:** host **`0=` / `0<>`** (kernel control / loop / find / catch / and other host uses) are **NOT** this tip’s surface and must **not** be redefined, aliased, shadowed, or Lab-grepped as zero-equals success. Optional `flag=` is host int / fixture echo only — not a live boolean cell, not LSHIFT reopen, not AND-OR reopen, not WITHIN reopen. Prefer **fixed demo stub-int fixtures** (classic `0` → true; `1` / nonzero → false for `0=`) so Lab hit is deterministic and FAIL is avoided. Optional `0<>` is welcome when free — do **not** fail Lab if only `0=` / `zero-eq-mark` is greppable.

## 3. Stub semantics

- **`0=` / `zero-eq-mark`:** take (or use fixed demo) stub int `( x )`. Classic ANS / common Forth picture: zero-equals → `flag = true` when `x = 0`, else `flag = false` (document whether Shipper echoes classic all-bits-set / `-1` vs thinner `1` for true — greppable `[zero] 0=` is enough for Lab OK; `-1`/`0` picture pairs with TRUE/FALSE without reopening TRUE-FALSE). Print `[zero] 0=` and optionally `flag=<n>` (Lab-greppable; prefer documenting which Shipper emits). Classic fixture welcome: `0 0= → flag=-1` (or `flag=1`); `1 0= → flag=0` (or nonzero fixture). **Does not** rewrite TRUE/FALSE constants, reopen LSHIFT / BITWISE / WITHIN, bump HERE, open a heap, or create dict entries. Captured values are host ints / fixture echo only.
- **`0<>` / `zero-ne-mark` (optional):** zero-not-equals → `flag = true` when `x <> 0`, else `flag = false`. Print `[zero] 0<>` (+ optional `flag=`). Classic fixture welcome: `1 0<> → flag=-1` / `flag=1`; `0 0<> → flag=0`. **Do not require** `0<>` for Lab OK — only require when Shipper finds the name free or uses the Forth mirror. Same constraints as `0=` — marker only.
- **Fixed demo stub-int fixtures (document):**
  - **Classic zero→true picture (required set):** e.g. `x=0` → `0=` true (`flag=-1` or `flag=1`); `x=1` (or other nonzero) → `0=` false (`flag=0`). Demo must exercise **`0=` / `zero-eq-mark`** so Lab greps `[zero] 0=`. Optional `flag=` welcome.
  - **Optional `0<>` picture:** nonzero → true; zero → false — document if Shipper emits `[zero] 0<>`. Greppable `[zero] 0=` alone is enough for Lab OK when `0<>` is not free.
  - **Optional result echo:** `flag=-1` / `flag=1` / `flag=0` — document which form Shipper emits. Greppable markers alone are enough for Lab OK.
- **Optional push:** if the host stack is easy, push the pictured flag; marker alone is enough for Lab OK — do not require a real boolean cell / flag algebra / LSHIFT reopen / AND-OR reopen / WITHIN reopen.
- **FAIL:** `[zero] FAIL reason=<…>` optional (demo **must avoid**). Prefer not emitting `[zero] FAIL` on the happy path.
- Storage: fixed demo stub ints / host int echo only. **No** real boolean cell rewrite, no LSHIFT reopen (leave tip1 mark-only; leave LSHIFT-RSHIFT.md primary untouched), no AND/OR/XOR/INVERT reopen / BITWISE reopen, no WITHIN reopen, no COMPARE reopen, no BASE-HEX / ACCEPT-REFILL reopen, no HERE bump, no arena.
- **Host `0=` / `0<>` stay untouched:** host `0=` / `0<>` remain kernel Forth primitives (control / loop / find / catch / etc.). This tip’s Lab greps are `[zero] 0=` (and optional `[zero] 0<>`) and `[zero-demo] OK` only — **never** Lab-grep bare host `0=` / `0<>` as this tip’s zero-equals success. Prefer mirrors whenever host `0=`/`0<>` collide.
- **TRUE-FALSE / BITWISE / LSHIFT stay:** do **not** redefine `TRUE` / `FALSE` / `true-mark` / `false-mark`. Do **not** redefine `AND` / `OR` / `XOR` / `INVERT` / `and-mark` / `or-mark` / `xor-mark` / `invert-mark`. Do **not** redefine `LSHIFT` / `RSHIFT` / `lshift-mark` / `rshift-mark`. Flag marks are **siblings** beside constant / bitwise / shift marks — not a reopen of any.
- Nest with prior shift / bit / compare / base / accept / exec / count / within / true / source / env / body / char / state / word / find / tick / create / allot / synonym / exit / buffer / marker / defer / imm / fill / pick / cell / throw / 2var / colon / control / string stubs OK. `dict-reset` unaffected (fixed host fixtures, not dictionary).
- Still no boolean cell rewrite, no `>NUMBER` (tip3), no SEARCH-WORDLIST (tip4), no tip5 DOCS-CITES, no LSHIFT / AND-OR / WITHIN reopen, no linked XT / real DOES> XT / real branch XT / full arena/heap / full Win/Android Forth VM. Wave22 tip1 + wave21 tip1–5 + wave20 tip1–4 + wave19 tip1–4 stay landed — keep cites; this tip does not reopen them. **Leave `LSHIFT-RSHIFT.md` primary untouched** (tip1 land md5 `e5a94d8a16d47aa7ceae92b54344712e` / 26299).

## 4. Markers

```
[zero] 0= [flag=<n>]         # flag= optional; classic 0 → flag=-1/1; nonzero → flag=0 welcome
[zero] 0<> [flag=<n>]        # OPTIONAL; do not require unless free; nonzero → true / zero → false
[zero] FAIL reason=<…>       # demo avoids
[zero-demo] OK
[zero-demo] FAIL
```

Lab greps `[zero-demo] OK` plus greppable **`[zero] 0=`** (optional `flag=` welcome). Optional `[zero] 0<>` welcome when free — **not** required for Lab OK. Demo avoids `[zero] FAIL`. Prefer not emitting `[zero] FAIL` on the happy path. **Do not** Lab-grep host bare `0=` / `0<>` as this tip’s surface. **Do not** Lab-grep `[shift]` / `[bit]` / `[true]` / `[within]` / `[compare]` / `[base]` as ZERO-EQUALS success (those stay their own tips). **Do not** Lab-grep `[shift] LSHIFT` / `[shift] RSHIFT` / `[bit] AND` / `[bit] OR` / `[bit] XOR` / `[bit] INVERT` / `[true] TRUE` / `[true] FALSE` as this tip’s surface (LSHIFT / BITWISE / TRUE-FALSE stay prior tips).

## 5. `zero-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; flag marks need no dict entries.
2. Ensure **fixed demo stub-int fixtures** exist (e.g. `x=0`, `x=1` / nonzero — or equivalent host ints) so the classic zero→true / nonzero→false picture is deterministic. Fixtures may live beside (not replacing) LSHIFT stub-int fixtures / BITWISE stub-int fixtures / TRUE/FALSE constant picture / CELL unit picture / PICK stack picture / WITHIN range-check — document; do **not** require boolean-cell rewrite / LSHIFT reopen / AND-OR reopen / BITWISE reopen / WITHIN reopen / COMPARE reopen.
3. Invoke `0=` (or **`zero-eq-mark`**) against classic zero fixture → `[zero] 0=` (+ optional `flag=` — classic true / `-1` / `1` welcome).
4. Invoke `0=` (or **`zero-eq-mark`**) against classic nonzero fixture → `[zero] 0=` (+ optional `flag=` — classic false / `0` welcome). Document if Shipper emits one combined marker line or two — greppable `[zero] 0=` is enough.
5. **Optional:** invoke `0<>` (or **`zero-ne-mark`**) if free → `[zero] 0<>` (+ optional `flag=`). Do **not** fail Lab if omitted.
6. Assert no `[zero] FAIL` on the happy path. Assert flag marks did **not** require a real boolean cell rewrite / LSHIFT reopen / AND/OR/XOR/INVERT reopen / BITWISE reopen / WITHIN reopen / COMPARE reopen / HERE bump / host `0=`/`0<>` redefine (marker-only is enough). Assert host `0=`/`0<>` were not redefined when using the Forth mirrors. Assert `true-mark` / `false-mark` / `and-mark` / `or-mark` / `xor-mark` / `invert-mark` / `lshift-mark` / `rshift-mark` were **not** redefined. Assert `LSHIFT-RSHIFT.md` primary was left untouched.
7. Prior `shift-demo` / `bit-demo` / `compare-demo` / `base-demo` / `accept-demo` / `exec-demo` / `count-demo` / `within-demo` / `true-demo` / `source-demo` / `env-demo` / `body-demo` / `char-demo` / `state-demo` / `word-demo` / `find-demo` / `tick-demo` / `string-demo` / `fill-demo` / `pick-demo` / `cell-demo` + earlier demos incl `trit-math-demo` / `fold-demo` still OK (host `0=`/`0<>` uses in control / loop / find / catch must remain intact).
8. `[zero-demo] OK`.

Required marker: `[zero] 0=`. Optional `[zero] 0<>` not required for Lab OK. Optional `flag=` echo is not required for Lab OK when `[zero] 0=` is greppable. No real boolean cell. No LSHIFT reopen. No AND-OR reopen. No BITWISE reopen. No WITHIN reopen. No host `0=`/`0<>` redefine.

## 6. Thin amend — companions

### `docs/TRUE-FALSE.md`

- Companions / Status: add `ZERO-EQUALS.md` (wave22 **2** companion cite); **keep** LSHIFT-RSHIFT / BITWISE / WITHIN / BASE-HEX / KERNEL / CELL-CELLS / PICK-ROLL / CONTROL / HOST-PARITY cites — do not wipe wave20 TRUE-FALSE content.
- Purpose / §3 / non-goals: TRUE/FALSE stay constant marks (`flag=` / all-bits `-1` vs `0`); `0=` (optional `0<>`) are sibling **flag marks** beside constants — **not** a TRUE/FALSE reopen / boolean-cell rewrite / LSHIFT reopen / AND-OR reopen / BITWISE reopen / WITHIN reopen. Do not wipe wave20 TRUE-FALSE content. Stress: ZERO-EQUALS sibling flag mark — **NOT** boolean cell / TRUE-FALSE reopen; **CRITICAL:** host `0=`/`0<>` stay untouched; prefer `zero-eq-mark`.
- Non-goals: `0=` / optional `0<>` → `docs/ZERO-EQUALS.md` (wave22 **2**). TRUE/FALSE stay on this tip (already landed). BITWISE stays wave21 **4**. LSHIFT-RSHIFT stays wave22 **1** (leave LSHIFT-RSHIFT.md primary untouched). WITHIN stays wave20 **2**.
- Acceptance: Lab smokes `zero-demo` (retains `shift-demo` + `true-demo` + `bit-demo` + `within-demo` + `base-demo` + `compare-demo`).
- Cite: `docs/ZERO-EQUALS.md`.

### `docs/BITWISE.md`

- Companions / Status: add `ZERO-EQUALS.md` (wave22 **2** companion cite); **keep** LSHIFT-RSHIFT / TRUE-FALSE / KERNEL / CELL-CELLS / PICK-ROLL / WITHIN / HOST-PARITY / COMPARE / BASE-HEX / ACCEPT-REFILL cites — do not wipe wave21 BITWISE content.
- Purpose / §3 / non-goals: AND/OR/XOR/INVERT stay bitwise marks; `0=` (optional `0<>`) are sibling **flag marks** — **not** an AND/OR/XOR/INVERT reopen / BITWISE reopen / boolean-cell rewrite / LSHIFT reopen / WITHIN reopen. Do not wipe wave21 BITWISE content. Stress: ZERO-EQUALS sibling — **NOT** AND-OR reopen; host `0=`/`0<>` stay untouched; host lowercase `and`/`or` stay untouched; do **not** redefine `and-mark`/`or-mark`/`xor-mark`/`invert-mark`.
- Non-goals: `0=` / optional `0<>` → `docs/ZERO-EQUALS.md` (wave22 **2**). AND/OR/XOR/INVERT stay on this tip (already landed). TRUE/FALSE stay wave20 **1**. LSHIFT-RSHIFT stays wave22 **1**. WITHIN stays wave20 **2**. Tip5 DOCS-CITES still later (wave22 **5**).
- Acceptance: Lab smokes `zero-demo` (retains `bit-demo` + `shift-demo` + `true-demo` + `within-demo` + `base-demo` + `compare-demo` + `accept-demo`).
- Cite: `docs/ZERO-EQUALS.md`.

### `docs/KERNEL.md`

- Companions: add `ZERO-EQUALS.md` (wave22 **2**); **keep** wave22 tip1 LSHIFT-RSHIFT cite and wave21 tip1–4 ACCEPT-REFILL / BASE-HEX / COMPARE / BITWISE cites and wave20 tip1–4 TRUE-FALSE / WITHIN / COUNT / EXECUTE cites and wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites and wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites and wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `0=` / `zero-eq-mark` stubs + optional `0<>` / `zero-ne-mark` + `zero-demo` (cite tip; Forth mirrors `zero-eq-mark` / optional `zero-ne-mark` — flag marks only; **CRITICAL:** do **not** redefine host `0=`/`0<>`; prefer mirrors; **do not** redefine `TRUE`/`FALSE`/`true-mark`/`false-mark`; **do not** redefine `AND`/`OR`/`XOR`/`INVERT`/`and-mark`/`or-mark`/`xor-mark`/`invert-mark`; **do not** redefine `LSHIFT`/`RSHIFT`/`lshift-mark`/`rshift-mark`; **not** real boolean cell rewrite / LSHIFT reopen / AND-OR reopen / BITWISE reopen / WITHIN reopen; optional `flag=` — classic zero→true / nonzero→false picture welcome).
- Non-goals: `0=` flag marks → `docs/ZERO-EQUALS.md`. LSHIFT/RSHIFT stay on `LSHIFT-RSHIFT.md`. AND/OR/XOR/INVERT stay on `BITWISE.md`. COMPARE stays on `COMPARE.md`. BASE/HEX/DECIMAL stay on `BASE-HEX.md`. ACCEPT/REFILL stay on `ACCEPT-REFILL.md`. TRUE/FALSE stay on `TRUE-FALSE.md`. WITHIN stays on `WITHIN.md`. host `0=`/`0<>` stay kernel Forth primitives — **not** this tip. Tip3 TO-NUMBER / tip4 SEARCH-WORDLIST / tip5 DOCS-CITES still later (wave22 **3–5**).
- Acceptance: Lab smokes `zero-demo` (and retains `shift-demo` + `bit-demo` + `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `source-demo` + `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `string-demo` + `fill-demo` + prior demos incl `trit-math-demo` / `fold-demo`).
- Cite: `docs/ZERO-EQUALS.md`.

### Optional — `docs/WITHIN.md`

- Companions / Status: add light `ZERO-EQUALS.md` (wave22 **2**) cite; **keep** BITWISE / COMPARE / TRUE-FALSE / LSHIFT-RSHIFT cites — do not wipe wave20 WITHIN content.
- Purpose / non-goals: WITHIN stays range-check mark; `0=` is sibling **flag mark** — **NOT** WITHIN reopen / runtime compare re-exec / boolean cell / LSHIFT reopen / AND-OR reopen. Host `0=`/`0<>` stay untouched.
- Acceptance: Lab smokes `zero-demo` (retains `within-demo` + `shift-demo` + `true-demo` + `bit-demo`). Cite: `docs/ZERO-EQUALS.md`.

### Optional — `docs/PICK-ROLL.md`

- Companions / Status: add light `ZERO-EQUALS.md` (wave22 **2**) cite; **keep** LSHIFT-RSHIFT / BITWISE / TRUE-FALSE / WITHIN cites — do not wipe wave15 PICK-ROLL content.
- Purpose / non-goals: stack/?DUP flag pictures stay; `0=` sibling **flag** mark — **not** PICK/?DUP reopen / boolean cell / LSHIFT reopen / AND-OR reopen. Host `2dup`/`2drop`/`2swap` and host `0=`/`0<>` stay untouched.
- Acceptance: Lab smokes `zero-demo` (retains `pick-demo` + `shift-demo` + `bit-demo` + `true-demo`). Cite: `docs/ZERO-EQUALS.md`.

### Optional — `docs/CELL-CELLS.md`

- Companions / Status: add light `ZERO-EQUALS.md` (wave22 **2**) cite; **keep** LSHIFT-RSHIFT / BITWISE / TRUE-FALSE / WITHIN / BASE-HEX cites — do not wipe wave15 CELL content.
- Purpose / non-goals: cell-unit stubs stay; `0=` may echo optional `flag=` on cell-sized stub ints — **not** CELL/ALIGN reopen / boolean cell / LSHIFT reopen / BITWISE reopen. Host `0=`/`0<>` stay untouched.
- Acceptance: Lab smokes `zero-demo` (retains `cell-demo` + `shift-demo` + `bit-demo` + `true-demo`). Cite: `docs/ZERO-EQUALS.md`.

### Optional — `docs/HOST-PARITY.md`

- Companions: add light `ZERO-EQUALS.md` (wave22 **2**) cite; **keep** LSHIFT-RSHIFT / BITWISE / TRUE-FALSE / BASE-HEX cites.
- Purpose / non-goals: `zero-demo CONTRACT` welcome — **not** full Win/Android Forth VM / boolean-cell port. Host `0=`/`0<>` stay untouched on Linux SoT.
- Acceptance: Lab smokes `zero-demo` (Win/Android: `zero-demo CONTRACT` OK). Cite: `docs/ZERO-EQUALS.md`.

Do **not** wipe wave22 tip1 / wave21 tip1–5 / wave20 tip1–4 / wave19 tip1–4 / wave18–15 prior content. Do **not** amend ARCHITECTURE / IMPLEMENTATION-GAPS / LSHIFT-RSHIFT / ACCEPT-REFILL / BASE-HEX / COMPARE primary this tip (amends are TRUE-FALSE + BITWISE + KERNEL + optional WITHIN / PICK-ROLL / CELL-CELLS / HOST-PARITY only). **Leave `LSHIFT-RSHIFT.md` untouched** (`e5a94d8a16d47aa7ceae92b54344712e` / 26299). **Leave `ACCEPT-REFILL.md` / `BASE-HEX.md` / `COMPARE.md` untouched** (`6bda5a5170ac71fa3ccf7db28286dd44` / `d12dd22a8502b321568fa370b6ecdd2e` / `dc429e1f010818957353c216adb5c317`). **Leave `ARCHITECTURE.md` / `IMPLEMENTATION-GAPS.md` untouched** (`b924e9ce5c1e14efd8e0ae738dde5f51` / `712d77305646b9275cb4e76195f00f38`). **Leave `WAVE22-PROPOSAL.md` / `WAVE22-COS-PASTE.txt` untouched.** Tip5 cites after 1–4 PASS. Skip 2DUP-FAMILY + ABORT" polish.

## 7. Non-goals

- Redefining / aliasing / shadowing / Lab-grepping host `0=` / `0<>` as this tip’s zero-equals surface
- Redefining `TRUE` / `FALSE` / `true-mark` / `false-mark` (TRUE-FALSE stays — **not** boolean cell / TRUE-FALSE reopen)
- Redefining `AND` / `OR` / `XOR` / `INVERT` / `and-mark` / `or-mark` / `xor-mark` / `invert-mark` (BITWISE stays — **not** AND-OR reopen / BITWISE reopen)
- Redefining `LSHIFT` / `RSHIFT` / `lshift-mark` / `rshift-mark` (LSHIFT-RSHIFT stays — **not** LSHIFT reopen; leave LSHIFT-RSHIFT.md primary untouched)
- Real boolean cell rewrite / live flag storage / VARIABLE-as-boolean
- `LSHIFT` / `RSHIFT` reopen (wave22 **1** — already stubbed; keep cites; leave primary untouched)
- `WITHIN` reopen (wave20 **2** — already stubbed; keep cites — **not** runtime compare re-exec)
- `TRUE` / `FALSE` reopen (wave20 **1** — already stubbed; keep cites; thin companion amend only — pairs without boolean-cell rewrite)
- `BITWISE` reopen (wave21 **4** — already stubbed; thin companion amend only — flag marks are siblings, not reopen)
- `COMPARE` / `BASE-HEX` / `ACCEPT-REFILL` reopen (wave21 **1–3** — already stubbed; leave those primaries untouched)
- `>NUMBER` / pictured numeric `#`/`HOLD`/`<#`/`#>` (tip3 TO-NUMBER — still deferred; do **not** bump HERE stub `$1000`)
- `SEARCH-WORDLIST` / FIND reopen (tip4 — still deferred)
- Docs cites pass (wave22 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `COUNT` / `EXECUTE` / `SOURCE` / `PAD` reopen (wave20 **3–4** / wave19 **4** — already stubbed; keep cites)
- `CELL` / `PICK` / `IF` reopen (wave15 / wave10 — already stubbed; flag marks on cell-sized ints — not CELL/PICK/?DUP/IF reopen)
- Real DOES> XT / real branch XT / LEAVE jump / full arena / linked XT / real STATE cell
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (**skip 2DUP-FAMILY**); `ABORT"` polish (**skip**)
- Real crypto / network fleet / opaque-weight ML; full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/ZERO-EQUALS.md` present (Research byte-copy OK); `TRUE-FALSE.md` + `BITWISE.md` + `KERNEL.md` thin amends present (+ optional `WITHIN.md` / `PICK-ROLL.md` / `CELL-CELLS.md` / `HOST-PARITY.md`); wave22 tip1 + wave21 tip1–5 + wave20 tip1–4 + wave19 tip1–4 + wave18–15 prior cites retained; host `0=`/`0<>` **not** redefined / aliased / shadowed / Lab-grepped as zero-equals success; true-mark/false-mark / and-mark/or-mark/xor-mark/invert-mark / lshift-mark/rshift-mark **not** redefined; `LSHIFT-RSHIFT.md` primary untouched (`e5a94d8a16d47aa7ceae92b54344712e` / 26299); ACCEPT-REFILL / BASE-HEX / COMPARE primaries untouched; ARCHITECTURE + IMPLEMENTATION-GAPS unchanged (`b924e9ce5c1e14efd8e0ae738dde5f51` / `712d77305646b9275cb4e76195f00f38`); WAVE22-PROPOSAL / WAVE22-COS-PASTE untouched.
2. `zero-demo` → OK (markers §4; `[zero] 0=` greppable; optional `flag=` welcome — classic zero→true / nonzero→false picture on stub ints; optional `[zero] 0<>` welcome when free — not required; no FAIL on happy path; no real boolean cell / LSHIFT reopen / AND-OR reopen / BITWISE reopen / WITHIN reopen / COMPARE reopen; no host `0=`/`0<>` redefine). Prior `shift-demo` + `bit-demo` + `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `source-demo` / `env` / `body` / `char` / `state` / `word` / `find` / `tick` + earlier demos incl `trit-math-demo` / `fold-demo` still OK.
3. Regression green (wave22 tip1 + wave21 tip1–5 + wave20 tip1–5 + wave19 tip1–5 + wave18 tip1–5 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `zero-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**)
- `docs/TRUE-FALSE.md` (wave20 **1** — TRUE/FALSE constant-picture companion; ZERO-EQUALS sibling flag mark — **not** boolean-cell rewrite / TRUE-FALSE reopen; host `0=`/`0<>` stay untouched)
- `docs/BITWISE.md` (wave21 **4** — AND/OR/XOR/INVERT sibling bitwise marks; ZERO-EQUALS sibling flag marks — **not** AND/OR/XOR/INVERT reopen / BITWISE reopen; host lowercase `and`/`or` stay untouched)
- `docs/LSHIFT-RSHIFT.md` (wave22 **1** — sibling shift marks; **not** LSHIFT reopen; leave LSHIFT-RSHIFT.md primary untouched this tip; host lowercase `lshift`/`rshift` stay untouched)
- `docs/WITHIN.md` (wave20 **2**, optional — range-check companion; **not** WITHIN reopen / runtime compare re-exec)
- `docs/CELL-CELLS.md` (wave15 **1**, optional — unit/cell picture companion; flag marks on cell-sized ints — not CELL reopen)
- `docs/PICK-ROLL.md` (wave15 **2**, optional — stack/?DUP flag companion; not PICK/?DUP reopen)
- `docs/HOST-PARITY.md` (wave8 **4**, optional — `zero-demo` CONTRACT parity welcome)
- `docs/COMPARE.md` (wave21 **3** — prior tip; keep cites; leave primary untouched this tip; host `cstr=` ≠ ANS COMPARE)
- `docs/BASE-HEX.md` (wave21 **2** — prior tip; keep cites; leave primary untouched this tip)
- `docs/ACCEPT-REFILL.md` (wave21 **1** — prior tip; keep cites; leave primary untouched this tip)
- `docs/COUNT.md` (wave20 **3** — prior tip; keep cites)
- `docs/EXECUTE.md` (wave20 **4** — prior tip; keep cites)
- `docs/SOURCE-PAD.md` (wave19 **4** — prior tip; keep cites)
- `docs/CONTROL.md` (wave10 **2** — IF `taken=` companion; ZERO-EQUALS is flag mark — not IF/THEN reopen / branch XT)
- `forth/tritium/kernel.fs` (zero-eq-mark / optional zero-ne-mark only — do not redefine host `0=`/`0<>`; **do not** redefine true-mark/false-mark; **do not** redefine and-mark/or-mark/xor-mark/invert-mark; **do not** redefine lshift-mark/rshift-mark)
- ANS Forth `0=` / optional `0<>` (flag marks only — not real boolean cell rewrite / LSHIFT reopen / AND-OR reopen / WITHIN reopen; host `0=`/`0<>` ≠ this tip)
- Explicit deferral: WAVE19-PROPOSAL + WAVE20-PROPOSAL + WAVE21-PROPOSAL + WAVE22-PROPOSAL (`0=` / optional `0<>` — flag marks; not boolean cell rewrite / LSHIFT reopen / WITHIN reopen / AND-OR reopen)
- Base tip: `4d53ecb` / `4d53ecb6af6b436ea04e4df72b90bf75db8355a0` (#102 wave22 tip1 LSHIFT-RSHIFT PASS)
- Wave22 proposal: `/workspace/tritium-research-docs/WAVE22-PROPOSAL.md`
