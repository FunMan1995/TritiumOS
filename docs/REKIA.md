# R.E.K.I.A. — Pure-math refinement → Forth

**Status:** Shipper-ready spec (matches PR #2 tip / Research backlog)  
**Full name:** Artificially Intelligent Knowledge Extraction and Refinement  
**Canonical brief:** `TritiumOS.txt` §4  
**Sources of truth (code):** `forth/tritium/rekia.fs`, host hooks in Linux C bridge  
**Companion:** `docs/NEURON.md` (header + D.R.E.N.A. graph)

## 1. Mandate

Refine on-demand intelligence for a neuron into **runnable Forth** — colon definitions / constants — not opaque weight tensors. Pipeline is **deterministic** given (neuron, links, seed policy), except when S3 is RANDOM and entropy is explicitly mixed.

Division of labor:

- **D.R.E.N.A.** — structure / topology (spawn, links, S3)
- **R.E.K.I.A.** — semantics (extract → contract → to-forth → label)

## 2. Canonical pipeline

```
rekiA-refine ( neuron-addr -- )
  rekiA-extract     \ scope K to header + link slice
  rekiA-contract    \ bounded fixed-point on trit nibbles
  rekiA-to-forth    \ emit source + write + include
  \ label-group (future: group-label!)
```

Demo entry point: `rekia-demo` / `rekiA-demo`  
(spawn → rewire → link → rewire → refine).

## 3. Word stack effects

| Word | Stack | Notes |
|------|-------|-------|
| `rekiA-extract` | `( n-addr -- s0 s1 s2 influence )` | S0..S2 from header; influence = link-count |
| `rekiA-one-step` | `( s0 s1 s2 influence -- s0' s1' s2' influence )` | one contraction pass |
| `rekiA-contract` | `( s0 s1 s2 influence -- s0' s1' s2' )` | bounded iters (currently 4) |
| `rekiA-to-forth` | `( s0 s1 s2 id -- )` | build path+source; host write+include |
| `rekiA-refine` | `( neuron-addr -- )` | full pipeline |
| `rekia-demo` | `( -- )` | smoke path for Test Lab |
| `rekiA-demo` | `( -- )` | alias |

Helpers (internal): `contract-trit`, `contract-nibble`, `extract-trit-signature`.

### 3.1 Host hooks (defer)

Pure Forth defaults are no-ops; Linux / Win / Android hosts implement:

| Deferred word | Stack | Duty |
|---------------|-------|------|
| `platform-write-refined` | `( src-addr src-u path-addr path-u -- )` | write `.fs` under evolve |
| `platform-include-refined` | `( path-addr path-u -- )` | `include` / evaluate into live vocab |

## 4. Emit path & artifact

- Path pattern: `evolve/forth/refined/refined-<id>.fs`
- Emitted form (demo): `: refined-<id> ( -- n ) <value> ;`
- Log line on success: `[REKIA] wrote+include evolve/forth/refined/refined-<id>.fs`
- Intelligence becomes **live vocabulary** only after include succeeds.

## 5. Acceptance (Test Lab — Linux+poly)

Gate on tip that includes this contract:

1. `tools/build-poly.sh` + `tools/build-linux.sh` clean.
2. AppImage smoke with `rekia-demo` (or `rekiA-demo`):
   - ADDRESS_FOLD φ line from D.R.E.N.A. (see `NEURON.md`)
   - `[REKIA] wrote+include …/refined-<id>.fs`
   - artifact exists on disk under evolve
   - include OK (refined word available / no abort)
3. Core still reports trit / drena / rekia load; `status` OK.
4. First failure reported by platform if any step fails.

Windows / Android: same acceptance — host VMs implement `platform-write-refined` / `platform-include-refined` under each platform evolve dir (`%LocalAppData%/TritiumOS/evolve`, Android `filesDir/evolve`).

## 6. Non-goals (this tip)

- Opaque ML weights as source of truth
- Inventing S3=`11` behavior
- Full `label-group` / `group-label!` persistence (follow-on)
- Qwantum dumps as a second intelligence path — dumps feed **atoms** into extract later; they do not replace R.E.K.I.A. See `docs/QWANTUM-REKIA.md`.

## 7. Follow-on ≤5 (after docs land)

1. Typed link records + trit-weight (`link!` / `links-for-neuron` per §3.5)
2. `rekiA-label-group` + `group-label!` writing labeled group metadata
3. Persist refine into `evolve/assistant-state.trit` / `evolve/user-graph.trit` (ship tip)
4. ~~Wire Win + Android `platform-*-refined` hooks~~ (landed on host tip)
5. ~~Qwantum → extract handoff note~~ (landed: `docs/QWANTUM-REKIA.md`)

## 8. Cite

- `TritiumOS.txt` §4 (pipeline + primitives)
- `docs/NEURON.md` (header / S3 / φ)
- `docs/DRENA.md` (topology / grow / groups)
- `docs/IMPLEMENTATION-GAPS.md` (historical gap ledger)
- `docs/QWANTUM.md` (T∥ → T₀ ingest; complementary)
- `docs/QWANTUM-REKIA.md` (dumps = K atoms for extract only)
