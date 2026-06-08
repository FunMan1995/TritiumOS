needs hal/opq hal/muldiv lib/str lib/psrs lib/bit lib/type lib/wordtbl \
      mem/cons mem/dict comp/w comp/sig \
      comp/oberon/type comp/oberon/ast comp/oberon/mem comp/oberon/module
unit comp/oberon/expr

\ binop table. Each row is 6 word references:
\ +00 Integer compile
\ +04 Reverse integer compile
\ +08 Integer const
\ +0c Set compile
\ +10 Reverse set compile
\ +14 Set const
\ Binary operators below refer to those rows by index.
: err err"invalid binop for SET" ;
: s/, signed) divorshift, ;
: smod, signed) modorand, ;
: s>>, signed) >>, ;
: invand, S>) @, -1 i) S>) ^, S) &) &, ;
:~ A) &) !, @, A) &) ;
: rev/, ~ s/, ;
: revmod, ~ smod, ;
: rev<<, ~ <<, ;
: rev>>, ~ >>, ;
: revs>>, ~ s>>, ;
: revinvand, ~ invand, ;
:~ over signed) swap if, swap @, [compile] then ;
: min, >) ~ ; : max, <) ~ ;
10 6 * wordrefs binoptbl
  mulorshift, mulorshift, * &, &, and
  s/, rev/, / ^, ^, xor
  smod, revmod, mod err err err
  <<, rev<<, lshift err err err
  >>, rev>>, rshift err err err
  s>>, revs>>, rshift err err err
  +, +, + |, |, or
  -, swap-, - invand, revinvand, invand
  min, min, min err err err
  max, max, max err err err

: ?bothconst ( rightop leftop -- rightop leftop 0 OR nleft nright 1 )
  over (i? if over (i? if ( r l nr nl )
      rot drop rot drop swap 1
      else ( r l nr ) drop 0 then
    else 0 then ;

:~ if ?>W reftype W) over type) then ;
: ?ptrderef, ( type halop -- type halop ) over pointer? ~ ;
: ?autoderef, ( type halop -- type halop )
  over tri stringptr? not | structptr? not | pointer? and and ~ ;
: ?gcderef, ( type halop -- type halop ) over gcptr? ~ ;

alias abort expr, ( astarg -- type halop ) \ forward decl
: 2expr, ( astleft astright -- type rightop leftop )
  expr, ?autoderef, ?W& rot ( rtype rightop astleft )
  PSdisp >r expr, ?autoderef, nip ( type right left )
  swap r> ?PSP+n swap ;

\ Ensure that leftop lives in W. This can only be called when
\ bothconst and bothW cases have been handled.
: left>W ( rightop leftop -- rightop )
  anyW? if rot W&# if A) &) !, @, A) &) then else ?2>W then ;

:~ ( ... idx row type -- ) SET = 3 * + + binoptbl swap wexec ;
: binop ( astarg binoprow -- type halop )
  6 * >r carcdr 2expr, oover >r ( type r l ) \ V1=row V2=type
  ?bothconst if ( type nl nr ) 2 2r> ~ i) else ( type r l )
    bothW? if swap W&# 0 2r> ~ nip, PS- else
      anyW? if rot W&# else ?2>W 0 then ( type otherop idx )
      2r> ~ then ( type ) W) &) then ;

: zero# ?err"too many args" ;
: onearg# ( args -- ast ) carcdr zero# ;
: twoarg# ( args -- two one ) carcdr onearg# ;

\ Provided that type is an array or open array, yield a halop that corresponds
\ to the length of that array
: arraylenop ( type halop -- halop )
  swap ?unwrapptr drop case ( halop )
    openarray? of 4 +) endof
    array? of drop r@ arraycount i) endof
    err"array expected" endcase ;

: rorn, ( n -- )
  $1f and S) &) !,
  dup i) >>, 32 swap- i) S>) <<, S) &) |, ;
: ror, ( halop -- )
  dup (W? ?err"ROR 2nd argument can't be an expression"
  dup A>) @, 32 i) A>) swap-, S) &) !, \ A=32-n
  ( halop ) >>, A) &) S>) <<, S) &) |, ;
: int# INTEGER expecttype# ;

\ same signature everywhere: ( ast -- type halop )
: LEN \ LEN(v:array)
  onearg# expr, arraylenop INTEGER swap ;

: VAL \ VAL(T, n)
  twoarg# expr, nip ( arg halop )
  dip ident# findtypeinmodule# | ;

: ADR onearg# expr, &) nip INTEGER swap ;
: ORD onearg# expr, nip BYTE swap ;
: CHR onearg# expr, nip CHAR swap ;

: ROR \ ROR(x, n)
  twoarg# expr, nip
  swap expr, nip ?>W ( halop )
  dup (i? if nip rorn, else ror, then
  INTEGER W) &) ;
:~ swap twoarg# cons swap binop ;
: LSL 3 ~ ; : LSR 4 ~ ; : ASR 5 ~ ; : MIN 8 ~ ; : MAX 9 ~ ;

:~ ( arg -- ) onearg# expr, swap int# ?>W ;
: ODD ~ 1 i) &, BOOLEAN W) &) ;
: ABS ~ 0 i) signed) <) if, 0 i) swap-, [compile] then INTEGER W) &) ;

extractdict LEN builtins

: ?builtin, ( ast -- type halop 1 OR ast 0 )
  dup cdrcar cdrcar op"ident" <> if 2drop 0 else ( ast args name )
    builtins find ?dup if execute rot drop 1 else drop 0 then then ;

: strcmp, ( cond halop -- )
  A>) @, 1 i) A>) -, here
    1 i) A>) +, W) 8b) S>) @+,
    0 i) S>) <>) if,
    swap A) 8b) S>) =) ?br,
  [compile] then ( cond ) A) 8b) S>) swap bool, ;

\ TODO: reverse the condition when anyW? yields right=1
\       current codegen is suboptimal
:~ ( cond type halop -- type halop )
  swap ?unwrapptr drop string? if strcmp, else signed) swap bool, then
  BOOLEAN W) &) ;
: condop ( astarg cond -- type halop )
  swap carcdr 2expr, ( cond type r l )
  ?bothconst if swap i) @, i) ~ else ( cond type r l )
    bothW? if swap W&# ~ nip, PS- else left>W ~ then then ;

: oob abort"out of bounds indexing" ;

\ if expected is a pointer to type and halop can get a "&)", do it.
\ if type is a pointer and expected is not, dereference.
: checktype ( expected type halop -- halop )
  >r over OPAQUE <> over OPAQUE <> and if
    over pointer? over pointer? not and if
      over reftype over
      swap type= if r> &) >r tarena1 newpointer then then
    over pointer? not over pointer? and if r> ?autoderef, >r then
  then swap expecttype# r> ;

: ?pusharraylen ( expected type halop -- expected type halop )
  oover pointer? if oover reftype openarray? if
    2dup arraylenop S>) @,
    over pointer? if @, W) &) then
    PSP) S>) -!, PS+ then then ;

\ If halop is PSP), it means that our type lives in PSP+4. Otherwise, our type
\ is exactly the same as "type".
: recordtypeop ( type halop -- halop )
  dup (src REGPSP = if nip 4 +) else drop i) then ;
: ?pushrecordtype ( expected type halop -- expected type halop )
  over OPAQUE = if exit then
  oover structptr? if
    2dup recordtypeop S>) @,
    over pointer? if @, W) &) then
    PSP) S>) -!, PS+ then ;

\ yes, it's hackish, but I'm in a tight corner here
: dryexpr ( ast -- type halop )
  here >r doto usesW? 0 | >r expr, r> to usesW? r> HERE ! ;

\ we need to push arguments in the opposite order.
: unwrapargs ( args -- ... n )
  0 swap begin ?dup while ( ... n args )
    cdrcar rot 1+ rot repeat ;
: arg, ( arg 'sigarg -- 'sigarg+4 )
  4- dup @ rot expr, ( 'sigarg expected type halop )
  ?pusharraylen ?pushrecordtype checktype ?>W$ ;
: proccall ( args -- funcsig )
  PSdisp >r cdrcar >r \ V1=PSdisp V2=funcast
  unwrapargs ( ... n )
  dup V2 dryexpr drop ( ... n n sig )
  tuck sigcounts drop <> ?err"wrong argument count" ( ... n sig )
  siginputs over 4* + ( ... n 'sigargs )
  over if swap 1- rot> arg, then ( ... n 'sigarg )
  swap 0 do dup, PS+ arg, loop ( 'sigarg ) drop
  r> ( ast ) useW expr, dup (W? if
    A>) @, drop, PS- A) &) then ( sig halop )
  brr, ( sig ) signature# r> to PSdisp freeW ;
: funcall ( args -- type halop )
  ?pushW drop
  ?builtin, if exit then
  proccall dup sigcounts 1 <> ?err"not a function" ( sig numinput )
  not if nip, then
  sigoutputs @ W) &) useW# ;
: typeguard? ( args -- ?type f )
  cdr ?dup not if 0 exit then
  carcdr ?dup if 2drop 0 exit then ( typeast )
  cdrcar case ( arg R: opid )
    op"ident" = of ( ident )
      findbasetype dup if 1 then endof
    op"." = of ( ast )
      carcdr dryexpr ( name type halop )
      swap moduletype <> if 2drop 0 else ( name module )
        types find dup if @ 1 then then endof
    2drop 0 endcase ;

: ?tgerr ?err"invalid typeguard" ;
: typeguard ( args tgttype -- type halop )
  swap car expr, >r ( tgttype type ) \ V1=halop
  over gcptr? if
    dup gcptr? not ?tgerr
    reftype over reftype swap ( type tgtstruct struct )
  else
    over struct? not over structptr? not or ?tgerr
    over tarena1 newpointer ( tgtstruct structptr type )
    rot> reftype ( type tgtstruct struct ) then
  containsstruct? not ?tgerr ( tgtstruct )
  \ TODO generate dynamic typguard check
  r> ;

: funcallortypeguard ( args -- type halop )
  dup typeguard? if typeguard else funcall then ;

\ Most of the time, set literals are entirely constant. When that happens, we
\ don't want to generate super inefficient code, we want to yield a constant
\ at compile time. However, we also need to support the cases where {} contains
\ non-constant identifiers. That case is less common and yes, we generate code
\ that could be tighter, but it's not worth bothering.
: ?shiftW, ( type -- ) SET <> if S) &) !, 1 i) @, S) &) <<, then ;
: mkset, ( res args type halop -- type halop )
  ?>W$ ?shiftW, ( res args )
  swap ?dup if i) |, then dup, PS+
  begin ?dup while ( args )
    cdrcar expr, ?>W$ ?shiftW, ( args )
    PSP) dir) |, repeat
  drop, PS- SET W) &) useW# ;
: mkset ( args -- type halop )
  0 swap begin ?dup while ( res args )
    cdrcar expr, ( res args type halop )
    dup (i? if nip else mkset, exit then ( res args type n )
    swap SET = if rot or else dipswap bit1! then ( args res )
    swap repeat ( res )
  i) SET swap ;

: mkrange1, ( lowop highop -- type halop )
  A>) @, ?>W \ W=lo A=hi
  W) &) A>) -, \ A=bitcnt-1
  1 i) S>) @, \ S=bit
  W) &) S>) <<,
  0 i) @,
  32 i) A>) <) if,
  S) &) @, 1 i) A>) +, \ W=res A=bitcnt
  [compile] begin
    1 i) A>) -, ifnz,
    1 i) S>) <<, S) &) |,
    swap [compile] again [compile] then
  [compile] then
  SET W) &) ;

: mkrange2, ( other halop -- type halop )
  ?>W$ dup, PS+ expr, nip PSP) mkrange1, nip, PS- ;

: mkrange
  carcdr expr, nip dup (i? not if mkrange2, exit then ( other halop hi )
  nip swap expr, nip ( hi lowop )
  dup (i? not if swap i) mkrange1, exit then ( hi lowop lo )
  nip dip 1+ | ( hi+1 lo )
  0 rot> do i bit1! loop i) SET swap ;

\ a expr, that can yield a constant
: exprc, ( ast -- type halop 0 OR type n 1 )
  expr, dup (i? if nip 1 else 0 then ;

\ compile code that makes W point to the type of the expression
: gettype, ( type halop -- )
   over gcptr? if nip ?>W W) -4 +) @, else recordtypeop @, then ;

: nilerr abort"Trying to access a NIL pointer!\n" ;

wordtbl[
:> ( lit ) i) INTEGER swap ;
:> ( char ) i) CHAR swap ;
:> ( str )
  bi str>zstr | c@ 1+ dup CHAR tarena1 newarray tarena1 newpointer ( a u t )
  rot> parena1@ cmoveallot i) ;
' findident#
:> ( ` ) sysmodule findinmodule# ;
:> ( neg ) exprc, if neg i) else ?>W 0 i) swap-, W) &) then ;
:> ( ~ ) exprc, if not i) else ?>W 0 i) =) bool, W) &) then ;
:> ( ^ ) expr, ?>W anyptr# ?unwrapgcptr if W) @, then W) ;
:> ( [] )
  carcdr expr, ?ptrderef, nip ?>W expr, ( type halop )
  dup (W? if \ rightop in PSP
    &) A>) @, drop, PS- A) &) then ( type halop )
  swap arraykind# if ( halop reftype count ptr? )
    ['] oob rot i) >=) ?br, ( halop reftype ptr? )
    not if dip &) | then
  else
    over 4 +) S>) @, ['] oob S) &) >=) ?br, then ( halop reftype )
  dup typesz i) mulorshift, swap +, W) over type) ;
:> ( . )
  carcdr expr, ( name type halop )
  over moduletype = if nip findinmodule# else
    ?ptrderef, ?gcderef, ( off type halop )
    over struct? not ?err"structure expected"
    dup (i? if drop else dup testz, ['] nilerr ?brz, then
    rot> findfield not ?err"struct field not found" ( halop type off )
    dipswap +) over type) then ;
' funcallortypeguard
' mkset
' mkrange
:> ( IS ) carcdr carcdr expr, gettype, ( modname-or-0 typename )
   findtypeinmodule# ( tgttype )
   gcptrorstruct# i) A>) @, [compile] begin \ W=structtype A=tgttype
     A) &) <>) if, W) REFTYPEOFF +) @, swap 0 i) <>) ?br,
   [compile] then 0 i) <>) bool, BOOLEAN W) &) ;
:> ( * ) 0 binop ;
:> ( / ) 1 binop ;
dup ( DIV )
:> ( MOD ) 2 binop ;
:~ ( ast cond -- type halop )
  swap cdrcar expr, ?ptrderef, swap BOOLEAN expecttype# ( cond rightast op )
  ?>W$ 0 i) rot if, ( rightast jmp )
  swap expr, ?ptrderef, swap BOOLEAN expecttype# ( jmp op )
  ?>W$ [compile] then BOOLEAN W) &) useW# ;
:> ( & ) <>) ~ ;
:> ( OR ) =) ~ ;
:> ( + ) 6 binop ;
:> ( - ) 7 binop ;
:> ( = ) =) condop ;
:> ( # ) <>) condop ;
:> ( < ) <) condop ;
:> ( <= ) <=) condop ;
:> ( > ) >) condop ;
:> ( >= ) >=) condop ;
:~ ( type halop -- type halop )
  nip 1 i) S>) @, W) &) S>) <<, S>) &,
  0 i) S>) <>) bool, BOOLEAN W) &) ;
:> ( IN )
  carcdr 2expr, ?bothconst ?abort"TODO: constop IN binop" ( type r l )
  bothW? if swap W&# ~ nip, PS- else left>W ~ then ;
]wordtbl opstbl

\ realias from builtin.fs
:realias expr, carcdr opstbl rot wexec ;

: (i# ( halop -- n ) (i? not ?err"literal expected" ;
: constexpr< ( -- type n ) expr< expr, (i# ;
