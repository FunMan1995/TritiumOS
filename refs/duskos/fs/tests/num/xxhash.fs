needs tests/harness num/xxhash
testbegin
0 value XXH_TEST_INPUT
here to XXH_TEST_INPUT

map< c,
$32 $FA $AC $AB $67 $44 $32 $B5 $19 $78 $CD \
$59 $01 $23 $45 $89 $EF $20 $00 $11 $22 $33 \
$44 $55 $66 $77 $88 $99 $AA $BB $CC $DD $EE \
$FF

\test xxh32[] standalone function
XXH_TEST_INPUT 34 9 xxh32[] $3c92d380 #eq

\test xxh32[] here stability
\ test that the xxh32[] doesn't change the value of here
\ e.g. doesn't allocate more memory 
here XXH_TEST_INPUT 34 $ABCDEF23 xxh32[] drop here #eq

\test XXH32 alignment
\ XXH32 must be correctly aligned to avoid any issue
XXH32 typesz 3 and 0 #eq

\test newXXH32
9 newXXH32 const XXH_TEST_STRUCT
XXH_TEST_STRUCT accs      @ $24234431 #eq
XXH_TEST_STRUCT accs 4 +  @ $85ebca80 #eq
XXH_TEST_STRUCT accs 8 +  @ $00000009 #eq
XXH_TEST_STRUCT accs 12 + @ $61c88658 #eq

\test _consumeLong
XXH_TEST_INPUT 34 XXH_TEST_STRUCT accs _consumeLong drop
XXH_TEST_STRUCT accs      @ $0dd85a82 #eq
XXH_TEST_STRUCT accs 4 +  @ $9ad19b05 #eq
XXH_TEST_STRUCT accs 8 +  @ $611fa655 #eq
XXH_TEST_STRUCT accs 12 + @ $7f0d3ed3 #eq

\test XXH32 basic process
9 XXH_TEST_STRUCT reset
XXH_TEST_INPUT 34 XXH_TEST_STRUCT update
XXH_TEST_STRUCT total_len 34 #eq
XXH_TEST_STRUCT digest $3c92d380 #eq

\test small input
$CAFEDEAD XXH_TEST_STRUCT reset
XXH_TEST_INPUT 9 + 13 XXH_TEST_STRUCT update
XXH_TEST_STRUCT digest
XXH_TEST_INPUT 9 + 13 $CAFEDEAD xxh32[] #eq

$12345678 XXH_TEST_STRUCT reset
XXH_TEST_INPUT $1f + 5 XXH_TEST_STRUCT update
XXH_TEST_STRUCT digest
XXH_TEST_INPUT $1f + 5 $12345678 xxh32[] #eq

\test XXH32.update
\ multistep
$CAFEBABE XXH_TEST_STRUCT reset
XXH_TEST_INPUT         13 XXH_TEST_STRUCT update
XXH_TEST_INPUT 13 +     9 XXH_TEST_STRUCT update
XXH_TEST_INPUT 13 9 + + 4 XXH_TEST_STRUCT update
XXH_TEST_STRUCT digest
\ singlestep
$CAFEBABE XXH_TEST_STRUCT reset
XXH_TEST_INPUT         26 XXH_TEST_STRUCT update
XXH_TEST_STRUCT digest dup #eq
\ standalone function
XXH_TEST_INPUT 26 $CAFEBABE xxh32[] #eq

\test XXH32.readbuf endianness
create RESULT map< c, $80 $d3 $92 $3c
9 XXH_TEST_STRUCT reset
XXH_TEST_INPUT 34 XXH_TEST_STRUCT update
:~ 4 0 do
    1 XXH_TEST_STRUCT readbuf
    1 #eq c@ RESULT i + c@ #eq
  loop ;
~

\test XXH32.writebuf
9 XXH_TEST_STRUCT reset
XXH_TEST_INPUT 34 XXH_TEST_STRUCT writebuf 34 #eq
XXH_TEST_STRUCT digest $3c92d380 #eq

\test XXH32.readbuf guards
9 XXH_TEST_STRUCT reset
XXH_TEST_INPUT 34 XXH_TEST_STRUCT update
3     XXH_TEST_STRUCT readbuf  3 #eq drop
0 $ff XXH_TEST_STRUCT writebuf 0 #eq 
0     XXH_TEST_STRUCT readbuf  0 #eq scntneutral#
9     XXH_TEST_STRUCT readbuf  1 #eq drop

\test XXH32 stream API
$F00BA781 XXH_TEST_STRUCT reset
XXH_TEST_INPUT      20 XXH_TEST_STRUCT writebuf 20 #eq
XXH_TEST_INPUT 20 + 13 XXH_TEST_STRUCT writebuf 13 #eq
4 XXH_TEST_STRUCT readbuf 4 #eq le@
XXH_TEST_INPUT 33 $F00BA781 xxh32[] #eq 

\ once an XXH's output has been exhausted, it must be resetted again
0 XXH_TEST_STRUCT readbuf 0 #eq
$ffffffff $83 XXH_TEST_STRUCT writebuf 0 #eq

\ test the close functionality
$F00BA781 XXH_TEST_STRUCT reset
XXH_TEST_INPUT 13 XXH_TEST_STRUCT writebuf 13 #eq
XXH_TEST_STRUCT close
$fefe     XXH_TEST_STRUCT readbuf  0 #eq scntneutral#
$efef $83 XXH_TEST_STRUCT writebuf 0 #eq

testend
