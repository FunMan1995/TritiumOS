needs lib/psrs lib/ival com/link com/ip4 com/ether
unit com/arp

\ Cache section
$40 const ARPCACHECNT
create ipaddrs ARPCACHECNT 4* allot
create macaddrs ARPCACHECNT 6 * allot
0 value cachecnt

: arpadd ( ip mac -- )
  cachecnt ARPCACHECNT = ?abort"ARP cache full"
  cachecnt 6 * macaddrs + macmove
  cachecnt 4* ipaddrs + !
  doto cachecnt 1+ | ;

: arplookup ( ip -- mac-or-0 )
  -1 over = if drop BROADCAST exit then
  ipaddrs cachecnt idx if 6 * macaddrs + else 0 then ;

: ?arpadd ( ip mac -- )
  over arplookup ?dup if macmove drop else arpadd then ;

: .arpcache ( -- )
  cachecnt 0 do
    ipaddrs i 4* + @ .addr ." : " macaddrs i 6 * + .mac nl> loop ;

\ Request section
28 const ARPSZ

addrof payloadptr ivalmap {
  beshort HRD PRO ;
  uchar HLN PLN ;
  beshort OP ;
  [void,0] SHA ;
}

: SPA SHA HLN + ;
: THA SHA HLN + PLN + ;
: TPA SHA HLN 2* + PLN + ;

create arpreq $00010800 be, $06040001 be,
: arpreq? addrof HRD arpreq 2 []= TPA be@ selfaddr = and ;
: .arp ( -- )
  ."src MAC: " SHA .mac nl>
  ."src IP:  " SPA be@ .addr nl>
  ."dst MAC: " THA .mac nl>
  ."dst IP:  " TPA be@ .addr nl> ;
: arpreply ( -- )
  curlink replytoframe
  ARPSZ to payloadsz
  2 to OP
  SPA @ TPA @! SPA !
  SHA THA macmove
  dup macptr SHA macmove
  curlink sendframe ;

create arprepl $00010800 be, $06040002 be,
: arpreply? addrof HRD arprepl 2 []= ;

: ?addcurrent ( -- ) SPA be@ SHA ?arpadd ;
: handlearp ( -- )
  arpreq? if ?addcurrent arpreply then
  arpreply? if ?addcurrent then ;

: arprequest ( ipaddr link -- )
  FTARP to frametype beginframe ( ipaddr )
  ARPSZ to payloadsz
  arpreq payloadptr 2 move
  curlink macptr SHA macmove
  selfaddr SPA be!
  THA 6 0 cfill
  ( ipaddr ) TPA be! ( )
  BROADCAST dstmac macmove
  curlink sendframe ;

\ realias from com/ether
:realias :sendframewrapper ( link xt -- )
   0 >r ethertype>ft FTIP4 = if ( link xt V1=arptarget )
    DestAddr arplookup ?dup not if ( link xt )
      DestAddr to V1 BROADCAST then ( link xt dstmac )
    dstmac macmove then ( link xt )
  dipdup execute ( link )
  r> ?dup if ( link ip ) swap arprequest else drop then ;
