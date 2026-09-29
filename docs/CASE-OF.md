# CASE-OF — `CASE` / `OF` / `ENDOF` / `ENDCASE` stubs + `case-demo`

**Status:** Shipper-ready stub spec (wave13 item **2**)
**Canonical brief:** ANS-shaped `CASE` / `OF` / `ENDOF` / `ENDCASE` (thin stub); `docs/CONTROL.md` (wave10 **2**)
**Sources of truth (code):** `forth/tritium/kernel.fs` (+ optional `control.fs` / `case.fs`); Linux host REPL
**Companions:** `docs/CONTROL.md` (thin amend this tip), `docs/BEGIN-UNTIL.md`, `docs/DO-LOOP.md`, `docs/LEAVE-AGAIN.md`, `docs/VALUE-TO.md` (wave13 **1**)
**Base tip SHA:** `50e0b53` (wave13 tip1 CLOSED / #57 VALUE-TO) / full `50e0b53a2103584f6003b2738711b1adb80ce313`

## 1. Purpose

`IF` / `THEN` / `ELSE` stubs exist (wave10 **2**, cs balance). This tip lands the **balance-only sibling** for multi-way select: stub `CASE` / `OF` / `ENDOF` / `ENDCASE` that recognize the words, maintain a tiny **case** (or shared cs) depth counter, print greppable `[case]` markers, and smoke via **`case-demo`**. Not real OF match, branch XT, or token skip.

## 2. Words / Surfaces

| Word | Stack | Notes |
|------|-------|-------|
| `CASE` / `control-case` | `( n -- )` *or* `( -- )` | Push case frame; optional selector stash (print only); print `[case] CASE depth=<n>` |
| `OF` / `control-of` | `( n -- )` *or* `( -- )` | Requires open CASE; mark OF branch; print `[case] OF` (+ optional `sel=` / `match=` stub — **no** token skip) |
| `ENDOF` / `control-endof` | `( -- )` | Close OF branch (no depth change); print `[case] ENDOF` |
| `ENDCASE` / `control-endcase` | `( -- )` | Pop case frame; print `[case] ENDCASE depth=<n>` |
| `case-cs-depth` | `( -- n )` | Optional; may share / sibling of `control-cs-depth` |
| `case-demo` | `( -- )` | See §5 |

Host note: bind `CASE` / `OF` / `ENDOF` / `ENDCASE` on Linux REPL; Forth mirrors `control-case` / `control-of` / `control-endof` / `control-endcase` if host Forth names collide (host Forth often has real `CASE`/`OF`).

## 3. Stub semantics

**Marker namespace (locked):** all four words print under **`[case]`** (same family as `case-demo`). Do not reuse `[control]` for these markers.

**Depth family (align with CONTROL, do not invent a third unrelated counter unless demos need it):**

- **CASE** +1 case-depth (or shared cs); **ENDCASE** −1.
- **OF** / **ENDOF** require open CASE; **do not** change depth (OF ≈ branch mark; ENDOF ≈ ELSE-shaped closer for that OF — balance only).
- Shared counter with `control-cs-depth` acceptable only if `control-demo` / `case-demo` each still finish at 0.

**Balance only (sibling of IF/ELSE/THEN):**

- Unbalanced `OF` / `ENDOF` / `ENDCASE` (no open CASE) → `[case] FAIL` reason=unbalanced (demo must avoid).
- Optional selector / match print on `OF` if a numeric is on the data stack — **no** requirement to skip tokens between OF…ENDOF or after a “match” this tip (skip can be a no-op print).
- No real forward/back branch patching, no OF equality test that drives control flow, no DROP of selector at ENDCASE beyond what a stub already does for markers.
- Nest with `IF`/`THEN`/`ELSE`, BEGIN/UNTIL, DO/LOOP, LEAVE/AGAIN OK if each depth family is 0 at demo end.
- Still no CREATE/DOES>, string lit, real branch XT; VALUE/TO already landed (wave13 **1**).

## 4. Markers

```
[case] CASE depth=<n>
[case] OF
[case] ENDOF
[case] ENDCASE depth=<n>
[case] FAIL reason=<…>
[case-demo] OK
[case-demo] FAIL
```

Optional suffixes on `OF` (e.g. `sel=` / `match=`) are fine; Lab may ignore them. Lab greps `[case-demo] OK` plus at least one each of `[case] CASE depth=`, `[case] OF`, `[case] ENDOF`, `[case] ENDCASE depth=`.

## 5. `case-demo`

1. Clean slate / `dict-reset` if available (resets case-depth / shared cs used by CASE).
2. Stream A — single OF: `CASE` … `OF` … `ENDOF` … `ENDCASE` (with or without selector push) → all four markers; case-depth 0.
3. Stream B (preferred): two OF arms — `CASE` … `OF` … `ENDOF` … `OF` … `ENDOF` … `ENDCASE` → CASE + ≥2 OF + ≥2 ENDOF + ENDCASE; depth 0.
4. Assert `case-cs-depth` is 0 after (if exposed).
5. Prior `value-demo` / `control-demo` / `leave-demo` / `do-loop-demo` / `comment-demo` / `var-demo` still OK.
6. `[case-demo] OK`.

Stream A is required (all four markers). Stream B preferred so multi-OF balance is greppable. Depth is 0 before `[case-demo] OK`.

## 6. Thin amend — companions

### `docs/CONTROL.md`

- Companions: add `CASE-OF.md` (wave13 **2**).
- Purpose / §3: point `CASE` / `OF` / `ENDOF` / `ENDCASE` stubs at this tip (balance-only sibling of IF/ELSE/THEN; no longer an open GAPS-only bullet for stub surface).
- Non-goals: `CASE` / `OF` / `ENDOF` / `ENDCASE` stubs → `docs/CASE-OF.md` (wave13 **2**); keep real OF match / branch XT / token skip out.
- Cite: `docs/CASE-OF.md`.

## 7. Non-goals

- Real OF match / equality-driven branch / token skip between OF…ENDOF
- Real compile-time branch XT / threaded CASE compiler
- CREATE / DOES> / CREATE-DOES> (wave13 **3** candidate)
- VALUE / TO (done — wave13 **1** / `docs/VALUE-TO.md`)
- String literals `S"` / `."` (wave13 **4** candidate)
- Docs cites pass (wave13 **5**)
- Real crypto / network fleet
- Full Win/Android Forth VM (CONTRACT acceptable)
- No merge. Stay out of Mango.

## 8. Acceptance (Test Lab)

1. `docs/CASE-OF.md` present (Research byte-copy OK); `CONTROL.md` thin amend present.
2. `case-demo` → OK (markers §4); case-depth 0; `value-demo` + `control-demo` + `leave-demo` + `do-loop-demo` + `comment-demo` + `var-demo` still OK.
3. Regression green (wave13 **1** + wave12 demos + prior).
4. Win/Android: CONTRACT acceptable (parity line `case-demo CONTRACT` OK).
5. No merge. Stay out of Mango.

## 9. Cite

- `docs/CONTROL.md` (wave10 **2**), `docs/BEGIN-UNTIL.md`, `docs/DO-LOOP.md`, `docs/LEAVE-AGAIN.md`, `docs/VALUE-TO.md` (wave13 **1**)
- `forth/tritium/kernel.fs`
- ANS Forth `CASE` / `OF` / `ENDOF` / `ENDCASE` (stub only); Dusk control surface (stub only)
- Base tip: `50e0b53` / `50e0b53a2103584f6003b2738711b1adb80ce313`
- Wave13 proposal: `/workspace/tritium-research-docs/WAVE13-PROPOSAL.md`
