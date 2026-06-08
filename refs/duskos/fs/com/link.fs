needs lib/struct lib/str lib/psrs lib/time
unit com/link

enum FTIP4 FTARP FTUNKNOWN FTCNT

0 value frameptr
FTUNKNOWN value frametype
0 value payloadptr
0 value payloadsz
0 value curlink

struct Link {
  xt readframe beginframe replytoframe sendframe waitsent ;
}

: newlink ( <methods> -- link ) 5 n,@ ;

$1000 const LOGSZ
create loggedframes LOGSZ allot
0 value logging?
loggedframes value logwptr \ where it's going to write next
loggedframes value logrptr \ where it's going to read next
0 value logtime

: beginlogging loggedframes dup to logwptr to logrptr 1 to logging? ;
: deframe noop ;
: nextf ( -- f )
  logrptr not if 0 else
    logrptr @+ to logrptr
    @+ to logtime @+ to curlink @+ to frametype
    @+ >r @+ ( frame framesz V1=payloadoff )
    r@ - to payloadsz ( frame )
    dup to frameptr r> + to payloadptr deframe 1 then ;

: framesz payloadptr frameptr - payloadsz + ;
: ?log ( -- )
  logging? not if exit then
  logwptr loggedframes - framesz + $18 + LOGSZ > if 0 to logging? exit then
  logwptr loggedframes <> if logwptr loggedframes llend ! then
  frametype curlink now 0 logwptr !+ !+ !+ !+ ( a )
  payloadptr frameptr - swap !+ ( a )
  framesz swap !+ ( a )
  frameptr swap framesz cmove+ to logwptr ;
