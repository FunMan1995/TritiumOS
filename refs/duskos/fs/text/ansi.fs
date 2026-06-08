needs lib/struct lib/str mem/roll mem/kv mem/range io/stream io/grid
unit text/ansi

\ decode states
enum NOESC WAIT[ ARG1 ARG2

extends Grid struct ANSIGrid {
  uint esclog escstate arg1 arg2 ;
}

: inesc? ( grid -- f ) escstate NOESC <> ;
: beginesc ( grid -- ) WAIT[ over to escstate 0 over to arg1 0 swap to arg2 ;
: stopesc ( grid -- ) NOESC swap to escstate ;
: .esclog ( grid -- ) console swap esclog spit ;

: CUU r! posxy r@ arg1 1 max - max0 r> seekxy ;
: CUD r! posxy r@ arg1 1 max + r@ lines 1- min r> seekxy ;
: CUF bi arg1 1 max | seekrel ;
: CUB bi arg1 1 max neg | seekrel ;
: CUP r! arg2 1- max0 r@ arg1 1- max0 r> seekxy ;
: ED
  r! pos >r \ V1=grid V2=oldpos
  V1 arg1 case
    1 = of 0 V1 pos 1+ endof
    2 = of V1 pos V1 maxn endof
    drop 0 V1 size endcase ( pos n )
  swap V1 seek V1 32SPCS fillstream spitn
  r> r> to pos ;
: EL
  r! pos >r \ V1=grid V2=oldpos
  V1 arg1 case
    1 = of 0 V1 posxy drop 1+ endof
    2 = of 0 V1 columns endof
    drop V1 posxy drop V1 columns over - endcase ( posx n )
  swap V1 posxy nip V1 seekxy ( n )
  V1 32SPCS fillstream spitn
  r> r> to pos ;

: SGR
  dup arg1 case ( grid )
    0 = of resetcolor endof
    1 = of INVERTEDCOLOR swap to color endof
    30 - 8 < of dup color $f0 and 8 or r@ 30 - or swap to color endof
    40 - 8 < of dup color $0f and r@ 40 - 4 lshift or swap to color endof
    drop endcase ;

kvtbl[ ( grid -- )
'A' ' CUU
'B' ' CUD
'C' ' CUF
'D' ' CUB
'H' ' CUP
'J' ' ED
'K' ' EL
'L' :> bi arg1 1 max | insertlines ;
'M' :> bi arg1 1 max | deletelines ;
'f' ' CUP
'm' ' SGR
]kvtbl handlers
: do[ ( c grid -- ) dup stopesc handlers rot ?kvexec not if drop then ;

: ?digit ( c -- ?n f ) '0' - dup 10 < if 1 else drop 0 then ;
: ?accumulatearg ( c arg' -- ?c f )
  over ?digit if ( c arg' n )
    over @ 10 * + swap ! drop 1
    else ( c arg' ) drop 0 then ;
: ?do[ ( grid c -- ) tuck ':' - 6 >= if do[ else 2drop then ;

kvtbl[ ( c grid -- )
  WAIT[ :> swap '[' = if ARG1 swap to escstate else stopesc then ;
  ARG1  :>
    tuck offsetof arg1 + ?accumulatearg if drop else ( grid c )
      dup ';' = if drop ARG2 swap to escstate else ?do[ then then ;
  ARG2  :> tuck offsetof arg2 + ?accumulatearg if drop else ?do[ then ;
]kvtbl handlers
: decode ( c grid -- )
  >r dup c[] r@ esclog write drop ( c )
  r@ handlers r> escstate ?kvexec not if nip stopesc then ;

: :writebuf ( a n grid -- n )
  dup inesc? if nip swap c@ swap decode 1 else
    >r over c@ ESC = if 2drop r> beginesc 1 else
      2dup ESC rot> cidx if nip then ( a u )
      r> io/grid :writebuf then then ;

: newansigrid ( columns lines -- grid )
  $400 newrollingbuffer rot>
  newgrid >r ['] :writebuf r@ to writebuf
  ( esclog ) , NOESC , 0 , 0 , r> ;

create template "export TERM=ansi\rstty rows     columns     \r" s,
: ttycfgcmd ( -- str )
  LINES formatdec template 28 + swap cmove
  COLS formatdec template 40 + swap cmove
  template ;

\ encode logic
code ansirtype RTYPE @ bbr,
64 const MAXCSISZ
create csibuf MAXCSISZ allot

: rtypecsi ( a -- ) csibuf tuck - ansirtype ;
: CSI ( -- a ) '[' ESC csibuf c!+ c!+ ;

0 value lastpos
: spitpos ( pos -- )
  doto lastpos over | over 1- = if drop exit then ( pos )
  grid columns /mod ( x y )
  1+ formatdec CSI swap cmove+ ( x a )
  ';' swap c!+
  >r 1+ formatdec r> swap cmove+
  'H' swap c!+ rtypecsi ;

-1 value lastcolor
: spitcolor ( color -- )
  doto lastcolor over | over = if drop exit then ( color )
  dup DEFAULTCOLOR = if drop CSI 'm' swap c!+ rtypecsi exit then ( color )
  dup 7 and 30 + formatdec CSI swap cmove+ 'm' swap c!+ rtypecsi ( color )
  4 rshift 7 and 40 + formatdec CSI swap cmove+ 'm' swap c!+ rtypecsi ;

: refreshgrid ( -- )
  -2 to lastpos -1 to lastcolor
  0 begin
    grid getnextchange while ( color char pos )
    spitpos swap spitcolor c[] ansirtype
    lastpos repeat ;

: refreshcaret ( -- ) refreshgrid grid pos spitpos ;
