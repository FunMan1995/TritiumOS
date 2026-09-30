# WORD-BL — Stub `WORD` / `BL` markers + `word-demo`

**Status:** Shipper-ready stub spec (wave18 item **3**; thin amend wave19 **1** CHAR-CHARS companion cite; thin amend wave19 **4** SOURCE-PAD companion cite; thin amend wave20 **3** COUNT companion cite; thin amend wave21 **1** ACCEPT-REFILL companion cite)
**Canonical brief:** ANS-shaped `WORD` / `BL` (thin token/pad stub markers only); `docs/PARSE-NAME.md` (wave17 **2** — token markers landed; WORD/BL rewrite explicitly deferred — closed here as stub markers only); `docs/COMMENT-PARSE.md` (wave12 **3**); `docs/INTERPRET.md` (wave8 **1**); `docs/KERNEL.md` (wave7 **5**); optional `docs/STRING-LIT.md` (wave13 **4**); explicit WAVE17 / WAVE18 deferral closed as WORD/BL stub markers only (not full interpret WORD/BL rewrite / tokenizer VM); wave19 **4** closes SOURCE/PAD as thin marks only (`docs/SOURCE-PAD.md`); wave21 **1** lands ACCEPT/REFILL as thin marks (`docs/ACCEPT-REFILL.md` — not WORD reopen / full input-buffer VM)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `word.fs` / `interpret.fs`); Linux host REPL; **do not** redefine host `WORDS` / `words` / `words-demo` or any host `WORD`
**Companions:** `docs/PARSE-NAME.md` (thin amend this tip), `docs/COMMENT-PARSE.md` (thin amend this tip), `docs/INTERPRET.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip), optional `docs/STRING-LIT.md` (thin amend this tip); `docs/CHAR-CHARS.md` (wave19 **1** — char-unit companion cite); `docs/SOURCE-PAD.md` (wave19 **4** — SOURCE/PAD thin marks beside word-buffer); `docs/COUNT.md` (wave20 **3** — ANS COUNT counted-string picture companion cite; host ENTRY-COUNT/SYN-COUNT/words-count ≠ ANS COUNT); `docs/ACCEPT-REFILL.md` (wave21 **1** — ACCEPT/REFILL thin marks companion cite; not WORD reopen / live TIB rewrite / full input-buffer VM)
**Base tip SHA:** `375836c` (wave18 tip2 FIND PASS / #83) / full `375836cff8885a7738a21568d39cab20b7495771`

## 1. Purpose

WAVE17 PARSE-NAME landed whitespace / delimiter **token-parse markers** and explicitly deferred a `WORD` / `BL` rewrite of the whole interpret path. WAVE18 tip **3** closes that deferral as **stub markers only**: `BL` prints `[word] BL` (+ optional `char=32` / `u=`); `WORD` (delimiter form — demo often uses BL) prints `[word] WORD delim=` (+ optional `tok=` / `u=` / `addr=`) for a stub token taken from a demo string/fixture into a **fixed host word-buffer / pad slot** (cap small; document size — **not** a heap). Smoke via **`word-demo`**. Empty → `[word] FAIL reason=empty` optional (demo avoids). Forth mirrors **`word-parse` / `bl-char`** so host `WORDS` / `words-demo` and any host `WORD` stay untouched. **Not** a rewrite of the live interpret splitter, not S"/escape heap, not SOURCE/PAD full input buffer tip (those stay deferred). Builds on PARSE-NAME without promoting either to a real tokenizer / input-buffer VM. Wave19 tip **1** lands the char-unit companion (`CHAR` / `CHARS` / `[CHAR]` — `docs/CHAR-CHARS.md`); BL optional `char=32` echo stays a blank-char mark — **not** a CHAR reopen / unicode / SOURCE/PAD. Wave19 tip **4** closes the SOURCE/PAD full input-buffer deferral as **thin marks only** (`docs/SOURCE-PAD.md`) — fixed host pad slot **sits beside** this tip’s word-buffer (separate slot; prefer pad cap **84**); **not** a word-buffer share / ACCEPT/REFILL / live TIB rewrite / full input-buffer VM. Wave20 tip **3** lands ANS `COUNT` counted-string picture (`docs/COUNT.md` / Forth mirror `count-mark`) — sibling picture mark over a fixed demo fixture; **not** a WORD reopen / word-buffer share / ACCEPT/REFILL / full counted-string heap; host `ENTRY-COUNT` / `SYN-COUNT` / `words-count` are **not** ANS COUNT. Wave21 tip **1** lands thin `ACCEPT` / `REFILL` marks (`docs/ACCEPT-REFILL.md` / Forth mirrors `accept-mark` / `refill-mark`) — sibling thin input marks over a fixed demo fixture into an existing stub buffer (prefer SOURCE-PAD pad); **not** a WORD reopen / word-buffer share / interpret splitter rewrite / live TIB rewrite / full input-buffer VM / real line editor.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `BL` / `bl-char` | `( -- char )` *or* `( -- )` stub | Blank char mark; print `[word] BL` (+ optional `char=32` / `u=<n>`); does **not** redefine host blank / space helpers |
| `WORD` / `word-parse` | `( delim -- c-addr )` *or* `( delim -- )` stub | Delimiter-form stub token from demo string/fixture into fixed host word-buffer / pad slot; print `[word] WORD delim=<d>` (+ optional `tok=<token>` / `u=<n>` / `addr=<a>`); does **not** rewrite live interpret splitter; does **not** redefine host `WORDS` / any host `WORD` |
| `word-demo` | `( -- )` | See §5 |

Host note: bind bare `WORD` / `BL` on the Linux REPL **only if** they do not collide with host `WORDS` / `words` / any host `WORD` in the same load path. Prefer Forth mirrors **`word-parse` / `bl-char`** as the Lab-facing surface — **do not** redefine host `WORDS` / `words-demo` (list-only — `WORDS-VOCAB.md`) or any host `WORD`. Prefer fixture/demo-string source over rewriting the live interpret splitter. Returned `( c-addr )` / `( char )` is optional — marker alone is enough for Lab. Delim may be shown as a printable char, `BL`, or ordinal — document choice; Lab greps `delim=`.

## 3. Stub semantics

- **Fixed host word-buffer / pad slot:** one host byte array / counted-string slot of **cap = 32** (document if Shipper picks another small size; Lab on Linux SoT expects a documented cap ≤ **64**). Classic picture: counted string (1 length byte + up to `cap-1` chars) or plain token bytes + separate `u=` — either OK if `tok=` / `u=` greppable. Addresses are **offsets** into this slot (`0 .. cap-1`) or a stub `addr=` echo — **not** a heap pointer, not SOURCE/PAD full input buffer, not wave14 HERE bump, not FILL/BUFFER: arena. Prefer plain offsets `0..` or a fixed stub addr echo (e.g. `addr=0`).
- **`BL` / `bl-char`:** print `[word] BL` and optionally `char=32` and/or `u=<n>` (blank ordinal / unit echo). **Does not** rewrite interpret whitespace splitting, SOURCE, or PAD. Captured value is a host int / char echo only.
- **`WORD` / `word-parse` (delimiter form):** given a delimiter char (demo often uses BL / space / `32`), take one stub token from a demo string/fixture (skip leading delims, collect until delim or end, store into the fixed word-buffer). Print `[word] WORD delim=<d>` and optionally `tok=<token>`, `u=<n>` (char count), and/or `addr=<a>` (offset / stub addr of the word-buffer). **Does not** rewrite the live interpret splitter / FIND / PARSE-NAME path for all inputs — demo/fixture path is enough. Does **not** redefine host `WORDS`.
- **Empty / no-token:** `[word] FAIL reason=empty` optional (demo **must avoid** — always feed a non-empty fixture with ≥1 delim-delimited token).
- **Bounds / overflow:** token longer than cap → truncate to cap (document) **or** `[word] FAIL reason=bounds` (demo **must avoid** — keep tokens short). Prefer truncate-or-fit; Lab does not require FAIL.
- Storage: the fixed word-buffer only. **No** heap, no ACCEPT/REFILL / full input-buffer VM, no interpret splitter rewrite, no S"/escape heap, no nested-paren / BLOCK reopen. Sibling `SOURCE` / `PAD` thin marks → `docs/SOURCE-PAD.md` (wave19 **4**; separate pad slot beside this word-buffer — not a share). ANS `COUNT` → `docs/COUNT.md` (wave20 **3**; sibling counted-string picture — not WORD reopen / heap; host ENTRY-COUNT ≠ ANS COUNT). Thin `ACCEPT` / `REFILL` → `docs/ACCEPT-REFILL.md` (wave21 **1**; sibling thin input marks — not WORD reopen / live TIB rewrite / full input-buffer VM).
- Nest with prior find / tick / recurse / eval / parse / synonym / exit / buffer / marker / defer / imm / fill / pick / cell / allot / throw / 2var / create / colon / control / string / comment / words stubs OK. `dict-reset` unaffected (word stubs do not create dict words from token text). Soft KERNEL `abort` / `(abort")`, wave14 CATCH/THROW, and wave16 EXIT/QUIT mark stubs stay as-is beside this marker. Host `WORDS` / `words-demo` + `[kernel] words (` markers stay as-is.
- Still no full interpret WORD/BL rewrite, no full input-buffer VM / live TIB rewrite / real line editor, no tokenizer VM, no linked XT / real DOES> XT / real branch XT / full arena/heap / full Win/Android Forth VM. `SOURCE` / `PAD` thin marks → `docs/SOURCE-PAD.md` (wave19 **4**; pad sits beside this tip’s word-buffer). ANS `COUNT` counted-string picture → `docs/COUNT.md` (wave20 **3**; host ENTRY-COUNT ≠ ANS COUNT). Thin `ACCEPT` / `REFILL` marks → `docs/ACCEPT-REFILL.md` (wave21 **1**; Forth mirrors `accept-mark` / `refill-mark` — not WORD reopen / full input-buffer VM). STATE-COMPILE already landed wave18 **4**. Those stay non-goals / later tips (SOURCE-PAD / COUNT / ACCEPT-REFILL thin marks excepted as mark-only).

## 4. Markers

```
[word] BL [char=32] [u=<n>]                          # char=/u= optional
[word] WORD delim=<d> [tok=<token>] [u=<n>] [addr=<a>]  # tok=/u=/addr= optional
[word] FAIL reason=empty                             # demo avoids
[word] FAIL reason=bounds                            # optional; demo avoids
[word-demo] OK
[word-demo] FAIL
```

Lab greps `[word-demo] OK` plus at least one `[word] BL` (optional `char=32` / `u=` welcome) and one `[word] WORD delim=` (optional `tok=` / `u=` / `addr=` welcome). Demo avoids `[word] FAIL reason=empty` (and bounds FAIL if implemented).

## 5. `word-demo`

1. Clean slate / `dict-reset` (or cold path). Ensure fixed host word-buffer exists (cap documented, prefer **32**, ≤ **64**).
2. Invoke `BL` (or `bl-char`) → `[word] BL` (+ optional `char=32` / `u=`).
3. Prepare a non-empty demo string/fixture with ≥1 BL/space-delimited token (e.g. `hello world` or `alpha`).
4. Invoke `WORD` (or `word-parse`) with BL / space / `32` as delimiter against the fixture → `[word] WORD delim=` (+ optional `tok=hello` / `u=5` / `addr=0`). Prefer `delim=BL` or `delim=32` or `delim= ` — Lab greps `delim=`.
5. Assert no `[word] FAIL reason=empty` on the happy path. Assert token body text is **not** required to `entry-create` / FIND-hit (marker-only is enough; no interpret splitter rewrite required). Assert host `WORDS` / `words-demo` path still list-only / untouched.
6. Prior `find-demo` / `tick-demo` / `recurse-demo` / `eval-demo` / `parse-demo` / `synonym-demo` / `exit-demo` / `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` / `kernel-demo` / `refined-boot-demo` / `host-boot-demo` / `interpret-demo` still OK.
7. `[word-demo] OK`.

`BL` + `WORD delim=` markers are required. Empty FAIL path is not exercised by the demo. Optional `( c-addr )` / `( char )` push and optional `tok=` / `u=` / `addr=` / `char=` echo are not required for Lab OK. No interpret splitter rewrite. No host WORDS redefine. No SOURCE/PAD full buffer.

## 6. Thin amend — companions

### `docs/PARSE-NAME.md`

- Companions: add `WORD-BL.md` (wave18 **3**); **keep** COMMENT-PARSE / INTERPRET / STRING-LIT / KERNEL / SYNONYM cites and wave17 tip2 content — do not wipe.
- Purpose / §3 / non-goals: PARSE/PARSE-NAME stay token-parse markers; WORD/BL stub markers deepen beside them and **close** the prior “WORD/BL rewrite of whole interpret” deferral as **stub markers only** — still **not** a full interpret splitter rewrite / tokenizer VM / SOURCE/PAD. Do not wipe wave17 PARSE content.
- Non-goals: `WORD` / `BL` stubs → `docs/WORD-BL.md` (wave18 **3**). Full interpret WORD/BL rewrite / SOURCE/PAD / tokenizer VM still later (beyond this stub). EVALUATE / RECURSE / FIND / TICK stay on their wave17/18 docs.
- Acceptance: Lab smokes `word-demo` (retains `parse-demo`).
- Cite: `docs/WORD-BL.md`.

### `docs/COMMENT-PARSE.md`

- Companions: add `WORD-BL.md` (wave18 **3**); **keep** INTERPRET / KERNEL / STRING-LIT / PARSE-NAME / VARIABLE-CONST cites.
- Purpose / §3 / non-goals: comment skip stays `\` EOL + `(` … `)`; WORD/BL is a sibling stub token/pad marker surface — **not** a comment reopen / nested-paren / BLOCK / interpret splitter rewrite. Do not wipe wave12 comment content.
- Non-goals: `WORD` / `BL` stubs → `docs/WORD-BL.md` (wave18 **3**). PARSE-NAME stays wave17 **2**. Nested paren / BLOCK comments still later.
- Acceptance: Lab smokes `word-demo` (retains `comment-demo` + `parse-demo`).
- Cite: `docs/WORD-BL.md`.

### `docs/INTERPRET.md`

- Status / companions: cite wave18 **3**; add `WORD-BL.md`; **keep** FIND / TICK / PARSE-NAME / EVALUATE-INCLUDE / EXIT-QUIT / IMMEDIATE / STRING-LIT / COMMENT-PARSE / KERNEL / COLON cites.
- Purpose / §2 / §3 / non-goals: interpret loop stays on host find + whitespace split; WORD/BL stub markers are a sibling demo/fixture pad/token surface via `word-parse` / `bl-char` — **does not** rewrite the live interpret splitter / SOURCE/PAD / WORDS. Do not wipe wave8–18 INTERPRET content. Host WORDS + `[kernel] words (` stay.
- Words table: add `WORD` / `word-parse`, `BL` / `bl-char` + `word-demo` (cite tip; Forth mirrors — stub markers only).
- Markers: one-line pointer to `[word]` markers.
- Non-goals: WORD/BL stub markers in scope via this tip; keep full interpret WORD/BL rewrite / SOURCE/PAD / tokenizer VM / STATE-COMPILE out.
- Acceptance: Lab smokes `word-demo` (retains `interpret-demo` + `find-demo` + `tick-demo` + `parse-demo` + prior).
- Cite: `docs/WORD-BL.md`.

### `docs/KERNEL.md`

- Companions: add `WORD-BL.md` (wave18 **3**); **keep** tip1 TICK + tip2 FIND cites and wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `WORD` / `word-parse`, `BL` / `bl-char` stubs + `word-demo` (cite tip; Forth mirrors `word-parse` / `bl-char` — stub token/pad markers only; **do not** redefine host `WORDS` / `words` / any host `WORD`; **not** interpret splitter rewrite / SOURCE/PAD / heap).
- Non-goals: WORD/BL stub markers → `docs/WORD-BL.md`. FIND stays on `FIND.md`. TICK stays on `TICK.md`. RECURSE / EVALUATE / PARSE / SYNONYM stay on wave17 docs. DEFER/IS/ACTION-OF / MARKER / BUFFER: / EXIT/QUIT stay on wave16 docs. STATE-COMPILE still later (wave18 **4**). SOURCE/PAD full input buffer / tokenizer VM still later.
- Acceptance: Lab smokes `word-demo` (and retains `find-demo` + `tick-demo` + `recurse-demo` + `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `colon-demo` + `kernel-demo` + `words-demo` + prior demos).
- Cite: `docs/WORD-BL.md`.

### `docs/STRING-LIT.md` (optional)

- Companions: add `WORD-BL.md` (wave18 **3**); **keep** COMMENT-PARSE / INTERPRET / KERNEL / CREATE-DOES / THROW-CATCH / PARSE-NAME cites.
- Purpose / §3 / non-goals: string-literal parse-until-`"` stays; WORD/BL is a sibling **token/pad stub** surface — **not** S"/escape heap / SOURCE/PAD / interpret rewrite. Do not wipe wave13 string content.
- Non-goals: `WORD` / `BL` stubs → `docs/WORD-BL.md` (wave18 **3**). Full counted-string heap / BLOCK / escape / SOURCE/PAD still out. PARSE-NAME stays wave17 **2**.
- Acceptance: Lab smokes `word-demo` (retains `string-demo` + `parse-demo`).
- Cite: `docs/WORD-BL.md`.

Do **not** wipe tip1 TICK cites or tip2 FIND cites or wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites or wave16 DEFER / MARKER / BUFFER / EXIT cites or wave15 IMMEDIATE / COLON / KERNEL prior content. Do **not** amend FIND.md / TICK.md / EVALUATE-INCLUDE / RECURSE / EXIT-QUIT / DEFER-IS / CREATE-DOES / COLON / WORDS-VOCAB / DOCS-CITES docs this tip (proposal amends are PARSE-NAME + COMMENT-PARSE + INTERPRET + KERNEL + optional STRING-LIT only).

## 7. Non-goals

- Full interpret path WORD/BL rewrite / live splitter replacement
- Full tokenizer / stream parser VM
- `SOURCE` / `PAD` thin marks → `docs/SOURCE-PAD.md` (wave19 **4**; pad sits beside this tip’s word-buffer — not a share)
- ANS `COUNT` counted-string picture → `docs/COUNT.md` (wave20 **3**; Forth mirror `count-mark`); host `ENTRY-COUNT` / `SYN-COUNT` / `words-count` are **not** ANS COUNT; full counted-string heap still later
- `ACCEPT` / `REFILL` thin marks → `docs/ACCEPT-REFILL.md` (wave21 **1**; Forth mirrors `accept-mark` / `refill-mark`); full input-buffer VM / live TIB rewrite / real line editor still later
- `CHAR` / `CHARS` / `[CHAR]` char-unit stubs → `docs/CHAR-CHARS.md` (wave19 **1**; not CHAR+/unicode / BL reopen)
- Redefining host `WORDS` / `words` / `words-demo` or any host `WORD`
- Nested paren comment deepen / BLOCK comments (COMMENT-PARSE remains non-nested stub)
- S" / escape heap rewrite (STRING-LIT stays marker stubs)
- `STATE` / `COMPILE,` deepen (wave18 **4**)
- Docs cites pass (wave18 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `FIND` reopen (wave18 **2** — already stubbed; keep cites)
- `'` / `[']` tick reopen (wave18 **1** — already stubbed; keep cites)
- `RECURSE` reopen (wave17 **4** — already mark-only; keep cites)
- `EVALUATE` / `INCLUDE` reopen (wave17 **3** — already stubbed; keep cites)
- `PARSE` / `PARSE-NAME` reopen (wave17 **2** — already stubbed; keep cites; this tip closes WORD/BL deferral as stubs only)
- `SYNONYM` / `ALIAS` reopen (wave17 **1** — already stubbed; keep cites)
- `MARKER` / `BUFFER:` / `EXIT` / `QUIT` (wave16 **2–4** — already stubbed; keep cites)
- `DEFER` / `IS` / `ACTION-OF` reopen (wave16 **1** — already stubbed; keep cites)
- IMMEDIATE / POSTPONE / FILL / PICK / CELL (wave15 — already stubbed)
- Real DOES> XT chaining / threaded child runtime body
- Real branch XT / LEAVE jump
- Full Dusk arena / pool / free / fragmentation model
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live — **skip 2DUP-FAMILY**)
- `ABORT"` polish (already optional-wired inside `throw-demo` — **skip**)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/WORD-BL.md` present (Research byte-copy OK); `PARSE-NAME.md` + `COMMENT-PARSE.md` + `INTERPRET.md` + `KERNEL.md` thin amends present (+ optional `STRING-LIT.md`); tip1 TICK cites, tip2 FIND cites, wave17 tip1–4 cites, wave16 DEFER/MARKER/BUFFER/EXIT cites, and wave15 IMMEDIATE/COLON/KERNEL prior text retained; host `WORDS` / `words-demo` / any host `WORD` untouched via mirrors; word-buffer cap documented (prefer 32, ≤ 64).
2. `word-demo` → OK (markers §4; `[word] BL` greppable; `[word] WORD delim=` greppable; optional `char=` / `tok=` / `u=` / `addr=` welcome; no empty FAIL on happy path; no interpret splitter rewrite). wave19 **1**: `char-demo` → OK (retains `word-demo`). wave19 **4**: `source-demo` → OK (retains `word-demo`; pad sits beside word-buffer). wave20 **3**: `count-demo` → OK (retains `word-demo` + `source-demo` + `string-demo`; host ENTRY-COUNT ≠ ANS COUNT). wave21 **1**: `accept-demo` → OK (retains `word-demo` + `source-demo` + `count-demo`; not WORD reopen / live TIB rewrite). Prior `find-demo` + `tick-demo` + `recurse-demo` + `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` + `kernel-demo` + `refined-boot-demo` + `host-boot-demo` + `interpret-demo` still OK.
3. Regression green (wave18 tip1–2 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `word-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/PARSE-NAME.md` (wave17 **2**), `docs/COMMENT-PARSE.md` (wave12 **3**), `docs/INTERPRET.md` (wave8 **1**), `docs/KERNEL.md` (wave7 **5**)
- `docs/STRING-LIT.md` (wave13 **4**, optional companion)
- `docs/FIND.md` (wave18 **2**), `docs/TICK.md` (wave18 **1**)
- `docs/WORDS-VOCAB.md` (wave11 **3** — host WORDS untouched)
- `docs/RECURSE.md` (wave17 **4**), `docs/EVALUATE-INCLUDE.md` (wave17 **3**), `docs/SYNONYM-ALIAS.md` (wave17 **1**)
- `docs/MARKER.md` (wave16 **2**), `docs/BUFFER-COLON.md` (wave16 **3**), `docs/EXIT-QUIT.md` (wave16 **4**)
- `docs/DEFER-IS.md` (wave16 **1**), `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**)
- `docs/FILL-MOVE.md` (wave15 **3** — fixed host-buffer pattern sibling; word-buffer is separate pad slot)
- `forth/tritium/kernel.fs` (host WORDS / words stay; word-parse / bl-char only)
- ANS Forth `WORD` / `BL` (token/pad mark only — no interpret splitter rewrite / SOURCE/PAD full buffer / heap)
- Explicit deferral: WAVE17-PROPOSAL + WAVE18-PROPOSAL (`WORD` / `BL` — not full interpret rewrite; PARSE-NAME deferred WORD/BL closed here as stubs only)
- Base tip: `375836c` / `375836cff8885a7738a21568d39cab20b7495771` (#83 wave18 tip2 FIND PASS)
- `docs/CHAR-CHARS.md` (wave19 **1** — char-unit companion; BL `char=` echo is blank mark only)
- `docs/SOURCE-PAD.md` (wave19 **4** — SOURCE/PAD thin marks; pad sits beside this tip’s word-buffer — not ACCEPT/REFILL / full input-buffer VM)
- `docs/COUNT.md` (wave20 **3** — ANS COUNT picture companion; host ENTRY-COUNT ≠ ANS COUNT)
- `docs/ACCEPT-REFILL.md` (wave21 **1** — ACCEPT/REFILL thin marks companion; not WORD reopen / live TIB rewrite / full input-buffer VM)
- Wave18 proposal: `/workspace/tritium-research-docs/WAVE18-PROPOSAL.md`
