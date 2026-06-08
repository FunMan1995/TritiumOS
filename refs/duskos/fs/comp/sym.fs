needs lib/struct lib/tagl mem/arena comp/tok
unit comp/sym

newarena bindrun1 arena1

enum GLOBAL LOCAL ARGUMENT
struct Symbol { uint type class offset ; }

\ these entries are *not* executable, payload is directly the symbol
variable localsyms
0 value localvariablesz
0 value argumentsz
0 value PSdisp

: newsym ( offset class type -- sym ) here# >r , , , r> ;

: addglobalsymbol ( type name -- symbol )
  NEXTWORD ! create here over typesz allot0 ( type a )
  GLOBAL rot newsym ( sym )
  dup current n"SYMB" rot addtag ;

: localentry ( name -- )
  localsyms swap Symbol typesz arena1 reserveentry ;
: alignlocal ( type -- ) localvariablesz swap typealign to localvariablesz ;
: addlocalvariable ( type name -- symbol )
  localentry dup alignlocal >r
  localvariablesz LOCAL r@ arena1 newsym ( sym )
  r> typesz doto localvariablesz + | ;

: growargzone ( -- ) doto argumentsz 4+ | ;
: addargument ( type name -- symbol )
  localentry >r argumentsz ARGUMENT r@ arena1 newsym ( sym )
  r> typesz 4 > ?err"type doesn't fit in PS" growargzone ;

: PS+ doto PSdisp 4+ | ;
: PS- doto PSdisp 4- | ;

: symbol) ( symbol -- halop )
  tri type | offset | class case
    GLOBAL = of m) swap type) endof
    LOCAL = of RSP) swap +) swap type) endof
    ARGUMENT = of PSdisp + PSP) swap +) nip endof \ PSP access always 32-bit
    abort"broken Symbol" endcase ;

: findsymbol ( name -- symbol-or-0 )
  dup localsyms find ?dup if nip else
    sysdict find dup if
      n"SYMB" findtag dup if drop then then then ;

: findsymbol# findsymbol ?wnf ;

: argtypes ( -- ... n )
  0 >r localsyms @ begin ( ... ll ) \ V1=cnt
    ?dup while
    dup e>xt dup class ARGUMENT = if
      doto V1 1+ | type swap  else drop then ( ll )
    @ repeat ( ... ) r> ;

: .localsymbols
  localsyms @ begin ?dup while
    dup entryname[] rtype spc> dup e>xt
    dup class . spc>
    dup offset .x spc>
    type .type nl>
    @ repeat ;

: symbols$
  0 to argumentsz 0 to localvariablesz
  0 localsyms ! 0 to PSdisp arena1 reset ;

: savesymstate localsyms @ localvariablesz argumentsz PSdisp ;
: restoresymstate to PSdisp to argumentsz to localvariablesz localsyms ! ;
