needs arch/core
unit asm/dis
: dis drop ."This platform has no disassembler\n" ;
alias noop disn
endunit

: realiasall
  "dis" sysdict find# ['] dis realias
  "disn" sysdict find# ['] disn realias ;

isx86? [if]
needs asm/x86d
realiasall [then]
FAMILY_arm instrfamily? [if]
needs asm/armd
realiasall [then]
FAMILY_riscv instrfamily? [if]
needs asm/riscvd
realiasall [then]
