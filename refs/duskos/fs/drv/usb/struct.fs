needs drv/usb/const lib/struct comp/sig
unit drv/usb/struct

struct Req {
  uchar reqtype reqnum ;
  leshort reqvalue reqindex reqcount ;
}
Req typesz const REQSZ

struct Hub { }

struct Dev {
  uint nb state speed ;
  *Hub parenthub ;
  uint portnb info ep0 ;
}
Dev typesz const DEVSZ

struct Hci { }

struct Ep {
  uint nb ;
  *Hci hci ;
  *Dev dev ;
  uint info clrhalt maxpkt ttype toggle ;
  [void,0] extra ;
}
8 const EXTRAEPSZ \ extra space for structs that extend Ep
Ep typesz EXTRAEPSZ + const EPSZ

struct Hci {
  uint nports highspeed epread epwrite ;
  xt roothubfeature ;
}
Hci typesz const HCISZ

struct IEp {
  uint addr dir ieptype isotype iepid iepmaxpkt ntds ifaceid ifacecsp ;
}
IEp typesz const IEPSZ

struct IDev {
  uint csp vid did dno vendor product serial vsid psid ssid class nconf ;
  [uint,Ndeveps] ieps* ;
}
IDev typesz const IDEVSZ

struct Port {
  uint portstate sts portdev porthub ;
}
Port typesz const PORTSZ

struct Hub {
  uint hubnext hci pwrmode compound pwrms maxcurrent leds nport hubports ;
  uint failed ;
  *Dev hubdev ; \ NULL when root hub
}
Hub typesz const HUBSZ
