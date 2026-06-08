needs tests/harness hal/muldiv
testbegin

\test sanity check
code mul3 ( n -- n ) 3 i) mulorshift, exit,
code mul2 ( n -- n ) 2 i) mulorshift, exit,
code mul2A ( n -- n )
  A) &) !, 2 i) A>) mulorshift, A) &) @, exit,
code mul1 ( n -- n ) 1 i) mulorshift, exit,
code mul0 ( n -- n ) 0 i) mulorshift, exit,
code div3 ( n -- n ) 3 i) divorshift, exit,
code div2 ( n -- n ) 2 i) divorshift, exit,
code div1 ( n -- n ) 1 i) divorshift, exit,
code mod3 ( n -- n ) 3 i) modorand, exit,
code mod2 ( n -- n ) 2 i) modorand, exit,
code mod1 ( n -- n ) 1 i) modorand, exit,
code mod2A ( n -- n )
  A) &) !, 2 i) A>) modorand, A) &) @, exit,
\ If you do a div by zero, it's your problem!

42 mul3 126 #eq
42 mul2 84 #eq
42 mul2A 84 #eq
42 mul1 42 #eq
42 mul0 0 #eq
42 div3 14 #eq
42 div2 21 #eq
42 div1 42 #eq
41 mod3 2 #eq
41 mod2 1 #eq
41 mod2A 1 #eq
41 mod1 0 #eq

\test modorand, with dir)
code domod ( a b -- a%b )
  PSP) dir) modorand, drop, exit,
5 2 domod 1 #eq

testend
