# CREATE-DOES — `CREATE` / `DOES>` defining-word stubs + `create-demo`

**Status:** Shipper-ready stub spec (wave13 item **3**)
**Canonical brief:** ANS-shaped `CREATE` / `DOES>` (thin stub); `docs/COLON.md` (wave9 **4**), `docs/VARIABLE-CONST.md` (wave12 **2**), `docs/KERNEL.md` (wave7 **5**)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `create.fs` / `does.fs`); Linux host REPL
**Companions:** `docs/COLON.md` (thin amend this tip), `docs/VARIABLE-CONST.md` (thin amend this tip), `docs/KERNEL.md` (thin amend this tip), `docs/VALUE-TO.md` (wave13 **1**), `docs/CASE-OF.md` (wave13 **2**), `docs/ALLOT-HERE.md` (wave14 **1**)
**Base tip SHA:** `7fd97dd` (wave13 tip2 CLOSED / #58 CASE-OF) / full `7fd97ddada221a0e1cd9929a2c43678b1cb78013`

## 1. Purpose

Colon body markers and named-cell stubs (`VARIABLE`/`CONSTANT`/`VALUE`) exist; defining-word surface does not. This tip lands **stub** `CREATE` / `DOES>`: `CREATE <name>` creates a named dict entry and prints a greppable `[create]` marker; `DOES>` marks a does-body stub on the most recent CREATE (or open defining frame) **without** real XT chaining or child runtime body. Smoke via **`create-demo`**. Dict presence greppable via `WORDS` / `find` / `entry-find`. Not real DOES> threaded child or REKIA emit rewrite. HERE/ALLOT pointer stubs → `docs/ALLOT-HERE.md` (wave14 **1**; full arena still out).

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `CREATE` / `create-entry` | `( "name" -- )` *or* `( -- )` then parse name | Create named dict entry; print `[create] CREATE name=<name>` |
| `DOES>` / `does-mark` | `( -- )` | Mark does-body stub on latest CREATE / defining frame; print `[create] DOES>` (+ optional `name=<name>`); **no** real XT chain / child body |
| `create-find` | `( c-addr u -- i )` | Optional; thin wrapper of `entry-find` for CREATE entries |
| `create-demo` | `( -- )` | See §5 |

Host note: bind `CREATE` / `DOES>` on Linux REPL; Forth mirrors `create-entry` / `does-mark` if host Forth names collide (host Forth often has real `CREATE`/`DOES>`).

## 3. Stub semantics

- **CREATE** → `entry-create` (or host mirror) named entry; dict presence greppable via `WORDS` / `entry-find` / find hit. Print `[create] CREATE name=<name>`. Unlike VARIABLE (init 0) / VALUE (init from TOS) / CONSTANT (immutable stub), CREATE this tip only asserts **presence** — no cell init required; optional host-side flag `create-pending` / `does-body=0` on the entry is fine.
- **DOES>** → mark does-body stub on the most recent CREATE entry (or open defining frame). Print `[create] DOES>` (optional `name=<name>`). **Does not** compile a child XT list, rewrite the CREATE child's runtime, or thread a DOES> body. Optional host flag `does-body=1` / `does-mark` on the entry is enough for greppable presence.
- Missing CREATE before DOES> → `[create] FAIL` reason=unbalanced (demo must avoid).
- Nest with prior colon / VARIABLE / VALUE / control / CASE stubs OK; `dict-reset` clears CREATE stubs along with other entries.
- Still no real XT chaining / REKIA emit rewrite. HERE/ALLOT pointer stubs → `docs/ALLOT-HERE.md` (wave14 **1**); full arena still out. String lit done wave13 **4**.

## 4. Markers

```
[create] CREATE name=<name>
[create] DOES> [name=<name>]     # name= optional
[create] FAIL reason=<…>
[create-demo] OK
[create-demo] FAIL
```

Lab greps `[create-demo] OK` plus at least one `[create] CREATE name=` and one `[create] DOES>`; dict presence via `WORDS` / find hit / entry-count greppable (N ≥ prior + 1 after CREATE).

## 5. `create-demo`

1. Clean slate / `dict-reset` if available.
2. `CREATE widget` (or fixture name) → `[create] CREATE name=widget`; find/WORDS shows `widget`.
3. `DOES>` → `[create] DOES>` (optional `name=widget`).
4. Assert dict presence greppable (find hit / WORDS / entry-count for `widget`).
5. Prior `case-demo` / `value-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `comment-demo` / `var-demo` / `colon-demo` still OK.
6. `[create-demo] OK`.

CREATE + DOES> markers and dict presence are required. Depth/cs families from prior tips stay 0 / untouched.

## 6. Thin amend — companions

### `docs/COLON.md`

- Companions: add `CREATE-DOES.md` (wave13 **3**).
- Non-goals / cite: strike open “still no DOES> / CREATE-DOES>”; point CREATE/DOES> stubs at this tip (defining-word markers only; no real XT child body).
- Cite: `docs/CREATE-DOES.md`.

### `docs/VARIABLE-CONST.md`

- Companions: add `CREATE-DOES.md` (wave13 **3**).
- Purpose / §3: strike open “Still no DOES>”; point CREATE/DOES> stubs at this tip. HERE/ALLOT pointer stubs → `docs/ALLOT-HERE.md` (wave14 **1**); still no full arena / XT chaining.
- Non-goals: DOES> / CREATE-DOES> → `docs/CREATE-DOES.md` (wave13 **3**).
- Cite: `docs/CREATE-DOES.md`.

### `docs/KERNEL.md`

- Companions: add `CREATE-DOES.md` (wave13 **3**).
- Words table: add `CREATE` / `DOES>` stubs + `create-demo` (cite tip; no full defining-word / memory model).
- Non-goals: strike open DOES> from “still later”; point to `docs/CREATE-DOES.md`. HERE/ALLOT pointer stubs → `docs/ALLOT-HERE.md` (wave14 **1**); full arena / XT chaining still later.
- Acceptance: Lab smokes `create-demo`.
- Cite: `docs/CREATE-DOES.md`.

## 7. Non-goals

- Real DOES> XT chaining / threaded child runtime body
- Full ALLOT / HERE arena / pool / free / linked cell memory (pointer stubs → `docs/ALLOT-HERE.md` wave14 **1**)
- REKIA emit rewrite (FORTH-BASE-REFERENCES models CREATE-DOES> as emit target — stub surface only this tip)
- String literals `S"` / `."` (wave13 **4** candidate)
- Docs cites pass (wave13 **5**)
- VALUE / TO (done — wave13 **1**); CASE / OF / ENDOF / ENDCASE (done — wave13 **2**)
- Real crypto / network fleet
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/CREATE-DOES.md` present (Research byte-copy OK); `COLON.md` + `VARIABLE-CONST.md` + `KERNEL.md` thin amends present.
2. `create-demo` → OK (markers §4); dict presence greppable; `case-demo` + `value-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `comment-demo` + `var-demo` + `colon-demo` still OK.
3. Regression green (wave13 **1–2** + wave12 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `create-demo CONTRACT` OK).
5. No merge. Stay out of Mango.

## 9. Cite

- `docs/COLON.md` (wave9 **4**), `docs/VARIABLE-CONST.md` (wave12 **2**), `docs/KERNEL.md` (wave7 **5**), `docs/VALUE-TO.md` (wave13 **1**), `docs/CASE-OF.md` (wave13 **2**), `docs/ALLOT-HERE.md` (wave14 **1**)
- `forth/tritium/kernel.fs`
- ANS Forth `CREATE` / `DOES>` (stub only); Dusk defining-word / REKIA emit model (stub surface only — see `docs/FORTH-BASE-REFERENCES.md`)
- Base tip: `7fd97dd` / `7fd97ddada221a0e1cd9929a2c43678b1cb78013`
- Wave13 proposal: `/workspace/tritium-research-docs/WAVE13-PROPOSAL.md`
