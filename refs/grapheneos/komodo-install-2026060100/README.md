# GrapheneOS komodo-install-2026060100 (Pixel 9 Pro XL / komodo)

This is the offline installer package added to the TritiumOS project root.

## Key metadata (from android-info.txt)
- board=komodo
- version-bootloader=ripcurrentpro-16.4-14791556
- version-baseband=g5400c-251201-260127-B-14784805
- partition-exists=vendor_kernel_boot

## Flash sequence highlights (from script.txt + flash-all.*)
- Dual-slot bootloader flash + active slot toggle
- Flash radio
- erase avb_custom_key ; flash avb_custom_key avb_pkmd.bin  (GrapheneOS custom verified boot key)
- oem uart disable
- erase fips ; erase dpm_a ; erase dpm_b
- set_active:a
- Flash: boot, init_boot, dtbo, vendor_kernel_boot, pvmfw, vendor_boot, vbmeta
- Erase userdata + metadata
- Flash super in 15 split images

## Usage in TritiumOS
- Referenced in Android host assimilation/bootstrap code (host-hw-info reports expected versions; bootstrap plans include exact flash details and comparison notes).
- See install/hosts/android/README-komodo.md and docs/SYSTEM-DESIGN-INITIAL-PLATFORMS.md
- For real device: fastboot flash using the scripts in this zip, then install the Tritium APK built from install/hosts/android. The app can then assimilate the GrapheneOS komodo host and emit refined modules aware of this release.

Extracted for reference by the project (do not commit the full 1.7GB zip).
