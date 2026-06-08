unit lib/time

consts 2000 EPOCHYEAR 60 S/M 60 M/H 24 H/D 365 D/Y
S/M M/H * const S/H
S/H H/D * const S/D

: minutes S/M * ;
: hours S/H * ;
: days S/D * ;

EPOCHYEAR value year
1 value month
1 value day
0 value doy \ day of year
0 value sod \ seconds of day
0 value hour
0 value minute
0 value second

: leap? ( year -- f ) tri 4 mod not | 100 mod not | 400 mod bool or and ;
: >year/doy/sod ( time -- )
  S/D /mod swap to sod EPOCHYEAR begin ( d y )
    tuck leap? 365 + 2dup >= while ( y d n )
    - swap 1+ repeat drop to doy to year ;
create cal map< c, 31 28 31 30 31 30 31 31 30 31 30 31
: adjcal 28 year leap? + cal 1+ c! ;
: >month/day ( -- )
  adjcal doy cal begin ( d a )
    c@+ rot ( a n d )
    2dup <= while ( a n d )
    swap- swap repeat ( a n d )
  nip 1+ to day cal - to month ;
: >hms ( -- ) sod S/M /mod M/H /mod to hour to minute to second ;
: decompose ( time -- ) >year/doy/sod >month/day >hms ;

: year0 year EPOCHYEAR - ;
: compose ( -- time )
  \ the 1+ is because the first year is a leap one
  year0 dup if D/Y * year0 4/ + 1+ year0 100 >= - then ( d )
  adjcal cal month 1 do c@+ rot + swap loop drop ( d )
  day 1- + ( d )
  S/D * ( t )
  hour S/H * + minute S/M * + second + ;

19 const TIMEFMTSZ
create fmt ,"0000/00/00 00:00:00"
: fmt2 ( n off -- )
  fmt + over 10 < if '0' swap c!+ then
  swap formatdec 2 min rot swap cmove ;
: formattime ( time -- a u )
  decompose year formatdec 4 min fmt swap cmove ( )
  month 5 fmt2
  day 8 fmt2
  hour 11 fmt2
  minute 14 fmt2
  second 17 fmt2
  fmt TIMEFMTSZ ;

: .time formattime rtype ;

: now 0 ;
: ago now swap- ;
