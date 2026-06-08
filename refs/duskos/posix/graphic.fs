needs io/kbd
:> ( kbd -- ?nkc event-type ) drop ?getnkc ; newkbd to keyboard

needs text/clip gr/damage gr/buf

variable mydmg

: _invalidate ( x y w h pb -- )
  drop damage 0 mydmg @! damage+ mydmg ! ;

: _activate ( -- )
  16BPP RGB565 screeninfo newpixbuf to screen
  screen allotbuf
  ['] _invalidate screen to invalidate
  screen buffer mydmg screencb ;
_activate

needs gr/font/uf2 io/grid lib/coop io/kbd gr/grid
"data/font/term12.uf2" loaduf2 grgrid$
:> grid write# ; console!
' refreshcaret ' promptforkey realias
createapplication RefreshGrid
' refreshgrid IDLE RefreshGrid sethandler
RefreshGrid newcontext launchcontext

needs io/mouse

:> ( mouse -- ) >r mousecb r@ to buttons r> moveto ;
newmouse to mouse
screeninfo 1 mouse configuremouse
' refreshmouse ' drawmousecursor realias

: hostcopy
  0 0 0 clipcb dup clipensure
  0 swap clipboard swap clipcb clipboardlen ! ;
: hostpaste 1 clipboard clipboardlen @ clipcb ;

needs lib/diag lib/coop app/gcon app/gmux
coop$ gmux$
gconctx 0 gmuxctx!
zsel edload<< init.txt
7 go
