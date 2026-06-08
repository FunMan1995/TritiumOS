\ Kernel for sunxi SoCs (so far, only Pine A64 LTS is tested)
\ Expect to be booted from u-boot's SPL with the "aarch64 to aarch32" treatment
\ applied.
needs asm/arm
\ SPL loads binary at $4a000000, but the first $100 bytes are taken by
\ toaarch32.bin
$4a000100 to binstart

f<< /xcomp/arm/kernel.fs

lblccache forward!
  ret,

lblcoldboot forward!
  mov) r7 rd) $4a000000 imm) ,) \ SYSVARS
  orr) r7 rdn) $100 imm) ,)
  mov) r0 rd) $4a000000 imm) ,)
  orr) r0 rdn) $20000 imm) ,)
  str) r0 rd) r7 rn) oSYSALLOC +i) ,)
  lblbaseboot absb,

xlatest org oSYSDICT + le!
pc org oINPTR + le!
kernelend
