needs lib/type lib/str mem/range comp/w comp/oberon/tok comp/oberon/gen
unit comp/oberon

: ob$ obtok$ freeW ;

: oberon<<
  ob$ ['] MODULE word dup curfile! ( str )
  lookup# open exec< ;

: obimport ( s -- )
  ."Loading Oberon module " dup stype nl> idle
  dup str>r
  "oberon/" swap c@+ 8 min []>str
  dup c@+ ['] lowcase cmap[]
  strcat ".mod" strcat NEXTWORD ! oberon<< ( )
  r>str curmodulename over s= if drop else
    ."Module name mismatch. got " curmodulename stype
    ." expected " stype nl> then ;

\ declared in comp/oberon/gen.fs
:realias ?obimport ( s -- ) dup modules find if drop else obimport then ;

: obvar'
  ob$ readIdent '.' readChar readIdent ( mod name )
  swap modules find# variables find# 4+ @ ;

\ only used in tests, parses input stream directly
: oberon< ob$ MODULE ;

: obcode 1 word ob$ symbols$ parsesig read; swap addproc implementcurproc ;
: :ob obcode compile] ;

\ DUSK builtins

: DebugStr ( u a -- ) swap 2dup 0 rot> cidx if nip then rtype ;
annotatelast ( STRING -- )
: DebugVal ( n -- ) .x nl> ;
annotatelast ( OPAQUE -- )
alias emit Emit
annotatelast ( CHAR -- )
: DuskStr ( u a -- str ) swap over zstrlen min []>str ;
annotatelast ( STRING -- OPAQUE )
: ObStr ( u a str -- )
  >r swap 1- r> c@+ rot min ( dst src u )
  rot swap cmove+ 0 swap c! ;
annotatelast ( STRING OPAQUE -- )
