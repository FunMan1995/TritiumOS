# ACCEPT-REFILL — `ACCEPT` / `REFILL` thin markers + `accept-demo`

**Status:** Shipper-ready stub spec (wave21 item **1**)
**Canonical brief:** ANS-shaped `ACCEPT` / `REFILL` (thin input marks only); `docs/SOURCE-PAD.md` (wave19 **4** — SOURCE/PAD thin marks companion; **do not** reopen / redefine `SOURCE` / `PAD` / `source-mark` / `pad-addr`); `docs/WORD-BL.md` (wave18 **3** — fixed word-buffer companion); `docs/KERNEL.md` (wave7 **5**); optional `docs/COUNT.md` (wave20 **3** — ANS COUNT picture companion; host ENTRY-COUNT ≠ ANS COUNT) / `docs/PARSE-NAME.md` (wave17 **2**) / `docs/INTERPRET.md` (wave8 **1**); explicit WAVE19 / WAVE20 / WAVE21 deferral closed as thin marks only (not full input-buffer VM / live TIB rewrite / SOURCE-PAD reopen / real line editor / COUNT reopen / EVALUATE nested VM).
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `accept.fs` / `refill.fs` / `accept-refill.fs`); Linux host REPL; **do not** redefine host `ACCEPT` / `REFILL` that already bind on the load path; **do not** redefine `SOURCE` / `PAD` / `source-mark` / `pad-addr`
**Companions:** `docs/SOURCE-PAD.md` (thin amend this tip), `docs/WORD-BL.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip); optional light cite `docs/COUNT.md` / `docs/PARSE-NAME.md` / `docs/INTERPRET.md`
**Base tip SHA:** `fb4a7a0` (wave20 tip5 CLOSED / #96 DOCS-CITES) / full `fb4a7a09a8fb400491709f48247bc7431c99e14b`

## 1. Purpose

WAVE19 tip **4** landed thin `SOURCE` / `PAD` marks (`docs/SOURCE-PAD.md`) and deferred `ACCEPT` / `REFILL` / full input-buffer VM / live TIB rewrite. WAVE20 tip **3** landed ANS `COUNT` counted-string picture (`docs/COUNT.md`) beside SOURCE/PAD without ACCEPT/REFILL. WAVE20 / WAVE21 explicitly deferred `ACCEPT` / `REFILL` as a full input-buffer / line-editor surface. This tip lands **stub thin marks only**: `ACCEPT` (or Forth mirror **`accept-mark`**) prints `[accept] ACCEPT` (+ optional `addr=` / `u=` / `n=` — picture of a **fixed demo input fixture** written into an **existing stub buffer** — prefer the wave19 SOURCE-PAD pad slot or a documented sibling stub buffer; **NOT** a live TIB rewrite); `REFILL` (or Forth mirror **`refill-mark`**) prints `[accept] REFILL` (+ optional `flag=` / `u=` — classic refill-ok picture welcome, e.g. `flag=1` / `flag=-1` / documented true). Smoke via **`accept-demo`**. Demo avoids FAIL. Forth mirrors **`accept-mark` / `refill-mark`** so host `ACCEPT` / `REFILL` stay safe. **Do not** redefine `SOURCE` / `PAD` / `source-mark` / `pad-addr`. **Not** a full input-buffer VM, not a live TIB rewrite, not a SOURCE-PAD reopen, not a real line editor, not COUNT reopen, not EVALUATE nested VM. Builds beside wave19 SOURCE-PAD + wave20 COUNT without promoting either. Thinnest next input surface after SOURCE/PAD + COUNT — before tip2 BASE-HEX.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `ACCEPT` / `accept-mark` | `( c-addr +n1 -- +n2 )` *or* `( -- )` with fixed demo fixture | Thin ACCEPT mark; print `[accept] ACCEPT` (+ optional `addr=<n>` / `u=<n>` / `n=<n>`); classic ANS picture: receive up to `+n1` chars into buffer at `c-addr`, return actual length `+n2` — **this tip** pictures a **fixed demo input fixture** into an **existing stub buffer** only |
| `REFILL` / `refill-mark` | `( -- flag )` *or* `( -- )` | Thin REFILL mark; print `[accept] REFILL` (+ optional `flag=<n>` / `u=<n>`); classic refill-ok picture welcome |
| `accept-demo` | `( -- )` | See §5 |

Host note: bind bare `ACCEPT` / `REFILL` on the Linux REPL **only if** those names do not collide with a host Forth `ACCEPT` / `REFILL` in the same load path. Prefer Forth mirrors **`accept-mark` / `refill-mark`** as the Lab-facing surface when in doubt — **do not** redefine host `ACCEPT` / `REFILL`. **Critical:** do **not** redefine, alias, or reopen `SOURCE` / `PAD` / `source-mark` / `pad-addr` (wave19 **4** stays landed). Optional `addr=` / `u=` / `n=` / `flag=` are host ints / stub offsets / length / flag echo only — not a live TIB pointer, not a heap pointer, not a line editor, not SOURCE/PAD reopen, not COUNT reopen. Prefer a **fixed non-empty demo input fixture** into the existing SOURCE-PAD pad slot (or documented sibling stub buffer) so Lab hit is deterministic and FAIL is avoided.

## 3. Stub semantics

- **`ACCEPT` / `accept-mark`:** picture classic ANS ACCEPT against a **fixed demo input fixture** (e.g. chars `hello` / `accept demo`) copied / echoed into an **existing stub buffer** — prefer the wave19 SOURCE-PAD fixed pad slot (cap prefer **84**) or a documented sibling stub buffer beside it. Print `[accept] ACCEPT` and optionally `addr=<n>` (stub offset / pad base echo), `u=<n>` and/or `n=<n>` (pictured accepted length — prefer matching fixture length, e.g. `u=5` / `n=5` for `hello`). **Does not** rewrite the live TIB / interpret input stream / SOURCE/PAD marks / WORD-BL word-buffer / COUNT fixture / EVALUATE nested path. **Does not** open a new heap buffer, bump HERE, or implement a real line editor / keyboard read. Captured values are host ints / fixture echo only.
- **`REFILL` / `refill-mark`:** picture classic ANS REFILL success — print `[accept] REFILL` and optionally `flag=<n>` (classic refill-ok — prefer `flag=1` or `flag=-1` / document; true picture welcome) and/or `u=<n>` (optional stub length / chars-available echo). **Does not** refill a live TIB from a file / block / keyboard, does not reopen SOURCE/PAD, does not nest EVALUATE. Marker alone is enough for Lab OK.
- **Fixed demo input fixture (document):** e.g. host string `hello` (length 5) pictured into the existing SOURCE-PAD pad slot (or sibling stub). Invoke `ACCEPT` / `accept-mark` against that fixture → `[accept] ACCEPT` (+ optional `addr=` / `u=` / `n=`). Alternate short fixture e.g. `ok` → `u=2` / `n=2` also fine — document which Shipper uses. Prefer non-empty (`u≥1` / `n≥1`) so FAIL is avoided.
- **FAIL:** `[accept] FAIL reason=<…>` optional (demo **must avoid**). Prefer not emitting `[accept] FAIL` on the happy path. No required FAIL reason this tip — empty / no-buffer / refill-miss stay out of the demo.
- Storage: fixed demo input fixture into **existing** stub buffer (SOURCE-PAD pad or sibling) + refill-ok flag echo only. **No** live TIB rewrite, no full input-buffer VM, no real line editor, no SOURCE/PAD reopen / redefine, no COUNT reopen, no EVALUATE nested VM, no HERE bump, no arena.
- Nest with prior exec / count / within / true / source / env / body / char / state / word / find / tick / create / allot / synonym / exit / buffer / marker / defer / imm / fill / pick / cell / throw / 2var / colon / control / string stubs OK. `dict-reset` unaffected (no new dict entries from accept marks — fixed host fixture into existing stub buffer).
- Still no full input-buffer VM / live TIB rewrite / real line editor, no SOURCE-PAD reopen, no COUNT reopen, no EVALUATE nested VM, no tip2 BASE-HEX / tip3 COMPARE / tip4 BITWISE / tip5 DOCS-CITES, no linked XT / real DOES> XT / real branch XT / full arena/heap / full Win/Android Forth VM. Wave19 SOURCE-PAD + wave20 COUNT + wave20 tip1–4 TRUE-FALSE / WITHIN / COUNT / EXECUTE stay landed — keep cites; this tip does not reopen them. Those stay non-goals / later tips (ACCEPT/REFILL thin marks excepted as this tip).

## 4. Markers

```
[accept] ACCEPT [addr=<n>] [u=<n>] [n=<n>]   # addr=/u=/n= optional; fixed demo input fixture into existing stub buffer
[accept] REFILL [flag=<n>] [u=<n>]           # flag=/u= optional; classic refill-ok picture welcome
[accept] FAIL reason=<…>                     # demo avoids
[accept-demo] OK
[accept-demo] FAIL
```

Lab greps `[accept-demo] OK` plus at least one `[accept] ACCEPT` (optional `addr=` / `u=` / `n=` welcome; prefer greppable `u=` or `n=` matching fixture length) and one `[accept] REFILL` (optional `flag=` / `u=` welcome; classic refill-ok picture welcome). Demo avoids `[accept] FAIL`. Prefer not emitting `[accept] FAIL` on the happy path. **Do not** Lab-grep `SOURCE` / `PAD` / `source-mark` / `pad-addr` / `[source]` as this tip’s ACCEPT/REFILL surface (those stay wave19 **4**). **Do not** Lab-grep `[count]` / `ENTRY-COUNT` as ACCEPT/REFILL.

## 5. `accept-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; accept marks need no dict entries.
2. Ensure an **existing stub buffer** is available (prefer wave19 SOURCE-PAD pad slot, cap documented prefer **84**, or a documented sibling stub buffer beside it — **do not** redefine / reopen SOURCE/PAD marks). Ensure a **non-empty** fixed demo input fixture exists (e.g. `hello` → `u=5` / `n=5`) so FAIL is avoided.
3. Invoke `ACCEPT` (or **`accept-mark`**) picturing the fixture into that stub buffer → `[accept] ACCEPT` (+ optional `addr=` / `u=` / `n=`). Prefer `u=5` or `n=5` (or documented fixture length) greppable when present.
4. Invoke `REFILL` (or **`refill-mark`**) → `[accept] REFILL` (+ optional `flag=` / `u=`). Prefer classic refill-ok `flag=` greppable when present.
5. Assert no `[accept] FAIL` on the happy path. Assert accept/refill marks did **not** require a live TIB rewrite / full input-buffer VM / real line editor / SOURCE-PAD reopen / COUNT reopen / EVALUATE nested VM / HERE bump (marker-only is enough). Assert host `ACCEPT` / `REFILL` were not redefined when using the Forth mirrors. Assert `SOURCE` / `PAD` / `source-mark` / `pad-addr` were **not** redefined / reopened. Assert prior `source-demo` / `count-demo` still OK.
6. Prior `exec-demo` / `count-demo` / `within-demo` / `true-demo` / `source-demo` / `env-demo` / `body-demo` / `char-demo` / `state-demo` / `word-demo` / `find-demo` / `tick-demo` / `recurse-demo` / `eval-demo` / `parse-demo` / `synonym-demo` / `exit-demo` / `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` / `refined-boot-demo` / `host-boot-demo` / `interpret-demo` / `kernel-demo` still OK.
7. `[accept-demo] OK`.

`[accept] ACCEPT` + `[accept] REFILL` markers are required. FAIL path is not exercised by the demo. Optional `addr=` / `u=` / `n=` / `flag=` echo is not all required for Lab OK when ACCEPT + REFILL lines are greppable. No live TIB rewrite. No full input-buffer VM. No real line editor. No SOURCE/PAD reopen. No COUNT reopen. No EVALUATE nested VM.

## 6. Thin amend — companions

### `docs/SOURCE-PAD.md`

- Companions / Status: add `ACCEPT-REFILL.md` (wave21 **1** companion cite); **keep** WORD-BL / PARSE-NAME / INTERPRET / KERNEL / BUFFER-COLON / STRING-LIT / CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / COUNT cites — do not wipe wave19 SOURCE-PAD content.
- Purpose / §3 / non-goals: SOURCE/PAD stay thin input-string / pad-slot marks; `ACCEPT` / `REFILL` are sibling **thin input marks** that may picture a fixed demo fixture into the **existing** pad slot — **not** a SOURCE/PAD reopen / redefine of `source-mark` / `pad-addr` / live TIB rewrite / full input-buffer VM / real line editor. Do not wipe wave19 SOURCE-PAD content.
- Non-goals: `ACCEPT` / `REFILL` thin marks → `docs/ACCEPT-REFILL.md` (wave21 **1**). Full input-buffer VM / live TIB rewrite / real line editor still out. SOURCE/PAD stay on this tip (already landed). COUNT stays wave20 **3**.
- Acceptance: Lab smokes `accept-demo` (retains `source-demo` + `count-demo` + `word-demo`).
- Cite: `docs/ACCEPT-REFILL.md`.

### `docs/WORD-BL.md`

- Companions / Status: add `ACCEPT-REFILL.md` (wave21 **1** companion cite); **keep** PARSE-NAME / COMMENT-PARSE / INTERPRET / KERNEL / STRING-LIT / CHAR-CHARS / SOURCE-PAD / COUNT cites — do not wipe wave18 WORD-BL content.
- Purpose / §3: WORD/BL stay stub token/pad markers into the **fixed word-buffer**; `ACCEPT` / `REFILL` are sibling **thin input marks** over a fixed demo fixture into an existing stub buffer (prefer SOURCE-PAD pad) — **not** a word-buffer share / WORD reopen / interpret splitter rewrite / full input-buffer VM / live TIB rewrite / real line editor. Do not wipe wave18 WORD-BL content.
- Non-goals: `ACCEPT` / `REFILL` → `docs/ACCEPT-REFILL.md` (wave21 **1**). SOURCE/PAD stay wave19 **4**. COUNT stays wave20 **3**. Full input-buffer VM / live TIB rewrite still later.
- Acceptance: Lab smokes `accept-demo` (retains `word-demo` + `source-demo` + `count-demo`).
- Cite: `docs/ACCEPT-REFILL.md`.

### `docs/KERNEL.md`

- Companions: add `ACCEPT-REFILL.md` (wave21 **1**); **keep** wave20 tip1–4 TRUE-FALSE / WITHIN / COUNT / EXECUTE cites and wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites and wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites and wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `ACCEPT` / `accept-mark`, `REFILL` / `refill-mark` stubs + `accept-demo` (cite tip; Forth mirrors `accept-mark` / `refill-mark` — thin input marks only; **do not** redefine host `ACCEPT` / `REFILL`; **do not** redefine `SOURCE` / `PAD` / `source-mark` / `pad-addr`; **not** full input-buffer VM / live TIB rewrite / real line editor / SOURCE-PAD reopen / COUNT reopen / EVALUATE nested VM; optional `addr=` / `u=` / `n=` — fixed demo fixture into existing stub buffer welcome; optional `flag=` / `u=` — classic refill-ok picture welcome).
- Non-goals: `ACCEPT` / `REFILL` thin marks → `docs/ACCEPT-REFILL.md`. SOURCE/PAD stay on `SOURCE-PAD.md`. COUNT stays on `COUNT.md`. TRUE/FALSE / WITHIN / EXECUTE stay on their docs. BASE/HEX/DECIMAL still later (wave21 **2**). COMPARE still later (wave21 **3**). BITWISE still later (wave21 **4**). Tip5 DOCS-CITES still later (wave21 **5**).
- Acceptance: Lab smokes `accept-demo` (and retains `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `source-demo` + `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + prior demos).
- Cite: `docs/ACCEPT-REFILL.md`.

### Optional — `docs/COUNT.md`

- Companions / Status: add light `ACCEPT-REFILL.md` (wave21 **1**) cite; **keep** STRING-LIT / WORD-BL / SOURCE-PAD / KERNEL / PARSE-NAME / FILL-MOVE / WORDS-VOCAB cites — do not wipe wave20 COUNT content.
- Purpose / §3 / non-goals: ANS `COUNT` stays counted-string picture mark; `ACCEPT` / `REFILL` are sibling **thin input marks** — **not** a COUNT reopen / full counted-string heap / live TIB rewrite / full input-buffer VM. Do not wipe wave20 COUNT content. Stress: host `ENTRY-COUNT` / `SYN-COUNT` / `words-count` ≠ ANS COUNT.
- Non-goals: `ACCEPT` / `REFILL` → `docs/ACCEPT-REFILL.md` (wave21 **1**). Full input-buffer VM / live TIB rewrite / full counted-string heap still out. COUNT stays on this tip (already landed).
- Acceptance: Lab smokes `accept-demo` (retains `count-demo` + `source-demo` + `word-demo`).
- Cite: `docs/ACCEPT-REFILL.md`.

### Optional — `docs/PARSE-NAME.md`

- Companions: add light `ACCEPT-REFILL.md` (wave21 **1**) cite; **keep** COMMENT-PARSE / INTERPRET / STRING-LIT / KERNEL / SYNONYM / WORD-BL / CHAR-CHARS / SOURCE-PAD / COUNT cites — do not wipe wave17 PARSE content.
- Purpose / non-goals: PARSE/PARSE-NAME stay token-parse markers; `ACCEPT` / `REFILL` are sibling **thin input marks** — **not** a parse reopen / tokenizer VM / live TIB rewrite / full input-buffer VM. Do not wipe wave17 PARSE content.
- Non-goals: `ACCEPT` / `REFILL` → `docs/ACCEPT-REFILL.md` (wave21 **1**). WORD/BL stays wave18 **3**. SOURCE/PAD stays wave19 **4**. COUNT stays wave20 **3**.
- Acceptance: Lab smokes `accept-demo` (retains `parse-demo` + `word-demo` + `source-demo`).
- Cite: `docs/ACCEPT-REFILL.md`.

### Optional — `docs/INTERPRET.md`

- Status / companions: cite wave21 **1**; add `ACCEPT-REFILL.md`; **keep** FIND / TICK / PARSE-NAME / EVALUATE-INCLUDE / EXIT-QUIT / IMMEDIATE / STRING-LIT / COMMENT-PARSE / KERNEL / COLON / WORD-BL / STATE-COMPILE / SOURCE-PAD cites.
- Purpose / §2 / §3 / non-goals: interpret loop stays on host find + whitespace split; `ACCEPT` / `REFILL` stub markers are a sibling demo/fixture input surface via `accept-mark` / `refill-mark` — **does not** rewrite the live interpret splitter / TIB / SOURCE/PAD / EVALUATE nested path / real line editor. Do not wipe wave8–20 INTERPRET content. Host WORDS + `[kernel] words (` stay. SOURCE/PAD stay landed.
- Words table: add `ACCEPT` / `accept-mark`, `REFILL` / `refill-mark` + `accept-demo` (cite tip; Forth mirrors — thin marks only).
- Markers: one-line pointer to `[accept]` markers.
- Non-goals: ACCEPT/REFILL thin marks in scope via this tip; keep full input-buffer VM / live TIB rewrite / real line editor / SOURCE-PAD reopen / EVALUATE nested reopen out.
- Acceptance: Lab smokes `accept-demo` (retains `interpret-demo` + `source-demo` + `word-demo` + `parse-demo` + `eval-demo` + prior).
- Cite: `docs/ACCEPT-REFILL.md`.

Do **not** wipe wave20 tip1–4 TRUE-FALSE / WITHIN / COUNT / EXECUTE content or wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites or wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites or wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites or wave16 DEFER / MARKER / BUFFER / EXIT cites or wave15 CELL / IMMEDIATE / FILL / PICK prior content. Do **not** amend ARCHITECTURE / IMPLEMENTATION-GAPS / TRUE-FALSE / WITHIN / EXECUTE / BASE-HEX-related / CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY this tip (proposal amends are SOURCE-PAD + WORD-BL + KERNEL + optional COUNT / PARSE-NAME / INTERPRET only). **Leave `ARCHITECTURE.md`, `IMPLEMENTATION-GAPS.md`, `TRUE-FALSE.md`, `WITHIN.md`, and `EXECUTE.md` untouched.** Tip5 cites come after 1–4 PASS.

## 7. Non-goals

- Full input-buffer VM / live TIB rewrite / interpret splitter replacement
- Real line editor / keyboard read / file-block refill machine
- `SOURCE` / `PAD` reopen / redefine of `source-mark` / `pad-addr` (wave19 **4** — already stubbed; keep cites; thin companion amend only — ACCEPT may picture fixture **into** existing pad, not reopen SOURCE/PAD)
- ANS `COUNT` reopen (wave20 **3** — already stubbed; keep cites; optional thin companion cite only)
- `EVALUATE` / `INCLUDE` nested VM reopen (wave17 **3** — already mark-only; keep cites)
- `WORD` / `BL` reopen (wave18 **3** — already stubbed; keep cites; thin companion amend only)
- `BASE` / `HEX` / `DECIMAL` base marks (wave21 **2**); not number parser / `>NUMBER` / pictured numeric; do not break HERE stub base
- ANS `COMPARE` string mark (wave21 **3**); host `cstr=` ≠ ANS COMPARE
- `AND` / `OR` / `XOR` / `INVERT` bitwise marks (wave21 **4**); not boolean cell rewrite
- Docs cites pass (wave21 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `TRUE` / `FALSE` reopen (wave20 **1** — already stubbed; keep cites; leave TRUE-FALSE.md untouched)
- `WITHIN` reopen (wave20 **2** — already stubbed; keep cites; leave WITHIN.md untouched)
- `EXECUTE` reopen (wave20 **4** — already stubbed; keep cites; leave EXECUTE.md untouched)
- `CHAR` / `CHARS` / `[CHAR]` reopen (wave19 **1** — already stubbed; keep cites)
- `>BODY` reopen (wave19 **2** — already stubbed; keep cites)
- `ENVIRONMENT?` reopen (wave19 **3** — already stubbed; keep cites)
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

1. `docs/ACCEPT-REFILL.md` present (Research byte-copy OK); `SOURCE-PAD.md` + `WORD-BL.md` + `KERNEL.md` thin amends present (+ optional `COUNT.md` / `PARSE-NAME.md` / `INTERPRET.md`); wave20 tip1–4 TRUE-FALSE / WITHIN / COUNT / EXECUTE cites, wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites, wave18 tip1–4 cites, wave17 tip1–4 cites, wave16 DEFER/MARKER/BUFFER/EXIT cites, and wave15 CELL/IMMEDIATE/COLON/KERNEL prior text retained; host `ACCEPT` / `REFILL` untouched via mirrors; `SOURCE` / `PAD` / `source-mark` / `pad-addr` **not** redefined; `ARCHITECTURE.md` + `IMPLEMENTATION-GAPS.md` + `TRUE-FALSE.md` + `WITHIN.md` + `EXECUTE.md` byte-copy unchanged.
2. `accept-demo` → OK (markers §4; `[accept] ACCEPT` greppable; `[accept] REFILL` greppable; optional `addr=` / `u=` / `n=` / `flag=` welcome; prefer `u=`/`n=` matching fixture; classic refill-ok `flag=` welcome; no FAIL on happy path; no live TIB rewrite / full input-buffer VM / real line editor / SOURCE-PAD reopen / COUNT reopen / EVALUATE nested VM). Prior `exec-demo` + `count-demo` + `within-demo` + `true-demo` + `source-demo` + `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `recurse-demo` + `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` + `refined-boot-demo` + `host-boot-demo` + `interpret-demo` + `kernel-demo` still OK.
3. Regression green (wave20 tip1–5 + wave19 tip1–5 + wave18 tip1–5 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `accept-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/SOURCE-PAD.md` (wave19 **4** — SOURCE/PAD thin marks companion; ACCEPT/REFILL picture fixture into existing stub buffer — **not** SOURCE/PAD reopen / live TIB rewrite)
- `docs/WORD-BL.md` (wave18 **3** — fixed word-buffer companion; ACCEPT/REFILL is thin mark — not WORD reopen)
- `docs/KERNEL.md` (wave7 **5**)
- `docs/COUNT.md` (wave20 **3**, optional — ANS COUNT picture companion; host ENTRY-COUNT ≠ ANS COUNT; not COUNT reopen)
- `docs/PARSE-NAME.md` (wave17 **2**, optional — token-parse companion)
- `docs/INTERPRET.md` (wave8 **1**, optional — interpret companion; ACCEPT/REFILL does not rewrite live splitter / TIB)
- `docs/TRUE-FALSE.md` (wave20 **1** — prior tip; keep cites; leave untouched this tip)
- `docs/WITHIN.md` (wave20 **2** — prior tip; keep cites; leave untouched this tip)
- `docs/EXECUTE.md` (wave20 **4** — prior tip; keep cites; leave untouched this tip)
- `docs/CHAR-CHARS.md` (wave19 **1** — prior tip; keep cites)
- `docs/TO-BODY.md` (wave19 **2** — prior tip; keep cites)
- `docs/ENVIRONMENT-QUERY.md` (wave19 **3** — prior tip; keep cites)
- `docs/TICK.md` (wave18 **1**), `docs/STATE-COMPILE.md` (wave18 **4**), `docs/FIND.md` (wave18 **2**)
- `docs/EVALUATE-INCLUDE.md` (wave17 **3** — mark-only; ACCEPT/REFILL does not reopen nested VM)
- `forth/tritium/kernel.fs` (accept-mark / refill-mark only — do not redefine host ACCEPT/REFILL / SOURCE/PAD)
- ANS Forth `ACCEPT` / `REFILL` (thin input marks only — fixed demo fixture into existing stub buffer; not full input-buffer VM / live TIB rewrite / real line editor / SOURCE-PAD reopen)
- Explicit deferral: WAVE19-PROPOSAL + WAVE20-PROPOSAL + WAVE21-PROPOSAL (`ACCEPT` / `REFILL` — thin marks; not full input-buffer VM / live TIB rewrite)
- Base tip: `fb4a7a0` / `fb4a7a09a8fb400491709f48247bc7431c99e14b` (#96 wave20 tip5 DOCS-CITES CLOSED)
- Wave21 proposal: `/workspace/tritium-research-docs/WAVE21-PROPOSAL.md`
