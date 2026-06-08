unit lib/match

: str<# ( -- c ) str< not ?abort"character expected" ;
: rmatch" ( "..." -- ) ( c -- f )
  0 str<# begin ( n lo )
    S) &) !, dup i) S>) -, ( n lo )
    str<# swap- i) S>) >) if, >r 1+ ( n )
    str< not until ( n )
  0 i) @, fbr, swap
  begin ?dup while r> [compile] then 1- repeat ( failjmp )
  1 i) @, [compile] then ; immediate

: 0-9? ( c -- f ) rmatch"09" ;
: A-Za-z? ( c -- f ) rmatch"AZaz" ;
: alnum? ( c -- f ) rmatch"AZaz09" ;

: rfind" ( "..." -- ) ( a u -- ?idx f )
  0 litn swap, rot, ( idx u a )
  A) &) !, [compile] begin
    PSP) @, PSP) 4 +) -, ifnz, ( loop nomatch )
    A) 8b) @+,
    [compile] rmatch"
    0 i) =) if, ( loop nomatch match )
    1 PSP) 4 +) +n, rot [compile] again ( nomatch match )
  swap [compile] then nip, ( match )
  [compile] then nip, ; immediate
