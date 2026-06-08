needs lib/match lib/macro comp/tok comp/c/pp
unit comp/c/tok

\ For symbol parsing, we exploit one particularity: all 2-3 chars symbols start
\ with a symbol that is also a 1 char symbol.
create symbols1 ,"+-*/~&<>=[](){}.%^?:;,|^\"!`"
: isSym1? ( c -- f ) symbols1 27 cidx dup if nip then ;

stringlist symbols
  <= >= == != && || ++ -- -> << >> += -= *= /= %= &= ^= |= <<= >>= .. ...
: isSym? ( str -- f ) symbols sfind dup if nip then ;

\ is c a proper 1st char for an identifier
: identifier1st? ( c -- f ) rmatch"AZaz__" ;
\ is c a proper char for an identifier after the first char?
: identifier? ( c -- f ) rmatch"09AZaz__" ;
\ is c a proper char for either an identifier or a number literal?
: ident-or-lit? ( c -- f ) rmatch"09AZaz__$$" ;

: expectConst ( tok -- n ) dup parse if nip else err"constant expected" then ;
: isIdent? ( tok -- f )
  dup 1+ c@ identifier1st? not if drop 0 exit then
  c@+ 0 do ( a ) c@+ identifier? not if break then loop drop broke? not ;
: expectIdent ( tok -- tok ) dup isIdent? not ?err"identifier expected" ;
: readIdent? ( -- s? f ) tok< dup isIdent? if 1 else drop tokstepback 0 then ;
: expectChar ( tok c -- ) tuck isChar? not ?err"wrong character" drop ;
: readChar tok< swap expectChar ;
\ Read token and yield whether it's "c", pushing it back if not.
: readChar? ( c -- f ) tok< swap isChar? dup not if tokstepback then ;
: read; ( -- ) ';' readChar ;
: read( ( -- ) '(' readChar ;
: read) ( -- ) ')' readChar ;

: _?0x< ( c -- c )
  dup '0' = if in< 'x' = if drop '$' else stepback then then ;

\ Check if there's a comment there. If there is, skip it
\ We avoid recursion in here because a big bunch of comments can smash the stack
: _?comment ( ws c -- ws c )
  1 begin ( ws c loop? ) over '/' = and while ( ws c )
    drop in< case
      '*' = of
        0 begin ( ws c ) '*' = in< ?line+ dup '/' = rot and until ( ws c )
        2drop tonws< 1 endof
      '/' = of
        begin ( ws ) in< dup 0< if ( EOF! ) rdrop exit then LF = until ( ws )
        drop stepback tonws< 1 endof
      ( ws c ) drop stepback '/' 0 endcase ( c loop? ) repeat ( ws c ) ;

: _?execmacro ( tok -- tok )
  dup findMacro ?dup if
    nip in< '(' = dup not if stepback then ( macro f )
    swap macro< ( f a u ) rot if read) then injectrange ( )
    ?tok< then ;

\ Returns the next token as a string or 0 when there's no more token to consume.
: _?tok< ( -- tok-or-0 )
  tonws< dup not if nip ( EOF ) exit then ( lastws c )
  _?comment dup not if nip ( EOF ) exit then ( ws c )
  dup '#' = rot LF = and if drop stepback run1 ?tok< exit then ( c )
  _?0x< newtok dup tokacc ( c ) case ( )
    isSym1? of ( )
      begin in< tokacc curtok isSym? not until ( )
      stepback tokdel ( ) curtokcopy ( tok ) endof
    ''' = of
      str< not if '"' then
      in< ''' <> ?err"invalid char literal" n>tok endof
    ident-or-lit? of \ identifier or number literal
      begin in< dup ident-or-lit? while tokacc repeat ( c ) drop stepback
      curtokcopy _?execmacro
    endof
    err"invalid token" endcase ;

: cctok$ tok$ ['] _?tok< ['] (?tok<) realias ;
