# Neuron header (S0–S3)

Canonical layout for the 16-bit neuron header in `TritiumOS.txt` §3, as implemented by `forth/trit.fs` and the record words in `forth/tritium/drena.fs`.

Field order is **low nibble = S0**. `pack-header ( s0 s1 s2 s3 -- h )` places S0 in bits 3–0 and S3 in bits 15–12.

## Trit pair (S0, S1, S2)

Each of S0, S1, and S2 is one 4-bit nibble holding a trit pair. Trits are `-1`, `0`, `+1`.

```
t_lo = decode-trit( n mod 3 )     \ 0→-1, 1→0, 2→+1
t_hi = decode-trit( (n / 3) mod 3 )
```

`encode-trit` is the inverse on a single trit: `-1→0`, `0→1`, `+1→2`. `trit-pair>nibble ( tlo thi -- nib )` is `hi * 3 + lo` in that encoding. Nibbles 9–15 collapse through `mod 3`; they are not a second packing.

## S3 mode

S3 is not a trit pair. `s3-mode ( s3 -- m )` keeps the low 2 bits:

| Code | Word | Meaning |
| --- | --- | --- |
| 0 | `random-mode` | RANDOM |
| 1 | `fold-mode` | ADDRESS_FOLD |
| 2 | `connected-mode` | CONNECTED |
| 3 | `reserved-mode` | RESERVED — do not invent a behavior |

High 2 bits of S3 are spare. Progression intent is RANDOM → ADDRESS_FOLD → CONNECTED. RESERVED does not advance.

## Record on this tree

`make-neuron` stores cells, not the 2-byte fields in the spec:

| Cell | Word | Contents |
| --- | --- | --- |
| 0 | `neuron-header` | packed header |
| 1 | `neuron-id` | id from `next-id` |
| 2 | `neuron-link-count` | number of following ids |
| 3+ | `neuron-links-base` | connected ids, one cell each |

`header>s0` and `header>s3` read one section. `header>mode` is `header>s3 s3-mode`.

## Roundtrip

`0 1 2 3 pack-header` is `$3210`. `unpack-header` returns `0 1 2 3`. Smoke: `python3 tools/smoke-neuron.py`.

`set-s3-mode` still mishandles the stack after the unpack. Do not treat a rewire as done until that word roundtrips under the smoke.
