needs tests/harness lib/coop drv/nic/loop com/udp com/net
testbegin
\ We wrap all our code in compiled words because there can't be any idling in
\ between (which can happen in interpret mode).

\test send a UDP packet

:~ ( -- )
  loopback newdgram
  LOCALHOST to DestAddr
  1234 to SourcePort
  2345 to DestPort
  "foobar" c@+ udp!
  com/udp wrap
  loopback sendframe
  loopback ['] incoming expectevent
  evtype IP4UDPRECV #eq
  SourcePort 1234 #eq
  DestPort 2345 #eq
  "foobar" udp[] #s[]= ; ~

testend
