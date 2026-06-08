needs asm/arm
unit drv/arm/psr

$10 const ARM_MODE_USER
$11 const ARM_MODE_FIQ
$12 const ARM_MODE_IRQ
$13 const ARM_MODE_SVC
$17 const ARM_MODE_ABORT
$1b const ARM_MODE_UNDEF
$1f const ARM_MODE_SYS
$1f const ARM_MODE_MASK
$100 const ARM_IMPRECISE_BIT
$80 const ARM_IRQ_BIT
$40 const ARM_FIQ_BIT
$1c0 const ARM_INTR_MASK

: .psr ( -- )
  ."CPSR: " [ dup, mrs) rW rd) ,) ] .x nl>
  ."SPSR: " [ dup, mrs) rW rd) spsr) ,) ] .x nl> ;

code cpsr! ( val mask -- )
  mrs) r0 rd) ,)
  bic) r0 rdn) rW rm) ,)
  rW ppop,
  orr) rW rdn) r0 rm) ,)
  msr) rW rm) ,)
  drop, exit,
