needs lib/coop com/link com/ip4 com/arp com/udp com/tcp
unit com/net

\ handler sig: ( -- )
create handlers ' handledgram , ' handlearp , ' noop ,

: ft@ frametype FTUNKNOWN min ;
: incoming ( link -- )
  begin dup readframe while ( link )
    deframe ?log ft@ 4* handlers + @ execute repeat drop ;

: .dgram+
  .dgram Protocol case
    UDPPROTO = of .udp endof
    TCPPROTO = of .tcp endof
  endcase ;

: .unknown ."Unknown frame type\n" ;
create handlers ' .dgram+ , ' .arp , ' .unknown ,
: .frame ft@ 4* handlers + @ execute ;
: .nextf nextf if .frame else ."No more logged frames\n" then ;

4 const MAXACTIVELINKS
create activelinks MAXACTIVELINKS 4* allot0
0 value activelink

: makeactive ( link -- )
  dup to activelink
  0 activelinks MAXACTIVELINKS idx not ?abort"Too many active links!"
  4* activelinks + ! ;

: makeinactive ( link -- )
  activelinks MAXACTIVELINKS idx if 4* activelinks + 0 swap ! then ;

: activeincoming ( -- )
  activelinks MAXACTIVELINKS 0 do @+ ?dup if incoming then loop drop ;

createapplication NetIncoming
' activeincoming IDLE NetIncoming sethandler