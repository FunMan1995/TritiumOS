needs lib/struct
unit io/typeln

struct TypeBuffer {
  uint maxlen curlen ;
  [uchar,0] typebuf ;
}

: newtypebuf ( maxlen -- typebuf ) here# over , 0 , swap allot ;

: bs? bi BS = | $7f = or ;
: cr>lf dup CR = if drop LF then ;
\ only emit c if it's within the visible ascii range
: emitv ( c -- ) dup SPC - $5f < if emit else drop then ;

: tb$ ( tb -- ) 0 swap to curlen ;
: tb[] ( tb -- a u ) bi typebuf | curlen ;
: type1 ( c tb -- ?a ?u f )
  over ESC = if nip tb$ 0 1 exit then
  >r dup bs? if ( c V1=tb )
    drop V1 curlen if V1 doto curlen 1- | BS emit then spc> BS emit 0
  else \ non-BS
    cr>lf V1 curlen V1 maxlen 1- >= if drop LF then ( c )
    dup emitv dup V1 typebuf V1 curlen + c! V1 doto curlen 1+ | ( c )
    SPC < if V1 tb[] V1 tb$ 1 else 0 then then
  rdrop ;
