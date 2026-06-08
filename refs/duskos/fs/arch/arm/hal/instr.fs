\ TODO: only fetch to r1 if op is indirect
\ this is tricky because it requires us to reimplement lbl?litwr from kernel
: &nf, ( n op -- ) 1 Rd! @, i) 1 Rd! 1 Rn! &, ;

0 value borrow?
: +c$, 0 to borrow? +, ;
: -c$, 1 to borrow? -, ;
: +c, $00b00000 ari, ;
: -c, $00d00000 ari, ;
: carry?, 0 i) 0 Rd! @, $23a00001 borrow? if $10000000 or then , 0 Rd! !, ;

: d*,
  ?imm>r0 $00800090
  over (signed? if $00400000 or then
  preari dup Rd@ 8 lshift or REGS Rn! swap op, drop ;
