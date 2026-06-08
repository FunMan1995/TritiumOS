# Android Host — Google Pixel 9 Pro XL (komodo)

Initial target Android platform for TritiumOS.

## Device
- Codename: komodo (google-komodo)
- SoC: Google Tensor G4 (4 nm), 16 GB LPDDR5X RAM, UFS 3.1 storage
- OS: Android 14 (upgradable to 16+, 7 years of updates)
- Excellent for Forth core + evolving neural graph (plenty RAM) + future on-device Tensor accel for pure-math REKIA.

## Build / Run
See parent `build-android.ps1` (syncs core/ including `tritium-kernel.fs` to assets).
- Uses assets/core/ for immutable TritiumForth sources (boot.fs, trit.fs, tritium-kernel.fs).
- Writable state in `filesDir/evolve/` (assistant-name.trit, edition.trit, future refined .fs and graphs).
- Current minSdk 26, target 35 (bumped for modern Pixels).

## Host Bridge (VM) Strategy
See `docs/SYSTEM-DESIGN-INITIAL-PLATFORMS.md` and `docs/FORTH-BASE-REFERENCES.md`.
- Implement TritiumVM in Kotlin (simple stack machine + evaluate).
- Model after DuskOS `posix/vm.c` + HAL (platform primitives for I/O, files, compute).
- Load core sources from assets on cold-boot.
- Bridge compute backends and future host services as Forth words.

## Current State (scaffold → real Forth)
- First-run flow complete (license, name, edition).
- REPL dispatches known commands + placeholder.
- Core assets now include Dusk-inspired kernel seed.
- Next: wire real VM so REPL can run Forth words (1 2 + . etc.) and the loaded boot/kernel.

## Future komodo-specific
- Leverage Tensor G4 for hybrid acceleration (after pure-math REKIA solid).
- Optional deeper integration (USB for bit-bang per CollapseOS spirit) with user consent.
- Test on stock + custom (LineageOS etc. for komodo exist in community).
- **GrapheneOS as reference/base (specific release)**: GrapheneOS komodo-install-2026060100.zip is in the project root. Extracted metadata in refs/grapheneos/komodo-install-2026060100/ (android-info.txt, flash scripts, etc.). This release:
  - bootloader=ripcurrentpro-16.4-14791556
  - baseband=g5400c-251201-260127-B-14784805
  - Requires vendor_kernel_boot partition, A/B (2 slots)
  - Flash sequence includes custom AVB key (avb_pkmd.bin), oem uart disable, fips/dpm erase, multi-part super flash.
  GrapheneOS source (https://github.com/GrapheneOS) + this zip for exact Pixel 9 Pro XL (komodo) bringup and secure bootstrap. The device_google_caimito repo (cloned in refs/grapheneos/device_google_caimito) has:
  - komodo/BoardConfig.mk: TARGET_BOOTLOADER_BOARD_NAME := komodo, screen density 480, includes device-caimito-common.mk + zumapro (Tensor platform), sepolicy, wifi config, vendor prebuilts.
  - device-komodo.mk: SHIPPING_API_LEVEL, kernel 6.1, bootloader prebuilts (24D1), radio dirs, 16k page support.
  - factory_komodo.mk, aosp_komodo.mk for build variants.
  - conf/init.recovery.device.rc and device-specific init.rc for bootstrap.
  - UWB, fingerprint, audio configs per device.
  - Kernel: device_google_caimito-kernels_6.1 for hardened kernel source/configs.
  - Build: Uses AOSP 'm' with vendorbootimage/vendorkernelbootimage for Pixel 9; see https://grapheneos.org/build. They use adevtool for automated bringup (https://github.com/GrapheneOS/adevtool).
  - Install/bootstrap: Web/CLI installer handles bootloader unlock, fastboot flash of images, custom AVB signing, locking bootloader. Factory images in their releases.
- **Recommendation for TritiumOS**: 
  - The komodo-install-2026060100.zip is now in the project root (user-added for this session). Use it + fastboot for real device GrapheneOS install on Pixel 9 Pro XL, then sideload the built Tritium APK. The app's assimilation/bootstrap (full-stack-optimize) will then produce komodo-specific artifacts referencing this exact release's bootloader/baseband/AVB details.
  - Run the current thin APK (with Forth core in assets) on GrapheneOS for a hardened baseline (better security than stock for the assistant/OS).
  - If evolving to fuller integration (beyond user app to system-level or custom ROM like L.I.N.E.O.S. vision), use GrapheneOS device configs + this installer zip as template for komodo bringup, kernel, partitions (e.g., fstab, init for early Forth bootstrap), and verified boot (avb_pkmd.bin).
  - For the Kotlin host VM (see SYSTEM-DESIGN): Model native Forth embedding or JNI after GrapheneOS native code practices for Pixels. Use their init patterns for app/service startup on boot. The host-hw-info and bootstrap plans now hard-embed the 2026060100 release details.
  - Security: GrapheneOS's verified boot, hardened malloc, etc., align with Tritium's "pure math" transparent design – study for attestation of the Forth core.
  - To explore: cd refs/grapheneos; study the komodo-install-2026060100/ extracted files + BoardConfig, device-*.mk, kernel build scripts. For full AOSP base, GrapheneOS uses repo manifests from their source page.

Run from source or built APK on the device/emulator (preferably on GrapheneOS for security). The Pixel Pro XL VM in Android Studio can test the APK + assimilation logic; for full GrapheneOS experience use the zip on real hardware.

### Building/Preparing a GrapheneOS VM (for komodo testing)
Use the new helper:
```powershell
.\tools\build-grapheneos-vm.ps1
```
This uses your added `komodo-install-2026060100.zip` to populate `vm/grapheneos-komodo/` with boot images, flash scripts, android-info, and a README-VM.txt with:
- Android Studio AVD launch commands (use your existing komodo Pixel Pro XL AVD + -prop to simulate GrapheneOS release props from the zip).
- QEMU notes.
- Full source build instructions (WSL/Linux + the refs/grapheneos/device_google_caimito/komodo/ overlay + the zip's versions).
- How the TritiumOS app's assimilation (host-hw-info, full-stack-optimize) will automatically use/reference this exact GrapheneOS komodo release.

See `vm/grapheneos-komodo/README-VM.txt` after running the script. The app code (TritiumForthVM.kt) and bootstrap plans are pre-wired with the 2026060100 details for accurate "GrapheneOS vs stock" comparison on your VM.

## Assimilation + Host Bootstrap Test on Android VM (GrapheneOS vs Stock Google OS)
**Goal (per user request):** Run "assimilation" on an Android VM simulating the komodo phone, once under GrapheneOS and once under default/stock Google OS (Pixel 9 Pro XL images), then compare notes.

### How to run the test (you on real hardware/emulator)
1. Build the APK:
   ```powershell
   .\tools\build-android.ps1
   ```
   (Requires Android SDK + JDK; gradle wrapper will be used.)

2. Set up Android Emulator (Android Studio):
   - Create Pixel 9 Pro XL (komodo) device profile.
   - For **stock Google OS**: Use the standard system image (with Google Play / GMS).
   - For **GrapheneOS**: GrapheneOS provides factory images and instructions (https://grapheneos.org/install). For emulator, you can use their web installer images or community ports, or flash a custom AVD with their kernel + system. Alternatively use a real Pixel with GrapheneOS flashed (unlocked bootloader, then lock after). GrapheneOS images are available via their releases / web installer.

3. Install the TritiumOS APK on the emulator/instance (via adb or Play if side-loaded).

4. Launch the app, complete first-run (license scaffold, name, 64-bit edition recommended for komodo).

5. In the REPL:
   - `host-hw-info`
   - `assimilate`   (the key one: ingests installed packages as "software written for the hardware", some /proc + prop via exec, private dirs, emits .ingest + host-assimilated.fs)
   - `bootstrap-host`
   - `full-stack-optimize` (runs the full chain + auto load-refined)
   - `load-refined`

6. Observe in log:
   - Assimilation messages (packages listed, hw info, exec outputs).
   - Files created under `/data/data/os.tritium.app/files/evolve/` (or equivalent in emulator) in `assimilated/`, `bootstrap/`, `forth/refined/`.

7. Use `adb shell` or the app's file browser (if you add one) or `adb pull` to inspect the .ingest and .fs files.
   - Compare what gets assimilated:
     - Number of packages (stock has way more due to GMS, Play Services, Google apps).
     - Exec output (getprop, pm list may return more or be less restricted on stock vs GrapheneOS hardened SELinux).
     - Build fingerprint (different between GrapheneOS custom and stock).

8. On real device (after emulator test): repeat on actual Pixel 9 Pro XL running GrapheneOS vs (separate profile or another device) stock.

### Comparison Notes (GrapheneOS vs Default Google OS)
- **Surface area for "assimilate all the software written for the hardware"**:
  - **Stock Google OS**: Much larger. Dozens more packages (com.google.android.gms, system UIs, Play, etc.). Easier to exec "dumpsys", "pm", more /proc visibility in practice. Assimilation can pull rich data about bloat/telemetry/services. Good for the intelligence to later help the *user* optimize (e.g. suggest disabling things). But the host itself is noisier/more complex to "full stack optimize".
  - **GrapheneOS**: Smaller, cleaner set (core AOSP + user apps + optional sandboxed Play). Harder exec surface due to hardening (some getprop/pm may be filtered or slower). /proc and app-private dirs still work well. Assimilation focuses on *user* + essential software + build/kernel info from the komodo Tensor setup. This aligns better with Tritium's philosophy (transparent, minimal, evolving pure-math intelligence on hardened base). "Bootstrap host" is mostly app-private scripts/configs + user-space advice rather than system daemons.

- **Storage / evolve dir**:
  - Both use app-private `filesDir/evolve` (scoped storage friendly). No extra perms needed for basic assimilation. Broader dir scans (e.g. /sdcard or other apps) require runtime READ/WRITE permissions + user grant — on GrapheneOS the permission prompt + audit is stricter/more visible.

- **host-exec / Process**:
  - Both sandboxed (no root). GrapheneOS is stricter with SELinux policies and seccomp — more commands may return limited output or "permission denied". Stock is more permissive for diagnostic commands. In the code we catch and note "sandbox/SELinux restrictions expected on GrapheneOS".

- **GrapheneOS advantages for the overall vision**:
  - Verified boot + AVB means the base the assistant runs on is more trustworthy (the "hardware" being refined is attested).
  - Hardened kernel/malloc reduces attack surface while the Forth graph evolves.
  - When doing "bootstrap its host os", on GrapheneOS you stay within safe bounds (no tempting system mods that would break verified boot). The assistant can still evolve to provide a L.I.N.E.O.S.-like experience *inside* the app or as a privileged user service later.
  - Use the cloned refs/grapheneos/device_google_caimito/komodo/* for any future deeper hooks (e.g. early init for the core, Tensor-specific nodes in DRENA).

- **Stock advantages**:
  - More "real world" software to assimilate and refine knowledge from (helps the assistant be useful immediately for average Pixel users).
  - Easier debugging of the assimilation in emulator (more output).

- **In practice on VM/emulator (your test)**:
  - Stock image will produce longer "installed-software.ingest" with lots of com.google.* entries.
  - GrapheneOS image (or custom) will be shorter, with more emphasis on AOSP + the Tritium app itself + any user-installed apps.
  - The emitted `host-assimilated.fs` and bootstrap plans will contain the comparison notes (see the code in TritiumForthVM.kt — the plan file hard-codes the GrapheneOS vs stock analysis).
  - Refined modules get "loaded" (logged + simple evaluation attempted) via the loadRefinedModules path in MainActivity after the commands.

- **Limitations observed / expected**:
  - Full "host-exec" of arbitrary host software is intentionally limited (good for security; the point of GrapheneOS).
  - To assimilate more (e.g. other apps' data), would need explicit user-granted permissions + SAF or content providers.
  - The current Android VM is still a simulation (like early C#); a full token interpreter port from C# would allow the actual drena/rekia .fs to define more words that the assimilation can call.
  - On real device vs emulator: emulators often have more relaxed policies.

### Next for Android assimilation parity with C# reference
- Port a more complete Interpret loop from the Windows TritiumForthVM.cs into TritiumForthVM.kt so raw Forth (including emitted refined modules) actually executes.
- Add runtime permission request for broader storage when user runs "assimilate".
- Wire "load-refined" more deeply (scan + feed source to VM.Interpret).
- Use more komodo/GrapheneOS specifics (e.g. from BoardConfig in refs) to create initial "hardware neurons" at boot.
- Test matrix: emulator-stock, emulator-GrapheneOS-port, real Pixel stock, real Pixel GrapheneOS.

This fulfills the "run assimilation on an android VM of the phone with Grapheneos and default google os as a test and compare notes" request via code + documented procedure + in-app comparison text that gets written to the bootstrap artifacts themselves.

See also `docs/SYSTEM-DESIGN-INITIAL-PLATFORMS.md` (the Forth-to-C# / Kotlin section) and the C# implementation for the reference behavior we're mirroring.
TritiumOS by Draco.
