\ This unit contains kernel words that are common to both i386 and amd64
\ It it expected to be loaded in the middle of the compilation process and needs
\ those macros:
\ sysv) ( off -- opmod )
\   Yields an opmod that references the specified SYSVARS offset. Under i386
\   it's "bp swap d)" but under amd64 it's platform-specific.

xcode (wnf) 0 jmp, 3 allot0 \ for realias
xcode (div0) 0 jmp, 3 allot0 \ for realias
xcode dbg ret,

xcode +   dx ppop, ax dx add, ret,
xcode -   dx ppop, ax neg, ax dx add, ret,
xcode and dx ppop, ax dx and, ret,
xcode invand ax not, dx ppop, ax dx and, ret,
xcode or  dx ppop, ax dx or, ret,
xcode xor dx ppop, ax dx xor, ret,
xcode lshift cx ax mov, xdrop, ax cl shl, ret,
xcode rshift cx ax mov, xdrop, ax cl shr, ret,
xcode not ax ax test, al setz, ax 1 imm) and, ret,
xcode bool ax ax test, al setnz, ax 1 imm) and, ret,
xcode dup  xdup, ret,
xcode drop xdrop, ret,
xcode swap ax si 0 ?d+bp) xchg, ret,
xcode over xdup, ax si 4 ?d+bp) mov, ret,
xcode rot  ax si 0 ?d+bp) xchg, ax si 4 ?d+bp) xchg, ret,
xcode @  ax ax 0 ?d+bp) mov, ret,
xcode c@ ax ax 0 ?d+bp) byte) movzx, ret,
xcode !  cx ppop, ax 0 ?d+bp) cx mov, xdrop, ret,
xcode c!  cx ppop, ax 0 ?d+bp) cl mov, xdrop, ret,

xcode HERE xdup, ax oCURALLOC sysv) mov, ret,
xcode , ax dwrite, xdrop, ret,
xcode w, ax wwrite, xdrop, ret,
xcode c, al cwrite, xdrop, ret,

xcode exit, $c3 imm) cwrite, ret,
xcode popexit, wjmp, exit,
xcode pushlr, ret,
xcode poplr, ret,

xcode word ( -- str )
  xdup, \ reserve wiggle room on PS.
  di oINPTR sysv) mov,
  pc
    al di 0 ?d+bp) mov,
    di inc,
    al SPC 1+ imm) cmp, \ is ws?
    ( pc ) abs>rel jc,
  dx di mov,
  dx dec, dx dec,
  dx 0 ?d+bp) byte) 0 imm) mov,
  pc
    dx 0 ?d+bp) byte) inc,
    al di 0 ?d+bp) mov,
    di inc,
    al SPC 1+ imm) cmp, \ is ws?
    ( pc ) abs>rel jnc,
  oINPTR sysv) di mov,
  ax dx mov,
  ret,

xcode findentry ( str 'dict -- entry-or-0 )
  bx ax mov, xdrop, \ ax=str bx='dict
  oFINDSTR sysv) ax mov,
  si push,
  cx ax 0 ?d+bp) byte) movzx, \ cx=sz
  si ax mov,
  si cx add, \ si=last char
  si si -2 ?d+bp) mov,
  si $ffffff imm) and, \ si=last 3 chars
  ax inc, \ ax=str+1
  dx cx mov, dx 24 imm) shl, \ dx=tofind
  dx si or,
  cx 1 imm) cmp, forward8 jnz, dx $ffff0000 imm) and, forward!
  cx 2 imm) cmp, forward8 jnz, dx $ffffff00 imm) and, forward!
pc ( loop )
  cx bx -4 ?d+bp) mov, \ cx=last 3 chars + len
  cx $3fffffff imm) and,
  dx cx cmp,
  forward jz, ( loop forward )
    pc to L1 \ no match, try next
    bx bx 0 ?d+bp) mov,
    bx bx test,
    swap ( loop ) abs>rel jnz,
    \ not found
    si pop,
    ax ax xor,
    ret,
  forward!
  \ same length
  di bx mov,
  di 1 imm) sub,
  cx 24 imm) shr, \ cx=len
  di cx sub, \ di=beginning of name
  si ax mov,
  si ?bp+, di ?bp+,
  repz, cmpsb,
  L1 abs>rel jnz,
  \ same contents
  si pop,
  ax bx mov,
  ret,

pc to L1 \ parse unsuccessful
  ax ax xor,
  ret,

xcode parsehex ( a u -- n? f ) \ *without the $*
  cx ppop, ( u )
  ax ax or,
  L1 abs>rel jz,   \ fail
  ax cx xchg, \ eax=a ecx=u
  di di xor, \ res
  dx dx xor,
pc ( loop )
  dl ax 0 ?d+bp) mov,
  dl $20 imm) or,
  dl '0' imm) sub,
  L1 abs>rel jc,   \ fail
  dl 10 imm) cmp,
  forward8 jc, \ parse ok, under 10
    dl 'a' '0' - imm) sub,
    L1 abs>rel jc,   \ fail
    dl 10 imm) add,
    dl 16 imm) cmp,
    L1 abs>rel jnc,   \ fail
  forward! \ parse ok
  di 4 imm) shl,     \ res*16
  di dx add,
  ax inc,
  ( pc ) abs>rel loop, ( loop )
  xgrow,
  si 0 ?d+bp) di mov,
  ax 1 imm) mov,
  ret,

xcode parse ( str -- n? f )
  cx ax 1 ?d+bp) byte) movzx,
  cx '$' imm) cmp,
  L1 abs>rel jnz,   \ fail
  cx ax 0 ?d+bp) byte) movzx,
  cx dec,
  ax inc,
  ax inc,
  xdup,
  ax cx mov,
  wjmp, parsehex

xcode stack? ret, 4 allot0 \ for realias

xcode execute ( a -- )
  di ax mov,
  xdrop,
  di ?bp+, di jmpr,

pc to L1 ( -- e ) \ dx=str. find in sys dict
  xdup,
  ax dx mov,
  xdup, ax oSYSDICT sysv) mov,
  wcall, findentry
  ax ax test,
  xwordlbl (wnf) abs>rel jz,
  ret,

pc to L2 ( xt -- ) \ execute imm word
  wcall, execute
  wjmp, stack?

pc to lblcallwr \ di=absaddr
  $e8 imm) cwrite, \ CALL opcode
  cx oCURALLOC sysv) mov,
  di cx 0 ?d+bp) sub, \ displacement
  di 4 imm) sub,       \ ... from *after* call op
  di dwrite,
  ret,

pc to L3 ( str -- )
  ax push, wcall, parse dx pop,
  di ax mov, xdrop,
  di di test,
  xwordlbl litn abs>rel jnz, \ literal: jump to litn
  \ not a literal, find and compile
  L1 abscall, \ ax=e
  xdup,
  ax 4 imm) add,
  ax -5 ?d+bp) byte) $40 imm) test, forward8 jz,
    oFINDCOMPILER sysv) 1 imm) mov,
    L2 abscall, forward!
  oFINDCOMPILER sysv) 0 imm) mov,
  di ppop, \ DI=e
  di -1 ?d+bp) byte) $80 imm) test, forward8 jz,
    L2 absjmp, forward!
  di ax mov,
  xdrop,
  lblcallwr absjmp,

xcode ]
  oCOMPILING sysv) 1 imm) mov,
pc
  wcall, word
  L3 abscall,
  oCOMPILING sysv) -1 imm) test,
  ( pc ) abs>rel jnz,
pc to L3
  ret,

xcode run1 ( -- )
  wcall, word
  ax push, wcall, parse dx pop,
  di ax mov, xdrop,
  di di test,
  L3 abs>rel jnz, \ literal: nothing to do
  \ not a literal, find and execute
  L1 abscall,
  di ax mov,
  ax 4 imm) add,
  oFINDCOMPILER sysv) 0 imm) mov,
  di -1 ?d+bp) byte) $40 imm) test, forward8 jz,
    di ppush, L2 abscall, xnip, forward!
  L2 absjmp,

xcode alignhere
  cx HERE@,
  cx 3 imm) and,
  forward8 jz,
    cx 4 imm) sub,
    cx neg,
    di oCURALLOC sysv) mov,
    di 0 ?d+bp) cx add,
  forward!
  ret,

xcode entry[] ( 'dict a u -- )
  wcall, alignhere
  di si 0 ?d+bp) mov, \ di=a
  cx ax mov, \ cx=len
  ax cx mov,
  ax inc,
  ax 3 imm) and,
  forward8 jz,
    dx HERE@,
	dx 0 ?d+bp) 0 imm) mov,
    dx ax sub,
    dx 4 imm) add,
    ax oCURALLOC sysv) mov,
    ax 0 ?d+bp) dx mov,
  forward!
  xnip, xdrop, \ ( 'dict -- )
  cx push,
  si push,
  si di mov,
  dx oCURALLOC sysv) mov,
  di dx 0 ?d+bp) mov,
  dx 0 ?d+bp) cx add,
  si ?bp+, di ?bp+,
  rep, movsb,
  si pop,
  cx pop,
  cl cwrite,
  di ax 0 ?d+bp) mov, \ ax='dict di=dict
  dx HERE@,
  ax 0 ?d+bp) dx mov, xdrop, ( -- )
  di dwrite,
  oRCNT sysv) 0 imm) mov,
  ret,

xcode code
  xdup, ax oSYSDICT sysv) lea, ax ?bp-,
  wcall, word
  cx ax 0 ?d+bp) byte) movzx,
  ax inc,
  xdup,
  ax cx mov,
  wjmp, entry[]

xcode :
  wcall, code
  wjmp, ]

xcode [ ximm
  oCOMPILING sysv) 0 imm) mov,
  ret,

xcode ; ximm
  oCOMPILING sysv) 0 imm) mov,
  wjmp, exit,

\ Compiling a call to "create code" in all possible kernels is tricky. We're
\ using a spring here.
pc to L2 di pop, ret,
pc to L1 L2 abscall, \ cell code addr will be in EDI.
  \ always aligned. call is 5b, 3b padding.
  xdup, ax pop, ax ?bp-, ax 3 imm) add, ret,

xcode create
  wcall, code
  L1 abscall,
  di ?bp-,
  lblcallwr abscall,
  0 imm) wwrite, 0 imm) cwrite, \ align to 4b
  ret,

xcode SYSVARS xdup, ax 0 sysv) lea, ax ?bp-, ret,

