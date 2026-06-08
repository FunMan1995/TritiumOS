needs lib/str lib/psrs lib/time io/stream fs/core text/ts
unit fs/sh

\ struct: curfs, walksize, walkmtime, walkdir?, walkcontext, walkname, walkpath
STR_MAXSZ 2* WALKERCTXSZ + 3 4* + const BUFSZ
create srcbuf BUFSZ allot0
create dstbuf BUFSZ allot0

: savectx ( buf -- )
  >r walkdir? walksize walkmtime curfs r> !+ !+ !+ !+ ( buf+ )
  walkcontext swap WALKERCTXSZ cmove+ ( buf+ )
  walkname over strmove s) ( buf+ )
  walkpath swap strmove ;

: ?restorectx ( buf -- )
  @+ ?dup not if drop exit then ( buf+ curfs )
  to curfs @+ to walksize @+ to walkmtime @+ to walkdir? ( buf+ )
  dup walkcontext WALKERCTXSZ cmove WALKERCTXSZ + ( buf+ )
  dup walkname strmove s) ( buf+ )
  walkpath strmove ;

0 value ondst?
: walksrc doto ondst? 0 | if dstbuf savectx srcbuf ?restorectx then ;
: walkdst doto ondst? 1 | not if srcbuf savectx dstbuf ?restorectx then ;

: '>r ( a -- ) m) A>) @, RSP) A>) -!, -4 [rcnt] +! ;
: walk>r
  walkpath litn [compile] str>r
  walkname litn [compile] str>r
  compile walkcontext WALKERCTXSZ litn [compile] []>r
  addrof walkdir? '>r
  addrof walkmtime '>r
  addrof walksize '>r
  addrof curfs '>r ; immediate

: r>' ( a -- ) RSP) A>) @+, 4 [rcnt] +! m) A>) !, ;
: r>walk
  addrof curfs r>'
  addrof walksize r>'
  addrof walkmtime r>'
  addrof walkdir? r>'
  [compile] r>[] compile walkcontext swap, compile cmove
  [compile] r>str walkname litn compile strmove
  [compile] r>str walkpath litn compile strmove ; immediate

: ensurepath ( dir? path -- )
  c@+ begin iterpath while ( dir? a u a u )
    []>str lookupchild not if lookupname newdir then
    enterdir repeat ( dir? a u )
  []>str lookupchild if
    walkdir? <> ?abort"ensurepath dir? inconsistency"
    else lookupname swap addfsnode then ;

:> [compile] " compile lookup# ;
:> [rcompile] " lookup# ;
compiling p"
:> [compile] p" compile open ;
:> [rcompile] p" open ;
compiling f"

: _ ( dir? str -- ) walktopathroot ensurepath ;
: ensurefile 0 swap _ ;
: ensuredir 1 swap _ ;
: ensuredst walksrc walkdir? walkname str>pool walkdst ensurepath walksrc ;

: _c litn [compile] " compile _ ;
: _r [rcompile] " _ ;
:> 0 _c ; :> 0 _r ; compiling pf"
:> 1 _c ; :> 1 _r ; compiling pd"

: .walkpath ( -- )
  fsletter ?dup if emit .":" then
  walkpath walkname pathcat stype ;
: .walk ( -- )
  ts[ .walkpath spc> 30 tsgo
  walkdir? if "[DIR]" c@+ else walksize formatdec then ( a u )
  10 over - nspcs rtype spc> walkmtime .time nl> ]ts ;

:~ ( ?enterxt doxt -- ?enterxt doxt )
  enterdir begin gotonext while
    walkdir? if
      over execute if walk>r ~ r>walk then
      else dup execute then repeat ;
: walkdo ~ 2drop ;

:~ .walk 0 ;
: listdir walkdir? if ['] ~ ['] .walk walkdo else .walk then ;
:~ .walk 1 ;
: listtree ['] ~ ['] .walk walkdo ;

\ In all of the words below, we generally stay on "walksrc", only switching
\ to "walkdst" when needed, then switching back.
: copyfile ( -- )
  walkdst open walksrc open 2dup size swap resize spitcloseboth walksrc ;

: copyfile.
  ."Copying " walksrc .walkpath ." --> " walkdst .walkpath walksrc nl> idle
  copyfile ;

: ensurecopyfile. ensuredst copyfile. ;

: enterboth walkdst enterdir walksrc enterdir ;
:~ ( ?enterxt doxt -- ?enterxt doxt )
  enterboth begin gotonext while
    walkdir? if
      over execute if
        walk>r ensuredst walkdst walk>r walksrc
        ~ walkdst r>walk walksrc r>walk then
      else dup execute then repeat ;
: walkdoboth ~ 2drop ;

: copyall ['] ONE ['] ensurecopyfile. walkdoboth ;

: copylist ( stringlist -- )
  walkdst enterdir walksrc begin dup c@ while ( sl )
    dup lookup# ( sl )
    walkdir? walkdst walk>r over ensurepath
    walkdir? if copyall else copyfile. then
    walkdst r>walk walksrc s) repeat ;
