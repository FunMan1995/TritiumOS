needs tests/harness io/typeln
testbegin

4 newtypebuf const tb

\test type something
'f' tb type1 0 #eq
'o' tb type1 0 #eq
'o' tb type1 0 #eq
"foo\n" LF tb type1 1 #eq #s[]=
tb curlen 0 #eq

\test fill buffer
'f' tb type1 0 #eq
'u' tb type1 0 #eq
'l' tb type1 0 #eq
"ful\n" 'l' tb type1 1 #eq #s[]=

\test backspace
'b' tb type1 0 #eq
'a' tb type1 0 #eq
'r' tb type1 0 #eq
BS tb type1 0 #eq
'z' tb type1 0 #eq
"baz\n" LF tb type1 1 #eq #s[]=

\test ESC
'f' tb type1 0 #eq
'o' tb type1 0 #eq
'o' tb type1 0 #eq
ESC tb type1 1 #eq 0 #eq

testend
