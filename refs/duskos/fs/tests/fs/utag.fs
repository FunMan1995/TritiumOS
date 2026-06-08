needs tests/harness fs/utag
testbegin

\test utag>path
"lib/str" utag>path "lib/str.fs" #s=
"[lib/str]" utag>path "lib/str.fs" #s=
"[lib/str]." utag>path "lib/str.fs" #s=
"doc/lib/str" utag>path "lib/str.fs" #s=
"lib/str.fs" utag>path "lib/str.fs" #s=
"lib/str.fs" utag>path "lib/str.fs" #s=

\test utag>.txt
"lib/str" utag>.txt "doc/lib/str.txt" #s=
"lib/str.fs" utag>.txt "doc/lib/str.txt" #s=
"s)" utag>.txt "doc/lib/str.txt" #s=
"hal" utag>.txt "doc/hal.txt" #s=
"create" utag>.txt "doc/dict.txt" #s=

\test utag>.fs
"lib/str" utag>.fs "lib/str.fs" #s=
"doc/lib/str.txt" utag>.fs "lib/str.fs" #s=
testend
