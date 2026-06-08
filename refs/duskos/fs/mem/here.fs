needs mem/alloc
unit mem/here

create herealloc 12 allot
SYSALLOC herealloc 3 move

: here[ ( newhere -- oldhere ) herealloc alloc[ herealloc @! ;
: ]here ( oldhere -- ) herealloc ! ]alloc ;
