: &nf, ( n op -- ) $f600 arii, ;
alias +, +c$,
alias -, -c$,
: +c, $2 ari, ; \ adc
: -c, $3 ari, ; \ sbb
: carry?, dup 0 swap !n, HAL8B invand $92 opc! op0f, ;

: ?AX>src, dup HALINV and if $0 reg! dup @, $f889 ( ax di mov, ) w, then ;
: ?AXdst! dup b2:0 if else dup >>3 b2:0 or then ;
: ?src>DX, dup (sz $4 - if S>) @, S) &) then ;
: ?AX>DI, dup HALINV and if HALINV invand $c789 ( di ax mov, ) w, then ;

: d*, ( op -- )
  deref! ?immDI! ?AXdst!
  dup ?AX>DI, ?swapAX,
  ?src>DX,
  $f620 over (signed? if $8 or then
  or xcomp/boot op,
  ?AX>src, ?swapAX, drop ;
