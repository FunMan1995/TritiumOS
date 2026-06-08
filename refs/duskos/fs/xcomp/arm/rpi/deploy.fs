needs xcomp/tools xcomp/deploy drv/arm/cache mem/stream
unit xcomp/arm/rpi/deploy

: makekernel1 word"xcomp/arm/rpi/kernel1.fs" f<< ;
: makekernel2 word"xcomp/arm/rpi/kernel2.fs" f<< ;

stringlist postlude drv/timer drv/rpi/timer drv/rpi/emmc xcomp/arm/rpi/glue
: spitkernel ( stream -- ) >r \ V1=stream
  kernel[] V1 write#
  V1 "arm" spitboot
  V1 fsUnits spitunits
  V1 fatUnits spitunits
  V1 postlude spitunits
  r> close ;

: rebootinto ( -- )
  disableicache disabledcache
  $20000 newmemstreambuf dup spitkernel ( st )
  writtenrange $8000 swap cmove $8000 execute ;
