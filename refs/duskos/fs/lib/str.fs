unit lib/str

create NULLSTR 0 c,
32 allot@ dup 32 SPC cfill : 32SPCS [ litn ] 32 ;

: string create STR_MAXSZ allot0 ;
: []strmove ( a u dst -- ) over swap c!+ swap cmove ;
: []>str ( a u -- str ) dup 1+ strallot r! []strmove r> ;
: str>pool c@+ []>str ;
: []>pool r! strallot r! V1 cmove r> r> ;
: s[]= ( str a u -- f ) rot A! c@ = if A> c@+ c[]= else drop 0 then ;

: cmove+ ( src dst u -- dst+ ) cmove A> ;
: cmove" ( dst -- a u ) [rcompile] " c@+ >r over r@ cmove r> ;
: strcat ( s1 s2 -- str )
  swap c@+ newstr swap cmove ( s2 A=a )
  c@+ A> swap cmove+ endstr ;
: zstrlen ( zstr -- len )
  0 swap STR_MAXSZ cidx not ?abort"string too long" ;
: zmove ( zsrc dst -- dst+ ) over zstrlen 1+ cmove+ ;
: str>zstr ( str -- zstr )
  c@+ dup 1+ strallot ( a u dst )
  r! swap cmove+ 0 swap c! r> ;
: z[] dup zstrlen ;

code consume[] ( a u n -- a u )
  S) &) !, drop, S) &) -, PSP) dir) S>) +, exit,

: do[] ( a u -- ) PSP) +, swap, [compile] do ; immediate
: [str]? ( str a u -- idx ) rot >r ( a u ) \ V1=str
  V1 c@ - dup 0>= if ( a u-adj ) 1+ 0 do ( a )
      dup i + V1 c@+ c[]= if i break then loop
      broke? not if -1 then nip
    else 2drop -1 then ( idx ) rdrop ;

: rcidx ( n a u -- ?idx f )
  rot >r begin ?dup while ( a u V1=n )
    1- 2dup + c@ V1 <> while repeat
  ( a u ) nip 1 else ( a ) drop 0 then rdrop ;

: s) ( str -- a ) c@+ + ;
: sfind ( str list -- idx? f )
  0 >r begin ( s a )
    2dup s= not while
    r> 1+ >r s) dup c@ while repeat
    2drop rdrop 0 else 2drop r> 1 then ;

: slistiter ( idx list -- str ) swap 0 do s) loop ;
: slistlen ( list -- len ) >A 0 begin A> s) A! c@ while 1+ repeat ;

: stringlist create begin wordorquote s, eol? until 0 c, ;
: strings< 0 begin wordorquote swap 1+ eol? until ;

: startswith? ( substr str -- f ) c@+ [str]? not ;
: endswith? ( substr str -- f )
  A! c@ over c@ < if drop 0 else ( sub A=str )
    A> @Ac@ + 1+ over c@ - ( sub a )
    swap c@+ c[]= then ;

: ws? ( c -- f ) SPC <= ;
: upcase ( c -- c ) dup 'a' - 26 < if $df and then ;
: upstr ( s -- s ) str>pool dup c@+ do[] i c@ upcase i c! loop ;
: lowcase ( c -- c ) dup 'A' - 26 < if $20 or then ;
: lowstr ( s -- s ) str>pool dup c@+ do[] i c@ lowcase i c! loop ;
