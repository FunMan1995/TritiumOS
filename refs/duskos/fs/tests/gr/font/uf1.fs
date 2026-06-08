needs tests/harness tests/grh gr/buf gr/font gr/font/uf1
testbegin

"data/font/atari8.uf1" loaduf1 const fnt

16BPP RGB565 8 8 newpixbuf const mypb
mypb allotbuf

\test draw a "A"
0 0 mypb fnt fonttarget!
1 2 fnt fontcolors!
'A' fnt drawglyph
8 8 mypb expectpix
11122111
11222211
12211221
12211221
12222221
12211221
12211221
11111111

\test draw a "1"
0 0 mypb fnt fonttarget!
'1' fnt drawglyph
8 8 mypb expectpix
11122111
11222111
11122111
11122111
11122111
11122111
12222221
11111111

testend
