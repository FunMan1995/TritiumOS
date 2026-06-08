needs lib/bit drv/timer gr/buf gr/varvara emul/uxn emul/varvara/core
unit emul/varvara/screen

0 value changed \ set when pixel or sprite is called, reset on refresh
: change! 1 to changed ;

: expand ( nibble -- byte ) $f and dup 4 lshift or ;
: color@ ( 0-1-2-3 -- color )
  4* 12 swap- >r \ V1=shiftn
  [dev2@] $08 V1 rshift expand 16 lshift ( rgb24 )
  [dev2@] $0a V1 rshift expand 8 lshift or ( rgb24 )
  [dev2@] $0c r> rshift expand or >screencolor ;
: updatecolors ( -- ) 0 color@ 1 color@ 2 color@ 3 color@ setvpalette ;
: updatesize vscreen width [dev2!] $22 vscreen height [dev2!] $24 ;

' updatecolors dup $09 setdeo dup $0b setdeo $0d setdeo
:~ [dev2@] $22 [dev2@] $24 vscreen resize updatesize ;
' ~ $23 setdeo ' ~ $25 setdeo
: rd [dev2@] $28 sex16 to X
     [dev2@] $2a sex16 to Y
     [dev2@] $2c [ uxn+, ] to sprite
     [dev@] $26 8 lshift ;
: wr X [dev2!] $28
     Y [dev2!] $2a
     sprite [ uxn-, ] [dev2!] $2c ;
:> change! rd [dev@] $2e or drawpixel wr ; $2e setdeo
:> change! rd [dev@] $2f or drawsprite wr ; $2f setdeo

: ?refreshscreen doto changed 0 | if refreshscreen then ;

0 value lastts
16 const FRAMEMS \ ms per screen frame
: ?handlescreen ( -- )
  [dev2@] $20 not if exit then
  FRAMEMS lastts elapsedms? not if exit then
  ticks to lastts [dev2@] $20 callvector ;

: initscreen updatesize vscreen clear ;
