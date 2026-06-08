needs tests/harness mem/stream
testbegin
create mymem ,"hello there!"
mymem 12 newmemstream const m
here 12 m read#
mymem here 12 c[]= #
m rewind
"foobar" dup m puts ( s )
c@+ ( a len ) mymem rot> c[]= #
m pos 6 #eq
m rewind
here 6 m read#
"foobar" here 6 #s[]=
testend
