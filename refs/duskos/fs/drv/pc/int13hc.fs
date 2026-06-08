$200 const BOOTSECSZ
$7c18 w@ const SECPERTRK
$7c1a w@ const NUMHEADS
$7c24 c@ const DRVNO
$7d68 @ const LBASTART
variable buf

: _int13h ( lbasec cmd -- )
  >r DRVNO swap SECPERTRK /mod ( drv sec trk )
  NUMHEADS /mod ( drv sec head cyl ) rot 1+ r> int13hchs buf ! ;

:~ BOOTSECSZ 4/ move ;
: biossec@ ( sec buf -- ) swap $02 _int13h buf @ swap ~ ;
: biossec! ( sec buf -- ) buf @ ~ $03 _int13h ;
: bootsec@ swap LBASTART + swap biossec@ ;
