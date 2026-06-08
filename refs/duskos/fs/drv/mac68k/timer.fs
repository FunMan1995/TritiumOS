needs drv/timer
unit drv/mac68k/timer

: Ticks $16a @ ;
\ 16666 us per tick means 60 ticks per second
' Ticks ' noop 16666 ?timer$