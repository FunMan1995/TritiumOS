needs lib/ival io/kbd mem/ll
unit lib/coop

16 const SLOTCNT
enum REGISTER INITIALIZE FINALIZE IDLE KEYPRESS
enum STOPPED RUNNING

create curevent $14 allot0
curevent absvalmap { uint evtype evarg1 evarg2 evarg3 evarg4 ; }

variable context
context ivalmap { uint ctxapp ctxstate ; }
8 const CONTEXTSZ

create generators SLOTCNT 4* allot0
0 value generatorcnt
0 value rootcontext

$20 const BGMAXCNT
0 value bgcnt
create bgctxs BGMAXCNT 4* allot0 \ array of contexts

: dispatch ctxapp @ execute ;
: dispatch! ( xt app -- ) ! ;

KEYPRESS value lastslotidx
: slot# ( n -- n ) dup SLOTCNT >= ?abort"ran out of event slots" ;
: addeventtype ( -- slotidx ) lastslotidx 1+ slot# dup to lastslotidx ;

: addgenerator ( xt -- )
  generatorcnt slot# dup 1+ to generatorcnt
  4* generators + ! ;

: :selfdispatch ctxapp 4+ evtype 4* + @ execute ;
: :register abort"disorderly cooperation! get in line, comrade!" ;
create tmpl ' :selfdispatch , ' :register , SLOTCNT 1- 4* allot
tmpl 8+ SLOTCNT 1- ' noop fill

: cloneapplication ( app -- ) create SLOTCNT 1+ 4* cmoveallot CURWORD @ s, ;
: createapplication ( -- ) tmpl cloneapplication ;
: eventid# ( n -- n ) dup SLOTCNT >= ?abort"not an event ID" ;
: sethandler ( xt type app -- ) 4+ swap eventid# 4* + ! ;

: newcontext ( app -- context ) 0 swap 2 n,@ ;

: context[ ( newctx -- oldctx ) context @! ;
: ]context ( oldctx -- ) context ! ;

: appname ( app -- str ) SLOTCNT 4* 4+ + ;

:~ dup bgcnt >= ?abort"bg index out of bounds!" ;
: bgget ( index -- context-or-0 )
  ~ 4* bgctxs + @ ;

: bgremove ( index -- context-or-0 )
  ~ 0 swap 4* bgctxs + @! ;

: .bg ( -- )
  context @ >r
  bgcnt 0 do i bgget ?dup if context !
    i . spc> ctxstate . spc> ctxapp appname stype nl>
  then loop r> context ! ;

: bgpause ( index -- )
  bgget ?dup if context[ STOPPED to ctxstate ]context then ;

: bgresume ( index -- )
  bgget ?dup if context[ RUNNING to ctxstate ]context then ;

: bgrevive ( index -- )
  bgget ?dup if context[ ctxstate STOPPED = if
    RUNNING to ctxstate INITIALIZE to evtype dispatch
    then ]context then ;

: bgkill ( index -- )
  bgget ?dup if context[ ctxstate STOPPED <> if
    STOPPED to ctxstate FINALIZE to evtype dispatch
    then ]context then ;

: bghas ( context -- f )
  bgctxs bgcnt idx dup if nip then ;

: bgdispatch ( -- )
  evtype REGISTER = if exit then
  context @ >r
  bgcnt 0 do i bgget ?dup if context !
    ctxstate STOPPED <> if dispatch then
  then loop
  r> context ! ;

: rootdispatch ( -- )
  rootcontext ?dup not ?abort"no runloop!"
  context[ >r dispatch r> ]context
  bgdispatch ;

: ctxrunloop ( context -- )
  context[ >r
  RUNNING to ctxstate
  INITIALIZE to evtype dispatch
  promptforkey begin ( V1=ctx )
    ctxstate while
    generatorcnt 0 do generators i 4* + @ execute loop
    idle repeat
  FINALIZE to evtype dispatch
  r> ]context ;

: runloop rootcontext ctxrunloop ;
: unstoppableloop begin runloop again ;

: launchrootcontext ( context -- )
  doto rootcontext swap | >r runloop r> to rootcontext ;

: bg? ( app -- f ) 4+ KEYPRESS 4* + @ ['] noop = ;
: runbg ( ctx -- ctx ) dup context[
  RUNNING to ctxstate INITIALIZE to evtype ]context ;
: nilbgidx ( -- idx )
  0 bgctxs BGMAXCNT idx not ?abort"too many background contexts!" ;

: bgadd ( ctx -- idx )
  nilbgidx dup bgcnt = if doto bgcnt 1+ | then tuck 4* bgctxs + ! ; 

: launchcontext ( context -- )
  dup @ bg? if runbg dup bghas not if bgadd drop else drop then exit then
  rootcontext if
    to evarg1 REGISTER to evtype rootdispatch else launchrootcontext then ;

: stopcurrent ( -- ) STOPPED to ctxstate ;

: abortrecover ." lib/coop recovering from abort. Press a key" key (abort) ;
: coop$ ['] unstoppableloop MAINLOOP ! ['] abortrecover ABORTPTR ! ;

:realias idle IDLE to evtype rootcontext if rootdispatch else bgdispatch then ;

:> ( -- )
  keyboard ?nkc if
    dup keyboard nkc>char to evarg1
    keyboard melt to evarg2
    KEYPRESS to evtype
    rootdispatch promptforkey then ;
addgenerator
