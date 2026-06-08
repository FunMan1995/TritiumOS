needs io/kbd io/grid lib/coop drv/pc/acpi drv/pc/vga
vgagrid$
:> grid write# ; console!
createapplication RefreshGrid
' refreshgrid IDLE RefreshGrid sethandler
RefreshGrid newcontext launchcontext
' refreshcaret ' promptforkey realias
' refreshmouse ' drawmousecursor realias
: consolemode
  ['] refreshgrid IDLE RefreshGrid sethandler
  ['] refreshcaret ['] promptforkey realias
  ['] refreshmouse ['] drawmousecursor realias
  vgatextmode vgagrid$ ;

needs drv/pc/pci drv/pc/pic drv/pc/pit drv/pc/a20 drv/pc/rtc
pic$ idt$ pit$ a20$ pcpci$

needs drv/pc/ps28042 drv/ps2

8042ps2$
:realias (ps2@?) 8042kbd@? ;
8042scancodeset newps2kbd to keyboard

needs lib/coop app/gcon
coop$ initgcon

8042mouse$
new8042mouse to mouse
800 600 1 mouse configuremouse

needs gr/font/uf1 io/grid gr/grid drv/pc/vesa
"data/font/atari8.uf1" loaduf1 const gridfont
: graphicsmode
  ['] refreshgrid IDLE RefreshGrid sethandler
  ['] refreshcaret ['] promptforkey realias
  ['] refreshmouse ['] drawmousecursor realias
  $114 vesamode gridfont grgrid$ ;

needs lib/diag
.free nl>
zsel edload<< init.txt
