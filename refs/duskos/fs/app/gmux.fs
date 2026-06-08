needs lib/coop io/kbd io/mouse io/grid app/prompt
unit app/gmux

4 const SUBCTXCNT
create subctxs SUBCTXCNT 4* allot
:~ SUBCTXCNT 0 do newpromptcontext i 4* subctxs + ! loop ; ~
0 value activeidx

create subgrids SUBCTXCNT 4* allot0
: ng COLS LINES newgrid
     grid pixw over to pixw
     grid pixh over to pixh ;
:~ SUBCTXCNT 0 do ng i 4* subgrids + ! loop ; ~

createapplication GMuxApp
GMuxApp newcontext const gmuxctx

create activeonly map< , KEYPRESS MOUSEMOVE MOUSECLICK REGISTER
: activeonly? evtype activeonly 4 idx dup if nip then ;

:~ SUBCTXCNT 1- min 4* ;
: activategrid ( idx -- ) ~ subgrids + @ to grid ;
: activatectx ( idx -- ) dup ~ subctxs + @ context ! activategrid ;
: gmuxctx! ( ctx idx -- ) ~ subctxs + ! ;

:> :selfdispatch
   context grid 2>r
   activeonly? if activeidx activatectx dispatch else
     SUBCTXCNT 0 do i activatectx dispatch loop then
   2r> to grid context ! ;
GMuxApp dispatch!

' noop REGISTER GMuxApp sethandler

:> SUBCTXCNT 0 do i activatectx RUNNING to ctxstate loop 0 activatectx ;
INITIALIZE GMuxApp sethandler

:> evarg2 F1 F4 within? if
     evarg2 F1 - dup to activeidx
     activategrid grid alldirty 1
     IDLE to evtype then ;
KEYPRESS GMuxApp sethandler

: gmux$ ( -- ) gmuxctx to rootcontext ;
