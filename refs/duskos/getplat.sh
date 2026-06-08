#!/bin/sh
case $(uname -s) in
	NetBSD) echo bsd;;
	FreeBSD) echo bsd;;
	OpenBSD) echo openbsd;;
	Linux) echo linux;;
	Darwin) echo darwin;;
	*) echo other;;
esac
