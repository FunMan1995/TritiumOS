needs tests/harness tests/grh gr/color gr/buf gr/rdwr
testbegin

16BPP RGB565 8 4 newpixbuf const mypb
mypb allotbuf

\test set pixel rgb565
3 5 2 mypb pixel!
8 4 mypb expectpix
00000000
00000300
00000000
00000000

\test set pixel rgb565, Y flipped
1 mypb to flipY
4 5 2 mypb pixel!
0 mypb to flipY
8 4 mypb expectpix
00000000
00000300
00000400
00000000

\ Let's try 4-bit now.
4BPPL PALETTE 8 4 newpixbuf const mypb
mypb allotbuf

\test set pixel pal4
3 4 2 mypb pixel!
8 4 mypb expectpix
00000000
00003000
00000000
00000000

\test set another pixel next to it
2 5 2 mypb pixel!
8 4 mypb expectpix
00000000
00003200
00000000
00000000

\test set the other one again
1 4 2 mypb pixel!
8 4 mypb expectpix
00000000
00001200
00000000
00000000

testend
