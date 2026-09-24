# R.E.K.I.A.

Refiner Math Engine: pure-math refinement of a D.R.E.N.A. neuron into Forth text. Source: `forth/tritium/rekia.fs`. Spec pipeline: `TritiumOS.txt` §4. Load after `forth/trit.fs` and `forth/tritium/drena.fs`. The print-only copies in `forth/rekia/stub.fs` are not this engine.

## Pipeline

`rekiA-refine ( neuron-addr -- )` does:

1. `validate-neuron`
2. `rekiA-extract` — S0 S1 S2, link-count as influence, sum of connected ids as bias
3. drop the bias
4. `rekiA-contract` — bounded loop, currently drops influence before the step
5. `rekiA-to-forth` — prints `: refined-<id> ( -- n ) <sum> ;`
6. `rekiA-label-group` — `stable-core`, `positive-flow`, or `negative-drift` from the trit sum

The host is supposed to capture that print into `evolve/forth/refined/<label>.fs` and `include` it. No host does that yet.

## Math that is pure

- `trit-abs`, `trit-sign`
- `majority-trit ( t1 t2 t3 -- t )` — sign of the sum, zero stays zero
- `contract-trit ( t influence -- t' )` — influence `> 0` steps a non-zero trit toward 0 by 1; zero stays; influence `<= 0` leaves the trit
- `contract-nibble` — contract both trits of a nibble and re-encode

S3=RESERVED must not be given a refine rule here.

## Stack bugs (do not paper over)

These are why a refine smoke is not green yet. See `docs/SHIPPER-BACKLOG.md`.

- `rekiA-extract` interleaves `neuron-link-count` and the link walk on a stack that still holds S0 S1 S2. The influence and bias are not reliable.
- `rekiA-contract` drops influence, then calls `rekiA-one-step`, which expects `( s0 s1 s2 influence -- )`.
- `rekiA-to-forth` runs `decode-trit` on nibbles. `decode-trit` expects a residue `0..2`, not a nibble. The printed sum is not the trit sum until the word decodes pairs first.
- `rekiA-label-group` is documented as returning a string. The engine leaves a counted-string-shaped print, not a standard `( c-addr u )`.

## What Test Lab can run today

`python3 tools/smoke-neuron.py` checks the header roundtrip `docs/NEURON.md` requires. It does not execute `rekiA-refine`.
