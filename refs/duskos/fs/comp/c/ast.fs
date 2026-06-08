needs lib/str mem/cons comp/c/tok
unit comp/c/ast

stringlist ops
  * / % + - << >> < > <= >= == != & ^ | \
  && || ? = += -= *= /= %= <<= >>= &= ^= |= \
  ++ -- ~ ! neg ref deref \
  sizeof funcall array typecast \
  postinc postdec -> ?: \
  lit str sym

ops slistlen const OPERATORCNT

:~ [rcompile] " ops sfind not ?abort"operator not found" ;
:> ~ litn ;
' ~ compiling op"
: opname ops slistiter dup c@ not ?abort"wrong opid" ;

alias abort .ast ( ast -- )
: .list ( ast -- ) begin cdrcar .ast ?dup while spc> repeat ;
:realias .ast ( ast -- )
  '(' emit carcdr over opname stype spc> ( car cdr ) swap case
    op"lit" = of .x endof
    op"str" = of stype endof
    op"sym" = of stype endof
    op"funcall" = of .list endof
    op"array" = of ?dup if .list then endof
    op"typecast" = of cdrcar stype spc> cdrcar . spc> .ast endof
    op"sizeof" = of stype endof
    op"?:" = of cdrcar .ast spc> cdrcar .ast spc> .ast endof
    op"->" = of cdrcar stype spc> .ast endof
    op"++" >= of .ast endof
    drop cdrcar .ast spc> .ast endcase
  ')' emit ;

alias abort tokast< ( tok -- ast ) \ Forward declaration
alias abort left< ( tok -- ast ) \ forward declaration

: ast< tok< tokast< ;

: mk-> ( ast str -- ast ) swap cons op"->" swap cons ;

stringlist postfixes ++ --
: ?postfix< ( ast -- ast ) \ maybe add postfix operator
  ?tok< case
    not of endof
    '[' isChar? of
      ast< cons ']' readChar
      op"+" swap cons op"deref" swap cons ?postfix< endof
    '(' isChar? of ( funsym )
      0 cons ')' readChar? not if ( arglist )
        dup ast< begin append ',' readChar? while ast< repeat read) drop then
      op"funcall" swap cons endof
    "->" s= of tok< mk-> ?postfix< endof
    '.' isChar? of op"ref" swap cons tok< mk-> ?postfix< endof
    postfixes sfind of ( ast idx ) op"postinc" + swap cons ?postfix< endof
    ( ast tok ) drop tokstepback endcase ;

: binop? ( opid -- f ) op"|=" <= ;

create pri map< c, 0 0 0 1 1 2 2 3 3 3 3 4 4 5 6 7 \
                   8 9 11 11 11 11 11 11 11 11 11 11 11 11
: pri@ ( opid -- pri ) pri + c@ ;

: ?binary ( tok -- ?id f )
  ops sfind if dup binop? ?dup not if drop 0 then else 0 then ;

: ?ternary< ( ast -- ast )
  dup car op"?" = if
    cdr carcdr ':' readChar ast< cons cons op"?:" swap cons then ;

: rightwins? ( opleft opright -- f )
  pri@ swap pri@ swap
  2dup = if drop 11 = ( pri11 is rassoc ) else > then ;

: binary< ( ast opid -- ast )
  >r tok< left< tok< ?binary not if
    tokstepback cons r> swap cons ?ternary< exit then ( l r otherid ) \ V1=opid
  r@ over rightwins? if
    binary< cons r> swap cons
    else rot> cons r> swap cons swap binary< then ?ternary< ;

: right< ( ast -- ast ) \ binary op or postfix
  ?postfix< ?tok< ?binary if binary< else tokstepback then ;

: mklit ( n -- ) op"lit" swap cons ;
: mksym ( str -- ) op"sym" swap cons ;
: mktypecast ( ident lvl ast -- ast ) cons cons op"typecast" swap cons ;

\ if "ast" is a symbol, check whether all we have following us is either "*"
\ characters ending with a closing ")". If not, do nothing. If it's a typecast,
\ yield a typecast AST. If it's ambiguous multiply, "manually" yield the
\ multiply.
:~ ( tok -- ast ) left< 0 swap mktypecast ;
: ?typecast ( ident -- ast )
  '(' readChar? if "(" ~ else
    '`' readChar? if "`" ~ else
      readIdent? if ~ else mksym then then then ;
: handle( ( -- ast )
  '(' readChar? if handle( right< read) exit then
  readIdent? not if ast< read) else ( tok )
    ')' readChar? if ?typecast else
      '*' readChar? not if tokast< read) else
        ')' readChar? if 1 tok< left< mktypecast else
          '*' readChar? if
            2 begin '*' readChar? while 1+ repeat read) ast< mktypecast
            else mksym op"*" binary< read) right< then
         then then then then ;

: sizeof( ( -- ast )
  read( tok< expectIdent '*' readChar? if
    drop begin '*' readChar? not until 4 mklit
    else ( str ) op"sizeof" swap cons then read) ;

stringlist prefixes ++ -- ~ ! - & *
: pre< ( tok -- ast )
  case
    '(' isChar? of handle( endof
    '{' isChar? of ( )
      '}' readChar? if 0 else
        ast< 0 cons dup begin ',' readChar? while ast< append repeat
        drop '}' readChar then
      op"array" swap cons endof
    '"' isChar? of
      [rcompile] " ( str )
      in< '0' = if c@+ 2dup + 0 swap c! 1+
        else stepback dup c@ 1+ then ( a u )
      parena1@ cmoveallot op"str" swap cons endof
    "sizeof" s= of sizeof( endof
    prefixes sfind of op"++" + tok< left< cons endof
    '`' isChar? of word mksym endof
    isIdent? of r@ mksym endof
    expectConst mklit endcase ;

:realias left< pre< ?postfix< ;
:realias tokast< pre< right< ;
