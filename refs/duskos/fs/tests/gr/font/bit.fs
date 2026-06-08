needs tests/harness tests/grh gr/buf gr/font gr/font/bit
testbegin

\ Here, we build a font of 2x2 glyphs, with 16 glyphs in it, that is, all
\ possible values. First glyph is all BG, last one is all FG.
2 2 16 newbitfont const fnt
:~ ( buf -- ) 4 0 do 4 0 do i j rot !+ !+ loop loop drop ;
fnt BitFont.pixbuf Pixbuf.buffer ~

16BPP RGB565 8 4 newpixbuf const mypb
mypb allotbuf

\test draw glyph at bottom left
0 0 mypb fnt fonttarget!
1 2 fnt fontcolors!
6 ( BF / FB ) fnt drawglyph
8 4 mypb expectpix
00000000
00000000
12000000
21000000

\test draw unaligned glyph
1 2 mypb fnt fonttarget!
9 ( FB / BF ) fnt drawglyph
8 4 mypb expectpix
02100000
01200000
12000000
21000000

\test draw on Y-flipped buffer
1 mypb to flipY
2 2 mypb fnt fonttarget!
6 ( BF / FB ) fnt drawglyph
0 mypb to flipY
8 4 mypb expectpix
02100000
01200000
12210000
21120000

\test let's try on a 2BPPH buffer
2BPPH GRAY2 8 4 newpixbuf const mypb
mypb allotbuf

1 2 mypb fnt fonttarget!
9 ( FB / BF ) fnt drawglyph
8 4 mypb expectpix
02100000
01200000
00000000
00000000

\test ensure that 2BPPH uses 32-bit aligned addresses
4 2 mypb fnt fonttarget!
9 ( FB / BF ) fnt drawglyph
8 4 mypb expectpix
02102100
01201200
00000000
00000000
testend
