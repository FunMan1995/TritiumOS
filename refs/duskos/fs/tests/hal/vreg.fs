needs tests/harness hal/vreg hal/opq
testbegin

\test R0/R1 consts
R0) (src REGR0 #eq
W) R1>) (dst REGR1 #eq

\test some HAL code using vregs
variable data
code test1 ( -- n ) \ TODO: not testing R1>) yet because POSIX is broken
  1 i) R0>) @,
  R1) &) R0>) !, 1 R1) &) +n,
  R2) R0>) !, 2 R2) +n,
  R3) R0>) !, 3 R2) +n,
  0 i) A>) @, 0 i) S>) @,
  dup, R0) &) @, R1) &) +, R2) +, R3) +, exit,
test1 10 #eq
testend
