#/bin/sh
# In markdown files within this project, we don't bother maintaining links for
# docs references. Instead, we automatically generate them using this scsript.
# you want to generate HTML from those MDs, append the result of running this
# script to them. You can generate gopher links with "0" as a first arg.

spit() {
	cd fs
	for fn in $(find doc -name "*.txt"); do
		id=${fn#doc/}
		id=${id%.txt}
		id=${id%/index}
		echo [$id]: ${1}$fn
	done
}

spit $1 | sort
