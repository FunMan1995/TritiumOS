\ 8042 PS/2 Controller driver, polling version
\ You will generally want to use the interrupt-based version of this driver,
\ but sometimes, when debugging hardware, you'll prefer to keep interrupts out.
\ This driver only supports the keyboard device
needs drv/pc/ioport
unit drv/pc/ps28042p

$60 ioportb ps2data
$64 ioportb ps2cmd

: 8042kbd@? ( -- keycode? f ) ps2cmd 1 and dup if ps2data swap then ;
