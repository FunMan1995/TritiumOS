\ UART0 was already configured by u-boot, we piggy back on it
unit drv/sunxi/uart

$01c28000 const UART0_BASE
UART0_BASE const UART0_DR
UART0_BASE $14 + const UART0_LSR

: uart! begin UART0_LSR @ $20 and until UART0_DR ! ;
: uart@ begin UART0_LSR @ $01 and until UART0_DR @ ;
: uart@? UART0_LSR @ $01 and dup if UART0_DR @ swap then ;
