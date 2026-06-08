needs hal/vreg
unit mem/sort

: pre,
  PSP) A>) @+, \ W=hi A=lo
  S) &) !, A) &) S>) -,
  3 i) S>) >>, 2 i) S>) <<,
  A) &) S>) +, S) S>) @, \ S=pivot
  4 i) +, 4 i) A>) -, ;

: ?exit, A) &) <=) if, exit, [compile] then ;
: swapWA, A) R0>) @, W) R0>) @!, A) R0>) !, ;

code regular ( lo hi -- n )
  pre, begin
    begin 4 i) A>) +, A) S>) >) ?br,
    begin 4 i) -, W) S>) <) ?br,
    ?exit, swapWA, again

variable offset
code indirect ( lo hi -- n )
  pre, offset m) R1>) @, R1) &) S>) +, S) S>) @, begin \ S=pivot R1=offset
    begin
      4 i) A>) +, A) R0>) @, R1) &) R0>) +,
      R0) S>) >) ?br,
    begin
      4 i) -, W) R0>) @, R1) &) R0>) +,
      R0) S>) <) ?br,
    ?exit, swapWA, again

create PARTITION ' regular ,
PARTITION @alias partition

: qsort ( lo hi -- )
  2dup >= if 2drop exit then
  2dup partition ( lo hi mid )
  rot over qsort ( hi mid )
  4+ swap qsort ;

: []lohi 1- 4* over + ;
: sort ( a u -- ) ['] regular PARTITION ! []lohi qsort ;
: sort@ ( off a u -- ) rot offset ! ['] indirect PARTITION ! []lohi qsort ;
