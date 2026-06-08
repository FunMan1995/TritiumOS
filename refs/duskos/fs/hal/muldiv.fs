needs lib/type num/math hal/opq
unit hal/muldiv

: _one? ( op -- ?op f ) dup (i? if 1 = if drop 1 exit then then 0 ;
: _shiftable? ( op -- op f )
  dup (i? if ?dup if log2mod swap if drop else ( op n )
    \ we don't want to create a new i) because we want to preserve other
    \ attributes of "op".
    over (slot hbank' ! 1 exit then then then 0 ;
: mulorshift, ( op -- ) _one? not if _shiftable? if <<, else *, then then ;
: divorshift, ( op -- ) _one? not if _shiftable? if >>, else /mod, then then ;
:~ ( op -- )
  dup /mod, dup (dst REGS = if drop else ( op )
    dup (dir? if S>) !, else (dst S) &) swap dst) @, then then ;
: modorand, ( op -- )
  dup (i? not if ~ else ( op n )
    log2mod swap if drop ~ else ( op n )
      pow2 1- i) swap (dst dst) &, then then ;

: [*n] ( n -- n ) n< i) mulorshift, ; immediate
: [*n+] ( idx a -- a ) PSP) S>) @+, n< i) S>) mulorshift, S) &) +, ; immediate
