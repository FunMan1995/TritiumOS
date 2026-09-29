# INTERPRET — Token-stream interpret stub

**Status:** Shipper-ready stub spec (wave8 item **1**; thin amend wave9 **4** colon-stub; thin amend wave12 **3** comment-parse)  
**Canonical brief:** Dusk `fs/doc/kernel.txt` interpret loop; `docs/KERNEL.md` (wave7 **5**)  
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `interpret.fs`); Linux host mirrors  
**Companions:** `docs/KERNEL.md`, `docs/GROUPS-NESTED.md`, `docs/FORTH-BASE-REFERENCES.md`, `docs/COLON.md` (wave9 **4**), `docs/COMMENT-PARSE.md` (wave12 **3**)

## 1. Purpose

Wave7 landed lookup-only `interpret-token`. This tip deepens a **minimal interpret loop**: walk a whitespace-separated token stream, `find` each name, execute or print a stub action. Wave8 `:` was **create-only**; wave9 **4** deepens a **body/marker** stub (`docs/COLON.md`) — still not a full colon compiler or control-flow VM. Wave12 **3** adds **comment skip** on the interpret/token path (`\` EOL + `(` … `)`) — see `docs/COMMENT-PARSE.md`.

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

## 4. `interpret-demo`

1. `dict-reset` (or cold path)
2. `entry-create` (or `:`) two names
3. `interpret` a string containing both + one unknown → hits + one miss marker
4. Assert miss counted / greppable; hits ≥2
5. `[interpret-demo] OK`

Prior `kernel-demo` must stay green.

## 5. Non-goals

- Full immediate/compile state machine beyond colon-def flag
- Control flow (`if`/`then`/`do`) — later
- Full Dusk `findentry` linked units / XT lists
- Host C#/Kotlin full VM rewrite
- String `.(` / `S"` rewrite; real BLOCK comments — still out (comment skip stubs via wave12 **3** only)

Body/marker deepen is **in scope** via `docs/COLON.md` (wave9 **4`). Comment skip is **in scope** via `docs/COMMENT-PARSE.md` (wave12 **3**).

## 6. Acceptance (Test Lab)

1. `docs/INTERPRET.md` present; KERNEL / COLON may one-line cite; wave12 **3**: `COMMENT-PARSE.md` + thin amend.
2. `interpret-demo` → OK; `kernel-demo` still OK; wave9 **4**: `colon-demo` → OK; wave12 **3**: `comment-demo` → OK.
3. Regression green (nested / vocab / rekia / s3 / …).
4. No merge.

## 7. Cite

- `docs/KERNEL.md`, `docs/FORTH-BASE-REFERENCES.md`
- `docs/COMMENT-PARSE.md` (wave12 **3**)
- `forth/tritium/kernel.fs`
- Dusk `fs/doc/kernel.txt`
