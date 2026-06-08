needs io/stream xcomp/tools xcomp/deploy
unit xcomp/riscv/virt/deploy

: makekernel word"xcomp/riscv/virt/kernel.fs" f<< ;

stringlist postlude drv/riscv/ramdrive xcomp/riscv/virt/glue
: spitkernel ( stream -- ) >r \ V1=stream
  kernel[] V1 write#
  V1 "riscv" spitboot
  V1 fsUnits spitunits
  V1 fatUnits spitunits
  r> postlude spitunits ;
