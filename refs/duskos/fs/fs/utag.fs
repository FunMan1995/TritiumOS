needs lib/str fs/core mem/dict
unit fs/utag

create buf STR_MAXSZ allot
: ?debracket ( str -- str )
  "[" over startswith? if
    ']' over c@+ cidx if ( str idx )
      dip 2 + | 1- []>str then then ;

: stripext ( str -- str )
  '.' over c@+ cidx if ( str idx ) swap str>pool tuck c! then ;
: stripdoc ( str -- str )
  "doc/" over startswith? if str>pool dup c@ 4- swap 4+ tuck c! then ;
: wrapfs ( str -- str ) ".fs" strcat ;
: wrapdoc ( str -- str )
  ".txt" strcat ( str )
  "doc/" over startswith? not if
    buf strmove "doc/" buf strcat then ;

: ?findunit ( str -- str )
  dup sysdict findentry ?dup if
    unitofentry ?dup if
      nip unitname dup "xcomp/boot" s= if drop "dict" then then then ;

: utag>.fs ( str -- str ) ?debracket stripdoc stripext wrapfs ;
: utag>.txt ( str -- str ) ?findunit ?debracket stripext wrapdoc ;
: goodpath? ( str -- f ) '.' over c@+ cidx if drop lookup else drop 0 then ;
: utag>path ( str -- str )
  ?debracket dup goodpath? not if stripdoc stripext wrapfs then ;
