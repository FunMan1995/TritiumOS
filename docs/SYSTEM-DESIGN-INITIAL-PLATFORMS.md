# Systems Design: Initial Platforms for TritiumOS
**Platforms:** 
- Android: Google Pixel 9 Pro XL (codename: komodo / google-komodo)
- Windows: Windows 11 (x64 primary; ARM64 secondary consideration)

**Role:** Systems Designer
**Date:** 2026
**References:**
- TritiumOS.txt (full spec, phases, success criteria, neuron encoding, polyglot bootstrap)
- docs/FORTH-BASE-REFERENCES.md + cloned `refs/duskos/` (Dusk OS kernel anatomy, HAL, dict/units, POSIX VM, comp/ emission, simplicity design) and `refs/collapseos/`
- Previous fixes: Windows persistence (LocalApplicationData), core asset sync, usings, RepoRoot, Android defaults/build ordering
- Current scaffolds: `install/hosts/android/`, `install/hosts/windows/`
- `tritium.poly/` + manifest + core/ (now includes tritium-kernel.fs seed)

## 1. Executive Summary / Goals for Initial Platforms
Deliver runnable **TritiumOS.exe** (Win11) and **TritiumOS.apk** (Pixel 9 Pro XL / komodo) that:
- Perform first-run bootstrap (license >=8 chars scaffold, user-chosen assistant name → `evolve/assistant-name.trit`, 32/64 edition).
- Boot a real (even minimal) TritiumForth core from the bundled `tritium.poly` / assets.
- Provide REPL that can execute Forth (starting with existing `core/boot.fs` + `trit.fs` + new `tritium-kernel.fs`).
- Support basic commands + path to "connect R.E.K.I.A." (route input → refinement + DRENA tick).
- Persist user state correctly (name, edition, future graph/refined words, compute overrides).
- Integrate existing compute/quantum (qd/compute.json, backends) and qwantum tools.
- Use DuskOS/CollapseOS patterns for the Forth base (minimal kernel + build-up, HAL abstraction for platforms, units/dict for groups, code emission model for REKIA).
- Be self-contained single artifacts (exe/apk) built from `tritium.poly`.
- Set foundation for S0 personal assistant, later phases (full engines, license enforcement, queue/assimilate, graduation to L.I.N.E.O.S., integrate to other hosts).

**Non-goals for v0.1 on these platforms:** Bare-metal drivers (use host OS), full DRENA/REKIA (stubs + first emission), master-signed keys (scaffold), multi-device fleet.

**Why these platforms first?**
- Windows 11: Easy dev (dotnet publish single-file), broad reach, good for tools (qwantum, compute ps1/py).
- Pixel 9 Pro XL (komodo): Modern flagship (Tensor G4, 16GB RAM excellent for Forth graph + future on-device math/AI hybrid, long support to Android 21?, clean Android for reliable app behavior). "Komodo" lizard theme fun tie-in to CollapseOS lizard codenames? Large screen/battery good for assistant UI + long sessions.

**Key constraints from platforms:**
- **Pixel komodo (Android 14 → 16+):** Scoped storage (filesDir for evolve/ + writable qd), assets for immutable core bundle, minSdk 26+ ok (Pixel is new), 16GB RAM → can hold large evolving dict/graph. Tensor G4 good for future accel of pure-math REKIA (but keep pure first). No root assumed for stock app.
- **Win11:** Single-file publish extracts to temp on run (hence previous LocalAppData fix for evolve). x64 (or arm64). .NET 8+ runtime self-contained. File I/O for poly/ + evolve in AppData. Good spawning for compute tests.

## 2. High-Level Architecture (Layers)
Inspired directly by Dusk OS (see refs/duskos/fs/doc/kernel.txt, hal.txt, arch/core.fs, posix/):

```
[ Host UI / REPL (WinForms / Android Activity) ]
          ↓ (commands, display)
[ Host Bridge / VM Layer (C# or Kotlin "TritiumVM") ]
  - Provides primitive words (stack, mem, I/O, file, compute hooks)
  - Loads bundled sources (poly/core/* or assets/core/*)
  - Implements cold-boot, interpret loop (DTC/ITC or simple)
  - HAL for platform (file paths, console, edition, compute)
          ↓ (Forth execution)
[ TritiumForth Core Sources (from tritium.poly / assets) ]
  - boot.fs + trit.fs + tritium-kernel.fs (Dusk-style minimal kernel)
  - Later: drena/*.fs, rekia/*.fs, higher vocabs, units for groups
          ↓ (on refine)
[ User Evolution State (evolve/ in platform-private storage) ]
  - assistant-name.trit, edition.trit, assistant-state.trit
  - forth/refined/<label>.fs (emitted + included)
  - future: user-graph.trit, license-slots, wallets, etc.
```

**Platform-specific HAL responsibilities (modeled on Dusk HAL + arch/):**
- Windows (C#): Use `Environment.SpecialFolder.LocalApplicationData`, `AppContext.BaseDirectory` for bundled poly (with care for single-file extraction), Process for external compute, WinForms TextBox for grid-like REPL.
- Android/komodo (Kotlin): `Context.filesDir` / `getFilesDir()` for evolve/, `assets` for core (synced at build), `AssetManager` + File copy for writable, EditText + TextView, no external process for compute initially (call Java/Kotlin impls or keep ps1 via termux? but prefer pure for now).
- Common: Edition affects ARCH (32/64), assistant name as counted string, compute config as "device" word.
- **GrapheneOS insights for komodo (Pixel 9 Pro XL)**: See updated install/hosts/android/README-komodo.md and cloned refs/grapheneos/. The user-added komodo-install-2026060100.zip (in project root) provides the exact factory images and scripts for this release:
- bootloader=ripcurrentpro-16.4-14791556, baseband=g5400c-251201-260127-B-14784805
- A/B slots, vendor_kernel_boot, custom AVB (erase avb_custom_key + flash avb_pkmd.bin), oem uart disable, fips/dpm erase, multi-part super flash.
Extracted metadata lives in refs/grapheneos/komodo-install-2026060100/ (android-info.txt, script.txt, flash-all.*).
GrapheneOS provides production-grade Pixel hardware abstraction and bootstrap (AOSP-based with custom kernel, verified boot, device bringup). Use their device_google_caimito (komodo/BoardConfig.mk, device-komodo.mk) + this installer zip for Tensor G4 specifics, init.rc, partitions, kernel 6.1 configs if deepening integration beyond thin app (e.g., custom system image for full L.I.N.E.O.S. bootstrap of Forth core). Their build (vendorbootimage etc.) and install (fastboot + AVB signing) are models for secure hardware bootstrap on the target device. Install Tritium APK on GrapheneOS for hardened runtime. The Android host's assimilation/bootstrap now emits plans referencing these exact versions.

**Forth VM Strategy (using Dusk refs):**
- Start hosted: Implement a simple Forth interpreter/VM *in the host language* (C# class `TritiumForthVM`, Kotlin object) that provides the minimal primitives Dusk kernel requires (dup/drop/swap/over/rot, + - and/or/xor, @ ! c@ c! , w, c, , lit, exit, create, here, etc.).
- Load text sources via `evaluate` / `include` equivalent.
- Use Dusk's "kernel provides cold boot + interpret + basic words; xcomp/boot builds the rest".
- Later: Optimize (threaded code), or adopt/adapt Dusk's bytecode VM ideas from posix/vm.c for a portable Tritium VM that both hosts can use.
- This matches "thin bootstrap stubs" in spec + Dusk POSIX VM as "gateway".

**Storage model (Dusk fs/ + io/blk inspiration, but simplified):**
- Immutable core in bundle (assets / published content).
- Writable evolve/ + overrides in platform app-private dir (survives updates, no root needed).
- Future: Block-like or FAT emulation if we want CollapseOS-style blk editor, but start with normal files for .fs and .trit (counted strings or JSON for now).

## 3. Bootstrap / First-Run Flow (Unified, Platform HAL Differences Hidden)
1. App launch → check for `evolve/assistant-name.trit` in platform storage.
2. If missing: License dialog (scaffold >=8), Name assistant dialog (1-32 chars, user-chosen), Edition dialog (64 magenta? or 32 cyan) → write name + edition.trit.
3. Set title: "{name} — powered by TritiumOS".
4. **Cold boot the Forth core:**
   - Locate bundled core (Android: assets/core/ via AssetManager → temp copy or direct stream; Windows: poly/core/ or tritium.poly/ next to exe).
   - Host VM initializes (stacks, dict, SYSVARS with edition + name as counted string).
   - Load `boot.fs` → `trit.fs` → `tritium-kernel.fs` (sets ARCH from edition, prints "Tritium core").
   - Run any init (drena initial graph? rekia vocab?).
5. Log: "Core booted. Type help. Forth sources: ..."
6. REPL ready: input → host sends to VM `evaluate(line)` or word-by-word → output to log.
7. Default unknown → still "scaffold" but now can be real Forth error or assistant hook.

On subsequent runs: load name/edition, cold-boot Forth (fast), REPL.

**Edition impact:** ARCH constant (affects later neuron id width in DRENA). UI branding (cyan/magenta) stays in host.

## 4. Key Components & Design Decisions
### 4.1 Host Apps (Current → Target)
- **Keep simple REPL + compute commands** initially.
- Add "forth" or direct input to VM.
- Add "core-reload", "words", "drena-stats", "rekiA-test" as we build.
- **Android (komodo) specifics:**
  - Bump compileSdk/targetSdk to 35 (Android 15) or 36 for Pixel 9+.
  - minSdk 26 still fine (Pixel is 14+).
  - Use `AssetFileDescriptor` + copy to cache for core if needed, or stream-evaluate.
  - `filesDir` for evolve (already good).
  - Consider `WorkManager` for background compute/refine if long-running.
  - Permissions: none extra for basic (scoped storage). Later for USB/serial if bit-banging.
  - Large RAM (16GB) → generous dict/heap in VM.
- **Windows 11 specifics:**
  - Keep single-file + SelfContained win-x64 (add win-arm64 publish profile later).
  - Persistence already fixed to LocalApplicationData\TritiumOS\evolve.
  - For bundled poly: during publish, contents go alongside or extracted; VM must handle both dev (bin/Debug/poly) and installed (next to .exe or in %temp% extract — prefer sidecar files).
  - Use `System.IO` + perhaps `Microsoft.Extensions.FileProviders` for embedded.
  - UI: enhance TextBox log (perhaps RichText for colors later).

### 4.2 Forth Core & VM (Dusk-inspired)
- **tritium-kernel.fs** (already seeded): Expand with real words following Dusk kernel.txt exactly (cold boot, interpret loop skeleton, entry creation, basic stack/arith/mem, ARCH, SYSVARS).
- Primitives: Implement in host VM (start with stack machine in C#/Kotlin; map to .NET/Java primitives for speed).
- Dictionary: Follow Dusk mem/dict.fs (small ENTRYSZ, xt>e, words listing, units for "GROUP-xxx" namespaces and neural groups).
- Later additions: trit math fully, neuron: struct (header 16-bit as 4 nibbles, id, links ptr, group, rekia-cache), drena words, rekia emitter.
- Boot sources order: boot.fs (minimal) → trit.fs → tritium-kernel.fs → (future drena.fs rekia.fs user init).
- Self-hosting aspiration: Once running, user (or R.E.K.I.A.) can define new words that persist in evolve/refined/.

**VM Interface sketch (to implement in hosts):**
```kotlin / csharp
interface TritiumVM {
    fun coldBoot(edition: Int, assistantName: String, coreSources: List<String>)
    fun evaluate(line: String): String  // or stream output
    fun defineWord(name: String, body: String)
    // hooks for host: fileRead, fileWrite, computeCall, etc.
}
```

### 4.3 Persistence & Data
- evolve/ always in platform-private writable (AppData Local / filesDir). (Already fixed for Win.)
- Core immutable in bundle.
- Compute config: prefer user copy in evolve/qd/ or filesDir/qd/, fall back to bundle (current logic good; enhance for komodo/Win11).
- Future: encrypted user-graph if sensitive.

### 4.4 Compute / Quantum Integration
- Already working (spawns ps1/py or direct in some cases).
- Expose as Forth words later (e.g. `compute-bell` that runs the test and pushes counts).
- On komodo: Tensor could accelerate local sims (Aer via some ML? or custom), but keep current backends.

### 4.5 Build & Packaging
- `tools/build-windows.ps1`: unchanged (dotnet publish includes poly + qd).
- `tools/build-android.ps1`: Already improved sync; bump SDK in build.gradle.kts; ensure `tritium-kernel.fs` + any new core files are in assets (via the * copy or explicit).
- `tools/build-poly.ps1`: zip the poly (will include new kernel if placed in tritium.poly/core or symlinked).
- Manifest: update version, add "initial_platforms": ["windows11", "android-komodo"] or similar.
- Add platform READMEs: `install/hosts/windows/README-Win11.md`, `install/hosts/android/README-komodo.md`.

### 4.6 Security / Licensing / Future
- Current: scaffold license.
- Later: embed validator, device fingerprint (for Win: COMPUTERNAME + more; Android: ANDROID_ID + build props, careful with privacy on komodo).
- Single-user per install (per spec "per profile").

## 5. Risks & Mitigations (Platform-specific)
- **Single-file extraction (Win):** Data loss if using BaseDir → mitigated (using LocalAppData).
- **Android assets immutable + large core:** Copy core to cache on first boot if needed for "include" that requires writable? Or implement in-memory evaluate from asset streams.
- **Performance on Tensor G4:** Forth is fast enough even interpreted; 16GB RAM plenty for evolving graph. Pure math REKIA should be lightweight.
- **Permissions / scoped storage (Android):** Use only private dirs; for future "bit banging" on komodo use USB OTG + appropriate perms.
- **.NET version on Win11:** Self-contained → no host runtime dep.
- **Forth bootstrap complexity:** Use Dusk's proven "tiny kernel + xcomp build-up" to avoid big-bang interpreter. Start with hosted eval of text sources.
- **Komodo-specific:** Stock Android, no custom kernel needed for app. If later porting Dusk-style bare, use LineageOS etc. for komodo (community exists).

## 6. Implementation Roadmap (for these platforms)
1. **Forth VM skeleton in hosts** (high priority): Add TritiumVM in C# and Kotlin. Make REPL feed it. Load the 3 core .fs files. Implement enough primitives so "1 2 + . " works, then the current trit words.
2. **Expand kernel.fs** using Dusk refs (dict, units, more primitives, ARCH handling).
3. **Wire first real command:** e.g. "drena-spawn 0" that creates a neuron entry (using dict patterns) and shows header.
4. **REKIA stub that emits:** Simple word that on input writes a .fs to evolve/refined/test.fs with a colon def and includes it.
5. **Bump Android SDK + test on komodo emulator or device.**
6. **Polish bootstrap + status** to report "Forth core: booted, neurons: X, refined words: Y".
7. **Docs:** Fill NEURON.md / DRENA.md using Dusk dict + Tritium spec §3.
8. **Build verification:** Run builds, install .apk on komodo (or emulator), run .exe on Win11, exercise first-run + REPL + compute.
9. **Next after basics:** Full trit math + neuron header packing, minimal DRENA graph, license integration, etc.

## 7. Open Questions / Decisions for User/Draco
- Exact Forth threading model (direct/indirect threaded, or hosted eval first)?
- Should the host VM be a full port of Dusk's VM or custom for Tritium primitives (trit ops, neuron alloc)?
- Future: Run Dusk itself inside TritiumForth for tools (via compat layer)?
- UI evolution: Keep simple text REPL or add grid-like (Dusk has gr/) for assistant "desktop"?
- On komodo: Leverage Tensor for REKIA math acceleration once pure-math core is solid?

## 8. Deliverables from this Design
- This document (living; update as we implement).
- Updated build files + host code skeletons (in follow-up changes).
- Enhanced `forth/tritium/kernel.fs` and bundle copies.
- Cross-refs in existing docs and code.
- (Future) Platform-specific HAL .fs files under `forth/tritium/hal/` (android-komodo.fs, windows11.fs) modeled on Dusk arch/hal/.

This design ensures the initial platforms are solid, use the powerful DuskOS reference material for the critical Forth base, and align with the overall TritiumOS vision of a personal, evolving, Forth-native intelligence system.

**Next action:** Implement the host VM + load the kernel seed on one platform (recommend Windows C# first for fast iteration), then port to Android komodo.

## Forth-to-C# Bootstrap, Assimilation, and Full-Stack Host OS Optimization (Confirmed Architecture)
All current OS host implementations are bootstrapped in C# (the Windows `TritiumForthVM` + `Program.cs` is the primary detailed reference implementation of this layer). 

Pure Forth (the Trit intelligence engine from `drena.fs` + `rekia.fs`, plus future kernel) executes *inside* the C# layer ("forth to c#"). The C# VM provides:

- The execution environment and primitives (stack, HERE memory for DRENA neuron blocks, dictionary for words)
- Host bridges: `host-pwd`, `host-list-dir`, `host-read-file`, `host-exec`, `host-hw-info`, `host-evolve-dir`
- Assimilation entry points: `assimilate-host-dir`, `assimilate` (scans strategic host locations containing software written for the hardware — System32, Program Files, configs, scripts, live systeminfo/wmic queries — reads text/config/source artifacts, writes timestamped `.ingest` files under `evolve/assimilated/`)
- Host bootstrap/optimization: `bootstrap-host`, `full-stack-optimize` (after engines + assimilation, emits host-specific optimization plans, runnable `.ps1` / shell scripts, and refined Forth modules under `evolve/bootstrap/` + `evolve/forth/refined/` that the intelligence can load on subsequent boots)

This directly implements the user's confirmation:

> yep all current os is bootsraped in c# so forth to c# to alow it to asimilate all the sofware riten for the hardware its launched on with the ability to bootstrap its host os to full stack opimize the system

**Flow enabled:**
1. Cold boot loads core (DRENA blocks + REKIA math become live words).
2. Run `drena-demo` / `rekiA-demo` (or real user-driven neuron creation + refinement).
3. `assimilate` (or `full-stack-optimize`) uses the bridges from within Forth (or host REPL cmds) to ingest host software artifacts.
4. REKIA (or its C# simulation/emission) refines ingested + DRENA graph state into new runnable `: word ... ;` modules written to evolve/forth/refined/ (auto-includable).
5. `bootstrap-host` emits concrete host optimization artifacts (power plans, service notes, maintenance scripts, L.I.N.E.O.S. roadmap notes) + a corresponding Forth module.
6. The cycle repeats: refined Forth can call back into host-exec / assimilate for deeper optimization. Over iterations the system full-stack optimizes itself and can evolve toward L.I.N.E.O.S. (Forth layer becoming the primary personality of the host).

**Mirroring on other hosts (minimal interface):**
- Linux `.AppImage` (native C `tritiumos.c`): provides `assimilate_host_software()`, `bootstrap_host_optimization()`, `full_stack_demo()` + REPL commands `assimilate` / `bootstrap-host` / `full-stack-optimize`. Uses `$HOME/.tritiumos/evolve/...` (assimilated/, bootstrap/, forth/refined/). Same conceptual model even though immediate bootstrap layer is C not C#.
- Android/komodo (Kotlin host): will expose equivalent methods (scan Context assets + external dirs + package info, write to filesDir/evolve, emit .kts or script notes + refined .fs). Follows the C# reference for behavior.

**Why C# bootstrap layer is powerful here:**
- Easy full access to Win32/.NET surface (the "software written for the hardware").
- Can exec anything, read the registry/configs, list processes, etc. safely from user profile.
- The Forth intelligence stays pure and portable; the thin C# (and mirrors) just supply the assimilation surface and a place for emitted host actions to run.
- Matches DuskOS hosted "posix/vm.c" pattern: small host VM/gateway around the real kernel.

**Current concrete state (post-implementation):**
- Windows: fully working `AssimilateHostDirImpl` + `AssimilateHostSoftwareImpl` + `BootstrapHostOptimizationImpl` (write real files, use real dir/exec, emit plans + ps1 + .fs modules).
- Linux C: parallel native impls using opendir, system for queries, fopen writes, mkdir for subdirs, emit .ingest + .sh + .fs.
- Exposed as both host commands and Forth words so the intelligence itself can drive the loop (`full-stack-optimize` chains the demos + assimilation + bootstrap for a one-command cycle).
- evolve/ is the writable side the system uses to persist assimilated knowledge and its own bootstrap plans (survives single-file publish on Windows, AppImage runs on Linux).
- **Refined module persistence (added in "go")**: After any emission during assimilation/bootstrap, `LoadRefinedModules()` (in the Windows host entry point) scans `evolve/forth/refined/*.fs`, reads them, and feeds to the VM's `Interpret`/`Evaluate`. This happens automatically on boot and immediately after the commands that write new .fs. The emitted words (e.g. `host-assimilated`, `host-optimize`) become live Forth extensions without manual include. Same conceptual load step applies to the native Linux C host on next .AppImage launch.

This closes the "bootstrap its host os to full stack optimize" capability as the enabling mechanism for the on-demand assistant vision.

## Linux .AppImage (End Product — On-Demand Intelligent Assistant)
The end project is an **on-demand intelligent assistant** that full-stack refines the hardware (DRENA data blocks for the neuromorphic graph + REKIA pure-math refinement of intelligence into runnable Forth) and assists the user (REPL, tasks, co-evolution with the engines).

For Linux: delivered as a portable **.AppImage** (the "on demand" format — self-contained, no installation, runs on most modern Linux distros without root or deps).

- Host: `install/hosts/linux/tritiumos.c` (native C REPL/VM bridge mirroring the C# reference; bootstraps the poly core from the bundle, runs the engines, provides assimilation + bootstrap primitives).
- Build: `tools/build-linux.sh` (gcc -static into AppDir with only the poly core + native binary + desktop/icon/AppRun; packages with appimagetool into `dist/TritiumOS.AppImage`. Strictly no Python anywhere in the final artifact or build inputs for the AppImage).
- The Linux delivery matches the confirmed architecture: native bootstrap layer around the shared Forth (DRENA/REKIA) intelligence. C# is the detailed "current OS" reference implementation for the forth-to-C# assimilation + full-stack host optimization loop.
- See BUILD.md for full instructions. The .AppImage provides the complete assistant experience: first-run (name, edition), REPL with engine demos, hardware refinement via the data blocks + math, assistance to the user.
- GrapheneOS refs (cloned) can inform any hardware-specific extensions inside the AppImage (e.g., sensor access for "refining hardware" on Linux systems that support it).

This completes the polyglot vision: .exe (Win11), .apk (komodo), .AppImage (Linux on-demand).

Slogan: *The line tread between madness and genius.*
TritiumOS by Draco.
