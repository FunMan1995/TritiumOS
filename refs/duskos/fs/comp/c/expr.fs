needs hal/opq hal/muldiv lib/psrs lib/type lib/wordtbl num/math mem/cons \
      comp/w comp/c/glob comp/c/ast comp/sym
unit comp/c/expr

: int# ( type -- ) int? not ?err"integer type expected" ;
: int4? ( type -- f ) bi int? | typesz 4 = and ;
: ref? ( type -- f ) bi pointer? | array? or ;
: ?unwrapptr ( type -- type f ) dup ref? if reftype 1 else 0 then ;
: unwrapptr# ( type -- type )
  ?unwrapptr not if .type nl> err"is not a pointer" then ;
: voidptr? ?unwrapptr if void? else drop 0 then ;

create flexints map< , flexuint flexint
: flexint? flexints 2 idx dup if nip then ;
:~ over flexint? if nip dup then
   over voidptr? over ref? and if nip dup then ;
: ?flex ( t1 t2 -- t1 t2 ) ~ swap ~ swap ;

\ TODO: actually compare signatures
: type= ( other type -- f )
  2dup = if 2drop 1 exit then
  ?flex over voidptr? over voidptr? and if 2drop 1 exit then
  case ( other )
    int? over int? and of
      bi intsigned? r@ intsigned? = | typesz r@ typesz = and endof
    ref? over ref? and of reftype r@ reftype type= endof
    signature? of signature? endof \ TODO: actually compare signatures
    drop 0 endcase ;
: type=# ( type expected -- )
  2dup type= if 2drop else
    ."want: " dup .x spc> .type nl>
    ."have: " dup .x spc> .type nl>
    err"unexpected type" then ;

: merge# ( right left -- type ) tuck type=# ;

: const# ( halop -- n ) (i? not ?err"not a constant halop" ;

: sz>idx ( 1-2-4 -- 0-1-2 ) log2 dup 2 > if abort"bad type size" then ;
: moveresize16 0 do over @ over w! 2+ swap 4+ swap loop 2drop ;
: moveresize8 0 do over @ over c! 1+ swap 4+ swap loop 2drop ;

3 wordrefs moveresizetbl moveresize8 moveresize16 move ( src dst u -- )

alias abort expr, ( ast -- type halop ) \ forward reference

\ these below are only valid right after a 2expr call, without having recursed
\ another expr.
0 value lefttype
0 value righttype
: mergedtype righttype lefttype merge# ;
: bothtypes! dup to lefttype to righttype ;
: _intsigned? dup int? if intsigned? else drop 0 then ;
: ?signed ( op -- op ) mergedtype _intsigned? if signed) then ;

: 2expr, ( astleft astright -- rightop leftop )
  expr, ?W& swap rot
  PSdisp >r expr, rot> to lefttype to righttype ( right left )
  swap r> ?PSP+n swap ;

: _assignop ( right left xt -- type halop )
  >r bothW? if ( Wop PSP ) \ V1=xt
    A) &) !, @+, PS- REGA src)
  else ( right left )
    dup (W? if A) &) !, REGA src) then swap ?2>W then ( halop )
  ?signed dir) r> execute mergedtype W) &) ;
: assignop ( ast xt -- type halop )
  >r cdr carcdr 2expr, mergedtype int# r> _assignop ;

\ binop table. Each row is 3 word references:
\ +00 Integer compile
\ +04 Reverse integer compile
\ +08 Integer const
\ Binary operators below refer to those rows by index.
:~ A) &) !, @, A) &) ?signed ;
: rev/, ~ divorshift, ;
: revmod, ~ modorand, ;
: rev<<, ~ <<, ;
: rev>>, ~ >>, ;
10 3 * wordrefs binoptbl
  mulorshift, mulorshift, *
  divorshift, rev/, /
  modorand, revmod, mod
  <<, rev<<, lshift
  >>, rev>>, rshift
  +, +, +
  -, swap-, -
  &, &, and
  ^, ^, xor
  |, |, or

: ?bothconst ( rightop leftop -- rightop leftop 0 OR nleft nright 1 )
  over (i? if over (i? if ( r l nr nl )
      rot drop rot drop swap 1
      else ( r l nr ) drop 0 then
    else 0 then ;

:~ ( ... idx row -- ) 3 * + binoptbl swap wexec ;
: _binop ( type right left binoprow -- type halop )
  >r ?bothconst if ( nl nr ) 2 r> ~ i) else ( r l )
    bothW? if swap W&# ?signed 0 r> ~ nip, PS- else
      anyW? if rot W&# else ?2>W 0 then ( otherop idx )
      swap ?signed swap r> ~ then W) &) then ( halop )
  mergedtype swap ;
: binop ( astarg binoprow -- type halop )
  >r cdr carcdr 2expr, mergedtype int# r> _binop ;

0 value arin
: ari*, ( halop -- halop )
  arin 1 > if case ( )
    (i? of arin * i) endof
    (&? of arin i) r@ (src dst) mulorshift, r@ endof
    dup S) &) <> if S>) @, then arin i) S>) mulorshift, S) &)
  endcase then ;
: ari*right, ( right left -- right left )
    over (W? over (W? and if
      nip PSP) S>) @+, PS- S) &) ari*, swap else dip ari*, | then ;
: ari*? ( type -- f ) ?unwrapptr if typesz to arin 1 else drop 0 then ;

\ do pointer arithmetics if one side is a pointer and the other is a int
: do+, ( ast -- type halop )
  cdr carcdr 2expr, ( right left )
  lefttype ari*? if
    righttype int# lefttype bothtypes! ari*right,
    else righttype ari*? if
      lefttype int# righttype bothtypes! ari*, then then ( right left )
  5 _binop ;

: do+-=, ( ast xt -- type halop )
  >r cdr carcdr 2expr, ( right left ) \ V1=xt
  lefttype ari*? if
    righttype int# lefttype bothtypes! ari*right, then ( right left )
  r> _assignop ;

: do-, ( ast -- type halop )
  cdr carcdr 2expr, ( right left )
  lefttype ari*? if
    righttype ref? if
      6 _binop ( type op )
      ?>W reftype typesz i) divorshift, ( )
      flexint W) &) exit then ( right left )
    righttype int# lefttype bothtypes! ari*right, then ( right left )
  6 _binop ;

: unary ( ast runxt compxt -- type halop )
  rot cdr expr, dup (i? if ( runxt compxt type op n )
    nip rot drop rot execute i)
  else ( runxt compxt type op )
    ?>W swap execute nip W) &) then ;

create _ map< , <) >) <=) >=) =) <>)
: idx>cond 4* _ + @ ;

6 wordrefs _ < > <= >= = <>
: applycond ( n n condidx -- f )
  >r mergedtype _intsigned? if rot $80000000 + rot $80000000 + rot then
  _ r> wexec ;

\ yields halop compared to W in a way that makes "bool," work.
\ To avoid W juggling, we check if our right operand is W. If it is, no need
\ for juggling, all we need is to invert the condition we use.
: boolexpr, ( ast -- halop cond 0 OR f 1 )
  cdrcar op"<" - >r carcdr 2expr, ( right left ) \ V1=condidx
  ?bothconst if r> applycond 1 else ( right left )
    r> idx>cond >r \ V1=cond
    bothW? if ( Wop PSP )
      A>) @+, PS- ?>W A) &)
    else ( right left )
      over (W? if swap r> swappedcond >r then ?2>W then ( op )
    ?signed r> 0 then ;
: boolop ( ast -- type halop )
  boolexpr, if i) else bool, W) &) then ( halop )
  flexuint swap ;

: incop ( type halop n -- type halop )
  >r over ari*? if arin doto V1 * | then ( type halop )
  r> over +n, ;

: nosym tokdbg (wnf) ;
: getsym ( name -- type halop )
  dup findsymbol ?dup if nip bi type | symbol) else
    sysdict findentry ?dup not if nosym then ( e )
    dup entrytag case ( e )
      n"CNST" = of flexuint swap e>xtsel execute i) endof
      n"VALU" = of flexuint swap scryentry# m) endof
      drop e>xtsel dup n"SIGT" findtag if swap i) else nosym then endcase then ;

\ Wordtbl below is index by AST opid
wordtbl[ ( ast -- type halop )
:> ( * ) 0 binop ;
:> ( / ) 1 binop ;
:> ( % ) 2 binop ;
' do+,
' do-,
:> ( << ) 3 binop ;
:> ( >> ) 4 binop ;

\ all boolops have the same handler
' boolop dup 2dup 2dup

:> ( & ) 7 binop ;
:> ( ^ ) 8 binop ;
:> ( | ) 9 binop ;

\ TODO: these two below can be integrated in boolexpr, for speed
: _then ( a PSdisp -- ) psrestore here br! ;
:~ ( ast cond -- type halop )
  >r cdr cdrcar expr, nip ?>W$ 0 i) r> if, ( ast a )
  PSdisp rot expr, nip ?>W 0 i) <>) bool, _then ( )
  flexuint W) &) ;
:> ( && ) <>) ~ ;
:> ( || ) =) ~ ;
:> err"invalid op '?'" ;

:> ( = )
  cdr dup cdr car op"array" <> if carcdr 2expr, ['] @, _assignop else
    carcdr expr, const# litn arraycount ( leftast u )
    swap expr, dup, dup @, ( u type halop )
    rot litn ( type halop )
    over unwrapptr# typesz sz>idx moveresizetbl swap wexec, then ;

:> ( += ) ['] +, do+-=, ;
:> ( -= ) ['] -, do+-=, ;
:> ( *= ) ['] mulorshift, assignop ;
:> ( /= ) ['] divorshift, assignop ;
:> ( %= ) ['] modorand, assignop ;
:> ( <<= ) ['] <<, assignop ;
:> ( >>= ) ['] >>, assignop ;
:> ( &= ) ['] &, assignop ;
:> ( ^= ) ['] ^, assignop ;
:> ( |= ) ['] |, assignop ;

:> ( ++ ) cdr expr, 1 incop ;
:> ( -- ) cdr expr, -1 incop ;
:~ -1 i) ^, ;
:> ( ~ ) ['] inv ['] ~ unary ;
:~ 0 i) =) bool, ;
:> ( ! ) ['] not ['] ~ unary ;
:~ 0 i) swap-, ;
:> ( neg ) ['] neg ['] ~ unary ;

:> ( ref )
  cdr expr, over struct? not if
    dup (&? ?err"can't reference this halop" &) then
  dip tarena1 newpointer | ;

:> ( deref )
  cdr expr, dup (i? if nip m) else ?>W W) then ( type op )
  dip unwrapptr# | over type) ;

:> ( sizeof )
  cdr dup findtype ?dup if nip else getsym drop then ( type )
  typesz i) flexuint swap ;

:> ( funcall )
  \ our args contain, first, the function to call, then arguments. We want to
  \ first resolve the function address, then resolve and push our arguments in
  \ *reverse* order.
  cdr carcdr dup length >r swap expr, ( args sig op ) \ V1=cnt
  dup (bank 2>r >r \ V2=op V3=hbank V4=sig
  V4 signature? not ?err"funcsig expected"
  begin ?dup while carcdr repeat ( ... )
  V2 (W? if V2 A>) @, A) &) to V2 freeW then ( ... )
  V4 sigcounts drop V1 V4 sigvarinput? if > else <> then
  ?err"wrong argument count" ( ... )
  ?pushW drop PSdisp >r \ V5=PSdisp
  V1 begin dup V4 sigcounts drop > while swap expr, nip ?>W 1- repeat
  0 do ( ... nextarg )
    expr, ?>W ( ... type )
    V4 sigcounts drop i - 1- ( ... type idx )
    4* V4 siginputs + @ ( ... t1 t2 )
    type=# loop ( )
  V2 V3 bank) brr, ( )
  r> to PSdisp
  V4 sigcounts if ( inputcount )
    not if nip, then V4 sigoutputs @ useW
    else if dup, then void then
  W) &) 2rdrop 2rdrop ;

\ Maximum size in bytes that a single list literal can have
$400 const MAXLITSZ
create _buf MAXLITSZ allot

:> ( array )
  cdr _buf begin ( elems dst )
    swap ?dup while ( dst elems )
    cdrcar expr, nip const# ( a elems n )
    rot !+ repeat ( a )
  _buf - _buf over ( u a u ) parena1@ cmoveallot ( u a )
  swap 4/ flexuint tarena1 newarray ( a type )
  swap i) ;

:> ( typecast )
  cdr carcdr carcdr ( name lvl ast )
  rot findtype dup not ?err"bad typecast" ( lvl ast type )
  rot 0 do tarena1 newpointer loop ( ast type )
  swap expr, >r ( newtype oldtype ) \ V1=halop
  over typesz over typesz - ?dup if
    V1 ?>W W) &) to V1 0< if
      over typesz 8* 32 swap- i) dup <<,
      oover bi int? | intsigned? and if signed) then >>,
    else
      dupbi int? | intsigned? and if ( newtype oldtype )
        dup typesz 8* 32 swap- i) dup <<, signed) >>, then
    then then ( newtype oldtype )
  drop r> ;

:~ ( ast n )
  >r cdr expr, dup (W? if
    A) &) !, dup ?>W REGA src) else dup ?>W then ( type op )
  r> incop drop W) &) ;
:> ( postinc ) 1 ~ ;
:> ( postdec ) -1 ~ ;
:> ( -> )
  cdr carcdr expr, ( fieldname type halop )
  ?>W unwrapptr# dup struct? not ?err"structure expected" ( fieldname struct )
  findfield not ?err"field not found" ( type offset )
  ?dup if i) +, then \ TODO: +) doesn't work, but it would be nice
  W) over array? if &) else over type) then ;

:> ( ?: )
  cdr carcdr carcdr ( cond true false )
  swap rot expr, nip ?>W$ 0 i) <>) if, PSdisp ( f t jmp pslvl )
  rot expr, nip ?>W$ psrestore fbr, swap here br! PSdisp ( f jmp pslvl )
  rot expr, ?>W rot> _then W) &) ;

:> ( lit ) cdr i) flexuint swap ;
:> ( str ) cdr i) String swap ;
:> ( sym ) cdr getsym ;
]wordtbl tbl

:realias expr, dup car tbl swap wexec ;

\ from comp/c/glob
:realias parseConstExpr ( -- n ) ast< expr, nip const# ;
