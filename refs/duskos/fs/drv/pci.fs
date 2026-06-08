needs lib/bit lib/str lib/psrs lib/fmt mem/dict text/ts
unit drv/pci

: err abort"PCI not configured" ;
alias err pci@
alias err pci!
: pci$ ['] pci! realias ['] pci@ realias ;

: pcibus) ( busno -- addr ) 16 lshift $80000000 or ;
: pcislot) ( addr slotno -- addr ) 11 lshift or ;
: pcifunc) ( addr funcno -- addr ) 8 lshift or ;
: pcireg) ( addr off -- addr ) $fc and or ;

0 value _currentlist
0 value fieldsoff
: fields$ 0 to fieldsoff ;
: fallot ( n -- ) doto fieldsoff + | ;
: fieldsoff+ ( n -- off ) fieldsoff swap fallot ;
: fieldsoff+# ( -- off ) 4 fieldsoff+ dup 4 align# ;

\ Field struct:
\ 4b offset
\ 4b shift
\ 4b mask
\ Xb description string

: _ ( field -- ) _currentlist ! doto _currentlist 4+ | ;
:~ ( addr 'field -- n ) @ pcireg) pci@ ;
: pcifield ( "desc" -- )
  word NEXTWORD ! here# fieldsoff+# , 0 , -1 , wordorquote s,
  dup ['] ~ bind _ ;

: pcifield@ ( addr 'field -- n )
  @+ rot swap pcireg) >r ( 'field+4 ) \ V1=addr
  @+ swap @ r> pci@ ( shift mask n ) and swap rshift ;
: pcifieldp ( offset 'mask "desc" ) \ "p" for partial
  word NEXTWORD ! here# >r
  swap ( offset ) , @+ ( shift ) , @ ( mask ) , wordorquote s,
  r@ ['] pcifield@ bind r> _ ;

create _masks 0 , $0000ffff , 16 , $ffff0000 ,
: pcifieldw 2 fieldsoff+ 2 bitsplit dup 2 align# 4* _masks + pcifieldp ;
create _masks 0 , $000000ff , 8 , $0000ff00 , 16 , $00ff0000 , 24 , $ff000000 ,
: pcifieldb 1 fieldsoff+ 2 bitsplit 8* _masks + pcifieldp ;

\ Common header
12 const COMMONFIELDCNT
create commonfieldlist COMMONFIELDCNT 4* allot
commonfieldlist to _currentlist

fields$
pcifieldw  pci.vendor      "Vendor"
pcifieldw  pci.device      "Device"
pcifieldw  pci.command     "Command"
pcifieldw  pci.status      "Status"
pcifieldb  pci.revision    "Revision"
pcifieldb  pci.progIF      "progIF"
pcifieldb  pci.subclass    "Subclass"
pcifieldb  pci.class       "Class"
pcifieldb  pci.cachelinesz "Cache Line Size"
pcifieldb  pci.lattimer    "Latency Timer"
pcifieldb  pci.hdrtype     "Type"
pcifieldb  pci.bist        "BIST"

:~ 4* commonfieldlist + @ , ;
create summaryfields 0 ~ 1 ~ 7 ~ 6 ~ 10 ~ 5 ~

: pci.cs 8+ pci@ 16 rshift ;

\ Type 0 header
15 const TYPE0FIELDCNT
create type0fieldlist TYPE0FIELDCNT 4* allot
type0fieldlist to _currentlist

$10 to fieldsoff
pcifield   pci0.bar0            "BAR0"
pcifield   pci0.bar1            "BAR1"
pcifield   pci0.bar2            "BAR2"
pcifield   pci0.bar3            "BAR3"
pcifield   pci0.bar4            "BAR4"
pcifield   pci0.bar5            "BAR5"
pcifield   pci0.cisptr          "Cardbus CIS ptr"
pcifieldw  pci0.subsystemvendor "Subsystem Vendor"
pcifieldw  pci0.subsystem       "Subsystem ID"
pcifield   pci0.expansionaddr   "Expansion Addr"
pcifieldb  pci0.capabiliesptr   "Capabilities ptr"
7 fallot
pcifieldb  pci0.interruptline   "Interrupt Line"
pcifieldb  pci0.interruptpin    "Interrupt PIN"
pcifieldb  pci0.mingrant        "Min grant"
pcifieldb  pci0.maxlatency      "Max latency"

\ Type 1 header
22 const TYPE1FIELDCNT
create type1fieldlist TYPE1FIELDCNT 4* allot
type1fieldlist to _currentlist

$10 to fieldsoff
pcifield   pci1.bar0            "BAR0"
pcifield   pci1.bar1            "BAR1"
pcifieldb  pci1.pribus          "Primary Bus"
pcifieldb  pci1.secbus          "Secondary Bus"
pcifieldb  pci1.subbus          "Subordinate Bus"
pcifieldb  pci1.seclattimer     "Sec Latency timer"
pcifieldb  pci1.iobase          "I/O base"
pcifieldb  pci1.iolimit         "I/O limit"
pcifieldw  pci1.secstatus       "Secondary status"
pcifieldw  pci1.membase         "Memory base"
pcifieldw  pci1.memlimit        "Memory limit"
pcifieldw  pci1.prefmembase     "Prefetch mbase"
pcifieldw  pci1.prefmemlimit    "Prefetch mlimit"
pcifield   pci1.prefhmem        "Prefetch high mem"
pcifield   pci1.preflmem        "Prefetch low mem"
pcifieldw  pci1.iobaseh         "I/O base (high)"
pcifieldw  pci1.iolimith        "I/O limit (high)"
pcifieldb  pci1.capabiliesptr   "Capabilities ptr"
3 fallot
pcifield   pci1.expansionbase   "Exp ROM base"
pcifieldb  pci1.interruptline   "Interrupt Line"
pcifieldb  pci1.interruptpin    "Interrupt PIN"
pcifieldw  pci1.bridgectl       "Bridge Control"

: _getdesc ( 'field -- str ) 12 + ;

: pciexists? ( addr -- f ) pci@ -1 <> ;
: pciexists# ( addr -- )
  dup pciexists? not if dup .x abort" invalid PCI address" else drop then ;

: ?bridge ( addr -- ?addr f )
  dup pci.cs $0604 = if pci1.secbus pcibus) 1 else drop 0 then ;

: slotchildren ( addr -- ... n )
  dup pciexists? if dup pci.hdrtype $80 and if ( addr )
    0 >r 8 0 do dup pciexists? if doto V1 1+ | dup then $100 + loop drop r>
    else 1 then else drop 0 then ;

: buschildren ( addr -- ... n )
  >r 0 >r $20 0 do V1 slotchildren doto V2 + | doto V1 $800 + | loop r> rdrop ;

: busdescendants ( addr -- ... n )
  buschildren r! ps[] 0 do ( ... a V1=n )
    r! @ ?bridge if buschildren doto V1 + | then r> 4+ loop ( ... a )
  drop r> ;

: IF= ( n expected -- f ) dup 1+ if = else 2drop 1 then ;
: pcifilter ( ... n cl subcl if -- ... n )
  >r swap 256* or >r 0 >r begin ( ... n V1=progIF V2=cs V3=cnt )
    ?dup while
    over bi pci.cs V2 = | pci.progIF V1 IF= and
    if doto V3 1+ | rollk> else nip then 1-
  repeat r> 2rdrop ;

: pcifilter1 pcifilter nfirst ;

: pcifiltervendor ( ... n vendor -- ... n )
  >r 0 >r begin ( ... n V1=vendor V2=cnt )
    ?dup while
    over pci.vendor V1 = if
      doto V2 1+ | rollk> else nip then
    1- repeat r> rdrop ;

16 const MAXDEVCNT
create devs MAXDEVCNT 4* allot

: pcifilterdevices ( ... n ... n -- ... n )
  dup MAXDEVCNT > ?abort"too many devices in pcifilterdevices"
  r! ps[] devs swap move r@ ndrop 0 >r begin ( ... n V1=devcnt V2=cnt )
    ?dup while
    over pci.device devs V1 idx if
      drop doto V2 1+ | rollk> else nip then
    1- repeat r> rdrop ;

: .pciaddr ( addr -- )
  tri 8 rshift 7 and | 11 rshift $1f and | 16 rshift $ff and .f"%b:%b.%b " ;

\ Print current PCI device in a one line summary
: .pciln ( addr -- )
  r! pciexists# V1 .pciaddr summaryfields 6 0 do ( 'field ) \ V1=addr
    @+ dup _getdesc stype spc> V1 swap pcifield@ .x? spc> loop drop rdrop nl> ;

: .pcibus ( addr -- ) buschildren 0 do .pciln loop ;
: .pciall ( -- ) 0 pcibus) busdescendants 0 do .pciln loop ;

: .pcifield ( addr 'field -- )
  ts[ rot> dup _getdesc stype ( oldemit addr 'field )
  18 tsgo pcifield@ .x? nl> ]ts ;

: .pcifields ( addr fieldlist cnt -- )
  rot >r nl> 0 do @+ V1 swap .pcifield loop drop rdrop ;

: .pci ( addr -- )
  dup pciexists# commonfieldlist COMMONFIELDCNT .pcifields ;

: .pci0 ( addr -- ) dup pciexists# type0fieldlist TYPE0FIELDCNT .pcifields ;
: .pci1 ( addr -- ) dup pciexists# type1fieldlist TYPE1FIELDCNT .pcifields ;
