needs lib/str lib/coop lib/ival io/typeln io/kbd io/mouse io/grid lib/psrs \
      mem/range mem/kv fs/utag text/ed
unit app/ed

20 const PAGESZ

enum Normal Insert InsertLine Replace
Normal value edmode
: .edmode [ 'e Replace litn ] 3 edmode - 0 do @ loop entryname[] rtype ;

: height LINES 1- ;

createapplication EDApp
context CONTEXTSZ ivalmapfrom {
  uint prevbuf ;
  xt processtyped ;
  uint typebuf ;
}

2 const TYPING \ for context state
3 const SELBUF \ for CTRL+B
4 const SELLINE \ for 'g'
5 const SELROW  \ for 'g'

: nspcs grid 32SPCS fillstream spitn ;
: spitline ( lineidx -- )
  0 over top - at-xy
  line line[] COLS min tuck grid write# ( u )
  COLS swap- max0 nspcs ;

: spitpage ( -- )
  height 0 do ( )
    0 i at-xy
    top i + dup linecnt < if ( idx )
      spitline else drop COLS nspcs then loop ;

: drawgutter ( -- )
  gutter COLS < if
    gutter height 0 do ( pos )
      dup INVERTEDCOLOR grid setcolor COLS + loop drop then ;

: normalcolor ( -- ) DEFAULTCOLOR grid fillcolor ;
: edpos>gridpos ( pos -- pos ) bi cpos | lpos top - COLS * + ;
: highlightsel ( -- )
  visualmode not if exit then
  mark epos ?swap ( lo hi )
  dip edpos>gridpos max0 | edpos>gridpos over - ( pos n )
  dup 0< if 2drop else INVERTEDCOLOR grid setcolor[] then ;

: statusclr height grid clrline ;
: statusline statusclr 0 height grid seekxy ;
: statuspos
  statusline .edmode spc> epos dup lpos 1+ .
  ."," cpos . ." " linecnt . ;
: edmode! to edmode statuspos ;

: pagerefresh normalcolor spitpage highlightsel drawgutter ;
: fullrefresh pagerefresh statuspos ;
: top! ( lineno -- ) dup doto top swap | <> if pagerefresh then ;

: reframe ( -- )
  top epos lpos tuck > if top! else ( lpos )
    height 1- - max0 top over <= if top! else drop then then ;

: displaypos ( -- )
  ctxstate RUNNING <> if exit then
  reframe epos bi cpos | lpos top - grid seekxy ;
: saveprevbuf curbuf @ to prevbuf ;

create alphapos alphacnt 4* allot0
0 value lastcnt

: .selline ( -- )
  INVERTEDCOLOR grid to color
  alphanum[] height min ( a u ) 0 do ( a )
    dup i + c@ 0 i grid xypos cell! loop
  drop grid resetcolor ;
: .selrow ( -- )
  INVERTEDCOLOR grid to color
  curline line[] words[] ps[] ( ... a u )
  2dup swap[] r! alphapos swap alphacnt min r! move ( ... V1=dropcnt V2=cnt )
  r> to lastcnt r> ndrop ( )
  epos lpos top - lastcnt 0 do ( y )
    i 4* alphapos + @ over grid xypos alphanum i + c@ swap cell! loop drop
  grid resetcolor ;

\ delete selection if visual mode
: visualdelete ( -- deleted? )
  visualmode if
    0 to visualmode mark delto 1 else 0 then ;

\ delete selection and/or next char depending on mode
: write1 ( c -- )
  visualdelete drop
  edmode Replace = if replchar 1 goright else edstream putc then
  fullrefresh ;

: shifted evarg2 LShift and ;
\ enable/disable visual mode depending on shift state
: shiftmove
  edmode Normal <> if shifted if
    visualmode not if
      1 to visualmode epos to mark then
  else 0 to visualmode pagerefresh
  then then ;

: moved visualmode if pagerefresh then statuspos ;
:~ ( xt -- ) shiftmove execute moved ;
: :move> :> ['] ~ bind> ;

: zsel edbufs @ curbuf ! ;
: stoptyping ( -- ) RUNNING to ctxstate displaypos ;
: selbuf ( c -- )
  c>edbuf ?dup if saveprevbuf curbuf ! then fullrefresh stoptyping ;
: selline ( c -- )
  alphanum[] cidx if top + go moved then
  .selrow SELROW to ctxstate ;
: selrow ( c -- )
  alphanum[] cidx if dup lastcnt >= if drop else
    4* alphapos + @ cpos! then then
  fullrefresh stoptyping ;
: dotype ( c -- )
  typebuf type1 if ?dup if 1- processtyped then stoptyping then ;
: ?type1 ( -- f )
  ctxstate case
    TYPING = of evarg1 dotype 1 endof
    SELBUF = of evarg1 selbuf 1 endof
    SELLINE = of evarg1 selline 1 endof
    SELROW = of evarg1 selrow 1 endof
    drop 0 endcase ;
: bottomtype ( xt -- ) \ xt: ( a u -- )
  to processtyped
  TYPING to ctxstate
  statusline ;
: :bottomtype> :> ['] bottomtype bind> ;

: textsel[] ( -- a u )
  visualmode if mark clipto clipboard[] else 0 0 then ;
: zoom ( -- ) epos lpos height 2/ - max0 top! moved ;
: setmark epos to mark ;
: ?gedload ( path -- ) saveprevbuf ?edload fullrefresh ;

: |S LShift or ;
: |C LControl or ;
: V! doto visualmode 1 | if pagerefresh else setmark then ;
: |Sdup over |S over ;

kvtbl[
'Q'    :> statusline stopcurrent ;
'H'    :move> 1 goleft ;
ArrowLeft over |Sdup
'J'    :move> 1 godown ;
ArrowDown over |Sdup
'K'    :move> 1 goup ;
ArrowUp over |Sdup
'L'    :move> 1 goright ;
ArrowRight over |Sdup
'J' |S :move> bol V! 1 godown ;
'K' |S :move> bol V! 1 goup ;
'H' |S :move> prevword ;
ArrowLeft |C over |Sdup
'L' |S :move> nextword ;
ArrowRight |C over |Sdup
'H' |C :move> bol ;
Home over |Sdup
'L' |C :move> bol COLS 1- goright ;
End over |Sdup
'['    :move> PAGESZ goup ;
PageUp over |Sdup
']'    :move> PAGESZ godown ;
PageDown over |Sdup
Home |C :move> 0 go bol ;
|Sdup
End  |C :move> linecnt go eol ;
|Sdup
CR :>
  edmode case
    Normal = of Insert edmode! endof
    Insert = of LF write1 endof
    drop Normal edmode! endcase ;
BS :>
  visualdelete not if
    epos cpos if 1 goleft 1 delchars else
      epos if 1 goup eol jl then then then
  fullrefresh ;
Delete |S :> visualmode if mark clipto visualdelete drop then ;
Delete :>
  visualdelete not if poseol? if jl else 1 delchars then then fullrefresh ;
'G'    :> .selline SELLINE to ctxstate ;
'G' |S :bottomtype> ?[] if ['] n< rot> exec[] 1- max0 go moved then ;
'R'    :> Replace edmode! ;
'I'    :> InsertLine edmode! ;
ESC    :> Normal edmode! ;
io/kbd Insert :> edmode Insert = if Replace else Insert then edmode! ;
'F'    :bottomtype> ?[] if []>str edfind moved then ;
'N'    :> edfindnext zoom ;
'O'    :> appendline fullrefresh InsertLine edmode! ;
'O' |S :> insertline fullrefresh InsertLine edmode! ;
'\'    :> jl fullrefresh ;
'\' |S :> sl fullrefresh ;
'Z'    ' zoom
'Z' |S :> epos lpos top! moved ;
'M'    ' setmark
'M' |S :> epos doto mark swap | to epos zoom ;
:~  doto visualmode 1 xor | visualmode ;
'V'    :> ~ if setmark else normalcolor then pagerefresh ;
'V' |S :> ~ not if normalcolor then pagerefresh ;
'Y'    :> visualmode if mark clipto then ;
io/kbd Insert |C over
'P'    :> visualdelete drop clipboard[] edstream write# fullrefresh ;
io/kbd Insert |S over
'D' |S :> doto visualmode 0 | if mark delto fullrefresh then ;
'X' |S :> 1 delchars fullrefresh ;
';' |S :bottomtype> ?[] if []>pool statusline interpret[] pagerefresh then ;
'B' |C :> statusline .edbufs SELBUF to ctxstate ;
'Z' |C :> saveprevbuf zsel fullrefresh ;
']' |C :> saveprevbuf nextedbuf fullrefresh ;
'[' |C :> prevbuf ?dup if curbuf @! to prevbuf fullrefresh then ;
'Q' |C :> empty nextedbuf fullrefresh ;
'W' |S :> statusline edsave ;
'W' |C over
'E' |C :>
  wordundercursor ?[] if
    []>str utag>path dup lookup if ?gedload then then ;
'D' |C :>
  wordundercursor ?[] if
    []>str "\n" over strcat findstr strmove
    utag>.txt dup lookup if ?gedload then then ;
]kvtbl handlers

: handlekey ( -- )
  ?type1 if exit then
  edmode Normal <> evarg1 SPC >= and if evarg1 write1 displaypos else
    handlers evarg2 ?kvexec if displaypos then then ;
current KEYPRESS EDApp sethandler

: evedpos ( -- edpos )
  evxy grid xypix ( x y )
  top + swap bounds ;
: V! ( f -- ) to visualmode pagerefresh ;
:> ( -- )
  ctxstate RUNNING <> if exit then
  Ldown? if evedpos to epos 0 V! then
  Rdown? if evedpos to mark 0 V! then
  Rup? if
    evedpos to epos
    epos mark <> if 1 V! then then
  displaypos promptforkey ;
MOUSECLICK EDApp sethandler

: ed$ grid clear fullrefresh displaypos ;
:> ed$ ; INITIALIZE EDApp sethandler

: newedctx ( app -- ctx ) COLS newtypebuf >r newcontext 0 , ['] 2drop , r> , ;
EDApp newedctx const edctx \ TODO: multi-task
: ed edctx launchcontext ;
