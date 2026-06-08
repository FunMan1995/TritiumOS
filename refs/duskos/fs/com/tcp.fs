needs lib/str lib/ival lib/psrs lib/diag io/stream com/ip4
unit com/tcp

\ Packet section
6 const TCPPROTO
16 5 - 4* const MAXOPTSZ

\ Points to byte following the last option byte. Used for opts writing only.
\ For reading, Options+DataOffset gives our range.
0 value optptr 

ip4payload ivalmap {
  beshort SourcePort DestPort ;
  beint SeqNum AckNum ;
  beshort Flags Window Checksum UrgentPtr ;
  [void,0] Options ;
}

: DataOffset Flags 12 rshift 4* ;
: Flags! doto Flags $fff invand or | ;
: opts[ ( -- ) Options to optptr ;
: ]opts ( -- )
  optptr Options - dup MAXOPTSZ >= if drop 0 else ( optlen )
    dup 4 mod if optptr 4 0 cfill then 4 /+ then ( optoff )
  5 + 12 lshift doto Flags $fff and or | ;

: tcp[] ( -- a u ) ip4payload @ DataLength DataOffset consume[] ;
\ Can only be called after optsdone has been called.
: tcp[]! ( a u -- ) DataOffset + DataLength! tcp[] cmove ;

consts 1 FIN 2 SYN 4 RST 8 PSH $10 ACK $20 URG
: FIN? Flags FIN and ;
: SYN? Flags SYN and bool ;
: RST? Flags RST and bool ;
: PSH? Flags PSH and bool ;
: ACK? Flags ACK and bool ;
: URG? Flags URG and bool ;

: tcpcksum ( -- n )
  0 addrof SourceAddr 8 ck[] ( sum )
  Protocol ck1 DataLength ck1
  doto Checksum 0 | >r ip4payload @ DataLength ck[] ckinv r> to Checksum ;

: wrap ( -- )
  TCPPROTO to Protocol
  com/ip4 wrap
  tcpcksum to Checksum ;

: .opt ( a n -- a+ )
  2- max0 case
    1 = of c@+ .x1 endof
    2 = of dup wbe@ .x2 2+ endof
    4 = of dup be@ .x 4+ endof
    2dup cspit[] + endcase ;

: addopt ( len code -- a ) optptr c!+ dipdup c! doto optptr tuck + | 2+ ;

: .opts ( -- )
  Options begin dup tcp[] drop < while c@+ ?dup while ( a c )
    dup 1 = if drop else
      . spc> c@+ .opt ." | " then
    repeat then ( a ) drop ;

: .tcp ( -- )
  ."Port   " SourcePort . ." --> " DestPort . nl>
  ."Seq    " SeqNum .x nl>
  ."Ack    " AckNum .x nl>
  ."Flags  " Flags .x2 nl>
  ."Window " Window .x2 nl>
  ."CkSum  " Checksum .x2 spc> tcpcksum .x2 nl>
  ."Urgent " UrgentPtr .x2 nl>
  ."Opts   " .opts nl>
  tcp[] dumpn ;

\ TCB section

536 const DEFAULTMSS
$400 const MAXWINDOWSZ
400 const MAXINACKMS
4000 const MAXOUTACKMS
\ These values are super low. If you run a server, beef these up
8 const BUFCNT
32 const TCBCNT

create bufs BUFCNT MAXWINDOWSZ * allot
: buf@ ( idx -- a ) 1- max0 MAXWINDOWSZ * bufs + ;

0 value curtcb
addrof curtcb ivalmap {
  uint TSourceAddr TDestAddr ;
  ushort TSourcePort TDestPort ;
  uint MSS ;
  uint INorig INbegin INend ;
  uint OUTorig OUTnext OUTwindowsz ;
  uint INbuf OUTbuf ;
  *Stream INstream OUTstream ;
}
offsetof OUTstream 4+ const TCBSZ

create tcbs( TCBCNT TCBSZ * allot0
here const )tcbs

: firsttcb tcbs( to curtcb ;
: nexttcb ( -- f ) curtcb TCBSZ + dup )tcbs < if to curtcb 1 else drop 0 then ;
: findtcb ( -- f )
  firsttcb begin nexttcb while 
    TSourceAddr SourceAddr = TDestAddr DestAddr = and
    TSourcePort SourcePort = and TDestPort DestPort = and not while repeat
    1 else 0 then ;
: freetcb ( -- f )
  firsttcb begin nexttcb while TSourceAddr while repeat 1 else 0 then ;

: acceptcon ( -- )
  SourceAddr to TSourceAddr
  DestAddr to TDestAddr
  SourcePort to TSourcePort
  DestPort to TDestPort
  com/ip4 reply
  SourcePort doto DestPort swap | to SourcePort
  SeqNum 1+ to AckNum
  0 to SeqNum
  SYN ACK or Flags!
  opts[ 1460 4 2 addopt wbe! ]opts
  0 0 tcp[]!
  wrap ?log curlink sendframe ;

: handletcp ( -- )
  SYN? ACK? not and if
    freetcb not ?abort"TODO: out of TCBs"
    acceptcon then ;
TCPPROTO current registerproto