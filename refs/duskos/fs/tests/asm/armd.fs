needs tests/harness lib/str asm/arm asm/armd
testbegin

variable asmbuf

\ we don't compare the whole disasm line, only the "description" part.
: chk" ( opcode -- )
  asmbuf ! [rcompile] " ( str )
  asmbuf to dpc word".op" exec>str ( expected disline )
  10 + over c@ []>str #s= ;

\test warm up with easy patterns
add) r1 rd) r2 rn) 42 imm)         chk"   ADD  r1  r2  0000002a"
add) ne) r1 rd) r2 rn) r3 rm)      chk"NE ADD  r1  r2  r3"

\test imm shift
mov) r1 rd) r2 rm) 3 asr)          chk"   MOV  r1  r2 asr 3"

\test reg shift
sub) r1 rd) r2 rn) r3 rm) r4 rror) chk"   SUB  r1  r2  r3 ror r4"

\test PS leak on ??? desc
$2f6d7361                          chk"CS ???"

testend
