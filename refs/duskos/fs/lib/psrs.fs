unit lib/psrs

code dipdup ( a b -- a a b )
  PSP) S>) @, PSP) S>) -!, exit,

code dipswap ( a b c -- b a c )
  PSP) @!, PSP) 4 +) @!, PSP) @!, exit,

code dipnip ( a b c -- b c )
  PSP) S>) @+, PSP) S>) !, exit,

code dipover ( a b c -- a b a c )
  PSP) 4 +) S>) @, PSP) S>) -!, exit,

: rswap RSP) @!, RSP) 4 +) @!, RSP) @!, ; immediate

code dig ( ... n -- ... val )
  2 i) <<, PSP) &) +, W) @, exit,

:~ 0 i) <>) if, 1 i) -, [compile] then ifz, drop, exit, [compile] then ;
code roll ( ... n -- ... )
  ~ S) &) !, PSP) &) A>) @, A) @+, begin
    A) @!, 4 i) A>) +, 1 i) S>) -, ?brnz,
  nip, exit,

: rollk r! roll r> ;

code roll> ( ... n -- ... )
  ~ S) &) !, 2 i) <<, PSP) &) A>) @, W) &) A>) +, PSP) @, begin
    A) @!, 4 i) A>) -, 1 i) S>) -, ?brnz,
  nip, exit,

: rollk> r! roll> r> ;

code ps[] ( ... n -- ... a u )
  PSP) &) S>) @, PSP) S>) -!, exit,

code rs[] ( n -- a u )
  RSP) &) S>) @, 4 i) S>) -, PSP) S>) -!, exit,

code ndrop ( ... n -- )
  2 i) <<, PSP) &) +, PSP) &) !,
  drop, exit,

code ndup ( ... n -- ... )
  S) &) !, A) &) !, PSP) &) @,
  2 i) A>) <<, PSP) &) A>) swap-, PSP) &) A>) !,
  move, drop, exit,

: nconcat ( ... n ... n -- ... n ) 1+ rollk + 1- ;
: nfirst ( ... n -- elem-or-0 ) dup if swap >A 1- ndrop A> then ;
: nsame ( ... n cnt -- ... n )
  ?dup not if ndrop 0 exit then
  over not if drop exit then
  over >r * r! V1 - 0 do V1 1- dig loop r> rdrop ;

