needs drv/efi
unit drv/efi/image

efiguid loadedimageguid
  a1 31 1b 5b 62 95 d2 11 8e 3f 00 a0 c9 69 72 3b

qvariable bootloadedimage

:~ loadedimageguid ImageHandle HandleProtocol ?err
   interface bootloadedimage 2 move ; ~

: BootDeviceHandle bootloadedimage aq@ [q+@] 0 3 ;
