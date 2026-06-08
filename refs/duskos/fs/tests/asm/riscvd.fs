1 here# ! here c@ not [if] \ TODO
  ."asm/riscvd doesn't work on big endian yet, skipping\n" \s [then]

needs tests/harness lib/str asm/riscvd
testbegin

0 value chk_addr

\ we don't compare the whole disasm line, only the "description" part.

: chk"
  [rcompile] " ( str )
  chk_addr word"dis1" exec>str ( actual disline )
  chk_addr 4+ to chk_addr
  dup stype
  10 + over c@ []>str #s= ;

: chkinit ( a )
  current to dpc_offset
  0 to chk_addr ;

\test li
code _ li) x11 rd) $12345678 imm) ,)
chkinit
chk"lui    x11, 305418240 ($12345000)"
chk"addi   x11, x11, 1656 -> $12345678"

\test auipc
code _ auipc) x11 rd) $1234 imm) ,)
chkinit
chk"auipc  x11, 19087360 ($01234000)"

\test jal
code testjal jal) x11 rd) ' testjal abs>rel imm) ,)
chkinit
chk"jal    x11, 0 -> testjal"

\test call
code testcall call) ' testcall abs>rel imm) ,)
chkinit
chk"auipc  xRA, 0 ($00000000)"
chk"jalr   xRA, xRA[0] -> testcall"

\test tail
code _ tail) $12345678 imm) ,)
chkinit
chk"auipc  x30, 305418240 ($12345000)"
chk"jalr   xZERO, x30[1656] -> $12345678"

\test ret
code _ ret) ,)
chkinit
chk"ret"

\test beq
code testbeq beq) x10 rs1) x20 rs2) ' testbeq abs>rel imm) ,)
chkinit
chk"beq    x10, x20, 0 -> testbeq"

\test bne
code testbne bne) x10 rs1) x20 rs2) ' testbne abs>rel imm) ,)
chkinit
chk"bne    x10, x20, 0 -> testbne"

\test blt
code testblt blt) x10 rs1) x20 rs2) ' testblt abs>rel imm) ,)
chkinit
chk"blt    x10, x20, 0 -> testblt"

\test bge
code testbge bge) x10 rs1) x20 rs2) ' testbge abs>rel imm) ,)
chkinit
chk"bge    x10, x20, 0 -> testbge"

\test bltu
code testbltu bltu) x10 rs1) x20 rs2) ' testbltu abs>rel imm) ,)
chkinit
chk"bltu   x10, x20, 0 -> testbltu"

\test bgeu
code testbgeu bgeu) x10 rs1) x20 rs2) ' testbgeu abs>rel imm) ,)
chkinit
chk"bgeu   x10, x20, 0 -> testbgeu"

\test lb
code _ lb) x12 rd) xA rs1) $8 imm) ,)
chkinit
chk"lb     x12, xA[8]"

\test lh
code _ lh) x12 rd) xA rs1) $8 imm) ,)
chkinit
chk"lh     x12, xA[8]"

\test lw
code _ lw) x12 rd) xA rs1) $8 imm) ,)
chkinit
chk"lw     x12, xA[8]"

\test lbu
code _ lbu) x12 rd) xA rs1) $8 imm) ,)
chkinit
chk"lbu    x12, xA[8]"

\test lhu
code _ lhu) x12 rd) xA rs1) $8 imm) ,)
chkinit
chk"lhu    x12, xA[8]"

\test sb
code _ sb) xPSP src) xRSP base) $20 imm) ,)
chkinit
chk"sb     xPSP, xRSP[32]"

\test sh
code _ sh) xPSP src) xRSP base) $20 imm) ,)
chkinit
chk"sh     xPSP, xRSP[32]"

\test sw
code _ sw) xPSP src) xRSP base) $20 imm) ,)
chkinit
chk"sw     xPSP, xRSP[32]"

\test addi
code _ addi) x9 rd) x10 rs1) $ff imm) ,)
chkinit
chk"addi   x9, x10, 255"

\test nop
code _ nop) ,)
chkinit
chk"nop"

\test mv
code _ mv) x10 rd) x20 rs1) ,)
chkinit
chk"mv     x10, x20"

\test la
code testla
  nop) ,)
  la) x10 rd) ' testla abs>rel imm) ,)
chkinit
chk"nop"
chk"auipc  x10, 0 ($00000000)"
chk"addi   x10, x10, -4 -> testla"

\test slti
code _ slti) x17 rd) x18 rs1) $bb imm) ,)
chkinit
chk"slti   x17, x18, 187"

\test sltiu
code _ sltiu) x17 rd) x18 rs1) $bb imm) ,)
chkinit
chk"sltiu  x17, x18, 187"

\test xori
code _ xori) x1 rd) x2 rs1) -1 imm) ,)
chkinit
chk"xori   xRA, xRSP, -1"

\test ori
code _ ori) x1 rd) x2 rs1) 1 imm) ,)
chkinit
chk"ori    xRA, xRSP, 1"

\test andi
code _ andi) x1 rd) x2 rs1) 1 imm) ,)
chkinit
chk"andi   xRA, xRSP, 1"

\test slli
code _ slli) x11 rd) x12 rs1) $1f imm) ,)
chkinit
chk"slli   x11, x12, 31"

\test srai
code _ srai) x11 rd) x12 rs1) $1f imm) ,)
chkinit
chk"srai   x11, x12, 31"

\test add
code _ add) x9 rd) x10 rs1) x11 rs2) ,)
chkinit
chk"add    x9, x10, x11"

\test sub
code _ sub) x9 rd) x10 rs1) x11 rs2) ,)
chkinit
chk"sub    x9, x10, x11"

\test sll
code _ sll) x9 rd) x10 rs1) x11 rs2) ,)
chkinit
chk"sll    x9, x10, x11"

\test slt
code _ slt) x9 rd) x10 rs1) x11 rs2) ,)
chkinit
chk"slt    x9, x10, x11"

\test sltu
code _ sltu) x9 rd) x10 rs1) x11 rs2) ,)
chkinit
chk"sltu   x9, x10, x11"

\test xor
code _ xor) x9 rd) x10 rs1) x11 rs2) ,)
chkinit
chk"xor    x9, x10, x11"

\test srl
code _ srl) x9 rd) x10 rs1) x11 rs2) ,)
chkinit
chk"srl    x9, x10, x11"

\test sra
code _ sra) x9 rd) x10 rs1) x11 rs2) ,)
chkinit
chk"sra    x9, x10, x11"

\test or
code _ or) x9 rd) x10 rs1) x11 rs2) ,)
chkinit
chk"or     x9, x10, x11"

\test and
code _ and) x9 rd) x10 rs1) x11 rs2) ,)
chkinit
chk"and    x9, x10, x11"

\test mul
code _ mul) x9 rd) x10 rs1) x11 rs2) ,)
chkinit
chk"mul    x9, x10, x11"

\test mulh
code _ mulh) x9 rd) x10 rs1) x11 rs2) ,)
chkinit
chk"mulh   x9, x10, x11"

\test mulhsu
code _ mulhsu) x9 rd) x10 rs1) x11 rs2) ,)
chkinit
chk"mulhsu x9, x10, x11"

\test mulhu
code _ mulhu) x9 rd) x10 rs1) x11 rs2) ,)
chkinit
chk"mulhu  x9, x10, x11"

\test div
code _ div) x9 rd) x10 rs1) x11 rs2) ,)
chkinit
chk"div    x9, x10, x11"

\test divu
code _ divu) x9 rd) x10 rs1) x11 rs2) ,)
chkinit
chk"divu   x9, x10, x11"

\test rem
code _ rem) x9 rd) x10 rs1) x11 rs2) ,)
chkinit
chk"rem    x9, x10, x11"

\test remu
code _ remu) x9 rd) x10 rs1) x11 rs2) ,)
chkinit
chk"remu   x9, x10, x11"

