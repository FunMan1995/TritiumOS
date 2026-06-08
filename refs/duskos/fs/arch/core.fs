needs lib/str fs/core
unit arch/core

consts $10 FAMILY_i386 $20 FAMILY_arm \
       $30 FAMILY_riscv $40 FAMILY_amd64 $50 FAMILY_m68k

: instrfamily? ( id -- f ) ARCH $f0 and = ;
: isx86? ARCH $f0 and bi FAMILY_i386 = | FAMILY_amd64 = or ;
: instrset ARCH $ff and ;
: machineoptions ARCH 8 rshift $ff and ;
: familyname ARCH $f0 and case
    FAMILY_i386 = of "i386" endof
    FAMILY_arm = of "arm" endof
    FAMILY_riscv = of "riscv" endof
    FAMILY_amd64 = of "amd64" endof
    FAMILY_m68k = of "m68k" endof
    abort"invalid ARCH" endcase ;

:~ "arch" lookup# enterdir familyname lookuprel# enterdir ;
: archlookup ~ lookuprel ;
: archlookup# ~ lookuprel# ;
: _load ['] interpret open exec< ;
: arch<< word archlookup# _load ;
: finsert<< word lookup# _load ;

: needsasm
  "asm/" isx86? if "x86" else familyname then strcat ( str )
  NEXTWORD ! needs ;
