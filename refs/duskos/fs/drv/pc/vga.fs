needs drv/pc/ioport io/mouse io/grid
unit drv/pc/vga

$b8000 adder mem+
$3d4 ioportb vgareg
$3d5 ioportb vgadata

: vgareg@ ( idx -- c ) to vgareg vgadata ;
: vgareg! ( c idx -- ) to vgareg to vgadata ;

: vgagrid$ ( -- )
  80 25 grid resize
  10 grid to pixw 24 grid to pixh ;
: refreshgrid ( -- )
  0 begin
    grid getnextchange while ( color char pos )
    r! 2* mem+ c!+ dip $7f and | c! r> repeat ;

: refreshcaret ( -- )
  refreshgrid grid pos dup $0f vgareg! 8 rshift $0e vgareg! ;

variable oldmouse
: cellcolor! ( color pos -- ) dip $7f and | 2* mem+ 1+ c! ;
: refreshmouse ( -- ) \ hardcoded for 800x600 --> 80x25
  refreshgrid
  LBLUE GREEN joinfgbg
  mouse xy grid xypixpos
  ['] cellcolor! oldmouse ?showmarker ;

\ Set video mode to text mode, 80x25
: vgatextmode ( -- ) 0 0 3 int10h 2drop ;
