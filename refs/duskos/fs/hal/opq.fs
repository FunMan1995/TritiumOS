needs xcomp/boot arch/core hal/vreg lib/str
unit hal/opq
arch<< hal/opq.fs

: (bank (slot hbank@ ;
: bank) hbank! slot) ;

create src map< , <) >) <=) >=)
create dst map< , >) <) >=) <=)
: swappedcond dup src 4 idx if nip 4* dst + @ then ;

: ?err ?abort"invalid HAL op" ;
create regs map< , REGW REGA REGS REGPSP REGRSP REGR0 REGR1
create names map< s, "W" "A" "S" "PSP" "RSP" "R0" "R1"
: .reg regs 7 idx not ?err names slistiter stype ;
: .hal ( op -- )
  dup .x spc>
  dup (dst .reg
  dup (dir? if .">" else ."<" then
  dup &) (i? if swap dup &) = if ."imm " else ."mem " then .x else
    dup (&? if ."&" then
    dup (src .reg
    dup (sz case 1 = of ."c" endof 2 = of ."w" endof drop endcase
    (slot hbank@ ?dup if ."+" .x then
  then
  nl> ;
