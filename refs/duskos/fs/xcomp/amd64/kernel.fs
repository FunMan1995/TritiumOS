\ Requirements: SYSVARSOFFSET
needs fs/sh asm/x86 xcomp/tools
64bmode

$1000 const STACKSZ
0 value lblcoldboot
0 value lblbaseboot
0 value lblinitialRSP \ must be set during lblcoldboot

: sysv) ( off -- ) SYSVARSOFFSET + bp swap d) ;
f<< xcomp/x86/kernelm.fs

f"xcomp/x86/kernel.fs" const x86file \ has to be opened outside comp zone
\ Let's go!
kernelbegin
forward jmp, to lblcoldboot
pc to lblinitialRSP 8 allot0

$40 xconst ARCH

xcode litn
  \ si 4 imm) sub, si mem) 0 d) ax mov, ax XX imm) mov,
  $89fcc683 imm) dwrite, $b8003544 imm) dwrite,
  ax dwrite, xdrop, ret,

x86file interpretstream

xcode main
  rex.r r15 oRSORIGIN sysv) mov,
  rex.w sp bp lblinitialRSP d) mov,
pc
  wcall, run1
  ( pc ) absjmp,

pc to lblbaseboot
  cld, \ all i386 code assumes cleared D flag.
  si oPSORIGIN sysv) mov,
  wjmp, main
