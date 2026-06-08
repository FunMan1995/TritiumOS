needs lib/struct lib/ival lib/diag com/link com/ip4
unit com/ether

$0e const ETHERHDRSZ
create BROADCAST -1 , -1 w,

extends Link struct EtherLink {
  *uchar macptr ;
  xt rxbuf txbuf ;
}

addrof frameptr ivalmap {
  [uchar,6] dstmac srcmac ;
  beshort ethertype ;
}

: .mac ( a -- ) 6 cspit[] ;
: macmove ( src dst -- ) 6 cmove ;
: .ether
  ."src:  " srcmac .mac nl>
  ."dst:  " dstmac .mac nl>
  ."type: " ethertype .x2 nl> ;

create ethertypes map< , $0800 $0806 0
: ethertype>ft ( -- type ) ethertype ethertypes 2 idx not if FTUNKNOWN then ;
: ft>ethertype ( type -- )
  FTUNKNOWN min 4* ethertypes + @
  ?dup not ?abort"com/ether doesn't support this frame type"
  to ethertype ;

: >buf ( a u -- )
  over to frameptr ETHERHDRSZ consume[] to payloadsz to payloadptr ;
: :readframe ( link -- f )
  r! rxbuf dup if >buf r@ to curlink ethertype>ft to frametype 1 then rdrop ;
: :beginframe ( link -- )
  dup to curlink txbuf >buf frametype ft>ethertype ( )
  curlink macptr srcmac macmove ;
: :replytoframe ( link -- )
  srcmac dstmac macmove
  macptr srcmac macmove ;

: :sendframewrapper abort"com/arp needs to be loaded!" ;
: newetherlink ( waitsentxt sendframext txbufxt rxbufrx macptr -- link )
  >r 2>r ( waitsentxt sendframext )
  ['] :sendframewrapper bind>
  ['] :replytoframe ['] :beginframe ['] :readframe
  newlink 2r> r> , , , ;
