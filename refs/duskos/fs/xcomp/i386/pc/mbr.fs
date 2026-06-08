\ x86 bootloader
needs asm/x86 xcomp/tools

$7c3e to binstart \ we begin this binary right after the BPB, at $7c3e
0 value lblstart
0 value lblerror
0 value lblspit
0 value lblpayload
0 value lblreadlba
0 value lblreadchs
0 value lblread
realmode

xcompbegin
cli, cld, ax ax xor, es ax mov, ds ax mov, ss ax mov, sp $7c00 imm) mov,
forward jmp, to lblstart
pc to lblspit \ al=char
  \ the BIOS is supposed to preserve unused registers, but we never know...
  bx push, cx push, dx push, si push, di push,
  ah $0e imm) mov, bx 7 imm) mov, $10 int,
  di pop, si pop, dx pop, cx pop, bx pop, ret,

pc ,"MBR failure"
pc to lblerror
  si swap ( pc ) imm) mov, cl 11 imm) mov,
  pc al 0 si+) mov, lblspit abs>rel call, si inc, cl dec, ( pc ) abs>rel jnz,
  0 jmp,

pc to lblpayload 32bmode
  \ initialize all segments
  ax $10 imm) mov, ds ax mov, ss ax mov, es ax mov, gs ax mov, fs ax mov,
  \ Jump to payload (interrupts disabled)
  $08 $500 jmpfar,
realmode

pc to lblreadlba
  si $7d70 imm) mov, \ si=offset of disk address packet within ds (should be 0)

  \ read 59 sectors, starting from sector 2 from boot disk to the
  \ memory at address $500. DL (drive number) is set by BIOS.
  al '.' imm) mov, lblspit abs>rel call,
  ah $42 imm) mov,  \ ah=read with LBA cmd
  $13 int, lblerror abs>rel jc,
  al '.' imm) mov, lblspit abs>rel call,
  ret,

pc to lblreadchs
  si $7c18 abs) mov, \ si=sec per trk
  si dec,            \ we begin at trk 2
  di 59 imm) mov,    \ we always copy 59 sectors (to fill $500-$7c00)
  bx $500 imm) mov,  \ bx=dest addr
  cx $0002 imm) mov, \ ch=cyl cl=sec
  dh dh xor,         \ dh=head

  \ read BPB_RsvdSecCnt-1 sectors, starting from sector 2 from boot disk in
  \ memory at address $500 DL is set by BIOS. If we're on floppy, this will
  \ break track boundaries so we need to proceed in multiple calls.
  pc
    al '.' imm) mov, lblspit abs>rel call,
    ax $0201 imm) mov,  \ ah=read cmd al=read 1 sector
    $13 int, lblerror abs>rel jc,
    si dec, forward8 jnz,
      al 'H' imm) mov, lblspit abs>rel call,
      si $7c18 abs) mov, cl cl xor,
      dh inc, dh $7c1a abs) cmp, forward8 jnz,
        al 'C' imm) mov, lblspit abs>rel call,
        dh dh xor, ch inc,
      forward!
    forward!
    bx $200 imm) add,
    cl inc,
    di dec, abs>rel jnz,
  ret,

pc to lblread
  ah ah xor, $13 int, \ reset
  \ if numheads <= 2 then load with CHS
  $7c1a abs) byte) 3 imm) cmp,
  lblreadchs abs>rel jc,
  lblreadlba abs>rel jmp,

lblstart forward!
$7c24 abs) dl mov,
$7d80 abs) lgdt, sti,

lblread abs>rel call,

\ Sometimes, you need a pause for debugging...
\ al 'D' imm) mov, lblspit abs>rel call,
\ ah ah xor, $16 int,

ax $0003 imm) mov, $10 int, \ video mode 80x25
cli, ax cr0 mov, ax 1 imm) or, cr0 ax mov,
$08 lblpayload jmpfar,

\ Partition table entry at $7d60 (invalid, all zeroes)

$7d70 tgtallot
\ Disk Address Packet Structure (from wiki.osdev.org)
$10 c,    \ size of packet
0 c,      \ always 0
59 wle,   \ number of sectors to transfer (59 to fill $500 to $7bff inclusive)
$500 wle, \ +4: offset of transfer buffer within segment
0 wle,    \ segment of transfer buffer
1 wle,    \ +8: lowest 16 bits of LBA to read from
0 wle,    \ bits 31-16 of LBA to read from
0 wle,    \ bits 47-32 of LBA to read from

\ Global Descriptor Table at $7d80
\ Inspired by GNU grub at grub-core/kern/i386/realmode.S
$7d80 tgtallot
$27 wle, $7d88 le, $7d88 tgtallot
8 allot0 \ first entry is always null
\ Code segment. base=0, limit=ffffff*4kb, present, 32bit, execute/read, DPL=0
$ffff wle, 0 wle, 0 c, $9a c, $cf c, 0 c,
\ Data segment. base=0, limit=ffffff*4kb, present, 32bit, read/write, DPL=0
$ffff wle, 0 wle, 0 c, $92 c, $cf c, 0 c,
\ Code segment. base=0, limit=ffff*1b, present, 32bit, execute/read, DPL=0
$ffff wle, 0 wle, 0 c, $9a c, 0 c, 0 c,
\ Data segment. base=0, limit=ffff*1b, present, 32bit, read/write, DPL=0
$ffff wle, 0 wle, 0 c, $92 c, 0 c, 0 c,
xcompend livemode
