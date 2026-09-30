# INTERPRET — Token-stream interpret stub

**Status:** Shipper-ready stub spec (wave8 item **1**; thin amend wave9 **4** colon-stub; thin amend wave12 **3** comment-parse; thin amend wave13 **4** string-lit; thin amend wave15 **4** IMMEDIATE-POSTPONE; thin amend wave16 **4** EXIT-QUIT; thin amend wave17 **2** PARSE-NAME; thin amend wave17 **3** EVALUATE-INCLUDE; thin amend wave18 **2** FIND; thin amend wave18 **3** WORD-BL; thin amend wave18 **4** STATE-COMPILE; thin amend wave19 **4** SOURCE-PAD)  
**Canonical brief:** Dusk `fs/doc/kernel.txt` interpret loop; `docs/KERNEL.md` (wave7 **5**)  
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `interpret.fs`); Linux host mirrors  
**Companions:** `docs/KERNEL.md`, `docs/GROUPS-NESTED.md`, `docs/FORTH-BASE-REFERENCES.md`, `docs/COLON.md` (wave9 **4**), `docs/COMMENT-PARSE.md` (wave12 **3**), `docs/STRING-LIT.md` (wave13 **4**), `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**), `docs/EXIT-QUIT.md` (wave16 **4**), `docs/PARSE-NAME.md` (wave17 **2**), `docs/EVALUATE-INCLUDE.md` (wave17 **3**), `docs/FIND.md` (wave18 **2**), `docs/TICK.md` (wave18 **1**, light), `docs/WORD-BL.md` (wave18 **3**), `docs/STATE-COMPILE.md` (wave18 **4**), `docs/SOURCE-PAD.md` (wave19 **4**)

## 1. Purpose

Wave7 landed lookup-only `interpret-token`. This tip deepens a **minimal interpret loop**: walk a whitespace-separated token stream, `find` each name, execute or print a stub action. Wave8 `:` was **create-only**; wave9 **4** deepens a **body/marker** stub (`docs/COLON.md`) — still not a full colon compiler or control-flow VM. Wave12 **3** adds **comment skip** on the interpret/token path (`\` EOL + `(` … `)`) — see `docs/COMMENT-PARSE.md`. Wave13 **4** adds **string-literal parse** stubs (`S"` / `."`, optional `.(`) — see `docs/STRING-LIT.md`. Wave15 **4** adds **IMMEDIATE/POSTPONE** flag + name mark stubs — see `docs/IMMEDIATE-POSTPONE.md` (still no linked XT compiler / executing postponed XT). Wave16 **4** adds thin **EXIT/QUIT** control markers (interpret-reset stub mark only — **not** a real restart VM) — see `docs/EXIT-QUIT.md`. Wave17 **2** adds thin **PARSE/PARSE-NAME** token-parse markers (demo/fixture path — **not** a WORD/BL rewrite of the whole interpret path / full tokenizer VM) — see `docs/PARSE-NAME.md`. Wave17 **3** adds thin **EVALUATE/INCLUDE** mark-only echo markers (demo/fixture path — **not** a nested interpret re-entry / file VM; refined-boot include markers stay) — see `docs/EVALUATE-INCLUDE.md`. Wave18 **2** adds thin **FIND** ANS-ish find markers via Forth mirror `find-xt` / `find-mark` (demo/fixture path — **not** a rewrite of host `find`/`findentry`/`entry-find` used by interpret / kernel-demo; not SEARCH-WORDLIST / execute-through) — see `docs/FIND.md`. Wave18 **1** TICK stub `xt=` ids stay consistent — see `docs/TICK.md`. Wave18 **3** adds thin **WORD/BL** stub token/pad markers via Forth mirrors `word-parse` / `bl-char` (demo/fixture path into a fixed host word-buffer — **not** a rewrite of the live interpret splitter / SOURCE/PAD / host `WORDS`) — see `docs/WORD-BL.md`. Wave18 **4** adds thin **STATE/COMPILE,** stub markers via Forth mirrors `state-flag` / `compile-comma` (query + compile-comma mark only — **not** a linked XT compiler / real compile-vs-interpret machine / real STATE cell / POSTPONE reopen) — see `docs/STATE-COMPILE.md`. Wave19 **4** adds thin **SOURCE/PAD** stub markers via Forth mirrors `source-mark` / `pad-addr` (demo/fixture input-string + fixed host pad slot beside WORD-BL’s word-buffer — **not** a live TIB rewrite / ACCEPT/REFILL / full input-buffer VM / interpret splitter rewrite / EVALUATE nested reopen) — see `docs/SOURCE-PAD.md`.

## 2. Words

| Word | Stack | Notes |
|------|-------|-------|
| `interpret-token` | `( c-addr u -- flag )` | Existing (KERNEL.md); comment starts skip rather than miss — `COMMENT-PARSE.md` |
| `interpret` | `( c-addr u -- )` | Split on bl; **skip** `\` EOL / `(` … `)` comments first (`COMMENT-PARSE.md`); each real token → find → hit: print/exec stub; miss: miss marker (continue or soft-abort — prefer **continue** + count misses) |
| `\` / `(` | (stream) | Comment skip stubs — see `COMMENT-PARSE.md` (wave12 **3**) |
| `:` / `colon-create` | `( "name" -- )` | Create + colon-def; body until `;` — see `COLON.md` (wave9 **4**) |
| `;` / `semicolon` | `( -- )` | Close colon-def; body-present marker (`COLON.md`) |
| `interpret-demo` | `( -- )` | See §4 |
| `colon-demo` | `( -- )` | Body/marker smoke (`COLON.md`) |
| `comment-demo` | `( -- )` | Comment skip smoke (`COMMENT-PARSE.md`) |
| `S"` / `."` | (stream) | String-literal parse stubs — see `STRING-LIT.md` (wave13 **4**) |
| `.(` | (stream) | Optional string-paren stub — `STRING-LIT.md` |
| `string-demo` | `( -- )` | String-literal smoke (`STRING-LIT.md`) |
| `IMMEDIATE` / `immediate-mark` | (see tip) | Immediate-bit stub — see `IMMEDIATE-POSTPONE.md` (wave15 **4**) |
| `POSTPONE` / `postpone-mark` | (see tip) | Postpone name-mark stub — see `IMMEDIATE-POSTPONE.md` (wave15 **4**) |
| `imm-demo` | `( -- )` | IMMEDIATE + POSTPONE smoke (`IMMEDIATE-POSTPONE.md`) |
| `EXIT` / `exit-mark` | `( -- )` | Thin control marker — see `EXIT-QUIT.md` (wave16 **4**); Forth mirror `exit-mark` (do not redefine host `exit`) |
| `QUIT` / `quit-mark` | `( -- )` | Interpret-reset stub mark — see `EXIT-QUIT.md` (wave16 **4**); Forth mirror `quit-mark` |
| `exit-demo` | `( -- )` | EXIT + QUIT smoke (`EXIT-QUIT.md`) |
| `PARSE-NAME` / `parse-name` | (see tip) | Whitespace-delimited stub token marker — see `PARSE-NAME.md` (wave17 **2**); Forth mirror `parse-name` |
| `PARSE` / `parse-delim` | (see tip) | Delimiter-form stub token marker — see `PARSE-NAME.md` (wave17 **2**); Forth mirror `parse-delim` |
| `parse-demo` | `( -- )` | PARSE + PARSE-NAME smoke (`PARSE-NAME.md`) |
| `EVALUATE` / `evaluate-mark` | (see tip) | Mark-only nested-interpret echo — see `EVALUATE-INCLUDE.md` (wave17 **3**); Forth mirror `evaluate-mark` (does **not** re-enter nested interpret) |
| `INCLUDE` / `include-mark` | (see tip) | Mark-only include echo — see `EVALUATE-INCLUDE.md` (wave17 **3**); Forth mirror `include-mark` (**do not** redefine host/poly `include` / refined-boot load) |
| `eval-demo` | `( -- )` | EVALUATE + INCLUDE smoke (`EVALUATE-INCLUDE.md`) |
| `FIND` / `find-xt` | (see tip) | ANS-ish find mark — see `FIND.md` (wave18 **2**); Forth mirrors `find-xt` / `find-mark` (**do not** redefine host `find` / `findentry` / `entry-find` used by interpret) |
| `find-demo` | `( -- )` | FIND / find-xt smoke (`FIND.md`) |
| `BL` / `bl-char` | (see tip) | Blank char mark — see `WORD-BL.md` (wave18 **3**); Forth mirror `bl-char` |
| `WORD` / `word-parse` | (see tip) | Delimiter-form stub token into fixed word-buffer — see `WORD-BL.md` (wave18 **3**); Forth mirror `word-parse` (**do not** redefine host `WORDS` / any host `WORD`; not interpret splitter rewrite) |
| `word-demo` | `( -- )` | WORD + BL smoke (`WORD-BL.md`) |
| `STATE` / `state-flag` | (see tip) | Compile-state query mark — see `STATE-COMPILE.md` (wave18 **4**); Forth mirror `state-flag` (**do not** redefine host assistant-state; not real STATE cell) |
| `COMPILE,` / `compile-comma` | (see tip) | Compile-comma mark — see `STATE-COMPILE.md` (wave18 **4**); Forth mirror `compile-comma` (does **not** append XT to body / execute it) |
| `state-demo` | `( -- )` | STATE + COMPILE, smoke (`STATE-COMPILE.md`) |
| `SOURCE` / `source-mark` | (see tip) | Input-string mark — see `SOURCE-PAD.md` (wave19 **4**); Forth mirror `source-mark` (**do not** redefine host `SOURCE`; not live TIB rewrite / ACCEPT/REFILL) |
| `PAD` / `pad-addr` | (see tip) | Fixed host pad-slot mark — see `SOURCE-PAD.md` (wave19 **4**); Forth mirror `pad-addr` (**do not** redefine host `PAD`; pad sits beside WORD-BL word-buffer; prefer cap **84**) |
| `source-demo` | `( -- )` | SOURCE + PAD smoke (`SOURCE-PAD.md`) |

Execute stub on hit: print `[interpret] exec #<i> <name>` (host may call a bound id later). No nested interpret required this tip.

## 3. Markers

```
[interpret] exec #N name=<word>
[interpret] miss name=<word>
[interpret] : created <name>
[interpret-demo] OK
[interpret-demo] FAIL
```

Comment skip markers → `docs/COMMENT-PARSE.md` (`[comment] skip line` / `[comment] skip paren`).
String-literal markers → `docs/STRING-LIT.md` (`[string] S"` / `[string] ."`).
IMMEDIATE/POSTPONE markers → `docs/IMMEDIATE-POSTPONE.md` (`[imm] IMMEDIATE name=` / `[imm] POSTPONE name=`).
EXIT/QUIT markers → `docs/EXIT-QUIT.md` (`[exit] EXIT` / `[exit] QUIT`).
PARSE/PARSE-NAME markers → `docs/PARSE-NAME.md` (`[parse] PARSE-NAME` / `[parse] PARSE delim=`).
EVALUATE/INCLUDE markers → `docs/EVALUATE-INCLUDE.md` (`[eval] EVALUATE` / `[eval] INCLUDE name=`|`path=`); refined-boot `[VM] include` / `[refined-boot] include=` stay on `REFINED-BOOT.md`.
FIND markers → `docs/FIND.md` (`[find] FIND name=` / optional `[find] FIND miss`); host `[kernel] find hit` / `[kernel] find miss` stay on `KERNEL.md`.
WORD/BL markers → `docs/WORD-BL.md` (`[word] BL` / `[word] WORD delim=`); host `WORDS` / `[kernel] words (` stay on `WORDS-VOCAB.md` / `KERNEL.md`.
STATE/COMPILE, markers → `docs/STATE-COMPILE.md` (`[state] STATE` / `[state] COMPILE,`); host assistant-state stays on `ASSISTANT-STATE.md`.
SOURCE/PAD markers → `docs/SOURCE-PAD.md` (`[source] SOURCE` / `[source] PAD`); pad sits beside WORD-BL word-buffer; host SOURCE/PAD untouched via mirrors.

## 4. `interpret-demo`

1. `dict-reset` (or cold path)
2. `entry-create` (or `:`) two names
3. `interpret` a string containing both + one unknown → hits + one miss marker
4. Assert miss counted / greppable; hits ≥2
5. `[interpret-demo] OK`

Prior `kernel-demo` must stay green.

## 5. Non-goals

- Full immediate/compile state machine beyond colon-def flag + immediate-bit + STATE query mark — flag + name mark stubs → `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**); STATE/COMPILE, query + compile-comma mark stubs → `docs/STATE-COMPILE.md` (wave18 **4**); still no linked XT / executing postponed XT / real STATE cell
- `EXIT` / `QUIT` thin control markers → `docs/EXIT-QUIT.md` (wave16 **4**); interpret-reset stub mark only — still no real restart VM / RECURSE XT / host `exit` redefine
- Control flow (`if`/`then`/`do`) — later
- Full Dusk `findentry` linked units / XT lists
- Host C#/Kotlin full VM rewrite
- Full counted-string heap / BLOCK / escape rewrite — still out; string-literal stubs via wave13 **4** (`STRING-LIT.md`) only
- Real BLOCK comments — still out (comment skip stubs via wave12 **3** only)
- Full parser / tokenizer VM / WORD/BL rewrite of the whole interpret path — still out; PARSE/PARSE-NAME marker deepen via wave17 **2** (`PARSE-NAME.md`) only; WORD/BL stub markers via wave18 **3** (`WORD-BL.md`) only — **do not** rewrite live interpret splitter / host `WORDS`; SOURCE/PAD thin marks via wave19 **4** (`SOURCE-PAD.md`) only — **do not** live-TIB-rewrite / ACCEPT/REFILL / full input-buffer VM
- `EVALUATE` / `INCLUDE` real nested interpret / file VM — still out; mark-only echo stubs via wave17 **3** (`EVALUATE-INCLUDE.md`) only; refined-boot include markers stay
- `FIND` deepen — ANS-ish find mark via wave18 **2** (`FIND.md`) only; **do not** rewrite host `find`/`findentry`/`entry-find` used by interpret; not SEARCH-WORDLIST / execute-through / SYNONYM FIND rewrite

Body/marker deepen is **in scope** via `docs/COLON.md` (wave9 **4`). Comment skip is **in scope** via `docs/COMMENT-PARSE.md` (wave12 **3**). String-literal parse is **in scope** via `docs/STRING-LIT.md` (wave13 **4**). IMMEDIATE/POSTPONE flag stubs are **in scope** via `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**). EXIT/QUIT thin control markers are **in scope** via `docs/EXIT-QUIT.md` (wave16 **4**). PARSE/PARSE-NAME token-parse markers are **in scope** via `docs/PARSE-NAME.md` (wave17 **2**). EVALUATE/INCLUDE mark-only echo markers are **in scope** via `docs/EVALUATE-INCLUDE.md` (wave17 **3**). FIND ANS-ish find markers are **in scope** via `docs/FIND.md` (wave18 **2**; host find untouched). WORD/BL stub token/pad markers are **in scope** via `docs/WORD-BL.md` (wave18 **3**; host WORDS untouched; not interpret splitter rewrite). STATE/COMPILE, stub markers are **in scope** via `docs/STATE-COMPILE.md` (wave18 **4**; host assistant-state untouched; not linked XT / real STATE cell). SOURCE/PAD thin marks are **in scope** via `docs/SOURCE-PAD.md` (wave19 **4**; host SOURCE/PAD untouched; pad beside WORD-BL word-buffer; not ACCEPT/REFILL / live TIB rewrite / full input-buffer VM).

## 6. Acceptance (Test Lab)

1. `docs/INTERPRET.md` present; KERNEL / COLON may one-line cite; wave12 **3**: `COMMENT-PARSE.md` + thin amend; wave13 **4**: `STRING-LIT.md` + thin amend; wave15 **4**: `IMMEDIATE-POSTPONE.md` + thin amend; wave16 **4**: `EXIT-QUIT.md` + thin amend; wave17 **2**: `PARSE-NAME.md` + thin amend; wave17 **3**: `EVALUATE-INCLUDE.md` + thin amend; wave18 **2**: `FIND.md` + thin amend; wave18 **3**: `WORD-BL.md` + thin amend; wave18 **4**: `STATE-COMPILE.md` + thin amend; wave19 **4**: `SOURCE-PAD.md` + thin amend.
2. `interpret-demo` → OK; `kernel-demo` still OK; wave9 **4**: `colon-demo` → OK; wave12 **3**: `comment-demo` → OK; wave13 **4**: `string-demo` → OK; wave15 **4**: `imm-demo` → OK; wave16 **4**: `exit-demo` → OK; wave17 **2**: `parse-demo` → OK; wave17 **3**: `eval-demo` → OK; wave18 **1**: `tick-demo` → OK; wave18 **2**: `find-demo` → OK; wave18 **3**: `word-demo` → OK; wave18 **4**: `state-demo` → OK; wave19 **4**: `source-demo` → OK.
3. Regression green (nested / vocab / rekia / s3 / …).
4. No merge.

## 7. Cite

- `docs/KERNEL.md`, `docs/FORTH-BASE-REFERENCES.md`
- `docs/COMMENT-PARSE.md` (wave12 **3**)
- `docs/STRING-LIT.md` (wave13 **4**)
- `forth/tritium/kernel.fs`
- Dusk `fs/doc/kernel.txt`
- `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**)
- `docs/EXIT-QUIT.md` (wave16 **4**)
- `docs/PARSE-NAME.md` (wave17 **2**)
- `docs/EVALUATE-INCLUDE.md` (wave17 **3**)
- `docs/FIND.md` (wave18 **2**)
- `docs/TICK.md` (wave18 **1**)
- `docs/WORD-BL.md` (wave18 **3**)
- `docs/STATE-COMPILE.md` (wave18 **4**)
- `docs/SOURCE-PAD.md` (wave19 **4**)
