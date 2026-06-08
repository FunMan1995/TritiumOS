needs lib/type lib/ival lib/coop com/ether com/ip4 com/udp
unit com/dhcp

\ Possible values for Op
consts 1 OPREQUEST 2 OPREPLY

\ DHCP option codes
consts
  1 SUBNETMASK 3 ROUTERADDRS \
  50 REQUESTADDR 51 LEASETIME 52 OPTOVERLOAD 53 MSGTYPE 54 SERVERID \
  255 ENDOPTS

\ DHCP message types
consts 1 DISCOVER 2 OFFER 3 REQUEST 4 DECLINE \
       5 ACK 6 NACK 7 RELEASE 8 INFORM

ip4payload offsetof Data ivalmapfrom {
  uchar Op HType Hlen Hops ;
  beint XID ;
  beshort Secs Flags ;
  beint CIAddr YIAddr SIAddr GIAddr ;
  [uchar,16] CHAddr ;
  [uchar,64] SName ;
  [uchar,128] File ;
  [void,0] Options ;
}

\ When creating a msg, this points to the address to write the next opt at.
0 value optptr

: length! ( addr -- ) ip4payload @ - to Length ;

$63825363 const MAGIC
: opts$ MAGIC Options be! Options 4+ dup to optptr length! ;
: dhcpmsg$ ( -- )
  Data offsetof Options 0 cfill
  OPREQUEST to Op
  1 to HType 6 to Hlen \ Ethernet
  opts$ ;

: newopt ( code len -- a ) tuck doto optptr tuck + 2+ dup length! | c!+ c!+ ;
: endopts ENDOPTS 0 newopt drop ;
: readopts ( -- )
  Options be@ MAGIC = if Options 4+ else 0 then to optptr ;
: nextopt ( -- ?len ?a code-or-0 )
  optptr dup not if exit then
  begin c@+ ?dup until ( a code )
  dup ENDOPTS = if 0 to optptr 2drop 0 exit then ( a code )
  swap c@+ 2dup + to optptr ( code a len )
  swap rot ;
: findopt ( code -- ?len ?a f )
  >r readopts begin nextopt ?dup while ( len a code )
    r@ = if 1 rdrop exit then 2drop repeat rdrop 0 ;

enum Idle Discover Request Bound
Idle value state
: .dhcp [ 'e Bound litn ] 3 state - 0 do @ loop entryname[] rtype nl> ;
: .dhcpwait ( -- )
  state .dhcp begin
    dup Discover Request within? while idle
    state tuck <> if .dhcp then repeat drop .ip4 ;
: fail Idle to state ;

: msgtype ( -- n-or-0 ) MSGTYPE findopt if nip c@ else 0 then ;

: replyoffer ( -- )
  SERVERID findopt if nip be@ else exit then ( serverip )
  Request to state
  com/udp reply
  0 to SourceAddr -1 to DestAddr \ request also needs to be broadcasted
  OPREQUEST to Op
  opts$
  REQUEST MSGTYPE 1 newopt c!
  ( serverip ) SERVERID 4 newopt be!
  YIAddr REQUESTADDR 4 newopt be! endopts
  com/udp wrap curlink sendframe ;

: dhcp>ip4 ( -- )
  Bound to state
  YIAddr to selfaddr
  readopts begin nextopt ?dup while ( len a code ) case
    SUBNETMASK = of be@ to subnetmask drop endof
    ROUTERADDRS = of be@ to routeraddr drop endof
    2drop drop endcase repeat ;

createapplication DHCPBG
:> DestPort 68 <> Op OPREPLY <> or if exit then
   state case
     Discover = of msgtype OFFER = if replyoffer else fail then endof
     Request = of msgtype ACK = if dhcp>ip4 else fail then endof
     drop endcase ;
IP4UDPRECV DHCPBG sethandler
DHCPBG newcontext const dhcpctx

: senddiscover ( link -- )
  dhcpctx launchcontext
  Discover to state
  r! newdgram dhcpmsg$
  0 to SourceAddr -1 to DestAddr
  68 to SourcePort 67 to DestPort
  $8000 to Flags
  srcmac CHAddr macmove
  DISCOVER MSGTYPE 1 newopt c! endopts
  com/udp wrap r> sendframe ;
