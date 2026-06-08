needs tests/harness gr/color
testbegin

\test >rgb565
$123456 >rgb565 $11aa #eq

\test >rgb555
$123456 >rgb555 $8ca #eq

\test >gray8
$123456 >gray8 $52 #eq
$ff8822 >gray8 $ff #eq

\test >gray4
$123456 >gray4 $5 #eq
$ff8822 >gray4 $f #eq

\test >gray2
$123456 >gray2 1 #eq
$ff8822 >gray2 3 #eq
testend
