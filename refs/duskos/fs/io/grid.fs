needs lib/struct lib/str mem/reuse mem/range io/stream mem/stream
unit io/grid

extends MemStream struct Grid {
  uint columns lines color dirty pixw pixh ;
}

enum BLACK BLUE  GREEN  CYAN  RED  MAGENTA  BROWN  LGRAY \
     DGRAY LBLUE LGREEN LCYAN LRED LMAGENTA YELLOW WHITE

: joinfgbg ( fg bg -- ) 16* or ;
WHITE BLACK joinfgbg value DEFAULTCOLOR
BLACK WHITE joinfgbg value INVERTEDCOLOR
8 const TABSTOP

: colorptr bi bufptr | size + ;
: mirrorptr bi bufptr | size 2* + ;
: mirrorcolorptr bi colorptr | size 2* + ;
: colorrange bi colorptr | size ;
: posxy ( grid -- x y ) bi pos | columns /mod ;
: xypos ( x y grid -- pos ) columns * + ;
: xypix tuck pixh / rot> pixw / swap ;
: xypixpos r! xypix r> xypos ;
: seekxy r! xypos r> seek ;
: xyptr ( x y grid -- a ) r! xypos r> bufptr + ;

: line[] ( idx grid -- a u )
  0 rot> r! xyptr ( a V1=grid )
  dup r> columns + begin ( aref a )
    2dup < while
    1- dup c@ ws? while repeat
    1+ else then ( aref a )
  over - ;

: clrline ( idx grid -- )
  r! columns * ( pos V1=grid )
  r@ bufptr over + r@ columns SPC cfill
  r@ colorptr + r@ columns DEFAULTCOLOR cfill
  1 r> to dirty ;

: copyline ( srcidx dstidx grid -- )
  dup bufptr >r dup colorptr >r columns >r ( src dst V1=buf V2=cbuf V3=len )
  V3 * swap V3 * swap
  over V1 + over V1 + V3 cmove
  V2 + swap V2 + swap V3 cmove
  2rdrop rdrop ;

: scrollup ( grid -- )
  r! lines 1- 0 do i 1+ i V1 copyline loop ( V1=grid )
  r@ lines 1- r@ clrline
  0 r@ lines 1- r> seekxy ;
: seekxyscroll r! seekxy r@ pos r@ size = if r@ scrollup then rdrop ;

: bol ( grid -- ) dup posxy nip 0 swap rot seekxy ;
: insertlines ( n grid -- )
  r! lines over - V1 posxy nip do ( n V1=grid )
    i over + i swap V1 copyline loop
  dup 0 do V1 posxy nip i + V1 clrline loop drop
  r> bol ;

: deletelines ( n grid -- )
  r! lines over - V1 posxy nip do ( n V1=grid )
    i over + i V1 copyline loop
  dup 0 do V1 lines 1- i - V1 clrline loop drop
  r> bol ;

: ctlidx ( a u -- ?i f )
  0 do dup i + c@ SPC < if i break then loop
  broke? if nip 1 else drop 0 then ;

: ctlhandle ( c grid -- )
  swap case ( grid )
    BS = of dup rewind1 SPC over putc rewind1 endof
    LF = of dup posxy nip 1+ 0 swap rot seekxyscroll endof
    CR = of dup posxy nip 0 swap rot seekxyscroll endof
    9 = of dup posxy dip TABSTOP / 1+ TABSTOP * | rot seekxy endof
    2drop endcase ;

: setcolor[] ( pos n col grid -- )
  1 over to dirty
  swap >r colorptr rot + swap r> cfill ;
: setcolor ( pos col grid -- ) 1 over to dirty colorptr rot + c! ;
: getcolor ( pos grid -- col ) colorptr + c@ ;
: getchar ( pos grid -- c ) bufptr + c@ ;

: :writebuf ( a n st -- n )
  over not if drop nip exit then
  rot dup c@ SPC < if c@ rot> nip ctlhandle 1 exit then ( n st a )
  rot 2dup ctlidx if nip then ( st a n )
  rot dup bi pos | size = if dup scrollup then
  r! r@ pos >r mem/stream :writebuf r> ( n oldpos )
  over r@ color r> setcolor[] ;

: resetcolor DEFAULTCOLOR swap to color ;
: fillcolor ( col grid -- ) colorrange rot cfill ;
: alldirty ( grid -- ) 1 over to dirty bi mirrorptr | size 2* 0 cfill ;
: clear ( grid -- )
  r! range SPC cfill
  r@ resetcolor DEFAULTCOLOR r@ fillcolor
  0 r@ seek r> alldirty ;
: newgrid ( columns lines -- grid )
  2dup * 4* dup ?reuse ( cols lines sz a )
  swap 4/ newmemstream >r ( cols lines )
  2>r 1 1 0 DEFAULTCOLOR 2r> swap 6 n,@ drop
  ['] :writebuf r@ to writebuf
  r@ clear r> ;
: resize ( columns lines grid -- )
  >r dup r@ to lines over r@ to columns
  * dup r@ to size ( sz )
  4* r@ bufptr swap ?realloc r@ to bufptr
  r> clear ;

: eq? ( a A=grid -- f ) dup c@ swap A> size 2* + c@ = ;
: getnextchange ( frompos grid -- ?color ?char ?pos f )
  A! dirty not if drop 0 exit then
  A> bufptr + A> size >r begin ( a A=grid V1=size )
    dup A> bufptr V1 + < while
    dup eq? over V1 + eq? and while
    1+ repeat ( a )
    dup A> bufptr - ( a pos )
    over dup c@ tuck swap V1 2* + c! ( a pos char )
    rot V1 + dup c@ tuck swap V1 2* + c! ( pos char color )
    swap rot 1
    else 0 A> to dirty drop 0 then rdrop ;

8 4 newgrid value grid
: COLS grid columns ;
: LINES grid lines ;
: cell! ( c pos -- ) grid seek grid putc ;
: at-xy ( x y -- ) grid seekxy ;

: ?showmarker ( color newpos 'updxt 'oldpos -- )
  swap >r tuck @ 2dup = if nip nip else ( col 'old new old V1=updxt )
    dup grid size >= if drop else
      dup grid getcolor swap V1 execute then ( col 'old new )
      dup rot ! then ( col new )
  dup grid size >= if 2drop else ( col new ) V1 execute then rdrop ;

: showruler
  grid pos >r
  0 0 at-xy COLS grid "0123456789" c@+ fillstream spitn
  LINES 1 do COLS 1- i at-xy i 10 mod . loop
  COLS 10 / 0 do i 10 * INVERTEDCOLOR grid setcolor loop
  LINES 10 / 0 do i 1+ 10 * 1+ COLS * 1- INVERTEDCOLOR grid setcolor loop
  r> grid to pos ;
: showcolors $100 0 do i grid to color 'X' grid putc loop grid resetcolor ;
