# HOST-PARITY — Win / Android smoke markers (Linux SoT)

**Status:** Shipper-ready stub spec (wave8 item **4**; thin amend wave21 **4** BITWISE companion cite; thin amend wave22 **1** LSHIFT-RSHIFT companion cite; thin amend wave22 **2** ZERO-EQUALS companion cite; thin amend wave22 **3** TO-NUMBER companion cite; thin amend wave22 **4** SEARCH-WORDLIST companion cite; thin amend wave23 **1** CHAR-PLUS companion cite; thin amend wave23 **2** HOLD companion cite; thin amend wave23 **3** U-LESS companion cite; thin amend wave23 **4** ABS-NEGATE companion cite; thin amend wave24 **1** SPACE-SPACES-TYPE companion cite; thin amend wave24 **2** MIN-MAX companion cite; thin amend wave24 **3** SHARP-SIGN companion cite)  
**Canonical brief:** Priority-1 hosts; Linux demos are Lab source of truth  
**Companions:** `docs/BUILD.md`, `docs/INSTALL.md`, `docs/KERNEL.md`, `docs/FLEET.md`, `docs/MASTER.md`, `docs/LINEOS-BRAND.md`, `docs/GROUPS-NESTED.md`, `docs/INTERPRET.md`, `docs/USERLAND.md`; `docs/ENVIRONMENT-QUERY.md` (wave19 **3** — `env-demo` CONTRACT parity welcome); `docs/TRUE-FALSE.md` (wave20 **1** — `true-demo` CONTRACT parity welcome); `docs/BASE-HEX.md` (wave21 **2** — `base-demo` CONTRACT parity welcome); `docs/BITWISE.md` (wave21 **4** — `bit-demo` CONTRACT parity welcome); `docs/LSHIFT-RSHIFT.md` (wave22 **1** — `shift-demo` CONTRACT parity welcome); `docs/ZERO-EQUALS.md` (wave22 **2** — `zero-demo` CONTRACT parity welcome); `docs/TO-NUMBER.md` (wave22 **3** — `number-demo` CONTRACT parity welcome); `docs/SEARCH-WORDLIST.md` (wave22 **4** — `search-demo` CONTRACT parity welcome); `docs/CHAR-PLUS.md` (wave23 **1** — `charplus-demo` CONTRACT parity welcome); `docs/HOLD.md` (wave23 **2** — `hold-demo` CONTRACT parity welcome); `docs/U-LESS.md` (wave23 **3** — `uless-demo` CONTRACT parity welcome); `docs/ABS-NEGATE.md` (wave23 **4** — `abs-demo` CONTRACT parity welcome); `docs/SPACE-SPACES-TYPE.md` (wave24 **1** — `space-demo` CONTRACT parity welcome); `docs/MIN-MAX.md` (wave24 **2** — `minmax-demo` CONTRACT parity welcome); `docs/SHARP-SIGN.md` (wave24 **3** — `sharp-demo` CONTRACT parity welcome)

## 1. Purpose

Wave7–8 demos (`kernel-demo`, `fleet-demo`, `master-demo`, `lineos-brand-demo`, `group-nested-demo` / vocab-persist, `interpret-demo`, `userland-demo`, …) are green on **Linux**. This tip adds **contract markers** (and light .NET smoke where available) so Win / Android report the same greppable OK / SKIP / CONTRACT lines — not a full VM rewrite.

## 2. Policy

| Host | Expectation this tip |
|------|----------------------|
| Linux | SoT — full demos already gate |
| Windows | Prefer real smoke if `TritiumOS.exe` / host REPL can print markers; else **contract** file + stub command |
| Android | **Contract-only OK** if SDK/emulator missing (Lab precedent) |

Never fail the tip solely for missing Android SDK.

## 3. Contract files

```
install/hosts/windows/parity/
  HOST-PARITY.txt      # lists demo → OK|CONTRACT|SKIP
install/hosts/android/parity/
  HOST-PARITY.txt      # same schema
docs/HOST-PARITY.md    # this doc
```

`HOST-PARITY.txt` schema (one demo per line):

```
kernel-demo OK|CONTRACT|SKIP
fleet-demo …
master-demo …
lineos-brand-demo …
group-nested-demo …
group-vocab-persist-demo …
interpret-demo …
userland-demo …
lineos-confirm-demo …
```

Minimum for PASS: file present on Win + Android paths; every listed demo has a status; Linux suite still green.

## 4. Surfaces

| Surface | Behavior |
|---------|----------|
| `host-parity-demo` | Linux: print SoT note + verify both parity files exist in tree → OK |
| Win stub | Optional: print `[host-parity] windows CONTRACT` reading the txt |
| Android stub | Optional: same with `android` |

## 5. Markers

```
[host-parity] linux SoT
[host-parity] windows OK|CONTRACT|SKIP
[host-parity] android OK|CONTRACT|SKIP
[host-parity-demo] OK
[host-parity-demo] FAIL
```

## 6. Non-goals

- Porting every Forth demo into C#/Kotlin this tip
- Requiring emulator/SDK in CI
- Changing Linux SoT behavior
- Full ANS `ENVIRONMENT?` table / SEARCH-WORDLIST on Win/Android — query mark only → `docs/ENVIRONMENT-QUERY.md` (wave19 **3**); `env-demo CONTRACT` acceptable
- Real boolean cell / `TRUE`/`FALSE` rewrite on Win/Android — constant marks only → `docs/TRUE-FALSE.md` (wave20 **1**); `true-demo CONTRACT` acceptable
- Real number parser / `BASE`/`HEX`/`DECIMAL` rewrite on Win/Android — base marks only → `docs/BASE-HEX.md` (wave21 **2**); `base-demo CONTRACT` acceptable; do not break HERE stub base
- Real boolean / bitwise ALU rewrite on Win/Android — bitwise marks only → `docs/BITWISE.md` (wave21 **4**); `bit-demo CONTRACT` acceptable; do not redefine host lowercase `and`/`or`
- Real shift ALU rewrite on Win/Android — shift marks only → `docs/LSHIFT-RSHIFT.md` (wave22 **1**); `shift-demo CONTRACT` acceptable; do not redefine host lowercase `lshift`/`rshift`
- Real boolean-cell / flag-ALU rewrite on Win/Android — flag marks only → `docs/ZERO-EQUALS.md` (wave22 **2**); `zero-demo CONTRACT` acceptable; do not redefine host `0=`/`0<>`
- Real number parser / pictured numeric rewrite on Win/Android — thin number-parse mark only → `docs/TO-NUMBER.md` (wave22 **3**); `number-demo CONTRACT` acceptable; do not bump HERE stub base `$1000`; prefer `to-number-mark`; not BASE reopen
- Real linked-dict / FIND reopen / wordlist-stack rewrite on Win/Android — thin vocab-search mark only → `docs/SEARCH-WORDLIST.md` (wave22 **4**); `search-demo CONTRACT` acceptable; do not redefine host find/find-xt/find-mark; prefer `search-wl-mark`; not FIND reopen
- Real unicode / XCHAR / CHAR-CHARS reopen on Win/Android — thin char-unit advance mark only → `docs/CHAR-PLUS.md` (wave23 **1**); `charplus-demo CONTRACT` acceptable; do not redefine CHAR/CHARS/[CHAR]/char-unit/chars-n/bracket-char; prefer `char-plus-mark`; not unicode/XCHAR / CHAR-CHARS reopen / ALIGN reopen / HOLD
- Real pictured numeric `#S`/`#>`/`SIGN` / HERE-bump on Win/Android — thin pictured-numeric start only → `docs/HOLD.md` (wave23 **2**); `hold-demo CONTRACT` acceptable; do not redefine/alias/bump `_here`/`HERE`/`here-at` — HERE stub stays `$1000`; prefer `hold-mark`; do not redefine to-number-mark / base-mark/hex-mark/decimal-mark / char-plus-mark; not full `#S`/`#>`/`SIGN` / BASE reopen / TO-NUMBER reopen
- Real unsigned compare / boolean-cell / WITHIN runtime on Win/Android — thin unsigned compare flag mark only → `docs/U-LESS.md` (wave23 **3**); `uless-demo CONTRACT` acceptable; prefer `u-less-mark`; do not redefine zero-eq-mark/zero-ne-mark / compare-mark / within-mark / hold-mark / char-plus-mark / true-mark/false-mark; not WITHIN reopen / ZERO-EQUALS reopen / COMPARE reopen / boolean cell
- Real signed magnitude / negate / boolean-cell / BITWISE ALU rewrite on Win/Android — thin signed magnitude / negate marks only → `docs/ABS-NEGATE.md` (wave23 **4**); `abs-demo CONTRACT` acceptable; prefer `abs-mark`/`negate-mark`; do not redefine and-mark/or-mark/xor-mark/invert-mark / lshift-mark/rshift-mark / true-mark/false-mark / zero-eq-mark / u-less-mark / hold-mark / char-plus-mark; not boolean cell / BITWISE reopen / LSHIFT reopen; NEGATE ≠ INVERT
- Real console I/O / STRING-LIT reopen / full output VM on Win/Android — thin SPACE/SPACES/TYPE (+ optional EMIT) output marks only → `docs/SPACE-SPACES-TYPE.md` (wave24 **1**); `space-demo CONTRACT` acceptable; prefer `space-mark`/`spaces-mark`/`type-mark`/optional `emit-mark`; do not redefine `S"`/`."`/`.(`/`string-s`/`string-dot`/`string-paren` / `WORD`/`BL`/`word-mark`/`bl-mark`; not STRING-LIT reopen / full I/O / WORD-BL reopen
- Signed min/max marks only on Win/Android — thin MIN/MAX marks → `docs/MIN-MAX.md` (wave24 **2**); `minmax-demo CONTRACT` acceptable; prefer `min-mark`/`max-mark`; do not redefine `abs-mark`/`negate-mark` / `u-less-mark` / `zero-eq-mark` / `within-mark` / `true-mark`/`false-mark`; **CRITICAL:** kernel `MAX-ENTRIES`/`MAX-SYNONYMS`/`MAX-CHAR` ≠ ANS `MAX`; not boolean cell / ABS / U-LESS / ZERO-EQUALS / WITHIN reopen
- Thin pictured-numeric continue only on Win/Android — thin `#`/`#>`/`SIGN` (+ optional `#S`) → `docs/SHARP-SIGN.md` (wave24 **3**); `sharp-demo CONTRACT` acceptable; prefer `sharp-mark`/`sharp-end-mark`/`sign-mark`; do not redefine `HOLD`/`hold-mark`/`hold-demo`/`<#` / `to-number-mark` / `base-mark`/`hex-mark`/`decimal-mark`; **CRITICAL:** do not bump `_here`/`HERE`/`here-at` — HERE `$1000` stays; not HOLD reopen / full pictured rewrite / BASE/TO-NUMBER reopen

## 7. Acceptance (Test Lab)

1. `docs/HOST-PARITY.md` present (Research byte-copy OK).
2. Win + Android `parity/HOST-PARITY.txt` present with schema.
3. `host-parity-demo` → OK; Linux regression green.
4. Android CONTRACT-only acceptable without SDK.
5. No merge.

## 8. Cite

- `docs/BUILD.md`, `docs/INSTALL.md`, `docs/SYSTEM-DESIGN-INITIAL-PLATFORMS.md`
- Wave7–8 demo docs listed in §1 companions
- `docs/ENVIRONMENT-QUERY.md` (wave19 **3** — `env-demo` CONTRACT parity)
- `docs/TRUE-FALSE.md` (wave20 **1** — `true-demo` CONTRACT parity)
- `docs/BASE-HEX.md` (wave21 **2** — `base-demo` CONTRACT parity)
- `docs/BITWISE.md` (wave21 **4** — `bit-demo` CONTRACT parity)
- `docs/LSHIFT-RSHIFT.md` (wave22 **1** — `shift-demo` CONTRACT parity)
- `docs/ZERO-EQUALS.md` (wave22 **2** — `zero-demo` CONTRACT parity)
- `docs/TO-NUMBER.md` (wave22 **3** — `number-demo` CONTRACT parity)
- `docs/SEARCH-WORDLIST.md` (wave22 **4** — `search-demo` CONTRACT parity)
- `docs/CHAR-PLUS.md` (wave23 **1** — `charplus-demo` CONTRACT parity)
- `docs/HOLD.md` (wave23 **2** — `hold-demo` CONTRACT parity)
- `docs/U-LESS.md` (wave23 **3** — `uless-demo` CONTRACT parity)
- `docs/ABS-NEGATE.md` (wave23 **4** — `abs-demo` CONTRACT parity)
- `docs/SPACE-SPACES-TYPE.md` (wave24 **1** — `space-demo` CONTRACT parity)
- `docs/MIN-MAX.md` (wave24 **2** — `minmax-demo` CONTRACT parity)
- `docs/SHARP-SIGN.md` (wave24 **3** — `sharp-demo` CONTRACT parity)
