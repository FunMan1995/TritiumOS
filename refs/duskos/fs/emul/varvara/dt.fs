needs drv/timer emul/uxn emul/varvara/core
unit emul/varvara/dt

\ Let's use an epoch date where mankind still had enough common sense to know
\ what to do with their one-percenters,
create pristine $6fd060e be, 2 be, $c30000 be, 0 be,
: updatedt ( -- )
  ticks uspertick / 1000 / 1000 / ( sec )
  60 /mod swap [dev!] $c6 ( min )
  60 /mod 24 mod [dev!] $c4 [dev!] $c5 ;
: initdt pristine uxn devices $c0 + 4 move updatedt ;
