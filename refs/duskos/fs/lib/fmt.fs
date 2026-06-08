needs lib/str comp/sig
unit lib/fmt

: .x? dup $ffff > if .x else dup $ff > if .x2 else .x1 then then ;

variable MERGEFMT ( a u -- )
MERGEFMT @alias mergefmt
create _fmtchars "bwxdszc" s,
\ sig: ( n -- a u )
create _fmtwords ' formathex1 , ' formathex2 , ' formathex , ' formatdec ,
                 ' c@+ , ' z[] , ' c[] ,

: dofmt ( ... fmt -- )
  c@+ 2>r begin ( ... ) \ V1=a V2=u
    '%' V1 V2 cidx while ( ... idx )
    V1 over mergefmt 2+ 2r> rot consume[] 2>r ( ... )
    V2 0>= while \ ending with literal %!
    V1 1- c@ dup _fmtchars c@+ cidx if
      nip 4* _fmtwords + @ execute else c[] then ( ... a u )
    mergefmt ( ... ) repeat
    2rdrop else 2r> mergefmt then ;

variable a
:~ a @ swap cmove+ a ! ;
: sprintf ['] ~ MERGEFMT ! newstr a ! dofmt a @ endstr ;
:~ a @ write# ;
: streamprintf ['] ~ MERGEFMT ! a ! dofmt ;

: printf ['] rtype MERGEFMT ! dofmt ;
annotatelast ( ... *uchar -- )

:> [compile] " compile printf ;
:> [rcompile] " printf ;
compiling .f"
