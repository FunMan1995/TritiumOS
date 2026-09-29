# COLON — Deepen `:` beyond create-only (body marker)

**Status:** Shipper-ready stub spec (wave9 item **4**)
**Canonical brief:** Dusk colon compile (thin stub); `docs/INTERPRET.md` (wave8 **1**)
**Sources of truth (code):** `forth/tritium/kernel.fs` (`colon-create*`); Linux host REPL `:` bind
**Companions:** `docs/INTERPRET.md` (thin amend wave9 **4**), `docs/KERNEL.md`, `docs/CONTROL.md` (wave10 **2**), `docs/VARIABLE-CONST.md` (wave12 **2**), `docs/CREATE-DOES.md` (wave13 **3**)

## 1. Purpose

Wave8 `:` / `colon-create` is **create-only** (name into dict, no body). This tip adds a **body/marker stub**: open with `:`, accept body tokens until `;`, mark the entry as having a body, and on exec print a greppable body marker. Not a real threaded compiler, not `IF`/`THEN`/`DO`.

## 2. Words

| Word | Stack | Notes |
|------|-------|-------|
| `:` / `colon-create` | `( "name" -- )` | Create entry; enter **colon-def** stub state |
| `colon-create-from` | `( c-addr u -- )` | Same from counted/addr pair (demos) |
| `;` / `semicolon` | `( -- )` | End colon-def; store **body-present** (and optional body-token count); leave interpret state |
| `colon-body?` | `( i -- flag )` | Entry index has body marker (optional helper) |
| `colon-demo` | `( -- )` | See §5 |

While in colon-def state, each subsequent token (until `;`) increments a body-token counter and may print `[colon] body + <tok>` — **no** real compile XT list required this tip. A single fixed body string `"BODY"` stored on the entry is enough if counting tokens is awkward on host.

Host note: cannot always redefine host Forth `:` / `;` while boot includes follow — Forth mirrors `colon-create` / `semicolon` OK; Linux REPL may bind `:` / `;` to the stubs.

## 3. Exec behavior

On `interpret` / exec stub hit for an entry with body-present:

```
[interpret] exec #N name=<word>
[colon] run body name=<word> tokens=<k>
```

Create-only entries (no `;`) keep prior create-only behavior (exec name only, no `[colon] run body`).

## 4. Markers

```
[colon] : <name>
[colon] body + <tok>          # optional per token
[colon] ; name=<name> tokens=<k>
[colon] run body name=<name> tokens=<k>
[colon-demo] OK
[colon-demo] FAIL
```

Also keep `[interpret] : created <name>` if useful for back-compat, or replace with `[colon] : <name>` — Lab greps `[colon-demo] OK` + at least one `[colon] run body`.

## 5. `colon-demo`

1. `dict-reset`.
2. Define `: square` (or fixture name) with ≥1 body token then `;` → tokens ≥1.
3. `interpret` a stream that executes that name → `[colon] run body` greppable.
4. Assert create-only path still works for a second name without `;` **or** skip and only test body path (prefer both).
5. Prior `interpret-demo` + `kernel-demo` still OK.
6. Print `[colon-demo] OK`.

## 6. Thin amend — `docs/INTERPRET.md`

- Status: cite wave9 **4**.
- Companions: add `COLON.md`.
- §1 / `:` row: deepen to body/marker; point to `COLON.md`.
- Non-goals: full compiler / control flow remain out; body stub is in scope via colon tip.
- Acceptance: Lab smokes `colon-demo`.

## 7. Non-goals

- Immediate vs compile state machine (beyond colon-def flag)
- Control flow beyond stubs — see `docs/CONTROL.md` (wave10 **2**) for `IF`/`THEN`/`ELSE` stubs; `DO`/`BEGIN` still out
- Named-cell stubs (`VARIABLE`/`CONSTANT`) → `docs/VARIABLE-CONST.md` (wave12 **2**); CREATE/DOES> defining-word stubs → `docs/CREATE-DOES.md` (wave13 **3**; markers only, no real XT child)
- Linked XT lists / Dusk `comp/` emitter
- assistant-state / docs cites (wave9 **3** done; **5** next)

## 8. Acceptance (Test Lab)

1. `docs/COLON.md` present (Research byte-copy OK); `INTERPRET.md` thin amend present.
2. `colon-demo` → OK (markers §4); `interpret-demo` + `kernel-demo` still OK.
3. Regression green (`assistant-state-demo`, `economy-wire-demo`, `trit-math-demo`, …).
4. No merge.

## 9. Cite

- `docs/INTERPRET.md`, `docs/KERNEL.md`
- `forth/tritium/kernel.fs`
- Dusk `fs/doc/kernel.txt` (colon — stub only)
- `docs/CONTROL.md` (wave10 **2**)
- `docs/VARIABLE-CONST.md` (wave12 **2**)
- `docs/CREATE-DOES.md` (wave13 **3**)
