\ this "needs" below is for the "common signatures" part, not actually needed
\ by comp/sig.
needs io/stream fs/core drv/timer
needs lib/type lib/tagl
unit comp/sig

:~ n"SIGT" rot addtag ;
: annotatelast type< current ~ ;
: annotate type< begin dup ' ~ eol? until drop ;

: findannotated ( name -- ?sig xt-or-0 )
  sysdict find dup if
    dup n"SIGT" findtag if swap else drop 0 then then ;

\ common signatures
annotate ( -- ) abort quit noop snooze
annotate ( uchar -- ) emit
annotate ( *uchar uint -- ) rtype
annotate ( *uchar -- ) stype
annotate ( uint -- ) . .x .x2 .x1 allot
annotate ( uint -- *void ) allot@
annotate ( *void uint uint -- ) cfill fill
annotate ( *void *void uint -- ) cmove move
annotate ( *void -- uint ) le@ wle@ be@ wbe@
annotate ( uint *void -- ) le! wle! be! wbe!
annotate ( *char *char -- *char ) strcat
annotate ( -- *Stream ) console herestream nullstream
annotate ( *void uint *Stream -- uint ) read write
annotate ( *void uint *Stream -- ) read# write#
annotate ( *Stream -- ) close flush truncate interpretstream
annotate ( *Stream *Stream -- ) spit
annotate ( *Stream -- uint ) getc
annotate ( uchar *Stream -- ) putc
annotate ( uint *Stream -- ) seek
annotate ( *uchar -- *Stream ) openpath
annotate ( -- uint ) ticks
annotate ( uint uint -- uint ) elapsedus? elapsedms?
annotate ( uint -- ) waitus waitms
