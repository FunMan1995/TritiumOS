# INTEGRATE — `tritium-integrate` stub

**Status:** Shipper-ready stub spec (wave6 item **4**)  
**Canonical brief:** `TritiumOS.txt` §5a.2 Phase C, Phase 8  
**Sources of truth (code):** stub CLI and/or Forth under `tools/` or `userland/` / `forth/tritium/`; scaffold from `install/hosts/_template/`  
**Companions:** `docs/INSTALL.md` (bootstrap checklist), `docs/LICENSE.md` (slots), `docs/ARCHITECTURE.md` (layout)

## 1. Purpose

Post-bootstrap, TritiumOS **integrates** onto an additional host by scaffolding a platform adapter from `_template` and binding license + evolve identity. This wave lands a **local stub** + demo — not fleet sync, not production crypto, not a new Priority-1 ship artifact.

## 2. Contract (Phase C)

`tritium-integrate` on the target host must:

1. Require a **free device slot** (or same key with capacity) — refuse when 10/10 or slot 11 (`LICENSE.md`).
2. Copy / emit adapter scaffold from `install/hosts/_template/` → `install/hosts/<platform>/` (or a demo path under `evolve/integrate/<platform>/` for smoke when repo write is unwanted).
3. Export or reference license-bound identity + `evolve/` markers (user-graph path optional stub).
4. Print clear markers for Test Lab.

Non-goals this wave: real cross-host evolve sync, store packaging for a new OS, irreversible fleet ops.

## 3. Surfaces (stub)

| Surface | Behavior |
|---------|----------|
| `tritium-integrate <platform>` | Scaffold from `_template`; register device id if needed; write markers |
| `tritium-integrate-demo` | Smoke: platform=`demo` (or `stub`); force free-slot path → `[tritium-integrate-demo] OK` |
| `_template/README.txt` | Keep one-line purpose; stub may add `INTEGRATE.txt` checklist under template |

Suggested layout (Shipper picks one primary):

- CLI: `tools/tritium-integrate` (bash/ps1) **or**
- Forth: `forth/tritium/integrate.fs` words `tritium-integrate` / `tritium-integrate-demo` **and/or**
- Linux REPL: host commands mirroring the demo (same markers)

Demo may write under `evolve/integrate/` only (gitignored) rather than mutating `install/hosts/` in CI.

## 4. Markers

```
[tritium-integrate] platform=<name> from=_template
[tritium-integrate] refuse — no free license slot (§5a.4)
[tritium-integrate-demo] OK
[tritium-integrate-demo] FAIL
```

Cite §5a.2 / Phase 8 in refuse / OK lines when practical.

## 5. Acceptance (Test Lab — code + docs tip)

1. Stub present (CLI and/or Forth/host); `tritium-integrate-demo` → OK.
2. Slot gate: full registry → refuse (reuse license smoke helpers).
3. Scaffold lands under demo path or `_template` copy with readable marker file.
4. Poly + existing suite stay green (queue / assimilate / lineos / license / s0 / …).
5. `docs/INSTALL.md` may land in the same tip (item **5**); no merge.

## 6. Cite

- `TritiumOS.txt` §§5a.2, Phase 8, success criteria `tritium-integrate`
- `install/hosts/_template/README.txt`
- `docs/LICENSE.md`, `docs/INSTALL.md`, `docs/ARCHITECTURE.md`
