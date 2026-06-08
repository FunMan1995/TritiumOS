needs gr/buf gr/rdwr gr/font gr/font/uf1 gr/font/uf2 gr/varvara
unit bench/gr

\ Gradient
512 value gh
0 value mycol

: colr ( x y -- )
  over 255 swap- 8 lshift or swap 16 lshift or >screencolor to mycol ;
: pixl ( x y -- ) mycol rot> screen pixel! ;

: hori ( y -- )
  dup $ff > if 511 over - else dup then >r \ V1 = colY
  256 0 do i dup V1 colr over pixl loop
  drop rdrop ;

: vert ( x -- )
  gh 0 do i dup $ff > if 511 over - else dup then ( x y colY )
  >r over r> colr ( x y ) over 511 swap- swap pixl loop drop ;

: jump ( y -- )
  256 0 do i swap ( x y )
    over over colr over 512 + over pixl over 1023 swap- over pixl
    over 512 + over 511 swap- pixl over 1023 swap- over 511 swap- pixl ( x y )
  nip loop drop ;

: gradient
  screen height gh < if screen height to gh then
  gh 0 do i hori loop
  256 0 do i vert loop
  screen width 1024 >= gh 512 = and if
	256 0 do i jump loop
  then ;

\ Varvara
create heart map< be, $0066ffff $ff7e3c18 $00000018 $18000000
create diag map< be,  $0103070f $1f3f7fff

: varvara
  \ A diagonal line of hearts
  heart to sprite 64 to Y 16 to X
  16 0 do i X+) drawsprite loop
  1 8 times) X+) drawsprite
  \ a 16x16 diamond shape
  diag to sprite 72 to Y 16 to X
  2 X+) drawsprite 2 flipX) X+) Y+) drawsprite
  2 flipY) X+) drawsprite 2 flipX) flipY) drawsprite
  \ ... with a 2bpp heart in the middle
  doto X 4- | doto Y 4- | heart to sprite 5 2bpp) drawsprite
  \ draw partially out of bounds in the four corners
  -1 to X -1 to Y 1 drawsprite
  -1 to X vscreen height 7 - to Y 1 drawsprite
  vscreen width 7 - to X -1 to Y 1 drawsprite
  vscreen width 7 - to X vscreen height 7 - to Y 1 drawsprite
  \ and completely out of bounds
  0 to X vscreen height to Y 1 drawsprite
  vscreen width to X 0 to Y 1 drawsprite
  refreshscreen
  \ test that partial refreshscreen works well
  8 to X 8 to Y 2 drawsprite
  refreshscreen ;

\ UFX
: drawall ( y font -- )
  >r rgbblack >screencolor rgbwhite >screencolor r@ fontcolors! ( y V1=font )
  0 swap screen V1 fonttarget! ( )
  $100 0 do ( )
    i V1 drawglyph
    V1 tgtx V1 Font.maxwidth + screen Pixbuf.width >= if
      0 V1 to tgtx V1 Font.height V1 doto tgty + | then
    loop
  rdrop ;

: ufx
  0 "data/font/atari8.uf1" loaduf1 drawall
  100 "data/font/sans12.uf2" loaduf2 drawall ;

\ All
: kd key drop ;
: grall
  ."A gradient of colors across the RGB spectrum\n" kd gradient
  ."An orange square with a bunch of hearts and diamonds in it.\n" kd varvara
  ."Drawing 256 glyphs of UF1 and UF2 fonts\n" kd ufx
  kd ."End of tests\n" ;

