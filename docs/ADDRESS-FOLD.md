# ADDRESS-FOLD — Deepen `phi-fold` / `fold-target`

**Status:** Shipper-ready stub spec (wave10 item **3**)
**Canonical brief:** `TritiumOS.txt` §3 (S3 ADDRESS_FOLD); `docs/DRENA.md` / `docs/NEURON.md`
**Sources of truth (code):** `forth/tritium/drena.fs` (`phi-fold`, `fold-target`, `drena-link`); poly mirror
**Companions:** `docs/DRENA.md` (thin amend this tip), `docs/NEURON.md` (thin amend), `docs/TRIT-MATH.md`

## 1. Purpose

ADDRESS_FOLD already remaps link targets via a thin `phi-fold` mix. This tip **deepens** the fold: document the algorithm, add optional multi-round / influence markers, expose helpers for Lab, and smoke with **`fold-demo`** against fixed golden vectors. Not a neural trainer; pure deterministic integer mix.

## 2. Algorithm (keep compatible)

Current SoT (must stay greppable / behavior-compatible unless Shipper bumps demo vectors):

```
phi-fold ( addr addr' -- influence )
  xor → x ^= (x << 13) → x ^= (x >> 7) → x &= $7fff

fold-target ( src-id candidate -- target )
  2dup phi-fold nip 1 max
```

### 2.1 Deepen (this tip)

| Addition | Notes |
|----------|-------|
| `phi-fold-rounds` | Optional cell (default **1**); if >1, re-apply mix rounds−1 times on influence before mask |
| `phi-fold3` | Optional 3-arg stub `( a b c -- influence )` = `phi-fold(phi-fold(a,b),c)` — for later multi-hop |
| `last-fold-influence` / `last-fold-target` | Variables updated by `fold-target` (or wrappers) for demos |
| Markers | See §4 |

Default rounds=1 preserves existing `drena-link` / grow-step behavior. Changing default mix bits requires updating golden vectors in §5.

## 3. Words

| Word | Stack | Notes |
|------|-------|-------|
| `phi-fold` | `( addr addr' -- influence )` | As §2 (+ optional rounds) |
| `fold-target` | `( src-id candidate -- target )` | Deterministic; **target ≥ 1** |
| `phi-fold-rounds` | `( -- addr )` | Variable; default 1 |
| `fold-demo` | `( -- )` | See §5 |

`drena-link` with S3 mode=ADDRESS_FOLD continues to call `fold-target` and may print existing `[DRENA] ADDRESS_FOLD …` plus new `[fold]` lines.

## 4. Markers

```
[fold] phi a=<n> b=<n> influence=<n>
[fold] target src=<n> cand=<n> -> <n>
[fold-demo] OK
[fold-demo] FAIL
```

Lab greps `[fold-demo] OK`. Optional: keep `[DRENA] ADDRESS_FOLD` from link path.

## 5. Golden vectors (`fold-demo`)

Assert exact integers (rounds=1, current mix):

| src | cand | influence (`phi-fold`) | target (`fold-target`) |
|-----|------|------------------------|------------------------|
| 1 | 2 | 24771 | 24771 |
| 7 | 11 | 780 | 780 |
| 100 | 100 | 0 | 1 |

Procedure:

1. Set `phi-fold-rounds` = 1 (if exposed).
2. For each row: run `phi-fold` / `fold-target`; print `[fold]` markers; abort FAIL on mismatch.
3. Optional: spawn ADDRESS_FOLD neuron + `drena-link` → folded target greppable.
4. Prior `grow-step-demo` / `groups-demo` / `control-demo` still OK.
5. `[fold-demo] OK`.

**Shipper note:** goldens above match SoT `phi-fold` / `fold-target` at tip time (rounds=1). Recompute if mix bits change.

## 6. Thin amend — `docs/DRENA.md` / `docs/NEURON.md`

**DRENA.md**

- Companions: add `ADDRESS-FOLD.md`.
- §3 `phi-fold` / `fold-target` rows: point to deepen + `fold-demo`.
- Acceptance: Lab smokes `fold-demo`.

**NEURON.md**

- §4.1: cite `docs/ADDRESS-FOLD.md` (wave10 **3**); mention markers / rounds.

## 7. Non-goals

- Changing S3 progression or RESERVED policy
- Real φ / float math / crypto hash
- Edition 32/64 address-width wiring (still GAPS)
- Refined-boot / docs cites (wave10 **4–5**)

## 8. Acceptance (Test Lab)

1. `docs/ADDRESS-FOLD.md` present (Research byte-copy OK); DRENA + NEURON thin amends present.
2. `fold-demo` → OK (markers §4 + pinned goldens); `grow-step-demo` still OK.
3. Regression green (wave10 **1–2** + wave9 demos).
4. No merge.

## 9. Cite

- `docs/DRENA.md`, `docs/NEURON.md`, `docs/TRIT-MATH.md`
- `forth/tritium/drena.fs` / `tritium.poly/core/drena.fs`
- `TritiumOS.txt` §3 ADDRESS_FOLD
