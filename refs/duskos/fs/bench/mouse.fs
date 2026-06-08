needs lib/coop io/mouse
unit bench/mouse

createapplication MBApp

:> ( event -- ) ."Quitting!\n" drop stopcurrent ;
KEYPRESS MBApp sethandler

:> ( event -- ) 4+ @+ . spc> @+ . spc> @+ .x1 spc> @ .x1 nl> ;
MOUSECLICK MBApp sethandler

:> ( event -- )
  drop
  ."Clicks show mouse pos and state\n"
  ."Quits on any key press\n" ;
INITIALIZE MBApp sethandler

MBApp newcontext const mbctx \ this is a static app
: mousebench mbctx launchcontext ;
