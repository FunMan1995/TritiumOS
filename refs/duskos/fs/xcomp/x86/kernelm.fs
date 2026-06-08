\ Kernel macros needed on both i386 and amd64 kernels
\ Requirements: have asm/x86 and xcomp/tools loaded
\ Also, a "sysv) ( off -- )" word

: xnip, si 4 imm) add, ;
: ppop, ( reg -- ) si 0 ?d+bp) mov, xnip, ;
: xgrow, si 4 imm) sub, ;
: ppush, ( reg -- ) xgrow, si 0 ?d+bp) swap mov, ;
: xdrop, ax ppop, ;
: xdup, ax ppush, ;
: absjmp, abs>rel jmp, ;
: abscall, abs>rel call, ;
: wcall, xwordlbl abscall, ;
: wjmp, xwordlbl absjmp, ;
: xconst ( n -- ) xcode xdup, ax swap imm) mov, ret, ;

: HERE@, ( dstreg -- ) dup oCURALLOC sysv) mov, dup 0 ?d+bp) mov, ;
\ DX cannot be used as input in the Xwrite, words below. Also, destroys DX.
: nwrite, ( opmod n -- )
  dx oCURALLOC sysv) mov,
  dx 0 ?d+bp) over imm) add, ( opmod n )
  dx dx 0 ?d+bp) mov,
  dx swap neg ?d+bp) swap mov, ;
: cwrite, ( opmod -- ) byte) 1 nwrite, ;
: wwrite, ( opmod -- ) word) 2 nwrite, ;
: dwrite, ( opmod -- ) 4 nwrite, ;

0 value lblcallwr
