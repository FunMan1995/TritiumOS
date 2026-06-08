consts 7 REGW 6 REGA 5 REGS $e REGPSP $f REGRSP
: (src src@ ;
: (dst dst@ ;
: src) src! ;
: dst) dst! ;
: (W? src@ REGW = ;
: (sz case HAL8B and of 1 endof HAL16B and of 2 endof drop 4 endcase ;
: (i? ( op -- ?n f )
  dup HALIMM HALDIRECT or tuck and = if hslot@ 1 else drop 0 then ;
