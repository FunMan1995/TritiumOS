needs tests/harness lib/struct lib/ival
testbegin
create data $cafebabe , $deadbeef , $1234 w, 42 c, 54 c,

struct Struct {
  uint first second ;
  ushort third ;
  uchar fourth ;
  [void,0] tail ;
}

\test field access
data first $cafebabe #eq
data second $deadbeef #eq
data third $1234 #eq
: foo data second ; foo $deadbeef #eq
data tail data Struct typesz + #eq

\test offsetof
offsetof first 0 #eq
offsetof second 4 #eq
:~ offsetof second ; ~ 4 #eq ~ 4 #eq
offsetof third 8 #eq
offsetof fourth 10 #eq
offsetof tail 11 #eq

\test offsetof on IVAL
$1234 ivalmap { +$2345 uint myival ; }
offsetof myival $2345 #eq

\test prefixed field names
data Struct.first $cafebabe #eq

\test Modifications below
$12345678 data to second
data second $12345678 #eq
: foo $234567890 data to second ; foo
data second $234567890 #eq
$2345 data to third
data third $2345 #eq

\test endian-aware fields
create mynum map< c, $12 $34 $56 $78
struct Foo {
  beshort mybe ;
}
mynum mybe $1234 #eq
:~ mynum mybe ; ~ $1234 #eq
$23456789 mynum le!
mynum be@ $89674523 #eq
0 mynum ! $2345 mynum wle!
mynum be@ $45230000 #eq
$23456789 mynum be!
mynum le@ $89674523 #eq
0 mynum ! $2345 mynum to mybe
mynum le@ $4523 #eq
:~ $7890 mynum to mybe ; ~
mynum be@ $78900000 #eq

\test array offsets
struct Arrays {
  [uint,10] foo ;
  [uchar,42] bar ;
  [void,0] baz ;
}
0 bar 40 #eq
0 baz 82 #eq

\test containsstruct?
struct Foo { }
extends Foo struct Bar { }
Foo Struct containsstruct? not #true
Foo Bar containsstruct? not #true
Bar Foo containsstruct? #true

\test a recursive structure can be printed
\ Note that this is a fake broken struct that is created only for printing
\ purposes. In real Forth code, it's impossible to reference an unfinalized
\ struct into an array. Only compilers do fancy things like that.
struct RecursiveStruct {
  [RecursiveStruct,1] foo ;
}

RecursiveStruct exec>str .type "{foo +0 [{...},1]}" #s=

\test struct placeholder
struct Placeholder { }
Placeholder const fooptr
struct Placeholder { }
Placeholder fooptr #eq

testend
