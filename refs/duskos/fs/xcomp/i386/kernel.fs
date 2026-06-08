\ This is the i386 Dusk kernel. It is called when the bootloader has finished
\ loading this binary as well as the Forth boot code following it in memory.
\ We're in protected mode and all segments have been initialized.
\ ESP=RSP ESI=PSP EDX=S EBX=A EAX=W. They begin uninitialized.
\ EBP is reserve as the "SYSVARS pointer". If you use EBP at some point, it must
\ be preserved.

\ REQUIREMENTS
\ This unit is designed to be loaded under these conditions:
\ 1. Have the "binstart" value set.
\ 2. Somewhere to make lblcoldboot jump to afterwards.
\ 3. That coldboot routine must make EBP point to an initialized SYSVARS.
needs fs/sh asm/x86 xcomp/tools

32bmode
0 value lblcoldboot
0 value lblbaseboot

: sysv) bp swap d) ;
f<< xcomp/x86/kernelm.fs

f"xcomp/x86/kernel.fs" const x86file \ has to be opened outside comp zone
\ Let's go!
kernelbegin
forward jmp, to lblcoldboot

$10 xconst ARCH

xcode litn
  \ si 4 imm) sub, si 0 d) ax mov, ax XX imm) mov,
  $89fcc683 imm) dwrite, $b806 imm) wwrite,
  ax dwrite, xdrop, ret,

x86file interpretstream

xcode main
  sp bp oRSORIGIN d) mov,
pc
  wcall, run1
  ( pc ) abs>rel jmp,

pc to lblbaseboot
  cld, \ all i386 code assumes cleared D flag.
  si oPSORIGIN sysv) mov,
  wjmp, main
\ kernelend is called by target-specific kernel unit
