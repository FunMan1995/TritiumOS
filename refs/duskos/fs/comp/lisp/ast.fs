needs mem/cons comp/tok comp/lisp/env comp/lisp/sig
unit comp/lisp/ast

\ All AST nodes are pairs ( type . data ), with types being:
0 const NUMBER   \ a straight number. can be a quoted cons
1 const CALLABLE \ address to a callable
2 const PSOFF    \ offset, in cells, from PSP

\ The types below point to lists. When we refer to "nodes", it will be other
\ AST node pairs, but otherwise, it's straight values.
3 const FUNCALL  \ ( callnode [argsnodes] )
4 const LAMBDA   \ ( name-or-0 argcnt depth [exprnodes] )
5 const LET      \ ( initnodes [exprnodes] )
6 const TO       \ ( w exprnode )

\ Small note in case one is ever tempted to move psoff management in AST (thus
\ have PSOFF values be adjusted depending on their position in the arg list). It
\ looks like a good idea, simpler than doing it during compilation, but don't
\ forget one fundamental showstopper: this means macros can't modify the AST
\ anymore. For example, a macro that would swap two arguments would utterly
\ break the AST.

\ argcnt and depth
\ argcnt is the number of arguments in the lambda. depth is the maximum PS depth
\ of arguments referenced in lambda's body. In a regular lambda, depth is equal
\ (or even smaller) than argcnt. In a lambda generator, depth is greater. In:
\ (lambda (x) (lambda (y) (+ x y)))
\ outer argcnt=1 depth=1 and inner argcnt=1 depth=2.

: symchar? ( c -- f ) "()'\"" c@+ cidx dup if nip then ;
: _?tok< ( -- tok-or-0 )
  begin tonws< nip ( c )
    dup '\' = while drop [compile] \ repeat ( c )
  newtok dup tokacc symchar? not if begin
    in< dup ws? not while dup symchar? not while tokacc repeat
    ( c ) stepback then ( c ) drop then
  curtokcopy ;
: ast$ tok$ ['] _?tok< ['] (?tok<) realias ;

:~ ( c -- ?tok f ) tok< tuck swap isChar? dup if nip then ;
: ?read( '(' ~ ; : ?read) ')' ~ ;
:~ ( c -- ) tok< over isChar? not if emit abort" expected" else drop then ;
: read( '(' ~ ; : read) ')' ~ ;

\ Some lisp builtin have name that clash with xcomp/boot. Add "(" prefixes.
stringlist _ if to
: hasprefix? ( s -- f ) _ sfind dup if nip then ;

: findsymbol ( s -- w )
  dup hasprefix? if "(" swap strcat then
  sysdict find# ;
: ?lit ( tok -- n-or-tok f )
  dup '"' isChar? if
    drop [rcompile] " 1 else dup parse if nip 1 else 0 then then ;
: quoteatom ( tok -- res ) ?lit not if findsymbol then ;

: list< ( w -- )
  >r 0 0 cons dup begin ( res tail ) \ V1=w
    ?read) not while ( res tail tok )
    dup '.' isChar? if drop car tok< r> execute cons nip read) exit then
    r@ execute append repeat ( res tail )
  drop cdr rdrop ;
alias abort quote<
:~ ( tok -- elem )
  dup '(' isChar? if drop ['] ~ list< else quoteatom then ;
:realias quote< ( -- ast ) tok< ~ NUMBER swap cons ;

alias abort ast< ( -- ast )

0 value maxdepth
: maxdepth! ( n -- ) doto maxdepth max | ;

: args< ( -- cnt ) 0 read( begin ?read) not while envadd 1+ repeat ;
: expr< ( tok -- ast ) drop tokstepback ast< ;
: exprs< ['] expr< list< ;
:~ ( name -- ast )
  envtail >r doto maxdepth 0 | >r
  LAMBDA swap args< exprs< ( LAMBDA name argcnt exprnodes )
  r> doto maxdepth swap | swap r> to envtail
  cons cons cons cons ;
: lambda 0 ~ ;
1 parser current addsig
: defun tok< ~ ;
1 parser current addsig
:~ ( tok -- ast ) drop tokstepback read( tok< envadd tok< expr< read) ;
: let
  envtail >r doto maxdepth 0 | >r
  LET 0 cons r! read( ['] ~ list< append
  exprs< swap to cdr r>
  r> to maxdepth r> to envtail ;
1 parser current addsig
: (to TO to' tok< expr< cons cons read) ;
1 parser current addsig

: single< ( tok -- ast )
  dup ''' isChar? if drop quote< else
    ?local if dup 1+ maxdepth! PSOFF else
      ?lit if NUMBER else findsymbol CALLABLE then then
    swap cons then ;

:~ ( tok -- elem ) drop tokstepback ast< ;
: funcall< ( -- ast )
  tok< dup '(' isChar? if ~ else single< then
  dup carcdr ?parser swap CALLABLE = and if cdr execute else
    ['] ~ list< cons FUNCALL swap cons then ;
:realias ast< ( -- ast ) ?read( if funcall< else single< then ;
