needs lib/struct lib/coop
unit io/mouse

consts $01 BTN_LEFT $02 BTN_RIGHT $04 BTN_MIDDLE

struct Mouse {
  uint rawx rawy xmax ymax divisor buttons ;
  xt update ;
}

: x A! rawx A> divisor / ;
: y A! rawy A> divisor / ;
: xy bi x | y ;

: bounds ( mouse -- )
  r! rawx max0 r@ xmax 1- min r@ to rawx
  r@ rawy max0 r@ ymax 1- min r> to rawy ;

: moveto ( rawx rawy mouse -- ) A! rot> A> to rawy A> to rawx bounds ;
: moveby ( rawx rawy mouse -- )
  A! rot> A> doto rawy + | A> doto rawx + | bounds ;
: configuremouse ( scrw scrh divisor mouse -- )
  >A dup A> to divisor tuck * A> to ymax * A> to xmax ;

: newmouse ( 'update -- mouse ) here# 0 , 0 , 640 , 480 , 1 , 0 , swap , ;

' drop newmouse value mouse
: mouseupdate mouse update ;

addeventtype const MOUSEMOVE
addeventtype const MOUSECLICK
0 value oldbuttons
0 value oldx
0 value oldy
alias noop drawmousecursor
:> ( -- )
  mouse update
  mouse y doto oldy over | <> mouse x doto oldx over | <> or if
    MOUSEMOVE to evtype
    oldx to evarg1 oldy to evarg2 oldbuttons to evarg3 0 to evarg4
    rootdispatch evarg4 not if drawmousecursor then then ( )
  mouse buttons doto oldbuttons over | 2dup = if 2drop else ( btn old )
    swap mouse y mouse x ( old btn y x )
    MOUSECLICK to evtype to evarg1 to evarg2 to evarg3 to evarg4
    rootdispatch then ;
addgenerator

: evxy ( -- x y ) evarg1 evarg2 ;
:~ does> ( b ) bi evarg3 and bool | evarg4 and not and ;
BTN_LEFT ~ Ldown? BTN_RIGHT ~ Rdown? BTN_MIDDLE ~ Mdown?

:~ does> ( b ) bi evarg4 and bool | evarg3 and not and ;
BTN_LEFT ~ Lup? BTN_RIGHT ~ Rup? BTN_MIDDLE ~ Mup?
