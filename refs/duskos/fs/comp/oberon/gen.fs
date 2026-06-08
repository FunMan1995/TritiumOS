needs lib/str lib/psrs lib/tagl lib/diag mem/kv comp/tok comp/sym comp/sig \
      comp/oberon/tok comp/oberon/ast comp/oberon/type comp/oberon/expr \
      comp/oberon/mem comp/oberon/gc comp/oberon/module
unit comp/oberon/gen

0 value curprocsig
create curprocname STR_MAXSZ allot0

alias abort ?obimport ( s -- ) \ forward declaration
alias abort begin< ( -- )

stringlist stopwords
  PROCEDURE TYPE VAR CONST LOADFORTH BEGIN END ELSE ELSIF UNTIL |
: stopword? ( tok -- f ) stopwords sfind dup if nip then ;

: prelude, pushlr, dup, localvariablesz align4 ?dup if neg rs+, then ;
:~ localvariablesz align4 ?dup if rs+, then
   argumentsz ?dup if ps+, then ;
: return, ~ popexit, ;
: postlude, ~ drop, popexit, ;

alias abort type< ( -- type ) \ forward declaration
\ Each element in "..." is in fact two elements: "public? name"
\ The first argument is on top.
alias abort decl< ( -- ... n type )

\ If type is a pointer to an OpenArray or Structure, grow arg zone by 4.
: ?growargzone ( type -- type )
  dup pointer? if dup reftype bi openarray? | struct? or if
    growargzone then then ;

: parsesig ( -- sig )
  '(' readChar? if ')' readChar? not if begin
    decl< swap 0 do ( ... public? name type )
      dup rot addargument drop ?growargzone nip loop drop
    ';' readChar? while repeat read) then then ( )
  argtypes ':' readChar? if type< 1 else 0 then ( ... n ... n )
  0 newsignature ;

: POINTER
  "TO" readStr
  readIdent '.' readChar? if readIdent findtypeinmodule# else
    dup findbasetype ?dup if nip else
      0 0 newstructure r! rot obaddtype r> then then ( type )
  newgcpointer ;

:~ constexpr< nip ',' readChar? if ~ else "OF" readStr type< then newarray ;
: ARRAY "OF" readStr? if type< newopenarray newpointer else ~ then ;
: RECORD
  '(' readChar? if type< read) else 0 then
  newstructure "END" readStr? if exit then
  dup cur ! begin ( struct )
    symbols$ decl< >r ( struct ... n ) \ V1=type
    0 do nip NEXTWORD ! V1 addalignedfield drop loop rdrop
    ';' readChar? not until
  "END" readStr ;

: PROCEDURE savesymstate parsesig >r restoresymstate r> ;

extractdict POINTER decllevel ( -- type )

:realias type< ( -- type )
  readIdent dup decllevel find ?dup if nip execute else
    '.' readChar? if readIdent else 0 swap then findtypeinmodule# then ;

:realias decl< ( -- ... n type )
  "VAR" readStr? >r \ V1=var?
  0 begin
    readIdent swap 1+ rollk>
    '*' readChar? swap 1+ rollk>
    ',' readChar? not until ( ... n*2 )
  2/ read: type<
  r> if dup pointer? not if newpointer then then ;

: deeptype? ?unwrapptr drop bi struct? | array? or ;
: ?deepcopy ( leftast rtype right -- ?leftast ?rtype ?right f )
  over deeptype? not if 0 exit then
  \ alright, let's deep copy!
  swap struct? if &) then ( ast right )
  ?W& swap expr, ( right ltype left )
  over ?unwrapptr drop typesz i) S>) @, \ S=sz
  over struct? if &) then ( right ltype left )
  swap deeptype? not ?err"invalid operand for deep copy assignment"
  bothW? if drop W&# A) &) !, drop, PS- else anyW? if
    if A>) @, ?>W$ else swap &) A>) @, @, then
    else A>) @, ?>W$ then then cmove, freeW 1 ;
: ?derefW ( type halop -- halop )
  swap ?unwrapptr not if drop else swap @, W) swap type) then ;
: ?derefA ( type halop -- halop )
  swap ?unwrapptr not if drop else swap A>) @, A) swap type) then ;
: ?derefdst ?derefA dup (&? ?err"assignment to dereferenced halop" ;
: assignment< ( leftast rightast -- )
  expr, ?deepcopy if exit then ( leftast rtype right )
  ?W& rot expr, ( rtype right ltype left )
  over OPAQUE = if nip rot drop INTEGER rot> INTEGER swap then
  dipswap bothW? if ( rtype ltype Wop PSP )
    A>) @+, PS- ?derefW A>) !, drop
  else ( rtype ltype right left )
    anyW? if
      if 4 roll rot ?derefW ?>W$ ?derefdst !,
      else 4 roll swap ?derefA S>) @, ?derefdst S>) !, then
      else 4 roll rot ?derefW ?>W$ ?derefdst !, then then
  freeW ;

: procrettype#
  curprocsig dup sigcounts nip not ?err"RETURN without a type!"
  sigoutputs @ ;

: bool>W$ ( type halop -- ) ?ptrderef, ?>W$ BOOLEAN expecttype# ;

0 value hasret
: RETURN
  expr< expr, ?>W$ ( type )
  procrettype# 2dup type= not if ( type expected )
    over pointer? if dip reftype | W) @, then then ( type expected )
  expecttype# return,
  1 to hasret ;

:~ ( neg? -- )
  read( expr< ',' readChar? if ( neg? leftast )
  tuck expr< rot if op"-" else op"+" then ( left left right op )
  rot> cons cons assignment<
  else expr, ?derefW ( neg? halop ) 1 rot if neg then swap +n, then
  read) freeW ;
: INC 0 ~ ;
: DEC 1 ~ ;

:~ ( op -- )
  read( expr< tuck ',' readChar expr< ( left op left right )
  0 cons op"{}" swap cons ( left op left {right} )
  cons cons assignment<
  read) freeW ;
: INCL op"+" ~ ;
: EXCL op"-" ~ ;

:~ ( -- type halop ) \ W=addr
  read( expr< ',' readChar expr< read)
  expr, ?autoderef, ?W& rot expr, ?autoderef, nip ( type right left )
  ?>W dup (W? if drop PSP) A>) @+, PS- A) &) then ( type halop ) ;
: GET ~ W) rot type) @, !, freeW ;
: PUT ~ S>) @, W) swap type) S>) !, freeW ;

:~ abort"Oberon assertion failed" ;
: ASSERT
  read( expr< read) expr, bool>W$ ['] ~ 0 i) =) ?br, ;

: DBG ."compile time debug point\n"
      ."Procedure: " curprocname stype nl>
      .S nl> tokdbg .localsymbols
      ."Stack sizes " localvariablesz . spc> argumentsz . nl> ;


: DBGTYPE
  read( expr< read) expr,
  gettype, dup, ['] .x execute, ['] spc> execute,
  dup, ['] .type execute, ['] nl> execute, freeW ;

:~ ( type -- ) gcptr# i) @, compile newptr ;
: NEW
  read( expr< read) expr, ( type halop )
  dup (W? if dup, freeW swap ~ A) &) !, drop, A>)
   else swap ?unwrapptr if ( halop type )
    ~ A>) @, A) else ~ then then ( halop ) !, ;

:~
  expr< expr, bool>W$ 0 i) <>) if, "THEN" readStr
  begin< tok< case
    "ELSE" s= of [compile] else begin< endof
    "ELSIF" s= of [compile] else ~ endof
    drop tokstepback endcase
  [compile] then ;
: IF ~ "END" readStr ;

:~ ( -- exitjmp ) expr< expr, bool>W$ 0 i) <>) if, "DO" readStr begin< ;
: WHILE
  here ~ ( loop exitjmp )
  begin tok< "ELSIF" s= while ( loop exitjmp )
    over bbr, [compile] then ~ repeat tokstepback ( loop exitjmp )
  swap bbr, [compile] then "END" readStr ;

: REPEAT
  here begin< "UNTIL" readStr ( loop )
  expr< expr, bool>W$ ( loop )
  0 i) =) ?br, ;

\ the code below seems needlessly convoluted, but that's to work around a
\ fundamental HAL limitation: its hbank can't survive through a "begin<" call.
: FOR
  readIdent ":=" readStr mkident dup expr< assignment< ( identast )
  "TO" readStr expr< expr, nip ?>W$ dup, PS+ ( ast )
  "BY" readStr? if constexpr< nip else 1 then >r ( ast ) \ V1=incn
  here swap dup expr, ?ptrderef, nip ?>W$ ( loop identast )
  PSP) signed) r@ 0< if >=) else <=) then if, ( loop ast exitjmp )
  "DO" readStr begin< r> rot expr, ?ptrderef, nip +n, ( loop exitjmp )
  swap bbr, [compile] then "END" readStr
  drop, PS- freeW ;

\ logic is similar to switch in comp/c/fgen.fs, but there's no fallthrough logic
\ here so it's not quite the same.
\ Here, we have an arbitrary number of exit jumps to resolve. We put those exit
\ jumps in RS and resolve them using "n" (+1, for "nomatch")
\ So that part is simple. The convoluted part is the "type matching" part.
\ When have this case, we have to temporarily override reftype of the gcptr
\ so that the body of the case can access subrecord fields. A bit messy.
variable overridetype
variable origtype
: reftype! ( type tgt -- ) $10 + ! ;
: override! ( type ) dup reftype origtype ! overridetype ! ;
: case<
  overridetype @ ?dup if
    type< dup gcptr? if reftype then ( tgt type )
    dup rot reftype! ( type )
    else constexpr< nip then ;

: _regular, ( -- notfoundjump ) kv', W) br, ;

\ perform multiple lookups, each time "descending" into the struct's hierarchy,
\ until a match is found.
: _type, ( -- notfoundjump )
  here PSP) A>) -!, kv', nip, W) br, [compile] then
  PSP) A>) @+, W) REFTYPEOFF +) @, ( reftype ) 0 i) <>) ?br, fbr, ;

: CASE
  0 overridetype @! >r origtype @ >r \ V1/V2=saved vars
  expr< expr, "OF" readStr ( type halop ) \ W=n
  swap case ( halop )
    gcptr? of ?>W$ W) -4 +) @, r@ override! endof
    structptr? of r@ swap recordtypeop @, r@ override! endof
    drop ?>W$ endcase ( )
  4 parena1@ allot r! m) A>) @, \ V3='kvtbl A=kvtbl
  overridetype @ if _type, else _regular, then >r \ RS stores forward jumps
  0 begin ( ... n )
    case< ':' readChar here begin< rot 1+ ( ... n )
    fbr, >r \ exit jump
    '|' readChar? not until ( ... n )
  "END" readStr
  dup 1+ begin r> [compile] then 1- ?dup not until ( ... n )
  parena1@ kvtbl, r> ( 'lookup ) !
  overridetype @ ?dup if origtype @ swap reftype! then
  r> origtype ! r> overridetype ! ;

extractdict RETURN bodylevel ( -- )

: proccall< ( procast -- )
  dup car op"()" = if cdr else 0 cons then
  proccall ( sig )
  sigcounts ?err"proper procedure expected" if dup, then ;

: assignmentorproc<
  designator< ":=" readStr? if expr< assignment< else proccall< then ;
:realias begin<
  begin ( )
    begin ';' readChar? not until
    peektok< stopword? not while
    tok< bodylevel find
    ?dup if execute else tokstepback assignmentorproc< then repeat ;

: constdirective< begin
    readIdent '*' readChar? "=" readStr
    constexpr< ( name public? type n )
    4 roll addconst
  read; peektok< stopword? until ;

: VAR begin
  decl< swap 0 do ( ... public? name type )
    dup rot addlocalvariable drop nip loop drop
  read; peektok< stopword? until 0 ;

: CONST constdirective< 0 ;

: BEGIN
  obmemreserve
  createtag >r curprocname fullqual NEXTWORD ! code
  current n"SIGT" curprocsig r> settag
  implementcurproc
  0 to hasret
  prelude, begin< "END" readStr
  curprocname tok< s= not ?err"wrong END qualifier"
  hasret not if
    curprocsig sigcounts nip ?err"function without a RETURN"
    postlude, then
  read; 1 ;
extractdict VAR proclevel ( -- f )

: PROCEDURE
  symbols$
  readIdent dup curprocname strmove
  '*' readChar? parsesig read; dup to curprocsig rot addproc
  begin tok< proclevel find# execute until ;

: TYPE begin
    symbols$
    readIdent '*' readChar? "=" readStr type< ( name public? type )
    rot obaddtype
  read; peektok< stopword? until ;

: CONST constdirective< ;

: VAR begin
  decl< swap 0 do ( ... public? name type )
    tuck >r addvar r> loop drop
  read; peektok< stopword? until ;

: IMPORT
  curmodule >r curline @ >r curfile str>r curmodulename str>r
  begin readIdent ?obimport ';' readChar? not while ',' readChar repeat
  r>str curmodulename strmove r>str curfile! r> curline ! r> restoremodule ;

: LOADFORTH f<< ;

: BEGIN \ module level begin, we compile then execute directly.
  symbols$ obmemreserve here
  pushlr, dup, begin<
  drop, popexit, execute ;

extractdict PROCEDURE toplevel

: MODULE
  obmemclear
  "MODULE" readStr
  readIdent read; activatemodule
  begin tok< dup "END" s= not while toplevel find# execute repeat ( tok )
  drop curmodulename readStr '.' readChar ;
