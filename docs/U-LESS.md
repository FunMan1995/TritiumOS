# U-LESS — `U<` unsigned compare flag mark + `uless-demo`

**Status:** Shipper-ready stub spec (wave23 item **3**)
**Canonical brief:** ANS-shaped `U<` (thin unsigned compare flag mark only); `docs/ZERO-EQUALS.md` (wave22 **2** — `0=` / optional `0<>` flag-mark companion; **not** a ZERO-EQUALS reopen; **do not** redefine `zero-eq-mark`/`zero-ne-mark`); `docs/COMPARE.md` (wave21 **3** — ANS string-compare companion; **not** a COMPARE reopen; **do not** redefine `compare-mark`; host `cstr=` ≠ ANS COMPARE); `docs/KERNEL.md` (wave7 **5**); optional `docs/TRUE-FALSE.md` (wave20 **1** — TRUE/FALSE constant-picture companion; **not** boolean cell / TRUE-FALSE reopen; **do not** redefine `true`/`false` constants / `true-mark`/`false-mark`) / `docs/WITHIN.md` (wave20 **2** — range-check companion cite-only; **not** WITHIN reopen; **do not** redefine `within-mark`) / `docs/HOST-PARITY.md` (wave8 **4**); WAVE19–23 deferral closed as **unsigned compare flag mark only** (not WITHIN reopen; not ZERO-EQUALS reopen; not COMPARE reopen; not boolean cell rewrite; **not** HOLD reopen; **not** CHAR-PLUS reopen; **not** ABS-NEGATE — tip4; prefer Forth mirror **`u-less-mark`** whenever host `U<` collides).
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `uless.fs` / `u-less.fs`); Linux host REPL; prefer Forth mirror **`u-less-mark`** whenever host `U<` collides; **do not** redefine `zero-eq-mark` / `zero-ne-mark` (ZERO-EQUALS stays — **not** a ZERO-EQUALS reopen); **do not** redefine `compare-mark` (COMPARE stays — **not** a COMPARE reopen); **do not** redefine `within-mark` (WITHIN stays — **not** a WITHIN reopen); **do not** redefine `hold-mark` (HOLD stays — leave HOLD.md primary untouched); **do not** redefine `char-plus-mark` (CHAR-PLUS stays — leave CHAR-PLUS.md primary untouched); **do not** redefine `TRUE` / `FALSE` / `true-mark` / `false-mark` (TRUE-FALSE stays — **not** boolean cell)
**Companions:** `docs/ZERO-EQUALS.md` (thin amend this tip), `docs/COMPARE.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip); optional light cite `docs/TRUE-FALSE.md` / `docs/WITHIN.md` (cite-only) / `docs/HOST-PARITY.md`
**Base tip SHA:** `98147d08` (wave23 tip2 PASS / #108 HOLD) / full `98147d08c54ada2ad739fd4d8586d750b965d59e`

## 1. Purpose

WAVE20 tip **1** landed `TRUE` / `FALSE` constant marks (`docs/TRUE-FALSE.md` — classic `-1` vs `0`). WAVE20 tip **2** landed `WITHIN` range-check mark (`docs/WITHIN.md` — signed `lo ≤ n < hi` picture; **not** unsigned compare). WAVE21 tip **3** landed ANS `COMPARE` string-compare mark (`docs/COMPARE.md`; host `cstr=` ≠ ANS COMPARE). WAVE22 tip **2** landed `0=` / optional `0<>` flag marks (`docs/ZERO-EQUALS.md`; host `0=`/`0<>` untouched). WAVE23 tip **1** landed `CHAR+` thin char-unit advance (`docs/CHAR-PLUS.md`; leave primary untouched this tip). WAVE23 tip **2** landed `HOLD` thin pictured-numeric start (`docs/HOLD.md`; leave primary untouched this tip — **CRITICAL** HERE stub `$1000` stays). WAVE18–23 deferred `U<` as an unsigned compare flag mark beside ZERO-EQUALS + COMPARE + WITHIN (**not** WITHIN reopen; **not** ZERO-EQUALS/COMPARE reopen; **not** boolean cell rewrite). This tip lands **stub** unsigned compare flag mark only: `U<` (or Forth mirror **`u-less-mark`**) prints `[uless] U<` (+ optional `flag=` — classic unsigned less-than picture welcome: **u1 u2 → true when u1 <u u2**). Smoke via **`uless-demo`**. Prefer Forth mirror **`u-less-mark`** whenever host `U<` collides. Pairs with wave22 ZERO-EQUALS (`0=` flag) and wave21 COMPARE (string mark) and wave20 WITHIN (range-check mark) **without** reopening any of them and **without** a boolean-cell rewrite. **Not** tip4 ABS-NEGATE / tip5 DOCS-CITES. Independent of tip4–5. **Leave `HOLD.md` primary untouched** (tip2 land md5 `dcc8a16c5f2999dd3aba7d828383f475` / 30969). **Leave `CHAR-PLUS.md` primary untouched** (tip1 land md5 `bb879e6fe97f8f8ebd25418d43b43751` / 30666).

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `U<` / `u-less-mark` | `( u1 u2 -- flag )` *or* `( -- )` with fixed demo fixture | Unsigned less-than flag mark; print `[uless] U<` (+ optional `flag=<n>`); classic `u1 <u u2 → flag=true/-1/1`; otherwise `flag=false/0` |
| `uless-demo` | `( -- )` | See §5 |

Host note: bind bare `U<` on the Linux REPL **only if** that name does not collide with host Forth `U<`. Prefer Forth mirror **`u-less-mark`** as the Lab-facing surface when in doubt — **do not** redefine host `U<`. Optional `flag=` is host int / fixture echo only — not a live boolean cell, not WITHIN reopen, not ZERO-EQUALS reopen, not COMPARE reopen, not HOLD reopen, not CHAR-PLUS reopen. Prefer **fixed demo unsigned stub-int fixtures** (classic `u1 <u u2` → true; equal / `u1 >u u2` → false) so Lab hit is deterministic and FAIL is avoided. Classic unsigned picture welcome includes high-bit / wrap-aware fixtures (e.g. `$FFFFFFFF` / `-1` treated as unsigned max vs small unsigned) — document which Shipper emits; greppable `[uless] U<` is enough for Lab OK. **Do not** redefine `zero-eq-mark` / `zero-ne-mark` / `compare-mark` / `within-mark` / `hold-mark` / `char-plus-mark` / `TRUE`/`FALSE`/`true-mark`/`false-mark`.

## 3. Stub semantics

- **`U<` / `u-less-mark`:** take (or use fixed demo) unsigned stub ints `( u1 u2 )`. Classic ANS / common Forth picture: unsigned less-than → `flag = true` when `u1 <u u2` (unsigned compare), else `flag = false` (document whether Shipper echoes classic all-bits-set / `-1` vs thinner `1` for true — greppable `[uless] U<` is enough for Lab OK; `-1`/`0` picture pairs with TRUE-FALSE without reopening TRUE-FALSE). Print `[uless] U<` and optionally `flag=<n>` (Lab-greppable; prefer documenting which Shipper emits). Classic fixture welcome: `1 2 U< → flag=-1` (or `flag=1`); `2 1 U< → flag=0`; `5 5 U< → flag=0`; optional high-bit unsigned picture (e.g. `0 $FFFFFFFF U< → true`; `$FFFFFFFF 0 U< → false`) welcome when free — **do not require** high-bit for Lab OK. **Does not** rewrite TRUE/FALSE constants, reopen ZERO-EQUALS / COMPARE / WITHIN / HOLD / CHAR-PLUS, bump HERE, open a heap, or create dict entries. Captured values are host ints / fixture echo only.
- **Fixed demo unsigned stub-int fixtures (document):**
  - **Classic less-than → true (required set):** e.g. `u1=1`, `u2=2` → `U<` true (`flag=-1` or `flag=1`). Demo must exercise **`U<` / `u-less-mark`** so Lab greps `[uless] U<`. Optional `flag=` welcome.
  - **Classic not-less → false (welcome):** e.g. `u1=2`, `u2=1` → false (`flag=0`); equal `u1=u2` → false. Document if Shipper emits one combined marker line or two — greppable `[uless] U<` is enough.
  - **Optional high-bit / unsigned-wrap picture:** e.g. small vs all-bits-set treated as unsigned — welcome when free; do **not** require for Lab OK; do **not** promote to WITHIN reopen / signed compare rewrite / boolean cell.
  - **Optional result echo:** `flag=-1` / `flag=1` / `flag=0` — document which form Shipper emits. Greppable markers alone are enough for Lab OK.
- **Optional push:** if the host stack is easy, push the pictured flag; marker alone is enough for Lab OK — do not require a real boolean cell / flag algebra / WITHIN reopen / ZERO-EQUALS reopen / COMPARE reopen.
- **FAIL:** `[uless] FAIL reason=<…>` optional (demo **must avoid**). Prefer not emitting `[uless] FAIL` on the happy path. No required FAIL reason this tip — missing args / type-miss stay out of the demo (or document as optional miss picture that still avoids FAIL).
- Storage: fixed demo unsigned stub ints / host int echo only. **No** real boolean cell rewrite, no WITHIN reopen (leave wave20 range-check mark-only), no ZERO-EQUALS reopen (leave wave22 flag marks; thin companion amend only), no COMPARE reopen (leave wave21 string mark; thin companion amend only), no HOLD reopen (leave HOLD.md primary untouched), no CHAR-PLUS reopen (leave CHAR-PLUS.md primary untouched), no ABS-NEGATE, no HERE bump, no arena.
- **Host `U<` stays untouched when colliding:** prefer mirrors whenever host `U<` collides. This tip’s Lab greps are `[uless] U<` and `[uless-demo] OK` only — **never** Lab-grep bare host `U<` as this tip’s success when using the Forth mirror path.
- **ZERO-EQUALS / COMPARE / WITHIN / TRUE-FALSE / HOLD / CHAR-PLUS stay:** do **not** redefine `zero-eq-mark` / `zero-ne-mark`. Do **not** redefine `compare-mark`. Do **not** redefine `within-mark`. Do **not** redefine `TRUE` / `FALSE` / `true-mark` / `false-mark`. Do **not** redefine `hold-mark`. Do **not** redefine `char-plus-mark`. Unsigned compare flag mark is a **sibling** beside zero-equals flag / string-compare / range-check / constant marks — not a reopen of any.
- Nest with prior hold / charplus / search / number / zero / shift / bit / compare / base / accept / exec / count / within / true + earlier stubs OK. `dict-reset` unaffected. Assert prior `zero-demo` / `compare-demo` / `within-demo` / `true-demo` / `hold-demo` / `charplus-demo` still OK.
- Still no WITHIN / ZERO-EQUALS / COMPARE / HOLD / CHAR-PLUS reopen; no boolean cell; no ABS-NEGATE (tip4); no tip5 DOCS-CITES; no linked XT / real DOES> XT / full arena / full Win/Android Forth VM. Keep wave23 tip1–2 + wave22–15 cites. **Leave `HOLD.md` untouched** (`dcc8a16c5f2999dd3aba7d828383f475` / 30969). **Leave `CHAR-PLUS.md` untouched** (`bb879e6fe97f8f8ebd25418d43b43751` / 30666).

## 4. Markers

```
[uless] U< [flag=<n>]         # flag= optional; classic u1 <u u2 → flag=-1/1; else flag=0 welcome
[uless] FAIL reason=<…>       # demo avoids
[uless-demo] OK
[uless-demo] FAIL
```

Lab greps `[uless-demo] OK` plus greppable **`[uless] U<`** (optional `flag=` welcome — classic unsigned less-than picture on fixed fixture). Demo avoids `[uless] FAIL`. Prefer not emitting `[uless] FAIL` on the happy path. **Do not** Lab-grep `[zero]` / `[compare]` / `[within]` / `[true]` / `[hold]` / `[char+]` / `[shift]` / `[bit]` as U-LESS success (those stay their own tips). **Do not** Lab-grep `[zero] 0=` / `[compare] COMPARE` / `[within] WITHIN` / `[true] TRUE` / `[hold] HOLD` / `[char+] CHAR+` as this tip’s surface (ZERO-EQUALS / COMPARE / WITHIN / TRUE-FALSE / HOLD / CHAR-PLUS stay prior tips). **Do not** Lab-grep `ABS` / `NEGATE` as this tip (those stay tip4).

## 5. `uless-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; unsigned compare flag mark needs no dict entries.
2. Ensure **fixed demo unsigned stub-int fixtures** exist (e.g. `u1=1`, `u2=2` for less-than→true; optional `u1=2`, `u2=1` / equal for false — or equivalent host ints) so the classic unsigned less-than picture is deterministic. Fixtures may live beside (not replacing) ZERO-EQUALS stub-int fixtures / COMPARE string-pair fixtures / WITHIN range-check / TRUE/FALSE constant picture / CELL unit picture / HOLD char fixture / CHAR-PLUS advance picture — document; do **not** require boolean-cell rewrite / WITHIN reopen / ZERO-EQUALS reopen / COMPARE reopen / HOLD reopen / CHAR-PLUS reopen / ABS-NEGATE.
3. Invoke `U<` (or **`u-less-mark`**) against classic less-than fixture → `[uless] U<` (+ optional `flag=` — classic true / `-1` / `1` welcome).
4. Optional: invoke against classic not-less / equal fixture → `[uless] U<` (+ optional `flag=` — classic false / `0` welcome). Document if Shipper emits one combined marker line or two — greppable `[uless] U<` is enough.
5. Optional (when free): high-bit unsigned picture — **do not require**.
6. Assert no `[uless] FAIL` on the happy path. Assert unsigned compare flag mark did **not** require a real boolean cell rewrite / WITHIN reopen / ZERO-EQUALS reopen / COMPARE reopen / HOLD reopen / CHAR-PLUS reopen / HERE bump / host `U<` redefine when using the Forth mirror (marker-only is enough). Assert `zero-eq-mark` / `zero-ne-mark` / `compare-mark` / `within-mark` / `hold-mark` / `char-plus-mark` / `true-mark` / `false-mark` were **not** redefined. Assert `HOLD.md` + `CHAR-PLUS.md` primaries were left untouched. Assert prior `zero-demo` / `compare-demo` / `within-demo` / `true-demo` / `hold-demo` / `charplus-demo` still OK.
7. Prior `hold-demo` / `charplus-demo` / `search-demo` / `number-demo` / `zero-demo` / `shift-demo` / `bit-demo` / `compare-demo` / `base-demo` / `accept-demo` / `exec-demo` / `count-demo` / `within-demo` / `true-demo` + earlier incl `trit-math-demo` / `fold-demo` still OK.
8. `[uless-demo] OK`.

Required: `[uless] U<`. Optional `flag=` not required for Lab OK. No WITHIN / ZERO-EQUALS / COMPARE / HOLD / CHAR-PLUS reopen; no boolean cell; no ABS-NEGATE; no host `U<` redefine when using the mirror.

## 6. Thin amend — companions

### `docs/ZERO-EQUALS.md`

- Companions / Status: add `U-LESS.md` (wave23 **3** companion cite); **keep** TRUE-FALSE / BITWISE / LSHIFT-RSHIFT / KERNEL / WITHIN / PICK-ROLL / CELL-CELLS / HOST-PARITY cites — do not wipe wave22 ZERO-EQUALS content.
- Purpose / §3 / non-goals: `0=` (optional `0<>`) stay flag marks; `U<` is sibling **unsigned compare flag mark** — **not** a ZERO-EQUALS reopen / boolean-cell rewrite / WITHIN reopen / COMPARE reopen / HOLD reopen. Do not wipe wave22 ZERO-EQUALS content. Stress: U-LESS sibling unsigned compare — **NOT** ZERO-EQUALS reopen; prefer `u-less-mark` whenever host `U<` collides; **do not** redefine `zero-eq-mark`/`zero-ne-mark`; host `0=`/`0<>` stay untouched.
- Non-goals: `U<` → `docs/U-LESS.md` (wave23 **3**). `0=` / optional `0<>` / `zero-eq-mark` / `zero-ne-mark` stay on this tip (already landed). WITHIN stays wave20 **2**. COMPARE stays wave21 **3**. Tip4 ABS-NEGATE / tip5 DOCS-CITES still later (wave23 **4–5**).
- Acceptance: Lab smokes `uless-demo` (retains `zero-demo` + `compare-demo` + `within-demo` + `true-demo` + `hold-demo` + `charplus-demo`).
- Cite: `docs/U-LESS.md`.

### `docs/COMPARE.md`

- Companions / Status: add `U-LESS.md` (wave23 **3** companion cite); **keep** STRING-LIT / FILL-MOVE / KERNEL / COUNT / WORD-BL / WITHIN cites — do not wipe wave21 COMPARE content.
- Purpose / §3 / non-goals: ANS `COMPARE` stays string-compare mark; `U<` is sibling **unsigned compare flag mark** — **not** a COMPARE reopen / SEARCH-WORDLIST / FIND reopen / full string heap / WITHIN reopen / ZERO-EQUALS reopen / boolean cell. Do not wipe wave21 COMPARE content. Stress: U-LESS sibling unsigned compare — **NOT** COMPARE reopen; host `cstr=` still ≠ ANS COMPARE; prefer `u-less-mark`; **do not** redefine `compare-mark`.
- Non-goals: `U<` → `docs/U-LESS.md` (wave23 **3**). `COMPARE` / `compare-mark` stay on this tip (already landed). WITHIN stays wave20 **2**. ZERO-EQUALS stays wave22 **2**. Tip4 ABS-NEGATE / tip5 DOCS-CITES still later.
- Acceptance: Lab smokes `uless-demo` (retains `compare-demo` + `zero-demo` + `within-demo` + `count-demo` + `hold-demo` + `charplus-demo`).
- Cite: `docs/U-LESS.md`.

### `docs/KERNEL.md`

- Companions: add `U-LESS.md` (wave23 **3**); **keep** wave23 tip1–2 CHAR-PLUS / HOLD cites and wave22 tip1–4 LSHIFT-RSHIFT / ZERO-EQUALS / TO-NUMBER / SEARCH-WORDLIST cites and wave21 tip1–4 ACCEPT-REFILL / BASE-HEX / COMPARE / BITWISE cites and wave20 tip1–4 / wave19 tip1–4 / wave18 tip1–4 / wave17 tip1–4 / wave16 DEFER/MARKER/BUFFER/EXIT / wave15 CELL cites.
- Words table: add `U<` / `u-less-mark` stub + `uless-demo` (cite tip; Forth mirror `u-less-mark` — unsigned compare flag mark only; prefer `u-less-mark` whenever host `U<` collides; **do not** redefine `zero-eq-mark`/`zero-ne-mark` / `compare-mark` / `within-mark` / `hold-mark` / `char-plus-mark` / `TRUE`/`FALSE`/`true-mark`/`false-mark`; **not** WITHIN reopen; **not** ZERO-EQUALS reopen; **not** COMPARE reopen; **not** boolean cell rewrite; **not** HOLD reopen; **not** CHAR-PLUS reopen; optional `flag=` — classic unsigned less-than picture welcome: u1 u2 → true when u1 <u u2).
- Non-goals: `U<` unsigned compare flag mark → `docs/U-LESS.md`. `0=` stays on `ZERO-EQUALS.md` (thin companion amend — do not wipe). COMPARE stays on `COMPARE.md` (thin companion amend — do not wipe). WITHIN stays on `WITHIN.md`. TRUE/FALSE stay on `TRUE-FALSE.md`. HOLD stays on `HOLD.md` (leave primary untouched). CHAR+ stays on `CHAR-PLUS.md` (leave primary untouched). Tip4 ABS-NEGATE / tip5 DOCS-CITES still later (wave23 **4–5**).
- Acceptance: Lab smokes `uless-demo` (and retains `hold-demo` + `charplus-demo` + `search-demo` + `number-demo` + `zero-demo` + `shift-demo` + `bit-demo` + `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `env-demo` + `allot-demo` + prior demos incl `trit-math-demo` / `fold-demo`).
- Cite: `docs/U-LESS.md`.

### Optional — `docs/TRUE-FALSE.md`

- Light `U-LESS.md` (wave23 **3**) cite; `U<` sibling unsigned compare flag mark may echo optional `flag=` — **NOT** boolean cell / TRUE-FALSE reopen / ZERO-EQUALS reopen / WITHIN reopen; prefer `u-less-mark`; **do not** redefine `true-mark`/`false-mark` / `TRUE`/`FALSE`. Retain `true-demo`. Cite: `docs/U-LESS.md`.

### Optional — `docs/WITHIN.md` (cite-only)

- Light `U-LESS.md` (wave23 **3**) cite; WITHIN stays range-check mark; `U<` is sibling **unsigned compare flag** — **NOT** WITHIN reopen / runtime compare re-exec / boolean cell / ZERO-EQUALS reopen / COMPARE reopen. Prefer `u-less-mark`; **do not** redefine `within-mark`. Retain `within-demo`. Cite: `docs/U-LESS.md`.

### Optional — `docs/HOST-PARITY.md`

- Light `U-LESS.md` (wave23 **3**) cite; `uless-demo CONTRACT` welcome — **not** full Win/Android Forth VM / boolean-cell port / WITHIN runtime port. Prefer `u-less-mark`. Cite: `docs/U-LESS.md`.

Do **not** wipe wave23 tip1–2 / wave22–15 prior content. Amends are ZERO-EQUALS + COMPARE + KERNEL (+ optional TRUE-FALSE / WITHIN / HOST-PARITY) only. **Leave untouched:** HOLD.md (`dcc8a16c5f2999dd3aba7d828383f475` / 30969); CHAR-PLUS.md (`bb879e6fe97f8f8ebd25418d43b43751` / 30666); LSHIFT-RSHIFT.md (`e5a94d8a16d47aa7ceae92b54344712e` / 26299); SEARCH-WORDLIST.md (`0f9fa84fb70d08c2da7e32f20d1a6e29` / 31054); TO-NUMBER.md (do not amend); ARCHITECTURE (`42902a5431b3f868b9ff7d73414cccc8`); IMPLEMENTATION-GAPS (`465ea6f99db2983a626d42b8731e59c2`); WAVE23-PROPOSAL (`01fb36f4ea8c4d25e3c3b7ec998e6718`); WAVE23-COS-PASTE (`3c5e643d45d43a7f46770e2a8563fdf3`). Tip5 cites after 1–4 PASS. Skip 2DUP-FAMILY + ABORT" polish.

## 7. Non-goals

- Redefining / aliasing / shadowing host `U<` when colliding — prefer Forth mirror `u-less-mark`
- Redefining `zero-eq-mark` / `zero-ne-mark` / host `0=` / `0<>` (ZERO-EQUALS stays — **not** a ZERO-EQUALS reopen; thin companion amend only; host `0=`/`0<>` stay untouched)
- Redefining `compare-mark` / host `COMPARE` / host `cstr=` (COMPARE stays — **not** a COMPARE reopen; thin companion amend only; host `cstr=` ≠ ANS COMPARE)
- Redefining `within-mark` / host `WITHIN` (WITHIN stays — **not** WITHIN reopen / runtime compare re-exec; optional cite-only)
- Redefining `TRUE` / `FALSE` / `true-mark` / `false-mark` (TRUE-FALSE stays — **not** boolean cell / TRUE-FALSE reopen; optional thin companion cite)
- Redefining `hold-mark` / host `HOLD` / bumping `_here`/`HERE`/`here-at` (HOLD stays — leave HOLD.md primary untouched; HERE stub `$1000` untouched)
- Redefining `char-plus-mark` / `CHAR`/`CHARS`/`[CHAR]`/`char-unit`/`chars-n`/`bracket-char` (CHAR-PLUS stays — leave CHAR-PLUS.md primary untouched)
- Real boolean cell rewrite / live flag storage / VARIABLE-as-boolean / flag algebra deepen
- `WITHIN` reopen (wave20 **2** — already stubbed; keep cites — **not** runtime compare re-exec)
- `ZERO-EQUALS` reopen (wave22 **2** — already stubbed; thin companion amend only — pairs without reopen)
- `COMPARE` reopen (wave21 **3** — already stubbed; thin companion amend only — pairs without reopen)
- `HOLD` reopen / full pictured `#S`/`#>`/`SIGN` (wave23 **2** — already stubbed; leave HOLD.md primary untouched)
- `CHAR-PLUS` reopen / unicode / XCHAR (wave23 **1** — already stubbed; leave CHAR-PLUS.md primary untouched)
- `ABS` / `NEGATE` marks (tip4 ABS-NEGATE — still deferred)
- Docs cites pass (wave23 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `SEARCH-WORDLIST` / FIND reopen (wave22 **4** / wave18 **2** — already stubbed; leave SEARCH-WORDLIST.md / FIND surfaces)
- `LSHIFT` / `BITWISE` / `BASE-HEX` / `ACCEPT-REFILL` / `TO-NUMBER` reopen (wave21–22 — already stubbed; leave those primaries untouched this tip — do not amend TO-NUMBER)
- `COUNT` / `EXECUTE` / `SOURCE` / `PAD` reopen (wave20 **3–4** / wave19 **4** — already stubbed; keep cites)
- Real DOES> XT / real branch XT / LEAVE jump / full arena / linked XT / real STATE cell
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live — **skip 2DUP-FAMILY**)
- `ABORT"` polish (already optional-wired inside `throw-demo` — **skip**)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/U-LESS.md` present (Research byte-copy OK); `ZERO-EQUALS.md` + `COMPARE.md` + `KERNEL.md` thin amends present (+ optional TRUE-FALSE / WITHIN / HOST-PARITY); prior cites retained; host `U<` untouched via `u-less-mark`; zero-eq-mark/zero-ne-mark / compare-mark / within-mark / hold-mark / char-plus-mark / true-mark/false-mark **not** redefined; HOLD.md untouched (`dcc8a16c5f2999dd3aba7d828383f475` / 30969); CHAR-PLUS.md untouched (`bb879e6fe97f8f8ebd25418d43b43751` / 30666); LSHIFT / SEARCH / TO-NUMBER untouched; ARCHITECTURE (`42902a5431b3f868b9ff7d73414cccc8`) + GAPS (`465ea6f99db2983a626d42b8731e59c2`) + WAVE23-PROPOSAL (`01fb36f4ea8c4d25e3c3b7ec998e6718`) + COS-PASTE (`3c5e643d45d43a7f46770e2a8563fdf3`) untouched.
2. `uless-demo` → OK (markers §4; `[uless] U<` greppable; optional `flag=` welcome — classic unsigned less-than picture on stub ints: u1 u2 → true when u1 <u u2; no FAIL on happy path; no WITHIN reopen / ZERO-EQUALS reopen / COMPARE reopen / boolean cell / HOLD reopen / CHAR-PLUS reopen / ABS-NEGATE; no `zero-eq-mark`/`compare-mark`/`within-mark`/`hold-mark`/`char-plus-mark`/`true-mark`/`false-mark` redefine). Prior `hold-demo` + `charplus-demo` + `search-demo` + `number-demo` + `zero-demo` + `shift-demo` + `bit-demo` + `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `env-demo` + `allot-demo` + earlier demos incl `trit-math-demo` / `fold-demo` still OK.
3. Regression green (wave23 tip1–2 + wave22–14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `uless-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**)
- `docs/ZERO-EQUALS.md` (wave22 **2** — `0=` / optional `0<>` flag-mark companion; U-LESS sibling unsigned compare — **not** a ZERO-EQUALS reopen; host `0=`/`0<>` stay untouched; prefer `u-less-mark`; do not redefine `zero-eq-mark`/`zero-ne-mark`)
- `docs/COMPARE.md` (wave21 **3** — ANS string-compare companion; U-LESS sibling unsigned compare — **not** a COMPARE reopen; host `cstr=` ≠ ANS COMPARE; prefer `u-less-mark`; do not redefine `compare-mark`)
- `docs/TRUE-FALSE.md` (wave20 **1**, optional — TRUE/FALSE constant-picture companion; U-LESS sibling unsigned compare flag — **not** boolean-cell rewrite / TRUE-FALSE reopen; do not redefine `true-mark`/`false-mark`)
- `docs/WITHIN.md` (wave20 **2**, optional cite-only — range-check companion; **not** WITHIN reopen / runtime compare re-exec; do not redefine `within-mark`)
- `docs/HOST-PARITY.md` (wave8 **4**, optional — `uless-demo` CONTRACT parity welcome)
- `docs/HOLD.md` (wave23 **2** — prior tip; keep cites; leave primary untouched this tip; do not redefine `hold-mark`; HERE stub `$1000` untouched)
- `docs/CHAR-PLUS.md` (wave23 **1** — prior tip; keep cites; leave primary untouched this tip; do not redefine `char-plus-mark`)
- `docs/SEARCH-WORDLIST.md` (wave22 **4** — prior tip; keep cites; leave primary untouched this tip)
- `docs/TO-NUMBER.md` (wave22 **3** — prior tip; keep cites; leave primary untouched this tip — do not amend TO-NUMBER)
- `docs/LSHIFT-RSHIFT.md` (wave22 **1** — prior tip; keep cites; leave primary untouched this tip)
- `docs/BITWISE.md` / `docs/BASE-HEX.md` / `docs/ACCEPT-REFILL.md` / wave20 COUNT/EXECUTE / wave19 SOURCE-PAD/CHAR-CHARS (prior; keep cites)
- `forth/tritium/kernel.fs` (u-less-mark only — do not redefine host `U<` when colliding; **do not** redefine zero-eq-mark/zero-ne-mark / compare-mark / within-mark / hold-mark / char-plus-mark / true-mark/false-mark)
- ANS Forth `U<` (unsigned compare flag mark only — not WITHIN reopen; not ZERO-EQUALS reopen; not COMPARE reopen; not boolean cell; host `U<` ≠ force bare bind — prefer `u-less-mark`)
- Explicit deferral: WAVE19-PROPOSAL + WAVE20-PROPOSAL + WAVE21-PROPOSAL + WAVE22-PROPOSAL + WAVE23-PROPOSAL (`U<` — unsigned compare flag mark; not WITHIN reopen; not ZERO-EQUALS/COMPARE reopen; not boolean cell)
- Base tip: `98147d08` / `98147d08c54ada2ad739fd4d8586d750b965d59e` (#108 wave23 tip2 HOLD PASS)
- Wave23 proposal: `/workspace/tritium-research-docs/WAVE23-PROPOSAL.md`

## 10. Shipper implementation notes (Linux SoT)

These notes are normative for Shipper drafting on branch `shipper/u-less-w23` from base `98147d08c54ada2ad739fd4d8586d750b965d59e`. They do **not** authorize merge, push, Mango touch, opaque-weight ML, or reopening closed wave17–22 tips / wave23 tip1 CHAR-PLUS primary / wave23 tip2 HOLD primary.

### 10.1 Binding order

1. Prefer Forth mirror **`u-less-mark`** first. Lab greps do not require the bare ANS name `U<` to be bound when the mirror prints `[uless] U<`.
2. If bare `U<` is free on the Linux REPL load path, Shipper **may** bind it as a thin alias that emits the same markers — still **must not** redefine `zero-eq-mark`/`zero-ne-mark` / `compare-mark` / `within-mark` / `hold-mark` / `char-plus-mark` / `true-mark`/`false-mark`.
3. Never alias `U<` onto `zero-eq-mark` / `compare-mark` / `within-mark` / `hold-mark` / `char-plus-mark` / `TRUE`/`FALSE`. Unsigned compare ≠ zero-equals / string-compare / range-check / pictured start / char-advance / boolean constants.

### 10.2 Classic picture (unsigned less-than)

Document one fixed fixture pair. Recommended:

| Field | Example | Notes |
|-------|---------|-------|
| u1 | `1` | Classic left unsigned operand |
| u2 | `2` | Classic right unsigned operand |
| result | true (`flag=-1` or `flag=1`) | `1 <u 2` |
| optional not-less | `u1=2`, `u2=1` → `flag=0` | Welcome; not required |
| optional equal | `u1=5`, `u2=5` → `flag=0` | Welcome; not required |
| optional high-bit | `0` vs `$FFFFFFFF` / `-1` as unsigned max | Welcome when free — not required; not WITHIN reopen |

Shipper may use other unsigned stub ints — document which. Lab greps `[uless] U<` regardless of which fixture is chosen, as long as the marker line is greppable and FAIL is avoided. **Do not** implement unsigned compare by reopening WITHIN range-check or by redefining `0=`.

### 10.3 What Lab greps / does not grep

**Required:** `[uless-demo] OK` + `[uless] U<`. **Optional welcome:** `flag=`; high-bit unsigned picture; `uless-demo CONTRACT`. **Must NOT Lab-grep as success:** `[zero]`/`zero-eq-mark`; `[compare]`/`compare-mark`/`cstr=`; `[within]`/`within-mark`; `[true]`/`true-mark`/`false-mark`; `[hold]`/`hold-mark`; `[char+]`/`char-plus-mark`; `ABS`/`NEGATE`; `[search]`/`[shift]`/`[bit]`/`[number]`/`[base]`/`[cell]`.

### 10.4 Regression + companions + non-goals

Retain green: `hold-demo` / `charplus-demo` / `search-demo` / `number-demo` / `zero-demo` / `shift-demo` / `bit-demo` / `compare-demo` / `base-demo` / `accept-demo` / `exec-demo` / `count-demo` / `within-demo` / `true-demo` / `env-demo` / `char-demo` / `cell-demo` / `allot-demo` / `kernel-demo` / `trit-math-demo` / `fold-demo` (+ earlier). Required thin: ZERO-EQUALS + COMPARE + KERNEL. Optional: TRUE-FALSE / WITHIN (cite-only) / HOST-PARITY. **Untouched:** ARCHITECTURE / IMPLEMENTATION-GAPS / WAVE23-PROPOSAL / WAVE23-COS-PASTE / HOLD primary / CHAR-PLUS primary / LSHIFT-RSHIFT / SEARCH-WORDLIST / TO-NUMBER.

Do **not**: redefine zero-eq-mark/zero-ne-mark / compare-mark / within-mark / hold-mark / char-plus-mark / true-mark/false-mark; reopen WITHIN / ZERO-EQUALS / COMPARE / HOLD / CHAR-PLUS; land boolean cell / ABS/NEGATE; amend ARCH/GAPS / HOLD.md / CHAR-PLUS.md / TO-NUMBER.md; tip Shipper / push / merge / touch Mango; redefine host 2dup/2drop/2swap; polish ABORT".

U-LESS = thin **unsigned compare flag mark** only. Sibling to ZERO-EQUALS / COMPARE / WITHIN — **not** a reopen. Prefer `u-less-mark`. Classic: u1 u2 → true when u1 <u u2. Preferred emit: `[uless] U< flag=-1` — minimum `[uless] U<` — close `[uless-demo] OK`.

Research drafts under `/workspace/tritium-research-docs/`. Shipper byte-copies into repo `docs/` on `shipper/u-less-w23` only. Parent tips Shipper after. Research does **not** tip Shipper / push / merge / touch Mango.

Still deferred: ABS/NEGATE (tip4); DOCS-CITES (tip5); WITHIN/ZERO-EQUALS/COMPARE/HOLD/CHAR-PLUS reopen; boolean cell; full `#S`/`#>`/`SIGN`; unicode/XCHAR; FIND reopen; full Win/Android Forth VM; 2DUP-FAMILY; ABORT" polish; Mango.

Acceptance one-liner: `uless-demo` prints greppable `[uless] U<` and ends `[uless-demo] OK`; prior zero/compare/within/true/hold/charplus demos still OK; mirrors not redefined; Linux SoT; no merge.

Paste anchors: WAVE23-PROPOSAL.md § Tip 3 (~lines 50–59); base `98147d08c54ada2ad739fd4d8586d750b965d59e`; branch `shipper/u-less-w23`; handoff `WAVE23-TIP3-HANDOFF.md` (= `TIP3-HANDOFF.md`).

### 10.5 Disambiguation (Shipper)

| Surface | Tip | This tip? |
|---------|-----|-----------|
| `U<` / `u-less-mark` | wave23 **3** | **YES** |
| `0=` / `zero-eq-mark` | wave22 **2** | NO — do not redefine |
| `COMPARE` / `compare-mark` | wave21 **3** | NO — do not redefine |
| `WITHIN` / `within-mark` | wave20 **2** | NO — do not redefine |
| `TRUE`/`FALSE` | wave20 **1** | NO — do not redefine |
| `HOLD` / `hold-mark` | wave23 **2** | NO — leave HOLD.md untouched |
| `CHAR+` / `char-plus-mark` | wave23 **1** | NO — leave CHAR-PLUS.md untouched |
| `ABS` / `NEGATE` | wave23 **4** | NO — tip4 |

Unsigned less-than (`U<`) ≠ zero-equals (`0=`) ≠ range-check (`WITHIN`) ≠ string compare (`COMPARE`). Optional high-bit unsigned fixture welcome when free — **not required**. Leave HOLD.md (`dcc8a16c5f2999dd3aba7d828383f475` / 30969) + CHAR-PLUS.md (`bb879e6fe97f8f8ebd25418d43b43751` / 30666) byte-identical. HERE stub `$1000` untouched. Docs ONLY under `/workspace/tritium-research-docs/`. Skip 2DUP-FAMILY + ABORT" polish. Parent tips Shipper after.
