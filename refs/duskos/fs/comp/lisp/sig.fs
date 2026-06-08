needs lib/tagl
unit comp/lisp/sig

\ signature format
\ b31:7 unused
\ b6    is a parser word?
\ b5    is a compiler word?
\ b4    has no return value?
\ b3:0  argument count

: _ does> or ; ( sig -- sig )
map< _ $10 noret $20 compiler $40 parser
: ?sig ( xt -- ?sig f ) n"LISP" findtag ;
: ?argcnt ( xt -- ?cnt f ) ?sig if $f and 1 else 0 then ;
: _ does> swap ?sig if and bool else drop 0 then ; ( xt -- f )
map< _ $10 ?noret $20 ?compiler $40 ?parser
: addsig ( sig xt -- ) n"LISP" rot addtag ;
2 noret current addsig
