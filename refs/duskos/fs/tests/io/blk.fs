needs tests/harness io/blk io/stream
testbegin

create storage ,"abcdefghijklmnop" \ 4x 4b blocks

:> drop swap 4* storage + swap 4 cmove ;
:> drop swap 4* storage + 4 cmove ;
4 4 newblk const myblk

\test read
8 myblk gets "abcdefgh" #s=
myblk pos 8 #eq

\test window wr?=0
"mnop" 12 0 myblk window #s[]=
myblk pos 8 #eq

\test write
"foo!" myblk puts
myblk flush
myblk pos 12 #eq
create expected ,"abcdefghfoo!mnop"
storage expected 16 c[]= #true

\test window wr?=1
8 1 myblk window 4 #eq 'b' swap c!
myblk flush
myblk pos 12 #eq
create expected ,"abcdefghboo!mnop"
storage expected 16 c[]= #true

\test readbuf into writebuf
0 myblk seek
4 myblk readbuf dup 4 #eq ( a u )
1- \ don't write a full buffer, don't trigger the write shortcut
myblk writebuf 3 #eq
myblk flush
create expected ,"abcdabchboo!mnop"
storage expected 16 c[]= #true

\test readbuf on unflushed written data
0 myblk seek
4 myblk readbuf 2drop \ blk0 in read buffer
0 myblk seek
"bar" myblk puts \ 3 bytes in write buffer
0 myblk seek
4 myblk readbuf []>str "bard" #s=

\test write whole blk at once
\ there used to be a bug where pos wasn't advanced
0 myblk seek
"whol" myblk puts
myblk pos 4 #eq
0 myblk seek
4 myblk readbuf []>str "whol" #s=
testend
