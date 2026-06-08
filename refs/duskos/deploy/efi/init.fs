needs drv/efi/gop
gop$

needs gr/font/uf2 io/grid lib/coop io/kbd gr/grid
"data/font/term12.uf2" loaduf2 grgrid$
:> grid write# ; console!
' refreshcaret ' promptforkey realias
createapplication RefreshGrid
' refreshgrid IDLE RefreshGrid sethandler
RefreshGrid newcontext launchcontext

needs io/kbd drv/efi/kbdex
bootkbd newefikbdex to keyboard

needs io/mouse drv/efi/mouse
0 newefimouse to mouse
screen Pixbuf.width screen Pixbuf.height 1 mouse configuremouse
' refreshmouse ' drawmousecursor realias

needs drv/efi/timer lib/coop
createapplication EFIIdle
' efiidle IDLE EFIIdle sethandler
EFIIdle newcontext launchcontext

needs lib/coop app/gcon lib/diag
coop$ initgcon

.free nl>
zsel edload<< init.txt
DisableWatchdog
