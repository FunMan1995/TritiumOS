needs io/stream num/math xcomp/deploy xcomp/tools xcomp/m68k/cpusel fs/fatt
unit xcomp/m68k/mac/deploy

$200 const SECSZ
SECSZ 2* const BOOTSECSZ
\ These few bytes are the beginning of a HFS header that is wired to
\ jump to offset +8a on boot. It's small enough that it can fit in a FAT!
create hfshdr map< c, $4c $4b $60 $00 $00 $86 $44 $18 $00 $00
create image BOOTSECSZ allot0

stringlist postlude drv/mac68k/disk xcomp/m68k/mac/glue

: mkfloppyfat ( blk -- )
  fatopts$ 1 to secperclus 120 to rsvdsec
  18 to secpertrk 2 to numheads 0 to drvnum
  newFAT ;

: installboot ( blk -- blk )
  0 over seek
  dup 0 image rot readblk
  hfshdr image 10 cmove
  xcomp[] image $8a + swap cmove ( blk )
  dup image BOOTSECSZ rot write# ;

: installdbg ( blk -- )
  "xcomp/m68k/mac/bootdbg.fs" loadpath
  installboot flush ;

: install ( blk -- )
  "xcomp/m68k/kernel.fs" loadpath
  "xcomp/m68k/mac/boot.fs" loadpath
  installboot >r ( ) \ V1=blk
  kernel[] V1 write#
  V1 pos SECSZ roundup V1 seek
  V1 "m68k" spitboot
  V1 "drv/mac68k/qd" spitunit
  V1 fsUnits spitunits
  V1 fatUnits spitunits
  V1 postlude spitunits
  r> flush ;

stringlist floppypaths app ar asm com data fs gr hal io lib mem num text \
  comp/c comp/c.fs comp/sig.fs comp/sym.fs comp/tok.fs comp/w.fs \
  drv/mac68k drv/timer.fs arch/core.fs arch/m68k \
  xcomp/m68k xcomp/boot.fs xcomp/deploy.fs xcomp/lo.fs xcomp/tools.fs \
  tests/harness.fs tests/kernel.fs tests/hal
: copyfloppy floppypaths copylist ;
