#!/bin/sh
# TritiumOS Linux .AppImage (native, no-Python, on-demand portable intelligent assistant)
set -e
ROOT="$(cd "$(dirname "$0")/../../.." && pwd)"
echo "TritiumOS by Draco — Linux .AppImage (end product for on-demand use)"
echo "The end project: on-demand intelligent assistant that full-stack refines the hardware (DRENA/REKIA engines) and assists the user."
echo "Primary ship targets: TritiumOS.exe (Win11), TritiumOS.apk (komodo/Pixel 9 Pro XL), TritiumOS.AppImage (Linux portable, native C)"
echo "Build: bash tools/build-linux.sh (gcc + appimagetool, produces native dist/TritiumOS.AppImage)"
echo "The .AppImage is fully self-contained (native binary + poly core), no Python."
echo "No install needed — on-demand assistant for Linux."