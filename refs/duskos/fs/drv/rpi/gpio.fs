needs lib/ival
unit drv/rpi/gpio

MMIO_BASE $200000 + absvalmap {
  uint fsel0 fsel1 fsel2 fsel3 fsel4 fsel5 ;
  +$1c uint set0 set1 ;
  +$28 uint clr0 clr1 ;
  +$34 uint lev0 lev1 ;
  +$40 uint eds0 eds1 ;
  +$4c uint ren0 ren1 ;
  +$58 uint fen0 fen1 ;
  +$64 uint hen0 hen1 ;
  +$70 uint len0 len1 ;
  +$7c uint aren0 aren1 ;
  +$88 uint afen0 afen1 ;
  +$94 uint pud pudclk0 pudclk1 ;
}

enum GPIOPullNone GPIOPullDown GPIOPullUp

: _delay 100 begin 1- ?dup not until ;
:~ ( type mask reg -- )
  rot to pud _delay
  tuck ! _delay
  0 to pud 0 swap ! ;
: gpiopull0 addrof pudclk0 ~ ;
: gpiopull1 addrof pudclk1 ~ ;
