# Qwantum → R.E.K.I.A. handoff

**Status:** Runtime tip (Research item 3 — `qwantum-atoms-load`; docs landed as follow-on 5)  
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

Runtime tip (`qwantum-atoms-load`):

1. `qwantum-atoms-load` feeds dump text under `evolve/qwantum-dump/<id>/` into **`rekiA-extract` scope only** (K influence). Dump `.fs` must **not** be included as live vocab.
2. `qwantum-atoms-demo` seeds fixture `sample01test` → load → refine; greppable: `[qwantum-atoms-demo] OK — refined written; dump not vocab` and `[QWANTUM] atoms-load → extract scope (no vocab)`.
3. `rekiA-refine` still writes `evolve/forth/refined/refined-*.fs` via existing `rekiA-to-forth` path.
4. Regression: `rekia-demo` green; `s3-reserved-demo` before=3 / after=3 (S3 RESERVED untouched by atoms-load).

## 6. Cite

- `docs/QWANTUM.md`, `qwantum/prompts/SEARCH-AND-DUMP.txt`
- `docs/REKIA.md` §6 non-goals
- `docs/NEURON.md`, `docs/ASSUMPTIONS.md`
