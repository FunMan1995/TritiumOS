# WORDLIST — thin vocab create/query (`WORDLIST` / `FORTH-WORDLIST`, optional `GET-CURRENT` / `SET-CURRENT`) + `wordlist-demo`

**Status:** Shipper-ready stub spec (wave24 item **4**)
**Canonical brief:** ANS-shaped `WORDLIST` / `FORTH-WORDLIST` (thin vocab create/query beside wave22 SEARCH-WORDLIST); `docs/SEARCH-WORDLIST.md` (wave22 **4** — **not** runtime reopen; do not redefine `search-wl-mark`/`SEARCH-WORDLIST`/`search-demo`); `docs/FIND.md` (wave18 **2** — **not** FIND reopen; do not redefine `find`/`findentry`/`entry-find`/`find-xt`/`find-mark`); `docs/KERNEL.md` (wave7 **5**); optional `docs/WORDS-VOCAB.md` / `docs/HOST-PARITY.md`; WAVE19–24 deferral closed as **thin vocab create/query only** (not FIND reopen; **not** SEARCH-WORDLIST runtime; **not** linked dict; **not** DEFINITIONS / search-order stack; prefer mirrors **`wordlist-mark` / `forth-wordlist-mark`** (+ optional get/set-current-mark) whenever host collide). **CRITICAL:** do **not** redefine search-wl-/find-/words surfaces; **do not** bump HERE `$1000`; do **not** redefine sharp-/hold-/min-/max-/space-/abs-/u-less-/to-number-/base-/char-plus marks. Kernel `MAX-*` still NOT ANS MAX. **Prefer leave `SHARP-SIGN.md` untouched** (md5 `51909b17e780c5f0096567d75f5d20e2` / 30461).
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `wordlist.fs` / `vocab.fs`); Linux host REPL; prefer **`wordlist-mark` / `forth-wordlist-mark`** (+ optional **`get-current-mark` / `set-current-mark`**) whenever host collide; **CRITICAL — do not** redefine `SEARCH-WORDLIST`/`search-wl-mark`/`search-demo` / host `find`/`findentry`/`entry-find`/`find-xt`/`find-mark` / `WORDS`/`.words`/`words-demo` / `_here`/`HERE`/`here-at` (HERE `$1000` stays) / sharp-/hold-/min-/max-/space-/abs-/u-less-/to-number-/base-/char-plus marks; optional GET/SET-CURRENT when free — **do not require** DEFINITIONS / search-order / linked dict
**Companions:** `docs/SEARCH-WORDLIST.md` (thin amend this tip), `docs/FIND.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip); optional light cite `docs/WORDS-VOCAB.md` / `docs/HOST-PARITY.md`; **prefer leave `docs/SHARP-SIGN.md` untouched** (tip3 primary)
**Base tip SHA:** `7db5fbdd` (wave24 tip3 SHARP-SIGN PASS / #114 `shipper/sharp-sign-w24`) / full `7db5fbdd67ef8f49e955aadcb28ec02c05caf85a`

## 1. Purpose

WAVE11 tip **3** landed flat `WORDS` (`docs/WORDS-VOCAB.md`). WAVE18 tip **2** landed ANS-ish `FIND` (`docs/FIND.md`; `find-xt`/`find-mark`; host find untouched). WAVE22 tip **4** landed thin `SEARCH-WORDLIST` (`docs/SEARCH-WORDLIST.md`; `search-wl-mark` — **not** FIND reopen; **not** linked dict). WAVE19 tip **3** landed thin `ENVIRONMENT?` (not wordlist rewrite). WAVE24 tip **1** SPACE + tip **2** MIN-MAX + tip **3** SHARP-SIGN landed (leave those primaries untouched — **prefer leave SHARP-SIGN.md untouched**). WAVE18–24 deferred `WORDLIST`/`FORTH-WORDLIST` as thin vocab create/query beside SEARCH-WORDLIST (**not** FIND reopen; **not** SEARCH-WORDLIST runtime; **not** linked dict; **not** DEFINITIONS / search-order; **must not** redefine `search-wl-mark`/`find-mark`/`WORDS`). This tip lands **stub** thin create/query only: `WORDLIST`/`wordlist-mark` → `[wordlist] WORDLIST` (+ optional `wid=` — new-wordlist id); `FORTH-WORDLIST`/`forth-wordlist-mark` → `[wordlist] FORTH-WORDLIST` (+ optional `wid=` — forth-wordlist id). Optional `GET-CURRENT`/`SET-CURRENT` (mirrors `get-current-mark`/`set-current-mark`) when free — **do not require** DEFINITIONS / search-order / linked dict. Smoke via **`wordlist-demo`**. Prefer mirrors whenever host collide. **CRITICAL:** do **not** redefine `SEARCH-WORDLIST`/`search-wl-mark`/`search-demo` / `find`/`findentry`/`entry-find`/`find-xt`/`find-mark` / `WORDS`/`.words`/`words-demo`; do **not** bump `_here`/`HERE`/`here-at` (`$1000` stays). Pairs with SEARCH-WORDLIST + FIND + WORDS **without** FIND reopen / SEARCH-WORDLIST runtime / linked dict / DEFINITIONS / SHARP-SIGN/MIN-MAX/SPACE/HOLD reopen. **Not** tip5 DOCS-CITES. **Leave untouched (md5s §8):** SHARP-SIGN / MIN-MAX / SPACE / HOLD / CHAR-PLUS / ABS / U-LESS / ARCH / GAPS / WAVE24-PROPOSAL / COS-PASTE. SEARCH-WORDLIST + FIND + KERNEL thin-amended this tip. Optional WORDS-VOCAB + HOST-PARITY thin cites welcome.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `WORDLIST` / `wordlist-mark` | `( -- wid )` *or* `( -- )` with fixed demo fixture | Thin new-wordlist create/query mark; print `[wordlist] WORDLIST` (+ optional `wid=<n>`); classic new-wordlist id picture welcome |
| `FORTH-WORDLIST` / `forth-wordlist-mark` | `( -- wid )` *or* `( -- )` with fixed demo fixture | Thin forth-wordlist id query mark; print `[wordlist] FORTH-WORDLIST` (+ optional `wid=<n>`); classic forth-wordlist id picture welcome |
| `GET-CURRENT` / `get-current-mark` (optional) | `( -- wid )` *or* `( -- )` | Optional current-wordlist query mark; print `[wordlist] GET-CURRENT` (+ optional `wid=`) when free — **do not require** |
| `SET-CURRENT` / `set-current-mark` (optional) | `( wid -- )` *or* `( -- )` | Optional current-wordlist set mark; print `[wordlist] SET-CURRENT` (+ optional `wid=`) when free — **do not require** DEFINITIONS / search-order stack |
| `wordlist-demo` | `( -- )` | See §5 |

Host note: bind bare `WORDLIST`/`FORTH-WORDLIST`/`GET-CURRENT`/`SET-CURRENT` **only if** free; prefer mirrors **`wordlist-mark` / `forth-wordlist-mark`** (+ optional get/set-current-mark). **CRITICAL:** do **not** redefine `SEARCH-WORDLIST`/`search-wl-mark`/`search-demo` / host `find`/`findentry`/`entry-find`/`find-xt`/`find-mark` / `WORDS`/`.words`/`words-demo`; do **not** bump `_here`/`HERE`/`here-at` (`$1000` stays). Do **not** redefine sharp-/hold-/min-/max-/space-/abs-/u-less-/to-number-/base-/char-plus marks. Optional `wid=` is host int echo only — not linked dict / FIND reopen / SEARCH-WORDLIST runtime / DEFINITIONS. Prefer **fixed demo wordlist id fixtures**. Optional GET/SET-CURRENT when free — **do not require**. Demo **must** exercise both required surfaces so Lab greps `[wordlist] WORDLIST` + `[wordlist] FORTH-WORDLIST` + `[wordlist-demo] OK`.

## 3. Stub semantics

- **`WORDLIST` / `wordlist-mark`:** classic ANS `( -- wid )` create empty wordlist — **marker only**: print `[wordlist] WORDLIST` (+ optional `wid=`). Classic fixture: new-wordlist id picture (e.g. stub wid=`1` / `2` / documented host int — document). **Does not** rewrite linked dict, implement search-order stack / DEFINITIONS, reopen FIND / SEARCH-WORDLIST runtime, or bump HERE. **Does not** redefine `search-wl-mark`/`SEARCH-WORDLIST` / `find`/`findentry`/`entry-find`/`find-xt`/`find-mark` / `WORDS`/`.words`/`words-demo` / `_here`/`HERE`/`here-at`.
- **`FORTH-WORDLIST` / `forth-wordlist-mark`:** classic ANS `( -- wid )` return forth wordlist id — marker only: print `[wordlist] FORTH-WORDLIST` (+ optional `wid=`). Classic fixture: forth-wordlist id (e.g. stub wid=`0` / documented host forth-wid — document). **Does not** open linked dict / search-order stack, reopen FIND / SEARCH-WORDLIST runtime, or bump HERE.
- **Optional `GET-CURRENT` / `get-current-mark`:** print `[wordlist] GET-CURRENT` (+ optional `wid=`) when free — **do not require**. Does not require DEFINITIONS / search-order stack / linked dict / FIND reopen / SEARCH-WORDLIST runtime / HERE bump.
- **Optional `SET-CURRENT` / `set-current-mark`:** print `[wordlist] SET-CURRENT` (+ optional `wid=`) when free — **do not require**. Does **not** implement DEFINITIONS / search-order stack / linked dict. Does not reopen FIND / SEARCH-WORDLIST runtime / bump HERE.
- **Fixed demo wordlist fixtures (document):**
  - **`WORDLIST` new-wordlist id (required):** stub wid picture — Lab greps `[wordlist] WORDLIST`.
  - **`FORTH-WORDLIST` forth-wordlist id (required):** stub forth-wid picture — Lab greps `[wordlist] FORTH-WORDLIST`.
  - **Optional `GET-CURRENT` / `SET-CURRENT`:** welcome when free — do **not** require / promote to DEFINITIONS / search-order stack / linked dict / FIND reopen / SEARCH-WORDLIST runtime.
  - **Optional echo:** `wid=` — greppable two required markers enough for Lab OK.
- **Optional push:** marker alone enough — do not require real wordlist stack / FIND reopen / SEARCH-WORDLIST runtime / linked dict / DEFINITIONS / HERE bump.
- **FAIL:** `[wordlist] FAIL reason=<…>` optional (demo **must avoid**). Prefer not emitting FAIL on the happy path.
- Storage: fixed demo wordlist id fixtures / host int echo only. **No** linked dict / search-order / DEFINITIONS; no FIND reopen; no SEARCH-WORDLIST runtime; no HERE bump; no SHARP-SIGN / MIN-MAX / SPACE / HOLD / ABS / U-LESS / CHAR-PLUS reopen; no arena; no full ANS ENVIRONMENT? reopen; **no** touch to kernel `MAX-*`.
- **HERE stub base stays `$1000`:** Lab greps `[wordlist] WORDLIST` + `[wordlist] FORTH-WORDLIST` + `[wordlist-demo] OK` only — **never** Lab-grep `_here`/`HERE`/`here-at`/`[allot]` as WORDLIST success.
- **SEARCH-WORDLIST stays (thin companion amend only):** do **not** redefine `SEARCH-WORDLIST`/`search-wl-mark`/`search-demo`. Sibling **create/query** beside vocab-**search** — **not** runtime reopen. Do not Lab-grep `[search]` as this tip’s success.
- **FIND stays (thin companion amend only):** do **not** redefine `find`/`findentry`/`entry-find`/`find-xt`/`find-mark`. Sibling create/query — **not** FIND reopen. Do not Lab-grep `[find]`/`find-xt`/`find-mark` as this tip’s success.
- **WORDS-VOCAB stays (optional thin companion):** do **not** redefine `WORDS`/`.words`/`words-demo`. Flat list stays list-only — **not** WORDS reopen / linked dict.
- **SHARP-SIGN / MIN-MAX / SPACE / HOLD / ABS / U-LESS / CHAR-PLUS stay untouched primaries:** do **not** redefine their mirrors. **Prefer leave `SHARP-SIGN.md` untouched** (`51909b17e780c5f0096567d75f5d20e2`/30461). Locked md5s §8.
- Nest with prior sharp/minmax/space/abs/uless/hold/charplus/search/find/words/number/zero/shift/bit/compare/base + earlier stubs OK. Assert prior `search-demo` / `find-demo` / `words-demo` / `sharp-demo` / `minmax-demo` / `space-demo` still OK.
- Still no FIND reopen; no SEARCH-WORDLIST runtime; no linked dict; no DEFINITIONS; no tip5; no linked XT / full arena / full Win/Android Forth VM. SEARCH-WORDLIST + FIND + KERNEL thin-amended only (new md5s in tip summary). Optional WORDS-VOCAB + HOST-PARITY.

## 4. Markers

```
[wordlist] WORDLIST [wid=<n>]              # wid= optional; classic new-wordlist id picture welcome
[wordlist] FORTH-WORDLIST [wid=<n>]        # wid= optional; classic forth-wordlist id picture welcome
[wordlist] GET-CURRENT [wid=<n>]           # optional; welcome when free — do not require
[wordlist] SET-CURRENT [wid=<n>]           # optional; welcome when free — do not require DEFINITIONS / search-order
[wordlist] FAIL reason=<…>                 # demo avoids
[wordlist-demo] OK
[wordlist-demo] FAIL
```

Lab greps `[wordlist-demo] OK` plus greppable **`[wordlist] WORDLIST`** + **`[wordlist] FORTH-WORDLIST`** (optional `wid=` welcome). Demo avoids `[wordlist] FAIL`. Optional GET-CURRENT / SET-CURRENT when free — **do not require**. **Do not** Lab-grep `_here`/`HERE`/`here-at`/`[allot]` / `[search]`/`search-wl-mark` / `[find]`/`find-xt`/`find-mark`/host find* / `[words]`/`WORDS` / `[sharp]`*/`[hold]`*/`[minmax]`*/`[space]`*/`[abs]`*/`[uless]`*/`[char+]`*/`[number]`*/`[base]`*/`[zero]`/`[shift]`/`[bit]` / kernel `MAX-*` / DOCS-CITES as this tip’s success.

## 5. `wordlist-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; vocab create/query marks need no dict entries beyond fixed host fixtures.
2. Ensure **fixed demo wordlist id fixtures** exist so classic pictures are deterministic:
   - `WORDLIST` new-wordlist id fixture (e.g. stub wid=`1` / `2` / documented host int — document).
   - `FORTH-WORDLIST` forth-wordlist id fixture (e.g. stub wid=`0` / documented forth-wid — document).
   Fixtures may live beside (not replacing) SEARCH-WORDLIST hit picture / FIND mark picture / WORDS list / TICK stub xt-id / ENVIRONMENT-QUERY stub / ALLOT-HERE stub pointer / SHARP pictured continue / MIN-MAX signed fixtures — document; do **not** require FIND reopen / SEARCH-WORDLIST runtime / linked dict / DEFINITIONS / search-order stack / HERE bump / SHARP-SIGN reopen / MIN-MAX reopen / SPACE reopen.
3. Invoke `WORDLIST` (or **`wordlist-mark`**) against classic new-wordlist fixture → `[wordlist] WORDLIST` (+ optional `wid=`).
4. Invoke `FORTH-WORDLIST` (or **`forth-wordlist-mark`**) against classic forth-wordlist fixture → `[wordlist] FORTH-WORDLIST` (+ optional `wid=`).
5. Optional (when free): invoke `GET-CURRENT` / `get-current-mark` → `[wordlist] GET-CURRENT` (+ optional `wid=`) — **do not require**.
6. Optional (when free): invoke `SET-CURRENT` / `set-current-mark` → `[wordlist] SET-CURRENT` (+ optional `wid=`) — **do not require** DEFINITIONS / search-order stack.
7. Assert no `[wordlist] FAIL`. Assert no FIND reopen / SEARCH-WORDLIST runtime / linked dict / DEFINITIONS / HERE bump / SHARP-SIGN/MIN-MAX/SPACE/HOLD reopen / full ANS ENVIRONMENT? reopen. Assert host WORDLIST/FORTH-WORDLIST/GET/SET-CURRENT not redefined via mirrors. Assert `search-wl-mark`/`SEARCH-WORDLIST`/`search-demo` / find*/find-xt/find-mark / `WORDS`/`.words`/`words-demo` **not** redefined. Assert `_here`/`HERE`/`here-at` **not** bumped (`$1000` stays). Assert sharp-/hold-/min-/max-/space-/abs-/u-less-/to-number-/base-/char-plus marks **not** redefined. Assert kernel `MAX-*` + locked primaries (§8) untouched — **prefer SHARP-SIGN.md untouched**. Assert prior `search-demo` / `find-demo` / `words-demo` / `sharp-demo` / `minmax-demo` / `space-demo` / `hold-demo` still OK.
8. Prior sharp/minmax/space/abs/uless/hold/charplus/search/find/words/number/zero/shift/bit/compare/base + earlier demos still OK.
9. `[wordlist-demo] OK`.

Required markers: `[wordlist] WORDLIST` + `[wordlist] FORTH-WORDLIST`. Optional `wid=` echo is not required for Lab OK when both required markers are greppable. Optional `GET-CURRENT`/`SET-CURRENT` not required. No FIND reopen. No SEARCH-WORDLIST runtime reopen. No linked dict. No DEFINITIONS deep rewrite. No HERE stub base bump. No tip5 DOCS-CITES. No full ANS ENVIRONMENT? table reopen.

## 6. Thin amend — companions

### `docs/SEARCH-WORDLIST.md`

- Companions / Status: add `WORDLIST.md` (wave24 **4**); **keep** FIND / WORDS-VOCAB / KERNEL / SYNONYM-ALIAS / TICK / EXECUTE / INTERPRET / HOST-PARITY cites — do not wipe SEARCH-WORDLIST primary content.
- Purpose / §3 / non-goals: SEARCH-WORDLIST stays thin vocab-**search** mark; `WORDLIST`/`FORTH-WORDLIST` are sibling **thin vocab create/query** marks — **NOT** SEARCH-WORDLIST runtime reopen / FIND reopen / linked dict / DEFINITIONS deep rewrite. Stress: prefer `wordlist-mark`/`forth-wordlist-mark`; **do not** redefine `SEARCH-WORDLIST`/`search-wl-mark`/`search-demo`; HERE `$1000` untouched.
- Non-goals: `WORDLIST`/`FORTH-WORDLIST` → `docs/WORDLIST.md` (wave24 **4**). SEARCH-WORDLIST stays landed. Tip5 later.
- Acceptance: Lab smokes `wordlist-demo` (retains `search-demo` + `find-demo` + `words-demo` + `sharp-demo` + `minmax-demo` + `space-demo` + `hold-demo` + `number-demo`). Cite: `docs/WORDLIST.md`.

### `docs/FIND.md`

- Companions / Status: add `WORDLIST.md` (wave24 **4**); **keep** SEARCH-WORDLIST / KERNEL / WORDS-VOCAB / SYNONYM-ALIAS / INTERPRET / TICK / ENVIRONMENT-QUERY / EXECUTE cites — do not wipe FIND content.
- Purpose / §3 / non-goals: FIND stays ANS-ish find mark via `find-xt`/`find-mark`; host `find`/`findentry`/`entry-find` stay; `WORDLIST`/`FORTH-WORDLIST` sibling thin vocab create/query — **NOT** FIND reopen / SEARCH-WORDLIST runtime / linked dict / DEFINITIONS. Prefer mirrors; **do not** redefine `find`/`findentry`/`entry-find`/`find-xt`/`find-mark`. HERE `$1000` untouched.
- Non-goals: → `docs/WORDLIST.md` (wave24 **4**). Tip5 later. Acceptance: Lab smokes `wordlist-demo` (retains `find-demo` + `search-demo` + `words-demo` + `tick-demo`). Cite: `docs/WORDLIST.md`.

### `docs/KERNEL.md`

- Companions: add `WORDLIST.md` (wave24 **4**); **keep** wave24 tip1–3 SPACE + MIN-MAX + SHARP-SIGN cites and wave23 tip1–4 + wave22 tip1–4 + wave21 tip1–4 + wave20–15 cites.
- Words table: add `WORDLIST`/`wordlist-mark` + `FORTH-WORDLIST`/`forth-wordlist-mark` (+ optional GET/SET-CURRENT mirrors) + `wordlist-demo` (mirrors; thin create/query only; **CRITICAL:** do not redefine `search-wl-mark`/`SEARCH-WORDLIST` / find*/find-xt/find-mark / `WORDS`/`.words`/`words-demo`; do not bump `_here`/`HERE`/`here-at` — `$1000` stays; do not redefine sharp-/hold-/min-/max-/space-/abs-/u-less-/to-number-/base-/char-plus marks; kernel `MAX-*` ≠ ANS MAX; **not** FIND reopen / SEARCH-WORDLIST runtime / linked dict / DEFINITIONS; optional `wid=`; optional GET/SET-CURRENT when free — do not require).
- Non-goals: → `docs/WORDLIST.md`. SEARCH-WORDLIST / FIND thin amends only. HERE `$1000` untouched. SHARP-SIGN / MIN-MAX / SPACE primaries untouched. Tip5 later.
- Acceptance: Lab smokes `wordlist-demo` (retains sharp/minmax/space/abs/uless/hold/charplus/search/find/words/number/zero/shift/bit/compare/base + prior incl trit-math/fold).
- Cite: `docs/WORDLIST.md`.

### Optional — `docs/WORDS-VOCAB.md` / `docs/HOST-PARITY.md`

- WORDS-VOCAB: optional vocab sibling cite — `WORDLIST`/`FORTH-WORDLIST` thin create/query beside flat WORDS list — **NOT** WORDS reopen / linked dict; do **not** wipe `WORDS`/`.words`/`words-demo`. Retain `words-demo`. Prefer `wordlist-mark`/`forth-wordlist-mark`.
- HOST-PARITY: `wordlist-demo CONTRACT` welcome — **not** full Win/Android linked-dict / FIND-reopen / SEARCH-WORDLIST-runtime / DEFINITIONS port; prefer mirrors.

Do **not** wipe prior tip content. Amends: SEARCH-WORDLIST + FIND + KERNEL (+ optional WORDS-VOCAB / HOST-PARITY). **Prefer leave `SHARP-SIGN.md` untouched** (`51909b17e780c5f0096567d75f5d20e2`/30461). **Leave untouched:** MIN-MAX / SPACE / HOLD / CHAR-PLUS / ABS / U-LESS / ARCH / GAPS / WAVE24-PROPOSAL / COS-PASTE (locked md5s §8). Tip5 after 1–4 PASS. Skip 2DUP-FAMILY + ABORT" polish.

## 7. Non-goals

- Redefining `SEARCH-WORDLIST`/`search-wl-mark`/`search-demo` (**not** SEARCH-WORDLIST runtime reopen; thin companion only)
- Redefining `find`/`findentry`/`entry-find`/`find-xt`/`find-mark` (**not** FIND reopen; thin companion only)
- Redefining `WORDS`/`.words`/`words`/`words-demo` (**not** WORDS reopen; optional thin companion only)
- Bumping `_here` / `HERE` / `here-at` / HERE stub `$1000` (**CRITICAL** — vocab create/query must not bump HERE)
- Linked dict / wordlist-stack / search-order stack / DEFINITIONS deep rewrite — do **not** require GET-CURRENT/SET-CURRENT; do **not** promote to DEFINITIONS
- Tip5 DOCS-CITES
- SHARP-SIGN / MIN-MAX / SPACE / HOLD / ABS / U-LESS / CHAR-PLUS / TO-NUMBER / BASE / COMPARE / ACCEPT / COUNT / EXECUTE / ZERO / WITHIN / BITWISE / LSHIFT reopen (leave locked primaries untouched; **prefer leave SHARP-SIGN.md untouched**)
- Lab-grep-as-success kernel `MAX-*` as ANS `MAX` — leave untouched
- Full ANS `ENVIRONMENT?` table reopen; real DOES> XT / branch XT / LEAVE jump / full arena / linked XT / real STATE cell
- `2DUP`/`2DROP`/`2SWAP` redefine (**skip 2DUP-FAMILY**); `ABORT"` polish (**skip**)
- Real crypto / network fleet / opaque-weight ML; full Win/Android Forth VM (CONTRACT OK)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/WORDLIST.md` present (Research byte-copy OK); `SEARCH-WORDLIST.md` + `FIND.md` + `KERNEL.md` thin amends present (+ optional WORDS-VOCAB / HOST-PARITY); prior cites retained; host WORDLIST/FORTH-WORDLIST/GET/SET-CURRENT untouched via mirrors; `search-wl-mark`/`SEARCH-WORDLIST`/`search-demo` **not** redefined; find*/find-xt/find-mark **not** redefined; `WORDS`/`.words`/`words-demo` **not** redefined; `_here`/`HERE`/`here-at` **not** bumped (`$1000` stays); sharp-/hold-/min-/max-/space-/abs-/u-less-/to-number-/base-/char-plus marks **not** redefined; kernel `MAX-*` untouched; **prefer SHARP-SIGN.md untouched** (`51909b17e780c5f0096567d75f5d20e2`/30461); MIN-MAX (`0f6fca115a1b1bd279eea4f9fe7c4238`/29667); SPACE (`f869c0b9990b023d2cef8d52beb19155`/30928); HOLD (`f9e4e455ee91e7ee1b9da507cb940e10`/32774); CHAR-PLUS (`bb879e6fe97f8f8ebd25418d43b43751`/30666); ABS (`932392d9b62576dea8747de10b1f8efb`/31908); U-LESS (`181ed8664ad1914a302667a007281b02`/32007); ARCH (`2575a64f907756ab5ef78afa485f9a16`); GAPS (`1f2a5d9969d394e716dcb25c6234cf03`); WAVE24-PROPOSAL (`1cceddbdf85d16eef46863f03aec8c27`); COS-PASTE (`df576fdb220979cfa9e2f7dbf4c1c416`) untouched.
2. `wordlist-demo` → OK (markers §4; `[wordlist] WORDLIST` + `[wordlist] FORTH-WORDLIST` greppable; optional `wid=` / GET-CURRENT / SET-CURRENT welcome; no FAIL; no FIND reopen / SEARCH-WORDLIST runtime / linked dict / DEFINITIONS / HERE bump / SHARP-SIGN/MIN-MAX/SPACE reopen; no search-wl-/find-/words redefine). Prior `sharp-demo` + `minmax-demo` + `space-demo` + `abs-demo` + `uless-demo` + `hold-demo` + `charplus-demo` + `search-demo` + `find-demo` + `words-demo` + `number-demo` + `zero-demo` + `shift-demo` + `bit-demo` + `compare-demo` + `base-demo` + earlier incl `trit-math-demo` / `fold-demo` still OK.
3. Regression green (wave24 tip1–3 + wave23–14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `wordlist-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**)
- `docs/SEARCH-WORDLIST.md` (wave22 **4** — vocab-**search** companion; WORDLIST sibling **create/query** — **not** SEARCH-WORDLIST runtime reopen; prefer wordlist-/forth-wordlist-mark; do not redefine `SEARCH-WORDLIST`/`search-wl-mark`/`search-demo`)
- `docs/FIND.md` (wave18 **2** — **not** FIND reopen; do not redefine `find`/`findentry`/`entry-find`/`find-xt`/`find-mark`)
- `docs/WORDS-VOCAB.md` (wave11 **3**, optional — flat WORDS list companion; **not** WORDS reopen / linked dict; do not wipe `WORDS`/`.words`/`words-demo`)
- `docs/HOST-PARITY.md` (wave8 **4**, optional — `wordlist-demo` CONTRACT welcome)
- `docs/SHARP-SIGN.md` (wave24 **3** — **prefer leave untouched**; do not redefine sharp-/sharp-end-/sign-mark; HERE `$1000` untouched)
- `docs/MIN-MAX.md` / `docs/SPACE-SPACES-TYPE.md` / `docs/HOLD.md` / `docs/ABS-NEGATE.md` / `docs/U-LESS.md` / `docs/CHAR-PLUS.md` (leave primaries untouched)
- Prior: TO-NUMBER / BASE-HEX / ZERO / LSHIFT / COMPARE / ACCEPT / BITWISE / TRUE-FALSE / WITHIN / COUNT / EXECUTE / SOURCE-PAD / CHAR-CHARS / ENV / TICK / SYNONYM (keep cites; leave locked)
- `forth/tritium/kernel.fs` (wordlist-/forth-wordlist-mark only; do not redefine search-wl-/find-/words / sharp-/hold-/min-/max-/prior mirrors; do not bump HERE; optional GET/SET-CURRENT when free; kernel MAX-* untouched)
- ANS Forth `WORDLIST`/`FORTH-WORDLIST`/optional GET/SET-CURRENT (thin create/query only — prefer mirrors; not FIND reopen / SEARCH-WORDLIST runtime / linked dict / DEFINITIONS)
- Explicit deferral: WAVE19–24 PROPOSAL (thin vocab create/query beside SEARCH-WORDLIST; HERE `$1000` stays)
- Base tip: `7db5fbdd` / `7db5fbdd67ef8f49e955aadcb28ec02c05caf85a` (#114 wave24 tip3 SHARP-SIGN PASS)
- Wave24 proposal: `/workspace/tritium-research-docs/WAVE24-PROPOSAL.md`

## 10. Shipper implementation notes (Linux SoT)

Normative for Shipper on branch `shipper/wordlist-w24` from base `7db5fbdd67ef8f49e955aadcb28ec02c05caf85a`. Do **not** authorize merge, push, Mango, opaque-weight ML, or reopening closed tips / SHARP-SIGN / MIN-MAX / SPACE / HOLD / WAVE24-PROPOSAL.

### 10.1 Binding order

1. Prefer mirrors **`wordlist-mark`** / **`forth-wordlist-mark`** first — Lab greps markers, not bare ANS names.
2. Bare `WORDLIST`/`FORTH-WORDLIST` may alias same markers if free — **must not** redefine `SEARCH-WORDLIST`/`search-wl-mark`/`search-demo` or `find`/`findentry`/`entry-find`/`find-xt`/`find-mark` or `WORDS`/`.words`/`words-demo` or bump `_here`/`HERE`/`here-at`.
3. Optional `GET-CURRENT`/`get-current-mark` + `SET-CURRENT`/`set-current-mark` when free — **do not require** DEFINITIONS / search-order stack / linked dict.
4. Never alias onto search-wl-/find-/words-/sharp-/hold-/min-/max-mark or `_here`/`HERE`/`here-at`. Vocab create/query ≠ vocab-search / FIND / WORDS list / pictured continue / minmax / pointer.

### 10.2 Classic pictures (WORDLIST + FORTH-WORDLIST)

Document one fixed fixture per required word. Recommended:

| Field | Example | Notes |
|-------|---------|-------|
| `WORDLIST` wid | stub wid=`1` or `2` | Classic new-wordlist id picture |
| optional `WORDLIST` `wid=` | `wid=1` | New-wordlist id echo welcome |
| `FORTH-WORDLIST` wid | stub wid=`0` (forth) | Classic forth-wordlist id picture |
| optional `FORTH-WORDLIST` `wid=` | `wid=0` | Forth-wordlist id echo welcome |
| optional `GET-CURRENT` | `[wordlist] GET-CURRENT` (+ `wid=`) | Welcome when free — not required |
| optional `SET-CURRENT` | `[wordlist] SET-CURRENT` (+ `wid=`) | Welcome when free — not DEFINITIONS / search-order |

Shipper may use other pictured stub fixtures — document which. Lab greps `[wordlist] WORDLIST` + `[wordlist] FORTH-WORDLIST` regardless of fixtures, as long as both markers are greppable and FAIL is avoided. **Do not** implement vocab create/query by redefining `search-wl-mark` / `find-mark` / `WORDS`. **Do not** implement by bumping HERE stub `$1000`. **Do not** reopen FIND / SEARCH-WORDLIST runtime / linked dict / DEFINITIONS / SHARP-SIGN / MIN-MAX / SPACE-SPACES-TYPE.

### 10.3 What Lab greps / does not grep

**Required:** `[wordlist-demo] OK` + `[wordlist] WORDLIST` + `[wordlist] FORTH-WORDLIST`. **Optional welcome:** `wid=`; GET-CURRENT / SET-CURRENT when free; `wordlist-demo CONTRACT`. **Must NOT Lab-grep as success:** `_here`/`HERE`/`here-at`/`[allot]`; `[search]`/`search-wl-mark`; `[find]`/`find-xt`/`find-mark`/host find*; `[words]`/`WORDS`/`.words`; `[sharp]`*/`[hold]`*/`[minmax]`*/`[space]`*/`[abs]`*/`[uless]`*/`[char+]`*/`[number]`*/`[base]`*; `[zero]`/`[shift]`/`[bit]`/`[compare]`/`[cell]`; kernel `MAX-*`; DOCS-CITES.

### 10.4 Regression + companions + non-goals

Retain green: `sharp-demo` / `minmax-demo` / `space-demo` / `abs-demo` / `uless-demo` / `hold-demo` / `charplus-demo` / `search-demo` / `find-demo` / `words-demo` / `number-demo` / `zero-demo` / `shift-demo` / `bit-demo` / `compare-demo` / `base-demo` / `accept-demo` / `exec-demo` / `count-demo` / `within-demo` / `true-demo` / `string-demo` / `word-demo` / `env-demo` / `cell-demo` / `allot-demo` / `kernel-demo` / `trit-math-demo` / `fold-demo` (+ earlier). Required thin: SEARCH-WORDLIST + FIND + KERNEL. Optional: WORDS-VOCAB / HOST-PARITY. **Untouched:** ARCH / GAPS / WAVE24-PROPOSAL / COS-PASTE / SHARP-SIGN / MIN-MAX / SPACE-SPACES-TYPE / HOLD / CHAR-PLUS / ABS-NEGATE / U-LESS primaries.

Do **not**: redefine search-wl/find/words/sharp/hold/min/max/abs/u-less/zero/within/true/space/charplus/to-number/base mirrors; bump HERE `$1000`; reopen FIND/SEARCH-WORDLIST-runtime/linked-dict/DEFINITIONS/SHARP-SIGN/MIN-MAX/SPACE; land tip5; amend ARCH/GAPS / locked primaries / WAVE24-PROPOSAL; confuse ANS MAX with kernel `MAX-*`; tip Shipper / push / merge / touch Mango; redefine host 2dup/2drop/2swap; polish ABORT".

WORDLIST = thin vocab **create/query** only. Sibling to SEARCH-WORDLIST / FIND / WORDS — **not** a reopen. Prefer `wordlist-mark` / `forth-wordlist-mark`. Classic: `WORDLIST` → new-wordlist id; `FORTH-WORDLIST` → forth-wordlist id. Emit both → `[wordlist-demo] OK`.

Research drafts under `/workspace/tritium-research-docs/`. Shipper byte-copies into `docs/` on `shipper/wordlist-w24` only. Research does **not** tip Shipper / push / merge / touch Mango. Still deferred: tip5; FIND reopen; SEARCH-WORDLIST runtime; linked dict; DEFINITIONS; full pictured beyond tip3; boolean cell; full Win/Android VM; 2DUP-FAMILY; ABORT" polish; Mango.

Acceptance one-liner: `wordlist-demo` greps `[wordlist] WORDLIST` + `[wordlist] FORTH-WORDLIST` + `[wordlist-demo] OK`; prior search/find/words/sharp demos OK; search-wl-mark / find-mark / WORDS / HERE `$1000` untouched; Linux SoT; no merge. Anchors: WAVE24-PROPOSAL § Tip 4; base `7db5fbdd67ef8f49e955aadcb28ec02c05caf85a`; branch `shipper/wordlist-w24`; handoff `WAVE24-TIP4-HANDOFF.md` (= `TIP4-HANDOFF.md`).

### 10.5 Disambiguation (Shipper)

| Surface | Tip | This tip? |
|---------|-----|-----------|
| `WORDLIST` / `wordlist-mark` | wave24 **4** | **YES** |
| `FORTH-WORDLIST` / `forth-wordlist-mark` | wave24 **4** | **YES** |
| `GET-CURRENT` / `SET-CURRENT` (+ mirrors) | wave24 **4** optional | YES when free — **do not require** DEFINITIONS |
| `SEARCH-WORDLIST` / `search-wl-mark` | wave22 **4** | NO — thin amend only; **NOT** runtime reopen |
| `FIND` / `find-xt` / `find-mark` / host find* | wave18 **2** / wave7 | NO — thin amend only; **NOT** FIND reopen |
| `WORDS` / `.words` / `words-demo` | wave11 **3** | NO — optional thin cite; **NOT** WORDS reopen |
| `_here` / `HERE` / `here-at` | wave14 **1** | NO — HERE `$1000` untouched; **do not bump** |
| `#`/`#>`/`SIGN` / sharp-* | wave24 **3** | NO — prefer leave SHARP-SIGN.md untouched |
| `HOLD`/`hold-mark`/`<#` / MIN/MAX / SPACE / ABS / U< / CHAR+ | prior tips | NO — leave locked primaries untouched |
| kernel `MAX-ENTRIES`/`MAX-SYNONYMS`/`MAX-CHAR` | kernel caps | **NO** — leave untouched |
| DOCS-CITES (ARCH/GAPS) | wave24 **5** | NO — tip5 |

Vocab create/query ≠ SEARCH-WORDLIST ≠ FIND ≠ WORDS ≠ pictured continue ≠ MIN/MAX ≠ HERE. Leave locked primaries (§8) byte-identical. HERE `$1000` + search-wl-mark + find-mark + WORDS untouched. Docs ONLY under `/workspace/tritium-research-docs/`. Skip 2DUP-FAMILY + ABORT" polish.

### 10.6 Marker grammar + boundaries

Preferred: `[wordlist] WORDLIST wid=1` + `[wordlist] FORTH-WORDLIST wid=0` — both required markers — close `[wordlist-demo] OK` — avoid FAIL. Optional GET/SET-CURRENT when free. WORDLIST = thin vocab **create/query** only — sibling to SEARCH-WORDLIST/FIND/WORDS; **not** a reopen. HERE `$1000` + search-wl-mark + find-mark + WORDS + SHARP-SIGN primary untouched. Tip5 later. Linked dict / DEFINITIONS / SEARCH-WORDLIST runtime / FIND reopen out.

### 10.7 Prior demos + handoff

Retain green (abbrev): `sharp-demo` / `minmax-demo` / `space-demo` / `abs-demo` / `uless-demo` / `hold-demo` / `charplus-demo` / `search-demo` / `find-demo` / `words-demo` / `number-demo` / `zero-demo` / `shift-demo` / `bit-demo` / `compare-demo` / `base-demo` / `accept-demo` / `exec-demo` / `count-demo` / `within-demo` / `true-demo` / `string-demo` / `word-demo` / `env-demo` / `cell-demo` / `allot-demo` / `kernel-demo` / `trit-math-demo` / `fold-demo` (+ earlier suite).

Wave24 LOCKED: 1 SPACE-SPACES-TYPE → 2 MIN-MAX → 3 SHARP-SIGN → **4 WORDLIST** → 5 DOCS-CITES. Base tip3 PASS #114 @ `7db5fbdd67ef8f49e955aadcb28ec02c05caf85a`. Branch `shipper/wordlist-w24` — **no merge**. Handoff: `WAVE24-TIP4-HANDOFF.md` (= `TIP4-HANDOFF.md`). Research does **NOT** tip Shipper / push / merge / touch Mango. Closes GAPS deferred WORDLIST/FORTH-WORDLIST as **thin create/query only**. HERE `$1000` untouched. Prefer mirrors. Lab greps `[wordlist-demo] OK` + `[wordlist] WORDLIST` + `[wordlist] FORTH-WORDLIST`. Skip 2DUP-FAMILY + ABORT" polish. Stay out of Mango.

Shipper **MUST NOT** redefine: `SEARCH-WORDLIST`/`search-wl-mark`/`search-demo` / find*/find-xt/find-mark / `WORDS`/`.words`/`words-demo` / sharp-/hold-/min-/max-/space-/abs-/u-less-/to-number-/base-/char-plus marks / HERE/`_here`/`here-at` / kernel `MAX-*`. Prefer `wordlist-mark`/`forth-wordlist-mark` (+ optional get/set-current-mark). Demo avoids FAIL; emits both markers → `[wordlist-demo] OK`. Tip5 ARCH/GAPS later. Tip1–3 primaries untouched. Linux SoT. No merge. No Mango. Research docs only under `/workspace/tritium-research-docs/`.
