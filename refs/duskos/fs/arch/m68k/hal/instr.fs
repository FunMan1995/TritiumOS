: &nf, D0@, drop $0280 w, , ;

alias +, +c$,
alias -, -c$,
: opc, ( op mask -- )
  $180 or swap dup D0@, over (dir? if src<>dst then
  rot over src@ or swap dst@ <<9 or w,
  dup (dir? if D0 dst! !, else drop then ;
: +c, $d000 opc, ;
: -c, $9000 opc, ;
:~ ( op instr -- op ) over sz2 <<6 or over rot> eaop, ;
: carry?, ( op -- ) $4200 ~ $4000 ~ $4400 ~ drop ;

:~ ( op -- ) dup dst@ <<12 $0405 or
  over (signed? if $0800 or then
  hbank! hslot< $4c00 or eaop, ;
: d*, ( op -- )
  dup (dir? if
    dup (&? if src<>dst ~ else
      dup D0@, src<>dst ~ D0 dst! @, then
    else dup HAL816B and if D0@, then ~ then ;
