#!/bin/bash
# Build dist/TritiumOS.AppImage — on-demand Linux portable assistant for TritiumOS.
# End product: **no-python** .AppImage (native C binary + bundled poly core).
# Runs the on-demand intelligent assistant that full-stack refines the hardware (DRENA/REKIA engines)
# and assists the user. Self-contained, no Python, no external runtime deps beyond the AppImage.
#
# Run on Linux with:
# - gcc (for building the native host)
# - appimagetool in PATH (https://github.com/AppImage/AppImageKit/releases)
# - fuse (for testing)
#
# Usage: bash tools/build-linux.sh
# Output: dist/TritiumOS.AppImage

set -e

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
DIST="$ROOT/dist"
APPDIR="$DIST/TritiumOS.AppDir"
APPIMAGE="$DIST/TritiumOS.AppImage"
HOST_SRC="$ROOT/install/hosts/linux/tritiumos.c"
HOST_BIN="$APPDIR/usr/bin/tritiumos"

echo "TritiumOS Linux .AppImage builder (native, no Python)"
echo "Creator: Draco | Slogan: The line tread between madness and genius."

mkdir -p "$DIST" "$APPDIR/usr/bin" "$APPDIR/usr/share/applications" "$APPDIR/usr/share/icons/hicolor/256x256/apps" "$APPDIR/usr/share/tritium.poly"

# Bundle the poly core (source of truth for Forth + DRENA/REKIA engines)
echo "Bundling tritium.poly core..."
if [ -d "$ROOT/tritium.poly" ]; then
    cp -r "$ROOT/tritium.poly" "$APPDIR/usr/share/tritium.poly"
else
    echo "Warning: tritium.poly not found; run tools/build-poly.ps1 first."
    mkdir -p "$APPDIR/usr/share/tritium.poly/core"
fi

# Build the native C host (no Python)
echo "Building native host (gcc -static for portability)..."
if ! command -v gcc &> /dev/null; then
    echo "Error: gcc not found. Install build-essential."
    exit 1
fi
gcc -static -O2 -o "$HOST_BIN" "$HOST_SRC"
chmod +x "$HOST_BIN"

# Create desktop entry
cat > "$APPDIR/usr/share/applications/tritiumos.desktop" <<EOF
[Desktop Entry]
Name=TritiumOS
Comment=On-demand intelligent assistant (full-stack hardware refinement + user assistance)
Exec=tritiumos
Icon=tritiumos
Terminal=true
Type=Application
Categories=Utility;Development;
EOF

# Copy icon
if [ -f "$ROOT/TritiumOS_logo.jpg" ]; then
    cp "$ROOT/TritiumOS_logo.jpg" "$APPDIR/usr/share/icons/hicolor/256x256/apps/tritiumos.jpg"
else
    touch "$APPDIR/usr/share/icons/hicolor/256x256/apps/tritiumos.jpg"
fi

# AppRun
cat > "$APPDIR/AppRun" << 'EOF'
#!/bin/sh
HERE="$(dirname "$(readlink -f "${0}")")"
export PATH="${HERE}/usr/bin:${PATH}"
exec "${HERE}/usr/bin/tritiumos" "$@"
EOF
chmod +x "$APPDIR/AppRun"

ln -sf usr/bin/tritiumos "$APPDIR/tritiumos"

# appimagetool
if ! command -v appimagetool &> /dev/null; then
    echo "Downloading appimagetool..."
    wget -q https://github.com/AppImage/AppImageKit/releases/download/continuous/appimagetool-x86_64.AppImage -O /tmp/appimagetool
    chmod +x /tmp/appimagetool
    APPIMAGETOOL=/tmp/appimagetool
else
    APPIMAGETOOL=$(command -v appimagetool)
fi

echo "Building .AppImage..."
ARCH=x86_64 "$APPIMAGETOOL" "$APPDIR" "$APPIMAGE" || echo "Note: If appimagetool fails, the AppDir is still usable."

echo "SUCCESS: $APPIMAGE"
echo "chmod +x $APPIMAGE && ./$APPIMAGE"
echo "Native (no Python) on-demand Linux assistant."