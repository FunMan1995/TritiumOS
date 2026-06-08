needs drv/efi gr/buf
unit drv/efi/gop

efiguid gopguid
  de a9 42 90 dc 23 38 4a 96 fb 7a de d0 80 51 6a

0 value gopcnt
0 value gops
qvariable curgop

: gop ( -- a64 ) curgop aq@ ;
: gop! gopguid swap 8* gops + aq@ HandleProtocol ?err
       interface curgop 2 move ;
:~ gopguid LocateProtocol dup 8 < ?err ( a u )
   dup 8/ to gopcnt here# to gops cmoveallot
   0 gop! ; ~

: mode@ gop [q+@] 0 3 ;
: info@ mode@ [q+@] 8 0 ;
: .gop ( -- )
  ."Max modes: " mode@ q@ . nl>
  ."Mode: " mode@ [q+@] 4 0 . nl>
  ."Resolution: " info@ [q+@] 4 0 . ."x" info@ [q+@] 8 0 noq . nl>
  ."Pixfmt: " info@ [q+@] 12 0 .x nl>
  ."Pixmask: " info@ [q+@] $10 0 .x spc> info@ [q+@] $14 0 .x spc>
               info@ [q+@] $18 0 .x nl>
  ."Pixperline: " info@ [q+@] $20 0 .x nl> ;

: SetMode ( mode -- ) argstart arg1! gop arg0k! [q+@] 0 1 efiexec# ;

qvariable sz
qvariable info
:~ ( mode -- )
  dup . spc>
  argstart arg1! sz absaddr arg2! info absaddr arg3!
  gop arg0k! q@ ( QueryMode ) efiexec# ( )
  info aq@ [q+@] 4 0 . ."x" info aq@ [q+@] 8 0 .
  .":" info aq@ [q+@] 12 0 . nl> ;
: .gopmodes ( -- ) mode@ q@ 0 do i ~ loop ;

: _invalidate ( x y w h pb -- )
  fliprect
  argstart 2 arg2! dup linesz arg9! buffer absaddr arg1! ( x y w h )
  arg8! arg7! ( x y )
  dup arg4! arg6! ( x )
  dup arg3! arg5! ( )
  gop arg0k! [q+@] 0 2 efiexec# ;
: gop$ ( -- )
  32BPP RGB24 info@ [q+@] 4 0 info@ [q+@] 8 0 newpixbuf to screen
  screen allotbuf
  ['] _invalidate screen to invalidate
  1 screen to flipY ;
