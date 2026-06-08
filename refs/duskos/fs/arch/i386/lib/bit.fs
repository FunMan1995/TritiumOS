\ TODO: use i386's BSF
code findbit0 ( n -- idx ) \ idx=32 if n is -1
  bx ax mov, ax ax xor, begin
    bx 1 imm) shr, forward8 jc, exit, forward!
    ax inc, again

code bitscnt ( n -- cnt )
  bx ax mov, ax ax xor, begin
    ax 0 imm) adc,
    bx 1 imm) shr, ?brnz,
  ax 0 imm) adc, ret,
