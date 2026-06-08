needs lib/ival io/blk drv/usb/struct drv/usb/const drv/usb
unit drv/usb/blk

struct UMS { uint epin epout nlun ; }
extends Blk struct USBBlk { uint ums lun ; }

31 const CBWSZ
create CBW n"USBC" be, CBWSZ 4- allot0
CBW absvalmap {
  +$04 leint CBWTag CBWDataTransferLength ;
  uchar CBWFlags CBWLUN CBWCBLength ;
  [uchar,0] CBWCB ;
}

: CBW$ ( blk -- )
  CBW 4+ CBWSZ 4- 0 cfill
  lun to CBWLUN ;

: CB! ( a u -- )
  dup to CBWCBLength
  CBWCB swap cmove ;

16 const CSWSZ \ struct is 13, but USB yields 16 bytes
create CSW CSWSZ allot0
CSW absvalmap {
  beint CSWSig ;
  leint CSWTag CSWDataResidue ;
  uchar CSWStatus ;
}

: CSW# CSWSig n"USBS" <> ?abort"wrong CSW signature" ;

: writeCBW ( ums -- )
  CBWSZ CBW rot epout epwrite CBWSZ <> ?abort"writeCBW failed" ;

: readCSW ( ums -- sts )
  CSWSZ CSW rot epin epread 13 < ?abort"readCSW failed"
  CSW# CSWStatus ;
: readCSW# readCSW ?abort"CSW reports error" ;

: resetblk ( dev -- )
  >r 0 0 0 0 $ff ( reset ) Rh2d Rclass Riface or or r> Dev.ep0 usbcmd drop ;

create _ 1 allot0
: getmaxlun ( dev -- n )
  >r 1 _ 0 0 $fe ( getmaxlun ) Rd2h Rclass Riface or or r> Dev.ep0 usbcmd
  1 <> ?abort"getmaxlun bad request" _ c@ ;

\ yes I know, it means only 1 usbblk. I'm planning a drv/usb overhaul
$500608 const UMSCSP
: findep# findep ?dup not ?abort"expected EP not found" ;
: umsieps ( dev -- iepin iepout )
  >r Tbulk Ein UMSCSP r@ findep# Tbulk Eout UMSCSP r> findep# ;
: findums# ( -- dev ) UMSCSP findcsp ?dup not ?abort"couldn't find ums" ;
: newums ( dev -- ums )
  r! resetblk r@ getmaxlun 1+ ( nlun V1=dev )
  r@ umsieps ( nlun iepin iepout )
  V1 newdevep ( nlun iepin epout )
  swap r> newdevep 3 n,@ ;

create _ 6 allot0
: testunitready ( blk -- sts )
  dup CBW$ ums _ 6 CB!
  dup writeCBW readCSW ;

: CBread# ( a ums -- )
  CBWDataTransferLength r! rot> epin epread
  r> <> ?abort"CBread# length mismatch" ;

: CBwrite# ( a ums -- )
  CBWDataTransferLength r! rot> epout epwrite
  r> <> ?abort"CBwrite# length mismatch" ;

create _ $12 c, 0 c, 0 c, $24 wle, 0 ,
: inquiry ( a n blk -- )
  dup CBW$ ums
  swap to CBWDataTransferLength
  $80 to CBWFlags
  _ 6 CB! ( a ums )
  dup writeCBW
  tuck CBread#
  readCSW# ;

create buf 8 allot
create _ $25 c, 9 allot0
: readcapacity(10) ( blk -- blkcnt blksz )
  dup CBW$ ums
  8 to CBWDataTransferLength
  $80 to CBWFlags
  _ 10 CB! ( ums )
  dup writeCBW
  buf over CBread#
  readCSW#
  buf be@ 1+ buf 4+ be@ ;

create _ $28 c, 0 c, 0 be, 0 c, 1 wbe, 0 c,
: :readblk ( n dst blk -- )
  dup CBW$
  rot _ 2+ be! ( dst blk )
  dup blksz to CBWDataTransferLength
  $80 to CBWFlags
  _ 10 CB! ( dst blk )
  ums dup writeCBW
  tuck CBread#
  readCSW# ;

create _ $2a c, 0 c, 0 be, 0 c, 1 wbe, 0 c,
: :writeblk ( n src blk -- )
  dup CBW$
  rot _ 2+ be! ( src blk )
  dup blksz to CBWDataTransferLength
  _ 10 CB! ( src blk )
  ums dup writeCBW
  tuck CBwrite#
  readCSW# ;

: *max ( n n -- n ) 2dup * dup rot / rot <> if drop -1 then ;

: newusbblk ( lun ums -- blk )
  2dup nlun >= ?abort"LUN out of bounds"
  ['] :readblk ['] :writeblk 512 -1 newblk >r , , \ V1=blk
  r@ readcapacity(10) dup r@ to blksz *max r@ to size r> ;
