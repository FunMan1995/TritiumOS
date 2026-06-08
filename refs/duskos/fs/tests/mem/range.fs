needs tests/harness mem/range
testbegin

\test cmove-
create foo map< c, 1 2 3 4
foo foo 1+ 3 cmove-
create expected map< c, 1 1 2 3
foo expected 4 c[]= #

\test rslide+
create foo map< c, 1 2 3 4 5 6
2 foo 3 rslide+
create expected map< c, 1 2 1 2 3 6
foo expected 6 c[]= #

\test rslide-
create foo map< c, 1 2 3 4 5 6
2 foo 3 + 3 rslide-
create expected map< c, 1 4 5 6 5 6
foo expected 6 c[]= #

\test rtrim[] ltrim[]
"foobar" c@+ 3 rtrim[] "foo" rot> s[]= #
"foobar" c@+ 3 ltrim[] "bar" rot> s[]= #

\test cswap[]
"foobar" dup c@+ cswap[] "raboof" #s=

\test swap[]
create foo 1 , 2 , 3 , 4 , 5 ,
create expected 5 , 4 , 3 , 2 , 1 ,
foo 5 swap[]
foo expected 5 []= #

\test split[]
"foobar" c@+ 3 split[] "bar" rot> s[]= #true "foo" rot> s[]= #true

\test map[]
create foo 1 , 2 , 3 , 4 , 5 ,
create expected 2 , 3 , 4 , 5 , 6 ,
foo 5 ' 1+ map[]
foo expected 5 []= #

\test cmap[]
create foo ,"hello"
create expected ,"HELLO"
foo 5 ' upcase cmap[]
foo expected 5 c[]= #

\test glue[]
1 2 3 4 glue[] 6 #eq 1 #eq
1 6 3 4 glue[] 6 #eq 1 #eq
1 0 3 4 glue[] 4 #eq 3 #eq
1 2 3 0 glue[] 2 #eq 1 #eq
testend
