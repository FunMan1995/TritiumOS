needs xcomp/tools
$80000000 to binstart

\ Macros
: xnip, addi) xPSP rdrs1) 4 imm) ,) ;
: xdrop, xW ppop, ;
: xgrow, addi) xPSP rdrs1) 4 neg imm) ,) ;
: xdup, xW ppush, ;
: pushlr, xRA push, ;
: poplr, xRA pop, ;
: popexit, poplr, ret, ; \ impossible to do it like on ARM port
: abscall, pushlr, absbl, poplr, ;
: wcall, xwordlbl abscall, ;
: wjmp, xwordlbl absb, ; \ only for leaf words!

: pc'>reg, ( pc r -- )
  dip pc swap- | ( off r )
  dup pc>reg,
  subi) swap rdrs1) swap imm) ,) ;

: pc@>reg, ( pc r -- )
  dip pc swap- | ( off r )
  dup pc>reg,
  lw) over rd) swap base) swap neg imm) ,) ;

: xconst ( n -- ) pc swap le, xcode xdup, xW pc@>reg, ret, ;

: values ( n -- ) 0 do 0 value loop ;
8 values lblcwrite lbldwrite lblwriterange
         lblbaseboot lblcoldboot lblccache
         lblhibits lblUimm
$800 const STACKSZ

: HERE@, ( dstreg ptrreg -- )
  lw) over rd) xSYSVARS base) oCURALLOC imm) ,)
  lw) rot rd) swap base) ,) ;

\ SYSVARS placement done in the ARM-style
kernelbegin
j) rforward imm) ,) to lblcoldboot \ binstart + 4. SYSVARS => can fit in one instr
\ but might be better to put a far jump here
\ if we put a far jump, every sysvars will be misplaced (by 4)
0 le, \ COMPILING
binstart oSYSALLOC + le, \ CURALLOC
0 le, \ SYSDICT
0 le, \ unused
0 le, \ RCNT
0 le, \ FINDCOMPILER
$80080000 le, \ SYSALLOC HERE
0 le, \ SYSALLOC HEREMAX
0 le, \ SYSALLOC NEWHERE
0 le, \ INPTR
$14 allot0 \ unused
0 le, \ SYSVARS+40=/mod pointer => unused on RISC-V because using the Mul extension we have / and mod
0 le, \ unused
$83000000 STACKSZ - le, \ PSORIGIN
STACKSZ le, \ PSSZ
$83000000 le, \ RSORIGIN
binstart SYSVARSSZ + tgtallot

$30 xconst ARCH

\ make all them take 2 instructions slot because they will be realiased later with far jump
xcode (wnf) j) 0 imm) ,) j) 0 imm) ,)
xcode (div0) j) 0 imm) ,) j) 0 imm) ,)
xcode bye j) 0 imm) ,) j) 0 imm) ,)
xcode dbg x26 43 li, j) 0 imm) ,) j) 0 imm) ,)

pc to L3 \ fail
  xW 0 li,
  ret,

xcode parsehex ( a u -- n? f ) \ *without* the $
  mv) x10 rd) xW rs1) ,)
  xdrop,
pc to L2 \ x10=u xW=a
  beq) x10 rs1) xZERO rs2) L3 abs>rel imm) ,)
  x12 0 li, \ x12=accumulated result
pc \ loop
  lbu) x11 rd) xW rs1) 0 imm) ,)
  addi) xW rdrs1) 1 imm) ,)
  subi) x10 rdrs1) 1 imm) ,)
  ori) x11 rdrs1) $20 imm) ,)
  x15 '0' li,
  bltu) x11 rs1) x15 rs2) L3 abs>rel imm) ,)
  sub) x11 rdrs1) x15 rs2) ,)
  x15 10 li,
  bltu) x11 rs1) x15 rs2) rforward8 imm) ,) \ parse ok, under 10
    subi) x11 rdrs1) 'a' '0' - imm) ,)
    x15 6 li,
    bge) x11 rs1) x15 rs2) L3 abs>rel imm) ,)
    addi) x11 rdrs1) 10 imm) ,)
  rforward! \ parse ok, digit in x11
  slli) x12 rdrs1) 4 imm) ,)
  add) x12 rdrs1) x11 rs2) ,)
  bne) x10 rs1) xZERO rs2) swap ( loop ) abs>rel imm) ,)
  x12 ppush,
  xW 1 li,
  ret,

xcode parse ( str -- n? f )
  lbu) x11 rd) xW rs1) 1 imm) ,)
  x15 '$' li,
  bne) x11 rs1) x15 rs2) L3 abs>rel imm) ,)
  lbu) x10 rd) xW rs1) 0 imm) ,)
  subi) x10 rdrs1) 1 imm) ,)
  addi) xW rdrs1) 2 imm) ,)
  L2 absb,

xcode word ( -- str )
  lw) x11 rd) xSYSVARS rs1) oINPTR imm) ,) \ x11=addr
  pc
    lbu) x10 rd) x11 rs1) 0 imm) ,)
    addi) x11 rdrs1) 1 imm) ,)
    x19 SPC li,
    ( pc ) ble) x10 rs1) x19 rs2) swap abs>rel imm) ,) \ x10=first non-ws
  xdup, subi) xW rd) x11 rs1) 2 imm) ,) \ xW=res
  pc
    lbu) x10 rd) x11 rs1) 0 imm) ,)
    addi) x11 rdrs1) 1 imm) ,)
    x19 SPC li,
    bgt) x10 rs1) x19 rs2) swap abs>rel imm) ,)
  sw) x11 src) xSYSVARS base) oINPTR imm) ,)
  sub) x11 rdrs1) xW rs2) ,)
  subi) x11 rdrs1) 2 imm) ,)
  sb) x11 src) xW base) 0 imm) ,)
  ret,

xcode execute ( a -- )
  mv) x10 rd) xW rs1) ,)
  xdrop,
  jalr) x0 rd) x10 rs1) 0 imm) ,)

xcode findentry ( name 'dict -- e-or-0 )
  x12 ppop, \ x12=name xW=dict
  sw) x12 src) xSYSVARS base) oFINDSTR imm) ,)
  x17 $3fffffff li, \ x17=lenmask
  lbu) x11 rd) x12 rs1) 0 imm) ,) \ x11=len
  slli) x16 rd) x11 rs1) 24 imm) ,) \ x16=tofind
  add) x13 rd) x11 rs1) x12 rs2) ,) \ x13=lastchar
  lbu) x10 rd) x13 rs1) 0 imm) ,) \ x10=lastchar
  slli) x19 rd) x10 rs1) 16 imm) ,)
  or) x16 rdrs1) x19 rs2) ,)
  x19 2 li,
  bltu) x11 rs1) x19 rs2) rforward8 imm) ,)
    lbu) x10 rd) x13 rs1) -1 imm) ,)
    slli) x18 rd) x10 rs1) 8 imm) ,)
    or) x16 rdrs1) x18 rs2) ,)
  rforward!
  bleu) x11 rs1) x19 rs2) rforward8 imm) ,)
    lbu) x10 rd) x13 rs1) -2 imm) ,)
    or) x16 rdrs1) x10 rs2) ,)
  rforward!
pc \ loop1
  lw) x13 rd) xW rs1) -4 imm) ,)
  and) x13 rdrs1) x17 rs2) ,)
  beq) x16 rs1) x13 rs2) rforward8 imm) ,)
    pc to L1 \ not matching, try next
    lw) xW rd) xW base) 0 imm) ,)
    swap bne) xW rs1) xZERO rs2) swap abs>rel imm) ,)
    \ not found
    ret,
  rforward!
  \ same length
  subi) x14 rd) xW rs1) 2 imm) ,)
  sub) x14 rdrs1) x11 rs2) ,) \ x14=beginning of name range
  mv) x13 rd) x11 rs1) ,) \ x13=running len
pc \ loop 2
  add) x18 rd) x14 rs1) x13 rs2) ,)
  lbu) x15 rd) x18 base) 0 imm) ,)
  add) x18 rd) x12 rs1) x13 rs2) ,)
  lbu) x10 rd) x18 base) 0 imm) ,)
  bne) x15 rs1) x10 rs2) L1 abs>rel imm) ,)
  subi) x13 rdrs1) 1 imm) ,)
  bne) x13 rs1) xZERO rs2) swap abs>rel imm) ,)
  \ same contents
  ret,

xcode HERE xdup, lw) xW rd) xSYSVARS base) oCURALLOC imm) ,) ret,

pc to lblcwrite \ x10=char. out: x11=here x12=hereptr
  x11 x12 HERE@,
  sb) x10 src) x11 base) 0 imm) ,)
  addi) x11 rdrs1) 1 imm) ,)
  sw) x11 src) x12 base) ,)
  ret,

xcode , ( n -- )
  mv) x10 rd) xW rs1) ,) xdrop,
pc to lbldwrite \ x10=n. out: x11=here x12=hereptr
  x11 x12 HERE@,
  sw) x10 src) x11 base) 0 imm) ,)
  addi) x11 rdrs1) 4 imm) ,)
  sw) x11 src) x12 base) ,)
  ret,

pc to lblwriterange \ x10=addr x11=len
  bne) x11 rs1) xZERO rs2) rforward8 imm) ,)
    ret,
  rforward!
  x12 x13 HERE@,
  pc
    lbu) x14 rd) x10 rs1) 0 imm) ,)
    addi) x10 rdrs1) 1 imm) ,)
    sb) x14 src) x12 base) 0 imm) ,)
    addi) x12 rdrs1) 1 imm) ,)
    addi) x11 rdrs1) -1 imm) ,)
    bne) x11 rs1) xZERO rs2) swap abs>rel imm) ,)
  sw) x12 src) x13 base) ,)
ret,

xcode alignhere ( -- ) \ output: x10=here x12=hereptr
  x10 x12 HERE@,
  andi) x11 rd) x10 rs1) 3 imm) ,)
  beq) x11 rs1) xZERO rs2) rforward8 imm) ,)
    addi) x10 rdrs1) 4 imm) ,)
    sub) x10 rdrs1) x11 rs2) ,)
    sw) x10 src) x12 base) ,)
  rforward!
  ret,

xcode entry[] pushlr, ( 'dict a u -- )
  xwordlbl alignhere absbl, \ x10=here x12=hereptr
  sw) xZERO src) xSYSVARS base) oRCNT imm) ,)
  mv) x15 rd) xW rs1) ,) \ x15=len
  x16 ppop, \ x16=a
  addi) xW rd) x15 rs1) 1 imm) ,) \ xW=len+1
  andi) xW rdrs1) 3 imm) ,)
  beq) xW rs1) xZERO rs2) rforward8 imm) ,)
    sw) xZERO src) x10 rs1) ,)
    sub) x10 rdrs1) xW rs2) ,)
    addi) x10 rdrs1) 4 imm) ,)
    sw) x10 src) x12 base) ,)
  rforward!
  xdrop, \ xW='dict
  mv) x10 rd) x16 rs1) ,)
  mv) x11 rd) x15 rs1) ,)
  lblwriterange absbl,
  mv) x10 rd) x15 rs1) ,)
  lblcwrite absbl, \ x11=here
  lw) x10 rd) xW base) 0 imm) ,) \ x10=dict
  sw) x11 src) xW base) 0 imm) ,) \ "here" is new 'dict
  xdrop,
  poplr,
  lbldwrite absb,

xcode code
  xdup, addi) xW rd) xSYSVARS rs1) oSYSDICT imm) ,)
  wcall, word
  addi) x19 rd) xW rs1) 1 imm) ,)
  x19 ppush,
  lbu) xW rd) xW base) 0 imm) ,)
  wjmp, entry[]

pc xdup, auipc) xW rd) 0 imm) ,) addi) xW rdrs1) 12 imm) ,) ret,
xcode create
  wcall, code
  ( pc ) x10 pc'>reg,
  x11 20 li,
  lblwriterange absb,

xcode or x10 ppop, or) xW rdrs1) x10 rs2) ,) ret,
xcode and x10 ppop, and) xW rdrs1) x10 rs2) ,) ret,
xcode xor x10 ppop, xor) xW rdrs1) x10 rs2) ,) ret,
xcode invand not) xW rdrs1) ,) wjmp, and
xcode lshift x10 ppop, sll) xW rd) x10 rs1) xW rs2) ,) ret,
xcode rshift x10 ppop, srl) xW rd) x10 rs1) xW rs2) ,) ret,
xcode + x10 ppop, add) xW rdrs1) x10 rs2) ,) ret,
xcode - x10 ppop, sub) xW rd) x10 rs1) xW rs2) ,) ret,
xcode bool
  beq) xW rs1) xZERO rs2) rforward8 imm) ,) xW 1 li, rforward! ret,
xcode not
  beq) xW rs1) xZERO rs2) rforward8 imm) ,)
    mv) xW rd) xZERO rs1) ,) ret, rforward!
  addi) xW rdrs1) 1 imm) ,) ret,
xcode dup xdup, ret,
xcode drop xdrop, ret,
xcode swap
  lw) x10 rd) xPSP base) ,)
  sw) xW src) xPSP base) ,)
  mv) xW rd) x10 rs1) ,)
  ret,
xcode over
  xdup,
  lw) xW rd) xPSP base) 4 imm) ,)
  ret,
xcode rot
  lw) x10 rd) xPSP base) ,)
  sw) xW src) xPSP base) ,)
  lw) xW rd) xPSP base) 4 imm) ,)
  sw) x10 src) xPSP base) 4 imm) ,)
  ret,
xcode @ lw) xW rd) xW base) ,) ret,
xcode c@ lbu) xW rd) xW base) ,) ret,
xcode !
  x10 ppop,
  sw) x10 src) xW base) ,)
  xdrop,
  ret,
xcode c!
  x10 ppop,
  sb) x10 src) xW base) ,)
  xdrop,
  ret,

\ HAL operations
\ x10 is used as the immediate accumulator
\ x11 is used as the source value accumulator

\ Used by 2instr sequence instructions allowing to load immediates
\ similar to _hi asm macro, preserves x10 and x11 and x12
pc to lblhibits ( n -- n:31-12>>12 )
  x14 $fffff000 li,
  and) x13 rd) xW rs1) x14 rs2) ,)
  srli) x13 rd) x13 rs1) 12 imm) ,)
  x14 $00000800 li,
  and) x15 rd) xW rs1) x14 rs2) ,)
  beq) x15 rs1) xZERO rs2) rforward8 imm) ,)
  addi) x13 rd) x13 rs1) 1 imm) ,)
  rforward!
  mv) xW rd) x13 rs1) ,)
  ret,

\ insert immediate in "I type" of instructions, preserves x11 and x12
xcode Iimm! ( instr n -- instr )
  x10 ppop,
  x13 $fff00000 inv li,
  and) x10 rd) x10 rs1) x13 rs2) ,) \ cleaning bits, just in case
  x13 $00000fff li,
  and) x14 rd) xW rs1) x13 rs2) ,) \ xW=n, x10=instr
  slli) x14 rd) x14 rs1) 20 imm) ,)
  or) xW rd) x10 rs1) x14 rs2) ,)
  ret,

\ insert immediate in "U type" of instructions, preserves x11 and x12
pc to lblUimm ( w -- )
  x13 $fffff000 inv li,
  and) x10 rd) x10 rs1) x13 rs2) ,) \ cleaning bits, just in case
  x13 $fffff li,
  and) x14 rd) xW rs1) x13 rs2) ,) \ xW=n, x10=instr
  slli) x14 rd) x14 rs1) 12 imm) ,)
  or) x10 rd) x10 rs1) x14 rs2) ,)
  xdrop, ret,

\ Insert 32-bit imm into two sequence instructions allowing to load immediates (li or la)
pc li) xZERO rd) 0 imm) ,)
xcode loadimm, ( reg n -- )
  x11 ppop,
  dup x10 pc@>reg,
  pushlr, xdup, lblhibits absbl, lblUimm absbl, poplr,
  slli) x11 rd) x11 rs1) 7 imm) ,)
  or) x10 rd) x10 rs1) x11 rs2) ,)
  x11 ppush, pushlr, lbldwrite absbl, poplr, x11 ppop, \ lui instr written
  4 + x10 pc@>reg,
  x10 ppush,
  wcall, Iimm!
  or) xW rdrs1) x11 rs2) ,)
  slli) x11 rd) x11 rs1) 8 imm) ,)
  \ addi instruction add missing bits to already written in Rd
  or) xW rdrs1) x11 rs2) ,)
  wjmp, ,

xcode 12bovfl ( n -- ovfl ) \ preserves x10 and x11
  x19 $800 li,
  add) xW rdrs1) x19 rs2) ,)
  srli) xW rdrs1) 12 imm) ,)
  ret,

\ Compile code resulting in register Rd to contain "n"
pc addi) xZERO rd) xZERO rs1) 0 imm) ,)
xcode imm, ( reg n -- )
  xdup,
  wcall, 12bovfl
  mv) x19 rd) xW rs1) ,)
  xdrop,
  bne) x19 rs1) xZERO rs2) xwordlbl loadimm, abs>rel imm) ,)
  x11 ppop,
  x10 pc@>reg,
  x10 ppush,
  wcall, Iimm!
  slli) x11 rd) x11 rs1) 7 imm) ,)
  or) xW rdrs1) x11 rs2) ,)
  wjmp, ,

pc pushlr, \ generate two instructions
xcode pushlr,
  ( pc ) x10 pc'>reg, x11 8 li, lblwriterange absb,

pc poplr, \ generate two instructions
xcode poplr,
  ( pc ) x10 pc'>reg, x11 8 li, lblwriterange absb,

pc popexit, \ generate three instructions
xcode popexit,
  ( pc ) x10 pc'>reg, x11 12 li, lblwriterange absb,

pc ret,
xcode exit,
  ( pc ) x10 pc@>reg, lbldwrite absb,

\ Used to write 2-instructions far call
\ x11=Rd and Rs1
pc auipc) xZERO rd) 0 imm) ,) \ insert 31:12 bits
jalr) xZERO rd) xZERO rs1) 0 imm) ,) \ insert 11:0 bits
xcode relcall, ( dstreg rel -- )
  x11 ppop,
  ( pc ) dup x10 pc@>reg,
  slli) x19 rd) x11 rs1) 7 imm) ,)
  or) x10 rd) x10 rs1) x19 rs2) ,)
  x11 push,
  xdup, lblhibits abscall,
  lblUimm abscall,
  lbldwrite abscall,
  ( pc ) 4 + x10 pc@>reg,
  x11 pop,
  slli) x19 rd) x11 rs1) 7 imm) ,)
  or) x10 rd) x10 rs1) x19 rs2) ,)
  slli) x19 rd) x11 rs1) 15 imm) ,)
  or) x10 rd) x10 rs1) x19 rs2) ,)
  x10 ppush,
  wcall, Iimm!
  wjmp, ,

xcode call, ( w -- )
  x11 x12 HERE@,
  sub) xW rd) xW rs1) x11 rs2) ,)
  x11 xRA li, x11 ppush,
  wjmp, relcall,

pc xdup,
xcode litn
  ( pc ) x10 pc'>reg, x11 8 li, lblwriterange abscall,
  x11 xW li, x11 ppush,
  wjmp, imm,

\ Interpret loop
xcode stack?
  ret, ret, \ make it take 2 instrs slot because it will be realiased later

pc to L1 ( -- e ) \ x10=str. find in sys dict
  xdup, x10 ppush,
  lw) xW rd) xSYSVARS base) oSYSDICT imm) ,)
  wcall, findentry
  bne) xW rs1) xZERO rs2) rforward8 imm) ,)
    xwordlbl (wnf) absb,
  rforward!
  ret,

pc to L2 ( w -- ) \ execute
  wcall, execute
  wjmp, stack?

pc to L4 ( str -- )
  xW push, wcall, parse x10 pop, \ x10=str
  mv) x19 rd) xW rs1) ,)
  xdrop,
  bne) x19 rs1) xZERO rs2) xwordlbl litn abs>rel imm) ,)
  L1 abscall,
  mv) x18 rd) xW rs1) ,) \ x18=e
  addi) xW rd) xW rs1) 4 imm) ,)
  lbu) x10 rd) x18 base) -1 imm) ,)
  andi) x19 rd) x10 rs1) $40 imm) ,)
  beq) x19 rs1) xZERO rs2) rforward8 imm) ,)
    x19 1 li,
    sw) x19 src) xSYSVARS base) oFINDCOMPILER imm) ,)
    x18 ppush, L2 abscall, x18 ppop, rforward!
  sw) xZERO src) xSYSVARS base) oFINDCOMPILER imm) ,)
  lbu) x10 rd) x18 base) -1 imm) ,)
  andi) x19 rd) x10 rs1) $80 imm) ,)
  bne) x19 rs1) xZERO rs2) L2 abs>rel imm) ,) \ immediate? execute
  \ compile word
  wjmp, call,

xcode ]
  x10 1 li,
  sw) x10 src) xSYSVARS base) oCOMPILING imm) ,)
pc
  wcall, word
  L4 abscall,
  lw) x10 rd) xSYSVARS rs1) oCOMPILING imm) ,)
  ( pc ) bne) x10 rs1) xZERO rs2) swap abs>rel imm) ,)
  ret,

xcode run1 ( -- )
  wcall, word
  xW push, wcall, parse x10 pop,
  mv) x19 rd) xW rs1) ,)
  xdrop,
  beq) x19 rs1) xZERO rs2) rforward8 imm) ,) ret, rforward!
  L1 abscall,
  sw) xZERO src) xSYSVARS base) oFINDCOMPILER imm) ,)
  mv) x18 rd) xW rs1) ,) \ x18=e
  addi) xW rd) xW rs1) 4 imm) ,)
  lbu) x10 rd) x18 base) -1 imm) ,)
  andi) x19 rd) x10 rs1) $40 imm) ,)
  beq) x19 rs1) xZERO rs2) rforward8 imm) ,)
    x18 ppush, L2 abscall, x18 ppop, rforward!
  L2 absb,

xcode :
  wcall, code
  wcall, pushlr,
  wjmp, ]

xcode [ ximm
  sw) xZERO src) xSYSVARS base) oCOMPILING imm) ,)
  ret,

xcode ; ximm
  sw) xZERO src) xSYSVARS base) oCOMPILING imm) ,)
  wjmp, popexit,

xcode SYSVARS xdup, mv) xW rd) xSYSVARS rs1) ,) ret,

xcode main
  lw) xRSP rd) xSYSVARS rs1) oRSORIGIN imm) ,)
pc
  wcall, run1
  ( pc ) j) swap abs>rel imm) ,)

pc to lblbaseboot
  lw) xPSP rd) xSYSVARS rs1) oPSORIGIN imm) ,)
  wjmp, main
\ kernelend is called in target-specific kernels
