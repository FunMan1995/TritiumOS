needs tests/harness comp/oberon/tok
testbegin

obtok$
: chk< ( -- )
  0 begin tok< dup "STOP" s= not while swap 1+ repeat drop ( ... n )
  dup begin ?dup while word >r 1- repeat ( ... n )
  begin ?dup while swap r> #s= 1- repeat ( ) ;

\test
chk< a+b STOP a + b
chk< a+42H STOP a + 42H
chk< SomeIdent42:=b STOP SomeIdent42 := b
chk< a:b STOP a : b
chk< a..b STOP a .. b
chk< a.b STOP a . b

\test quotes
chk< a"b STOP a " b
chk< a'b STOP a ' b

\test comments
chk< a(*comment followed by space*) +b STOP a + b
chk< a(*comment(*nested*)*)+b STOP a + b
chk< a(*comment*)  (*followed by another*)+b STOP a + b

\test `
chk< `foo STOP ` foo

testend
