needs mem/dict mem/stack
unit mem/alloc

: entry+ ( payloadsz 'dict str -- payload ) rot r! reserveentry r> allot@ ;

8 newstack const s

: alloc[ CURALLOC @! s push alignhere ;
: ]alloc s pop CURALLOC ! ;
: ]sysmem ( -- ) s empty SYSALLOC CURALLOC ! ;
: newallocator ( newhere a u -- alloc ) here# >r over , + , , r> ;

: bindalloc[ ['] alloc[ bind ;
: _run, ( alloc -- ) litn compile alloc[ compile run1 compile ]alloc ;
: _compile, ( alloc -- ) litn compile alloc[ ' execute, compile ]alloc ;
: bindrun1 ( alloc "name" -- )
  code> over litn ['] _compile, bbr,
  code> pushlr, rot _run, popexit,
  compiling ;
: _, _compile, dup, PREVRESERVE m) @, ;
: bindrun1@ ( alloc "name" -- )
  code> over litn ['] _, bbr,
  code> pushlr, rot _run, dup, PREVRESERVE m) @, popexit,
  compiling ;

SYSALLOC bindalloc[ sys[
