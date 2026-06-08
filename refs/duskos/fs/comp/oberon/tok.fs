needs lib/str lib/match comp/tok
unit comp/oberon/tok

\ '"' and ''' are in there because we parse them in ast.fs
create symbols "+-*/~&.,;|()[]{}:^=#<>.'`\"" s,
: sym1? ( c -- f ) symbols c@+ cidx dup if nip then ;
create with= ":<>" s,
: sym2? ( c -- ?c f )
  case
    '.' = of in< dup '.' = if 1 else drop stepback 0 then endof
    with= c@+ cidx of ( idx )
      drop in< dup '=' = if 1 else drop stepback 0 then endof
    drop 0 endcase ;
: digit? ( c -- f ) rmatch"09" ;
: letter? ( c -- f ) rmatch"AZaz" ;
: identifier? ( c -- f ) rmatch"09AZaz" ;
: _in< in< ?line+ ;
: ?comment ( c -- c )
  dup '(' = if
    in< '*' <> if stepback else
      drop _in< begin ( c ) dup EOF <> while ( c )
        ?comment '*' <> if _in< 1 else
          _in< dup ')' = if drop _in< 0 else 1 then then ( c f )
      while repeat then
      dup ws? if drop tonws< nip ?comment then
  then then ( c ) ;

: expectConst ( tok -- n ) dup parse if nip else err"constant expected" then ;
: isIdent? ( tok -- f )
  dup 1+ c@ letter? not if drop 0 exit then
  c@+ 0 do ( a ) c@+ identifier? not if break then loop drop broke? not ;
: expectIdent ( tok -- tok ) dup isIdent? not ?err"identifier expected" ;
: readIdent tok< expectIdent ;
: expectChar ( tok c -- ) tuck isChar? not ?err"wrong character" drop ;
: readChar tok< swap expectChar ;
: readStr tok< s= not ?err"unexpected token" ;
\ Read token and yield whether it's "c", pushing it back if not.
: readChar? ( c -- f ) tok< swap isChar? dup not if tokstepback then ;
: readStr? ( str -- f ) tok< s= dup not if tokstepback then ;
: read; ( -- ) ';' readChar ;
: read: ( -- ) ':' readChar ;
: read( ( -- ) '(' readChar ;
: read) ( -- ) ')' readChar ;
: isHex? ( tok -- ?n ?suffix f )
  dup 1+ c@ digit? over s) 1- c@ tuck bi 'X' = | 'H' = or and if ( tok suffix )
    swap c@+ 1- parsehex if ( suffix n ) swap 1 else drop 0 then
    else 2drop 0 then ;

: _?tok< ( -- tok-or-0 )
  tonws< nip ?comment dup not if ( EOF ) exit then ( c )
  newtok dup tokacc ( c ) case ( )
    sym1? of r@ sym2? if tokacc then endof
    identifier? of
      begin in< dup identifier? while tokacc repeat drop stepback endof
    err"invalid token" endcase
  curtokcopy ;

: obtok$ tok$ ['] _?tok< ['] (?tok<) realias ;
