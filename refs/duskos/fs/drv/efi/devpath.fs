needs drv/efi
unit drv/efi/devpath

efiguid devpathguid
  91 6e 57 09 3f 6d d2 11 8e 39 00 a0 c9 69 72 3b
efiguid devpathtotextguid
  20 3e 84 8b 32 81 52 48 90 cc 55 1a 4e 4a 7f 1c

create devpathtotext 8 allot0
0 value hasdevpathtotext
: hasdevpathtotext# hasdevpathtotext not ?abort"Can't print UEFI paths" ;

:~ devpathtotextguid LocateProtocol 8 <> if exit then
   devpathtotextguid swap aq@ HandleProtocol ?err
   interface devpathtotext 2 move 1 to hasdevpathtotext ; ~

: ConvertDevicePathToText ( allowshortcut displayonly devpath64 -- str64 )
  hasdevpathtotext#
  argstart arg0! arg1! arg2! devpathtotext aq@ [q+@] 0 1 efiexec ;

: .str64 ( str64 -- )
  [ tmpbuf i) A>) @, PSP) A>) -!,
    $200 i) A>) @, PSP) A>) -!, ]
  qmove tmpbuf begin ( a )
    c@+ ?dup while emit 1+ repeat drop ;

qvariable devaddr
: .devpath ( allowshortcut displayonly a64 -- )
  dup qrshift32 devaddr tuck 4+ ! !
  devpathguid devaddr aq@ HandleProtocol ( allowshortcut displayonly res64 )
  if 2drop ."(unprintable)" else
    interface aq@ ConvertDevicePathToText .str64 then ;
