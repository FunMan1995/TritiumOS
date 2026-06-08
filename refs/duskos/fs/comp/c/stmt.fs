needs lib/psrs lib/wordtbl mem/stack mem/scratch mem/kv lib/tagl \
      comp/c/tok comp/c/glob comp/c/expr
unit comp/c/stmt

$40 const MAXSWITCHCASES
\ breaks are a list of forward jumps addr that need to be resolved at the end
\ of the "breakeable" structure.
MAXSWITCHCASES newstack const _breaks
: resolvebreaks ( tgtlvl -- ) begin ( tgt )
    _breaks count over > while _breaks pop [compile] then repeat drop ;
10 newstack const _conts
: resolvecontinues ( tgtlvl -- ) begin ( tgt )
    _conts count over > while _conts pop [compile] then repeat drop ;

alias noop parseStatement ( -- ) \ forward declaration

: parseStatements ( -- ) begin '}' readChar? not while parseStatement repeat ;

: ast<, ( -- type halop ) ast< expr, ;
: boolast<, ( -- halop cond )
  ast< dup car op"<" op"!=" within? if ( ast )
    boolexpr, if i) @, 0 i) <>) then
    else expr, nip ?>W$ 0 i) <>) then
  psneutral ;

: emitRet ( -- )
  curfunc sigcounts swap 4* doto PSdisp + |
  psneutral ( outputcnt ) not if drop, then
  localvariablesz ?dup if align4 rs+, then
  popexit, ;
: _return \ empty returns are allowed
  ';' readChar? not if
    ast<, read; ( type halop ) ?>W$
    curfunc sigcounts nip 1 <> ?err"this function has a void return"
    curfunc sigoutputs @ type=# then
  emitRet ;

: _if
  read( boolast<, read) if,
  parseStatement psneutral
  tok< "else" s= if ( jump_addr )
    [compile] else parseStatement psneutral
    else tokstepback then ( jump_addr )
  [compile] then ;

: _for
  _breaks count >r _conts count >r
  read( ';' readChar? not if ast<, freeW 2drop read; then \ initialization
  psneutral here boolast<, read; if, ( loop exitjmp ) \ control
  ')' readChar? if 0 else ast< read) then ( loop jmp adjast-or-0 )
  parseStatement ( loop jmp adjast )
  r> resolvecontinues
  ?dup if expr, 2drop freeW then psneutral ( loop jmp )
  swap [compile] again [compile] then r> resolvebreaks ;

: ?ps+, PSdisp ?dup if ps+, then ;
: _break ?ps+, fbr, _breaks push read; ;
: _continue ?ps+, fbr, _conts push read; ;

: _while
  _breaks count >r _conts count >r
  psneutral here read( boolast<, read) if,
  parseStatement psneutral ( tgt jmp )
  r> resolvecontinues
  swap [compile] again [compile] then r> resolvebreaks ;

: _do
  _breaks count >r _conts count >r
  psneutral here parseStatement ( tgt )
  r> resolvecontinues
  tok< "while" s= not ?err"'while' expected"
  read( boolast<, read) ?br,
  read; r> resolvebreaks ;

: _switch
  _breaks count >r \ V1=breakcnt
  read( ast<, nip ?>W$ psneutral read) \ W=n
  4 parena1@ allot r! m) A>) @, 0 >r \ V2='lookup A=lookup V3=case count
  kv', >r W) br, \ V4=defjump
  '{' readChar tok< begin ( ... accumulated cases ... tok )
    dup '}' isChar? not while ( tok )
    dup "default" s= not while ( tok )
    "case" s= if
      ast<, nip const# here doto V3 1+ |
      ':' readChar ( )
      else tokstepback parseStatement psneutral then ( )
      tok< repeat ( tok ) \ default
    r> ( defjump ) [compile] then ':' readChar
    parseStatements psneutral
    else ( tok ) r> ( defjump ) [compile] then then ( tok ) drop
  \ local variables are broken here because of the conditional "r>" above.
  r> ( cnt ) parena1@ kvtbl, r> ( 'lookup ) !
  r> ( breakcnt ) resolvebreaks ;

\ Goto logic
\ Technically, this pad should be an arena, but 8K only for labels seems a bit
\ much to me. Let's go with a 1K scratchpad.
$400 newscratchpad bindrun1 _pad1
\ dict of address of labels for current function.
\ The payload is a 4b address which is 0 when it's a forward reference
variable gotolabels
\ For each label refs, we push two elements: "fbr," reference and then a
\ label reference
$40 newstack const labelrefs

: createLabel ( addr name -- lbl )
  4 gotolabels rot _pad1 entry+ tuck ! ;
\ Look for label in gotolabels dict. If not found, we create a new label with
\ that name, which means that in all cases, we return a label.
: findLabel ( name -- lbl )
  dup gotolabels find ?dup if ( name lbl ) nip else 0 swap createLabel then ;
: createLabel# ( name -- )
  dup findLabel dup @ if swap stype abort" label already exists" else
    ( name lbl ) nip here swap ! then ;

: _goto
  tok< expectIdent findLabel dup @ ?dup if bbr, drop else ( lbl )
    labelrefs push fbr, labelrefs push then read; ;
: resolvegotos
  begin labelrefs count while
    labelrefs pop labelrefs pop @ dup not ?err"unresolved goto"
    br! repeat ;

stringlist statementnames
  { return if for break continue while do switch goto
( -- )
10 wordrefs statementhandler
parseStatements _return          _if               _for
_break          _continue        _while
_do             _switch          _goto

variable _laststmtid
:realias parseStatement ( -- )
  freeW ';' readChar? if exit then
  tok< dup statementnames sfind if ( tok idx )
    nip statementhandler over wexec else ( tok )
    in< ':' = if createLabel# else ( tok )
      drop stepback tokstepback
      ast<, 2drop read; then ( ) 0 then ( idx )
  _laststmtid ! freeW ;

: writeArray ( array elemsz -- )
  >r @+ ( a u ) \ V1=elemsz
  dup V1 * allot@ swap ( a dst u )
  moveresizetbl r> sz>idx wexec ;

: parseDeclLine ( [initasts] cnt type -- [initasts] cnt )
  >r begin ( ... cnt ) \ V1=basetype
    V1 decl addlocalvariable drop ( ... cnt )
    '=' readChar? if ( ... cnt )
      \ place ast at the *bottom* of the list, to preserve order.
      ast< localsyms @ entryname[] []>str mksym swap cons ( ... cnt args )
      op"=" swap cons ( ... cnt ast )
      swap 1+ rollk> then ( ... cnt )
  tok< dup ';' isChar? not while ( ... cnt tok )
  ',' expectChar repeat ( ... cnt tok ) drop rdrop ;

: ?funcword ( type name -- )
  dup findannotated ?dup if ( type name sig xt )
    dup @ n"IMPL" = if
      \ TODO: check that sigs are the same
      here swap realias 2drop to curfunc exit
      else 2drop then then ( type name )
  NEXTWORD ! to curfunc ( )
  createtag >r code current n"SIGT" curfunc r> settag ;
\ '{' is already parsed
: parseFunctionBody ( type name -- )
  2>r 0 begin parsetype while ( type ) parseDeclLine repeat ( [initast] cnt )
  tokstepback
  2r> ?funcword 0 gotolabels !
  \ prelude: space for stack frame. "dup," is wiggle room for W
  pushlr, dup, localvariablesz ?dup if align4 neg rs+, then ( [initast] cnt )
  0 do expr, 2drop freeW loop ( )
  0 _laststmtid ! parseStatements
  _laststmtid @ 1 <> if emitRet then \ emit implicit return if needed
  resolvegotos ;

: parseFunctionProto ( type name -- )
  NEXTWORD ! code n"IMPL" , 0 , ( type )
  current n"SIGT" rot addtag ;

variable _running
: ?parseEnum ( -- f )
  tok< "enum" s= dup if ( f )
    -1 _running ! '{' readChar begin ( f )
      tok< expectIdent '=' readChar? if
        parseConstExpr _running ! else 1 _running +! then ( f name )
      NEXTWORD ! _running @ const
      ',' readChar? drop '}' readChar? until read;
  else tokstepback then ;

3 wordrefs _ c! w! !
: ?! ( n a type -- ) typesz sz>idx _ swap wexec ;

: parseGlobalDecl ( basetype type name -- )
  addglobalsymbol ( basetype sym )
  '=' readChar? not if drop else ( basetype sym )
    ast<, const# ( basetype sym consttype n )
    over array? if
      rot r! offset rot arraycount ( type src dst u ) \ V1=sym
      moveresizetbl r> Symbol.type unwrapptr# typesz sz>idx wexec
    else nip over offset rot Symbol.type ?! then then ( basetype )
  ',' readChar? if dup decl parseGlobalDecl else read; drop then ;

\ Begin parsing incoming tokens for a new "element" (a function or a
\ declaration) and consume tokens until that element is finished parsing. That
\ element is written to memory at "here".
: cparse ( -- )
  ?parseEnum if exit then
  ccmemreserve
  tok< "typedef" s= if parsetype# decl NEXTWORD ! addtype read; exit then
  tokstepback parsetype# ( type )
  \ If it's only a type on a line, it's fine, carry on
  ';' readChar? if drop exit then ( type )
  dup decl ( basetype type name )
  over signature? if ( basetype type name )
    rot drop
    '{' readChar? if parseFunctionBody else parseFunctionProto then
    else parseGlobalDecl then ( ) ;
