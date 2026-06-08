needs hal/opq comp/sym comp/tok
unit comp/w

0 value usesW?
: freeW 0 to usesW? ;
: useW 1 to usesW? ;
: freeW# usesW? ?err"W not free!" ;
: useW# freeW# useW ;

: ?pushW doto usesW? 0 | dup if dup, PS+ then ;
: ?popW if freeW# drop, PS- useW then ;
: hasW# usesW? if dup (W? not ?err"not holding W" then ;

: ?PSP+4 dup (src REGPSP = if 4 +) then ;
: ?PSP+n over (src REGPSP = if PSdisp swap- +) else drop then ;

:~ W) &) over = if drop else @, then ;
: W&# ( halop -- ) dup (W? not ?err"W halop expected" ~ ;
: ?W& ( halop -- halop )
  dup (W? if dup W) &) <> if @, W) &) then then ;
: ?>W ( halop -- )
  dup (W? not usesW? and if ?PSP+4 dup, PS+ then
  ~ useW ;
: ?>W$ dup (W? not if freeW# then ?>W freeW ;
: ?2>W dup (W? not usesW? and if dip ?PSP+4 | then ?>W ;

: bothW? ( right left -- Wop PSP 1 OR right left 0 )
  over (W? over (W? and if nip PSP) 1 else 0 then ;

: anyW? ( right left -- Wop other right? 1 OR right left 0 )
  over (W? if 1 1 else dup (W? if swap 0 1 else 0 then then ;
