(addsig (noret 2) '#eq)
(addsig (noret 1) '#true)
(defun #false (f) (#true (not f)))
(#eq 5 (+ 2 3))
(#eq 37 (- 42 (+ 2 3)))
(#eq 5 (apply '+ '(2 3)))
(#eq 5 (eval '(+ 2 3)))
(#true (equal 42 42))
(#false (equal 41 42))
(#false (equal '(1 2 3) 42))
(#false (= '(1 2 3) '(1 2 3)))
(#true (equal '(1 2 3) '(1 2 3)))
(#eq 43 (+ answertouniverse 1))

(#eq 2 ((lambda (a b) (- b a)) 2 4))
(defun twice (f x) (f (f x)))
(#eq 12 (twice '1+ 10))
(#eq 10000 ((lambda (f x) (f (f x))) (lambda (y) (* y y)) 10))
(#eq 43 ((lambda (x) ((lambda (x) (+ x 1)) x)) 42))

(addsig (noret 0) 'noop)
(#eq 0 (noop))

(#eq 42 ((lambda (n) ((lambda (n a b) n) n 0 1)) 42))

(#eq 21 ((lambda (x) ((lambda (x f) (f 1)) 10 (lambda (z) (+ z x)))) 20))
(#eq 44 ((lambda (x)
  (lambda (y) (+ x 1))
  ((lambda (z) (+ x 2)) 12))
  42))

(#true (equal (map '1+ '(1 2 3)) '(2 3 4)))
(#true (equal
  (map (lambda (n) (* n 3)) '(1 2 3))
  '(3 6 9)))

\ tail optimized fibonacci
(defun fib (n)
  ((lambda (n a b) (if n (tailcall (- n 1) b (+ a b)) a)) n 0 1))
(#eq 0 (fib 0))
(#eq 1 (fib 1))
(#eq 1 (fib 2))
(#eq 2 (fib 3))
(#eq 5 (fib 5))
(#eq 34 (fib 9))

(defun foo (n) (let ((a 54) (b (+ n answertouniverse))) (- a b)))
(#eq 10 (foo 2))

(defun scale+ (mul add) (lambda (n) (+ (* n mul) add)))
(#eq 169 ((scale+ 4 1) 42))
