needs fs/sh xcomp/deploy io/stream xcomp/efi
stringlist therest drv/efi drv/efi/devpath drv/efi/image drv/efi/blkio
f<< xcomp/amd64/efi/kernel.fs
herestream "amd64" spitboot
herestream fsUnits spitunits
herestream fatUnits spitunits
herestream therest spitunits
herestream f"xcomp/amd64/efi/glue.fs" spitclose
kernelend
unit xcomp/amd64/efi/deploy

: spitefi ( stream -- ) >r \ V1=stream
  PEAMD64 kernel[] nip spitpe V1 write#
  kernel[] V1 write#
  kernel[] nip $200 mod ?dup if $200 swap- 0 do 0 V1 putc loop then rdrop ;
