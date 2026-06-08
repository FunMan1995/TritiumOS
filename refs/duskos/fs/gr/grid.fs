needs gr/color gr/font gr/buf io/mouse io/grid
unit gr/grid

0 value _font
: dark $fefefe and 1 rshift ;
:~ >screencolor , ;
create palette rgbblack ~    rgbblue dark ~    rgbgreen dark ~  rgbcyan dark ~
               rgbred dark ~ rgbmagenta dark ~ rgbyellow dark ~ rgbwhite dark ~
               $707070 ~     rgbblue ~         rgbgreen ~       rgbcyan ~
               rgbred ~      rgbmagenta ~      rgbyellow ~      rgbwhite ~

: toxy ( pos -- x y )
  COLS /mod 1+ LINES swap- _font Font.height * dip _font Font.maxwidth * | ;
: _cell! ( col char pos -- )
  toxy screen _font fonttarget! ( col c )
  over 2 rshift $3c and palette + @ ( cols c col0 )
  rot 2 lshift $3c and palette + @ ( c col0 col1 )
  _font fontcolors! _font drawglyph ;
: grgrid$ ( font -- )
  to _font
  _font Font.maxwidth grid to pixw
  _font Font.height grid to pixh
  screen Pixbuf.width grid pixw /
  screen Pixbuf.height grid pixh /
  grid resize ;
: refreshgrid ( -- )
  0 begin
    grid getnextchange while
    r! _cell! r> repeat ;

LGREEN LBLUE joinfgbg value caretcolor
LBLUE LGREEN joinfgbg value mousecolor
BLUE GREEN joinfgbg value bothcolor
variable caretpos
variable mousepos
: cellcolor! ( color pos -- ) dup grid getchar swap _cell! ;
: refreshcaret ( -- )
  refreshgrid caretcolor grid pos ['] cellcolor! caretpos ?showmarker ;
: refreshmouse ( -- )
  refreshgrid
  mousepos @ ( oldpos )
  mouse xy grid xypixpos ( old new )
  2dup = if 2drop exit then
  mousecolor over grid pos = if drop bothcolor then swap
  ['] cellcolor! mousepos ?showmarker ( oldpos )
  caretpos @ = if caretcolor caretpos @ cellcolor! then ;
