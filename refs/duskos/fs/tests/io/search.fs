needs tests/harness lib/str io/stream io/search
testbegin

stringlist pieces "hello" "hello" "hello" "\n" "\n" "goodbye"
pieces value cur

:> ( n st -- ?a n )
  cur c@ not if 2drop 0 exit then
  >r cur c@+ rot min dup not if nip then r> doto pos over + |
  cur s) to cur ;
' ioerr
newstream const st
: reset ( -- ) 0 st to pos pieces to cur ;

\test matching mechanics
create foo ,"foo"
"foo" to searchfor
0 to sidx
foo 1 match
sidx 1 #eq
match? not #true
foo 1+ 42 match
sidx 3 #eq
match? #true

\test simple search
reset
"ll" st search #true
st pos 2 #eq

\test can find string split in 2
reset
"lohe" st search #true
st pos 3 #eq

\test can find string split in 3
reset
"ohelloh" st search #true
st pos 4 #eq

\test double newline isn't confusing to the algo
reset
"\ngood" st search #true
st pos 16 #eq
testend
