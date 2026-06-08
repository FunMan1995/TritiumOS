needs lib/type lib/ival hal/opq asm/x86
unit drv/pc/ioport

\ all compiler words always have port number in S (EDX) and target W (EAX)
( port -- n )
: AX>DX dx ax mov, ;
: p@, ax dx in, ;
: pw@, ax ax xor, ax word) dx in, ;
: pc@, ax ax xor, ax byte) dx in, ;
code p@ AX>DX p@, exit,
code pw@ AX>DX pw@, exit,
code pc@ AX>DX pc@, exit,

( n port -- )
: p!, ax dx out, ;
: pw!, ax word) dx out, ;
: pc!, ax byte) dx out, ;
code p! AX>DX drop, p!, drop, exit,
code pw! AX>DX drop, pw!, drop, exit,
code pc! AX>DX drop, pc!, drop, exit,

\ We rely on the fact that ivalue's "addr," implementation fetches is value in
\ S (EDX), exactly where we want it.
:~ 0 swap word dup NEXTWORD ! newint addtype ;
:> (dir? if pc!, else pc@, then ; 1 ~ port8
:> (dir? if pw!, else pw@, then ; 2 ~ port16
:> (dir? if p!, else p@, then ; 4 ~ port32

: ioportb 1 n,@ port8 ivalue ;
: ioportw 1 n,@ port16 ivalue ;
: ioport 1 n,@ port32 ivalue ;
