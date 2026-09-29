#!/usr/bin/env bash
# Optional Android host-parity stub (wave8 item 4 / docs/HOST-PARITY.md)
# Prints CONTRACT marker from HOST-PARITY.txt. Never requires SDK/emulator.
set -euo pipefail
DIR="$(cd "$(dirname "$0")" && pwd)"
PARITY="$DIR/HOST-PARITY.txt"
if [[ ! -f "$PARITY" ]]; then
  echo "[host-parity] android FAIL"
  echo "[host-parity-stub] FAIL"
  exit 1
fi
# Tip policy: Android surface is CONTRACT-only without SDK (Lab OK).
echo "[host-parity] android CONTRACT"
echo "[host-parity-stub] OK"
