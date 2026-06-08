needs tests/harness mem/ll
testbegin
here# 0 , 42 , value ll
ll lladd 54 ,
here# 0 , 33 , addrof ll llinsert
ll 4+ @ 33 #eq
ll @ 4+ @ 42 #eq
ll @ @ 4+ @ 54 #eq
ll llcnt 3 #eq
2 ll llitern 4+ @ 54 #eq 4+ @ 42 #eq
4 ll llitern 0 #eq 4+ @ 54 #eq
$1234 ll llfind 0 #eq 3 #eq
ll @ ll llfind 4+ @ 33 #eq 1 #eq
testend
