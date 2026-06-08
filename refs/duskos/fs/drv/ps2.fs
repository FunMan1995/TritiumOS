needs io/kbd
unit drv/ps2

\ Return a PS/2 keycode if one is available.
alias abort (ps2@?) ( -- keycode? f )

: ps2@ ( -- kc ) begin (ps2@?) until ;
: ?flag ( kc -- kc f )
  dup $ef and $e0 = if $30 and $10 - 4 lshift 1 else 0 then ;

\ Fetch kc through (ps2@?). If it's F0, fetch again and set $200 flag on
\ result. If it's E0, set $100.
: ?ps2@merge ( -- kc? f )
  (ps2@?) if 0 swap begin ?flag while or ps2@ repeat or 1 else 0 then ;

\ Table mapping of PS/2 Set 1 Make codes to io/kbd NKCs
\ $d0-$df (reserved in NKC) values are flags

\     1   2   3   4   5   6   7   8   9   a   b   c   d   e   f
create map map< c,
  0   ESC '1' '2' '3' '4' '5' '6' '7' '8' '9' '0' '-' '=' BS  $09 \
  'Q' 'W' 'E' 'R' 'T' 'Y' 'U' 'I' 'O' 'P' '[' ']' CR  $d2 'A' 'S' \
  'D' 'F' 'G' 'H' 'J' 'K' 'L' ';' ''' '`' $d0 '\' 'Z' 'X' 'C' 'V' \
  'B' 'N' 'M' ',' '.' '/' $d1 $b3 $d4 SPC $d8 $81 $82 $83 $84 $85 \
  $86 $87 $88 $89 $8a $8b $8c $c7 $c8 $c9 $b4 $c4 $c5 $c6 $b5 $c1 \
  $c2 $c3 $c0 $ca 0   0   '<' 0   0   0   0   0   0   0   0   0 \
\ E0 prefix
  0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0 \
  0   0   0   0   0   0   0   0   0   0   0   0   0   $d3 0   0 \
  0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0 \
  0   0   0   0   0   0   0   0   $d5 0   0   0   0   0   0   0 \
  0   0   0   0   0   0   0   0   $ab 0   0   $a9 0   $aa 0   0 \
  $ac 0   0   $a4 0   0   0   0   0   0   0   $d6 $d7 $ad 0   0

:~ ( self -- ?nkc event-type )
  drop ?ps2@merge if ( kc )
    dup $80 and if \ break
      $80 invand RELEASE else PRESS then ( kc etype )
    swap dup $100 and if \ extension key
      $7f and $60 + then ( etype mapidx )
    map + c@ ?dup if swap else drop NONE then
    else NONE then ;
: newps2set1kbd ['] ~ newkbd ;

\ Table mapping Make codes of Set 2 to ASCII char

\     1   2   3   4   5   6   7   8   9   a   b   c   d   e   f
create map map< c,
  0   $89 0   $85 $83 $81 $82 $8c 0   $8a $88 $86 $84 $09 '`' 0 \
  0   $d5 $d0 0   $d2 'Q' '1' 0   0   0   'Z' 'S' 'A' 'W' '2' 0 \
  0   'C' 'X' 'D' 'E' '4' '3' 0   0   SPC 'V' 'F' 'T' 'R' '5' 0 \
  0   'N' 'B' 'H' 'G' 'Y' '6' 0   0   0   'M' 'J' 'U' '7' '8' 0 \
  0   ',' 'K' 'I' 'O' '0' '9' 0   0   '.' '/' 'L' ';' 'P' '-' 0 \
  0   0   ''' 0   '[' '=' 0   0   $d8 $d1 CR  ']' 0   '\' 0   0 \
  0   '<'   0   0   0   0   BS  0   0   '1' 0   '4' '7' 0   0   0 \
  '0' '.' '2' '5' '6' '8' ESC $b0 $8b '+' '3' '-' '*' '9' 0   0 \
  0   0   0   $87 0   0   0   0   0   0   0   0   0   0   0   0 \
\ E0 prefix
  0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0 \
  0   $d5 0   0   $d3 0   0   0   0   0   0   0   0   0   0   $d6 \
  0   0   0   0   0   0   0   $d7 0   0   0   0   0   0   0   0 \
  0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0 \
  0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0 \
  0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0 \
  0   0   0   0   0   0   0   0   0   0   0   $a9 0   0   0   0 \
  0   $a4 $ac 0   $aa $ab 0   0   0   0   0   0   0   0   0   0 \
  0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0

:~ ( self -- ?nkc event-type )
  drop ?ps2@merge if ( kc )
    dup $200 and if \ break
      $200 invand RELEASE else PRESS then ( kc etype )
    swap dup $100 and if \ extension key
      $ff and $8f min $90 + then ( etype mapidx )
    map + c@ ?dup if swap else drop NONE then
    else NONE then ;
: newps2set2kbd ['] ~ newkbd ;
: newps2kbd ( scancodeset -- kbd )
  case 1 = of newps2set1kbd endof
       2 = of newps2set2kbd endof
       abort"unsupported PS/2 Scan Code Set" endcase ;