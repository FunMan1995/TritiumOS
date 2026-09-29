# COMMENT-PARSE — `\` line + `(` `)` paren comment skip + `comment-demo`

**Status:** Shipper-ready stub spec (wave12 item **3**)
**Canonical brief:** Dusk / ANS Forth comment skip (thin stub); `docs/INTERPRET.md` (wave8 **1**), `docs/KERNEL.md` (wave7 **5**)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `interpret.fs` / `comment.fs`); Linux host REPL
**Companions:** `docs/INTERPRET.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip, optional), `docs/VARIABLE-CONST.md` (wave12 **2**)
**Base tip SHA:** `c3b8c6a` (wave12 tip2 CLOSED / #53) / full `c3b8c6aec26c849e9e72f69fe7081050a0e501b0`

## 1. Purpose

Interpret walks whitespace-separated tokens today; it does **not** skip Forth comments. This tip adds **comment-parse stubs** on the interpret / token path:

- Backslash **`\`** — skip from `\` through **end-of-line** (rest of that line is not tokens).
- Paren **`(` … `)`** — skip a paren comment; **prefer non-nested** first for this stub (nested optional later).

Print greppable `[comment]` markers and smoke via **`comment-demo`**. Prove comments do **not** create dict words and do **not** break interpret of real tokens mixed around them. Not string `.(` / `S"` rewrite, not `LEAVE`/`AGAIN`, not real Forth BLOCK comments.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `\` / `comment-line` | `( -- )` *or* consumed in stream | Skip to EOL; print `[comment] skip line` |
| `(` / `comment-paren` | `( -- )` *or* consumed in stream | Skip until matching `)`; print `[comment] skip paren`; **non-nested first** (first `)` closes) |
| `comment-skip?` | `( c-addr u -- flag )` | Optional helper: true if token starts a comment form |
| `interpret` (deepen) | `( c-addr u -- )` | Existing; **before** find/exec, skip comment spans in the stream |
| `interpret-token` (deepen) | `( c-addr u -- flag )` | Existing; if token is `\` or `(` start, skip rather than find-miss |
| `comment-demo` | `( -- )` | See §5 |

Host note: bind `\` / `(` on Linux REPL interpret path (stream scanner preferred over redefining host Forth `\`). Forth mirrors `comment-line` / `comment-paren` if host names collide. Scanner may run **inside** `interpret` before whitespace-split, or treat `\` / `(` as immediate skip words — either OK if demo markers + no spurious dict creates.

## 3. Stub semantics

- **Line comment:** when interpret sees `\` (as a token, or as start of remainder after BL), discard characters through newline (or end of buffer). Print `[comment] skip line` once per skip. Tokens on the **same** line *before* `\` still interpret; after `\` do not.
- **Paren comment:** when interpret sees `(` (token), discard until the next `)` (non-nested stub). Print `[comment] skip paren` once per skip. Content inside is not tokens / not `entry-create` / not find hits. Unclosed `(` → `[comment] FAIL` reason=unclosed (demo must avoid).
- **Nested paren (optional):** if easy, count nest depth; **not required** this tip — prefer non-nested first.
- Comments must **not** create words (no `entry-create` for comment text) and must **not** emit `[interpret] miss` for discarded spans.
- Real tokens before/after comments still find/exec as today (`[interpret] exec` / miss only for real tokens).
- Nest with prior colon / control / loop / do-loop / var stubs OK; dict-reset unaffected by comment skips.
- Still no `.(` string print, `S"` rewrite, `LEAVE`/`AGAIN`, or BLOCK / `\`-in-string edge rewrite this tip.

## 4. Markers

```
[comment] skip line
[comment] skip paren
[comment] FAIL reason=<…>
[comment-demo] OK
[comment-demo] FAIL
```

Lab greps `[comment-demo] OK` plus at least one `[comment] skip line` and one `[comment] skip paren`. Optional: assert no `[interpret] miss` for comment-body text; real token hits still greppable.

## 5. `comment-demo`

1. Clean slate / `dict-reset` if available.
2. Create ≥1 known name (e.g. `entry-create` / prior fixture) so real tokens can hit.
3. Feed a stream / lines mixing:
   - Real token(s) + `\ this is a line comment` → real token(s) exec; `[comment] skip line`; comment text not in dict / not miss-as-word.
   - Real token(s) + `( paren comment )` + real token(s) → `[comment] skip paren`; flanking tokens still exec.
4. Assert: comment body strings are **not** `entry-find` hits / not created words; interpret of real tokens still OK (hit markers ≥1).
5. Prior `var-demo` / `do-loop-demo` / `interpret-demo` still OK.
6. `[comment-demo] OK`.

## 6. Thin amend — companions

### `docs/INTERPRET.md`

- Status / companions: cite wave12 **3**; add `COMMENT-PARSE.md`.
- §1 / `interpret` row: note comment skip (`\` EOL + `(` … `)`) before find/exec; point to `COMMENT-PARSE.md`.
- Words table: add `\` / `(` skip + `comment-demo` (cite tip).
- Markers: optional one-line pointer to `[comment]` markers.
- Non-goals: strike blanket “no comments”; keep full string/`S"` / BLOCK out.
- Acceptance: Lab smokes `comment-demo`.
- Cite: `docs/COMMENT-PARSE.md`.

### `docs/KERNEL.md` (optional one-line)

- Companions / See also: add `COMMENT-PARSE.md` (wave12 **3**).
- Non-goals / cite: interpret comment skip via wave12 **3**; still no full parser rewrite.
- Cite: `docs/COMMENT-PARSE.md`.

### `docs/VARIABLE-CONST.md` (optional one-line)

- Non-goals: strike open “Comment-parse (wave12 **3**)”; point to `docs/COMMENT-PARSE.md`.

## 7. Non-goals

- String `.(` / `S"` / `."` rewrite or string-literal parser
- `LEAVE` / `AGAIN` (wave12 **4**); docs cites (wave12 **5**)
- Real Forth BLOCK comments / screen comments / multi-line `\` beyond EOL
- Nested paren comments (optional; prefer non-nested stub first)
- Real crypto / network fleet
- Full Win/Android Forth VM (CONTRACT acceptable)

## 8. Acceptance (Test Lab)

1. `docs/COMMENT-PARSE.md` present (Research byte-copy OK); `INTERPRET.md` thin amend present (`KERNEL.md` one-line optional).
2. `comment-demo` → OK (markers §4); `var-demo` + `do-loop-demo` + `interpret-demo` still OK.
3. Regression green (wave12 **1–2** + prior demos).
4. Win/Android: CONTRACT acceptable (parity line `comment-demo CONTRACT` OK).
5. No merge. Stay out of Mango.

## 9. Cite

- `docs/INTERPRET.md` (wave8 **1**), `docs/KERNEL.md` (wave7 **5**), `docs/VARIABLE-CONST.md` (wave12 **2**)
- `forth/tritium/kernel.fs`
- Dusk / ANS Forth `\` and `(` comments (stub only)
- Base tip: `c3b8c6a` / `c3b8c6aec26c849e9e72f69fe7081050a0e501b0`
