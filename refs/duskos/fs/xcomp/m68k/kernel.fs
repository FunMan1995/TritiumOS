needs xcomp/tools asm/label asm/m68k xcomp/m68k/cpusel
unit xcomp/m68k/kernel

\ A4=payload address
\ A5=$400 bytes scratch area.
\ A5+$100 is our SYSVARS. We need this to avoid Quickdraw-related
\ memory corruption. When we initialize QD later in the boot process,
\ we can make A5=SYSVARS
\ A6=A5+$300. That's PSP.
\ SYSV+00=wordk flag
\ SYSV+40=console line num
\ Macros
: push, ( src -- ) RSP -[An]) swap move, ;
: pushw, ( src -- ) RSP -[An]) word) swap move, ;
: pop, ( dst -- ) RSP [An]+) move, ;
: ppush, ( src -- ) PSP -[An]) swap move, ;
: ppop, ( dst -- ) PSP [An]+) move, ;
: xdrop, W ppop, ;
: xdup, W ppush, ;
: absbsr, abs>rel bsr, ;
: absbra, abs>rel bra, ;
: wcall, xwordlbl absbsr, ;
: wjmp, xwordlbl absbra, ;
: sysv) ( off -- ) A5 swap $100 + [An,d]) ;
: willrealias, rts, rts, ; \ needs 4 bytes

: HERE@, ( dst ptr -- ) dup oCURALLOC sysv) move, [An]) move, ;

0 value lblcoldboot
0 value lblwwrite
0 value lbldwrite
kernelbegin

forward16 bra, to lblcoldboot
xcode clearicache clearcache, rts,
xcode dbg 0 bra,

xcode + W PSP [An]+) add, rts,
xcode - PSP [An]) W sub, xdrop, rts,
xcode or W PSP [An]+) or, rts,
xcode and W PSP [An]+) and, rts,
xcode xor D0 ppop, W D0 eor, rts,
xcode invand W not, W PSP [An]+) and, rts,
xcode lshift D0 ppop, W D0 exg, W D0 lsl#r, rts,
xcode rshift D0 ppop, W D0 exg, W D0 lsr#r, rts,
xcode bool W tst, W sne, W 1 andi, rts,
xcode not W tst, W seq, W 1 andi, rts,
xcode @ A0 W move, W A0 [An]) move, rts,
xcode ! A0 W move, xdrop, A0 [An]) W move, xdrop, rts,
xcode w@ A0 W move, W clr, W A0 [An]) word) move, rts,
xcode w! A0 W move, xdrop, A0 [An]) word) W move, xdrop, rts,
xcode c@ A0 W move, W clr, W A0 [An]) byte) move, rts,
xcode c! A0 W move, xdrop, A0 [An]) byte) W move, xdrop, rts,
xcode dup xdup, rts,
xcode drop xdrop, rts,
xcode swap
  D0 PSP [An]) move,
  PSP [An]) W move,
  W D0 move,
  rts,
xcode over xdup, W PSP 4 [An,d]) move, rts,
xcode rot
  D0 PSP [An]) move,
  PSP [An]) W move,
  W PSP 4 [An,d]) move,
  PSP 4 [An,d]) D0 move,
  rts,

xcode SYSVARS xdup, A0 0 sysv) lea, W A0 move, rts,

pc to L1 \ fail
  xdrop, W clr, rts,

xcode parsehex ( a u -- n? f ) \ *without* the $
  A4 PSP [An]) move, \ A4=a W=u
  W tst,
  L1 abs>rel beq,
  D4 clr, \ D4=accumulated result
  D0 clr,
  W 1 subi, \ W = counter
  pc
    D0 A4 [An]+) byte) move,
    D0 byte) $20 ori,
    D0 byte) '0' subi,
    L1 abs>rel bcs,
    D0 10 cmpi,
    forward8 bcs, to L2 \ parse ok, under 10
    D0 'a' '0' - subi,
    L1 abs>rel bcs,
    D0 10 addi,
    D0 16 cmpi,
    L1 abs>rel bcc,
    L2 forward! \ parse ok
    D4 4 lsl#,
    D4 D0 add,
    W swap ( pc ) abs>rel dbra,
  PSP [An]) D4 move,
  W 1 moveq,
  rts,

xcode parse ( str -- ?n f )
  A0 W move,
  A0 1 [An,d]) byte) '$' cmpi,
  forward8 beq, W clr, rts, forward!
  W 2 addi,
  xdup, W clr,
  W A0 [An]) byte) move,
  W 1 subi,
  wjmp, parsehex

xcode word ( -- str )
  A0 oINPTR sysv) move,
  pc
    A0 [An]+) byte) SPC cmpi,
    ( pc ) abs>rel ble,
  A1 A0 -2 [An,d]) lea, \ A1=str
  pc
    A0 [An]+) byte) SPC cmpi,
    ( pc ) abs>rel bgt,
  oINPTR sysv) A0 move,
  D0 A0 move,
  D0 A1 sub,
  D0 2 subi, \ D0=len
  A1 [An]) byte) D0 move,
  xdup, W A1 move,
  rts,

xcode findentry ( name 'dict -- e-or-0 ) \ Z is set depending on "e"
  A4 W move, \ A4=dict
  A3 ppop,   \ A3=name
  oFINDSTR sysv) A3 move,
  D4 clr, D4 A3 [An]) byte) move, \ D4=len
  \ Because m68k data bus is 16-bit, we use a 16-bit "tofind"
  D3 clr, D3 A3 [An]) D4 Xn]) byte) move,
  D3 8 lsl#,
  D3 D4 or, \ D3=16-bit tofind
  pc to L1 \ loop1
    D2 A4 -2 [An,d]) word) move, \ D2=len+lastchar
    D2 $ff3f andi,
    D2 D3 word) cmp,
    forward8 beq, to L2
      pc to L3 \ not found, try next
      A4 A4 [An]) move,
      W A4 move,
      L1 abs>rel bne,
      \ word not found!
      rts,
    L2 forward!
  \ same length
  D1 D4 move,
  D1 1 subi, \ D1=counter
  A2 A4 -1 [An,d]) lea, \ A2=entry tail
  A1 A3 1 [An,d]) D4 Xn]) lea, \ A1=name tail
  pc to L1 \ loop2
    D0 A2 -[An]) byte) move,
    D0 A1 -[An]) byte) cmp,
    L3 abs>rel bne, \ nope, not this one, try again!
    D1 L1 abs>rel dbra,
  \ we have a match!
  W A4 move,
  rts,

xcode HERE xdup, W oCURALLOC sysv) move, rts,

xcode c, ( n -- )
  D0 W move, xdrop,
  A1 A0 HERE@,
  A1 [An]) byte) D0 move,
  A0 [An]) 1 addi,
  rts,

xcode w, ( n -- )
  D0 W move, xdrop,
pc to lblwwrite \ D0=n
  A1 A0 HERE@,
  A1 [An]) word) D0 move,
  A0 [An]) 2 addi,
  rts,

xcode , ( n -- )
  D0 W move, xdrop,
pc to lbldwrite \ D0=n
  A1 A0 HERE@,
  A1 [An]) D0 move,
  A0 [An]) 4 addi,
  rts,

xcode alignhere ( -- ) \ output: D0=newhere A0='here
  D0 A0 HERE@,
  D1 D0 move,
  D1 byte) 3 andi,
  forward8 beq,
    D0 byte) $fc andi,
    D0 4 addi,
    A0 [An]) D0 move,
  forward!
  rts,

xcode entry[] ( 'dict a u -- )
  oRCNT sysv) clr,
  wcall, alignhere \ D0=here A0='here
  A1 D0 move, \ A1=here
  A2 ppop, \ A2=a
  W tst, forward8 bne,
    A1 [An]+) W move, forward8 bra, to L1 forward!
  D1 W move, \ D1=len
  D1 1 addi,
  D1 3 andi,
  forward8 beq,
    A1 [An]) clr,
    D0 D1 sub,
    D0 4 addi,
    A1 D0 move,
  forward! \ A1=here A0='here
  D1 W move,
  D1 1 subi, \ D1=counter
  pc
    D0 A2 [An]+) byte) move,
    A1 [An]+) byte) D0 move,
    D1 swap abs>rel dbra,
  A1 [An]+) byte) W move, \ write len
  L1 forward!
  A2 ppop, xdrop, \ A2='dict
  D0 A2 [An]) move, \ D0=dict
  A2 [An]) A1 move, \ "here" is the new dict top
  A1 [An]+) D0 move, \ write dict link
  A0 [An]) A1 move, \ update "here"
  rts,

xcode code
  xdup, A0 oSYSDICT sysv) lea, W A0 move,
  wcall, word
  A0 W move,
  W clr, W A0 [An]+) byte) move,
  A0 ppush,
  wjmp, entry[]

xcode pushlr, rts,
xcode poplr, rts,
xcode exit,
  D0 $4e75 imm) move,
  lblwwrite abs>rel bsr,
  clearcache,
  rts,

xcode litn
  D0 $2d072e3c imm) move, \ xdup, W imm) move,
  lbldwrite abs>rel bsr,
  wjmp, ,

xcode execute ( a -- )
  A0 W move,
  xdrop,
  A0 [An]) jmp,

xcode (wnf) willrealias,
xcode (div0) willrealias,

xcode '
  wcall, word
  xdup, W oSYSDICT sysv) move,
  wcall, findentry
  W 4 addi,
  rts,

xcode bri, ( absaddr instr -- )
  D1 A0 HERE@,
  D1 2 addi,
  D0 ppop,
  D0 D1 sub, \ D0=rel
  forward8 beq, \ special case: rel=0 can't be 8b
    D1 D0 move,
    D1 $80 addi,
    D1 $ff cmpi,
    forward8 bcc, \ 8-bit
      W byte) D0 or,
      wcall, w,
      clearcache,
      rts, forward!
    forward!
  D0 ppush,
  D0 $8000 addi,
  D0 $10000 cmpi,
  forward8 bcc, \ 16-bit
    wcall, w,
    wcall, w,
    clearcache,
    rts, forward!
  \ 32-bit
  W $ff ori,
  wcall, w,
  wcall, ,
  clearcache,
  rts,

xcode stack? willrealias,

pc to L3 ( xt -- )
  wcall, execute
  wjmp, stack?

pc to L4 ( -- )
  wcall, word
  xdup, wcall, parse
  W tst,
  forward8 beq,
    xdrop, wcall, litn xdrop, rts, forward!
  W oSYSDICT sysv) move,
  wcall, findentry
  xwordlbl (wnf) abs>rel beq,
  A0 W move, \ A0=e
  W 4 addi,
  D0 A0 -1 [An,d]) byte) move,
  D0 byte) $40 andi,
  forward8 beq,
    oFINDCOMPILER sysv) 1 imm) move,
    A0 ppush, L3 absbsr, A0 ppop, forward!
  oFINDCOMPILER sysv) 0 imm) move,
  D0 A0 -1 [An,d]) byte) move,
  D0 byte) $80 andi,
  L3 abs>rel bne, \ immediate? execute
  xdup, W $6100 imm) move,
  wjmp, bri,

xcode ]
  oCOMPILING sysv) 1 imm) move,
  pc \ loop
    L4 absbsr,
    oCOMPILING sysv) tst,
    ( loop ) abs>rel bne,
  rts,

xcode [ ximm
  oCOMPILING sysv) 0 imm) move,
  rts,

xcode ; ximm
  oCOMPILING sysv) 0 imm) move,
  wjmp, exit,

xcode :
  wcall, code
  wjmp, ]

xcode create
  wcall, code
  D0 $2d0741fa imm) move, \ xdup, A0 8 [PC,d]) lea,
  lbldwrite absbsr,
  D0 $00082e08 imm) move, \ PC,d offset W A0 move,
  lbldwrite absbsr,
  D0 $4e714e75 imm) move, \ nop, rts,
  lbldwrite absbsr,
  clearcache,
  rts,

xcode run1 ( -- )
  wcall, word
  W push, wcall, parse D0 pop, \ D0=str
  W tst,
  forward8 beq, xdrop, rts, forward!
  D0 ppush,
  W oSYSDICT sysv) move,
  wcall, findentry
  xwordlbl (wnf) abs>rel beq,
  oFINDCOMPILER sysv) 0 imm) move,
  \ TODO: add BTST in asm/m68k
  A0 W move, \ A0=e
  W 4 addi,
  D0 A0 -1 [An,d]) byte) move,
  D0 byte) $40 andi,
  forward8 beq, A0 ppush, L3 absbsr, A0 ppop, forward!
  L3 absbra,

xcode main
  pc dup to L1 \ L1=xlatest's payload address
    wcall, run1
    abs>rel bra,

lblcoldboot forward!
  A6 A5 $400 [An,d]) lea, \ A4=INPTR A5=SYSVARS A6=PSP
  oPSORIGIN sysv) A6 move,
  oPSSZ sysv) $400 imm) move,
  oRSORIGIN sysv) A7 move,
  oINPTR sysv) A4 move,
  \ this code will never be needed again, so HERE starts here!
  A0 0 [PC,d]) lea,
  oSYSALLOC sysv) A0 move,
  A0 oSYSALLOC sysv) lea,
  oCURALLOC sysv) A0 move,

  \ Dictionary relinking
  A1 L1 pc - 4- [PC,d]) lea, \ A1=top sysdict entry
  oSYSDICT sysv) A1 move,
  D0 A1 move, D0 xlatest imm) sub, \ D0=offset
  A0 A1 move,
  pc \ A0 = entry
    A0 [An]) D0 add,
    A0 A0 [An]) move,
    A0 [An]) tst,
    ( pc ) abs>rel bne,

  wjmp, main

kernelendbe
