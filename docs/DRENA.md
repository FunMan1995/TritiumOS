# D.R.E.N.A. — Dynamic Recursive Evolving Neural Architecture

**Status:** Shipper-ready spec (matches PR #10 tip / Research item 5)  
**Canonical brief:** `TritiumOS.txt` §3, §5  
**Sources of truth (code):** `forth/tritium/drena.fs`  
**Companions:** `docs/NEURON.md` (header / trit / S3), `docs/REKIA.md` (refine → Forth), `docs/ASSUMPTIONS.md` (S3=`11`), `docs/QWANTUM-REKIA.md` (K atoms)

## 1. Purpose

D.R.E.N.A. owns **structure and topology**: neuron allocation, S3 wiring progression, typed links, labeled groups, grow/step evolution, and graph persist. It does **not** emit refined Forth — that is R.E.K.I.A.

## 2. Neuron record

Cell-based layout (see also `NEURON.md` §5):

| Offset | Field | Accessor |
|--------|-------|----------|
| 0 | packed 16-bit header (S0..S3) | `neuron-header` |
| 1 cell | neuron id | `neuron-id` |
| 2 cells | embedded link-count | `neuron-link-count` |
| 3+ cells | connected ids | `neuron-links-base` |

| Word | Stack | Notes |
|------|-------|-------|
| `make-neuron` | `( id mode -- n-addr )` | allocate at HERE |
| `set-s3-mode` | `( mode n-addr -- )` | rewrite header via `pack-neuron-header` |
| `validate-neuron` | `( n-addr -- )` | abort if invalid |
| `header>mode` | `( h -- m )` | S3 low 2 bits |

## 3. S3 modes & RESERVED

| Mode | Name | D.R.E.N.A. behavior |
|------|------|---------------------|
| 0 | RANDOM | Early life; `drena-link` may stamp LINK-RANDOM |
| 1 | ADDRESS_FOLD | `drena-link` picks target via `phi-fold` / `fold-target` |
| 2 | CONNECTED | Links use dst as-is |
| 3 | RESERVED | **Never auto-advanced** (`ASSUMPTIONS.md`) |

| Word | Stack | Notes |
|------|-------|-------|
| `phi-fold` | `( addr addr' -- influence )` | Pure-math mix |
| `fold-target` | `( src-id candidate -- target )` | Deterministic, target ≥ 1 |
| `drena-rewire` | `( neuron -- )` | 0→1→2 only; skip if RESERVED or already CONNECTED |

## 4. Spawn / link / grow / step

| Word | Stack | Notes |
|------|-------|-------|
| `drena-spawn` | `( variation -- neuron )` | variation = initial S3 mode; sets `last-grown` |
| `drena-link` | `( src-neuron dst-id -- )` | Embedded connection + typed `link!`; φ when mode=1 |
| `drena-grow` | `( parent -- child )` | Child **inherits** parent S3; links parent→child; never rewires |
| `drena-step` | `( -- )` | Scheduler tick: no neuron → spawn 0; RESERVED → skip; mode≥2 → grow; else rewire |

Smoke: `grow-step-demo` — RESERVED parent → RESERVED child; `drena-step` skips RESERVED.

## 5. Typed link records (`TritiumOS.txt` §3.5)

Global link table (separate from embedded connection list):

| Field | Accessor |
|-------|----------|
| src | `link-src` |
| dst | `link-dst` |
| type | `link-type@` (intra / inter / fold-φ / random) |
| w_lo, w_hi | `link-w-lo` / `link-w-hi` (trit-weight pair) |
| s3 at form | `link-s3@` |

| Word | Stack | Notes |
|------|-------|-------|
| `link!` | `( src dst type w -- )` | Append; w often neutral nibble encoding |
| `link@` | `( idx -- rec )` | |
| `links-for-neuron` | `( neuron -- addr count )` | |

## 6. Labeled groups

| Word | Stack | Notes |
|------|-------|-------|
| `drena-group` | `( label-addr -- group-id )` | Counted string → new group |
| `group-label!` | `( c-addr u gid -- )` | Set label$; builds vocab prefix |
| `drena-join` | `( neuron group -- )` | Persist member id (no dups; capacity-checked) |
| `group-members` | `( gid -- addr count )` | |
| `group-vocab-prefix` | `( gid -- c-addr u )` | `GROUP-<label>/` prefix string |

`GROUP-<label>/` is a **prefix string**, not a full Forth wordlist hierarchy (yet).

Smoke: `groups-demo` — join grows member count; graph shows `group` / `member` lines.

## 7. Persist

| Word | Stack | Notes |
|------|-------|-------|
| `graph-save` | `( -- )` | Host `platform-graph-save` → `evolve/user-graph.trit` |
| `graph-load` | `( -- )` | Host `platform-graph-load` |

R.E.K.I.A. refine path also touches `evolve/assistant-state.trit` (see `REKIA.md` / persist tip).

## 8. Acceptance (Test Lab — docs tip)

1. `docs/DRENA.md` present; one-line cite from `docs/NEURON.md`.
2. Linux regression unchanged: `rekia-demo`, `s3-reserved-demo`, `grow-step-demo`, `groups-demo`, `qwantum-atoms-demo` still PASS.
3. No Forth behavior change required for this docs-only tip.

## 9. Cite

- `TritiumOS.txt` §§3, 5
- `docs/NEURON.md`, `docs/REKIA.md`, `docs/ASSUMPTIONS.md`, `docs/QWANTUM-REKIA.md`
- `forth/tritium/drena.fs` (PR #10 tip)
