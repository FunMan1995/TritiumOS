needs fs/sh asm/x86 xcomp/tools

0 to binstart
$20000 const SYSVARSOFFSET \ 128K, enough place for kernel+payload
$1000 const STACKSZ
0 value lblspring
0 value lblreloc
0 value lbl1ststage

\ RSP indexes off by 1 because of return address on stack
: argN!, ( n -- ) rex.w sp swap 1+ 8* d) ax mov, ;
f<< /xcomp/amd64/kernel.fs

pc to lblspring ax pop, ret,
pc to lblreloc lblspring abscall, pc le, \ reloc canary

xcode q@ rex.w ax ax 0 d) mov, ret,
xcode q+n, $0548 imm) wwrite, ax dwrite, xdrop, ret,
xcode absaddr ax ?bp+, ret,
xcode reladdr ax ?bp-, ret,
xcode qrshift32 rex.w ax 32 imm) shr, ret,
xcode q* ax 3 imm) shl, ret,
xcode qmove ( dst u src64 -- )
  si push,
  cx si 0 ?d+bp) mov,
  di si 4 ?d+bp) mov,
  di ?bp+,
  rex.w si ax mov,
  rep, movsb,
  si pop,
  xnip, xnip, xdrop,
  ret,

xcode inccb \ void *notify(EFI_EVENT event, void *ctx)
  dx 0 d) inc, ret,

xcode argstart ( -- )
  bx pop,
  rex.w di sp mov,
  rex.w sp $f inv imm) and, \ align to 16 bytes
  di push, \ Stack is 16-b *disaligned* by exactly 8b
  rex.w sp 13 8* imm) sub, \ 16b aligned
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
  rex.w cx sp 0 d) mov,
  rex.w dx sp 8 d) mov,
  rex.w rex.r r8 sp 16 d) mov,
  rex.w rex.r r9 sp 24 d) mov,
  ax callr,
  rex.w sp 13 8* imm) add,
  sp pop,
  di jmpr,

\ system 64-bit values store: +0=SystemTable +8=ImageHandle
pc to L1 2 8* allot0 \ initial 64-bit value for RSP
xcode SystemTable
  xdup, rex.w ax bp L1 d) mov, ret,

xcode ImageHandle
  xdup, rex.w ax bp L1 8 + d) mov, ret,

pc to L3 16 allot0 \ +0=Alloc addr +8=AllocatePages
xcode AllocatedPages
  xdup,
  di L3 imm) mov,
  ax di 8 ?d+bp) mov,
  ret,

pc to lbl1ststage \ RDX=SystemTable RCX=ImageHandle RAX=baseaddr
  \ ESP starts disaligned by 8, 3 pushes align it.
  dx push, cx push, ax push,
  rex.w dx dx $60 d) mov, \ EDX=BootServices
  rex.w di dx $28 d) mov, \ EDI=AllocatePages, callee-saved
  si $80000 imm) mov, \ npages. ESI is callee-saved
  bx ax mov, bx L3 imm) add, \ Buffer. calee-saved
  rex.w sp 4 8* imm) sub,
  pc \ loop
    cx cx xor, \ Alloc type: any address
    dx 2 imm) mov, \ Memtype=EfiLoaderData
    rex.r r8 si mov, \ npages
    si 1 imm) shr,
    rex.r r9 bx mov, \ Buffer
    rex.b rex.r r9 8 d) r8 mov,
    di callr,
    rex.w ax ax or,
    abs>rel jnz,
  rex.w sp 4 8* imm) add,
  ax pop, \ RAX=baseaddr
  rex.w si ax mov,
  rex.w di ax L3 d) mov,
  cx SYSVARSOFFSET 4/ imm) mov,
  rep, movs,
  rex.w ax ax L3 d) mov,
  cx pop, dx pop,
  ax jmpr,

pc to L2 0 le, \ start value for INPTR
lblcoldboot forward! \ ESP starts disaligned by 8
  lblreloc abscall,
  di ax 0 d) mov,
  rex.w ax di sub,
  ax L3 8 + d) 0 imm) cmp, \ AllocatedPages
  lbl1ststage abs>rel jz,
  bp push, \ ESP is 16b-aligned
  rex.w bp ax mov, \ RBP=base addr
  rex.w bp lblinitialRSP d) sp mov,
  rex.w bp L1 d) dx mov, \ SystemTable
  rex.w bp L1 8 + d) cx mov, \ ImageHandle
  \ fill SYSVARS with zeroes
  rex.w di bp mov,
  rex.w di SYSVARSOFFSET imm) add,
  ax ax xor,
  cx SYSVARSSZ 4/ imm) mov, rep, stos,
  oPSSZ sysv) STACKSZ SYSVARSSZ - imm) mov,
  oPSORIGIN sysv) SYSVARSOFFSET STACKSZ + imm) mov,
  oRSORIGIN sysv) SYSVARSOFFSET STACKSZ 2* + imm) mov,
  oSYSALLOC sysv) SYSVARSOFFSET STACKSZ 2* + imm) mov, \ HERE is after stacks
  ax bp L2 d) mov,
  oINPTR sysv) ax mov,
  oCURALLOC sysv) SYSVARSOFFSET oSYSALLOC + imm) mov,
  oSYSDICT sysv) xlatest imm) mov,
  lblbaseboot absjmp,

pc L2 pc>addr le!
livemode
