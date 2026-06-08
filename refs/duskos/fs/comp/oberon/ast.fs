needs lib/str mem/cons comp/oberon/tok
unit comp/oberon/ast

stringlist ops
  lit char str ident ` \
  neg ~ ^ \
  [] . () "{}" .. IS \
  * / DIV MOD & OR + - = # < <= > >= IN

ops slistlen const OPERATORCNT

:~ [rcompile] " ops sfind not ?err"operator not found" ;
:> ~ litn ;
' ~ compiling op"
: opname ops slistiter dup c@ not ?err"wrong opid" ;

: unary? ( id -- f ) op"neg" op"^" within? ;
: binary? ( id -- f ) op"*" op"IN" within? ;
:~ cdrcar op"ident" <> ?err"ident expected" ;
: ident# ( ast -- modname-or-0 name )
  dup car op"." = if cdr carcdr ~ swap else ~ 0 swap then ;

create pri map< c, 0 0 0 0 0 1 1 1 2 2 2 2 2 2 2
: pri@ ( id -- pri )
  dup binary? not ?err"not a binop"
  op"*" - pri + c@ ;

alias abort .ast ( ast -- )
: .pair ( ast -- ) cdrcar .ast spc> .ast ;
: .list ( ast -- ) begin cdrcar .ast ?dup while spc> repeat ;
:realias .ast ( ast -- )
  '(' emit carcdr over opname stype spc> ( car cdr ) swap case
    op"lit" = of .x endof
    op"char" = of .x1 endof
    op"str" = of stype endof
    op"ident" = of stype endof
    op"`" = of stype endof
    op"()" = of .list endof
    op"[]" = of .pair endof
    op"." = of cdrcar stype spc> .ast endof
    op"{}" = of ?dup if .list then endof
    op".." = of .pair endof
    op"IS" = of cdrcar ?dup if stype spc> then cdrcar stype spc> .ast endof
    unary? of .ast endof
    binary? of .pair endof
    err"unknown AST" endcase
  ')' emit ;

: mklit ( n -- ) op"lit" swap cons ;
: mkchar ( n -- ) op"char" swap cons ;
: mkident ( str -- ) op"ident" swap cons ;
: mk. ( ast str -- ast ) swap cons op"." swap cons ;

alias abort expr< ( -- ast ) \ forward declaration
alias abort left< ( -- ast ) \ forward declaration

: ?postfix< ( ast -- ast ) \ maybe add postfix operator
  ?tok< case
    not of endof
    '^' isChar? of op"^" swap cons endof
    '[' isChar? of
      expr< cons ']' readChar op"[]" swap cons ?postfix< endof
    '(' isChar? of
      0 cons ')' readChar? not if ( arglist )
        dup expr< begin append ',' readChar? while expr< repeat read) drop then
      op"()" swap cons ?postfix< endof
    '.' isChar? of tok< mk. ?postfix< endof
    ( ast tok ) drop tokstepback endcase ;

: ?binary ( tok -- ?id f )
  ops sfind if dup binary? ?dup not if drop 0 then else 0 then ;

: rightwins? ( opleft opright -- f ) pri@ swap pri@ swap > ;

\ example: a+b*c<d. '<' has the lowest pri, but is rightmost
\ left is (a), right is (< (* (b) (c)) (d)) and opid is +
\ we want (< (+ (a) (* (b) (c))) (d))
: retreebinop ( left right opid -- ast )
  >r swap >r ( right ) \ V1=opid V2=left
  carcdr cdrcar ( ropid rright rleft )
  r> swap cons r> swap cons ( ropid rright newleft )
  swap cons cons ;

: binary< ( ast opid -- ast )
  >r left< tok< ?binary not if ( left right ) \ V1=opid
    tokstepback cons r> swap cons exit then ( l r otherid )
  r@ over rightwins? if
    binary< r@ over car rightwins? if
      cons r> swap cons else r> retreebinop then
    else rot> cons r> swap cons swap binary< then ;

: is< ( ast -- ast )
  readIdent '.' readChar? if readIdent else 0 swap then ( ast mod type )
  rot cons cons op"IS" swap cons ;

: right< ( ast -- ast )
  ?tok< case
    ?binary of binary< endof
    "IS" s= of is< endof
    drop tokstepback endcase ;

: designator< ( -- ast )
  '`' readChar? if op"`" word cons else readIdent mkident then ?postfix< ;

: set< ( -- ast ) \ opening '{' is read already
  op"{}" 0 cons '}' readChar? if exit then dup begin ( head tail )
    expr< tok< ".." s= not if tokstepback else
       expr< cons op".." swap cons then ( head list expr )
    append ',' readChar? not until ( head tail )
  drop '}' readChar ;

: strlit< ( -- ast ) op"str" [rcompile] " cons ;
: chrlit< in< mkchar in< ''' <> ?err"invalid char literal" ;
:realias left<
  tok< case
    '(' isChar? of expr< read) endof
    '{' isChar? of set< endof
    '"' isChar? of strlit< endof
    ''' isChar? of chrlit< endof
    '+' isChar? of left< endof
    '-' isChar? of op"neg" left< cons endof
    '~' isChar? of op"~" left< cons endof
    isHex? of ( n suffix )
      'X' = if mkchar else mklit then endof
    parse of ( n ) mklit endof
    drop tokstepback designator< endcase ;

:realias expr< left< right< ;
