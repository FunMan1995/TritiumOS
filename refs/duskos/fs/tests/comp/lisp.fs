needs tests/harness mem/cons comp/lisp comp/lisp/ast
testbegin

\test
42 value answertouniverse
ast$
ast< 42 carcdr 42 #eq NUMBER #eq
ast< answertouniverse carcdr ' answertouniverse #eq CALLABLE #eq
ast< 'answertouniverse carcdr ' answertouniverse #eq NUMBER #eq
ast< (+ \this is a comment
  "foo" 3)
  cdrcar FUNCALL #eq
    cdrcar
      carcdr ' + #eq CALLABLE #eq
    cdrcar
      carcdr "foo" #s= NUMBER #eq
    single#
      carcdr 3 #eq NUMBER #eq
ast< '(+ "foo" 3)
  cdrcar NUMBER #eq
    cdrcar ' + #eq
    cdrcar "foo" #s=
    single# 3 #eq
ast< '(1 . 2)
  cdrcar NUMBER #eq
    carcdr 2 #eq 1 #eq

\test
lisp (+ 2 3) 5 #eq
lisp answertouniverse 42 #eq
lisp 'answertouniverse ' answertouniverse #eq
lisp '(1 (2 3) 4)
  cdrcar 1 #eq
  cdrcar
    cdrcar 2 #eq
    carcdr 0 #eq 3 #eq
  carcdr 0 #eq 4 #eq
lisp '(1 . 2) carcdr 2 #eq 1 #eq
lisp '((1 . 2) . (3 . 4))
  carcdr
    carcdr 4 #eq 3 #eq
    carcdr 2 #eq 1 #eq

\test
lisp. (defun foo (a b) (- b a))
2 3 foo 1 #eq
lisp. (defun foo (n) n)
42 foo 42 #eq

\test
lisp (to answertouniverse 44) 44 #eq
answertouniverse 44 #eq
lisp. (defun foo (n) (to answertouniverse n))
43 foo 43 #eq
answertouniverse 43 #eq
lisp. (defun foo (n) (+ 2 (to answertouniverse n)))
42 foo 44 #eq
answertouniverse 42 #eq

\test we can have more than one list in function bodies
variable myvar
: foo 42 myvar ! ;
0 noret current addsig
lisp. (defun bar () (foo) (+ 2 3))
myvar @ 0 #eq
bar 5 #eq
myvar @ 42 #eq

\test
exec>str lisp (stype "Hello World!")
"Hello World!" #s= 0 #eq

\test
lisp (defun foo (n) (if n 42 54)) drop
0 foo 54 #eq
1 foo 42 #eq

\test
lisp<< tests/comp/lisp.l

\test compiled quotes cons references are properly leaked
lisp. (defun foo () '(1 2 3))
lisp (equal (foo) '(1 2 3)) #true
: fillcons CONSCNT 0 do 0 0 cons drop loop ;
fillcons
lisp (equal (foo) '(1 2 3)) #true
testend
