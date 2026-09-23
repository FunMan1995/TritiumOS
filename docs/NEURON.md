# Neuron encoding — TritiumOS

**Status:** Shipper-ready spec (matches PR #2 tip / Research backlog)  
**Canonical brief:** `TritiumOS.txt` §3  
**Sources of truth (code):** `forth/trit.fs`, `forth/tritium/drena.fs`  
**Policy:** S3 mode `11` (RESERVED) — see `docs/ASSUMPTIONS.md`; do not invent behavior.

## 1. Purpose

Each neuron begins with a fixed **16-bit header** (four 4-bit sections S0..S3). D.R.E.N.A. allocates the record and graph links; R.E.K.I.A. reads the header + links to refine intelligence into Forth.

## 2. Header layout

Sections packed low→high into a 16-bit cell via `pack-neuron-header` / `pack-header`:

| Section | Bits | Content |
|---------|------|---------|
| S0 | 4 | trit pair A = (t0, t1), each ti ∈ {-1, 0, +1} |
| S1 | 4 | trit pair B = (t2, t3) |
| S2 | 4 | trit pair C = (t4, t5) |
| S3 | 4 | wiring / variation (see §4); **low 2 bits = mode** |

### 2.1 Nibble → trit pair (`trit-pair@`)

Canonical mod-3 split (`TritiumOS.txt` §3.1):

```
t_lo = decode-trit( n mod 3 )           ; lookup {0→-1, 1→0, 2→+1}
t_hi = decode-trit( (n / 3) mod 3 )
```

Inverse: `trit-pair>nibble` via `encode-trit` (-1→0, 0→1, +1→2).

Dense 2+2 packing is **not** the default; if ever chosen, record it in `docs/ASSUMPTIONS.md` and keep one packing only.

## 3. Trit arithmetic stack effects

| Word | Stack | Notes |
|------|-------|-------|
| `decode-trit` | `( n -- t )` | n mod 3 → trit |
| `encode-trit` | `( t -- n )` | -1→0, 0→1, +1→2 |
| `trit+` | `( t1 t2 -- t3 )` | clamp sum into {-1,0,+1} |
| `trit*` | `( t1 t2 -- t3 )` | product (already in trit range) |
| `trit-pair@` | `( nibble -- tlo thi )` | |
| `trit-pair>nibble` | `( tlo thi -- nib )` | |
| `pack-header` | `( s0 s1 s2 s3 -- h )` | 16-bit header |
| `pack-neuron-header` | `( s0 s1 s2 s3 -- h )` | **alias** → `pack-header` |
| `unpack-header` | `( h -- s0 s1 s2 s3 )` | |
| `s3-mode` | `( s3 -- m )` | `3 and` → mode 0..3 |
| `.trit` | `( t -- )` | print |

## 4. S3 modes (wiring policy)

| Mode | Code | Name | Behavior |
|------|------|------|----------|
| 0 | `00` | RANDOM | Seed / early life; no address fold yet |
| 1 | `01` | ADDRESS_FOLD | Target selection via φ(addr, addr') |
| 2 | `10` | CONNECTED | Use explicit linked addresses as-is |
| 3 | `11` | RESERVED | **Leave alone** — no product invent |

Progression (D.R.E.N.A.): `RANDOM → ADDRESS_FOLD → CONNECTED` via `drena-rewire`. Never advance into or out of RESERVED automatically.

### 4.1 φ fold (ADDRESS_FOLD)

| Word | Stack | Notes |
|------|-------|-------|
| `phi-fold` | `( addr addr' -- influence )` | Pure-math mix |
| `fold-target` | `( src-id candidate -- target )` | Deterministic remap, target ≥ 1 |

When `drena-link` runs with source S3 mode = ADDRESS_FOLD, it prints  
`[DRENA] ADDRESS_FOLD φ(src,dst)->target` and stores the folded target.

## 5. Neuron record (current Forth layout)

Cell-based (32/64-friendly); edition width for ids is future work.

| Offset | Field | Accessor |
|--------|-------|----------|
| 0 | packed header | `neuron-header` `( n-addr -- h )` |
| 1 cell | neuron id | `neuron-id` |
| 2 cells | link-count | `neuron-link-count` |
| 3+ cells | connected ids | `neuron-links-base` |

Validation: `valid-header?`, `valid-neuron?`, `validate-neuron`.

## 6. D.R.E.N.A. word stack effects (neuron ops)

| Word | Stack | Notes |
|------|-------|-------|
| `drena-spawn` | `( variation -- neuron )` | variation = initial S3 mode; sets `last-grown` |
| `drena-grow` | `( parent -- child )` | child inherits parent S3; link parent→child; never rewire / never advance RESERVED |
| `drena-step` | `( -- )` | one tick: no neuron→spawn0; RESERVED→skip; CONNECTED(mode≥2)→grow; else rewire `last-grown` |
| `drena-link` | `( src-neuron dst-id -- )` | fold if mode=1 |
| `drena-rewire` | `( neuron -- )` | writes advanced S3 into header |
| `set-s3-mode` | `( mode n-addr -- )` | packs via `pack-neuron-header` |
| `neuron-add-connection` | `( connected-id n-addr -- )` | append link |
| `make-neuron` | `( id mode -- n-addr )` | allocate at HERE |
| `drena-group` | `( label-addr -- group-id )` | labeled neural group |
| `drena-join` | `( neuron group -- )` | assign neuron to group |

## 7. Acceptance (Test Lab)

1. `pack-neuron-header` / `unpack-header` round-trip preserves S0..S3.
2. `drena-rewire` **writes** S3 (not print-only): mode advances 0→1→2 and stops.
3. On mode 1, `drena-link` emits a φ line and stores folded target.
4. S3=`11` never invented: `drena-rewire` leaves RESERVED unchanged.
5. Core boot still loads `trit` → kernel → `drena` → `rekia` (see `docs/REKIA.md`).

## 8. Follow-ons (not this tip)

- Typed link records with trit-weight (`TritiumOS.txt` §3.5)
- Labeled groups + `group-label!` / `GROUP-<label>/` namespaces
- 32 vs 64 edition id width
- `docs/ASSUMPTIONS.md` if packing or S3=`11` is ever decided
