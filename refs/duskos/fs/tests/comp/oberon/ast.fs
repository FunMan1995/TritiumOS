needs tests/harness comp/oberon/ast
testbegin

obtok$
:~ word".ast" exec>str #s= ;
: expr" [rcompile] " expr< read; ~ ;
: designator" [rcompile] " designator< read; ~ ;

\test lit
expr"(lit 0000002a)" 42;
expr"(lit cafe1234)" 0CAFE1234H;

\test char
expr"(char 78)" 78X;
expr"(char fa)" 0FAX;
expr"(char 41)" 'A';

\test ident
expr"(ident foo)" foo;
expr"(ident CAFE1234H)" CAFE1234H;
expr"(ident FAX)" FAX;

\test `
expr"(() (` allot@))" `allot@ ();

\test prefix unary
expr"(neg (ident a))" -a;
expr"(~ (ident a))" ~a;
expr"(ident a)" +a; \ yup, a "+" prefix does nothing
expr"(~ (ident a))" ~(a);

\test postfix unary
expr"(^ (ident a))" a^;

\test .
expr"(. b (ident a))" a.b;

\test []
expr"([] (ident a) (+ (ident b) (ident c)))" a[b+c];

\test binops
expr"(+ (ident a) (ident b))" a+b;

\test binops priority
expr"(+ (* (ident a) (ident b)) (ident c))" a*b+c;
expr"(+ (ident a) (* (ident b) (ident c)))" a+b*c;
expr"(* (+ (ident a) (ident b)) (ident c))" (a+b)*c;
expr"(* (ident a) (+ (ident b) (ident c)))" a*(b+c);
expr"(+ (* (ident a) (ident b)) (/ (ident c) (ident d)))" a*b+c/d;
expr"(< (+ (ident a) (* (ident b) (ident c))) (ident d))" a+b*c<d;

\test spurious parens
expr"(+ (ident a) (ident b))" a+((b));
expr"(+ (ident a) (lit 0000002a))" ((a+(42)));

\test function call
expr"(() (ident foo) (+ (ident a) (ident b)) (neg (ident c)))" foo(a+b, -c);
expr"(() (ident foo) (() (ident bar)))" foo(bar());

\test sets
expr"(IN (ident a) ({} (.. (ident b) (ident c)) (.. (ident d) (ident e))))"
  a IN {b..c, d..e};
expr"(IN (ident a) ({} ))" a IN {};
expr"(IN (ident a) ({} (ident b) (ident c) (ident d)))" a IN {b,c,d};

\test string literals
expr"(() (ident foo) (str bar))" foo("bar");

\test typeguards in designator<
designator"(. baz (() (ident foo) (ident Bar)))" foo(Bar).baz;

\test IS
expr"(IS Bar (ident foo))" foo IS Bar;
expr"(IS Bar Baz (ident foo))" foo IS Bar.Baz;
testend
