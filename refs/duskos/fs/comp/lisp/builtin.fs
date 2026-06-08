needs lib/str mem/cons comp/lisp/ast
unit comp/lisp/builtin

: apply ( w list -- res ) swap >r begin ?dup while carcdr repeat r> execute ;
: eval ( list -- res ) ?carcdr if apply then ;

: map ( w list -- list )
  carcdr dup if dipover map then ( w car cdr )
  swap rot execute ( cdr car ) swap cons ;

: equal ( a b -- f )
  2dup = if 2drop 1 else
    over iscons? over iscons? and if
      over car over car equal ( a b f )
      swap cdr rot cdr equal and ( f )
      else 2drop 0 then then ;
