code findbit0 ( n -- idx )
  mv) xS rd) xW rs1) ,) mv) xW rd) xZERO rs1) ,) begin
    andi) x10 rd) xS rs1) 1 imm) ,)
    srli) xS rdrs1) 1 imm) ,)
    addi) xW rdrs1) 1 imm) ,)
    bne) x10 rs1) xZERO rs2) swap abs>rel imm) ,)
  subi) xW rdrs1) 1 imm) ,)
  ret,

code bitscnt ( n -- cnt )
  mv) xS rd) xW rs1) ,) mv) xW rd) xZERO rs1) ,) begin
    andi) x10 rd) xS rs1) 1 imm) ,)
    srli) xS rdrs1) 1 imm) ,)
    add) xW rdrs1) x10 rs2) ,)
    bne) xS rs1) xZERO rs2) swap abs>rel imm) ,)
  ret,
