needs hal/vreg
unit mem/kv

: kvtbl,
  alignhere dup dup 4* r! 2* 4+ allot@ ( ... n n a ) \ V1=off
  !+ swap 0 do ( ... k v a ) tuck V1 + ! !+ loop rdrop drop ;

: kv', ( -- notfoundjmp )
  R2) A>) !, A) S>) @+, \ W=key A=tbl+4 S=cnt R2=tbl
  idx, ifz, R2) @, W) S>) +, 2 i) S>) <<, S) &) +, 4 i) +, ;

code kv@ ( tbl key -- ?val f )
  PSP) A>) @+, kv', W) @, dup, 1 i) @, exit, then 0 i) @, exit,

:~ abort"key not found" ;
code kv@# ( tbl key -- val )
  PSP) A>) @+, kv', W) @, exit, then ' ~ bbr,

code kv! ( tbl key val -- f )
  rot>, PSP) A>) @+, kv', ( val val' )
    PSP) A>) @+, W) A>) !, 1 i) @, exit, then
  nip, 0 i) @, exit,
: kv!# kv! not if ~ then ;

code kvreplace ( tbl oldkey newkey -- f )
  rot>, PSP) A>) @, A) S>) @+, ( newk tbl k A=tbl+4 S=cnt )
  idx, ifz,
    drop, 2 i) S>) <<, S) &) +, PSP) A>) @+, W) 4 +) A>) !, 1 i) @,
    else 2drop, 0 i) @, then exit,
: kvreplace# kvreplace not if ~ then ;

0 value pslvl
: kvtbl[ ( -- ) scnt to pslvl ;
: ]kvtbl ( ... "name" -- )
  create scnt pslvl - ( ... n ) 2/ kvtbl, ;
: ?kvexec ( tbl key -- f ) kv@ if execute 1 else 0 then ;
