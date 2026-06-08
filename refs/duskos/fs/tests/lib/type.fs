needs tests/harness lib/type
testbegin

variable data

\test integer types
ushort exec>str .type "ushort" #s=
ushort typesz 2 #eq
int typesz 4 #eq
uint intsigned? not #true
char intsigned? #true
leint int? #true

\test integer type HAL
42 data !
code foo dup, data m) uint type@, exit,
foo 42 #eq

code foo data m) uint type!, drop, exit,
54 foo
data @ 54 #eq

\test pointer type
uchar newpointer exec>str .type "*uchar" #s=
beint newpointer reftype beint #eq

\test array type
42 short newarray dup typesz 84 #eq ( type )
exec>str .type "[short,42]" #s=

\test signature type
short 1 uint char 2 $100 newsignature const mysig
mysig signature? #true
mysig sigcounts 2 #eq 1 #eq
mysig siginputs @ short #eq
mysig sigoutputs @+ char #eq @ uint #eq
mysig sigvarinput? #true
mysig sigvaroutput? not #true

\test empty signature type printing
0 0 0 newsignature exec>str .type "( -- )" #s=

\test parse types
: #pointer ( type chktype -- ) swap dup pointer? #true reftype #eq ;
type< short short #eq
type< *leint leint #pointer
type< [*int,42] dup array? #true dup arraycount 42 #eq reftype int #pointer

\test parse xt
type< ( uchar -- *void int ) const mysig
mysig signature? #true
mysig sigcounts 2 #eq 1 #eq
mysig siginputs @ uchar #eq
mysig sigoutputs @+ int #eq @ void #pointer
mysig sigvarinput? not #true
mysig sigvaroutput? not #true
mysig exec>str .type "( uchar -- *void int )" #s=

\test parse xt with argument names
type< ( foo:int -- bar: int ) const mysig
mysig signature? #true
mysig sigcounts 1 #eq 1 #eq
mysig siginputs @ int #eq
mysig sigoutputs @ int #eq
mysig exec>str .type "( int -- int )" #s=

\test parse xt with ...
type< ( ... uchar -- ... *void int ) const mysig
mysig signature? #true
mysig sigcounts 2 #eq 1 #eq
mysig sigvarinput? #true
mysig sigvaroutput? #true

testend
