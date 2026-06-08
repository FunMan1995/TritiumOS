#!/bin/sh
# Usage: ./fatpush.sh /dev/sdX
# The mirror of fatfetch.sh. Copies whole FS into the target, but preserve
# init.fs
mcopy -i $1 -D o -ns fs/* ::
