needs lib/psrs lib/ival lib/struct lib/diag drv/pci com/ether
unit drv/nic/rtl8169

1500 const MTU
MTU 8 align const BUFSZ

variable ptr
: ptr# ptr @ ?dup not ?abort"rtl8169 not configured" ;

ptr ivalmap {
  +$00 [uchar,6] IDR ;
  +$08 uint  MAR MARHI DTCCR DTCCRHI ;
  +$20 uint TNPDS TNPDSHI THPDS THPDSHI FLASH ;
  +$34 ushort ERBCR ;
  +$36 uchar ERSR CMD TPPOLL ;
  +$3c ushort IMR ISR ;
  +$40 uint TCR RCR TCTR MPC ;
  +$50 uchar 9346CR CONFIG0 CONFIG1 CONFIG2 CONFIG3 CONFIG4 CONFIG5 ;
  +$58 uint TIMERINT ;
  +$5c ushort MULINT ;
  +$60 uint PHYAR TBICSR0 ;
  +$68 ushort TBIANAR TBILPAR ;
  +$6c uchar PHYSTATUS ;
  +$da ushort RMS ;
  +$e0 ushort C+CR ;
  +$e4 uint RDSAR RDSARHI ;
  +$ec uchar ETTHR ;
}

8 const RXDESCCNT
8 alignheren BUFSZ RXDESCCNT * allot@ const RXBUF
RXBUF BUFSZ RXDESCCNT * 0 cfill

256 alignheren 4 4* RXDESCCNT * allot@ const RXDESC
256 alignheren 4 4* allot@ const TXDESC
8 alignheren BUFSZ allot@ const TXBUF

struct TxRxDesc {
  uint Flags1 Flags2 BufAddr BufAddrHi ;
}

: resetrxdesc ( idx -- A=a )
  dup 16 * RXDESC + >A ( idx )
  RXDESCCNT 1- = if $c0000000 else $80000000 then
  BUFSZ or A> to Flags1
  0 A> to Flags2 ;

: rxbuf ( idx -- ) BUFSZ * RXBUF + ;

: rx$
  RXDESCCNT 0 do
    i resetrxdesc
    i rxbuf A> to BufAddr
    0 A> to BufAddrHi
    loop ;

: tx$ $40000000 TXDESC A! to Flags1 0 A> to BufAddrHi TXBUF A> to BufAddr ;

: dumprx RXDESCCNT 0 do RXDESC i 16 * + BufAddr dump loop ;

: linkup? PHYSTATUS 2 and bool ;

: .rtl8169
  ."Base Addr " ptr# .x nl>
  ."Link up?  " linkup? . nl>
  ."CMD       " CMD .x1 nl>
  ."MAC       " IDR .mac nl>
  ."CONFIG0-5 " addrof CONFIG0 6 cspit[] nl>
  ."RXDESC    " RXDESC RXDESCCNT 0 do dup @ .x spc> 16 + loop drop nl>
  ."TXDESC    " TXDESC @ .x nl>
  ."TPPOLL    " TPPOLL .x nl> ;

create macaddr 6 allot0

: rtl8169pci$ ( -- )
  0 pcibus) busdescendants $10ec pcifiltervendor ( ... n )
  $8136 1 pcifilterdevices ( ... n )
  nfirst ?dup if pci0.bar2 $ff invand ptr ! then
  IDR macaddr 6 cmove
  0 to RDSARHI RXDESC to RDSAR
  0 to TNPDSHI TXDESC to TNPDS
  0 to THPDSHI TXDESC to THPDS
  rx$ tx$
  $c0 to 9346CR \ unlock config
  $e70f to RCR
  MTU to RMS
  $0c to CMD
  $00 to 9346CR ; \ normal mode

0 value prevdesc
: :rxbuf ( link -- ?a u-or-0 )
  drop doto prevdesc 0 | ?dup if 1- resetrxdesc then ( )
  RXDESC RXDESCCNT 0 do
    dup @ $80000000 and not if
      i 1+ to prevdesc
      i rxbuf break then
    16 + loop ( desc ?a )
  broke? if nip BUFSZ else drop 0 then ;

: :waitsent ( link -- )
  drop ticks begin
    10 over ?timeoutms":txbuf timeout"
    TXDESC Flags1 $80000000 and not until drop ;

: :txbuf ( link -- a u ) :waitsent TXBUF BUFSZ ;

: :sendframe ( link -- )
  drop frameptr TXDESC to BufAddr
  0 TXDESC to Flags2
  payloadsz ETHERHDRSZ + $f0000000 or TXDESC to Flags1
  $40 to TPPOLL ;

' :waitsent ' :sendframe ' :txbuf ' :rxbuf macaddr newetherlink const rtl8169
