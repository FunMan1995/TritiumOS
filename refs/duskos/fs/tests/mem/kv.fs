needs tests/harness mem/kv
testbegin

\test simple case
create tbl 1 2 3 4 2 kvtbl,
tbl 1 kv@ #true 2 #eq
tbl 3 kv@ #true 4 #eq
tbl 3 kv@# 4 #eq
tbl 2 kv@ not #true

\test kv!
tbl 42 1 kv! not #true
tbl 3 5 kv! #true
tbl 3 kv@ #true 5 #eq

\test kvreplace
tbl 42 54 kvreplace not #true
tbl 3 8 kvreplace #true
tbl 3 kv@ not #true
tbl 8 kv@ #true 5 #eq

testend
