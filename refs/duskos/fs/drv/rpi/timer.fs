unit drv/rpi/timer

MMIO_BASE $3004 + const TIMER_CLO
MMIO_BASE $3008 + const TIMER_CLH

\ This is a 64-bit register for a free-running counter at 1MHz.
: (ticks) TIMER_CLO @ ;
current ' noop 1 ?timer$
