# THROW-CATCH — Stub `CATCH` / `THROW` (+ optional `ABORT"`) + `throw-demo`

**Status:** Shipper-ready stub spec (wave14 item **4**)
**Canonical brief:** ANS-shaped `CATCH` / `THROW` (thin stub); optional `ABORT"`; `docs/CONTROL.md` (wave10 **2**), `docs/KERNEL.md` (wave7 **5**), `docs/STRING-LIT.md` (wave13 **4**)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `throw.fs` / `control.fs`); Linux host REPL
**Companions:** `docs/CONTROL.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip), `docs/STRING-LIT.md` (thin amend this tip — optional `ABORT"`), `docs/INTERPRET.md` (wave8 **1**; ABORT" may ride interpret/stream path)
**Base tip SHA:** `238258b` (wave14 tip3 CLOSED / #64 2VARIABLE) / full `238258bafa82d818beebd572a7cadc3739204531`

## 1. Purpose

Control / loop / cell / string stubs exist; exception frame does not. This tip lands **stub** `CATCH` / `THROW` (optional thin `ABORT"`): `CATCH` pushes a catch-depth / frame mark, `THROW 0` is a no-op (depth unchanged), `THROW n` (n≠0) pops one catch frame and prints greppable `[throw]` markers, and smokes via **`throw-demo`**. Optional `ABORT"` = parse-until-`"` + THROW-equivalent marker (reuse string-lit parse path). **Frame-mark only** — not a real exception stack, RS unwind, or frame restore. Independent of tips 1–3; closes useful abort surface without real unwind. Soft `abort` / `(abort")` already in KERNEL remain; this tip adds ANS-shaped CATCH/THROW markers beside them.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `CATCH` / `catch-mark` | `( xt -- n )` *or* `( -- )` stub | Push catch-depth / frame mark; print `[throw] CATCH depth=<n>`; stub may ignore real xt exec — marker + depth enough |
| `THROW` / `throw-code` | `( n -- )` | `n=0` → no-op / depth unchanged; print `[throw] THROW code=0` (optional) or silent; `n≠0` → pop one catch frame + print `[throw] THROW code=<n>` |
| `ABORT"` / `abort-quote` (optional) | `( -- )` *or* parse stream | Parse until `"`; print `[throw] ABORT" …` (content or `length=`) + THROW-equivalent nonzero code marker; reuse string-lit parse path |
| `catch-depth` | `( -- n )` | Optional helper; open catch frames |
| `throw-demo` | `( -- )` | See §5 |

Host note: bind `CATCH` / `THROW` (and optional `ABORT"`) on Linux REPL; Forth mirrors `catch-mark` / `throw-code` / `abort-quote` if host Forth names collide (host Forth often has real `CATCH`/`THROW`). Soft KERNEL `abort` / `(abort")` stay as-is — do **not** replace with hard exit. Missing CATCH when THROW n≠0 → `[throw] FAIL` reason=uncaught (demo must avoid — always CATCH then THROW).

## 3. Stub semantics

- **CATCH** → +1 catch-depth (or push frame mark); print `[throw] CATCH depth=<n>` after push. Optional stub “execute xt” is a no-op / immediate continue — **no** real nested interpret / RS save this tip.
- **THROW 0** → no-op; catch-depth unchanged; optional `[throw] THROW code=0` marker (Lab OK without it if silent).
- **THROW n** (n≠0) → require open CATCH; −1 catch-depth; print `[throw] THROW code=<n>`. Does **not** walk real RS / restore stacks / skip tokens beyond the mark.
- Uncaught THROW (depth 0 / no CATCH) → `[throw] FAIL` reason=uncaught (demo must avoid).
- Optional **`ABORT"`** → parse-until-`"` (same line preferred; reuse `STRING-LIT.md` scanner); print `[throw] ABORT" <content>` **or** `[throw] ABORT" length=<n>`; act as THROW-equivalent (nonzero code, pop catch if open). Must **not** `entry-create` the literal text. Unclosed → `[throw] FAIL` reason=unclosed (demo avoids) **or** reuse string FAIL form.
- Nest with prior control / loop / allot / 2var / string / colon stubs OK; `dict-reset` may clear catch-depth to 0 (or leave host counter — demo resets explicitly if needed).
- Still no real exception RS unwind / frame restore, IMMEDIATE/POSTPONE, real branch XT, full Win/Android Forth VM — those → non-goals / later.

## 4. Markers

```
[throw] CATCH depth=<n>
[throw] THROW code=<n>
[throw] ABORT" <content>          # optional; or length=<n>
[throw] FAIL reason=<…>
[throw-demo] OK
[throw-demo] FAIL
```

Lab greps `[throw-demo] OK` plus at least one `[throw] CATCH depth=` and one `[throw] THROW code=` with nonzero code (or explicit `code=0` path plus a nonzero path — prefer one nonzero THROW under CATCH). Optional `ABORT"` not required for Lab OK.

## 5. `throw-demo`

1. Clean slate / `dict-reset` if available; ensure catch-depth 0.
2. `CATCH` (or fixture that marks a catch frame) → `[throw] CATCH depth=1` (or ≥1).
3. `0 THROW` → depth unchanged (optional `code=0` marker).
4. `CATCH` again if needed so depth ≥1; then nonzero `THROW` (e.g. `1 THROW` or `42 THROW`) → `[throw] THROW code=<n>` with n≠0; depth returns toward 0.
5. Optional: under CATCH, `ABORT" boom"` → `[throw] ABORT" boom` (or length=) + THROW-equivalent; else skip.
6. Assert no `[throw] FAIL` reason=uncaught on the happy path; catch-depth 0 after balanced sequence (if exposed).
7. Prior `2var-demo` / `unloop-demo` / `allot-demo` / `leave-demo` / `do-loop-demo` / `loop-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `create-demo` / `string-demo` / `colon-demo` still OK.
8. `[throw-demo] OK`.

CATCH + nonzero THROW markers under an open catch frame are required. Optional `ABORT"` preferred when cheap (reuse string-lit parse). Demo must not exercise uncaught THROW.

## 6. Thin amend — companions

### `docs/CONTROL.md`

- Companions: add `THROW-CATCH.md` (wave14 **4**).
- Purpose / §3: point exception-frame stubs (`CATCH` / `THROW`) at this tip (mark-only; no real RS unwind). Sibling of IF/THEN/ELSE balance stubs.
- Non-goals: `CATCH` / `THROW` / optional `ABORT"` → `docs/THROW-CATCH.md` (wave14 **4**); keep real exception stack / branch XT out.
- Cite: `docs/THROW-CATCH.md`.

### `docs/KERNEL.md`

- Companions: add `THROW-CATCH.md` (wave14 **4**).
- Words table: add `CATCH` / `THROW` stubs + `throw-demo` (cite tip; soft `abort` / `(abort")` remain; CATCH/THROW are mark-only beside them).
- Non-goals: point exception stubs to `docs/THROW-CATCH.md`. Full arena / FLOAT / real RS unwind still later.
- Acceptance: Lab smokes `throw-demo`.
- Cite: `docs/THROW-CATCH.md`.

### `docs/STRING-LIT.md`

- Companions: add `THROW-CATCH.md` (wave14 **4**).
- Purpose / §3: optional `ABORT"` reuses parse-until-`"` path (markers under `[throw]`; must not entry-create literal text) → this tip.
- Non-goals: `ABORT"` stub → `docs/THROW-CATCH.md` (wave14 **4**); keep full counted-string heap / BLOCK / escape rewrite out.
- Cite: `docs/THROW-CATCH.md`.

## 7. Non-goals

- Real exception stack / RS unwind / frame restore / stack picture restore beyond mark-stub
- Full `ABORT"` counted-string heap (parse + marker only if optional wired)
- `IMMEDIATE` / `POSTPONE` / linked XT compiler
- Real branch XT / runtime counted re-exec / LEAVE jump
- Full Dusk arena/pool / free / fragmentation (tip1 pointer stub only)
- Docs cites pass (wave14 **5**)
- Real crypto / network fleet
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/THROW-CATCH.md` present (Research byte-copy OK); `CONTROL.md` + `KERNEL.md` + `STRING-LIT.md` thin amends present.
2. `throw-demo` → OK (markers §4); prior `2var-demo` + `unloop-demo` + `allot-demo` + `leave-demo` + `do-loop-demo` + `loop-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `create-demo` + `string-demo` + `colon-demo` still OK.
3. Regression green (wave14 **1–3** + wave13 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `throw-demo CONTRACT` OK).
5. No merge. Stay out of Mango.

## 9. Cite

- `docs/CONTROL.md` (wave10 **2**), `docs/KERNEL.md` (wave7 **5**), `docs/STRING-LIT.md` (wave13 **4**), `docs/INTERPRET.md` (wave8 **1**)
- `docs/ALLOT-HERE.md` (wave14 **1**), `docs/UNLOOP-J.md` (wave14 **2**), `docs/2VARIABLE.md` (wave14 **3**)
- `forth/tritium/kernel.fs` (soft `abort` / `(abort")`)
- ANS Forth `CATCH` / `THROW` / `ABORT"` (stub only); Dusk / soft-abort surface (mark only)
- Base tip: `238258b` / `238258bafa82d818beebd572a7cadc3739204531`
- Wave14 proposal: `/workspace/tritium-research-docs/WAVE14-PROPOSAL.md`
