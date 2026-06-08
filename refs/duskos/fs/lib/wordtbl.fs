needs lib/psrs mem/range
unit lib/wordtbl

: wordrefs ( n -- ) create dup , 0 do ' , loop ;
: _oob abort"wexec out of bounds" ;
:~ PSP) S>) @+, ['] _oob S) >=) ?br, 1 i) +, 2 i) <<, W) &) S>) +, ;
code wtbl@ ( tbl idx -- xt ) ~ S) @, exit,
code wexec ~ drop, S) br,
: wexec, ( tbl idx -- ) wtbl@ execute, ;
alias @ wordtbllen

0 value _pslvl \ PS level recorded at last wordtbl[ call
: wordtbl[ ( -- ) scnt to _pslvl ;
: ]wordtbl ( ... "name" -- )
  create scnt _pslvl - ( ... n )
  dup , ps[] 2dup swap[] tuck 4* cmoveallot ( ... n )
  ndrop ;
