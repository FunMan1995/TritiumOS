code findbit0 ( n -- idx ) \ idx=32 if n is -1
  mov) rS rd) rW rm) ,) mov) rW rd) 0 imm) ,) begin
    mov) rS rd) rS rm) 1 lsr) f) ,)
    return) cc) ,)
    add) rW rdn) 1 imm) ,) again

code bitscnt ( n -- cnt )
  mov) rS rd) rW rm) ,) mov) rW rd) 0 imm) ,) begin
    mov) rS rd) rS rm) 1 lsr) f) ,)
    adc) rW rdn) 0 imm) ,) ?brnz,
  ret,
