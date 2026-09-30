# COMPARE — ANS `COMPARE` string mark + `compare-demo`

**Status:** Shipper-ready stub spec (wave21 item **3**; thin amend wave23 **3** U-LESS companion cite)
**Canonical brief:** ANS-shaped `COMPARE` (thin string-compare mark only); `docs/STRING-LIT.md` (wave13 **4** — string-lit companion); `docs/FILL-MOVE.md` (wave15 **3** — fixed host-buffer companion); `docs/KERNEL.md` (wave7 **5**); optional `docs/COUNT.md` (wave20 **3** — counted-string picture companion; host ENTRY-COUNT ≠ ANS COUNT) / `docs/WORD-BL.md` (wave18 **3** — fixed word-buffer companion) / `docs/WITHIN.md` (wave20 **2** — range-check companion; **not** WITHIN reopen); explicit WAVE19 / WAVE20 / WAVE21 deferral closed as ANS string-compare mark only (not SEARCH-WORDLIST / FIND reopen / full string heap / WITHIN reopen; **critical:** host `cstr=` is **not** ANS `COMPARE`); `docs/U-LESS.md` (wave23 **3** — sibling unsigned compare flag mark; **not** a COMPARE reopen; prefer `u-less-mark`).
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `compare.fs`); Linux host REPL; **do not** redefine host `COMPARE` that already binds on the load path; **do not** redefine, alias, or Lab-grep host `cstr=` as this tip’s surface
**Companions:** `docs/STRING-LIT.md` (thin amend this tip), `docs/FILL-MOVE.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip); optional light cite `docs/COUNT.md` / `docs/WORD-BL.md` / `docs/WITHIN.md`; `docs/U-LESS.md` (wave23 **3** — thin companion cite; sibling unsigned compare flag mark — **not** a COMPARE reopen; prefer `u-less-mark`; do not redefine `compare-mark`; host `cstr=` ≠ ANS COMPARE)
**Base tip SHA:** `46867efe` (wave21 tip2 PASS / #98 BASE-HEX) / full `46867efe8001c9c96e691bae47a1b0dda2692826`

## 1. Purpose

WAVE13 landed string-literal parse stubs (`docs/STRING-LIT.md`); WAVE15 landed fixed host-buffer FILL/MOVE (`docs/FILL-MOVE.md`); WAVE20 tip **3** landed ANS `COUNT` counted-string picture (`docs/COUNT.md`); WAVE20 tip **2** landed `WITHIN` range-check mark (`docs/WITHIN.md` — **not** a string compare). WAVE18 / WAVE19 / WAVE20 / WAVE21 explicitly deferred ANS `COMPARE` (string compare mark; **critical:** host `cstr=` in `kernel.fs` is a length+bytes compare helper — **not** ANS COMPARE). This tip lands a **stub** ANS string-compare mark only: `COMPARE` (or Forth mirror **`compare-mark`**) prints `[compare] COMPARE` (+ optional `flag=` / `n=` for equal / before / after picture) for **fixed demo string pair fixtures** (equal required + optional unequal). Equal → greppable `flag=0` or `n=0` (classic ANS **0 = equal**); optional before/after may show `flag=-1` / `flag=1` or `n=` (document the picture; demo may show one unequal or avoids FAIL). Smoke via **`compare-demo`**. Prefer Forth mirror **`compare-mark`** whenever host `COMPARE` collides — and **never** treat host `cstr=` as this tip’s surface (do **not** redefine, alias, or Lab-grep `cstr=`). **Not** SEARCH-WORDLIST, not FIND reopen, not full string heap / BLOCK / escape rewrite, not WITHIN reopen (wave20 range-check stays mark-only), not COUNT reopen, not BASE-HEX / ACCEPT-REFILL reopen. Closes a string-compare deferral as **thin mark** beside wave20 COUNT / wave13 STRING-LIT without promoting either. Independent of tip2 BASE-HEX and tip4 BITWISE. Wave23 tip **3** lands `U<` unsigned compare flag mark (`docs/U-LESS.md`): sibling **unsigned compare** beside ANS string-compare — **not** a COMPARE reopen / SEARCH-WORDLIST / FIND reopen / full string heap / WITHIN reopen / ZERO-EQUALS reopen / boolean cell; prefer `u-less-mark`; **do not** redefine `compare-mark`; host `cstr=` still ≠ ANS COMPARE.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `COMPARE` / `compare-mark` | `( c-addr1 u1 c-addr2 u2 -- n )` *or* `( -- )` with fixed demo string pair | ANS string-compare picture mark; print `[compare] COMPARE` (+ optional `flag=<n>` / `n=<n>`); classic ANS: **0** = equal, **-1** = before (str1 < str2), **1** = after (str1 > str2) |
| `compare-demo` | `( -- )` | See §5 |

Host note: bind bare `COMPARE` on the Linux REPL **only if** that name does not collide with a host Forth `COMPARE` in the same load path. Prefer Forth mirror **`compare-mark`** as the Lab-facing surface when in doubt — **do not** redefine host `COMPARE`. **Critical disambiguation:** host `cstr=` (`kernel.fs`, length+bytes compare helper used by dict / find paths) is **NOT** ANS `COMPARE`. Do **not** redefine, alias, Lab-grep, or document `cstr=` as this tip’s surface. Optional `flag=` / `n=` are host ints / compare-result echo only — not a live SEARCH-WORDLIST, not FIND rewrite, not WITHIN reopen, not a heap. Prefer **fixed demo string pair fixtures** (equal required; optional unequal) so Lab hit is deterministic and FAIL is avoided.

## 3. Stub semantics

- **`COMPARE` / `compare-mark`:** take (or use fixed demo) string pair `( c-addr1 u1 c-addr2 u2 )`. Classic ANS picture: compare the two strings lexicographically by length+bytes → result `n` where **`n=0`** means equal, **`n=-1`** means str1 is before (less than) str2, **`n=1`** means str1 is after (greater than) str2. Print `[compare] COMPARE` and optionally `flag=<n>` and/or `n=<n>` (either form Lab-greppable; prefer documenting which Shipper emits). **Does not** rewrite FIND / SEARCH-WORDLIST / STRING-LIT parse / COUNT picture / WITHIN range-check / FILL host buffer / WORD-BL word-buffer. Does **not** allocate, bump HERE, open a heap, or create dict entries. Captured values are host ints / fixture echo only.
- **Fixed demo string pair fixtures (document):**
  - **Equal (required):** e.g. `"abc"` vs `"abc"` (or any identical pair) → `[compare] COMPARE` with greppable **`flag=0`** or **`n=0`** (classic ANS 0 = equal). Prefer short ASCII fixture so Lab is deterministic.
  - **Unequal / before / after (optional):** e.g. `"abc"` vs `"abd"` → before (`flag=-1` / `n=-1`) or after (`flag=1` / `n=1`) depending on order — **document** the picture; demo may show **one** unequal or avoids FAIL. Miss/unequal is **not** FAIL.
- **Optional push:** if the host stack is easy, push the pictured `n` (`0` / `-1` / `1`); marker alone is enough for Lab OK — do not require a real string heap / SEARCH-WORDLIST.
- **FAIL:** `[compare] FAIL reason=<…>` optional (demo **must avoid**). Prefer not emitting `[compare] FAIL` on the happy path. Unequal is **not** FAIL — unequal is `flag=-1`/`flag=1` or `n=-1`/`n=1`.
- Storage: fixed demo string pair fixtures / host buffer echo only. **No** SEARCH-WORDLIST, no FIND reopen, no full string heap / BLOCK / escape rewrite, no WITHIN reopen, no COUNT reopen, no HERE bump, no arena.
- **`cstr=` stays untouched:** host `cstr=` remains the kernel length+bytes compare helper. This tip’s Lab greps are `[compare] COMPARE` and `[compare-demo] OK` only — **never** Lab-grep `cstr=` / `cstr =` as ANS COMPARE success.
- Nest with prior base / accept / exec / count / within / true / source / env / body / char / state / word / find / tick / create / allot / synonym / exit / buffer / marker / defer / imm / fill / pick / cell / throw / 2var / colon / control / string stubs OK. `dict-reset` unaffected (no new dict entries from compare marks — fixed host fixtures, not dictionary).
- Still no SEARCH-WORDLIST / FIND reopen, no full string heap, no WITHIN reopen, no tip4 BITWISE / tip5 DOCS-CITES, no linked XT / real DOES> XT / real branch XT / full arena/heap / full Win/Android Forth VM. Wave21 tip1 ACCEPT-REFILL + tip2 BASE-HEX + wave20 tip1–4 TRUE-FALSE / WITHIN / COUNT / EXECUTE + wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD stay landed — keep cites; this tip does not reopen them. Those stay non-goals / later tips (COMPARE marks excepted as this tip).

## 4. Markers

```
[compare] COMPARE [flag=<n>] [n=<n>]   # flag=/n= optional; equal → flag=0 or n=0 (required greppable)
[compare] FAIL reason=<…>              # demo avoids
[compare-demo] OK
[compare-demo] FAIL
```

Lab greps `[compare-demo] OK` plus at least one `[compare] COMPARE` with greppable **`flag=0`** or **`n=0`** (equal fixture required). Optional unequal may show `flag=-1` / `flag=1` or `n=-1` / `n=1` (document; demo may show one unequal or avoids FAIL). Demo avoids `[compare] FAIL`. Prefer not emitting `[compare] FAIL` on the happy path. **Do not** Lab-grep `cstr=` / host `cstr =` as this tip’s COMPARE surface. **Do not** Lab-grep `[within]` / `[find]` / `[count]` as COMPARE success (those stay their own tips).

## 5. `compare-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; compare marks need no dict entries.
2. Ensure **equal** fixed demo string pair fixtures exist (e.g. `"abc"` / `"abc"`, or length+bytes identical pair in host buffers / stub c-addrs) so equal picture is deterministic. Fixtures may live beside (not replacing) STRING-LIT parse path / FILL host buffer / WORD-BL word-buffer / COUNT counted-string fixture / SOURCE-PAD pad — document; do **not** require SEARCH-WORDLIST / FIND reopen / WITHIN reopen / full string heap.
3. Invoke `COMPARE` (or **`compare-mark`**) against the equal pair → `[compare] COMPARE` (+ optional `flag=` / `n=`). **Required:** greppable `flag=0` or `n=0`.
4. Optional: invoke against one unequal pair → `[compare] COMPARE` with `flag=-1`/`flag=1` or `n=-1`/`n=1` (document before/after picture). Demo may show one unequal or skip unequal entirely — either OK; do **not** emit `[compare] FAIL`.
5. Assert no `[compare] FAIL` on the happy path. Assert compare mark did **not** require SEARCH-WORDLIST / FIND reopen / full string heap / WITHIN reopen / COUNT reopen / HERE bump / `cstr=` redefine (marker-only is enough). Assert host `COMPARE` was not redefined when using the Forth mirror. Assert host `cstr=` was **not** redefined, aliased, or used as this tip’s Lab surface.
6. Prior `base-demo` / `accept-demo` / `exec-demo` / `count-demo` / `within-demo` / `true-demo` / `source-demo` / `env-demo` / `body-demo` / `char-demo` / `state-demo` / `word-demo` / `find-demo` / `tick-demo` / `recurse-demo` / `eval-demo` / `parse-demo` / `synonym-demo` / `exit-demo` / `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` / `refined-boot-demo` / `host-boot-demo` / `interpret-demo` / `kernel-demo` still OK.
7. `[compare-demo] OK`.

`[compare] COMPARE` with greppable equal `flag=0` or `n=0` is required. FAIL path is not exercised by the demo. Optional unequal / before/after and optional stack push of `n` are not all required for Lab OK when equal picture is greppable. No SEARCH-WORDLIST. No FIND reopen. No full string heap. No WITHIN reopen. No `cstr=` alias.

## 6. Thin amend — companions

### `docs/STRING-LIT.md`

- Companions / Status: add `COMPARE.md` (wave21 **3** companion cite); **keep** COMMENT-PARSE / INTERPRET / KERNEL / CREATE-DOES / THROW-CATCH / PARSE-NAME / WORD-BL / CHAR-CHARS / SOURCE-PAD / COUNT cites — do not wipe wave13 STRING-LIT content.
- Purpose / §3 / non-goals: string-literal parse-until-`"` stays; ANS `COMPARE` is the sibling **string-compare** mark over fixed demo string pair fixtures — **not** an S"/escape heap / STRING-LIT reopen / SEARCH-WORDLIST / FIND reopen / full string heap. Do not wipe wave13 string content. Stress: host `cstr=` ≠ ANS COMPARE.
- Non-goals: ANS `COMPARE` → `docs/COMPARE.md` (wave21 **3**). Full counted-string heap / BLOCK / escape / SEARCH-WORDLIST still out. COUNT stays wave20 **3**. FILL-MOVE stays wave15 **3**.
- Acceptance: Lab smokes `compare-demo` (retains `string-demo` + `count-demo` + `fill-demo`).
- Cite: `docs/COMPARE.md`.

### `docs/FILL-MOVE.md`

- Companions / Status: add `COMPARE.md` (wave21 **3** companion cite); **keep** ALLOT-HERE / KERNEL / BUFFER-COLON / CELL-CELLS / PICK-ROLL / COUNT cites — do not wipe wave15 FILL content.
- Purpose / §3 / non-goals: FILL/ERASE/MOVE/CMOVE stay fixed host-buffer stubs; ANS `COMPARE` may companion fixed string pair fixtures in a separate host buffer / stub — **not** a FILL reopen / arena / ALLOCATE / full string heap / SEARCH-WORDLIST. Do not wipe wave15 FILL content. Stress: host `cstr=` ≠ ANS COMPARE; do not redefine kernel `cmove`.
- Non-goals: ANS `COMPARE` → `docs/COMPARE.md` (wave21 **3**). FILL/ERASE/MOVE/CMOVE stay on this tip (already landed). Full string heap / arena still later. COUNT stays wave20 **3**.
- Acceptance: Lab smokes `compare-demo` (retains `fill-demo` + `count-demo` + `string-demo`).
- Cite: `docs/COMPARE.md`.

### `docs/KERNEL.md`

- Companions: add `COMPARE.md` (wave21 **3**); **keep** wave21 tip1 ACCEPT-REFILL + tip2 BASE-HEX cites and wave20 tip1–4 TRUE-FALSE / WITHIN / COUNT / EXECUTE cites and wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites and wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites and wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `COMPARE` / `compare-mark` stub + `compare-demo` (cite tip; Forth mirror `compare-mark` — ANS string-compare mark only; **do not** redefine host `COMPARE`; **do not** redefine / alias / Lab-grep host `cstr=` as this tip; **not** SEARCH-WORDLIST / FIND reopen / full string heap / WITHIN reopen / COUNT reopen; optional `flag=` / `n=` — equal → `flag=0`/`n=0` required greppable; optional before/after `flag=-1`/`1` or `n=`).
- Non-goals: ANS `COMPARE` string mark → `docs/COMPARE.md`. BASE/HEX/DECIMAL stay on `BASE-HEX.md`. ACCEPT/REFILL stay on `ACCEPT-REFILL.md`. COUNT stays on `COUNT.md`. WITHIN stays on `WITHIN.md`. STRING-LIT / FILL-MOVE / FIND stay on their docs. host `cstr=` stays kernel length+bytes helper — **not** ANS COMPARE. BITWISE still later (wave21 **4**). Tip5 DOCS-CITES still later (wave21 **5**).
- Acceptance: Lab smokes `compare-demo` (and retains `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `source-demo` + `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `string-demo` + `fill-demo` + prior demos).
- Cite: `docs/COMPARE.md`.

### Optional — `docs/COUNT.md`

- Companions / Status: add light `COMPARE.md` (wave21 **3**) cite; **keep** STRING-LIT / WORD-BL / SOURCE-PAD / KERNEL / PARSE-NAME / FILL-MOVE / WORDS-VOCAB / ACCEPT-REFILL cites — do not wipe wave20 COUNT content.
- Purpose / §3 / non-goals: COUNT stays counted-string picture mark; ANS `COMPARE` is a sibling **string-compare** mark over fixed demo string pair fixtures — **not** a COUNT reopen / full counted-string heap / SEARCH-WORDLIST / live TIB rewrite. Do not wipe wave20 COUNT content. Stress: host `cstr=` ≠ ANS COMPARE; host `ENTRY-COUNT` ≠ ANS COUNT (still).
- Non-goals: ANS `COMPARE` → `docs/COMPARE.md` (wave21 **3**). COUNT stays on this tip (already landed). ACCEPT-REFILL stays wave21 **1**.
- Acceptance: Lab smokes `compare-demo` (retains `count-demo` + `string-demo` + `accept-demo`).
- Cite: `docs/COMPARE.md`.

### Optional — `docs/WORD-BL.md`

- Companions / Status: add light `COMPARE.md` (wave21 **3**) cite; **keep** PARSE-NAME / COMMENT-PARSE / INTERPRET / KERNEL / STRING-LIT / CHAR-CHARS / SOURCE-PAD / COUNT / ACCEPT-REFILL cites — do not wipe wave18 WORD-BL content.
- Purpose / §3 / non-goals: WORD/BL stay stub token/pad markers into the **fixed word-buffer**; ANS `COMPARE` is a sibling **string-compare** mark — **not** a word-buffer share / WORD reopen / interpret splitter rewrite / SEARCH-WORDLIST / full string heap. Do not wipe wave18 WORD-BL content. Stress: host `cstr=` ≠ ANS COMPARE.
- Non-goals: ANS `COMPARE` → `docs/COMPARE.md` (wave21 **3**). WORD/BL stay on this tip (already landed). COUNT stays wave20 **3**. ACCEPT-REFILL stays wave21 **1**.
- Acceptance: Lab smokes `compare-demo` (retains `word-demo` + `count-demo` + `string-demo`).
- Cite: `docs/COMPARE.md`.

### Optional — `docs/WITHIN.md`

- Companions / Status: add light `COMPARE.md` (wave21 **3**) cite; **keep** KERNEL / CONTROL / TRUE-FALSE / PICK-ROLL / CELL-CELLS cites — do not wipe wave20 WITHIN content.
- Purpose / §3 / non-goals: WITHIN stays range-check mark (`lo ≤ n < hi`); ANS `COMPARE` is a sibling **string-compare** mark — **not** a WITHIN reopen / runtime compare re-exec / IF/THEN reopen / SEARCH-WORDLIST. Do not wipe wave20 WITHIN content. Stress: host `cstr=` ≠ ANS COMPARE; WITHIN and COMPARE are different surfaces (range-check vs string-compare).
- Non-goals: ANS `COMPARE` → `docs/COMPARE.md` (wave21 **3**). WITHIN stays on this tip (already landed). BITWISE still later (wave21 **4**).
- Acceptance: Lab smokes `compare-demo` (retains `within-demo` + `true-demo` + `count-demo`).
- Cite: `docs/COMPARE.md`.

Do **not** wipe wave21 tip1 ACCEPT-REFILL content or tip2 BASE-HEX content or wave20 tip1–4 TRUE-FALSE / WITHIN / COUNT / EXECUTE content or wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites or wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites or wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites or wave16 DEFER / MARKER / BUFFER / EXIT cites or wave15 CELL / IMMEDIATE / FILL / PICK prior content. Do **not** amend ARCHITECTURE / IMPLEMENTATION-GAPS / BASE-HEX / ACCEPT-REFILL primary this tip (proposal amends are STRING-LIT + FILL-MOVE + KERNEL + optional COUNT / WORD-BL / WITHIN only). **Leave `BASE-HEX.md` and `ACCEPT-REFILL.md` primary untouched** (tip2 land md5 `d12dd22a8502b321568fa370b6ecdd2e` / 21270; tip1 land md5 `6bda5a5170ac71fa3ccf7db28286dd44` / 23948). **Leave `ARCHITECTURE.md` and `IMPLEMENTATION-GAPS.md` untouched.** Tip5 cites come after 1–4 PASS.

### `docs/U-LESS.md` (wave23 **3** thin companion cite)

- Companions / Status: COMPARE cites U-LESS as unsigned compare flag sibling; U-LESS cites COMPARE as string-compare companion (**not** COMPARE reopen).
- Purpose: U-LESS sibling unsigned compare — **NOT** COMPARE reopen; host `cstr=` still ≠ ANS COMPARE; prefer `u-less-mark`; **do not** redefine `compare-mark`.
- Non-goals: `U<` → `docs/U-LESS.md` (wave23 **3**). `COMPARE` / `compare-mark` stay on this tip (already landed). WITHIN / ZERO-EQUALS stay prior. Tip4 ABS-NEGATE / tip5 DOCS-CITES still later.
- Acceptance: Lab smokes `uless-demo` (retains `compare-demo` + `zero-demo` + `within-demo` + `count-demo` + `hold-demo` + `charplus-demo`).
- Cite: `docs/U-LESS.md`.

## 7. Non-goals

- Confusing / redefining / aliasing / Lab-grepping host `cstr=` as ANS `COMPARE`
- SEARCH-WORDLIST / FIND reopen (wave18 **2** FIND already stubbed; keep cites — do not reopen)
- Full string heap / `ALLOCATE` / BLOCK / screen strings / escape rewrite (`\"` etc.)
- `WITHIN` reopen (wave20 **2** — already stubbed; keep cites; optional thin companion cite only — **not** runtime compare re-exec)
- `COUNT` reopen (wave20 **3** — already stubbed; keep cites; optional thin companion cite only; host ENTRY-COUNT ≠ ANS COUNT)
- `S"` / `."` / `.(` reopen (wave13 **4** — already stubbed; keep cites; thin companion amend only)
- `FILL` / `ERASE` / `MOVE` / `CMOVE` reopen (wave15 **3** — already stubbed; keep cites; thin companion amend only; do not redefine kernel `cmove`)
- `WORD` / `BL` reopen (wave18 **3** — already stubbed; keep cites; optional thin companion cite only)
- `BASE` / `HEX` / `DECIMAL` reopen (wave21 **2** — already stubbed; leave BASE-HEX.md primary untouched)
- `ACCEPT` / `REFILL` reopen (wave21 **1** — already stubbed; leave ACCEPT-REFILL.md primary untouched)
- `AND` / `OR` / `XOR` / `INVERT` bitwise marks (wave21 **4**); not boolean cell rewrite
- `U<` unsigned compare flag mark → `docs/U-LESS.md` (wave23 **3**; sibling unsigned compare — **not** a COMPARE reopen; prefer `u-less-mark`; do not redefine `compare-mark`; host `cstr=` ≠ ANS COMPARE)
- Docs cites pass (wave21 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `TRUE` / `FALSE` reopen (wave20 **1** — already stubbed; keep cites)
- `EXECUTE` reopen (wave20 **4** — already stubbed; keep cites)
- `SOURCE` / `PAD` reopen (wave19 **4** — already stubbed; keep cites)
- Real DOES> XT chaining / threaded child runtime body
- Real branch XT / LEAVE jump
- Full Dusk arena / pool / free / fragmentation model
- Linked XT / executing postponed XT / real STATE cell
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live — **skip 2DUP-FAMILY**)
- `ABORT"` polish (already optional-wired inside `throw-demo` — **skip**)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/COMPARE.md` present (Research byte-copy OK); `STRING-LIT.md` + `FILL-MOVE.md` + `KERNEL.md` thin amends present (+ optional `COUNT.md` / `WORD-BL.md` / `WITHIN.md`); wave21 tip1 ACCEPT-REFILL + tip2 BASE-HEX cites, wave20 tip1–4 TRUE-FALSE / WITHIN / COUNT / EXECUTE cites, wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites, wave18 tip1–4 cites, wave17 tip1–4 cites, wave16 DEFER/MARKER/BUFFER/EXIT cites, and wave15 CELL/IMMEDIATE/COLON/KERNEL prior text retained; host `COMPARE` untouched via mirrors; host `cstr=` **not** redefined / aliased / Lab-grepped as ANS COMPARE; `BASE-HEX.md` primary untouched (tip2 land md5 `d12dd22a8502b321568fa370b6ecdd2e` / 21270); `ACCEPT-REFILL.md` primary untouched (tip1 land md5 `6bda5a5170ac71fa3ccf7db28286dd44` / 23948); `ARCHITECTURE.md` + `IMPLEMENTATION-GAPS.md` byte-copy unchanged.
2. `compare-demo` → OK (markers §4; `[compare] COMPARE` greppable; equal → greppable `flag=0` or `n=0` required; optional before/after `flag=-1`/`1` or `n=` welcome; no FAIL on happy path; no SEARCH-WORDLIST / FIND reopen / full string heap / WITHIN reopen / COUNT reopen; no `cstr=` alias). Prior `base-demo` + `accept-demo` + `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `source-demo` + `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `recurse-demo` + `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` + `refined-boot-demo` + `host-boot-demo` + `interpret-demo` + `kernel-demo` still OK.
3. Regression green (wave21 tip1–2 + wave20 tip1–5 + wave19 tip1–5 + wave18 tip1–5 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `compare-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**)
- `docs/STRING-LIT.md` (wave13 **4** — string-lit companion; COMPARE is string-compare mark — not S"/escape heap)
- `docs/FILL-MOVE.md` (wave15 **3** — fixed host-buffer companion; COMPARE may use separate fixtures — not FILL reopen; do not redefine kernel `cmove`)
- `docs/COUNT.md` (wave20 **3**, optional — counted-string picture companion; not COUNT reopen; host ENTRY-COUNT ≠ ANS COUNT)
- `docs/WORD-BL.md` (wave18 **3**, optional — fixed word-buffer companion; not WORD reopen)
- `docs/WITHIN.md` (wave20 **2**, optional — range-check companion; **not** WITHIN reopen / runtime compare re-exec)
- `docs/FIND.md` (wave18 **2** — prior tip; keep cites; do not reopen SEARCH-WORDLIST / FIND)
- `docs/BASE-HEX.md` (wave21 **2** — prior tip; keep cites; leave primary untouched this tip)
- `docs/ACCEPT-REFILL.md` (wave21 **1** — prior tip; keep cites; leave primary untouched this tip)
- `docs/TRUE-FALSE.md` (wave20 **1** — prior tip; keep cites)
- `docs/EXECUTE.md` (wave20 **4** — prior tip; keep cites)
- `docs/SOURCE-PAD.md` (wave19 **4** — prior tip; keep cites)
- `forth/tritium/kernel.fs` (compare-mark only — do not redefine host COMPARE; **do not** redefine/alias/Lab-grep host `cstr=` as ANS COMPARE)
- ANS Forth `COMPARE` (string-compare mark only — `( c-addr1 u1 c-addr2 u2 -- n )`; 0=equal / -1=before / 1=after; not SEARCH-WORDLIST / FIND reopen / full string heap / WITHIN reopen; host `cstr=` ≠ ANS COMPARE)
- Explicit deferral: WAVE19-PROPOSAL + WAVE20-PROPOSAL + WAVE21-PROPOSAL (ANS `COMPARE` — string mark; host `cstr=` ≠ ANS COMPARE)
- Base tip: `46867efe` / `46867efe8001c9c96e691bae47a1b0dda2692826` (#98 wave21 tip2 BASE-HEX PASS)
- `docs/U-LESS.md` (wave23 **3** — sibling unsigned compare flag mark; **not** a COMPARE reopen; prefer `u-less-mark`; do not redefine `compare-mark`; host `cstr=` ≠ ANS COMPARE)
- Wave21 proposal: `/workspace/tritium-research-docs/WAVE21-PROPOSAL.md`
