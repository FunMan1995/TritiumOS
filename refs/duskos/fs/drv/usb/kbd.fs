needs hal/muldiv io/kbd drv/usb drv/usb/hid
unit drv/usb/kbd

\ Queue item structure is 1b event type and 1b nkc
\ Queue size is 2b per event, but 8 events can spawn from mod bits

8 const KBDBUFSZ
16 const EVQUEUESZ
8 const IdleVal \ 8x4ms SetIdle on keyboards

extends Keyboard struct USBKeyboard {
  uint pollep ;
  uint ridx widx ; \ ev queue index
  [uchar,KBDBUFSZ] prevkeys ;
  [ushort,EVQUEUESZ] evqueue ;
}

: addev ( nkc etype kbd -- )
  A! widx dup 1+ EVQUEUESZ mod A> to widx ( nkc etype idx )
  2* A> evqueue + c!+ c! ;

$010103 const KBDCSP
: kbdiep ( dev -- iep )
  >r Tintr Ein KBDCSP r> findep ?dup not ?abort"no kbd iface" ;
: findkbd# KBDCSP findcsp ?dup not ?abort"no boot proto USB kbd attached" ;

create buf KBDBUFSZ allot
: poll ( kbd -- a u )
  KBDBUFSZ buf rot pollep epread buf swap ;

\ Mapping from USB code to NKC
create sctab map< c,
0   0   0   0   'A' 'B' 'C' 'D' 'E' 'F' 'G' 'H' 'I' 'J' 'K' 'L' \
'M' 'N' 'O' 'P' 'Q' 'R' 'S' 'T' 'U' 'V' 'W' 'X' 'Y' 'Z' '1' '2' \
'3' '4' '5' '6' '7' '8' '9' '0' $0d $1b $08 $09 $20 '-' '=' '[' \
']' '\' '|' ';' ''' '`' ',' '.' '/' $ae $81 $82 $83 $84 $85 $86 \
$87 $88 $89 $8a $8b $8c $a0 $a1 $a2 $a3 $a5 $a7 $a4 $a6 $a8 $aa \
$a9 $ac $ab $b0 $b2 $b3 $b4 $b5 $b1 $c1 $c2 $c3 $c4 $c5 $c6 $c7 \
$c8 $c9 $c0 $ca '<' 0   0   0   $8d $8e $8f $90 $91 $92 $93 $94 \
$95 $96 $97 $98 0   0   0   0   0   0   0   0   0   0   0   0
$80 allot0

\ NKC corresponding to every bit in the USB "mod" byte
create modbits map< c, $d2 $d0 $d4 $d6 $d3 $d1 $d5 $d7

: chkmods ( omods mods kbd -- )
  >r modbits >r begin ( omods mods ) \ V1=kbd V2=modbits
    2dup <> while
    over 1 and if dup 1 and not if
      V2 c@ RELEASE V1 addev then
      else dup 1 and if
        V2 c@ PRESS V1 addev then then
    2/ swap 2/ swap doto V2 1+ | repeat ( omods mods )
  2drop 2rdrop ;
: haskey? ( val keys -- f ) 2 + 6 cidx dup if nip then ;
: chkkeys ( etype okeys keys kbd -- )
  >r rot >r 1+ 0 6 do ( okeys keys ) \ V1=kbd V2=etype
    tuck i + c@ ?dup if ( keys okeys val )
    tuck over haskey? if nip else ( keys val okeys )
      swap sctab + c@ ?dup if V2 V1 addev then then then ( keys okeys )
    swap 1 -loop
  2drop 2rdrop ;

: pollcmp ( kbd -- )
  r! poll ( a u ) ?dup not if drop rdrop exit then
  over c@ r@ prevkeys c@ swap r@ chkmods ( a u )
  over PRESS r@ prevkeys rot r@ chkkeys ( a u )
  over RELEASE swap r@ prevkeys r@ chkkeys ( a u )
  r> prevkeys swap cmove ;

: _?event ( kbd -- ?nkc event-type )
  A! widx A> ridx <> ?dup not if ( A=kbd )
    A> dup pollcmp A! widx A> ridx <> then ( f A=kbd )
  if
    A> ridx dup 1+ EVQUEUESZ mod A> to ridx ( idx )
    2* A> evqueue + c@+ swap c@ swap
    else 0 then ;

: newusbkbd ( dev -- kbd )
  r! kbdiep dup ifaceid r@ Dev.ep0 bootproto! ( iep )
  IdleVal over ifaceid r@ Dev.ep0 setidle! ( iep )
  r> newdevep ( pollep )
  ['] _?event newkbd swap , 0 , 0 ,
  KBDBUFSZ EVQUEUESZ 2* + allot0 ;
