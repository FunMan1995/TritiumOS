#!/bin/sh
# Usage: ./fatfetch.sh /dev/sdX
# Invokes mcopy in a way that handles upper/lower conversion.
# I never remember the arguments, so here we go.
MTOOLS_LOWER_CASE=1 mcopy -i $1 -ns ::* fs
# Remove the usual "polluters"
rm -rf fs/efi
rm -f fs/init.fs fs/init.txt fs/*.bin fs/*.elf fs/*.img
