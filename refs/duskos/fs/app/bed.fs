needs lib/coop mem/kv io/typeln io/kbd io/stream io/grid
unit app/bed

createapplication BEDApp
2 const TYPING \ for context state
BEDApp newcontext const bedctx \ TODO: multi-task
0 value processtyped \ TODO: put into context ivalmap
COLS newtypebuf const typebuf

: bed bedctx launchcontext ;

create buffer $2000 allot
0 value cellsperline
-1 value topleft \ address of the topleft cell
0 value bedpos \ relative to topleft, in *nibbles*
0 value oldbedpos
0 value maxpos

: cellsperscreen cellsperline LINES * ;
: nibblesperline cellsperline 8* ;
: bytesperline cellsperline 4* ;
: bytesperpage LINES 3 / 2* bytesperline * ;
: posaddr bedpos 2/ topleft + ;
: posline ( pos -- lineno ) 8/ cellsperline / ;
: buf@ ( pos -- different? c ) 2/ bi buffer + c@ | topleft + c@ over <> swap ;
: ?highlight ( f n -- )
  swap if grid pos over - swap INVERTEDCOLOR grid setcolor[] else drop then ;
: gridposhex ( pos -- gridpos )
  8 /mod cellsperline /mod COLS * swap 9 * 10 + + + ;
: .hex ( pos -- ) dup gridposhex grid seek buf@ .x1 2 ?highlight ;
: hexlines ( -- ) maxpos 0 do i .hex 2 +loop ;
: ascii ( c -- c ) dup SPC - $5e > if drop '.' then ;
: gridposascii ( pos -- gridpos )
  2/ bytesperline /mod COLS * + cellsperline 9 * + 10 + ;
: .ascii ( pos -- )
  dup gridposascii grid seek
  buf@ ascii emit 1 ?highlight ;
: asciilines ( -- ) maxpos 0 do i .ascii 2 +loop ;
: addresses ( -- )
  bedpos posline 0 over at-xy ( curline )
  dup bytesperline * topleft + .x ( curline )
  dup 0 do ( curline )
    0 i at-xy dup i - ."   -" bytesperline * .x2 loop ( curline )
  LINES over 1+ do ( curline )
    0 i at-xy i over - ."   +" bytesperline * .x2 loop drop ;
: redraw ( -- ) grid clear addresses hexlines asciilines ;
: jumpto ( a -- )
  3 invand dup to topleft 0 to bedpos buffer cellsperscreen move ;
: jumpby ( by -- ) topleft + bedpos swap jumpto to bedpos ;
: updatecursor ( -- )
  bedpos oldbedpos <> if
    bedpos doto oldbedpos swap | .ascii
    bedpos gridposascii INVERTEDCOLOR grid setcolor then
  bedpos gridposhex grid seek ;
: ?updatecursor ctxstate RUNNING = if updatecursor then ;
: movecursor ( by -- )
  bedpos tuck + max0 maxpos 1- min dup to bedpos ( old new )
  posline swap posline <> if addresses then ;
: movecursoral 1 inv doto bedpos and | movecursor ;
: zoom ( -- )
  posaddr dup cellsperscreen 2/ 4* - jumpto
  topleft - 2* to bedpos redraw ;
: statusline 0 LINES 1- dup grid clrline at-xy ;
: w ( n -- )
  bedpos 2 /mod buffer + dup c@ ( n f a c )
  rot if $f0 and rot or else $f and rot 4 lshift or then ( a c )
  swap c! bedpos 1 invand dup .hex .ascii 1 movecursor ;

: stoptyping ( -- ) RUNNING to ctxstate redraw ;
: dotype ( c -- )
  typebuf type1 if ?dup if processtyped execute then stoptyping then ;
: ?type1 ( -- f )
  ctxstate case
    TYPING = of evarg1 dotype 1 endof
    drop 0 endcase ;
: bottomtype ( xt -- ) \ xt: ( a u -- )
  to processtyped
  TYPING to ctxstate
  statusline ;
: :bottomtype> :> ['] bottomtype bind> ;

: |S LShift or ;

kvtbl[
'Q'    :> 0 LINES 1- at-xy stopcurrent ;
'H'    :> -2 movecursoral ;
'L'    :> 2 movecursoral ;
'J'    :> nibblesperline movecursoral ;
'K'    :> nibblesperline neg movecursoral ;
'['    :> bytesperpage neg jumpby redraw ;
']'    :> bytesperpage jumpby redraw ;
'Z'    ' zoom
'L' |S :> posaddr jumpto redraw ;
'J' |S :bottomtype> ['] n< rot> exec[] jumpto ;
'T' |S :bottomtype> bedpos 2/ buffer + swap 1- cmove ;
'W' |S :> buffer topleft cellsperscreen move redraw ;
'0' :> 0 w ;  '1' :> 1 w ;  '2' :> 2 w ;  '3' :> 3 w ;
'4' :> 4 w ;  '5' :> 5 w ;  '6' :> 6 w ;  '7' :> 7 w ;
'8' :> 8 w ;  '9' :> 9 w ;  'A' :> 10 w ; 'B' :> 11 w ;
'C' :> 12 w ; 'D' :> 13 w ; 'E' :> 14 w ; 'F' :> 15 w ;
]kvtbl handlers

:> ?type1 not if handlers evarg2 ?kvexec drop ?updatecursor then ;
KEYPRESS BEDApp sethandler

: bed$ ( -- )
  COLS 10 - 13 / dup to cellsperline not ?abort"grid too small"
  cellsperline 8* LINES * to maxpos
  topleft -1 = if here jumpto then ;
:> bed$ redraw updatecursor ;
INITIALIZE BEDApp sethandler
