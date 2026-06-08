needs lib/coop drv/rpi/gpio drv/rpi/intr
unit drv/rpi/break

$08000000 const PINMASK \ pin 27
$00e00000 const FSELMASK \ pin27 function mask
$00020000 const INTRMASK \ IRQ 49, gpio_int[0]
5 const BREAK_FORCE_AFTER

create breakcnt BREAK_FORCE_AFTER ,
: _dobreak BREAK_FORCE_AFTER breakcnt ! abort ;
createapplication Breaker
:> breakcnt @ BREAK_FORCE_AFTER < if _dobreak then ;
IDLE Breaker sethandler

: breakisr, ( -- handled-jmp ) \ Destroys r0 and r1
  mov) r1 rd) MMIO_BASE imm) ,)
  add) r1 rdn) $200000 imm) ,) \ GPIO_BASE
  ldr) r0 rd) r1 rn) $40 +i) ,) \ GPEDS0
  and) r0 rdn) PINMASK imm) f) ,)
  ifnz,
  str) r0 rd) r1 rn) $40 +i) ,) \ GPEDS0
  -1 breakcnt m) +n,
  ifz,
    ['] _dobreak i) @,
    mov) f) rPC rd) rW rm) ,)
  [compile] then
  [compile] else ;

: break$ ( -- )
  GPIOPullUp PINMASK gpiopull0
  fsel2 FSELMASK invand to fsel2 \ set pin27 as input
  fen0 PINMASK or to fen0 \ enable falling edge detection on pin 27
  enable2 INTRMASK or to enable2 \ enable GPIO bank 2 interrupts
  Breaker newcontext launchcontext ;
