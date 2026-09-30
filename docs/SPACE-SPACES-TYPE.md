# SPACE-SPACES-TYPE — `SPACE` / `SPACES` / `TYPE` (+ optional `EMIT`) output marks + `space-demo`

**Status:** Shipper-ready stub spec (wave24 item **1**)
**Canonical brief:** ANS-shaped `SPACE` / `SPACES` / `TYPE` (+ optional `EMIT`) thin output marks only; `docs/STRING-LIT.md` (wave13 **4** — `S"` / `."` / optional `.(` string-literal companion; **not** a STRING-LIT reopen; **do not** redefine `S"` / `."` / `.(` / `string-s` / `string-dot` / `string-paren`); `docs/WORD-BL.md` (wave18 **3** — `WORD` / `BL` token/pad companion; **not** a WORD-BL reopen; **do not** redefine `WORD` / `BL` / `word-mark` / `bl-mark` / `word-parse` / `bl-char`); `docs/KERNEL.md` (wave7 **5**); optional `docs/HOST-PARITY.md` (wave8 **4**); WAVE23–24 deferral closed as **thin output marks only** (not STRING-LIT reopen; not full I/O / console VM; not Win/Android VM; **not** MIN-MAX tip2; **not** SHARP-SIGN tip3; **not** WORDLIST tip4; **not** DOCS-CITES tip5; prefer Forth mirrors **`space-mark` / `spaces-mark` / `type-mark`** (+ optional **`emit-mark`**) whenever host `SPACE`/`SPACES`/`TYPE`/`EMIT` collide). **Do not** redefine prior wave23 marks (`hold-mark` / `char-plus-mark` / `u-less-mark` / `abs-mark` / `negate-mark`).
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `space.fs` / `spaces-type.fs` / `emit.fs`); Linux host REPL; prefer Forth mirrors **`space-mark` / `spaces-mark` / `type-mark`** (+ optional **`emit-mark`**) whenever host `SPACE`/`SPACES`/`TYPE`/`EMIT` collide; **do not** redefine `S"` / `."` / `.(` / `string-s` / `string-dot` / `string-paren` (STRING-LIT stays — **not** a STRING-LIT reopen); **do not** redefine `WORD` / `BL` / `word-mark` / `bl-mark` / `word-parse` / `bl-char` (WORD-BL stays — **not** a WORD-BL reopen); **do not** redefine `hold-mark` / `char-plus-mark` / `u-less-mark` / `abs-mark` / `negate-mark` (wave23 tip1–4 primaries stay untouched)
**Companions:** `docs/STRING-LIT.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip); optional light cite `docs/WORD-BL.md` / `docs/HOST-PARITY.md`
**Base tip SHA:** `2b8acfc1` (wave23 tip5 DOCS-CITES PASS / #111) / full `2b8acfc1ea46c5d4676eb969c8cc1be30d89d0cc`

## 1. Purpose

WAVE13 tip **4** landed `S"` / `."` / optional `.(` string-literal parse marks (`docs/STRING-LIT.md` — marker stubs; no counted-string heap). WAVE18 tip **3** landed `WORD` / `BL` token/pad stub markers (`docs/WORD-BL.md` — fixed word-buffer; host WORDS untouched). WAVE20–21 COUNT/COMPARE marks landed. WAVE23 tip **1–4** CHAR-PLUS/HOLD/U-LESS/ABS-NEGATE stay untouched. WAVE23 tip **5** closed DOCS-CITES on tip SHA `2b8acfc1…` (#111). WAVE23–24 deferred `SPACE` / `SPACES` / `TYPE` as thin output marks beside STRING-LIT + WORD-BL (**not** STRING-LIT reopen; **not** full I/O / console VM; **not** Win/Android VM). This tip lands **stub** output marks only: `SPACE` (or Forth mirror **`space-mark`**) prints `[space] SPACE` (classic emit-one-blank picture welcome); `SPACES` (or **`spaces-mark`**) prints `[space] SPACES` (+ optional `u=` / `n=` — classic emit-n-blanks picture welcome); `TYPE` (or **`type-mark`**) prints `[space] TYPE` (+ optional `addr=` / `u=` — classic type-c-addr-u picture welcome on a fixed demo fixture). Optional `EMIT` (or **`emit-mark`**) prints `[space] EMIT` (+ optional `c=` / `u=` — classic emit-one-char picture welcome) when free — **do not require** EMIT for Lab OK. Smoke via **`space-demo`**. Prefer Forth mirrors **`space-mark` / `spaces-mark` / `type-mark`** (+ optional **`emit-mark`**) whenever host `SPACE`/`SPACES`/`TYPE`/`EMIT` collide. Builds beside wave13 STRING-LIT + wave18 WORD-BL **without** reopening either and **without** a full I/O / console VM. **Not** tip2 MIN-MAX / tip3 SHARP-SIGN / tip4 WORDLIST / tip5 DOCS-CITES. Independent of tip2–5. **Leave untouched:** ABS-NEGATE (`caa1e0d…`/30661), HOLD (`dcc8a16…`/30969), CHAR-PLUS (`bb879e6…`/30666), U-LESS (`1f17242…`/30870), ARCH (`2575a64…`/18387), GAPS (`1f2a5d9…`/63563), WAVE24-PROPOSAL (`1cceddb…`), COS-PASTE (`df576fd…`).

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `SPACE` / `space-mark` | `( -- )` | Emit-one-blank output mark; print `[space] SPACE`; classic single-blank picture welcome |
| `SPACES` / `spaces-mark` | `( u -- )` *or* `( -- )` with fixed demo fixture | Emit-n-blanks output mark; print `[space] SPACES` (+ optional `u=` / `n=`); classic n-blanks picture welcome |
| `TYPE` / `type-mark` | `( c-addr u -- )` *or* `( -- )` with fixed demo fixture | Type-c-addr-u output mark; print `[space] TYPE` (+ optional `addr=` / `u=` / content echo); classic type picture on fixed fixture welcome |
| `EMIT` / `emit-mark` | `( char -- )` *or* `( -- )` with fixed demo fixture | **Optional** this tip; emit-one-char output mark; print `[space] EMIT` (+ optional `c=` / `u=`); classic single-char picture welcome |
| `space-demo` | `( -- )` | See §5 |

Host note: bind bare `SPACE` / `SPACES` / `TYPE` (+ optional `EMIT`) on the Linux REPL **only if** those names do not collide with host Forth. Prefer Forth mirrors **`space-mark` / `spaces-mark` / `type-mark`** (+ optional **`emit-mark`**) when in doubt — **do not** redefine host `SPACE`/`SPACES`/`TYPE`/`EMIT`. Optional `u=` / `n=` / `addr=` / `c=` are host ints / fixture echo only — not a live console VM / STRING-LIT reopen / WORD-BL reopen / pictured-numeric reopen. Prefer **fixed demo fixtures** (classic SPACE blank; SPACES with small `u` e.g. `3`; TYPE over a short fixed host string/fixture e.g. `hi` / `ok`; optional EMIT with printable char) so Lab hit is deterministic and FAIL is avoided. Greppable `[space] SPACE` + `[space] SPACES` + `[space] TYPE` are enough for Lab OK (optional `[space] EMIT` welcome). **Do not** redefine `S"` / `."` / `.(` / `string-s` / `string-dot` / `string-paren` / `WORD` / `BL` / `word-mark` / `bl-mark` / `word-parse` / `bl-char` / `hold-mark` / `char-plus-mark` / `u-less-mark` / `abs-mark` / `negate-mark`.

## 3. Stub semantics

- **`SPACE` / `space-mark`:** emit-one-blank output mark. Classic ANS / common Forth picture: print one blank (space character). Print `[space] SPACE` (Lab-greppable). **Does not** rewrite `S"` / `."` / `.(`, reopen STRING-LIT / WORD-BL, open a console VM, bump HERE, open a heap, or create dict entries. Marker alone is enough — no requirement to write to a real output device beyond the greppable marker line.
- **`SPACES` / `spaces-mark`:** take (or use fixed demo) unsigned/count stub `( u )`. Classic ANS / common Forth picture: emit `u` blanks. Print `[space] SPACES` and optionally `u=<n>` / `n=<n>` (Lab-greppable; prefer documenting which Shipper emits). Classic fixture welcome: `u=3` → `[space] SPACES` (+ optional `u=3`). Zero count welcome when free — **do not require**. **Does not** reopen STRING-LIT / WORD-BL / full I/O VM.
- **`TYPE` / `type-mark`:** take (or use fixed demo) `( c-addr u )` over a fixed host string/fixture. Classic ANS / common Forth picture: type `u` chars from `c-addr`. Print `[space] TYPE` and optionally `addr=<a>` / `u=<n>` / content substring echo (Lab-greppable; prefer documenting which Shipper emits). Classic fixture welcome: short fixture `hi` / `ok` / `abc` → `[space] TYPE` (+ optional `u=2` / `addr=0`). **Does not** `entry-create` the typed text, reopen STRING-LIT / COMPARE / COUNT, or open a heap. Captured values are host ints / fixture echo only.
- **`EMIT` / `emit-mark` (optional):** take (or use fixed demo) char stub `( char )`. Classic ANS / common Forth picture: emit one character. Print `[space] EMIT` and optionally `c=<char>` / `u=<n>` (Lab-greppable). Welcome when free — **do not require** for Lab OK. Same constraints as SPACE — marker only; **not** a console VM / STRING-LIT reopen.
- **Fixed demo fixtures (document):**
  - **Classic SPACE (required):** invoke `SPACE` / `space-mark` → `[space] SPACE`. Demo must exercise SPACE so Lab greps `[space] SPACE`.
  - **Classic SPACES (required):** e.g. `u=3` (or equivalent small count) → `[space] SPACES` (+ optional `u=` / `n=`). Demo must exercise SPACES so Lab greps `[space] SPACES`.
  - **Classic TYPE (required):** fixed short fixture (e.g. `hi` / `ok`) → `[space] TYPE` (+ optional `addr=` / `u=` / content). Demo must exercise TYPE so Lab greps `[space] TYPE`.
  - **Optional EMIT:** e.g. char `A` / `*` / blank → `[space] EMIT` (+ optional `c=` / `u=`) — welcome when free; do **not** require for Lab OK; do **not** promote to full I/O / console VM / STRING-LIT reopen.
  - **Optional result echo:** `u=` / `n=` / `addr=` / `c=` / content substring — document which form Shipper emits. Greppable markers alone are enough for Lab OK when `[space] SPACE` + `[space] SPACES` + `[space] TYPE` appear.
- **Optional push:** if the host stack is easy, leave stack effects as pictured; marker alone is enough for Lab OK — do not require a real console / stdout contract / STRING-LIT reopen / WORD-BL reopen.
- **FAIL:** `[space] FAIL reason=<…>` optional (demo **must avoid**). Prefer not emitting `[space] FAIL` on the happy path. No required FAIL reason this tip — missing args / empty TYPE / negative SPACES stay out of the demo (or document as optional miss picture that still avoids FAIL).
- Storage: fixed demo fixtures / host int echo / short host string slot only. **No** real console VM, no STRING-LIT reopen (leave STRING-LIT.md thin-amended companion only — do not wipe wave13 content), no WORD-BL reopen (optional thin companion amend only), no HOLD / CHAR-PLUS / U-LESS / ABS-NEGATE reopen (leave those primaries untouched), no HERE bump, no arena, no full I/O buffer.
- **Host `SPACE` / `SPACES` / `TYPE` / `EMIT` stay untouched when colliding:** prefer mirrors whenever host names collide. This tip’s Lab greps are `[space] SPACE`, `[space] SPACES`, `[space] TYPE`, and `[space-demo] OK` only (optional `[space] EMIT` welcome) — **never** Lab-grep bare host `SPACE`/`SPACES`/`TYPE`/`EMIT` as this tip’s success when using the Forth mirror path.
- **STRING-LIT / WORD-BL / wave23 marks stay:** do **not** redefine `S"` / `."` / `.(` / `string-s` / `string-dot` / `string-paren`. Do **not** redefine `WORD` / `BL` / `word-mark` / `bl-mark` / `word-parse` / `bl-char`. Do **not** redefine `hold-mark` / `char-plus-mark` / `u-less-mark` / `abs-mark` / `negate-mark`. Output marks are **siblings** beside string-literal / token-pad / pictured-start / char-advance / unsigned-compare / magnitude marks — not a reopen of any.
- Nest with prior abs / uless / hold / charplus / search / number / zero / shift / bit / compare / base / accept / exec / count / within / true / string / word + earlier stubs OK. `dict-reset` unaffected. Assert prior `string-demo` / `word-demo` / `abs-demo` / `uless-demo` / `hold-demo` / `charplus-demo` still OK.
- Still no STRING-LIT / WORD-BL reopen; no full I/O VM; no tip2–5; no linked XT / full arena / full Win/Android Forth VM. Keep wave23 tip1–4 + wave22–13 cites. Leave ABS-NEGATE / HOLD / CHAR-PLUS / U-LESS / ARCH / GAPS / WAVE24-PROPOSAL / COS-PASTE byte-identical (md5s §1 / §8).

## 4. Markers

```
[space] SPACE                              # classic emit-one-blank picture welcome
[space] SPACES [u=<n>] [n=<n>]             # u=/n= optional; classic emit-n-blanks picture welcome
[space] TYPE [addr=<a>] [u=<n>] […content…]  # addr=/u=/content optional; classic type-c-addr-u picture welcome
[space] EMIT [c=<char>] [u=<n>]            # optional this tip; classic emit-one-char picture welcome
[space] FAIL reason=<…>                    # demo avoids
[space-demo] OK
[space-demo] FAIL
```

Lab greps `[space-demo] OK` plus greppable **`[space] SPACE`** + **`[space] SPACES`** + **`[space] TYPE`** (optional `u=`/`n=`/`addr=`/content/`[space] EMIT` welcome). Demo avoids `[space] FAIL`. **Do not** Lab-grep `[string]`/`[word]`/`[abs]`/`[uless]`/`[hold]`/`[char+]`/`[compare]`/`[count]` or `MIN`/`MAX`/`#`/`#>`/`SIGN`/`WORDLIST` as this tip’s success (those stay prior/later tips).

## 5. `space-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; output marks need no dict entries.
2. Ensure **fixed demo fixtures** exist (SPACE needs none beyond the mark; SPACES e.g. `u=3`; TYPE e.g. short host string `hi` / `ok` at a stub addr; optional EMIT char) so classic emit-one-blank / emit-n-blanks / type-c-addr-u pictures are deterministic. Fixtures may live beside (not replacing) STRING-LIT parse fixtures / WORD-BL word-buffer / COUNT counted-string picture / COMPARE string-pair fixtures / HOLD char fixture — document; do **not** require STRING-LIT reopen / WORD-BL reopen / full I/O console VM / HOLD reopen / ABS-NEGATE.
3. Invoke `SPACE` (or **`space-mark`**) → `[space] SPACE`.
4. Invoke `SPACES` (or **`spaces-mark`**) against classic n-blanks fixture → `[space] SPACES` (+ optional `u=` / `n=`).
5. Invoke `TYPE` (or **`type-mark`**) against classic type fixture → `[space] TYPE` (+ optional `addr=` / `u=` / content).
6. Optional (when free): invoke `EMIT` / **`emit-mark`** → `[space] EMIT` (+ optional `c=` / `u=`) — **do not require**.
7. Assert no `[space] FAIL` on the happy path. Assert output marks did **not** require a real console VM / STRING-LIT reopen / WORD-BL reopen / HERE bump / host `SPACE`/`SPACES`/`TYPE`/`EMIT` redefine when using Forth mirrors (marker-only is enough). Assert `S"` / `."` / `.(` / `string-s` / `string-dot` / `string-paren` / `WORD` / `BL` / `word-mark` / `bl-mark` / `hold-mark` / `char-plus-mark` / `u-less-mark` / `abs-mark` / `negate-mark` were **not** redefined. Assert ABS-NEGATE / HOLD / CHAR-PLUS / U-LESS / ARCHITECTURE / IMPLEMENTATION-GAPS / WAVE24-PROPOSAL / WAVE24-COS-PASTE primaries were left untouched. Assert prior `string-demo` / `word-demo` / `abs-demo` / `uless-demo` / `hold-demo` / `charplus-demo` still OK.
8. Prior abs/uless/hold/charplus/search/number/zero/shift/bit/compare/base/accept/exec/count/within/true/string/word demos + trit-math/fold still OK.
9. `[space-demo] OK`.

Required: `[space] SPACE` + `[space] SPACES` + `[space] TYPE`. Optional `u=` / `n=` / `addr=` / content / `[space] EMIT` not required for Lab OK. No STRING-LIT / WORD-BL reopen; no full I/O / console VM; no MIN-MAX / SHARP-SIGN / WORDLIST / DOCS-CITES; no host `SPACE`/`SPACES`/`TYPE`/`EMIT` redefine when using mirrors.

## 6. Thin amend — companions

### `docs/STRING-LIT.md`

- Companions / Status: add `SPACE-SPACES-TYPE.md` (wave24 **1** companion cite); **keep** COMMENT-PARSE / INTERPRET / KERNEL / CREATE-DOES / THROW-CATCH / PARSE-NAME / WORD-BL / CHAR-CHARS / SOURCE-PAD / COUNT / COMPARE cites — do not wipe wave13 STRING-LIT content.
- Purpose / §3 / non-goals: `S"` / `."` / optional `.(` stay string-literal parse marks; `SPACE` / `SPACES` / `TYPE` (+ optional `EMIT`) are sibling **thin output marks** — **not** a STRING-LIT reopen / full I/O / console VM / WORD-BL reopen / counted-string heap. Do not wipe wave13 STRING-LIT content. Stress: SPACE-SPACES-TYPE sibling output marks — **NOT** STRING-LIT reopen; prefer `space-mark` / `spaces-mark` / `type-mark` (+ optional `emit-mark`) whenever host collide; **do not** redefine `S"` / `."` / `.(` / `string-s` / `string-dot` / `string-paren`.
- Non-goals: `SPACE` / `SPACES` / `TYPE` (+ optional `EMIT`) → `docs/SPACE-SPACES-TYPE.md` (wave24 **1**). `S"` / `."` / optional `.(` / `string-s` / `string-dot` / `string-paren` stay on this tip (already landed). WORD-BL stays wave18 **3**. Tip2 MIN-MAX / tip3 SHARP-SIGN / tip4 WORDLIST / tip5 DOCS-CITES still later (wave24 **2–5**).
- Acceptance: Lab smokes `space-demo` (retains `string-demo` + `word-demo` + `compare-demo` + `count-demo` + `abs-demo` + `uless-demo` + `hold-demo` + `charplus-demo`).
- Cite: `docs/SPACE-SPACES-TYPE.md`.

### `docs/KERNEL.md`

- Companions: add `SPACE-SPACES-TYPE.md` (wave24 **1**); **keep** wave23 tip1–4 CHAR-PLUS / HOLD / U-LESS / ABS-NEGATE cites and wave22 tip1–4 LSHIFT-RSHIFT / ZERO-EQUALS / TO-NUMBER / SEARCH-WORDLIST cites and wave21 tip1–4 ACCEPT-REFILL / BASE-HEX / COMPARE / BITWISE cites and wave20 tip1–4 / wave19 tip1–4 / wave18 tip1–4 / wave17 tip1–4 / wave16 DEFER/MARKER/BUFFER/EXIT / wave15 CELL cites.
- Words table: add `SPACE` / `space-mark`, `SPACES` / `spaces-mark`, `TYPE` / `type-mark` (+ optional `EMIT` / `emit-mark`) stubs + `space-demo` (cite tip; Forth mirrors — thin output marks only; prefer mirrors whenever host `SPACE`/`SPACES`/`TYPE`/`EMIT` collide; **do not** redefine `S"` / `."` / `.(` / `string-s` / `string-dot` / `string-paren` / `WORD` / `BL` / `word-mark` / `bl-mark` / `hold-mark` / `char-plus-mark` / `u-less-mark` / `abs-mark` / `negate-mark`; **not** STRING-LIT reopen; **not** full I/O / console VM; **not** WORD-BL reopen; optional `u=` / `n=` / `addr=` / `c=` — classic emit-one-blank / emit-n-blanks / type-c-addr-u / emit-one-char pictures welcome).
- Non-goals: `SPACE` / `SPACES` / `TYPE` (+ optional `EMIT`) thin output marks → `docs/SPACE-SPACES-TYPE.md`. STRING-LIT stays on `STRING-LIT.md` (thin companion amend — do not wipe). WORD-BL stays on `WORD-BL.md` (optional thin companion amend — do not wipe). ABS/NEGATE stay on `ABS-NEGATE.md` (leave primary untouched). HOLD / CHAR+ / U< stay on their wave23 docs (leave primaries untouched). Tip2 MIN-MAX / tip3 SHARP-SIGN / tip4 WORDLIST / tip5 DOCS-CITES still later (wave24 **2–5**).
- Acceptance: Lab smokes `space-demo` (and retains `abs-demo` + `uless-demo` + `hold-demo` + `charplus-demo` + `search-demo` + `number-demo` + `zero-demo` + `shift-demo` + `bit-demo` + `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `string-demo` + `word-demo` + `env-demo` + `allot-demo` + prior demos incl `trit-math-demo` / `fold-demo`).
- Cite: `docs/SPACE-SPACES-TYPE.md`.

### Optional — `docs/WORD-BL.md`

- Light `SPACE-SPACES-TYPE.md` (wave24 **1**) cite; sibling thin output — **NOT** WORD-BL reopen; prefer mirrors; do not redefine `WORD`/`BL`/`word-mark`/`bl-mark`. Retain `word-demo`.

### Optional — `docs/HOST-PARITY.md`

- Light `SPACE-SPACES-TYPE.md` (wave24 **1**) cite; `space-demo CONTRACT` welcome — **not** full Win/Android Forth VM / console I/O port. Prefer mirrors.

Do **not** wipe wave23 tip1–4 / wave22–13 prior content. Amends: STRING-LIT + KERNEL (+ optional WORD-BL / HOST-PARITY) only. **Leave untouched:** ABS-NEGATE (`caa1e0d524c18590039193b7b74d074b`/30661); HOLD (`dcc8a16c5f2999dd3aba7d828383f475`/30969); CHAR-PLUS (`bb879e6fe97f8f8ebd25418d43b43751`/30666); U-LESS (`1f17242555fde002eaf2e7dbabadc5d6`/30870); ARCH (`2575a64f907756ab5ef78afa485f9a16`); GAPS (`1f2a5d9969d394e716dcb25c6234cf03`); WAVE24-PROPOSAL (`1cceddbdf85d16eef46863f03aec8c27`); COS-PASTE (`df576fdb220979cfa9e2f7dbf4c1c416`). Tip5 after 1–4 PASS. Skip 2DUP-FAMILY + ABORT" polish.

## 7. Non-goals

- Redefining / aliasing / shadowing host `SPACE` / `SPACES` / `TYPE` / `EMIT` when colliding — prefer Forth mirrors `space-mark` / `spaces-mark` / `type-mark` / optional `emit-mark`
- Redefining `S"` / `."` / `.(` / `string-s` / `string-dot` / `string-paren` (STRING-LIT stays — **not** a STRING-LIT reopen; thin companion amend only)
- Redefining `WORD` / `BL` / `word-mark` / `bl-mark` / `word-parse` / `bl-char` / host `WORDS` (WORD-BL stays — **not** a WORD-BL reopen; optional thin companion cite)
- Redefining `hold-mark` / `char-plus-mark` / `u-less-mark` / `abs-mark` / `negate-mark` (wave23 tip1–4 stay — leave those primaries untouched)
- Full I/O / console VM / live stdout contract / real terminal rewrite / BLOCK screen I/O
- `STRING-LIT` reopen (wave13 **4** — already stubbed; thin companion amend only — pairs without reopen)
- `WORD-BL` reopen (wave18 **3** — already stubbed; optional thin companion amend only)
- `MIN` / `MAX` marks (tip2 MIN-MAX — still deferred)
- Thin pictured continue `#` / `#>` / `SIGN` / optional `#S` (tip3 SHARP-SIGN — still deferred; **not** HOLD reopen; do not bump HERE stub `$1000`)
- `WORDLIST` / `FORTH-WORDLIST` marks (tip4 WORDLIST — still deferred)
- Docs cites pass (wave24 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `HOLD` / `CHAR-PLUS` / `U-LESS` / `ABS-NEGATE` reopen (wave23 **1–4** — already stubbed; leave primaries untouched)
- `COMPARE` / `COUNT` / `ACCEPT-REFILL` / `SOURCE-PAD` reopen (wave19–21 — already stubbed; keep cites)
- Real DOES> XT / real branch XT / LEAVE jump / full arena / linked XT / real STATE cell
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live — **skip 2DUP-FAMILY**)
- `ABORT"` polish (already optional-wired inside `throw-demo` — **skip**)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/SPACE-SPACES-TYPE.md` present (Research byte-copy OK); `STRING-LIT.md` + `KERNEL.md` thin amends present (+ optional WORD-BL / HOST-PARITY); prior cites retained; host `SPACE`/`SPACES`/`TYPE`/`EMIT` untouched via mirrors; `S"`/`."`/`.(`/`string-s`/`string-dot`/`string-paren` / `WORD`/`BL`/`word-mark`/`bl-mark` / `hold-mark`/`char-plus-mark`/`u-less-mark`/`abs-mark`/`negate-mark` **not** redefined; ABS-NEGATE.md untouched (`caa1e0d524c18590039193b7b74d074b` / 30661); HOLD.md untouched (`dcc8a16c5f2999dd3aba7d828383f475` / 30969); CHAR-PLUS.md untouched (`bb879e6fe97f8f8ebd25418d43b43751` / 30666); U-LESS.md untouched (`1f17242555fde002eaf2e7dbabadc5d6` / 30870); ARCHITECTURE (`2575a64f907756ab5ef78afa485f9a16` / 18387) + GAPS (`1f2a5d9969d394e716dcb25c6234cf03` / 63563) + WAVE24-PROPOSAL (`1cceddbdf85d16eef46863f03aec8c27`) + COS-PASTE (`df576fdb220979cfa9e2f7dbf4c1c416`) untouched.
2. `space-demo` → OK (markers §4; `[space] SPACE` + `[space] SPACES` + `[space] TYPE` greppable; optional `u=`/`n=`/`addr=`/`c=`/`[space] EMIT` welcome — classic emit-one-blank / emit-n-blanks / type-c-addr-u pictures on stub fixtures; no FAIL on happy path; no STRING-LIT reopen / WORD-BL reopen / full I/O console VM / HOLD reopen / ABS-NEGATE reopen / MIN-MAX / SHARP-SIGN / WORDLIST; no `string-s`/`string-dot`/`word-mark`/`bl-mark`/`hold-mark`/`char-plus-mark`/`u-less-mark`/`abs-mark`/`negate-mark` redefine). Prior `abs-demo` + `uless-demo` + `hold-demo` + `charplus-demo` + `search-demo` + `number-demo` + `zero-demo` + `shift-demo` + `bit-demo` + `compare-demo` + `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `string-demo` + `word-demo` + `env-demo` + `allot-demo` + earlier demos incl `trit-math-demo` / `fold-demo` still OK.
3. Regression green (wave23 tip1–5 + wave22–14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `space-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**)
- `docs/STRING-LIT.md` (wave13 **4** — `S"` / `."` / optional `.(` string-literal companion; SPACE-SPACES-TYPE sibling thin output marks — **not** a STRING-LIT reopen; prefer `space-mark`/`spaces-mark`/`type-mark`; do not redefine `S"`/`."`/`.(`/`string-s`/`string-dot`/`string-paren`)
- `docs/WORD-BL.md` (wave18 **3**, optional — `WORD` / `BL` token/pad companion; SPACE-SPACES-TYPE sibling thin output — **not** a WORD-BL reopen; prefer mirrors; do not redefine `WORD`/`BL`/`word-mark`/`bl-mark`)
- `docs/HOST-PARITY.md` (wave8 **4**, optional — `space-demo` CONTRACT parity welcome)
- `docs/ABS-NEGATE.md` (wave23 **4** — prior tip; keep cites; leave primary untouched this tip; do not redefine `abs-mark`/`negate-mark`)
- `docs/U-LESS.md` (wave23 **3** — prior tip; keep cites; leave primary untouched this tip; do not redefine `u-less-mark`)
- `docs/HOLD.md` (wave23 **2** — prior tip; keep cites; leave primary untouched this tip; do not redefine `hold-mark`; HERE stub `$1000` untouched)
- `docs/CHAR-PLUS.md` (wave23 **1** — prior tip; keep cites; leave primary untouched this tip; do not redefine `char-plus-mark`)
- `docs/COMPARE.md` / `docs/COUNT.md` / `docs/ACCEPT-REFILL.md` / `docs/SOURCE-PAD.md` / wave20 EXECUTE / wave19 CHAR-CHARS (prior; keep cites)
- `forth/tritium/kernel.fs` (space-/spaces-/type-/optional emit-mark only — do not redefine host SPACE/SPACES/TYPE/EMIT when colliding; do not redefine string-* / word-*/bl-* / wave23 marks)
- ANS Forth `SPACE`/`SPACES`/`TYPE`/`EMIT` (thin output marks only — prefer mirrors; not STRING-LIT reopen / full I/O)
- Explicit deferral: WAVE23-PROPOSAL + WAVE24-PROPOSAL (SPACE/SPACES/TYPE thin output marks)
- Base tip: `2b8acfc1` / `2b8acfc1ea46c5d4676eb969c8cc1be30d89d0cc` (#111 wave23 tip5 DOCS-CITES PASS)
- Wave24 proposal: `/workspace/tritium-research-docs/WAVE24-PROPOSAL.md`

## 10. Shipper implementation notes (Linux SoT)

These notes are normative for Shipper drafting on branch `shipper/space-spaces-type-w24` from base `2b8acfc1ea46c5d4676eb969c8cc1be30d89d0cc`. They do **not** authorize merge, push, Mango touch, opaque-weight ML, or reopening closed wave13–23 tips / wave23 tip1–4 primaries / WAVE24-PROPOSAL.

### 10.1 Binding order

1. Prefer Forth mirrors **`space-mark` / `spaces-mark` / `type-mark`** first (+ optional **`emit-mark`**). Lab greps do not require bare ANS names `SPACE`/`SPACES`/`TYPE`/`EMIT` to be bound when the mirrors print `[space] SPACE` / `[space] SPACES` / `[space] TYPE` (optional `[space] EMIT`).
2. If bare `SPACE`/`SPACES`/`TYPE` (optional `EMIT`) are free on the Linux REPL load path, Shipper **may** bind them as thin aliases that emit the same markers — still **must not** redefine `S"`/`."`/`.(`/`string-s`/`string-dot`/`string-paren` / `WORD`/`BL`/`word-mark`/`bl-mark` / `hold-mark`/`char-plus-mark`/`u-less-mark`/`abs-mark`/`negate-mark`.
3. Never alias `SPACE`/`SPACES`/`TYPE`/`EMIT` onto `string-dot` / `string-s` / `word-parse` / `bl-char` / `hold-mark` / `abs-mark`. Output marks ≠ string-literal parse / token-pad / pictured start / magnitude.

### 10.2 Classic pictures (emit-one-blank / emit-n-blanks / type-c-addr-u)

Document fixed fixtures. Recommended:

| Field | Example | Notes |
|-------|---------|-------|
| SPACE | (none) | Classic emit-one-blank → `[space] SPACE` |
| SPACES u | `3` | Classic emit-n-blanks → `[space] SPACES` (+ optional `u=3`) |
| TYPE fixture | `hi` / `ok` | Classic type-c-addr-u → `[space] TYPE` (+ optional `u=2` / `addr=0`) |
| optional EMIT char | `A` / `*` / blank | Welcome when free — not required |
| optional zero SPACES | `u=0` | Welcome when free — not required |

Shipper may use other small fixtures — document which. Lab greps `[space] SPACE` + `[space] SPACES` + `[space] TYPE` regardless of which fixtures are chosen, as long as the three marker lines are greppable and FAIL is avoided. **Do not** implement TYPE by reopening STRING-LIT `."` or by redefining `string-dot`. **Do not** implement SPACE by reopening WORD-BL `BL` / `bl-char`.

### 10.3 What Lab greps / does not grep

**Required:** `[space-demo] OK` + `[space] SPACE` + `[space] SPACES` + `[space] TYPE`. **Optional welcome:** `u=` / `n=` / `addr=` / content / `[space] EMIT` / `c=`; `space-demo CONTRACT`. **Must NOT Lab-grep as success:** `[string]`/`string-s`/`string-dot`/`string-paren`; `[word]`/`word-mark`/`bl-mark`/`word-parse`/`bl-char`; `[abs]`/`abs-mark`/`negate-mark`; `[uless]`/`u-less-mark`; `[hold]`/`hold-mark`; `[char+]`/`char-plus-mark`; `[compare]`/`compare-mark`; `[count]`/`count-mark`; `MIN`/`MAX`; `#`/`#>`/`SIGN`; `WORDLIST`; `[search]`/`[shift]`/`[bit]`/`[number]`/`[base]`/`[cell]`.

### 10.4 Regression + companions + non-goals

Retain green: `abs-demo` / `uless-demo` / `hold-demo` / `charplus-demo` / `search-demo` / `number-demo` / `zero-demo` / `shift-demo` / `bit-demo` / `compare-demo` / `base-demo` / `accept-demo` / `exec-demo` / `count-demo` / `within-demo` / `true-demo` / `string-demo` / `word-demo` / `env-demo` / `char-demo` / `cell-demo` / `allot-demo` / `kernel-demo` / `trit-math-demo` / `fold-demo` (+ earlier). Required thin: STRING-LIT + KERNEL. Optional: WORD-BL / HOST-PARITY. **Untouched:** ARCHITECTURE / IMPLEMENTATION-GAPS / WAVE24-PROPOSAL / WAVE24-COS-PASTE / ABS-NEGATE primary / HOLD primary / CHAR-PLUS primary / U-LESS primary / wave23 tip1–4 primaries.

Do **not**: redefine string-s/string-dot/string-paren / word-mark/bl-mark / hold-mark/char-plus-mark/u-less-mark/abs-mark/negate-mark; reopen STRING-LIT / WORD-BL / HOLD / CHAR-PLUS / U-LESS / ABS-NEGATE; land full I/O console VM / MIN-MAX / SHARP-SIGN / WORDLIST / tip5 DOCS-CITES; amend ARCH/GAPS / ABS-NEGATE.md / HOLD.md / CHAR-PLUS.md / U-LESS.md / WAVE24-PROPOSAL; tip Shipper / push / merge / touch Mango; redefine host 2dup/2drop/2swap; polish ABORT".

SPACE-SPACES-TYPE = thin **output marks** only. Sibling to STRING-LIT / WORD-BL — **not** a reopen. Prefer `space-mark` / `spaces-mark` / `type-mark` (+ optional `emit-mark`). Classic: SPACE → one blank mark; SPACES → n blanks mark; TYPE → type-c-addr-u mark. Preferred emit: `[space] SPACE` + `[space] SPACES u=3` + `[space] TYPE u=2` — minimum the three markers — close `[space-demo] OK`.

Research drafts under `/workspace/tritium-research-docs/`. Shipper byte-copies into repo `docs/` on `shipper/space-spaces-type-w24` only. Parent tips Shipper after. Research does **not** tip Shipper / push / merge / touch Mango.

Still deferred: MIN/MAX (tip2); `#`/`#>`/`SIGN` (tip3); WORDLIST (tip4); DOCS-CITES (tip5); STRING-LIT/WORD-BL reopen; full I/O VM; wave23 reopen; full Win/Android Forth VM; 2DUP-FAMILY; ABORT" polish; Mango.

Acceptance one-liner: `space-demo` prints greppable `[space] SPACE` + `[space] SPACES` + `[space] TYPE` and ends `[space-demo] OK`; prior string/word/abs/uless/hold/charplus demos still OK; mirrors not redefined; Linux SoT; no merge.

Paste anchors: WAVE24-PROPOSAL.md § Tip 1; base `2b8acfc1ea46c5d4676eb969c8cc1be30d89d0cc`; branch `shipper/space-spaces-type-w24`; handoff `WAVE24-TIP1-HANDOFF.md` (= `TIP1-HANDOFF.md`).

### 10.5 Disambiguation (Shipper)

| Surface | Tip | This tip? |
|---------|-----|-----------|
| `SPACE` / `SPACES` / `TYPE` / optional `EMIT` (+ mirrors) | wave24 **1** | **YES** |
| `S"` / `."` / `.(` / string-* | wave13 **4** | NO — STRING-LIT thin amend only |
| `WORD` / `BL` / word-*/bl-* | wave18 **3** | NO — optional WORD-BL thin cite |
| wave23 `abs`/`u-less`/`hold`/`char-plus` marks | wave23 **1–4** | NO — leave primaries untouched |
| MIN/MAX / `#`/`#>`/`SIGN` / WORDLIST / DOCS-CITES | wave24 **2–5** | NO |

Output marks ≠ string-literal parse ≠ token/pad ≠ pictured start ≠ magnitude. Leave ABS-NEGATE / HOLD / CHAR-PLUS / U-LESS / ARCH / GAPS / WAVE24-PROPOSAL / COS-PASTE byte-identical (md5s §8). HERE stub `$1000` untouched. Docs ONLY under `/workspace/tritium-research-docs/`. Skip 2DUP-FAMILY + ABORT" polish. Parent tips Shipper after.
