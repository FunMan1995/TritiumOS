needs lib/str mem/cons comp/lisp/ast comp/lisp/exec comp/lisp/builtin
unit comp/lisp

: lisp ast$ env$ ast< exec ;
: lisp. lisp .cons ;
: _ begin toword? while env$ ast< exec drop repeat ;
: lisp<< ast$ ['] _ exec<< ;
lisp<< comp/lisp/core.l
