needs lib/type mem/dict comp/tok
unit comp/oberon/type

: openarray? bi array? | arraycount not and ;
: newopenarray 0 swap newarray ;

\ Same as a Pointer, but points exclusively to GCed structs
: gcptrsz drop 4 ;
: gcptr? typeszxt ['] gcptrsz = ;
: _. ."GC*" reftype .type ;
: newgcpointer ( type -- type )
  ['] @, ['] gcptrsz ['] _. 2 newtype swap , ;

variable systypes
:~ ['] @, rot> word newint repeatword dup addtype systypes CURWORD @ entry , ;
1 4 ~ INTEGER
1 1 ~ BYTE
0 1 ~ CHAR
0 1 ~ BOOLEAN
0 4 ~ SET
0 4 ~ OPAQUE

\ Needs to be a struct for GC to work
struct Inner { OPAQUE n ; }
Inner newgcpointer addtype StreamRef
CHAR newopenarray newpointer addtype STRING

: anyptr? bi pointer? | gcptr? or ;
: ?unwrapptr ( type -- type f ) dup pointer? if reftype 1 else 0 then ;
: ?unwrapgcptr ( type -- type f ) dup gcptr? if reftype 1 else 0 then ;
: unwrapptr# ( type -- type )
  ?unwrapptr not if .type nl> err"is not a pointer" then ;

: arraykind# ( type -- reftype count ptr? 1 OR reftype 0 )
  ?unwrapptr over array? not ?err"array expected" ( unwrapped f )
  swap dup openarray? if nip reftype 0 else bi reftype | arraycount rot 1 then ;

: gcptr# ( ptr -- struct )
  dup gcptr? not ?err"RECORD pointer expected" reftype ;
: gcptrorstruct# ( ptr-or-struct -- struct )
  dup gcptr? if reftype then
  dup struct? not ?err"RECORD expected" ;
: anyptr# ( ptr -- unwrappedtype )
  dup gcptr? if reftype else unwrapptr# then ;

: structptr? ?unwrapptr if struct? else drop 0 then ;

alias abort type= ( type expected -- f )

: signature# dup signature? not ?err"signature expected" ;
: signature= ( sig expected -- f )
  r! sigcounts rot r! sigcounts rot = rot> = and not if 2rdrop 0 exit then
  r> siginputs r@ siginputs r> sigcounts + 0 do ( a1 a2 )
    @+ rot @+ rot type= not if break then loop 2drop ( )
  broke? not ;

create compatible BYTE , ushort , uint ,
: ?upg ( type -- type ) dup compatible 3 idx if 2drop INTEGER then ;
: ref? dup pointer? over array? or swap gcptr? or ;
create compatible OPAQUE , AnyPtr ,
: any? compatible 2 idx dup if nip then ;
: string? bi array? | reftype CHAR = and ;
: stringptr? ?unwrapptr if string? else drop 0 then ;
: array=
  over arraycount over arraycount = if 2drop 1 else
    arraycount not swap arraycount not or then ;
:realias type= ( type expected -- f )
  over any? over any? or if 2drop 1 exit then
  dup ref? if
    dup array? if array= exit then
    dip reftype | reftype type= exit then
  dup struct? if containsstruct? exit then
  dup signature? if signature= exit then
  ?upg swap ?upg = ;
: expecttype# ( type expected -- )
  2dup type= if 2drop else
    ."expected: " dup .x spc> .type nl>
    ."got: " dup .x spc> .type nl>
    err"unexpected type" then ;
