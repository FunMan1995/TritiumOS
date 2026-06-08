needs asm/label asm/m68k xcomp/tools xcomp/m68k/cpusel

\ xcomp/m68k/kernel must have been built before loading this unit
$200 const SECSZ
kernel[] nip SECSZ /+ const KERNELSEC \ size of kernel in sectors
$7fff const PAYLOADOFFSET \ where we put the payload relative to memory start

: Read, $a002 wbe, ;
xcompbegin

\ we are at offset 8a and we want to fetch rsvdsec low byte at offset 0e
D7 clr, D7 byte) $7e neg [PC,d]) move, \ D7=rsvdsec
A0 $22000 imm) move,
\ We put A5 at heap start +$100 bytes
A5 A0 $100 [An,d]) lea,
A0 12 [An,d]) 0 imm) move,        \ ioCompletion
A0 22 [An,d]) word) 1 imm) move,  \ ioVRefNum (1=floppy, apparently...)
A0 24 [An,d]) word) -5 imm) move, \ ioRefNum (-5=.Sony)
A3 A5 $400 [An,d]) lea,           \ A3=kernel start
A0 32 [An,d]) A3 move,            \ ioBuffer
A0 36 [An,d]) KERNELSEC SECSZ * imm) move, \ ioReqCount
A0 44 [An,d]) word) 1 imm) move,  \ ioPosMode (1=fsFromStart)
A0 46 [An,d]) $400 imm) move,     \ ioPosOffset
Read,
\ Now read the payload
A0 $22000 imm) move,
A4 A5 PAYLOADOFFSET [An,d]) lea,  \ A4=payload
A0 32 [An,d]) A4 move,            \ ioBuffer
D7 8 lsl#, D7 1 lsl#,
A0 36 [An,d]) D7 move, \ ioReqCount
A0 46 [An,d]) KERNELSEC SECSZ * $400 + imm) move, \ ioPosOffset
Read,
clearcache,
\ We jump to kernel with A5 pointing to a small scratch area. It's the future
\ SYSVARS
A3 [An]) jmp,
xcompend
