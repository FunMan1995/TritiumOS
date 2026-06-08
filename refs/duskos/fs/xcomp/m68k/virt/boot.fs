needs asm/label asm/m68k xcomp/tools

xcompbegin
A4 0 imm) move, \ INPTR, filled out in binary by deploy script
A5 0 imm) move, \ SYSVARS, also filled out
\ continue to kernel
xcompend
