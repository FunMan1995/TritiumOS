needs lib/macro mem/reuse
unit text/clip

$400 ?reuse value clipboard
variable clipboardlen

: clipboard[] clipboard clipboardlen @ ;

: clipclear 0 clipboardlen ! ;

: clipensure ( n -- ) clipboard swap ?realloc to clipboard ;

: clipset ( n -- a ) dup clipensure clipboardlen ! clipboard ;

: _rtype ( a u -- )
  clipboardlen @ over + clipensure
  r! clipboard clipboardlen + swap cmove r> clipboardlen +! ;

variable _old
: clip[ _old @ not if ['] _rtype RTYPE @! _old ! then ;

: ]clip _old @ if 0 _old @! RTYPE ! then ;

: cliprun clipboard[] injectrange ;

: cliprun" ( "prefix" -- ) [rcompile] " c@+ cliprun injectrange ;
