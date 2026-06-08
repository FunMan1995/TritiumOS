1 const hasfloats?

variable mem
variable cw
create ten 10 ,

: >di, di swap i) mov, di ?bp+, ;
: di) di 0 d) ;

code n>f ( n -- float )
  finit,
  mem >di,
  di) ax mov,
  di) fild,
  di) fstp,
  ax di) mov,
  ret,

\ Can't use FISTTP! It requires SSE3. On a CPU like the AMD Geode, it fails.
\ This is why we take the long way around.
code f>n ( float -- n )
  finit,
  cw >di,
  di) fnstcw,
  di) $0c00 i) or, \ truncate mode
  di) fldcw,
  mem >di,
  di) ax mov,
  di) fld,
  di) fistp,
  ax di) mov,
  cw >di,
  di) $0c00 inv i) and, \ regular mode
  di) fldcw,
  ret,

( float float -- float )
: ari
  finit,
  si 0 ?d+bp) fld,
  swap, si 0 ?d+bp) fld,
  ST1 ST0 ' execute
  si 0 ?d+bp) fstp,
  drop, ret, ;

code f+ ari faddp,
code f- ari fsubp,
code f* ari fmulp,
code f/ ari fdivp,

code fscale10 ( float exp -- float )
  finit,
  ten abs) fild,
  si 0 ?d+bp) fld,
  ax 0 imm) cmp, ifnz,
  begin
    forward js,
      ST0 ST1 fmul,
      ax dec,
    else
      ST0 ST1 fdiv,
      ax inc,
    then
    ( loop ) abs>rel jnz, then
  si 0 ?d+bp) fstp,
  ST0 ffree, fincstp,
  drop, ret,

