consts 7 REGR0 1 REGR1
$107 const R0) \ EDI
$101 const R1) \ ECX
$10000100 &) const R2) \ R8
$10000101 &) const R3) \ R9
: R0>) $38 or ;
: R1>) $38 invand $08 or ;
: ?saveR0, R0>) !, ;
: ?saveR1, R1>) !, ;
: ?restoreR0, R0>) @, ;
: ?restoreR1, R1>) @, ;
