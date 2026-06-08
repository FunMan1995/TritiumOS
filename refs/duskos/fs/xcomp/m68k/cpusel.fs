needs asm/m68k
unit xcomp/m68k/cpusel

: clearcache40, $f4f8 wbe, ; \ CPUSH
: clearcache30, D0 CACR movec, D0 $808 ori, CACR D0 movec, ;
alias clearcache40, clearcache,

: selectM68040 ['] clearcache40, ['] clearcache, realias ;
: selectM68030 ['] clearcache30, ['] clearcache, realias ;