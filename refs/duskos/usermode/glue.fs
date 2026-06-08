needs io/kbd lib/time
:> 2drop (?key) dup if drop c[] then ;
' ioerr
newstream newstreamkbd to keyboard

0 syscallback (snooze)
needs drv/timer lib/coop
' (ticks) ' (snooze) 1 timer$
createapplication Snoozer
' snooze IDLE Snoozer sethandler
Snoozer newcontext launchcontext

: transferout ( srcpath dstname -- )
  1 fdopen# >r drop ( srcpath ) \ V1=fd
  bootfs openpath >r \ V2=file
  0 V2 seek begin
    -1 V2 readbuf ?dup while ( a n ) V1 fdwrite drop repeat
  r> close r> fdclose ;

create _buf 512 allot
: transferin ( srcname dstid dstname -- )
  bootfs newfile bootfs open >r 0 fdopen# >r drop ( ) \ V1=file V2=fd
  begin _buf 512 V2 fdread ?dup while ( n )
    _buf swap V1 write repeat ( )
  r> fdclose r> close ;

: _reset [ PSORIGIN m) @, PSP) &) !, ] SYSVARS sysvars! ;
: handleSIGINT _reset abort"break " ;
: handleSIGSEGV _reset abort"segmentation fault " ;
: handleSIGINTnewThread
  [ RSP) &) @, -16 i) &, RSORIGIN m) !, ] handleSIGINT ;

13 syscallback _
' handleSIGINT ' handleSIGSEGV ' handleSIGINTnewThread _

:realias now (now) ;
