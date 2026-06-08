needs tests/harness comp/c/ast
testbegin

\test op""
op"+" 3 #eq
: foo op"%" ; foo 2 #eq

\test opname
5 opname "<<" #s=

cctok$
:~ word".ast" exec>str #s= ;
: asteq" [rcompile] " swap ~ ;
: chk" [rcompile] " ast< read; ~ ;

\test lit
chk"(lit 0000002a)" 42;

\test sym
chk"(sym foo)" foo;

\test prefix
chk"(++ (sym a))" ++a;
chk"(-- (sym a))" --a;
chk"(neg (sym a))" -a;

\test postfix
chk"(postinc (sym a))" a++;
chk"(postdec (sym a))" a--;

\test binops
chk"(+ (sym a) (sym b))" a+b;

\test binops priority
chk"(+ (* (sym a) (sym b)) (sym c))" a*b+c;
chk"(+ (sym a) (* (sym b) (sym c)))" a+b*c;
chk"(* (+ (sym a) (sym b)) (sym c))" (a+b)*c;
chk"(* (sym a) (+ (sym b) (sym c)))" a*(b+c);
chk"(= (sym a) (+= (sym b) (sym c)))" a=b+=c;
chk"(+ (* (sym a) (sym b)) (/ (sym c) (sym d)))" a*b+c/d;
chk"(&& (!= (sym a) (sym b)) (!= (sym c) (| (sym d) (sym e))))"
   a != (b) && c != (d|e);
\ |= is the last op. there used to be a missing entry in pri table!
chk"(|= (sym a) (| (sym b) (sym c)))" a |= b | c;


\test spurious parens
chk"(+ (sym a) (sym b))" a+((b));
chk"(+ (sym a) (lit 0000002a))" ((a+(42)));

\test prefix/postfix ambiguity
chk"(+ (postinc (sym a)) (deref (sym b)))" a+++*b;
chk"(-- (postinc (sym a)))" --a++;

\test []
chk"(deref (+ (sym a) (+ (sym b) (sym c))))" a[b+c];

\test function call
chk"(funcall (sym foo) (+ (sym a) (sym b)) (ref (sym c)))" foo(a+b, &c);
chk"(funcall (sym foo) (funcall (sym bar)))" foo(bar());

\test {}
chk"(array (sym a) (+ (sym b) (sym c)) (sym d))" {a, b+c, d};

\test typecast
chk"(typecast int 2 (sym a))" (int * * )a;

\test ?:
chk"(?: (sym a) (+ (sym b) (sym c)) (sym d))" a?b+c:d;
chk"(?: (< (sym a) (sym b)) (sym c) (sym d))" a<b?c:d;

\test string literals
chk"(funcall (sym foo) (str bar))" foo("bar");

\test -> and .
chk"(postinc (-> b (sym a)))" a->b++;
chk"(postdec (-> b (ref (sym a))))" a.b--;
chk"(postdec (-> c (ref (deref (+ (sym a) (sym b))))))" a[b].c--;

\test deref
chk"(= (deref (sym pa)) (sym b))" *pa = b;

\test struct access in binop in parens
chk"(+ (-> b (sym a)) (sym c))" (a->b+c);

\test buggy expr from ar/puff.c
\ The core reason of the bug was a pad overwrite in comp/tok. We keep the
\ expression as-is for an example.
#define OUTBUFLEN $10000
ast< (s.outcnt>=OUTBUFLEN) && ((s.outptr % (OUTBUFLEN/2))); read;
cdrcar op"&&" #eq
cdrcar asteq"(>= (-> outcnt (ref (sym s))) (lit 00010000))"
asteq"(% (-> outptr (ref (sym s))) (/ (lit 00010000) (lit 00000002)))"

\test sizeof
chk"(sizeof short)" sizeof(short);
chk"(lit 00000004)" sizeof(short**);
chk"(sizeof uint)" sizeof(uint);

\test bug that surfaced up in drv/rpi/dwc after "word" began using the strpool
#define FOO (%<)
chk"(= (sym n) (+ (sym hey) (sym man)))" n = FOO "hey+man";

\test typecast with subexpr
chk"(typecast char 0 (- (sym y) (lit 00000002)))" (char)(y-2);

\test typecast with [] inside parens
chk"(<< (typecast int 0 (deref (+ (sym array) (lit 00000000)))) (lit 00000008))"
  ((int)array[0]<<8);

\test not a typecast but almost!
chk"(+ (* (sym a) (sym b)) (sym c))" (a*b)+c;

\test typecasts and literals
chk"(+ (typecast int 1 (lit 00000020)) (lit 00000002))" (int*)$20 + 2;

\test backtick escape
chk"(sym !+)" `!+ ;

\test backtick with typecast
chk"(typecast uint 0 (sym wle@))" (uint)`wle@ ;

\test char literal with escape
chk"(lit 00000078)" 'x' ;
chk"(lit 00000000)" '\0' ;
chk"(lit 0000000a)" '\n' ;
chk"(lit 00000022)" '"' ;
testend
