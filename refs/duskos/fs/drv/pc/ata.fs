needs drv/pc/ioport io/blk asm/x86
unit drv/pc/ata

extends Blk struct ATADrive { uint bus drvunit ; }
$200 const SECSZ

: _iobase ( drv -- port ) bus if $170 else $1f0 then ;
: _ctlbase ( drv -- port ) _iobase $206 or ; \ $3f6/$376
: _data _iobase ;
: _error _iobase 1+ ;
: _features _error ;
: _seccnt _iobase 2 + ;
: _secno _iobase 3 + ;
: _cyllo _iobase 4 + ;
: _cylhi _iobase 5 + ;
: _drvhd _iobase 6 + ;
: _stat _iobase 7 + ;
: _cmd _stat ;
: _altreg _ctlbase ;
: _devctl _ctlbase ;
: _drvaddr _ctlbase 1+ ;

$ec const IDENTIFY

: stat ( drv -- r ) _stat pc@ ;

: identify ( drv -- r )
  r! drvunit bool 4 lshift $a0 or r@ _drvhd pc!
  0 r@ _secno pc! 0 r@ _cyllo pc! 0 r@ _cylhi pc!
  IDENTIFY r@ _cmd pc! r> stat ;
: reset ( drv -- r ) >r $04 r@ _devctl pc! 0 r@ _devctl pc! r> stat ;
: _wait ( drv -- ) begin dup stat $80 and not until drop ;

: _locate ( sec drv -- )
  >r dup 24 rshift $f and $e0 or r@ drvunit bool 4 lshift or
  r@ _wait r@ _drvhd pc!
  0 r@ _features pc!
  1 r@ _seccnt pc!
  dup $ff and r@ _secno pc!
  dup 8 rshift r@ _cyllo pc!
  16 rshift r> _cylhi pc! ;

code _ ( dst port -- )
  dx ax mov, bx si 0 d) mov,
  cx $100 imm) mov, pc
    ax word) dx in,
    bx 0 d) word) ax mov,
    bx inc, bx inc, cx dec,
  ( pc ) abs>rel jnz, nip, drop, ret,

: ata@ ( sec dst drv -- ) >r
  swap r@ _locate $20 ( read sectors ) r@ _cmd pc!
  r@ _wait ( dst ) r> _data _ ;

code _ ( src port -- )
  dx ax mov, bx si 0 d) mov,
  cx $100 imm) mov, pc
    ax word) bx 0 d) mov,
    ax word) dx out,
    bx inc, bx inc, cx dec,
  ( pc ) abs>rel jnz, nip, drop, ret,

: ata! ( sec src drv -- ) >r
  swap r@ _locate $30 ( write sectors ) r@ _cmd pc!
  r@ _wait ( src ) r@ _data _
  $e7 ( flush cache ) r@ _cmd pc! r> _wait ;

: .ata ( drv -- )
  r! bus . ':' emit r@ drvunit . spc> r> identify .x1 ;

\ TODO: check geometry and set seccnt
: newatadrive ( bus unit -- drv )
  ['] ata@ ['] ata! SECSZ -1 newblk rot , swap , ;

0 0 newatadrive const ATA0:0
0 1 newatadrive const ATA0:1
1 0 newatadrive const ATA1:0
1 1 newatadrive const ATA1:1

create _ ATA0:0 , ATA0:1 , ATA1:0 , ATA1:1 ,
: .ataall ( -- ) _ 4 0 do @+ .ata nl> loop drop ;
