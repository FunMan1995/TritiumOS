needs lib/struct io/blk
unit drv/sunxi/smhc

$01c0f000 const SMHC0 \ SD card
$01c10000 const SMHC1 \ SD IO
$01c11000 const SMHC2 \ eMMC
$200 const SECSZ

struct SMHC {
  uint ctrl clkdiv tmout ctype blksiz bytcnt cmd cmdarg ;
  uint resp0 resp1 resp2 resp3 intmask mintsts rintsts status ;
  uint fifoth funs tbc0 tbc1 _pad csdc a12a ntsr ;
  uint _pad _pad _pad _pad _pad _pad hwrst _pad ;
  uint dmac dlba idst idie ;
  +$100 uint thdl _pad _pad ;
  uint edsd res_crc d7_crc d6_crc d5_crc d4_crc d3_crc d2_crc d1_crc d0_crc ;
  uint crc_sta _pad _pad drv_dl smap_dl ds_dl ;
  +$200 uint fifo ;
}

: hasrx? ( smhc -- f ) status 4 and not ;
: waitrx ( smhc -- ) begin dup hasrx? until drop ;
: cmd17 ( lba smhc -- ) A! to cmdarg $80000211 A> to cmd ;
: smhc$ ( smhc -- ) SECSZ swap to bytcnt ;

extends Blk struct SMHCBlk { *SMHC smhc ; }

: _readsector ( sec dst drv -- )
  smhc >r swap V1 cmd17 ( dst ) \ V1=smhc
  SECSZ 4/ 0 do V1 waitrx V1 fifo swap !+ loop ( dst )
  drop rdrop ;
: _writesector abort"TODO: writesector" ;
: newsmhcdrive ( smhc -- drv )
  dup smhc$
  ['] _readsector ['] _writesector SECSZ -1 newblk ( smhc drv )
  swap , ;
