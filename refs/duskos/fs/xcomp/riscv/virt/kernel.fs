\ Kernel for RISC-V Virt emulation
\ Target : https://www.qemu.org/docs/master/system/riscv/virt.html
f<< /asm/riscv.fs

$80000000 to binstart
f<< /xcomp/riscv/kernel.fs

lblcoldboot rforward!
    xSYSVARS binstart li, \ SYSVARS = binstart+4 (1 byte for 1-byte instr)
    lblbaseboot absb,

xlatest org oSYSDICT + le!
pc org oINPTR + le!
kernelend
