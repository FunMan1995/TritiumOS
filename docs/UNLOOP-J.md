# UNLOOP-J — `UNLOOP` / `J` do-loop siblings + `unloop-demo`

**Status:** Shipper-ready stub spec (wave14 item **2**)
**Canonical brief:** ANS-shaped `UNLOOP` / `J` (thin stub); `docs/DO-LOOP.md` (wave12 **1**), `docs/LEAVE-AGAIN.md` (wave12 **4**)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `control.fs` / `loop.fs`); Linux host REPL
**Companions:** `docs/DO-LOOP.md` (thin amend this tip), `docs/LEAVE-AGAIN.md` (thin amend this tip), `docs/BEGIN-UNTIL.md`, `docs/CONTROL.md`, `docs/ALLOT-HERE.md` (wave14 **1**)
**Base tip SHA:** `3f30adb` (wave14 tip1 CLOSED / #62 ALLOT-HERE) / full `3f30adbb74bdc1ddcf1bd2ef7bf4e1638339c0b5`

## 1. Purpose

`DO` / `LOOP` / `+LOOP` / `I` (do-depth) and `LEAVE` / `AGAIN` stubs exist. This tip lands the **do-loop sibling** pair deferred by WAVE13 → wave14:

- `UNLOOP` — pop / mark one open DO frame **without** `LOOP` / `+LOOP` closer semantics (no step, no body re-exec).
- `J` — print an **outer-index** stub when ≥2 DO frames are open (sibling of `I` for the enclosing loop).

Print greppable `[do-loop]` markers and smoke via **`unloop-demo`**. Not a real LEAVE jump, branch XT, nested RS walk, or runtime counted re-exec. Explicit WAVE13-PROPOSAL deferral (“candidate wave14”).

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `UNLOOP` / `control-unloop` | `( -- )` | Requires open DO; **pop** one do-frame (−1 do-depth); print `[do-loop] UNLOOP depth=<n>` (post-pop) |
| `J` / `control-j` | `( -- )` *or* `( -- n )` | Requires ≥2 open DO frames; print outer-index stub `[do-loop] J index=<j>`; optional push of stub index |
| `unloop-demo` | `( -- )` | See §5 |

Host note: bind `UNLOOP` / `J` on Linux REPL; Forth mirrors `control-unloop` / `control-j` if host Forth names collide (host Forth often has real `UNLOOP`/`J`).

## 3. Stub semantics

**Marker namespace (locked):** both words print under **`[do-loop]`** (same family as `DO` / `LOOP` / `+LOOP` / `I` / `do-loop-demo`). Do not invent a separate `[unloop]` marker family for the word lines — only the demo banner is `[unloop-demo]`.

**Depth family (align with DO-LOOP, do not invent a third counter):**

- DO do-depth = existing `DO-LOOP` counter (`do-loop-depth`). `DO` +1; `LOOP` / `+LOOP` / **`UNLOOP`** −1.
- `J` does **not** change depth (read-only outer-index stub, like `I` for the enclosing frame).
- Shared counter with BEGIN loop-depth acceptable only if `loop-demo` / `do-loop-demo` / `leave-demo` / `unloop-demo` each still finish at 0.

**`UNLOOP`:**

- Legal only when do-loop depth > 0. Else `[do-loop] FAIL` reason=unbalanced (demo must avoid).
- Pop one DO frame (−1 do-depth). Print **post-pop** depth: `[do-loop] UNLOOP depth=<n>`.
- **Does not** compile or execute a branch XT, skip body tokens, re-exec, or perform `LOOP`/`+LOOP` step semantics. Closes the stub frame the way `LOOP` does for depth only — without the closer’s step / re-exec shape.
- Distinct from `LEAVE`: `LEAVE` **marks** and does **not** pop; `UNLOOP` **pops**. An open BEGIN with no DO is **not** enough for `UNLOOP` (BEGIN → use `AGAIN` / `UNTIL`).

**`J`:**

- Legal only when do-loop depth ≥ 2. Else `[do-loop] FAIL` reason=no-outer (or unbalanced); demo must avoid.
- Outer-index stub may be a fixed `0`, the optional outer start value, or a host-held counter for the enclosing frame — print it; no requirement to walk a real return stack beyond stub depth this tip.
- Optional push of stub index if host stack easy. Does not change do-depth.

Nest with `IF`/`THEN`/`ELSE`, BEGIN/UNTIL, DO/LOOP, LEAVE/AGAIN, CASE/OF OK if each depth family is 0 at demo end.

Still no real branch XT / LEAVE jump / runtime counted re-exec; ALLOT arena; THROW; 2VARIABLE; full Win/Android Forth VM — those → later tips / non-goals.

## 4. Markers

```
[do-loop] UNLOOP depth=<n>     # post-pop do-depth
[do-loop] J index=<j>
[do-loop] FAIL reason=<…>      # UNLOOP with no open DO; J with depth < 2
[unloop-demo] OK
[unloop-demo] FAIL
```

(Existing `[do-loop] DO` / `I` / `LOOP` / `+LOOP` markers from `docs/DO-LOOP.md` remain unchanged and appear in the demo streams.)

Lab greps `[unloop-demo] OK` plus at least one `[do-loop] UNLOOP depth=` and one `[do-loop] J index=`.

## 5. `unloop-demo`

1. Clean slate / `dict-reset` if available (resets do-loop depth).
2. Stream A — UNLOOP pops without LOOP: `DO` … `UNLOOP` → `[do-loop] DO`, `[do-loop] UNLOOP depth=0`; do-depth 0. No `LOOP`/`+LOOP`.
3. Stream B — nested DO + J: `DO` … `DO` … `J` … `LOOP` … `LOOP` (or `UNLOOP` … `UNLOOP`) → DO + DO + `J index=` + closers; do-depth 0. Prefer `LOOP` closers so J is greppable with classic counted-loop balance; `UNLOOP` closers also OK if depth ends at 0.
4. Prior `allot-demo` / `leave-demo` / `do-loop-demo` / `loop-demo` / `case-demo` / `value-demo` / `var-demo` / `comment-demo` / `control-demo` / `create-demo` / `string-demo` / `colon-demo` still OK.
5. `[unloop-demo] OK`.

Stream A is required (UNLOOP). Stream B is required (J with ≥2 open DO). Depth is 0 before `[unloop-demo] OK`.

## 6. Thin amend — companions

### `docs/DO-LOOP.md`

- Companions: add `UNLOOP-J.md` (wave14 **2**).
- Purpose / §3: point `UNLOOP` / `J` stubs at this tip (no longer an open later-wave / WAVE13 deferral bullet). `UNLOOP` pops do-depth (unlike `LEAVE`, which only marks); `J` is outer-index when depth ≥ 2.
- Markers note: `UNLOOP` / `J` reuse `[do-loop]` — `[do-loop] UNLOOP depth=<n>`, `[do-loop] J index=<j>` — cite `docs/UNLOOP-J.md`. Do not add them to the `do-loop-demo` required grep.
- Non-goals: `UNLOOP` / `J` stubs → `docs/UNLOOP-J.md` (wave14 **2**); keep real branch XT / LEAVE jump / runtime counted re-exec out.
- Cite: `docs/UNLOOP-J.md`.

### `docs/LEAVE-AGAIN.md`

- Companions: add `UNLOOP-J.md` (wave14 **2**).
- Purpose / §3: contrast `LEAVE` (mark, no pop) vs `UNLOOP` (pop do-frame without LOOP step) — point UNLOOP/J at this tip. `AGAIN` remains BEGIN-only closer.
- Non-goals: `UNLOOP` / `J` → `docs/UNLOOP-J.md` (wave14 **2**); keep real LEAVE jump / branch XT out.
- Cite: `docs/UNLOOP-J.md`.

## 7. Non-goals

- Real LEAVE jump / branch XT / token skip / runtime counted re-exec
- Nested return-stack walk beyond stub depth (J = host-held outer stub only)
- Full ALLOT arena / pool / free (pointer stubs done tip1 — `docs/ALLOT-HERE.md`)
- 2VARIABLE / 2CONSTANT (wave14 **3** candidate)
- THROW / CATCH / ABORT" (wave14 **4** candidate)
- Docs cites pass (wave14 **5**)
- Real crypto / network fleet
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/UNLOOP-J.md` present (Research byte-copy OK); `DO-LOOP.md` + `LEAVE-AGAIN.md` thin amends present.
2. `unloop-demo` → OK (markers §4); do-depth 0; prior `allot-demo` + `leave-demo` + `do-loop-demo` + `loop-demo` + `case-demo` + `value-demo` + `var-demo` + `comment-demo` + `control-demo` + `create-demo` + `string-demo` + `colon-demo` still OK.
3. Regression green (wave14 **1** + wave13 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `unloop-demo CONTRACT` OK).
5. No merge. Stay out of Mango.

## 9. Cite

- `docs/DO-LOOP.md` (wave12 **1**), `docs/LEAVE-AGAIN.md` (wave12 **4**), `docs/BEGIN-UNTIL.md` (wave11 **1**), `docs/CONTROL.md` (wave10 **2**), `docs/ALLOT-HERE.md` (wave14 **1**)
- `forth/tritium/kernel.fs`
- ANS Forth `UNLOOP` / `J` (stub only); Dusk counted-loop siblings (stub only)
- Base tip: `3f30adb` / `3f30adbb74bdc1ddcf1bd2ef7bf4e1638339c0b5`
- Wave14 proposal: `/workspace/tritium-research-docs/WAVE14-PROPOSAL.md`
- WAVE13-PROPOSAL deferral: `UNLOOP` / `J` stubs (candidate wave14)
