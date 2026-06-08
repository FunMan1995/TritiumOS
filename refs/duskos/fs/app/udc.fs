needs lib/str lib/coop io/typeln com/net com/link com/ip4 com/udp
unit app/udc

1234 const LISTENPORT
createapplication UDCApp

$100 newtypebuf const tb
variable thelist \ a LL of 4b IP4 addr and 4b port, native endian

: inlist? ( addr port -- f )
  2>r thelist begin @ ?dup while ( ll V1=addr V2=port )
    dupbi 4+ @ V1 <> | 8+ @ V2 <> or while
    repeat drop 1 else 0 then 2rdrop ;

: addtolist ( addr port -- )
  ."Adding to list: " over .addr spc> dup . nl>
  thelist lladd swap , , ;

: .list thelist begin @ ?dup while dup 4+ @ .addr spc> dup 8+ @ . nl> repeat ;

: sendall ( -- )
  thelist begin @ ?dup while
    activelink waitsent resetdgram
    dup 4+ @ to DestAddr dup 8+ @ to DestPort
    com/udp wrap activelink sendframe repeat ;

create buf 'N' c, 6 allot0
: newcomer ( -- )
  SourceAddr buf 1+ be!
  SourcePort buf 5 + wbe!
  activelink newdgram LISTENPORT to SourcePort
  buf 7 udp! sendall ;

: .msg ( -- ) udp[] 1 consume[] rtype nl> ; 
: doM ( -- )
  .msg SourceAddr SourcePort inlist? not if
    SourceAddr SourcePort newcomer addtolist then ;
: doN ( -- )
  Data 1+ bi be@ | 4+ wbe@ 2dup inlist? if 2drop else addtolist then ;

:> DestPort LISTENPORT <> if exit then 
   Length not if exit then
   Data c@ case
     'M' = of doM endof
     'N' = of doN endof
     drop endcase ;
IP4UDPRECV UDCApp sethandler

:> evarg1 tb type1 if ( a u )
     nl> 1- \ trim ending LF
     "quit" oover oover s[]= if ."quitting!\n" 2drop stopcurrent exit then  
     activelink newdgram LISTENPORT to SourcePort ( a u )
     []>str "M" swap strcat c@+ udp!
     sendall then ;
KEYPRESS UDCApp sethandler
