consts 4 REGW 5 REGA 6 REGS 3 REGPSP 2 REGRSP
: (src rs1@ ;
: (dst rd@ ;
: src) rs1! ;
: dst) rd! ;
: (W? HALRS1MASK and $20000 = ;
: (sz case HAL8B and of 1 endof HAL16B and not of 2 endof drop 4 endcase ;
: _i? ( op -- f ) HALIMM and bool ;
: hslot ( op -- slot ) 28 rshift $f and ;
: (i? ( op -- ?n f ) dup _i? if hslot hbank@ 1 else drop 0 then ;
