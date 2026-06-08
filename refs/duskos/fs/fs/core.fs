needs lib/str lib/struct lib/ival io/stream
unit fs/core

$10 const WALKERCTXSZ
0 value curfs
addrof curfs ivalmap {
  uint files storage caseinsensitive? ;
  xt gotoroot gotonext enterdir ;
  xt initfilestruct openfile addfsnode removefsnode writefsnode ;
  [void,WALKERCTXSZ] walkcontext ;
}

string walkpath
string walkname
0 value walksize
0 value walkmtime
0 value walkdir?
: walkdir# walkdir? not ?abort"directory expected" ;
: walkfile# walkdir? ?abort"file expected" ;
: pathcat ( s s -- s ) over c@ if "/" swap strcat strcat else nip then ;
: walkpathcat walkpath walkname pathcat walkpath strmove ;
: walkdepth ( -- n )
  walkpath c@ dup if
    drop 1 walkpath c@+ do[] i c@ '/' = if 1+ then loop then ;

: walk ( fs -- ) to curfs 0 walkpath c! 1 to walkdir? gotoroot ;

0 value bootfs
: bootfs! to bootfs ;

create map 26 4* allot0
: c>mapkey ( c -- c ) upcase 'A' - dup 26 >= ?abort"invalid fs map key" ;
: mapfs ( fs c -- ) c>mapkey 4* map + ! ;
: fsletter ( -- c-or-0 )
  curfs bootfs = if 0 else curfs map 26 idx if 'A' + else '?' then then ;

: closecursor ( file -- ) 0 over 4- ( LL's used field ) ! flush ;
: open ( -- file )
  walkfile# files @ begin ( ll )
    ?dup while dup 4+ @ while @ repeat
    8+ else files lladd 0 , here initfilestruct then ( file )
  1 over 4- ! \ flag as used
  dup openfile ;
: closeall ( -- ) files @ begin ( ll ) ?dup while dup 8+ close @ repeat ;

: ?upstr caseinsensitive? if upstr then ;

string lookupname
: lookupchild ( name -- f )
  dup c@ not if drop 1 exit then
  ?upstr lookupname strmove
  begin gotonext while
    walkname ?upstr lookupname s= not while repeat
    1 else 0 then ;

: iterpath ( a u -- ?a ?u a u f )
  over c@ '/' = over 1 = and if 1 consume[] then
  '/' oover oover cidx if
    oover >r r! 1+ consume[] 2r> 1
    else 0 then ;

: lookuprel ( path -- f )
  c@+ begin iterpath while
    []>str lookupchild while enterdir repeat
    2drop 0 else []>str lookupchild then ;

: walktopathroot ( path -- restofpath )
  A! dup 2+ c@ ':' = @Ac@ 2 >= and if
    dup 1+ c@ c>mapkey 4* map + @
    ?dup not ?abort"unmapped fs key"
    dip c@+ 2 consume[] []>str |
    else bootfs then ( path fs ) walk ;

: lookup ( path -- f ) walktopathroot lookuprel ;

: res# not if lookupname stype ." not found in " walkpath stype nl> abort then ;
: lookup# lookup res# ;
: lookuprel# lookuprel res# ;
: openpath lookup# open ;
: newfile 0 addfsnode ;
: newdir 1 addfsnode ;

: newfs ( <xts in reverse order> case? storage -- fs )
  here# 0 , 11 n,@ WALKERCTXSZ allot0 ;

: loadpath ( path -- ) openpath interpretstream endunit ;
: f<< word loadpath ;
: exec<< word openpath exec< ;

:realias loadunit ( str -- )
  bootfs not if (wnf) then ".fs" strcat loadpath ;
