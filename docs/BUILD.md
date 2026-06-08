# TritiumOS build guide

**Creator:** Draco  
**Ship targets:** `dist/TritiumOS.exe` (Windows) · `dist/TritiumOS.apk` (Android) · `dist/TritiumOS.AppImage` (Linux, on-demand portable)

## Prerequisites

### Windows (.exe)

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Android (.apk)

- JDK 17+
- Android SDK (Android Studio recommended)
- Gradle wrapper in `install/hosts/android` (generated on first build)

## Build commands

From repo root `C:\Test\TritiumOS`:

```powershell
# Windows executable
.\tools\build-windows.ps1

# Android APK (after SDK + gradle wrapper)
.\tools\build-android.ps1

# Source bundle zip
.\tools\build-poly.ps1

# Linux .AppImage (on-demand portable, run on most distros)
# Requires Linux env with appimagetool or use the generated AppDir
bash tools/build-linux.sh
```

## Outputs

| Script | Output |
|--------|--------|
| `build-windows.ps1` | `dist/TritiumOS.exe` |
| `build-android.ps1` | `dist/TritiumOS.apk` |
| `build-poly.ps1` | `dist/tritium.poly.zip` |
| `build-linux.sh` | `dist/TritiumOS.AppImage` (and AppDir) |

## First run (both hosts)

1. License key (scaffold: 8+ characters; production: master-minted)
2. **Name your assistant** (user-chosen)
3. Edition 32 (cyan) or 64 (magenta)

Mint a dev license key:

```powershell
.\master\mint-license.ps1
```

## Qwantum field (search + dump)

Pull refined program from Qwantum Compute into the repo:

```powershell
.\tools\qwantum-field.ps1 -Action search    # paste prompt into Qwantum Compute
.\tools\qwantum-dump.ps1 -InputPath dist\qwantum-reply.txt -Apply
```

See `docs/QWANTUM.md`.

## Quantum compute (one setting)

Edit **`qd/compute.json`** or use:

```powershell
.\tools\compute-config.ps1 -Action list
.\tools\compute-config.ps1 -Action set -Backend braket_local
.\tools\run-compute.ps1
```

Override once: `.\tools\test-compute.ps1 -Provider aer`

See `docs/QD-COMPUTE.md` and `docs/COMPUTE-ALTERNATIVES.md`.

## Initial Platforms (Systems Design)

- **Android:** Google Pixel 9 Pro XL (codename komodo) — primary mobile target.
- **Windows:** Windows 11 — primary dev + desktop target.

See `docs/SYSTEM-DESIGN-INITIAL-PLATFORMS.md` (host + Forth VM + HAL architecture, bootstrap, using DuskOS references) and `docs/FORTH-BASE-REFERENCES.md`.

## Linux (.AppImage - on-demand portable assistant)

**The end project for Linux is a portable `.AppImage`** for on-demand use.

The overall end product: an **on-demand intelligent assistant** that full-stack refines the hardware (via DRENA neuromorphic data blocks + REKIA pure-math refinement into executable Forth) and assists the user (REPL, co-evolution, tasks). Linux delivery is the .AppImage (self-contained, no install, runs on most distros).

See the vision in `TritiumOS.txt` (S0 assistant first, evolves; polyglot bundle).

### Prerequisites (build on Linux)
- appimagetool (https://github.com/AppImage/AppImageKit/releases — download the x86_64.AppImage, chmod +x, or in PATH)
- No Python (the .AppImage is native C only; see "no-python" requirement in history and SYSTEM-DESIGN)
- fuse (for testing the AppImage)

### Build
```bash
# On a Linux system (or WSL)
bash tools/build-linux.sh
```
Outputs:
- `dist/TritiumOS.AppImage` (the portable executable — `chmod +x` and run)
- `dist/TritiumOS.AppDir` (for inspection/customization)

The .AppImage bundles:
- `usr/share/tritium.poly/` — the core (Forth sources + DRENA/REKIA engines for hardware refinement + assistance)
- `usr/bin/tritiumos` — the **native C** host (REPL + engine sims + assimilation + host bootstrap primitives, mirroring the C# reference "forth to c#" model)
- Desktop entry, icon, AppRun
- Assets

Run: `./TritiumOS.AppImage` (or double-click after chmod)

It provides the on-demand assistant: name your assistant, edition, REPL with demos for DRENA (hardware graph) + REKIA (refine to Forth), plus `assimilate`, `bootstrap-host`, `full-stack-optimize` (the concrete implementation of "assimilate all the software written for the hardware" + "bootstrap its host os to full stack optimize").

See `install/hosts/linux/tritiumos.c` (native host) and `install.sh`.

The Linux .AppImage follows the same architecture as Windows: native bootstrap layer around the shared Forth intelligence so it can assimilate host software and optimize the launched system.

For hardware refinement on Linux: the engines abstract it; extend with real sensors via GrapheneOS-style bringup if needed.

## Spec

Full product spec: `TritiumOS.txt`
<!-- AV + assimilation note added during "go" iteration -->

## Assimilation + Full-Stack Host OS Optimization (new in this phase)
The builds now produce artifacts that support the core architecture ("all current os is bootstrapped in c# so forth to c# to allow it to assimilate all the software... with the ability to bootstrap its host os to full stack optimize").

- After running the .exe (or .AppImage), use `full-stack-optimize` (or the individual commands).
- This writes to `evolve/assimilated/`, `evolve/bootstrap/`, and `evolve/forth/refined/`.
- On Windows the refined modules are auto-loaded via `LoadRefinedModules()` (in same session + next boot).
- **AV exclusion is mandatory** for these features on Windows (see the build script output, the Win11 README, and SYSTEM-DESIGN-INITIAL-PLATFORMS.md for details). The exception you added for the project folder enables this.

See `install/hosts/windows/Program.cs` (LoadRefined + command wiring) and the host impls for the concrete bridges.
