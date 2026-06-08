needs asm/arm
unit drv/arm/sccp

15 const cpSC

: _@, ( rn rm opcode -- )
  mrc) cpSC cp#) rW rd) swap cpi) swap rm) swap rn) ,) ;
: _!, ( rn rm opcode -- )
  mcr) cpSC cp#) rW rd) swap cpi) swap rm) swap rn) ,) ;
: sbz, dup, 0 i) @, ;

code cpuid@ ( -- n ) dup, 0 0 0 _@, exit,
code cachetype@ dup, 0 0 1 _@, exit,
code tlbtype@ dup, 0 0 3 _@, exit,
code armctl@ dup, 1 0 0 _@, exit,
code armctl! ( n -- ) 1 0 0 _!, drop, exit,
code ttbr0@ dup, 2 0 0 _@, exit,
code ttbr0! ( n -- ) 2 0 0 _!, drop, exit,
code ttbr1@ dup, 2 0 1 _@, exit,
code ttbr1! ( n -- ) 2 0 1 _!, drop, exit,
code ttbrctl@ dup, 2 0 2 _@, exit,
code ttbrctl! ( n -- ) 2 0 2 _!, drop, exit,
code domctl@ dup, 3 0 0 _@, exit,
code domctl! ( n -- ) 3 0 0 _!, drop, exit,

\ Register 7 cache mgmt
code invalidatedcache ( -- ) sbz, 7 14 0 _!, drop, exit,
code invalidateicache ( -- ) sbz, 7 5 0 _!, drop, exit,
code invalidateucache ( -- ) sbz, 7 15 0 _!, drop, exit,
