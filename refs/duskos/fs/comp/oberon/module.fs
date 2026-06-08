needs lib/str lib/type lib/struct mem/dict comp/sig comp/sym comp/oberon/gc
unit comp/oberon/module

struct Module {
  [uint,1] types constants variables procedures ;
}

\ not a real type. Used in the "." handler in expr.fs
: _. drop ."Oberon module" ;
: _sz drop 0 ;
: err abort"compiling access to module!" ;
' err ' _sz ' _. 0 newtype addtype moduletype

variable modules
create privmodule Module typesz allot0
privmodule value curmodule
create curmodulename STR_MAXSZ allot
: fullqual ( str -- str ) curmodulename "." strcat swap strcat ;

\ sysconsts is a dictionary for NIL, FALSE, TRUE. +00=type +04=value
variable sysconsts
sysconsts "NIL" entry OPAQUE , 0 ,
sysconsts "TRUE" entry BOOLEAN , 1 ,
sysconsts "FALSE" entry BOOLEAN , 0 ,

: restoremodule ( module -- )
  dup privmodule Module typesz cmove to curmodule ;
: activatemodule ( name -- )
  dup curmodulename strmove
  dup modules find ?dup if nip else
    modules swap entry here# Module typesz allot0 then ( module )
  restoremodule ;

"DUSK" activatemodule curmodule const sysmodule

: curproc ( -- ll )
  privmodule procedures begin ( ll )
    dup not ?err"no curproc!"
    dup e>xt 4+ @ while @ repeat ;

\ "here" is where "curproc" is going to live. Adjust appropriate offsets!
: implementcurproc ( -- )
  here curproc e>xt 4+ ! \ set curproc address
  curmodule procedures @ ?dup if \ maybe set public entry
    e>xt 4+ dup @ if drop else here swap ! then then ;

:~ ( sig name mod -- ) procedures swap entry , 0 , ;
: addproc ( public? sig name -- )
  2dup privmodule ~ rot if curmodule ~ else 2drop then ;

:~ ( type n name mod -- ) constants swap entry swap , , ;
: addconst ( public? type n name -- )
  >r 2dup r@ privmodule ~ rot if r> curmodule ~ else 2drop rdrop then ;

:~ ( type name mod addr -- )
  >r variables swap entry dup , r@ , ( type )
  r> ?addgcslot ;
: addvar ( public? type name -- )
  here# >r \ V1=addr
  over typesz allot0
  2dup privmodule V1 ~ rot if curmodule r> ~ else 2drop rdrop then ;

\ "ob" is to avoid shadowing "addtype" from lib/type
:~ ( type name mod -- ) types swap entry , ;

\ If type is a struct and that it's already in privmodule with typesz=0, it
\ means that it's a forward reference from a POINTER TO. In that case, copy over
\ the struct metadata in the privmodule's entry, then maybe add the entry as a
\ public type.
: ?overrideforwardrecord ( public? type name -- public? type name 0 OR 1 )
  over struct? if
    dup privmodule types find ?dup if
      @ dup typesz if drop else ( ... foundtype ) \ yup, it's a forward ref
        rot over 7 move ( public? name foundtype )
        rot if swap curmodule ~ else 2drop then 1 exit
  then then then 0 ;

: obaddtype ( public? type name -- )
  ?overrideforwardrecord if exit then
  dup fullqual NEXTWORD ! over addtype
  2dup privmodule ~ rot if curmodule ~ else 2drop then ;

: findinmodule ( name module -- ?type ?halop f )
  sysmodule over = if drop findannotated dup if i) 1 then exit then
  2dup procedures find ?dup if nip nip @+ swap @ i) 1 else
    2dup constants find ?dup if nip nip @+ swap @ i) 1 else
      variables find dup if
        @+ swap @ m) over type) 1 then then then ;
: findinmodule# findinmodule not if (wnf) then ;
: findident# ( name -- type halop-or-module )
  case
    sysconsts find ?dup of @+ swap @ i) endof
    findsymbol ?dup of bi type | symbol) endof
    privmodule findinmodule of endof
    modules find# moduletype swap endcase ;

: findbasetype ( name -- type-or-0 )
  dup privmodule types find ?dup if nip else systypes find then dup if @ then ;
: findbasetype# findbasetype ?wnf ;

: findtypeinmodule ( modname-or-0 name -- type-or-0 )
  swap ?dup not if findbasetype exit then
  modules find ?dup not if drop 0 else ( name module )
    sysmodule over = if drop findtype else
      types find dup if @ then then then ;
: findtypeinmodule# findtypeinmodule ?wnf ;

: findprocinmodule ( modname name -- ?sig ?xt f )
  swap modules find ?dup not if drop 0 else ( name module )
    sysmodule over = if
      drop findannotated dup if ( type xt )
        over signature? if 1 else 2drop 0 then then
    else ( name module )
      procedures find dup if @+ swap @ 1 then then then ;
