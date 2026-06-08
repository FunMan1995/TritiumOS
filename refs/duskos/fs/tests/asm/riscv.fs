needs tests/harness asm/riscv asm/label
testbegin
\ Compare DuskOS RISC-V assembler generated code with GNU Assembler generated code
\test
lb) x12 rd) x5 rs1) $8 imm)
  $00828603 #eq
lh) x12 rd) x5 rs1) $8 imm)
  $00829603 #eq
lw) x12 rd) x5 rs1) $8 imm)
  $0082a603 #eq
lbu) x12 rd) x5 rs1) $8 imm)
  $0082c603 #eq
lhu) x12 rd) x5 rs1) $8 imm)
  $0082d603 #eq

\test
sb) x2 src) x3 base) $20 imm)
  $02218023 #eq
sh) x2 src) x3 base) $20 imm)
  $02219023 #eq
sw) x2 src) x3 base) $20 imm)
  $0221a023 #eq

\test
sll) x4 rd) x5 rs1) x6 rs2)
  $00629233 #eq
srl) x4 rd) x5 rs1) x6 rs2)
  $0062d233 #eq
sra) x4 rd) x5 rs1) x6 rs2)
  $4062d233 #eq

\test
slli) x7 rd) x8 rs1) $1f imm)
  $01f41393 #eq
srli) x7 rd) x8 rs1) $1f imm)
  $01f45393 #eq
srai) x7 rd) x8 rs1) $1f imm)
  $41f45393 #eq

\test
add) x9 rd) x10 rs1) x11 rs2)
  $00b504b3 #eq
sub) x9 rd) x10 rs1) x11 rs2)
  $40b504b3 #eq
addi) x9 rd) x10 rs1) $ff imm)
  $0ff50493 #eq

\test
lui) x11 rd) $fffff imm)
  $fffff5b7 #eq
auipc) x11 rd) $fffff imm)
  $fffff597 #eq

\test
xor) x12 rd) x13 rs1) x14 rs2)
  $00e6c633 #eq
or) x12 rd) x13 rs1) x14 rs2)
  $00e6e633 #eq
and) x12 rd) x13 rs1) x14 rs2)
  $00e6f633 #eq

\test
xori) x15 rd) x16 rs1) $aa imm)
  $0aa84793 #eq
ori) x15 rd) x16 rs1) $aa imm)
  $0aa86793 #eq
andi) x15 rd) x16 rs1) $aa imm)
  $0aa87793 #eq

\test
slt) x17 rd) x18 rs1) x19 rs2)
  $013928b3 #eq
sltu) x17 rd) x18 rs1) x19 rs2)
  $013938b3 #eq
slti) x17 rd) x18 rs1) $bb imm)
  $0bb92893 #eq
sltiu) x17 rd) x18 rs1) $bb imm)
  $0bb93893 #eq

\test
mul) x25 rd) x24 rs1) x23 rs2)
  $037c0cb3 #eq
mulh) x25 rd) x24 rs1) x23 rs2)
  $037c1cb3 #eq
mulhsu) x25 rd) x24 rs1) x23 rs2)
  $037c2cb3 #eq
mulhu) x25 rd) x24 rs1) x23 rs2)
  $037c3cb3 #eq

\test
div) x21 rd) x22 rs1) x23 rs2)
  $037b4ab3 #eq
divu) x21 rd) x22 rs1) x23 rs2)
  $037b5ab3 #eq
rem) x21 rd) x22 rs1) x23 rs2)
  $037b6ab3 #eq
remu) x21 rd) x22 rs1) x23 rs2)
  $037b7ab3 #eq

\test
bne) x0 rs1) x1 rs2) -12 imm)
  $fe101ae3 #eq
beq) x19 rs1) x20 rs2) -16 imm)
  $ff4988e3 #eq
bge) x19 rs1) x20 rs2) -20 imm)
  $ff49d6e3 #eq
blt) x19 rs1) x20 rs2) -24 imm)
  $ff49c4e3 #eq
bgeu) x19 rs1) x20 rs2) -28 imm)
  $ff49f2e3 #eq
bltu) x19 rs1) x20 rs2) -32 imm)
  $ff49e0e3 #eq

\test
beq) x19 rs1) x20 rs2) rforward16 imm) ,)
beq) x19 rs1) x20 rs2) -4 imm) ,)
beq) x19 rs1) x20 rs2) -8 imm) ,)
beq) x19 rs1) x20 rs2) -12 imm) ,)
dup rforward! le@
  $01498863 #eq

\test
jal) xZERO rd) rforward16 imm) ,)
beq) x19 rs1) x20 rs2) -4 imm) ,)
beq) x19 rs1) x20 rs2) -8 imm) ,)
beq) x19 rs1) x20 rs2) -12 imm) ,)
dup rforward! le@
  $0100006f #eq

\test
jal) x20 rd) -4 imm)
  $ffdffa6f #eq
jalr) x20 rd) x21 base) 16 imm)
  $010a8a67 #eq

\test Pseudo instructions test
\ Compare pseudo instruction generated code with DuskOS riscv assembler generated code
\ => Below tests are only valid if every tests above are passed

li12) x1 rd) 10 imm)
  addi) x1 rd) xZERO rs1) 10 imm) #eq

\test
li) x1 rd) 8000 imm) _$rst 1 to _nochecking?
  lui) x1 rd) 8000 _hi imm) #eq
  addi) x1 rd) x1 rs1) 8000 _lo imm) #eq 0 to _nochecking?

\test
la) x1 rd) 8000 imm) 0 0 ,) _$rst 1 to _nochecking?
  auipc) x1 rd) 8000 _hi imm) #eq
  addi) x1 rd) x1 rs1) 8000 _lo imm) #eq 0 to _nochecking?

\test
mv) x1 rd) x2 rs1)
  addi) x1 rd) x2 rs1) 0 imm) #eq

\test
not) x1 rd) x2 rs1)
  xori) x1 rd) x2 rs1) -1 imm) #eq

\test
neg) x1 rd) x2 rs1) 0 ,) _$rst
  sub) x1 rd) xZERO rs1) x2 rs2) #eq

\test
bgt) x1 rs1) x2 rs2) 4 imm) _$rst
  blt) x2 rs1) x1 rs2) 4 imm) #eq
ble) x1 rs1) x2 rs2) 4 imm) _$rst
  bge) x2 rs1) x1 rs2) 4 imm) #eq
bgtu) x1 rs1) x2 rs2) 4 imm) _$rst
  bltu) x2 rs1) x1 rs2) 4 imm) #eq
bleu) x1 rs1) x2 rs2) 4 imm) _$rst
  bgeu) x2 rs1) x1 rs2) 4 imm) #eq

\test
beqz) x1 rs1) 4 imm)
  beq) x1 rs1) xZERO rs2) 4 imm) #eq
bnez) x1 rs1) 4 imm)
  bne) x1 rs1) xZERO rs2) 4 imm) #eq
bgez) x1 rs1) 4 imm)
  bge) x1 rs1) xZERO rs2) 4 imm) #eq

\test
blez) x1 rs1) 4 imm) 0 ,) _$rst
  bge) x1 rs2) xZERO rs1) 4 imm) #eq
bgtz) x1 rs1) 4 imm) 0 ,) _$rst
  blt) x1 rs2) xZERO rs1) 4 imm) #eq

\test
j) 4 imm)
  jal) xZERO rd) 4 imm) #eq

\test
call12) 4 imm)
  jal) xRA rd) 4 imm) #eq
call) 8000 imm) _$rst 1 to _nochecking?
  auipc) xRA rd) 8000 _hi imm) #eq
  jalr) xRA rd) xRA rs1) 8000 _lo imm) #eq 0 to _nochecking?

\test
tail12) 4 imm)
  jal) x0 rd) 4 imm) #eq
tail) 8000 imm) _$rst 1 to _nochecking?
  auipc) x30 rd) 8000 _hi imm) #eq
  jalr) x0 rd) x30 rs1) 8000 _lo imm) #eq 0 to _nochecking?

\test
ret)
  jalr) xZERO rd) xRA base) 0 imm) #eq
nop)
  addi) xZERO rd) xZERO rs1) 0 imm) #eq

\test
jal) xZERO rd) rforward16 imm) ,) to L1
beq) x2 rs1) x3 rs2) -4 imm) ,)
beq) x4 rs1) x5 rs2) -8 imm) ,)
beq) x6 rs1) x7 rs2) -12 imm) ,)
L1 rforward!

\test
j) rforward16 imm) ,) to L2
beq) x2 rs1) x3 rs2) -4 imm) ,)
beq) x4 rs1) x5 rs2) -8 imm) ,)
beq) x6 rs1) x7 rs2) -12 imm) ,)
L2 rforward!
L1 le@ L2 le@ #eq

\test
jal) xRA rd) rforward16 imm) ,) to L1
beq) x2 rs1) x3 rs2) -4 imm) ,)
beq) x4 rs1) x5 rs2) -8 imm) ,)
beq) x6 rs1) x7 rs2) -12 imm) ,)
L1 rforward!

\test
call12) rforward16 imm) ,) to L2
beq) x2 rs1) x3 rs2) -4 imm) ,)
beq) x4 rs1) x5 rs2) -8 imm) ,)
beq) x6 rs1) x7 rs2) -12 imm) ,)
L2 rforward!
L1 le@ L2 le@ #eq

\test Macros testing
here x10 push,
dup le@ addi) xRSP rd) xRSP rs1) -4 imm) #eq
4 + le@ sw) x10 src) xRSP base) 0 imm)  #eq

\test
here x10 ppush,
dup le@ addi) xPSP rd) xPSP rs1) -4 imm) #eq
4 + le@ sw) x10 src) xPSP base) 0 imm)  #eq

\test
here x10 pop,
dup le@ lw) x10 rd) xRSP base) 0 imm) #eq
4 + le@ addi) xRSP rd) xRSP rs1) 4 imm) #eq

\test
here x10 ppop,
dup le@ lw) x10 rd) xPSP base) 0 imm) #eq
4 + le@ addi) xPSP rd) xPSP rs1) 4 imm) #eq

\test
here x0 $10 li,
li12) x0 rd) $10 imm)
  swap le@ #eq

\test
x0 $1000 li,
li) x0 rd) $1000 imm) _$rst
here dup rot> 8 - le@ #eq
4 - le@ #eq

testend
