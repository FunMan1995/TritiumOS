needs io/stream xcomp/deploy xcomp/tools xcomp/m68k/cpusel
unit xcomp/m68k/virt/deploy

$400 const BINSTART
$200000 const RAMSZ \ in sync with m68k-virt
RAMSZ $1000 - const SYSVARS
\ HERE and INPTR are in a race for memory at boot time and INPTR needs a
\ headstart. This is the size of the head start
$8000 const FILLERSZ

stringlist postlude xcomp/m68k/virt/glue

: installboot ( blk -- blk )
  0 over seek xcomp[] oover write# ;

: install ( blk -- )
  "xcomp/m68k/kernel.fs" loadpath
  "xcomp/m68k/virt/boot.fs" loadpath
  \ set INPTR
  xcomp[] nip kernel[] nip + BINSTART + FILLERSZ + ( blk inptr )
  xcomp[] drop 2+ be!
  SYSVARS xcomp[] drop 8+ be!
  >r xcomp[] V1 write#
  kernel[] V1 write#
  FILLERSZ V1 32SPCS fillstream spitn
  V1 "m68k" spitboot
  V1 fsUnits spitunits
  V1 fatUnits spitunits
  V1 postlude spitunits
  r> flush ;
