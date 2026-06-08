needs tests/harness lib/bit
testbegin

\test bitmask
2 bitmask $04 #eq
31 bitmask $80000000 #eq
5 bitmask inv $ffffffdf #eq
$42 0 bit? not #
$42 1 bit? #
$42 2 bit1! $46 #eq
$42 1 bit0! $40 #eq

\test bitfield
4 6 bitfield foo
$12345eaf foo 42 #eq
42 $12345110 to foo $123452a0 #eq

\test sex
$7f sex8 $7f #eq
$80 sex8 $ffffff80 #eq
$7fff sex16 $7fff #eq
$8000 sex16 $ffff8000 #eq

\test swapbits
$12c0 swapbits8 $1203 #eq

\test lrot
$f071f023 01 lrot $e0e3e047 #eq
$abf03213 13 lrot $0642757e #eq
$fa00b100 07 lrot $0058807d #eq
$01234578 04 lrot $12345780 #eq

\test rrot
$f071f023 01 rrot $f838f811 #eq
$abf03213 13 rrot $909d5f81 #eq
$fa00b100 07 rrot $01f40162 #eq
$01234578 04 rrot $80123457 #eq

testend
