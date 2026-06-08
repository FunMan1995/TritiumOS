\ Kernel for Raspberry Pi model 1
needs asm/arm
$8000 to binstart
f<< /xcomp/arm/kernel.fs
$20000000 xconst MMIO_BASE
0 xconst DMABASE

lblccache forward!
  ret,

lblcoldboot forward!
  mov) r7 rd) binstart imm) ,) \ SYSVARS
  lblbaseboot absb,

xlatest org oSYSDICT + le!
pc org oINPTR + le!
kernelend
