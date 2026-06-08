needs hal/vreg hal/instr lib/wordtbl gr/color gr/buf
unit gr/rdwr

\ A=buf -- W=color
:~ ( mask shift -- ) A) &) @, i) A>) >>, i) &, ;
: rd1l, 1 1 ~ ; : rd2l, 3 2 ~ ; : rd4l, $f 4 ~ ;
:~ ( shift -- ) A) &) @, dup i) A>) <<, 32 swap- i) >>, ;
: rd1h, 1 ~ ; : rd2h, 2 ~ ; : rd4h, 4 ~ ;

: #pixels ( shift -- n ) 1 swap lshift 32 swap / ;

\ A=buf R1=idx -- A=buf R1=idx
: _ ( shift -- )
  dup ?dup if i) R1>) <<, then R1) &) A>) <<, ?dup if i) R1>) >>, then ;
:~ ( shift-- )
  dup #pixels 1- i) R1>) &, ifnz, swap _ [compile] then ;
: ?skip1h, 0 ~ ; : ?skip2h, 1 ~ ; : ?skip4h, 2 ~ ;

: _ ( shift -- )
  dup ?dup if i) R1>) <<, then R1) &) A>) >>, ?dup if i) R1>) >>, then ;
:~ ( shift-- )
  dup #pixels 1- i) R1>) &, ifnz, swap _ [compile] then ;
: ?skip1l, 0 ~ ; : ?skip2l, 1 ~ ; : ?skip4l, 2 ~ ;

\ S=buf W=color -- S=buf
:~ ( shift -- ) dup i) S>) >>, 32 swap- i) <<, W) &) S>) |, ;
: wr1l, 1 ~ ; : wr2l, 2 ~ ; : wr4l, 4 ~ ;
:~ ( shift -- ) i) S>) <<, W) &) S>) |, ;
: wr1h, 1 ~ ; : wr2h, 2 ~ ; : wr4h, 4 ~ ;

\ S=output buf W=dst buf R1=idx -- S=mixed buf
:~ ( shift -- )
  ?dup if i) R1>) <<, then
  R1) &) <<, R1) &) >>,
  32 i) R1>) swap-, R1) &) S>) <<,
  W) &) S>) |, ;
: mix1h, 0 ~ ; : mix2h, 1 ~ ; : mix4h, 2 ~ ;

:~ ( shift -- )
  ?dup if i) R1>) <<, then
  R1) &) >>, R1) &) <<,
  32 i) R1>) swap-, R1) &) S>) >>,
  W) &) S>) |, ;
: mix1l, 0 ~ ; : mix2l, 1 ~ ; : mix4l, 2 ~ ;

\ R1=Xpos -- S=trimmed buf R1=idx
: _ ( shift -- )
  dup ?dup if i) R1>) <<, then R1) &) S>) >>, ?dup if i) R1>) >>, then ;
:~ ( addrop shift -- )
  0 i) S>) @, dup #pixels 1- i) R1>) &, ifnz,
    rot S>) @, swap
    dup #pixels tuck i) R1>) swap-,
    _ i) R1>) swap-, [compile] then ;
: ?trim1h, 0 ~ ; : ?trim2h, 1 ~ ; : ?trim4h, 2 ~ ;

: _ ( shift -- )
  dup ?dup if i) R1>) <<, then R1) &) S>) <<, ?dup if i) R1>) >>, then ;
:~ ( addrop shift -- )
  0 i) S>) @, dup #pixels 1- i) R1>) &, ifnz,
    rot S>) @, swap
    dup #pixels tuck i) R1>) swap-,
    _ i) R1>) swap-, [compile] then ;
: ?trim1l, 0 ~ ; : ?trim2l, 1 ~ ; : ?trim4l, 2 ~ ;

code 3dup
  -12 ps+, PSP) 8 +) !,
  PSP) 12 +) S>) @, PSP) S>) !,
  PSP) 16 +) S>) @, PSP) 4 +) S>) !, exit,

: TODO abort"gr/buf pixel TODO" ;
:~ ( mix wr shift trim -- )
  3 inv i) &, W) S>) @, A) &) !,
  PSP) R1>) @, A) swap execute
  ( col idx junk A=a S=buf R1=idx )
  PSP) 4 +) @, tuck 2* 1 swap lshift 1- i) &, execute
  PSP) R1>) @, 1 i) R1>) +,
  #pixels 1- i) R1>) &, ifnz, swap A) @, execute [compile] then A) S>) !,
  PSP) 8 +) @, 12 ps+, ;
wordtbl[ ( col x a -- )
code> ' mix1l, ' wr1l, 0 ' ?trim1l, ~ exit,
code> ' mix1h, ' wr1h, 0 ' ?trim1h, ~ exit,
code> ' mix2l, ' wr2l, 1 ' ?trim2l, ~ exit,
code> ' mix2h, ' wr2h, 1 ' ?trim2h, ~ exit,
code> ' mix4l, ' wr4l, 2 ' ?trim4l, ~ exit,
code> ' mix4h, ' wr4h, 2 ' ?trim4h, ~ exit,
:> nip c! ;
:> nip w! ;
' TODO
:> nip ! ;
]wordtbl _
: pixel! ( color x y pb -- )
  >r 3dup r@ xyaddr _ A> depth depth>idx wexec
  1 1 r> invalidate drop ;

:~ 3 inv i) &, W) A>) @, PSP) R1>) @+, ;
wordtbl[ ( x a -- col )
code> ~ ?skip1l, rd1l, exit,
code> ~ ?skip1h, rd1h, exit,
code> ~ ?skip2l, rd2l, exit,
code> ~ ?skip2h, rd2h, exit,
code> ~ ?skip4l, rd4l, exit,
code> ~ ?skip4h, rd4h, exit,
:> nip c@ ;
:> nip w@ ;
' TODO
:> nip @ ;
]wordtbl _
: pixel@ ( x y pb -- color )
  xyaddr _ A> depth depth>idx wexec ;
