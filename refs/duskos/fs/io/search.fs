needs io/stream mem/range
unit io/search

0 value searchfor
0 value sidx
: match ( a u -- )
  searchfor c@+ sidx - rot min ( a sa u )
  dip sidx + | doto sidx over + | ( a sa u )
  c[]= not if 0 to sidx then ;
: match? searchfor c@ sidx = ;

0 value refpos
0 value anchor
: readbufanchor ( st -- ?a n )
  dup pos to anchor -1 swap readbuf
  sidx not if anchor to refpos then ;

: search ( str st -- f )
  over c@ not if 2drop 0 exit then
  r! pos to refpos 0 to sidx to searchfor V1 readbufanchor begin ( ?a u V1=st )
    ?dup while ( a u )
    2dup match sidx not if anchor refpos max to refpos 2dup match then ( a u )
    match? not while ( a u )
    sidx if 2drop V1 readbufanchor else
      dup if 1 ltrim[] doto refpos 1+ | then ( a u )
      2dup searchfor 1+ c@ rot> cidx if ( a u idx )
        doto refpos over + | ltrim[] ( a u )
        else 2drop V1 readbufanchor then then ( ?a u )
    repeat ( a u ) 2drop then ( )
  refpos r> to pos match? ;
