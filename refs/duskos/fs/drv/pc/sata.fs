needs lib/struct
unit drv/pc/sata

struct FISH2D {
  uchar fistype ;
  uchar fisflags ; \ b7=C
  uchar command features sector ;
  leshort cylinder ;
  uchar device sectorhi ;
  leshort cylinderhi ;
  uchar featureshi ;
  leshort count ;
  uchar _pad control ;
  uint _pad ;
}
$27 const FISH2DTYPE

: lba! ( sec self -- ) over 8 rshift over to cylinder to sector ;
: flagc! $80 swap to fisflags ;
: dma@ ( sec self -- )
  tuck lba! dup flagc! $25 ( READ DMA EX ) over to command
  1 over to count $40 swap to device ;
: dma! ( sec self -- )
  tuck lba! dup flagc! $35 ( WRITE DMA EX ) over to command
  1 over to count $40 swap to device ;

\ FISPIOSetup
\ Yes, some fields shadow, FISH2D fields, but they're exactly the same, it
\ doesn't matter.
struct FISPIOSetup {
  uchar fistype ;
  uchar fisflags ; \ b6=I b5=D
  uchar status error sector ;
  leshort cylinder ;
  uchar device sectorhi ;
  leshort cylinderhi ;
  uchar _pad e_status ;
  leint _pad ;
  leshort transfercount _pad ;
}
$5f const FISPIOSETUPTYPE
