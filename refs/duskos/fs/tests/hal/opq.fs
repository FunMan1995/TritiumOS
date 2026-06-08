needs tests/harness hal/opq
testbegin

\test (i?
42 i) (i? #true 42 #eq
42 m) (i? not #true
W) (i? not #true

\test (W?
42 i) (W? not #true

\test (bank and bank)
42 i) (bank 42 #eq
42 i) 54 bank) (i? #true 54 #eq

\test (src
RSP) 4 +) (src REGRSP #eq

\test signedcond
\test swappedcond
>) swappedcond <) #eq
<>) swappedcond <>) #eq
<=) swappedcond >=) #eq

\test (signed?
W) (signed? not #true
W) signed) (signed? #true

testend
