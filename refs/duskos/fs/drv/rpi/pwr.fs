\ Power management
unit drv/rpi/pwr

MMIO_BASE $100000 + const PWRREGS
PWRREGS $1c + const PWRRSTC
PWRREGS $24 + const PWRWDOG
$5a000000 const _cmd
$30 const _cfgmask
$20 const _cfgreset

: reboot ( -- )
  _cmd 1 or PWRWDOG !
  PWRRSTC @ _cfgmask invand _cfgreset or _cmd or
  PWRRSTC ! ;
