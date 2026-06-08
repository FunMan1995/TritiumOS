$10000000 HEREMAX !

needs drv/timer gr/buf drv/rpi/screen
\ If you want the "native" resolution, you can try replacing "1024 768" with
\ "fbgetres". That's a lot of pixel though, it can get slow.
1024 768 rpiscreen$

needs gr/font/uf2 io/grid lib/coop io/kbd gr/grid
"data/font/term12.uf2" loaduf2 grgrid$
:> grid write# ; console!
' refreshcaret ' promptforkey realias
createapplication RefreshGrid
' refreshgrid IDLE RefreshGrid sethandler
RefreshGrid newcontext launchcontext

..
needs io/kbd drv/rpi/dwc drv/usb/kbd
dwc$
findkbd# newusbkbd to keyboard

..
needs lib/coop app/gcon
coop$ initgcon

..
needs drv/arm/exc
armexc$
needs drv/rpi/pwr drv/rpi/break
intr$

code irqhandler ( -- )
  isrsave,
  breakisr,
  then
  isrrestore, reti,

current ARMEXCTBL $18 + !
break$

..
needs drv/arm/cache
' invalidateicache ' clearicache realias
enableicache enabledcache enablewritebuffer

..
needs io/mouse drv/usb/mouse
:~
  findmouse ?dup not if
    ."No USB mouse, skipping config" exit then
  newusbmouse to mouse
  screen Pixbuf.width screen Pixbuf.height 1 mouse configuremouse ; ~
' refreshmouse ' drawmousecursor realias

..
needs lib/diag
nl> .free nl>
zsel edload<< init.txt
3 go
