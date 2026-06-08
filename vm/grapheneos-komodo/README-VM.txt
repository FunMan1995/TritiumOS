# GrapheneOS komodo VM for TritiumOS Testing

Prepared from komodo-install-2026060100.zip + refs/grapheneos/

## Quick Start (Android Studio Emulator - Recommended)

1. Build the TritiumOS Android app (includes GrapheneOS komodo awareness):
   .\tools\build-android.ps1

2. Find your AVD name:
   Open Android Studio > Device Manager, note the name of your Pixel 9 Pro XL AVD (e.g. Pixel_9_Pro_XL_API_35).

3. Launch the emulator with GrapheneOS simulation props (from the zip android-info.txt):
   Use this command in PowerShell (adjust AVD name):

   $emulator = "C:\Users\happy\AppData\Local\Android\Sdk\emulator\emulator.exe"
   & $emulator -avd Pixel_9_Pro_XL_API_35 -prop ro.product.device=komodo -prop ro.product.model="Pixel 9 Pro XL" -prop ro.build.fingerprint="google/komodo/komodo:15/AP2A.240905.003/14791556:user/release-keys" -prop ro.build.tags=release-keys -prop ro.build.type=user -prop ro.bootloader=ripcurrentpro-16.4-14791556 -prop ro.baseband=g5400c-251201-260127-B-14784805

   This makes getprop return GrapheneOS-like values. The Tritium app will report the exact komodo GrapheneOS details.

4. Once emulator is running (wait for full boot):
   & "C:\Users\happy\AppData\Local\Android\Sdk\platform-tools\adb.exe" install -r (path to your TritiumOS.apk)   # e.g. from dist after build

5. Open the app.
   Test commands like:
     host-hw-info
     assimilate
     full-stack-optimize
   It will behave as if on GrapheneOS komodo.

## Compare to Stock Google OS
Launch a separate AVD (or same without the -prop flags, or use a Google Play system image AVD).
Run the same test commands to see differences (e.g. more packages assimilated on stock).

## Notes
- The app code is pre-configured for this specific GrapheneOS komodo release from your zip.
- For real GrapheneOS on hardware: use the zip with fastboot (see flash-all.bat in this dir).
- Full custom build from source: use WSL + the refs/grapheneos/ device files.
- This setup allows testing the "assimilate all the software... bootstrap host os" on GrapheneOS vs stock in your VM.

See android-info.txt and script.txt in this dir for exact values used.

