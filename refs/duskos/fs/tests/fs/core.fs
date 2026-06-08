needs tests/harness fs/core mem/blk fs/fat fs/fatt fs/sh
\ here, we test both fs/core and fs/sh at once
testbegin
\test find (and that it keeps the query string intact!)
"asm/x86.fs" dup lookup #true
lowstr "asm/x86.fs" #s=
walkname lowstr "x86.fs" #s=
walkdir? not #true
walksize #true
walkdepth 1 #eq

\test can lookup "/"
"/" lookup #true

\test we also support leading '/'
"/lib/str.fs" lookup #true

\test what if it doesn't exist?
"lib/nope.fs" lookup not #true
"/nope.fs" lookup not #true

\test File seek doesn't let us go out of bounds
"asm/x86.fs" openpath const myfile
myfile pos 0 #eq
-1 myfile seek
myfile pos myfile size #eq
myfile close

\test walk info
:~ p"lib/str.fs" ; ~ \ p" can be compiled
walkdir? not #true
walksize #true
walkname upstr "STR.FS" #s=
walkpath upstr "LIB" #s=
p"lib"
walkdir? #true
walkname upstr "LIB" #s=
walkpath "" #s=

\test after walk, walkdir? is true and walkdepth is 0
p"lib/str.fs" walkdir? not #true
bootfs walk walkdir? #true
walkdepth 0 #eq

\test file creation
\ For the writeable part of the tests, we use a memblk with a FAT12 mounted
\ in it so that write tests can be made without affecting the host.
100 const TOTSEC
512 TOTSEC newmemblk value myblk
fatopts$
myblk newFAT
myblk newfatfs const myfat
myfat 'X' mapfs
myfat walk "foo.fs" newfile
"X:foo.fs" lookup#
curfs myfat #eq
walkname "FOO.FS" #s=
walksize 0 #eq
walkdir? not #true
open const myfile

\test reading an empty file doesn't cause a IO error
here 0 myfile read 0 #eq \ no IO error

\test write into the created file
myfile FATFile.cl0 0 #eq \ no cluster allocated yet
"42" myfile puts myfile close
"x:foo.fs" lookup# open interpretstream 42 #eq

\test how about a directory?
pd"x:bar"
curfs myfat #eq
walkname "BAR" #s=
walkdir? #true

\test let's copy foo.fs in that directory
walksrc p"x:foo.fs"
walkdst pf"x:bar/baz.fs" p"x:bar/baz.fs"
copyfile
f"x:bar/baz.fs" interpretstream 42 #eq
f"x:bar/baz.fs" const myfile
1 myfile seek myfile truncate myfile close
f"x:bar/baz.fs" interpretstream 4 #eq
p"x:foo.fs" removefsnode
p"x:bar/baz.fs" removefsnode
"x:foo.fs" lookup not #true
"x:bar/baz.fs" lookup not #true

\test Can we copy a bigger file? From another FS?
walkdst pf"x:big.fs"
walksrc p"fs/core.fs"
copyfile
p"x:big.fs" walksize p"fs/core.fs" walksize #eq

\test copy a dir
\ Note that TOTSEC can be too tight. The directory that is chosen is one of the
\ smaller ones so that we don't make the test too memory intensive.
walkdst pd"x:dstdir"
walksrc p"text"
copyall
p"x:dstdir/ed.fs" walksize p"text/ed.fs" walksize #eq

\test ensureXXX
"x:path/to/fname" ensurefile
"x:path/to/fname" lookup#
walkdir? not #true
walkdepth 2 #eq
"x:other/to/dname" ensuredir
"x:other/to/dname" lookup#
walkdir? #true
walkdepth 2 #eq

\test iterpath
"foo/bar/baz" c@+ iterpath #true ( a u a u )
"foo" rot> #s[]= ( a u )
iterpath #true
"bar" rot> #s[]= ( a u )
iterpath not #true
"baz" rot> #s[]= ( )
"noslash" c@+ iterpath not #true
"noslash" rot> #s[]= ( )

\test ensurepath can use relative lookups
p"x:path" enterdir 0 "to/fname" ensurepath
p"x:path" enterdir 0 "to/other" ensurepath
p"x:path/to/fname"
p"x:path/to/other"

\test rename
p"x:path"
walkname lowstr "path" #s=
"poth" walkname strmove
writefsnode
"x:path" lookup not #true
"x:poth" lookup #true

\test whole listtree
\ nothing is asserted, just that we don't crash.
myfat walk listtree

testend
