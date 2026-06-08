needs io/stream io/wait drv/timer num/crc
unit com/xmodem

3000 const TIMEOUTMS
consts 1 SOH 4 EOT 6 ACK $15 NACK 128 PKTLEN 132 TOTLEN $1a PADCHR
create packet TOTLEN allot
TIMEOUTMS nullstream newwaitingstream const commline
EOT value curhdr
1 value pktnum

: xmodem$ ( commstream -- ) commline to tgtstream ;
: flushline ( -- ) nullstream commline spit ;
: waithdr ( -- f )
  ticks begin
    commline getc dup to curhdr EOF = while
    TIMEOUTMS over elapsedms? not while idle repeat
    0 else 1 then nip ;
: waithdr# ( -- ) waithdr not ?abort"timeout on packet wait" ;
: goodnum? packet c@+ pktnum = swap c@ pktnum inv $ff and = and ;
: goodcrc? packet 2+ PKTLEN crc16[] packet 130 + wbe@ = ;
: good? ( -- f ) goodnum? goodcrc? and ;

: SOH? curhdr SOH = ;
: SOH# SOH? not ?abort"SOH expected" ;
: beginrecv ( -- ) flushline begin 'C' commline putc waithdr until SOH# ;

: pktnum+ doto pktnum 1+ $ff and | ;
: ack pktnum+ ACK commline putc ;
: nack NACK commline putc ;
: pktin ( -- f ) packet TOTLEN commline read TOTLEN = ;
: recv1 ( -- )
  begin pktin if good? if ack exit then then
        nack waithdr# SOH# again ;
: EOT# curhdr EOT <> ?abort"EOT expected" ack ;

\ stream implementation
0 value recvcnt
0 value sentcnt
: reset EOT to curhdr 0 to recvcnt 0 to sentcnt 1 to pktnum flushline ;

: waitC begin idle commline getc 'C' = until reset ;
:~ ( n -- a u )
  packet 2+ swap PKTLEN mod tuck + ( off a )
  PKTLEN rot - ;
: pktread[] recvcnt ~ ;
: pktwrite[] sentcnt ~ ;
: _readbuf ( n stream -- ?a read-n )
  drop recvcnt PKTLEN mod if ( n )
    pktread[] rot min else ( n )
    recvcnt if waithdr# else beginrecv then
    SOH? if recv1 pktread[] rot min
    else drop EOT# reset 0 then then ( ?a read-n )
  doto recvcnt over + | ;
: wrappkt ( -- )
  pktnum dup inv swap packet c!+ c!+ ( a )
  PKTLEN crc16[] packet 130 + wbe! ( ) ;
: waitack ( -- ) begin waithdr# curhdr 'C' <> until ;
: sendpkt ( -- )
  wrappkt begin
    SOH commline putc
    packet TOTLEN commline write#
    waitack curhdr NACK <> until
  curhdr ACK <> ?abort"ACK expected"
  doto pktnum 1+ | ;
: flushpkt ( -- ) \ send incomplete pkt
  sentcnt PKTLEN mod if
    pktwrite[] PADCHR cfill sendpkt then ;
: _writebuf ( a n stream -- written-n )
  drop sentcnt not if waitC then
  pktwrite[] rot min ( src dst n )
  r! doto sentcnt over + | cmove r> ( written-n )
  sentcnt PKTLEN mod not if sendpkt then ;
' _readbuf ' _writebuf newstream const xmodem
: _close flushpkt sentcnt if EOT commline putc then reset ;
' _close xmodem to close
