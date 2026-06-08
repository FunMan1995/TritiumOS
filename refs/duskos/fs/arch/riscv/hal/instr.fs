\ TODO: only fetch to x11 if op is indirect
: &nf, ( n op -- ) x11 rd! @, i) x11 rd! &, ;
: carry?, x12 rd! !, ;

: +-x12, ( instr op -- op ) tuck x12 rs2! dup rd@ rs1! op, ;
: carry>x12, ( op -- ) x12 rd! $00003633 swap op, ;

:~ swap $10000000 invand swap ;
: +c$, $10000033 preari, ~ tuck op, carry>x12, ?x11>src, ;
: -c$, $40000033 preari, tuck op, carry>x12, ?x11>src, ;
: +c, $10000033 preari, ~ 2dup op, +-x12, carry>x12, ?x11>src, ;
: -c, $40000033 preari, 2dup op, +-x12, carry>x12, ?x11>src, ;

: ?S>x11, ( op -- op )
  dup rs1@ xS = if dup x11 rd! @, x11 rs1! then ;

: d*, ( op -- )
  ?S>x11, $02003033 over (signed? if $2000 invand then preari,
  tuck xS rd! op, rs1<>rs2 *, ?x11>src, ;
