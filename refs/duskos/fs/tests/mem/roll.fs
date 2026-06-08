needs tests/harness mem/roll io/stream
testbegin

8 newrollingbuffer const mybuf

\test write then read without bounds
"hello" mybuf puts
5 mybuf gets "hello" #s=

\test read from empty buffer
42 mybuf readbuf 0 #eq

\test full the buffer
"hellogoodbye" c@+ mybuf write 7 #eq

\test a full buffer can't get anything else
"hey" c@+ mybuf write 0 #eq

\test read that buffer
7 mybuf gets "hellogo" #s=

\test try that fastputc, thing
code myfastputc ( c stream -- f )
  A) &) !, drop,
  fastputc, 1 i) @, else 0 i) @, then
  exit,

'X' mybuf myfastputc #true
mybuf getc 'X' #eq
mybuf getc EOF #eq \ update ridx from nextridx

\test fastputc, on a full buffer
"hellogoodbye" c@+ mybuf write 7 #eq
'X' mybuf myfastputc not #true

\test writing window
4 const WINSZ
mybuf reset
"ab" c@+ 2 mybuf writeahead 2 #eq
"cd" c@+ 0 mybuf writeahead 2 #eq
mybuf wrmax 7 #eq \ writing ahead doesn't affect wrmax
mybuf getc EOF #eq \ can't read yet, hasn't advanced!
4 WINSZ mybuf advancewindow 3 #eq
mybuf wrmax 4 #eq \ *now* it affects it
"cda" 3 mybuf gets #s=
1 WINSZ mybuf advancewindow 1 #eq
mybuf getc 'b' #eq

\test advancewindow doesn't go past wlim - WINSZ
mybuf reset
2 WINSZ mybuf advancewindow 2 #eq
2 WINSZ mybuf advancewindow 1 #eq \ remember: actual buf capacity is 7b
2 WINSZ mybuf advancewindow 0 #eq
mybuf getc drop
2 WINSZ mybuf advancewindow 1 #eq


testend
