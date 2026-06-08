needs lib/struct
unit mem/reuse

struct Buf {
  uint next magic size used ;
  [void,0] buf ;
}

variable root
: newbuf ( sz -- buf ) 1 over n"REUS" root @ 4 n,@ tuck root ! allot ;
\ Find smallest buffer with size >= "sz".
: findbuf ( sz ll -- buf-or-0 )
  begin @ dup while ( sz buf )
    2dup size > over used or while repeat then ( sz buf )
  dup if ( sz buf )
    tuck findbuf ?dup if ( buf buf )
      over size over size < if drop else nip then then
    else nip then ;
: findandusebuf ( sz ll -- buf-or-0 ) findbuf dup if 1 over to used then ;

: >buf ( a -- buf-or-0 ) 16 - dup align4 magic n"REUS" <> if drop 0 then ;
: >buf# ( a -- buf ) >buf ?dup not ?abort"not a reuse buf" ;
: free >buf ?dup if 0 swap to used then ;
: free# >buf# 0 swap to used ;
: ?reuse ( sz -- a ) dup root findandusebuf ?dup if nip else newbuf then buf ;
: ?realloc ( a sz -- a )
  over >buf# size over >= if drop else ( a sz )
    ?reuse tuck over >buf# size cmove then ;

0 value reusebuf
: reuse[
  reusebuf ?abort"can't nest reuse["
  0 newbuf to reusebuf ;
: ]reuse
  reusebuf not ?abort"call reuse[ first"
  here reusebuf buf - reusebuf to size
  0 to reusebuf ;
