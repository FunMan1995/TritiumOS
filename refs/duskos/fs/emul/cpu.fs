needs lib/struct
unit emul/cpu

struct CPU {
  uint mem( ;
  xt step halted? ;
}

: stepN ( n cpu -- ) swap 0 do dup step dup halted? if break then loop drop ;
: run ( cpu -- ) begin dup step dup halted? until drop ;
: newcpu ( 'halted? 'step mem( -- cpu ) 3 n,@ ;
