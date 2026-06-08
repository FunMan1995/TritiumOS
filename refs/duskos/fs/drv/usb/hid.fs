needs drv/usb
unit drv/usb/hid

consts
  $03 Getproto \
  $0a Setidle \
  $0b Setproto \
  0 Bootproto \
  1 Reportproto

: bootproto! ( ifaceid ep0 -- )
  >r 0 0 rot Bootproto Setproto ( u a index value req )
  Rh2d Rclass Riface or or r> usbcmd ( res ) drop ;

: setidle! ( val ifaceid ep0 -- )
  >r swap 8 lshift 0 rot> 0 rot> Setidle ( u a index value req )
  Rh2d Rclass Riface or or r> usbcmd ( res ) drop ;
