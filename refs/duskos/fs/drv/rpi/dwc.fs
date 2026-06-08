needs lib/type lib/struct drv/usb drv/rpi/vcore comp/sig comp/c
unit drv/rpi/dwc

annotate ( uint -- AnyPtr ) usbpadallot

\ From Plan 9's block system. sys/src/9/port/ucallocb.c
\ rp   first unconsumed byte
\ wp   first empty byte
\ lim  1 past the end of the buffer
\ base start of the buffer
struct Block {
  *Block next list ;
  *uchar fieldst rp wp lim base ;
}

: allocb ( u a -- block )
  Block typesz usbpadallot A! ( u a blk )
  rot> 0 A> to next 0 A> to list ( blk u a )
  dup A> to base dup A> to wp dup A> to rp + A> to lim ;
annotatelast ( uint AnyPtr -- *Block )

\ kernel xconsts can't be picked up by comp/c
DMABASE const DMABASE

:c enum {
  Maxctllen = 32*1024, /* max allowed sized for ctl. xfers; see Maxdevconf */

  /* req offsets */
  Rtype   = 0,
  Rreq    = 1,
  Rvalue  = 2,
  Rindex  = 4,
  Rcount  = 6,
  Rsetuplen = 8,
};

\ from vcore
annotate ( uint uint -- ) setpower

struct Hostchan {
  uint hcchar hcsplt hcint hcintmsk hctsiz hcdma _pad hcdmab ;
}

struct Dwcregs {
  uint gotgctl gotgint gahbcfg gusbcfg grstctl gintsts gintmsk grxstsr
       grxstsp grxfsiz gnptxfsiz gnptxsts gi2cctl gpvndctl ggpio guid
       gsnpsid ghwcfg1 ghwcfg2 ghwcfg3 ghwcfg4 glpmcfg gpwrdn gdfifocfg
       adpctl ;
  +$100 uint hptxfsiz ;
  [uint,15] dtxfsiz ;
  +$400 uint hcfg hfir hfnum _pad hptxsts haint haintmsk hflbaddr ;
  +$440 uint hport0 ;
  +$500 [Hostchan,16] hchan ;
  +$e00 uint pcgcctl ;
}

extends Hci struct DWCHci {
  *Dwcregs regs ;
  uint nchan splitretry ;
}

extends Ep struct DWCEp {
  *Block epbuf ;
}

cc<< /drv/rpi/dwc.c

: attached# ( ep -- ep ) dup Ep.dev Dev.state Ddetach = ?abort"deatched EP" ;
: :epread ( n a ep -- n ) attached# _epread ;
: :epwrite ( n a ep -- n ) attached# _epwrite ;

create myhci DWCHci typesz allot0

: dwc$
  MMIO_BASE $980000 + myhci to regs
  myhci regs gsnpsid $ffff invand n"OT\0\0" <> ?abort"dwc$ error"
  1 myhci to Hci.nports
  1 myhci to Hci.highspeed
  ['] :epread myhci to Hci.epread
  ['] :epwrite myhci to Hci.epwrite
  ['] _roothubfeature myhci to Hci.roothubfeature
  myhci init
  myhci newroothub drop
  usbwork ;
