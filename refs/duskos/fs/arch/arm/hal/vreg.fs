consts 6 REGR0 5 REGR1
: R) 16 lshift $e4009000 or ;
: R0) REGR0 R) ;
: R1) REGR1 R) ;
: R2) 4 R) &) ;
: R3) 3 R) &) ;
: R>) 12 lshift swap $f000 invand or ;
: R0>) REGR0 R>) ;
: R1>) REGR1 R>) ;
: ?saveR0, drop ;
: ?saveR1, drop ;
: ?restoreR0, drop ;
: ?restoreR1, drop ;
