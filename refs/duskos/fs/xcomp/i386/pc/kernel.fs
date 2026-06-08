needs asm/x86
\ Macros
0 value lblintnoop
0 value lblidt
0 value lblpicmasks
: setpicmasks, \ bl=master bh=slave. destroys ax
  al bl mov, al $21 imm) out, al bh mov, al $a1 imm) out, ;
: restorepicmasks,  bx lblpicmasks abs) mov, setpicmasks, ; \ destroys ax bx
$500 to binstart
$8000 const IDT
$8800 const HERESTART
$1000 const STACKSZ
$7800 const RSORIGIN
$7800 const BIOSBUF
$7e00 const SYSVARS
$7d70 const I13HPCKT
$80000 const PSORIGIN
f<< /xcomp/i386/kernel.fs

pc to lblintnoop iret,

\ To avoid lockups, we map all PIC IRQs on boot to words that acknowledge those
\ IRQs. The rest of PIC initialization is done in /drv/pc/pic.fs
xcode piceoi
  ax push, al $20 imm) mov, al $20 imm) out, ax pop, iret,

xcode piceoi2 \ slave IRQs have to EOI both master and slave.
  ax push, al $20 imm) mov, al $a0 imm) out, al $20 imm) out, ax pop, iret,

pc to lblidt $100 8 * 1- wle, IDT le,
IDT xconst IDT

\ b0=master mask b1=slave mask
pc to lblpicmasks 0 le,
lblpicmasks xconst PICMASKS

\ For the PC platform, we need to fiddle with real mode/protected mode so that
\ we can use int13hlba (or perhaps int13hchs) to properly boot ourselves to
\ init.fs

pc to L1 \ back to protected mode!
  ax $10 imm) mov, ds ax mov, ss ax mov, es ax mov, gs ax mov, fs ax mov,
  lblidt abs) lidt, sti,
  restorepicmasks,
  ax BIOSBUF imm) mov,
  si pop,
  ret,

pc to L2 $3ff wle, 0 le, \ real mode IVT

realmode
pc to L3
  ax ax xor, es ax mov, ds ax mov, ss ax mov, L2 abs) lidt, sti,
  ax di mov, ( cmd ) ah al mov, al al xor,
  \ if bit 6 of ah is set, then lba else chs
  ah $40 imm) test,
  forward8 jz,      \ if zero, go to chs
    \ lba
    si I13HPCKT imm) mov,
    $13 int,
  forward8 jmp,     \ go to done
  swap forward!
    \ chs
    al 1 imm) mov,
    bx BIOSBUF imm) mov, $13 int,
  forward!
  \ we've done what we came for, let's go back to 32bit
  cli, ax cr0 mov, ax 1 imm) or, cr0 ax mov,
  $08 L1 jmpfar,

pc to L4 \ segment with ffff limits
  ax $20 imm) mov, ds ax mov, ss ax mov, es ax mov, gs ax mov, fs ax mov,
  ax cr0 mov, ax $fffffffe imm) and, cr0 ax mov,
  0 L3 jmpfar,
32bmode

xcode int13hlba ( drv lbasec cmd -- buf )
  di ax mov, \ cmd
  bx si 0 d) mov, \ lbasec
  I13HPCKT 8 + abs) bx mov, \ save to Disk Address Packet offset 8
  dl si 4 d) mov, \ drv
  si 8 imm) add, \ keep 1 PS slot for "buf"
  \ Allow IRQ6 for floppy, IRQ2 for cascade, IRQ 14-15 for ATA
  bx $3fbb imm) mov, setpicmasks,
  cli, si push, si si xor, $18 L4 jmpfar,

xcode int13hchs ( drv head cyl sec cmd -- buf )
  di ax mov, \ cmd
  cl si 0 d) mov, \ sec
  ch si 4 d) mov, \ cyl
  dh si 8 d) mov, \ head
  dl si 12 d) mov, \ drv
  si 16 imm) add, \ keep 1 PS slot for "buf"
  \ Allow IRQ6 for floppy, IRQ2 for cascade, IRQ 14-15 for ATA
  bx $3fbb imm) mov, setpicmasks,
  cli, si push, $18 L4 jmpfar,

pc to L1 \ back to protected mode!
  dx $10 imm) mov, ds dx mov, ss dx mov, es dx mov, gs dx mov, fs dx mov,
  \ we still need to push bx and ax, PS slot is already reserved
  si 0 d) bx mov, ( -- bx ax )
  lblidt abs) lidt, sti,
  ax push, restorepicmasks, ax pop,
  ret,

pc to L3 realmode
  dx dx xor, es dx mov, ds dx mov, ss dx mov, L2 abs) lidt, sti,
  \ VESA calls need a 512b buffer that can be accessed by real mode.
  di $4e00 imm) mov,
  dx cx mov, $10 int,
  \ we've done what we came for, let's go back to 32bit
  cli, dx cr0 mov, dx 1 imm) or, cr0 dx mov,
  $08 L1 jmpfar,

pc to L4 \ segment with ffff limits
  dx $20 imm) mov, ds dx mov, ss dx mov, es dx mov, gs dx mov, fs dx mov,
  dx cr0 mov, dx $fffffffe imm) and, cr0 dx mov,
  0 L3 jmpfar,
32bmode

xcode int10h ( cx/dx bx ax -- bx ax )
  ax push,
  bx $ffff imm) mov, setpicmasks, \ no IRQ at all during call
  ax pop,
  bx si 0 d) mov, xnip,
  cx si 0 d) mov, \ keep this PS slot reserved
  cli, $18 L4 jmpfar,

pc to L1 0 le,
lblcoldboot forward!
cld, ax ax xor,
bp I13HPCKT imm) mov,
bp 0 d) $10010 imm) mov,  \ 16 byte packet size + transfer 1 sector
bp 4 d) BIOSBUF imm) mov, \ 16 bit offset to transfer buffer (real mode seg 0)
\ offset 8 is written on each int13hlba call
bp 12 d) ax mov,          \ Highest 16 bits of 48 bit LBA are zero
bp SYSVARS imm) mov,
cx SYSVARSSZ 4/ imm) mov, di bp mov, rep, stos,
ax SYSVARS oSYSALLOC + imm) mov,
bp oCURALLOC d) ax mov,
ax 0 d) HERESTART imm) mov,
ax L1 abs) mov,
bp oINPTR d) ax mov,
bp oSYSDICT d) xlatest imm) mov,
bp oPSORIGIN d) PSORIGIN imm) mov,
bp oPSSZ d) STACKSZ imm) mov,
bp oRSORIGIN d) RSORIGIN imm) mov,
di IDT imm) mov, ax lblintnoop imm) mov, stosw, ax $08 imm) mov, stosw,
ax $8e00 imm) mov, stosw, ax lblintnoop 16 rshift imm) mov, stosw,
cx $1fe imm) mov, si IDT imm) mov, rep, movs,
si IDT 8 8 * + imm) mov, ax xwordlbl piceoi imm) mov, cx 8 imm) mov,
pc si 0 d) word) ax mov, si 8 imm) add, cx dec, ( pc ) abs>rel jnz,
si IDT $70 8 * + imm) mov, ax xwordlbl piceoi2 imm) mov, cx 8 imm) mov,
pc si 0 d) word) ax mov, si 8 imm) add, cx dec, ( pc ) abs>rel jnz,
lblidt abs) lidt, sti,
lblbaseboot absjmp,

pc L1 pc>addr le!
kernelend livemode
