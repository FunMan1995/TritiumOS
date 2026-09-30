# MIN-MAX — `MIN` / `MAX` signed min/max marks + `minmax-demo`

**Status:** Shipper-ready stub spec (wave24 item **2**)
**Canonical brief:** ANS-shaped `MIN` / `MAX` (thin signed min/max marks only); `docs/ABS-NEGATE.md` (wave23 **4** — signed magnitude / negate companion; **not** an ABS reopen; **do not** redefine `abs-mark`/`negate-mark`); `docs/U-LESS.md` (wave23 **3** — unsigned compare companion; **not** a U-LESS reopen; **do not** redefine `u-less-mark`); `docs/ZERO-EQUALS.md` (wave22 **2** — `0=` flag companion; **not** ZERO-EQUALS reopen; **do not** redefine `zero-eq-mark`/`zero-ne-mark`); `docs/TRUE-FALSE.md` (wave20 **1** — constant-picture companion; **not** boolean cell; **do not** redefine `true`/`false` / `true-mark`/`false-mark`); `docs/WITHIN.md` (wave20 **2** — range-check cite-only; **not** WITHIN reopen; **do not** redefine `within-mark`); `docs/KERNEL.md` (wave7 **5**); optional `docs/HOST-PARITY.md` (wave8 **4**); WAVE23–24 deferral closed as **signed min/max marks only** (not boolean cell rewrite; **not** ABS/U</ZERO-EQUALS/WITHIN reopen; **not** BITWISE/LSHIFT reopen; **not** SPACE-SPACES-TYPE reopen; **not** SHARP-SIGN tip3; **not** WORDLIST tip4; **not** DOCS-CITES tip5; prefer Forth mirrors **`min-mark` / `max-mark`** whenever host `MIN`/`MAX` collide). **CRITICAL:** kernel `MAX-ENTRIES` / `MAX-SYNONYMS` / `MAX-CHAR` are **NOT** ANS `MAX` — leave them untouched. **Do not** redefine `space-mark`/`spaces-mark`/`type-mark`/`emit-mark` / `hold-mark` / `char-plus-mark`.
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `minmax.fs` / `min-max.fs`); Linux host REPL; prefer Forth mirrors **`min-mark` / `max-mark`** whenever host `MIN`/`MAX` collide; **do not** redefine `abs-mark`/`negate-mark` / `u-less-mark` / `zero-eq-mark`/`zero-ne-mark` / `within-mark` / `TRUE`/`FALSE`/`true-mark`/`false-mark` / `space-mark`/`spaces-mark`/`type-mark`/`emit-mark` / `hold-mark` / `char-plus-mark`; **CRITICAL: do not** redefine / alias / Lab-grep kernel `MAX-ENTRIES` / `MAX-SYNONYMS` / `MAX-CHAR` as ANS `MAX`
**Companions:** `docs/ABS-NEGATE.md` (thin amend this tip), `docs/U-LESS.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip); optional light cite `docs/ZERO-EQUALS.md` / `docs/TRUE-FALSE.md` / `docs/HOST-PARITY.md`
**Base tip SHA:** `cc01d3fa` (wave24 tip1 SPACE-SPACES-TYPE PASS / #112) / full `cc01d3fac331b09f2e7cfee5e4782c1edcf23490`

## 1. Purpose

WAVE20 tip **1** landed `TRUE` / `FALSE` constant marks (`docs/TRUE-FALSE.md`). WAVE20 tip **2** landed `WITHIN` range-check mark (`docs/WITHIN.md` — **not** min/max). WAVE22 tip **2** landed `0=` flag marks (`docs/ZERO-EQUALS.md`). WAVE23 tip **3** landed `U<` (`docs/U-LESS.md`; thin companion this tip — **not** U-LESS reopen). WAVE23 tip **4** landed `ABS` / `NEGATE` (`docs/ABS-NEGATE.md`; thin companion this tip — **not** ABS reopen). WAVE24 tip **1** landed `SPACE` / `SPACES` / `TYPE` (`docs/SPACE-SPACES-TYPE.md`; leave primary untouched). WAVE23–24 deferred `MIN` / `MAX` as signed min/max marks beside ABS/NEGATE + `0=` + `U<` (**not** boolean cell rewrite; **not** ABS/U</ZERO-EQUALS/WITHIN reopen). This tip lands **stub** signed min/max marks only: `MIN` (or **`min-mark`**) prints `[minmax] MIN` (+ optional `n=` — classic signed minimum: **n1 n2 → lesser**); `MAX` (or **`max-mark`**) prints `[minmax] MAX` (+ optional `n=` — classic signed maximum: **n1 n2 → greater**). Smoke via **`minmax-demo`**. Prefer mirrors **`min-mark` / `max-mark`** whenever host `MIN`/`MAX` collide. Pairs with wave23 ABS/NEGATE + wave22 `0=` + wave23 `U<` **without** boolean-cell rewrite and **without** reopening ABS / U< / ZERO-EQUALS / WITHIN. **CRITICAL:** kernel `MAX-ENTRIES` / `MAX-SYNONYMS` / `MAX-CHAR` are **NOT** ANS `MAX` — leave untouched. **Not** tip3 SHARP-SIGN / tip4 WORDLIST / tip5 DOCS-CITES. **Leave untouched:** SPACE-SPACES-TYPE (`f869c0b9990b023d2cef8d52beb19155`/30928), HOLD (`dcc8a16c5f2999dd3aba7d828383f475`/30969), CHAR-PLUS (`bb879e6fe97f8f8ebd25418d43b43751`/30666), ARCH (`2575a64f907756ab5ef78afa485f9a16`/18387), GAPS (`1f2a5d9969d394e716dcb25c6234cf03`/63563), WAVE24-PROPOSAL (`1cceddbdf85d16eef46863f03aec8c27`), COS-PASTE (`df576fdb220979cfa9e2f7dbf4c1c416`). ABS-NEGATE + U-LESS are thin-amended companions this tip, not reopened as primaries.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `MIN` / `min-mark` | `( n1 n2 -- n3 )` *or* `( -- )` with fixed demo fixture | Signed minimum mark; print `[minmax] MIN` (+ optional `n=<lesser>`); classic `n1 n2 → lesser` (signed) |
| `MAX` / `max-mark` | `( n1 n2 -- n3 )` *or* `( -- )` with fixed demo fixture | Signed maximum mark; print `[minmax] MAX` (+ optional `n=<greater>`); classic `n1 n2 → greater` (signed) |
| `minmax-demo` | `( -- )` | See §5 |

Host note: bind bare `MIN` / `MAX` on the Linux REPL **only if** those names do not collide with host Forth. Prefer Forth mirrors **`min-mark` / `max-mark`** when in doubt — **do not** redefine host `MIN`/`MAX`. **CRITICAL:** kernel `MAX-ENTRIES` / `MAX-SYNONYMS` / `MAX-CHAR` are **NOT** ANS `MAX` — do **not** redefine / alias / Lab-grep those as this tip’s success. Optional `n=` is host int / fixture echo only — not a live boolean cell / ABS / U-LESS / ZERO-EQUALS / WITHIN / SPACE-SPACES-TYPE reopen. Prefer **fixed demo signed stub-int fixtures** so Lab hit is deterministic. Classic MIN: `3 5 MIN → n=3` (optional `-2 7 MIN → n=-2`). Classic MAX: `3 5 MAX → n=5` (optional `-2 7 MAX → n=7`). Greppable `[minmax] MIN` + `[minmax] MAX` are enough for Lab OK. **Do not** redefine `abs-mark`/`negate-mark` / `u-less-mark` / `zero-eq-mark`/`zero-ne-mark` / `within-mark` / `TRUE`/`FALSE`/`true-mark`/`false-mark` / `space-mark`/`spaces-mark`/`type-mark`/`emit-mark` / `hold-mark` / `char-plus-mark` / `and-mark`/`or-mark`/`xor-mark`/`invert-mark` / `lshift-mark`/`rshift-mark`.

## 3. Stub semantics

- **`MIN` / `min-mark`:** take (or use fixed demo) signed stub ints `( n1 n2 )`. Classic ANS picture: signed minimum → `n3 = lesser of n1,n2`. Print `[minmax] MIN` (+ optional `n=<lesser>`). Classic fixture: `3 5 → n=3`; `-2 7 → n=-2`; equal `5 5 → n=5`. **Does not** rewrite TRUE/FALSE, reopen ABS / U-LESS / ZERO-EQUALS / WITHIN / SPACE-SPACES-TYPE / BITWISE / LSHIFT / HOLD / CHAR-PLUS, bump HERE, open a heap, or create dict entries.
- **`MAX` / `max-mark`:** take (or use fixed demo) signed stub ints `( n1 n2 )`. Classic ANS picture: signed maximum → `n3 = greater of n1,n2`. Print `[minmax] MAX` (+ optional `n=<greater>`). Classic fixture: `3 5 → n=5`; `-2 7 → n=7`; equal `5 5 → n=5`. Marker only. **CRITICAL:** ANS-shaped signed `MAX` — **not** kernel `MAX-ENTRIES` / `MAX-SYNONYMS` / `MAX-CHAR` (those stay untouched).
- **Fixed demo signed stub-int fixtures (document):**
  - **Classic MIN lesser (required):** e.g. `n1=3`, `n2=5` → `MIN` → `n=3`. Demo must exercise **`MIN` / `min-mark`** so Lab greps `[minmax] MIN`.
  - **Classic MAX greater (required):** e.g. `n1=3`, `n2=5` → `MAX` → `n=5`. Demo must exercise **`MAX` / `max-mark`** so Lab greps `[minmax] MAX`.
  - **Optional negative / equal pictures:** `-2 7 MIN` / `-2 7 MAX` / `5 5 MIN` / `5 5 MAX` — welcome when free; do **not** require; do **not** promote to boolean cell / ABS / U-LESS / ZERO-EQUALS / WITHIN reopen.
  - **Optional result echo:** `n=3` / `n=5` / `n=-2` — document which form Shipper emits. Greppable markers alone are enough when both `[minmax] MIN` and `[minmax] MAX` appear.
- **Optional push:** push pictured result if host stack is easy; marker alone is enough for Lab OK — do not require boolean cell / ABS / U-LESS / ZERO-EQUALS / WITHIN reopen.
- **FAIL:** `[minmax] FAIL reason=<…>` optional (demo **must avoid**). Prefer not emitting FAIL on the happy path.
- Storage: fixed demo signed stub ints / host int echo only. **No** boolean cell rewrite; no ABS / U-LESS / ZERO-EQUALS / WITHIN / SPACE-SPACES-TYPE / BITWISE / LSHIFT / HOLD / CHAR-PLUS reopen (ABS-NEGATE + U-LESS thin companion amends only; SPACE-SPACES-TYPE / HOLD / CHAR-PLUS primaries untouched); no HERE bump; no arena; **no** touch to kernel `MAX-ENTRIES` / `MAX-SYNONYMS` / `MAX-CHAR`.
- **Host `MIN` / `MAX` stay untouched when colliding:** prefer mirrors. Lab greps are `[minmax] MIN`, `[minmax] MAX`, and `[minmax-demo] OK` only — **never** Lab-grep bare host `MIN`/`MAX` or kernel `MAX-ENTRIES`/`MAX-SYNONYMS`/`MAX-CHAR` as this tip’s success when using mirrors.
- **Prior marks stay:** do **not** redefine `abs-mark`/`negate-mark` / `u-less-mark` / `zero-eq-mark`/`zero-ne-mark` / `within-mark` / `TRUE`/`FALSE`/`true-mark`/`false-mark` / `space-mark`/`spaces-mark`/`type-mark`/`emit-mark` / `hold-mark` / `char-plus-mark` / `and-mark`/`or-mark`/`xor-mark`/`invert-mark` / `lshift-mark`/`rshift-mark`. Signed min/max marks are **siblings** beside magnitude / unsigned-compare / zero-equals / range-check / constant / output marks — not a reopen of any.
- Nest with prior space / abs / uless / hold / charplus / search / number / zero / shift / bit / compare / base / accept / exec / count / within / true / string / word + earlier stubs OK. Assert prior `space-demo` / `abs-demo` / `uless-demo` / `hold-demo` / `charplus-demo` / `zero-demo` / `true-demo` / `within-demo` still OK.
- Still no boolean cell; no ABS / U-LESS / ZERO-EQUALS / WITHIN / SPACE-SPACES-TYPE / BITWISE / LSHIFT / HOLD / CHAR-PLUS reopen; no tip3–5; no linked XT / full arena / full Win/Android Forth VM. Leave SPACE-SPACES-TYPE / HOLD / CHAR-PLUS / ARCH / GAPS / WAVE24-PROPOSAL / COS-PASTE byte-identical (md5s §1 / §8). ABS-NEGATE + U-LESS thin-amended companions only (new md5s in tip summary).

## 4. Markers

```
[minmax] MIN [n=<lesser>]          # n= optional; classic signed minimum: n1 n2 → lesser
[minmax] MAX [n=<greater>]         # n= optional; classic signed maximum: n1 n2 → greater
[minmax] FAIL reason=<…>           # demo avoids
[minmax-demo] OK
[minmax-demo] FAIL
```

Lab greps `[minmax-demo] OK` plus greppable **`[minmax] MIN`** and greppable **`[minmax] MAX`** (optional `n=` welcome). Demo avoids `[minmax] FAIL`. **Do not** Lab-grep `[abs]` / `[uless]` / `[zero]` / `[within]` / `[true]` / `[space]` / `[hold]` / `[char+]` / `[bit]` / `[shift]` / `MAX-ENTRIES` / `MAX-SYNONYMS` / `MAX-CHAR` / `#` / `#>` / `SIGN` / `WORDLIST` / DOCS-CITES as this tip’s success (those stay prior/later tips or kernel caps).

## 5. `minmax-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; signed min/max marks need no dict entries.
2. Ensure **fixed demo signed stub-int fixtures** exist (e.g. MIN: `n1=3`, `n2=5` → lesser `3`; MAX: `n1=3`, `n2=5` → greater `5`) so classic pictures are deterministic. Fixtures may live beside ABS / U-LESS / ZERO-EQUALS / WITHIN / TRUE-FALSE / SPACE fixtures — do **not** require boolean-cell rewrite / ABS / U-LESS / ZERO-EQUALS / WITHIN / SPACE-SPACES-TYPE reopen.
3. Invoke `MIN` (or **`min-mark`**) against classic lesser fixture → `[minmax] MIN` (+ optional `n=`).
4. Invoke `MAX` (or **`max-mark`**) against classic greater fixture → `[minmax] MAX` (+ optional `n=`).
5. Optional (when free): negative / equal MIN/MAX pictures — **do not require**.
6. Assert no `[minmax] FAIL` on the happy path. Assert no boolean cell / ABS / U-LESS / ZERO-EQUALS / WITHIN / SPACE-SPACES-TYPE / BITWISE / LSHIFT / HOLD / CHAR-PLUS reopen / HERE bump / host `MIN`/`MAX` redefine when using mirrors. Assert prior mirrors (§2) were **not** redefined. Assert kernel `MAX-ENTRIES` / `MAX-SYNONYMS` / `MAX-CHAR` were **not** redefined / aliased / Lab-grepped as ANS `MAX`. Assert SPACE-SPACES-TYPE / HOLD / CHAR-PLUS / ARCH / GAPS / WAVE24-PROPOSAL / COS-PASTE left untouched. Assert prior `space-demo` / `abs-demo` / `uless-demo` / `hold-demo` / `charplus-demo` / `zero-demo` / `true-demo` / `within-demo` still OK.
7. Prior space/abs/uless/hold/charplus/search/number/zero/shift/bit/compare/base/accept/exec/count/within/true/string/word demos + trit-math/fold still OK.
8. `[minmax-demo] OK`.

Required: `[minmax] MIN` + `[minmax] MAX`. Optional `n=` not required for Lab OK. No boolean cell; no ABS / U-LESS / ZERO-EQUALS / WITHIN / SPACE-SPACES-TYPE reopen; no host `MIN`/`MAX` redefine when using mirrors; no kernel `MAX-*` confuse.

## 6. Thin amend — companions

### `docs/ABS-NEGATE.md`

- Companions / Status: add `MIN-MAX.md` (wave24 **2** companion cite); **keep** BITWISE / TRUE-FALSE / KERNEL / LSHIFT / U-LESS / HOLD / CHAR-PLUS / ZERO-EQUALS / PICK-ROLL / HOST-PARITY cites — do not wipe wave23 ABS-NEGATE content.
- Purpose / §3 / non-goals: ABS/NEGATE stay magnitude / negate marks; `MIN`/`MAX` are sibling **signed min/max marks** — **not** an ABS reopen / boolean-cell rewrite / U-LESS / ZERO-EQUALS / WITHIN reopen. Stress: MIN-MAX sibling — **NOT** ABS reopen; prefer `min-mark`/`max-mark`; **do not** redefine `abs-mark`/`negate-mark`. **CRITICAL:** kernel `MAX-ENTRIES`/`MAX-SYNONYMS`/`MAX-CHAR` ≠ ANS `MAX`.
- Non-goals: `MIN` / `MAX` → `docs/MIN-MAX.md` (wave24 **2**). ABS/NEGATE stay landed. Tip3 SHARP-SIGN / tip4 WORDLIST / tip5 DOCS-CITES still later.
- Acceptance: Lab smokes `minmax-demo` (retains `abs-demo` + `uless-demo` + `zero-demo` + `true-demo` + `within-demo` + `space-demo` + `hold-demo` + `charplus-demo`).
- Cite: `docs/MIN-MAX.md`.

### `docs/U-LESS.md`

- Companions / Status: add `MIN-MAX.md` (wave24 **2** companion cite); **keep** ZERO-EQUALS / COMPARE / KERNEL / TRUE-FALSE / WITHIN / HOLD / CHAR-PLUS / HOST-PARITY cites — do not wipe wave23 U-LESS content.
- Purpose / §3 / non-goals: `U<` stays unsigned compare flag; `MIN`/`MAX` are sibling **signed min/max marks** — **not** a U-LESS reopen / boolean cell / ABS / ZERO-EQUALS / WITHIN reopen. Stress: MIN-MAX sibling — **NOT** U-LESS reopen; prefer `min-mark`/`max-mark`; **do not** redefine `u-less-mark`. Signed MIN/MAX ≠ unsigned `U<`.
- Non-goals: `MIN` / `MAX` → `docs/MIN-MAX.md` (wave24 **2**). `U<` / `u-less-mark` stay landed. Tip3–5 still later.
- Acceptance: Lab smokes `minmax-demo` (retains `uless-demo` + `abs-demo` + `zero-demo` + `compare-demo` + `within-demo` + `space-demo` + `hold-demo` + `charplus-demo`).
- Cite: `docs/MIN-MAX.md`.

### `docs/KERNEL.md`

- Companions: add `MIN-MAX.md` (wave24 **2**); **keep** wave24 tip1 SPACE-SPACES-TYPE + wave23 tip1–4 + wave22 tip1–4 + wave21 tip1–4 + wave20–15 cites.
- Words table: add `MIN` / `min-mark` + `MAX` / `max-mark` stubs + `minmax-demo` (Forth mirrors — signed min/max only; prefer mirrors whenever host collide; **CRITICAL: kernel `MAX-ENTRIES`/`MAX-SYNONYMS`/`MAX-CHAR` are NOT ANS `MAX`**; **do not** redefine abs-/u-less-/zero-/within-/true-/space-/hold-/char-plus-/bitwise-/shift marks; **not** boolean cell / ABS / U-LESS / ZERO-EQUALS / WITHIN / SPACE-SPACES-TYPE reopen; optional `n=` — classic signed min/max pictures welcome).
- Non-goals: `MIN` / `MAX` → `docs/MIN-MAX.md`. ABS/NEGATE stay on `ABS-NEGATE.md` (thin amend). `U<` stays on `U-LESS.md` (thin amend). SPACE/SPACES/TYPE stay on `SPACE-SPACES-TYPE.md` (leave primary untouched). HOLD / CHAR+ leave primaries untouched. Tip3 SHARP-SIGN / tip4 WORDLIST / tip5 DOCS-CITES still later.
- Acceptance: Lab smokes `minmax-demo` (retains `space-demo` + `abs-demo` + `uless-demo` + `hold-demo` + `charplus-demo` + `search-demo` + `number-demo` + `zero-demo` + `shift-demo` + `bit-demo` + `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `string-demo` + `word-demo` + prior incl `trit-math-demo` / `fold-demo`).
- Cite: `docs/MIN-MAX.md`.

### Optional — `docs/ZERO-EQUALS.md`

- Light `MIN-MAX.md` (wave24 **2**) cite; sibling signed min/max — **NOT** ZERO-EQUALS reopen / boolean cell; prefer `min-mark`/`max-mark`; do not redefine `zero-eq-mark`/`zero-ne-mark`. Retain `zero-demo`.

### Optional — `docs/TRUE-FALSE.md`

- Light `MIN-MAX.md` (wave24 **2**) cite; sibling signed min/max — **NOT** boolean cell / TRUE-FALSE reopen; prefer mirrors; do not redefine `true-mark`/`false-mark`. Retain `true-demo`.

### Optional — `docs/HOST-PARITY.md`

- Light `MIN-MAX.md` (wave24 **2**) cite; `minmax-demo CONTRACT` welcome — **not** full Win/Android Forth VM. Prefer mirrors. **CRITICAL:** kernel `MAX-*` ≠ ANS `MAX`.

Do **not** wipe wave24 tip1 / wave23 tip1–4 / wave22–15 prior content. Amends: ABS-NEGATE + U-LESS + KERNEL (+ optional ZERO-EQUALS / TRUE-FALSE / HOST-PARITY) only. **Leave untouched:** SPACE-SPACES-TYPE (`f869c0b9990b023d2cef8d52beb19155`/30928); HOLD (`dcc8a16c5f2999dd3aba7d828383f475`/30969); CHAR-PLUS (`bb879e6fe97f8f8ebd25418d43b43751`/30666); ARCH (`2575a64f907756ab5ef78afa485f9a16`); GAPS (`1f2a5d9969d394e716dcb25c6234cf03`); WAVE24-PROPOSAL (`1cceddbdf85d16eef46863f03aec8c27`); COS-PASTE (`df576fdb220979cfa9e2f7dbf4c1c416`). Tip5 after 1–4 PASS. Skip 2DUP-FAMILY + ABORT" polish.

## 7. Non-goals

- Redefining / aliasing / shadowing host `MIN` / `MAX` when colliding — prefer Forth mirrors `min-mark` / `max-mark`
- Redefining / aliasing / Lab-grep-as-success kernel `MAX-ENTRIES` / `MAX-SYNONYMS` / `MAX-CHAR` as ANS `MAX` — **CRITICAL:** those stay untouched
- Redefining `abs-mark` / `negate-mark` / host `ABS`/`NEGATE` (ABS-NEGATE stays — thin companion amend only — **not** ABS reopen)
- Redefining `u-less-mark` / host `U<` (U-LESS stays — thin companion amend only — **not** U-LESS reopen)
- Redefining `zero-eq-mark` / `zero-ne-mark` / host `0=` / `0<>` (ZERO-EQUALS stays — optional thin cite — **not** ZERO-EQUALS reopen)
- Redefining `within-mark` / host `WITHIN` (WITHIN stays — **not** WITHIN reopen / runtime compare re-exec)
- Redefining `TRUE` / `FALSE` / `true-mark` / `false-mark` (TRUE-FALSE stays — **not** boolean cell)
- Redefining `space-mark` / `spaces-mark` / `type-mark` / `emit-mark` (SPACE-SPACES-TYPE stays — leave primary untouched)
- Redefining `hold-mark` / host `HOLD` / bumping `_here`/`HERE`/`here-at` (HOLD stays; HERE stub `$1000` untouched)
- Redefining `char-plus-mark` / `CHAR`/`CHARS`/`[CHAR]`/`char-unit`/`chars-n`/`bracket-char` (CHAR-PLUS stays — leave primary untouched)
- Redefining `and-mark`/`or-mark`/`xor-mark`/`invert-mark` / `lshift-mark`/`rshift-mark` (BITWISE / LSHIFT stay)
- Real boolean cell rewrite / live flag storage / VARIABLE-as-boolean / flag algebra deepen
- `ABS-NEGATE` / `U-LESS` / `ZERO-EQUALS` / `WITHIN` / `BITWISE` / `LSHIFT` / `SPACE-SPACES-TYPE` reopen (already stubbed; thin ABS/U-LESS companions only; leave SPACE primary untouched)
- Thin pictured continue `#` / `#>` / `SIGN` / optional `#S` (tip3 SHARP-SIGN — still deferred; **not** HOLD reopen; do not bump HERE `$1000`)
- `WORDLIST` / `FORTH-WORDLIST` marks (tip4 — still deferred)
- Docs cites pass (wave24 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `COMPARE` / `SEARCH-WORDLIST` / `FIND` / `BASE-HEX` / `ACCEPT-REFILL` / `TO-NUMBER` / `COUNT` / `EXECUTE` reopen
- Real DOES> XT / real branch XT / LEAVE jump / full arena / linked XT / real STATE cell
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (**skip 2DUP-FAMILY**)
- `ABORT"` polish (**skip** — already in `throw-demo`)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/MIN-MAX.md` present (Research byte-copy OK); `ABS-NEGATE.md` + `U-LESS.md` + `KERNEL.md` thin amends present (+ optional ZERO-EQUALS / TRUE-FALSE / HOST-PARITY); prior cites retained; host `MIN`/`MAX` untouched via mirrors; prior mirrors (§2) **not** redefined; kernel `MAX-ENTRIES`/`MAX-SYNONYMS`/`MAX-CHAR` untouched; SPACE-SPACES-TYPE untouched (`f869c0b9990b023d2cef8d52beb19155`/30928); HOLD untouched (`dcc8a16c5f2999dd3aba7d828383f475`/30969); CHAR-PLUS untouched (`bb879e6fe97f8f8ebd25418d43b43751`/30666); ARCH (`2575a64f907756ab5ef78afa485f9a16`/18387) + GAPS (`1f2a5d9969d394e716dcb25c6234cf03`/63563) + WAVE24-PROPOSAL (`1cceddbdf85d16eef46863f03aec8c27`) + COS-PASTE (`df576fdb220979cfa9e2f7dbf4c1c416`) untouched.
2. `minmax-demo` → OK (markers §4; `[minmax] MIN` + `[minmax] MAX` greppable; optional `n=` welcome — classic signed min/max on stub ints; no FAIL on happy path; no boolean cell / ABS / U-LESS / ZERO-EQUALS / WITHIN / SPACE-SPACES-TYPE / BITWISE / LSHIFT reopen; no prior-mirror redefine; no `MAX-ENTRIES`/`MAX-SYNONYMS`/`MAX-CHAR` confuse). Prior `space-demo` + `abs-demo` + `uless-demo` + `hold-demo` + `charplus-demo` + `search-demo` + `number-demo` + `zero-demo` + `shift-demo` + `bit-demo` + `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `string-demo` + `word-demo` + earlier incl `trit-math-demo` / `fold-demo` still OK.
3. Regression green (wave24 tip1 + wave23 tip1–5 + wave22–14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `minmax-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**)
- `docs/ABS-NEGATE.md` (wave23 **4** — magnitude/negate companion; MIN-MAX sibling signed min/max — **not** ABS reopen; prefer `min-mark`/`max-mark`; do not redefine `abs-mark`/`negate-mark`)
- `docs/U-LESS.md` (wave23 **3** — unsigned compare companion; MIN-MAX sibling — **not** U-LESS reopen; prefer mirrors; do not redefine `u-less-mark`; signed MIN/MAX ≠ unsigned `U<`)
- `docs/ZERO-EQUALS.md` (wave22 **2**, optional — flag companion; **not** ZERO-EQUALS reopen; do not redefine `zero-eq-mark`/`zero-ne-mark`)
- `docs/TRUE-FALSE.md` (wave20 **1**, optional — constant companion; **not** boolean cell; do not redefine `true-mark`/`false-mark`)
- `docs/WITHIN.md` (wave20 **2**, optional cite-only — **not** WITHIN reopen; do not redefine `within-mark`)
- `docs/HOST-PARITY.md` (wave8 **4**, optional — `minmax-demo` CONTRACT welcome)
- `docs/SPACE-SPACES-TYPE.md` (wave24 **1** — leave primary untouched; do not redefine space-/spaces-/type-/emit-mark)
- `docs/HOLD.md` / `docs/CHAR-PLUS.md` (wave23 **1–2** — leave primaries untouched; HERE stub `$1000` untouched)
- `docs/BITWISE.md` / `docs/LSHIFT-RSHIFT.md` / `docs/COMPARE.md` / `docs/SEARCH-WORDLIST.md` / `docs/TO-NUMBER.md` / `docs/BASE-HEX.md` / `docs/ACCEPT-REFILL.md` / wave20 COUNT/EXECUTE / wave19 SOURCE-PAD/CHAR-CHARS (prior; leave primaries untouched)
- `forth/tritium/kernel.fs` (min-mark / max-mark only — do not redefine host MIN/MAX when colliding; **CRITICAL: do not** redefine kernel `MAX-ENTRIES`/`MAX-SYNONYMS`/`MAX-CHAR` as ANS `MAX`)
- ANS Forth `MIN` / `MAX` (signed min/max marks only — prefer mirrors; kernel MAX-* caps ≠ ANS MAX)
- Explicit deferral: WAVE23-PROPOSAL + WAVE24-PROPOSAL (`MIN`/`MAX` — marks only)
- Base tip: `cc01d3fa` / `cc01d3fac331b09f2e7cfee5e4782c1edcf23490` (#112 wave24 tip1 SPACE-SPACES-TYPE PASS)
- Wave24 proposal: `/workspace/tritium-research-docs/WAVE24-PROPOSAL.md`

## 10. Shipper implementation notes (Linux SoT)

These notes are normative for Shipper drafting on branch `shipper/min-max-w24` from base `cc01d3fac331b09f2e7cfee5e4782c1edcf23490`. They do **not** authorize merge, push, Mango touch, opaque-weight ML, or reopening closed wave13–23 tips / wave24 tip1 SPACE-SPACES-TYPE primary / WAVE24-PROPOSAL.

### 10.1 Binding order

1. Prefer Forth mirrors **`min-mark`** and **`max-mark`** first. Lab greps do not require bare ANS names `MIN`/`MAX` when mirrors print `[minmax] MIN` / `[minmax] MAX`.
2. If bare `MIN` / `MAX` are free on the Linux REPL load path, Shipper **may** bind them as thin aliases that emit the same markers — still **must not** redefine abs-/u-less-/zero-/within-/true-/space-/hold-/char-plus-/bitwise-/shift marks.
3. Never alias `MIN`/`MAX` onto `abs-mark` / `u-less-mark` / `zero-eq-mark` / `within-mark` / `TRUE`/`FALSE` / kernel `MAX-ENTRIES`/`MAX-SYNONYMS`/`MAX-CHAR`. Signed min/max ≠ magnitude / unsigned compare / zero-equals / range-check / boolean constants / kernel caps.

### 10.2 Classic pictures (MIN + MAX)

Document one fixed fixture pair per word. Recommended:

| Field | Example | Notes |
|-------|---------|-------|
| MIN n1 / n2 | `3` / `5` | Classic signed operands |
| MIN result n | `3` | lesser of 3,5 |
| optional MIN negative | `-2 7 → n=-2` | Welcome; not required |
| optional MIN equal | `5 5 → n=5` | Welcome; not required |
| MAX n1 / n2 | `3` / `5` | Classic signed operands |
| MAX result n | `5` | greater of 3,5 |
| optional MAX negative | `-2 7 → n=7` | Welcome; not required |
| optional MAX equal | `5 5 → n=5` | Welcome; not required |

Shipper may use other signed stub ints — document which. Lab greps `[minmax] MIN` + `[minmax] MAX` regardless of fixtures, as long as both markers are greppable and FAIL is avoided. **Do not** implement MAX by redefining kernel `MAX-ENTRIES`/`MAX-SYNONYMS`/`MAX-CHAR`. **Do not** implement MIN/MAX by reopening ABS / U-LESS / ZERO-EQUALS / WITHIN / boolean cell.

### 10.3 What Lab greps / does not grep

**Required:** `[minmax-demo] OK` + `[minmax] MIN` + `[minmax] MAX`. **Optional welcome:** `n=`; negative / equal pictures; `minmax-demo CONTRACT`. **Must NOT Lab-grep as success:** `[abs]`/`abs-mark`/`negate-mark`; `[uless]`/`u-less-mark`; `[zero]`/`zero-eq-mark`; `[within]`/`within-mark`; `[true]`/`true-mark`/`false-mark`; `[space]`/`space-mark`/`spaces-mark`/`type-mark`/`emit-mark`; `[hold]`/`hold-mark`; `[char+]`/`char-plus-mark`; `[bit]`/`[shift]`; `MAX-ENTRIES`/`MAX-SYNONYMS`/`MAX-CHAR`; `#`/`#>`/`SIGN`; `WORDLIST`.

### 10.4 Regression + companions + non-goals

Retain green: `space-demo` / `abs-demo` / `uless-demo` / `hold-demo` / `charplus-demo` / `search-demo` / `number-demo` / `zero-demo` / `shift-demo` / `bit-demo` / `compare-demo` / `base-demo` / `accept-demo` / `exec-demo` / `count-demo` / `within-demo` / `true-demo` / `string-demo` / `word-demo` / `env-demo` / `cell-demo` / `allot-demo` / `kernel-demo` / `trit-math-demo` / `fold-demo` (+ earlier). Required thin: ABS-NEGATE + U-LESS + KERNEL. Optional: ZERO-EQUALS / TRUE-FALSE / HOST-PARITY. **Untouched:** ARCHITECTURE / IMPLEMENTATION-GAPS / WAVE24-PROPOSAL / WAVE24-COS-PASTE / SPACE-SPACES-TYPE primary / HOLD primary / CHAR-PLUS primary.

Do **not**: redefine abs/u-less/zero/within/true/space/hold/charplus/bitwise/shift mirrors; reopen ABS/U-LESS/ZERO-EQUALS/WITHIN/SPACE-SPACES-TYPE/BITWISE/LSHIFT/HOLD/CHAR-PLUS; land boolean cell / tip3 SHARP-SIGN / tip4 WORDLIST / tip5 DOCS-CITES; amend ARCH/GAPS / SPACE-SPACES-TYPE.md / HOLD.md / CHAR-PLUS.md / WAVE24-PROPOSAL; confuse ANS `MAX` with kernel `MAX-ENTRIES`/`MAX-SYNONYMS`/`MAX-CHAR`; tip Shipper / push / merge / touch Mango; redefine host 2dup/2drop/2swap; polish ABORT".

MIN-MAX = thin **signed min/max marks** only. Sibling to ABS-NEGATE / U-LESS / ZERO-EQUALS — **not** a reopen. Prefer `min-mark` / `max-mark`. Classic: MIN → lesser; MAX → greater. Preferred emit: `[minmax] MIN n=3` + `[minmax] MAX n=5` — minimum both markers — close `[minmax-demo] OK`.

Research drafts under `/workspace/tritium-research-docs/`. Shipper byte-copies into repo `docs/` on `shipper/min-max-w24` only. Parent tips Shipper after. Research does **not** tip Shipper / push / merge / touch Mango.

Still deferred: tip3 SHARP-SIGN; tip4 WORDLIST; tip5 DOCS-CITES; boolean cell; ABS/U</ZERO/WITHIN/SPACE reopen; full pictured beyond tip3 thin continue; FIND reopen / linked dict; full Win/Android Forth VM; 2DUP-FAMILY; ABORT" polish; Mango.

Acceptance one-liner: `minmax-demo` prints greppable `[minmax] MIN` + `[minmax] MAX` and ends `[minmax-demo] OK`; prior space/abs/uless demos still OK; mirrors not redefined; kernel MAX-* caps untouched; Linux SoT; no merge.

Paste anchors: WAVE24-PROPOSAL.md § Tip 2; base `cc01d3fac331b09f2e7cfee5e4782c1edcf23490`; branch `shipper/min-max-w24`; handoff `WAVE24-TIP2-HANDOFF.md` (= `TIP2-HANDOFF.md`).

### 10.5 Disambiguation (Shipper)

| Surface | Tip | This tip? |
|---------|-----|-----------|
| `MIN` / `min-mark` | wave24 **2** | **YES** |
| `MAX` / `max-mark` | wave24 **2** | **YES** |
| kernel `MAX-ENTRIES` / `MAX-SYNONYMS` / `MAX-CHAR` | kernel caps | **NO** — **NOT** ANS `MAX`; leave untouched |
| `ABS` / `NEGATE` / `abs-mark` / `negate-mark` | wave23 **4** | NO — ABS-NEGATE thin amend only |
| `U<` / `u-less-mark` | wave23 **3** | NO — U-LESS thin amend only |
| `0=` / `zero-eq-mark` | wave22 **2** | NO — optional ZERO-EQUALS thin cite |
| `WITHIN` / `within-mark` | wave20 **2** | NO — cite-only; not WITHIN reopen |
| `TRUE`/`FALSE` / `true-mark`/`false-mark` | wave20 **1** | NO — optional TRUE-FALSE thin cite |
| `SPACE`/`SPACES`/`TYPE` / space-*/type-*/emit-* | wave24 **1** | NO — leave SPACE-SPACES-TYPE.md untouched |
| `HOLD` / `hold-mark` | wave23 **2** | NO — leave HOLD.md untouched |
| `CHAR+` / `char-plus-mark` | wave23 **1** | NO — leave CHAR-PLUS.md untouched |
| `#` / `#>` / `SIGN` / optional `#S` | wave24 **3** | NO — tip3 SHARP-SIGN |
| `WORDLIST` / `FORTH-WORDLIST` | wave24 **4** | NO — tip4 |
| DOCS-CITES (ARCH/GAPS) | wave24 **5** | NO — tip5 |

Signed minimum (`MIN`) ≠ signed maximum (`MAX`) ≠ absolute value (`ABS`) ≠ unsigned compare (`U<`) ≠ zero-equals (`0=`) ≠ range-check (`WITHIN`) ≠ kernel `MAX-ENTRIES`/`MAX-SYNONYMS`/`MAX-CHAR`. Leave SPACE-SPACES-TYPE / HOLD / CHAR-PLUS / ARCH / GAPS / WAVE24-PROPOSAL / COS-PASTE byte-identical (md5s §8). HERE stub `$1000` untouched. Docs ONLY under `/workspace/tritium-research-docs/`. Skip 2DUP-FAMILY + ABORT" polish. Parent tips Shipper after.
