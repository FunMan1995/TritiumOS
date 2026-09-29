# CONTROL — `IF` / `THEN` / `ELSE` stubs

**Status:** Shipper-ready stub spec (wave10 item **2**)
**Canonical brief:** Dusk control compile (thin stub); `docs/COLON.md` (wave9 **4**)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `control.fs`); Linux host REPL
**Companions:** `docs/COLON.md` (thin amend this tip), `docs/INTERPRET.md`, `docs/KERNEL.md`

## 1. Purpose

Colon body/marker exists; control words do not. This tip lands **stub** `IF` / `THEN` / `ELSE` (and optional `control-if` aliases) that recognize the words, maintain a tiny **cs** (control-stack) depth counter, and print greppable markers — **not** real branch XT patching or a threaded compiler.

## 2. Words

| Word | Stack | Notes |
|------|-------|-------|
| `IF` / `control-if` | `( flag -- )` *or* `( -- )` in colon-def | Consume flag if available; push cs frame; print `[control] IF` (+ optional `taken`/`skip` stub) |
| `ELSE` / `control-else` | `( -- )` | Flip stub branch side; print `[control] ELSE` |
| `THEN` / `control-then` | `( -- )` | Pop cs frame; print `[control] THEN depth=<n>` |
| `control-cs-depth` | `( -- n )` | Optional helper |
| `control-demo` | `( -- )` | See §5 |

Outside colon-def, `IF` may still run as an immediate stub that prints markers and balances with `THEN` (demo-friendly). Inside colon-def, body-token counter (COLON) still increments for these tokens; control markers are additional.

Host note: may bind `IF`/`THEN`/`ELSE` on Linux REPL; Forth mirrors `control-if` / `control-then` / `control-else` if host Forth names collide.

## 3. Stub semantics

- **Balance only:** each `IF` +1 cs; each `THEN` −1; `ELSE` requires open `IF` and does not change depth.
- Unbalanced `THEN` / `ELSE` → `[control] FAIL` reason=unbalanced (demo must avoid this).
- Flag path (optional): if a numeric flag is on data stack at `IF`, print `taken=1` when nonzero else `taken=0` — **no** requirement to skip tokens this tip (skip can be a no-op print).
- No real forward/back branch patching, no `BEGIN`/`UNTIL`/`DO`/`LOOP`.

## 4. Markers

```
[control] IF [taken=0|1]      # taken= optional
[control] ELSE
[control] THEN depth=<n>
[control] FAIL reason=<…>     # only on error paths
[control-demo] OK
[control-demo] FAIL
```

Lab greps `[control-demo] OK` plus at least one each of `IF` / `THEN` (ELSE preferred).

## 5. `control-demo`

1. `dict-reset` (or equivalent clean slate).
2. Run a balanced stream: `IF` … `THEN` (with or without a flag push) → markers.
3. Run `IF` … `ELSE` … `THEN` → all three markers.
4. Assert `control-cs-depth` is 0 after (if exposed).
5. Prior `colon-demo` + `interpret-demo` + `host-boot-demo` still OK.
6. Print `[control-demo] OK`.

## 6. Thin amend — `docs/COLON.md`

- Companions: add `CONTROL.md`.
- Non-goals: strike “control flow remain out” / point to wave10 **2** for `IF`/`THEN`/`ELSE` stubs.
- Cite: `docs/CONTROL.md`.

## 7. Non-goals

- Real compile-time branch resolution / XT lists
- `BEGIN`/`UNTIL`/`WHILE`/`REPEAT` / `DO`/`LOOP`
- Address-fold / refined-boot (wave10 **3–4**)
- Full Win/Android Forth VM

## 8. Acceptance (Test Lab)

1. `docs/CONTROL.md` present (Research byte-copy OK); `COLON.md` thin amend present.
2. `control-demo` → OK (markers §4); `colon-demo` + `interpret-demo` still OK.
3. Regression green (wave10 **1** + wave9 demos).
4. No merge.

## 9. Cite

- `docs/COLON.md`, `docs/INTERPRET.md`, `docs/KERNEL.md`
- `forth/tritium/kernel.fs`
- Dusk control words (stub only)
