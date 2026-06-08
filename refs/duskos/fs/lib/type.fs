unit lib/type

: addtype n"TYPE" tag, code litn exit, ;
: _find n"TYPE" swap sysdict findtagged ;
: findtype _find dup if e>xt execute then ;

: flags @ ;
: .type dup 4+ @ execute ;
: typeszxt 8+ @ ;
: typesz dup typeszxt execute ;
: type@, 12 + @ execute ;
:~ abort"can't write to a &) type" ;
: type!, r! flags 3 and 3 = if drop ['] ~ bbr, rdrop else dir) r> type@, then ;

create _ ' 8b) , ' 16b) , ' 32b) , ' &) ,
: type) flags 3 and 4* _ + @ execute ;

: newtype 4 n,@ ;

: .int $10 + stype ;
: intsz flags 3 and 1+ dup 3 = if 1+ then ;
: int? typeszxt ['] intsz = ;
: intsigned? flags 256/ 1 and ;

: repeatword CURWORD @ NEXTWORD ! ;
: newint ( fetcherxt signed? width name -- )
  >r 1- dup 3 = if 1- then swap 256* or ( fetcherxt flags )
  ['] intsz ['] .int rot newtype r> s, ;

:~ word newint repeatword addtype ;
' @, 0 4 ~ uint
' @, 0 2 ~ ushort
' @, 0 1 ~ uchar
' @, 1 4 ~ int
' @, 1 2 ~ short
' @, 1 1 ~ char
' le@, 0 4 ~ leint
' le@, 0 2 ~ leshort
' be@, 0 4 ~ beint
' be@, 0 2 ~ beshort

: .void drop ."void" ;
: voidsz drop 0 ;
: void? typeszxt ['] voidsz = ;
: err abort"compiling access to void!" ;
' err ' voidsz ' .void 0 newtype addtype void

: pointersz drop 4 ;
: pointer? typeszxt ['] pointersz = ;
: reftype $10 + @ ;
: .pointer ."*" reftype .type ;
: newpointer ( type -- type )
  ['] @, ['] pointersz ['] .pointer 2 newtype swap , ;
uchar newpointer addtype String
void newpointer addtype AnyPtr

: arraycount $14 + @ ;
: arraysz bi reftype typesz | arraycount * ;
: array? typeszxt ['] arraysz = ;
: .array ."[" dup reftype .type ."," arraycount . ."]" ;
: newarray ( count type -- type )
  ['] @, ['] arraysz ['] .array 3 newtype >r , , r> ;

: .xt drop ."XT" ;
: xtsz drop 4 ;
: xt? typeszxt ['] xtsz = ;
: xt@, dup (dir? if @, else brr, then ;
' xt@, ' xtsz ' .xt 2 newtype addtype xt

: sigincnt flags 16 rshift $ff and ;
: sigoutcnt flags 24 rshift ;
: sigcounts bi sigincnt | sigoutcnt ;
: sigoutputs $10 + ;
: siginputs bi sigoutputs | sigoutcnt 4* + ;
: sigvarinput? flags $100 and bool ;
: sigvaroutput? flags $200 and bool ;
: .sig
  ."( " 0 over sigincnt do dup siginputs i 1- 4* + @ .type spc> 1 -loop
  ."-- " 0 over sigoutcnt do dup sigoutputs i 1- 4* + @ .type spc> 1 -loop
  .")" drop ;
: sigsz drop 4 ;
: signature? typeszxt ['] sigsz = ;
: flagsor! tuck @ or swap ! ;
: newsignature ( ... n ... n flags -- type )
  >r ['] @, ['] sigsz ['] .sig r> 2 or newtype >r ( ... n ... n V1=sig )
  dup 24 lshift V1 flagsor! n,@ drop ( ... n )
  dup 16 lshift V1 flagsor! n,@ drop r> ;

: typealign typesz 4 min dup 3 = if 1+ then align ;

create _ ,"], "
: acc< ( "..." -- str )
  newstr dup begin ( orig a )
    in< dup ':' = if 2drop dup toword# in< then ( orig a c )
    dup _ 3 cidx not while swap c!+ repeat ( orig a c idx )
  2drop nip endstr ;
: ?-- ( -- f )
  toword# in< '-' = dup if in< '-' <> ?abort"-- expected" else stepback then ;
: ?... ( -- f )
  toword# in< '.' = dup if
    word ".." s= not ?abort"... expected" else stepback then ;
: type< ( "..." -- type )
  toword# in< case
    '*' = of type< newpointer endof
    '[' = of
      type< acc< dup parse if nip else sysdict find# execute then
      swap newarray endof
    '(' = of
       ?... 256* >r \ V2=flags
       0 begin ?-- not while type< swap 1+ repeat
       ?... if r> $200 or >r then
       0 begin toword# in< ')' <> while stepback type< swap 1+ repeat
       r> newsignature endof
     drop stepback acc< findtype ?wnf endcase ;

