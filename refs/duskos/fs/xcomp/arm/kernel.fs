\ SYSVARS override: +0=port-specific
\ REQUIREMENTS
\ This unit is designed to be loaded under these conditions:
\ 1. /asm/arm.fs loaded.
\ 2. Have the "binstart" value set.
\ 3. Somewhere to make lblcoldboot jump to afterwards. This will call
\    lblbaseboot.
\ 4. When calling lblbaseboot, have r7 set to an initialized SYSVARS. In many
\    cases, you'll want to point to binstart which is initialized at compile
\    time, but in some cases, you might need something else.
\ 5. Make lblccache forward label land somewhere.

needs xcomp/tools

\ Macros
: xdrop, rW ppop, ;
: xdup, rW ppush, ;
: pushlr, rLR push, ;
: poplr, rLR pop, ;
: popexit, rPC pop, ;
: abscall, pushlr, absbl, poplr, ;
: wcall, xwordlbl abscall, ;
: wjmp, xwordlbl abs>rel b) ,) ; \ only for leaf words!

: pc>reg, ( pc r -- )
  pc rot - 8+ ( r off )
  sub) rot rd) rPC rn) swap imm) ,) ;
: pc@>reg, ( pc r -- )
  pc rot - 8+ ( r off )
  ldr) rot rd) rPC rn) swap -i) ,) ;

: xconst ( n -- ) pc swap le, xcode xdup, rW pc@>reg, ret, ;

: values ( n -- ) 0 do 0 value loop ;
10 values lblimmsplit lbladdnwr lbllitwr
          lblcwrite lbldwrite lblwriterange lblcallwr
          lblbaseboot lblcoldboot lblccache
$800 const STACKSZ

: HERE@, ( dstreg ptrreg -- )
  ldr) over rd) r7 rn) oCURALLOC +i) ,)
  ldr) rot rd) swap rn) ,) ;

\ ARM immediate system makes it difficult to place sysvars at arbitrary places
\ in the code and they need to be neatly arranged in an easy to refer
\ base+offset place. However, to facilitate initialization, we also want them to
\ be pre-initialized in the binary, at compile time. For this reason, we place
\ sysvars directly at the beginning of the binary, right after the initial
\ forward jump.
kernelbegin
forward b) ,) to lblcoldboot \ binstart + 4. SYSVARS
0 le, \ COMPILING
binstart oSYSALLOC + le, \ CURALLOC
0 le, \ SYSDICT
0 le, \ unused
0 le, \ RCNT
0 le, \ FINDCOMPILER
$20000 le, \ SYSALLOC HERE
0 le,      \ SYSALLOC HEREMAX
0 le,      \ SYSALLOC NEWHERE
0 le, \ INPTR
$1c allot0 \ unused
binstart STACKSZ - le, \ PSORIGIN
STACKSZ le, \ PSSZ
binstart le, \ RSORIGIN
binstart SYSVARSSZ + tgtallot

\ Theoretically, this code runs fine on ARMv4, but I'm not sure.
$23 xconst ARCH

xcode (wnf) mov) r6 rd) 54 imm) ,) 0 b) ,)
xcode (div0) 0 b) ,)
xcode dbg mov) r6 rd) 42 imm) ,) 0 b) ,)

xcode clearicache
  forward b) ,) to lblccache

pc to L1 \ fail
  mov) rW rd) 0 imm) ,)
  ret,

xcode parsehex ( a u -- n? f ) \ *without* the $
  mov) r0 rd) rW rm) ,)
  xdrop, \ rW=a r0=u
  cmp) r0 rn) 0 imm) ,)
  L1 abs>rel b) z) ,)
  mov) r2 rd) 0 imm) ,) \ r2=accumulated result
pc \ loop
  ldr) r1 rd) rW rn) byte) 1 +i) post) ,)
  orr) r1 rdn) $20 imm) ,)
  sub) r1 rdn) '0' imm) f) ,)
  L1 abs>rel b) lo) ,)
  cmp) r1 rn) 10 imm) ,)
  forward b) lo) ,) to L4 \ parse ok, under 10
  sub) r1 rdn) 'a' '0' - imm) f) ,)
  L1 abs>rel b) lo) ,)
  add) r1 rdn) 10 imm) ,)
  cmp) r1 rn) 16 imm) ,)
  L1 abs>rel b) hs) ,)
L4 forward! \ parse ok
  add) r2 rd) r1 rn) r2 rm) 4 lsl) ,) \ r2 = r2*16 + r1
  sub) r0 rdn) 1 imm) f) ,)
  ( loop ) abs>rel b) nz) ,)
  r2 ppush,
  mov) rW rd) 1 imm) ,)
  ret,

xcode parse ( str -- n? f )
  ldr) r0 rd) rW rn) byte) 1 +i) ,)
  cmp) r0 rn) '$' imm) ,)
  L1 abs>rel b) nz) ,) \ fail
  add) r0 rd) rW rn) 2 imm) ,)
  r0 ppush,
  ldr) rW rdn) byte) ,)
  sub) rW rdn) 1 imm) ,)
  wjmp, parsehex

xcode word ( -- str )
  ldr) r1 rd) r7 rn) oINPTR +i) ,) \ r1=addr
  pc
    ldr) r0 rd) r1 rn) byte) 1 +i) post) ,)
    cmp) r0 rn) SPC imm) ,)
    ( pc ) abs>rel b) le) ,) \ r0=first non-ws
  xdup, sub) rW rd) r1 rn) 2 imm) ,) \ rW=res
  pc
    ldr) r0 rd) r1 rn) byte) 1 +i) post) ,)
    cmp) r0 rn) SPC imm) ,)
    ( pc ) abs>rel b) gt) ,)
  str) r1 rd) r7 rn) oINPTR +i) ,)
  sub) r1 rdn) rW rm) ,)
  sub) r1 rdn) 2 imm) ,)
  str) r1 rd) rW rn) byte) ,)
  ret,

xcode execute ( a -- )
  mov) r0 rd) rW rm) ,)
  xdrop,
  r0 bx) ,)

xcode findentry ( name 'dict -- e-or-0 )
  r2 ppop, \ r2=name rW=dict
  str) r2 rd) r7 rn) oFINDSTR +i) ,)
  ldr) r1 rd) r2 rn) byte) ,) \ r1=len
  mov) r6 rd) r1 rm) 24 lsl) ,) \ r6=tofind
  add) r3 rd) r1 rn) r2 rm) ,) \ r3=lastchar
  ldr) r0 rd) r3 rn) byte) ,) \ r0=lastchar
  orr) r6 rdn) r0 rm) 16 lsl) ,)
  cmp) r1 rn) 2 imm) ,)
    ldr) hs) r0 rd) r3 rn) byte) 1 -i) ,)
    orr) hs) r6 rdn) r0 rm) 8 lsl) ,)
    ldr) hi) r0 rd) r3 rn) byte) 2 -i) ,)
    orr) hi) r6 rdn) r0 rm) ,)
pc \ loop1
  ldr) r3 rd) rW rn) 4 -i) ,) \ entry len
  bic) r3 rdn) $c0000000 imm) ,) \ remove imm flag
  cmp) r6 rn) r3 rm) ,)
  forward b) eq) ,) ( loop1 forward )
    pc to L1 \ not matching, try next
    ldr) rW rdn) ,)
    cmp) rW rn) 0 imm) ,)
    swap ( loop1 ) abs>rel b) ne) ,)
    \ not found
    ret,
  forward!
  \ same length
  sub) r4 rd) rW rn) 2 imm) ,)
  sub) r4 rdn) r1 rm) ,) \ r4=beginning of name range-1
  mov) r3 rd) r1 rm) ,) \ r3=running len
pc \ loop2
  ldr) r5 rd) r4 rn) byte) r3 +r) ,)
  ldr) r0 rd) r2 rn) byte) r3 +r) ,)
  cmp) r5 rn) r0 rm) ,)
  L1 abs>rel b) ne) ,)
  sub) r3 rdn) 1 imm) f) ,)
  ( loop2 ) abs>rel b) nz) ,)
  \ same contents
  ret,

xcode HERE xdup, ldr) rW rd) r7 rn) oCURALLOC +i) ,) ret,

pc to lblcwrite \ r0=char -- destroys r1 r2, preserves flags
  r1 r2 HERE@,
  str) r0 rd) r1 rn) byte) 1 +i) post) ,)
  str) r1 rd) r2 rn) ,)
  ret,

xcode , ( n -- )
  mov) r0 rd) rW rm) ,) xdrop,
pc to lbldwrite \ r0=n. Destroys r1 and r2, preserves flags
  r1 r2 HERE@,
  str) r0 rd) r1 rn) 4 +i) post) ,)
  str) r1 rd) r2 rn) ,)
  ret,

pc to lblwriterange \ r0=addr r1=len
  cmp) r1 rn) 0 imm) ,) \ guard against zero
  return) z) ,)
  r2 r3 HERE@,
  pc
    ldr) r4 rd) r0 rn) byte) 1 +i) post) ,)
    str) r4 rd) r2 rn) byte) 1 +i) post) ,)
    sub) r1 rdn) 1 imm) f) ,)
    ( pc ) abs>rel b) ne) ,)
  str) r2 rd) r3 rn) ,)
  ret,

xcode alignhere ( -- )
  r0 r1 HERE@,
  and) r2 rd) r0 rn) 3 imm) f) ,)
  add) ne) r0 rdn) 4 imm) ,)
  sub) ne) r0 rdn) r2 rm) ,)
  str) ne) r0 rd) r1 rn) ,)
  ret,

xcode entry[] pushlr, ( 'dict a u -- )
  xwordlbl alignhere absbl,
  mov) r0 rd) 0 imm) ,)
  str) r0 rd) r7 rn) oRCNT +i) ,)
  mov) r5 rd) rW rm) ,) \ r5=len
  r6 ppop, \ r6=a
  add) rW rdn) 1 imm) ,) \ rW=len+1
  and) rW rdn) 3 imm) f) ,)
  forward b) eq) ,)
    r1 r2 HERE@,
    str) r0 rd) r1 rn) ,) \ fill first 4b with 0. r0 is still 0
    sub) r1 rdn) rW rm) ,)
    add) r1 rdn) 4 imm) ,)
    str) r1 rd) r2 rn) ,)
  forward!
  xdrop, \ rW='dict
  mov) r0 rd) r6 rm) ,)
  mov) r1 rd) r5 rm) ,)
  lblwriterange absbl,
  mov) r0 rd) r5 rm) ,)
  lblcwrite absbl,
  ldr) r0 rd) rW rn) ,) \ r0=dict
  r1 r1 HERE@,
  str) r1 rd) rW rn) ,) \ "here" is new 'dict
  xdrop,
  poplr,
  lbldwrite abs>rel b) ,)

xcode code
  xdup,
  add) rW rd) r7 rn) oSYSDICT imm) ,)
  wcall, word
  ldr) r0 rd) rW rn) byte) 1 +i) post) ,)
  xdup, mov) rW rd) r0 rm) ,)
  wjmp, entry[]

xcode or r0 ppop, orr) rW rdn) r0 rm) f) ,) ret,
xcode and r0 ppop, and) rW rdn) r0 rm) f) ,) ret,
xcode xor r0 ppop, eor) rW rdn) r0 rm) f) ,) ret,
xcode invand r0 ppop, bic) rW rd) r0 rn) rW rm) f) ,) ret,
xcode lshift r0 ppop, mov) rW rd) r0 rm) rW rlsl) f) ,) ret,
xcode rshift r0 ppop, mov) rW rd) r0 rm) rW rlsr) f) ,) ret,
xcode + r0 ppop, add) rW rdn) r0 rm) f) ,) ret,
xcode - r0 ppop, rsb) rW rdn) r0 rm) f) ,) ret,
xcode not
  cmp) rW rn) 0 imm) ,) mov) rW rd) 0 imm) ,) add) z) rW rdn) 1 imm) ,) ret,
xcode bool
  cmp) rW rn) 0 imm) ,) mov) rW rd) 0 imm) ,) add) nz) rW rdn) 1 imm) ,) ret,
xcode dup xdup, ret,
xcode drop xdrop, ret,
xcode swap
  ldr) r0 rd) rPSP rn) ,)
  str) rW rd) rPSP rn) ,)
  mov) rW rd) r0 rm) ,)
  ret,
xcode over
  xdup,
  ldr) rW rd) rPSP rn) 4 +i) ,)
  ret,
xcode rot
  ldr) r0 rd) rPSP rn) ,)
  str) rW rd) rPSP rn) ,)
  ldr) rW rd) rPSP rn) 4 +i) ,)
  str) r0 rd) rPSP rn) 4 +i) ,)
  ret,
xcode @ ldr) rW rdn) ,) ret,
xcode c@ ldr) rW rdn) byte) ,) ret,
xcode !
  r0 ppop,
  str) r0 rd) rW rn) ,)
  xdrop,
  ret,
xcode c!
  r0 ppop,
  str) r0 rd) rW rn) byte) ,)
  xdrop,
  ret,

\ Extract 12b rotate+imm from n. Preserves r0
pc to lblimmsplit \ In: rW=n Out: rW=rotate+imm r1=rest of n.
  mov) r2 rd) 0 imm) ,) \ r2=rotate
  mov) r1 rd) 0 imm) ,)
  cmp) rW rn) $100 imm) ,)
  return) lo) ,)
pc
  mov) r3 rd) rW rm) r2 rlsr) f) ,)
  return) z) ,) \ rW is zero, nothing to do
  tst) r3 rn) 3 imm) ,)
  add) z) r2 rdn) 2 imm) ,)
  ( pc ) abs>rel b) z) ,)
  and) r3 rdn) $ff imm) ,) \ r3=8b imm
  sub) r1 rd) rW rn) r3 rm) r2 rlsl) f) ,) \ r1=rest of n
  rsb) r2 rdn) 0 imm) ,) \ rotate is to the *right*
  and) r2 rdn) $1e imm) ,) \ 0-32, even numbers
  orr) rW rd) r3 rn) r2 rm) 7 lsl) ,) \ rW=rotate+imm
  ret,

xcode immrot ( n -- imm+rot rest *Z* )
  lblimmsplit abscall,
  xdup,
  mov) rW rd) r1 rm) f) ,)
  ret,

\ Compile a add) of immediate "n" with target register selected in r1
pc add) 0 imm) f) ,)
pc to lbladdnwr ( n -- ) \ r1=Rd/Rn
  ( pc ) r0 pc@>reg,
  orr) r0 rdn) r1 rm) 12 lsl) ,)
  orr) r0 rdn) r1 rm) 16 lsl) ,)
  tst) rW rn) $80000000 imm) ,)
  eor) ne) r0 rdn) $00c00000 imm) ,) \ add) to sub)
  rsb) ne) rW rdn) 0 imm) ,) \ negate rW
  pushlr,
pc
  lblimmsplit abs>rel bl) ,)
  r0 push,
  orr) r0 rdn) rW rm) ,)
  mov) rW rd) r1 rm) ,)
  lbldwrite abs>rel bl) ,)
  r0 pop,
  cmp) rW rn) 0 imm) ,)
  ( pc ) abs>rel b) nz) ,)
  xdrop, popexit,

xcode addlit, ( dstreg n -- ) r1 ppop, lbladdnwr absb,

pc mov) 0 imm) ,)
pc to L1 ( imm+rotate -- ) \ r0=Rd
  r1 ppush, ( rest imm+rotate )
  ( pc ) r3 pc@>reg,
  r0 push,
  orr) r0 rd) r3 rn) r0 rm) 12 lsl) ,) \ merge Rd in instr
  orr) r0 rdn) rW rm) ,) xdrop,
  lbldwrite abscall,
  r1 pop, \ r1=Rd
  cmp) rW rn) 0 imm) ,)
  lbladdnwr abs>rel b) ne) ,)
  xdrop, ret,

xcode imm, ( dstreg n -- )
  r0 ppop,
\ Compile code resulting in register Rd to contain "n"
pc to lbllitwr ( n -- ) \ r0=Rd
  lblimmsplit abscall,
  L1 absb,

pc pushlr,
xcode pushlr,
  ( pc ) r0 pc@>reg, lbldwrite abs>rel b) ,)

pc poplr,
xcode poplr,
  ( pc ) r0 pc@>reg, lbldwrite abs>rel b) ,)

pc popexit,
xcode popexit,
  ( pc ) r0 pc@>reg, lbldwrite abscall,
  wjmp, clearicache

pc ret,
xcode exit,
  ( pc ) r0 pc@>reg, lbldwrite abscall,
  wjmp, clearicache

pc to lblcallwr ( w -- )
  mov) r2 rd) $eb000000 imm) ,)
  r1 r1 HERE@,
  sub) r0 rd) rW rn) r1 rm) ,)
  mov) r0 rd) r0 rm) 2 lsr) ,)
  sub) r0 rdn) 2 imm) ,)
  bic) r0 rn) $ff000000 imm) ,)
  orr) r0 rdn) r2 rm) ,)
  xdrop,
  lbldwrite abscall,
  wjmp, clearicache

pc xdup,
xcode litn
  ( pc ) r0 pc@>reg, lbldwrite abscall,
  mov) r0 rd) rW imm) ,) lbllitwr absb,

xcode (/mod) \ r0=dividend rS=divisor
  cmp) rS rn) 0 imm) ,)
  xwordlbl (div0) abs>rel b) z) ,)
  \ r0 is active remainder
  mov) r1 rd) 0 imm) ,) \ r1=quotient
  mov) r2 rd) rS rm) ,) \ r2=tmp
  \ double tmp until 2 * tmp > dividend
  cmp) r2 rn) r0 rm) 1 lsr) ,)
pc
  mov) ls) r2 rd) r2 rm) 1 lsl) ,)
  cmp) r2 rn) r0 rm) 1 lsr) ,)
  ( pc ) abs>rel b) ls) ,)
pc
  cmp) r0 rn) r2 rm) ,) \ can we subtract temp from dividend?
  sub) cs) r0 rdn) r2 rm) ,) \ if yes, do it
  adc) r1 rdn) r1 rm) ,) \ double and add carry
  mov) r2 rd) r2 rm) 1 lsr) ,)
  cmp) r2 rn) rS rm) ,)
  ( pc ) abs>rel b) hs) ,)
  mov) rS rd) r0 rm) ,) \ set remainder
  mov) r0 rd) r1 rm) ,) \ set quotient
  ret,

xcode (s/mod)
  cmp) r0 rn) 0 imm) ,)
  forward b) ge) ,)
    rsb) r0 rdn) 0 imm) ,)
    cmp) rS rn) 0 imm) ,)
    forward b) ge) ,)
      rsb) rS rdn) 0 imm) ,)
      wcall, (/mod)
      rsb) rS rdn) 0 imm) ,)
      ret,
    forward!
    wcall, (/mod)
    rsb) r0 rdn) 0 imm) ,)
    rsb) rS rdn) 0 imm) ,)
    ret,
  forward!
  cmp) rS rn) 0 imm) ,)
  xwordlbl (/mod) abs>rel b) ge) ,)
  rsb) rS rdn) 0 imm) ,)
  wcall, (/mod)
  rsb) r0 rdn) 0 imm) ,)
  ret,

pc xdup, mov) rW rd) rPC rm) ,) rLR bx) ,)
xcode create
  wcall, code
  ( pc ) r0 pc>reg,
  mov) r1 rd) 12 imm) ,)
  lblwriterange abscall,
  wjmp, clearicache

\ Interpret loop
xcode stack? ret,

pc to L1 ( str -- e )
  xdup,
  ldr) rW rd) r7 rn) oSYSDICT +i) ,)
  wcall, findentry
  teq) rW rn) 0 imm) ,)
  xwordlbl (wnf) abs>rel b) eq) ,)
  ret,

pc to L2 ( w -- ) \ execute
  wcall, execute
  wjmp, stack?

\ Z is set if parse successful, str is present if not successful
pc to L3 ( -- ?str n-or-a )
  wcall, word
  xdup, wcall, parse
  cmp) rW rn) 0 imm) ,)
  xdrop,
  add) rPSP rdn) 4 imm) ne) ,) \ maybe nip
  ret,

pc to L4 ( -- )
  L3 abscall,
  xwordlbl litn abs>rel b) ne) ,) \ literal: jump to litn
  L1 abscall,
  mov) r1 rd) rW rm) ,) \ r1=e
  add) rW rdn) 4 imm) ,)
  ldr) r0 rd) r1 rn) byte) 1 -i) ,)
  tst) r0 rn) $40 imm) ,)
  forward b) z) ,)
    mov) r0 rd) 1 imm) ,)
    str) r0 rd) r7 rn) oFINDCOMPILER +i) ,)
    r1 ppush, L2 abscall, r1 ppop, forward!
  mov) r0 rd) 0 imm) ,)
  str) r0 rd) r7 rn) oFINDCOMPILER +i) ,)
  ldr) r0 rd) r1 rn) byte) 1 -i) ,)
  tst) r0 rn) $80 imm) ,)
  L2 abs>rel b) ne) ,) \ immediate? execute
  \ compile word
  lblcallwr absb,

xcode ]
  mov) r0 rd) 1 imm) ,)
  str) r0 rd) r7 rn) oCOMPILING +i) ,)
pc
  L4 abscall,
  ldr) r0 rd) r7 rn) oCOMPILING +i) ,)
  cmp) r0 rn) 0 imm) ,)
  ( pc ) abs>rel b) nz) ,)
  ret,

xcode run1 ( -- )
  L3 abscall,
  return) ne) ,) \ literal: nothing to do
  L1 abscall,
  mov) r0 rd) 0 imm) ,)
  str) r0 rd) r7 rn) oFINDCOMPILER +i) ,)
  mov) r1 rd) rW rm) ,) \ r1=e
  add) rW rdn) 4 imm) ,)
  ldr) r0 rd) r1 rn) byte) 1 -i) ,)
  tst) r0 rn) $40 imm) ,)
  forward b) z) ,)
    r1 ppush, L2 abscall, r1 ppop, forward!
  L2 abs>rel b) ,)

xcode :
  wcall, code
  wcall, pushlr,
  wjmp, ]

xcode [ ximm
  mov) r0 rd) 0 imm) ,)
  str) r0 rd) r7 rn) oCOMPILING +i) ,)
  ret,

xcode ; ximm
  mov) r0 rd) 0 imm) ,)
  str) r0 rd) r7 rn) oCOMPILING +i) ,)
  wjmp, popexit,

xcode SYSVARS xdup, mov) rW rd) r7 rm) ,) ret,

xcode main
  ldr) rSP rd) r7 rn) oRSORIGIN +i) ,)
pc
  wcall, run1
  ( pc ) abs>rel b) ,)

pc to lblbaseboot
  ldr) rPSP rd) r7 rn) oPSORIGIN +i) ,)
  wjmp, main
\ kernelend is called in target-specific kernels
