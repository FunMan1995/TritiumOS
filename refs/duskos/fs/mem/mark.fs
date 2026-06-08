needs lib/bit
unit mem/mark

: newmarklist ( n -- ) here# swap 32 /+ dup , 4* allot0 ;
: markall @+ -1 fill ;
: unmarkall @+ 0 fill ;

: oob abort"mark list out of bounds" ;
: [?oob] ( idx mlist -- idx mlist )
  PSP) S>) @, 5 i) S>) >>, ['] oob W) S>) >=) ?br, ; immediate

:~ ( idx mlist -- bit a ) [?oob] 4+ swap 32 /mod 4* rot + ;
: mark ( idx mlist -- ) ~ >A bitmask @A@ or @A! ;
: unmark ( idx mlist -- ) ~ >A bitmask @A@ invand @A! ;

: findunmarked ( mlist -- ?idx f )
  @+ 0 do ( a ) [
    W) S>) @+, -1 i) S>) <>) if,
      S) &) @, ] i break then loop
  broke? if ( bits i ) 32 * swap findbit0 + 1 else drop 0 then ;

: markcnt ( mlist -- n ) 0 swap @+ 0 do @+ bitscnt rot + swap loop drop ;
