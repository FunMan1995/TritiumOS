\ Kernel for Raspberry Pi model 2 and 3
needs asm/arm
$8000 to binstart
f<< /xcomp/arm/kernel.fs

lblccache forward!
  ret,

\ Alright, we have 1 CPU. Now, is it in HYP mode? If yes, we need to get back
\ to SVC mode.
pc to L1
  mov) r7 rd) binstart imm) ,) \ SYSVARS
  mrs) r1 rd) ,)
  and) r0 rd) r1 rn) $1f imm) ,)
  cmp) r0 rn) $1a imm) ,)
  lblbaseboot abs>rel b) nz) ,) \ no HYP? onwards!
  \ Get out of this crappy mode.
  bic) r1 rdn) $1f imm) ,)
  orr) r1 rdn) $13 imm) ,) \ SVC
  orr) r1 rdn) $1c0 imm) ,) \ no interrupt
  $e16ff001 ,) \ msr spsr_cxsf,r1
  mov) r1 rd) binstart imm) ,)
  $e12ef301 ,) \ msr ELR_hyp, r1
  $e160006e ,) \ eret

\ Coldboot procedure. Before jumping to base coldboot, we want to deactivate
\ all but one CPU core.
lblcoldboot forward!
  mrc) 15 cp#) r0 rd) 5 cpi) ,) \ r0=CPUID
  and) r0 rdn) 3 imm) f) ,)
  L1 abs>rel b) z) ,) \ CPU0, onwards!
  0 b) ,) \ Other CPUs, infinite loop

$3f000000 xconst MMIO_BASE
$c0000000 xconst DMABASE
xlatest org oSYSDICT + le!
pc org oINPTR + le!
kernelend
