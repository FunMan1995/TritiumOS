# Windows Host — Windows 11

Initial target Windows platform for TritiumOS (primary dev host for tools too).

## Requirements
- Windows 11 (x64 recommended for start; arm64 supported via .NET).
- .NET 8 SDK for build/dev (`tools/run-windows-dev.ps1`).
- Built artifact: self-contained single-file `dist/TritiumOS.exe`.

## Packaging
- `TritiumOS.csproj`: PublishSingleFile + SelfContained + win-x64.
- Bundles `tritium.poly/**` (as `poly/`) + `qd/compute.json`.
- See `tools/build-windows.ps1`.

## Persistence (important for single-file)
- User evolve/ (name, edition, future graphs) → `%LOCALAPPDATA%\TritiumOS\evolve` (fixed previously).
- Bundled core resolved via search paths + RepoRoot improvements.
- Avoid writing to BaseDirectory (temp extract dir on run).

## Host Bridge (VM) Strategy
See `docs/SYSTEM-DESIGN-INITIAL-PLATFORMS.md` and `docs/FORTH-BASE-REFERENCES.md`.
- C# TritiumVM (stack machine, evaluate, primitives).
- Inspired by DuskOS POSIX VM + usermode (thin layer over host OS).
- Spawn powershell for compute tests (already present); later pure interop.
- REPL feeds the VM directly.

## Current State
- Full first-run (license scaffold + name + 32/64 edition persisted to AppData).
- TritiumForthVM integrated: auto-loads core (trit + kernel + drena + rekia), routes REPL, self-tests engines on boot.
- Real DRENA (exact 16-bit header nibbles: S0-S2 trit pairs + S3 mode incl. RANDOM/ADDRESS_FOLD/CONNECTED + id + connected node graph for neuromorphic) + REKIA (extract over links, contract fixed-point, to-forth emission) run inside the VM.
- **Forth-to-C# bootstrap + assimilation + full-stack host OS optimization** (per user confirmation):
  - Host bridges exposed to Forth: host-pwd, host-list-dir, host-read-file, host-exec, host-hw-info, host-evolve-dir.
  - `assimilate` / `assimilate-host-dir`: scans host dirs for software written for this hardware (System32, Program Files, configs, sources, live systeminfo via exec), writes real `.ingest` artifacts (with metadata) under `evolve/assimilated/`.
  - `bootstrap-host`: emits optimization plans, runnable `.ps1`, and refined Forth modules under `evolve/bootstrap/` + `evolve/forth/refined/`.
  - `full-stack-optimize`: chains drena-demo + rekiA-demo + assimilate + bootstrap. The intelligence (Forth inside C#) drives host ingestion + optimization.
  - Same model mirrored (minimally) in the Linux native C .AppImage host.
- REPL commands: help, status, drena-demo, rekiA-demo, assimilate, bootstrap-host, full-stack-optimize, host-info, load-core, etc. Unknown input → VM.Evaluate (real Forth).

## Critical: Antivirus / Windows Security Exclusion (required for assimilation features)
The assimilation + bootstrap features deliberately:
- Execute host tools (`cmd /c systeminfo`, `ver`, dir scans, file reads of .ps1/.ini/.cs etc.)
- Write many small timestamped files into `%LOCALAPPDATA%\TritiumOS\evolve\assimilated\` + `\bootstrap\` + `\forth\refined\`

This behavior (scanning "all the software written for the hardware" + bulk writes + process spawn) triggers Defender / AV heuristics (looks like dropper or ransomware patterns on first runs).

**Before first launch (and after any publish):**
1. Windows Security → Virus & threat protection → Manage settings → Exclusions → Add or remove exclusions.
2. Add exclusion for the **entire project folder** (e.g. `C:\Test\TritiumOS`) **and** the published exe location.
3. Also add `%LocalAppData%\TritiumOS` (the evolve root).
4. If using dev `run-windows-dev.ps1` or `dotnet run`, exclude the `bin/` `obj/` `dist/` subdirs too.

After adding exception you had to "fix and add exception for project folder" — this is why. The feature set (forth to c# to assimilate + bootstrap host os full stack) requires it on Windows.

Once excluded, `full-stack-optimize` etc. will create the directories and artifacts successfully and the cycle works.

## Next / Polish
- Real control flow (if/then/case) in VM so arbitrary loaded .fs (including emitted refined ones) execute fully.
- REKIA actually refining the content of .ingest files (currently emission is bridge-driven; feed snippets into contract math).
- Auto-include of `evolve/forth/refined/*.fs` on boot.
- Android komodo host mirror of the assimilate/bootstrap primitives.
- Real dict + units (Dusk mem/dict.fs) so neurons/groups are first-class.

TritiumOS by Draco. (Run on Win11 for best tool compatibility with qwantum + quantum scripts. The C# layer is the reference "current OS is bootstrapped in c#" implementation.)
