# LINEOS — Graduation to L.I.N.E.O.S.

**Status:** Shipper-ready spec (wave5 items **4+5**; stub thresholds + `lineos-graduate`)  
**Canonical brief:** `TritiumOS.txt` §1a / §1a.1, Phase 9  
**Sources of truth (code):** `evolve/graduation.json`; `lineos-graduate` stub (host and/or Forth); `lineos/graduate.txt`  
**Companions:** `docs/ASSISTANT.md` (S0), `docs/LICENSE.md` (slots), `docs/DRENA.md` / `docs/REKIA.md` (graph + refine gates), `docs/LINEOS-BRAND.md` (splash/about markers stub)

## 1. Purpose

Users install a **personal assistant** (S0) that co-evolves into **TritiumOS** and eventually **graduates** into **L.I.N.E.O.S.** — near-OS control as the primary environment. Graduation is a **gated event**, not a day-one rebrand.

This wave stubs the gate: configurable thresholds in `evolve/graduation.json`, a `lineos-graduate` word/CLI that checks them and performs a **scaffold** product_id flip. Real fleet sync, irreversible UX confirm, and full branding packs stay later.

## 2. Evolution ladder (summary)

| Stage | User experience | Milestone |
|-------|-----------------|-----------|
| S0 App | Chat, tasks, context | Assistant UI + R.E.K.I.A. / D.R.E.N.A. seeds |
| S1 TritiumOS | Forth shell, host bridges, evolve/ | Bootstrap + persist graph |
| S2 Transitional | Deep integration on 2+ hosts | `tritium-integrate`; density threshold |
| S3 L.I.N.E.O.S. | Near-OS control | Graduation event (§3) |

Slogan (env / about): `LINEOS_SLOGAN` — *"The line tread between madness and genius."* Shown on L.I.N.E.O.S. splash after graduation; Tritium may remain as origin badge (32/64 edition preserved).

## 3. Fire conditions (`lineos-graduate`)

Fire when **all** are true (defaults tunable in `graduation.json`):

1. Assistant session count ≥ N (default **30**) **OR** user invokes `become-lineos`
2. D.R.E.N.A. graph ≥ minimum neurons **and** ≥1 CONNECTED cluster
3. R.E.K.I.A. has refined ≥1 knowledge cell per active neuron class
4. Installed on ≥1 Priority-1 host; license valid (≤10 devices — see `LICENSE.md`)
5. User confirms: “Promote to L.I.N.E.O.S.” (irreversible without export/reset)

**Stub policy (this wave):** thresholds may be forced low / demo-flagged so `lineos-graduate-demo` can PASS without 30 real sessions; still print which gates passed/failed. Do **not** merge to main or flip production branding without an explicit user ask.

## 4. `evolve/graduation.json` (v1 stub shape)

```json
{
  "minSessions": 30,
  "minNeurons": 8,
  "minConnectedClusters": 1,
  "minRefinedPerClass": 1,
  "requireLicenseValid": true,
  "requireUserConfirm": true,
  "productIdBefore": "tritium",
  "productIdAfter": "lineos",
  "demoForceReady": false
}
```

- Created with defaults if missing.
- `demoForceReady: true` (or CLI `--demo`) lets smoke skip session grind; must be documented in demo output.
- Runtime file under `evolve/` — do not commit secrets; committing a **template** under `evolve/graduation.json.example` or `lineos/` is OK if Shipper prefers.

## 5. On graduation (scaffold)

| Effect | Stub behavior |
|--------|----------------|
| UI product name | Print / set host flag → **L.I.N.E.O.S.** |
| Splash / about | Show slogan |
| Manifest | Scaffold write or overlay: `product_id` → `lineos` (do not force-push real release artifacts) |
| Edition | Preserve 32/64 |
| Origin badge | Tritium branding may remain |

Word / CLI:

| Surface | Notes |
|---------|-------|
| `lineos-graduate` | Check gates; on success scaffold flip + markers |
| `lineos-graduate-demo` | Smoke: force-ready or low thresholds → `[lineos-graduate-demo] OK` |
| `become-lineos` | Optional alias / user invoke path (may set confirm flag) |

## 6. Acceptance (Test Lab)

1. `docs/LINEOS.md` present (Research draft byte-copy OK).
2. `evolve/graduation.json` (or committed example + runtime create) loads; fields readable.
3. `lineos-graduate-demo` (or equivalent) OK — prints gate results; scaffold product_id / branding markers; cite §1a.1.
4. Poly + existing suite stay green (queue / assimilate / license / s0 / groups / rekia / s3-reserved / …).
5. No merge; no irreversible production release.

## 7. Cite

- `TritiumOS.txt` §§1a, 1a.1, Phase 9
- `docs/ASSISTANT.md`, `docs/LICENSE.md`, `docs/DRENA.md`, `docs/REKIA.md`
- `lineos/graduate.txt`; manifest product ids `tritium` \| `lineos`
