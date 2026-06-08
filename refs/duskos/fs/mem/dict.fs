unit mem/dict

\ Dictionary
8 const ENTRYSZ

: forget 'e @ SYSDICT ! ;
: delete 0 'e e>wlen c! ;
: extractdict 0 'e @! SYSDICT @! const ;
: reserveentry ( dict str sz -- ) over c@ + ENTRYSZ + reserve entry ;

\ Address-to-Entry mechanism
: memok? ( a -- f )
  bi KERNELSTART KERNELEND within? | HERESTART here within? or ;
: namerange? ( c -- f ) $21 - $5d <= ;
: ?xt>e ( a -- e-or-0 )
  4 - dup memok? if ( e )
    dup entryname[] ?dup if ( e a u )
      0 do c@+ namerange? not if break then loop drop ( e )
      broke? if drop 0 then ( e )
      else 2drop 0 then
    else drop 0 then ;
: xt>e# ?xt>e dup not ?abort"no associated entry" ;
: .word ( w -- ) xt>e# entryname[] rtype ;
: words ( -- )
  SYSDICT @ begin dup while dup e>xt .word spc> @ repeat drop ;

\ Unit
: inunit? ( e unit -- f )
  dup unittop @ >A begin ( e unit A=ll )
    over A> <> while
    dup unitbottom @ A> <> while
    @A@ >A repeat 0 else 1 then nip nip ;

: unitofentry ( e -- unit-or-0 )
  currentunit @ begin ( e ll )
    ?dup while
    2dup inunit? not while @ repeat
    ( e unit ) else ( e ) 0 then nip ;
