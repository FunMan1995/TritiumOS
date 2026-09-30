# STATE-COMPILE — Stub `STATE` / `COMPILE,` markers + `state-demo`

**Status:** Shipper-ready stub spec (wave18 item **4**)
**Canonical brief:** ANS-shaped `STATE` / `COMPILE,` (thin compile-state query + compile-comma mark only); `docs/IMMEDIATE-POSTPONE.md` (wave15 **4** — flag + name mark sibling); `docs/COLON.md` (wave9 **4** — colon-def flag); `docs/INTERPRET.md` (wave8 **1**); `docs/KERNEL.md` (wave7 **5**); optional `docs/TICK.md` (wave18 **1** — stub `xt=` ids stay consistent); explicit WAVE17 / WAVE18 deferral closed as STATE query + COMPILE, mark only (not linked XT compiler / real compile-vs-interpret machine / real STATE cell in dictionary image)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `state.fs` / `compile.fs`); Linux host REPL; **do not** redefine host assistant-state / host `STATE` (if any) / host EXIT
**Companions:** `docs/IMMEDIATE-POSTPONE.md` (thin amend this tip), `docs/COLON.md` (thin amend this tip), `docs/INTERPRET.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip); optional `docs/TICK.md` (thin amend this tip — companion cite)
**Base tip SHA:** `88f3b91` (wave18 tip3 WORD-BL PASS / #84) / full `88f3b911266cdb356b95861cd426b148865c6dae`

## 1. Purpose

WAVE15 IMMEDIATE/POSTPONE landed compile-only **flag + name mark** stubs beside the colon-def flag; WAVE17/WAVE18 explicitly deferred a real compile-vs-interpret machine and linked XT compiler. WAVE18 tip **4** closes that deferral as a **thin query + compile-comma mark** only: `STATE` prints `[state] STATE` (+ optional `flag=<0|1>` / `n=` echoing colon-def / compile stub flag — interpret=0 / compile=1 picture only); `COMPILE,` prints `[state] COMPILE,` (+ optional `xt=<id>` / `name=`) and does **not** append an XT to a body list or execute it (optional `[state] compile-only` when colon-def flag is set — Lab-optional). Smoke via **`state-demo`**. Outside any compile picture → still OK as mark; missing xt/name → `[state] FAIL reason=miss` optional (demo avoids). Forth mirrors **`state-flag` / `compile-comma`** so assistant-state / host names stay safe. **Not** a linked XT compiler, not FIND-then-compile, not POSTPONE reopen, not real STATE cell in a dictionary image. Builds on IMMEDIATE/POSTPONE + colon-def flag + tip1 stub `xt=` ids without promoting any to a real compile machine / XT body append.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `STATE` / `state-flag` | `( -- n )` *or* `( -- )` stub | Compile-state query mark; print `[state] STATE` (+ optional `flag=<0\|1>` / `n=<0\|1>`); interpret=0 / compile=1 picture only — does **not** allocate a real STATE cell in a dictionary image; does **not** redefine host assistant-state |
| `COMPILE,` / `compile-comma` | `( xt -- )` *or* `( "name" -- )` *or* `( -- )` stub | Compile-comma mark; print `[state] COMPILE,` (+ optional `xt=<id>` / `name=<name>`); does **not** append XT to a body list or execute it; optional `[state] compile-only` when colon-def flag set (Lab-optional) |
| `state-demo` | `( -- )` | See §5 |

Host note: bind bare `STATE` / `COMPILE,` on the Linux REPL **only if** they do not collide with host assistant-state / any host `STATE` / host Forth names in the same load path. Prefer Forth mirrors **`state-flag` / `compile-comma`** as the Lab-facing surface — **do not** redefine host assistant-state helpers (`assistant-state!` / `assistant-state@` / `assistant-state-demo` stay on `ASSISTANT-STATE.md`). Optional stub `xt=` is a host int / entry index (same convention as tip1 TICK / tip2 FIND — stub xt id = entry index) — not a callable XT. Prefer a known dict/demo name (CREATE / colon / VARIABLE / DEFER entry, or tip1 `widget` fixture) so miss FAIL is avoided. Returned `( n )` / `( xt )` stack effects are optional — marker alone is enough for Lab.

## 3. Stub semantics

- **Compile stub flag picture:** one host int / bool echoing the existing **colon-def flag** (or a demo fixture flag) is enough. `flag=0` / `n=0` → interpret picture; `flag=1` / `n=1` → compile picture. Lab greps the marker, not a real STATE cell dump. **Not** a dictionary-image STATE variable, not HERE/ALLOT cell, not VARIABLE named `STATE`.
- **`STATE` / `state-flag`:** print `[state] STATE` and optionally `flag=<0|1>` and/or `n=<0|1>` echoing the current colon-def / compile stub flag. Outside any compile picture (interpret=0) → still OK as mark (demo may show `flag=0` then enter colon-def and show `flag=1`, or show one picture only). **Does not** allocate / poke a real STATE cell, change interpret loop behavior, or redefine host assistant-state.
- **`COMPILE,` / `compile-comma`:** accept a stub xt id and/or known name (from tip1 tick convention, demo fixture, or parse-name form). Print `[state] COMPILE,` (+ optional `xt=<id>` / `name=<name>`). **Do not** append an XT to a colon body list, FIND-then-compile, execute the named word, or reopen POSTPONE. If colon-def flag is currently set, optionally also print `[state] compile-only` (Lab-optional — nice-to-have when cheap; mirrors IMMEDIATE optional `[imm] compile-only`).
- **Outside compile picture:** `COMPILE,` is still OK as a mark (demo may call it in interpret picture and/or while colon-def flag set). Marker greppability is enough either way.
- **Missing xt/name:** `[state] FAIL reason=miss` optional (demo **must avoid** — always COMPILE, a known name / stub xt id).
- Storage: flag echo + optional name string / stub xt id. **No** linked XT list, no FIND-then-compile, no XT body append, no postponed-XT queue, no real STATE cell in dictionary image, no EXIT redefinition, no POSTPONE reopen.
- Nest with prior word / find / tick / recurse / eval / parse / synonym / exit / buffer / marker / defer / imm / fill / pick / cell / allot / throw / 2var / create / colon / control / string / comment / words stubs OK. `dict-reset` unaffected (state stubs do not create dict words). Soft KERNEL `abort` / `(abort")`, wave14 CATCH/THROW, and wave16 EXIT/QUIT mark stubs stay as-is beside this marker. Host assistant-state + IMMEDIATE/POSTPONE markers stay as-is.
- Still no linked XT compiler / FIND-then-compile / executing postponed XT / real compile-vs-interpret machine beyond colon-def flag + immediate-bit + this query mark / real STATE cell / POSTPONE reopen / real DOES> XT / real branch XT / full arena/heap / full Win/Android Forth VM. Those stay non-goals / later tips (docs cites = wave18 **5**).

## 4. Markers

```
[state] STATE [flag=<0|1>] [n=<0|1>]              # flag=/n= optional; interpret=0 / compile=1 picture
[state] COMPILE, [xt=<id>] [name=<name>]          # xt=/name= optional
[state] compile-only                              # optional when colon-def flag set
[state] FAIL reason=miss                          # demo avoids
[state-demo] OK
[state-demo] FAIL
```

Lab greps `[state-demo] OK` plus at least one `[state] STATE` (optional `flag=` / `n=` welcome) and one `[state] COMPILE,` (optional `xt=` / `name=` welcome). Optional `[state] compile-only` is not required for Lab OK. Demo avoids `[state] FAIL reason=miss`.

## 5. `state-demo`

1. Clean slate / `dict-reset` (or cold path).
2. Invoke `STATE` (or `state-flag`) → `[state] STATE` (+ optional `flag=0` / `n=0` interpret picture). Optional: enter a colon-def (`:` …) briefly so a second `STATE` can echo `flag=1` / `n=1` — not required for Lab OK; one STATE line is enough.
3. Ensure a known name / stub xt id exists (e.g. create via `entry-create` / VARIABLE / CREATE / colon stub, or use tip1 `widget` / tip2 FIND fixture; prefer same stub xt-id convention as tip1).
4. Invoke `COMPILE,` (or `compile-comma`) with that known name / stub xt → `[state] COMPILE,` (+ optional `xt=<id>` / `name=<name>`). Optional: while colon-def flag is set, emit `[state] compile-only` once.
5. Assert no `[state] FAIL reason=miss` on the happy path. Assert COMPILE, did **not** append an XT to a body list or execute it (marker-only is enough). Assert host assistant-state path still untouched. Assert IMMEDIATE/POSTPONE + colon remain green.
6. Prior `word-demo` / `find-demo` / `tick-demo` / `recurse-demo` / `eval-demo` / `parse-demo` / `synonym-demo` / `exit-demo` / `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` / `kernel-demo` / `refined-boot-demo` / `host-boot-demo` / `interpret-demo` / `assistant-state-demo` still OK.
7. `[state-demo] OK`.

`STATE` + `COMPILE,` markers are required. Miss FAIL path is not exercised by the demo. Optional `flag=` / `n=` / `xt=` / `name=` echo and optional `compile-only` are not required for Lab OK. No linked XT append. No real STATE cell. No POSTPONE reopen. No host assistant-state redefine.

## 6. Thin amend — companions

### `docs/IMMEDIATE-POSTPONE.md`

- Companions: add `STATE-COMPILE.md` (wave18 **4**); **keep** COLON / INTERPRET / KERNEL / RECURSE / EXIT / TICK cites.
- Purpose / §3 / non-goals: IMMEDIATE/POSTPONE stay flag + name mark; STATE/COMPILE, is a sibling **query + compile-comma mark** — **not** a linked XT / FIND-then-compile / executing postponed XT / POSTPONE reopen / real STATE cell. Do not wipe wave15 IMMEDIATE-POSTPONE content. Closes “compile state machine beyond flag” as thin query + mark.
- Non-goals: `STATE` / `COMPILE,` stubs → `docs/STATE-COMPILE.md` (wave18 **4**). `'` / `[']` tick stays on `TICK.md`. `RECURSE` mark-only stays on `RECURSE.md`. Host `EXIT` / `QUIT` stay on wave16 `EXIT-QUIT.md`. Linked XT / executing postponed XT still later.
- Acceptance: Lab smokes `state-demo` (retains `imm-demo` + `tick-demo` + `recurse-demo` + `colon-demo`).
- Cite: `docs/STATE-COMPILE.md`.

### `docs/COLON.md`

- Companions: add `STATE-COMPILE.md` (wave18 **4**); **keep** EXIT-QUIT / IMMEDIATE-POSTPONE / RECURSE / TICK / INTERPRET / KERNEL cites.
- Purpose / §7 Non-goals: colon-def / body-marker stubs stay; STATE may optionally echo the colon-def / compile stub flag (`flag=0|1`) — **not** a real compile-vs-interpret machine / linked XT / STATE cell. COMPILE, mark does **not** append XT to colon body. **Do not break** `:` / `;` / `colon-demo`. Do not wipe wave9/15/16/17/18 colon text.
- Non-goals: `STATE` / `COMPILE,` stubs → `docs/STATE-COMPILE.md` (wave18 **4**). IMMEDIATE / EXIT / RECURSE / TICK stay on prior docs.
- Acceptance: Lab smokes `state-demo` (retains `colon-demo` + `imm-demo` + `recurse-demo` + `tick-demo`).
- Cite: `docs/STATE-COMPILE.md`.

### `docs/INTERPRET.md`

- Status / companions: cite wave18 **4**; add `STATE-COMPILE.md`; **keep** WORD-BL / FIND / TICK / PARSE-NAME / EVALUATE-INCLUDE / EXIT-QUIT / IMMEDIATE / STRING-LIT / COMMENT-PARSE / KERNEL / COLON cites.
- Purpose / §2 / §3 / non-goals: interpret loop stays on host find + whitespace split; STATE/COMPILE, stub markers are a sibling demo/fixture query + compile-comma surface via `state-flag` / `compile-comma` — **does not** install a real compile-vs-interpret machine / linked XT / STATE cell / POSTPONE reopen. Do not wipe wave8–18 INTERPRET content. Host WORDS + `[kernel] words (` stay. IMMEDIATE/POSTPONE stay flag stubs.
- Words table: add `STATE` / `state-flag`, `COMPILE,` / `compile-comma` + `state-demo` (cite tip; Forth mirrors — stub markers only).
- Markers: one-line pointer to `[state]` markers.
- Non-goals: STATE/COMPILE, stub markers in scope via this tip; keep linked XT / executing postponed XT / real STATE cell / real compile machine out.
- Acceptance: Lab smokes `state-demo` (retains `interpret-demo` + `word-demo` + `find-demo` + `tick-demo` + `imm-demo` + prior).
- Cite: `docs/STATE-COMPILE.md`.

### `docs/KERNEL.md`

- Companions: add `STATE-COMPILE.md` (wave18 **4**); **keep** tip1 TICK + tip2 FIND + tip3 WORD-BL cites and wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `STATE` / `state-flag`, `COMPILE,` / `compile-comma` stubs + `state-demo` (cite tip; Forth mirrors `state-flag` / `compile-comma` — stub query + compile-comma markers only; **do not** redefine host assistant-state / host EXIT; **not** linked XT / FIND-then-compile / real STATE cell / POSTPONE reopen).
- Non-goals: STATE/COMPILE, stub markers → `docs/STATE-COMPILE.md`. WORD-BL stays on `WORD-BL.md`. FIND stays on `FIND.md`. TICK stays on `TICK.md`. RECURSE / EVALUATE / PARSE / SYNONYM stay on wave17 docs. DEFER/IS/ACTION-OF / MARKER / BUFFER: / EXIT/QUIT stay on wave16 docs. Docs cites still later (wave18 **5**). Linked XT / executing postponed XT / real compile machine still later.
- Acceptance: Lab smokes `state-demo` (and retains `word-demo` + `find-demo` + `tick-demo` + `recurse-demo` + `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `colon-demo` + `kernel-demo` + `words-demo` + prior demos).
- Cite: `docs/STATE-COMPILE.md`.

### `docs/TICK.md` (optional)

- Companions: add light `STATE-COMPILE.md` (wave18 **4**) cite; **keep** KERNEL / DEFER-IS / IMMEDIATE-POSTPONE / CREATE-DOES / COLON cites.
- Purpose / §3 / non-goals: tick stays name→stub-xt-id mark; COMPILE, may optionally echo the same stub `xt=` id — **not** a linked XT compile / FIND-then-compile / XT body append / STATE cell. Do not wipe wave18 tip1 tick content. STATE query is sibling mark only.
- Non-goals: `STATE` / `COMPILE,` stubs → `docs/STATE-COMPILE.md` (wave18 **4**). FIND / WORD-BL stay on tip2/3 docs. Linked XT / XT execute still later.
- Acceptance: Lab smokes `state-demo` (retains `tick-demo`).
- Cite: `docs/STATE-COMPILE.md`.

Do **not** wipe tip1 TICK cites or tip2 FIND cites or tip3 WORD-BL cites or wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites or wave16 DEFER / MARKER / BUFFER / EXIT cites or wave15 IMMEDIATE / COLON / KERNEL prior content. Do **not** amend FIND.md / WORD-BL.md / EVALUATE-INCLUDE / RECURSE / EXIT-QUIT / DEFER-IS / CREATE-DOES / PARSE-NAME / COMMENT-PARSE / WORDS-VOCAB / ASSISTANT-STATE / DOCS-CITES docs this tip (proposal amends are IMMEDIATE-POSTPONE + COLON + INTERPRET + KERNEL + optional TICK only).

## 7. Non-goals

- Linked XT compiler / FIND-then-compile / XT body append
- Real compile-vs-interpret machine beyond colon-def flag + immediate-bit + this query mark
- Real STATE cell in a dictionary image / VARIABLE named STATE / HERE bump for STATE
- Executing a postponed XT / postponed-XT queue / runtime compile semantics
- Reopening POSTPONE / IMMEDIATE (wave15 **4** — already stubbed; keep cites)
- Redefining host assistant-state / `assistant-state!` / `assistant-state@` / `assistant-state-demo`
- Redefining host `EXIT` / `QUIT` — thin control markers → `docs/EXIT-QUIT.md` (wave16 **4**; mirrors only)
- Docs cites pass (wave18 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `WORD` / `BL` reopen (wave18 **3** — already stubbed; keep cites)
- `FIND` reopen (wave18 **2** — already stubbed; keep cites)
- `'` / `[']` tick reopen (wave18 **1** — already stubbed; keep cites; optional `xt=` companion)
- `RECURSE` reopen (wave17 **4** — already mark-only; keep cites)
- `EVALUATE` / `INCLUDE` reopen (wave17 **3** — already stubbed; keep cites)
- `PARSE` / `PARSE-NAME` reopen (wave17 **2** — already stubbed; keep cites)
- `SYNONYM` / `ALIAS` reopen (wave17 **1** — already stubbed; keep cites)
- `MARKER` / `BUFFER:` / `EXIT` / `QUIT` (wave16 **2–4** — already stubbed; keep cites)
- `DEFER` / `IS` / `ACTION-OF` reopen (wave16 **1** — already stubbed; keep cites)
- IMMEDIATE / POSTPONE / FILL / PICK / CELL (wave15 — already stubbed)
- Real DOES> XT chaining / threaded child runtime body
- Real branch XT / LEAVE jump
- Full Dusk arena / pool / free / fragmentation model
- `SOURCE` / `PAD` / `ACCEPT` / `REFILL` full input-buffer surface
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live — **skip 2DUP-FAMILY**)
- `ABORT"` polish (already optional-wired inside `throw-demo` — **skip**)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/STATE-COMPILE.md` present (Research byte-copy OK); `IMMEDIATE-POSTPONE.md` + `COLON.md` + `INTERPRET.md` + `KERNEL.md` thin amends present (+ optional `TICK.md`); tip1 TICK cites, tip2 FIND cites, tip3 WORD-BL cites, wave17 tip1–4 cites, wave16 DEFER/MARKER/BUFFER/EXIT cites, and wave15 IMMEDIATE/COLON/KERNEL prior text retained; host assistant-state / host EXIT untouched via mirrors; no real STATE cell / linked XT append.
2. `state-demo` → OK (markers §4; `[state] STATE` greppable; `[state] COMPILE,` greppable; optional `flag=` / `n=` / `xt=` / `name=` / `compile-only` welcome; no miss FAIL on happy path; no XT body append). Prior `word-demo` + `find-demo` + `tick-demo` + `recurse-demo` + `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` + `kernel-demo` + `refined-boot-demo` + `host-boot-demo` + `interpret-demo` + `assistant-state-demo` still OK.
3. Regression green (wave18 tip1–3 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `state-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**), `docs/COLON.md` (wave9 **4**), `docs/INTERPRET.md` (wave8 **1**), `docs/KERNEL.md` (wave7 **5**)
- `docs/TICK.md` (wave18 **1**, optional companion — stub `xt=` ids)
- `docs/WORD-BL.md` (wave18 **3**), `docs/FIND.md` (wave18 **2**)
- `docs/RECURSE.md` (wave17 **4**), `docs/EVALUATE-INCLUDE.md` (wave17 **3**), `docs/PARSE-NAME.md` (wave17 **2**), `docs/SYNONYM-ALIAS.md` (wave17 **1**)
- `docs/MARKER.md` (wave16 **2**), `docs/BUFFER-COLON.md` (wave16 **3**), `docs/EXIT-QUIT.md` (wave16 **4**)
- `docs/DEFER-IS.md` (wave16 **1**), `docs/ASSISTANT-STATE.md` (wave9 **3** — host assistant-state untouched)
- `docs/FILL-MOVE.md` (wave15 **3**), `docs/PICK-ROLL.md` (wave15 **2**), `docs/CELL-CELLS.md` (wave15 **1**)
- `forth/tritium/kernel.fs` (assistant-state / host EXIT stay; state-flag / compile-comma only)
- ANS Forth `STATE` / `COMPILE,` (query + compile-comma mark only — no linked XT / no real STATE cell / no XT body append)
- Explicit deferral: WAVE17-PROPOSAL + WAVE18-PROPOSAL (`STATE` / `COMPILE,` — not linked XT / real state machine); IMMEDIATE-POSTPONE non-goal called this out as later
- Base tip: `88f3b91` / `88f3b911266cdb356b95861cd426b148865c6dae` (#84 wave18 tip3 WORD-BL PASS)
- Wave18 proposal: `/workspace/tritium-research-docs/WAVE18-PROPOSAL.md`
