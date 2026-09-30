# ABS-NEGATE — `ABS` / `NEGATE` signed magnitude / negate marks + `abs-demo`

**Status:** Shipper-ready stub spec (wave23 item **4**; thin amend wave24 **2** MIN-MAX companion cite)
**Canonical brief:** ANS-shaped `ABS` / `NEGATE` (thin signed magnitude / negate marks only); `docs/BITWISE.md` (wave21 **4** — bitwise companion; **not** BITWISE reopen; do not redefine `and-mark`/`or-mark`/`xor-mark`/`invert-mark`); `docs/TRUE-FALSE.md` (wave20 **1** — constant-picture companion; **not** boolean cell; do not redefine `true`/`false` / `true-mark`/`false-mark`); `docs/LSHIFT-RSHIFT.md` (wave22 **1** — shift companion; **not** LSHIFT reopen; leave primary untouched; do not redefine `lshift-mark`/`rshift-mark`); `docs/KERNEL.md` (wave7 **5**); optional `docs/PICK-ROLL.md` / `docs/HOST-PARITY.md`; WAVE19–23 deferral closed as **signed magnitude / negate marks only** (not boolean cell / BITWISE / LSHIFT / ZERO-EQUALS / U-LESS / HOLD / CHAR-PLUS / COMPARE reopen; prefer Forth mirrors **`abs-mark` / `negate-mark`** whenever host `ABS`/`NEGATE` collide).
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `abs.fs` / `abs-negate.fs`); Linux host REPL; prefer Forth mirrors **`abs-mark` / `negate-mark`** whenever host `ABS`/`NEGATE` collide; **do not** redefine `and-mark`/`or-mark`/`xor-mark`/`invert-mark` / `lshift-mark`/`rshift-mark` / `TRUE`/`FALSE`/`true-mark`/`false-mark` / `zero-eq-mark`/`zero-ne-mark` / `u-less-mark` / `hold-mark` / `char-plus-mark` / `compare-mark` / `within-mark` (those stay prior tips — leave their primaries untouched as listed in §8)
**Companions:** `docs/BITWISE.md` (thin amend this tip), `docs/TRUE-FALSE.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip); optional light cite `docs/PICK-ROLL.md` / `docs/HOST-PARITY.md`; `docs/MIN-MAX.md` (wave24 **2** — MIN/MAX signed min/max companion cite; **not** ABS reopen; prefer `min-mark`/`max-mark`; do not redefine `abs-mark`/`negate-mark`; **CRITICAL:** kernel `MAX-ENTRIES`/`MAX-SYNONYMS`/`MAX-CHAR` ≠ ANS `MAX`)
**Base tip SHA:** `f76ca819` (wave23 tip3 U-LESS PASS / #109) / full `f76ca819fd71bb67e91a4a3228746c02af770d3e`

## 1. Purpose

WAVE20 tip **1** landed `TRUE` / `FALSE` constant marks (`docs/TRUE-FALSE.md` — classic `-1` vs `0`). WAVE21 tip **4** landed `AND` / `OR` / `XOR` / `INVERT` bitwise marks (`docs/BITWISE.md` — classic `0xFF` picture; host lowercase `and`/`or` untouched). WAVE22 tip **1** landed `LSHIFT` / `RSHIFT` shift marks (`docs/LSHIFT-RSHIFT.md` — classic `1 LSHIFT 4 → 16` picture; host lowercase `lshift`/`rshift` untouched). WAVE22 tip **2** landed `0=` / optional `0<>` flag marks (`docs/ZERO-EQUALS.md`; leave as tip3 thin — **not** a ZERO-EQUALS reopen this tip). WAVE23 tip **1** landed `CHAR+` thin char-unit advance (`docs/CHAR-PLUS.md`; leave primary untouched). WAVE23 tip **2** landed `HOLD` thin pictured-numeric start (`docs/HOLD.md`; leave primary untouched — **CRITICAL** HERE stub `$1000` stays). WAVE23 tip **3** landed `U<` unsigned compare flag mark (`docs/U-LESS.md`; leave primary untouched this tip). WAVE18–23 deferred `ABS` / `NEGATE` as signed magnitude / negate marks beside TRUE-FALSE + BITWISE + LSHIFT (**not** boolean cell rewrite; **not** BITWISE reopen; **not** LSHIFT reopen). This tip lands **stub** signed magnitude / negate marks only: `ABS` (or Forth mirror **`abs-mark`**) prints `[abs] ABS` (+ optional `n=` / `u=` — classic absolute-value picture welcome: **|n|** on fixed signed stub ints); `NEGATE` (or Forth mirror **`negate-mark`**) prints `[abs] NEGATE` (+ optional `n=` — classic two’s-complement negate picture welcome: **-n**). Smoke via **`abs-demo`**. Prefer Forth mirrors **`abs-mark` / `negate-mark`** whenever host `ABS`/`NEGATE` collide. Pairs with wave20 TRUE-FALSE + wave21 BITWISE + wave22 LSHIFT **without** a boolean-cell rewrite and **without** reopening BITWISE or LSHIFT. **Not** tip5 DOCS-CITES. Independent of tip5. **Leave untouched this tip:** U-LESS.md (`1f17242555fde002eaf2e7dbabadc5d6` / 30870), HOLD.md (`dcc8a16c5f2999dd3aba7d828383f475` / 30969), CHAR-PLUS.md (`bb879e6fe97f8f8ebd25418d43b43751` / 30666), LSHIFT-RSHIFT.md (`e5a94d8a16d47aa7ceae92b54344712e`), ZERO-EQUALS.md tip3 thin (`0ccdfe471e323cbd93343ae06398c089`).  Sibling `MIN` / `MAX` signed min/max marks → `docs/MIN-MAX.md` (wave24 **2**; Forth mirrors `min-mark`/`max-mark`; **not** an ABS-NEGATE reopen / boolean-cell rewrite; prefer mirrors whenever host `MIN`/`MAX` collide; **do not** redefine `abs-mark`/`negate-mark`; **CRITICAL:** kernel `MAX-ENTRIES`/`MAX-SYNONYMS`/`MAX-CHAR` are **NOT** ANS `MAX`). Prefer **not** amending WITHIN (optional cites → PICK-ROLL + HOST-PARITY).

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `ABS` / `abs-mark` | `( n -- u )` *or* `( -- )` with fixed demo fixture | Signed absolute-value / magnitude mark; print `[abs] ABS` (+ optional `n=<signed>` / `u=<magnitude>`); classic `\|n\|` picture welcome |
| `NEGATE` / `negate-mark` | `( n1 -- n2 )` *or* `( -- )` with fixed demo fixture | Two’s-complement negate mark; print `[abs] NEGATE` (+ optional `n=<result>`); classic `-n` picture welcome |
| `abs-demo` | `( -- )` | See §5 |

Host note: bind bare `ABS` / `NEGATE` on the Linux REPL **only if** those names do not collide with host Forth `ABS`/`NEGATE`. Prefer Forth mirrors **`abs-mark` / `negate-mark`** when in doubt — **do not** redefine host `ABS`/`NEGATE`. Optional `n=` / `u=` are host ints / fixture echo only — not a live boolean cell / BITWISE / LSHIFT / ZERO-EQUALS / U-LESS / HOLD / CHAR-PLUS reopen. Prefer **fixed demo signed stub-int fixtures** so Lab hit is deterministic. Classic ABS: `ABS -5 → u=5` (optional `ABS 0` / `ABS 7`). Classic NEGATE: `NEGATE 5 → n=-5` (optional `NEGATE -3` / `NEGATE 0`). Greppable `[abs] ABS` + `[abs] NEGATE` are enough for Lab OK. **Do not** redefine prior mirrors listed in Sources of truth.


## 3. Stub semantics

- **`ABS` / `abs-mark`:** take (or use fixed demo) signed stub int `( n )`. Classic ANS / common Forth picture: absolute value → `u = |n|` (magnitude; non-negative). Print `[abs] ABS` and optionally `n=<signed input>` and/or `u=<magnitude>` (Lab-greppable; prefer documenting which Shipper emits). Classic fixture welcome: `n=-5` → `u=5` (or `ABS -5 → u=5`); `n=0` → `u=0`; `n=7` → `u=7`. **Does not** rewrite TRUE/FALSE constants, reopen BITWISE / LSHIFT / ZERO-EQUALS / U-LESS / HOLD / CHAR-PLUS, bump HERE, open a heap, or create dict entries. Captured values are host ints / fixture echo only.
- **`NEGATE` / `negate-mark`:** take (or use fixed demo) signed stub int `( n1 )`. Classic ANS / common Forth picture: two’s-complement negate → `n2 = -n1`. Print `[abs] NEGATE` and optionally `n=<result>` (and/or input echo — document). Classic fixture welcome: `n1=5` → `n=-5`; `n1=-3` → `n=3`; `n1=0` → `n=0`. Same constraints as ABS — marker only. **Does not** reopen BITWISE INVERT (ones-complement ≠ two’s-complement negate) / TRUE-FALSE / boolean cell.
- **Fixed demo signed stub-int fixtures (document):**
  - **Classic ABS negative→magnitude (required set):** e.g. `n=-5` → `ABS` → `u=5` (or equivalent). Demo must exercise **`ABS` / `abs-mark`** so Lab greps `[abs] ABS`. Optional `n=` / `u=` welcome.
  - **Classic NEGATE positive→negative (required set):** e.g. `n1=5` → `NEGATE` → `n=-5` (or equivalent). Demo must exercise **`NEGATE` / `negate-mark`** so Lab greps `[abs] NEGATE`. Optional `n=` welcome.
  - **Optional zero / positive ABS / negative NEGATE pictures:** `ABS 0` / `ABS 7` / `NEGATE -3` / `NEGATE 0` — welcome when free; do **not** require for Lab OK; do **not** promote to boolean cell / BITWISE reopen / LSHIFT reopen.
  - **Optional result echo:** `n=-5` / `u=5` / `n=5` — document which form Shipper emits. Greppable markers alone are enough for Lab OK when both `[abs] ABS` and `[abs] NEGATE` appear.
- **Optional push:** if the host stack is easy, push the pictured result; marker alone is enough for Lab OK — do not require a real boolean cell / flag algebra / BITWISE reopen / LSHIFT reopen / ZERO-EQUALS reopen.
- **FAIL:** `[abs] FAIL reason=<…>` optional (demo **must avoid**). Prefer not emitting `[abs] FAIL` on the happy path. No required FAIL reason this tip — missing args / type-miss stay out of the demo (or document as optional miss picture that still avoids FAIL).
- Storage: fixed demo signed stub ints / host int echo only. **No** real boolean cell rewrite, no BITWISE reopen (leave wave21 bitwise marks; thin companion amend only), no LSHIFT reopen (leave LSHIFT-RSHIFT.md primary untouched), no ZERO-EQUALS reopen (leave ZERO-EQUALS.md tip3 thin as-is), no U-LESS reopen (leave U-LESS.md primary untouched), no HOLD reopen (leave HOLD.md primary untouched), no CHAR-PLUS reopen (leave CHAR-PLUS.md primary untouched), no COMPARE reopen, no HERE bump, no arena.
- **Host `ABS` / `NEGATE` stay untouched when colliding:** prefer mirrors whenever host `ABS`/`NEGATE` collide. This tip’s Lab greps are `[abs] ABS`, `[abs] NEGATE`, and `[abs-demo] OK` only — **never** Lab-grep bare host `ABS`/`NEGATE` as this tip’s success when using the Forth mirror path.
- **BITWISE / LSHIFT / TRUE-FALSE / ZERO-EQUALS / U-LESS / HOLD / CHAR-PLUS / COMPARE / WITHIN stay:** do **not** redefine `and-mark`/`or-mark`/`xor-mark`/`invert-mark`. Do **not** redefine `lshift-mark`/`rshift-mark`. Do **not** redefine `TRUE`/`FALSE`/`true-mark`/`false-mark`. Do **not** redefine `zero-eq-mark`/`zero-ne-mark`. Do **not** redefine `u-less-mark`. Do **not** redefine `hold-mark`. Do **not** redefine `char-plus-mark`. Do **not** redefine `compare-mark`/`within-mark`. Signed magnitude / negate marks are **siblings** beside bitwise / shift / constant / flag / unsigned-compare marks — not a reopen of any.
- Nest with prior uless / hold / charplus / search / number / zero / shift / bit / compare / base / accept / exec / count / within / true + earlier stubs OK. `dict-reset` unaffected. Assert prior `uless-demo` / `hold-demo` / `charplus-demo` / `zero-demo` / `shift-demo` / `bit-demo` / `true-demo` still OK.
- Still no boolean cell; no BITWISE / LSHIFT / ZERO-EQUALS / U-LESS / HOLD / CHAR-PLUS reopen; no tip5 DOCS-CITES; no linked XT / real DOES> XT / full arena / full Win/Android Forth VM. Keep wave23 tip1–3 + wave22–15 cites. Leave U-LESS / HOLD / CHAR-PLUS / LSHIFT-RSHIFT / ZERO-EQUALS (tip3 thin) primaries byte-identical (md5s §1 / §8).

## 4. Markers

```
[abs] ABS [n=<signed>] [u=<magnitude>]   # n=/u= optional; classic |n| picture welcome
[abs] NEGATE [n=<result>]                # n= optional; classic two's-complement -n picture welcome
[abs] FAIL reason=<…>                    # demo avoids
[abs-demo] OK
[abs-demo] FAIL
```

Lab greps `[abs-demo] OK` plus greppable **`[abs] ABS`** and greppable **`[abs] NEGATE`** (optional `n=` / `u=` welcome — classic absolute-value / negate pictures on fixed fixtures). Demo avoids `[abs] FAIL`. Prefer not emitting `[abs] FAIL` on the happy path. **Do not** Lab-grep `[bit]` / `[shift]` / `[true]` / `[zero]` / `[uless]` / `[hold]` / `[char+]` / `[compare]` / `[within]` as ABS-NEGATE success (those stay their own tips). **Do not** Lab-grep `[bit] AND` / `[bit] INVERT` / `[shift] LSHIFT` / `[true] TRUE` / `[zero] 0=` / `[uless] U<` / `[hold] HOLD` / `[char+] CHAR+` as this tip’s surface (BITWISE / LSHIFT / TRUE-FALSE / ZERO-EQUALS / U-LESS / HOLD / CHAR-PLUS stay prior tips). **Do not** Lab-grep DOCS-CITES / ARCHITECTURE / GAPS edits as this tip (those stay tip5).

## 5. `abs-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; signed magnitude / negate marks need no dict entries.
2. Ensure **fixed demo signed stub-int fixtures** exist (e.g. for ABS: `n=-5` → magnitude `5`; for NEGATE: `n1=5` → `-5` — or equivalent host ints) so the classic absolute-value / negate pictures are deterministic. Fixtures may live beside (not replacing) BITWISE stub-int fixtures / LSHIFT shift fixtures / TRUE/FALSE constant picture / ZERO-EQUALS stub-int fixtures / U-LESS unsigned fixtures / CELL unit picture / HOLD char fixture / CHAR-PLUS advance picture — document; do **not** require boolean-cell rewrite / BITWISE reopen / LSHIFT reopen / ZERO-EQUALS reopen / U-LESS reopen / HOLD reopen / CHAR-PLUS reopen.
3. Invoke `ABS` (or **`abs-mark`**) against classic negative→magnitude fixture → `[abs] ABS` (+ optional `n=` / `u=` — classic `|n|` welcome).
4. Invoke `NEGATE` (or **`negate-mark`**) against classic positive→negative fixture → `[abs] NEGATE` (+ optional `n=` — classic `-n` welcome).
5. Optional (when free): zero / positive ABS / negative NEGATE pictures — **do not require**.
6. Assert no `[abs] FAIL` on the happy path. Assert no boolean cell / BITWISE / LSHIFT / ZERO-EQUALS / U-LESS / HOLD / CHAR-PLUS reopen / HERE bump / host `ABS`/`NEGATE` redefine when using mirrors. Assert prior mirrors (`and-mark`…`within-mark` list in §2) were **not** redefined. Assert U-LESS / HOLD / CHAR-PLUS / LSHIFT-RSHIFT / ZERO-EQUALS (tip3 thin) primaries left untouched. Assert prior `uless-demo` / `hold-demo` / `charplus-demo` / `zero-demo` / `shift-demo` / `bit-demo` / `true-demo` still OK.
7. Prior `uless-demo` / `hold-demo` / `charplus-demo` / `search-demo` / `number-demo` / `zero-demo` / `shift-demo` / `bit-demo` / `compare-demo` / `base-demo` / `accept-demo` / `exec-demo` / `count-demo` / `within-demo` / `true-demo` + earlier incl `trit-math-demo` / `fold-demo` still OK.
8. `[abs-demo] OK`.

Required: `[abs] ABS` + `[abs] NEGATE`. Optional `n=` / `u=` not required for Lab OK. No boolean cell; no BITWISE / LSHIFT / ZERO-EQUALS / U-LESS / HOLD / CHAR-PLUS reopen; no host `ABS`/`NEGATE` redefine when using the mirrors.

## 6. Thin amend — companions

### `docs/BITWISE.md`

- Companions / Status: add `ABS-NEGATE.md` (wave23 **4** companion cite); **keep** TRUE-FALSE / LSHIFT-RSHIFT / ZERO-EQUALS / KERNEL / CELL-CELLS / PICK-ROLL / WITHIN / HOST-PARITY cites — do not wipe wave21 BITWISE content.
- Purpose / §3 / non-goals: AND/OR/XOR/INVERT stay bitwise marks; `ABS`/`NEGATE` are sibling **signed magnitude / negate marks** — **not** a BITWISE reopen / boolean-cell rewrite / LSHIFT reopen / INVERT-as-NEGATE confuse / ZERO-EQUALS reopen. Do not wipe wave21 BITWISE content. Stress: ABS-NEGATE sibling magnitude/negate — **NOT** BITWISE reopen; prefer `abs-mark`/`negate-mark` whenever host `ABS`/`NEGATE` collide; **do not** redefine `and-mark`/`or-mark`/`xor-mark`/`invert-mark`; host lowercase `and`/`or` stay untouched. NEGATE (two’s-complement) ≠ INVERT (ones-complement).
- Non-goals: `ABS` / `NEGATE` → `docs/ABS-NEGATE.md` (wave23 **4**). AND/OR/XOR/INVERT / `and-mark`/`or-mark`/`xor-mark`/`invert-mark` stay on this tip (already landed). LSHIFT stays wave22 **1**. TRUE-FALSE stays wave20 **1**. Tip5 DOCS-CITES still later (wave23 **5**).
- Acceptance: Lab smokes `abs-demo` (retains `bit-demo` + `shift-demo` + `true-demo` + `zero-demo` + `uless-demo` + `hold-demo` + `charplus-demo`).
- Cite: `docs/ABS-NEGATE.md`.

### `docs/TRUE-FALSE.md`

- Companions / Status: add `ABS-NEGATE.md` (wave23 **4** companion cite); **keep** WITHIN / BASE-HEX / BITWISE / LSHIFT-RSHIFT / ZERO-EQUALS / U-LESS / KERNEL / CELL-CELLS / PICK-ROLL / CONTROL / HOST-PARITY cites — do not wipe wave20 TRUE-FALSE content.
- Purpose / §3 / non-goals: TRUE/FALSE stay constant marks; `ABS`/`NEGATE` are sibling **signed magnitude / negate marks** — **not** a TRUE-FALSE reopen / boolean-cell rewrite / BITWISE reopen / LSHIFT reopen / ZERO-EQUALS reopen / U-LESS reopen. Do not wipe wave20 TRUE-FALSE content. Stress: ABS-NEGATE sibling magnitude/negate — **NOT** boolean cell / TRUE-FALSE reopen; prefer `abs-mark`/`negate-mark`; **do not** redefine `true-mark`/`false-mark` / `TRUE`/`FALSE`.
- Non-goals: `ABS` / `NEGATE` → `docs/ABS-NEGATE.md` (wave23 **4**). TRUE/FALSE / `true-mark`/`false-mark` stay on this tip (already landed). BITWISE stays wave21 **4**. LSHIFT stays wave22 **1**. U-LESS stays wave23 **3**. Tip5 DOCS-CITES still later.
- Acceptance: Lab smokes `abs-demo` (retains `true-demo` + `bit-demo` + `shift-demo` + `zero-demo` + `uless-demo` + `hold-demo` + `charplus-demo`).
- Cite: `docs/ABS-NEGATE.md`.

### `docs/KERNEL.md`

- Companions: add `ABS-NEGATE.md` (wave23 **4**); **keep** wave23 tip1–3 CHAR-PLUS / HOLD / U-LESS cites and wave22 tip1–4 LSHIFT-RSHIFT / ZERO-EQUALS / TO-NUMBER / SEARCH-WORDLIST cites and wave21 tip1–4 ACCEPT-REFILL / BASE-HEX / COMPARE / BITWISE cites and wave20 tip1–4 / wave19 tip1–4 / wave18 tip1–4 / wave17 tip1–4 / wave16 DEFER/MARKER/BUFFER/EXIT / wave15 CELL cites.
- Words table: add `ABS` / `abs-mark` + `NEGATE` / `negate-mark` stubs + `abs-demo` (cite tip; Forth mirrors `abs-mark`/`negate-mark` — signed magnitude / negate marks only; prefer mirrors whenever host `ABS`/`NEGATE` collide; **do not** redefine `and-mark`/`or-mark`/`xor-mark`/`invert-mark` / `lshift-mark`/`rshift-mark` / `TRUE`/`FALSE`/`true-mark`/`false-mark` / `zero-eq-mark`/`zero-ne-mark` / `u-less-mark` / `hold-mark` / `char-plus-mark` / `compare-mark` / `within-mark`; **not** boolean cell rewrite; **not** BITWISE reopen; **not** LSHIFT reopen; **not** ZERO-EQUALS reopen; **not** U-LESS reopen; **not** HOLD reopen; **not** CHAR-PLUS reopen; optional `n=` / `u=` — classic absolute-value / two’s-complement negate pictures welcome).
- Non-goals: `ABS` / `NEGATE` signed magnitude / negate marks → `docs/ABS-NEGATE.md`. AND/OR/XOR/INVERT stay on `BITWISE.md` (thin companion amend — do not wipe). TRUE/FALSE stay on `TRUE-FALSE.md` (thin companion amend — do not wipe). LSHIFT/RSHIFT stay on `LSHIFT-RSHIFT.md` (leave primary untouched). `0=` stays on `ZERO-EQUALS.md` (leave tip3 thin as-is). `U<` stays on `U-LESS.md` (leave primary untouched). HOLD stays on `HOLD.md` (leave primary untouched). CHAR+ stays on `CHAR-PLUS.md` (leave primary untouched). Tip5 DOCS-CITES still later (wave23 **5**).
- Acceptance: Lab smokes `abs-demo` (and retains `uless-demo` + `hold-demo` + `charplus-demo` + `search-demo` + `number-demo` + `zero-demo` + `shift-demo` + `bit-demo` + `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `env-demo` + `allot-demo` + prior demos incl `trit-math-demo` / `fold-demo`).
- Cite: `docs/ABS-NEGATE.md`.

### Optional — `docs/PICK-ROLL.md`

- Light `ABS-NEGATE.md` (wave23 **4**) cite; `ABS`/`NEGATE` sibling signed magnitude / negate marks may echo optional `n=` / `u=` — **NOT** PICK/?DUP reopen / boolean cell / BITWISE reopen / LSHIFT reopen; prefer `abs-mark`/`negate-mark`; **do not** redefine pick-nth / roll-nth / stack-depth / qdup. Retain `pick-demo`. Cite: `docs/ABS-NEGATE.md`.

### Optional — `docs/HOST-PARITY.md`

- Light `ABS-NEGATE.md` (wave23 **4**) cite; `abs-demo CONTRACT` welcome — **not** full Win/Android Forth VM / boolean-cell port / BITWISE ALU port / LSHIFT ALU port. Prefer `abs-mark`/`negate-mark`. Cite: `docs/ABS-NEGATE.md`.

Do **not** wipe wave23 tip1–3 / wave22–15 prior content. Amends are BITWISE + TRUE-FALSE + KERNEL (+ optional PICK-ROLL / HOST-PARITY) only. Prefer **not** amending WITHIN. **Leave untouched** (md5s §8): U-LESS / HOLD / CHAR-PLUS / LSHIFT-RSHIFT / ZERO-EQUALS (tip3 thin) / COMPARE / SEARCH-WORDLIST / TO-NUMBER / CHAR-CHARS / WITHIN / ARCHITECTURE / IMPLEMENTATION-GAPS / WAVE23-PROPOSAL / WAVE23-COS-PASTE. Tip5 cites after 1–4 PASS. Skip 2DUP-FAMILY + ABORT" polish.

## 7. Non-goals

- Redefining / aliasing / shadowing host `ABS` / `NEGATE` when colliding — prefer Forth mirrors `abs-mark` / `negate-mark`
- Redefining `and-mark` / `or-mark` / `xor-mark` / `invert-mark` / host `AND`/`OR`/`XOR`/`INVERT` / host lowercase `and`/`or` (BITWISE stays — **not** a BITWISE reopen; thin companion amend only; host lowercase `and`/`or` stay untouched; NEGATE ≠ INVERT)
- Redefining `lshift-mark` / `rshift-mark` / host `LSHIFT`/`RSHIFT` / host lowercase `lshift`/`rshift` (LSHIFT stays — leave LSHIFT-RSHIFT.md primary untouched — **not** a LSHIFT reopen)
- Redefining `TRUE` / `FALSE` / `true-mark` / `false-mark` (TRUE-FALSE stays — **not** boolean cell / TRUE-FALSE reopen; thin companion amend only)
- Redefining `zero-eq-mark` / `zero-ne-mark` / host `0=` / `0<>` (ZERO-EQUALS stays — leave tip3 thin as-is — **not** a ZERO-EQUALS reopen)
- Redefining `u-less-mark` / host `U<` (U-LESS stays — leave U-LESS.md primary untouched — **not** a U-LESS reopen)
- Redefining `hold-mark` / host `HOLD` / bumping `_here`/`HERE`/`here-at` (HOLD stays — leave HOLD.md primary untouched; HERE stub `$1000` untouched)
- Redefining `char-plus-mark` / `CHAR`/`CHARS`/`[CHAR]`/`char-unit`/`chars-n`/`bracket-char` (CHAR-PLUS stays — leave CHAR-PLUS.md primary untouched)
- Redefining `compare-mark` / `within-mark` (COMPARE / WITHIN stay — prefer not amending WITHIN this tip)
- Real boolean cell rewrite / live flag storage / VARIABLE-as-boolean / flag algebra deepen
- `BITWISE` / `LSHIFT` / `ZERO-EQUALS` / `U-LESS` / `HOLD` / `CHAR-PLUS` reopen (already stubbed; thin BITWISE companion only; leave LSHIFT/ZERO/U-LESS/HOLD/CHAR-PLUS primaries untouched; HOLD: no full `#S`/`#>`/`SIGN`; CHAR-PLUS: no unicode/XCHAR)
- `MIN` / `MAX` signed min/max marks: see `docs/MIN-MAX.md` (wave24 **2**; Forth mirrors `min-mark`/`max-mark`); **not** an ABS-NEGATE reopen / boolean cell; do not redefine `abs-mark`/`negate-mark`; kernel `MAX-ENTRIES`/`MAX-SYNONYMS`/`MAX-CHAR` ≠ ANS `MAX`
- Docs cites pass (wave23 **5** — ARCHITECTURE + GAPS after 1–4 PASS; wave24 tip5 DOCS-CITES still later)
- `SEARCH-WORDLIST` / FIND / `COMPARE` / `BASE-HEX` / `ACCEPT-REFILL` / `TO-NUMBER` / `COUNT` / `EXECUTE` / `SOURCE` / `PAD` reopen (already stubbed; leave those primaries untouched; keep cites)
- Real DOES> XT / real branch XT / LEAVE jump / full arena / linked XT / real STATE cell
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live — **skip 2DUP-FAMILY**)
- `ABORT"` polish (already optional-wired inside `throw-demo` — **skip**)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/ABS-NEGATE.md` present (Research byte-copy OK); `BITWISE.md` + `TRUE-FALSE.md` + `KERNEL.md` thin amends present (+ optional PICK-ROLL / HOST-PARITY); prior cites retained; host `ABS`/`NEGATE` untouched via mirrors; prior mirrors **not** redefined. Untouched md5s: U-LESS `1f17242555fde002eaf2e7dbabadc5d6`/30870; HOLD `dcc8a16c5f2999dd3aba7d828383f475`/30969; CHAR-PLUS `bb879e6fe97f8f8ebd25418d43b43751`/30666; LSHIFT-RSHIFT `e5a94d8a16d47aa7ceae92b54344712e`; ZERO-EQUALS tip3 thin `0ccdfe471e323cbd93343ae06398c089`; COMPARE `0ddd84eab056bdfca18bf4c0d34a663b`; ARCHITECTURE `42902a5431b3f868b9ff7d73414cccc8`; GAPS `465ea6f99db2983a626d42b8731e59c2`; WAVE23-PROPOSAL `01fb36f4ea8c4d25e3c3b7ec998e6718`; COS-PASTE `3c5e643d45d43a7f46770e2a8563fdf3`.
2. `abs-demo` → OK (markers §4; `[abs] ABS` + `[abs] NEGATE` greppable; optional `n=` / `u=` welcome; no FAIL on happy path; no boolean cell / BITWISE / LSHIFT / ZERO-EQUALS / U-LESS / HOLD / CHAR-PLUS reopen; prior mirrors not redefined). Prior `uless-demo` + `hold-demo` + `charplus-demo` + `search-demo` + `number-demo` + `zero-demo` + `shift-demo` + `bit-demo` + `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `env-demo` + `allot-demo` + earlier incl `trit-math-demo` / `fold-demo` still OK; wave24 **2**: `minmax-demo` → OK (retains `abs-demo`; not ABS reopen).
3. Regression green (wave23 tip1–3 + wave22–14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `abs-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**)
- `docs/BITWISE.md` (wave21 **4** — AND/OR/XOR/INVERT bitwise-mark companion; ABS-NEGATE sibling magnitude/negate — **not** a BITWISE reopen; host lowercase `and`/`or` stay untouched; prefer `abs-mark`/`negate-mark`; do not redefine `and-mark`/`or-mark`/`xor-mark`/`invert-mark`; NEGATE ≠ INVERT)
- `docs/TRUE-FALSE.md` (wave20 **1** — TRUE/FALSE constant-picture companion; ABS-NEGATE sibling magnitude/negate — **not** boolean-cell rewrite / TRUE-FALSE reopen; do not redefine `true-mark`/`false-mark`)
- `docs/LSHIFT-RSHIFT.md` (wave22 **1** — LSHIFT/RSHIFT shift-mark companion; **not** a LSHIFT reopen; leave primary untouched this tip; do not redefine `lshift-mark`/`rshift-mark`)
- `docs/PICK-ROLL.md` (wave15 **2**, optional — stack/?DUP flag companion; not PICK/?DUP reopen)
- `docs/HOST-PARITY.md` (wave8 **4**, optional — `abs-demo` CONTRACT parity welcome)
- `docs/U-LESS.md` / `docs/HOLD.md` / `docs/CHAR-PLUS.md` (wave23 **1–3** — prior; leave primaries untouched; do not redefine `u-less-mark`/`hold-mark`/`char-plus-mark`; HERE stub `$1000` untouched)
- `docs/ZERO-EQUALS.md` (wave22 **2** — leave tip3 thin as-is; do not redefine `zero-eq-mark`/`zero-ne-mark`)
- `docs/COMPARE.md` / `docs/SEARCH-WORDLIST.md` / `docs/TO-NUMBER.md` / `docs/BASE-HEX.md` / `docs/ACCEPT-REFILL.md` / wave20 COUNT/EXECUTE / wave19 SOURCE-PAD/CHAR-CHARS (prior; leave primaries untouched)
- `forth/tritium/kernel.fs` (abs-mark / negate-mark only — do not redefine host `ABS`/`NEGATE` or prior mirrors)
- ANS Forth `ABS` / `NEGATE` (signed magnitude / negate marks only — prefer mirrors; not boolean cell / BITWISE / LSHIFT reopen)
- Explicit deferral: WAVE19–23 PROPOSALs (`ABS`/`NEGATE` — marks only)
- Base tip: `f76ca819` / `f76ca819fd71bb67e91a4a3228746c02af770d3e` (#109 wave23 tip3 U-LESS PASS)
- `docs/MIN-MAX.md` (wave24 **2** — MIN/MAX signed min/max companion; ABS-NEGATE sibling magnitude/negate — **not** an ABS reopen; prefer `min-mark`/`max-mark`; do not redefine `abs-mark`/`negate-mark`; kernel MAX-* ≠ ANS MAX)
- Wave23 proposal: `/workspace/tritium-research-docs/WAVE23-PROPOSAL.md`

## 10. Shipper implementation notes (Linux SoT)

These notes are normative for Shipper drafting on branch `shipper/abs-negate-w23` from base `f76ca819fd71bb67e91a4a3228746c02af770d3e`. They do **not** authorize merge, push, Mango touch, opaque-weight ML, or reopening closed wave17–22 tips / wave23 tip1 CHAR-PLUS primary / wave23 tip2 HOLD primary / wave23 tip3 U-LESS primary / BITWISE primary beyond thin companion cite / LSHIFT-RSHIFT primary.

### 10.1 Binding order

1. Prefer Forth mirrors **`abs-mark`** and **`negate-mark`** first. Lab greps do not require the bare ANS names `ABS`/`NEGATE` to be bound when the mirrors print `[abs] ABS` / `[abs] NEGATE`.
2. If bare `ABS` / `NEGATE` are free on the Linux REPL load path, Shipper **may** bind them as thin aliases that emit the same markers — still **must not** redefine `and-mark`/`or-mark`/`xor-mark`/`invert-mark` / `lshift-mark`/`rshift-mark` / `true-mark`/`false-mark` / `zero-eq-mark`/`zero-ne-mark` / `u-less-mark` / `hold-mark` / `char-plus-mark` / `compare-mark` / `within-mark`.
3. Never alias `ABS`/`NEGATE` onto `invert-mark` / `and-mark` / `TRUE`/`FALSE` / `zero-eq-mark` / `u-less-mark`. Absolute-value / two’s-complement negate ≠ ones-complement INVERT / bitwise AND / boolean constants / zero-equals / unsigned compare.

### 10.2 Classic pictures (ABS + NEGATE)

Document one fixed fixture pair per word. Recommended:

| Field | Example | Notes |
|-------|---------|-------|
| ABS input n | `-5` | Classic signed negative operand |
| ABS result u | `5` | `\|-5\| = 5` |
| optional ABS zero | `n=0` → `u=0` | Welcome; not required |
| optional ABS positive | `n=7` → `u=7` | Welcome; not required |
| NEGATE input n1 | `5` | Classic positive operand |
| NEGATE result n | `-5` | `-5` two’s-complement |
| optional NEGATE negative | `n1=-3` → `n=3` | Welcome; not required |
| optional NEGATE zero | `n1=0` → `n=0` | Welcome; not required |

Shipper may use other signed stub ints — document which. Lab greps `[abs] ABS` and `[abs] NEGATE` regardless of which fixtures are chosen, as long as both marker lines are greppable and FAIL is avoided. **Do not** implement NEGATE by reopening BITWISE INVERT (ones-complement ≠ two’s-complement). **Do not** implement ABS by reopening boolean cell / TRUE-FALSE.

### 10.3 What Lab greps / does not grep

**Required:** `[abs-demo] OK` + `[abs] ABS` + `[abs] NEGATE`. **Optional welcome:** `n=` / `u=`; zero / positive ABS / negative NEGATE pictures; `abs-demo CONTRACT`. **Must NOT Lab-grep as success:** `[bit]`/`and-mark`/`or-mark`/`xor-mark`/`invert-mark`; `[shift]`/`lshift-mark`/`rshift-mark`; `[true]`/`true-mark`/`false-mark`; `[zero]`/`zero-eq-mark`; `[uless]`/`u-less-mark`; `[hold]`/`hold-mark`; `[char+]`/`char-plus-mark`; `[compare]`/`compare-mark`; `[within]`/`within-mark`; `[search]`/`[number]`/`[base]`/`[cell]`.

### 10.4 Regression + companions + non-goals

Retain green: `uless-demo` / `hold-demo` / `charplus-demo` / `search-demo` / `number-demo` / `zero-demo` / `shift-demo` / `bit-demo` / `compare-demo` / `base-demo` / `accept-demo` / `exec-demo` / `count-demo` / `within-demo` / `true-demo` / `env-demo` / `cell-demo` / `allot-demo` / `kernel-demo` / `trit-math-demo` / `fold-demo` (+ earlier). Required thin: BITWISE + TRUE-FALSE + KERNEL. Optional: PICK-ROLL / HOST-PARITY. **Untouched:** ARCH / GAPS / WAVE23-PROPOSAL / COS-PASTE / U-LESS / HOLD / CHAR-PLUS / LSHIFT-RSHIFT / ZERO-EQUALS (tip3 thin) / COMPARE / SEARCH / TO-NUMBER / WITHIN (prefer not).

Do **not**: redefine bitwise/shift/true/zero/uless/hold/charplus/compare/within mirrors; reopen BITWISE/LSHIFT/ZERO-EQUALS/U-LESS/HOLD/CHAR-PLUS; land boolean cell / tip5 DOCS-CITES; amend ARCH/GAPS / those primaries; tip Shipper / push / merge / touch Mango; redefine host 2dup/2drop/2swap; polish ABORT".

ABS-NEGATE = thin **signed magnitude / negate marks** only. Prefer `abs-mark` / `negate-mark`. Preferred emit: `[abs] ABS u=5` + `[abs] NEGATE n=-5` — minimum both markers — close `[abs-demo] OK`. Research drafts under `/workspace/tritium-research-docs/`; Shipper byte-copies on `shipper/abs-negate-w23` only. Parent tips Shipper after. Still deferred: tip5 DOCS-CITES; boolean cell; prior-tip reopen; full Win/Android Forth VM; 2DUP-FAMILY; ABORT" polish; Mango. Paste anchors: WAVE23-PROPOSAL.md § Tip 4; base `f76ca819fd71bb67e91a4a3228746c02af770d3e`; branch `shipper/abs-negate-w23`; handoff `WAVE23-TIP4-HANDOFF.md` (= `TIP4-HANDOFF.md`).

### 10.5 Disambiguation (Shipper)

| Surface | Tip | This tip? |
|---------|-----|-----------|
| `ABS` / `abs-mark` | wave23 **4** | **YES** |
| `NEGATE` / `negate-mark` | wave23 **4** | **YES** |
| `AND`/`OR`/`XOR`/`INVERT` / `and-mark`/`or-mark`/`xor-mark`/`invert-mark` | wave21 **4** | NO — do not redefine (NEGATE ≠ INVERT) |
| `LSHIFT`/`RSHIFT` / `lshift-mark`/`rshift-mark` | wave22 **1** | NO — leave LSHIFT-RSHIFT.md untouched |
| `TRUE`/`FALSE` / `true-mark`/`false-mark` | wave20 **1** | NO — do not redefine |
| `0=` / `zero-eq-mark` | wave22 **2** | NO — leave tip3 thin as-is |
| `U<` / `u-less-mark` | wave23 **3** | NO — leave U-LESS.md untouched |
| `HOLD` / `hold-mark` | wave23 **2** | NO — leave HOLD.md untouched |
| `CHAR+` / `char-plus-mark` | wave23 **1** | NO — leave CHAR-PLUS.md untouched |
| DOCS-CITES (ARCH/GAPS) | wave23 **5** | NO — tip5 |

Absolute value (`ABS`) ≠ ones-complement (`INVERT`) ≠ two’s-complement negate (`NEGATE`) ≠ unsigned compare (`U<`) ≠ zero-equals (`0=`). Leave U-LESS / HOLD / CHAR-PLUS / LSHIFT-RSHIFT / ZERO-EQUALS (tip3 thin) byte-identical (md5s §8). HERE stub `$1000` untouched. Docs ONLY under `/workspace/tritium-research-docs/`. Skip 2DUP-FAMILY + ABORT" polish. Parent tips Shipper after.
