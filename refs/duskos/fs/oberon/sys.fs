needs lib/coop comp/oberon comp/oberon/gc io/kbd io/mouse

oberon<< oberon/system.mod
unit oberon/sys

createapplication OberonApp
context CONTEXTSZ ivalmapfrom { uint savedrtype ; }

:> evarg2 Input.TranslateNKC Oberon.TypeChar ;
KEYPRESS OberonApp sethandler

:> 1 to evarg4 fixedmouse Oberon.ProcessMouse ;
dup MOUSEMOVE OberonApp sethandler
MOUSECLICK OberonApp sethandler
:> drawbuf [ 100 ] ifelapsedms gc then ;
IDLE OberonApp sethandler

: oberonrtype
  dup 1+ r! strallot r! swap cmove+ 0 swap c! ( )
  2r> System.LogString ;

: newobctx OberonApp newcontext RTYPE @ , ;

:> fulldamage ['] oberonrtype RTYPE @! to savedrtype ;
INITIALIZE OberonApp sethandler

:> savedrtype RTYPE ! ;
FINALIZE OberonApp sethandler

newobctx const oberonctx
: oberon oberonctx launchcontext ;
