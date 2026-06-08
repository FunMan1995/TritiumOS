$7d68 @ const LBASTART
$7d6c @ const LBASIZE

: maybepart ( blk -- blk )
  LBASTART bool LBASIZE bool and if LBASTART LBASIZE rot newpart then ;

:> drop biossec@ ;
:> drop biossec! ;
BOOTSECSZ -1 newblk maybepart newfatfs bootfs!
:~ f<< quit ; ~ init.fs
