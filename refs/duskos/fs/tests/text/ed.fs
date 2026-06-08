needs tests/harness mem/stream io/stream text/ed
testbegin

\test wordunder
"foo " "hello foo bar" c@+ 7 wordunder #s[]=
"foo " "hello foo bar" c@+ 9 wordunder #s[]=
"bar" "hello foo bar" c@+ 11 wordunder #s[]=
"bar" "hello foo bar" c@+ 42 wordunder #s[]=

\ Testing edbuf within an app/gcon is tricky. Emitted output fights with the
\ tested edbuf. This is why we need this tested/origed scheme.

curbuf @ const origbuf
addedbuf curbuf @ const testbuf
: tested testbuf curbuf ! ;
: origed origbuf curbuf ! ;
: s
  curline printline
  epos cpos nspcs '^' emit nl>
  epos lpos . ." / " linecnt . ;
: #cs= word"s" exec>str #s= ; \ "captured status equals"

origed
\test edbuf operations
tested
linecnt 1 #eq
1 dellines
linecnt 1 #eq
edload<< data/tests/txtfile
1 goup epos lpos 0 #eq
1 go
nextword
"with some text\n     ^\n1 / 17" #cs=
7 godown
"be\n  ^\n8 / 17" #cs=
5 goup
"(oh well maybe grow)\n  ^\n3 / 17" #cs=
"maybe" edfind
"(oh well maybe grow)\n         ^\n3 / 17" #cs=
eol
"(oh well maybe grow)\n                    ^\n3 / 17" #cs=
bol
"(oh well maybe grow)\n^\n3 / 17" #cs=
4 goright 5 delchars
"(oh maybe grow)\n    ^\n3 / 17" #cs=
epos 6 + delto
"(oh grow)\n    ^\n3 / 17" #cs=
2 dellines
"tests that process text\n    ^\n3 / 15" #cs=
eol "abc" edstream puts
"tests that process textabc\n                          ^\n3 / 15" #cs=
appendline "appended line" edstream puts
"appended line\n             ^\n4 / 16" #cs=
4 goleft "hello" edstream puts
"appended helloline\n              ^\n4 / 16" #cs=
insertline "inserted line" edstream puts
"inserted line\n             ^\n4 / 17" #cs=
0 go 1 dellines
"with some text\n             ^\n0 / 16" #cs=
0 go insertline "at the beginning of the buf" edstream puts
"at the beginning of the buf\n                           ^\n0 / 17" #cs=
17 godown epos lpos 16 #eq
1 godown epos lpos 16 #eq

$10005 to epos
"with some text\n     ^\n1 / 17" #cs=
$3000b delto
"with process textabc\n     ^\n1 / 15" #cs=

origed
\test writing multi-line contents behaves correctly
tested

"foo\nbar" edstream puts
"barprocess textabc\n   ^\n2 / 16" #cs=
1 goup
"with foo\n   ^\n1 / 16" #cs=

origed
\test replchar
tested

'X' replchar
"witX foo\n   ^\n1 / 16" #cs=

origed
\test replchar at the end of a line
tested

eol
'Y' replchar
"witX fooY\n         ^\n1 / 16" #cs=

origed
\test Deleting the last line used to generate an error
tested

linecnt 16 #eq
15 go 1 dellines \ no crash
linecnt 15 #eq

origed
\test using delto up from bol to the end of file
\ resulting pos used to be out of bounds
tested

1 goup bol
epos 1 godown eol delto
linecnt 13 #eq
epos lpos 12 #eq

origed
\test delto ending at the beginning of an empty line doesn't delete that line
tested
insertline
epos lpos 12 #eq
linecnt 14 #eq
13 0 joinpos delto
epos lpos 12 #eq
linecnt 13 #eq

origed
\test delto on empty line doesn't copy previous line
tested

"hello there" edstream puts
insertline
epos lpos 12 #eq
linecnt 14 #eq
curline Line.cnt 0 #eq
13 0 joinpos delto
epos lpos 12 #eq
linecnt 13 #eq
"hello there" curline line[] #s[]=

origed
\test Now let's save this
tested

$400 newmemstreambuf const myfile
myfile edsaveto
0 myfile seek
here 16 myfile read#
"at the beginning" here 16 #s[]=

origed
testend
