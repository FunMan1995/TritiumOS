# HOLD — thin pictured-numeric start (`HOLD` / `hold-mark` + optional `<#` / `#`) + `hold-demo`

**Status:** Shipper-ready stub spec (wave23 item **2**; thin amend wave24 **3** SHARP-SIGN companion cite)
**Canonical brief:** ANS-shaped `HOLD` (thin pictured-numeric **start** mark only); `docs/TO-NUMBER.md` (wave22 **3** — `>NUMBER` thin number-parse companion; **not** a TO-NUMBER reopen; **do not** redefine `to-number-mark`); `docs/BASE-HEX.md` (wave21 **2** — radix-mark companion; **not** BASE/HEX/DECIMAL reopen; **do not** redefine `base-mark`/`hex-mark`/`decimal-mark`); `docs/KERNEL.md` (wave7 **5**); optional `docs/ALLOT-HERE.md` (wave14 **1** — HERE stub reminder; **CRITICAL do not bump** `$1000 _here !`) / `docs/ENVIRONMENT-QUERY.md` (wave19 **3** — optional pictured-numeric stub cite; **not** full ANS ENVIRONMENT? table reopen) / `docs/HOST-PARITY.md` (wave8 **4**); WAVE19–23 deferral closed as **thin pictured-numeric start only** (wave24 **3** SHARP-SIGN lands thin `#`/`#>`/`SIGN` continue beside this tip — **not** a HOLD reopen; **not** full pictured rewrite beyond thin continue; **not** BASE reopen; **not** TO-NUMBER reopen; **not** CHAR-PLUS reopen; **CRITICAL:** do **not** redefine / alias / bump `_here` / `HERE` / `here-at` — HERE stub base stays `$1000`; **do not** redefine `HOLD`/`hold-mark`/`hold-demo`/`<#`).
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `hold.fs` / `pictured.fs`); Linux host REPL; prefer Forth mirror **`hold-mark`** whenever host `HOLD` collides; **CRITICAL — do not** redefine, alias, or bump `_here` / `HERE` / `here-at` (HERE stub base `$1000 _here !` stays landed); **do not** redefine `to-number-mark` (TO-NUMBER stays — **not** a TO-NUMBER reopen); **do not** redefine `BASE`/`HEX`/`DECIMAL`/`base-mark`/`hex-mark`/`decimal-mark` (BASE-HEX stays — **not** BASE reopen); **do not** redefine `char-plus-mark` (CHAR-PLUS stays — **not** a CHAR-PLUS reopen); **do not** require `#S` / `#>` / `SIGN`
**Companions:** `docs/TO-NUMBER.md` (thin amend this tip), `docs/BASE-HEX.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip); optional light cite `docs/ALLOT-HERE.md` / `docs/ENVIRONMENT-QUERY.md` / `docs/HOST-PARITY.md`; `docs/SHARP-SIGN.md` (wave24 **3** — thin pictured-numeric **continue** companion cite; **not** HOLD reopen; HERE stub `$1000` untouched; prefer `sharp-mark`/`sharp-end-mark`/`sign-mark`; **do not** redefine `HOLD`/`hold-mark`/`hold-demo`/`<#`)
**Base tip SHA:** `998b2fab` (wave23 tip1 PASS / #107 CHAR-PLUS) / full `998b2fab19b4a5d26d8fec2ceee9a27c886ec28a`

## 1. Purpose

WAVE14 tip **1** landed `HERE` / `ALLOT` (`docs/ALLOT-HERE.md`) with stub base `$1000 _here !`. WAVE21 tip **2** landed `BASE` / `HEX` / `DECIMAL` radix marks (`docs/BASE-HEX.md`; HERE stub `$1000` untouched). WAVE22 tip **3** landed `>NUMBER` thin number-parse mark (`docs/TO-NUMBER.md`; HERE stub `$1000` untouched; **not** pictured numeric). WAVE23 tip **1** landed `CHAR+` thin char-unit advance (`docs/CHAR-PLUS.md`; leave primary untouched this tip). WAVE18–23 deferred `HOLD` (and/or `<#` / `#`) as a thin pictured-numeric **start** beside TO-NUMBER + BASE-HEX (**not** full pictured `#S`/`#>`/`SIGN`; **not** BASE reopen; **not** TO-NUMBER reopen; **must not** bump HERE stub `$1000`). This tip lands **stub** thin pictured-numeric start only: `HOLD` (or Forth mirror **`hold-mark`**) prints `[hold] HOLD` (+ optional `u=` / `c=` for a classic hold-one-char into pictured buffer picture welcome). Optional `<#` / `#` marks welcome when free — **do not require** `#S` / `#>` / `SIGN`. Smoke via **`hold-demo`**. Prefer Forth mirror **`hold-mark`** whenever host `HOLD` collides. **CRITICAL:** kernel HERE stub base is `$1000 _here !` (“stub base”) — this tip must **NOT** redefine, alias, or bump `_here` / `HERE` / `here-at`. Pairs with wave22 TO-NUMBER + wave21 BASE-HEX **without** a BASE reopen, **without** a TO-NUMBER reopen, **without** a full pictured `#S`/`#>`/`SIGN` rewrite, and **without** reopening CHAR-PLUS / ZERO-EQUALS / LSHIFT / SEARCH-WORDLIST. Wave23 tip3–5 + wave24 tip1–2 stay landed elsewhere. Sibling thin pictured-numeric **continue** → `docs/SHARP-SIGN.md` (wave24 **3**; Forth mirrors `sharp-mark`/`sharp-end-mark`/`sign-mark` + optional `sharps-mark`; **not** a HOLD reopen / full pictured rewrite; HERE stub `$1000` untouched; **do not** redefine `HOLD`/`hold-mark`/`hold-demo`/`<#`).

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `HOLD` / `hold-mark` | `( char -- )` *or* `( -- )` with fixed demo fixture | Thin pictured-numeric start mark; print `[hold] HOLD` (+ optional `u=<n>` / `c=<char|ord>`); classic hold-one-char into pictured buffer picture welcome |
| `<#` / `hold-lt-number` (optional) | `( -- )` | Optional pictured-numeric begin mark; print `[hold] <#` when free — **do not require** |
| `#` / `hold-hash` (optional) | `( ud1 -- ud2 )` *or* `( -- )` | Optional pictured digit emit mark; print `[hold] #` when free — **do not require**; **not** `#S` / `#>` / `SIGN` |
| `hold-demo` | `( -- )` | See §5 |

Host note: bind bare `HOLD` on the Linux REPL **only if** that name does not collide with host Forth `HOLD`. Prefer Forth mirror **`hold-mark`** as the Lab-facing surface when in doubt — **do not** redefine host `HOLD`. **CRITICAL:** do **not** redefine, alias, or bump `_here` / `HERE` / `here-at` (HERE stub base `$1000 _here !` stays landed — this tip is a **thin pictured-numeric start**, not a HERE rewrite). **Do not** redefine `to-number-mark` — TO-NUMBER stays mark-only sibling (**not** a TO-NUMBER reopen). **Do not** redefine `BASE`/`HEX`/`DECIMAL`/`base-mark`/`hex-mark`/`decimal-mark` — BASE-HEX stays (**not** BASE reopen). **Do not** redefine `char-plus-mark` — CHAR-PLUS stays (**not** a CHAR-PLUS reopen). Optional `u=` / `c=` are host ints / char echo only — not a full pictured buffer VM, not a HERE bump, not BASE reopen, not `#S`/`#>`/`SIGN`. Prefer **fixed demo char fixtures** (classic pictured char e.g. `'A'` / `65` / digit `'0'+n` — document) so Lab hit is deterministic and FAIL is avoided. Optional `<#` / `#` marks are **welcome when free** — do **not** require them for Lab OK; do **not** require `#S` / `#>` / `SIGN`.

## 3. Stub semantics

- **`HOLD` / `hold-mark`:** take (or use fixed demo) a pictured character fixture — classic ANS sketch `( char -- )` where `char` is held into a pictured numeric buffer (pictured output builds right-to-left). This tip is **marker only**: print `[hold] HOLD` and optionally `u=<n>` and/or `c=<char|ord>` (Lab-greppable; prefer documenting which Shipper emits). Classic fixture welcome: hold one pictured char (e.g. `'A'` / ord `65` / digit char — document) → greppable `[hold] HOLD` (+ optional `u=` / `c=`). **Does not** rewrite a real pictured numeric buffer VM, implement full `#S`/`#>`/`SIGN`, reopen BASE/HEX/DECIMAL, reopen `>NUMBER`, allocate, or bump HERE. **Does not** redefine / alias / store into `_here` / `HERE` / `here-at`. Captured values are host ints / char echo only.
- **Optional `<#` / `hold-lt-number`:** picture classic pictured-numeric begin — print `[hold] <#` when free. Marker only — **do not require** for Lab OK. Does not open a real pictured buffer / bump HERE / reopen BASE.
- **Optional `#` / `hold-hash`:** picture classic pictured digit conversion step — print `[hold] #` when free. Marker only — **do not require**. **Not** `#S` (convert remaining), **not** `#>` (end pictured), **not** `SIGN` (optional leading minus) — those stay deferred beyond this thin start.
- **Fixed demo char fixtures (document):** classic hold-one-char (e.g. `'A'` / ord `65`) so Lab greps `[hold] HOLD`; optional `u=`/`c=` welcome. Optional `<#`/`#` when free — do **not** require `#S`/`#>`/`SIGN`. Fixture may sit beside `base-demo`/`number-demo` pictures — **not** BASE/TO-NUMBER reopen / HERE bump. Marker alone is enough for Lab OK.
- **FAIL:** `[hold] FAIL reason=<…>` optional (demo **must avoid**). Prefer not emitting `[hold] FAIL` on the happy path. No required FAIL reason this tip — missing char / buffer overrun stay out of the demo (or document as optional miss picture that still avoids FAIL).
- Storage: fixed demo char fixture / host int / char echo only. **No** real pictured numeric buffer VM, no full `#S`/`#>`/`SIGN`, no BASE reopen, no TO-NUMBER reopen, no HERE / `_here` / `here-at` redefine / bump, no CHAR-PLUS reopen, no U-LESS, no arena, no full ANS ENVIRONMENT? table reopen.
- **HERE stub base stays `$1000`:** wave14 **1** `$1000 _here !` (or documented stub base) remains the HERE pointer base. This tip’s Lab greps are `[hold] HOLD` and `[hold-demo] OK` only — **never** Lab-grep `_here` / `HERE` / `here-at` / `[allot]` as this tip’s HOLD success. **Do not** redefine / alias / bump those names. Pictured buffer picture must **not** be implemented by bumping HERE.
- **TO-NUMBER stays (thin companion amend only):** do **not** redefine `to-number-mark` / `>NUMBER`. HOLD is a **sibling** thin pictured-numeric **start** beside `>NUMBER` thin parse — not a TO-NUMBER reopen. Thin companion amend of TO-NUMBER.md only. **Do not** Lab-grep `[number] >NUMBER` as this tip’s HOLD success (that stays wave22 **3**).
- **BASE-HEX stays (thin companion amend only):** do **not** redefine `BASE` / `HEX` / `DECIMAL` / `base-mark` / `hex-mark` / `decimal-mark`. HOLD is a sibling beside radix marks — not a BASE reopen. Thin companion amend of BASE-HEX.md only — do not wipe wave21 BASE-HEX content.
- **CHAR-PLUS stays untouched primary:** do **not** redefine `CHAR+` / `char-plus-mark`. **Leave `CHAR-PLUS.md` primary untouched** (tip1 land md5 `bb879e6fe97f8f8ebd25418d43b43751` / 30666).
- Nest with prior charplus / search / number / zero / shift / bit / compare / base / accept / exec / count / within / true / env / allot / cell stubs OK. `dict-reset` unaffected (fixed host fixtures, not dictionary). Assert prior `allot-demo` / `base-demo` / `number-demo` / `charplus-demo` still OK (HERE stub base untouched; BASE radix marks untouched; TO-NUMBER mark untouched; CHAR+ mark untouched).
- Still no full pictured rewrite beyond thin HOLD start this tip (wave24 **3** SHARP-SIGN lands thin `#`/`#>`/`SIGN` continue beside HOLD — **not** HOLD reopen; HERE `$1000` untouched; prefer sharp mirrors; do not redefine hold-mark). No BASE reopen, no TO-NUMBER reopen, no HERE stub base bump, no CHAR-PLUS reopen. Wave23 tip1 + wave22–19 stay landed — keep cites; this tip does not reopen them. Wave24 **3** SHARP-SIGN is sibling thin continue — keep cites; do not reopen HOLD.

## 4. Markers

```
[hold] HOLD [u=<n>] [c=<char|ord>]   # u=/c= optional; classic hold-one-char into pictured buffer picture welcome
[hold] <#                            # optional; welcome when free — do not require
[hold] #                             # optional; welcome when free — do not require; not #S/#>/SIGN
[hold] FAIL reason=<…>               # demo avoids
[hold-demo] OK
[hold-demo] FAIL
```

Lab greps `[hold-demo] OK` plus greppable **`[hold] HOLD`** (optional `u=` / `c=` welcome — classic hold-one-char picture on fixed fixture). Demo avoids `[hold] FAIL`. Prefer not emitting `[hold] FAIL` on the happy path. Optional `[hold] <#` / `[hold] #` welcome when free — **do not require**. **Do not** Lab-grep `_here` / `HERE` / `here-at` / `[allot]` as this tip’s surface (those stay wave14 **1**). **Do not** Lab-grep `[number]` / `[base]` / `[char+]` / `[zero]` / `[shift]` / `[bit]` / `[compare]` / `[accept]` / `[env]` as HOLD success (those stay their own tips). **Do not** Lab-grep `#S` / `#>` / `SIGN` as this tip’s HOLD success (thin continue → wave24 **3** `docs/SHARP-SIGN.md` — sibling; **not** HOLD reopen). **Do not** Lab-grep `U<` / `ABS` / `NEGATE` as this tip (wave23 **3–4** — leave primaries).

## 5. `hold-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; pictured-numeric start mark needs no dict entries.
2. Ensure **fixed demo char fixture** exists (e.g. `'A'` / ord `65` / `'0'` / documented host char — or equivalent) so the classic hold-one-char picture is deterministic. Fixture may live beside (not replacing) BASE-HEX radix picture / TO-NUMBER digit-string picture / ENVIRONMENT-QUERY stub / CELL unit picture / ALLOT-HERE stub pointer / CHAR-PLUS advance picture — document; do **not** require full `#S`/`#>`/`SIGN` / BASE reopen / TO-NUMBER reopen / HERE bump / CHAR-PLUS reopen / U-LESS.
3. Invoke `HOLD` (or **`hold-mark`**) against the classic fixture → `[hold] HOLD` (+ optional `u=` / `c=` — classic hold-one-char / `c=A` / `u=65` welcome).
4. Optional (when free): invoke `<#` / `#` mirrors → `[hold] <#` / `[hold] #` — **do not require**; do **not** invoke `#S` / `#>` / `SIGN`.
5. Assert no `[hold] FAIL` on the happy path. Assert pictured-numeric start did **not** require full `#S`/`#>`/`SIGN` / BASE reopen / TO-NUMBER reopen / HERE bump / `_here` redefine / CHAR-PLUS reopen / U-LESS / full ANS ENVIRONMENT? table reopen (marker-only is enough). Assert host `HOLD` was not redefined when using the Forth mirror. Assert `_here` / `HERE` / `here-at` were **not** redefined / aliased / bumped (HERE stub base stays `$1000`). Assert `to-number-mark` / `base-mark` / `hex-mark` / `decimal-mark` / `char-plus-mark` were **not** redefined. Assert `CHAR-PLUS.md` primary left untouched. Assert prior `allot-demo` / `base-demo` / `number-demo` / `charplus-demo` / `env-demo` still OK.
6. Prior `charplus-demo` / `search-demo` / `number-demo` / `zero-demo` / `shift-demo` / `bit-demo` / `compare-demo` / `base-demo` / `accept-demo` / `exec-demo` / `count-demo` / `within-demo` / `true-demo` / `env-demo` / `char-demo` / `allot-demo` / `cell-demo` + earlier incl `trit-math-demo` / `fold-demo` still OK (HERE stub `$1000`, BASE, TO-NUMBER, CHAR+ intact).
7. `[hold-demo] OK`.

Required marker: `[hold] HOLD`. Optional `u=` / `c=` echo is not required for Lab OK when `[hold] HOLD` is greppable. Optional `<#` / `#` not required. No full `#S`/`#>`/`SIGN`. No BASE reopen. No TO-NUMBER reopen. No HERE stub base bump. No CHAR-PLUS reopen. No U-LESS. No full ANS ENVIRONMENT? table reopen.

## 6. Thin amend — companions

### `docs/TO-NUMBER.md`

- Companions / Status: add `HOLD.md` (wave23 **2** companion cite); **keep** BASE-HEX / KERNEL / ENVIRONMENT-QUERY / CELL-CELLS / ALLOT-HERE / HOST-PARITY cites — do not wipe wave22 TO-NUMBER content.
- Purpose / §3 / non-goals: `>NUMBER` stays thin number-parse mark; `HOLD` is sibling **thin pictured-numeric start** — **not** a TO-NUMBER reopen / full `#S`/`#>`/`SIGN` / BASE reopen / HERE stub base bump. Do not wipe wave22 TO-NUMBER content. Stress: HOLD sibling thin pictured start — **NOT** TO-NUMBER reopen; HERE stub untouched (`$1000 _here !`); prefer `hold-mark` whenever host `HOLD` collides; **do not** redefine `to-number-mark`.
- Non-goals: `HOLD` → `docs/HOLD.md` (wave23 **2**). `>NUMBER` / `to-number-mark` stay on this tip (already landed). Full pictured `#S`/`#>`/`SIGN` still out beyond thin HOLD start. HERE stub base `$1000` stays wave14 **1**. Tip3 U-LESS / tip4 ABS-NEGATE / tip5 DOCS-CITES still later (wave23 **3–5**).
- Acceptance: Lab smokes `hold-demo` (retains `number-demo` + `base-demo` + `charplus-demo` + `allot-demo` + `env-demo`).
- Cite: `docs/HOLD.md`.

### `docs/BASE-HEX.md`

- Companions / Status: add `HOLD.md` (wave23 **2** companion cite); **keep** TO-NUMBER / KERNEL / ENVIRONMENT-QUERY / CELL-CELLS / TRUE-FALSE / HOST-PARITY / ACCEPT-REFILL / ALLOT-HERE cites — do not wipe wave21 BASE-HEX content.
- Purpose / §3 / non-goals: BASE/HEX/DECIMAL stay radix marks; `HOLD` is sibling **thin pictured-numeric start** — **not** a BASE/HEX/DECIMAL reopen / full `#S`/`#>`/`SIGN` / HERE stub base bump / TO-NUMBER reopen. Do not wipe wave21 BASE-HEX content. Stress: HOLD sibling thin pictured start — **NOT** BASE reopen; HERE stub untouched (`$1000 _here !`); prefer `hold-mark`; **do not** redefine `base-mark`/`hex-mark`/`decimal-mark`.
- Non-goals: `HOLD` → `docs/HOLD.md` (wave23 **2**). BASE/HEX/DECIMAL stay on this tip (already landed). Full pictured `#S`/`#>`/`SIGN` still out beyond thin HOLD start. HERE stub base `$1000` stays wave14 **1**. TO-NUMBER stays wave22 **3** (thin companion — not reopen).
- Acceptance: Lab smokes `hold-demo` (retains `base-demo` + `number-demo` + `charplus-demo` + `allot-demo` + `env-demo`).
- Cite: `docs/HOLD.md`.

### `docs/KERNEL.md`

- Companions: add `HOLD.md` (wave23 **2**); **keep** wave23 tip1 CHAR-PLUS cites and wave22 tip1–4 LSHIFT-RSHIFT / ZERO-EQUALS / TO-NUMBER / SEARCH-WORDLIST cites and wave21 tip1–4 ACCEPT-REFILL / BASE-HEX / COMPARE / BITWISE cites and wave20 tip1–4 / wave19 tip1–4 / wave18 tip1–4 / wave17 tip1–4 / wave16 DEFER/MARKER/BUFFER/EXIT / wave15 CELL cites.
- Words table: add `HOLD` / `hold-mark` stub + optional `<#` / `#` + `hold-demo` (cite tip; Forth mirror `hold-mark` — thin pictured-numeric start only; **CRITICAL:** do **not** redefine / alias / bump `_here` / `HERE` / `here-at` — HERE stub base stays `$1000`; prefer `hold-mark` whenever host `HOLD` collides; **do not** redefine to-number-mark / base-mark/hex-mark/decimal-mark / char-plus-mark; **not** full `#S`/`#>`/`SIGN`; **not** BASE reopen; **not** TO-NUMBER reopen; **not** CHAR-PLUS reopen; optional `u=` / `c=` — classic hold-one-char picture welcome; optional `<#` / `#` welcome when free — do not require).
- Non-goals: `HOLD` thin pictured-numeric start → `docs/HOLD.md`. `>NUMBER` stays on `TO-NUMBER.md` (thin companion amend — do not wipe). BASE/HEX/DECIMAL stay on `BASE-HEX.md`. HERE/ALLOT stay on `ALLOT-HERE.md` (HERE stub base `$1000` untouched). `CHAR+` stays on `CHAR-PLUS.md` (leave primary untouched). Tip3 U-LESS / tip4 ABS-NEGATE / tip5 DOCS-CITES still later (wave23 **3–5**).
- Acceptance: Lab smokes `hold-demo` (and retains `charplus-demo` + `search-demo` + `number-demo` + `zero-demo` + `shift-demo` + `bit-demo` + `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `env-demo` + `allot-demo` + prior demos incl `trit-math-demo` / `fold-demo`).
- Cite: `docs/HOLD.md`.

### Optional — `docs/ALLOT-HERE.md`

- Light `HOLD.md` (wave23 **2**) cite; HERE stub `$1000 _here !` **reminder — do not bump**; HOLD must not redefine/alias/bump `_here`/`HERE`/`here-at`; pictured buffer must not bump HERE. Retain `allot-demo`. Cite: `docs/HOLD.md`.

### Optional — `docs/ENVIRONMENT-QUERY.md`

- Light `HOLD.md` (wave23 **2**) cite; optional pictured-numeric stub echo welcome — **not** full ANS ENVIRONMENT? table reopen / BASE reopen / HERE bump. Retain `env-demo`. Cite: `docs/HOLD.md`.

### Optional — `docs/HOST-PARITY.md`

- Light `HOLD.md` (wave23 **2**) cite; `hold-demo CONTRACT` welcome — **not** full Win/Android pictured `#S`/`#>`/`SIGN` / HERE-bump port; prefer `hold-mark`. Cite: `docs/HOLD.md`.

### `docs/SHARP-SIGN.md` (wave24 **3** thin companion cite)

- Companions / Status: HOLD cites SHARP-SIGN as thin pictured-numeric **continue** sibling; SHARP-SIGN cites HOLD as pictured **start** companion (**not** HOLD reopen).
- Purpose: SHARP-SIGN sibling thin pictured continue — **NOT** HOLD reopen; HERE stub untouched (`$1000 _here !`); prefer `sharp-mark`/`sharp-end-mark`/`sign-mark`; **do not** redefine `HOLD`/`hold-mark`/`hold-demo`/`<#`.
- Non-goals: `#`/`#>`/`SIGN` → `docs/SHARP-SIGN.md` (wave24 **3**). HOLD/`hold-mark`/optional `<#` stay on this tip (already landed). Full pictured rewrite beyond thin continue still out.
- Acceptance: Lab smokes `sharp-demo` (retains `hold-demo` + `number-demo` + `base-demo` + `allot-demo`).
- Cite: `docs/SHARP-SIGN.md`.

Do **not** wipe wave23 tip1 / wave22 tip1–5 / wave21 tip1–5 / wave20 tip1–4 / wave19 tip1–4 / wave18–15 prior content. Do **not** amend ARCHITECTURE / IMPLEMENTATION-GAPS / CHAR-PLUS / LSHIFT-RSHIFT / ZERO-EQUALS / SEARCH-WORDLIST / WAVE23-PROPOSAL / WAVE23-COS-PASTE primary this tip (amends are TO-NUMBER + BASE-HEX + KERNEL + optional ALLOT-HERE / ENVIRONMENT-QUERY / HOST-PARITY only). **Leave `CHAR-PLUS.md` untouched** (`bb879e6fe97f8f8ebd25418d43b43751` / 30666). **Leave `LSHIFT-RSHIFT.md` untouched** (`e5a94d8a16d47aa7ceae92b54344712e` / 26299). **Leave `ZERO-EQUALS.md` untouched** (`fd250f768e41fd70dd32c01faa32fc74` / 26165). **Leave `SEARCH-WORDLIST.md` untouched** (`0f9fa84fb70d08c2da7e32f20d1a6e29` / 31054). **Leave `ARCHITECTURE.md` / `IMPLEMENTATION-GAPS.md` untouched** (`42902a5431b3f868b9ff7d73414cccc8` / `465ea6f99db2983a626d42b8731e59c2`). **Leave `WAVE23-PROPOSAL.md` / `WAVE23-COS-PASTE.txt` untouched** (`01fb36f4ea8c4d25e3c3b7ec998e6718` / `3c5e643d45d43a7f46770e2a8563fdf3`). Tip5 cites after 1–4 PASS. Skip 2DUP-FAMILY + ABORT" polish.

## 7. Non-goals

- Redefining / aliasing / bumping `_here` / `HERE` / `here-at` / HERE stub base `$1000 _here !` (wave14 **1** — already stubbed; keep cites; **CRITICAL** — HERE stub base stays `$1000`; pictured buffer must not bump HERE)
- Redefining `to-number-mark` / `>NUMBER` (TO-NUMBER stays — **not** a TO-NUMBER reopen; thin companion amend only)
- Redefining `BASE` / `HEX` / `DECIMAL` / `base-mark` / `hex-mark` / `decimal-mark` (BASE-HEX stays — **not** BASE reopen; thin companion amend only)
- Redefining `CHAR+` / `char-plus-mark` (CHAR-PLUS stays — leave CHAR-PLUS.md primary untouched; **not** a CHAR-PLUS reopen)
- Full pictured numeric rewrite beyond thin continue — thin HOLD start this tip; thin `#`/`#>`/`SIGN` continue → `docs/SHARP-SIGN.md` (wave24 **3**; **not** HOLD reopen; HERE `$1000` untouched; prefer sharp mirrors; do not redefine hold-mark)
- `U<` / `ABS`/`NEGATE` already landed wave23 **3–4** (leave primaries; not reopen)
- Docs cites pass (wave24 **5** — ARCHITECTURE + GAPS after 1–4 PASS; wave23 **5** CLOSED)
- `SEARCH-WORDLIST` / FIND reopen (wave22 **4** / wave18 **2** — already stubbed; leave SEARCH-WORDLIST.md / FIND surfaces)
- `0=` / LSHIFT / BITWISE / TRUE-FALSE / WITHIN / COUNT / EXECUTE / SOURCE / PAD / ACCEPT-REFILL / COMPARE reopen (wave20–22 — already stubbed; keep cites)
- Full ANS `ENVIRONMENT?` table reopen (wave19 **3** — already stubbed; keep cites; optional thin companion cite only)
- Real DOES> XT / real branch XT / LEAVE jump / full arena / linked XT / real STATE cell
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live — **skip 2DUP-FAMILY**)
- `ABORT"` polish (already optional-wired inside `throw-demo` — **skip**)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/HOLD.md` present (Research byte-copy OK); `TO-NUMBER.md` + `BASE-HEX.md` + `KERNEL.md` thin amends present (+ optional `ALLOT-HERE.md` / `ENVIRONMENT-QUERY.md` / `HOST-PARITY.md`); wave23 tip1 + wave22 tip1–5 + wave21 tip1–5 + wave20 tip1–4 + wave19 tip1–4 + wave18–15 prior cites retained; host `HOLD` untouched via mirrors (`hold-mark` preferred); `_here` / `HERE` / `here-at` **not** redefined / aliased / bumped (HERE stub base stays `$1000`); `to-number-mark` / `base-mark`/`hex-mark`/`decimal-mark` / `char-plus-mark` **not** redefined; `CHAR-PLUS.md` primary untouched (`bb879e6fe97f8f8ebd25418d43b43751` / 30666); `LSHIFT-RSHIFT.md` primary untouched (`e5a94d8a16d47aa7ceae92b54344712e` / 26299); `ZERO-EQUALS.md` primary untouched (`fd250f768e41fd70dd32c01faa32fc74` / 26165); `SEARCH-WORDLIST.md` primary untouched (`0f9fa84fb70d08c2da7e32f20d1a6e29` / 31054); `ARCHITECTURE.md` + `IMPLEMENTATION-GAPS.md` unchanged (`42902a5431b3f868b9ff7d73414cccc8` / `465ea6f99db2983a626d42b8731e59c2`); `WAVE23-PROPOSAL.md` / `WAVE23-COS-PASTE.txt` untouched (`01fb36f4ea8c4d25e3c3b7ec998e6718` / `3c5e643d45d43a7f46770e2a8563fdf3`).
2. `hold-demo` → OK (markers §4; `[hold] HOLD` greppable; optional `u=` / `c=` welcome — classic hold-one-char picture on fixed fixture; optional `[hold] <#` / `[hold] #` welcome when free — not required; no FAIL on happy path; no full `#S`/`#>`/`SIGN` / BASE reopen / TO-NUMBER reopen / HERE stub base bump / CHAR-PLUS reopen / U-LESS / full ANS ENVIRONMENT? table reopen; no `_here`/`HERE`/`here-at` redefine; no `to-number-mark`/`base-mark`/`char-plus-mark` redefine). Prior `charplus-demo` + `search-demo` + `number-demo` + `zero-demo` + `shift-demo` + `bit-demo` + `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `env-demo` + `allot-demo` + earlier demos incl `trit-math-demo` / `fold-demo` still OK.
3. Regression green (wave23 tip1 + wave22–14 demos + prior); wave24 **3**: `sharp-demo` → OK retains `hold-demo` — SHARP sibling thin continue, **not** HOLD reopen; HERE `$1000` untouched.
4. Win/Android: CONTRACT acceptable (parity line `hold-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**)
- `docs/TO-NUMBER.md` (wave22 **3** — `>NUMBER` thin number-parse companion; HOLD sibling thin pictured start — **not** a TO-NUMBER reopen; HERE stub base `$1000` untouched; prefer `hold-mark`; do not redefine `to-number-mark`)
- `docs/BASE-HEX.md` (wave21 **2** — BASE/HEX/DECIMAL radix-mark companion; HOLD sibling thin pictured start — **not** BASE reopen; HERE stub base `$1000` untouched; do not redefine `base-mark`/`hex-mark`/`decimal-mark`)
- `docs/ALLOT-HERE.md` (wave14 **1**, optional — HERE/ALLOT pointer stubs; HERE stub base `$1000 _here !` reminder — **do not bump**; pictured buffer must not bump HERE)
- `docs/ENVIRONMENT-QUERY.md` (wave19 **3**, optional — optional pictured-numeric stub cite; **not** a full ANS ENVIRONMENT? table reopen)
- `docs/HOST-PARITY.md` (wave8 **4**, optional — `hold-demo` CONTRACT parity welcome)
- `docs/CHAR-PLUS.md` (wave23 **1** — prior tip; keep cites; leave primary untouched this tip; do not redefine `char-plus-mark`)
- `docs/SEARCH-WORDLIST.md` (wave22 **4** — prior tip; keep cites; leave primary untouched this tip)
- `docs/ZERO-EQUALS.md` (wave22 **2** — prior tip; keep cites; leave primary untouched this tip)
- `docs/LSHIFT-RSHIFT.md` (wave22 **1** — prior tip; keep cites; leave primary untouched this tip)
- `docs/COMPARE.md` / `docs/ACCEPT-REFILL.md` / `docs/BITWISE.md` / wave20 TRUE-FALSE/WITHIN/COUNT/EXECUTE / wave19 SOURCE-PAD/CHAR-CHARS (prior; keep cites)
- `forth/tritium/kernel.fs` (hold-mark only — do not redefine host `HOLD`; **do not** redefine/bump `_here`/HERE/here-at; **do not** redefine to-number-mark / base-mark/hex-mark/decimal-mark / char-plus-mark; optional `<#`/`#` welcome — do not require `#S`/`#>`/`SIGN`)
- ANS Forth `HOLD` (thin pictured-numeric start only — not full `#S`/`#>`/`SIGN`; not BASE reopen; not TO-NUMBER reopen; not HERE stub base bump; host `HOLD` ≠ force bare bind — prefer `hold-mark`)
- Explicit deferral: WAVE19-PROPOSAL + WAVE20-PROPOSAL + WAVE21-PROPOSAL + WAVE22-PROPOSAL + WAVE23-PROPOSAL (`HOLD` — thin pictured-numeric start; not full `#S`/`#>`/`SIGN`; do not break HERE stub base `$1000`; not BASE reopen; not TO-NUMBER reopen)
- Base tip: `998b2fab` / `998b2fab19b4a5d26d8fec2ceee9a27c886ec28a` (#107 wave23 tip1 CHAR-PLUS PASS)
- Wave23 proposal: `/workspace/tritium-research-docs/WAVE23-PROPOSAL.md`


## 10. Shipper implementation notes (Linux SoT)

These notes are normative for Shipper drafting on branch `shipper/hold-w23` from base `998b2fab19b4a5d26d8fec2ceee9a27c886ec28a`. They do **not** authorize merge, push, Mango touch, opaque-weight ML, or reopening closed wave17–22 tips / wave23 tip1 CHAR-PLUS primary.

### 10.1 Binding order

1. Prefer Forth mirror **`hold-mark`** first. Lab greps do not require the bare ANS name `HOLD` to be bound when the mirror prints `[hold] HOLD`.
2. If bare `HOLD` is free on the Linux REPL load path, Shipper **may** bind it as a thin alias that emits the same markers — still **must not** redefine / bump `_here` / `HERE` / `here-at`.
3. Optional `<#` / `#` mirrors (`hold-lt-number` / `hold-hash`) only when free — **do not require**. Never alias them onto `#S` / `#>` / `SIGN`.
4. Never alias `HOLD` onto `to-number-mark` / `base-mark` / `hex-mark` / `decimal-mark` / `char-plus-mark` / `_here` / `HERE` / `here-at`. Pictured start ≠ number-parse / radix / char-advance / dictionary pointer.

### 10.2 Classic picture (hold-one-char)

Document one fixed fixture. Recommended:

| Field | Example | Notes |
|-------|---------|-------|
| char | `'A'` or ord `65` | Classic hold-one-char into pictured buffer picture |
| optional `c=` | `c=A` or `c=65` | Char / ord echo |
| optional `u=` | `u=65` | Ordinal echo |
| optional `<#` | `[hold] <#` | Welcome when free — not required |
| optional `#` | `[hold] #` | Welcome when free — not required; **not** `#S`/`#>`/`SIGN` |

Shipper may use digit char `'0'+n` / PAD pictured slot / documented host char instead — document which. Lab greps `[hold] HOLD` regardless of which fixture is chosen, as long as the marker line is greppable and FAIL is avoided. **Do not** implement the pictured buffer by bumping HERE stub `$1000`.

### 10.3 What Lab greps / does not grep

**Required:** `[hold-demo] OK` + `[hold] HOLD`. **Optional welcome:** `u=`/`c=`; `[hold] <#`/`#` when free; `hold-demo CONTRACT`. **Must NOT Lab-grep as success:** `_here`/`HERE`/`here-at`/`[allot]`; `[number]`/`to-number-mark`; `[base]`*; `[char+]`/`char-plus-mark`; `#S`/`#>`/`SIGN`; `U<`/`ABS`/`NEGATE`; `[search]`/`[zero]`/`[shift]`/`[bit]`/`[compare]`/`[cell]`.

### 10.4 Regression retain list (abbrev)

Retain green: `charplus-demo` / `search-demo` / `number-demo` / `zero-demo` / `shift-demo` / `bit-demo` / `compare-demo` / `base-demo` / `accept-demo` / `exec-demo` / `count-demo` / `within-demo` / `true-demo` / `env-demo` / `char-demo` / `cell-demo` / `allot-demo` / `kernel-demo` / `trit-math-demo` / `fold-demo` (+ earlier suite).

### 10.5 Companion amend checklist

Required thin: `TO-NUMBER.md` + `BASE-HEX.md` + `KERNEL.md` (keep TO-NUMBER / BASE-HEX surfaces + wave23 tip1 CHAR-PLUS cites + wave22 tip1–4 cites). Optional thin: `ALLOT-HERE.md` / `ENVIRONMENT-QUERY.md` / `HOST-PARITY.md`. **Untouched this tip:** ARCHITECTURE / IMPLEMENTATION-GAPS / WAVE23-PROPOSAL / WAVE23-COS-PASTE / CHAR-PLUS primary / LSHIFT-RSHIFT / ZERO-EQUALS / SEARCH-WORDLIST (land md5s locked).

### 10.6 Non-goals restated for Shipper

Do **not**: bump `_here`/HERE/here-at (stub stays `$1000`); redefine `to-number-mark` / `base-mark`/`hex-mark`/`decimal-mark` / `char-plus-mark`; land full `#S`/`#>`/`SIGN`; reopen BASE/TO-NUMBER/CHAR-PLUS; land U</ABS/NEGATE; amend ARCH/GAPS; tip Shipper / push / merge / touch Mango; redefine host 2dup/2drop/2swap; polish ABORT".

### 10.7 Boundaries (summary)

HOLD = thin pictured-numeric **start** only. Sibling to TO-NUMBER (parse) and BASE-HEX (radix); **not** a reopen of either. HERE stub `$1000` untouched — pictured buffer must not bump `_here`/`HERE`/`here-at`. CHAR-PLUS primary untouched. U-LESS / ABS-NEGATE / DOCS-CITES later. Optional `<#`/`#` welcome; `#S`/`#>`/`SIGN` out.

### 10.8 Marker grammar (Shipper emit)

Preferred: `[hold] HOLD c=A u=65` — minimum: `[hold] HOLD` — demo close: `[hold-demo] OK` — avoid FAIL. Optional `[hold] <#` / `[hold] #` when free.

### 10.9 Docs byte-copy reminder

Research drafts under `/workspace/tritium-research-docs/`. Shipper byte-copies into repo `docs/` on branch `shipper/hold-w23` only. Parent tips Shipper after Research handoff. Research does **not** tip Shipper / push / merge / touch Mango.

### 10.10 Still deferred

U< (tip3); ABS/NEGATE (tip4); DOCS-CITES (tip5); full `#S`/`#>`/`SIGN`; BASE/TO-NUMBER/CHAR-PLUS reopen; unicode/XCHAR beyond thin CHAR+; FIND reopen / linked dict; real boolean cell; WITHIN runtime; full ANS ENVIRONMENT? table; real DOES> XT; linked XT / real STATE; real XT execute; real RECURSE; real EVALUATE/INCLUDE; full arena; real branch XT; full Win/Android Forth VM; 2DUP-FAMILY; ABORT" polish; Mango.

### 10.11 Acceptance one-liner

`hold-demo` prints greppable `[hold] HOLD` and ends `[hold-demo] OK`; prior `number-demo`/`base-demo`/`charplus-demo`/`allot-demo` still OK; HERE stub stays `$1000`; to-number-mark/base-mark/char-plus-mark not redefined; no full `#S`/`#>`/`SIGN`; Linux SoT; no merge.

### 10.12 Paste anchors

- Proposal tip2 shape: `/workspace/tritium-research-docs/WAVE23-PROPOSAL.md` § Tip 2 — HOLD (~lines 39–48)
- Base: `998b2fab19b4a5d26d8fec2ceee9a27c886ec28a`
- Branch: `shipper/hold-w23`
- Handoff: `WAVE23-TIP2-HANDOFF.md` (= `TIP2-HANDOFF.md`)

