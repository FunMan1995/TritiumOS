needs tests/harness lib/type lib/tagl mem/cons comp/sym comp/oberon
testbegin

\ NOTE: tests below are not comprehensive. Writing a comprehensive suite is
\ really boring. A big part of test coverage is done through tests of the Oberon
\ system itself. When those tests reveal bugs in the compiler, a more precise
\ test is added here.

\test add const
oberon< MODULE Test;
PROCEDURE expr1(x: INTEGER): INTEGER;
BEGIN RETURN x+22 END expr1;
END Test.

12 Test.expr1 34 #eq
"Test.expr1" findannotated #true
exec>str .type "( INTEGER -- INTEGER )" #s=

\test expr that pushes to PS
oberon< MODULE Test;
PROCEDURE expr2*(x,y: INTEGER): INTEGER;
BEGIN RETURN (x+1)-(y+2) END expr2;
END Test.

12 23 Test.expr2 10 #eq

\test assign string to CHAR
oberon< MODULE Test;
PROCEDURE expr3(): CHAR;
BEGIN RETURN 78X END expr3;

PROCEDURE expr4(): CHAR;
VAR ch: CHAR;
BEGIN ch := 79X; RETURN ch END expr4;
END Test.

Test.expr3 'x' #eq
Test.expr4 'y' #eq

\test constant expression
oberon< MODULE Test;
PROCEDURE expr5(): INTEGER;
BEGIN RETURN 3 + 4 END expr5;
END Test.

Test.expr5 7 #eq

\test set operators
oberon< MODULE Test;
PROCEDURE expr6(x, y: SET): SET;
BEGIN RETURN x * y END expr6;
END Test.

5 3 Test.expr6 1 #eq

\test INC with second parameter
oberon< MODULE Test;
PROCEDURE expr7(x, y: INTEGER): INTEGER;
BEGIN
  INC(x, y*3);
  RETURN x
END expr7;
END Test.

5 3 Test.expr7 18 #eq

\test ~ operator
\ I stupidly hadn't understood that it was a boolean operator.
oberon< MODULE Test;
PROCEDURE expr8(x: BOOLEAN): BOOLEAN;
BEGIN RETURN ~x END expr8;
END Test.

0 Test.expr8 1 #eq
1 Test.expr8 0 #eq
42 Test.expr8 0 #eq

\test BYTE overflow in INTEGER promotion
oberon< MODULE Test;
PROCEDURE expr9(x: INTEGER): INTEGER;
VAR b: BYTE;
BEGIN b := x; RETURN 100H*b END expr9;
END Test.

42 Test.expr9 $2a00 #eq

\test comparison with bothW AND BothW on right side
oberon< MODULE Test;
PROCEDURE expr10(x, y, a, b: INTEGER): BOOLEAN;
BEGIN RETURN x + a < (y + b) + (y - y) END expr10;
END Test.

1 2 3 4 Test.expr10 0 #eq
42 2 3 4 Test.expr10 1 #eq

\test funcall in comparison
oberon< MODULE Test;
PROCEDURE private(x: INTEGER): INTEGER;
BEGIN RETURN x+1 END private;

PROCEDURE expr11*(x, y: INTEGER): BOOLEAN;
BEGIN RETURN (private(x) < y + y) END expr11;
END Test.

1 2 Test.expr11 0 #eq
2 2 Test.expr11 1 #eq

\test signed division
oberon< MODULE Test;
PROCEDURE expr12*(x, y: INTEGER): INTEGER;
BEGIN RETURN x / y END expr12;
END Test.

3 -43 Test.expr12 -14 #eq

\test MIN/MAX
oberon< MODULE Test;
PROCEDURE expr13*(x, y: INTEGER): INTEGER;
BEGIN x := MIN(x, y+0); RETURN x END expr13;
PROCEDURE expr14*(x, y: INTEGER): INTEGER;
BEGIN RETURN MAX(x, y) END expr14;
END Test.

3 -43 Test.expr13 -43 #eq
3 42 Test.expr13 3 #eq
3 -43 Test.expr14 3 #eq
3 42 Test.expr14 42 #eq

\test LSL
oberon< MODULE Test;
PROCEDURE expr15*(x, y: INTEGER): INTEGER;
BEGIN RETURN LSL(x, y) END expr15;
END Test.

4 $2a Test.expr15 $2a0 #eq

\test local variables
oberon< MODULE Test;
PROCEDURE lvar1*(x: INTEGER): INTEGER;
VAR y: INTEGER;
BEGIN
  y := -x; (*empty statements allowed*);
  RETURN y
END lvar1;
END Test.

$2a Test.lvar1 -42 #eq

\test assignment to parameter
oberon< MODULE Test;
PROCEDURE lvar2*(x: INTEGER): INTEGER;
BEGIN
  x := -x;
  RETURN x
END lvar2;
END Test.

$2a Test.lvar2 -42 #eq

\test GET() into a BYTE
oberon< MODULE Test;
PROCEDURE lvar3*(adr: INTEGER): BYTE;
VAR x: BYTE;
BEGIN GET(adr, x); RETURN x END lvar3;
END Test.

create data 42 c, 54 c, 123 c,
data Test.lvar3 42 #eq

\test PUT() with a bothW preserves BYTE typesz)
oberon< MODULE Test;
PROCEDURE put1*(adr: INTEGER; val: BYTE);
VAR arr: ARRAY 1 OF BYTE;
BEGIN arr[0] := val; PUT(adr+1, arr[0]) END put1;
END Test.

111 data Test.put1
data c@ 42 #eq
data 1+ c@ 111 #eq
data 2+ c@ 123 #eq

\test VAR parameter
create data 1234 , 5678 ,
oberon< MODULE Test;
PROCEDURE ptr1*(VAR ptr: INTEGER): INTEGER;
BEGIN RETURN ptr END ptr1;
END Test.

data 4+ Test.ptr1 5678 #eq

\test assignment to VAR parameter
oberon< MODULE Test;
PROCEDURE ptr2*(VAR ptr: INTEGER; val: INTEGER);
BEGIN ptr := val END ptr2;
END Test.

2345 data Test.ptr2
data @ 2345 #eq
1234 data Test.ptr2
data @ 1234 #eq

\test BYTE indexed assignment to VAR parameter
oberon< MODULE Test;
PROCEDURE ptr3*(VAR ptr: INTEGER; arr: ARRAY OF BYTE);
BEGIN ptr := arr[1] END ptr3;
END Test.

create myarr 42 c, 54 c,
2 myarr data Test.ptr3
data @ 54 #eq
1234 data Test.ptr2
scntneutral# \ don't leak to PS!

\test VAR derefercing on assignment preserves size
oberon< MODULE Test;
PROCEDURE private(VAR x: BYTE);
BEGIN x := x + 1 END private;

PROCEDURE ptr4*():BYTE;
VAR a,b:BYTE;
BEGIN a := 42; b := 54; private(a); RETURN b END ptr4;
END Test.

Test.ptr4 54 #eq

\test array element in local variable can be passed to VAR argument
oberon< MODULE Test;
PROCEDURE private(VAR x: INTEGER);
BEGIN x := x + 1 END private;

PROCEDURE ptr5*(): INTEGER;
VAR x: ARRAY 2 OF INTEGER;
BEGIN x[0] := 42; x[1] := 54; private(x[1]); RETURN x[1] END ptr5;
END Test.

Test.ptr5 55 #eq

\test VAR argument can be passed to non-VAR arg of other proc
oberon< MODULE Test;
PROCEDURE private(x: INTEGER): INTEGER;
BEGIN RETURN x + 1 END private;

PROCEDURE ptr6*(VAR x: INTEGER): INTEGER;
BEGIN RETURN private(x) END ptr6;
END Test.

create data 42 ,
data Test.ptr6 43 #eq

\test assignment auto-derefs right side
struct MyStruct { uint foo bar ; }

oberon< MODULE Test;
PROCEDURE ptr7*(VAR x: INTEGER): INTEGER; (* anyW=0 *)
VAR y: INTEGER;
BEGIN y := x; RETURN y END ptr7;

PROCEDURE ptr8*(VAR x: INTEGER): INTEGER; (* anyW=left *)
VAR y: ARRAY 1 OF INTEGER;
BEGIN y[0] := x; RETURN y[0] END ptr8;

PROCEDURE ptr9*(VAR x: DUSK.MyStruct): INTEGER; (* anyW=right *)
VAR y: INTEGER;
BEGIN y := x.bar RETURN y END ptr9;

PROCEDURE ptr10*(VAR x: DUSK.MyStruct): INTEGER; (* bothW=1 *)
VAR y: ARRAY 1 OF INTEGER;
BEGIN y[0] := x.bar RETURN y[0] END ptr10;
END Test.

create data 42 , 54 ,
data Test.ptr7 42 #eq
data Test.ptr8 42 #eq
MyStruct data Test.ptr9 54 #eq
MyStruct data Test.ptr10 54 #eq

\test INC with VAR argument
oberon< MODULE Test;
PROCEDURE ptr11*(VAR x: INTEGER);
BEGIN INC(x); INC(x, 2); END ptr11;
END Test.

create data 42 ,
data Test.ptr11
data @ 45 #eq

\test string assignment
oberon< MODULE Test;
TYPE StructWithName* = RECORD name: ARRAY 32 OF CHAR END;
PROCEDURE ptr11*(name: ARRAY OF CHAR; VAR dst: StructWithName);
BEGIN dst.name := name END ptr11;
END Test.

create src ,"Hello!\0"
create dst 32 allot0
Test.StructWithName dst 7 src Test.ptr11
dst z[] []>str "Hello!" #s=

\test VAR BYTE argument read is of the correct width
oberon< MODULE Test;
PROCEDURE ptr12*(VAR x: BYTE): BOOLEAN;
BEGIN RETURN x = 42 END ptr12;
END Test.

create data 42 c, 54 c,
data Test.ptr12 1 #eq

\test OPAQUE variables never dereferences
: foobar 42 ;
annotatelast ( -- *uint )

oberon< MODULE Test;
PROCEDURE ptr13*(): OPAQUE;
VAR x: OPAQUE;
BEGIN x := `foobar (); RETURN x END ptr13;
END Test.

Test.ptr13 42 #eq

\test VAR struct argument deepcopy
oberon< MODULE Test;
PROCEDURE ptr14*(VAR a, b: DUSK.MyStruct);
BEGIN a := b END ptr14;
END Test.

create src ,"abcdefgh"
create dst 8 allot0

MyStruct src MyStruct dst Test.ptr14
src dst 8 c[]= #true

\test VAR parameter assigment and set
oberon< MODULE Test;
PROCEDURE ptr15*(a: INTEGER; VAR b: SET);
BEGIN b := b + {a} END ptr15;
END Test.

variable b
$2a b !
b 2 Test.ptr15
b @ $2e #eq

\test fixed array
oberon< MODULE Test;
PROCEDURE fixedarray*(VAR a: ARRAY 2 OF INTEGER; idx: INTEGER): INTEGER;
BEGIN RETURN a[idx] END fixedarray;
END Test.

create data 1234 , 5678 ,
0 data Test.fixedarray 1234 #eq
1 data Test.fixedarray 5678 #eq
2 data expectabort Test.fixedarray

\test open array
oberon< MODULE Test;
PROCEDURE openarray*(a: ARRAY OF INTEGER; idx: INTEGER): INTEGER;
BEGIN RETURN a[idx] END openarray;
END Test.

0 2 data Test.openarray 1234 #eq
1 2 data Test.openarray 5678 #eq
2 2 data expectabort Test.openarray

\test can index a VAR array
oberon< MODULE Test;
PROCEDURE vararray*(n: INTEGER): INTEGER;
VAR a: ARRAY 3 OF INTEGER;
BEGIN a[1] := n+1; RETURN a[1]+1 END vararray;
END Test.

42 Test.vararray 44 #eq

\test can call an open array with an array variable
oberon< MODULE Test;
PROCEDURE foo(arr: ARRAY OF INTEGER);
BEGIN arr[1] := 42 END foo;

PROCEDURE vararraycall*(): INTEGER;
VAR a: ARRAY 3 OF INTEGER;
BEGIN foo(a); RETURN a[1] END vararraycall;
END Test.

Test.vararraycall 42 #eq

\test procedure call to a private proc
oberon< MODULE Test;
PROCEDURE private1(x,y: INTEGER): INTEGER;
BEGIN RETURN (x+1)-(y+2) END private1;

PROCEDURE proccall1*(x:INTEGER): INTEGER;
BEGIN RETURN private1(x, 42) END proccall1;
END Test.

100 Test.proccall1 57 #eq
"private1" sysdict findentry not #true

\test procedure call to other module
oberon< MODULE Other;
PROCEDURE proccall2*(x:INTEGER): INTEGER;
BEGIN RETURN Test.expr2(x, 43) END proccall2;
END Other.

100 Other.proccall2 56 #eq

\test procedure sig check order
oberon< MODULE Test;
PROCEDURE foo(x: INTEGER; y: CHAR);
BEGIN END foo;
PROCEDURE bar(x: INTEGER; y: CHAR);
BEGIN foo(x, y) END bar;
END Test.
\ the test is to compile without error

\test SET literal
\ we use the same bit order as lib/bit, that is bit0 is LSB and bit31 is MSB
oberon< MODULE Test;
PROCEDURE set1*(): SET;
BEGIN RETURN {1,12..14} END set1;
END Test.

Test.set1 $00007002 #eq

\test SET non-constant literal
oberon< MODULE Test;
PROCEDURE set2*(x: INTEGER): SET;
BEGIN RETURN {1,x,12..14} END set2;
END Test.

0 Test.set2 $00007003 #eq
1 Test.set2 $00007002 #eq
31 Test.set2 $80007002 #eq

\test non-constant .. literal
oberon< MODULE Test;
PROCEDURE set3*(x, y: INTEGER): SET;
BEGIN RETURN {1,x..y} END set3;
END Test.

14 12 Test.set3 $00007002 #eq
14 15 Test.set3 $00000002 #eq
14 7 Test.set3 $00007f82 #eq

\test .. literal w/ only the high part as constant
oberon< MODULE Test;
PROCEDURE set4*(x: INTEGER): SET;
BEGIN RETURN {1,x..14} END set4;
END Test.

12 Test.set4 $00007002 #eq
15 Test.set4 $00000002 #eq
7 Test.set4 $00007f82 #eq

\test can dereference a structure from lib/struct
oberon< MODULE Test;
PROCEDURE struct1*(VAR data: DUSK.MyStruct): INTEGER;
BEGIN RETURN data.bar END struct1;
END Test.

create data 1234 , 5678 , 9012 ,
MyStruct data Test.struct1 5678 #eq

\test can create structs in Oberon
oberon< MODULE Test;
TYPE ObStruct* = RECORD
  baz, quux: INTEGER
END;
PROCEDURE struct2*(VAR data: ObStruct): INTEGER;
BEGIN RETURN data.quux END struct2;
END Test.

Test.ObStruct data Test.struct2 5678 #eq

\test can refer to types in other modules
oberon< MODULE Other;
PROCEDURE struct3*(VAR data: Test.ObStruct): INTEGER;
BEGIN RETURN data.quux END struct3;
END Other.

Test.ObStruct data Other.struct3 5678 #eq

\test can assign value to struct field
oberon< MODULE Test;
PROCEDURE struct4*(VAR data: DUSK.MyStruct; val: INTEGER);
BEGIN data.bar := val+1 END struct4;
END Test.

$cafebabe MyStruct data Test.struct4
data 4+ @ $cafebabf #eq

\test can extend struct
oberon< MODULE Test;
TYPE MyStructExt* = RECORD(DUSK.MyStruct) ext: INTEGER END;
PROCEDURE struct5*(VAR data: MyStructExt): INTEGER;
BEGIN RETURN data.ext END struct5;
END Test.

Test.MyStructExt data Test.struct5 9012 #eq

\test struct field accesses are of correct width
oberon< MODULE Test;
TYPE StructWithByte* = RECORD foo: BYTE END;
PROCEDURE struct6*(VAR data: StructWithByte): BYTE;
BEGIN RETURN data.foo END struct6;
END Test.

create data 42 c, 54 c,
Test.StructWithByte data Test.struct6 42 #eq

\test array of structs in local variables behave well in exprs
oberon< MODULE Test;
PROCEDURE struct7*(): INTEGER;
VAR arr: ARRAY 2 OF ObStruct;
BEGIN
  arr[1].baz := 3;
  arr[1].quux := 4;
  RETURN arr[1].quux * arr[1].baz
END struct7;
END Test.

Test.struct7 12 #eq

\test IS operator on GCed pointers
oberon< MODULE Test;
TYPE MyStructPtr = POINTER TO DUSK.MyStruct;
     MyStructExtPtr = POINTER TO MyStructExt;

PROCEDURE private(s: MyStructPtr): BOOLEAN;
BEGIN RETURN s IS MyStructExt END private;

PROCEDURE struct8*(): BOOLEAN;
VAR s: MyStructPtr;
BEGIN NEW(s); RETURN private(s) END struct8;

PROCEDURE struct9*(): BOOLEAN;
VAR s: MyStructExtPtr;
BEGIN NEW(s); RETURN private(s) END struct9;
END Test.

Test.struct8 not #true
Test.struct9 #true

\test IS operator on VAR argument
oberon< MODULE Test;
PROCEDURE private(VAR s: DUSK.MyStruct): BOOLEAN;
BEGIN RETURN s IS MyStructExt END private;

PROCEDURE struct10*(): BOOLEAN;
VAR s: DUSK.MyStruct;
BEGIN RETURN private(s) END struct10;

PROCEDURE struct11*(): BOOLEAN;
VAR s: MyStructExt;
BEGIN RETURN private(s) END struct11;
END Test.

Test.struct10 not #true
Test.struct11 #true

\test VAR structs aren't auto-dereferenced in pointer comparison
oberon< MODULE Test;
PROCEDURE struct12*(VAR s1, s2: DUSK.MyStruct): BOOLEAN;
BEGIN RETURN s1 = s2 END struct12;
END Test.

create data1 42 ,
create data2 42 , \ same data, different pointer

MyStruct data1 2dup Test.struct12 #true
MyStruct data1 MyStruct data2 Test.struct12 not #true

\test struct deepcopy through ^
oberon< MODULE Test;
TYPE ObStructPtr = POINTER TO ObStruct;
PROCEDURE struct13*(): INTEGER;
VAR x, y: ObStructPtr;
BEGIN
  NEW(x); NEW(y);
  x.baz := 10; x.quux := 11;
  y^ := x^;
  RETURN y.quux
END struct13;
END Test.

Test.struct13 11 #eq

\test gcptrs can be passed around as VAR arguments
oberon< MODULE Test;
TYPE MyStructPtr = POINTER TO DUSK.MyStruct;

PROCEDURE private1(VAR x: MyStructPtr);
VAR y: MyStructPtr;
BEGIN NEW(x); NEW(y); y.foo := 42; x^ := y^ END private1;

PROCEDURE struct14*(): INTEGER;
VAR s: MyStructPtr;
BEGIN private1(s); RETURN s.foo END struct14;

PROCEDURE private2(VAR x, y: MyStructPtr): BOOLEAN;
BEGIN RETURN x = y END private2;

PROCEDURE struct15*(): BOOLEAN;
VAR a, b: MyStructPtr;
BEGIN NEW(a); NEW(b); RETURN private2(a, b) END struct15;

PROCEDURE struct16*(): BOOLEAN;
VAR a, b: MyStructPtr;
BEGIN NEW(a); b := a; RETURN private2(a, b) END struct16;

END Test.

Test.struct14 42 #eq
Test.struct15 0 #eq
Test.struct16 1 #eq

\test POINTER can do backward and forward references
oberon< MODULE Test;
TYPE DummyRec = RECORD foo: BYTE END;
     DummyPtr* = POINTER TO DummyRec;
     DummyExtPtr* = POINTER TO DummyRecExt;
     DummyRecExt = RECORD(DummyRec) bar: BYTE END;
END Test.

Test.DummyPtr reftype typesz 1 #eq
Test.DummyExtPtr reftype typesz 2 #eq

\test can declare CONST
oberon< MODULE Test;
CONST hey* = 42;
PROCEDURE const1*(): INTEGER;
BEGIN RETURN hey END const1;
END Test.

Test.const1 42 #eq

\test can declare global VAR
oberon< MODULE Test;
VAR globvar*: INTEGER;
PROCEDURE glob1*(): INTEGER;
BEGIN
  globvar := globvar + 1;
  RETURN globvar
END glob1;
END Test.

Test.glob1 1 #eq
Test.glob1 2 #eq
Test.glob1 3 #eq

\test can refer to public consts and vars in other modules
oberon< MODULE Other;
PROCEDURE glob2*(): INTEGER;
BEGIN
  Test.globvar := Test.globvar + Test.hey;
  RETURN Test.globvar
END glob2;
END Other.

Other.glob2 45 #eq
Test.glob1 46 #eq

\test global variable accesses are of the correct width
oberon< MODULE Test;
VAR globbyte*: BYTE; filler: INTEGER;
PROCEDURE glob3*(): BYTE;
BEGIN
  RETURN globbyte
END glob3;
END Test.

obvar' Test.globbyte $cafebabe over ! 42 swap c!
Test.glob3 42 #eq

\test LEN
oberon< MODULE Test;
PROCEDURE len1*(VAR x: ARRAY 42 OF BYTE): INTEGER;
BEGIN RETURN LEN(x) END len1;

PROCEDURE len2*(x: ARRAY OF BYTE): INTEGER;
BEGIN RETURN LEN(x) END len2;
END Test.

data Test.len1 42 #eq
34 data Test.len2 34 #eq

\test proper procedure call
\ globvar and glob1 is from test above
oberon< MODULE Test;
PROCEDURE private(x:INTEGER);
BEGIN Test.globvar := x END private;

PROCEDURE proccall3*(x:INTEGER);
BEGIN private(x) END proccall3;
END Test.

54 Test.proccall3
Test.glob1 55 #eq

\test assignment with an argless proccall doesn't leak PS
oberon< MODULE Test;
PROCEDURE private(): INTEGER;
BEGIN RETURN 42 END private;

PROCEDURE proccall4*();
VAR x: INTEGER;
BEGIN x := private() END proccall4;
END Test.

Test.proccall4
scntneutral#

\test inline callback
oberon< MODULE Test;

PROCEDURE proccall5*(cb: PROCEDURE(x: INTEGER));
BEGIN cb(42); END proccall5;
END Test.

variable bar
: foo bar ! ;
' foo Test.proccall5
bar @ 42 #eq

\test string literal
oberon< MODULE Test;
PROCEDURE strlit1*();
BEGIN `DebugStr ("Hello!") END strlit1;
END Test.

exec>str Test.strlit1 "Hello!" #s=

\test string comparisons (eq)
\ also, test that VAR on an open array has no effect
oberon< MODULE Test;
PROCEDURE streq1*(VAR str1: ARRAY OF CHAR; str2: ARRAY OF CHAR):BOOLEAN;
BEGIN RETURN str1 = str2 END streq1;
END Test.

3 "foo" str>zstr 3 "foo" str>zstr Test.streq1 #true
3 "foo" str>zstr 3 "bar" str>zstr Test.streq1 not #true
3 "foo" str>zstr 3 "foobar" str>zstr Test.streq1 not #true
3 "foobar" str>zstr 3 "foo" str>zstr Test.streq1 not #true

\test string comparison with "invisible" part
oberon< MODULE Test;
PROCEDURE streq2*(): BOOLEAN;
VAR s1, s2: ARRAY 16 OF CHAR;
BEGIN
  s1 := "Hello";
  s2 := s1;
  RETURN s1 = s2
END streq2;
END Test.

Test.streq2 #true

\test string comparisons (lt)
oberon< MODULE Test;
PROCEDURE strlt*(str1: ARRAY OF CHAR; str2: ARRAY OF CHAR):BOOLEAN;
BEGIN RETURN str1 < str2 END strlt;
END Test.

3 "foo" str>zstr 3 "bar" str>zstr Test.strlt #true
3 "bar" str>zstr 3 "foo" str>zstr Test.strlt not #true
4 "foo2" str>zstr 4 "foo1" str>zstr Test.strlt #true

\test can call indirect procedures
oberon< MODULE Test;
TYPE MyFunc = PROCEDURE (x: INTEGER): INTEGER;
PROCEDURE indcall1*(f: MyFunc; val: INTEGER): INTEGER;
BEGIN RETURN f(val) END indcall1;
END Test.

42 ' 1+ Test.indcall1 43 #eq

\test RECORD can hold indirect procedures
oberon< MODULE Test;
TYPE MyRec* = RECORD func: PROCEDURE (x: INTEGER): INTEGER END;
PROCEDURE indcall2*(VAR r: MyRec; val: INTEGER): INTEGER;
BEGIN RETURN r.func(val)+1 END indcall2;
END Test.

create myrec ' 1+ ,
42 Test.MyRec myrec Test.indcall2 44 #eq

\test CASE statement
oberon< MODULE Test;
PROCEDURE case1*(x: INTEGER): INTEGER;
VAR res: INTEGER;
BEGIN
  res := 12;
  CASE x OF
    42: res := 54 |
    123: res := 112 |
    666: res := 777
  END
  RETURN res
END case1;
END Test.

42 Test.case1 54 #eq
123 Test.case1 112 #eq
666 Test.case1 777 #eq
1234 Test.case1 12 #eq

\test CASE with RECORD GC pointer
oberon< MODULE Test;
TYPE BaseDesc = RECORD foo: INTEGER END;
     Sub1Desc = RECORD(BaseDesc) bar: INTEGER END;
     Sub2Desc = RECORD(BaseDesc) baz: INTEGER END;
     Base = POINTER TO BaseDesc;
     Sub1 = POINTER TO Sub1Desc;
     Sub2 = POINTER TO Sub2Desc;

PROCEDURE private(r: Base): INTEGER;
VAR res: INTEGER;
BEGIN
  res := 123;
  CASE r OF
    Sub1: res := r.foo |
    Sub2Desc: res := r.baz (* both POINTER and direct forms work *)
  END
  RETURN res
END private;

PROCEDURE case2*: INTEGER;
VAR r: Sub1;
BEGIN NEW(r); r.foo := 42; RETURN private(r) END case2;

PROCEDURE case3*: INTEGER;
VAR r: Sub2;
BEGIN NEW(r); r.baz := 54; RETURN private(r) END case3;
END Test.

Test.case2 42 #eq
Test.case3 54 #eq

\test CASE with RECORD VAR pointer
oberon< MODULE Test;
TYPE Base = RECORD foo: INTEGER END;
     Sub1 = RECORD(Base) bar: INTEGER END;
     Sub2 = RECORD(Base) baz: INTEGER END;
     SubSub = RECORD(Sub1) END;

PROCEDURE private(VAR r: Base): INTEGER;
VAR res: INTEGER;
BEGIN
  res := 123;
  CASE r OF
    Sub1: res := r.foo |
    Sub2: res := r.baz
  END
  RETURN res
END private;

(* Type is properly carried across calls! *)
PROCEDURE indirect(VAR r: Base): INTEGER;
BEGIN RETURN private(r) END indirect;

PROCEDURE case4*: INTEGER;
VAR r: Sub1;
BEGIN r.foo := 42; RETURN indirect(r) END case4;

PROCEDURE case5*: INTEGER;
VAR r: Sub2;
BEGIN r.baz := 54; RETURN indirect(r) END case5;

PROCEDURE case6*: INTEGER;
VAR r: SubSub;
BEGIN r.foo := 88; RETURN indirect(r) END case6;
END Test.

Test.case4 42 #eq
Test.case5 54 #eq
Test.case6 88 #eq

\test FOR statement
oberon< MODULE Test;
PROCEDURE for1(): INTEGER;
VAR res, i: INTEGER;
BEGIN
  res := 0;
  FOR i := 0 TO 9 DO res := res + i END;
  RETURN res
END for1;
END Test.

Test.for1 45 #eq

\test FOR statement with VAR argument loop var
oberon< MODULE Test;
PROCEDURE for2(VAR i: INTEGER): INTEGER;
VAR res: INTEGER;
BEGIN
  res := 0;
  FOR i := 0 TO 9 DO res := res + i END;
  RETURN res
END for2;
END Test.

variable foo
foo Test.for2 45 #eq
foo @ 10 #eq

\test GC typeguards
oberon< MODULE Test;
TYPE BaseDesc = RECORD foo: INTEGER END;
     SubDesc = RECORD(BaseDesc) bar: INTEGER END;
     Base = POINTER TO BaseDesc;
     Sub* = POINTER TO SubDesc;

PROCEDURE private1(r: Base): INTEGER;
BEGIN RETURN r(Sub).bar END private1;

PROCEDURE typeguard1*(): INTEGER;
VAR r: Sub;
BEGIN NEW(r); r.foo := 42 ; r.bar := 54; RETURN private1(r) END typeguard1;

PROCEDURE private2(r: Base): INTEGER;
BEGIN RETURN r(Test.Sub).bar END private2;

PROCEDURE typeguard2*(): INTEGER;
VAR r: Sub;
BEGIN NEW(r); r.foo := 42 ; r.bar := 54; RETURN private2(r) END typeguard2;
END Test.

Test.typeguard1 54 #eq
Test.typeguard2 54 #eq


\test non-GC typeguards
oberon< MODULE Test;

TYPE BaseDesc = RECORD foo: INTEGER END;
     SubDesc = RECORD(BaseDesc) bar: INTEGER END;
     Base = POINTER TO BaseDesc;
     Sub = POINTER TO SubDesc;

PROCEDURE private1(VAR r: BaseDesc): INTEGER;
BEGIN RETURN r(SubDesc).bar END private1;

PROCEDURE typeguard3*(): INTEGER;
VAR r: SubDesc;
BEGIN r.foo := 42 ; r.bar := 54; RETURN private1(r) END typeguard3;

PROCEDURE private2(VAR r: BaseDesc): INTEGER;
BEGIN RETURN r(SubDesc).bar END private2;

PROCEDURE typeguard4*(): INTEGER;
VAR r: SubDesc;
BEGIN r.foo := 42 ; r.bar := 54; RETURN private2(r) END typeguard4;
END Test.

Test.typeguard3 54 #eq
Test.typeguard4 54 #eq

\test module initialization clears local symbols
oberon< MODULE Test;
VAR foo: INTEGER;
PROCEDURE GetFoo*: INTEGER;
BEGIN RETURN foo END GetFoo;

PROCEDURE MessUp;
VAR foo: INTEGER;
BEGIN END MessUp;

BEGIN foo := 42;
END Test.

Test.GetFoo 42 #eq

\test WHILE and ELSIF
oberon< MODULE Test;
PROCEDURE while1(a, b: INTEGER): INTEGER;
VAR res1, res2: INTEGER;
BEGIN
  res1 := 0; res2 := a*b;
  WHILE res1 < a * b DO INC(res1)
  ELSIF res2 > a + b DO DEC(res2) END;
  RETURN LSL(res2, 8) + res1
END while1;
END Test.

3 5 Test.while1 $80f #eq

\test BOOLEAN and VAR and IF
oberon< MODULE Test;
PROCEDURE boolvar(VAR x: BOOLEAN): INTEGER;
VAR r: INTEGER;
BEGIN IF x THEN r := 42 ELSE r := 54 END RETURN r END boolvar;
END Test.

variable b
b Test.boolvar 54 #eq
1 b c!
b Test.boolvar 42 #eq

\test BOOLEAN and VAR and WHILE and &
oberon< MODULE Test;
PROCEDURE Combi*(VAR b: BOOLEAN);
   VAR bb: BOOLEAN;
BEGIN bb := TRUE;
   WHILE b & bb DO bb := FALSE END;
   WHILE bb OR b DO b := FALSE END;
END Combi;
END Test.
\ just test that it compiles

\test VAL with fully qualify type
oberon< MODULE Other;
TYPE Foo* = INTEGER;
END Other.
oberon< MODULE Test;
PROCEDURE valqual(x: INTEGER): Other.Foo;
BEGIN RETURN VAL(Other.Foo, x) END valqual;
END Test.

42 Test.valqual 42 #eq

\test NIL literal as argument
oberon< MODULE Test;
TYPE
 Type = POINTER TO TypeDesc;
 TypeDesc = RECORD END ;
PROCEDURE Proc(T: Type);
BEGIN
END Proc;

PROCEDURE Call*;
BEGIN Proc(NIL)
END Call;
END Test.
\ just test that it compiles

\test VAR argument as array indexes
oberon< MODULE Test;
PROCEDURE GetChar(VAR idx: INTEGER; arr: ARRAY OF CHAR): CHAR;
BEGIN RETURN arr[idx] END GetChar;
END Test.

create myarr ,"hello"
variable y
4 y !
5 myarr y Test.GetChar 'o' #eq
1 y !
5 myarr y Test.GetChar 'e' #eq

\test deepcopy where destination has an offset
\ previously, the offset calculated for dst's ".b" was lost, so deepcopy would
\ overwrite the struct's beginning

oberon< MODULE Test;
TYPE
 MyStruct = RECORD a: INTEGER; b: ARRAY 4 OF CHAR END ;

PROCEDURE DeepCopy(VAR src, dst: MyStruct);
BEGIN dst.b := src.b END DeepCopy;
END Test.

create mysrc 42 , n"hey!" ,
create mydst 54 , 0 ,
0 mydst 0 mysrc Test.DeepCopy
mydst @ 54 #eq
mydst 4+ @ n"hey!" #eq

\test complex expression that used to be buggy
oberon< MODULE Test;
TYPE
 RP = POINTER TO R;
 R = RECORD f: INTEGER END;

PROCEDURE ComplexExpr(y: INTEGER): INTEGER;
VAR
   x, one: INTEGER;
   T: RP;
BEGIN
  NEW(T);
  T.f := 6;
  one := 1;
  x := (one + 10 - y) * T.f;
  RETURN x
END ComplexExpr;
END Test.

4 Test.ComplexExpr 42 #eq

\ The modules below each contain code that is expected to abort
\ We don't use oberon<< because expectabort really doesn't play well with exec<
\ Parsed code only go up to the exact point where the error is expected, with a
\ STOP where the parser *shouldn't* go to. Therefore, we expect "STOP" to be
\ ran as a *forth* word. As a stopgap for overeager parsing leading to false
\ successes, we call noop after STOP.
scntneutral#
variable stopped
: STOP 1 stopped ! ;
: stopped# 0 stopped @! not ?abort"STOP wasn't called" ;

\test can't use a proper procedure in an expression
expectabort oberon<
MODULE Err;

PROCEDURE foo(x:INTEGER);
BEGIN END foo;

PROCEDURE bar(x: INTEGER);
BEGIN RETURN foo(42); STOP
noop stopped#

\test argcount check in proccall
expectabort oberon<
MODULE Err;

PROCEDURE foo(x: INTEGER): INTEGER;
BEGIN RETURN 42 END foo;

PROCEDURE bar(x: INTEGER): INTEGER;
BEGIN RETURN foo(x, 42); STOP
noop stopped#

\test type check in arguments
expectabort oberon<
MODULE Err;

PROCEDURE foo(x: CHAR): INTEGER;
BEGIN RETURN 42 END foo;

PROCEDURE bar(x: INTEGER): INTEGER;
BEGIN RETURN foo(x); STOP
noop stopped#

\test straight struct without VAR
expectabort oberon<
MODULE Err;

PROCEDURE foo(x: DUSK.MyStruct STOP
noop stopped#

testend
