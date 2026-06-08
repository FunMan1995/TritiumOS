needs lib/str lib/wordtbl lib/macro lib/bit io/stream fs/core \
      mem/here mem/arena mem/kv comp/tok asm/label
unit asm/uxntal

: doerr tokdbg ;
: _err" compile doerr [compile] abort" ; immediate
: nozp# here org < ?abort"can't write to zero page!" ;

newarena dup bindalloc[ tmp[ bindrun1 tmp1

$1f const REGCNT
create regular
  ,"INCPOPNIPSWPROTDUPOVREQUNEQGTHLTHJMPJCNJSRSTH"
  ,"LDZSTZLDRSTRLDASTADEIDEOADDSUBMULDIVANDORAEORSFT"

: regop? ( str -- idx? f )
  1+ regular REGCNT 0 do 2dup 3 c[]= if 2drop i break then 3 + loop
  broke? if 1+ 1 else 2drop 0 then ;

: mod# ( c -- code )
  "2rk" c@+ cidx if 5 + bitmask else _err"invalid mod\n" then ;

stringlist zrow BRK JCI JMI JSI LIT LIT2 LITr LIT2r

: ?op ( tok -- ?opcode f )
  dup zrow sfind if nip $20 * else ( str )
    dup regop? not if drop 0 exit then ( str idx )
    swap c@+ 3 consume[] ( code a u ) 0 do
      c@+ mod# rot or swap loop drop then 1 ;

: c,# nozp# c, ;
: ?, ( short? n -- ) nozp# swap if wbe, else c, then ;

consts $80 LIT $a0 LIT2 $20 SHORT
: spitlit ( short? n -- ) over SHORT * LIT + c,# ?, ;

: ?parsehex ( tok -- ?isshort ?n f )
  c@+ dup 2 > rot> parsehex dup not if nip then ;
: parsehex# ( tok -- short? n ) ?parsehex not if _err"not a number\n" then ;

variable labels \ 2b offset 2b usecnt
0 value curlabel \ link to current "@" label entry
variable references \ 4b addr 1b type (0=abs 1=zp 2=rel)

: curlabel# curlabel dup not ?abort"curlabel expected" ;
: deprefix ( a u -- a u ) 2dup '/' rot> cidx if nip then ;
: curprefix curlabel# entryname[] deprefix []>str ;
: prefixedName ( name -- name )
  curlabel if curprefix "/" strcat swap strcat then ;

: findLabel ( name -- ?off f )
  dup 1+ c@ '/' = if curprefix swap strcat then
  labels find ?dup if dup 2+ dup w@ 1+ swap w! w@ 1 else 0 then ;
: findLabel# ( name -- off ) findLabel not ?abort"label not found" ;
: addLabel ( name -- )
  dup findLabel if drop stype _err" already exists\n" then
  4 labels rot tmp1 entry+ pc swap A! w! 0 A> 2+ w! ;

wordtbl[ ( tgtpc refaddr -- )
:> ( abs ) wbe! ;
:> ( zp ) c! ;
:> ( rel ) tuck addr>pc - 2- swap c! ;
:> ( rel16 ) tuck addr>pc - 2- swap wbe! ;
]wordtbl writeref

: ?addReference ( type name -- )
  dup findLabel if ( type name off )
    nip here rot writeref swap wexec else ( type name )
    5 references rot tmp1 entry+ here swap !+ c! then ;

\ why stepback? empty sublabels are valid
: lblref< in< ws? if NULLSTR else stepback tok< then ;
: label< ( -- str )
  in< bi '&' = | '/' = or if lblref< prefixedName else stepback tok< then ;
: ?addReference< ( type -- ) nozp# label< ?addReference ;

: setreference ( tgtpc ref -- ) e>xt @+ writeref rot c@ wexec ;
: resolve ( -- )
  references begin @ ?dup while dup e>wlen c@ if
    dup entryname[] []>str findLabel not if
      entryname[] rtype abort" not found" then ( ll off )
    over setreference then ( ll ) repeat ;

: findunused ( -- )
  labels begin @ ?dup while
    dup e>wlen c@ if dup e>xt 2+ w@ not if
      dup entryname[] rtype nl> then then repeat ;

: closelambda ( -- )
  "{" references findentry ?dup not if _err"unmatched {" then ( e )
  0 over e>wlen c! pc swap setreference ;

variable macros
: addMacro< ( -- )
  tmp[ MAXMACROSZ reserve
  macros tok< entry
  tonws< nip '{' <> if _err"{ expected\n" then ( m )
  begin in< dup '}' <> while c, repeat ( m c )
  drop 0 c, ]alloc ;

alias abort parseall \ forward reference

: hex<# tok< parsehex# nip ;
: hexorlabel<#
  label< dup ?parsehex if nip nip else findLabel not if doerr (wnf) then then ;
: incbin
  hex<# hex<# swap ( len off )
  tok< openpath tuck seek ( len stream )
  2dup here rot> read# ( len stream )
  close allot ;
: include ( -- )
  tok< dup "bin" s= if drop incbin else openpath ['] parseall swap exec< then ;

: comment< begin tok< "(" over s= if comment< then ")" s= until ;

kvtbl[
'#' :> tok< parsehex# spitlit ;
'|' :> hexorlabel<# $100 - org + HERE ! ;
'$' :> hexorlabel<# here org >= if allot0 else allot then ;
'@' :> tok< addLabel labels @ to curlabel ;
'&' :> lblref< prefixedName addLabel ;
'/' :> $60 c,# 3 tok< prefixedName ?addReference 2 allot ;
'!' :> $40 c,# 3 ?addReference< 2 allot ;
'?' :> $20 c,# 3 ?addReference< 2 allot ;
';' :> LIT2 c,# 0 ?addReference< 2 allot ;
'.' :> LIT c,# 1 ?addReference< 1 allot ;
',' :> LIT c,# 2 ?addReference< 1 allot ;
'=' :> 0 ?addReference< 2 allot ;
'-' :> 1 ?addReference< 1 allot ;
'_' :> 2 ?addReference< 1 allot ;
'"' :> tok< c@+ cmoveallot ;
'(' ' comment<
'[' ' noop
']' ' noop
'%' ' addMacro<
'}' ' closelambda
'~' ' include
]kvtbl handlers

: parsetok ( c -- )
  handlers swap ?kvexec not if
    stepback tok< dup ?op if nip c,# else
      dup macros find ?dup if nip z[] injectrange else
        ?parsehex if ?, else
          tokstepback $60 c,# 3 ?addReference< 2 allot then then then then ;

:realias parseall ( -- ) begin tonws< nip ?dup while parsetok repeat ;

0 value uxntallen
: uxntal ( dst -- )
  tmp1 reset
  here[ >r
  here to org $100 to binstart
  tok$ 0 labels ! 0 references ! 0 to curlabel 0 macros !
  parseall resolve
  here org - to uxntallen resetasm r> ]here ;
: uxntal<< ( dst -- ) ['] uxntal exec<< ;
: uxntal" ( dst -- ) ['] uxntal [rcompile] " c@+ exec[] ;
