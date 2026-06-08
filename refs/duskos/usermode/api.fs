\ SYSVARS+0=BINSTART, placed by the kernel
SYSVARS @ const BINSTART
8 1024 * const BOOTZONESZ \ in sync with common.c

: mul 4* ARCH $40 = if 2* then ;
BINSTART BOOTZONESZ + 8 mul - const INTEROPZONE

INTEROPZONE const funcs
INTEROPZONE 4 mul + const argc
INTEROPZONE 5 mul + const argv
INTEROPZONE 7 mul + const memsz

BINSTART memsz @ + HEREMAX !

: syscallback ( id -- ) code i) A>) @, ['] syscallA bbr, ;

3 syscallback dbgPrint
4 syscallback sysexit
\ We loop forever after sysexit because some flavors (SDL) don't exit
\ immediately, but at the next event loop.
: bye 0 sysexit begin again ;
: byefail 1 sysexit begin again ;

current ABORTPTR !
5 syscallback sleep
1 syscallback (rtype)
' (rtype) console!
2 syscallback ?getnkc ( nkc event-type )
2 syscallback (?key) ( ?c f )

6 syscallback (ticks)

16 syscallback fdwrite ( a u fd -- n )
17 syscallback fdread ( a u fd -- n )
18 syscallback fdopen ( strpath write? -- ?size fd-or-0 )
19 syscallback fdclose ( fd -- )
20 syscallback fdseek ( n fd -- )
21 syscallback (now) ( -- time )

10 syscallback bootdrv@
10 syscallback gridput
9 syscallback gridinfo
10 syscallback screencb
9 syscallback screeninfo
11 syscallback mousecb
14 syscallback clipcb
