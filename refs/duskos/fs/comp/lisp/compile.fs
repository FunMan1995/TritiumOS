needs lib/wordtbl mem/cons comp/lisp/sig comp/lisp/ast
unit comp/lisp/compile
\ In the code below, we exit the "cute and cuddly" part of a Lisp and dive into
\ compiling wickedness and awesomeness. Yes, it's mind bending, but it's also
\ what makes this lisp crazy fast.

\ PS-relative offsets are relative to the "baseline" of the function, that is,
\ its PS level at prelude. Because we have a PS top register, offset 0 refers
\ to W, offset 1 to PSP+0, offset 2 to PSP+4, etc. In a "(lambda (x y z) ...)"
\ Z lives in W, y in PSP+0, x in PSP+4.
\ When we construct a function all, however, we need to place arguments on PS
\ and that changes things. If I'm preparing to call "((lambda (a b)) y z)", I
\ will first do, for "y", a "dup, PSP) 4 +) @," (the dup, immediately changed
\ PSP+0 into PSP+4), but when comes the time to compile "z", if I try to do
\ "dup, PSP) 0 +) @,", my offset will be wrong because of the "y" I have just
\ placed! Therefore, whenever we place arguments on PS after the function
\ prelude, we need to increase the "psoff" variable below by 1. Then, when we
\ compile arguments, we apply this offset on top of what comes from the
\ compiling context. After a function call, we decrease this offset by the
\ argument count of the function.
0 value psoff
: ps+ doto psoff 1+ | ;
0 value tailcalladdr

\ Freeze "ctxcnt" arguments from the "..." part of PS into a hardcoded
\ compilation of those args. Those frozen arguments will be placed *under*
\ runtime arguments, which are expected to be "argcnt" in size. For example,
\ if called with "42 43 2 1 0", it will generate code that, if called with
\ "... 44" in PS will result in PS becoming "... 42 43 44".
\ "digdepth is for when "..." is deeper. For example, "42 43 123 2 1 1" will
\ result in code that when called with "44", result in PS becoming "42 43 44"
: ctxfreeze ( ... ctxcnt argcnt digdepth -- ... )
  >r \ V1=digdepth
  dup not ?abort"TODO: support argcnt=0"
  over neg 4* ps+, ( ctx arg )
  dup 1 > if
    dup 1- 0 do ( ctx arg )
      over i + 4* PSP) swap +) S>) @,
      PSP) i 4* +) !, loop then ( ctx arg )
  0 rot do ( ... arg )
    i V1 + dig i) S>) @,
    i over + 1- 1- 4* PSP) swap +) S>) !, 1 -loop
  drop rdrop ;

alias abort funcall,
alias abort lambda,
alias abort arg,
: err abort"lisp compile error" ;
: number, dup iscons? if dup leak then litn ;
: psoff, ( data -- ) psoff + dup, ?dup if PSP) swap 4* +) @, then ;
: to, ( data -- ) carcdr arg, dup, execute, ;

wordtbl[ ( data -- )
  ' number,
  ' execute,
  ' psoff,
  ' funcall,
  :> ( LAMBDA )
    fbr, over lambda, ( data br w )
    rot cdr carcdr car ( br w argcnt depth )
    over - max0 ?dup if ( br w argcnt ctxsz )
      here# >r ( ... br w argcnt ctxsz )
      psoff 2 + ctxfreeze bbr, r> ( br w )
      else drop then ( br w )
    swap here br! litn ;
  ' err ( LET )
  ' to,
]wordtbl argtbl
:realias arg, ( node -- ) cdrcar argtbl swap wexec ps+ ;

alias abort exprs,
: chkcnt ( n n -- ) <> ?abort"wrong argument count" ;
0 value curargcnt
wordtbl[ ( data -- )
  ' err ( NUMBER )
  :> ( CALLABLE )
    dup ?argcnt if curargcnt chkcnt then
    dup execute,
    ?noret if 0 litn then ;
  :> ( LOCVAR ) psoff + ?dup if 1- 4* PSP) swap +) else W) &) then brr, ;
  :> ( FUNCALL ) funcall, A) &) !, drop, A) &) brr, ;
  \ In the particular case of compiling a FUNCALL that targets a lambda,
  \ the "lambda," word is overkill. Our arguments are already on PS and we're
  \ only going to call this lambda once. So, we can just inline exprs!
  :> ( LAMBDA )
    doto psoff 0 | >r
    here to tailcalladdr
    cdr cdrcar curargcnt chkcnt cdr exprs, curargcnt 4* ps+,
    r> to psoff ;
]wordtbl calltbl
:realias funcall, ( node -- )
  dup car cdrcar CALLABLE <> if drop else
    dup ?compiler if dip cdr | execute exit then drop then ( node )
  carcdr tuck length doto curargcnt swap | >r ( argnodes callnode ) \ V2=oldcnt
  >r begin ( argnodes ) \ V3=callnode
    ?dup while cdrcar ( argnodes argnode ) arg, repeat ( )
  r> ( callnode ) cdrcar calltbl swap wexec
  r> doto curargcnt swap | doto psoff swap- | ;

\ Lambda generation
\ Here's another fresh new level of mind twisting. When a lambda references
\ arguments from outer scope, for example in:
\ (lambda (x) (lambda (y) (+ x y)))
\ it stops being a simple lambda and becomes a lambda generator. Calling the
\ outer lambda generates a push of "x" to PS followed by a jump to the inner
\ lambda. This way, the "overreaching" of "x" in the expr (+ x y) will reference
\ the proper number. It works pretty much like the sysword "bind".
: lambdagen, ( w argcnt ctxsz -- )
  compile here#
  litn litn 1 litn compile ctxfreeze ( w )
  litn compile bbr, ;

alias abort expr,
wordtbl[ ( data -- )
  ' number,
  ' err ( CALLABLE )
  ' psoff,
  ' funcall,
  :> ( LAMBDA )
    fbr, over lambda, swap here br! ( data tgt )
    swap cdr carcdr car ( tgt argcnt depth )
    over - max0 ?dup if lambdagen, else drop litn then ;
  :> ( LET )
    cdrcar ( exprnodes initnodes )
    \ here, we are a bit weird: we begin with a negative psoff that goes towards
    \ neutral as we add variables to PS.
    dup length doto psoff over - | >r \ V1=varcnt
    begin ?dup while cdrcar expr, repeat ( exprnodes )
    exprs,
    r> ( varcnt ) 4* ps+, ;
  ' to,
]wordtbl exprtbl
:realias expr, ( node -- ) cdrcar exprtbl swap wexec ps+ ;

:realias exprs, ( nodes -- )
  \ generating all expr but the last one
  dup not ?abort"empty body!" begin ( expr )
    dup cdr while cdrcar expr, drop, doto psoff 1- | repeat ( lastexpr )
  car expr, ;

:realias lambda, ( data -- w ) ( name-or-0 argcnt exprnodes )
  0 to psoff
  cdrcar ?dup if SYSDICT swap entry then
  cdrcar here# 2>r ( cdr ) \ V1=argcnt V2=w
  cdrcar >r ( exprnodes ) \ V3=depth
  pushlr, here to tailcalladdr exprs,
  r> V1 max 4* ps+, popexit,
  2r> tuck addsig ;

: (if ( data -- )
  carcdr carcdr single# ( cond true false )
  rot expr, [compile] if doto psoff 1- | ( true false br )
  rot expr, [compile] else doto psoff 1- | ( false br )
  swap expr, [compile] then ;
1 compiler current addsig
: tailcall ( data -- )
  dup length >r begin ( argnodes ) \ V1=argscnt
    ?dup while cdrcar ( args arg ) arg, repeat
  V1 0 do PSP) V1 1- 4* +) !, drop, loop
  tailcalladdr bbr, r> ( argcnt ) doto psoff swap- | ;
1 compiler current addsig
