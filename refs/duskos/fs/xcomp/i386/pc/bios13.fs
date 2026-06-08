\ Tool to debug PC INT13H. see doc/hw/i386/pc/bios13
needs asm/x86
realmode

$7c00 to binstart
0 value lblstart
0 value lblmsg
0 value lblprinthex
0 value lblkeypress
0 value lblloop

here# to org $aa55 here# $1fe + w!
forward8 jmp, to lblstart
pc \ list of messages, 4 chars each
,"DL  StopGeo SectErr Data"

pc to lblmsg \ si=idx preserves cx/dx
  cx push, dx push,
  si 2 imm) shl, ( x4 ) si swap ( pc ) imm) add, cl 4 imm) mov,
  ah $0e imm) mov, bx $0007 imm) mov,
  pc al 0 si+) mov, $10 int, si inc, cl dec, ( pc ) abs>rel jnz,
  dx pop, cx pop, ret,

pc to lblprinthex \ al=num preserves cx/dx
  cx push, dx push, ah $0e imm) mov, bx $0007 imm) mov,
  ax push, al 4 imm) shr,
  al '0' imm) add, al ':' imm) cmp,
  forward8 jc, al 7 imm) add, forward! $10 int,
  ax pop, al $0f imm) and,
  al '0' imm) add, al ':' imm) cmp,
  forward8 jc, al 7 imm) add, forward! $10 int,
  dx pop, cx pop, ret,

pc to lblkeypress
  ah ah xor, $16 int, \ read key press
  al 's' imm) cmp, forward8 jnz,
    cl inc,
  forward!
  al 'h' imm) cmp, forward8 jnz,
    cl 1 imm) mov, dh inc,
  forward!
  al 'c' imm) cmp, forward8 jnz,
    cl 1 imm) mov, dh dh xor, ch inc,
  forward!
  ret,

lblstart forward!
cli, cld, ax ax xor, es ax mov, ds ax mov, ss ax mov, sp $7c00 imm) mov, sti,
ax $0003 imm) mov, $10 int, \ video mode 80x25
si 0 imm) mov, lblmsg abs>rel call,
al dl mov, lblprinthex abs>rel call,

ah ah xor, $13 int, \ reset

\ If you want a hardcoded drive parameters, uncomment code below
\ dl $80 imm) mov,

si 2 imm) mov, lblmsg abs>rel call,
dx push, ah 8 imm) mov, $13 int, \ dh=numheads-1 cl&3f=sec per trk
al dh mov, lblprinthex abs>rel call,
al cl mov, lblprinthex abs>rel call,
dx pop,
\ int13h AH=08 can change ES! reset it
ax ax xor, es ax mov,

\ We begin at cylinder 0, head 0, sector 1
cx $0001 imm) mov, \ ch=cyl cl=sec
dh dh xor, \ dh=head
pc to lblloop
  si 3 imm) mov, lblmsg abs>rel call,
  al ch mov, lblprinthex abs>rel call,
  al cl mov, lblprinthex abs>rel call,
  al dh mov, lblprinthex abs>rel call,
  al dl mov, lblprinthex abs>rel call,
  bx $8000 imm) mov,
  ax $0201 imm) mov,  \ ah=read cmd al=read 1 sector
  $13 int, forward8 jnc,
    \ error
    si 4 imm) mov, lblmsg abs>rel call,
    lblprinthex abs>rel call,
    al ah mov, lblprinthex abs>rel call,
    lblkeypress abs>rel call,
    lblloop abs>rel jmp,
  forward!
  \ no error, print
  si 5 imm) mov, lblmsg abs>rel call,
  di 8 imm) mov, si $8000 imm) mov,
  pc al 0 si+) mov, lblprinthex abs>rel call, si inc, di dec, abs>rel jnz,
  lblkeypress abs>rel call,
  lblloop abs>rel jmp,
resetasm livemode
