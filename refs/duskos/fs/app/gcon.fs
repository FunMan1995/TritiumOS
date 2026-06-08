needs lib/coop io/stream io/grid fs/utag fs/sh mem/kv text/ed app/ed
unit app/gcon

addedbuf curbuf @ const gconbuf
: selectcon gconbuf curbuf ! -1 to gutter ;

: insertgridcontents ( -- )
  grid Grid.lines begin
    dup while 1- dup grid io/grid line[] nip not while repeat
    1+ else then ( lastidx )
  0 do i grid io/grid line[] edstream write# LF edstream putc loop
  "Grid command inserted above" edstream puts ;

: oobgridpos -1 grid to pos ;

0 value hasoutput
0 value dirty?
0 value gridmode
: runrtype
  1 to dirty? doto hasoutput 1 | not if
    selectcon appendline
    epos to mark then
  grid pos -1 <> gridmode not and if 1 to gridmode grid clear then
  gridmode if grid else edstream then write# ;
: consolerun ( ? xt -- ? )
  statusline ."Running..." idle
  0 to hasoutput 0 to gridmode oobgridpos
  ['] runrtype RTYPE @! >r ( xt ) execute r> RTYPE !
  gridmode if insertgridcontents then
  hasoutput if
    selectcon grid clear
    else statusline ."Ran command. There was no output." then ;
: run[] ( ... a u -- ... ) ['] interpret[] consolerun ;

: |C LControl or ;

kvtbl[
  CR |C  :>
    textsel[] ?dup not if
      drop curline line[] then ( a u )
    0 to visualmode
    run[] reframe pagerefresh ;
  'R' |C :>
    wordundercursor ?[] if
      []>str dup utag>.fs lookup if
        fs/core open nip ['] interpretstream consolerun endunit
        else sysdict find ?dup if consolerun then then
      reframe pagerefresh then ;
  'S' |C :>
    wordundercursor ?[] if
      []>str lookup# ['] listdir consolerun then ;
  'A' |C :> saveprevbuf selectcon fullrefresh ;
]kvtbl handlers

EDApp cloneapplication GConApp
GConApp newedctx const gconctx \ TODO: multi-task
0 value subctx \ nonzero when an App is invoked in run[]

:> subctx if
     evtype IDLE = if :selfdispatch then
     subctx context ! dispatch
   else :selfdispatch then ;
GConApp dispatch!

:> ( -- )
  ?type1 if exit then
  handlers evarg2 ?kvexec if
    displaypos else app/ed handlekey then ;
KEYPRESS GConApp sethandler

:> ( -- ) evarg1 dup to subctx ctxrunloop 0 to subctx ;
REGISTER GConApp sethandler

:> ( -- )
  doto dirty? 0 | gridmode not and
  ctxstate TYPING <> and
  if reframe pagerefresh oobgridpos then ;
IDLE GConApp sethandler

: gcon gconctx launchcontext ;
:~ edstream write# ;
: initgcon gconctx to rootcontext selectcon ['] ~ RTYPE ! ;
