needs lib/psrs hal/instr
unit num/double

: dneg
  neg over 0 <> - swap neg swap ;

:~ PSP) 8 +) S>) @!, ;
code dswap
  PSP) 4 +) @!, ~
  PSP) S>) @!, ~
  exit,

code dnip
  PSP) S>) @,
  PSP) 8 +) S>) !,
  8 ps+, exit,

code d+
  PSP) S>) @,
  PSP) 8 +) dir) S>) +c$,
  PSP) 4 +) dir) +c,
  PSP) 4 +) @,
  8 ps+,
  exit,

code d-
  PSP) S>) @,
  PSP) 8 +) dir) S>) -c$,
  PSP) 4 +) dir) -c,
  PSP) 4 +) @,
  8 ps+,
  exit,

code n>d
  dup,
  31 i) >>,
  0 i) swap-,
  exit,

code d2/
  S) &) !,
  31 i) S>) <<,
  1 i) >>,
  swap,
  1 i) >>,
  S) &) |,
  swap,
  exit,

code d2*
  swap,
  S) &) !,
  1 i) <<,
  31 i) S>) >>,
  swap,
  1 i) <<,
  S) &) |,
  exit,

:~ PSP) swap +) =) bool, ;
code d=
  4 ~ PSP) @!,
  8 ~ PSP) &,
  12 ps+, exit,

alias drop d>n
