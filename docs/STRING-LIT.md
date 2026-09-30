# STRING-LIT — Stub string-literal parse `S"` / `."` (optional `.(`) + `string-demo`

**Status:** Shipper-ready stub spec (wave13 item **4**; thin amend wave19 **1** CHAR-CHARS companion cite; thin amend wave19 **4** SOURCE-PAD companion cite; thin amend wave20 **3** COUNT companion cite)
**Canonical brief:** ANS-shaped `S"` / `."` (optional `.(`) string parse (thin stub); `docs/COMMENT-PARSE.md` (wave12 **3**), `docs/INTERPRET.md` (wave8 **1**)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `interpret.fs` / `string.fs`); Linux host REPL
**Companions:** `docs/COMMENT-PARSE.md` (thin amend this tip), `docs/INTERPRET.md` (thin amend this tip), `docs/KERNEL.md`, `docs/CREATE-DOES.md` (wave13 **3**), `docs/THROW-CATCH.md` (wave14 **4**), `docs/PARSE-NAME.md` (wave17 **2**), `docs/WORD-BL.md` (wave18 **3**); `docs/CHAR-CHARS.md` (wave19 **1** — char-unit companion cite); `docs/SOURCE-PAD.md` (wave19 **4** — SOURCE/PAD thin marks companion cite); `docs/COUNT.md` (wave20 **3** — ANS COUNT counted-string picture companion cite; host ENTRY-COUNT/SYN-COUNT/words-count ≠ ANS COUNT)
**Base tip SHA:** `5933306` (wave13 tip3 CLOSED / #59 CREATE-DOES) / full `59333061739637c78c83e8be2ced7275d567f7ea`

## 1. Purpose

Comment skip exists on the interpret/token path (wave12 **3**); string-literal parse was an explicit deferral. This tip deepens the **interpret path** with **stub** `S"` / `."` (optional `.(`): parse until closing `"`, print greppable `[string]` markers (content greppable or `length=`), and smoke via **`string-demo`**. Must **not** `entry-create` the literal text. Not a full counted-string heap / BLOCK, escape rewrite, or real print-to-stdout contract beyond markers. Optional `ABORT"` (parse-until-`"` + THROW-equivalent) → `docs/THROW-CATCH.md` (wave14 **4**; reuses this parse path). Sibling `PARSE` / `PARSE-NAME` token-parse markers → `docs/PARSE-NAME.md` (wave17 **2**; not full tokenizer). Sibling `WORD` / `BL` stub token/pad markers → `docs/WORD-BL.md` (wave18 **3**; not S"/escape heap / interpret rewrite / SOURCE/PAD). Sibling `CHAR` / `CHARS` / `[CHAR]` char-unit markers → `docs/CHAR-CHARS.md` (wave19 **1**; not CHAR+/unicode / S"/escape heap). Sibling `SOURCE` / `PAD` thin marks → `docs/SOURCE-PAD.md` (wave19 **4**; not S"/escape heap / ACCEPT/REFILL / live TIB rewrite / full input-buffer VM). Sibling ANS `COUNT` counted-string picture → `docs/COUNT.md` (wave20 **3**; Forth mirror `count-mark`; **not** S"/escape heap / ACCEPT/REFILL / live TIB rewrite / full counted-string heap; host `ENTRY-COUNT` / `SYN-COUNT` / `words-count` are **not** ANS COUNT).

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `S"` / `string-s` | `( -- c-addr u )` *or* `( -- )` stub | Parse until `"`; print `[string] S" …` (content or `length=<n>`); **no** dict create of literal text |
| `."` / `string-dot` | `( -- )` | Parse until `"`; print `[string] ." …` (content or `length=<n>`); stub “display” = marker only OK |
| `.(` / `string-paren` | `( -- )` | **Optional** this tip; parse until `)`; print `[string] .( …`; prefer if easy alongside comment `(` |
| `string-parse` | `( delim -- c-addr u )` | Optional helper; delim `"` or `)` |
| `string-demo` | `( -- )` | See §5 |

Host note: bind `S"` / `."` (and optional `.(`) on Linux REPL interpret/stream path; Forth mirrors `string-s` / `string-dot` / `string-paren` if host Forth names collide (host Forth often has real `S"` / `."`). Scanner may run inside `interpret` (parse-from-stream after recognizing the word) — either OK if demo markers land and literal text is **not** `entry-create`d.

## 3. Stub semantics

- **`S"`** → when interpret sees `S"` (token), consume characters until the next `"` (same line preferred; EOL-before-close → `[string] FAIL` reason=unclosed — demo must avoid). Print `[string] S" <content>` **or** `[string] S" length=<n>` (either form Lab-greppable). Optional stub push of `( c-addr u )` if host has a temp buffer — **not required**; marker alone is enough.
- **`."`** → same parse-until-`"`; print `[string] ." <content>` **or** `[string] ." length=<n>`. No requirement to write to a real output device beyond the greppable marker line.
- **`.(` (optional)** → parse until `)`; print `[string] .( <content>` or `length=`. Must **not** collide with comment `(` skip (`COMMENT-PARSE.md`): `.(` is a distinct word; `(` remains comment-skip.
- Literal text must **not** create words (no `entry-create` / find hit for the string body) and must **not** emit `[interpret] miss` for the body characters.
- Unclosed string → `[string] FAIL` reason=unclosed (demo must avoid).
- Nest with prior comment / colon / VARIABLE / VALUE / CREATE / control / CASE stubs OK; `dict-reset` unaffected by string parse (no new entries from literals).
- Still no full counted-string heap / BLOCK, escape rewrite (`\"` etc.), real LEAVE jump; those → non-goals / later tips. Optional `ABORT"` (THROW-equivalent + parse-until-`"`) → `docs/THROW-CATCH.md` (wave14 **4**). `PARSE` / `PARSE-NAME` token-parse markers → `docs/PARSE-NAME.md` (wave17 **2**; sibling surface — not S"/escape heap). `WORD` / `BL` stub markers → `docs/WORD-BL.md` (wave18 **3**; sibling token/pad surface — not S"/escape heap / SOURCE/PAD). `CHAR` / `CHARS` / `[CHAR]` char-unit markers → `docs/CHAR-CHARS.md` (wave19 **1**; sibling char-unit — not CHAR+/unicode / S"/escape heap). `SOURCE` / `PAD` thin marks → `docs/SOURCE-PAD.md` (wave19 **4**; sibling input-string / pad-slot — not S"/escape heap / ACCEPT/REFILL / full input-buffer VM). ANS `COUNT` counted-string picture → `docs/COUNT.md` (wave20 **3**; sibling picture mark — not S"/escape heap / ACCEPT/REFILL / full counted-string heap; host ENTRY-COUNT ≠ ANS COUNT).

## 4. Markers

```
[string] S" <content>          # or: [string] S" length=<n>
[string] ." <content>          # or: [string] ." length=<n>
[string] .( <content>          # optional
[string] FAIL reason=<…>
[string-demo] OK
[string-demo] FAIL
```

Lab greps `[string-demo] OK` plus at least one `[string] S"` and one `[string] ."` (content substring greppable **or** `length=` present). Optional `.(` marker if implemented.

## 5. `string-demo`

1. Clean slate / `dict-reset` if available.
2. Feed / invoke `S" hello"` (or fixture) → `[string] S" hello` **or** `[string] S" length=5`; assert `"hello"` / body text **not** an `entry-find` hit / not created word.
3. Feed / invoke `." world"` → `[string] ." world` **or** `[string] ." length=5`.
4. Optional: `.( hi)` → `[string] .( hi` (or length=); comment `(` still skips via prior tip.
5. Prior `create-demo` / `case-demo` / `value-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `var-demo` / `colon-demo` still OK.
6. `[string-demo] OK`.

`S"` + `."` markers and no-dict-create for literal text are required. Optional `.(` preferred when cheap.

## 6. Thin amend — companions

### `docs/COMMENT-PARSE.md`

- Companions: add `STRING-LIT.md` (wave13 **4**).
- Purpose / §3: strike open “Still no `.(` string print, `S"` rewrite”; point string-literal stubs at this tip (parse + markers only; no heap / BLOCK / escape).
- Non-goals: `S"` / `."` / optional `.(` stubs → `docs/STRING-LIT.md` (wave13 **4**); keep full counted-string heap / BLOCK / escape rewrite out.
- Cite: `docs/STRING-LIT.md`.

### `docs/INTERPRET.md`

- Status / companions: cite wave13 **4**; add `STRING-LIT.md`.
- §1 / `interpret` row: note string-literal parse (`S"` / `."`, optional `.(`) on the interpret/stream path after comment skip; point to `STRING-LIT.md`.
- Words table: add `S"` / `."` (+ optional `.(`) + `string-demo` (cite tip).
- Markers: one-line pointer to `[string]` markers.
- Non-goals: strike blanket “String `.(` / `S"` rewrite still out”; stubs in scope via this tip; keep full counted-string heap / BLOCK / escape out.
- Acceptance: Lab smokes `string-demo`.
- Cite: `docs/STRING-LIT.md`.

## 7. Non-goals

- Full counted-string heap / `ALLOCATE` / BLOCK / screen strings
- Escape rewrite (`\"`, hex embeds, etc.)
- `PARSE` / `PARSE-NAME` token-parse markers: see `docs/PARSE-NAME.md` (wave17 **2**); full parser / tokenizer VM still out
- `WORD` / `BL` stub markers: see `docs/WORD-BL.md` (wave18 **3**); full interpret WORD/BL rewrite / S"/escape heap still out
- `SOURCE` / `PAD` thin marks: see `docs/SOURCE-PAD.md` (wave19 **4**); ACCEPT/REFILL / live TIB rewrite / full input-buffer VM / S"/escape heap still out
- ANS `COUNT` counted-string picture: see `docs/COUNT.md` (wave20 **3**; Forth mirror `count-mark`); host `ENTRY-COUNT` / `SYN-COUNT` / `words-count` are **not** ANS COUNT; ACCEPT/REFILL / full counted-string heap / live TIB rewrite still out
- `CHAR` / `CHARS` / `[CHAR]` char-unit stubs: see `docs/CHAR-CHARS.md` (wave19 **1**); CHAR+/unicode / XCHAR still out
- Real LEAVE jump (LEAVE-AGAIN remains stub markers)
- Optional `ABORT"` / CATCH / THROW stubs: see `docs/THROW-CATCH.md` (wave14 **4**; may reuse this parse path)
- Docs cites pass (wave13 **5** / wave14 **5**)
- CREATE / DOES> (done — wave13 **3**); CASE / OF (done — wave13 **2**); VALUE / TO (done — wave13 **1**); UNLOOP / J (done — wave14 **2**)
- Real crypto / network fleet
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/STRING-LIT.md` present (Research byte-copy OK); `COMMENT-PARSE.md` + `INTERPRET.md` thin amends present.
2. `string-demo` → OK (markers §4); literal text not in dict; `create-demo` + `case-demo` + `value-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `var-demo` + `colon-demo` still OK; wave17 **2**: `parse-demo` → OK (retains `string-demo`); wave18 **3**: `word-demo` → OK (retains `string-demo`); wave19 **1**: `char-demo` → OK (retains `string-demo`); wave19 **4**: `source-demo` → OK (retains `string-demo` + `word-demo`); wave20 **3**: `count-demo` → OK (retains `string-demo` + `source-demo` + `word-demo`; host ENTRY-COUNT ≠ ANS COUNT).
3. Regression green (wave13 **1–3** + wave12 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `string-demo CONTRACT` OK).
5. No merge. Stay out of Mango.

## 9. Cite

- `docs/COMMENT-PARSE.md` (wave12 **3**), `docs/INTERPRET.md` (wave8 **1**), `docs/KERNEL.md` (wave7 **5**), `docs/CREATE-DOES.md` (wave13 **3**), `docs/THROW-CATCH.md` (wave14 **4**), `docs/PARSE-NAME.md` (wave17 **2**), `docs/WORD-BL.md` (wave18 **3**), `docs/CHAR-CHARS.md` (wave19 **1**), `docs/SOURCE-PAD.md` (wave19 **4**), `docs/COUNT.md` (wave20 **3** — ANS COUNT picture; host ENTRY-COUNT ≠ ANS COUNT)
- `forth/tritium/kernel.fs`
- ANS Forth `S"` / `."` / `.(` (stub only); Dusk interpret / string surface (stub only)
- Base tip: `5933306` / `59333061739637c78c83e8be2ced7275d567f7ea`
- Wave13 proposal: `/workspace/tritium-research-docs/WAVE13-PROPOSAL.md`
