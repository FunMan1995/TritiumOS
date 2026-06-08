needs tests/harness lib/str mem/dict lib/macro
testbegin
\test interpret loop
4 2 run1 + 6 #eq

\test arithmetic
3 5 * 15 #eq
11 3 /mod 3 #eq ( q ) 2 #eq ( r )

42 max0 42 #eq
-42 max0 0 #eq

\test templates
: foo bi 2* | 2/ ;
42 foo 21 #eq 84 #eq
: foo tri 1+ | 1- | 2* ;
54 foo 108 #eq 53 #eq 55 #eq

\test I/O
: wordmaker word"hello" code 42 litn exit, ;
wordmaker hello 42 #eq

NULLSTR exec>str stype NULLSTR #s=

\test Entry
\ don't crash when creating a NULLSTR entry
\ also test that we don't allocate outrageous amount of memory.
here
sysdict NULLSTR entry
here swap- $10 < #true

\test check that entries are properly aligned
alignhere
,"XX" \ not supposed to end up right before the entry
code Z
sysdict e>wlen 3 -
dup 4 align#
be@ $00005a01 #eq

\test [if]..then
1 [if] 42 42 #eq [then]
0 [if] abort [then]

\test does> words
: incer does> 1+ ;
41 incer foo
101 incer bar

foo 42 #eq
bar 102 #eq

\test case
: foo ( n ) case
    1 = of 111 endof
    42 < of 222 endof
    drop 333
  endcase ;
: testrcnt 42 case 42 = of endof endcase [ [rcnt] @ 0 #eq ] ;

1 foo 111 #eq
2 foo 222 #eq
3 foo 222 #eq
42 foo 333 #eq

\test while..repeat
: foo begin dup 9 20 within? not while dup 3 5 within? not while 1+ repeat
  100 + else 200 + then ;

1 foo 103 #eq
10 foo 210 #eq
6 foo 209 #eq
20 foo 220 #eq

\test do..loop
: foo 0 5 do i , 1 -loop ;
create expected 5 , 4 , 3 , 2 , 1 ,
here foo expected 5 []= #

: foo 42 43 do noop noop 0 # loop ; foo \ 0 iterations work

: foo 43 38 do i , loop ;
create expected 38 , 39 , 40 , 41 , 42 ,
here foo expected 5 []= #

\test Range words
create data 1234 , 42 , 12 ,
54 data 3 idx 0 #eq
12 data 3 idx 1 #eq 2 #eq
12 data 2 idx 0 #eq

create data ,"hello"
'X' data 5 cidx 0 #eq
'l' data 5 cidx 1 #eq 2 #eq
'o' data 4 cidx 0 #eq

\test Number formatting
42 exec>str .x1 "2a" #s=
"2a" 42 formathex1 s[]= #true
$1234 exec>str .x "00001234" #s=
"00001234" $1234 formathex s[]= #true
42 exec>str . "42" #s=
"42" 42 formatdec s[]= #true
"4294967295" -1 formatdecu s[]= #true
-1984 exec>str . "-1984" #s=
0 exec>str . "0" #s=
0 exec>str .sz "0B" #s=
1024 1024 * 1- exec>str .sz "1023KB" #s=
42 1024 * 1024 * exec>str .sz "42MB" #s=
-1 exec>str .sz "3GB" #s=

\test having a 0 in the input stream does not equate end of stream
"41 \0 1+ " c@+ injectrange
42 #eq

\test n""
n"DUSK" $4455534b #eq
n"XDUSK" $4455534b #eq
n"USK" $0055534b #eq
: foo n"DUSK" ;
foo $4455534b #eq

\test tagged#
create foo n"CAFE" , $cafebabe ,
create bar n"DEAD" , $deadbeef ,
foo n"CAFE" tagged# @ $cafebabe #eq
bar n"CAFE" expectabort tagged#

\test parsehex
\ it used to fail on m68k in interpret mode
"cafe" c@+ parsehex #true $cafe #eq
"nan" c@+ parsehex not #true

\test endianness
create mynum map< c, $12 $34 $56 $78
mynum le@ $78563412 #eq
mynum wle@ $3412 #eq
mynum be@ $12345678 #eq
mynum wbe@ $1234 #eq

\test fill is zero guarded
here 0 42 cfill \ doesn't crash the machine

\test ~ is properly find-selected
:~ 42 ;
: foo ~ ;
foo 42 #eq
: bar ['] ~ execute ;
bar 42 #eq
:~ 54 ;
foo 42 #eq
bar 42 #eq

\test string escaping
create expected 4 c, LF c, CR c, 0 c, '\' c,
"\n\r\
\0\\" expected #s=

\test map<
map< 2* 1 2 3
6 #eq 4 #eq 2 #eq

\test FINDCOMPILER is reset at each word
\ it's very meta, but if FINDCOMPILER is not reset after each word, a [compile]
\ followed by a compile leads to the wrong form being compiled.
:> 1 litn ; :> 0 ; compiling iscompiling?
: dummy ; immediate
: foo, [compile] dummy compile iscompiling? ; immediate
: foo foo, ;
foo not #true

\test unit findsel and immediates
\ There used to be a confusion when an immediate word was selected through
\ unit words: they wouldn't be immediately executed as they should.
unit dummy/unit
: foo 42 ;
: bar 54 ; immediate
endunit

dummy/unit foo 42 #eq
dummy/unit bar 54 #eq
:~ dummy/unit foo ;
~ 42 #eq
:~ dummy/unit bar ; 54 #eq

testend
