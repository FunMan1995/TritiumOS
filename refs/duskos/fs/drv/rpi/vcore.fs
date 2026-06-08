needs lib/ival
unit drv/rpi/vcore

0 const PHYSDRAM        \ addresses as seen from the arm cpu
$40000000 const BUSDRAM \ addresses as seen from the videocore gpu

8 const NRegs
$18 const Status
$f const ChanMask
$80000000 const Full
$40000000 const Empty
MMIO_BASE $b880 + const MAILBOXIN
MAILBOXIN NRegs 4* + const MAILBOXOUT

: vcwrite ( val chan -- )
  begin MAILBOXOUT Status + @ Full and not until ( val chan )
  swap ChanMask invand or MAILBOXOUT ! ;

: vcread ( chan -- val )
  begin
    begin MAILBOXIN Status + @ Empty and not until ( chan )
    dup MAILBOXIN @ tuck ChanMask and <> while ( chan val )
    drop repeat ( chan val )
  nip ChanMask invand ;

\ Pointers passed around in mailboxes must be aligned to 16 bytes
16 alignheren here $100 allot const mbox
mbox absvalmap {
  uint _len _req _tag _tagbuflen _taglen ;
  [void,0] _data ;
}
13 4* const PROPSZ

: _chk# ( len expected -- ) <> if abort"bad vcreq" then ;

0 const Req
$80000000 const RspOk
$80000000 const TagResp
8 const ChanProps
: _vcreq ( rsplen -- rsplen )
  mbox ChanProps vcwrite ( rsplen )
  ChanProps vcread mbox _chk# ( rsplen )
  _req RspOk _chk#
  _taglen dup TagResp and TagResp _chk# ( rsplen taglen )
  TagResp invand min ( rsplen ) ;
: vcreq ( rsplen vallen buf tag -- res ) >r >r \ V1=tag V2=buf
  tuck max align4 ( vallen rsplen )
  mbox PROPSZ 0 cfill
  PROPSZ to _len
  Req to _req
  tuck ( rsplen ) to _tagbuflen ( rsplen vallen )
  dup ( vallen ) to _taglen
  V2 ( buf ) _data rot cmove ( rsplen )
  V1 to _tag
  _vcreq ( rsplen )
  _tag V1 _chk#
  dup _data r> ( buf ) rot cmove ( rsplen )
  rdrop ;

$10 alignheren here 8 4* allot const _buf

\ Values that can be used in setpower
enum PowerSd   PowerUart0 PowerUart1 PowerUsb    PowerI2c0 \
     PowerI2c1 PowerI2c2  PowerSpi   PowerCcp2tx

2 const Powerwait
: setpower ( on dev -- )
  _buf ! bool Powerwait or _buf 4+ c!
  8 8 _buf $00028001 vcreq drop ;

: getclock ( -- hz )
  3 _buf ! 8 4 _buf $00030002 vcreq drop _buf 4+ @ ;

: getmaxclock ( -- hz )
  3 _buf ! 8 4 _buf $00030004 vcreq drop _buf 4+ @ ;

: setclock ( hz -- )
  0 swap 3 _buf !+ !+ !
  8 12 _buf $00038002 vcreq drop ;

: fbgetres ( -- width height )
  8 0 _buf $00040003 vcreq 8 _chk# _buf @+ swap @ ;

\ Allocate FB through the VC mailbox
\ Doing so must be done in a single request containing multiple tags.
create _fbreq
  35 4* , Req ,
  $48003 , 8 , 8 , 0 , 0 , \ set physical resolution
  $48004 , 8 , 8 , 0 , 0 , \ set virtual resolution
  $48009 , 8 , 8 , 0 , 0 , \ set virtual offset
  $48005 , 4 , 4 , 16 ,    \ set depth
  $48006 , 4 , 4 , 1 ,     \ set pixel order to RGB
  $40001 , 8 , 8 , $1000 , 0 , \ allocate FB
  $40008 , 4 , 4 , 0 ,     \ get pitch
  0 ,                      \ end of tags

: fbinit ( width height -- a pitch )
  _fbreq mbox 35 move
  dup mbox 6 4* + !
  mbox 11 4* + !
  dup mbox 5 4* + !
  mbox 10 4* + ! ( )
  33 _vcreq drop
  28 4* mbox + @ BUSDRAM 1- and
  33 4* mbox + @ ;
