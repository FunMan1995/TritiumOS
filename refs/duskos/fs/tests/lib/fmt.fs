needs tests/harness lib/fmt
testbegin
\test
10 11 12 13 "foo %b bar %w baz %x bleh %d" sprintf
"foo 0d bar 000c baz 0000000b bleh 10" #s=

\test
create s0 ,"null-terminated\0"
s0 "hello" 'X' "foo %c bar %s baz %z" exec>str printf
"foo X bar hello baz null-terminated" #s=

\test A format string ending with a % doesn't crash the machine
42 "hey %d %" sprintf "hey 42 " #s=
testend
