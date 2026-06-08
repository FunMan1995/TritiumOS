needs io/stream io/grid
unit tests/gridh

\ Use "overridegrid" instead of directly setting "grid" so that when there's a
\ failure, we don't end up with a broken system grid.
0 value realgrid
: overridegrid ( g -- ) doto grid swap | to realgrid ;
: restoregrid doto realgrid 0 | ?dup if to grid then ;
current addquithook

: .grid ( g -- )
  >r 0 V1 seek V1 lines 0 do \ V1=g
    V1 columns console V1 spitn ."|\n" loop rdrop ;
: expect|
  in< '|' <> in< LF <> or if ."line length mismatch " i . nl> abort then ;
: expectgrid ( g -- )
  r! pos >r 0 V1 seek V1 lines 0 do V1 columns 0 do \ V1=g V2=oldpos
      in< V1 getc <> if ."grid mismatch\n" V1 .grid abort then loop
    expect| loop
  V1 getc EOF #eq r> r> to pos ;

create _ ,"0123456789abcdef"
: .1 $f and _ + c@ emit ;
: h< ( -- n )
  0 begin drop in< dup SPC > until
  c[] parsehex not ?abort"bad hex value" ;
: .gridfg ( g -- )
  r! colorptr V1 lines 0 do ( a V1=g )
    V1 columns 0 do c@+ .1 loop ."|\n" loop drop rdrop ;
: expectfg ( g -- )
  r! colorptr V1 lines 0 do V1 columns 0 do ( a V1=g )
      c@+ $f and h< <> if ."gridfg mismatch\n" V1 .gridfg abort then loop
    expect| loop drop rdrop ;
: .gridbg ( g -- )
  r! colorptr V1 lines 0 do ( a V1=g )
    V1 columns 0 do c@+ 4 rshift .1 loop ."|\n" loop drop rdrop ;
: expectbg ( g -- )
  r! colorptr V1 lines 0 do V1 columns 0 do ( a V1=g )
      c@+ 4 rshift h< <> if ."gridfg mismatch\n" V1 .gridbg abort then loop
    expect| loop drop rdrop ;
