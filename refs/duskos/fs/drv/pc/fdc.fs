needs io/blk drv/timer drv/pc/ioport drv/pc/cmos drv/pc/pic asm/x86
unit drv/pc/fdc

variable initialized
: err 0 initialized ! abort ;

\ hardcode for 1.44M disks.
2 const FDHEADS
80 const FDCYLCNT
18 const FDSECPERTRK
FDSECPERTRK FDHEADS * const FDSECPERCYL

\ Reminder: sectors are 1-based!
: lba>chs ( lba -- cyl head sec )
  FDSECPERCYL /mod ( sec*hd cyl )
  swap FDSECPERTRK /mod ( cyl sec head ) swap 1+ ;

$200 const SECSZ
$3f0 ioportb sra  \ R
$3f1 ioportb srb  \ R
$3f2 ioportb dor  \ RW
$3f3 ioportb tdr  \ RW
$3f4 ioportb msr  \ R RMQ DIO NONDMA CMDBSY DBSY3 DBSY2 DBSY1 DBSY0
$3f4 ioportb dsr  \ W
$3f5 ioportb fifo \ RW
$3f7 ioportb dir  \ R
$3f7 ioportb ccr  \ W

\ Last seeked cylinder and head
0 value cyl
0 value head

\ We can't rely on sra/srb. On some hardware they don't work.
\ We therefore need to set up IRQ6
variable _cnt
: hadirq6? 0 _cnt @! bool ;
: clearirq hadirq6? drop ;
code isrIRQ6
  _cnt abs) 1 i) add,
  ax push, piceoi, ax pop,
  iret,

: wait ( n mask -- )
  ticks >r begin
    2dup msr ( n mask n mask msr )
    1000 r@ elapsedms? if
      ."FDC timeout " .x1 spc> .x1 spc> .x1 nl> err then
    and = until 2drop rdrop ;
: waitread $f0 $f0 wait ;
: waitwrite $b0 $f0 wait ;
: waitcmd $80 $f0 wait ;
: waitparam $90 $f0 wait ;
: waitresult $d0 $f0 wait ;
\ waitirq timeout is not fatal
: waitirq
  ticks begin
    1000 over elapsedms? not while
    hadirq6? not while repeat drop ;

create params 8 allot
: sendcmd ( paramsz cmd -- )
  waitcmd to fifo
  params swap 0 do c@+ waitparam to fifo loop drop ;
create result 8 allot
: readresult ( n -- a )
  result 2 0 fill
  result swap 0 do waitresult fifo swap c!+ loop drop
  result ;
: readresult# ( n -- )
  readresult le@ $00ff7f00 and if ."FDC bad result" err then ;
: sendcmdr ( ressz paramsz cmd -- a ) sendcmd readresult ;

: motoron ( -- ) $1c to dor ;
: motoroff ( -- ) $0c to dor ;
: version ( -- version ) 1 0 $10 sendcmdr c@ ;

: senseint ( -- ) 2 0 $08 sendcmdr drop ;

\ HUT=0 SRT=8 HLT=5 NON-DMA=1
: specify ( -- ) $0b80 params ! 2 $03 sendcmd ;
: recalibrate ( -- )
  clearirq
  $00 params ! 1 $07 sendcmd
  waitirq senseint ;
\ RESET=1 POWERDOWN=0 PRECOMP=0 (default) DRATE=0 (500 Kbps)
: reset ( -- ) $80 to dsr ;
: unlock ( -- ) 1 0 $14 sendcmdr drop ;
: init ( -- )
  reset 1 waitms senseint
  version $90 <> if ."FDC version mismatch" err then
  \ EIS=0 EFIFO=0 POLL=1 FIFOTHR=15
  $001f00 params ! 3 $13 sendcmd \ Configure
  specify motoron \ TODO: add a mechanism to turn this off
  recalibrate 1 initialized ! ;
: ?init initialized @ not if init then ;

: seek ( cyl head -- f )
  to head to cyl ( )
  ?init clearirq
  head 4* cyl 8 lshift or params le!
  2 $0f sendcmd
  waitirq senseint
  result 1+ c@ cyl = ;
: seek# seek not if ."FDC seek error" err then specify ;

: rwcmd ( cmd sec -- )
  ?init head 2 lshift params c!
  cyl params 1+ c!
  head params 2+ c!
  dup params 3 + c!
  \ GAP1=default, no DTL, EOT=sec, 512b sector size
  8 lshift $ff1b0002 or params 4+ ! ( cmd )
  8 swap sendcmd ;

: lbarw ( sec cmd -- )
  swap lba>chs ( cmd cyl head sec )
  rot> seek# rwcmd ;

: readfifo ( a u -- )
  0 do waitread fifo swap c!+ loop drop ( )
  7 readresult# ;

: writefifo ( a u -- )
  0 do waitwrite c@+ to fifo loop drop ( )
  7 readresult# ;

: readtrack ( a -- )
  $42 ( MFM+READTRK ) FDSECPERTRK rwcmd begin ( a )
    $d0 $d0 wait msr $20 and while dup .x spc>
    fifo swap c!+ repeat drop ( a )
  7 readresult# ;

create formatdata 4 allot
: formattrack ( -- )
  2 0 head cyl formatdata c!+ c!+ c!+ c! \ C/H/R/N
  ?init head 2 lshift params c!
  $af541202 params 1+ le! \ N=2 SC=18 GPL=suggested $54 FILLER=$af
  5 $4d sendcmd ( )
  FDSECPERTRK 0 do
    i 1+ formatdata 2+ c!
    formatdata 4 0 do waitwrite c@+ to fifo loop drop loop
  7 readresult# ;

: _sec@ drop swap $46 ( MFM+READ ) lbarw ( dst ) SECSZ readfifo ;
: _sec! drop swap $45 ( MFM+WRITE ) lbarw ( src ) SECSZ writefifo ;
: newfloppyblk ( -- blk )
  ['] _sec@ ['] _sec! SECSZ FDSECPERCYL FDCYLCNT * newblk ;

newfloppyblk const floppy

: fdc?
  $10 cmos@ bool \ we don't care about drive types or master/slave yet
  if version $90 = else 0 then ;

: fdc$ ['] isrIRQ6 $26 setISR 6 pic1unmask 0 initialized ! ;
