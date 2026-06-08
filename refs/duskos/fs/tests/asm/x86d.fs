needs tests/harness lib/str asm/x86d
testbegin

create asmbuf 16 allot

\ we don't compare the whole disasm line, only the "description" part.
: chk"
  [rcompile] " asmbuf begin n< swap c!+ eol? until drop ( str )
  asmbuf word"dis1" exec>str ( expected disline )
  10 + over c@ []>str #s= ;

\test warm up with easy patterns
chk"RET" $c3
chk"ADD  ESI,04" $83 $c6 $04

\test SIB with index register
chk"ADD  EAX,[ESI+EBP]" $03 $04 $2e

\test SIB with index register and scale
chk"ADD  EAX,[ESI+EBP*4]" $03 $04 $ae
testend
