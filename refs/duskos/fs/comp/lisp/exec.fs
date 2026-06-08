needs lib/wordtbl mem/cons comp/lisp/sig comp/lisp/ast comp/lisp/compile
unit comp/lisp/exec

: err abort"lisp exec error" ;
alias abort exec
: chkcnt ( n n -- ) <> ?abort"wrong argument count" ;
: chkexec ( w cnt )
  over ?argcnt if chkcnt else drop then
  r! execute r> ?noret if 0 then ;
: funcall ( data -- res )
  carcdr dup length >r swap >r ( argnodes ) \ V1=argcnt \ V2=callnode
  begin ?dup while carcdr dip exec | repeat
  r> cdrcar case
    CALLABLE = of V1 chkexec endof
    LAMBDA = of dup cdr car V1 chkcnt lambda, execute endof
    FUNCALL = of funcall V1 chkexec endof
    err endcase
  rdrop ;
wordtbl[ ( data -- res )
  ' noop ( NUMBER )
  ' execute ( CALLABLE )
  ' err ( PSOFF )
  ' funcall
  ' lambda, ( LAMBDA )
  ' err ( LET )
  :> ( TO ) carcdr exec tuck swap execute ;
]wordtbl exectbl
:realias exec ( ast -- res ) carcdr exectbl rot wexec ;
