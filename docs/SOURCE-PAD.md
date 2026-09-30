# SOURCE-PAD — `SOURCE` / `PAD` thin markers + `source-demo`

**Status:** Shipper-ready stub spec (wave19 item **4**)
**Canonical brief:** ANS-shaped `SOURCE` / `PAD` (thin input-string / pad-slot markers only); `docs/WORD-BL.md` (wave18 **3** — fixed word-buffer companion; SOURCE/PAD full input buffer deferred — closed here as stub markers only); `docs/PARSE-NAME.md` (wave17 **2**); `docs/INTERPRET.md` (wave8 **1**); `docs/KERNEL.md` (wave7 **5**); optional `docs/BUFFER-COLON.md` (wave16 **3**) / `docs/STRING-LIT.md` (wave13 **4**); explicit WAVE18 / WAVE19 deferral closed as thin marks only (not ACCEPT/REFILL / full input-buffer VM / interpret TIB rewrite)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `source.fs` / `pad.fs` / `source-pad.fs`); Linux host REPL; **do not** redefine host `SOURCE` / `PAD` that already bind on the load path
**Companions:** `docs/WORD-BL.md` (thin amend this tip), `docs/PARSE-NAME.md` (thin amend this tip), `docs/INTERPRET.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip); optional light cite `docs/BUFFER-COLON.md` / `docs/STRING-LIT.md`
**Base tip SHA:** `3b72e90` (wave19 tip3 PASS / #89 ENVIRONMENT-QUERY) / full `3b72e90e570dbaf6675c8881e24acc7ec445000b`

## 1. Purpose

WAVE18 WORD-BL landed a **fixed host word-buffer** for stub `WORD` / `BL` tokens (`docs/WORD-BL.md`) and explicitly deferred a full `SOURCE` / `PAD` / `ACCEPT` / `REFILL` input-buffer surface. WAVE19 tip **4** closes that deferral as **stub markers only**: `SOURCE` (or Forth mirror `source-mark`) prints `[source] SOURCE` (+ optional `addr=` / `u=` echoing a **demo input-string fixture / stub buffer length** — **not** a live TIB rewrite); `PAD` (or Forth mirror `pad-addr`) prints `[source] PAD` (+ optional `addr=` / `u=` / `cap=`) for a **fixed host pad slot** (cap small — prefer **84** classic picture or document ≤ **128**; **not** a heap). Smoke via **`source-demo`**. Forth mirrors **`source-mark` / `pad-addr`** so host `SOURCE` / `PAD` stay safe. Empty SOURCE fixture → `[source] FAIL` reason=empty optional (demo avoids). **Not** ACCEPT/REFILL, not interpret TIB rewrite, not EVALUATE nested VM reopen, not full input-buffer VM. Thin deepen beside wave18 WORD-BL — pad slot **sits beside** WORD-BL’s word-buffer as a **separate** fixed host slot (document; do **not** share / enlarge the word-buffer). Closes WORD-BL’s “SOURCE/PAD full input buffer” deferral as **thin marks only** before tip5 cites. Keep tip1 CHAR-CHARS + tip2 TO-BODY + tip3 ENVIRONMENT-QUERY cites; this tip does not reopen char-unit / body address / env query.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `SOURCE` / `source-mark` | `( -- c-addr u )` *or* `( -- )` stub | Input-string mark; print `[source] SOURCE` (+ optional `addr=<n>` / `u=<n>` echoing demo fixture / stub buffer length); **not** a live TIB rewrite |
| `PAD` / `pad-addr` | `( -- c-addr )` *or* `( -- )` stub | Fixed host pad-slot mark; print `[source] PAD` (+ optional `addr=<n>` / `u=<n>` / `cap=<n>`); cap prefer **84**, document ≤ **128**; **not** a heap |
| `source-demo` | `( -- )` | See §5 |

Host note: bind `SOURCE` / `PAD` on the Linux REPL **only if** those names do not collide with a host Forth `SOURCE` / `PAD` in the same load path. Prefer Forth mirrors **`source-mark` / `pad-addr`** as the Lab-facing surface when in doubt — **do not** redefine host `SOURCE` / `PAD`. Optional `addr=` / `u=` / `cap=` are host ints / stub offsets only — not a live TIB pointer, not a heap pointer, not ACCEPT/REFILL. Prefer a non-empty demo input-string fixture so empty FAIL is avoided.

## 3. Stub semantics

- **Fixed host pad slot (beside WORD-BL word-buffer):** one host byte array / counted-string slot of **cap = 84** (classic picture; document if Shipper picks another small size ≤ **128**). **Sits beside** WORD-BL’s fixed word-buffer (cap prefer **32**, ≤ **64**) as a **separate** slot — do **not** share / enlarge / replace the word-buffer. Addresses are **offsets** into this pad slot (`0 .. cap-1`) or a stub `addr=` echo — **not** a heap pointer, not wave14 HERE bump, not FILL/BUFFER: arena, not a live TIB. Prefer plain offsets `0..` or a fixed stub addr echo (e.g. `addr=0`).
- **`SOURCE` / `source-mark`:** echo a **demo input-string fixture** (or stub buffer length) — print `[source] SOURCE` and optionally `addr=<n>` (stub offset / fixed base of the fixture picture) and/or `u=<n>` (char count / stub length). **Does not** rewrite the live TIB / interpret input stream / ACCEPT / REFILL / EVALUATE nested path. Captured values are host ints / fixture echo only. Prefer a non-empty fixture so Lab greps `SOURCE` without FAIL.
- **`PAD` / `pad-addr`:** print `[source] PAD` and optionally `addr=<n>` (offset / stub addr of the pad slot), `u=<n>` (current used / stub length — optional; may be `0` on cold path), and/or `cap=<n>` (documented pad capacity — prefer `cap=84`). **Does not** allocate, bump HERE, rewrite WORD-BL’s word-buffer, or open a heap. Pad is a **fixed host slot** only.
- **Empty SOURCE fixture:** `[source] FAIL reason=empty` optional (demo **must avoid** — always feed a non-empty demo input-string fixture with `u≥1`).
- Storage: demo fixture string echo + fixed pad slot only. **No** live TIB rewrite, no ACCEPT/REFILL, no full input-buffer VM, no interpret splitter rewrite, no EVALUATE nested VM reopen, no heap, no HERE bump, no arena.
- Nest with prior env / body / char / state / word / find / tick / create / allot / synonym / exit / buffer / marker / defer / imm / fill / pick / cell / throw / 2var / colon / control / string / parse / eval stubs OK. `dict-reset` unaffected (source/pad marks need no dict entries — fixed host slots / fixtures only).
- Still no ACCEPT/REFILL / full input-buffer VM, no interpret TIB rewrite, no EVALUATE nested VM reopen, no tip5 DOCS-CITES (wave19 **5**), no linked XT / real DOES> XT / real branch XT / full arena/heap / full Win/Android Forth VM. CHAR-CHARS (wave19 **1**), TO-BODY (wave19 **2**), and ENVIRONMENT-QUERY (wave19 **3**) stay landed — keep cites; this tip does not reopen char-unit / body address / env query. Those stay non-goals / later tips.

## 4. Markers

```
[source] SOURCE [addr=<n>] [u=<n>]              # addr=/u= optional; demo fixture / stub length echo
[source] PAD [addr=<n>] [u=<n>] [cap=<n>]       # addr=/u=/cap= optional; prefer cap=84
[source] FAIL reason=empty                      # demo avoids
[source-demo] OK
[source-demo] FAIL
```

Lab greps `[source-demo] OK` plus at least one `[source] SOURCE` (optional `addr=` / `u=` welcome) and one `[source] PAD` (optional `addr=` / `u=` / `cap=` welcome; prefer greppable `cap=84` or documented ≤128). Demo avoids `[source] FAIL reason=empty`. Prefer not emitting `[source] FAIL` on the happy path.

## 5. `source-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; source/pad marks need no dict entries (fixed host slots / fixtures).
2. Ensure a non-empty demo input-string fixture exists (e.g. `hello` / `source pad demo`) so empty FAIL is avoided. Ensure fixed host pad slot exists (cap documented, prefer **84**, ≤ **128**) — **beside** WORD-BL’s word-buffer, not sharing it.
3. Invoke `SOURCE` (or `source-mark`) → `[source] SOURCE` (+ optional `addr=` / `u=` echoing the fixture / stub length).
4. Invoke `PAD` (or `pad-addr`) → `[source] PAD` (+ optional `addr=` / `u=` / `cap=`). Prefer `cap=84` (or documented ≤128) greppable when `cap=` is present.
5. Assert no `[source] FAIL reason=empty` on the happy path. Assert source/pad marks did **not** require a live TIB rewrite / ACCEPT/REFILL / full input-buffer VM / interpret splitter rewrite / EVALUATE nested reopen / heap / HERE bump (marker-only is enough). Assert host `SOURCE` / `PAD` were not redefined when using the Forth mirrors. Assert WORD-BL word-buffer still separate / untouched.
6. Prior `env-demo` / `body-demo` / `char-demo` / `state-demo` / `word-demo` / `find-demo` / `tick-demo` / `recurse-demo` / `eval-demo` / `parse-demo` / `synonym-demo` / `exit-demo` / `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` / `refined-boot-demo` / `host-boot-demo` / `interpret-demo` / `kernel-demo` still OK.
7. `[source-demo] OK`.

`SOURCE` + `PAD` markers are required. Empty FAIL path is not exercised by the demo. Optional `addr=` / `u=` / `cap=` echo is not all required for Lab OK when SOURCE + PAD lines are greppable. No live TIB rewrite. No ACCEPT/REFILL. No full input-buffer VM. No interpret splitter rewrite. No heap.

## 6. Thin amend — companions

### `docs/WORD-BL.md`

- Companions / Status: add `SOURCE-PAD.md` (wave19 **4** companion cite); **keep** PARSE-NAME / COMMENT-PARSE / INTERPRET / KERNEL / STRING-LIT / CHAR-CHARS cites — do not wipe wave18 WORD-BL content.
- Purpose / §3: WORD/BL stay stub token/pad markers into the **fixed word-buffer** (cap prefer 32); `SOURCE` / `PAD` thin marks deepen beside that buffer as a **separate** fixed host pad slot — **not** a word-buffer share / enlarge / interpret splitter rewrite / ACCEPT/REFILL / full input-buffer VM. Do not wipe wave18 WORD-BL content.
- Non-goals: `SOURCE` / `PAD` thin marks → `docs/SOURCE-PAD.md` (wave19 **4**). Full ACCEPT/REFILL / input-buffer VM still later. PARSE-NAME stays wave17 **2**. CHAR-CHARS stays wave19 **1**.
- Acceptance: Lab smokes `source-demo` (retains `word-demo` + `parse-demo` + `char-demo`).
- Cite: `docs/SOURCE-PAD.md`.

### `docs/PARSE-NAME.md`

- Companions: add `SOURCE-PAD.md` (wave19 **4**); **keep** COMMENT-PARSE / INTERPRET / STRING-LIT / KERNEL / SYNONYM / WORD-BL / CHAR-CHARS cites — do not wipe wave17 PARSE content.
- Purpose / §3 / non-goals: PARSE/PARSE-NAME stay token-parse markers; `SOURCE` / `PAD` are sibling **input-string / pad-slot** marks — **not** a parse reopen / tokenizer VM / interpret TIB rewrite / ACCEPT/REFILL. Do not wipe wave17 PARSE content.
- Non-goals: `SOURCE` / `PAD` → `docs/SOURCE-PAD.md` (wave19 **4**). WORD/BL stays wave18 **3**. Full ACCEPT/REFILL / input-buffer VM still later.
- Acceptance: Lab smokes `source-demo` (retains `parse-demo` + `word-demo`).
- Cite: `docs/SOURCE-PAD.md`.

### `docs/INTERPRET.md`

- Status / companions: cite wave19 **4**; add `SOURCE-PAD.md`; **keep** FIND / TICK / PARSE-NAME / EVALUATE-INCLUDE / EXIT-QUIT / IMMEDIATE / STRING-LIT / COMMENT-PARSE / KERNEL / COLON / WORD-BL / STATE-COMPILE cites.
- Purpose / §2 / §3 / non-goals: interpret loop stays on host find + whitespace split; `SOURCE` / `PAD` stub markers are a sibling demo/fixture input-string / pad-slot surface via `source-mark` / `pad-addr` — **does not** rewrite the live interpret splitter / TIB / ACCEPT/REFILL / EVALUATE nested path. Do not wipe wave8–19 INTERPRET content. Host WORDS + `[kernel] words (` stay. WORD-BL word-buffer stays separate.
- Words table: add `SOURCE` / `source-mark`, `PAD` / `pad-addr` + `source-demo` (cite tip; Forth mirrors — thin marks only).
- Markers: one-line pointer to `[source]` markers.
- Non-goals: SOURCE/PAD thin marks in scope via this tip; keep ACCEPT/REFILL / full input-buffer VM / interpret TIB rewrite / EVALUATE nested reopen out.
- Acceptance: Lab smokes `source-demo` (retains `interpret-demo` + `word-demo` + `parse-demo` + `eval-demo` + prior).
- Cite: `docs/SOURCE-PAD.md`.

### `docs/KERNEL.md`

- Companions: add `SOURCE-PAD.md` (wave19 **4**); **keep** tip1 CHAR-CHARS + tip2 TO-BODY + tip3 ENVIRONMENT-QUERY cites and wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites and wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `SOURCE` / `source-mark`, `PAD` / `pad-addr` stubs + `source-demo` (cite tip; Forth mirrors `source-mark` / `pad-addr` — thin input-string / pad-slot marks only; **do not** redefine host `SOURCE` / `PAD`; **not** ACCEPT/REFILL / live TIB rewrite / full input-buffer VM / interpret splitter rewrite / heap; pad sits beside WORD-BL word-buffer).
- Non-goals: `SOURCE` / `PAD` thin marks → `docs/SOURCE-PAD.md`. CHAR/CHARS/[CHAR] stay on `CHAR-CHARS.md`. >BODY stays on `TO-BODY.md`. ENVIRONMENT? stays on `ENVIRONMENT-QUERY.md`. WORD-BL / FIND / TICK / STATE stay on wave18 docs. ACCEPT/REFILL / full input-buffer VM still later. Tip5 DOCS-CITES still later (wave19 **5**).
- Acceptance: Lab smokes `source-demo` (and retains `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `create-demo` + `allot-demo` + `cell-demo` + prior demos).
- Cite: `docs/SOURCE-PAD.md`.

### Optional — `docs/BUFFER-COLON.md`

- Companions: add light `SOURCE-PAD.md` (wave19 **4**) cite; **keep** ALLOT-HERE / FILL-MOVE / VARIABLE-CONST / KERNEL / MARKER / DEFER cites.
- Purpose / non-goals: BUFFER: stays named-slot over fill cap / HERE bump; `PAD` is a sibling **fixed host pad slot** (cap prefer 84) — **not** a BUFFER: reopen / arena / ALLOCATE / heap. Do not wipe wave16 BUFFER-COLON content.
- Non-goals: `SOURCE` / `PAD` → `docs/SOURCE-PAD.md` (wave19 **4**). BUFFER: stays wave16 **3**. Full ACCEPT/REFILL / arena still later.
- Acceptance: Lab smokes `source-demo` (retains `buffer-demo` + `fill-demo`).
- Cite: `docs/SOURCE-PAD.md`.

### Optional — `docs/STRING-LIT.md`

- Companions: add light `SOURCE-PAD.md` (wave19 **4**) cite; **keep** COMMENT-PARSE / INTERPRET / KERNEL / CREATE-DOES / THROW-CATCH / PARSE-NAME / WORD-BL / CHAR-CHARS cites.
- Purpose / non-goals: string-literal parse-until-`"` stays; `SOURCE` / `PAD` are sibling **input-string / pad-slot** marks — **not** S"/escape heap / SOURCE full buffer / ACCEPT/REFILL. Do not wipe wave13 string content.
- Non-goals: `SOURCE` / `PAD` → `docs/SOURCE-PAD.md` (wave19 **4**). Full counted-string heap / BLOCK / escape / ACCEPT/REFILL still out. WORD-BL stays wave18 **3**.
- Acceptance: Lab smokes `source-demo` (retains `string-demo` + `word-demo` + `parse-demo`).
- Cite: `docs/SOURCE-PAD.md`.

Do **not** wipe tip1 CHAR-CHARS cites or tip2 TO-BODY cites or tip3 ENVIRONMENT-QUERY cites or wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites or wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites or wave16 DEFER / MARKER / BUFFER / EXIT cites or wave15 CELL / IMMEDIATE / FILL / PICK prior content. Do **not** amend ARCHITECTURE / IMPLEMENTATION-GAPS / CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / DOCS-CITES docs this tip (proposal amends are WORD-BL + PARSE-NAME + INTERPRET + KERNEL + optional BUFFER-COLON / STRING-LIT only). Leave `CHAR-CHARS.md`, `TO-BODY.md`, and `ENVIRONMENT-QUERY.md` untouched (tip1–3 byte-copy stays). Tip5 cites come after 1–4 PASS.

## 7. Non-goals

- Full input-buffer VM / live TIB rewrite / interpret splitter replacement
- `ACCEPT` / `REFILL` (still deferred — tip4 is thin SOURCE/PAD **marks only**)
- EVALUATE nested VM reopen / INCLUDE file VM reopen (wave17 **3** — already mark-only; keep cites)
- Heap / arena / ALLOCATE / HERE bump via SOURCE/PAD
- Sharing / enlarging / replacing WORD-BL’s fixed word-buffer (pad **sits beside** it)
- Redefining host `SOURCE` / `PAD`
- Docs cites pass (wave19 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `ENVIRONMENT?` reopen (wave19 **3** — already stubbed; keep cites)
- `>BODY` reopen (wave19 **2** — already stubbed; keep cites)
- `CHAR` / `CHARS` / `[CHAR]` reopen (wave19 **1** — already stubbed; keep cites)
- `WORD` / `BL` / `FIND` / `'` / `[']` / `STATE` / `COMPILE,` reopen (wave18 — already stubbed; keep cites)
- `PARSE` / `PARSE-NAME` reopen (wave17 **2** — already stubbed; keep cites)
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

1. `docs/SOURCE-PAD.md` present (Research byte-copy OK); `WORD-BL.md` + `PARSE-NAME.md` + `INTERPRET.md` + `KERNEL.md` thin amends present (+ optional `BUFFER-COLON.md` / `STRING-LIT.md`); tip1 CHAR-CHARS cites, tip2 TO-BODY cites, tip3 ENVIRONMENT-QUERY cites, wave18 tip1–4 cites, wave17 tip1–4 cites, wave16 DEFER/MARKER/BUFFER/EXIT cites, and wave15 CELL/IMMEDIATE/COLON/KERNEL prior text retained; host `SOURCE` / `PAD` untouched via mirrors; pad cap documented (prefer 84, ≤ 128); pad sits beside WORD-BL word-buffer; `CHAR-CHARS.md` + `TO-BODY.md` + `ENVIRONMENT-QUERY.md` byte-copy unchanged.
2. `source-demo` → OK (markers §4; `[source] SOURCE` greppable; `[source] PAD` greppable; optional `addr=` / `u=` / `cap=` welcome; prefer `cap=84` when present; no empty FAIL on happy path; no live TIB rewrite / ACCEPT/REFILL / full input-buffer VM / interpret splitter rewrite / heap). Prior `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `recurse-demo` + `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` + `refined-boot-demo` + `host-boot-demo` + `interpret-demo` + `kernel-demo` still OK.
3. Regression green (wave19 tip1–3 + wave18 tip1–5 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `source-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/WORD-BL.md` (wave18 **3** — fixed word-buffer companion; SOURCE/PAD thin marks sit beside it — not word-buffer share / ACCEPT/REFILL / full input-buffer VM)
- `docs/PARSE-NAME.md` (wave17 **2**), `docs/INTERPRET.md` (wave8 **1**), `docs/KERNEL.md` (wave7 **5**)
- `docs/BUFFER-COLON.md` (wave16 **3**, optional), `docs/STRING-LIT.md` (wave13 **4**, optional)
- `docs/ENVIRONMENT-QUERY.md` (wave19 **3** — prior tip; keep cites)
- `docs/TO-BODY.md` (wave19 **2** — prior tip; keep cites)
- `docs/CHAR-CHARS.md` (wave19 **1** — prior tip; keep cites)
- `docs/FIND.md` (wave18 **2**), `docs/TICK.md` (wave18 **1**), `docs/STATE-COMPILE.md` (wave18 **4**)
- `docs/EVALUATE-INCLUDE.md` (wave17 **3** — mark-only; SOURCE does not reopen nested VM)
- `docs/DEFER-IS.md` (wave16 **1**), `docs/SYNONYM-ALIAS.md` (wave17 **1**)
- `docs/COLON.md` (wave9 **4**), `docs/CELL-CELLS.md` (wave15 **1**)
- `forth/tritium/kernel.fs` (source-mark / pad-addr only — do not redefine host SOURCE/PAD)
- ANS Forth `SOURCE` / `PAD` (thin input-string / pad-slot marks only — not ACCEPT/REFILL / live TIB rewrite / full input-buffer VM)
- Explicit deferral: WAVE18-PROPOSAL + WAVE19-PROPOSAL (`SOURCE` / `PAD` — thin marks; not ACCEPT/REFILL / full input-buffer VM)
- Base tip: `3b72e90` / `3b72e90e570dbaf6675c8881e24acc7ec445000b` (#89 wave19 tip3 ENVIRONMENT-QUERY PASS)
- Wave19 proposal: `/workspace/tritium-research-docs/WAVE19-PROPOSAL.md`
