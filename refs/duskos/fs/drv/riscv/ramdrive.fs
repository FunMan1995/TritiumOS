\ Drive in RAM (for RISC-V Emulation)
needs io/stream io/blk
unit drv/riscv/ramdrive

$83000000 const RRAMDStart
: buf( ( self -- ) drop RRAMDStart ;
: )buf ( self -- a ) bi buf( | size + ;
: _addr ( sec self -- a ) >r
  V1 blksz * r@ buf( +
  r> )buf over <= if abort"sector out of range" then ;
: _sec@ ( sec dst self -- ) >r
  swap r@ _addr swap ( src dst ) r> blksz cmove ;
: _sec! ( sec src self -- ) >r
    swap r@ _addr ( src dst ) r> blksz cmove ;

: newramdrive ( blksz blkcnt -- drv )
  dip dip ['] _sec@ ['] _sec! | | newblk ;
