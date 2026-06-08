needs asm/label
unit xcomp/tools

variable xbindict

: tgtallot ( tgtpc -- ) pc - dup 0< if abort"tgtallot overflow" then allot0 ;
: xoffset binstart org - ;
: xcode xbindict word entry ;
: xlatest xbindict @ xoffset + ;
: ximm xbindict @ 1- dup c@ $80 or swap c! ;

\ Usage: xwordlbl foo call, "foo" being a word name in xbindict
: xwordlbl ( "name" -- pc )
  word xbindict find ?dup not if (wnf) then xoffset + ;

\ Traverse xdict and change all its prev field values so that they are related
\ to current org+binstart. Once this is done, the xdict can't be traversed
\ anymore and becomes opaque binary contents.
:~ ( xdict w -- ) >r
  0 swap @! begin ( entry )
    dup @ ( entry next )
    ?dup while ( entry next )
    dup xoffset + ( entry old new ) rot r@ execute ( entry ) repeat
  drop rdrop ;
: orgifydict ( xdict -- ) ['] le! ~ ;
: orgifydictbe ( xdict -- ) ['] be! ~ ;

0 value xcompaddr
0 value xcomplen
: xcompbegin ( -- ) here# dup to org to xcompaddr ;
: xcompend ( -- ) here xcompaddr - to xcomplen resetasm ;
: xcomp[] xcompaddr xcomplen ;

$1000 const MAXKERNELLEN
0 value kernel
0 value kernellen
: kernelbegin
  0 xbindict ! here# dup to kernel dup to org MAXKERNELLEN 0 4/ fill ;
:~ here kernel - to kernellen ;
: kernelend ~ xbindict orgifydict resetasm ;
: kernelendbe ~ xbindict orgifydictbe resetasm ;
: kernel[] kernel kernellen ;

\ SYSVARS offsets
consts
  $04 oCOMPILING $08 oCURALLOC     $0c oSYSDICT \
  $10 oFINDSTR   $14 oRCNT         $18 oFINDCOMPILER $1c oSYSALLOC \
  $28 oINPTR \
  $48 oPSORIGIN $4c oPSSZ \
  $50 oRSORIGIN
