# TRIT-MATH — Phase 2 trit ops + pack (demo)

**Status:** Shipper-ready stub spec (wave9 item **1**)  
**Canonical brief:** `TritiumOS.txt` §3, Phase 2  
**Sources of truth (code):** `forth/trit.fs` (+ Linux host / `tools/` demo mirrors)  
**Companions:** `docs/NEURON.md` (encoding SoT), `docs/DRENA.md`

## 1. Purpose

`forth/trit.fs` already defines Phase 2 vocabulary. This tip **locks the Lab smoke surface**: greppable `trit-math-demo` exercising `trit+` / `trit*` / `pack-neuron-header` (+ unpack round-trip). Docs tip only where code already matches; **align `trit+` to clamp** if the tip still wraps ±2 (see §3).

Not a full neuron/DRENA rewrite. Not Win/Android Forth VM.

## 2. Words (contract)

| Word | Stack | Contract |
|------|-------|----------|
| `trit+` | `( t1 t2 -- t3 )` | Sum **clamped** into `{-1,0,+1}` (saturate; not wrap). Examples: `1 1 trit+` → `1`; `-1 -1 trit+` → `-1`; `1 -1 trit+` → `0` |
| `trit*` | `( t1 t2 -- t3 )` | Product (always in trit range). Examples: `1 -1 trit*` → `-1`; `-1 -1 trit*` → `1` |
| `pack-neuron-header` | `( s0 s1 s2 s3 -- h )` | Alias of `pack-header`; 4×4-bit → 16-bit |
| `unpack-header` | `( h -- s0 s1 s2 s3 )` | Inverse |
| `trit-math-demo` | `( -- )` | See §4 |

Nibbles / decode / encode stay as in `NEURON.md`; this tip does not change packing policy.

## 3. `trit+` note for Shipper

If tip `forth/trit.fs` maps `2→-1` / `-2→1` (wrap), **change to clamp** for this tip so Lab asserts match `NEURON.md` + §2. Prefer one behavior only; record any exception in `ASSUMPTIONS.md` (none expected).

## 4. Markers

```
[trit-math] trit+ ok
[trit-math] trit* ok
[trit-math] pack-roundtrip ok
[trit-math-demo] OK
[trit-math-demo] FAIL
```

Optional detail lines (`[trit-math] 1 1 + => 1`) OK if greppable; FAIL must print on first failed assert.

## 5. `trit-math-demo`

1. Assert `trit+` cases: `(1,1)→1`, `(-1,-1)→-1`, `(1,-1)→0`, `(0,1)→1`.
2. Assert `trit*` cases: `(1,-1)→-1`, `(-1,-1)→1`, `(0,1)→0`.
3. Pack known nibbles `s0..s3` → `pack-neuron-header` → `unpack-header` → equal.
4. Print the three `ok` markers + `[trit-math-demo] OK`.

Surfaces: Forth word and/or Linux host / `tools/trit-math-demo` (match prior demo pattern). Prior demos stay green.

## 6. Non-goals

- S3 ADDRESS_FOLD / φ deepen (later)
- DRENA/REKIA bugfix beyond what demo needs
- Host C#/Kotlin VM embed
- Economy / assistant-state / colon (wave9 **2–4**)

## 7. Acceptance (Test Lab)

1. `docs/TRIT-MATH.md` present (Research byte-copy OK); `NEURON.md` may one-line cite this tip.
2. `trit-math-demo` → OK (markers above).
3. Regression green (interpret / kernel / queue / assimilate / …).
4. No merge.

## 8. Cite

- `TritiumOS.txt` §3, Phase 2
- `docs/NEURON.md`, `forth/trit.fs`
