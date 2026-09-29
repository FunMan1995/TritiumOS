# INTERPRET — Token-stream interpret stub

**Status:** Shipper-ready stub spec (wave8 item **1**; thin amend wave9 **4** colon-stub)  
**Canonical brief:** Dusk `fs/doc/kernel.txt` interpret loop; `docs/KERNEL.md` (wave7 **5**)  
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `interpret.fs`); Linux host mirrors  
**Companions:** `docs/KERNEL.md`, `docs/GROUPS-NESTED.md`, `docs/FORTH-BASE-REFERENCES.md`, `docs/COLON.md` (wave9 **4**)

## 1. Purpose

Wave7 landed lookup-only `interpret-token`. This tip deepens a **minimal interpret loop**: walk a whitespace-separated token stream, `find` each name, execute or print a stub action. Wave8 `:` was **create-only**; wave9 **4** deepens a **body/marker** stub (`docs/COLON.md`) — still not a full colon compiler or control-flow VM.

## 2. Words

| Word | Stack | Notes |
|------|-------|-------|
| `interpret-token` | `( c-addr u -- flag )` | Existing (KERNEL.md) |
| `interpret` | `( c-addr u -- )` | Split on bl; each token → find → hit: print/exec stub; miss: miss marker (continue or soft-abort — prefer **continue** + count misses) |
| `:` / `colon-create` | `( "name" -- )` | Create + colon-def; body until `;` — see `COLON.md` (wave9 **4**) |
| `;` / `semicolon` | `( -- )` | Close colon-def; body-present marker (`COLON.md`) |
| `interpret-demo` | `( -- )` | See §4 |
| `colon-demo` | `( -- )` | Body/marker smoke (`COLON.md`) |

Execute stub on hit: print `[interpret] exec #<i> <name>` (host may call a bound id later). No nested interpret required this tip.

## 3. Markers

```
[interpret] exec #N name=<word>
[interpret] miss name=<word>
[interpret] : created <name>
[interpret-demo] OK
[interpret-demo] FAIL
```

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

Body/marker deepen is **in scope** via `docs/COLON.md` (wave9 **4**).

## 6. Acceptance (Test Lab)

1. `docs/INTERPRET.md` present; KERNEL / COLON may one-line cite.
2. `interpret-demo` → OK; `kernel-demo` still OK; wave9 **4**: `colon-demo` → OK.
3. Regression green (nested / vocab / rekia / s3 / …).
4. No merge.

## 7. Cite

- `docs/KERNEL.md`, `docs/FORTH-BASE-REFERENCES.md`
- `forth/tritium/kernel.fs`
- Dusk `fs/doc/kernel.txt`
