#!/bin/sh
# Usage: ./initfetch.sh /dev/sdX
mcopy -i $1 -ns ::init.fs ::init.txt .
