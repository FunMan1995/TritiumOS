needs tests/harness lib/str
testbegin
\test sfind
stringlist list "hello" foo "bar"

"foo" list sfind 1 #eq 1 #eq
"hello" list sfind 1 #eq 0 #eq
"baz" list sfind 0 #eq

\test [str]?
"bar" "foobar" c@+ [str]? 3 #eq
"baz" "foobar" c@+ [str]? -1 #eq
"foo" NULLSTR c@+ [str]? -1 #eq
NULLSTR "foobar" c@+ [str]? 0 #eq

\test strcat
"foo" "bar" strcat "foobar" s= #

\test startswith? endswith?
"foo" "foobar" startswith? #true
"bar" "foobar" startswith? not #true
"foo" "foobar" endswith? not #true
"bar" "foobar" endswith? #true
"bar" "barbar" endswith? #true
"bar" "ba" endswith? not #true \ don't crash

\test startswith? endswith? and null substrings
NULLSTR "foobar" startswith? #true
NULLSTR "foobar" endswith? #true

\test zstr
create zstr ,"hello\0"
zstr here zmove here 6 + #eq
here zstr 6 c[]= #

\test rcidx
'/' "foo/bar/baz" c@+ rcidx #true 7 #eq
'!' "foo/bar/baz" c@+ rcidx not #true

testend
