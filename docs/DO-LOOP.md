# DO-LOOP — Counted-loop stubs (`DO` / `LOOP` / `+LOOP` / `I`)

**Status:** Shipper-ready stub spec (wave12 item **1**)
**Canonical brief:** Dusk counted-loop compile (thin stub); `docs/BEGIN-UNTIL.md` (wave11 **1**), `docs/CONTROL.md` (wave10 **2**)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `control.fs` / `loop.fs`); Linux host REPL
**Companions:** `docs/BEGIN-UNTIL.md` (thin amend this tip), `docs/CONTROL.md` (thin amend this tip), `docs/COLON.md`, `docs/INTERPRET.md`, `docs/VARIABLE-CONST.md` (wave12 **2**), `docs/LEAVE-AGAIN.md` (wave12 **4**), `docs/UNLOOP-J.md` (wave14 **2**)
**Base tip SHA:** `23fe4e5` (wave11 tip5 CLOSED / #51)

## 1. Purpose

`BEGIN`/`UNTIL`/`WHILE`/`REPEAT` stubs exist (loop-depth balance). This tip adds **counted-loop stubs**: `DO` / `LOOP` / `+LOOP` / `I` that push/pop a do-loop frame (sibling of cs / BEGIN loop-depth), print greppable `[do-loop]` markers, and smoke via **`do-loop-demo`**. Not real branch XT patching, not a runtime counted loop that re-executes body tokens.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `DO` / `control-do` | `( limit start -- )` *or* `( -- )` | Push do-loop frame; optional limit/start stash for `I` stub; print `[do-loop] DO depth=<n>` (+ optional `index=<i>`) |
| `LOOP` / `control-loop` | `( -- )` | Close do-loop (+1 step stub); pop frame; print `[do-loop] LOOP depth=<n>` |
| `+LOOP` / `control-plus-loop` | `( n -- )` *or* `( -- )` | Close do-loop with step stub; pop frame; print `[do-loop] +LOOP depth=<n>` (+ optional `step=<n>`) |
| `I` / `control-i` | `( -- )` *or* `( -- n )` | Print stub index for open DO; `[do-loop] I index=<i>`; optional push of stub index |
| `do-loop-depth` | `( -- n )` | Optional; may share / sibling of `loop-cs-depth` / `control-cs-depth` |
| `do-loop-demo` | `( -- )` | See §5 |

Host note: bind `DO`/`LOOP`/`+LOOP`/`I` on Linux REPL; Forth mirrors `control-do` / `control-loop` / `control-plus-loop` / `control-i` if host Forth names collide (host Forth often uses real `DO`/`LOOP`).

## 3. Stub semantics

- **DO** +1 do-loop depth; **LOOP** / **+LOOP** −1.
- **I** requires open DO; does not change depth. Index stub may be the optional start value, a fixed `0`, or a host-held counter — print it; no requirement to increment across body tokens this tip.
- Unbalanced LOOP/+LOOP/I → `[do-loop] FAIL` reason=unbalanced.
- No requirement to re-execute body tokens this tip — markers + depth balance are enough (optional `index=` / `step=` from stack if present).
- Nest with `IF`/`THEN`/`ELSE` and `BEGIN`/`UNTIL`/`WHILE`/`REPEAT` stubs OK if each depth family returns to 0 at demo end (do-loop depth independent of BEGIN loop-depth preferred; shared counter acceptable if demos stay balanced).
- Still no real compile-time branch / XT lists. `LEAVE` may mark an open DO frame and does not pop it (`LOOP` / `+LOOP` still close); `AGAIN` is a BEGIN closer — `docs/LEAVE-AGAIN.md` (wave12 **4**). `UNLOOP` pops one DO frame without `LOOP`/`+LOOP` step semantics; `J` is outer-index when do-depth ≥ 2 — `docs/UNLOOP-J.md` (wave14 **2**).

## 4. Markers

```
[do-loop] DO depth=<n> [index=<i>]   # index= optional
[do-loop] I index=<i>
[do-loop] LOOP depth=<n>
[do-loop] +LOOP depth=<n> [step=<n>] # step= optional
[do-loop] FAIL reason=<…>
[do-loop-demo] OK
[do-loop-demo] FAIL
```

Lab greps `[do-loop-demo] OK` plus `[do-loop]` depth/index lines: at least one `DO`, one `I`, and at least one of `LOOP` or `+LOOP` (prefer both patterns in the demo). `UNLOOP` / `J` also print under `[do-loop]` — see `docs/UNLOOP-J.md` (wave14 **2**); do not require them in `do-loop-demo`.

## 5. `do-loop-demo`

1. Clean slate / `dict-reset` if available (resets do-loop depth too).
2. Stream A: `DO` … `I` … `LOOP` (with or without limit/start) → DO + I + LOOP markers; do-loop depth 0.
3. Stream B: `DO` … `I` … `+LOOP` (with or without step) → DO + I + +LOOP markers; depth 0.
4. Prior `loop-demo` / `control-demo` still OK.
5. `[do-loop-demo] OK`.

## 6. Thin amend — companions

### `docs/BEGIN-UNTIL.md`

- Companions: add `DO-LOOP.md` (wave12 **1**).
- Purpose / §3: strike blanket “not `DO`/`LOOP`/`+LOOP`” / “Still no `DO`/`LOOP`/`+LOOP` (later wave)”; point to wave12 **1** for counted-loop stubs.
- Non-goals: move `DO`/`LOOP`/`+LOOP` to cite `docs/DO-LOOP.md`; keep `LEAVE` / `AGAIN` out (wave12 **4**).
- Cite: `docs/DO-LOOP.md`.

### `docs/CONTROL.md`

- Companions: add `DO-LOOP.md` (wave12 **1**).
- §3 / Non-goals: strike “`DO`/`LOOP` still out”; point to `docs/DO-LOOP.md` (wave12 **1**).
- Cite: `docs/DO-LOOP.md`.

## 7. Non-goals

- Real compile-time branch / XT lists / runtime counted re-exec
- `LEAVE` / `AGAIN` stubs: see `docs/LEAVE-AGAIN.md` (wave12 **4**)
- `UNLOOP` / `J` stubs: see `docs/UNLOOP-J.md` (wave14 **2**)
- `VARIABLE` / `CONSTANT` stubs: see `docs/VARIABLE-CONST.md` (wave12 **2**); comment-parse: see `docs/COMMENT-PARSE.md` (wave12 **3**); docs cites (wave12 **5**)
- Real crypto / network fleet
- Full Win/Android Forth VM (CONTRACT acceptable)

## 8. Acceptance (Test Lab)

1. `docs/DO-LOOP.md` present (Research byte-copy OK); `BEGIN-UNTIL.md` + `CONTROL.md` thin amends present.
2. `do-loop-demo` → OK (markers §4); `loop-demo` + `control-demo` still OK.
3. Regression green (wave11 demos).
4. Win/Android: CONTRACT acceptable (parity line `do-loop-demo CONTRACT` OK).
5. No merge. Stay out of Mango.

## 9. Cite

- `docs/BEGIN-UNTIL.md` (wave11 **1**), `docs/CONTROL.md` (wave10 **2**), `docs/COLON.md`, `docs/KERNEL.md`
- `forth/tritium/kernel.fs`
- Dusk counted-loop words (stub only)
- Base tip: `23fe4e5`
- `docs/LEAVE-AGAIN.md` (wave12 **4**)
- `docs/UNLOOP-J.md` (wave14 **2**)
