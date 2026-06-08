needs gr/color gr/buf drv/rpi/vcore
unit drv/rpi/screen

\ TODO: support depths other than 16
: rpiscreen$ ( width height -- )
  2dup 16BPP rot> RGB565 rot> newpixbuf to screen ( w h )
  fbinit swap screen setbuf
  1 screen to flipY ;
