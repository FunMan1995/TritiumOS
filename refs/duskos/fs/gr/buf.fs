needs hal/vreg lib/struct gr/color
unit gr/buf

struct Pixbuf {
  uint buffer width height encoding depth linesz bufsz palette flipY ;
  xt invalidate ?allotbuf ;
}

consts $10 1BPPL $11 1BPPH $22 2BPPL $23 2BPPH $44 4BPPL $45 4BPPH \
       $86 8BPP  $107 16BPP $188 24BPP $209 32BPP

code depth>idx $f i) &, exit,
code depth>bpp 4 i) >>, exit,

: setbuf ( linesz buffer pb -- )
  r! to buffer dup r@ to linesz ( linesz )
  r@ height * r> to bufsz ;

: allotbuf ( pb -- )
  r! linesz dup r@ height * ( linesz bufsz )
  here# swap allot0 r> setbuf ;

: configurebuf ( depth encoding width height linesz buffer pb -- )
  r! setbuf r@ to height r@ to width ( depth encoding )
  r@ to encoding r> to depth ;

: computelinesz ( pb -- linesz ) bi depth depth>bpp | width * 32 /+ 4* ;

: def?allotbuf ( pb -- )
  dup computelinesz over to linesz
  dupbi linesz | height * over bufsz > if allotbuf else drop then ;

code _ 16 ps+, drop, exit,
: newpixbuf ( depth encoding width height -- pb )
  here# >r 0 , swap , , , , ( ) r@ computelinesz ,
  0 , 0 , 0 , ['] _ , ['] def?allotbuf , r> ;

: clear bi buffer | bufsz 4/ 0 fill ;
: resize ( width height pb -- )
  tuck to height tuck to width dup ?allotbuf clear ;

: yaddr,
  A) offsetof height +) <) if,
    A) offsetof flipY +) testz, ifnz,
      1 i) +, A) offsetof height +) swap-, [compile] then
    A) offsetof linesz +) *, A) offsetof buffer +) +,
    [compile] else 0 i) @, [compile] then ;

\ IN: A=pb W=y S=x OUT: W=addr
: xyaddr,
  yaddr, A) offsetof width +) S>) <) if,
    R3) S>) !, A) offsetof depth +) S>) @,
    4 i) S>) >>, R3) S>) *, 3 i) S>) >>, S) &) +,
  [compile] else 0 i) @, [compile] then ;

: oob abort"coordinates out of bounds" ;
code yaddr ( y pb -- a )
  A) &) !, drop, yaddr, ' oob 0 i) =) ?br, exit,
code xyaddr ( x y pb -- x a ) \ OUT: A=pb
  A) &) !, drop, PSP) S>) @, xyaddr, ' oob 0 i) =) ?br, exit,

code _null drop, 0 i) @, PSP) !, PSP) 4 +) !, PSP) 8 +) !, exit,
code bounds ( x y w h pb -- x y w h )
  A) &) !, PSP) 12 +) @, ( x y w h x A=pb )
  0 i) signed) <) if, PSP) 4 +) dir) +, 0 i) @, PSP) 12 +) !, then
  ' _null A) offsetof width +) >=) ?br,
  PSP) 4 +) +, A) offsetof width +) >) if,
    A) offsetof width +) -, PSP) 4 +) dir) -, then
  PSP) 8 +) @, ( x y w h y )
  0 i) signed) <) if, PSP) dir) +, 0 i) @, PSP) 8 +) !, then
  ' _null A) offsetof height +) >=) ?br,
  PSP) +, A) offsetof height +) >) if,
    A) offsetof height +) -, PSP) dir) -, then
  drop, exit,

code s2/ 1 i) signed) >>, exit,
: centerxy ( pb -- x y pb ) tri width s2/ | height s2/ | ;
: centerrect ( w h pb -- x y pb ) r! width rot - s2/ r@ height rot - s2/ r> ;
: fliprect ( x y w h pb -- x y w h pb )
  r! bounds r> [
  W) offsetof Pixbuf.height +) S>) @, PSP) 8 +) S>) -,
  PSP) S>) -, PSP) 8 +) S>) !, ] ;

\ screen global
16BPP RGB565 0 0 newpixbuf dup const dummyscreen value screen
: >screencolor screen encoding >color ;
