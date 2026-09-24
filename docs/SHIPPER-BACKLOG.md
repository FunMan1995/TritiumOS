# Shipper backlog

Order for the next builds. Item 1 is in this tree. Later items are not.

1. **Header roundtrip** — `unpack-header` and `header>s3` match `docs/NEURON.md`. Smoke: `python3 tools/smoke-neuron.py`.
2. **`set-s3-mode`** — after a correct unpack, write the new mode back and leave the neuron address. Must not change S0–S2. Must not advance mode 3 (RESERVED).
3. **`rekiA-extract` stack** — `( n-addr -- s0 s1 s2 influence bias )` with influence = link count and bias = sum of connected ids. Fix before any contract change.
4. **`rekiA-contract`** — keep influence for `contract-trit`. Stop early when S0–S2 stop changing. Bound the loop.
5. **`rekiA-to-forth`** — decode each nibble with `trit-pair@` before summing. Print one colon definition. Do not `include` it from the engine; the host does that.
6. **Host boot** — Win/Android/Linux load `forth/trit.fs`, `forth/tritium/drena.fs`, `forth/tritium/rekia.fs` and run the smoke words. Stubs in `forth/drena/stub.fs` and `forth/rekia/stub.fs` stay out of that path.

Out of this slice: license slots, queue, assimilate, `lineos-graduate`.
