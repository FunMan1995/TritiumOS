needs io/blk
unit drv/mac68k/disk

$200 const SECSZ
create args 64 allot0
: base ( -- )
  1 args 22 + w! \ ioVRefNum 1=floppy
  -5 args 24 + w! \ ioRefNum -5=.SONY
  1 args 44 + w! ; \ ioPosMode 1=fsFromStart

:~ ( sec dst disk -- )
  drop base ( dst ) args 32 + ! \ ioBuffer
  ( sec ) SECSZ * args 46 + ! \ ioPosOffset
  SECSZ args 36 + ! \ ioReqCount
  args [ $2047 w, ( A0 W move, ) drop, ] ;
: sec@ ~ [ $a002 w, ( Read ) ] ;
: sec! ~ [ $a003 w, ( Write ) ] ;

: eject
  base 7 args 26 + w! \ csCode 7=ejectCode
  args [ $2047 w, drop, $a004 w, ( Control ) ] ;

' sec@ ' sec! SECSZ -1 newblk const MacDisk