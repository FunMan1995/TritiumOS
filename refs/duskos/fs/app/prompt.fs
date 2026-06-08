needs lib/ival lib/coop io/typeln
unit app/prompt

$80 const LNSZ
createapplication PromptApp
context CONTEXTSZ ivalmapfrom { uint tb subctx ; }

: newpromptcontext
  LNSZ newtypebuf PromptApp newcontext ( tb ctx ) swap , 0 , ;

:> subctx if subctx context ! dispatch else :selfdispatch then ;
PromptApp dispatch!

: .ok ." ok\n" ;
:> .ok ; INITIALIZE PromptApp sethandler

:> evarg1 dup to subctx ctxrunloop 0 to subctx ;
REGISTER PromptApp sethandler

:> evarg1 tb type1 if nl> ?dup if interpret[] .ok then then ;
KEYPRESS PromptApp sethandler

: prompt$ coop$ newpromptcontext to rootcontext ;
