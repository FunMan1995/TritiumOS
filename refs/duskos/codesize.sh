#!/bin/sh
DUSK=./dusk
make -s -C usermode dusk || echo "Usermode dusk build failed, using POSIX"
# Usermode dusk is orders of magnitude faster than POSIX Dusk.
# when available, use it.
if [ -e usermode/dusk ]; then
	DUSK=./usermode/dusk
fi
$DUSK -f fs/bench/codesz.fs
