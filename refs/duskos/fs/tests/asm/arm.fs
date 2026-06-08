needs tests/harness asm/arm
testbegin
mov) r0 rd) 42 imm)
  $e3a0002a #eq \ mov r0, #42
ldr) r1 rd) r2 rn) r3 +r) pre) 1 lsr)
  $e79210a3 #eq \ ldr r1, [r2, r3 lsr #1]
swp) r1 rd) r2 rn) r3 rm)
  $e1021093 #eq \ swp r1, r3, [r2]
mov) r3 rd) $3f0000 imm)
  $e3a039fc #eq \ mov r3, #3f0000
mul) r1 rd) r2 rm) r3 rs)
  $e0010392 over #eq \ mul r1, r2, r3
r4 acc) $e0214392 #eq \ mla r1, r2, r3, r4
testend
