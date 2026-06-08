needs tests/harness comp/sym comp/c comp/c/type
testbegin
cctok$ cc$

: p# ( -- type name ) parsetype# decl read; ;
: chk" ( name -- )
  [rcompile] " p# ( expvarname exptypestr type name )
  rot> word".type" exec>str #s= #s= ;

\test base types
NULLSTR chk"int" int;
NULLSTR chk"long" long;
NULLSTR chk"ushort" ushort;
NULLSTR chk"void" void;
NULLSTR chk"**char" char**;
NULLSTR chk"*ushort" ushort*;
NULLSTR chk"[uint,42]" uint[42];
"foo" chk"[*short,54]" short *foo[54];

\test structure size are aligned, but their fields are not automatically padded
NULLSTR chk"{foo +0 uint, bleh +4 uint, bar +8 *short, baz +12 [char,2]}"
struct Struct1 { uint foo, bleh; short *bar; char baz[2]; };
Struct1 typesz 16 #eq

\test once defined, parsetype will find the struct
p# Struct1; drop Struct1 #eq

\test A structure can embed another structure.
p# struct Struct2 { Struct1 s; Struct1 *sp; }; drop ( type )
dup typesz 20 #eq ( type )
"sp" swap findfield #true 16 #eq reftype Struct1 #eq

\test Anonymous structs work too
p# struct { int foo; struct { short bar; short baz; } mystruct; }; drop ( type )
dup typesz 8 #eq ( type )
"mystruct" swap findfield #true 4 #eq typesz 4 #eq

\test Unions work too
p# union { struct { int a; int b; } s; int foo; }; drop ( type )
dup typesz 8 #eq ( type )
"s" over findfield #true 0 #eq drop
"foo" swap findfield #true 0 #eq drop

\test Forward struct references
p# struct Struct3 { Struct4 *ptr; int (*func)(Struct4*); }; drop
typesz 8 #eq ( type )
p# struct Struct4 { int foo; }; 2drop
"ptr" Struct3 findfield #true 0 #eq reftype Struct4 #eq
Struct4 typesz 4 #eq

\test Function pointers
"foo" chk"( *short char -- uint )"
uint (*foo)(char,short *argname);

\test A function pointer can return a pointer
"foo" chk"( *short char -- *short )"
short* (*foo)(char,short *argname);

\test Or what about a vanilla function prototype?
"foo" chk"( int int -- *char )"
char* foo(int a, int b);

\test arguments were added as symbols in comp/sym
argumentsz 8 #eq
"a" findsymbol offset 0 #eq
"b" findsymbol offset 4 #eq

\test Or a function with no arg?
"foo" chk"( -- )" void foo();

\test Function with void* return type is considered to have a return value
"foo" chk"( -- *void )" void* foo();

\test Funcsigs can have "..." to indicate variable argument count.
p# int foo(int arg, ...); drop sigvarinput? #true
argumentsz 4 #eq

\test "..." also work as the sole argument
p# int foo(...); drop sigvarinput? #true
argumentsz 0 #eq

\test "unsigned" and "signed"
NULLSTR chk"uint" unsigned long;
NULLSTR chk"uint" unsigned int;
NULLSTR chk"ushort" unsigned short;
NULLSTR chk"uchar" unsigned char;
NULLSTR chk"int" signed long;
NULLSTR chk"int" signed int;
NULLSTR chk"short" signed short;
NULLSTR chk"char" signed char;
testend
