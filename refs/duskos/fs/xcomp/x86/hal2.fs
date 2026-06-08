: ind -&) dup HALIMM and if HALIMM invand $05 or then ;
: 8b) ind HAL8B invand ;
: 16b) ind HAL16B or ;
: clrb HAL8B or HAL16B invand ;
: 32b) ind clrb ;
: &)
  dup HALIMM and not if
  dup $c7 and $05 - if 32b) HALDIRECT or else $c7 invand HALIMM or then then ;

: ?dispupg
  dup hslot@ $ffffff80 and if
    dup $c0 and $40 - not if $40 + then then ;

: +)
  dup if
    over (slot if
      over hslot@ + hslot!
      else bank) or $40 or then
    ?dispupg
    else drop then ;

: (sz dup HAL16B and if drop $2 else HAL8B and if $4 else $1 then then ;
: opc, HALINV and $a rshift xor dup $8 rshift c, ;
: ?66, dup HAL16B and if $66 c, then ;
