consts 24 REGR0 25 REGR1
: R) $240 swap rs1! ;
: R0) REGR0 R) ;
: R1) REGR1 R) ;
: R2) 26 R) &) ;
: R3) 27 R) &) ;
: R0>) REGR0 rd! ;
: R1>) REGR1 rd! ;
: ?saveR0, drop ;
: ?saveR1, drop ;
: ?restoreR0, drop ;
: ?restoreR1, drop ;
