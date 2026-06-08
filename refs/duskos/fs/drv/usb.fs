\ Parts of this code is derived from Plan 9
\ Source: http://www.9legacy.org/ 4th release from 2015-01-10
\ Filenames: sys/src/cmd/usb/usbd.c sys/src/cmd/usb/usbd/dev.c
\ License: /licenses/plan9legacy
needs hal/muldiv lib/psrs lib/str lib/fmt mem/scratch text/ts drv/timer \
      drv/usb/const drv/usb/struct drv/usb/parse
unit drv/usb

$1000 newscratchpad bindalloc[ _pad[
32 const USBALIGN \ we go with the strongest need among all controllers
: usbpadallot _pad[ USBALIGN alignheren allot@ ]sysmem ;

: epread ( n a ep -- n )
  >r over r@ maxpkt = r@ ttype Tctl = or if ( n a V1=ep )
    r> dup Ep.hci Hci.epread execute
  else
    r@ maxpkt dup usbpadallot tuck ( orign dst a n a )
    r> dup Ep.hci Hci.epread execute ( orign dst a n )
    >r rot r@ min ( dst src n ) dipswap cmove r> then ;

: epwrite ( n a ep -- n ) dup Ep.hci Hci.epwrite execute ;
: epopen ( ep -- ) 0 over to Ep.clrhalt Ep.extra EXTRAEPSZ 0 cfill ;

0 value usbidgen
: newusbid ( -- n )
  doto usbidgen 1+ dup | dup $7f >= if abort"too many USB addresses" then ;

: epalloc ( hp -- ep )
  here# EPSZ allot0 A! swap A> to Ep.hci ( ep )
  8 A> to maxpkt ;

\ Create endpoint 0 for a new device
: newep0 ( hci -- ep )
  r! highspeed if Highspeed else Fullspeed then \ V1=hp
  newusbid here# DEVSZ allot0 A! >r \ V2=dev
  A> to Dev.nb A> to speed Dconfig A> to Dev.state ( )
  r> r> epalloc ( dev ep )
  A! over to Dev.ep0 A> to Ep.dev ( )
  Tctl A> to ttype A> ;

: newdev ( port speed hub -- ep )
  dup Hub.hci newep0 ( port speed hub newep )
  A! >r over Lowspeed <> if 64 A> to maxpkt then \ assume full speed
  A> Ep.dev A! to parenthub A> to speed A> to portnb ( ) \ V1=newep
  r> dup epopen ;

: newdevep ( iep dev -- ep )
  r! Dev.ep0 Ep.hci epalloc ( iep ep ) \ V1=dev
  2dup to Ep.info
  over iepid over to Ep.nb
  over ieptype over to Ep.ttype
  swap iepmaxpkt over to Ep.maxpkt ( ep )
  r> over to Ep.dev dup epopen ;

create reqbuf 8 allot
: usbreq ( u a index value req type -- u a )
  reqbuf c!+ c! ( u a index value )
  reqbuf 2 + wle!
  reqbuf 4 + wle!
  over reqbuf 6 + wle! ( u a )
  ?dup if
    over dup 8 + usbpadallot ( u a u dst )
    r! swap cmove ( u ) r>
    else drop 8 reqbuf then ( u a ) ;

4 const tries
50 const delay
: usbcmd ( u a index value req type ep -- n )
  >r dup Rd2h and if
    5 roll 0 6 roll> else 0 then >r \ V1=ep V2=read-a
  5 dig >r \ V3=count
  usbreq tries 0 do ( u a )
    2dup dipdup V1 epwrite ( u a expected n )
    tuck <> ( u a n f ) if ."cmdreq: short write " else
      V2 if drop V3 V2 V1 epread then ( n )
      ?dup if break then then
    delay waitms loop ( u a n )
  broke? not if abort"usbcmd error" then
  nip nip 2rdrop rdrop ;

1024 const Maxdevconf
create buf Maxdevconf allot
:~ ( payloadsz n dev -- res )
  >r >r buf 0 Dconf 256* r> or ( u a index value )
  Rgetdesc Rd2h Rstd Rdev or or r> Dev.ep0 usbcmd ( res ) ;
: loaddevconf ( nconf dev -- nr )
  2dup REQSZ rot> ~ ( n dev res )
  Maxdevconf > ?abort"loaddevconf error" ( n dev )
  tuck buf wTotalLength rot> ~ ( dev res )
  dup 0< if nip exit then ( dev res )
  buf rot Dev.info parseconf ;

: mkstr ( n a -- str )
  swap 2/ dup 1 < if 2drop "none" exit then ( a n )
  1- dip 2+ | \ skip LANGID
  here >r dup c, 0 do ( a ) c@+ c, 1+ loop ( a )
  drop r> ;

:~ ( payloadsz sid dev -- res )
  >r >r buf 0 Dstr 256* r> or ( u a index value )
  Rgetdesc Rd2h Rstd Rdev or or r> Dev.ep0 usbcmd ( res ) ;
: loaddevstr ( sid dev -- str )
  over not if 2drop "none" exit then
  2dup REQSZ rot> ~ ( sid dev res )
  128 > ?abort"loaddevstr error" ( sid dev )
  buf bLength rot> ~ ( res )
  buf mkstr ;

: loaddevdesc ( dev -- )
  >r buf Ddevlen 0 cfill ( ) \ V1=dev
  Ddevlen buf 0 Ddev 256* ( u a index value )
  Rgetdesc Rd2h Rstd Rdev or or r@ Dev.ep0 usbcmd ( res )
  dup 0< ?abort"loaddevdesc error"
  \ Several hubs are returning descriptors of 17 bytes, not 18.
  \ We accept them and leave number of configurations as zero.
  \ (a get configuration descriptor also fails for them!)
  dup Ddevlen 1- < ?abort"short device descriptor\n"
  dup Ddevlen < if ."warning: device with short desc\n" then ( res )
  \ TODO: reuse memory
  here# r! V1 to Dev.info IDEVSZ allot0 ( res ) \ V2=idev
  0 V2 mkep ( res iep0 )
  >A Eboth A> to dir Tctl A> to ieptype 8 A> to iepmaxpkt ( res )
  buf V1 parsedev ( )
  V2 vsid V1 loaddevstr dup V2 to vendor ( str )
  "none" s= not if
    V2 psid V1 loaddevstr V2 to product
    V2 ssid V1 loaddevstr V2 to serial then ( )
  2rdrop ;

: configdev ( dev -- )
  dup loaddevdesc ( dev )
  dup Dev.info nconf 0 do ( dev )
    i over loaddevconf 0< ?abort"configdev failed" loop ( dev )
  drop ;

: isroot? hubdev not ;
: hubfeature ( on f port hub -- res )
  dup isroot? if Hub.hci roothubfeature exit then
  >r >r >r if Rsetfeature else Rclearfeature then ( cmd )
  0 0 rot r> r> ( 0 0 cmd f port )
  rot> swap ( 0 0 port f cmd )
  Rh2d Rclass Rother or or r> hubdev Dev.ep0 usbcmd ;

: getmaxpkt ( isslow ep -- n-or--1 )
  >r if 8 else 64 then buf dipdup to bMaxPacketSize0 ( u )
  buf 0 Ddev 256* Rgetdesc ( u a index value req )
  Rd2h Rstd Rdev or or r> usbcmd ( res )
  dup 0>= if drop buf bMaxPacketSize0 then ;

alias abort portdetach ( p hub -- )

\ If during enumeration we get an I/O error the hub is gone or
\ in pretty bad shape. Because of retries of failed usb commands
\ (and the sleeps they include) it can take a while to detach all
\ ports for the hub. This detaches all ports and makes the hub void.
\ The parent hub will detect a detach (probably right now) and close it later.
: hubfail ( hub -- )
  dup nport 1 do ( h ) i over portdetach loop ( h )
  1 swap to failed ;

variable hubs
variable nhubs

: detach ( dev -- ) Ddetach swap to Dev.state ;

\ TODO: As I've Forth-ified this, I noticed that this was never actually tested
\ to behave correctly, even in its C incarnation.
: closehub ( hub -- )
  dup hubs llfind ?dup not ?abort"hub not found" ( hub idx prev )
  nip over @ swap ! ( hub )
  -1 nhubs +!
  dup hubfail ( hub )
  hubdev detach ;

: portstatus ( p hub -- sts )
  dup isroot? if 0 rot> Rgetstatus rot> Hub.hci roothubfeature exit then
  >r 4 buf rot 0 Rgetstatus ( u a index value )
  Rd2h Rclass Rother or or r> hubdev Dev.ep0 usbcmd
  0< if -1 else buf wle@ then ;

create chars ,"pezor???wlh?????"
: stsstr ( sts -- str )
  $ffff and ?dup not if "-" exit then ( sts )
  buf 16 0 do ( sts a )
    over 1 and if i chars + c@ swap c!+ then ( sts a )
    dip 2/ | loop ( sts a )
  nip buf tuck - []>str ;

: confighub ( hub -- )
  >r 128 buf 0 Dhub 256* Rgetdesc ( u a index value ) \ V1=hub
  Rd2h Rclass Rdev or or V1 hubdev Dev.ep0 usbcmd
  Dhublen < ?abort"Hub short descriptor" ( )
  buf V1 parsehub
  here# V1 to hubports
  r> nport 1+ PORTSZ * allot0 ;

: createhub ( hci -- hub ) here# HUBSZ allot0 tuck to Hub.hci ;
: addhub ( hub -- ) hubs @! over ! 1 nhubs +! ;

: newroothub ( hci -- hub )
  dup createhub ( hci hub )
  swap nports 2dup swap to nport ( hub nport )
  over here# swap to hubports ( hub nport )
  1+ PORTSZ * allot0 ( hub )
  \ TODO: this was added by me. Without this, there's a timing error at USB
  \ initialization on a RPi1. Is 100ms really necessary? can it be shorter?
  Powerdelay waitms
  dup addhub ;

: newhub ( dev -- hub )
  dup Dev.ep0 Ep.hci createhub tuck to hubdev ( hub )
  dup confighub ( hub )
  dup nport 1+ 1 do ( hub )
    1 Fportpower i 3 dig hubfeature drop loop
  dup pwrms waitms ( hub )
  dup leds if dup nport 1+ 1 do ( hub )
    1 Fportindicator i 3 dig hubfeature drop loop then
  dup addhub ;

: startdev ( port -- )
  dup portdev dup Dev.info class Clhub = if ( port dev )
    newhub swap to porthub else 2drop then ;

:realias portdetach ( p hub -- )
  hubports [*n+] PORTSZ ( port )
  \ Clear present, so that we detect an attach on reconnects.
  dup doto sts PSpresent PSenable or invand |
  dup portstate Pdisabled = if drop exit then ( port )
  0 over doto porthub swap | ?dup if closehub then ( port )
  0 swap doto portdev swap | ?dup if ( dev ) detach then ( ) ;

: unstall ( ep -- )
  >r 0 0 V1 Ep.info IEp.addr ( u a idx V1=ep )
  0 ( Fhalt ) Rclearfeature Rh2d Rstd Rep or or ( u a idx val req type )
  r> Ep.dev Dev.ep0 usbcmd not ?abort"unstall failed" ;

: setmaxpkt ( val ep -- )
  over 1- 1023 > ?abort"maxpkt not in [1:1024]"
  to maxpkt ;

: portattach ( sts p hub -- ep )
  r! hubports over PORTSZ * + >r ( sts p ) \ V1=hub V2=port
  >r drop ( ) \ V3=pnum
  Pattached V2 to portstate
  Connectdelay waitms
  1 Fportenable V3 V1 hubfeature drop
  Enabledelay waitms
  1 Fportreset V3 V1 hubfeature drop
  Resetdelay waitms
  V3 V1 portstatus ( sts )
  dup 0< ?abort"portattach failed" ( sts )
  dup PSenable and not if
    1 Fportenable V3 V1 hubfeature drop ( sts )
    drop V3 V1 portstatus ( sts )
    dup PSenable and not ?abort"portattach failed" then ( sts )
  case
    PSslow and of Lowspeed endof
    PShigh and of Highspeed endof
    drop Fullspeed endcase ( speed )
  V3 over V1 newdev ( speed nep )
  r! not ?abort"newdev error" ( speed ) \ V4=nep
  0 0 0 V4 Ep.dev Dev.nb Rsetaddress ( speed u a index value )
  Rh2d Rstd Rdev or or V4 usbcmd ( speed res )
  0< ?abort"setaddress error" ( speed )
  Denabled V4 Ep.dev to Dev.state ( speed )
  Lowspeed = V4 getmaxpkt ( mp )
  dup 0< ?abort"getmaxpkt error" ( mp )
  V4 setmaxpkt ( )
  V4 Ep.dev configdev
  \ We always set conf #1. BUG.
  0 0 0 1 Rsetconf ( u a index value )
  Rh2d Rstd Rdev or or V4 usbcmd ( res )
  0< ?abort"setconf error" ( )
  Pconfiged V2 to portstate
  r> ( nep ) Ep.dev rdrop r> ( port ) rdrop ( dev port )
  dipdup to portdev ;

0 value _p
0 value _hub
0 value _oldsts
: port@ ( -- port ) _p _hub hubports [*n+] PORTSZ ;
: ?suspend ( sts -- sts )
  dup PSsuspend and not if exit then
  1 Fportenable _p _hub hubfeature drop
  Enabledelay waitms
  drop _p _hub portstatus
  dup port@ to sts ;
: ?attach ( sts -- sts )
  dup PSpresent and not if exit then
  _oldsts PSpresent and if exit then
  dup _p _hub portattach if port@ startdev then ;
: ?detach ( sts -- sts )
  dup PSpresent and if exit then
  _oldsts PSpresent and not if exit then
  _p _hub portdetach ;
: enumhub ( p hub -- res )
  dup failed if 2drop 0 exit then ( p hub )
  over to _p dup to _hub portstatus ( sts )
  dup 0< ?abort"enumhub portstatus failed"
  dup port@ doto sts swap | to _oldsts ( sts )
  nhubs @ >r ?suspend ?attach ?detach ( sts )
  drop r> ( old-nhubs ) nhubs @ <> neg ;

: usbwork ( -- )
  hubs @ begin ?dup while ( hub )
    dup nport 1+ 1 do
      i over enumhub 0< if break then loop
    \ if hub changed, start again
    broke? if drop hubs @ else hubnext then repeat ;

: ?stype ( str-or-0 -- ) ?dup if stype spc> then ;
stringlist d "IN " "OUT" "I/O"
stringlist t "ERR " "CTL " "ISO " "BULK" "INTR"
: .iep ( iep -- )
  ."ep " r! addr $f and . spc>
  r@ dir d slistiter stype spc>
  r@ ieptype t slistiter stype
  ." makpkt " r@ iepmaxpkt .
  ." if " r@ ifaceid .
  ." csp " r> ifacecsp .x nl> ;
: lsdev ( lvl dev -- )
  r! Dev.info tri did | vid | csp V1 Dev.nb \ V1=dev
  .f"dev %d csp %x vid %w did %w " ( lvl )
  r> Dev.info dup vendor ?stype ( lvl idev )
  dup product ?stype
  dup serial ?stype nl>
  Ndeveps 0 do ( lvl idev )
    i 4* over ieps* + @ ?dup if ( lvl idev iep )
      oover 1+ nspcs .iep then loop
  2drop ;

: lshub ( lvl hub -- )
  dup isroot? if
    dup nport .f"root hub with %d ports\n"
  else
    dup tri hubdev Dev.ep0 maxpkt | nport | hubdev Dev.nb
    .f"hub %d with %d ports %d maxpkt\n" then ( lvl hub )
  swap 1+ swap
  dup nport 1+ 1 do ( lvl hub )
    over nspcs
    i over portstatus i .f"port %d [%w]: "
    i over hubports [*n+] PORTSZ case ( lvl hub )
      porthub ?dup of dipover lshub endof
      portdev ?dup of dipover lsdev endof
      drop ."nothing\n" endcase
    loop 2drop ;

: lsusb ( -- )
  hubs @ ?dup not ?abort"no root hub!"
  begin ?dup while
    dup isroot? if 0 over lshub then hubnext repeat ;

: alldevs ( -- ... n )
  hubs @ >r 0 begin ( ... n V1=hub )
    V1 while V1 nport 1+ 1 do ( ... n )
      V1 hubports i [*n] PORTSZ + portdev ?dup if swap 1+ then loop
    V1 hubnext to V1 repeat rdrop ;

: finddevofclass ( subclass class -- dev-or-0 )
  swap 256* or >r alldevs begin ( ... n V1=csp )
    ?dup while 1- swap ( ... n dev )
    dup Dev.info csp $ffff and V1 <> while drop repeat
    ( ... n dev ) dip ndrop | else ( ) 0 then rdrop ;

: findcsp ( csp -- dev-or-0 )
  >r alldevs begin ( ... n V1=csp )
    ?dup while 1- swap ( ... n dev )
    dup Dev.info csp V1 <> while drop repeat
    ( ... n dev ) dip ndrop | else ( ) 0 then rdrop ;

: device@ ( nb -- dev-or-0 )
  >r alldevs begin ( ... n V1=nb )
    ?dup while 1- swap ( ... n dev )
    dup Dev.nb V1 <> while drop repeat
    ( ... n dev ) dip ndrop | else ( ) 0 then rdrop ;

: devieps ( dev -- ... n )
  0 swap Dev.info ieps* Ndeveps 0 do ( ... n ieps )
    @+ ?dup if ( ... n ieps iep ) rot> dip 1+ | then loop drop ;

: findep ( type dir csp dev -- iep-or-0 )
  swap >r rot> 2>r devieps begin ( ... n V1=csp V2=type V3=dir )
    ?dup while 1- swap ( ... n iep )
    dup tri ifacecsp V1 <> | ieptype V2 <> | dir V3 <> or or while drop repeat
    ( ... n iep ) dip ndrop | else ( ) 0 then 2rdrop rdrop ;
