needs io/mouse
unit drv/mac68k/mouse

variable loc
: GetMouse ( -- x y ) loc >r [ $a972 w, ] loc @ dup $ffff and swap 16 rshift ;

$172 const MBState

:> ( mouse -- )
  MBState w@ 15 rshift not over to buttons
  GetMouse rot moveto ;
: newmacmouse [ litn ] newmouse ;
