needs lib/str io/stream com/link com/ip4 com/udp
unit com/slip

consts $c0 C0 $dc DC $db DB $dd DD

$1000 const MAXFRAMESZ
\ data of a single datagram being constructed. Escaped.
create recvbuf MAXFRAMESZ allot
0 value recvsz
\ leftover from after the closing C0 of the last readbuf. Not escaped.
create leftover MAXFRAMESZ allot
0 value leftoversz
nullstream value tgt

: recv$ 0 to recvsz ;

: findC0DB ( a u -- ?idx ?DC-or-DD f )
  C0 oover oover cidx if ( a u idx )
    >r DB rot> cidx if ( idx )
      dup r@ < if rdrop DD else drop r> DC then
      else r> DC then ( idx esc ) 1
  else
    DC rot> cidx dup if DD swap then then ;

: appendrecv ( a u -- )
  MAXFRAMESZ recvsz - over < ?abort"recv buffer overflow"
  >r recvbuf recvsz + r@ cmove
  r> doto recvsz + | ;

: unescape ( c -- c ) DD = if DC else C0 then ;
: ?trimC0 ( a u -- a u )
    begin over c@ C0 = over bool and while 1 consume[] repeat ;
: ?recv ( a u -- f )
  recvsz not if ?trimC0 then
  2dup findC0DB if ( a u idx esc )
    >r oover over appendrecv r>
    DD = if ( a u idx )
      2+ consume[] ( a u )
      over 1- c@ unescape c[] appendrecv ?recv
    else
      1+ consume[] ( a u )
      dup to leftoversz leftover swap cmove ( ) 1 then
  else ( a u ) appendrecv 0 then ;

: yieldrecv
  recvbuf dup to frameptr to payloadptr
  recvsz to payloadsz
  recv$ FTIP4 to frametype 1 ;

: :readframe ( link -- f )
  drop leftoversz if
    leftover leftoversz ?recv if yieldrecv exit then then
  MAXFRAMESZ tgt readbuf dup if
    ?recv if yieldrecv else 0 then then ;

: :beginframe ( link -- a u ) abort"TODO com/slip beginframe" ;
: :replytoframe ( type link -- a u ) abort"TODO com/slip replytoframe" ;
: :sendframe ( u link -- ) abort"TODO com/slip sendframe" ;
\  drop C0 tgt putc begin ( a u )
\    2dup findC0DB while ( a u idx esc )
\    >r dup if oover over tgt write# then r> ( a u idx esc )
\    DB tgt putc tgt putc 1+ consume[] repeat ( a u )
\  tgt write#
\  C0 tgt putc ;

' drop ' :sendframe ' :beginframe ' :replytoframe ' :readframe
newlink const sliplink

: slip$ ( st -- ) to tgt recv$ ;
