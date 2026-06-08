needs tests/harness gr/damage
testbegin

\test simple damage with no scaling
1 2 3 4 damage 1 21 lshift 2 14 lshift or 4 7 lshift or 6 or #eq

\test damage with 2x scaling
128 4 6 8 damage
  1 28 lshift 64 21 lshift or 2 14 lshift or 67 7 lshift or 6 or #eq

\test damagerect
1 2 3 4 damage damagerect 4 #eq 3 #eq 2 #eq 1 #eq

\test damagerect yields upper bounds for maxx and maxy
128 2 3 4 damage damagerect 5 #eq 3 #eq 2 #eq 128 #eq

\test damage+ same scale
1 2 3 4 damage 2 3 4 5 damage damage+
damagerect 6 #eq 5 #eq 2 #eq 1 #eq

\test damage+ different scale
128 2 3 4 damage 2 3 4 5 damage damage+
damagerect 7 #eq 129 #eq 2 #eq 2 #eq

\test damage+ with 0 on one side
$cafebabe 0 damage+ $cafebabe #eq
0 $cafebabe damage+ $cafebabe #eq

\test damage+ never descales
\ This example used to yield a smaller damage because of scale mixup
$3006355d $422b48ae damage+ $40031aae #eq

testend
