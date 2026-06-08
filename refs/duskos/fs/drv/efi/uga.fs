needs drv/efi gr/buf
unit drv/efi/uga

create ugaguid map< c,
  $8b $29 $2c $98 $fa $f4 $cb $41 $b8 $38 $77 $aa $68 $8f $b8 $39

0 value ugacnt
0 value ugas
qvariable curuga

: uga ( -- a64 ) curuga aq@ ;
: uga! ugaguid swap q* ugas + aq@ HandleProtocol ?err
       interface curuga 2 move ;

:~ ugaguid LocateProtocol dup not ?err ( a u )
   dup q/ to ugacnt here# to ugas cmoveallot
   0 uga! ; ~

qvariable resx
qvariable resy
qvariable depth
qvariable rrate
: GetMode ( -- resx resy depth refreshrate )
  argstart resx absaddr arg1! resy absaddr arg2!
  depth absaddr arg3! rrate absaddr arg4!
  uga arg0k! [q+@] 0 0 efiexec#
  resx aq@ resy aq@ depth aq@ rrate aq@ ;

: _invalidate ( x y w h pb -- )
  fliprect
  argstart 2 arg2! dup linesz arg9! buffer absaddr arg1! ( x y w h )
  arg8! arg7! ( x y )
  dup arg4! arg6! ( x )
  dup arg3! arg5! ( )
  uga arg0k! [q+@] 0 2 efiexec# ;
: uga$ ( -- )
  32BPP RGB24 GetMode 2drop newpixbuf to screen
  screen allotbuf
  ['] _invalidate screen to invalidate
  1 screen to flipY ;
