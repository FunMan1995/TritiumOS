needs lib/struct io/stream
unit io/kbd

enum NONE PRESS RELEASE BOTH

consts
  $80000000 Passthrough \
  $00ff0000 ModMask \
  $000000ff CodeMask \
  $00800000 RGUI \
  $00400000 LGUI \
  $00c00000 GUI \
  $00200000 RAlt \
  $00100000 LAlt \
  $00300000 Alt \
  $00080000 RControl \
  $00040000 LControl \
  $000c0000 Control \
  $00020000 RShift \
  $00010000 LShift \
  $00030000 Shift

consts
  $81 F1 $82 F2 $83 F3 $84 F4 $85 F5 $86 F6 $87 F7 \
  $88 F8 $89 F9 $8a F10 $8b F11 $8c F12 \
  $a0 PrintScreen $a1 ScrollLock $a2 Break $a3 Insert \
  $a4 Delete $a5 Home $a6 End $a7 PageUp $a8 PageDown \
  $a9 ArrowLeft $aa ArrowRight $ab ArrowUp $ac ArrowDown \
  $ad Popup $ae CapsLock \
  $b0 KPNumlock $b1 KPEnter \
  $b2 KP/ $b3 KP* $b4 KP- $b5 KP+ \
  $c0 KP0 $c1 KP1 $c2 KP2 $c3 KP3 $c4 KP4 $c5 KP5 \
  $c6 KP6 $c7 KP7 $c8 KP8 $c9 KP9 $ca KP.

struct Keyboard {
  uint layout mods pressed ;
  xt ?event ;
}

\ We duplicate data/kbdl/us here because we want to be able to use io/kbd in
\ systems that don't have a functional FS.
create USLayout map< c,
  0   0   0   0   0   0   0   0   $08 $09 0   0   0   $0d 0   0 \
  0   0   0   0   0   0   0   0   0   0   0   $1b 0   0   0   0 \
  $20 0   0   0   0   0   0   ''' 0   0   0   0   ',' '-' '.' '/' \
  '0' '1' '2' '3' '4' '5' '6' '7' '8' '9' 0   ';' 0   '=' 0   0 \
  0   'a' 'b' 'c' 'd' 'e' 'f' 'g' 'h' 'i' 'j' 'k' 'l' 'm' 'n' 'o' \
  'p' 'q' 'r' 's' 't' 'u' 'v' 'w' 'x' 'y' 'z' '[' '\' ']' 0   0 \
  '`' 0   0   0   0   0   0   0   0   0   0   0   0   0   0   0 \
  0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0 \
\ Shifted
  0   0   0   0   0   0   0   0   $08 $09 0   0   0   $0d 0   0 \
  0   0   0   0   0   0   0   0   0   0   0   $1b 0   0   0   0 \
  $20 0   0   0   0   0   0   '"' 0   0   0   0   '<' '_' '>' '?' \
  ')' '!' '@' '#' '$' '%' '^' '&' '*' '(' 0   ':' 0   '+' 0   0 \
  0   'A' 'B' 'C' 'D' 'E' 'F' 'G' 'H' 'I' 'J' 'K' 'L' 'M' 'N' 'O' \
  'P' 'Q' 'R' 'S' 'T' 'U' 'V' 'W' 'X' 'Y' 'Z' '{' '|' '}' 0   0 \
  '~' 0   0   0   0   0   0   0   0   0   0   0   0   0   0   0 \
  0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0
  $100 allot0

: loadkbdl ( stream -- layout ) $200 allot@ tuck $200 oover read# close ;

: pressed' ( nkc kbd -- a-or-0 )
  >A CodeMask and A> offsetof pressed + tuck 4 cidx if + else drop 0 then ;
: pressed? ( nkc kbd -- f ) pressed' bool ;
: press ( nkc kbd -- )
  2dup pressed? if 2drop else
    0 swap pressed' ?dup if c! else drop then then ;
: depress ( nkc kbd -- ) pressed' ?dup if 0 swap c! then ;

: ?nkc>mods ( nkc -- nkc )
  dup $d0 - dup $f > if drop else ( nkc modidx )
    nip 16 + 1 swap lshift then ;

: ?nkc ( kbd -- ?nkc f )
  dup ?event ?dup if ( kbd nkc type )
    over Passthrough and if
      PRESS and if nip 1 else 2drop 0 then exit then
    over ?nkc>mods ModMask and ?dup if ( kbd nkc type mods )
      swap PRESS = if ( kbd nkc mods )
        rot doto mods or | else inv rot doto mods and | then ( nkc )
      drop 0 exit then ( kbd nkc type )
    dup BOTH < if
      dip 2dup swap | ( kbd nkc nkc kbd type )
      r! PRESS = if press else depress then r> then
    PRESS and if swap mods or 1 else 2drop 0 then
    else ( kbd ) drop 0 then ;

: nkc>char ( nkc kbd -- c )
  swap case ( kbd )
    Passthrough and of drop r@ $ff and endof
    $80 and of ( special keys ) drop 0 endof
    ( kbd nkc ) tuck CodeMask and swap layout + ( nkc a )
    over Shift and if $80 + then
    over RAlt and if $100 + then c@ ( nkc c )
    swap Control and if $1f and then ( c ) endcase ;

: ?char>nkc ( c kbd -- ?nkc f )
  layout $100 cidx if ( idx )
    dup $80 >= if $80 - LShift or then 1
    else 0 then ;

: normalizemods ( nkc -- nkc )
  $00aa0000 over and 2/ or $00aa0000 invand ;

: melt ( nkc kbd -- nkc )
  over Passthrough and if
    over $ff and ( nkc kbd c )
    dup rot ?char>nkc if nip swap $7fff0000 and or else ( nkc c )
      dup $20 < if nip $40 + LControl or else drop then then
    else drop then ( nkc ) normalizemods ;

: pollkbd ( kbd -- c-or-0 )
  dup ?nkc if swap nkc>char else drop 0 then ;

: newkbd ( '?event -- kbd ) here# USLayout , 0 , 0 , swap , ;
:~ ( kbd stream -- ?nkc eventtype )
  nip getc dup EOF = if drop 0 else Passthrough or 1 then ;
: newstreamkbd ['] ~ bind> newkbd ;

:> drop 0 ; newkbd value keyboard
: ?key ( -- ?c f ) keyboard pollkbd dup if 1 then ;
alias noop promptforkey
: key promptforkey begin ?key not while idle repeat ;
