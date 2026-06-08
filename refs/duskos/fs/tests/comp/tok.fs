needs tests/harness comp/tok
testbegin
tok$
newtok 'f' tokacc 'o' tokacc 'o' tokacc
curtok "foo" #s=
'b' tokacc
curtok "foob" #s=
curtokcopy newtok 'X' tokacc ( oldtok )
curtok "X" #s=
( oldtok ) "foob" #s=
testend
