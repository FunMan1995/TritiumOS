needs drv/arm/exc lib/ival asm/arm drv/arm/psr
unit drv/rpi/intr

MMIO_BASE $b200 + absvalmap {
  uint pendingb pending1 pending2 fiqctrl ;
  uint enable1 enable2 enableb disable1 ;
  uint disable2 disableb ;
}

:~ .x spc> ;
: .intr ( -- )
  ."Pending (basic): " pendingb ~ nl>
  ."Pending (31:0): " pending1 ~ nl>
  ."Pending (63:32): " pending2 ~ nl>
  ."Enabled (basic): " enableb ~ nl>
  ."Enabled (31:0): " enable1 ~ nl>
  ."Enabled (63:32): " enable2 ~ nl>
  ."FIQ: " fiqctrl ~ nl> ;

: intr$ 0 ARM_IRQ_BIT cpsr! ; \ enable IRQs

: isrsave, ( -- )
  mov) rSP rd) $60 imm) ,) r0 push, r1 push, r2 push, ;
: isrrestore, ( -- ) r2 pop, r1 pop, r0 pop, ;

: israbort, ['] abort i) @, mov) f) rPC rd) rW rm) ,) ;
