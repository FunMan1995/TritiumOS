# SHARP-SIGN — thin pictured-numeric continue (`#` / `#>` / `SIGN`, optional `#S`) + `sharp-demo`

**Status:** Shipper-ready stub spec (wave24 item **3**)
**Canonical brief:** ANS-shaped `#` / `#>` / `SIGN` (thin pictured-numeric **continue** beside wave23 HOLD start); `docs/HOLD.md` (wave23 **2** — **not** HOLD reopen; **do not** redefine `HOLD`/`hold-mark`/`hold-demo`/`<#`; **CRITICAL do not bump** HERE `$1000`); `docs/TO-NUMBER.md` (wave22 **3** — **not** TO-NUMBER reopen; do not redefine `to-number-mark`); `docs/BASE-HEX.md` (wave21 **2** — **not** BASE reopen; do not redefine base/hex/decimal marks); `docs/KERNEL.md` (wave7 **5**); optional `docs/ALLOT-HERE.md` / `docs/ENVIRONMENT-QUERY.md` / `docs/HOST-PARITY.md`; WAVE19–24 deferral closed as **thin pictured continue only** (not HOLD reopen; **not** full pictured rewrite / pictured buffer heap; **not** BASE/TO-NUMBER/MIN-MAX/SPACE reopen; prefer mirrors **`sharp-mark` / `sharp-end-mark` / `sign-mark`** (+ optional **`sharps-mark`**) whenever host `#`/`#>`/`SIGN`/`#S` collide). **CRITICAL:** HERE stub stays `$1000`. Do **not** redefine hold-/to-number-/base-/min-/max-/abs-/u-less-/zero-/within-/true-/space-/char-plus marks. Kernel `MAX-ENTRIES`/`MAX-SYNONYMS`/`MAX-CHAR` still NOT ANS MAX.
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `sharp.fs` / `sharp-sign.fs` / `pictured.fs`); Linux host REPL; prefer Forth mirrors **`sharp-mark` / `sharp-end-mark` / `sign-mark`** (+ optional **`sharps-mark`**) whenever host `#` / `#>` / `SIGN` / `#S` collide; **CRITICAL — do not** redefine/alias/bump `_here` / `HERE` / `here-at` (HERE stub `$1000` stays); **do not** redefine `HOLD` / `hold-mark` / `hold-demo` / `<#`; **do not** redefine `to-number-mark` / `base-mark`/`hex-mark`/`decimal-mark` / `min-mark`/`max-mark` / `abs-mark`/`negate-mark` / `u-less-mark` / `zero-eq-mark` / `within-mark` / `true-mark`/`false-mark` / `space-mark`/`spaces-mark`/`type-mark`/`emit-mark` / `char-plus-mark`; optional `#S` welcome when free — **do not require**
**Companions:** `docs/HOLD.md` (thin amend this tip), `docs/TO-NUMBER.md` (thin amend this tip), `docs/BASE-HEX.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip); optional light cite `docs/ALLOT-HERE.md` / `docs/ENVIRONMENT-QUERY.md` / `docs/HOST-PARITY.md`
**Base tip SHA:** `fca71c4a` (wave24 tip2 MIN-MAX PASS / #113 `shipper/min-max-w24`) / full `fca71c4a8c5ad8dd3d3fd3f3646820e9ce313d9b`

## 1. Purpose

WAVE14 tip **1** landed HERE/ALLOT (`docs/ALLOT-HERE.md`) stub `$1000 _here !`. WAVE21 tip **2** landed BASE/HEX/DECIMAL (`docs/BASE-HEX.md`; HERE `$1000` untouched). WAVE22 tip **3** landed `>NUMBER` (`docs/TO-NUMBER.md`; HERE `$1000` untouched). WAVE23 tip **2** landed HOLD thin pictured **start** (`docs/HOLD.md`; optional `<#`/`#` when free — do **not** require `#S`/`#>`/`SIGN`; prefer `hold-mark`). WAVE24 tip **1** SPACE/SPACES/TYPE + tip **2** MIN/MAX landed (leave primaries untouched). WAVE18–24 deferred thin pictured **continue** (`#`/`#>`/`SIGN`, optional `#S`) beside HOLD (**not** HOLD reopen; **not** full pictured rewrite; **not** BASE/TO-NUMBER reopen; **must not** bump HERE `$1000`; **must not** redefine `hold-mark`/`HOLD`/`<#`). This tip lands **stub** thin continue only: `#`/`sharp-mark` → `[sharp] #` (+ optional `u=`/`c=` — convert-one-digit); `#>`/`sharp-end-mark` → `[sharp] #>` (+ optional `u=`/`addr=` — pictured-end); `SIGN`/`sign-mark` → `[sharp] SIGN` (+ optional `n=` — sign-prefix). Optional `#S`/`sharps-mark` when free — **do not require**. Smoke via **`sharp-demo`**. Prefer mirrors whenever host collide. **CRITICAL:** do **not** bump `_here`/`HERE`/`here-at` (`$1000` stays). **CRITICAL:** do **not** redefine `HOLD`/`hold-mark`/`hold-demo`/`<#`. Pairs with HOLD + TO-NUMBER + BASE-HEX **without** HOLD/BASE/TO-NUMBER reopen / full pictured rewrite / MIN-MAX/SPACE/ABS/U-LESS/CHAR-PLUS reopen. **Not** tip4 WORDLIST / tip5 DOCS-CITES. **Leave untouched (md5s §8):** MIN-MAX / SPACE-SPACES-TYPE / CHAR-PLUS / ABS-NEGATE / U-LESS / ARCH / GAPS / WAVE24-PROPOSAL / COS-PASTE. HOLD + TO-NUMBER + BASE-HEX + KERNEL are thin-amended companions this tip, not reopened as primaries.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `#` / `sharp-mark` | `( ud1 -- ud2 )` *or* `( -- )` with fixed demo fixture | Thin pictured convert-one-digit continue mark; print `[sharp] #` (+ optional `u=<n>` / `c=<char|ord>`); classic convert-one-digit picture welcome |
| `#>` / `sharp-end-mark` | `( xd -- c-addr u )` *or* `( -- )` with fixed demo fixture | Thin pictured-end continue mark; print `[sharp] #>` (+ optional `u=<n>` / `addr=<n>`); classic pictured-end picture welcome |
| `SIGN` / `sign-mark` | `( n -- )` *or* `( -- )` with fixed demo fixture | Thin sign-prefix continue mark; print `[sharp] SIGN` (+ optional `n=<signed>`); classic negative-n → hold `-` picture welcome |
| `#S` / `sharps-mark` (optional) | `( ud1 -- ud2 )` *or* `( -- )` | Optional convert-remaining digits mark; print `[sharp] #S` when free — **do not require** |
| `sharp-demo` | `( -- )` | See §5 |

Host note: bind bare `#` / `#>` / `SIGN` / `#S` **only if** free; prefer mirrors **`sharp-mark` / `sharp-end-mark` / `sign-mark`** (+ optional **`sharps-mark`**). **CRITICAL:** do **not** bump `_here`/`HERE`/`here-at` (HERE `$1000` stays). **CRITICAL:** do **not** redefine `HOLD`/`hold-mark`/`hold-demo`/`<#`. Do **not** redefine `to-number-mark` / `base-mark`/`hex-mark`/`decimal-mark` / `min-mark`/`max-mark` / `abs-mark`/`negate-mark` / `u-less-mark` / `zero-eq-mark` / `within-mark` / `true-mark`/`false-mark` / `space-mark`/`spaces-mark`/`type-mark`/`emit-mark` / `char-plus-mark`. Optional `u=`/`c=`/`addr=`/`n=` are host echo only — not full pictured buffer VM / HERE bump / HOLD/BASE reopen. Prefer **fixed demo pictured fixtures**. Optional `#S` welcome when free — **do not require**. Demo **must** exercise all three required surfaces so Lab greps `[sharp] #` + `[sharp] #>` + `[sharp] SIGN` + `[sharp-demo] OK`.

## 3. Stub semantics

- **`#` / `sharp-mark`:** classic ANS `( ud1 -- ud2 )` convert-one-digit — **marker only**: print `[sharp] #` (+ optional `u=` / `c=`). Classic fixture: ud → digit char (`'0'+n` / ord — document). **Does not** rewrite pictured buffer VM, implement full `<#`…`#>` heap, reopen HOLD/BASE/TO-NUMBER, or bump HERE. **Does not** redefine `hold-mark`/`HOLD`/`<#` / `_here`/`HERE`/`here-at`.
- **`#>` / `sharp-end-mark`:** classic ANS `( xd -- c-addr u )` pictured-end — marker only: print `[sharp] #>` (+ optional `u=` / `addr=`). **Does not** open pictured buffer heap, bump HERE, or reopen HOLD/BASE/TO-NUMBER.
- **`SIGN` / `sign-mark`:** classic ANS `( n -- )` hold `-` when `n < 0` — marker only: print `[sharp] SIGN` (+ optional `n=`). Classic fixture: `n=-7`. Positive/zero welcome when free — **do not require**. **Does not** reopen ABS/MIN-MAX/boolean cell/HOLD/HERE.
- **Optional `#S` / `sharps-mark`:** print `[sharp] #S` when free — **do not require**. Does not bump HERE / reopen HOLD/BASE/TO-NUMBER.
- **Fixed demo pictured fixtures (document):**
  - **`#` convert-one-digit (required):** ud → digit char — Lab greps `[sharp] #`.
  - **`#>` pictured-end (required):** length/address echo welcome — Lab greps `[sharp] #>`.
  - **`SIGN` sign-prefix (required):** negative `n=-7` — Lab greps `[sharp] SIGN`.
  - **Optional `#S`:** welcome when free — do **not** require / promote to full rewrite / HOLD reopen / HERE bump.
  - **Optional echo:** `u=`/`c=`/`addr=`/`n=` — greppable three markers enough for Lab OK.
- **Optional push:** marker alone enough — do not require full pictured VM / HOLD reopen / HERE bump.
- **FAIL:** `[sharp] FAIL reason=<…>` optional (demo **must avoid**). Prefer not emitting FAIL on the happy path.
- Storage: fixed demo pictured fixtures / host int / char / address echo only. **No** real pictured buffer VM / full `<#`…`#>` rewrite; no HOLD/BASE/TO-NUMBER reopen; no HERE / `_here` / `here-at` redefine / bump; no MIN-MAX / SPACE / ABS / U-LESS / CHAR-PLUS reopen; no arena; no full ANS ENVIRONMENT? table reopen; **no** touch to kernel `MAX-ENTRIES` / `MAX-SYNONYMS` / `MAX-CHAR`.
- **HERE stub base stays `$1000`:** Lab greps `[sharp] #` + `[sharp] #>` + `[sharp] SIGN` + `[sharp-demo] OK` only — **never** Lab-grep `_here` / `HERE` / `here-at` / `[allot]` as SHARP success. Pictured continue must **not** bump HERE.
- **HOLD stays (thin companion amend only):** do **not** redefine `HOLD` / `hold-mark` / `hold-demo` / `<#`. Sibling thin pictured **continue** beside HOLD **start** — **not** HOLD reopen. Do not wipe wave23 HOLD content. Do **not** Lab-grep `[hold] HOLD` as this tip’s success.
- **TO-NUMBER / BASE-HEX stay (thin companion amends only):** do **not** redefine `to-number-mark` / `base-mark`/`hex-mark`/`decimal-mark`. Sibling continue — **not** TO-NUMBER / BASE reopen.
- **MIN-MAX / SPACE-SPACES-TYPE / ABS-NEGATE / U-LESS / CHAR-PLUS stay untouched primaries:** do **not** redefine their mirrors. **Prefer leave `MIN-MAX.md` untouched** (`0f6fca115a1b1bd279eea4f9fe7c4238`/29667). Locked md5s §1 / §8 for SPACE / CHAR-PLUS / ABS / U-LESS / ARCH / GAPS / PROPOSAL / COS-PASTE.
- Nest with prior minmax/space/abs/uless/hold/charplus/search/number/zero/shift/bit/compare/base + earlier stubs OK. Assert prior `hold-demo` / `minmax-demo` / `space-demo` / `number-demo` / `base-demo` / `allot-demo` still OK.
- Still no full pictured rewrite beyond this thin continue; no HOLD/BASE/TO-NUMBER reopen; no HERE bump; no tip4–5; no linked XT / full arena / full Win/Android Forth VM. HOLD + TO-NUMBER + BASE-HEX thin-amended companions only (new md5s in tip summary).

## 4. Markers

```
[sharp] # [u=<n>] [c=<char|ord>]     # u=/c= optional; classic convert-one-digit picture welcome
[sharp] #> [u=<n>] [addr=<n>]        # u=/addr= optional; classic pictured-end picture welcome
[sharp] SIGN [n=<signed>]            # n= optional; classic sign-prefix (negative n) picture welcome
[sharp] #S                           # optional; welcome when free — do not require
[sharp] FAIL reason=<…>              # demo avoids
[sharp-demo] OK
[sharp-demo] FAIL
```

Lab greps `[sharp-demo] OK` plus greppable **`[sharp] #`** + **`[sharp] #>`** + **`[sharp] SIGN`** (optional `u=`/`c=`/`addr=`/`n=` welcome). Demo avoids `[sharp] FAIL`. Optional `[sharp] #S` when free — **do not require**. **Do not** Lab-grep `_here`/`HERE`/`here-at`/`[allot]` / `[hold]`/`hold-mark` / `[number]` / `[base]` / `[minmax]` / `[space]` / `[abs]` / `[uless]` / `[char+]` / `[zero]` / `[shift]` / `[bit]` / `WORDLIST` / `MAX-ENTRIES`/`MAX-SYNONYMS`/`MAX-CHAR` as this tip’s success.

## 5. `sharp-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; pictured-numeric continue marks need no dict entries.
2. Ensure **fixed demo pictured fixtures** exist so classic pictures are deterministic:
   - `#` convert-one-digit fixture (e.g. ud → digit char — document).
   - `#>` pictured-end fixture (e.g. length / address echo welcome — document).
   - `SIGN` negative-n fixture (e.g. `n=-7` — document).
   Fixtures may live beside (not replacing) HOLD start picture / BASE-HEX radix picture / TO-NUMBER digit-string picture / ENVIRONMENT-QUERY stub / ALLOT-HERE stub pointer / MIN-MAX signed fixtures — document; do **not** require full pictured rewrite / HOLD reopen / BASE reopen / TO-NUMBER reopen / HERE bump / MIN-MAX reopen.
3. Invoke `#` (or **`sharp-mark`**) against classic convert-one-digit fixture → `[sharp] #` (+ optional `u=` / `c=`).
4. Invoke `#>` (or **`sharp-end-mark`**) against classic pictured-end fixture → `[sharp] #>` (+ optional `u=` / `addr=`).
5. Invoke `SIGN` (or **`sign-mark`**) against classic negative-n fixture → `[sharp] SIGN` (+ optional `n=`).
6. Optional (when free): invoke `#S` / `sharps-mark` → `[sharp] #S` — **do not require**.
7. Assert no `[sharp] FAIL`. Assert no full pictured rewrite / HOLD/BASE/TO-NUMBER reopen / HERE bump / MIN-MAX/SPACE reopen / full ANS ENVIRONMENT? reopen. Assert host `#`/`#>`/`SIGN`/`#S` not redefined via mirrors. Assert `_here`/`HERE`/`here-at` **not** bumped (`$1000` stays). Assert `HOLD`/`hold-mark`/`hold-demo`/`<#` **not** redefined. Assert to-number-/base-/min-/max-/abs-/u-less-/zero-/within-/true-/space-/char-plus marks **not** redefined. Assert kernel `MAX-*` untouched. Assert locked primaries (§8) untouched. Assert prior `hold-demo` / `minmax-demo` / `space-demo` / `number-demo` / `base-demo` / `allot-demo` still OK.
8. Prior minmax/space/abs/uless/hold/charplus/search/number/zero/shift/bit/compare/base + earlier demos still OK.
9. `[sharp-demo] OK`.

Required markers: `[sharp] #` + `[sharp] #>` + `[sharp] SIGN`. Optional `u=` / `c=` / `addr=` / `n=` echo is not required for Lab OK when all three required markers are greppable. Optional `#S` not required. No full pictured rewrite. No HOLD reopen. No BASE reopen. No TO-NUMBER reopen. No HERE stub base bump. No tip4 WORDLIST. No full ANS ENVIRONMENT? table reopen.

## 6. Thin amend — companions

### `docs/HOLD.md`

- Companions / Status: add `SHARP-SIGN.md` (wave24 **3**); **keep** TO-NUMBER / BASE-HEX / KERNEL / ALLOT-HERE / ENV / HOST-PARITY / CHAR-PLUS cites — do not wipe HOLD primary content.
- Purpose / §3 / non-goals: HOLD stays thin pictured **start**; `#`/`#>`/`SIGN` (+ optional `#S`) are sibling **thin pictured continue** — **NOT** HOLD reopen / full pictured rewrite / BASE/TO-NUMBER reopen / HERE bump. Stress: HERE `$1000` untouched; prefer `sharp-mark`/`sharp-end-mark`/`sign-mark`; **do not** redefine `HOLD`/`hold-mark`/`hold-demo`/`<#`.
- Non-goals: `#`/`#>`/`SIGN` → `docs/SHARP-SIGN.md` (wave24 **3**). HOLD stays landed. Tip4–5 later.
- Acceptance: Lab smokes `sharp-demo` (retains `hold-demo` + `number-demo` + `base-demo` + `allot-demo` + `minmax-demo` + `space-demo` + `abs-demo` + `uless-demo` + `charplus-demo`). Cite: `docs/SHARP-SIGN.md`.

### `docs/TO-NUMBER.md`

- Companions / Status: add `SHARP-SIGN.md` (wave24 **3**); **keep** HOLD / BASE-HEX / KERNEL / ENV / ALLOT-HERE / HOST-PARITY cites — do not wipe TO-NUMBER content.
- Purpose / §3 / non-goals: `>NUMBER` stays thin parse; `#`/`#>`/`SIGN` sibling thin pictured continue — **NOT** TO-NUMBER reopen / HOLD reopen / HERE bump. Prefer mirrors; **do not** redefine `to-number-mark`. HERE `$1000` untouched.
- Non-goals: → `docs/SHARP-SIGN.md` (wave24 **3**). Tip4–5 later. Acceptance: Lab smokes `sharp-demo` (retains `number-demo` + `hold-demo` + `base-demo` + `allot-demo`). Cite: `docs/SHARP-SIGN.md`.

### `docs/BASE-HEX.md`

- Companions / Status: add `SHARP-SIGN.md` (wave24 **3**); **keep** HOLD / TO-NUMBER / KERNEL / ENV / HOST-PARITY / ALLOT-HERE cites — do not wipe BASE-HEX content.
- Purpose / §3 / non-goals: BASE/HEX/DECIMAL stay radix marks; `#`/`#>`/`SIGN` sibling thin pictured continue — **NOT** BASE reopen / HOLD reopen / HERE bump. Prefer mirrors; **do not** redefine `base-mark`/`hex-mark`/`decimal-mark`. HERE `$1000` untouched.
- Non-goals: → `docs/SHARP-SIGN.md` (wave24 **3**). Tip4–5 later. Acceptance: Lab smokes `sharp-demo` (retains `base-demo` + `hold-demo` + `number-demo` + `allot-demo`). Cite: `docs/SHARP-SIGN.md`.

### `docs/KERNEL.md`

- Companions: add `SHARP-SIGN.md` (wave24 **3**); **keep** wave24 tip1–2 SPACE-SPACES-TYPE + MIN-MAX cites and wave23 tip1–4 + wave22 tip1–4 + wave21 tip1–4 + wave20–15 cites.
- Words table: add `#`/`sharp-mark` + `#>`/`sharp-end-mark` + `SIGN`/`sign-mark` (+ optional `#S`/`sharps-mark`) + `sharp-demo` (mirrors; thin continue only; **CRITICAL:** do not bump `_here`/`HERE`/`here-at` — `$1000` stays; **do not** redefine `HOLD`/`hold-mark`/`hold-demo`/`<#` / to-number-/base-/min-/max-/abs-/u-less-/zero-/within-/true-/space-/char-plus marks; kernel `MAX-*` ≠ ANS MAX; **not** HOLD/BASE/TO-NUMBER reopen / full pictured rewrite; optional `u=`/`c=`/`addr=`/`n=`; optional `#S` when free — do not require).
- Non-goals: `#`/`#>`/`SIGN` → `docs/SHARP-SIGN.md`. HOLD / TO-NUMBER / BASE-HEX thin amends only (do not wipe). HERE `$1000` untouched. MIN-MAX / SPACE primaries untouched. Tip4–5 later.
- Acceptance: Lab smokes `sharp-demo` (retains `minmax-demo` + `space-demo` + `abs-demo` + `uless-demo` + `hold-demo` + `charplus-demo` + `search-demo` + `number-demo` + `zero-demo` + `shift-demo` + `bit-demo` + `compare-demo` + `base-demo` + prior incl `trit-math-demo` / `fold-demo`).
- Cite: `docs/SHARP-SIGN.md`.

### Optional — `docs/ALLOT-HERE.md` / `docs/ENVIRONMENT-QUERY.md` / `docs/HOST-PARITY.md`

- ALLOT-HERE: HERE `$1000` **reminder — do not bump**; pictured continue must not bump HERE. Retain `allot-demo`.
- ENVIRONMENT-QUERY: optional pictured continue cite — **not** full ANS ENVIRONMENT? reopen / HOLD/BASE reopen / HERE bump. Retain `env-demo`.
- HOST-PARITY: `sharp-demo CONTRACT` welcome — **not** full Win/Android pictured rewrite; prefer mirrors.

Do **not** wipe prior tip content. Amends: HOLD + TO-NUMBER + BASE-HEX + KERNEL (+ optional ALLOT-HERE / ENV / HOST-PARITY). **Prefer leave `MIN-MAX.md` untouched** (`0f6fca115a1b1bd279eea4f9fe7c4238`/29667). **Leave untouched:** SPACE (`f869c0b9990b023d2cef8d52beb19155`/30928); CHAR-PLUS (`bb879e6fe97f8f8ebd25418d43b43751`/30666); ABS (`932392d9b62576dea8747de10b1f8efb`/31908); U-LESS (`181ed8664ad1914a302667a007281b02`/32007); ARCH (`2575a64f907756ab5ef78afa485f9a16`); GAPS (`1f2a5d9969d394e716dcb25c6234cf03`); WAVE24-PROPOSAL (`1cceddbdf85d16eef46863f03aec8c27`); COS-PASTE (`df576fdb220979cfa9e2f7dbf4c1c416`). Tip5 after 1–4 PASS. Skip 2DUP-FAMILY + ABORT" polish.

## 7. Non-goals

- Bumping `_here` / `HERE` / `here-at` / HERE stub `$1000` (**CRITICAL** — pictured continue must not bump HERE)
- Redefining `HOLD`/`hold-mark`/`hold-demo`/`<#` (**not** HOLD reopen; thin companion only)
- Redefining `to-number-mark` / `base-mark`/`hex-mark`/`decimal-mark` (**not** TO-NUMBER / BASE reopen; thin companions only)
- Redefining `min-mark`/`max-mark` / abs-/u-less-/zero-/within-/true-/space-/char-plus marks (leave locked primaries untouched; **prefer leave MIN-MAX.md untouched**)
- Lab-grep-as-success kernel `MAX-ENTRIES`/`MAX-SYNONYMS`/`MAX-CHAR` as ANS `MAX` — leave untouched
- Full pictured rewrite / pictured buffer heap / full `<#`…`#>` beyond this thin continue — do **not** require `#S`; do **not** reopen HOLD start
- Tip4 WORDLIST / tip5 DOCS-CITES
- SEARCH-WORDLIST / FIND / COMPARE / ACCEPT / COUNT / EXECUTE / SPACE / ABS / U-LESS / ZERO / WITHIN / BITWISE / LSHIFT reopen
- Full ANS `ENVIRONMENT?` table reopen; real DOES> XT / branch XT / LEAVE jump / full arena / linked XT / real STATE cell
- `2DUP`/`2DROP`/`2SWAP` redefine (**skip 2DUP-FAMILY**); `ABORT"` polish (**skip**)
- Real crypto / network fleet / opaque-weight ML; full Win/Android Forth VM (CONTRACT OK)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/SHARP-SIGN.md` present (Research byte-copy OK); `HOLD.md` + `TO-NUMBER.md` + `BASE-HEX.md` + `KERNEL.md` thin amends present (+ optional ALLOT-HERE / ENVIRONMENT-QUERY / HOST-PARITY); prior cites retained; host `#`/`#>`/`SIGN`/`#S` untouched via mirrors; `_here`/`HERE`/`here-at` **not** bumped (HERE `$1000` stays); `HOLD`/`hold-mark`/`hold-demo`/`<#` **not** redefined; to-number-/base-/min-/max-/prior mirrors **not** redefined; kernel `MAX-*` untouched; **MIN-MAX.md untouched** (`0f6fca115a1b1bd279eea4f9fe7c4238`/29667); SPACE (`f869c0b9990b023d2cef8d52beb19155`/30928); CHAR-PLUS (`bb879e6fe97f8f8ebd25418d43b43751`/30666); ABS (`932392d9b62576dea8747de10b1f8efb`/31908); U-LESS (`181ed8664ad1914a302667a007281b02`/32007); ARCH (`2575a64f907756ab5ef78afa485f9a16`/18387); GAPS (`1f2a5d9969d394e716dcb25c6234cf03`/63563); WAVE24-PROPOSAL (`1cceddbdf85d16eef46863f03aec8c27`); COS-PASTE (`df576fdb220979cfa9e2f7dbf4c1c416`) untouched.
2. `sharp-demo` → OK (markers §4; `[sharp] #` + `[sharp] #>` + `[sharp] SIGN` greppable; optional `u=`/`c=`/`addr=`/`n=` / `#S` welcome; no FAIL; no full pictured rewrite / HOLD/BASE/TO-NUMBER reopen / HERE bump / MIN-MAX/SPACE reopen; no hold-/to-number-/base-/min-mark redefine). Prior `minmax-demo` + `space-demo` + `abs-demo` + `uless-demo` + `hold-demo` + `charplus-demo` + `search-demo` + `number-demo` + `zero-demo` + `shift-demo` + `bit-demo` + `compare-demo` + `base-demo` + earlier incl `trit-math-demo` / `fold-demo` still OK.
3. Regression green (wave24 tip1–2 + wave23–14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `sharp-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**)
- `docs/HOLD.md` (wave23 **2** — pictured **start** companion; SHARP sibling **continue** — **not** HOLD reopen; HERE `$1000` untouched; prefer sharp-/sharp-end-/sign-mark; do not redefine `HOLD`/`hold-mark`/`hold-demo`/`<#`)
- `docs/TO-NUMBER.md` (wave22 **3** — **not** TO-NUMBER reopen; HERE `$1000` untouched; do not redefine `to-number-mark`)
- `docs/BASE-HEX.md` (wave21 **2** — **not** BASE reopen; HERE `$1000` untouched; do not redefine base/hex/decimal marks)
- `docs/ALLOT-HERE.md` (wave14 **1**, optional — HERE `$1000` reminder — **do not bump**)
- `docs/ENVIRONMENT-QUERY.md` (wave19 **3**, optional — pictured continue cite; **not** full ANS ENVIRONMENT? reopen)
- `docs/HOST-PARITY.md` (wave8 **4**, optional — `sharp-demo` CONTRACT welcome)
- `docs/MIN-MAX.md` (wave24 **2** — **prefer leave untouched**; do not redefine min-/max-mark; kernel MAX-* ≠ ANS MAX)
- `docs/SPACE-SPACES-TYPE.md` / `docs/ABS-NEGATE.md` / `docs/U-LESS.md` / `docs/CHAR-PLUS.md` (leave primaries untouched)
- Prior: SEARCH-WORDLIST / ZERO-EQUALS / LSHIFT / COMPARE / ACCEPT / BITWISE / TRUE-FALSE / WITHIN / COUNT / EXECUTE / SOURCE-PAD / CHAR-CHARS (keep cites; leave locked)
- `forth/tritium/kernel.fs` (sharp-/sharp-end-/sign-mark only; do not bump HERE; do not redefine hold-/to-number-/base-/min-/max-/prior mirrors; optional `#S` when free; kernel MAX-* untouched)
- ANS Forth `#`/`#>`/`SIGN`/optional `#S` (thin continue only — prefer mirrors)
- Explicit deferral: WAVE19–24 PROPOSAL (thin pictured continue beside HOLD; not HOLD reopen; not full rewrite; HERE `$1000` stays)
- Base tip: `fca71c4a` / `fca71c4a8c5ad8dd3d3fd3f3646820e9ce313d9b` (#113 wave24 tip2 MIN-MAX PASS)
- Wave24 proposal: `/workspace/tritium-research-docs/WAVE24-PROPOSAL.md`

## 10. Shipper implementation notes (Linux SoT)

Normative for Shipper on branch `shipper/sharp-sign-w24` from base `fca71c4a8c5ad8dd3d3fd3f3646820e9ce313d9b`. Do **not** authorize merge, push, Mango, opaque-weight ML, or reopening closed tips / MIN-MAX primary / SPACE primary / WAVE24-PROPOSAL.

### 10.1 Binding order

1. Prefer mirrors **`sharp-mark`** / **`sharp-end-mark`** / **`sign-mark`** first — Lab greps markers, not bare ANS names.
2. Bare `#`/`#>`/`SIGN` may alias same markers if free — **must not** bump `_here`/`HERE`/`here-at` or redefine `HOLD`/`hold-mark`/`hold-demo`/`<#`.
3. Optional `#S`/`sharps-mark` when free — **do not require**. Never alias onto full pictured rewrite / HOLD reopen / HERE bump.
4. Never alias onto hold-/to-number-/base-/min-/max-mark or `_here`/`HERE`/`here-at`. Pictured continue ≠ start / parse / radix / minmax / pointer.

### 10.2 Classic pictures (# + #> + SIGN)

Document one fixed fixture per required word. Recommended:

| Field | Example | Notes |
|-------|---------|-------|
| `#` ud / digit | ud fixture → digit char `'0'+n` or ord | Classic convert-one-digit picture |
| optional `#` `c=` / `u=` | `c=5` / `u=53` | Char / ord echo welcome |
| `#>` pictured-end | pictured-end → length / address | Classic pictured-end picture |
| optional `#>` `u=` / `addr=` | `u=1` / `addr=…` | Length / address echo welcome |
| `SIGN` n | `n=-7` | Classic negative → hold `-` picture |
| optional `SIGN` `n=` | `n=-7` | Signed echo welcome |
| optional `#S` | `[sharp] #S` | Welcome when free — not required |

Shipper may use other pictured stub fixtures — document which. Lab greps `[sharp] #` + `[sharp] #>` + `[sharp] SIGN` regardless of fixtures, as long as all three markers are greppable and FAIL is avoided. **Do not** implement pictured continue by bumping HERE stub `$1000`. **Do not** implement by redefining `hold-mark` / `HOLD` / `<#`. **Do not** reopen BASE / TO-NUMBER / MIN-MAX / SPACE-SPACES-TYPE.

### 10.3 What Lab greps / does not grep

**Required:** `[sharp-demo] OK` + `[sharp] #` + `[sharp] #>` + `[sharp] SIGN`. **Optional welcome:** `u=`/`c=`/`addr=`/`n=`; `[sharp] #S` when free; `sharp-demo CONTRACT`. **Must NOT Lab-grep as success:** `_here`/`HERE`/`here-at`/`[allot]`; `[hold]`/`hold-mark`/`HOLD`/`<#`; `[number]`/`to-number-mark`; `[base]`*; `[minmax]`/`min-mark`/`max-mark`; `[space]`/`space-mark`/`spaces-mark`/`type-mark`/`emit-mark`; `[abs]`/`abs-mark`/`negate-mark`; `[uless]`/`u-less-mark`; `[char+]`/`char-plus-mark`; `[zero]`/`[shift]`/`[bit]`/`[compare]`/`[cell]`; `MAX-ENTRIES`/`MAX-SYNONYMS`/`MAX-CHAR`; `WORDLIST` / DOCS-CITES.

### 10.4 Regression + companions + non-goals

Retain green: `minmax-demo` / `space-demo` / `abs-demo` / `uless-demo` / `hold-demo` / `charplus-demo` / `search-demo` / `number-demo` / `zero-demo` / `shift-demo` / `bit-demo` / `compare-demo` / `base-demo` / `accept-demo` / `exec-demo` / `count-demo` / `within-demo` / `true-demo` / `string-demo` / `word-demo` / `env-demo` / `cell-demo` / `allot-demo` / `kernel-demo` / `trit-math-demo` / `fold-demo` (+ earlier). Required thin: HOLD + TO-NUMBER + BASE-HEX + KERNEL. Optional: ALLOT-HERE / ENVIRONMENT-QUERY / HOST-PARITY. **Untouched:** ARCH / GAPS / WAVE24-PROPOSAL / COS-PASTE / MIN-MAX / SPACE-SPACES-TYPE / CHAR-PLUS / ABS-NEGATE / U-LESS primaries.

Do **not**: redefine hold/to-number/base/min/max/abs/u-less/zero/within/true/space/charplus mirrors; bump HERE `$1000`; reopen HOLD/BASE/TO-NUMBER/MIN-MAX/SPACE; land full pictured rewrite / tip4 WORDLIST / tip5 DOCS-CITES; amend ARCH/GAPS / locked primaries / WAVE24-PROPOSAL; confuse ANS MAX with kernel `MAX-*`; tip Shipper / push / merge / touch Mango; redefine host 2dup/2drop/2swap; polish ABORT".

SHARP-SIGN = thin pictured-numeric **continue** only. Sibling to HOLD start / TO-NUMBER / BASE-HEX — **not** a reopen. Prefer `sharp-mark` / `sharp-end-mark` / `sign-mark`. Classic: `#` → convert-one-digit; `#>` → pictured-end; `SIGN` → sign-prefix. Emit all three markers → `[sharp-demo] OK`.

Research drafts under `/workspace/tritium-research-docs/`. Shipper byte-copies into `docs/` on `shipper/sharp-sign-w24` only. Research does **not** tip Shipper / push / merge / touch Mango. Still deferred: tip4–5; full pictured rewrite; HOLD/BASE/TO-NUMBER reopen; FIND/linked dict; boolean cell; full Win/Android VM; 2DUP-FAMILY; ABORT" polish; Mango.

Acceptance one-liner: `sharp-demo` prints greppable `[sharp] #` + `[sharp] #>` + `[sharp] SIGN` and ends `[sharp-demo] OK`; prior hold/minmax demos still OK; hold-mark / HERE `$1000` untouched; Linux SoT; no merge. Paste anchors: WAVE24-PROPOSAL § Tip 3; base `fca71c4a8c5ad8dd3d3fd3f3646820e9ce313d9b`; branch `shipper/sharp-sign-w24`; handoff `WAVE24-TIP3-HANDOFF.md` (= `TIP3-HANDOFF.md`).

### 10.5 Disambiguation (Shipper)

| Surface | Tip | This tip? |
|---------|-----|-----------|
| `#` / `sharp-mark` | wave24 **3** | **YES** |
| `#>` / `sharp-end-mark` | wave24 **3** | **YES** |
| `SIGN` / `sign-mark` | wave24 **3** | **YES** |
| `#S` / `sharps-mark` | wave24 **3** optional | YES when free — **do not require** |
| `HOLD` / `hold-mark` / `<#` | wave23 **2** | NO — HOLD thin amend only; **NOT** HOLD reopen |
| `_here` / `HERE` / `here-at` | wave14 **1** | NO — HERE stub `$1000` untouched; **do not bump** |
| `>NUMBER` / `to-number-mark` | wave22 **3** | NO — TO-NUMBER thin amend only |
| `BASE`/`HEX`/`DECIMAL` / base-*/hex-*/decimal-* | wave21 **2** | NO — BASE-HEX thin amend only |
| `MIN`/`MAX` / min-mark/max-mark | wave24 **2** | NO — leave MIN-MAX.md untouched |
| kernel `MAX-ENTRIES` / `MAX-SYNONYMS` / `MAX-CHAR` | kernel caps | **NO** — leave untouched |
| `SPACE`/`SPACES`/`TYPE` / space-*/type-*/emit-* | wave24 **1** | NO — leave SPACE-SPACES-TYPE.md untouched |
| `ABS`/`NEGATE` / abs-mark/negate-mark | wave23 **4** | NO — leave ABS-NEGATE.md untouched |
| `U<` / `u-less-mark` | wave23 **3** | NO — leave U-LESS.md untouched |
| `CHAR+` / `char-plus-mark` | wave23 **1** | NO — leave CHAR-PLUS.md untouched |
| `WORDLIST` / `FORTH-WORDLIST` | wave24 **4** | NO — tip4 |
| DOCS-CITES (ARCH/GAPS) | wave24 **5** | NO — tip5 |

Pictured continue ≠ HOLD start ≠ `>NUMBER` ≠ BASE ≠ MIN/MAX ≠ HERE. Leave locked primaries (§8) byte-identical. HERE `$1000` + hold-mark untouched. Docs ONLY under `/workspace/tritium-research-docs/`. Skip 2DUP-FAMILY + ABORT" polish.

### 10.6 Marker grammar + boundaries

Preferred: `[sharp] # c=5 u=53` + `[sharp] #> u=1` + `[sharp] SIGN n=-7` — minimum all three markers — close `[sharp-demo] OK` — avoid FAIL. Optional `[sharp] #S` when free. SHARP-SIGN = thin pictured **continue** only — sibling to HOLD/TO-NUMBER/BASE-HEX; **not** a reopen. HERE `$1000` + hold-mark/`HOLD`/`<#` + MIN-MAX primary untouched. Tip4 WORDLIST / tip5 DOCS-CITES later. Full pictured rewrite out.

### 10.8 Prior demos + handoff

Retain green (abbrev): `minmax-demo` / `space-demo` / `abs-demo` / `uless-demo` / `hold-demo` / `charplus-demo` / `search-demo` / `number-demo` / `zero-demo` / `shift-demo` / `bit-demo` / `compare-demo` / `base-demo` / `accept-demo` / `exec-demo` / `count-demo` / `within-demo` / `true-demo` / `string-demo` / `word-demo` / `env-demo` / `cell-demo` / `allot-demo` / `kernel-demo` / `trit-math-demo` / `fold-demo` (+ earlier suite).

Wave24 LOCKED: 1 SPACE-SPACES-TYPE → 2 MIN-MAX → **3 SHARP-SIGN** → 4 WORDLIST → 5 DOCS-CITES. Base tip2 PASS #113 @ `fca71c4a8c5ad8dd3d3fd3f3646820e9ce313d9b`. Branch `shipper/sharp-sign-w24` — **no merge**. Handoff: `WAVE24-TIP3-HANDOFF.md` (= `TIP3-HANDOFF.md`). Research does **NOT** tip Shipper / push / merge / touch Mango. Closes GAPS pictured-beyond-HOLD as **thin continue only**. HERE `$1000` untouched. Prefer mirrors. Lab greps `[sharp-demo] OK` + `[sharp] #` + `[sharp] #>` + `[sharp] SIGN`. Skip 2DUP-FAMILY + ABORT" polish. Stay out of Mango.
