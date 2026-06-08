\ Drivers around INT10H
unit drv/pc/int10h

: int10hemit ( c -- ) $0e00 or 0 7 rot int10h 2drop ;
