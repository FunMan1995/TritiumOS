# ARCHITECTURE — TritiumOS map

**Status:** Shipper-ready overview (wave9 item **5** — cite refresh; supersedes wave8 **5** map)  
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
│  parity markers (Win/Android CONTRACT OK)                   │
└────────────┬───────────────────────────┬────────────────────┘
             │ platform-* hooks          │ evolve/ I/O
             ▼                           ▼
┌────────────────────────────┐   ┌────────────────────────────┐
│  TritiumForth core         │   │  evolve/ (runtime)         │
│  kernel · interpret · colon│   │  user-graph · assistant-*  │
│  trit-math · drena · rekia │   │  edition · license-slots   │
│  queue · assimilate · lineos│  │  queue/ · assimilate/      │
│  integrate · master · fleet│   │  integrate/ · master/      │
│  userland scaffold         │   │  fleet/ · lineos/ · userland│
│                            │   │  forth/refined/*.fs        │
│  D.R.E.N.A. = structure    │   │  graduation.json           │
│  R.E.K.I.A. = refine→Forth │   │  assistant-state.trit v2 │
└────────────┬───────────────┘   └────────────────────────────┘
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
| Assistant state | v2 persist task/reminder/note hooks (`ASSISTANT-STATE.md`) |
| Topology | spawn / link / grow / step / groups / nested find (`DRENA.md`, `GROUPS.md`, `GROUPS-NESTED.md`) |
| Trit math | `trit+` / `trit*` / `pack-neuron-header` (`TRIT-MATH.md`, `NEURON.md`) |
| Interpret / colon | token stream → `interpret` / `:` body/marker → `;` (`KERNEL.md`, `INTERPRET.md`, `COLON.md`) |
| Persist | `graph-save` / assistant-state / edition / license-slots / vocab graph lines |
| Economy | enqueue → pull → `queue-prove!` → `assimilate-merge!` (`QUEUE.md`, `ASSIMILATE.md`, `ECONOMY-WIRE.md`) |
| Trust | format mint/verify scaffold (`MASTER.md`); slot gate (`LICENSE.md`) |
| Fleet | opt-in local export/import same-key (`FLEET.md`) — no network yet |
| Graduation | thresholds → `lineos-graduate` → confirm UX → brand markers (`LINEOS.md`, `LINEOS-CONFIRM.md`, `LINEOS-BRAND.md`) |
| Userland | scaffold tree + `userland-demo` (`USERLAND.md`) |
| Host parity | Linux SoT demos; Win/Android `parity/HOST-PARITY.txt` (`HOST-PARITY.md`) |
| Qwantum | dumps = K atoms for extract only — never live vocab (`QWANTUM-REKIA.md`) |

## 4. Doc index (contracts live here)

| Area | Doc |
|------|-----|
| Neuron / trit / S3 | `NEURON.md`, `ASSUMPTIONS.md`; `TRIT-MATH.md` (wave9 **1**) |
| Topology | `DRENA.md`, `GROUPS.md`, `GROUPS-NESTED.md` (wave7 **4**) |
| Kernel / interpret / colon | `KERNEL.md` (wave7 **5**); `INTERPRET.md` (wave8 **1**); `COLON.md` (wave9 **4**) |
| Refine | `REKIA.md`, `QWANTUM-REKIA.md` |
| Assistant S0 / state | `ASSISTANT.md`; `ASSISTANT-STATE.md` (wave9 **3**) |
| License / master | `LICENSE.md`; `MASTER.md` (wave7 **1**) |
| Queue / Assimilate / wire | `QUEUE.md`, `ASSIMILATE.md` (wave6); `ECONOMY-WIRE.md` (wave9 **2**) |
| Fleet sync | `FLEET.md` (wave7 **2**) |
| Graduation / brand | `LINEOS.md`; `LINEOS-BRAND.md` (wave7 **3**); `LINEOS-CONFIRM.md` (wave8 **2**) |
| Install / Integrate | `INSTALL.md`, `INTEGRATE.md` (wave6 **4+5**) |
| Userland scaffold | `USERLAND.md` (wave8 **3**) |
| Host parity | `HOST-PARITY.md` (wave8 **4**) |
| Build / platforms | `BUILD.md`, `SYSTEM-DESIGN-INITIAL-PLATFORMS.md` |
| Gaps (living) | `IMPLEMENTATION-GAPS.md` (this tip refreshes cites) |

**Wave cite summary (closed on tip, not merged):** wave6–8 as prior; **wave9** TRIT-MATH / ECONOMY-WIRE / ASSISTANT-STATE / COLON / **this cite refresh**.

## 5. Repo layout (as-built vs target)

| Path | Role |
|------|------|
| `forth/tritium/` | kernel, interpret, colon, drena, rekia, queue, assimilate, lineos, integrate, master, fleet |
| `forth/trit.fs` | trit+/trit*/pack-neuron-header (`TRIT-MATH.md`) |
| `tritium.poly/` | Bundle core + manifest |
| `install/hosts/{windows,android,linux,_template}` | Host bridges + `parity/HOST-PARITY.txt` |
| `tools/` | build-*, tritium-license, tritium-master, tritium-fleet, tritium-lineos, demos |
| `evolve/` | Runtime state (mostly gitignored); `assistant-state.trit` v2 |
| `queue/`, `assimilate/`, `license/`, `lineos/`, `master/`, `fleet/`, `userland/` | Subsystem notes / validators / scaffolds |
| `refs/` | DuskOS / CollapseOS / GrapheneOS references |
| `docs/` | Specs (this file + §4) |

Target layout in `TritiumOS.txt` §7 still lists top-level `/drena` `/rekia` `/boot` `/userland` — engines today live under `forth/tritium/` (+ stubs). Landed scaffolds: integrate (wave6 **4**), userland (wave8 **3**), host-parity (wave8 **4**), assistant-state v2 (wave9 **3**). Bare-metal `/boot` later.

## 6. Non-goals (this doc)

- Stack-effect tables (belong in subsystem docs)
- Build command recipes (`BUILD.md`)
- Product decisions (escalate via Chief of Staff)
- Forth behavior change (docs-only tip)

## 7. Acceptance (Test Lab — wave9 **5**)

1. `docs/ARCHITECTURE.md` matches Research draft (byte-cmp OK).
2. §4 index cites wave9 **1–4** docs (`TRIT-MATH`, `ECONOMY-WIRE`, `ASSISTANT-STATE`, `COLON`) plus prior wave6–8 index.
3. `docs/IMPLEMENTATION-GAPS.md` refreshed with wave9 **5** cite note + wave9 **1–4** landed markers.
4. Docs-only: light suite still green (no Forth behavior change required).
5. No merge.

## 8. Cite

- `TritiumOS.txt` §§1a, 5–8
- `README.md` project layout; subsystem docs in §4
- Wave6–9 tip stack (gate by SHA; unmerged tip stack OK)
