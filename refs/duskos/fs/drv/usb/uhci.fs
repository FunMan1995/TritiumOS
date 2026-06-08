needs lib/struct lib/ival lib/psrs \
      drv/pci drv/pc/ioport \
      drv/usb drv/usb/struct drv/usb/const
unit drv/usb/uhci

4 const FRLISTMAX
0 value curfrlist
$1000 const FRLISTSZ
FRLISTSZ alignheren here FRLISTSZ FRLISTMAX * allot const FRLISTS
: allocfrlist ( qh -- a )
  curfrlist FRLISTMAX = ?abort"too many UHCI FR lists"
  curfrlist FRLISTSZ * FRLISTS + ( qh a )
  tuck FRLISTSZ 4/ rot 2 or fill
  doto curfrlist 1+ | ;

variable hciport
hciport ivalmap {
  port16 USBCMD USBSTS USBINTR FRNUM ;
  port32 FRBASEADD ;
  port8 SOFMOD ;
}

$69 const PIDIN
$e1 const PIDOUT
$2d const PIDSETUP

$10 const TDSZ
$10 alignheren TDSZ allot@ const TD
TD absvalmap { uint link ctl token bufptr ; }

: newqh ( -- qh ) $10 alignheren 1 1 2 n,@ ;
variable curqh
curqh ivalmap { uint qlink elemlink ; }

0 value curep \ Ep currently being "worked on"

: QH$ curqh @ 2 1 fill 1 waitms ;
: TD$ QH$ TD 4 0 fill 1 to link TD to elemlink ;

: eppair ( -- addr enpt )
  curep Ep.dev dup Dev.state Denabled = if ( dev )
    Dev.nb curep Ep.nb else drop 0 0 then ;

\ Printing
: .link ( n -- ) 4 bitsplit .x1 spc> .x nl> ;
: .td link ."L " .link
      ctl ."C " .x nl>
      token ."T " .x nl>
      bufptr ."B " .x nl> ;
: .qh qlink ."L " .link elemlink ."E " .link ;

17 6 bitfield TDerr
23 1 bitfield TDactive
19 1 bitfield TDNAK
0 10 bitfield TDActLen
: NAK? ( -- f ) ctl TDNAK ;
\ excludes NAK from errors
: Err ( -- bits ) ctl TDerr 4 invand ;
: TDerr# ( -- ) Err if .td abort" TD err" then ;
: ActLen ctl 1+ TDActLen ;
\ Aborts on error, except on NAK, which yield f=0. Otherwise, yield f=1.
: waitTD ( -- f )
  ticks begin 10 over ?timeoutms"waitTD timeout"
    NAK? not while
    TDerr# ctl TDactive while repeat
    1 else 0 then ( ts f ) nip QH$ ;

0 8 bitfield TDPID
8 7 bitfield TDaddr
15 4 bitfield TDenpt
19 1 bitfield TDdata1
21 12 bitfield TDlen

: TDbuf! ( a -- ) to bufptr ;
: TDdata! ( data1? -- ) token to TDdata1 to token ;
: TDtok! ( pid len -- )
  1- 0 to TDlen to TDPID ( token )
  curep toggle swap to TDdata1 ( token )
  eppair rot to TDenpt to TDaddr ( token )
  to token ;
: ctlor! ( mask -- ) ctl or to ctl ;
: ?go! ( -- f )
  TD to elemlink $00800000 ctlor!
  waitTD
  token TDdata1 1 xor curep to toggle ;
: go! ( -- ) begin ?go! until ;

: reqisD2H? ( req -- f ) reqtype Rd2h and bool ;
: cfgtd ( -- ) curep Ep.dev speed Lowspeed = if $04000000 ctlor! then ;
: doreq ( req -- )
  TD$ cfgtd TDbuf! ( )
  PIDSETUP REQSZ TDtok! 0 TDdata! go! ;
: doctl ( req -- )
  dup doreq reqisD2H? not if
    TD$ cfgtd PIDIN 0 TDtok! go! then ;
: regularread ( n a -- n )
  over begin ( n a nleft )
    ?dup while
    TD$ cfgtd
    PIDIN curep maxpkt TDtok! ( n a nleft )
    over TDbuf! ?go! not if nip - exit then ( n a nleft )
    ActLen - tuck 0< ?abort"broken epread" ( n nleft a )
    ActLen + swap repeat ( n a )
  drop ;

\ IO ports for UHCI root hub ports are used dynamically.
\ remember: portno is 1-based
: uhciport ( portno -- ctlport ) 1- 2* hciport @ + $10 + ;

extends Hci struct UHCI { uint baseport ; }

: activate ( uhci -- )
  baseport hciport !
  FRBASEADD @ 3 invand curqh ! ;
: curep! ( ep -- ) dup to curep Ep.hci activate ;

: _portenable ( portno -- res )
  uhciport 4 swap pw! ( ) 0 ;
: _portreset ( portno -- res )
  uhciport dup pw@ tuck $200 or over pw! ( oldsts port )
  50 waitms dip $200 invand | pw! 0 ;
create _ PSpresent , PSstatuschg , PSenable , PSchange , 0 , 0 , 0 , 0 ,
         PSslow , PSreset , 0 , 0 , PSsuspend , 0 , 0 , 0 ,
: _portstatus ( portno -- res )
  uhciport dup pw@ ( port sts )
  dup $a and if \ acknowledge change bits
    $a rot pw! else nip then ( sts )
  >r 0 >r _ $10 0 do ( a ) \ V1=sts V2=res
    V1 1 and if @+ doto V2 or | else 4+ then
    doto V1 2/ | loop drop ( )
  r> rdrop ;

: :roothubfeature ( on feature port hci -- res )
  activate rot drop ( feature port ) \ TODO: shouldn't I consider the "on"?
  swap case ( port )
    Fportenable = of _portenable endof
    Fportreset = of _portreset endof
    Rgetstatus = of _portstatus endof
    .x spc> abort"invalid request for root hub"
  endcase ;
: _read ( n a ep -- n ) curep! regularread ;
: _write ( n a ep -- n )
  curep! swap REQSZ = if doctl REQSZ else abort"TODO: OUT writes" then ;

: initialize ( uhci -- )
  activate
  4 to USBCMD \ RESET
  10 waitms
  newqh allocfrlist to FRBASEADD 1 to USBCMD ;
: newuhci ( port -- hci )
  ['] :roothubfeature ['] _write ['] _read 0 2 6 n,@ ;
: uhci$ ( -- )
  \ for now, we only search bus 0
  0 pcibus) buschildren $c 3 0 pcifilter dup not ?abort"no UHCI device"
  0 do ( ... pci )
    pci0.bar4 $ffe0 and newuhci dup newroothub drop ( uhci )
    initialize loop ;
