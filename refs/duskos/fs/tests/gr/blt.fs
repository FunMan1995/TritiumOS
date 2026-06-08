needs tests/harness tests/grh gr/buf gr/rdwr gr/blt
testbegin

\ we want the palette to be different from their index to ensure that we
\ actually fetch palette colors, but we also want 0 to stay 0 for clarity.
create mypalette map< , 0 14 13 12 11 10 9 8 7 6 5 4 3 2 1 15
4BPPL PALETTE 16 4 newpixbuf const srcpb
mypalette srcpb to palette
srcpb allotbuf
16BPP RGB565 8 4 newpixbuf const dstpb
dstpb allotbuf

\test 8x4 pixels from PAL4 to RGB565
3 5 2 srcpb pixel!
9 0 0 srcpb pixel!
8 7 0 srcpb pixel!
1 7 3 srcpb pixel!
0 0 8 4 srcpb 0 0 dstpb movepixels
8 4 dstpb expectpix
0000000e
00000c00
00000000
60000007

\test use different src/dst indexes
dstpb clear
0 0 8 4 srcpb 1 0 dstpb movepixels
8 4 dstpb expectpix
00000000
000000c0
00000000
06000000

\test we used to use the wrong ?trim,
dstpb clear
0 0 8 4 srcpb 2 0 dstpb movepixels
8 4 dstpb expectpix
00000000
0000000c
00000000
00600000

\test we used to forget modding srcidx
14 8 0 srcpb pixel!
dstpb clear
8 0 8 4 srcpb 0 0 dstpb movepixels
8 4 dstpb expectpix
00000000
00000000
00000000
10000000

\test let's try a 2BPPH destination
\ same palette principle as before, but under 2bpp
create mypalette map< , 0 3 1 2 3 2 1 0 3 2 1 0 3 2 1 3
mypalette srcpb to palette
2BPPH GRAY2 8 4 newpixbuf const dstpb
dstpb allotbuf
0 0 8 4 srcpb 0 0 dstpb movepixels
8 4 dstpb expectpix
00000003
00000200
00000000
20000003

testend
