# EXECUTE — `EXECUTE` xt-id invoke mark + `exec-demo`

**Status:** Shipper-ready stub spec (wave20 item **4**)
**Canonical brief:** ANS-shaped `EXECUTE` (thin xt-id invoke **mark only**); `docs/TICK.md` (wave18 **1** — stub xt-id companion); `docs/FIND.md` (wave18 **2** — find mark companion; not execute-through); `docs/DEFER-IS.md` (wave16 **1** — deferred bind companion; not deferred-body run); `docs/KERNEL.md` (wave7 **5**); optional `docs/RECURSE.md` (wave17 **4**) / `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**) / `docs/STATE-COMPILE.md` (wave18 **4**); explicit WAVE18 / WAVE19 / WAVE20 deferral closed as xt-id invoke mark only (not real XT execute / linked XT / deferred-body run / RECURSE self-XT / DOES> child XT)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `execute.fs`); Linux host REPL; **do not** redefine host `EXECUTE` that already binds on the load path
**Companions:** `docs/TICK.md` (thin amend this tip), `docs/FIND.md` (thin amend this tip), `docs/DEFER-IS.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip); optional light cite `docs/RECURSE.md` / `docs/IMMEDIATE-POSTPONE.md` / `docs/STATE-COMPILE.md`
**Base tip SHA:** `6ccb410` (wave20 tip3 PASS / #94 COUNT) / full `6ccb410bda0a26b355925018200506e4ba482800`

## 1. Purpose

WAVE18 tip **1** landed `'` / `[']` name→stub-xt-id marks (`docs/TICK.md`); WAVE18 tip **2** landed ANS-ish FIND mark (`docs/FIND.md`); WAVE16 tip **1** landed DEFER/IS/ACTION-OF bind markers (`docs/DEFER-IS.md`). WAVE18 / WAVE19 / WAVE20 explicitly deferred `EXECUTE` (xt-id invoke **mark only**; builds on wave18 TICK xt-ids carefully — **not** real XT execute / linked XT / deferred-body run). This tip lands a **stub** invoke mark only: `EXECUTE` (or Forth mirror **`execute-mark`**) prints `[exec] EXECUTE` (+ optional `xt=`) for a **known tick stub xt-id / demo fixture** — echoes the id and a mark that “invoke was requested”; does **not** run a linked XT body, does not call deferred ACTION-OF, does not RECURSE self-XT, does not DOES> child XT. Smoke via **`exec-demo`**. Missing / unknown xt → `[exec] FAIL` reason=miss optional (demo avoids — use a known tick-demo id). Prefer Forth mirror **`execute-mark`** so host `EXECUTE` stays safe. **Not** real XT execute, not linked XT / deferred-body run, not RECURSE self-XT reopen, not DOES> XT, not Win/Android VM. Builds on wave18 TICK xt-ids carefully; companions TICK / FIND / DEFER-IS without promoting any to a real XT vector / execute path. Thinnest EXECUTE surface deferred since wave18/19 GAPS — after tip1 TRUE-FALSE + tip2 WITHIN + tip3 COUNT, before tip5 DOCS-CITES.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `EXECUTE` / `execute-mark` | `( xt -- )` *or* `( -- )` with fixed demo xt-id | Xt-id invoke **mark only**; print `[exec] EXECUTE` (+ optional `xt=<id>`); echoes that invoke was requested for a known tick stub xt-id / demo fixture — does **not** run a linked XT body |
| `exec-demo` | `( -- )` | See §5 |

Host note: bind bare `EXECUTE` on the Linux REPL **only if** that name does not collide with a host Forth `EXECUTE` in the same load path. Prefer Forth mirror **`execute-mark`** as the Lab-facing surface when in doubt — **do not** redefine host `EXECUTE`. Optional `xt=` is a host int / stub xt id matching wave18 TICK convention (entry-index host int — **not** a callable XT). Prefer a **known tick-demo stub xt-id / demo fixture** so miss FAIL is avoided. Do **not** call through ACTION-OF, do not reopen RECURSE self-XT, do not DOES> child XT.

## 3. Stub semantics

- **`EXECUTE` / `execute-mark`:** take (or use a fixed demo) stub xt-id consistent with tip1 TICK / tip2 FIND convention (host int / entry index — not a callable XT). Print `[exec] EXECUTE` and optionally `xt=<id>`. Marker means **invoke was requested** for that id — **does not** run a linked XT body, does not push/call a real XT, does not call deferred ACTION-OF / bound XT, does not RECURSE self-XT, does not DOES> child XT / child body, does not COMPILE, / STATE reopen, does not FIND-rewrite / execute-through. Captured values are host ints / stub-id echo only.
- **Known tick stub xt-id / demo fixture (document):** e.g. reuse the tip1 `tick-demo` known name’s stub `xt=` (CREATE / colon / VARIABLE / DEFER entry, or greppable demo fixture such as `widget`) so the same stub id convention is echoed. Invoke `EXECUTE` / `execute-mark` against that known id → `[exec] EXECUTE` (+ optional `xt=<id>` matching tick). Prefer documenting which fixture Shipper uses so Lab hit is deterministic.
- **Missing / unknown xt:** `[exec] FAIL reason=miss` optional (demo **must avoid** — always feed a known tick-demo stub xt-id / demo fixture).
- **Optional push:** if the host stack is easy, consume a stub xt id from the stack; marker alone is enough for Lab OK — do not require a real XT call / linked body / deferred-body run.
- **FAIL:** `[exec] FAIL reason=miss` optional (demo **must avoid**). Prefer not emitting `[exec] FAIL` on the happy path. Miss is the only documented optional FAIL reason this tip.
- Storage: stub xt-id echo only. **No** real XT execute, no linked XT body run, no deferred ACTION-OF call, no RECURSE self-XT reopen, no DOES> child XT, no HERE bump, no arena, no STATE/COMPILE, reopen.
- Nest with prior count / within / true / source / env / body / char / state / word / find / tick / create / allot / synonym / exit / buffer / marker / defer / imm / fill / pick / cell / throw / 2var / colon / control / string stubs OK. `dict-reset` unaffected (no new dict entries from exec marks — fixed stub id / tick fixture, not dictionary rewrite).
- Still no real XT execute / linked XT / deferred-body run, no RECURSE self-XT reopen, no DOES> XT, no ACCEPT/REFILL, no full arena/heap, no full Win/Android Forth VM. Tip1 TRUE-FALSE, tip2 WITHIN, tip3 COUNT, and wave19 tip1–4 (CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD) stay landed — keep cites; this tip does **not** reopen them. Leave COUNT.md + WITHIN.md + TRUE-FALSE.md untouched this tip. Those stay non-goals / prior tips. Tip5 DOCS-CITES still later.

## 4. Markers

```
[exec] EXECUTE [xt=<id>]     # xt= optional; known tick stub xt-id / demo fixture echo
[exec] FAIL reason=miss      # demo avoids
[exec-demo] OK
[exec-demo] FAIL
```

Lab greps `[exec-demo] OK` plus at least one `[exec] EXECUTE` (optional `xt=` welcome; prefer greppable `xt=` matching the known tick-demo stub id when present). Demo avoids `[exec] FAIL reason=miss`. Prefer not emitting `[exec] FAIL` on the happy path. **Do not** Lab-grep real XT body side-effects / ACTION-OF call / RECURSE self-XT / DOES> child as this tip’s success.

## 5. `exec-demo`

1. Clean slate / `dict-reset` (or cold path) — optional; exec marks need no new dict entries beyond a known tick fixture.
2. Ensure a **known** tick stub xt-id / demo fixture exists (e.g. reuse tip1 `tick-demo` known name / `widget` fixture stub `xt=`) so miss FAIL is avoided. Prefer same stub xt-id convention as wave18 TICK (`xt=` = entry index host int).
3. Invoke `EXECUTE` (or **`execute-mark`**) against that known stub xt-id → `[exec] EXECUTE` (+ optional `xt=`). Prefer greppable `xt=` matching the tick fixture when `xt=` is present.
4. Assert no `[exec] FAIL reason=miss` on the happy path. Assert exec mark did **not** run a linked XT body, did **not** call deferred ACTION-OF / bound XT, did **not** RECURSE self-XT, did **not** DOES> child XT, did **not** COMPILE, / STATE reopen / FIND rewrite (marker-only is enough). Assert host `EXECUTE` was not redefined when using the Forth mirror.
5. Prior `count-demo` / `within-demo` / `true-demo` / `source-demo` / `env-demo` / `body-demo` / `char-demo` / `state-demo` / `word-demo` / `find-demo` / `tick-demo` / `recurse-demo` / `eval-demo` / `parse-demo` / `synonym-demo` / `exit-demo` / `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` / `refined-boot-demo` / `host-boot-demo` / `interpret-demo` / `kernel-demo` still OK.
6. `[exec-demo] OK`.

`[exec] EXECUTE` (with optional greppable `xt=`) is required. Miss FAIL path is not exercised by the demo. Optional `xt=` echo is not required for Lab OK when `[exec] EXECUTE` is greppable. No real XT execute. No linked XT body. No deferred-body run. No RECURSE self-XT. No DOES> child XT.

## 6. Thin amend — companions

### `docs/TICK.md`

- Companions / Status: add `EXECUTE.md` (wave20 **4** companion cite); **keep** DEFER-IS / IMMEDIATE-POSTPONE / CREATE-DOES / KERNEL / STATE-COMPILE / TO-BODY cites — do not wipe wave18 TICK content.
- Purpose / §3 / non-goals: tick stays name→stub-xt-id mark; `EXECUTE` is the sibling **xt-id invoke mark** that may echo optional `xt=` consistent with this tip’s convention — **not** a tick reopen / real XT execute / linked XT / deferred-body run / RECURSE self-XT / DOES> child XT. Do not wipe wave18 TICK content.
- Non-goals: `EXECUTE` xt-id invoke mark → `docs/EXECUTE.md` (wave20 **4**; Forth mirror `execute-mark` — mark only; not real XT execute). FIND / WORD-BL / STATE-COMPILE stay wave18 **2–4**. Real XT execute still out.
- Acceptance: Lab smokes `exec-demo` (retains `tick-demo` + `find-demo` + `state-demo` + `body-demo`).
- Cite: `docs/EXECUTE.md`.

### `docs/FIND.md`

- Companions / Status: add `EXECUTE.md` (wave20 **4** companion cite); **keep** KERNEL / WORDS-VOCAB / SYNONYM-ALIAS / INTERPRET / TICK / ENVIRONMENT-QUERY cites — do not wipe wave18 FIND content.
- Purpose / §3 / non-goals: FIND stays ANS-ish find mark; `EXECUTE` is a sibling **xt-id invoke mark** — **not** a FIND reopen / execute-through / SEARCH-WORDLIST / linked dict / real XT execute. Do not wipe wave18 FIND content. Stress: execute-through stays out — this tip’s mark does not call the found XT.
- Non-goals: `EXECUTE` → `docs/EXECUTE.md` (wave20 **4**). TICK stays wave18 **1**. ENVIRONMENT? stays wave19 **3**. Real XT execute / execute-through still out.
- Acceptance: Lab smokes `exec-demo` (retains `find-demo` + `tick-demo`).
- Cite: `docs/EXECUTE.md`.

### `docs/DEFER-IS.md`

- Companions / Status: add `EXECUTE.md` (wave20 **4** companion cite); **keep** CREATE-DOES / VARIABLE-CONST / KERNEL / SYNONYM / TICK cites — do not wipe wave16 DEFER content.
- Purpose / §3 / non-goals: DEFER/IS/ACTION-OF stay name + bind markers; `EXECUTE` is a sibling **xt-id invoke mark** — **not** a deferred-body run / ACTION-OF call-through / linked XT / executing bound XT. Do not wipe wave16 DEFER content. Stress: ACTION-OF remains query/print only — this tip does **not** call the bound stub.
- Non-goals: `EXECUTE` → `docs/EXECUTE.md` (wave20 **4**). TICK stays wave18 **1**. Linked XT / executing bound XT / deferred-body run still out.
- Acceptance: Lab smokes `exec-demo` (retains `defer-demo` + `tick-demo`).
- Cite: `docs/EXECUTE.md`.

### `docs/KERNEL.md`

- Companions: add `EXECUTE.md` (wave20 **4**); **keep** tip1 TRUE-FALSE + tip2 WITHIN + tip3 COUNT cites and wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites and wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites and wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `EXECUTE` stub + `exec-demo` (cite tip; Forth mirror `execute-mark` — xt-id invoke **mark only**; **do not** redefine host `EXECUTE`; **not** real XT execute / linked XT / deferred-body run / RECURSE self-XT / DOES> child XT; optional `xt=` — known tick stub xt-id / demo fixture welcome).
- Non-goals: `EXECUTE` xt-id invoke mark → `docs/EXECUTE.md`. TRUE/FALSE stay on `TRUE-FALSE.md`. WITHIN stays on `WITHIN.md`. COUNT stays on `COUNT.md`. TICK / FIND / DEFER stay on their docs. Real XT execute / linked XT / deferred-body run still later (mark only this tip). DOCS-CITES still later (wave20 **5**).
- Acceptance: Lab smokes `exec-demo` (and retains `count-demo` + `within-demo` + `true-demo` + `source-demo` + `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `defer-demo` + prior demos).
- Cite: `docs/EXECUTE.md`.

### Optional — `docs/RECURSE.md`

- Companions: add light `EXECUTE.md` (wave20 **4**) cite; **keep** COLON / EXIT-QUIT / IMMEDIATE-POSTPONE / KERNEL / EVALUATE / PARSE / SYNONYM cites — do not wipe wave17 RECURSE content.
- Purpose / non-goals: RECURSE stays mark-only (`recurse-mark`); `EXECUTE` is a sibling **xt-id invoke mark** — **not** a RECURSE self-XT reopen / real recursive colon body / linked XT. Do not wipe wave17 RECURSE content.
- Non-goals: `EXECUTE` → `docs/EXECUTE.md` (wave20 **4**). Real self-XT still out.
- Acceptance: Lab smokes `exec-demo` (retains `recurse-demo` + `tick-demo`).
- Cite: `docs/EXECUTE.md`.

### Optional — `docs/IMMEDIATE-POSTPONE.md`

- Companions: add light `EXECUTE.md` (wave20 **4**) cite; **keep** COLON / INTERPRET / KERNEL / RECURSE / TICK / STATE-COMPILE cites — do not wipe wave15 IMMEDIATE-POSTPONE content.
- Purpose / non-goals: IMMEDIATE/POSTPONE stay flag + name mark; `EXECUTE` is a sibling **xt-id invoke mark** — **not** an executing postponed XT / linked XT / COMPILE, reopen / real XT execute. Do not wipe wave15 IMMEDIATE-POSTPONE content.
- Non-goals: `EXECUTE` → `docs/EXECUTE.md` (wave20 **4**). STATE/COMPILE, stays wave18 **4**. Executing postponed XT still out.
- Acceptance: Lab smokes `exec-demo` (retains `imm-demo` + `tick-demo` + `state-demo`).
- Cite: `docs/EXECUTE.md`.

### Optional — `docs/STATE-COMPILE.md`

- Companions: add light `EXECUTE.md` (wave20 **4**) cite; **keep** IMMEDIATE-POSTPONE / COLON / INTERPRET / KERNEL / TICK cites — do not wipe wave18 STATE-COMPILE content.
- Purpose / non-goals: STATE/COMPILE, stay query + compile-comma mark; `EXECUTE` is a sibling **xt-id invoke mark** — **not** a linked XT / XT body append / real STATE cell / real XT execute. Do not wipe wave18 STATE-COMPILE content. Stress: COMPILE, mark does not append XT; EXECUTE mark does not run XT.
- Non-goals: `EXECUTE` → `docs/EXECUTE.md` (wave20 **4**). TICK stays wave18 **1**. Linked XT / real XT execute still out.
- Acceptance: Lab smokes `exec-demo` (retains `state-demo` + `tick-demo`).
- Cite: `docs/EXECUTE.md`.

Do **not** wipe tip1 TRUE-FALSE content or tip2 WITHIN content or tip3 COUNT content or wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites or wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites or wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites or wave16 DEFER / MARKER / BUFFER / EXIT cites or wave15 CELL / IMMEDIATE / FILL / PICK prior content. Do **not** amend ARCHITECTURE / IMPLEMENTATION-GAPS / TRUE-FALSE / WITHIN / COUNT / CONTROL / HOST-PARITY this tip (proposal amends are TICK + FIND + DEFER-IS + KERNEL + optional RECURSE / IMMEDIATE-POSTPONE / STATE-COMPILE only). **Leave `COUNT.md`, `WITHIN.md`, and `TRUE-FALSE.md` untouched** (tip1/tip2/tip3 byte-copy stays; COUNT tip3 primary checksum `061fb39b4f5a53943488dae671fa3250` / 23415; WITHIN tip2 primary `9067f6978ee78e9e40f586b13666261e` / 17847; TRUE-FALSE tip1 primary `51546c35461275e1b8971fe3fcda91f9` / 18022). Tip5 cites come after 1–4 PASS.

## 7. Non-goals

- Real XT execute / calling through stub xt id / linked XT body run
- Deferred-body run / calling through ACTION-OF / executing bound XT
- `RECURSE` self-XT reopen (wave17 **4** — already mark-only; keep cites; optional thin companion cite only)
- `DOES>` child XT / threaded child runtime body (CREATE-DOES stays mark; keep cites)
- `'` / `[']` tick reopen (wave18 **1** — already stubbed; keep cites; thin companion amend only — xt= convention shared)
- `FIND` reopen / execute-through (wave18 **2** — already stubbed; keep cites; thin companion amend only)
- `DEFER` / `IS` / `ACTION-OF` reopen (wave16 **1** — already stubbed; keep cites; thin companion amend only)
- `STATE` / `COMPILE,` reopen (wave18 **4** — already stubbed; optional thin companion cite only)
- `IMMEDIATE` / `POSTPONE` reopen (wave15 **4** — already stubbed; optional thin companion cite only)
- Docs cites pass (wave20 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `TRUE` / `FALSE` reopen (wave20 **1** — already stubbed; keep cites; leave TRUE-FALSE.md untouched this tip)
- `WITHIN` reopen (wave20 **2** — already stubbed; keep cites; leave WITHIN.md untouched this tip)
- ANS `COUNT` reopen (wave20 **3** — already stubbed; keep cites; leave COUNT.md untouched this tip)
- `CHAR` / `CHARS` / `[CHAR]` reopen (wave19 **1** — already stubbed; keep cites)
- `>BODY` reopen (wave19 **2** — already stubbed; keep cites)
- `ENVIRONMENT?` reopen (wave19 **3** — already stubbed; keep cites)
- `SOURCE` / `PAD` reopen (wave19 **4** — already stubbed; keep cites)
- `ACCEPT` / `REFILL` / full input-buffer VM / live TIB rewrite
- Real branch XT / LEAVE jump
- Full Dusk arena / pool / free / fragmentation model
- Linked XT / executing postponed XT / real STATE cell
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live — **skip 2DUP-FAMILY**)
- `ABORT"` polish (already optional-wired inside `throw-demo` — **skip**)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/EXECUTE.md` present (Research byte-copy OK); `TICK.md` + `FIND.md` + `DEFER-IS.md` + `KERNEL.md` thin amends present (+ optional `RECURSE.md` / `IMMEDIATE-POSTPONE.md` / `STATE-COMPILE.md`); tip1 TRUE-FALSE + tip2 WITHIN + tip3 COUNT cites, wave19 tip1–4 CHAR-CHARS / TO-BODY / ENVIRONMENT-QUERY / SOURCE-PAD cites, wave18 tip1–4 cites, wave17 tip1–4 cites, wave16 DEFER/MARKER/BUFFER/EXIT cites, and wave15 CELL/IMMEDIATE/COLON/KERNEL prior text retained; host `EXECUTE` untouched via mirrors; `COUNT.md` + `WITHIN.md` + `TRUE-FALSE.md` byte-copy unchanged (COUNT tip3 primary `061fb39b4f5a53943488dae671fa3250` / 23415; WITHIN tip2 primary `9067f6978ee78e9e40f586b13666261e` / 17847; TRUE-FALSE tip1 primary `51546c35461275e1b8971fe3fcda91f9` / 18022); ARCHITECTURE / IMPLEMENTATION-GAPS byte-copy unchanged.
2. `exec-demo` → OK (markers §4; `[exec] EXECUTE` greppable; optional `xt=` welcome; prefer `xt=` matching known tick stub; no miss FAIL on happy path; no real XT execute / linked XT / deferred-body run / RECURSE self-XT / DOES> child XT). Prior `count-demo` + `within-demo` + `true-demo` + `source-demo` + `env-demo` + `body-demo` + `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `recurse-demo` + `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` + `refined-boot-demo` + `host-boot-demo` + `interpret-demo` + `kernel-demo` still OK.
3. Regression green (wave20 tip1–3 + wave19 tip1–5 + wave18 tip1–5 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `exec-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**), `docs/TICK.md` (wave18 **1** — stub xt-id companion; EXECUTE echoes known tick stub xt-id — not XT execute)
- `docs/FIND.md` (wave18 **2** — find mark companion; EXECUTE is invoke mark — not execute-through / FIND reopen)
- `docs/DEFER-IS.md` (wave16 **1** — deferred bind companion; EXECUTE is invoke mark — not deferred-body run / ACTION-OF call-through)
- `docs/RECURSE.md` (wave17 **4**, optional — mark-only companion; EXECUTE does not reopen self-XT)
- `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**, optional — flag + name mark companion; EXECUTE does not execute postponed XT)
- `docs/STATE-COMPILE.md` (wave18 **4**, optional — STATE/COMPILE, companion; EXECUTE does not linked-XT / COMPILE, reopen)
- `docs/TRUE-FALSE.md` (wave20 **1** — prior tip; keep cites; leave untouched this tip)
- `docs/WITHIN.md` (wave20 **2** — prior tip; keep cites; leave untouched this tip)
- `docs/COUNT.md` (wave20 **3** — prior tip; keep cites; leave untouched this tip)
- `docs/CHAR-CHARS.md` (wave19 **1** — prior tip; keep cites)
- `docs/TO-BODY.md` (wave19 **2** — prior tip; keep cites)
- `docs/ENVIRONMENT-QUERY.md` (wave19 **3** — prior tip; keep cites)
- `docs/SOURCE-PAD.md` (wave19 **4** — prior tip; keep cites)
- `docs/CREATE-DOES.md` (wave13 **3** — CREATE/DOES> stay mark; EXECUTE does not DOES> child XT)
- `forth/tritium/kernel.fs` (execute-mark only — do not redefine host EXECUTE)
- ANS Forth `EXECUTE` (xt-id invoke **mark only** — not real XT execute / linked XT / deferred-body run / RECURSE self-XT / DOES> child XT)
- Explicit deferral: WAVE18-PROPOSAL + WAVE19-PROPOSAL + WAVE20-PROPOSAL (`EXECUTE` — xt-id invoke mark only; builds on wave18 TICK xt-ids carefully — not real XT execute)
- Base tip: `6ccb410` / `6ccb410bda0a26b355925018200506e4ba482800` (#94 wave20 tip3 COUNT PASS)
- Wave20 proposal: `/workspace/tritium-research-docs/WAVE20-PROPOSAL.md`
