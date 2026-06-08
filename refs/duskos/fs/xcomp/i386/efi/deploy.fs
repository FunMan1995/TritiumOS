needs fs/sh xcomp/deploy io/stream xcomp/efi
stringlist therest drv/efi drv/efi/devpath drv/efi/image drv/efi/blkio
f<< xcomp/i386/efi/kernel.fs
herestream "i386" spitboot
herestream fsUnits spitunits
herestream fatUnits spitunits
herestream therest spitunits
herestream f"xcomp/i386/efi/glue.fs" spitclose
kernelend
unit xcomp/i386/efi/deploy

: spitefi ( stream -- ) >r \ V1=stream
  PEI386 kernel[] nip spitpe V1 write#
  kernel[] V1 write#
  kernel[] nip $200 mod ?dup if $200 swap- 0 do 0 V1 putc loop then rdrop ;
