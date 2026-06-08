unit drv/efi

: ?err ?dup if ."EFI error " .x abort then ;
: efiexec# compile efiexec compile ?err ; immediate
: [q+] run1 run1 q* + q+n, ; immediate
: qsz 1 q* ;
: q/ qsz / ;
: aq@ absaddr q@ ;
: [q+@] [compile] [q+] compile q@ ; immediate
: noq 0 drop ;
: .x8 dup qrshift32 .x .x ;
: qvariable create 8 allot0 ;
: ?reladdr reladdr dup qrshift32 if
             drop 0 else dup 31 rshift if drop 0 else 1 then then ;

: efiguid create 16 0 do word c@+ parsehex not ?err c, loop ;

$200 const TMPBUFSZ
create tmpbuf TMPBUFSZ allot0
: efistr ( str -- char16 )
  tmpbuf swap c@+ 0 do ( dst src )
    c@+ rot c!+ 1+ swap loop drop 0 swap c! tmpbuf ;
: ConIn SystemTable [q+@] $18 3 ;
: ConOut SystemTable [q+@] $18 5 ;
: RuntimeServices SystemTable [q+@] $18 8 ;
: BootServices SystemTable [q+@] $18 9 ;

:~ argstart 0 arg1! ConOut arg0k! q@ efiexec drop
   argstart 0 arg1! ConIn arg0k! q@ efiexec drop ; ~

: OutputString ( str64 -- )
  argstart arg1! ConOut arg0k! [q+@] 0 1 efiexec drop ;

create _ 4 allot0
create _nl map< c, CR 0 LF 0 0 0
: efirtype ( a u -- )
  0 do
    c@+ dup LF = if drop _nl else _ c! _ then
    absaddr OutputString loop drop ;

create _ 16 allot0
: ReadKeyStroke ( -- struct-or-0 )
  argstart _ absaddr arg1!
  ConIn arg0k! [q+@] 0 1 efiexec
  not if _ else 0 then ;

: Stall ( us -- ) argstart arg0! BootServices [q+@] $18 28 efiexec# ;
create _null 0 ,
: reboot ( -- ) argstart 0 arg0! 0 arg1! 0 arg2! _null absaddr arg3!
                RuntimeServices [q+@] $18 10 efiexec ;

qvariable sz
: LocateProtocol ( guidaddr -- ?a u-or-0 )
  TMPBUFSZ sz ! argstart absaddr arg1! ( )
  2 arg0! sz absaddr arg3! tmpbuf absaddr arg4!
  BootServices [q+@] $18 19 ( LocateHandle ) efiexec
  if 0 else sz @ dup if tmpbuf swap then then ;

create interface 8 allot
: HandleProtocol ( guidaddr a64 -- res64 )
  argstart arg0! absaddr arg1! interface absaddr arg2!
  BootServices [q+@] $18 16 efiexec ;
: HandleProtocol# HandleProtocol ?err interface aq@ ;

: DisableWatchdog ( -- )
  argstart 0 arg0! 0 arg1! 0 arg2! 0 arg3!
  BootServices [q+@] $18 29 efiexec ?err ;
