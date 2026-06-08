needs lib/str lib/ival lib/psrs mem/kv com/link
unit com/ip4

64 const DEFAULTTTL
$7f000001 const LOCALHOST

LOCALHOST value selfaddr
$ffffff00 value subnetmask
0 value routeraddr

variable ip4payload
addrof payloadptr ivalmap {
  uchar VersionIHL TOS ;
  beshort TL Identification Fragmentation ;
  uchar TTL Protocol ;
  beshort Checksum ;
  beint SourceAddr DestAddr ;
}

: Version VersionIHL 16/ ;
: HdrLength VersionIHL $f and 4* ;
: Data payloadptr HdrLength + ;
: DataLength ( -- n ) TL HdrLength - ;
: DataLength! ( n -- ) HdrLength + dup to TL to payloadsz ;

:~ 24 rshift . ;
: .addr ( n -- ) 3 0 do dup ~ ."." 256* loop ~ ;

: 16>> 16 rshift ;
: lo16 $ffff and ;
: ck1 ( sum n -- sum ) + dup 16>> + lo16 ;
: ck[] ( sum a u -- sum )
  r! 2/ 0 do tuck wbe@ ck1 swap 2+ loop ( sum a+ V1=u )
  r> 1 and if c@ 256* ck1 else drop then ;

: ckinv ( sum -- sum ) inv ?dup not if $ffff then lo16 ;

: dgramcksum ( -- n )
  doto Checksum 0 | 0 payloadptr 20 ck[] ckinv swap to Checksum ;

:realias deframe Data ip4payload ! doto payloadsz TL min | ;

: >dgram ( -- f )
  Version 4 = ( f )
  dup if dgramcksum Checksum = and then ;
: >dgram# >dgram not ?abort"invalid datagram" ;

: resetdgram ( -- ) DEFAULTTTL to TTL ;

: newdgram ( link -- )
  FTIP4 to frametype beginframe
  payloadptr 5 0 fill
  $45 to VersionIHL
  resetdgram
  selfaddr to SourceAddr
  Data ip4payload ! ;

: reply ( -- )
  curlink replytoframe
  resetdgram
  SourceAddr doto DestAddr swap | to SourceAddr ;

: wrap ( -- ) dgramcksum to Checksum ;

: broadcast? ( -- f ) DestAddr $ff and $ff = ;
: tome? ( -- f ) DestAddr selfaddr = ;

: .ip4 ( -- )
  ."Self:   " selfaddr .addr nl>
  ."Subnet: " subnetmask .addr nl>
  ."Router: " routeraddr .addr nl> ;

: .dgram ( -- )
  ."Ver.   " Version . nl>
  ."Addr   " SourceAddr .addr
  ." --> " DestAddr .addr tome? if ." (me!)" then nl>
  ."TOS    " TOS . nl>
  ."TL     " TL . nl>
  ."ID     " Identification .x2 nl>
  ."Frag.  " Fragmentation .x2 nl>
  ."TTL    " TTL . nl>
  ."Proto. " Protocol . nl>
  ."Cksum  " Checksum .x2 nl>
  ."Data   " Data .x spc> DataLength . nl> ;

2 const NUMPROTO
create tbl 0 ' noop 2 NUMPROTO nsame 2/ kvtbl,

: registerproto ( id xt -- )
  over tbl 0 rot kvreplace# ( id xt )
  tbl rot> kv!# ;

: handle? ( -- f ) >dgram if tome? broadcast? or else 1 then ;
: handledgram ( -- )
  handle? not if exit then
  tbl Protocol ?kvexec drop ;
