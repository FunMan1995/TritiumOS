# FIND — ANS-ish `FIND` markers + `find-demo`

**Status:** Shipper-ready stub spec (wave18 item **2**; thin amend wave19 **3** ENVIRONMENT-QUERY companion cite; thin amend wave20 **4** EXECUTE companion cite; thin amend wave22 **4** SEARCH-WORDLIST companion cite; thin amend wave24 **4** WORDLIST companion cite — sibling vocab create/query; **not** FIND reopen; do not redefine `find`/`findentry`/`entry-find`/`find-xt`/`find-mark`)
**Canonical brief:** ANS-shaped `FIND` (name→flag/id mark deepen beside wave7 `find`/`findentry` index aliases); `docs/KERNEL.md` (wave7 **5** — host `find`/`findentry`/`entry-find` stay); `docs/WORDS-VOCAB.md` (wave11 **3**); `docs/SYNONYM-ALIAS.md` (wave17 **1** — name map only; not FIND rewrite); `docs/INTERPRET.md` (wave8 **1**); `docs/TICK.md` (wave18 **1** — stub `xt=` ids stay consistent); explicit WAVE17 / WAVE18 deferral closed as FIND mark deepen only (not SEARCH-WORDLIST / linked dict / host find rewrite / execute-through); wave22 **4** `SEARCH-WORDLIST` sibling vocab-search mark → `docs/SEARCH-WORDLIST.md` (**not** FIND reopen; host find/find-xt/find-mark stay untouched)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `find.fs`); Linux host REPL; **do not** redefine host `find` / `findentry` / `entry-find` used by `kernel-demo` / interpret
**Companions:** `docs/KERNEL.md` (thin amend this tip), `docs/WORDS-VOCAB.md` (thin amend this tip), `docs/SYNONYM-ALIAS.md` (thin amend this tip), `docs/INTERPRET.md` (thin amend this tip); `docs/ENVIRONMENT-QUERY.md` (wave19 **3** — query-mark companion cite; not SEARCH-WORDLIST / FIND rewrite); `docs/EXECUTE.md` (wave20 **4** — xt-id invoke mark companion cite; not execute-through / real XT execute); `docs/SEARCH-WORDLIST.md` (wave22 **4** — sibling vocab-search mark; **not** FIND reopen; host find/findentry/entry-find + find-xt/find-mark untouched; prefer `search-wl-mark`); `docs/WORDLIST.md` (wave24 **4** — sibling thin vocab create/query; **not** FIND reopen; prefer `wordlist-mark`/`forth-wordlist-mark`; do not redefine find*/find-xt/find-mark)
**Base tip SHA:** `41e1438` (wave18 tip1 TICK PASS / #82) / full `41e1438623ffbf4a938e89d8195910de8d0fe9e6`

## 1. Purpose

WAVE17 and WAVE18 explicitly deferred `FIND` deepen (ANS-ish find mark beyond `entry-find` index alias; not SEARCH-WORDLIST / linked dict rewrite). Wave7 already landed `find` / `findentry` as aliases of `entry-find` (index or `-1` miss) used by `kernel-demo` / `interpret-token`. Tip1 TICK unlocked consistent stub `xt=` ids (entry-index host ints — not executable). This tip lands **stub** ANS-ish FIND markers only: `FIND` (or Forth mirror `find-xt`) prints `[find] FIND name=` (+ optional `xt=<id>` / `flag=<1|-1|0>` / `i=`) for a demo counted-string / name fixture — hit → greppable flag/id mark; miss → `[find] FIND miss` or `[find] FAIL reason=miss` (demo may show one miss line or avoids FAIL). Smoke via **`find-demo`**. Forth mirrors **`find-xt` / `find-mark`** so host `find` / `findentry` / `entry-find` stay untouched. **Not** SEARCH-WORDLIST, not linked dict, not execute-through, not SYNONYM FIND rewrite. Builds on tip1 so optional `xt=` ids stay consistent without a real XT table. Wave19 tip **3** lands `ENVIRONMENT?` query mark (`docs/ENVIRONMENT-QUERY.md`): sibling query against a fixed demo string set — **not** a FIND reopen / SEARCH-WORDLIST / wordlist rewrite. Wave20 tip **4** lands `EXECUTE` xt-id invoke mark (`docs/EXECUTE.md`; Forth mirror `execute-mark`): sibling invoke mark that may echo stub `xt=` — **not** a FIND reopen / execute-through / real XT execute / linked XT. Wave22 tip **4** lands `SEARCH-WORDLIST` thin vocab-search mark (`docs/SEARCH-WORDLIST.md`; Forth mirror `search-wl-mark`): sibling vocab-search mark over a fixed demo name / wordlist fixture — **not** a FIND reopen / host-find rewrite / find-xt redefine / linked-dict rewrite / SYNONYM FIND rewrite / EXECUTE reopen. **CRITICAL:** host `find` / `findentry` / `entry-find` + wave18 `find-xt` / `find-mark` stay untouched this tip and that tip. Wave24 tip **4** lands `WORDLIST`/`FORTH-WORDLIST` thin vocab create/query (`docs/WORDLIST.md`; Forth mirrors `wordlist-mark`/`forth-wordlist-mark`): sibling create/query beside FIND + SEARCH-WORDLIST — **not** a FIND reopen / SEARCH-WORDLIST runtime / linked dict / DEFINITIONS deep rewrite; **do not** redefine `find`/`findentry`/`entry-find`/`find-xt`/`find-mark`.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `FIND` / `find-xt` | `( c-addr -- )` *or* `( c-addr u -- )` *or* `( "name" -- )` | ANS-ish find mark; print `[find] FIND name=<name>` (+ optional `xt=<id>` / `flag=<1\|-1\|0>` / `i=<index>`); does **not** execute the found XT; does **not** redefine host `find`/`findentry`/`entry-find` |
| `find-mark` | `( c-addr -- )` *or* `( "name" -- )` | Optional alias of `find-xt` (document if used); same markers |
| `find-demo` | `( -- )` | See §5 |

Host note: bind bare `FIND` on the Linux REPL **only if** it does not collide with host `find` / `findentry` / `entry-find` in the same load path. Prefer Forth mirrors **`find-xt` / `find-mark`** as the Lab-facing surface — **do not** redefine host `find` / `findentry` / `entry-find` used by `kernel-demo` / interpret. Optional stub `xt=` / `i=` are host ints / entry indices only (same convention as tip1 TICK — stub xt id = entry index) — not callable XTs. Prefer looking up a known dict/demo name (CREATE / colon / VARIABLE / DEFER entry, or a greppable demo fixture such as `widget`) so miss FAIL is optional / controlled.

## 3. Stub semantics

- **`FIND` / `find-xt` name (or counted-string fixture):** resolve via `entry-find` / host mirror / demo fixture table (same lookup path as tip1 tick — **do not** rewrite host `find`). Print `[find] FIND name=<name>` (+ optional `xt=<id>` stub id matching tip1 convention, `flag=<1|-1|0>`, and/or `i=<index>` entry index). **Does not** execute the looked-up word, push a real XT, rewrite host find aliases, or SEARCH-WORDLIST. Captured values are host ints / name strings / ANS-ish flag echo only.
- **ANS-ish flag picture (optional):** `flag=1` → immediate (ENTRY-IMM? set or demo fixture); `flag=-1` → found non-immediate; `flag=0` → miss. Lab does **not** require stack effects matching full ANS `FIND ( c-addr -- c-addr 0 | xt 1 | xt -1 )` — marker greppability is enough. When `flag=` is omitted, hit is still greppable via `name=` (+ optional `xt=` / `i=`).
- **Hit:** greppable `[find] FIND name=<name>` with optional `xt=` / `flag=` / `i=`. Prefer `flag=-1` (or `flag=1` if the fixture is marked immediate) on happy-path hit.
- **Miss:** `[find] FIND miss` **or** `[find] FAIL reason=miss`. Demo may show **one** miss line (preferred Lab picture — greps miss without FAIL) **or** avoid FAIL entirely. Do not soft-abort the host / kernel-demo path.
- Storage: name string + optional stub xt id / entry index / flag echo. **No** SEARCH-WORDLIST, no linked dict, no execute-through, no SYNONYM FIND rewrite, no host `find`/`findentry`/`entry-find` redefine, no COMPILE, / STATE reopen.
- Nest with prior tick / recurse / eval / parse / synonym / exit / buffer / marker / defer / imm / fill / pick / cell / allot / throw / 2var / create / colon / control / string stubs OK. `dict-reset` clears as usual. Soft KERNEL `abort` / `(abort")`, wave14 CATCH/THROW, and wave16 EXIT/QUIT mark stubs stay as-is beside this marker. Host `find` / `findentry` / `entry-find` + `[kernel] find hit` / `[kernel] find miss` markers stay as-is (kernel-demo green).
- Still no linked dict / execute-through / WORD-BL (wave18 **3**) / STATE-COMPILE (wave18 **4**) / linked XT / real DOES> XT / real branch XT / full arena/heap / full Win/Android Forth VM. ENVIRONMENT? query mark → `docs/ENVIRONMENT-QUERY.md` (wave19 **3** — thin query mark; not SEARCH-WORDLIST / FIND rewrite). EXECUTE invoke mark → `docs/EXECUTE.md` (wave20 **4** — xt-id invoke mark only; not execute-through / real XT execute). SEARCH-WORDLIST thin vocab-search mark → `docs/SEARCH-WORDLIST.md` (wave22 **4** — sibling mark only; **not** FIND reopen; host find/find-xt/find-mark untouched; prefer `search-wl-mark`; not full linked dict / wordlist-stack runtime). WORDLIST thin vocab create/query → `docs/WORDLIST.md` (wave24 **4** — sibling create/query only; **not** FIND reopen; prefer `wordlist-mark`/`forth-wordlist-mark`; do not redefine find*/find-xt/find-mark; not SEARCH-WORDLIST runtime / linked dict / DEFINITIONS). Those stay non-goals / companion cites (EXECUTE mark still not execute-through; SEARCH-WORDLIST still not FIND reopen; WORDLIST still not FIND reopen).

## 4. Markers

```
[find] FIND name=<name> [xt=<id>] [flag=<1|-1|0>] [i=<index>]   # xt=/flag=/i= optional
[find] FIND miss                                                 # optional miss line (demo may show one)
[find] FAIL reason=miss                                          # demo may avoid; or use FIND miss instead
[find-demo] OK
[find-demo] FAIL
```

Lab greps `[find-demo] OK` plus at least one `[find] FIND name=` (hit). Optional `xt=` / `flag=` / `i=` fields are not required for Lab OK when present. Demo may emit one `[find] FIND miss` (preferred) or avoid miss/FAIL entirely. Prefer not emitting `[find] FAIL reason=miss` on the happy path.

## 5. `find-demo`

1. Clean slate / `dict-reset` (or cold path).
2. Ensure a known name exists (e.g. create via `entry-create` / VARIABLE / CREATE / colon stub, or use a greppable existing demo name such as tip1 `widget` fixture) so hit is greppable. Prefer same stub xt-id convention as tip1 (`xt=` = entry index).
3. Invoke `FIND` (or `find-xt` / `find-mark`) on that known name (counted-string fixture or parse-name form) → `[find] FIND name=<name>` (+ optional `xt=` / `flag=-1` or `flag=1` / `i=`).
4. Optional: invoke on an unknown name → `[find] FIND miss` (preferred Lab miss picture) **or** skip miss entirely. Do **not** require `[find] FAIL reason=miss`.
5. Assert find did **not** redefine host `find`/`findentry`/`entry-find`, did **not** execute the found XT, and did **not** SEARCH-WORDLIST / linked-dict rewrite (marker-only is enough). Assert `kernel-demo` path still uses host find aliases.
6. Prior `tick-demo` / `recurse-demo` / `eval-demo` / `parse-demo` / `synonym-demo` / `exit-demo` / `buffer-demo` / `marker-demo` / `defer-demo` / `imm-demo` / `fill-demo` / `pick-demo` / `cell-demo` / `allot-demo` / `throw-demo` / `2var-demo` / `unloop-demo` / `string-demo` / `create-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `colon-demo` / `words-demo` / `kernel-demo` / `refined-boot-demo` / `host-boot-demo` / `interpret-demo` still OK.
7. `[find-demo] OK`.

`FIND` / `find-xt` hit marker is required. Miss line is optional (one `[find] FIND miss` welcome). Optional `xt=` / `flag=` / `i=` echo is not required for Lab OK. No XT execute. No host find rewrite. No SEARCH-WORDLIST. No SYNONYM FIND rewrite.

## 6. Thin amend — companions

### `docs/KERNEL.md`

- Companions: add `FIND.md` (wave18 **2**); **keep** tip1 TICK cite and wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites and wave16 DEFER / MARKER / BUFFER / EXIT cites and wave15 IMMEDIATE/FILL/PICK/CELL cites.
- Words table: add `FIND` / `find-xt` stubs + `find-demo` (cite tip; Forth mirrors `find-xt` / `find-mark` — ANS-ish find mark only; **do not** redefine host `find` / `findentry` / `entry-find`; **not** SEARCH-WORDLIST / linked dict / execute-through / SYNONYM FIND rewrite). Host `find`/`findentry` rows stay as wave7 aliases.
- Non-goals: FIND deepen → `docs/FIND.md`. TICK stays on `TICK.md`. RECURSE / EVALUATE / PARSE / SYNONYM stay on wave17 docs. DEFER/IS/ACTION-OF / MARKER / BUFFER: / EXIT/QUIT stay on wave16 docs. WORD-BL / STATE-COMPILE still later (wave18 **3–4**). SEARCH-WORDLIST / linked dict / XT execute still later.
- Acceptance: Lab smokes `find-demo` (and retains `tick-demo` + `recurse-demo` + `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `colon-demo` + `kernel-demo` + prior demos).
- Cite: `docs/FIND.md`.

### `docs/WORDS-VOCAB.md`

- Companions: add `FIND.md` (wave18 **2**); **keep** KERNEL / INTERPRET / COLON / VARIABLE-CONST / MARKER / SYNONYM-ALIAS cites.
- Purpose / §4: flat list stays list-only; FIND deepen is a sibling ANS-ish find mark — **not** SEARCH-WORDLIST / linked dict / WORDS rewrite. Do not wipe wave11 / MARKER / SYNONYM content.
- Non-goals: `FIND` deepen → `docs/FIND.md` (wave18 **2**). SEARCH-WORDLIST / linked dict still later. SYNONYM/ALIAS stay on `SYNONYM-ALIAS.md`. MARKER restore-mark stays on `MARKER.md`.
- Acceptance: Lab smokes `find-demo` (retains `words-demo` + `synonym-demo`).
- Cite: `docs/FIND.md`.

### `docs/SYNONYM-ALIAS.md`

- Companions: add `FIND.md` (wave18 **2**); **keep** WORDS-VOCAB / DEFER-IS / KERNEL / CREATE-DOES cites and wave17 PARSE / EVALUATE / RECURSE closed cites if present (do not wipe).
- Purpose / §3 / non-goals: SYNONYM/ALIAS stay name→name map only; FIND deepen is a sibling ANS-ish find mark — **not** a SYNONYM FIND rewrite / linked XT / execute-through. Do not wipe wave17 SYNONYM content. Host find aliases stay untouched.
- Non-goals: `FIND` deepen → `docs/FIND.md` (wave18 **2**). PARSE / EVALUATE / RECURSE stay wave17 **2–4**. TICK stays wave18 **1**. Linked XT / executing aliased XT still later.
- Acceptance: Lab smokes `find-demo` (retains `synonym-demo`).
- Cite: `docs/FIND.md`.

### `docs/INTERPRET.md`

- Companions: add `FIND.md` (wave18 **2**); **keep** KERNEL / COLON / COMMENT-PARSE / STRING-LIT / IMMEDIATE-POSTPONE / EXIT-QUIT / PARSE-NAME / EVALUATE-INCLUDE cites; light TICK cite welcome.
- Purpose / §2 / §3 / non-goals: interpret loop stays on host `find`/`findentry`/`entry-find` lookup; FIND deepen is a sibling ANS-ish mark via `find-xt` — **does not** rewrite the interpret find path / SEARCH-WORDLIST / execute found XT. Do not wipe wave8–17 INTERPRET content. Host find aliases + `[kernel] find hit/miss` stay.
- Non-goals: `FIND` deepen → `docs/FIND.md` (wave18 **2**). WORD/BL rewrite stays wave18 tip **3**. STATE/COMPILE, stays wave18 tip **4**. Full SEARCH-WORDLIST / linked dict still later.
- Acceptance: Lab smokes `find-demo` (retains `interpret-demo` + `kernel-demo` + `tick-demo` + prior).
- Cite: `docs/FIND.md`.

Do **not** wipe tip1 TICK cites or wave17 tip1–4 SYNONYM / PARSE / EVALUATE / RECURSE cites or wave16 DEFER / MARKER / BUFFER / EXIT cites or wave15 IMMEDIATE / COLON / KERNEL prior content. Do **not** amend TICK.md / PARSE-NAME / EVALUATE-INCLUDE / RECURSE / EXIT-QUIT / DEFER-IS / CREATE-DOES / COLON / DOCS-CITES docs this tip (proposal amends are KERNEL + WORDS-VOCAB + SYNONYM-ALIAS + INTERPRET only).

## 7. Non-goals

- Full linked dict / wordlist-stack runtime rewrite (still out)
- `SEARCH-WORDLIST` thin vocab-search mark → `docs/SEARCH-WORDLIST.md` (wave22 **4**; sibling mark — **not** FIND reopen; host find/find-xt/find-mark untouched; prefer `search-wl-mark`; not full linked dict)
- `WORDLIST` / `FORTH-WORDLIST` thin vocab create/query → `docs/WORDLIST.md` (wave24 **4**; sibling create/query — **not** FIND reopen; prefer `wordlist-mark`/`forth-wordlist-mark`; do not redefine find*/find-xt/find-mark; not SEARCH-WORDLIST runtime / linked dict / DEFINITIONS)
- `ENVIRONMENT?` query mark → `docs/ENVIRONMENT-QUERY.md` (wave19 **3**; thin query mark — not SEARCH-WORDLIST / FIND rewrite / wordlist / linked dict)
- Redefining host `find` / `findentry` / `entry-find` used by `kernel-demo` / interpret
- Executing found XT / calling through stub xt id / execute-through → `docs/EXECUTE.md` (wave20 **4**; Forth mirror `execute-mark` — xt-id invoke **mark only**; not execute-through / real XT execute / linked XT)
- SYNONYM FIND rewrite / entry-body rewrite (SYNONYM stays name→name — wave17 **1**)
- `WORD` / `BL` stubs (wave18 **3**)
- `STATE` / `COMPILE,` deepen (wave18 **4**)
- Docs cites pass (wave18 **5** — ARCHITECTURE + GAPS after 1–4 PASS)
- `'` / `[']` tick reopen (wave18 **1** — already stubbed; keep cites; xt= convention shared)
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
- `2DUP` / `2DROP` / `2SWAP` stub redefinition (host primitives already live — **skip 2DUP-FAMILY**)
- `ABORT"` polish (already optional-wired inside `throw-demo` — **skip**)
- Real crypto / network fleet / opaque-weight ML
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/FIND.md` present (Research byte-copy OK); `KERNEL.md` + `WORDS-VOCAB.md` + `SYNONYM-ALIAS.md` + `INTERPRET.md` thin amends present; tip1 TICK cites, wave17 tip1–4 cites, wave16 DEFER/MARKER/BUFFER/EXIT cites, and wave15 IMMEDIATE/COLON/KERNEL prior text retained; host `find` / `findentry` / `entry-find` untouched via mirrors.
2. `find-demo` → OK (markers §4; `[find] FIND name=` greppable; optional `xt=` / `flag=` / `i=` welcome; optional one `[find] FIND miss`; no host find rewrite); wave19 **3**: `env-demo` → OK (retains `find-demo`); wave20 **4**: `exec-demo` → OK (retains `find-demo` + `tick-demo`; invoke mark only — not execute-through); wave22 **4**: `search-demo` → OK (retains `find-demo` + `tick-demo` + `words-demo`; vocab-search mark only — **not** FIND reopen; host find/find-xt/find-mark untouched); wave24 **4**: `wordlist-demo` → OK (retains `find-demo` + `search-demo` + `words-demo`; thin create/query only — **not** FIND reopen; prefer `wordlist-mark`/`forth-wordlist-mark`). Prior `tick-demo` + `recurse-demo` + `eval-demo` + `parse-demo` + `synonym-demo` + `exit-demo` + `buffer-demo` + `marker-demo` + `defer-demo` + `imm-demo` + `fill-demo` + `pick-demo` + `cell-demo` + `allot-demo` + `throw-demo` + `2var-demo` + `unloop-demo` + `string-demo` + `create-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `colon-demo` + `words-demo` + `kernel-demo` + `refined-boot-demo` + `host-boot-demo` + `interpret-demo` still OK.
3. Regression green (wave18 tip1 + wave17 tip1–5 + wave16 tip1–5 + wave15 tip1–5 + wave14 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `find-demo CONTRACT` OK).
5. No merge. Stay out of Mango. No opaque-weight ML. Skip 2DUP-FAMILY + ABORT" polish.

## 9. Cite

- `docs/KERNEL.md` (wave7 **5**), `docs/WORDS-VOCAB.md` (wave11 **3**), `docs/SYNONYM-ALIAS.md` (wave17 **1**), `docs/INTERPRET.md` (wave8 **1**)
- `docs/TICK.md` (wave18 **1** — stub `xt=` id convention)
- `docs/ENVIRONMENT-QUERY.md` (wave19 **3** — query-mark companion; not SEARCH-WORDLIST / FIND rewrite)
- `docs/EXECUTE.md` (wave20 **4** — invoke-mark companion; not execute-through / real XT execute)
- `docs/SEARCH-WORDLIST.md` (wave22 **4** — sibling vocab-search mark; **not** FIND reopen; host find/find-xt/find-mark untouched; prefer `search-wl-mark`)
- `docs/WORDLIST.md` (wave24 **4** — sibling thin vocab create/query; **not** FIND reopen; prefer `wordlist-mark`/`forth-wordlist-mark`; do not redefine find*/find-xt/find-mark)
- `docs/RECURSE.md` (wave17 **4**), `docs/EVALUATE-INCLUDE.md` (wave17 **3**), `docs/PARSE-NAME.md` (wave17 **2**)
- `docs/MARKER.md` (wave16 **2**), `docs/BUFFER-COLON.md` (wave16 **3**), `docs/EXIT-QUIT.md` (wave16 **4**)
- `docs/DEFER-IS.md` (wave16 **1**), `docs/IMMEDIATE-POSTPONE.md` (wave15 **4**)
- `docs/FILL-MOVE.md` (wave15 **3**), `docs/PICK-ROLL.md` (wave15 **2**), `docs/CELL-CELLS.md` (wave15 **1**)
- `docs/THROW-CATCH.md` (wave14 **4**)
- `forth/tritium/kernel.fs` (host find/findentry/entry-find stay; find-xt / find-mark only)
- ANS Forth `FIND` (name→flag/id mark only — no SEARCH-WORDLIST / linked dict / execute-through / host find rewrite)
- Explicit deferral: WAVE17-PROPOSAL + WAVE18-PROPOSAL (`FIND` deepen — not SEARCH-WORDLIST / host find rewrite)
- Tip1 TICK stub `xt=` = entry index → pairs with this tip without a real XT table
- Base tip: `41e1438` / `41e1438623ffbf4a938e89d8195910de8d0fe9e6` (#82 wave18 tip1 TICK PASS)
- Wave18 proposal: `/workspace/tritium-research-docs/WAVE18-PROPOSAL.md`
