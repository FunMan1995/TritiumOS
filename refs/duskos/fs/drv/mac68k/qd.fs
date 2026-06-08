unit drv/mac68k/qd

: w>r $3f07 w, ( 16-bit RS push ) drop, ; immediate

: MoveTo ( x y -- ) 16 lshift or >r [ $a893 w, ] ;
: DrawChar ( c -- ) w>r [ $a883 w, ] ;

create _ 8 allot0
: rect ( x y w h -- r )
  rot dup _ w!
  + _ 4+ w!
  over _ 2+ w!
  + _ 6 + w! _ ;

: EraseRect ( x y w h -- ) rect >r [ $a8a3 w, ] ;

SYSVARS $fc - const thePort
: portBits thePort 2+ @ ;

: qdrtype ( a u -- ) 0 do c@+ DrawChar loop drop ;
current RTYPE !

: qd$ ( -- )
  thePort 8- >r [ $a86e w, ( InitGraf ) ]
  thePort >r [ $a86f w, ( OpenPort ) ]
  0 0 640 480 EraseRect
  16 16 MoveTo ;
qd$
