#!/bin/sh
case $(uname -m) in
	i386) echo i386;;
	amd64|x86_64) echo amd64;;
	*arm*) echo arm;;
	*) echo unknown;;
esac
