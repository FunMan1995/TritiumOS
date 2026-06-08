needs tests/harness comp/sym comp/c comp/c/type comp/c/expr comp/c/ast
testbegin
cctok$ cc$

: p# ( -- type name ) parsetype# decl read; ;
\test type= considers a pointer to a type equal to an array of the same type
p# uint *foo; drop
p# uint foo[3]; drop
type= #true

\test ... but not to another type!
p# uint *foo; drop
p# ushort foo[3]; drop
type= not #true

\test two void* are compatible with each other
p# void *foo; drop
p# void *bar; drop
type= #true

\test a flex integer also matches pointers
String flexuint type= #true
AnyPtr flexuint type= #true
flexuint AnyPtr type= #true

\test boolops bothconst
parseConstExpr 42 < 54; read; 1 #eq
parseConstExpr 42 > 54; read; 0 #eq

\test myadder
:c int myadder(int arg1, int arg2) {
  int x;
  x = arg1 + arg2;
  return x;
}
42 12 myadder 54 #eq

\test ++
:c void foo() { char var; }
ast< var++; read; expr, W) &) #eq char #eq

\test RSP alignment
:c void foo(int arg) { char a; int b; }
ast< b; read; expr, ( type halop )
dup (src REGRSP #eq
(bank 4 #eq ( type )
int #eq
freeW

\test unary op and that we don't require whitespace around symbols
:c int negate() {int a=$2a; return -a;}
negate -42 #eq

\test ~
:c int bwnot() {
    int a='*';
    return ~a;
}
bwnot $ffffffd5 #eq

\test -
:c int subber(int a, int b) {
    return a - b;
}
2 3 subber 1 #eq

\test &
:c uint binopand() {
    int a=$ff;
    return (uint)a & 0x2a; // we also support "0x" prefix
}
binopand 42 #eq

\test |
:c int binopor() {
    int a=2;
    return 40 | a;
}
binopor 42 #eq

\test ^
:c int binopxor() {
    int a=43;
    return a ^ 1;
}
binopxor 42 #eq

\test <<
:c int binopshl() {
    int a=42;
    return a << 3;
}
binopshl 336 #eq

\test >>
:c int binopshr() {
    int a=42;
    return a >> 2;
}
binopshr 10 #eq

\test /
:c int binopdiv() {
    int a=42;
    return a / 3;
}
binopdiv 14 #eq

\test signed /
:c int binopdivs() {
    int a=-42;
    return a / 3;
}
binopdivs -14 #eq

\test %
:c int binopmod() {
    int a=43;
    return a % 3;
}
binopmod 1 #eq

\test signed %
:c int binopmods() {
    int a=-43;
    return a % 3;
}
binopmods -1 #eq

\test when arguments are signed, ">>" does an arithmetic shift right
:c int asr(int a, int b) {
  return a >> b;
}
2 33 asr 8 #eq
2 -33 asr -9 #eq

\test that signed >> works in its "reversed" form
:c int asr(int a, int b) {
  return a >> (b & 31);
}
2 33 asr 8 #eq
2 -33 asr -9 #eq

\test ?:
:c int binopcondeval(int x) {
	return x ? 42 : 12 ;
}
1 binopcondeval 42 #eq
0 binopcondeval 12 #eq

\test assignops
:c int assignops() {
  int a=42, b=2, c=3;
  a += b; // 44
  a -= c; // 41
  a *= b; // 82
  a |= c; // 83
  return a;
}
assignops 83 #eq

\test assignops chaining
:c int assignchain() {
  int a=42, b, c;
  b = c = a+1;
  return b;
}
assignchain 43 #eq

\test signed >>=
:c int signedrshift(int a) { a >>= 1; return a; }
-12 signedrshift -6 #eq

\test boolops
:c int boolops() {
  int a=66, b=2;
  return a < 54 && 2 == b;
}
boolops 0 #eq

\test signed boolops
:c int isneg(int a) { return a < 0; }
0 isneg not #true
-1 isneg #true

\test signed boolops some more
:c int lts(int a, int b) {
    return a < b;
}
0 -1 lts #true
:c int ltu(uint a, uint b) {
    return a < b;
}
0 -1 ltu not #true

\test Make sure that the "inverted comparison mechanism" works
:c int foo(int a, int b) { return a < b+1; }
41 41 foo #true
41 42 foo not #true

\test that ++ and -- modify the lvalue directly
:c int incdec(int x) {
    ++x;
    --x;
    return ++x;
}
42 incdec 43 #eq

\test that the final "--" doesn't affect the result
:c int incdecp(int x) {
    x++;
    x--;
    return x--;
}
54 incdecp 54 #eq

\test that parens override precedence
:c int exprparens() {
    return (1 + 2) * 3;
}
exprparens 9 #eq

\test funcall
:c int funcall() {
    return assignops();
}
funcall 83 #eq scntneutral#

\test We used to get an opslot conflict when having ternary ?: inside a funcall.
:c int ternarychoiceincall(int x) {
  return myadder(x < 42 ? 10 : 5, x == 55 ? 1 : x);
}
12 ternarychoiceincall 22 #eq
55 ternarychoiceincall 6 #eq
56 ternarychoiceincall 61 #eq

\test Funcall within a funcall with args used to confuse PS.
:c int triadd(int a, int b, int c) {
  return a+b+c;
}
:c int inception(int a, int b) {
  return triadd(myadder(a+1, b+2), myadder(a+3, b+4), a+b);
}
2 3 inception 25 #eq

\test void functions with arguments used to corrupt PS
\ ... but in a way that resolved itself at postlude, making it difficult to
\ detect in a test. This test reproduces such corruption in a detectable way.
:c void oneargvoidret(int n) { }
:c void changesarg(int n) { n = 22; }
:c int retsame(int n) {
  oneargvoidret(12);
  changesarg(n);
  return n;
}
32 retsame 32 #eq

\test that argument types are tested in the correct order
:c uint foo(ushort a, uint b) {
	return (uint)a + b;
}
:c uint bar() {
	ushort x = 12;
	uint y = 22;
	return foo(x, y);
}
bar 34 #eq

\test calling a function with varargs
\ we're just testing that it compiles, not running it
:c void foo(short a, uint b, ...) { }
:c void bar() {
	short x;
	uint y;
	foo(x, y, x, x);
}

\test funcsig
:c typedef uint (*AdderSig)(uint, uint);
:c uint funcsig(uint a, uint b) {
    AdderSig fn = myadder;
    return fn(a, b);
}
142 42 funcsig 184 #eq

\test typecast
:c int typecast() {
	char x = $ff;
	int y = 1;
	return x == (char)(y-2);
}
typecast 1 #eq

\test Typecasting a lvalue doesn't affect later references to it
:c int foo(int a) {
  char x = (char)a;
  return a;
}
$12345678 foo $12345678 #eq

\test Typecasting a "weak" type makes it "strong"
:c int* foo() { return (int*)$20 + 2; }
foo $28 #eq

\test Typecasting to a larger type does sign ext if *origin* type is signed
:c int foo(char n) { return (int)n; }
$7f foo $7f #eq
$80 foo $ffffff80 #eq
:c int foo(uchar n) { return (int)n; }
$7f foo $7f #eq
$80 foo $80 #eq

\test deref
:c int ptrget() {
    int a = 42;
    int *b = &a;
    return *b;
}
ptrget 42 #eq

\test deref assign
:c int ptrset() {
    int a = 42;
    int *b = &a;
    *b = 54;
    return a;
}
ptrset 54 #eq

\test pointer dereferencing and array subscripting
create mydata 42 , 54 , $cafebabe ,
:c int ptrari(int* ptr) {
	return *ptr + ptr[1];
}
mydata ptrari 42 54 + #eq

\test Pointer arithmetics between arrays and pointers is possible
:c int foo() {
  int array[3];
  int *ptr = &array[1];
  return ptr - array;
}
foo 1 #eq

\test that pointer arithmetics properly multiply operands by 2 or 4.
:c int* ptrari(int *x) {
    x++; ++x; x--;
    return x + 1;
}
42 ptrari 50 #eq

\test subtracting two pointers yield a number divided by the type size.
:c int ptrari2(int *lo, int *hi) {
    return hi-lo;
}
50 42 ptrari2 2 #eq

\test ptrari in +=
:c int* foo(int* a, uint n) {
  a += n;
  return a;
}
2 42 foo 50 #eq

\test array
:c int array() {
    int a[3] = {42, 12, 2};
    return *a + a[1] - *(a+2);
}
array 52 #eq
\ this function at some point behaved well, but broke RS!
rcnt 200 < #true

\test 8b get/set
"foobar" const mystr
:c char get8b(char *s, int i) {
    return s[i]; // 0th is length
}
2 "foobar" get8b 'o' #eq
4 "foobar" get8b 'b' #eq

:c void set8b(char *s, int i, char c) {
    s[i] = c;
}
'X' 2 mystr set8b mystr "fXobar" #s=
'X' 6 mystr set8b mystr "fXobaX" #s=

\test struct
:c typedef char MyType;
:c typedef MyType** MyTypePtr;

:c struct MyStruct {
    int foo;
    short bar;
    MyType baz[2];
    int array[2], another;
};

:c MyType structget(MyStruct *s) {
    int x = 1;
    return s->baz[x-1] + 1;
}

:c void structset(MyStruct *s, char val) {
    s->baz[1] = val;
}

create mydata 42 , $5678 w, $34 c, $12 c, $23456789 ,
mydata structget $35 #eq
$42 mydata structset
mydata 7 + c@ $42 #eq
\ other fields were untouched
mydata 4+ w@ $5678 #eq
mydata 6 + c@ $34 #eq
mydata 8+ @ $23456789 #eq

\test forward struct refs
:c struct Struct2 {
	ForwardStruct *s;
};
:c struct ForwardStruct {
	int foo;
};

:c int getforwardedstructfield(Struct2 *s) {
	return s->s->foo;
}
create struct1 $1234 ,
create struct2 struct1 ,
struct2 getforwardedstructfield $1234 #eq

\test global struct
:c MyStruct globdata;

:c short globstructget() {
    return globdata.bar;
}
:c void globstructset(short val) {
    globdata.bar = val;
}
42 globstructset globstructget 42 #eq
globdata 4 + w@ 42 #eq

\test Chaining assignments involving struct members used to generate bad code
:c struct MyStruct2 { int foo, bar; };
:c void foo(MyStruct2 *s, int n) { s->foo = s->bar = n + 42; }
create data 1 , 2 ,
3 data foo
data @ 45 #eq
data 4+ @ 45 #eq

\test sizeof
:c uint foo() { return sizeof(MyStruct); }
foo 20 #eq

\test "sizeof()" can be used on a symbol, not just a type.
:c int foo() {
  short array[3];
  return sizeof(array);
}
foo 6 #eq

\test global
:c int global1 = 1234, global2[2+1] = {4, 5, 6};

:c int global() {
    return global1;
}
global 1234 #eq

\test globalinc
:c int globalinc() {
    global1++;
    return ++global1;
}
globalinc 1236 #eq
globalinc 1238 #eq

\test short global array
:c short shortarray[6] = {4, 5, 6, 7, 8, 9};
:c short shortglobal() {
    return shortarray[1];
}
shortglobal 5 #eq

\test array in expressions
:c int constlist(int idx) {
  return {42, 54, $1234}[idx];
}
1 constlist 54 #eq

\test CNST entries are picked up
42 const MYCONST
:c int constsym() {
  return MYCONST;
}
constsym 42 #eq

\test VALU entries are picked up
42 value myval
:c uint valuesym(uint n) {
  uint x = myval;
  myval = n;
  return x;
}
54 valuesym 42 #eq
myval 54 #eq

\ Below this comment are simple construct that were buggy before

\test There used to be a mixup between private variable
\ ... in sexpr's Ops namespace and identifiers in C code. Now, identifiers
\ starting with "_" will never be looked for in the Ops namespace.
:c int foo(int _buf) { return _buf; }
42 foo 42 #eq

\test binop1
:c int binop1(int a, int b) {
    int c;
    c = a ^ b;
    return c;
}
2 3 binop1 1 #eq

\test binop2
:c int binop2(int n) {
    int x = 42;
    x = x + n - '0';
    return x;
}
'2' binop2 44 #eq

\test binop3
:c int binop3() {
    return global2[2] << 8 | global2[1];
}
binop3 $605 #eq

\test properly "boolify" arguments of logical operators
:c int binop4() {
    int x = 2;
    return x && 3;
}
binop4 1 #eq

\test the i386 VM performed this add in 8b mode, not carrying the $100.
:c uint binop5() {
    uint x = $ff;
    uchar y = $ff;
    return x + (uint)y;
}
binop5 $1fe #eq

\test properly "boolify" arguments of logical operators, again...
:c int binop6() {
    int x = 2;
    int y = 0;
    return x && y;
}
binop6 0 #eq

\test multiple function calls in an expression can do funky things
:c int binop7() {
    return bwnot() + negate();
}
binop7 $ffffffab #eq

\test binop8
:c int binop8() {
    char array[2] = {$12, $34};
    return ((int)array[0]<<8)|(int)array[1];
}
binop8 $1234 #eq

\test i386 VM used to crash on this
:c int binop9(int a, int b) {
	return a == 42 || b == 12;
}
12 123 binop9 1 #eq

\test i386 VM had a register allocation problem with this very form of op
:c char binop10(char a, char b) {
	MyStruct *s = &globdata;
	s->baz[0] = a >> b;
	return s->baz[0];
}
2 $72 binop10 $1c #eq

\test forth VM used to leak to PS when having a funcall in a ?: operator
:c int binop11(int a, int b) {
	return a < 42 ? myadder(a, b) : -1 ;
}
5 12 binop11 17 #eq scntneutral#
5 54 binop11 -1 #eq scntneutral#

\test mixup with intsigned? and decl
:c int binop12(ushort a, ushort b) {
	return a < b;
}
42 1 binop12 1 #eq
42 -1 binop12 0 #eq

\test Believe it or not, this made CC crash.
:c int binop13() {
	return 1 << 8;
}
binop13 $100 #eq

\test subscript and ==
:c int binop14(char *a, char *b, int i) {
	return a[i] == b[i];
}
1 "abc" "aac" binop14 #true
2 "abc" "aac" binop14 not #true

\test %= on PSP
:c int binop15(int a, int b) {
	a %= b;
	return a;
}
2 5 binop15 1 #eq

\test >= on PSP
:c int binop16(int x) {
    return x >= 0;
}
0 binop16 1 #eq
-1 binop16 0 #eq

\test >= on W
:c int binop17(int x) {
    return x - 10 >= 0;
}
10 binop17 1 #eq
9 binop17 0 #eq

\test an expression that uses PS for intermediate results
:c int largeexpr(int a, int b) { ;
  return (a*3)+(b*5);
}
20 10 largeexpr 130 #eq

\test structop1
:c short structop1() {
    globdata.bar += 2;
    return globdata.bar;
}
structop1 44 #eq

\test postop on a struct field failed under the Forth VM
:c short structop2() {
    return globdata.bar++;
}
structop2 44 #eq
structop2 45 #eq

\test indexing a struct array with a struct field resulted in TOS mixup
:c char structop3() {
    globdata.baz[1] = 42;
    globdata.bar = 1;
    return globdata.baz[globdata.bar];
}
structop3 42 #eq

\test structop4
:c int* structop4() {
    return &globdata.array[1];
}
structop4 globdata 12 + #eq

\test the combination of struct pointer, struct field subscripting...
\ ... assignment and postop all at once caused the i386 VM to misallocate
\ registers.
:c char structop5() {
    MyStruct *s = &globdata;
    s->foo = 1;
    s->baz[s->foo++] = 42;
    return globdata.baz[1];
}
structop5 42 #eq

\test PS would get mixed up in assignops that weren't "=", when being...
\ assigned a complex expression.
:c int structop6() {
    int n = 12;
    globdata.baz[1] = 42;
    globdata.bar = 1;
    n += (int)globdata.baz[globdata.bar];
    return n;
}
structop6 54 #eq

\test structop7
:c MyStruct *globdataptr;
:c short structop7() {
    globdata.bar = 42;
    globdataptr = &globdata;
    return globdataptr->bar;
}
structop7 42 #eq

\test the address of the call would get lost on PS in certain situations...
\ such as a function living in a struct and accessed through a pointer.
:c struct StructWithFunc { int (*func)(int, int); };
:c int structop8(int a, int b) {
    StructWithFunc s;
    StructWithFunc *ptr = &s;
    s.func = myadder;
    return ptr->func(a, b);
}
42 12 structop8 54 #eq

\test structop9
:c struct StructWithRef { MyStruct *ref; };
:c int structop9() {
	StructWithRef s;
	s.ref = &globdata;
	globdata.foo = 123;
	return s.ref->foo;
}
structop9 123 #eq

\test structop10
:c int structop10() {
	globdata.foo = 42;
	globdata.bar = 12;
	return globdata.foo - (int)globdata.bar;
}
structop10 30 #eq

\test The forth VM used to assign to the SF in the wrong width
:c short opwidth1() {
    short x = 42;
    short y = $12345678;
    return x;
}
opwidth1 42 #eq

\test opwidth2
:c short opwidth2() {
    short x = 42;
    short y = 12;
    y += $12345678;
    return x;
}
opwidth2 42 #eq

\test The i386 VM didn't carry the $100
:c int opwidth3() {
    int x = 42;
    uchar y = $ff;
    x += (int)y;
    return x;
}
opwidth3 $129 #eq

\test The Forth VM lost track of opwidth through expressions
\ Forth VM and i386 VM mis-initialized the char array.
:c char opwidth4() {
    char x[2] = {1, 2};
    int y = 0;
    x[y] = 12;
    return x[0] + x[1];
}
opwidth4 14 #eq

\test The Forth and i386 VMs didn't properly apply size in inc/dec ops
:c uchar opwidth5() {
    uchar x = 42;
    uchar y = $ff;
    y++; y--;
    ++y;
    return x;
}
opwidth5 42 #eq

\test The i386 VM didn't properly promote "a" to int
:c uint opwidth7(uchar a, uint b) {
	return (uint)a << b;
}
1 $ff opwidth7 $1fe #eq

\test Under i386, integer promotion of a non-reg operand would result...
\ in a buggy operation because we would read too much information from memory.
:c int opwidth8() {
	int x = 54;
	globdata.baz[0] = 42;
	globdata.baz[1] = 1;
	return x < (int)globdata.baz[0];
}
opwidth8 0 #eq

\test when subtracting 2 pointers, the result is considered a scalar for the...
\ remainder of the expression.
:c int* ptrari3(int *lo, int *hi) {
    return lo+((hi-lo)/2);
}
50 42 ptrari3 46 #eq

\test ptrari4
:c int ptrari4(int *a) {
    return *(a++) == 42;
}
create myval 41 ,
myval ptrari4 not #true
1 myval +!
myval ptrari4 #true

\test Previously, pointer arithmetics adjustments only worked with power of two
:c struct TenBytes { int foo; int bar; short baz; };
:c TenBytes globstructarray[2];
:c TenBytes* ptrari5(int idx) {
    globstructarray[idx].foo = 42;
    return globstructarray;
}
1 ptrari5 12 + @ 42 #eq

\test Assignment of a dereferenced pointer into another dereferenced pointer
:c int ptrari6(int a, int b) {
	int *pa = &a, *pb = &b;
	*pa += *pb;
	return *pa;
}
456 123 ptrari6 123 456 + #eq

\test The "div by cell size" logic used to trigger on the "-1" part.
:c int* ptrari7(int *a, uint offset) {
	return a+offset-1;
}
2 $1234 ptrari7 $1238 #eq

\test Spurious & on "reference" CDecls is accepted
:c TenBytes* ptrari8() {
    return &globstructarray[1];
}
ptrari8 @ 42 #eq \ struct that was changed in ptrari5()

\test Typecasting to struct pointer types would previously erronously ...
\ compute the struct size, that is, use the size of the base struct rather than
\ the size of a pointer. The conditions of reproduction for this bug were very
\ specific.
:c struct MyStruct2 {
    int whatever;
    TenBytes *ptr;
    int somethingelse;
};
:c MyStruct* ptrari9(MyStruct2* s2) {
    MyStruct *res = (MyStruct *)s2->ptr; // don't apply a mask to it!
	return res;
}
create struct2 42 , $12345678 ,
struct2 ptrari9 $12345678 #eq

\test double subscript with struct pointer
\ this comes from ar/puff.c, with the huffman struct
:c struct Container { uint *data; };
:c uint outer[2] = {42, 54}, inner[2] = {1, 0};
:c uint hey(uint idx, Container *s) {
  return s->data[inner[idx]];
}
create container outer ,
container 0 hey 54 #eq
container 1 hey 42 #eq

\test unary op, apart from ++ and --, *don't* modify their target.
:c int unaryop1(int n) {
    !n;
    return n;
}
42 unaryop1 42 #eq

\test ... even if it's a pointer!
:c int* unaryop2(int *n) {
    !*n;
    return n;
}
create myval 42 ,
myval unaryop2 myval #eq
myval @ 42 #eq

\test typecasting a deref op doesn't apply a "downgrade" mask to the address
:c char unaryop3(int *a, int idx) {
	return (char)a[idx];
}
create myarray 42 , $12345678 ,
1 myarray unaryop3 $78 #eq

\test Function calls with other function calls in arguments
:c int funcall1() {
    return myadder(54, (int)(binopand() + binopand()));
}
funcall1 138 #eq

\test funcall2
:c int funcall2(int x) {
    return myadder(++x, 42);
}
42 funcall2 85 #eq

\test Function signature type is correctly recognised in typedefs
:c void cnoop() {}
:c typedef void (*Voider)();
:c void funcall3() {
	Voider x = cnoop;
	void (*y)() = cnoop;
	x(); y(); // no PS leak/underflow
}
funcall3 scntneutral# \ no PS leak/underflow

\test voidptr return
:c void* _voidptr() { return "hello"; }
:c void funcall4() {
	char *str = (char*)_voidptr(); // this doesn't crash on compilation
}
funcall4 scntneutral#

\test Calling void functions with arguments used to mess up the stack
:c void voidwithargs(int a, int b) { }
:c void funcall5() {
    voidwithargs(2, 3);
    voidwithargs(2, 3);
}
funcall5 scntneutral#

\test funcall with complex expression
\ this call used to forget its PSdisp level in the middle of it
:c int funcall6(int a, int b) {
  return myadder(a + a + a, b + b + b);
}
2 3 funcall6 15 #eq

\test funcall burning through HAL bank
:c int funcall7(int a, int b) {
  return myadder(a + a + a + a + a + a + a + a,
                 b + b + b + b + b + b + b + b);
}

2 3 funcall7 2 8* 3 8* + #eq

\test deep expr requiring deep PS offset adjustments
:c int deepexpr(int x) {
  int y, one = 1;
  MyStruct s;
  s.foo = 6;
  y = (one + 10 - x) * s.foo;
  return y;
}
4 deepexpr 42 #eq

\test don't compare types across && and ||
:c int boolswitch(int x, uint y) { return x && y; }
1 1 boolswitch #true
0 1 boolswitch not #true

testend
