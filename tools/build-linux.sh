#!/bin/bash
# Build dist/TritiumOS.AppImage — on-demand Linux portable assistant for TritiumOS.
# End product: **no-python** .AppImage (native C binary + bundled poly core).
#
# Prerequisites: gcc, file(1), wget|curl, python3 (Pillow recommended for logo).
# Packaging uses APPIMAGE_EXTRACT_AND_RUN=1 so FUSE is not required.
#
# Usage: bash tools/build-linux.sh
# Output: dist/TritiumOS.AppImage

set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
DIST="$ROOT/dist"
APPDIR="$DIST/TritiumOS.AppDir"
APPIMAGE="$DIST/TritiumOS.AppImage"
HOST_SRC="$ROOT/install/hosts/linux/tritiumos.c"
HOST_BIN="$APPDIR/usr/bin/tritiumos"

echo "TritiumOS Linux .AppImage builder (native, no Python)"
echo "Creator: Draco | Slogan: The line tread between madness and genius."

rm -rf "$APPDIR"
mkdir -p "$DIST" \
  "$APPDIR/usr/bin" \
  "$APPDIR/usr/share/applications" \
  "$APPDIR/usr/share/icons/hicolor/256x256/apps" \
  "$APPDIR/usr/share/tritium.poly"

echo "Bundling tritium.poly core..."
if [ -d "$ROOT/tritium.poly" ]; then
  cp -a "$ROOT/tritium.poly/." "$APPDIR/usr/share/tritium.poly/"
else
  echo "Warning: tritium.poly not found; run tools/build-poly.sh (or .ps1) first."
  mkdir -p "$APPDIR/usr/share/tritium.poly/core"
fi

echo "Building native host (gcc -static for portability)..."
if ! command -v gcc >/dev/null 2>&1; then
  echo "Error: gcc not found. Install build-essential."
  exit 1
fi
gcc -static -O2 -o "$HOST_BIN" "$HOST_SRC"
chmod +x "$HOST_BIN"

cat > "$APPDIR/usr/share/applications/tritiumos.desktop" <<EOF
[Desktop Entry]
Name=TritiumOS
Comment=On-demand intelligent assistant (full-stack hardware refinement + user assistance)
Exec=tritiumos
Icon=tritiumos
Terminal=true
Type=Application
Categories=Utility;
EOF
cp "$APPDIR/usr/share/applications/tritiumos.desktop" "$APPDIR/tritiumos.desktop"

ICON_DST="$APPDIR/tritiumos.png"
ICON_SHARE="$APPDIR/usr/share/icons/hicolor/256x256/apps/tritiumos.png"
export ICON_DST ICON_SHARE ROOT
python3 - <<'PY'
import os, struct, zlib
from pathlib import Path

dst = Path(os.environ["ICON_DST"])
share = Path(os.environ["ICON_SHARE"])
logo = Path(os.environ["ROOT"]) / "TritiumOS_logo.jpg"

def write_placeholder():
    def chunk(tag, data):
        return struct.pack(">I", len(data)) + tag + data + struct.pack(">I", zlib.crc32(tag + data) & 0xffffffff)
    w = h = 256
    rgb = (0, 180, 200)
    raw = b"".join(b"\x00" + bytes(rgb) * w for _ in range(h))
    data = (
        b"\x89PNG\r\n\x1a\n"
        + chunk(b"IHDR", struct.pack(">IIBBBBB", w, h, 8, 2, 0, 0, 0))
        + chunk(b"IDAT", zlib.compress(raw, 9))
        + chunk(b"IEND", b"")
    )
    dst.write_bytes(data)
    share.write_bytes(data)
    print("logo: placeholder PNG")

if logo.is_file():
    try:
        from PIL import Image
        img = Image.open(logo).convert("RGBA").resize((256, 256))
        img.save(dst)
        img.save(share)
        print("logo: JPG→PNG via Pillow")
    except Exception as e:
        print(f"logo convert failed ({e}); using placeholder")
        write_placeholder()
else:
    write_placeholder()
PY

cat > "$APPDIR/AppRun" << 'EOF'
#!/bin/sh
HERE="$(dirname "$(readlink -f "${0}")")"
export PATH="${HERE}/usr/bin:${PATH}"
export TRITIUM_POLY="${HERE}/usr/share/tritium.poly"
exec "${HERE}/usr/bin/tritiumos" "$@"
EOF
chmod +x "$APPDIR/AppRun"
ln -sfn usr/bin/tritiumos "$APPDIR/tritiumos"

if command -v appimagetool >/dev/null 2>&1; then
  APPIMAGETOOL=$(command -v appimagetool)
else
  echo "Downloading appimagetool..."
  TOOL=/tmp/appimagetool-x86_64.AppImage
  URL="https://github.com/AppImage/AppImageKit/releases/download/continuous/appimagetool-x86_64.AppImage"
  if command -v wget >/dev/null 2>&1; then
    wget -q "$URL" -O "$TOOL"
  else
    curl -fsSL "$URL" -o "$TOOL"
  fi
  chmod +x "$TOOL"
  APPIMAGETOOL=$TOOL
fi

if ! command -v file >/dev/null 2>&1; then
  echo "Error: file(1) is required by appimagetool. Install package 'file'."
  exit 1
fi

echo "Building .AppImage..."
rm -f "$APPIMAGE"
ARCH=x86_64 APPIMAGE_EXTRACT_AND_RUN=1 "$APPIMAGETOOL" "$APPDIR" "$APPIMAGE"

if [ ! -f "$APPIMAGE" ]; then
  echo "Error: AppImage was not produced. AppDir left at $APPDIR"
  exit 1
fi
chmod +x "$APPIMAGE"

echo "SUCCESS: $APPIMAGE"
echo "Run: APPIMAGE_EXTRACT_AND_RUN=1 $APPIMAGE   # (or ./$APPIMAGE with FUSE)"
echo "Native (no Python) on-demand Linux assistant."
