# TO-BODY — `>BODY` CREATE-body address mark + `body-demo`

**Status:** Shipper-ready stub spec (wave19 item **2**)
**Canonical brief:** ANS-shaped `>BODY` (thin CREATE-body address mark only); `docs/CREATE-DOES.md` (wave13 **3** — CREATE presence companion); `docs/TICK.md` (wave18 **1** — stub xt-id companion); `docs/ALLOT-HERE.md` (wave14 **1** — HERE pointer companion); `docs/KERNEL.md` (wave7 **5**); optional `docs/VARIABLE-CONST.md` (wave12 **2**); explicit WAVE18 / WAVE19 deferral closed as address mark only (not real DOES> XT / body image / linked XT)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `body.fs` / `to-body.fs`); Linux host REPL; **do not** redefine host `>BODY` that already binds on the load path
**Companions:** `docs/CREATE-DOES.md` (thin amend this tip), `docs/TICK.md` (thin amend this tip), `docs/ALLOT-HERE.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip); optional light cite `docs/VARIABLE-CONST.md`
**Base tip SHA:** `c2ba302` (wave19 tip1 PASS / #87 CHAR-CHARS) / full `c2ba3026fbd4a94970bf64578a67788570284eee`

## 1. Purpose

WAVE13 landed `CREATE` / `DOES>` as defining-word markers (`docs/CREATE-DOES.md`); WAVE18 landed `'` / `[']` stub xt-ids (`docs/TICK.md`); WAVE18 / WAVE19 explicitly deferred `>BODY` (CREATE-body address mark only; not real DOES> XT / body image). This tip lands a **stub** CREATE-body address mark only: `>BODY` (or Forth mirror `to-body`) prints `[body] >BODY` (+ optional `name=` / `addr=` / `xt=<id>`) for a known CREATE demo name / stub xt-id — the address is a **stub offset / echo** into the CREATE entry’s body slot picture (host int / fixed base+offset OK), **not** a mapped dictionary image or DOES> child XT list. Smoke via **`body-demo`**. Forth mirror **`to-body`** so host `>BODY` stays safe. **Do not** reopen DOES> as real XT; CREATE/DOES> markers stay mark-only. Builds on tick xt-ids + CREATE presence without promoting either to a real body image. Thin CREATE companion beside wave14 HERE/ALLOT pointer stubs — **not** a HERE bump / arena reopen.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `>BODY` / `to-body` | `( xt -- addr )` *or* `( "name" -- )` *or* `( -- )` then parse name | CREATE-body address mark; print `[body] >BODY` (+ optional `name=<name>` / `addr=<n>` / `xt=<id>`); address is stub offset / echo only |
| `body-demo` | `( -- )` | See §5 |

Host note: bind `>BODY` on the Linux REPL **only if** that name does not collide with a host Forth `>BODY` in the same load path. Prefer Forth mirror **`to-body`** as the Lab-facing surface when in doubt — **do not** redefine host `>BODY`. Optional `name=` / `addr=` / `xt=` are host strings / ints / stub xt ids only — not a callable XT, not a mapped dictionary image. Prefer resolving a known CREATE demo name (or stub xt-id consistent with wave18 TICK / FIND entry-index convention) so miss FAIL is avoided.

## 3. Stub semantics

- **`>BODY` / `to-body`:** resolve a known CREATE demo name / stub xt-id (via `entry-find` / CREATE presence / host mirror / demo fixture). Print `[body] >BODY` and optionally `name=<name>`, `addr=<n>` (stub body-slot offset / fixed base+offset — host int OK), and/or `xt=<id>` (stub xt id consistent with tip1 TICK convention when present). **Does not** map a dictionary image, compile a DOES> child XT list, rewrite the CREATE child’s runtime, bump HERE, allocate, or execute. Address is a **picture** of the CREATE entry’s body slot — not a real body pointer into arena memory.
- **Missing CREATE / miss name:** `[body] FAIL reason=miss` (demo **must avoid** — always resolve a known existing CREATE demo name / stub).
- Storage: name string + optional stub addr / xt id. **No** real DOES> XT chain, no linked XT, no body image / threaded child runtime, no HERE bump, no arena.
- Nest with prior CREATE / tick / find / allot / char / state / word / synonym / exit / buffer / marker / defer / imm / fill / pick / cell / throw / 2var / colon / control / string stubs OK. `dict-reset` clears CREATE stubs along with other entries (body mark needs a CREATE presence for the happy path — demo creates or reuses a fixture).
- Still no real DOES> XT chaining / threaded child runtime body, no ENVIRONMENT? (wave19 **3**), no SOURCE/PAD (wave19 **4**), no ACCEPT/REFILL, no linked XT / real branch XT / full arena/heap / full Win/Android Forth VM. Those stay non-goals / later tips. CHAR-CHARS (wave19 **1**) stays landed — keep cites; this tip does not reopen char-unit.

## 4. Markers

```
[body] >BODY [name=<name>] [addr=<n>] [xt=<id>]   # name=/addr=/xt= optional
[body] FAIL reason=miss                            # demo avoids
[body-demo] OK
[body-demo] FAIL
```

Lab greps `[body-demo] OK` plus at least one `[body] >BODY` (optional `name=` / `addr=` / `xt=` welcome). Demo avoids `[body] FAIL reason=miss`.

## 5. `body-demo`

1. Clean slate / `dict-reset` (or cold path).
2. Ensure a known CREATE name exists (e.g. `CREATE widget` / `create-entry` fixture, or reuse a greppable CREATE demo name) so miss FAIL is avoided. Optional: record stub xt-id via tick / find for optional `xt=` echo consistency — not required for Lab OK.
3. Invoke `>BODY` (or `to-body`) on that known CREATE name / stub xt → `[body] >BODY` (+ optional `name=` / `addr=` / `xt=`).
4. Assert no `[body] FAIL reason=miss` on the happy path. Assert body mark did **not** require a real DOES> XT chain / mapped dictionary image / linked XT / HERE bump (marker-only is enough).
5. Prior `char-demo` / `state-demo` / `word-demo` / `find-demo` / `tick-demo` / `recurse-demo` / `eval-demo` / `parse-demo` / `synonym-demo` / `exit-demo` / `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` / `refined-boot-demo` / `host-boot-demo` / `interpret-demo` / `kernel-demo` still OK.
6. `[body-demo] OK`.

`>BODY` marker is required. Miss FAIL path is not exercised by the demo. Optional `name=` / `addr=` / `xt=` echo is not required for Lab OK. No real DOES> XT. No linked XT. No body image. No HERE bump.

## 6. Thin amend — companions

### `docs/CREATE-DOES.md`

- Companions / Status: add `TO-BODY.md` (wave19 **2** companion cite); **keep** COLON / VARIABLE-CONST / KERNEL / ALLOT-HERE / DEFER / SYNONYM / TICK cites — do not wipe wave13 CREATE-DOES content.
- Purpose / §3: CREATE/DOES> stay defining-word markers; `>BODY` CREATE-body address mark points at this tip — stub offset / echo only; **not** a real DOES> XT reopen / child runtime body / linked XT / body image.
- Non-goals: `>BODY` → `docs/TO-BODY.md` (wave19 **2**). Still no real DOES> XT chaining. TICK stays wave18 **1**. ALLOT-HERE stays wave14 **1**.
- Acceptance: Lab smokes `body-demo` (retains `create-demo` + `tick-demo`).
- Cite: `docs/TO-BODY.md`.

### `docs/TICK.md`

- Companions: add `TO-BODY.md` (wave19 **2**); **keep** KERNEL / DEFER-IS / IMMEDIATE-POSTPONE / CREATE-DOES / STATE-COMPILE cites.
- Purpose / §3 / non-goals: tick stays name→stub-xt-id mark; `>BODY` may echo optional stub `xt=` consistent with this tip’s convention when resolving a CREATE name — **not** XT execute / DOES> chain / body image. Do not wipe wave18 TICK content.
- Non-goals: `>BODY` → `docs/TO-BODY.md` (wave19 **2**). FIND / WORD-BL / STATE stay on wave18 docs. Still no XT execute / real DOES> XT.
- Acceptance: Lab smokes `body-demo` (retains `tick-demo` + `create-demo`).
- Cite: `docs/TO-BODY.md`.

### `docs/ALLOT-HERE.md`

- Companions: add `TO-BODY.md` (wave19 **2**); **keep** KERNEL / VARIABLE-CONST / CREATE-DOES / CELL-CELLS / FILL-MOVE / MARKER / BUFFER cites.
- Purpose / §3 / non-goals: HERE/ALLOT stay pointer/bump stubs; `>BODY` address is a sibling **stub offset / echo** into a CREATE body-slot picture — **not** a HERE bump / arena reopen / mapped dictionary image. Do not wipe wave14 ALLOT-HERE content.
- Non-goals: `>BODY` → `docs/TO-BODY.md` (wave19 **2**). Full arena / free / DOES> XT still out. CREATE/DOES> stay on `CREATE-DOES.md`.
- Acceptance: Lab smokes `body-demo` (retains `allot-demo` + `create-demo`).
- Cite: `docs/TO-BODY.md`.

### `docs/KERNEL.md`

- Companions: add `TO-BODY.md` (wave19 **2**); **keep** tip1 CHAR-CHARS cite and wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites and wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `>BODY` stub + `body-demo` (cite tip; Forth mirror `to-body` — CREATE-body address mark only; **do not** redefine host `>BODY`; **not** real DOES> XT / linked XT / body image / HERE bump).
- Non-goals: `>BODY` CREATE-body address mark → `docs/TO-BODY.md`. CHAR/CHARS/[CHAR] stay on `CHAR-CHARS.md`. CREATE/DOES> stay on `CREATE-DOES.md`. TICK / FIND / WORD-BL / STATE stay on wave18 docs. ENVIRONMENT? / SOURCE-PAD still later (wave19 **3–4**).
- Acceptance: Lab smokes `body-demo` (and retains `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `create-demo` + `allot-demo` + prior demos).
- Cite: `docs/TO-BODY.md`.

### Optional — `docs/VARIABLE-CONST.md`

- Companions: add light `TO-BODY.md` (wave19 **2**) cite; **keep** KERNEL / COLON / WORDS-VOCAB / CREATE-DOES / ALLOT-HERE / CELL / DEFER / BUFFER cites.
- Purpose / non-goals: VARIABLE/CONSTANT stay named-cell stubs; CREATE companion `>BODY` address mark points at this tip — **not** a VARIABLE body / DOES> XT / arena. Do not wipe wave12 VARIABLE-CONST content.
- Non-goals: `>BODY` → `docs/TO-BODY.md` (wave19 **2**). CREATE/DOES> stay on `CREATE-DOES.md`. ALLOT-HERE stays wave14 **1**.
- Acceptance: Lab smokes `body-demo` (retains `var-demo` + `create-demo`).
- Cite: `docs/TO-BODY.md`.

Do **not** wipe tip1 CHAR-CHARS cites or wave18 tip1–4 TICK / FIND / WORD-BL / STATE-COMPILE cites or wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites or wave16 DEFER / MARKER / BUFFER / EXIT cites or wave15 CELL / IMMEDIATE / FILL / PICK prior content. Do **not** amend ARCHITECTURE / IMPLEMENTATION-GAPS / CHAR-CHARS / ENVIRONMENT / SOURCE-PAD / DOCS-CITES docs this tip (proposal amends are CREATE-DOES + TICK + ALLOT-HERE + KERNEL + optional VARIABLE-CONST only). Tip5 cites come after 1–4 PASS.

## 7. Non-goals

- Real DOES> XT chaining / threaded child runtime body / rewriting CREATE
- Mapped dictionary image / real body pointer into arena memory
- Linked XT / executing looked-up XT / XT body append
- HERE bump via `>BODY` / composing body allot (optional later; not required)
- `ENVIRONMENT?` query stub (wave19 **3**)
- `SOURCE` / `PAD` thin marks (wave19 **4**); `ACCEPT` / `REFILL` / full input-buffer VM still out
- Docs cites pass (wave19 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `CHAR` / `CHARS` / `[CHAR]` reopen (wave19 **1** — already stubbed; keep cites)
- `CREATE` / `DOES>` reopen as real XT (wave13 **3** — already mark-only; keep cites)
- `'` / `[']` / `FIND` / `WORD` / `BL` / `STATE` / `COMPILE,` reopen (wave18 — already stubbed; keep cites)
- `HERE` / `ALLOT` arena reopen (wave14 **1** — pointer stubs already landed; keep cites)
- Real branch XT / LEAVE jump
- Full Dusk arena / pool / free / fragmentation model
- Linked XT / executing postponed XT / real STATE cell
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live — **skip 2DUP-FAMILY**)
- `ABORT"` polish (already optional-wired inside `throw-demo` — **skip**)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/TO-BODY.md` present (Research byte-copy OK); `CREATE-DOES.md` + `TICK.md` + `ALLOT-HERE.md` + `KERNEL.md` thin amends present (+ optional `VARIABLE-CONST.md`); tip1 CHAR-CHARS cites, wave18 tip1–4 cites, wave17 tip1–4 cites, wave16 DEFER/MARKER/BUFFER/EXIT cites, and wave15 CELL/IMMEDIATE/COLON/KERNEL prior text retained; host `>BODY` untouched via mirrors.
2. `body-demo` → OK (markers §4; `[body] >BODY` greppable; optional `name=` / `addr=` / `xt=` welcome; no miss FAIL on happy path; no real DOES> XT / linked XT / body image / HERE bump). Prior `char-demo` + `state-demo` + `word-demo` + `find-demo` + `tick-demo` + `recurse-demo` + `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` + `refined-boot-demo` + `host-boot-demo` + `interpret-demo` + `kernel-demo` still OK.
3. Regression green (wave19 tip1 + wave18 tip1–5 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `body-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/CREATE-DOES.md` (wave13 **3** — CREATE presence companion; >BODY is address mark beside CREATE — not DOES> XT reopen)
- `docs/TICK.md` (wave18 **1** — stub xt-id companion; optional `xt=` consistency)
- `docs/ALLOT-HERE.md` (wave14 **1** — HERE pointer companion; >BODY does not bump HERE / reopen arena)
- `docs/KERNEL.md` (wave7 **5**), `docs/VARIABLE-CONST.md` (wave12 **2**, optional)
- `docs/CHAR-CHARS.md` (wave19 **1** — prior tip; keep cites)
- `docs/FIND.md` (wave18 **2**), `docs/STATE-COMPILE.md` (wave18 **4**)
- `docs/DEFER-IS.md` (wave16 **1**), `docs/SYNONYM-ALIAS.md` (wave17 **1**)
- `docs/COLON.md` (wave9 **4**), `docs/CELL-CELLS.md` (wave15 **1**)
- `forth/tritium/kernel.fs` (to-body only — do not redefine host >BODY)
- ANS Forth `>BODY` (CREATE-body address mark only — not real DOES> XT / body image / linked XT)
- Explicit deferral: WAVE18-PROPOSAL + WAVE19-PROPOSAL (`>BODY` — address mark; not DOES> XT / body image)
- Base tip: `c2ba302` / `c2ba3026fbd4a94970bf64578a67788570284eee` (#87 wave19 tip1 CHAR-CHARS PASS)
- Wave19 proposal: `/workspace/tritium-research-docs/WAVE19-PROPOSAL.md`
