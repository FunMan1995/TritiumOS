needs lib/struct lib/bit lib/psrs mem/mark
unit mem/cons

struct Cons { uint car cdr ; }

$400 const CONSCNT \ must be divisible by 32
CONSCNT newmarklist const keeplist
create pool CONSCNT 8* allot

code iscons? ( a -- f ) \ Preserves A
  pool i) -, 8 i) /mod,
  0 i) S>) <>) if, 0 i) @, exit, then
  CONSCNT i) <) bool, exit,
: iscons# iscons? not ?abort"cons expected" ;

: markused ( cons -- ) pool - 8/ keeplist mark ;

\ we want to avoid blowing the stack on recursion, so we loop, but because both
\ elements of the pair are potentially cons, we use recursion on one of them. We
\ choose the first element because cons links are generally the second element.
: markusedall ( cons -- )
  begin dup markused
    dup car dup iscons? if markusedall else drop then ( cons )
    cdr dup iscons? not until ( a )
  drop ;
: ?markusedall ( a -- ) dup iscons? if markusedall else drop then ;

variable consrefLL
: consref value consrefLL lladd CURWORD @ scryfind# , ;

CONSCNT 32 / const LEAKMAXCNT
create leaked LEAKMAXCNT 4* allot0
0 value leakcnt
: leak ( cons -- )
  leakcnt LEAKMAXCNT = ?abort"out of cons leak space"
  leaked leakcnt 4* + ! doto leakcnt 1+ | ;

:~ ( a u -- ) 0 do @+ ?markusedall loop drop ;
: gc ( -- )
  keeplist unmarkall
  scnt ps[] ~
  rcnt rs[] ~
  leaked leakcnt ~
  consrefLL begin @ ?dup while ( ll ) dup 4+ @ @ ?markusedall repeat ;

\ find a free slot. If found, mark it as used and yield its address in the pool
: getslot ( -- a-or-0 )
  keeplist findunmarked if dup keeplist mark 8* pool + else 0 then ;
: getslot# ( -- a )
  getslot ?dup not if gc getslot then
  ?dup not ?abort"out of cons memory" ;

: cons ( car cdr -- cons ) getslot# A! rot> A> to cdr A> to car ;
: append ( cons car -- newcons ) 0 cons tuck swap to cdr ;
:~ A! car A> cdr ;
: carcdr ( cons -- car cdr ) dup iscons# ~ ;
: ?carcdr ( a -- car cdr 1 OR a 0 ) dup iscons? if ~ 1 else 0 then ;
:~ A! cdr A> car ;
: cdrcar ( cons -- cdr car ) dup iscons# ~ ;
: ?cdrcar ( a -- cdr car 1 OR a 0 ) dup iscons? if ~ 1 else 0 then ;
: ?single ( a -- x 1 OR a 0 )
  dup ?carcdr if if drop 0 else nip 1 then else drop 1 then ;
: single# ?single not ?abort"single cons expected" ;
: length ( list -- cnt ) 0 >r begin ?dup while doto V1 1+ | cdr repeat r> ;

: conscnt ( -- n ) keeplist markcnt ;

: islist? ( a -- f )
  ?carcdr if nip ?dup if islist? else 1 then else drop 0 then ;

alias abort .cons
: .list ( cons -- )
  ."(" begin
	cdrcar dup iscons? if .cons else . then ( cdr )
    dup iscons? dup if spc> then not until ( n )
  ?dup if spc> . then .")" ;
: .pair ( cons -- ) ."(" cdrcar .cons ." . " .cons .")" ;
:realias .cons ( cons -- )
  dup iscons? if dup islist? if .list else .pair then else . then ;
