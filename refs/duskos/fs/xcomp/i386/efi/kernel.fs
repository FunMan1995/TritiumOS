needs asm/x86 xcomp/tools

0 to binstart
$1000 const STACKSZ
0 value lblspring
0 value lblreloc

\ ESP indexes off by 1 because of return address on stack
: argN!, ( n -- ) sp swap 1+ 4* d) ax mov, ;
f<< /xcomp/i386/kernel.fs

pc to lblspring ax pop, ret,
pc to lblreloc lblspring abscall, pc le, \ reloc canary

xcode q@ ax ax 0 d) mov, ret,
xcode q+n, $05 imm) cwrite, ax dwrite, xdrop, ret,
xcode absaddr ret,
xcode reladdr ret,
xcode qrshift32 ax ax xor, ret,
xcode q* ax 2 imm) shl, ret,
xcode qmove ( dst u src64 -- )
  si push,
  cx si 0 d) mov,
  di si 4 d) mov,
  si ax mov,
  rep, movsb,
  si pop,
  xnip, xnip, xdrop,
  ret,

xcode inccb \ void *notify(EFI_EVENT event, void *ctx)
  ax sp 8 d) mov,
  ax 0 d) inc,
  ret,

xcode argstart ( -- )
  bx pop,
  di sp mov,
  sp $f inv imm) and, \ align to 16 bytes
  di push, \ Stack is 16b *disaligned* by exactly 4b
  sp 15 4* imm) sub, \ 16b aligned
  bx jmpr,

xcode arg0k! 0 argN!, ret,
xcode arg0! 0 argN!, xdrop, ret,
xcode arg1! 1 argN!, xdrop, ret,
xcode arg2! 2 argN!, xdrop, ret,
xcode arg3! 3 argN!, xdrop, ret,
xcode arg4! 4 argN!, xdrop, ret,
xcode arg5! 5 argN!, xdrop, ret,
xcode arg6! 6 argN!, xdrop, ret,
xcode arg7! 7 argN!, xdrop, ret,
xcode arg8! 8 argN!, xdrop, ret,
xcode arg9! 9 argN!, xdrop, ret,
xcode arg10! 10 argN!, xdrop, ret,
xcode arg11! 11 argN!, xdrop, ret,

xcode efiexec ( a64 -- res64 )
  di pop,
  ax callr,
  sp 15 4* imm) add,
  sp pop,
  di jmpr,

\ We put system variables right after SYSVARS, right before HERESTART
\ +00 SystemTable
\ +04 ImageHandle
\ +08 AllocatedPages
xcode SystemTable
  xdup, ax bp SYSVARSSZ d) mov, ret,

xcode ImageHandle
  xdup, ax bp SYSVARSSZ 4+ d) mov, ret,

xcode AllocatedPages
  xdup, ax bp SYSVARSSZ 8+ d) mov, ret,
  xdup,
  di L3 imm) mov,
  ax di 8 d) mov,
  ret,

\ This space is used at compile time to store INPTR, but also as a temporary
\ 4b buffer for the AllocatePages call.
pc to L2 0 le, \ start value for INPTR

\ ESP is disaligned by 4. ESP+4=ImageHandle ESP+8=SystemTable
lblcoldboot forward!
  lblreloc abscall,
  dx ax 0 d) mov,
  ax dx sub, \ EAX=baseaddr
  dx ax L2 d) mov,
  dx ax add, \ EBX=absolute INPTR
  dx push, \ INPTR AL+8
  ax push, \ baseaddr AL+12
  \ Relocate dictionary
  dx xlatest imm) mov,
  dx ax add,
  pc
    dx 0 d) ax add,
    dx dx 0 d) mov,
    dx 0 d) 0 imm) cmp,
    ( pc ) abs>rel jnz,
  \ Allocate HERE
  dx sp 16 d) mov, \ SystemTable
  dx dx $3c d) mov, \ BootServices
  dx dx $20 d) mov, \ AllocatePages
  cx $100000 imm) mov, \ npages
  pc \ loop
    ax sp 0 d) mov,
    cx 1 imm) shr,
    cx push, \ AL+0
    dx push, \ AL+4
    sp 12 imm) sub, \ Ensure 16b alignment for call
    ax L2 imm) add, ax push, \ Buffer
    cx push, \ npages
    ax 2 imm) mov, ax push, \ Memtype
    ax 0 imm) mov, ax push, \ Type
    dx callr,
    sp 16 imm) add,
    sp 12 imm) add, \ Undo alignment adjust
    dx pop,
    cx pop, \ ECX=npages
    ax ax or,
    abs>rel jnz,
  dx pop, \ EDX=baseaddr
  bp dx L2 d) mov, \ EBP=SYSVARS
  bp SYSVARSSZ 8+ d) cx mov, \ AllocatePages
  cx sp 12 d) mov, \ SystemTable
  bp SYSVARSSZ d) cx mov,
  cx sp 8 d) mov, \ ImageHandle
  bp SYSVARSSZ 4+ d) cx mov,
  \ fill SYSVARS with zeroes
  di bp mov, ax ax xor, cx SYSVARSSZ 4/ imm) mov, rep, stos,
  \ EDX is *still* baseaddr
  bp oSYSDICT d) xlatest imm) mov, bp oSYSDICT d) dx add,
  dx pop, \ INPTR
  bp oINPTR d) dx mov,
  bp oRSORIGIN d) sp mov,
  ax bp mov, ax STACKSZ imm) add, \ ax=PSORIGIN
  bp oPSORIGIN d) ax mov,
  bp oPSSZ d) STACKSZ SYSVARSSZ - imm) mov,
  di bp mov,
  di oSYSALLOC imm) add,
  bp oCURALLOC d) di mov,
  di 0 d) ax mov, \ HERESTART=PSORIGIN
  lblbaseboot absjmp,

pc L2 pc>addr le!
livemode
