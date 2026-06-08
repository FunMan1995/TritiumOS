needs tests/harness io/blk io/part
testbegin

variable ncalls
variable lastr
variable lastw
:> 2drop lastr ! 1 ncalls +! ;
:> 2drop lastw ! 1 ncalls +! ;
8 8 newblk const refblk \ 8x 8 bytes blks

\test regular usage
3 4 refblk newpart const mypart
ncalls @ 0 #eq
here 4 mypart read# \ load blk 3
ncalls @ 1 #eq
lastr @ 3 #eq
here 4 mypart read# \ still on the same blk
ncalls @ 1 #eq
here 4 mypart read# \ load blk 4
ncalls @ 2 #eq
lastr @ 4 #eq
here 4 mypart write# \ write in buffer
ncalls @ 2 #eq
lastw @ 0 #eq
mypart flush
ncalls @ 3 #eq
lastw @ 4 #eq

\test reframe
1 4 mypart reframe
here 4 mypart read# \ load blk 1
ncalls @ 4 #eq
lastr @ 1 #eq
testend
