needs lib/type mem/ll asm/label comp/sig io/stream comp/oberon/type
unit comp/oberon/gc

variable slots
variable typegroups

: addgcslot ( a -- ) slots lladd , ;

8 const SIZEXTOFF
16 const REFTYPEOFF
$18 const STRUCTFIELDSOFF

code structpointer? ( type -- f )
  ' gcptrsz i) S>) @, W) SIZEXTOFF +) S>) <>) if, 0 i) @, exit, then
  W) REFTYPEOFF +) @, \ W=reftype
  ' structsz i) S>) @, W) SIZEXTOFF +) S>) =) bool, exit,

: ?addgcslot ( type a -- )
  over array? if
    over reftype swap rot arraycount 0 do ( type a )
      2dup ?addgcslot over typesz + loop 2drop exit then
  over struct? if
    >r structfields @ begin ?dup while ( ll ) \ V1=a
      dup 4+ @+ swap @ ( ll type off )
      r@ + ?addgcslot @ repeat rdrop exit then
  swap structpointer? if addgcslot else drop then ;

: zeroptr ( a -- a ) dup dup 4- @ typesz align4 4/ 0 fill ;
: allocptr ( type A=gchdls -- a )
  [ dup, A) &) @, ] ( type gchdls )
  lladd 1 , dup , ( type )
  typesz align4 allot@ zeroptr ;

: newgroup ( type -- gchdls type ) typegroups lladd dup , here swap 0 , ;

code typegroup@ ( type -- gchdls type )
  typegroups i) A>) @, \ A=grpll
  begin
    0 A) +n, ' newgroup ?brz,
    A) A>) @,
    ( loop ) A) 4 +) <>) ?br,
  \ found
  8 i) A>) +,
  PSP) A>) -!,
  exit,

code newptr ( type -- gcptr )
  pushlr, ' typegroup@ execute, poplr, ( gchdls type )
  PSP) A>) @+, begin \ A=gchdls
    0 A) +n, ' allocptr ?brz,
    A) A>) @,
    0 A) 4 +) +n, ( loop ) ?brnz,
  \ we have our gchdl in A
  1 A) 4 +) +n,
  A) &) 12 +) @, ' zeroptr bbr,

code clearmarked ( -- )
  dup, typegroups m) @, 0 i) S>) @, begin
    0 i) =) if, drop, exit, then
    W) 8 +) A>) @, begin \ A=gchdls
      0 i) A>) <>) if, to L1
      A) 4 +) S>) !,
      A) A>) @, again
    L1 then
    W) @, again

alias abort _markgcptr

code markfields ( ptr -- ) \ A=struct type
  pushlr, dup,
  A) STRUCTFIELDSOFF +) @, begin ( ptr fieldsLL )
    0 i) =) if, 2drop, popexit, then
    W) 4 +) A>) @, A) SIZEXTOFF +) S>) @, \ A=type S=sizext
    ' structsz i) S>) =) if,
      W) 8 +) S>) @, \ S=offset
      over, S) &) +,
      ' markfields execute, then ( ptr ll )
    ' gcptrsz i) S>) =) if,
      A) REFTYPEOFF +) A>) @, A) SIZEXTOFF +) S>) @, \ A=reftype S=sizext
      ' structsz i) S>) =) if,
        W) 8 +) S>) @, \ S=offset
        over, S) &) +, W) @,
        ' _markgcptr execute, then then ( ptr ll )
    W) @, ( loop ) bbr,

code markgcptr ( ptr -- )
  0 i) =) if, drop, exit, then \ don't mark 0!
  W) &) A>) @,
  0 W) -8 +) +n, ifnz, drop, exit, then \ already marked? avoid infinite loop
  1 i) S>) @,
  W) -8 +) S>) !,
  W) -4 +) A>) @, A) SIZEXTOFF +) S>) @, \ A=type S=type's sizext
  ' markfields ' structsz i) S>) =) ?br, \ recurse into the structure
  drop, exit,
current ' _markgcptr realias

code closestreamrefs ( -- )
  dup, StreamRef i) @,
  pushlr, ' typegroup@ execute, poplr, ( gchdls type )
  PSP) A>) @+, begin \ A=gchdls
    A) A>) @,
    0 i) A>) =) if, drop, exit, then
    0 A) 4 +) +n, ifz, 0 A) 12 +) +n, ifnz,
      A) &) @, dup, A) 12 +) @,
      pushlr, ' close execute, poplr,
      A) &) !, 0 i) @, A) 12 +) !, then then
    again
  exit,

code gc ( -- )
  pushlr, ' clearmarked execute,
  dup, slots i) @, \ W=ll
  begin
    W) @, 0 i) =) if, drop, poplr, ' closestreamrefs bbr, then
    dup, W) 4 +) @, W) @, ' markgcptr execute,
    ( loop ) bbr,

annotate ( -- ) gc

: .ptr ( a -- ) dup .x spc> dup 8- @ . spc> 4- @ .type nl> ;
: .gcslots
  slots @ begin ?dup while
    dup 4+ @ dup .x spc> @ ( ll ptr )
    ?dup if .ptr else ."NIL" then nl>
    @ repeat ;
: gchdlcnt ( -- n )
  0 typegroups @ begin ( cnt ll ) ?dup while
    dup 8+ @ llcnt rot + swap @ repeat ;
:~ ( gchdls -- cnt )
  0 swap @ begin ?dup while dup 4+ @ if dip 1+ | then @ repeat ;
: markedcnt ( -- n )
  0 typegroups @ begin ( cnt ll ) ?dup while
    dup 8+ ~ rot + swap @ repeat ;
: .gc ( -- )
  ."Types:   " typegroups @ llcnt . nl>
  ."Handles: " gchdlcnt . nl>
  ."Marked:  " markedcnt . nl> ;
:~ ( gchdls -- )
  @ begin ?dup while dup 4+ @ not if dup 12 + .x spc> then @ repeat ;
: .gcfree ( -- )
  typegroups @ begin ( ll ) ?dup while
    .">> type " dup 4+ @ .x nl>
    dup 8+ ~ nl> @ repeat ;
: resetgc 0 typegroups ! 0 slots ! ;
