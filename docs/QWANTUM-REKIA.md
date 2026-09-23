# Qwantum → R.E.K.I.A. handoff

**Status:** Landed (Research follow-on item 5)  
**Companion:** `docs/QWANTUM.md` (ingest), `docs/REKIA.md` (refine → Forth)  
**Canonical brief:** `TritiumOS.txt` §4 (R.E.K.I.A.), §14–§15 (Qwantum)

## 1. One sentence

Qwantum dumps land **knowledge atoms** (K) into T₀; R.E.K.I.A. remains the only path that turns those atoms + neuron context into **runnable Forth**. Dumps never replace `rekiA-refine`.

## 2. Roles

| Path | Does | Does not |
|------|------|----------|
| Qwantum (`docs/QWANTUM.md`) | SEARCH → DUMP → INGEST files under `evolve/qwantum-dump/<search_id>/` | Emit live vocab; run neurons; invent S3=`11` |
| R.E.K.I.A. (`docs/REKIA.md`) | `extract` → `contract` → `to-forth` → write+include under `evolve/forth/refined/` | Pull from the field; bypass D.R.E.N.A. topology |

## 3. Handoff contract

1. Ingest leaves reviewed files in `evolve/qwantum-dump/<search_id>/` (optional `-Apply` into tree).
2. Host or Forth loader may expose dump text/symbols as **K atoms** to `rekiA-extract` (scoped by neuron links — see NEURON.md).
3. `rekiA-refine` still owns emission: `evolve/forth/refined/refined-<id>.fs` + `platform-include-refined`.
4. Qwantum field schema dimensions (timeline, trit, S3, artifact types in `qwantum/field-schema.json`) inform indexing only — they are not a second refinement engine.

## 4. Forbidden patterns

- Treating a dump `.fs` as already-refined intelligence without `rekiA-to-forth` / include
- Skipping D.R.E.N.A. spawn/link when “the field already has neurons”
- Opaque weight blobs as source of truth (violates TritiumOS mandate)
- Auto-advancing S3=`11` RESERVED from dump metadata (`docs/ASSUMPTIONS.md`)

## 5. Acceptance (Test Lab)

Docs tip only:

1. `docs/QWANTUM-REKIA.md` present and linked from `docs/QWANTUM.md` and/or `docs/REKIA.md` (one-line cite is enough).
2. Linux regression unchanged: `rekia-demo` + `s3-reserved-demo` still PASS after the docs tip.
3. No behavior change required in Forth/hosts for this item.

## 6. Cite

- `docs/QWANTUM.md`, `qwantum/prompts/SEARCH-AND-DUMP.txt`
- `docs/REKIA.md` §6 non-goals
- `docs/NEURON.md`, `docs/ASSUMPTIONS.md`
