needs lib/wordtbl
unit gr/color

enum PALETTE RGB565 RGB555 RGB24 GRAY8 GRAY4 GRAY2 BW1
consts $ffffff rgbwhite  $000000 rgbblack \
       $ff0000 rgbred    $00ff00 rgbgreen $0000ff rgbblue \
       $ffff00 rgbyellow $00ffff rgbcyan  $ff00ff rgbmagenta

code >rgb565 ( rgb24 -- n )
  W) &) A>) @, W) &) S>) @,
  3 i) >>, $1f i) &,
  5 i) A>) >>, $7e0 i) A>) &, A) &) |,
  8 i) S>) >>, $f800 i) S>) &, S) &) |, exit,

code >rgb555 ( rgb24 -- n )
  W) &) A>) @, W) &) S>) @,
  3 i) >>, $1f i) &,
  6 i) A>) >>, $3e0 i) A>) &, A) &) |,
  9 i) S>) >>, $7c00 i) S>) &, S) &) |, exit,

: >gray8,
  W) &) A>) @, W) &) S>) @,
  8 i) >>, $ff i) &,
  17 i) A>) >>, A) &) +,
  2 i) S>) >>, $3f i) S>) &, S) &) +,
  $ff i) >) if, $ff i) @, [compile] then ;

code >gray8 ( rgb24 -- n ) >gray8, exit,
code >gray4 ( rgb24 -- n ) >gray8, 4 i) >>, exit,
code >gray2 ( rgb24 -- n ) >gray8, 6 i) >>, exit,

wordtbl[
  :> abort"can't convert to PALETTE" ;
  ' >rgb565
  ' >rgb555
  ' noop
  ' >gray8
  ' >gray4
  ' >gray2
  ' bool
]wordtbl tbl
: >color ( rgb24 id -- color ) tbl swap wexec ;
