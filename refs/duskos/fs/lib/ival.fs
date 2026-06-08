needs lib/type
unit lib/ival

: addr, ( IVAL -- ) dup, @+ m) @, @ i) +, ;
: addrof
  word sysdict findentry ?wnf bi scryentry# | entrytag case
    n"IVAL" = of COMPILING @ if addr, else @+ @ swap @ + then endof
    n"VALU" = of COMPILING @ if litn then endof
    n"LVAR" = of dup, [rcnt] @ - RSP) swap +) &) @, endof
    abort"wrong addrof target" endcase ; immediate

: newival ( ptr off -- IVAL ) swap 2 n,@ ;
\ TODO: we avoid using +) for drv/pc/ioport, but we can do better...
: addr, ( IVAL -- halop ) @+ m) S>) @, @ i) S>) +, S) ;
: do@, ( halop type -- ) dup xt? not if dup, then tuck type) swap type@, ;
: do!, ( halop type -- ) tuck type) swap type!, drop, ;
: newmap ( ptr off type "name" -- ptr off+ type )
  oover oover newival >r \ V1=IVAL
  r! typesz + r>
  dup ['] do!, bind>
  over ['] do@, bind>
  r@ ['] addr, bind>
  r> n"IVAL" getset, ;
: ?} ( -- f ) toword# in< dup '\' = if drop [compile] \ ?} else '}' = then ;
: ?+ ( off base -- off ) in< '+' <> if drop stepback else nip n< + then ;
: ivalmapfrom ( ptr off -- )
  toword# in< '{' <> ?abort"{ expected"
  r! begin ?} not while stepback ( ptr off V1=base )
    V1 ?+ type< begin ( ptr off type )
      word dup ";" s= not while NEXTWORD ! newmap repeat ( ptr off type s )
    2drop repeat ( ptr off ) 2drop rdrop ;
: ivalmap ( ptr -- ) 0 ivalmapfrom ;
: absvalmap ( a -- ) 1 n,@ ivalmap ;
: ivalue 0 swap newmap 2drop drop ;
