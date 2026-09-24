#!/bin/bash
# Pack tritium.poly into dist/tritium.poly.zip (Linux/macOS parity for build-poly.ps1)
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
SRC="$ROOT/tritium.poly"
DIST="$ROOT/dist"
ZIP="$DIST/tritium.poly.zip"
mkdir -p "$DIST"
rm -f "$ZIP"
python3 - <<PY
import zipfile
from pathlib import Path
src = Path("$SRC")
out = Path("$ZIP")
with zipfile.ZipFile(out, "w", zipfile.ZIP_DEFLATED) as z:
    for p in sorted(src.rglob("*")):
        if p.is_file():
            z.write(p, p.relative_to(src))
print(f"OK: {out} ({out.stat().st_size} bytes)")
PY
