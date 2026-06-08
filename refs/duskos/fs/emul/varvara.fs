needs io/stream io/kbd io/mouse lib/coop asm/uxntal emul/uxn \
      emul/varvara/core emul/varvara/console emul/varvara/screen \
      emul/varvara/ctrl emul/varvara/file emul/varvara/dt
unit emul/varvara

: varvara$ uxn$ initscreen initdt ;
: newvarvara ( -- ) varvarahandlers newuxn ;
: varvara# uxn not if newvarvara then varvara$ ;

createapplication UxnApp

:> 0 to curnkc 0 [dev!] $f checkargs poweron handlearguments ;
INITIALIZE UxnApp sethandler

:> evarg2 to curnkc
   evarg1 4 = if stopcurrent then ;
KEYPRESS UxnApp sethandler

:> 1 to evarg4 \ inhibit
   evarg1 vscreen Pixbuf.width 1- min [dev2!] $92
   evarg2 vscreen Pixbuf.height 1- min [dev2!] $94
   [dev2@] $90 ?callvector ;
MOUSEMOVE UxnApp sethandler

\ in uxn, middle button is b1 and right button is b2. In Dusk, we swap
: butswap ( n -- n ) A! 1 and A> 2/ 2 and or A> 2* 4 and or ;

:> evarg3 butswap [dev!] $96
   [dev2@] $90 ?callvector ;
MOUSECLICK UxnApp sethandler

: stop? [dev@] $f [dev2@] $10 [dev2@] $20 [dev2@] $80 or or not or ;
:> ?refreshscreen stop? if stopcurrent else
     updatedt ?handleconsole ?handlecontroller ?handlescreen then ;
IDLE UxnApp sethandler

UxnApp newcontext const uxnctx
: uxn<< varvara# rstvec uxntal<< uxnctx launchcontext ;
: uxnrom<<
  varvara# word openpath rstvec $ff00 rot read drop uxnctx launchcontext ;
