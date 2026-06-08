needs tests/harness lib/psrs num/crc fs/core mem/blk fs/fatt fs/fat fs/sh
testbegin
100 const TOTSEC
512 TOTSEC newmemblk value myblk
\ make sure that new clusters are properly initialized
myblk buf( TOTSEC 512 * 4/ n"1234" fill
fatopts$
1 to secperclus
myblk newFAT
myblk newfatfs const myfat
myfat to curfs

\test playing with the FAT
FAT12? #true
0 FAT@ EOC #eq
2 FAT@ 0 #eq
EOC 54 FAT!
EOC 42 FAT!
54 23 FAT!
42 24 FAT!
23 FAT@ 54 #eq
24 FAT@ 42 #eq

\ At this point, we have a FAT with a 23->54->EOC an a 24->42->EOC chain

\test create a new file in empty FAT
curfs walk
"first.txt" newfile
walkpath "" #s=
walkname "FIRST.TXT" #s=
walksize 0 #eq
walkdir? not #true

\test walk that new file
curfs walk
gotonext #true
walkpath "" #s=
walkname "FIRST.TXT" #s=
walksize 0 #eq
walkdir? not #true
gotonext not #true

\test another file
curfs walk
"second.fs" newfile

\test walk those two files
curfs walk
gotonext #true
walkname "FIRST.TXT" #s=
gotonext #true
walkname "SECOND.FS" #s=
gotonext not #true

\test create directory
curfs walk
"subdir" newdir
walkpath "" #s=
walkname "SUBDIR" #s=
walksize 0 #eq
walkdir? #true

\test enter newly created directory and create file
enterdir
walkpath "SUBDIR" #s=
"subfile" newfile

\test walk to that subfile
curfs walk
"subdir" lookupchild #true
enterdir
gotonext #true
walkname "SUBFILE" #s=

\test full direntry cluster
\ There used to be a bug where a "full" entry cluster would yield the wrong
\ number of children

create name 1 c, 'A' c,
curfs walk
"subdir" lookupchild #true
enterdir
\ We add 13 because we have . .. and SUBFILE in there
:~ 13 0 do name newfile 1 name 1+ A! c@ + @Ac! loop ; ~
curfs walk
"subdir" lookupchild #true
enterdir
:~ 14 0 do gotonext #true loop ; ~
walkname "M" #s=
gotonext not #true

\test extend the direntry cluster
\ a bug used to have it overwrite the last entry
curfs walk
"subdir" lookupchild #true
enterdir
"foo" newfile
\ the last file previously there is still present!
curfs walk "subdir/m" lookuprel #true
curfs walk "subdir/foo" lookuprel #true

\test FS operations doesn't change storage position
42 myblk seek
curfs walk "subdir/foo" lookuprel #true
open "foobar" over puts close
myblk pos 42 #eq

\test copy big file in FAT and check CRC
walkdst myfat walk "big.fs" newfile
walksrc p"xcomp/boot.fs" copyfile
f"xcomp/boot.fs" dup crc32 swap close ( crc )
myfat walk "big.fs" lookupchild #true
open dup crc32 swap close #eq

\test out of sync readbuf and writebuf on same FS
\ this big test is to ensure that when the destination file is seeking its write
\ cluster, it doesn't overwrite the current readbuf.
myfat walk
"hello.txt" newfile
open "hello" over puts close
"async.txt" newfile
open const asyncfile
510 asyncfile "filler" c@+ fillstream spitn
curfs walk "hello.txt" lookupchild #true
asyncfile open spitcloseboth
create buf 5 allot
curfs walk "async.txt" lookupchild #true open ( file )
510 over seek
dup buf 5 rot read# close
"hello" buf 5 #s[]=

testend
