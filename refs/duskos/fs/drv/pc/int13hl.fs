$200 const BOOTSECSZ
$7c24 c@ const DRVNO
$7d68 @ const LBASTART
variable buf

: _int13h ( lbasec cmd -- ) DRVNO rot> $40 + int13hlba buf ! ;
:~ BOOTSECSZ 4/ move ;
: biossec@ ( sec buf -- ) swap $02 _int13h buf @ swap ~ ;
: biossec! ( sec buf -- ) buf @ ~ $03 _int13h ;
: bootsec@ swap LBASTART + swap biossec@ ;
