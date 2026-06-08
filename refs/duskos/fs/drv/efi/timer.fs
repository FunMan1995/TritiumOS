needs drv/efi drv/timer
unit drv/efi/timer

\ EFI has a very bad resolution on SetTimer(), hence 50ms
variable _ticks
code (ticks) dup, _ticks m) @, exit,

qvariable event
: SetTimer ( ms -- )
  argstart event aq@ arg0!
  1 arg1! \ Periodic
  10000 * arg2!
  BootServices [q+@] $18 8 ( SetTimer ) efiexec# ;

: CreateTimerCb ( ctx callback -- )
  argstart $80000200 arg0! \ EVT_TIMER | EVT_NOTIFY_SIGNAL
  16 arg1! absaddr arg2! absaddr arg3! event absaddr arg4!
  BootServices [q+@] $18 7 ( CreateEvent ) efiexec# ;

_ticks ' inccb CreateTimerCb 50 SetTimer

: CreateTimerWait ( -- )
  argstart $80000000 arg0! \ EVT_TIMER | EVT_NOTIFY_WAIT
  16 arg1! 0 arg2! 0 arg3! event absaddr arg4!
  BootServices [q+@] $18 7 ( CreateEvent ) efiexec# ;

qvariable waitfor
qvariable _
CreateTimerWait 50 SetTimer event waitfor 2 move
: (snooze) ( -- ) \ snoozes 50 ms, minumum resolution
  argstart 1 arg0! waitfor absaddr arg1! _ absaddr arg2!
  BootServices [q+@] $18 9 ( WaitForEvent ) efiexec# ;

create idlewait 2 8* allot0
:~ idlewait qsz ConIn [ 2 q* q+n, ] qmove
   waitfor idlewait 2 move ; ~
: efiidle ( -- ) \ 50 ms or kbd hit
  argstart 2 arg0! idlewait absaddr arg1! _ absaddr arg2!
  BootServices [q+@] $18 9 ( WaitForEvent ) efiexec drop ;

' (ticks) ' (snooze) 50000 ?timer$
