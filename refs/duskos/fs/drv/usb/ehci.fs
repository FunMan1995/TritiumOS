needs lib/ival lib/struct drv/pci drv/usb
unit drv/usb/ehci

extends Ep struct EHCIEp { uint epqh eptd ; }

variable capaddr
capaddr ivalmap {
  +$00 uchar CAPLENGTH ;
  +$02 ushort HCIVERSION ;
  +$04 uint HCSPARAMS HCCPARAMS ;
  +$0c [void,0] HCSP-PORTROUTE ;
}

variable opaddr
opaddr ivalmap {
  uint USBCMD USBSTS USBINTR FRINDEX ;
  uint CTRLDSSEGMENT PERIODICLISTBASE ASYNCLISTADDR ;
  +$40 uint CONFIGFLAG ;
  +$44 [void,0] PORTSC ;
}

: ehci# capaddr not ?abort"EHCI not configured" ;

: portcnt HCSPARAMS $f and ;

: .ehci ( -- )
  ehci#
  ."Ports cnt " portcnt . nl>
  ."Cap addr  " capaddr @ .x nl>
  ."Op addr   " opaddr @ .x nl>
  ."Version   " HCIVERSION .x2 nl>
  ."HCSPARAMS " HCSPARAMS .x nl>
  ."HCCPARAMS " HCCPARAMS .x nl>
  ."USBCMD    " USBCMD .x nl>
  ."USBSTS    " USBSTS .x nl>
  ."USBINTR   " USBINTR .x nl>
  ."PORTSC    " portcnt 0 do PORTSC i 4* + @ .x spc> loop nl> ;

\ QH and TD
0 value curep \ Ep currently being "worked on"
enum OUT IN SETUP

variable curtd
curtd ivalmap {
  +$08 uint token ;
  +$0c [void,0] bufptr ;
}

\ Why so big? because it's the 64-bit version of the TD!
: newtd ( -- td ) $20 alignheren here 1 , 1 , 11 4* allot0 dup curtd ! ;

variable curqh
curqh ivalmap {
  +$04 uint enptch ;
  +$08 uint enptcap ;
  +$10 uint qhnext ;
}

: newqh ( -- qh )
  $20 alignheren here
  dup $2 or , 0 , 0 , 0 , 1 , 1 , 0 , $40 allot0
  dup curqh ! ;

newqh const HQH
$8000 to enptch \ Set H bit

: addqh ( qh -- ) dup $2 or HQH @! swap ! ;

: eppair ( -- addr enpt )
  curep Ep.dev dup Dev.state Denabled = if ( dev )
    Dev.nb curep Ep.nb else drop 0 0 then ;

: ep! ( -- )
  $2000 curep ttype Tctl = 27 lshift or ( ch )
  curep maxpkt 16 lshift or ( ch )
  curep Ep.dev Dev.speed 12 lshift or ( ch )
  eppair 8 lshift or or to enptch ( )
  $40000000 to enptcap ;

: curep!
  dup to curep epqh not if
    newtd curep to eptd
    newqh dup curep to epqh addqh then
  curep eptd curtd !
  curep epqh curqh ! ;

: nodata 0 0 ;
: TD! ( a n pid -- )
  8 lshift swap 16 lshift or $80 or ( a token )
  over bufptr !+ rot ( tok dst a )
  $fff invand
  4 0 do $1000 + dup rot !+ swap loop ( tok dst a )
  2drop to token
  curtd @ to qhnext ;

\ Theoretically, we could be calling ep! only when creating the QH, but because
\ maxpkt can change, we call it on every transfer.
: QH! ( a n pid -- ) ep! TD! ;
: active? token $80 and bool ;
: trbytes token 16 rshift $7fff and ;
: waitQH ( -- )
  ticks begin 10000 over ?timeoutms"waitQH timeout" active? not until drop ;

: reqisD2H? ( req -- f ) reqtype Rd2h and bool ;
: doreq ( req -- ) REQSZ SETUP QH! waitQH ;
: doctl ( req -- )
   dup doreq reqisD2H? not if nodata IN TD! waitQH then ;
: regularread ( n a -- n ) over IN QH! waitQH ( n ) trbytes - ;
: regularwrite ( n a -- n ) over OUT QH! waitQH ( n ) trbytes - ;

\ Hci methods
: ehciport ( n -- a ) 1- 4* PORTSC + ;
: port! ( n a -- ) tuck @ $fff invand or swap ! ;
: _portenable ( portno -- res ) ehciport 4 swap port! 0 ;
: _portreset ( portno -- res )
  ehciport $100 over port! ( a )
  50 waitms 0 swap port! 0 ;
create _
  PSpresent , PSstatuschg , PSenable , PSchange ,
  PSovercurrent , 0 , 0 , PSsuspend ,
  PSreset , 0 , PSslow , 0 ,
  PSpower , 0 , 0 , 0 ,
: _portstatus ( portno -- res )
  ehciport dup @ ( port sts )
  dup $a and if \ acknowledge change bits
    $a rot ! else nip then ( sts )
  >r 0 >r _ 16 0 do ( a ) \ V1=sts V2=res
    V1 1 and if @+ doto V2 or | else 4+ then
    doto V1 2/ | loop drop ( )
  r> rdrop ;

: :roothubfeature ( on feature port hci -- res )
  drop rot drop ( feature port ) \ TODO: shouldn't I consider the "on"?
  swap case ( port )
    Fportenable = of _portenable endof
    Fportreset = of _portreset endof
    Rgetstatus = of _portstatus endof
    .x spc> abort"invalid request for root hub"
  endcase ;
: :read ( n a ep -- n ) curep! regularread ;
: :write ( n a ep -- n )
  curep! over REQSZ = if nip doctl REQSZ else regularwrite then ;

: newehci ( -- hub ) ['] :roothubfeature ['] :write ['] :read 1 portcnt 5 n,@ ;

: resetehci ( -- )
  doto USBCMD 3 invand 2 or |
  ticks begin
    250 over ?timeoutms"EHCI reset timeout"
    USBCMD 2 and not until drop ;

: startasync HQH to ASYNCLISTADDR USBCMD $21 or to USBCMD ;
: setehciregs ( a -- ) capaddr !  capaddr @ CAPLENGTH + opaddr ! ;
: ehci$ ( regsaddr -- )
  setehciregs startasync newehci newroothub drop ;
: ehcipci$ ( -- )
  \ for now, we only search bus 0
  0 pcibus) buschildren $c 3 $20 pcifilter1 dup not ?abort"no EHCI device"
  pci0.bar0 $ff invand ehci$ ;
