needs lib/str mem/stack
unit lib/macro

$400 const MAXMACROSZ

\ _buf is where we do parameter expansion before pushing it to INPTR
create _buf MAXMACROSZ allot
_buf value _ptr
$30 newstack const _s

\ Macro arguments
create _args $100 allot \ A stringlist for args
0 value _argptr
0 value _argcnt
: _arg@ ( idx -- arg )
  dup _argcnt >= if abort"invalid macro argument" then
  _args slistiter ;

\ "macro" is a null-terminated string
: arg< ( -- str )
  wordorquote _argptr tuck strmove dup s) to _argptr doto _argcnt 1+ | ;
: _c, ( c -- ) doto _ptr dup 1+ | c! ;
: _s, ( str -- ) c@+ _ptr swap cmove+ to _ptr ;
: macro< ( macro -- a u )
  _args to _argptr 0 to _argcnt _ptr swap begin ( a m )
    c@+ ?dup while dup '%' = if drop c@+ case ( a m c )
      '%' = of '%' _c, endof
      '<' = of arg< _s, endof
      '-' = of arg< drop endof
      ( c ) '0' - _arg@ _s, endcase
    else ( a m c ) _c, then repeat ( a m )
  drop _ptr over - 0 doto _argptr dup 1+ | c! ;

: _next ( -- a u )
  _s pop NEXTIN< ! _s pop _s pop
  _s count not if _buf to _ptr then
  ?dup not if drop nextin< then ;
: injectrange ( a u -- )
  INSZ @! _s push INPTR @! _s push ['] _next NEXTIN< @! _s push ;

:~ macro< injectrange ;
: macro
  word NEXTWORD !
  here wordorquote c@+ cmoveallot 0 c, ['] ~ bind immediate ;

create _tbuf $100 allot

: times" ( n -- )
  0 _tbuf cmove" + c! in< drop
  r! 0 do _tbuf macro< loop
  r> 0 do injectrange loop ; immediate
: twice" 2 [compile] times" ; immediate
: thrice" 3 [compile] times" ; immediate
