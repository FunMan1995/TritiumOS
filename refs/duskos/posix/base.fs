' byefail ABORTPTR !
SYSVARS HEREMAX !
' (rtype) console!
needs io/kbd io/kbd
:> 2drop (?key) dup if drop c[] then ;
' ioerr
newstream newstreamkbd to keyboard

needs drv/timer lib/coop lib/time
' (ticks) ' (snooze) 1 timer$
createapplication Snoozer
' snooze IDLE Snoozer sethandler
Snoozer newcontext launchcontext
:realias now (now) ;
