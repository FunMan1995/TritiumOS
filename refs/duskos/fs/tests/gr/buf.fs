needs tests/harness tests/grh gr/color gr/buf
testbegin

\test create and allocate pixbuf
16BPP RGB565 8 4 newpixbuf const mypb
mypb allotbuf
mypb linesz 8 2* #eq
mypb bufsz 8 4 * 2* #eq
mypb buffer 0 <> #true

\test bounds, everything fits
1 2 3 2 mypb bounds 2 #eq 3 #eq 2 #eq 1 #eq

\test bounds, shrink width
1 2 8 2 mypb bounds 2 #eq 7 #eq 2 #eq 1 #eq

\test bounds, shrink height
1 2 3 3 mypb bounds 2 #eq 3 #eq 2 #eq 1 #eq

\test bounds, negative target
-1 -1 3 2 mypb bounds 1 #eq 2 #eq 0 #eq 0 #eq

\test bounds, x larger than pb width yields a null rect
12 0 8 4 mypb bounds 0 #eq 0 #eq 0 #eq 0 #eq

\test allotbuf always aligns lines to 32-bit
1BPPL PALETTE 2 2 newpixbuf const mypb
mypb allotbuf
mypb linesz 4 #eq
mypb bufsz 8 #eq

testend
