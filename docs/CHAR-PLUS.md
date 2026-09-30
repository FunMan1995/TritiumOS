# CHAR-PLUS — `CHAR+` thin char-unit advance mark + `charplus-demo`

**Status:** Shipper-ready stub spec (wave23 item **1**)
**Canonical brief:** ANS-shaped `CHAR+` (thin char-unit **advance** mark only); `docs/CHAR-CHARS.md` (wave19 **1** — CHAR/CHARS/[CHAR] char-unit companion; **not** a CHAR-CHARS reopen; **do not** redefine `CHAR` / `CHARS` / `[CHAR]` / `char-unit` / `chars-n` / `bracket-char`); `docs/KERNEL.md` (wave7 **5**); optional `docs/CELL-CELLS.md` (wave15 **1** — cell/unit picture companion; **not** ALIGN/CELL reopen) / `docs/HOST-PARITY.md` (wave8 **4**); WAVE19–23 deferral closed as **thin char-unit advance mark only** (not unicode / XCHAR; **not** CHAR-CHARS reopen; **not** COMPARE reopen; **not** ALIGN reopen; **not** HOLD / pictured numeric — tip2; **CRITICAL:** char-unit marks only — do **not** redefine host/wave19 CHAR surfaces).
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `char-plus.fs` / `charplus.fs`); Linux host REPL; prefer Forth mirror **`char-plus-mark`** whenever host `CHAR+` collides; **CRITICAL — do not** redefine `CHAR` / `CHARS` / `[CHAR]` / `char-unit` / `chars-n` / `bracket-char` (CHAR-CHARS stays landed — **not** a CHAR-CHARS reopen); **do not** redefine `CELL` / `CELLS` / `ALIGN` / `ALIGNED` / `cell-size` / `cells-n` / `align-here` / `aligned-addr` (CELL-CELLS stays — **not** ALIGN reopen); **do not** redefine `compare-mark` / `search-wl-mark` / `to-number-mark` / `zero-eq-mark` / `lshift-mark` / `rshift-mark`
**Companions:** `docs/CHAR-CHARS.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip); optional light cite `docs/CELL-CELLS.md` / `docs/HOST-PARITY.md`
**Base tip SHA:** `f6eb303` (wave22 tip5 CLOSED / #106 DOCS-CITES) / full `f6eb30348b9f1395030b1c6f075b99d9327b0624`

## 1. Purpose

WAVE15 tip **1** landed `CELL` / `CELLS` / `ALIGN` / `ALIGNED` (`docs/CELL-CELLS.md`). WAVE19 tip **1** landed `CHAR` / `CHARS` / `[CHAR]` char-unit marks (`docs/CHAR-CHARS.md`; Forth mirrors `char-unit` / `chars-n` / `bracket-char`; **not** CHAR+ / unicode / XCHAR). WAVE18–22 deferred `CHAR+` as a thin char-unit **advance** beside CHAR-CHARS (**not** unicode / XCHAR; **not** a CHAR-CHARS reopen; **must not** redefine CHAR / CHARS / [CHAR] / char-unit / chars-n / bracket-char). WAVE21 tip **3** COMPARE and wave22 tip **4** SEARCH-WORDLIST kept CHAR+ deferred. WAVE22 tip **5** closed DOCS-CITES on tip SHA `f6eb303` (this base). This tip lands **stub** thin char-unit advance mark only: `CHAR+` (or Forth mirror **`char-plus-mark`**) prints `[char+] CHAR+` (+ optional `u=` / `addr=` for a classic char-unit advance picture — addr + 1 CHAR on Linux SoT char-size=1). Smoke via **`charplus-demo`**. Prefer Forth mirror **`char-plus-mark`** whenever host `CHAR+` collides. **CRITICAL:** do **NOT** redefine `CHAR` / `CHARS` / `[CHAR]` / `char-unit` / `chars-n` / `bracket-char` — those stay wave19 **1** surfaces (this tip is a **thin advance mark**, not a CHAR-CHARS reopen). Pairs with wave19 CHAR-CHARS **without** promoting CHAR-CHARS, **without** unicode / XCHAR, and **without** reopening COMPARE / ALIGN / CELL / SEARCH-WORDLIST / TO-NUMBER / HOLD. **Not** tip2 HOLD / tip3 U-LESS / tip4 ABS-NEGATE / tip5 DOCS-CITES. Independent of tip2–5.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `CHAR+` / `char-plus-mark` | `( c-addr -- c-addr+1 )` *or* `( -- )` with fixed demo fixture | Thin char-unit advance mark; print `[char+] CHAR+` (+ optional `u=<n>` / `addr=<a>`); classic picture: addr + 1 CHAR (Linux SoT char-size=1 → advance by 1) |
| `charplus-demo` | `( -- )` | See §5 |

Host note: bind bare `CHAR+` on the Linux REPL **only if** that name does not collide with host Forth `CHAR+`. Prefer Forth mirror **`char-plus-mark`** as the Lab-facing surface when in doubt — **do not** redefine host `CHAR+`. **CRITICAL:** do **not** redefine `CHAR` / `CHARS` / `[CHAR]` / `char-unit` / `chars-n` / `bracket-char` — those stay wave19 **1** (this tip is a **thin char-unit advance mark**, not a CHAR-CHARS reopen). **Do not** redefine `CELL` / `CELLS` / `ALIGN` / `ALIGNED` — CELL-CELLS stays (**not** ALIGN reopen). Optional `u=` / `addr=` are host ints / fixture echo only — not unicode codepoints, not XCHAR, not a HERE bump, not ALIGN. Prefer **fixed demo address / char-unit fixtures** (classic stub addr then +1 CHAR — document) so Lab hit is deterministic and FAIL is avoided.

## 3. Stub semantics

- **`CHAR+` / `char-plus-mark`:** take (or use fixed demo) a pictured address / char-unit fixture — classic ANS sketch `( c-addr -- c-addr' )` where `c-addr' = c-addr + 1 CHAR` (char-size units). On Linux SoT char-size=1 this is address + 1. This tip is **marker only**: print `[char+] CHAR+` and optionally `u=<n>` and/or `addr=<a>` (Lab-greppable; prefer documenting which Shipper emits). Classic fixture welcome: stub address (e.g. pictured `addr=1000` / PAD / SOURCE / WORD-BL word-buffer / fixed demo pointer — document) → advance by one char-unit (`u=1` advance and/or `addr=<advanced>`). **Does not** rewrite CHAR/CHARS/[CHAR], implement unicode / XCHAR / multi-byte width, reopen ALIGN/CELL, bump HERE, allocate, or compile into a body. **Does not** redefine `CHAR` / `CHARS` / `[CHAR]` / `char-unit` / `chars-n` / `bracket-char`. Captured values are host ints / fixture echo only.
- **Fixed demo address / char-unit fixtures (document):**
  - **Classic char-unit advance picture (required set):** e.g. fixture stub addr then `CHAR+` / `char-plus-mark` so Lab greps `[char+] CHAR+`. Optional `u=` / `addr=` welcome (classic `u=1` advance / `addr=<base+1>` picture on Linux SoT).
  - **Optional result echo:** `u=<advance-units>` and/or `addr=<advanced-address>` — document which form Shipper emits. Greppable `[char+] CHAR+` alone is enough for Lab OK when the marker line appears.
  - **Optional CHAR-CHARS pairing:** fixture may sit beside (not replacing) `char-demo` CHAR/CHARS/[CHAR] pictures — document; do **not** require a CHAR-CHARS reopen / CHAR redefine / unicode / ALIGN reopen.
  - **Optional CELL pairing:** fixture may sit beside cell-unit picture — document; do **not** require ALIGN/CELL reopen / cell-size rewrite.
- **Optional push:** if the host stack is easy, push pictured advanced `c-addr'`; marker alone is enough for Lab OK — do not require a real address arithmetic VM / unicode / XCHAR / ALIGN reopen / HERE bump.
- **FAIL:** `[char+] FAIL reason=<…>` optional (demo **must avoid**). Prefer not emitting `[char+] FAIL` on the happy path. No required FAIL reason this tip — missing addr / neg advance stay out of the demo (or document as optional miss picture that still avoids FAIL).
- Storage: fixed demo address / char-unit fixture / host int echo only. **No** unicode / XCHAR / multi-byte width / locale probe, no CHAR-CHARS reopen, no CHAR/CHARS/[CHAR]/char-unit/chars-n/bracket-char redefine, no COMPARE reopen, no ALIGN/CELL reopen, no HERE bump, no HOLD / pictured numeric, no SEARCH-WORDLIST reopen, no arena.
- **CHAR-CHARS stays (thin companion amend only):** do **not** wipe wave19 CHAR-CHARS content. CHAR+ is a **sibling** char-unit **advance** mark beside CHAR/CHARS/[CHAR] — not a CHAR-CHARS reopen. Thin companion amend of CHAR-CHARS.md only. **Do not** redefine `CHAR` / `CHARS` / `[CHAR]` / `char-unit` / `chars-n` / `bracket-char`. This tip’s Lab greps are `[char+] CHAR+` and `[charplus-demo] OK` only — **never** Lab-grep `[char] CHAR` / `[char] CHARS` / `[char] [CHAR]` / `char-unit` / `chars-n` / `bracket-char` as this tip’s CHAR+ success (those stay wave19 **1**).
- **COMPARE / ALIGN / HOLD stay out:** do **not** redefine `COMPARE` / `compare-mark`. Do **not** redefine `ALIGN` / `ALIGNED` / `align-here` / `aligned-addr`. Do **not** land HOLD / `<#` / `#` / `#S` / `#>` / `SIGN` this tip (tip2). **Leave wave22 primaries untouched** (LSHIFT-RSHIFT / ZERO-EQUALS / TO-NUMBER / SEARCH-WORDLIST land md5s below).
- Nest with prior search / number / zero / shift / bit / compare / base / accept / exec / count / within / true / env / body / char / state / word / find / tick / allot / cell stubs OK. `dict-reset` unaffected (fixed host fixtures, not dictionary). Assert prior `char-demo` / `cell-demo` still OK (CHAR-CHARS surfaces untouched; CELL/ALIGN untouched).
- Still no unicode / XCHAR, no CHAR-CHARS reopen, no COMPARE reopen, no ALIGN reopen, no HOLD (tip2), no U-LESS (tip3), no ABS-NEGATE (tip4), no tip5 DOCS-CITES, no linked XT / real DOES> XT / full arena / full Win/Android Forth VM. Wave22 tip1–5 + wave21 tip1–5 + wave20 tip1–4 + wave19 tip1–4 stay landed — keep cites; this tip does not reopen them.

## 4. Markers

```
[char+] CHAR+ [u=<n>] [addr=<a>]   # u=/addr= optional; classic char-unit advance picture: addr + 1 CHAR
[char+] FAIL reason=<…>            # demo avoids
[charplus-demo] OK
[charplus-demo] FAIL
```

Lab greps `[charplus-demo] OK` plus greppable **`[char+] CHAR+`** (optional `u=` / `addr=` welcome — classic addr + 1 CHAR picture on fixed fixture). Demo avoids `[char+] FAIL`. Prefer not emitting `[char+] FAIL` on the happy path. **Do not** Lab-grep `[char] CHAR` / `[char] CHARS` / `[char] [CHAR]` / `char-unit` / `chars-n` / `bracket-char` as this tip’s surface (those stay wave19 **1**). **Do not** Lab-grep `[search]` / `[number]` / `[zero]` / `[shift]` / `[bit]` / `[compare]` / `[accept]` / `[cell]` / `[allot]` as CHAR-PLUS success (those stay their own tips). **Do not** Lab-grep unicode / XCHAR / HOLD / `<#` / `#` / `#S` / `#>` / `SIGN` / `U<` / `ABS` / `NEGATE` as this tip (those stay deferred / later tips).

## 5. `charplus-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; char-unit advance mark needs no dict entries.
2. Ensure **fixed demo address / char-unit fixture** exists (e.g. stub addr `1000` / PAD / SOURCE / WORD-BL word-buffer / documented host pointer — or equivalent) so the classic addr + 1 CHAR picture is deterministic. Fixture may live beside (not replacing) CHAR-CHARS char-unit picture / CELL unit picture / SOURCE-PAD / WORD-BL pad — document; do **not** require unicode / XCHAR / CHAR-CHARS reopen / CHAR redefine / ALIGN reopen / COMPARE reopen / HOLD / HERE bump.
3. Invoke `CHAR+` (or **`char-plus-mark`**) against the classic fixture → `[char+] CHAR+` (+ optional `u=` / `addr=` — classic `u=1` / `addr=<base+1>` welcome on Linux SoT).
4. Assert no `[char+] FAIL` on the happy path. Assert char-unit advance mark did **not** require unicode / XCHAR / CHAR-CHARS reopen / CHAR redefine / ALIGN reopen / COMPARE reopen / HOLD / HERE bump / SEARCH-WORDLIST reopen (marker-only is enough). Assert host `CHAR+` was not redefined when using the Forth mirror. Assert `CHAR` / `CHARS` / `[CHAR]` / `char-unit` / `chars-n` / `bracket-char` were **not** redefined. Assert `compare-mark` / `align-here` / `aligned-addr` / `search-wl-mark` / `to-number-mark` were **not** redefined. Assert prior `char-demo` / `cell-demo` still OK.
5. Prior `search-demo` / `number-demo` / `zero-demo` / `shift-demo` / `bit-demo` / `compare-demo` / `base-demo` / `accept-demo` / `exec-demo` / `count-demo` / `within-demo` / `true-demo` / `source-demo` / `env-demo` / `body-demo` / `char-demo` / `state-demo` / `word-demo` / `find-demo` / `tick-demo` / `allot-demo` / `cell-demo` + earlier demos incl `trit-math-demo` / `fold-demo` still OK (CHAR-CHARS surfaces and CELL/ALIGN must remain intact).
6. `[charplus-demo] OK`.

Required marker: `[char+] CHAR+`. Optional `u=` / `addr=` echo is not required for Lab OK when `[char+] CHAR+` is greppable. No unicode / XCHAR. No CHAR-CHARS reopen. No CHAR/CHARS/[CHAR] redefine. No COMPARE reopen. No ALIGN reopen. No HOLD. No HERE stub base bump.

## 6. Thin amend — companions

### `docs/CHAR-CHARS.md`

- Companions / Status: add `CHAR-PLUS.md` (wave23 **1** companion cite); **keep** CELL-CELLS / KERNEL / STRING-LIT / WORD-BL / PARSE-NAME cites — do not wipe wave19 CHAR-CHARS content.
- Purpose / §3 / non-goals: CHAR/CHARS/[CHAR] stay char-unit marks; `CHAR+` is sibling **thin char-unit advance mark** — **not** a CHAR/CHARS/[CHAR] reopen / unicode / XCHAR / ALIGN reopen / HERE bump. Do not wipe wave19 CHAR-CHARS content. Stress: CHAR-PLUS sibling thin advance — **NOT** CHAR-CHARS reopen; **CRITICAL** do **not** redefine `CHAR` / `CHARS` / `[CHAR]` / `char-unit` / `chars-n` / `bracket-char`; prefer `char-plus-mark` whenever host `CHAR+` collides.
- Non-goals: `CHAR+` → `docs/CHAR-PLUS.md` (wave23 **1**). CHAR/CHARS/[CHAR] stay on this tip (already landed). Unicode / XCHAR still out. ALIGN/CELL stay wave15 **1**. Tip2 HOLD / tip3 U-LESS / tip4 ABS-NEGATE / tip5 DOCS-CITES still later (wave23 **2–5**).
- Acceptance: Lab smokes `charplus-demo` (retains `char-demo` + `cell-demo` + `search-demo` + `number-demo` + prior).
- Cite: `docs/CHAR-PLUS.md`.

### `docs/KERNEL.md`

- Companions: add `CHAR-PLUS.md` (wave23 **1**); **keep** wave22 tip1–4 LSHIFT-RSHIFT / ZERO-EQUALS / TO-NUMBER / SEARCH-WORDLIST cites and wave21 tip1–4 ACCEPT-REFILL / BASE-HEX / COMPARE / BITWISE cites and wave20 tip1–4 / wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD / wave18 tip1–4 / wave17 tip1–4 / wave16 DEFER/MARKER/BUFFER/EXIT / wave15 CELL cites.
- Words table: add `CHAR+` / `char-plus-mark` stub + `charplus-demo` (cite tip; Forth mirror `char-plus-mark` — thin char-unit advance mark only; **CRITICAL:** do **not** redefine `CHAR` / `CHARS` / `[CHAR]` / `char-unit` / `chars-n` / `bracket-char`; prefer `char-plus-mark` whenever host `CHAR+` collides; **not** unicode / XCHAR; **not** CHAR-CHARS reopen; **not** COMPARE reopen; **not** ALIGN reopen; **not** HOLD; optional `u=` / `addr=` — classic addr + 1 CHAR picture welcome).
- Non-goals: `CHAR+` thin char-unit advance mark → `docs/CHAR-PLUS.md`. CHAR/CHARS/[CHAR] stay on `CHAR-CHARS.md` (leave primary thin-amended only — do not wipe). CELL/CELLS/ALIGN/ALIGNED stay on `CELL-CELLS.md`. COMPARE stays on `COMPARE.md`. SEARCH-WORDLIST stays on `SEARCH-WORDLIST.md`. Tip2 HOLD / tip3 U-LESS / tip4 ABS-NEGATE / tip5 DOCS-CITES still later (wave23 **2–5**).
- Acceptance: Lab smokes `charplus-demo` (and retains `char-demo` + `search-demo` + `number-demo` + `zero-demo` + `shift-demo` + `bit-demo` + `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `env-demo` + `cell-demo` + prior demos incl `trit-math-demo` / `fold-demo`).
- Cite: `docs/CHAR-PLUS.md`.

### Optional — `docs/CELL-CELLS.md`

- Companions / Status: add light `CHAR-PLUS.md` (wave23 **1**) cite; **keep** TO-NUMBER / ZERO-EQUALS / LSHIFT-RSHIFT / BITWISE / TRUE-FALSE / WITHIN / BASE-HEX / CHAR-CHARS / ENVIRONMENT-QUERY / ALLOT-HERE / KERNEL cites — do not wipe wave15 CELL content.
- Purpose / §3 / non-goals: cell-unit stubs stay; `CHAR+` may echo optional stub `u=` / `addr=` that picture **char-unit** advance beside cell units — **not** a CELL/ALIGN reopen / cell-size rewrite / CHAR-CHARS reopen / unicode / HERE stub base bump. Do not wipe wave15 / wave19 / wave20 / wave21 / wave22 CELL/CHAR/ENV/TRUE/BASE/BITWISE/LSHIFT/ZERO/TO-NUMBER content. Stress: char-unit advance mark beside cell units — not CELL/ALIGN reopen; CHAR-CHARS stays; prefer `char-plus-mark`.
- Non-goals: `CHAR+` → `docs/CHAR-PLUS.md` (wave23 **1**). CELL/CELLS/ALIGN/ALIGNED stay on this tip (already landed). CHAR-CHARS stays wave19 **1**.
- Acceptance: Lab smokes `charplus-demo` (retains `cell-demo` + `char-demo` + `number-demo` + `search-demo`).
- Cite: `docs/CHAR-PLUS.md`.

### Optional — `docs/HOST-PARITY.md`

- Companions: add light `CHAR-PLUS.md` (wave23 **1**) cite; **keep** SEARCH-WORDLIST / TO-NUMBER / ZERO-EQUALS / LSHIFT-RSHIFT / BITWISE / TRUE-FALSE / BASE-HEX / ENVIRONMENT-QUERY / KERNEL / BUILD / INSTALL / INTERPRET cites.
- Purpose / non-goals: Win/Android stay CONTRACT-parity; `charplus-demo` CONTRACT line is acceptable — **not** a full Forth VM / unicode / XCHAR / CHAR-CHARS reopen / ALIGN port. Do not wipe wave8 HOST-PARITY content. Stress: prefer `char-plus-mark`; do not redefine CHAR/CHARS/[CHAR]/char-unit/chars-n/bracket-char.
- Non-goals: `CHAR+` → `docs/CHAR-PLUS.md` (wave23 **1**). Full Win/Android Forth VM still out. CHAR-CHARS stays wave19 **1**. SEARCH-WORDLIST stays wave22 **4**.
- Acceptance: Lab smokes `charplus-demo` (Win/Android: `charplus-demo CONTRACT` OK).
- Cite: `docs/CHAR-PLUS.md`.

Do **not** wipe wave22 tip1–5 / wave21 tip1–5 / wave20 tip1–4 / wave19 tip1–4 / wave18–15 prior content. Do **not** amend ARCHITECTURE / IMPLEMENTATION-GAPS / LSHIFT-RSHIFT / ZERO-EQUALS / TO-NUMBER / SEARCH-WORDLIST / COMPARE / ACCEPT-REFILL primary this tip (amends are CHAR-CHARS + KERNEL + optional CELL-CELLS / HOST-PARITY only). **Leave `LSHIFT-RSHIFT.md` untouched** (`e5a94d8a16d47aa7ceae92b54344712e` / 26299). **Leave `ZERO-EQUALS.md` untouched** (`fd250f768e41fd70dd32c01faa32fc74` / 26165). **Leave `TO-NUMBER.md` untouched** (`ab016df909ab12b87c1c3a7fff9a9a81` / 25997). **Leave `SEARCH-WORDLIST.md` untouched** (`0f9fa84fb70d08c2da7e32f20d1a6e29` / 31054). **Leave `ARCHITECTURE.md` / `IMPLEMENTATION-GAPS.md` untouched** (`42902a5431b3f868b9ff7d73414cccc8` / `465ea6f99db2983a626d42b8731e59c2`). **Leave `WAVE23-PROPOSAL.md` / `WAVE23-COS-PASTE.txt` untouched** (`01fb36f4ea8c4d25e3c3b7ec998e6718` / `3c5e643d45d43a7f46770e2a8563fdf3`). Tip5 cites after 1–4 PASS. Skip 2DUP-FAMILY + ABORT" polish.

## 7. Non-goals

- Redefining `CHAR` / `CHARS` / `[CHAR]` / `char-unit` / `chars-n` / `bracket-char` (CHAR-CHARS stays — **not** a CHAR-CHARS reopen; thin companion amend only; **CRITICAL**)
- Unicode / XCHAR / multi-byte char width / locale probe / codepoint arithmetic
- `CHAR-CHARS` reopen / re-spec of wave19 CHAR/CHARS/[CHAR] primary surface beyond thin companion cite
- `COMPARE` / `compare-mark` reopen (wave21 **3** — already stubbed; leave COMPARE.md primary untouched)
- `ALIGN` / `ALIGNED` / `align-here` / `aligned-addr` / `CELL` / `CELLS` reopen (wave15 — already stubbed; thin optional CELL-CELLS companion only — **not** ALIGN reopen)
- `HOLD` / `<#` / `#` / `#S` / `#>` / `SIGN` pictured numeric (tip2 HOLD — still deferred this tip)
- `U<` unsigned compare flag mark (tip3 U-LESS — still deferred)
- `ABS` / `NEGATE` marks (tip4 ABS-NEGATE — still deferred)
- Docs cites pass (wave23 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `SEARCH-WORDLIST` / FIND reopen (wave22 **4** / wave18 **2** — already stubbed; leave SEARCH-WORDLIST.md / FIND surfaces)
- `>NUMBER` / BASE / HERE stub base bump (wave22 **3** / wave21 **2** / wave14 **1** — already stubbed; leave those primaries untouched)
- `0=` / LSHIFT / BITWISE / TRUE-FALSE / WITHIN / COUNT / EXECUTE / SOURCE / PAD / ACCEPT-REFILL reopen (wave20–22 — already stubbed; keep cites)
- Real DOES> XT / real branch XT / LEAVE jump / full arena / linked XT / real STATE cell
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live — **skip 2DUP-FAMILY**)
- `ABORT"` polish (already optional-wired inside `throw-demo` — **skip**)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/CHAR-PLUS.md` present (Research byte-copy OK); `CHAR-CHARS.md` + `KERNEL.md` thin amends present (+ optional `CELL-CELLS.md` / `HOST-PARITY.md`); wave22 tip1–5 + wave21 tip1–5 + wave20 tip1–4 + wave19 tip1–4 + wave18–15 prior cites retained; host `CHAR+` untouched via mirrors (`char-plus-mark` preferred); `CHAR` / `CHARS` / `[CHAR]` / `char-unit` / `chars-n` / `bracket-char` **not** redefined; `compare-mark` / `align-here` / `search-wl-mark` / `to-number-mark` **not** redefined; `LSHIFT-RSHIFT.md` primary untouched (`e5a94d8a16d47aa7ceae92b54344712e` / 26299); `ZERO-EQUALS.md` primary untouched (`fd250f768e41fd70dd32c01faa32fc74` / 26165); `TO-NUMBER.md` primary untouched (`ab016df909ab12b87c1c3a7fff9a9a81` / 25997); `SEARCH-WORDLIST.md` primary untouched (`0f9fa84fb70d08c2da7e32f20d1a6e29` / 31054); `ARCHITECTURE.md` + `IMPLEMENTATION-GAPS.md` unchanged (`42902a5431b3f868b9ff7d73414cccc8` / `465ea6f99db2983a626d42b8731e59c2`); `WAVE23-PROPOSAL.md` / `WAVE23-COS-PASTE.txt` untouched (`01fb36f4ea8c4d25e3c3b7ec998e6718` / `3c5e643d45d43a7f46770e2a8563fdf3`).
2. `charplus-demo` → OK (markers §4; `[char+] CHAR+` greppable; optional `u=` / `addr=` welcome — classic addr + 1 CHAR picture on fixed fixture; no FAIL on happy path; no unicode / XCHAR / CHAR-CHARS reopen / CHAR redefine / COMPARE reopen / ALIGN reopen / HOLD / HERE stub base bump / SEARCH-WORDLIST reopen; no `char-unit`/`chars-n`/`bracket-char` redefine). Prior `char-demo` + `search-demo` + `number-demo` + `zero-demo` + `shift-demo` + `bit-demo` + `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `env-demo` + `cell-demo` + earlier demos incl `trit-math-demo` / `fold-demo` still OK.
3. Regression green (wave22 tip1–5 + wave21 tip1–5 + wave20 tip1–5 + wave19 tip1–5 + wave18 tip1–5 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `charplus-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**)
- `docs/CHAR-CHARS.md` (wave19 **1** — CHAR/CHARS/[CHAR] char-unit companion; CHAR-PLUS sibling thin advance — **not** a CHAR-CHARS reopen; **CRITICAL** do not redefine CHAR/CHARS/[CHAR]/char-unit/chars-n/bracket-char)
- `docs/CELL-CELLS.md` (wave15 **1**, optional — cell/unit picture companion; char-unit advance beside cell units — not CELL/ALIGN reopen)
- `docs/HOST-PARITY.md` (wave8 **4**, optional — `charplus-demo` CONTRACT parity welcome)
- `docs/STRING-LIT.md` (wave13 **4** — string-literal companion; CHAR+ is char-unit advance — not S"/escape/unicode heap)
- `docs/WORD-BL.md` (wave18 **3** — optional pad/word-buffer fixture companion; not WORD-BL reopen)
- `docs/SOURCE-PAD.md` (wave19 **4** — optional SOURCE/PAD fixture companion; not SOURCE-PAD reopen / ACCEPT-REFILL reopen)
- `docs/COMPARE.md` (wave21 **3** — prior tip; keep cites; leave primary untouched this tip; host `cstr=` ≠ ANS COMPARE)
- `docs/SEARCH-WORDLIST.md` (wave22 **4** — prior tip; keep cites; leave primary untouched this tip)
- `docs/TO-NUMBER.md` (wave22 **3** — prior tip; keep cites; leave primary untouched this tip; HERE stub `$1000` stays)
- `docs/ZERO-EQUALS.md` (wave22 **2** — prior tip; keep cites; leave primary untouched this tip)
- `docs/LSHIFT-RSHIFT.md` (wave22 **1** — prior tip; keep cites; leave primary untouched this tip)
- `docs/ACCEPT-REFILL.md` / `docs/BASE-HEX.md` / `docs/BITWISE.md` (wave21 **1–2 / 4** — prior tips; keep cites)
- `docs/TRUE-FALSE.md` / `docs/WITHIN.md` / `docs/COUNT.md` / `docs/EXECUTE.md` (wave20 **1–4** — prior tips; keep cites)
- `forth/tritium/kernel.fs` (char-plus-mark only — do not redefine host `CHAR+`; **do not** redefine CHAR/CHARS/[CHAR]/char-unit/chars-n/bracket-char; **do not** redefine compare-mark / align-here / search-wl-mark / to-number-mark)
- ANS Forth `CHAR+` (thin char-unit advance mark only — not unicode / XCHAR; not CHAR-CHARS reopen; not COMPARE reopen; not ALIGN reopen; not HOLD; host `CHAR+` ≠ force bare bind — prefer `char-plus-mark`)
- Explicit deferral: WAVE19-PROPOSAL + WAVE20-PROPOSAL + WAVE21-PROPOSAL + WAVE22-PROPOSAL + WAVE23-PROPOSAL (`CHAR+` — thin char-unit advance mark; not unicode/XCHAR; not CHAR-CHARS reopen)
- Base tip: `f6eb303` / `f6eb30348b9f1395030b1c6f075b99d9327b0624` (#106 wave22 tip5 DOCS-CITES CLOSED on tip)
- Wave23 proposal: `/workspace/tritium-research-docs/WAVE23-PROPOSAL.md`


## 10. Shipper implementation notes (Linux SoT)

These notes are normative for Shipper drafting on branch `shipper/char-plus-w23` from base `f6eb30348b9f1395030b1c6f075b99d9327b0624`. They do **not** authorize merge, push, Mango touch, opaque-weight ML, or reopening closed wave17–22 tips.

### 10.1 Binding order

1. Prefer Forth mirror **`char-plus-mark`** first. Lab greps do not require the bare ANS name `CHAR+` to be bound when the mirror prints `[char+] CHAR+`.
2. If bare `CHAR+` is free on the Linux REPL load path, Shipper **may** bind it as a thin alias that emits the same markers — still **must not** redefine wave19 CHAR surfaces.
3. Never alias `CHAR+` onto `CHAR` / `CHARS` / `[CHAR]` / `char-unit` / `chars-n` / `bracket-char`. Advance ≠ unit resolve / scale / compile-time sibling.

### 10.2 Classic picture (addr + 1 CHAR)

Document one fixed fixture. Recommended:

| Field | Example | Notes |
|-------|---------|-------|
| base addr | `1000` (decimal) or `$1000` pictured | May echo ALLOT-HERE stub base as **fixture only** — do **not** bump `_here` / `HERE` / `here-at` |
| advance | `1` char-unit | Linux SoT char-size=1 → byte advance 1 |
| optional `u=` | `u=1` | Advance units |
| optional `addr=` | `addr=1001` | Advanced address echo |

Shipper may use PAD / SOURCE / WORD-BL word-buffer addresses instead of `1000` — document which. Lab greps `[char+] CHAR+` regardless of which fixture is chosen, as long as the marker line is greppable and FAIL is avoided.

### 10.3 What Lab greps / does not grep

**Greps (required):**

- `[charplus-demo] OK`
- `[char+] CHAR+`

**Greps (optional welcome):**

- `u=` on the CHAR+ marker line
- `addr=` on the CHAR+ marker line
- `charplus-demo CONTRACT` on Win/Android parity

**Must NOT Lab-grep as this tip’s success:**

- `[char] CHAR` / `[char] CHARS` / `[char] [CHAR]` (wave19 **1**)
- `char-unit` / `chars-n` / `bracket-char` names as CHAR+ success
- `[search] SEARCH-WORDLIST` / `[number] >NUMBER` / `[zero]` / `[shift]` / `[bit]` / `[compare]` / `[cell]` / `[allot]`
- unicode / XCHAR / codepoint / locale strings
- `HOLD` / `<#` / `#` / `#S` / `#>` / `SIGN` / `U<` / `ABS` / `NEGATE`

### 10.4 Regression retain list (abbrev)

Retain green: prior wave7–22 demos including `char-demo` / `search-demo` / `number-demo` / `zero-demo` / `shift-demo` / `bit-demo` / `compare-demo` / `base-demo` / `accept-demo` / `exec-demo` / `count-demo` / `within-demo` / `true-demo` / `source-demo` / `env-demo` / `body-demo` / `cell-demo` / `allot-demo` / `kernel-demo` / `trit-math-demo` / `fold-demo` (+ earlier colon/control/string/throw/fill/pick/defer/marker/buffer/exit/synonym/parse/eval/recurse/tick/find/word/state suite).

### 10.5 Companion amend checklist

Required thin: `CHAR-CHARS.md` + `KERNEL.md` (keep wave19 CHAR surfaces + wave22 tip1–4 cites). Optional thin: `CELL-CELLS.md` / `HOST-PARITY.md`. **Untouched this tip:** ARCHITECTURE / IMPLEMENTATION-GAPS / WAVE23-PROPOSAL / WAVE23-COS-PASTE / wave22 primaries LSHIFT-RSHIFT / ZERO-EQUALS / TO-NUMBER / SEARCH-WORDLIST (land md5s locked).

### 10.6 Non-goals restated for Shipper

Do **not**: redefine CHAR/CHARS/[CHAR]/char-unit/chars-n/bracket-char; land unicode/XCHAR; reopen COMPARE; reopen ALIGN/CELL; land HOLD/`<#`/`#`/`#S`/`#>`/`SIGN` (tip2); land `U<` (tip3); land ABS/NEGATE (tip4); amend ARCHITECTURE/GAPS (tip5); tip Shipper yourself from Research beyond this handoff; push; PR-merge; touch FunMan1995/Mango; redefine host 2dup/2drop/2swap; polish ABORT"; bump HERE stub `$1000`.

### 10.7 Relation to CHAR-CHARS (why advance ≠ unit)

| Surface | Tip | Role |
|---------|-----|------|
| `CHAR` / `char-unit` | wave19 **1** | resolve a demo character / ordinal echo |
| `CHARS` / `chars-n` | wave19 **1** | scale count × char-size (no HERE bump) |
| `[CHAR]` / `bracket-char` | wave19 **1** | compile-time sibling **flag echo** only |
| `CHAR+` / `char-plus-mark` | wave23 **1** | **advance** a pictured address by one char-unit |

CHAR+ does not take a next-token character, does not scale `k`, and does not compile a char literal. It only pictures address + 1 CHAR.

### 10.8 Relation to CELL / ALIGN (why not reopen)

`CELL+` is not this tip. `ALIGN` / `ALIGNED` round to cell boundaries. `CHAR+` advances by **char-unit** (Linux SoT: 1). Optional CELL-CELLS companion cite may note “char-unit advance beside cell units” — that is **cite only**, not an ALIGN/CELL reopen or cell-size rewrite.

### 10.9 Relation to HOLD (tip2 boundary)

Pictured numeric `HOLD` / `<#` / `#` starts in wave23 tip **2**. CHAR+ must not emit hold-buffer markers, must not redefine `_here` / `HERE` / `here-at`, and must not claim pictured-numeric progress. Optional `u=` / `addr=` on `[char+] CHAR+` are **address advance** echoes only.

### 10.10 Host collision matrix

Prefer `char-plus-mark` if host `CHAR+` collides. Leave host/wave19 `CHAR`/`CHARS`/`[CHAR]`/`char-unit`/`chars-n`/`bracket-char` untouched. Leave find/find-xt/search-wl-mark, to-number-mark, and prior CRITICAL lowercase `lshift`/`rshift`/`and`/`or` untouched.

### 10.11 Marker grammar (Shipper emit)

Preferred single-line form:

```
[char+] CHAR+ u=1 addr=1001
```

Minimum greppable form:

```
[char+] CHAR+
```

Demo close:

```
[charplus-demo] OK
```

Avoid:

```
[char+] FAIL …
[charplus-demo] FAIL
```

### 10.12 Docs byte-copy reminder

Research drafts live under `/workspace/tritium-research-docs/`. Shipper byte-copies into repo `docs/` on branch `shipper/char-plus-w23` only. Parent tips Shipper after Research handoff. Research does **not** tip Shipper / push / merge / touch Mango.

### 10.13 Still deferred (wave23 and beyond)

HOLD (tip2); U< (tip3); ABS/NEGATE (tip4); DOCS-CITES (tip5); unicode/XCHAR beyond thin CHAR+; full pictured `#S`/`#>`/`SIGN`; FIND reopen / linked dict / SEARCH-WORDLIST runtime; real boolean cell; WITHIN runtime; full ANS ENVIRONMENT? table; real DOES> XT; linked XT / real STATE cell; real XT execute; real RECURSE self-XT; real EVALUATE/INCLUDE nested VM; full arena/heap; real branch XT/LEAVE jump; full ACCEPT/REFILL input-buffer VM; full Win/Android Forth VM; real crypto/network fleet; DRENA/REKIA deepen; ASSUMPTIONS.md; 2DUP-FAMILY; ABORT" polish; Mango.

### 10.14 Acceptance one-liner for Lab

`charplus-demo` prints greppable `[char+] CHAR+` and ends `[charplus-demo] OK`; prior `char-demo` still OK; CHAR/CHARS/[CHAR]/char-unit/chars-n/bracket-char not redefined; no unicode/XCHAR; no CHAR-CHARS/COMPARE/ALIGN/HOLD reopen; Linux SoT; no merge.

### 10.15 Paste anchors

- Proposal tip1 shape: `/workspace/tritium-research-docs/WAVE23-PROPOSAL.md` § Tip 1 — CHAR-PLUS
- Base: `f6eb30348b9f1395030b1c6f075b99d9327b0624`
- Branch: `shipper/char-plus-w23`
- Handoff: `WAVE23-TIP1-HANDOFF.md` (= `TIP1-HANDOFF.md`)
