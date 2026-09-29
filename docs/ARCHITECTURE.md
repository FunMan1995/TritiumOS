# ARCHITECTURE — TritiumOS map

**Status:** Shipper-ready overview (wave6 item **3**)  
**Canonical brief:** `TritiumOS.txt` §§1a, 5–8  
**Companions:** per-subsystem docs listed in §4 (do not duplicate their contracts here)

## 1. Intent

TritiumOS ships as a **personal assistant** that co-evolves with the user and may **graduate** into **L.I.N.E.O.S.** Intelligence is refined into **executable Forth** (R.E.K.I.A.), not opaque weights. Topology lives in **D.R.E.N.A.**; delivery is **tritium.poly** + Priority-1 host bridges.

## 2. Layer map

```
┌─────────────────────────────────────────────────────────────┐
│  Hosts (Priority-1)                                         │
│  Win TritiumOS.exe │ Android .apk │ Linux AppImage          │
│  first-run: license → name → edition │ REPL / S0 assist     │
└────────────┬───────────────────────────┬────────────────────┘
             │ platform-* hooks          │ evolve/ I/O
             ▼                           ▼
┌────────────────────────────┐   ┌────────────────────────────┐
│  TritiumForth core         │   │  evolve/ (runtime)         │
│  kernel.fs · trit math     │   │  user-graph · assistant-*  │
│  drena.fs · rekia.fs       │   │  edition · license-slots   │
│  queue · assimilate · lineos│   │  queue/ · assimilate/      │
│  · integrate               │   │  integrate/ (demo)       │
│                            │   │  forth/refined/*.fs        │
│  D.R.E.N.A. = structure    │   │  graduation.json           │
│  R.E.K.I.A. = refine→Forth │   └────────────────────────────┘
└────────────┬───────────────┘
             │ bundled by
             ▼
┌────────────────────────────┐
│  tritium.poly + manifest   │  product_id: tritium → lineos
└────────────────────────────┘
```

## 3. Data / control flows (thin)

| Flow | Path |
|------|------|
| S0 assist | Free-text → `drena-step` + `rekiA-refine` → `evolve/forth/refined/` + live word (`ASSISTANT.md`) |
| Topology | spawn / link / grow / step / groups / `group-link!` (`DRENA.md`, `GROUPS.md`) |
| Persist | `graph-save` / assistant-state / edition / license-slots |
| Economy | non-local job → queue cue → prove → assimilate merge → simti (`QUEUE.md`, `ASSIMILATE.md`) |
| Graduation | thresholds in `graduation.json` → `lineos-graduate` scaffold flip (`LINEOS.md`) |
| Qwantum | dumps = K atoms for extract only — never live vocab (`QWANTUM-REKIA.md`) |

## 4. Doc index (contracts live here)

| Area | Doc |
|------|-----|
| Neuron / trit / S3 | `NEURON.md`, `ASSUMPTIONS.md` |
| Topology | `DRENA.md`, `GROUPS.md`, `GROUPS-NESTED.md` |
| Kernel dict stub | `KERNEL.md` (find/interpret-token; wave7 **5**); `INTERPRET.md` (interpret loop; wave8 **1**) |
| Refine | `REKIA.md`, `QWANTUM-REKIA.md` |
| Assistant S0 | `ASSISTANT.md` |
| License slots | `LICENSE.md` |
| Queue / Assimilate | `QUEUE.md`, `ASSIMILATE.md` |
| Graduation | `LINEOS.md` |
| Install / Integrate | `INSTALL.md`, `INTEGRATE.md` |
| Build / platforms | `BUILD.md`, `SYSTEM-DESIGN-INITIAL-PLATFORMS.md` |
| Gaps (living) | `IMPLEMENTATION-GAPS.md` |

Wave6 **4+5** landed: `tritium-integrate` stub + `INSTALL.md` / `INTEGRATE.md` (host scaffold / bootstrap checklist).

## 5. Repo layout (as-built vs target)

| Path | Role |
|------|------|
| `forth/tritium/` | kernel, drena, rekia, queue, assimilate, lineos, integrate |
| `tritium.poly/` | Bundle core + manifest |
| `install/hosts/{windows,android,linux,_template}` | Host bridges |
| `tools/` | build-*, tritium-license, qwantum, tests |
| `evolve/` | Runtime state (mostly gitignored) |
| `queue/`, `assimilate/`, `license/`, `lineos/`, `master/` | Subsystem notes / validators |
| `refs/` | DuskOS / CollapseOS / GrapheneOS references |
| `docs/` | Specs (this file + §4) |

Target layout in `TritiumOS.txt` §7 still lists top-level `/drena` `/rekia` `/boot` `/userland` — engines today live under `forth/tritium/` (+ stubs); `tritium-integrate` stub landed (wave6 **4**); deeper userland later.

## 6. Non-goals (this doc)

- Stack-effect tables (belong in subsystem docs)
- Build command recipes (`BUILD.md`)
- Product decisions (escalate via Chief of Staff)

## 7. Acceptance (Test Lab — docs tip)

1. `docs/ARCHITECTURE.md` present; README may one-line cite.
2. Docs-only: light suite still green (`s0` / `rekia` / `queue` / `assimilate` / `s3-reserved` sample OK).
3. No Forth behavior change required for item **3** alone.

## 8. Cite

- `TritiumOS.txt` §§1a, 5–8
- `README.md` project layout; subsystem docs in §4
