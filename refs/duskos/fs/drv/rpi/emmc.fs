needs drv/timer lib/ival io/blk
unit drv/rpi/emmc

MMIO_BASE $300000 + const EMMC_BASE
EMMC_BASE absvalmap {
  +$04 uint blksizecnt arg1 cmdtm resp0 ;
  +$20 uint data _status control0 control1 interrupt irptmask ;
}

consts
  $200 SECSZ \
  $017e8000 INT_ERROR_MASK \
  $00000020 INT_READ_READY \
  $00000010 INT_WRITE_READY \
  $00000002 INT_DATA_DONE \
  $00000001 INT_CMD_DONE \
  $80000000 ACMD41_COMPLETE \
  $40000000 ACMD41_CCS \
  $01000000 SRST_HT \
  $00000002 CLK_STABLE

variable rca \ RCA of current card, stored in higher 16b
variable ccs

: err abort"EMMC error" ;
: # not if err then ;
: status ( mask -- )
  $10000 0 do
    _status over and not if drop break then
    interrupt INT_ERROR_MASK and not #
    1 waitus loop broke? not if abort"status timeout" then ;
: cmdwait 1 status ; \ CMD_INHIBIT mask
: datawait 2 status ; \ DAT_INHIBIT mask
: intclr interrupt to interrupt ;
: intwait ( mask -- )
  INT_ERROR_MASK or $10000 0 do
    interrupt over and if break then 1 waitus loop
  broke? not if abort"intwait timeout" then
  interrupt INT_ERROR_MASK and not #
  ( mask ) to interrupt ;
: readready INT_READ_READY intwait ;
: writeready INT_WRITE_READY intwait ;
: cmddone INT_CMD_DONE intwait ;
: datadone INT_DATA_DONE intwait ;
: cmd ( arg cmdidx -- ) cmdwait intclr swap to arg1 to cmdtm cmddone ;
: cmdr ( arg cmdidx -- resp ) cmd resp0 ;
: acmd ( arg cmdidx -- ) 0 $37020000 cmd ( CMD55 ) 100 waitus cmd ;
: cmd0 0 0 cmd ; \ CMD_GO_IDLE
: cmd2 0 $02010000 cmd ; \ CMD_ALL_SEND_CID
: cmd3 ( -- rca ) 0 $03020000 cmdr ; \ CMD_SEND_REL_ADDR
: cmd7 ( rca -- ) $07030000 cmd ;
: cmd8 ( arg -- resp ) $08020000 cmd 1000 waitus resp0 ;
: cmd17 ( lba -- ) $11220010 cmd ;
: cmd24 ( lba -- ) $18220000 cmd ;
: acmd41 ( arg -- resp ) $29020000 acmd 1000 waitus resp0 ;
: clk! ( div -- )
  cmdwait datawait 8 lshift $000e0005 or to control1 10000 0 do
    10 waitus
    control1 CLK_STABLE and if break then loop
  broke? not if abort"clk failed" then ;
: :sec@ ( sec dst self -- ) drop
  datawait $00010200 to blksizecnt
  swap ccs @ not if SECSZ * then cmd17 readready
  SECSZ 4/ swap addrof data [ ( n dst src )
    A) &) !, drop, begin ( n dst A=src )
      A) S>) @, W) S>) !+, -1 PSP) +n, ?brnz, ]
  2drop ;
: :sec! ( sec src self -- ) drop
  datawait $00010200 to blksizecnt
  swap ccs @ not if SECSZ * then cmd24 writeready
  SECSZ 4/ swap addrof data [ ( n src dst )
    A) &) !, drop, begin ( n src A=dst )
      W) S>) @+, A) S>) !, -1 PSP) +n, ?brnz, ]
  2drop datadone ;
\ We assume the EMMC base clock to be 100 MHz. During identification, we need
\ 400 kHz, which means a 250 divider. Then, later, we go to a "normal" frequency
\ of 25 MHz, which means a 4 divider.
: emmcinit ( -- )
  SRST_HT to control1 \ reset
  100 0 do 1 waitus control1 SRST_HT and not if break then loop
  broke? #
  250 clk! \ 400 kHz
  INT_READ_READY INT_WRITE_READY or INT_CMD_DONE or INT_DATA_DONE or
  INT_ERROR_MASK or to irptmask
  cmd0 $1aa cmd8 $fff and $1aa <> if abort"CMD8 error" then 10 0 do
    $50ff8000 acmd41 ACMD41_COMPLETE and if break then 400 waitus loop
  broke? not if abort"CMD41 timeout" then
  resp0 ACMD41_CCS and bool ccs !
  cmd2 cmd3 $ffff0000 and rca !
  4 clk! \ 25 MHz
  rca @ cmd7 ;
: newemmcblk ( -- blk ) ['] :sec@ ['] :sec! SECSZ -1 newblk ;

newemmcblk const emmc
