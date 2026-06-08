needs hal/opq lib/str
unit mem/range

: rtrim[] ( a u n -- a u ) - max0 ;
: ltrim[] consume[] max0 ;

: cmove- ( src dst u -- )
  rot> over - >r over + swap 0 do ( a ) \ V1=delta
    1- dup c@ over V1 + c! loop drop rdrop ;
: rslide+ ( by a u -- ) rot> tuck + ( u src dst ) rot cmove- ;
: rslide- ( by a u -- ) >r tuck swap- r> cmove ;

: intersect[] ( a1 u1 a2 u2 -- ?a u )
  over + >r rot> over + >r ( a2 a1 ) \ V1=a2hi V2=a1hi
  max r> r> min ( alo ahi ) 2dup < if over - else 2drop 0 then ;

: glue[] ( a1 u1 a2 u2 -- a u )
  ?dup if
    2>r ?dup if
      over + 2r> over + ( lo1 hi1 lo2 hi2 )
      rot max rot> min tuck -
      else drop 2r> then
    else drop then ;

: ?[] ( a u -- ?a ?u f ) dup if 1 else nip then ;

: _, ( a u -- )
  1 i) -, W) MOD) (sz i) *, PSP) +, PSP) A>) @, S) &) !, nip, \ A=lo S=hi
  [compile] begin S) &) A>) >=) if, drop, exit, [compile] then
    A) MOD) @, S) MOD) @!,
    A) MOD) !+,
    W) MOD) (sz i) S>) -,
  [compile] again ;
code swap[] 32b _, exit,
code wswap[] 16b _, exit,
code cswap[] 8b _, exit,

: split[] ( a u idx -- alo ulo ahi uhi )
  over min tuck - ( alo ulo uhi )
  >r 2dup + r> ;

: _, ( a u xt -- )
  0 PSP) +n, ifz, 2drop, drop, exit, [compile] then
  pushlr, RSP) -!, [compile] begin
    PSP) 4 +) A>) @, A) MOD) @,
    RSP) brr,
    PSP) 4 +) A>) @, A) MOD) !+, PSP) 4 +) A>) !,
    -1 PSP) +n, ?brnz,
  8 ps+, drop, 4 rs+, popexit, ;
code map[] 32b _,
code wmap[] 16b _,
code cmap[] 8b _,
