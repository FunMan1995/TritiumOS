unit drv/mac68k/xpram

33 const MAXSZ
create buf MAXSZ allot

:~ ( a n off -- a )
  swap MAXSZ min 16 lshift or [
  A) &) !, drop, ( D6=a+n D7=a )
  $2047 w, \ A0 D7 move,
  $2006 w, \ D0 D6 move,
  ] ;
: readxpram ( n off -- a ) buf rot> ~ [ $a051 w, ] ;
: writexpram ( a n off -- ) ~ [ $a052 w, ] drop ;

: set32bitmode ( -- )
  1 $8a readxpram ( a )
  dup c@ 5 or over c! ( a )
  1 $8a writexpram ;