needs lib/str lib/macro lib/diag mem/here comp/tok comp/c/glob
unit comp/c/pp

variable macros
: findMacro ( name -- macro-or-0 ) macros find ;

: inputline, begin in<# dup LF <> while c, repeat drop stepback 0 c, ;

: #define ( -- ) MAXMACROSZ reserve macros word entry inputline, ;

create _buf MAXMACROSZ allot
: _## ( -- )
  _buf here[ inputline, ]here ( )
  _buf z[] interpret[] ( ) ;
: _balance# ( scnt1 scnt2 -- ) <> ?err"PS imbalance in ##" ;
: ## scnt >r _## scnt r> _balance# ;
: ?curline+ eol? if 1 curline +! then ;
: toBOL begin eol? not while word drop repeat ?curline+ ;
: #if
  scnt >r _## scnt 1- r> _balance# ( f )
  not if begin toBOL word bi "#endif" s= | "#else" s= or until then
  ?curline+ ;
: #else begin toBOL word "#endif" s= until ?curline+ ;
: #endif ?curline+ ;
