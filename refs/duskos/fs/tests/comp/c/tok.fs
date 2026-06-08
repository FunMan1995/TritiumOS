needs tests/harness comp/c
testbegin
cctok$
: checktok ( expected n -- ) 0 do dup tok< #s= s) loop drop ;

\test
stringlist expected short retconst ( ) { return 42 ; }
expected 9 checktok
short retconst() {
    return 42;
}

\test A ' ' literal is valid in C even if it's not in Forth
stringlist expected void emitspc ( ) { emit ( 32 ) ; }
expected 11 checktok
void emitspc() { emit(' '); }

: checktok2 checktok ?tok< 0 #eq ;
\test Check whether we behave correctly when file ends with non-WS
stringlist expected foo bar { }
expected 4 ' checktok2 exec<< /tests/comp/c/nonl.txt
testend
