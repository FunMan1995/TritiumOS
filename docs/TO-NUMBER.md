# TO-NUMBER — `>NUMBER` thin number-parse mark + `number-demo`

**Status:** Shipper-ready stub spec (wave22 item **3**; thin amend wave23 **2** HOLD companion cite; thin amend wave24 **3** SHARP-SIGN companion cite)
**Canonical brief:** ANS-shaped `>NUMBER` (thin number-parse mark only); `docs/BASE-HEX.md` (wave21 **2** — radix-mark companion; **not** BASE reopen; HERE stub base `$1000` untouched); `docs/KERNEL.md` (wave7 **5**); `docs/ENVIRONMENT-QUERY.md` (wave19 **3** — optional parse-related stub cite; **not** full ANS ENVIRONMENT? table reopen); optional `docs/CELL-CELLS.md` (wave15 **1**) / `docs/ALLOT-HERE.md` (wave14 **1** — HERE stub reminder; **do not bump**) / `docs/HOST-PARITY.md` (wave8 **4**); WAVE19–22 deferral closed as **thin number-parse mark only** (not pictured numeric `#`/`HOLD`/`<#`/`#>`/`#S` at land time; wave23 **2** lands thin HOLD start beside this tip — **not** a TO-NUMBER reopen; **not** BASE reopen; **not** SEARCH-WORDLIST; **not** ZERO-EQUALS/LSHIFT reopen; **CRITICAL:** do **not** redefine / alias / bump `_here` / `HERE` / `here-at` — HERE stub base stays `$1000`).
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `number.fs` / `to-number.fs`); Linux host REPL; prefer Forth mirror **`to-number-mark`** whenever host `>NUMBER` collides; **CRITICAL — do not** redefine, alias, or bump `_here` / `HERE` / `here-at` (HERE stub base `$1000 _here !` stays landed); **do not** redefine `BASE`/`HEX`/`DECIMAL`/`base-mark`/`hex-mark`/`decimal-mark` (BASE-HEX stays — **not** BASE reopen); **do not** redefine `0=`/`0<>`/`zero-eq-mark`/`zero-ne-mark` (leave ZERO-EQUALS.md untouched); **do not** redefine `LSHIFT`/`RSHIFT`/`lshift-mark`/`rshift-mark` (leave LSHIFT-RSHIFT.md untouched)
**Companions:** `docs/BASE-HEX.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip), `docs/ENVIRONMENT-QUERY.md` (thin amend this tip); optional light cite `docs/CELL-CELLS.md` / `docs/ALLOT-HERE.md` / `docs/HOST-PARITY.md`; `docs/HOLD.md` (wave23 **2** — thin companion cite; sibling thin pictured-numeric start — **not** a TO-NUMBER reopen; HERE stub untouched; prefer `hold-mark`; do not redefine `to-number-mark`); `docs/SHARP-SIGN.md` (wave24 **3** — thin pictured-numeric **continue** companion cite; **not** TO-NUMBER reopen; HERE `$1000` untouched; prefer `sharp-mark`/`sharp-end-mark`/`sign-mark`; do not redefine `to-number-mark`)
**Base tip SHA:** `9efb055` (wave22 tip2 PASS / #103 ZERO-EQUALS) / full `9efb05513239b4291a68b87e18fc7ef82add9bca`

## 1. Purpose

WAVE14 tip **1** landed `HERE` / `ALLOT` (`docs/ALLOT-HERE.md`) with stub base `$1000 _here !`. WAVE19 tip **3** landed thin `ENVIRONMENT?` (`docs/ENVIRONMENT-QUERY.md` — **not** full ANS env table). WAVE21 tip **2** landed `BASE` / `HEX` / `DECIMAL` radix marks (`docs/BASE-HEX.md`; HERE stub `$1000` untouched). WAVE21 tip **1** / **3** / **4** landed ACCEPT-REFILL / COMPARE / BITWISE. WAVE22 tip **1** landed LSHIFT/RSHIFT (`docs/LSHIFT-RSHIFT.md`; leave primary untouched). WAVE22 tip **2** landed `0=` / optional `0<>` (`docs/ZERO-EQUALS.md`; leave primary untouched). WAVE18–22 deferred `>NUMBER` as a thin number-parse mark beside BASE-HEX (**not** pictured numeric `#`/`HOLD`/`<#`/`#>`/`#S`; **not** BASE reopen; **must not** bump HERE stub `$1000`). This tip lands **stub** thin number-parse mark only: `>NUMBER` (or Forth mirror **`to-number-mark`**) prints `[number] >NUMBER` (+ optional `u=` / `flag=` for a fixed demo digit-string fixture — classic decimal digit consume picture welcome). Smoke via **`number-demo`**. Prefer Forth mirror **`to-number-mark`** whenever host `>NUMBER` collides. **CRITICAL:** kernel HERE stub base is `$1000 _here !` (“stub base”) — this tip must **NOT** redefine, alias, or bump `_here` / `HERE` / `here-at`. Pairs with wave21 BASE-HEX radix marks **without** a BASE reopen, **without** pictured numeric output, and **without** reopening ZERO-EQUALS / LSHIFT / COMPARE / ACCEPT-REFILL / SEARCH-WORDLIST. **Not** tip4 SEARCH-WORDLIST / tip5 DOCS-CITES. Independent of tip4–5. Wave23 tip **2** lands `HOLD` thin pictured-numeric start (`docs/HOLD.md`): sibling **thin pictured start** beside `>NUMBER` thin parse — **not** a TO-NUMBER reopen / full `#S`/`#>`/`SIGN` / BASE reopen / HERE stub base bump; prefer `hold-mark`; **do not** redefine `to-number-mark`; HERE stub `$1000` stays untouched. Wave24 tip **3** lands `#`/`#>`/`SIGN` thin pictured-numeric **continue** (`docs/SHARP-SIGN.md`): sibling **thin pictured continue** beside `>NUMBER` — **not** a TO-NUMBER reopen / HOLD reopen / HERE bump; prefer `sharp-mark`/`sharp-end-mark`/`sign-mark`; **do not** redefine `to-number-mark`; HERE stub `$1000` stays untouched.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `>NUMBER` / `to-number-mark` | `( ud1 c-addr1 u1 -- ud2 c-addr2 u2 )` *or* `( -- )` with fixed demo fixture | Thin number-parse mark; print `[number] >NUMBER` (+ optional `u=<n>` / `flag=<n>`); classic decimal digit-string consume picture welcome (e.g. `"42"` → pictured `u=42` / `flag=1` / remaining `u=0`) |
| `number-demo` | `( -- )` | See §5 |

Host note: bind bare `>NUMBER` on the Linux REPL **only if** that name does not collide with host Forth `>NUMBER`. Prefer Forth mirror **`to-number-mark`** as the Lab-facing surface when in doubt — **do not** redefine host `>NUMBER`. **CRITICAL:** do **not** redefine, alias, or bump `_here` / `HERE` / `here-at` (HERE stub base `$1000 _here !` stays landed — this tip is a **thin number-parse mark**, not a HERE rewrite). **Do not** redefine `BASE`/`HEX`/`DECIMAL`/`base-mark`/`hex-mark`/`decimal-mark` — BASE-HEX stays mark-only sibling (**not** BASE reopen). Optional `u=` / `flag=` are host ints / fixture echo only — not pictured numeric, not a HERE bump, not BASE reopen. Prefer **fixed demo digit-string fixtures** (classic decimal `"42"` / `"0"` — document) so Lab hit is deterministic and FAIL is avoided.

## 3. Stub semantics

- **`>NUMBER` / `to-number-mark`:** take (or use fixed demo) a pictured digit-string fixture — classic ANS sketch `( ud1 c-addr1 u1 -- ud2 c-addr2 u2 )` where digits in the current BASE are consumed into an unsigned double accumulator. This tip is **marker only**: print `[number] >NUMBER` and optionally `u=<n>` and/or `flag=<n>` (Lab-greppable; prefer documenting which Shipper emits). Classic fixture welcome: digit-string `"42"` (decimal) → pictured consume complete (`u=42` or `flag=1` / remaining length `0`); optional partial-consume picture welcome when free — **do not require** partial / miss path for Lab OK. **Does not** rewrite the real number parser, implement pictured numeric `#`/`HOLD`/`<#`/`#>`/`#S`, reopen BASE/HEX/DECIMAL, allocate, or bump HERE. **Does not** redefine / alias / store into `_here` / `HERE` / `here-at`. Captured values are host ints / fixture echo only.
- **Fixed demo digit-string fixtures (document):**
  - **Classic decimal digit consume picture (required set):** e.g. fixture string `"42"` (or `"0"` / `"10"` — document) under pictured decimal radix beside BASE-HEX. Demo must exercise **`>NUMBER` / `to-number-mark`** so Lab greps `[number] >NUMBER`. Optional `u=` / `flag=` welcome (classic `u=42` / `flag=1` / remaining `u=0` picture).
  - **Optional result echo:** `u=<consumed-value>` and/or `flag=<1|0>` and/or remaining length — document which form Shipper emits. Greppable `[number] >NUMBER` alone is enough for Lab OK when the marker line appears.
  - **Optional BASE pairing:** fixture may sit beside (not replacing) `base-demo` radix picture — document; do **not** require a BASE reopen / live BASE variable rewrite / HERE bump.
- **Optional push:** if the host stack is easy, push pictured `ud2` / `c-addr2` / `u2`; marker alone is enough for Lab OK — do not require a real number parser / pictured numeric / BASE reopen / HERE bump.
- **FAIL:** `[number] FAIL reason=<…>` optional (demo **must avoid**). Prefer not emitting `[number] FAIL` on the happy path. No required FAIL reason this tip — unknown digit / BASE miss / empty string stay out of the demo (or document as optional miss picture that still avoids FAIL).
- Storage: fixed demo digit-string fixture / host int echo only. **No** real number parser rewrite, no pictured numeric `#`/`HOLD`/`<#`/`#>`/`#S`, no BASE reopen, no HERE / `_here` / `here-at` redefine / bump, no SEARCH-WORDLIST, no ZERO-EQUALS / LSHIFT reopen, no arena, no full ANS ENVIRONMENT? table reopen.
- **HERE stub base stays `$1000`:** wave14 **1** `$1000 _here !` (or documented stub base) remains the HERE pointer base. This tip’s Lab greps are `[number] >NUMBER` and `[number-demo] OK` only — **never** Lab-grep `_here` / `HERE` / `here-at` / `[allot]` as this tip’s number-parse success. **Do not** redefine / alias / bump those names.
- **BASE-HEX stays:** do **not** redefine `BASE` / `HEX` / `DECIMAL` / `base-mark` / `hex-mark` / `decimal-mark`. Number-parse mark is a **sibling** beside radix marks — not a BASE reopen. Thin companion amend of BASE-HEX.md only — do not wipe wave21 BASE-HEX content.
- **ZERO-EQUALS / LSHIFT stay untouched primaries:** do **not** redefine `0=` / `0<>` / `zero-eq-mark` / `zero-ne-mark`. Do **not** redefine `LSHIFT` / `RSHIFT` / `lshift-mark` / `rshift-mark`. **Leave `ZERO-EQUALS.md` primary untouched** (tip2 land md5 `fd250f768e41fd70dd32c01faa32fc74` / 26165). **Leave `LSHIFT-RSHIFT.md` primary untouched** (tip1 land md5 `e5a94d8a16d47aa7ceae92b54344712e` / 26299).
- Nest with prior zero / shift / bit / compare / base / accept / exec / count / within / true / env / allot / cell stubs OK. `dict-reset` unaffected (fixed host fixtures, not dictionary). Assert prior `allot-demo` / `base-demo` still OK (HERE stub base untouched; BASE radix marks untouched).
- Still no full pictured rewrite this tip (thin HOLD → wave23 **2**; thin `#`/`#>`/`SIGN` continue → wave24 **3** `docs/SHARP-SIGN.md` — sibling; **not** TO-NUMBER reopen). No BASE reopen, no HERE stub base bump. Wave22 tip1–2 + wave21–19 stay landed — keep cites. Wave23 **2** HOLD + wave24 **3** SHARP are siblings — keep cites; do not reopen `>NUMBER`.

## 4. Markers

```
[number] >NUMBER [u=<n>] [flag=<n>]   # u=/flag= optional; classic decimal digit-string consume picture welcome
[number] FAIL reason=<…>              # demo avoids
[number-demo] OK
[number-demo] FAIL
```

Lab greps `[number-demo] OK` plus greppable **`[number] >NUMBER`** (optional `u=` / `flag=` welcome — classic decimal digit consume picture on fixed fixture). Demo avoids `[number] FAIL`. Prefer not emitting `[number] FAIL` on the happy path. **Do not** Lab-grep `_here` / `HERE` / `here-at` / `[allot]` as this tip’s surface (those stay wave14 **1**). **Do not** Lab-grep `[base]` / `[zero]` / `[shift]` / `[bit]` / `[compare]` / `[accept]` / `[env]` as TO-NUMBER success (those stay their own tips). **Do not** Lab-grep pictured numeric `#` / `HOLD` / `<#` / `#>` / `#S` / `SIGN` as this tip (thin HOLD → wave23 **2**; thin continue → wave24 **3** `docs/SHARP-SIGN.md` — siblings; **not** TO-NUMBER reopen).

## 5. `number-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; number-parse mark needs no dict entries.
2. Ensure **fixed demo digit-string fixture** exists (e..g. `"42"` / `"0"` / `"10"` — or equivalent host counted-string / c-addr+u fixture) so the classic decimal digit consume picture is deterministic. Fixture may live beside (not replacing) BASE-HEX radix picture / ENVIRONMENT-QUERY stub / CELL unit picture / ALLOT-HERE stub pointer — document; do **not** require pictured numeric / BASE reopen / HERE bump / ZERO-EQUALS reopen / LSHIFT reopen / COMPARE reopen / SEARCH-WORDLIST.
3. Invoke `>NUMBER` (or **`to-number-mark`**) against the classic fixture → `[number] >NUMBER` (+ optional `u=` / `flag=` — classic decimal consume / `u=42` / `flag=1` welcome).
4. Assert no `[number] FAIL` on the happy path. Assert number-parse mark did **not** require pictured numeric `#`/`HOLD`/`<#`/`#>`/`#S` / BASE reopen / HERE bump / `_here` redefine / ZERO-EQUALS reopen / LSHIFT reopen / SEARCH-WORDLIST / full ANS ENVIRONMENT? table reopen (marker-only is enough). Assert host `>NUMBER` was not redefined when using the Forth mirror. Assert `_here` / `HERE` / `here-at` were **not** redefined / aliased / bumped (HERE stub base stays `$1000`). Assert `base-mark` / `hex-mark` / `decimal-mark` / `zero-eq-mark` / `lshift-mark` / `rshift-mark` were **not** redefined. Assert `ZERO-EQUALS.md` + `LSHIFT-RSHIFT.md` primaries left untouched. Assert prior `allot-demo` / `base-demo` / `env-demo` still OK.
5. Prior `zero-demo` / `shift-demo` / `bit-demo` / `compare-demo` / `base-demo` / `accept-demo` / `exec-demo` / `count-demo` / `within-demo` / `true-demo` / `source-demo` / `env-demo` / `body-demo` / `char-demo` / `state-demo` / `word-demo` / `find-demo` / `tick-demo` / `allot-demo` + earlier demos incl `trit-math-demo` / `fold-demo` still OK (HERE stub base `$1000` and BASE radix marks must remain intact).
6. `[number-demo] OK`.

Required marker: `[number] >NUMBER`. Optional `u=` / `flag=` echo is not required for Lab OK when `[number] >NUMBER` is greppable. No pictured numeric. No BASE reopen. No HERE stub base bump. No ZERO-EQUALS / LSHIFT reopen. No SEARCH-WORDLIST. No full ANS ENVIRONMENT? table reopen.

## 6. Thin amend — companions

### `docs/BASE-HEX.md`

- Companions / Status: add `TO-NUMBER.md` (wave22 **3** companion cite); **keep** KERNEL / ENVIRONMENT-QUERY / CELL-CELLS / TRUE-FALSE / HOST-PARITY / ACCEPT-REFILL / ALLOT-HERE cites — do not wipe wave21 BASE-HEX content.
- Purpose / §3 / non-goals: BASE/HEX/DECIMAL stay radix marks; `>NUMBER` is sibling **thin number-parse mark** over fixed demo digit-string fixture — **not** a BASE/HEX/DECIMAL reopen / pictured numeric `#`/`HOLD`/`<#`/`#>`/`#S` / HERE stub base bump. Do not wipe wave21 BASE-HEX content. Stress: TO-NUMBER sibling thin parse — **NOT** BASE reopen; HERE stub untouched (`$1000 _here !`); prefer `to-number-mark` whenever host `>NUMBER` collides.
- Non-goals: `>NUMBER` → `docs/TO-NUMBER.md` (wave22 **3**). BASE/HEX/DECIMAL stay on this tip (already landed). Pictured numeric `#`/`HOLD`/`<#`/`#>`/`#S` still out. HERE stub base `$1000` stays wave14 **1**. Tip4 SEARCH-WORDLIST / tip5 DOCS-CITES still later (wave22 **4–5**).
- Acceptance: Lab smokes `number-demo` (retains `base-demo` + `zero-demo` + `shift-demo` + `bit-demo` + `compare-demo` + `accept-demo` + `allot-demo` + `env-demo`).
- Cite: `docs/TO-NUMBER.md`.

### `docs/KERNEL.md`

- Companions: add `TO-NUMBER.md` (wave22 **3**); **keep** wave22 tip1–2 LSHIFT-RSHIFT / ZERO-EQUALS cites and wave21 tip1–4 ACCEPT-REFILL / BASE-HEX / COMPARE / BITWISE cites and wave20 tip1–4 / wave19 tip1–4 / wave18 tip1–4 / wave17 tip1–4 / wave16 DEFER/MARKER/BUFFER/EXIT / wave15 CELL cites.
- Words table: add `>NUMBER` / `to-number-mark` stub + `number-demo` (cite tip; Forth mirror `to-number-mark` — thin number-parse mark only; **CRITICAL:** do **not** redefine / alias / bump `_here` / `HERE` / `here-at` — HERE stub base stays `$1000`; prefer `to-number-mark` whenever host `>NUMBER` collides; **do not** redefine base-mark/hex-mark/decimal-mark / zero-eq-mark / lshift-mark/rshift-mark; **not** pictured numeric `#`/`HOLD`/`<#`/`#>`/`#S`; **not** BASE reopen; optional `u=` / `flag=` — classic decimal digit-string consume picture welcome).
- Non-goals: `>NUMBER` thin number-parse mark → `docs/TO-NUMBER.md`. BASE/HEX/DECIMAL stay on `BASE-HEX.md`. HERE/ALLOT stay on `ALLOT-HERE.md` (HERE stub base `$1000` untouched). `0=` stays on `ZERO-EQUALS.md` (leave primary untouched). LSHIFT/RSHIFT stay on `LSHIFT-RSHIFT.md` (leave primary untouched). COMPARE stays on `COMPARE.md`. ACCEPT/REFILL stay on `ACCEPT-REFILL.md`. Tip4 SEARCH-WORDLIST / tip5 DOCS-CITES still later (wave22 **4–5**).
- Acceptance: Lab smokes `number-demo` (and retains `zero-demo` + `shift-demo` + `bit-demo` + `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `env-demo` + `allot-demo` + prior demos incl `trit-math-demo` / `fold-demo`).
- Cite: `docs/TO-NUMBER.md`.

### `docs/ENVIRONMENT-QUERY.md`

- Companions / Status: add `TO-NUMBER.md` (wave22 **3** companion cite); **keep** BASE-HEX / KERNEL / CELL-CELLS / FIND / WORDS-VOCAB / HOST-PARITY / CHAR-CHARS / TO-BODY cites — do not wipe wave19 ENVIRONMENT-QUERY content.
- Purpose / §3 / non-goals: ENVIRONMENT? stays thin query mark against fixed demo query set; `>NUMBER` is sibling **thin number-parse mark** — optional env query string may echo a parse-related stub — **not** a full ANS ENVIRONMENT? table reopen / SEARCH-WORDLIST / wordlist rewrite / BASE reopen / pictured numeric / HERE stub base bump. Do not wipe wave19 ENVIRONMENT-QUERY content. Stress: optional parse-related stub cite — **NOT** full env table reopen.
- Non-goals: `>NUMBER` → `docs/TO-NUMBER.md` (wave22 **3**). Full ANS ENVIRONMENT? table still out. ENVIRONMENT? stays on this tip (already landed). BASE/HEX/DECIMAL stay wave21 **2**.
- Acceptance: Lab smokes `number-demo` (retains `env-demo` + `base-demo` + `zero-demo` + `cell-demo`).
- Cite: `docs/TO-NUMBER.md`.

### Optional — `docs/CELL-CELLS.md`

- Companions / Status: add light `TO-NUMBER.md` (wave22 **3**) cite; **keep** ZERO-EQUALS / LSHIFT-RSHIFT / BITWISE / TRUE-FALSE / WITHIN / BASE-HEX / CHAR-CHARS / ENVIRONMENT-QUERY / ALLOT-HERE / KERNEL cites — do not wipe wave15 CELL content.
- Purpose / §3 / non-goals: cell-unit stubs stay; `>NUMBER` may echo optional stub `u=` / `flag=` that picture **cell-sized** parse results on stub fixtures — **not** a CELL/ALIGN reopen / cell-size rewrite / HERE stub base bump / BASE reopen / pictured numeric. Do not wipe wave15 / wave19 / wave20 / wave21 / wave22 CELL/CHAR/ENV/TRUE/BASE/BITWISE/LSHIFT/ZERO content. Stress: number-parse mark on cell-sized ints — not CELL reopen; HERE stub base `$1000` stays untouched.
- Non-goals: `>NUMBER` → `docs/TO-NUMBER.md` (wave22 **3**). CELL/CELLS/ALIGN/ALIGNED stay on this tip (already landed). BASE-HEX stays wave21 **2**. ZERO-EQUALS stays wave22 **2**.
- Acceptance: Lab smokes `number-demo` (retains `cell-demo` + `base-demo` + `zero-demo` + `shift-demo` + `bit-demo` + `allot-demo`).
- Cite: `docs/TO-NUMBER.md`.

### Optional — `docs/ALLOT-HERE.md`

- Companions / Status: add light `TO-NUMBER.md` (wave22 **3**) cite; **keep** CELL-CELLS / KERNEL / VARIABLE-CONST / CREATE-DOES / TO-BODY / MARKER / BUFFER-COLON / FILL-MOVE cites — do not wipe wave14 ALLOT-HERE content.
- Purpose / §3 / non-goals: HERE/ALLOT stay pointer stubs; HERE stub base `$1000 _here !` **reminder — do not bump**; `>NUMBER` is sibling **thin number-parse mark** — **NOT** a HERE/ALLOT reopen / arena / pointer bump / BASE reopen. Do not wipe wave14 ALLOT-HERE content. Stress: HERE stub reminder — do not bump; TO-NUMBER must not redefine / alias / bump `_here` / `HERE` / `here-at`.
- Non-goals: `>NUMBER` → `docs/TO-NUMBER.md` (wave22 **3**). HERE/ALLOT stay on this tip (already landed). BASE-HEX stays wave21 **2** (numeric radix — not dictionary-pointer base).
- Acceptance: Lab smokes `number-demo` (retains `allot-demo` + `base-demo` + `cell-demo`; HERE stub base `$1000` untouched).
- Cite: `docs/TO-NUMBER.md`.

### Optional — `docs/HOST-PARITY.md`

- Companions: add light `TO-NUMBER.md` (wave22 **3**) cite; **keep** ZERO-EQUALS / LSHIFT-RSHIFT / BITWISE / TRUE-FALSE / BASE-HEX / ENVIRONMENT-QUERY / KERNEL / BUILD / INSTALL / INTERPRET cites.
- Purpose / non-goals: Win/Android stay CONTRACT-parity; `number-demo` CONTRACT line is acceptable — **not** a full Forth VM / number-parser / pictured-numeric / HERE-bump port. Do not wipe wave8 HOST-PARITY content. Stress: HERE stub base `$1000` stays untouched on Linux SoT; prefer `to-number-mark`.
- Non-goals: `>NUMBER` → `docs/TO-NUMBER.md` (wave22 **3**). Full Win/Android Forth VM still out. BASE-HEX stays wave21 **2**. ZERO-EQUALS stays wave22 **2**.
- Acceptance: Lab smokes `number-demo` (Win/Android: `number-demo CONTRACT` OK).
- Cite: `docs/TO-NUMBER.md`.


### `docs/HOLD.md` (wave23 **2** thin companion cite)

- Companions / Status: TO-NUMBER cites HOLD as thin pictured-numeric start sibling; HOLD cites TO-NUMBER as number-parse companion (**not** TO-NUMBER reopen).
- Purpose: HOLD sibling thin pictured start — **NOT** TO-NUMBER reopen; HERE stub untouched (`$1000 _here !`); prefer `hold-mark`; **do not** redefine `to-number-mark`.
- Non-goals: `HOLD` → `docs/HOLD.md` (wave23 **2**). `>NUMBER`/`to-number-mark` stay landed. Thin `#`/`#>`/`SIGN` continue → `docs/SHARP-SIGN.md` (wave24 **3**; **not** TO-NUMBER reopen). Full pictured rewrite beyond thin continue still out.
- Acceptance: Lab smokes `hold-demo` (retains `number-demo` + `base-demo` + `charplus-demo` + `allot-demo`).
- Cite: `docs/HOLD.md`.

### `docs/SHARP-SIGN.md` (wave24 **3** thin companion cite)

- Companions / Status: TO-NUMBER cites SHARP-SIGN as thin pictured-numeric **continue** sibling (**not** TO-NUMBER reopen).
- Purpose: SHARP-SIGN sibling thin pictured continue — **NOT** TO-NUMBER reopen; HERE stub untouched (`$1000 _here !`); prefer `sharp-mark`/`sharp-end-mark`/`sign-mark`; **do not** redefine `to-number-mark`.
- Non-goals: `#`/`#>`/`SIGN` → `docs/SHARP-SIGN.md` (wave24 **3**). `>NUMBER` stay landed. HOLD start stays wave23 **2**.
- Acceptance: Lab smokes `sharp-demo` (retains `number-demo` + `hold-demo` + `base-demo`).
- Cite: `docs/SHARP-SIGN.md`.


Do **not** wipe wave22 tip1–2 / wave21 tip1–5 / wave20 tip1–4 / wave19 tip1–4 / wave18–15 prior content. Do **not** amend ARCHITECTURE / IMPLEMENTATION-GAPS / ZERO-EQUALS / LSHIFT-RSHIFT / ACCEPT-REFILL / COMPARE primary this tip (amends are BASE-HEX + KERNEL + ENVIRONMENT-QUERY + optional CELL-CELLS / ALLOT-HERE / HOST-PARITY only). **Leave `ZERO-EQUALS.md` untouched** (`fd250f768e41fd70dd32c01faa32fc74` / 26165). **Leave `LSHIFT-RSHIFT.md` untouched** (`e5a94d8a16d47aa7ceae92b54344712e` / 26299). **Leave `ACCEPT-REFILL.md` / `COMPARE.md` untouched.** **Leave `ARCHITECTURE.md` / `IMPLEMENTATION-GAPS.md` untouched** (`b924e9ce5c1e14efd8e0ae738dde5f51` / `712d77305646b9275cb4e76195f00f38`). **Leave `WAVE22-PROPOSAL.md` / `WAVE22-COS-PASTE.txt` untouched.** Tip5 cites after 1–4 PASS. Skip 2DUP-FAMILY + ABORT" polish.

## 7. Non-goals

- Redefining / aliasing / bumping `_here` / `HERE` / `here-at` / HERE stub base `$1000 _here !` (wave14 **1** — already stubbed; keep cites; **CRITICAL** — HERE stub base stays `$1000`)
- Redefining `BASE` / `HEX` / `DECIMAL` / `base-mark` / `hex-mark` / `decimal-mark` (BASE-HEX stays — **not** BASE reopen; thin companion amend only)
- Full pictured numeric output beyond thin continue — thin HOLD start → `docs/HOLD.md` (wave23 **2**); thin `#`/`#>`/`SIGN` continue → `docs/SHARP-SIGN.md` (wave24 **3**; sibling thin pictured continue — **not** a TO-NUMBER reopen; HERE stub untouched; prefer sharp mirrors; do not redefine `to-number-mark`)
- Real number parser rewrite / tokenizer number path beyond thin mark
- `SEARCH-WORDLIST` / FIND reopen (tip4 — still deferred)
- Docs cites pass (wave22 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `0=` / `0<>` / `zero-eq-mark` / `zero-ne-mark` reopen (wave22 **2** — already stubbed; leave ZERO-EQUALS.md primary untouched)
- `LSHIFT` / `RSHIFT` / `lshift-mark` / `rshift-mark` reopen (wave22 **1** — already stubbed; leave LSHIFT-RSHIFT.md primary untouched)
- `COMPARE` / `ACCEPT-REFILL` reopen (wave21 **1** / **3** — already stubbed; leave those primaries untouched)
- `BITWISE` reopen (wave21 **4** — already stubbed; keep cites)
- Full ANS `ENVIRONMENT?` table reopen (wave19 **3** — already stubbed; keep cites; thin companion amend only — optional parse-related stub cite welcome, not table reopen)
- `TRUE` / `FALSE` / `WITHIN` / `COUNT` / `EXECUTE` / `SOURCE` / `PAD` reopen (wave20 / wave19 — already stubbed; keep cites)
- `CELL` / `CELLS` / `ALIGN` / `ALIGNED` reopen (wave15 — already stubbed; number-parse mark on cell-sized fixtures — not CELL reopen)
- Real DOES> XT / real branch XT / LEAVE jump / full arena / linked XT / real STATE cell
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live — **skip 2DUP-FAMILY**)
- `ABORT"` polish (already optional-wired inside `throw-demo` — **skip**)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/TO-NUMBER.md` present (Research byte-copy OK); `BASE-HEX.md` + `KERNEL.md` + `ENVIRONMENT-QUERY.md` thin amends present (+ optional `CELL-CELLS.md` / `ALLOT-HERE.md` / `HOST-PARITY.md`); wave22 tip1–2 + wave21 tip1–5 + wave20 tip1–4 + wave19 tip1–4 + wave18–15 prior cites retained; host `>NUMBER` untouched via mirrors (`to-number-mark` preferred); `_here` / `HERE` / `here-at` **not** redefined / aliased / bumped (HERE stub base stays `$1000`); `base-mark`/`hex-mark`/`decimal-mark` / `zero-eq-mark` / `lshift-mark`/`rshift-mark` **not** redefined; `ZERO-EQUALS.md` primary untouched (`fd250f768e41fd70dd32c01faa32fc74` / 26165); `LSHIFT-RSHIFT.md` primary untouched (`e5a94d8a16d47aa7ceae92b54344712e` / 26299); `ACCEPT-REFILL.md` + `COMPARE.md` primaries untouched; `ARCHITECTURE.md` + `IMPLEMENTATION-GAPS.md` unchanged (`b924e9ce5c1e14efd8e0ae738dde5f51` / `712d77305646b9275cb4e76195f00f38`); `WAVE22-PROPOSAL.md` / `WAVE22-COS-PASTE.txt` untouched.
2. `number-demo` → OK (markers §4; `[number] >NUMBER` greppable; optional `u=` / `flag=` welcome — classic decimal digit-string consume picture on fixed fixture; no FAIL on happy path; no full pictured rewrite this tip (thin HOLD → wave23 **2**; thin continue → wave24 **3** SHARP-SIGN sibling — not TO-NUMBER reopen) / BASE reopen / HERE stub base bump / ZERO-EQUALS reopen / LSHIFT reopen / COMPARE reopen / SEARCH-WORDLIST / full ANS ENVIRONMENT? table reopen; no `_here`/`HERE`/`here-at` redefine). Prior `zero-demo` + `shift-demo` + `bit-demo` + `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `env-demo` + `allot-demo` + earlier demos incl `trit-math-demo` / `fold-demo` still OK.
3. Regression green (wave22 tip1–2 + wave21 tip1–5 + wave20 tip1–5 + wave19 tip1–5 + wave18 tip1–5 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `number-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**)
- `docs/BASE-HEX.md` (wave21 **2** — BASE/HEX/DECIMAL radix-mark companion; TO-NUMBER sibling thin parse — **not** BASE reopen; HERE stub base `$1000` untouched)
- `docs/ENVIRONMENT-QUERY.md` (wave19 **3** — optional parse-related stub cite; **not** a full ANS ENVIRONMENT? table reopen)
- `docs/ALLOT-HERE.md` (wave14 **1**, optional — HERE/ALLOT pointer stubs; HERE stub base `$1000 _here !` reminder — **do not bump**)
- `docs/CELL-CELLS.md` (wave15 **1**, optional — cell/unit picture companion; number-parse mark on cell-sized fixtures — not CELL reopen / HERE stub base bump)
- `docs/HOST-PARITY.md` (wave8 **4**, optional — `number-demo` CONTRACT parity welcome)
- `docs/ZERO-EQUALS.md` (wave22 **2** — prior tip; keep cites; leave primary untouched this tip; host `0=`/`0<>` stay untouched)
- `docs/LSHIFT-RSHIFT.md` (wave22 **1** — prior tip; keep cites; leave primary untouched this tip; host lowercase `lshift`/`rshift` stay untouched)
- `docs/COMPARE.md` (wave21 **3** — prior tip; keep cites; leave primary untouched this tip; host `cstr=` ≠ ANS COMPARE)
- `docs/ACCEPT-REFILL.md` (wave21 **1** — prior tip; keep cites; leave primary untouched this tip)
- `docs/BITWISE.md` (wave21 **4** — prior tip; keep cites)
- `docs/TRUE-FALSE.md` / `docs/WITHIN.md` / `docs/COUNT.md` / `docs/EXECUTE.md` (wave20 **1–4** — prior tips; keep cites)
- `docs/SOURCE-PAD.md` (wave19 **4** — prior tip; keep cites)
- `forth/tritium/kernel.fs` (to-number-mark only — do not redefine host `>NUMBER`; **do not** redefine/bump `_here`/HERE/here-at; **do not** redefine base-mark/hex-mark/decimal-mark; **do not** redefine zero-eq-mark / lshift-mark/rshift-mark)
- ANS Forth `>NUMBER` (thin number-parse mark only — not pictured numeric `#`/`HOLD`/`<#`/`#>`/`#S`; not BASE reopen; not HERE stub base bump; host `>NUMBER` ≠ force bare bind — prefer `to-number-mark`)
- Explicit deferral: WAVE19-PROPOSAL + WAVE20-PROPOSAL + WAVE21-PROPOSAL + WAVE22-PROPOSAL (`>NUMBER` — thin number-parse mark; not pictured numeric; do not break HERE stub base `$1000`; not BASE reopen)
- Base tip: `9efb055` / `9efb05513239b4291a68b87e18fc7ef82add9bca` (#103 wave22 tip2 ZERO-EQUALS PASS)
- `docs/HOLD.md` (wave23 **2** — sibling thin pictured-numeric start; **not** a TO-NUMBER reopen; HERE stub untouched; prefer `hold-mark`; do not redefine `to-number-mark`)
- `docs/SHARP-SIGN.md` (wave24 **3** — sibling thin pictured-numeric **continue**; **not** a TO-NUMBER reopen; HERE stub untouched; prefer sharp mirrors; do not redefine `to-number-mark`)
- Wave22 proposal: `/workspace/tritium-research-docs/WAVE22-PROPOSAL.md`
