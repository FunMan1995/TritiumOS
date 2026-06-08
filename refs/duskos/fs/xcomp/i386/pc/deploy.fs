needs lib/str xcomp/tools xcomp/deploy io/stream mem/stream
unit xcomp/i386/pc/deploy

\ sector to target for "secld". absolute max: 58
58 const SECLDTGT \ ok, we're super tight!
stringlist preludelba drv/pc/int13hl io/secld
stringlist preludechs drv/pc/int13hc io/secld

: spitkernel1 ( stream prelude -- ) >r >r \ V1=prelude V2=stream
  "xcomp/i386/pc/kernel.fs" loadpath
  kernel[] V2 write#
  V2 "i386" spitboot
  V2 V1 spitunits
  SECLDTGT formatdec V2 write#
  " secload " r> puts rdrop ;

: spitkernel2 ( stream -- )
  dup fsUnits spitunits
  dup fatUnits spitunits
  "xcomp/i386/pc/glue" spitunit ;

: _install ( blk prelude -- )
  >r >r \ V1=prelude V2=blk
  $200 V2 seek V2 V1 spitkernel1
  SECLDTGT $200 * V2 seek V2 spitkernel2
  r> close rdrop ;

: install    ( drive -- ) preludelba _install ;
: installchs ( drive -- ) preludechs _install ;

stringlist floppypaths app ar asm bench com data fs hal io lib mem num text \
  comp/c comp/c.fs comp/sig.fs comp/sym.fs comp/tok.fs comp/w.fs \
  drv/pc drv/usb drv/pci.fs drv/ps2.fs drv/timer.fs drv/usb.fs \
  arch/core.fs arch/i386 \
  xcomp/i386 xcomp/boot.fs xcomp/deploy.fs xcomp/lo.fs xcomp/tools.fs
: copyfloppy floppypaths copylist ;

: rebootinto ( -- )
  $500 $7700 newmemstream preludelba spitkernel1 $500 execute ;
