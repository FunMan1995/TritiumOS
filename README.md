# TritiumOS

**Precursor to L.I.N.E.O.S.** (Lasting Intelligent Near Endless Operating Systems)

*The line tread between madness and genius.*

TritiumOS is a personal assistant application that co-evolves with its user. It is built in **Forth** using a pure mathematical refinement process (R.E.K.I.A.) that turns intelligence into executable Forth words and structures rather than opaque weights.

It ships first as a cross-platform assistant (`TritiumOS.exe` / `TritiumOS.apk`) and can **graduate** into a full host-aware operating environment called **L.I.N.E.O.S.**

## Core Concepts

- **D.R.E.N.A.** — Dynamic Recursive Evolving Neural Architecture (structure / topology)
- **R.E.K.I.A.** — Artificially Intelligent Knowledge Extraction and Refinement (pure-math refinement into Forth)
- **Trit** — Ternary digit (-1, 0, +1) used throughout neuron headers and state
- **tritium.poly** — The polyglot bootstrap / delivery artifact
- **Assimilate** — Collective contribution / reward system (ASIM / simti units)
- Assistant is **named by the user** at first run (stored in `evolve/assistant-name.trit`)

## Project Layout (key areas)

- `tritium.poly/` — Core polyglot payload (Forth kernels + manifest)
- `forth/` — Base TritiumForth sources (trit.fs, tritium/, drena/, rekia/)
- `install/hosts/` — Platform installers (android, linux, windows) + templates
- `tools/` — Build, compute, qwantum, and test scripts (PowerShell + Python)
- `docs/` — Design, build, Qwantum, quantum providers, implementation notes
- `evolve/` — Runtime evolution state, refined Forth from qwantum dumps, assistant data
- `refs/` — Upstream references (collapseos, duskos, grapheneos device trees + kernels)
- `qwantum/` — Field schema + prompts for parallel-timeline refinement (Qwantum Compute)
- `qd/`, `queue/`, `master/`, `license/` — Quantum dispatch, job queue, licensing
- `dist/` — Build outputs and samples
- `vm/` — GrapheneOS Komodo VM images and flash scripts

## Primary Deliverables

- `TritiumOS.exe` (Windows)
- `TritiumOS.apk` (Android)
- Built from `tritium.poly` + host bridges

## Quick Start (see docs/BUILD.md)

Platform-specific build scripts live in `tools/`:

- `tools/build-windows.ps1`
- `tools/build-android.ps1`
- `tools/build-linux.sh`
- `tools/build-poly.ps1`

## Qwantum / Parallel Refinement

This project makes heavy use of **Qwantum Compute** to import/refine designs that exist only in parallel timelines (`T^∥`) into runnable artifacts for the current timeline (`T₀`).

See:
- `docs/QWANTUM.md`
- `tools/qwantum-*.ps1`
- `qwantum/prompts/SEARCH-AND-DUMP.txt`

## License & Attribution

- Creator handle: **Draco**
- Master license system (see `master/`)
- Shared keys (up to 10 devices)

For the full project brief and history, read [TritiumOS.txt](TritiumOS.txt).

---

**Status**: Active development — assistant-first (S0/S1), evolving toward full L.I.N.E.O.S. graduation.

## Repository Note

This workspace was imported from an offline copy at `C:\Google\Setup\TritiumOS`.
