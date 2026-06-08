needs drv/arm/sccp
unit drv/arm/mmu

consts $0c02 UNCACHEABLE $0c0e CACHEABLE $0c06 DEVICE $2c02 XDEVICE

$4000 const 16KB
16KB alignheren here 16KB allot const ttbr

: ttbr$ ( -- )
  ttbr 16KB 4/ 0 do ( a ) i 20 lshift UNCACHEABLE or swap !+ loop drop ;

: mmu$ ( -- )
  ttbr$ -1 domctl! ttbr $19 ( RGN|C ) or ttbr0!
  armctl@ $1000 invand armctl! \ disable icache
  armctl@ $1001 or armctl! ; \ enable MMU and icache

: memflags! ( flags a u -- )
  swap 20 rshift 4* ttbr + ( f u a )
  swap 0 do ( f a )
    2dup @ $fff00000 and or swap !+ loop 2drop ;