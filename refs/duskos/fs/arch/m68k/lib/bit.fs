code findbit0 ( n -- idx )
  D0 W move, W clr, begin
    D0 1 lsr#, forward8 bcs, exit, then
    forward8 bne, W 1 imm) add, exit, then
    W 1 imm) add, again

code bitscnt ( n -- cnt )
  D0 W move, W clr, begin
    D0 1 lsr#, forward8 bcc, W 1 imm) add, then
    abs>rel bne,
  exit,
