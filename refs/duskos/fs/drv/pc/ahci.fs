needs lib/struct lib/ival io/blk lib/bit drv/timer drv/pci drv/pc/sata
unit drv/pc/ahci

struct HBAPort {
  uint clb clbu fb fbu is ie cmd _pad ;
  uint tfd sig ssts sctl serr sact ci sntf fbs ;
  +$70 [uint,4] vendor ;
}
$20 4* const HBAPORTSZ

: waitcmd begin dup cmd $8000 and ( CR ) not until drop ;
: startcmd
  dup waitcmd
  \ ST can only be set *after* FRE
  A! doto cmd $10 or | ( FRE )
  A> doto cmd 1 or | ( ST ) ;
: stopcmd doto cmd $ffffffee and | ( ~FRE+~ST ) ;
: ci!
  1 over to ci ticks begin
    500 over ?timeoutms"HBAPort CI timeout"
    over ci not until 2drop ;
: sigstr ( self -- str ) sig case
  $101 = of "ATA" endof
  $eb140101 = of "ATAPI" endof
  $c33c0101 = of "bridge" endof
  $96690101 = of "multiplier" endof
  drop "unknown" endcase ;

\ RsvdFIS: buffer for receiving FIS from AHCI controller
struct RsvdFIS {
  [uchar,$20] dma pio ;
  [uchar,$18] d2h ;
  [void,0] sdb ;
}
\ The rest is unknown/reserved
$100 const RSVDFISSZ
: newrsvdfis ( -- rsvdfis ) $100 alignheren here RSVDFISSZ allot0 ;

struct PRDTable {
  uint addr ; \ address of data, 2-bit aligned
  uint addrhi _pad ;
  ushort bytecount ;  \ 0-indexed! 0=1 byte 1=2 bytes etc. must be odd.
  uchar bytecounthi ; \ bytecount's maximum is 4 MB
  uchar flags ;       \ b7=Interrupt on Completion
}
PRDTable typesz const PRDTABLESZ

: setPRD ( a u prdtable -- )
  dup PRDTABLESZ 0 cfill ( a u prdtable )
  >A 1- \ 0-indexed bytecount!
  A> to bytecount A> to addr ;

struct CmdHeader {
  ushort flags ;
  ushort prdtl ; \ Physical Region Descriptor Table Length
  uint prdbc   ; \ PRD byte count
  uint cmdtba  ; \ address of a CmdTable, 128-bit aligned
  uint cmdtbau _pad _pad _pad _pad ;
}

: newcmdheader ( -- cmdlist )
  $400 alignheren here CmdHeader typesz $20 * allot0 ;
: cfl! ( n self -- ) doto flags $fff0 and or | ;
: write! ( self -- ) doto flags $40 or | ;
: read! ( self -- ) doto flags $40 invand | ;

struct CmdTable {
  [uchar,$40] fis atapicmd ;
  [void,0] prdt ; \ variable length, list of PRDTable
}
$80 const CMDTABLESZ

: newcmdtable ( cmdhdr prdt-count -- cmdt )
  $80 alignheren here swap PRDTABLESZ *
  CMDTABLESZ + allot0 ( cmdhdr cmdt )
  FISH2DTYPE over fis c! ( cmdhdr cmdt )
  2dup swap to cmdtba ( cmdhdr cmdt )
  over 1 swap to prdtl ( cmdhdr cmdt )
  FISH2D typesz 4/ rot cfl! ( cmdt ) ;

variable hbamem
hbamem ivalmap {
  uint CAP GHC IS PI VS CCC_CTL CCC_PORTS EM_LOC EM_CTL CAP2 ;
  +$38 uint BOHC ;
  +$100 [void,0] PORTS ;
}

: hbaport ( n -- 'port ) HBAPORTSZ * PORTS + ;

extends Blk struct AHCIDrive {
  uint portno ;
  *HBAPort port ;
  *CmdHeader cmds ;
  *RsvdFIS rsvdfis ;
}

$200 const SECSZ

: cmd0 cmds cmdtba ;
: fis0 cmd0 fis ;

: _sec@ ( sec dst self -- ) >r
  r@ cmds read! ( sec dst )
  SECSZ r@ cmd0 prdt setPRD ( sec )
  r@ fis0 dma@ ( )
  r> port ci! ;

: _sec! ( sec src self -- ) >r
  r@ cmds write! ( sec src )
  SECSZ r@ cmd0 prdt setPRD ( sec )
  r@ fis0 dma! ( )
  r> port ci! ;

\ TODO: check geometry and set seccnt
: newahcidrive ( portno -- self )
  newrsvdfis newcmdheader ( portno rsvdfis cmds )
  ['] _sec@ ['] _sec! SECSZ -1 newblk >r
  rot dup ( portno ) , hbaport , ( cmds ) , ( rsvdfis ) ,
  r@ port stopcmd
  r@ cmds r@ port to clb 0 r@ port to clbu
  r@ rsvdfis r@ port to fb 0 r@ port to fbu
  \ We only need a single command table, which itself only needs a single PRDT
  r@ cmds 1 newcmdtable drop
  r> ;
: enable port startcmd ;
: identify ( self -- status )
  dup fis0 $ec ( IDENTIFY ) over to command ( fis )
  $80 ( enable C ) swap to fisflags ( self )
  dup port ci!
  dup rsvdfis pio status ;


\ Search PCI bus for a AHCI controller and if found, bind "hba" to its BAR5
\ value.
: ahci$
  0 pcibus) buschildren 1 6 -1 pcifilter1 dup not ?abort"no AHCI device"
  pci0.bar5 hbamem ! ;

\ Do we have a AHCI controller?
: ahci? hbamem @ bool ;

: .ahci
  ahci? not if abort"No AHCI controller" then
  ."HBA addr: " hbamem @ .x nl>
  ."GHC  " GHC .x nl>
  ."CAP  " CAP .x nl>
  ."CAP2 " CAP2 .x nl>
  ."BOHC " BOHC .x nl>
  ."PI   " PI .x nl>
  PI 0 $20 0 do ( pi id )
    2dup bit? if
      dup hbaport dup ssts $0f0f and $0103 = if
        ( pi id port ) \ detected and active
        ."Device " over . ." type: " dup sigstr stype nl>
        ."  CLB  " dup clbu .x spc> dup clb .x nl>
        ."  FB   " dup fbu .x spc> dup fb .x nl>
        ."  CMD  " dup cmd .x nl>
        ."  CI   " dup ci .x nl>
        ."  SACT " dup sact .x nl>
        ."  SSTS " dup ssts .x nl>
        ."  TFD  " dup tfd .x nl>
        ."  SERR " serr .x nl>
      else drop then then
    ( pi id ) 1+ loop ;
