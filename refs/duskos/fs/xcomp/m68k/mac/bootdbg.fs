needs asm/label asm/m68k xcomp/tools

\ Macros
: push, ( src -- ) RSP -[An]) swap move, ;
: pushw, ( src -- ) RSP -[An]) word) swap move, ;
: pop, ( src -- ) RSP [An]+) move, ;
: absbsr, abs>rel bsr, ;
: litlbl) 4- A4 swap [An,d]) ;

0 value lblemit
0 value lblprint
0 value lbldothex

xcompbegin
forward16 bsr, to L1

pc to L2 "DuskDBG" s,

pc to lblemit \ D0=char
  D0 pushw,
  $a883 wbe, \ DrawChar
  rts,

pc to lblprint \ A0=str
  A0 push,
  $a884 wbe, \ DrawString
  rts,

,"0123456789abcdef"
pc to lbldothex \ D1=n
  A3 -16 [PC,d]) lea,
  D6 7 moveq,
  pc
    D1 4 rol#,
    D0 D1 move,
    \ D0 $f andi,
    $0280 wbe, 0 wbe, $f wbe,
    D0 A3 [An]) D0 Xn]) byte) move,
    D0 pushw, $a883 wbe,
    D6 swap abs>rel dbra,
  rts,

L1 forward!
A4 pop, \ A4=literal zone
A6 $22000 imm) move, \ A6=argument scratch area
\ We put A5 $100 bytes further than heap start for Quick Draw global vars.
A5 A6 $100 [An,d]) lea,
A5 $100 [An,d]) pea,
$a86e wbe, \ InitGraf
A5 $104 [An,d]) pea,
$a86f wbe, \ OpenPort
$00100010 imm) push, \ 16,16
$a893 wbe, \ MoveTo
A6 [An]) 0 imm) move, \ topleft=0,0
A6 4 [An,d]) $00800080 imm) move, \ bottomright=128,128
A6 push, \ rect
$a8a3 wbe, \ EraseRect

A0 L2 litlbl) lea,
lblprint absbsr,

D1 42 imm) move,
lbldothex absbsr,

A6 12 [An,d]) 0 imm) move,        \ ioCompletion
A6 22 [An,d]) word) 1 imm) move,  \ ioVRefNum (1=floppy, apparently...)
A6 24 [An,d]) word) -5 imm) move, \ ioRefNum (-5=.Sony)
A6 44 [An,d]) word) 1 imm) move,  \ ioPosMode (1=fsFromStart)
A6 26 [An,d]) word) 7 imm) move,  \ csCode=eject
A0 A6 move,
$a004 wbe, \ Control

0 bra,
xcompend
