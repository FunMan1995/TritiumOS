needs lib/struct
unit mem/stack

struct Stack {
  uint size ptr ;
  [void,0] buf( ;
}

: )buf A! buf( A> size + ;
: empty? bi buf( | ptr = ;
: empty A! buf( A> to ptr ;
: count A! ptr A> buf( - 4/ ;
: stack[] ( stack -- a u ) bi buf( | count ;
: push ( n stack -- )
  dup )buf swap A! ptr = ?abort"Stack overflow"
  ( n stack ) A> ptr ! A> doto ptr 4+ | ;
: peek' ( stack -- 'n )
  dup empty? ?abort"Stack underflow" ptr 4- ;
: peek ( stack -- n ) peek' @ ;
: pop ( stack -- n ) dup peek swap doto ptr 4- | ;
: newstack ( nbelems -- stack ) here# swap 4* dup , here 4+ , allot ;
