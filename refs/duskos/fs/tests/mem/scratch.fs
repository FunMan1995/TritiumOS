needs tests/harness mem/scratch
testbegin
$10 newscratchpad
  dup @ const scbuf
  dup bindalloc[ sc[ dup bindrun1 sc1 bindrun1@ sc1@

\test
sc1 newhere $10 #eq scbuf #eq

\test
variable refhere
here refhere !
sc[ 3 allot@ ]alloc scbuf #eq

\test
here refhere @ #eq

\test
: _ 8 sc1@ allot ;
_ scbuf 4 + #eq

\test The next call will make the pad rewind
: _ 8 reserve $1234 , ;
sc1 _
scbuf @ $1234 #eq
testend
