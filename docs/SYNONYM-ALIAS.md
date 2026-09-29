# SYNONYM-ALIAS — `SYNONYM` / `ALIAS` name-map stubs + `synonym-demo`

**Status:** Shipper-ready stub spec (wave17 item **1**)
**Canonical brief:** ANS-shaped `SYNONYM` / `ALIAS` (thin name→name map markers only); `docs/WORDS-VOCAB.md` (wave11 **3**); `docs/DEFER-IS.md` (wave16 **1**); `docs/KERNEL.md` (wave7 **5**); `docs/CREATE-DOES.md` (wave13 **3**); explicit WAVE16 deferral closed as name-map stub only
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `synonym.fs`); Linux host REPL
**Companions:** `docs/WORDS-VOCAB.md` (thin amend this tip), `docs/DEFER-IS.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip); optional light cite `docs/CREATE-DOES.md`
**Base tip SHA:** `d683662` (wave16 tip5 CLOSED / #76 docs cites) / full `d68366223298199849e957c83954017693ecbb86`

## 1. Purpose

WAVE16 explicitly deferred `SYNONYM` / `ALIAS` (linked XT / FIND rewrite / executing aliased XT). Flat WORDS list and DEFER name-bind stubs already exist; a synonym name-map stub surface does not. This tip lands **stub** name→name markers only: `SYNONYM <new> <old>` (or `ALIAS <new> <old>`) records a stub binding and prints `[synonym] SYNONYM new= old=` (ALIAS may share the `[synonym]` prefix or print `[synonym] ALIAS new= old=`). Lookup/print of the alias name echoes the bound old name (`[synonym] resolve new= old=`). Smoke via **`synonym-demo`**. Forth mirrors **`synonym-map` / `alias-map`** so any host name collisions stay safe. **Not** a linked XT, not FIND rewrite, not executing the old word through the new name. Pairs with wave16 DEFER name-bind surface and wave11 WORDS list without promoting either to a real dict alias table.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `SYNONYM` / `synonym-map` | `( "new" "old" -- )` *or* `( -- )` then parse two names | Record stub name→name binding; print `[synonym] SYNONYM new=<new> old=<old>` |
| `ALIAS` / `alias-map` | `( "new" "old" -- )` *or* `( -- )` then parse two names | Same as SYNONYM; print `[synonym] ALIAS new=<new> old=<old>` **or** share `[synonym] SYNONYM …` prefix (document choice; either OK for Lab) |
| *(resolve / print alias)* | `( "new" -- )` *or* invoke alias name | Lookup/print bound old name; print `[synonym] resolve new=<new> old=<old>` |
| `synonym-demo` | `( -- )` | See §5 |

Host note: bind `SYNONYM` / `ALIAS` on the Linux REPL; Forth mirrors **`synonym-map` / `alias-map`** if host Forth names collide. Prefer one map table shared by both spellings. Bound values are name strings only — not callable XTs.

## 3. Stub semantics

- **SYNONYM new old / ALIAS new old:** record a stub binding `new → old` (host side-table or entry flag holding two name strings). Print `[synonym] SYNONYM new=<new> old=<old>` for SYNONYM; for ALIAS either `[synonym] ALIAS new=<new> old=<old>` or the shared SYNONYM marker (Shipper documents which). Dict presence of `new` is optional this tip — Lab greps the marker; optional `WORDS` / find hit on `new` is fine when cheap but **not** required (name map only — not a new XT entry that executes `old`).
- **Resolve / print alias:** look up binding by `new`; print `[synonym] resolve new=<new> old=<old>` matching the last map. **Does not** FIND-rewrite, link XT, or execute `old` through `new`.
- **Missing old / unbound new:** `[synonym] FAIL reason=miss` (demo **must avoid** — always map a known existing `old` name, and resolve only after map). Optional miss on resolve of unbound `new` is OK if documented; prefer demo maps first.
- Storage: name string pair (`new`, `old`). **No** linked XT, no FIND/entry-body rewrite, no executing old through new, no DEFER vector, no PARSE-NAME deepen.
- Nest with prior exit / buffer / marker / defer / imm / fill / pick / cell / allot / throw / 2var / create / colon / control / string / words stubs OK. `dict-reset` clears synonym map entries as usual.
- Still no linked XT / FIND rewrite / executing aliased XT, no DEFER vector reopen, no PARSE-NAME deepen, no EVALUATE/INCLUDE nested VM, no RECURSE self-XT, no real DOES> XT, no real branch XT, no full Win/Android Forth VM. Those stay non-goals / later tips.

## 4. Markers

```
[synonym] SYNONYM new=<new> old=<old>
[synonym] ALIAS new=<new> old=<old>     # or share SYNONYM marker prefix
[synonym] resolve new=<new> old=<old>
[synonym] FAIL reason=miss              # demo avoids
[synonym-demo] OK
[synonym-demo] FAIL
```

Lab greps `[synonym-demo] OK` plus at least one `[synonym] SYNONYM` (or `[synonym] ALIAS`) with `new=` / `old=`, and one `[synonym] resolve` with matching `new=` / `old=`. Demo avoids `[synonym] FAIL reason=miss`.

## 5. `synonym-demo`

1. Clean slate / `dict-reset` (or cold path).
2. Ensure a known `old` name exists (e.g. create via `entry-create` / VARIABLE / CREATE stub, or use a greppable existing demo name) so miss FAIL is avoided.
3. `SYNONYM` (or `ALIAS`) binding `new` → `old` (e.g. `synonym-map widget gadget` or ANS-shaped `SYNONYM widget gadget`) → `[synonym] SYNONYM new=widget old=gadget` (or ALIAS form).
4. Resolve / print alias for `new` → `[synonym] resolve new=widget old=gadget`.
5. Assert no `[synonym] FAIL reason=miss` on the happy path. Assert resolve did **not** require executing `old` through `new` / FIND rewrite (marker-only is enough).
6. Prior `exit-demo` / `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` still OK.
7. `[synonym-demo] OK`.

SYNONYM (or ALIAS) + resolve markers are required. Miss FAIL path is not exercised by the demo. Optional WORDS/find presence of `new` is not required for Lab OK.

## 6. Thin amend — companions

### `docs/WORDS-VOCAB.md`

- Companions: add `SYNONYM-ALIAS.md` (wave17 **1**); **keep** KERNEL / INTERPRET / COLON / VARIABLE-CONST / MARKER cites.
- Purpose / §4: flat list stays list-only; SYNONYM/ALIAS name-map stubs point at this tip — **not** a real dict alias table / FIND rewrite / SEARCH-WORDLIST. Do not wipe wave11 / MARKER content.
- Non-goals: `SYNONYM` / `ALIAS` → `docs/SYNONYM-ALIAS.md` (wave17 **1**). SEARCH-WORDLIST / linked dict still later. MARKER restore-mark stays on `MARKER.md`.
- Acceptance: Lab smokes `synonym-demo` (retains `words-demo`).
- Cite: `docs/SYNONYM-ALIAS.md`.

### `docs/DEFER-IS.md`

- Companions: add `SYNONYM-ALIAS.md` (wave17 **1**); **keep** CREATE-DOES / VARIABLE-CONST / KERNEL cites and wave16 MARKER / BUFFER / EXIT closed cites (do not wipe).
- Purpose / §3: deferred-word stubs stay name + bind markers; SYNONYM/ALIAS is a sibling **name→name map** — **not** a DEFER vector / linked XT / executing bound XT. Do not wipe wave16 DEFER content.
- Non-goals: `SYNONYM` / `ALIAS` → `docs/SYNONYM-ALIAS.md` (wave17 **1**). MARKER / BUFFER: / EXIT-QUIT stay wave16 **2–4** closed (already stubbed — keep cites). Linked XT / executing bound XT still later.
- Acceptance: Lab smokes `synonym-demo` (retains `defer-demo`).
- Cite: `docs/SYNONYM-ALIAS.md`.

### `docs/KERNEL.md`

- Companions: add `SYNONYM-ALIAS.md` (wave17 **1**); **keep** wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `SYNONYM` / `ALIAS` stubs + `synonym-demo` (cite tip; Forth mirrors `synonym-map` / `alias-map` — name map only; **not** FIND rewrite / linked XT).
- Non-goals: SYNONYM/ALIAS name-map stubs → `docs/SYNONYM-ALIAS.md`. DEFER/IS/ACTION-OF / MARKER / BUFFER: / EXIT/QUIT stay on wave16 docs. Linked XT / FIND rewrite / PARSE-NAME deepen still later.
- Acceptance: Lab smokes `synonym-demo` (and retains `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + prior demos).
- Cite: `docs/SYNONYM-ALIAS.md`.

### Optional — `docs/CREATE-DOES.md`

- Companions / Purpose / Cite: light cross-cite `SYNONYM-ALIAS.md` (wave17 **1**) as name-map sibling to defining-word / DEFER surface — **not** a CREATE child XT / DOES> chain. Keep DEFER-IS / ALLOT-HERE / wave13 text.

Do **not** wipe wave16 DEFER / MARKER / BUFFER / EXIT cites or wave15 / WORDS / KERNEL prior content. Do **not** amend PARSE / EVALUATE / RECURSE docs this tip (proposal amends are WORDS-VOCAB + DEFER-IS + KERNEL; CREATE-DOES optional light cite only).

## 7. Non-goals

- Linked XT / executing aliased XT / FIND rewrite / entry-body rewrite
- Redefining host FIND / entry bodies
- DEFER vector reopen (wave16 **1** — already stubbed; keep cites; synonym is name→name, not XT bind)
- `PARSE` / `PARSE-NAME` deepen (wave17 **2** candidate)
- `EVALUATE` / `INCLUDE` nested interpret / file VM (wave17 **3** candidate — mark-only later; refined-boot stays)
- `RECURSE` real self-XT (wave17 **4** candidate — mark-only `recurse-mark` later)
- Docs cites pass (wave17 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `MARKER` / `BUFFER:` / `EXIT` / `QUIT` (wave16 **2–4** — already stubbed; keep cites)
- IMMEDIATE / POSTPONE / FILL / PICK / CELL (wave15 — already stubbed)
- Real DOES> XT chaining / threaded child runtime body
- Real branch XT / LEAVE jump
- Full Dusk arena / pool / free / fragmentation model
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live — skip 2DUP-FAMILY)
- `ABORT"` polish (already optional-wired inside `throw-demo` — skip)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/SYNONYM-ALIAS.md` present (Research byte-copy OK); `WORDS-VOCAB.md` + `DEFER-IS.md` + `KERNEL.md` thin amends present (wave11/16/7 text and wave16 DEFER/MARKER/BUFFER/EXIT cites retained); optional light `CREATE-DOES.md` cite OK.
2. `synonym-demo` → OK (markers §4; SYNONYM or ALIAS greppable with `new=`/`old=`; resolve greppable; no miss FAIL on happy path). Prior `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` still OK.
3. Regression green (wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `synonym-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/WORDS-VOCAB.md` (wave11 **3**), `docs/DEFER-IS.md` (wave16 **1**), `docs/KERNEL.md` (wave7 **5**), `docs/CREATE-DOES.md` (wave13 **3**)
- `docs/MARKER.md` (wave16 **2**), `docs/BUFFER-COLON.md` (wave16 **3**), `docs/EXIT-QUIT.md` (wave16 **4**)
- `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**), `docs/FILL-MOVE.md` (wave15 **3**), `docs/PICK-ROLL.md` (wave15 **2**), `docs/CELL-CELLS.md` (wave15 **1**)
- `forth/tritium/kernel.fs`
- ANS Forth `SYNONYM` / `ALIAS` (name→name map mark only — no linked XT / FIND rewrite / execute-through)
- Explicit deferral: WAVE16-PROPOSAL / WAVE17-PROPOSAL (`SYNONYM` / `ALIAS` — name map; not linked XT)
- Base tip: `d683662` / `d68366223298199849e957c83954017693ecbb86` (#76 wave16 tip5 docs cites)
- Wave17 proposal: `/workspace/tritium-research-docs/WAVE17-PROPOSAL.md`
