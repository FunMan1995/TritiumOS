needs lib/str comp/sig
unit lib/diag

create _ ,"KMG"
: .sz ( size-in-bytes -- )
  0 begin ( sz lvl )
    swap 1024 /mod ( lvl r q ) ?dup while
    nip swap 1+ repeat ( lvl sz )
  . ?dup if 1- _ + c@ emit then 'B' emit ;

: spit[] ( a u -- ) 4* do[] i @ .x spc> 4 +loop ;
: wspit[] 2* do[] i w@ .x2 spc> 2 +loop ;
: cspit[] do[] i c@ .x1 spc> loop ;
: squarespit 0 do tuck 0 do c@+ .x1 loop nl> swap loop 2drop ;
: psdump [ dup, PSP) &) @, ] PSORIGIN @ over - 4/ 1- spit[] ;
: .S ( -- ) ."PS " scnt .x2 ." RS " rcnt .x2 ." -- " stack? psdump ;
annotatelast ( -- )
: .free
  here ['] noop ( first word in xcomp/boot ) - .sz ." used "
  HEREMAX @ here - .sz ." free" ;

: dumpn ( a n -- )
  16 /+ 0 do
    ':' emit dup .x spc> ( a )
    dup 16 do[] i c@+ .x1 c@ .x1 spc> 2 +loop ( a )
    dup 16 do[] i c@ dup SPC - $5e > if drop '.' then emit loop nl>
    16 + loop drop ;
: dump 128 dumpn ;
annotatelast ( uint -- )
