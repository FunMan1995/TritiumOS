# BEGIN-UNTIL — Loop stubs (`BEGIN` / `UNTIL` / `WHILE` / `REPEAT`)

**Status:** Shipper-ready stub spec (wave11 item **1**)
**Canonical brief:** Dusk loop compile (thin stub); `docs/CONTROL.md` (wave10 **2**)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `control.fs` / `loop.fs`); Linux host REPL
**Companions:** `docs/CONTROL.md` (thin amend this tip), `docs/COLON.md`, `docs/INTERPRET.md`

## 1. Purpose

`IF`/`THEN`/`ELSE` stubs exist (cs balance). This tip adds **loop stubs**: `BEGIN` / `UNTIL` / `WHILE` / `REPEAT` that push/pop loop frames on the same (or sibling) control stack, print greppable markers, and smoke via **`loop-demo`**. Not real back-branch XT patching, not `DO`/`LOOP`/`+LOOP`.

## 2. Words

| Word | Stack | Notes |
|------|-------|-------|
| `BEGIN` / `control-begin` | `( -- )` | Push loop frame; print `[loop] BEGIN depth=<n>` |
| `UNTIL` / `control-until` | `( flag -- )` *or* `( -- )` | Pop loop frame; print `[loop] UNTIL [again=0\|1]`; flag optional (nonzero → again stub) |
| `WHILE` / `control-while` | `( flag -- )` *or* `( -- )` | Mid-loop gate; print `[loop] WHILE [cont=0\|1]`; requires open BEGIN |
| `REPEAT` / `control-repeat` | `( -- )` | Close WHILE-loop; pop frame; print `[loop] REPEAT` |
| `loop-cs-depth` | `( -- n )` | Optional; may share `control-cs-depth` |
| `loop-demo` | `( -- )` | See §5 |

Host note: bind names on Linux REPL; Forth mirrors `control-*` if collisions.

## 3. Stub semantics

- **BEGIN** +1 loop/cs depth; **UNTIL** / **REPEAT** −1.
- **WHILE** requires open BEGIN; does not change depth.
- Unbalanced UNTIL/REPEAT/WHILE → `[loop] FAIL` reason=unbalanced.
- No requirement to re-execute body tokens this tip — markers + depth balance are enough (optional `again=` / `cont=` from flag if present).
- `IF`/`THEN`/`ELSE` from wave10 still work; nest stubs OK if depth returns to 0 at demo end.
- Still no `DO`/`LOOP`/`+LOOP` (later wave).

## 4. Markers

```
[loop] BEGIN depth=<n>
[loop] UNTIL [again=0|1]
[loop] WHILE [cont=0|1]
[loop] REPEAT
[loop] FAIL reason=<…>
[loop-demo] OK
[loop-demo] FAIL
```

Lab greps `[loop-demo] OK` plus `BEGIN` and at least one of `UNTIL` or `REPEAT` (prefer both patterns).

## 5. `loop-demo`

1. Clean slate / `dict-reset` if available.
2. Stream A: `BEGIN` … `UNTIL` (with or without flag) → BEGIN + UNTIL markers; depth 0.
3. Stream B: `BEGIN` … `WHILE` … `REPEAT` → all three markers; depth 0.
4. Prior `control-demo` still OK.
5. `[loop-demo] OK`.

## 6. Thin amend — `docs/CONTROL.md`

- Companions: add `BEGIN-UNTIL.md`.
- §3 / Non-goals: strike blanket “no BEGIN/UNTIL”; point to wave11 **1** for loop stubs; keep `DO`/`LOOP` out.
- Cite: `docs/BEGIN-UNTIL.md`.

## 7. Non-goals

- Real compile-time branch / XT lists / counted loops
- `DO` / `LOOP` / `+LOOP` / `LEAVE`
- Assistant-s0 / words-vocab / AppImage hang (wave11 **2–4**)
- Full Win/Android Forth VM

## 8. Acceptance (Test Lab)

1. `docs/BEGIN-UNTIL.md` present (Research byte-copy OK); `CONTROL.md` thin amend present.
2. `loop-demo` → OK (markers §4); `control-demo` still OK.
3. Regression green (wave10 demos).
4. No merge.

## 9. Cite

- `docs/CONTROL.md`, `docs/COLON.md`, `docs/KERNEL.md`
- `forth/tritium/kernel.fs`
- Dusk loop words (stub only)
