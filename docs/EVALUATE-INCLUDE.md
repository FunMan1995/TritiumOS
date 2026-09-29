# EVALUATE-INCLUDE — `EVALUATE` / `INCLUDE` mark-only echo + `eval-demo`

**Status:** Shipper-ready stub spec (wave17 item **3**)
**Canonical brief:** ANS-shaped `EVALUATE` / `INCLUDE` (thin nested-interpret / include **echo markers** only); `docs/REFINED-BOOT.md` (wave10 **4**); `docs/INTERPRET.md` (wave8 **1**); `docs/HOST-BOOT.md` (wave10 **1**); `docs/KERNEL.md` (wave7 **5**); `docs/PARSE-NAME.md` (wave17 **2**); `docs/SYNONYM-ALIAS.md` (wave17 **1**); explicit WAVE16/WAVE17 deferral closed as stub echo only
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `evaluate.fs` / `include.fs`); Linux host REPL; **do not** redefine `tritium.poly/core/boot.fs` host `include` or `load_refined_modules`
**Companions:** `docs/REFINED-BOOT.md` (thin amend this tip), `docs/INTERPRET.md` (thin amend this tip), `docs/HOST-BOOT.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip)
**Base tip SHA:** `c0de0ab` (wave17 tip2 CLOSED / #78 PARSE-NAME) / full `c0de0abe358d6ef013a45f702e5b201549782143`

## 1. Purpose

WAVE16/WAVE17 explicitly deferred real `EVALUATE` / `INCLUDE` (nested-interpret VM / file VM). Interpret loop, PARSE/PARSE-NAME token-parse markers, host poly/core `include`, and refined-boot cold-load markers already exist; a Forth-surface **eval/include echo** does not. This tip lands **stub** mark-only echo markers: `EVALUATE` prints `[eval] EVALUATE` (+ optional `u=` / `src=` echo of a short demo string) and does **not** re-enter a real nested interpret VM; `INCLUDE` prints `[eval] INCLUDE name=` (or `[eval] INCLUDE path=`) as an echo marker only. Smoke via **`eval-demo`**. Forth mirrors **`evaluate-mark` / `include-mark`** so poly/core `include` and host file load stay untouched. **Refined-boot include markers stay** (`[VM] include`, `[refined-boot] include=`) — tip3 does **not** replace `load_refined_modules` / `refined-boot-demo`. Closes WAVE16 “EVALUATE / INCLUDE nested VM” deferral as **stub echo only**.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `EVALUATE` / `evaluate-mark` | `( c-addr u -- )` *or* `( -- )` stub | Echo marker only; print `[eval] EVALUATE` (+ optional `u=<n>` / `src=<short>`); does **not** re-enter nested interpret |
| `INCLUDE` / `include-mark` | `( "name" -- )` *or* `( -- )` then parse name/path | Echo marker only; print `[eval] INCLUDE name=<name>` **or** `[eval] INCLUDE path=<path>` (document choice; either OK for Lab) |
| `eval-demo` | `( -- )` | See §5 |

Host note: bind `EVALUATE` / `INCLUDE` on the Linux REPL only when safe; Forth mirrors **`evaluate-mark` / `include-mark`** are the required Lab surfaces so host Forth / poly/core `include` (boot.fs file load) and refined-boot `load_refined_modules` stay untouched. Prefer fixture/demo-string source for EVALUATE echo — **not** a nested `interpret` call.

## 3. Stub semantics

- **EVALUATE:** accept a short demo string (or fixture `( c-addr u )` / in-demo buffer). Print `[eval] EVALUATE` and optionally `u=<n>` (char count) and/or `src=<short>` (echo of the demo string, truncated OK). **Does not** re-enter a real nested interpret VM, does not walk the live token stream as a nested `interpret`, does not compile/execute the string body beyond the echo mark. Demo/fixture path is enough.
- **INCLUDE:** given a name or path (fixture / demo arg / parsed word), print `[eval] INCLUDE name=<name>` **or** `[eval] INCLUDE path=<path>` (Shipper documents which spelling; Lab greps `INCLUDE` plus `name=` or `path=`). **Does not** open/read a real file, does not call host boot `include`, does not call `load_refined_modules`, does not replace `[VM] include` / `[refined-boot] include=` markers.
- **Missing path / unbound name:** `[eval] FAIL reason=miss` optional (demo **must avoid** — always feed a known demo string for EVALUATE and a known fixture name/path for INCLUDE).
- Storage: short demo string + optional name/path string for marker echo only. **No** nested interpret VM, no file VM, no redefining host boot include / refined-boot load path, no RECURSE self-XT.
- Nest with prior parse / synonym / exit / buffer / marker / defer / imm / fill / pick / cell / allot / throw / 2var / create / colon / control / string / comment / words stubs OK. `dict-reset` unaffected (eval/include stubs do not create dict words from evaluated text).
- Still no real EVALUATE/INCLUDE nested/file VM, no host boot `include` redefine, no refined-boot load-path replace, no RECURSE self-XT, no full parser/tokenizer VM reopen, no real DOES> XT, no real branch XT, no full Win/Android Forth VM. Those stay non-goals / later tips.

## 4. Markers

```
[eval] EVALUATE                     # + optional u=<n> / src=<short>
[eval] INCLUDE name=<name>          # or path=<path>
[eval] FAIL reason=miss             # demo avoids
[eval-demo] OK
[eval-demo] FAIL
```

**Refined-boot / host markers stay (untouched by this tip):**

```
[VM]   include <name> (persisted refined)
[refined-boot] include=<name>
```

Lab greps `[eval-demo] OK` plus at least one `[eval] EVALUATE` (optional `u=` / `src=` welcome) and one `[eval] INCLUDE` with `name=` or `path=`. Demo avoids `[eval] FAIL reason=miss`. Prior `refined-boot-demo` / `host-boot-demo` / `parse-demo` / `synonym-demo` still OK — tip3 does not replace or suppress refined-boot / host-boot include markers.

## 5. `eval-demo`

1. Clean slate / `dict-reset` (or cold path).
2. Prepare a short non-empty demo string/fixture (e.g. `hi` or `1 2 +`).
3. `EVALUATE` (or `evaluate-mark`) against the fixture → `[eval] EVALUATE` (+ optional `u=2` / `src=hi`). Assert **no** nested interpret re-entry is required (marker alone is enough; string body need not execute).
4. `INCLUDE` (or `include-mark`) with a known fixture name/path (e.g. `eval-demo-fixture` or `demo.fs`) → `[eval] INCLUDE name=eval-demo-fixture` (or `path=`). Assert host boot `include` / `load_refined_modules` are **not** invoked by this mirror.
5. Assert no `[eval] FAIL reason=miss` on the happy path. Assert refined-boot markers (`[VM] include`, `[refined-boot] include=`) remain available to `refined-boot-demo` unchanged.
6. Prior `parse-demo` / `synonym-demo` / `exit-demo` / `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` / `refined-boot-demo` / `host-boot-demo` / `interpret-demo` still OK.
7. `[eval-demo] OK`.

EVALUATE + INCLUDE markers are required. Miss FAIL path is not exercised by the demo. Optional `u=` / `src=` and `name=` vs `path=` spelling choice are not required beyond greppable INCLUDE + name=|path=. No nested interpret VM. No host/refined-boot include redefine.

## 6. Thin amend — companions

### `docs/REFINED-BOOT.md`

- Companions: add `EVALUATE-INCLUDE.md` (wave17 **3**); **keep** HOST-BOOT / REKIA / ASSIMILATE / APPIMAGE-REFINED cites.
- Purpose / §3 / markers: refined-boot `[VM] include` / `[refined-boot] include=` / `load_refined_modules` / `refined-boot-demo` **stay** — tip3 is a Forth-surface mark-only echo **beside** them, not a replacement. Do not wipe wave10/11 refined-boot content.
- Non-goals: Forth `EVALUATE` / `INCLUDE` mark-only echo → `docs/EVALUATE-INCLUDE.md` (wave17 **3**). Still not a real file / nested-interpret VM; still not Win/Android refined execution.
- Acceptance: Lab smokes `eval-demo` (retains `refined-boot-demo`).
- Cite: `docs/EVALUATE-INCLUDE.md`.

### `docs/INTERPRET.md`

- Status / companions: cite wave17 **3**; add `EVALUATE-INCLUDE.md`; **keep** PARSE-NAME / EXIT-QUIT / COMMENT-PARSE / STRING-LIT / IMMEDIATE-POSTPONE / COLON / KERNEL cites.
- §1 / words: note EVALUATE/INCLUDE stub echo markers (not nested interpret re-entry / file VM); point to `EVALUATE-INCLUDE.md`.
- Words table: add `EVALUATE` / `INCLUDE` (+ mirrors `evaluate-mark` / `include-mark`) + `eval-demo` (cite tip).
- Markers: one-line pointer to `[eval]` markers; note refined-boot include markers stay elsewhere.
- Non-goals: EVALUATE/INCLUDE mark-only echo in scope via this tip; keep real nested interpret / file VM out.
- Acceptance: Lab smokes `eval-demo` (retains `interpret-demo` / `parse-demo` / `exit-demo` / `comment-demo` / `string-demo`).
- Cite: `docs/EVALUATE-INCLUDE.md`.

### `docs/HOST-BOOT.md`

- Companions: add `EVALUATE-INCLUDE.md` (wave17 **3**); **keep** REFINED-BOOT / HOST-PARITY / KERNEL / BUILD / INSTALL cites.
- Purpose / §2 / non-goals: poly/core `boot.fs` `include` load-order **stays** — tip3 Forth `include-mark` is echo-only and must **not** redefine host boot include list / semantics. Do not wipe wave10 host-boot content.
- Non-goals: Forth `EVALUATE` / `INCLUDE` mark-only echo → `docs/EVALUATE-INCLUDE.md` (wave17 **3**). Auto-including refined modules stays on REFINED-BOOT. Still not embedding a full Forth token VM.
- Acceptance: Lab smokes `eval-demo` (retains `host-boot-demo`).
- Cite: `docs/EVALUATE-INCLUDE.md`.

### `docs/KERNEL.md`

- Companions: add `EVALUATE-INCLUDE.md` (wave17 **3**); **keep** tip1 SYNONYM-ALIAS + tip2 PARSE-NAME cites and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `EVALUATE` / `evaluate-mark`, `INCLUDE` / `include-mark` stubs + `eval-demo` (cite tip; Forth mirrors — mark-only echo; **do not** redefine host/poly `include` or refined-boot load path; **not** a nested interpret / file VM).
- Non-goals: EVALUATE/INCLUDE mark-only echo → `docs/EVALUATE-INCLUDE.md`. PARSE/PARSE-NAME stays on wave17 **2**. SYNONYM/ALIAS stays on wave17 **1**. DEFER/IS/ACTION-OF / MARKER / BUFFER: / EXIT/QUIT stay on wave16 docs. Real nested interpret / file VM / RECURSE self-XT still later.
- Acceptance: Lab smokes `eval-demo` (and retains `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + prior demos).
- Cite: `docs/EVALUATE-INCLUDE.md`.

Do **not** wipe tip1 SYNONYM cites or tip2 PARSE cites or wave16 DEFER / MARKER / BUFFER / EXIT cites or wave15 / COMMENT / STRING / KERNEL / REFINED-BOOT / HOST-BOOT / INTERPRET prior content. Do **not** amend RECURSE / DOCS-CITES / PARSE-NAME / SYNONYM-ALIAS docs this tip (proposal amends are REFINED-BOOT + INTERPRET + HOST-BOOT + KERNEL only).

## 7. Non-goals

- Real `EVALUATE` nested-interpret VM / re-entering `interpret` on a string body
- Real `INCLUDE` file VM / opening and loading host files from the Forth mirror
- Redefining host boot `include` / poly/core `boot.fs` load order
- Replacing `load_refined_modules` / `refined-boot-demo` / `[VM] include` / `[refined-boot] include=`
- `RECURSE` real self-XT (wave17 **4** candidate — mark-only `recurse-mark` later)
- Docs cites pass (wave17 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `PARSE` / `PARSE-NAME` reopen (wave17 **2** — already stubbed; keep cites)
- `SYNONYM` / `ALIAS` reopen (wave17 **1** — already stubbed; keep cites)
- `MARKER` / `BUFFER:` / `EXIT` / `QUIT` (wave16 **2–4** — already stubbed; keep cites)
- IMMEDIATE / POSTPONE / FILL / PICK / CELL (wave15 — already stubbed)
- Full parser / tokenizer VM / WORD/BL rewrite
- Real DOES> XT chaining / threaded child runtime body
- Real branch XT / LEAVE jump
- Full Dusk arena / pool / free / fragmentation model
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live — skip 2DUP-FAMILY)
- `ABORT"` polish (already optional-wired inside `throw-demo` — skip)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/EVALUATE-INCLUDE.md` present (Research byte-copy OK); `REFINED-BOOT.md` + `INTERPRET.md` + `HOST-BOOT.md` + `KERNEL.md` thin amends present (wave10/8/10/7 text, tip1 SYNONYM cites, tip2 PARSE cites, and wave16 DEFER/MARKER/BUFFER/EXIT cites retained; refined-boot include markers retained).
2. `eval-demo` → OK (markers §4; EVALUATE greppable; INCLUDE name=|path= greppable; no miss FAIL on happy path). Prior `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` + `refined-boot-demo` + `host-boot-demo` + `interpret-demo` still OK.
3. Regression green (wave17 tip1–2 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `eval-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/REFINED-BOOT.md` (wave10 **4**), `docs/INTERPRET.md` (wave8 **1**), `docs/HOST-BOOT.md` (wave10 **1**), `docs/KERNEL.md` (wave7 **5**)
- `docs/PARSE-NAME.md` (wave17 **2**), `docs/SYNONYM-ALIAS.md` (wave17 **1**)
- `docs/DEFER-IS.md` (wave16 **1**), `docs/MARKER.md` (wave16 **2**), `docs/BUFFER-COLON.md` (wave16 **3**), `docs/EXIT-QUIT.md` (wave16 **4**)
- `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**), `docs/FILL-MOVE.md` (wave15 **3**), `docs/PICK-ROLL.md` (wave15 **2**), `docs/CELL-CELLS.md` (wave15 **1**)
- `docs/COMMENT-PARSE.md` (wave12 **3**), `docs/STRING-LIT.md` (wave13 **4**)
- `forth/tritium/kernel.fs`, `tritium.poly/core/boot.fs` (host `include` — untouched)
- `install/hosts/linux/tritiumos.c` (`load_refined_modules` — untouched)
- ANS Forth `EVALUATE` / `INCLUDE` (mark-only echo — no nested interpret / file VM)
- Explicit deferral: WAVE16-PROPOSAL / WAVE17-PROPOSAL (`EVALUATE` / `INCLUDE` — stub echo only; refined-boot stays)
- Base tip: `c0de0ab` / `c0de0abe358d6ef013a45f702e5b201549782143` (#78 wave17 tip2 PARSE-NAME)
- Wave17 proposal: `/workspace/tritium-research-docs/WAVE17-PROPOSAL.md`
