needs lib/psrs
unit lib/tagl

$100 const ELEMCNT \ element count per buffer
ELEMCNT 12 * const BUFSZ
here# 0 , BUFSZ allot
create allbufs , \ linked list of all buffers
variable curidx \ index at which the next tag will be added

: curbuf ( -- buf )
  curidx @ ELEMCNT < if allbufs llend 4+ else
    0 curidx ! allbufs lladd here# BUFSZ allot then ;

: createtag ( -- a ) curbuf curidx @ 12 * + 1 curidx +! ;
: settag ( addr tag value a -- ) tuck 8+ ! tuck 4+ ! ! ;
: addtag createtag settag ;

code findinbuf ( addr tag buf u -- ?value f )
  0 i) =) if, nip, nip, nip, exit, then
  S) &) !, PSP) A>) @+, drop, swap, begin ( tag addr ) \ A=a S=u
    A) =) if,
      swap, A) 4 +) =) if,
        A) 8 +) @, PSP) !, 1 i) @, exit, then
      swap, then
    12 i) A>) +, 1 i) S>) -, ?brnz,
  0 i) @, nip, exit,

: findtag ( addr tag -- ?value f )
  2>r allbufs @ begin ( tag ll ) \ V1=addr V2=tag
    ?dup while dup 4+ ( ll buf )
    over @ if ELEMCNT else curidx @ then ( ll buf u )
    V1 rot> V2 rot> findinbuf not while ( ll ) @ repeat
    ( ll value ) nip 1 else ( ) 0 then 2rdrop ;
