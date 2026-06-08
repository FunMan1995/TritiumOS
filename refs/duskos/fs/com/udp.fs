needs lib/ival lib/diag lib/coop com/ip4
unit com/udp

17 const UDPPROTO

ip4payload ivalmap {
  beshort SourcePort DestPort Length Checksum ;
  [void,0] Data ;
}

: udpcksum ( -- n )
  0 addrof SourceAddr 8 ck[] ( sum )
  Protocol ck1 Length ck1
  SourcePort ck1 DestPort ck1 Length ck1 ( sum )
  Data Length 8- max0 ck[] ckinv ;

: check ( -- f )
  Protocol UDPPROTO = Length 8 >= and if Checksum udpcksum = else 0 then ;

: udp! ( a u -- )
  dup payloadsz >= ?abort"can't fit payload in UDP message"
  tuck 8+ to Length
  Data rot cmove ;

: udp[] ( -- a u ) Data Length 8- ;

: reply ( -- )
  com/ip4 reply
  SourcePort doto DestPort swap | to SourcePort ;

: wrap ( -- )
  UDPPROTO to Protocol
  Length DataLength!
  com/ip4 wrap
  udpcksum to Checksum ;

: .udp ( -- )
  ."Port   " SourcePort . ." --> " DestPort . nl>
  ."Length " Length . nl>
  ."CkSum  " Checksum .x2 nl>
  check not ?abort"malformed UDP message"
  ."Payload\n" udp[] dumpn ;

addeventtype const IP4UDPRECV
: handleudp ( -- ) IP4UDPRECV curevent ! rootdispatch ;
UDPPROTO current registerproto
