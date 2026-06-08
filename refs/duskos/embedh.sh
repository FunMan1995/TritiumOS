#!/bin/sh

printf "unsigned char $1[] = \""
hexdump -v -e '"\\" "x" 1/1 "%02X"'
printf "\";"

