$10000000 HEREMAX !

needs io/kbd drv/rpi/uart
uart0$
:> uart0 write# ; console!
uart0 newstreamkbd to keyboard

..
needs app/prompt
prompt$

..
needs drv/arm/exc
armexc$
needs drv/rpi/pwr drv/rpi/break
intr$

code irqhandler ( -- )
  isrsave,
  breakisr,
  then
  isrrestore, reti,

current ARMEXCTBL $18 + !
break$

..
needs drv/arm/cache
' invalidateicache ' clearicache realias
enableicache enabledcache enablewritebuffer

..
needs lib/diag
." Dusk OS\n" .free
