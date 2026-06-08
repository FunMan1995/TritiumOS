# QD compute config

One file controls which quantum backend TritiumOS uses for smoke tests and future `qd` jobs.

## Canonical config

**`qd/compute.json`** — edit `active` or use the CLI:

```powershell
.\tools\compute-config.ps1 -Action list
.\tools\compute-config.ps1 -Action set -Backend aer_local
.\tools\run-compute.ps1
```

| `active` id       | Test runner        | Cost   |
|-------------------|--------------------|--------|
| `aer_local`       | Qiskit Aer         | Free   |
| `braket_local`    | Braket local sim   | Free   |
| `braket_cloud`    | Braket AWS         | Paid   |
| `ibm_open`        | IBM Quantum Open   | Free*  |

\* IBM blocked until instance CRN is visible — see `docs/IBM-INSTANCE.md`. Keep `ibm_enabled: false` until then.

## Poly bundle

`tritium.poly/manifest.json` points at `qd/compute.json`. `tritium.poly/compute.json` is a **stub** (active + limits only); the full `backends` map lives under `qd/`.

## Apps

- **Windows:** `compute`, `compute-set`, `compute-test` in the shell; config copied next to `TritiumOS.exe` as `qd/compute.json`.
- **Android:** `compute` / `compute-set` commands; bundled `assets/qd/compute.json`, writable copy under app files.

## Overrides

```powershell
.\tools\test-compute.ps1 -Provider aer
$env:TRITIUM_COMPUTE_CONFIG = "C:\path\to\compute.json"
```

Python: `tools/compute_config.py` — `load_config()`, `active_test_provider()`, `resolve_provider()`.

## Ranked alternatives

See `qwantum/providers.json` and `docs/COMPUTE-ALTERNATIVES.md`.