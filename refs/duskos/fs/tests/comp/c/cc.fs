needs tests/harness comp/c
testbegin

:c enum { FOUR=4, FIVE, };

\test binop precedence
:c uint exprbinops() {
    uint a=1, b=2;
    return a + b * FIVE + NULL;
}
exprbinops 11 #eq

\test let's try function prototyping support
:c int myadder(int a, int b);
\ are arguments, both constants and lvalues, properly passed?
\ do we support expressions as arguments?
:c int plusone(int x) {
    return myadder(1, x+x-x);
}
\ implements the above prototype
:c int myadder(int a, int b) {
    return a + b;
}
42 plusone 43 #eq

\test condif
:c int condif(int x) {
  if (x < 42) {
    x = x+100;
  } else {
    x = x+1;
  }
  return x;
}
54 condif 55 #eq
42 condif 43 #eq
41 condif 141 #eq

\test that a void function doesn't add anything to PS
:c void cnoop() {}
cnoop ( no result! ) scntneutral#

\test helloworld
:c uchar *msgs[1] = {"Hello World!"};
:c void helloworld() {
    stype(msgs[0]);
}
exec>str helloworld "Hello World!" #s=

\test nullstr
:c uchar* nullstr() {
	return "Null terminated"0;
}
create expected ,"Null terminated\0"
nullstr expected 16 c[]= #

\test forsum
:c int forsum(int n) {
    int i;
    int r = 0;
    for (i=0; i<n; i++) {
        r = r+i;
    }
    return r;
}
5 forsum 10 #eq

\test multret
:c uint multret(uint x) {
    if (x < 10) {
        return x;
    } else {
        return x-10;
    }
}
1 multret 1 #eq
42 multret 32 #eq

\test multretvoid
:c void multretvoid(uint x) {
    if (x == 42) {
        stype("Answer to the universe");
        return;
    }
    stype("Nope");
}
1234 \ test that void funcs with args don't mess with the PS underneath it.
55 exec>str multretvoid "Nope" #s=
42 exec>str multretvoid "Answer to the universe" #s=
1234 #eq

\test forbreak
:c short forbreak() {
    short i, j;
    for (i=0; i<100; i++) {
        if (i==10) break;
        // the presence of a for() after the break doesn't break "break".
        for (j=0; j<1; j++) {}
    }
    return i;
}
forbreak 10 #eq

\test forcontinue
:c short forcontinue() {
    short i, j=0;
    for (i=0; i<10; i++) {
        if (i==5) continue;
		++j;
    }
    return j;
}
forcontinue 9 #eq

\test the first and last element of for() can be empty
:c void forempty() {
	for (;1;) return;
}
forempty \ no crash

\test while
:c int whilesum(int n) {
    int res = 0;
    while (n) {
        res = res + n--;
    }
    return res;
}
5 whilesum 15 #eq

\test do..while
:c int dowhilesum(int n) {
    int res = 0;
    do {
        res = res + n--;
    } while (n);
    return res;
}
5 dowhilesum 15 #eq

\test support funcsig ident in global arrays and don't mess up thing when ...
\ ... the function's return type isn't 4b
:c typedef short (*ShortRet)();
:c ShortRet globfuncs[3] = {exprbinops, NULL, forbreak};

:c short callfuncidx(int idx) {
    if (idx != 1) {
        return globfuncs[idx]();
    } else {
        return 0;
    }
}
0 callfuncidx 11 #eq
2 callfuncidx 10 #eq

\test switch
:c int switchstmt(int x) {
	int y = 1;
	switch (x) {
		case 42: return 12;
		case 1234: ++y;
		case 'B'-1:
		case 5678: ++y; break;
		default: --y;
	}
	return y;
}
33 switchstmt 0 #eq
42 switchstmt 12 #eq
1234 switchstmt 3 #eq
'A' switchstmt 2 #eq
5678 switchstmt 2 #eq

\test nested switch
:c int nestedswitch() {
	int x = 7;
	switch (1) {
        case 1: x = 9;
        case 2: switch (4) { case 3: break; default: break; } break;
        default: break;
	}
	return x;
}
nestedswitch 9 #eq

\test break leak
\ ensure that breaking from a loop that needs to do a "psrestore" doesn't
\ corrupt PS.
:c uint dontleak(uint x) {
  while (--x) {
    x = x + x - x;
    if (x+x+1 == 5) break;
  }
  return x;
}
4 dontleak 2 #eq
scntneutral# \ don't leak!

\test goto
:c int gotostmt(int x) {
  if (x==42) goto forwardlbl;
mylbl:
  --x;
  if (x) goto mylbl;
forwardlbl:
  return x;
}
12 gotostmt 0 #eq
42 gotostmt 42 #eq

\test logshort
:c int logshort(int a) {
  int b = 12;
  if ((a < 42) && (b = a)) {
    return b;
  } else {
    return b+1;
  }
}
42 logshort 13 #eq
41 logshort 41 #eq

\test that non-shorcutted && yield the correct result
:c int foo(int a) {
  if ((a<42) && (a!=22)) return 1234; else return a;
}
55 foo 55 #eq
33 foo 1234 #eq
22 foo 22 #eq

\ Below this comment are simple construct that were buggy before
\test assigning a pointer to another pointer doesn't trigger pointer arithmetics
:c void globptrassign() {
    globfuncs[1] = forcontinue;
}
globptrassign \ no crash

\test we used to leak VM ops in condition blocks without {}
:c void cond1() {
    int x = 42;
    if (x==0) x++; else x--;
}

\test Having a return statement in a conditional, if nothing came after it...
\ ... would prevent the parent from having an implicit return.
:c void cond2() { if (0) return; }
cond2 scntneutral# \ don't crash or leak

\test The i386 VM always performed "test" on a 4b width
:c short opwidth6() {
    short x = 42;
    short y = 0;
    if (y) return 0; else return 1;
}
opwidth6 1 #eq

\test There used to be a bug in the forth VM where an expression ...
\ (something that isn't a simple reference, but the result of a computation)
\ with an arg on PS would mess PS up.
:c char* switch1(char *x) {
	switch (x+1) {
		case 43: return (char *)43;
	}
	return x;
}
41 switch1 41 #eq
42 switch1 43 #eq

\test The i386 VM used to misallocate EAX because vmop was a VM_*REGISTER=EAX
:c int switch2() {
	char c = 'X';
	char *p = &c;
	switch (*p) {
		case 'X': return 42;
	}
	return 0;
}
switch2 42 #eq

\test backtick escape
42 const hey!
:c int foo() { return `hey! ; }
foo 42 #eq

\test functions in test.c
cc<< /tests/comp/c/test.c
retconst 42 #eq
variables 82 #eq

\test If a C file doesn't end with a newline, will it fail?
cc<< /tests/comp/c/nonl.c
nonl 54 #eq
testend
