unit drv/timer

: ticks abort"timer uninitialized" ;
alias noop snooze
0 value uspertick

: timer$ to uspertick ['] snooze realias ['] ticks realias ;
: ?timer$ uspertick if 2drop drop else timer$ then ;

: elapsedus ( tickstamp -- ) ticks swap- uspertick * ;
: elapsedms elapsedus 1000 /+ ;
\ The "1+" is to palliate the fact that we're likely "midway" to a tick right
\ now and that we want to wait *at least* "ticks".
: us>ticks ( us -- ticks ) uspertick /+ 1+ ;
: elapsed? ( ticks tickstamp -- f ) ticks swap- <= ;
: elapsedus? ( targetus tickstamp -- f ) dip us>ticks | elapsed? ;
: elapsedms? ( targetms tickstamp -- f ) dip 1000 * us>ticks | elapsed? ;
: waitus ( us -- ) us>ticks ticks begin snooze 2dup elapsed? until 2drop ;
: waitms ( ms -- ) 1000 * waitus ;

: ?timeoutus" ( targetus tickstamp -- )
  compile elapsedus? [compile] ?abort" ; immediate
: ?timeoutms" compile elapsedms? [compile] ?abort" ; immediate

: ifelapsedus ( us -- )
  fbr, here# 0 , swap [compile] then ( targetus tickstampaddr )
  swap litn dup, dup ( tsaddr ) m) @,
  compile elapsedus?
  [compile] if
  compile ticks swap ( tsaddr ) m) !, drop, ; immediate
: ifelapsedms ( ms -- ) 1000 * [compile] ifelapsedus ; immediate
