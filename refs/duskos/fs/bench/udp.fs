needs lib/coop com/net com/ip4 com/udp
unit bench/udp

createapplication UDPDump
:> .dgram .udp ;
IP4UDPRECV UDPDump sethandler
UDPDump newcontext const udpdumpbgctx
: udpdumpbg udpdumpbgctx launchcontext ;

UDPDump cloneapplication UDPDumpApp
' stopcurrent KEYPRESS UDPDumpApp sethandler

UDPDumpApp newcontext const udpdumpctx
: udpdump udpdumpctx launchcontext ;

createapplication UDPEchoApp
:> udp[] rtype nl>
   com/udp reply com/udp wrap curlink sendframe ;
IP4UDPRECV UDPEchoApp sethandler
' stopcurrent KEYPRESS UDPEchoApp sethandler

UDPEchoApp newcontext const udpechoctx
: udpecho udpechoctx launchcontext ;

: sendudp[] ( a u srcport dstport dstaddr link -- )
  r! newdgram to DestAddr to DestPort to SourcePort udp! ( V1=link )
  com/udp wrap r> sendframe ;