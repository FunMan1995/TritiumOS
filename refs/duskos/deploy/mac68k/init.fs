..
needs drv/mac68k/xpram
set32bitmode

..
needs drv/mac68k/screen
needs gr/font/uf1 gr/grid lib/coop io/kbd io/grid
..
"data/font/atari8.uf1" loaduf1 grgrid$

:> grid write# ; console!
' refreshcaret ' promptforkey realias
:~ palette 4 0 do 4 0 do i swap !+ loop loop drop ; ~
createapplication RefreshGrid
' refreshgrid IDLE RefreshGrid sethandler
RefreshGrid newcontext launchcontext

..
needs drv/mac68k/timer
needs io/kbd drv/mac68k/kbd
newmackbd to keyboard

..
needs drv/mac68k/mouse io/mouse
newmacmouse to mouse
screen Pixbuf.width screen Pixbuf.height 1 mouse configuremouse

..
needs lib/diag lib/coop app/gcon
coop$ initgcon
zsel edload<< init.txt
