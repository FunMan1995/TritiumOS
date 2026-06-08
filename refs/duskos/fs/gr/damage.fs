needs asm/label
unit gr/damage

code damage ( x y w h -- damage )
  0 i) =) if, 12 ps+, exit, then
  PSP) testz, ifz, drop, 8 ps+, exit, then
  PSP) 4 +) +, PSP) 8 +) A>) @,
  PSP) A>) +, PSP) A>) !, ( x y maxx maxy A=maxx )
  W) &) A>) |, 0 i) S>) @, begin \ A=maxval S=scale
    127 i) A>) >=) if, to L1
    1 i) A>) >>, 1 i) S>) +, bbr,
    L1 then
  S) &) >>, ( x y maxx dmg )
  PSP) A>) @, S) &) A>) >>, \ maxx
  7 i) A>) <<, A) &) |,
  PSP) 4 +) A>) @, S) &) A>) >>, \ miny
  14 i) A>) <<, A) &) |,
  PSP) 8 +) A>) @, S) &) A>) >>, \ minx
  21 i) A>) <<, A) &) |,
  28 i) S>) <<, S) &) |,
  12 ps+, exit,

code damagerect ( damage -- x y w h )
  -12 ps+, S) &) !, 28 i) S>) >>, \ S=scale
  A) &) !, 21 i) A>) >>, $7f i) A>) &, S) &) A>) <<, PSP) 8 +) A>) !,
  A) &) !, 14 i) A>) >>, $7f i) A>) &, S) &) A>) <<, PSP) 4 +) A>) !,
  A) &) !, 7 i) A>) >>, $7f i) A>) &,
  1 i) A>) +, S) &) A>) <<, 1 i) A>) -, PSP) 8 +) A>) -, PSP) A>) !,
  $7f i) &, 1 i) +, S) &) <<, 1 i) -, PSP) 4 +) -, exit,

: _ ( shift -- )
  PSP) A>) @, dup i) A>) >>, $7f i) A>) &, S) &) A>) >>,
  i) A>) <<, A) &) |, ;

: __ ( cond shift -- )
  PSP) A>) @, PSP) 4 +) S>) @,
  dup if dup i) A>) >>, dup i) S>) >>, then ( cond shift )
  $7f i) A>) &, $7f i) S>) &,
  S) &) A>) rot if, S) &) A>) @, [compile] then ( shift )
  ?dup if i) A>) <<, then A) &) |, ;

code damage+ ( d1 d2 -- damage )
  PSP) >) if, swap, then ( hi lo )
  0 i) =) if, drop, exit, then
  dup, PSP) 4 +) S>) @, 28 i) S>) >>,
  A) &) !, 28 i) A>) >>,
  A) &) S>) -, ifnz, \ scale W by S
    $7f i) &, S) &) >>, 7 _ 14 _ 21 _
    PSP) 4 +) S>) @, $f0000000 i) S>) &,
    S) &) |, PSP) !, then
  $f0000000 i) &, ( d1 d2 new )
  >) 21 __ >) 14 __ <) 7 __ <) 0 __
  8 ps+, exit,

: .damage
  ."scale=" dup 28 rshift .
  ." minx=" dup 21 rshift $7f and .
  ." miny=" dup 14 rshift $7f and .
  ." maxx=" dup 7 rshift $7f and .
  ." maxy=" $7f and . nl> ;
