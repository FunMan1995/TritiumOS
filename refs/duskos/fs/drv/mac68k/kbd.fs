needs io/kbd
unit drv/mac68k/kbd

\ NOTE: mac68k has no mapping for ctrl! We map the command key to LControl
create map map< c,
  'A' 'S' 'D' 'F' 'H' 'G' 'Z' 'X' 'C' 'V' 0   'B' 'Q' 'W' 'E' 'R' \
  'Y' 'T' '1' '2' '3' '4' '6' '5' '=' '9' '7' '-' '8' '0' ']' 'O' \
  'U' '[' 'I' 'P' CR  'L' 'J' ''' 'K' ';' '\' ',' '/' 'N' 'M' '.' \
  9   SPC '`' BS  CR  ESC 0   $d2 $d0 $ae $d4 0   0   0   0   0

: map@ ( c -- nkc ) dup $40 < if map + c@ else 0 then ;

\ Even though mod keys have a key code, no event is triggered when they are
\ pressed. we need to update mod flags manually
create modmap map< , LControl LShift 0 LAlt
: updatemods ( modsfield kbd -- )
  0 rot 8 rshift 4 0 do ( kbd res modsfld )
    dup 1 and if swap modmap i 4* + @ or swap then 2/ loop
  drop swap to mods ;

: W>A0 $2047 w, ; immediate
: W>D0 $2007 w, ; immediate
: A0>W $2e08 w, ; immediate
: D0>W $2e00 w, ; immediate

: GetOSEvent ( mask a -- status ) W>A0 drop W>D0 [ $a031 w, ] D0>W ;

create event $10 allot0

: _?event ( kbd -- ?nkc event-type )
  $18 event GetOSEvent $ff and if drop NONE else
    event 14 + w@ swap updatemods
    event 4 + c@ map@ ?dup if
      event w@ 3 = if PRESS else RELEASE then else NONE then then ;

: newmackbd ( -- kbd )
  \ Initialize SysEvtMask and EventQueue
  $0018 $144 w! 0 $14a w! 0 $14c ! 0 $150 !
  ['] _?event newkbd ;
