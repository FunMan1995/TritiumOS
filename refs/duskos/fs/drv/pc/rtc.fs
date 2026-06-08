needs lib/time drv/pc/cmos
unit drv/pc/rtc

0 value regb

: bcd? regb 4 and not ;
: bcd ( n -- n ) bi $f and | 16/ 10 * + ;
: ?bcd bcd? if bcd then ;
: reg@ ( off -- n ) cmos@ ?bcd ;
: hour@ ( off -- n ) cmos@ bi $80 and | ?bcd swap if 12 + then 24 mod ;
:realias now ( -- time )
  $b cmos@ to regb
  0 reg@ to second
  2 reg@ to minute
  4 hour@ to hour
  7 reg@ to day
  8 reg@ to month
  9 reg@ 2000 + to year
  compose ;
