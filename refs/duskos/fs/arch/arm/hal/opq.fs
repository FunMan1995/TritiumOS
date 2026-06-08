: (src Rn@ ;
: (dst Rd@ ;
: src) Rn! ;
: dst) Rd! ;
: (W? (src REGW = ;
: (i? ( op -- ?n f ) dup HALIMM and if hslot@ 1 else drop 0 then ;
