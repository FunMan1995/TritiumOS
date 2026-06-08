needs lib/coop lib/ival drv/timer lib/time
unit bench/coop

context CONTEXTSZ ivalmapfrom { uint val limit ; }

createapplication IdlerApp
:> val limit < if doto val dup . 1+ | else stopcurrent then ;
IDLE IdlerApp sethandler
:> 0 to val ;
INITIALIZE IdlerApp sethandler

: idlerctx ( limit -- ctx ) IdlerApp newcontext 0 , swap , ;

createapplication EchoApp
EchoApp newcontext const echoctx

:> evarg1 dup emit 'q' = if ."\nstopping EchoApp\n" stopcurrent then ;
KEYPRESS EchoApp sethandler

createapplication TimerApp
context CONTEXTSZ ivalmapfrom {
  uint periodms lastping ;
  xt timercb ;
}

:> periodms lastping elapsedms? if ticks to lastping timercb then ;
IDLE TimerApp sethandler
: timerctx ( xt period -- ctx ) TimerApp newcontext rot> , 0 , , ;

createapplication AtApp
context CONTEXTSZ ivalmapfrom {
  uint date ;
  xt atcb ;
}

:> now date >= ctxstate RUNNING = and if stopcurrent atcb then ;
IDLE AtApp sethandler
: atctx ( xt time -- ctx ) AtApp newcontext rot> , , ;

: .at ( ctx -- )
  context[ ctxstate STOPPED = if ."executed at "
  else ."will execute at " then date .time nl> ]context ;
:> context @ .at ;
INITIALIZE AtApp sethandler
