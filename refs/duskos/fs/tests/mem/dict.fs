needs tests/harness mem/dict
testbegin

\test
: foo 42 ;
current
: foo 54 ;
forget foo
foo 42 #eq
current #eq

\test
: foo 54 ;
: bar 12 ;
delete foo
foo 42 #eq
bar 12 #eq

\test Check that ?xt>e doesn't try to read out of bounds memory
$ff876543 ?xt>e not #
' foo ?xt>e dup # entrylen 3 #eq

\test tagged entries
alignhere n"CAFE" , create foo $cafebabe ,
'e foo entrytag n"CAFE" #eq
n"CAFE" "foo" sysdict findtagged 'e foo #eq
n"DEAD" "foo" sysdict findtagged 0 #eq
n"CAFE" "doesnotexist" sysdict findtagged 0 #eq

\test extractdict
: foo 42 ;
: bar 123 ;
: baz 567 ;
extractdict bar mydict
"foo" sysdict @ entryname[] s[]= #true
"bar" mydict findentry #true
"baz" mydict findentry #true
"foo" mydict findentry not #true
testend
