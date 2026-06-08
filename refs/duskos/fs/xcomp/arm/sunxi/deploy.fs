needs xcomp/tools xcomp/deploy
unit xcomp/arm/sunxi/deploy

: makekernel word"xcomp/arm/sunxi/kernel.fs" f<< ;

stringlist postlude drv/sunxi/uart drv/sunxi/smhc xcomp/arm/sunxi/glue
: spitkernel ( stream -- ) >r \ V1=stream
  kernel kernellen V1 write#
  V1 "xcomp/lo.fs" spitfile
  V1 "xcomp/arm/hal.fs" spitfile
  V1 "xcomp/boot.fs" spitfile
  V1 fsUnits spitunits
  V1 fatUnits spitunits
  V1 postlude spitunits
  r> close ;

