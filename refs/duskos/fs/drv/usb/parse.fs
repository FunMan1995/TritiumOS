\ Parts of this code is derived from Plan 9
\ Source: http://www.9legacy.org/ 4th release from 2015-01-10
\ Filenames: sys/src/cmd/usb/usbd.c sys/src/cmd/usb/usbd/dev.c
\ License: /licenses/plan9legacy
needs lib/psrs lib/struct hal/muldiv lib/fmt drv/usb/const drv/usb/struct
unit drv/usb/parse

: mkep ( id idev -- iep )
  over here# IEPSZ allot0 ( id idev id iep )
  tuck to iepid ( id idev iep )
  rot> ieps* [*n+] 4 dipdup ! ( iep ) ;

\ layout of standard descriptor types
struct Common {
  uchar bLength bDescriptorType ;
}
2 const COMMONHDRSZ

: le24@ dup wle@ swap 2+ c@ 16 lshift or ;

extends Common struct Device {
  ushort bcdUSB ;
  [void,0] CSP24 ;
  uchar bDevClass bDevSubClass bDevProtocol bMaxPacketSize0 ;
  leshort idVendor idProduct bcdDev ;
  uchar iManufacturer iProduct iSerialNumber bNumConfigurations ;
}

: parsedev ( u ddev dev -- )
  rot drop ( ddev dev )
  over A! bLength Ddevlen < if
    drop bLength .f"short dev descr: %d\n" abort then
  A> bDescriptorType Ddev <> if
    drop bDescriptorType .f"%d is not a dev descriptor type\n" abort then
  A> bMaxPacketSize0 ( ddev dev maxpkt )
  over Dev.info rot> swap Dev.ep0 dipdup to maxpkt ( ddev info maxpkt )
  over ieps* @ to iepmaxpkt ( ddev info )
  swap A! CSP24 le24@ ( info csp ) \ A=ddev
  over to csp ( info )
  A> bNumConfigurations ?dup not ?abort"no configuration" ( info nconf )
  over to nconf ( info )
  A> bDevClass over to class
  A> idVendor over to vid
  A> idProduct over to did
  A> bcdDev over to dno
  A> iManufacturer over to vsid
  A> iProduct over to psid
  A> iSerialNumber swap to ssid ;

extends Common struct Iface {
  uchar bInterfaceNumber bAlternateSetting bNumEndpoints ;
  [void,0] CSP24 ;
  uchar bInterfaceClass bInterfaceSubClass bInterfaceProtocol iInterface ;
}

: parseiface ( 'csp 'id n diface idev -- )
  rot Difacelen < ?abort"short iface descriptor" ( 'csp 'id diface idev )
  swap A! bInterfaceNumber ( 'csp 'id idev id A=diface )
  rot dipdup ! ( 'csp idev id )
  rot A> CSP24 le24@ ( idev id 'csp csp )
  swap dipdup ! ( idev id csp )
  \ if dev doesn't have a CSP, set it
  rot A! csp not if dup A> to csp then ( id csp A=idev )
  A> class not if dup $ff and A> to class then
  \ if id=0, set ep0 iface info
  over if 2drop else A> ieps* @ A! to ifacecsp A> to ifaceid then ;

extends Common struct Endpoint {
  uchar bEndpointAddress bmAttributes ;
  leshort wMaxPacketSize ;
  uchar bInterval ;
}

: parseendpt ( n dep idev -- iep )
  rot Deplen < ?abort"short ep descriptor" ( dep idev )
  over bEndpointAddress $f and ( dep idev id )
  dup 4* oover ieps* + @ ( dep idev id iep )
  ?dup if \ existing IEp? maybe it has both directions
    nip nip over bEndpointAddress $80 and over addr $80 and <> if
      Eboth over to dir then
  else ( dep idev id ) \ create new IEp
    swap mkep ( dep iep )
    over bEndpointAddress $80 and if Ein else Eout then
    over to dir then ( dep iep )
  swap A! wMaxPacketSize ( iep maxpkt A=dep )
  2dup $7ff and swap to iepmaxpkt ( iep maxpkt )
  11 rshift 3 and 1+ over to ntds ( iep )
  A> bEndpointAddress over to addr ( iep )
  A> bmAttributes 2dup 3 and 1+ swap to ieptype ( iep attrs )
  4/ 3 and over to isotype ( iep ) ;

variable _id
variable _csp
: parsedesc ( n a idev -- )
  >r >r >r begin ( ) \ V1=idev V2=a V3=n
    V3 while
    V2 A! c@ dup V3 <= while ( len A=a )
    A> 1+ c@ ( type ) case ( len )
      bi Ddev = | Dconf = or of r@ . abort" unexpected descriptor\n" endof
      Diface = of _csp _id V3 V2 V1 parseiface endof
      Dep = of
        V3 V2 V1 parseendpt ( len iep )
        _id @ over to ifaceid
        _csp @ swap to ifacecsp endof
      drop endcase ( len )
    2r> rot consume[] 2>r repeat ( len ) drop then ( )
  2rdrop rdrop ;

extends Common struct Conf {
  leshort wTotalLength ;
  uchar bNumInterfaces bConfigurationValue iConfiguration bmAttributes ;
  uchar MaxPower ;
}

: parseconf ( n dconf idev -- len )
  oover Dconflen < ?abort"short configuration descriptor"
  over bDescriptorType Dconf <> ?abort"not a conf descriptor"
  oover oover wTotalLength r! < ?abort"truncated config" ( n dconf idev V1=len )
  rot Dconflen - rot Dconflen + rot ( n a idev )
  parsedesc r> ;

extends Common struct Hub {
  uchar bNbrPorts ;
  leshort wHubCharacteristics ;
  uchar bPwrOn2PwrGood bHubContrCurrent ;
  [void,0] DeviceRemovable ; \ variable length
}

\ TODO: parse "removable" and "pwrctl"
\ I didn't translate the logic because it *seems* to me that its logic is off
\ by one. I'll just redo from scratch when I need those fields.
: parsehub ( dhub hub -- )
  swap r! bPwrOn2PwrGood 2* Powerdelay max swap A! to pwrms \ A=hub V1=dhub
  V1 bHubContrCurrent A> to maxcurrent
  V1 wHubCharacteristics dup 3 and A> to pwrmode ( charac )
  dup 4 and bool A> to compound ( charac )
  $80 and bool A> to leds ( )
  r> bNbrPorts A> to nport ;
