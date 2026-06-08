needs lib/bit io/mouse drv/usb drv/usb/hid
unit drv/usb/mouse

\ Mouse events are 3b: 1b button state, 1b X move 1b Y move. signed.
\ Button state b0=left b1=right b2=middle

3 const MOUSEBUFSZ
8 const IdleVal \ TODO: adjust?

extends Mouse struct USBMouse { uint pollep ; }

create buf MOUSEBUFSZ allot
: poll ( mouse -- a u )
  MOUSEBUFSZ buf rot pollep epread buf swap ;

$020103 const MOUSECSP
: mouseiep ( dev -- iep )
  >r Tintr Ein MOUSECSP r> findep ?dup not ?abort"no mouse iface" ;
: findmouse ( -- ep0-or-0 ) MOUSECSP findcsp ;
: findmouse# findmouse ?dup not ?abort"no boot proto USB mouse found" ;

:> ( mouse -- )
  r! poll if ( a )
    c@+ r@ to buttons c@+ sex8 swap c@ sex8 r> moveby
    else drop rdrop then ;
: newusbmouse ( dev -- mouse )
  r! mouseiep dup ifaceid r@ Dev.ep0 bootproto! ( iep )
  \ IdleVal over ifaceid r@ Dev.ep0 setidle! ( iep )
  r> newdevep ( pollep )
  [ litn ] newmouse swap , ;
