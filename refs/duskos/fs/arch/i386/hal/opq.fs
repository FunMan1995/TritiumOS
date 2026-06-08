consts 0 REGW 3 REGA 2 REGS 6 REGPSP 4 REGRSP
: (src $10000007 and ;
: (dst $38 and >>3 ;
: src) swap 7 invand or ;
: dst) <<3 swap $38 invand or ;
: (W? $40007 and not ;
: (sz case HAL8B and not of 1 endof HAL16B and of 2 endof drop 4 endcase ;
: (i? ( op -- ?n f ) dup HALIMM and if (slot hbank@ 1 else drop 0 then ;
