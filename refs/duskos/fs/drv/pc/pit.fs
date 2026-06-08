\ Programmable Interval Timer
needs drv/pc/pic drv/timer asm/x86
unit drv/pc/pit

$40 const PITCH0
$43 const PITCMD

\ PIT clock is 1.193182 MHz with as 120 subcounter gives us a resolution of
\ 100.57 us. We *could* go to 1us resolution, but that makes us fire that IRQ
\ a bit too often to my taste...
120 const CNTVAL
variable _ticks
: (ticks) _ticks @ ;

code (snooze) hlt, exit,

code isrIRQ0
  _ticks m) inc,
  ax push, piceoi, ax pop,
  iret,

\ You need to remap the PIC before calling this
: pit$
  ['] (ticks) ['] (snooze) 100 timer$
  ['] isrIRQ0 $20 setISR
  $34 PITCMD pc! \ Channel 0, lobyte/hibyte, Mode 2 (rate generator)
  CNTVAL $ff and PITCH0 pc! CNTVAL 8 rshift PITCH0 pc!
  0 pic1unmask ;
