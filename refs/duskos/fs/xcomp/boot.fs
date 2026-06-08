code noop exit,
: !, dir) @, ;
: !+, dir) @+, ;
: -!, dir) -@, ;
: -n, $0 rot - swap +n, ;
: le!, dir) le@, ;
: be!, dir) be@, ;
: dup, PSP) -!, ;
: _ code dup, sys) &) @, exit, ;
$4 _ COMPILING
$c _ SYSDICT
$10 _ FINDSTR
$14 _ [rcnt]
$18 _ FINDCOMPILER
$1c _ SYSALLOC
$48 _ PSORIGIN
$4c _ PSSZ
$50 _ RSORIGIN
$54 _ SYSOFF

: _sv code dup litn [ ' sys) litn ] i) br, _ ;
$8 _sv curalloc) CURALLOC
$28 _sv inptr) INPTR

: ps+, PSP) &) +n, ;
: drop, PSP) @+, ;
: nip, $4 ps+, ;

code in< dup, inptr) @, W) 8b) @, $1 inptr) +n, exit,
: \ $0 begin in< [ nip, $20 i) signed) >=) ?br, drop, popexit, immediate
\ "signed)" is to handle EOF (-1)

: A> dup, A) &) @, ; immediate
: A! A) &) !, ; immediate
: >A A) &) !, drop, ; immediate
: A+ W) &) A>) +, drop, ; immediate
: @A@ dup, A) @, ; immediate
: @A! A) !, drop, ; immediate
: @Ac@ dup, A) 8b) @, ; immediate
: @Ac! A) 8b) !, drop, ; immediate

: findselector sysdict e>wlen A! c@ $40 or @Ac! ;
: compiling code findselector immediate
  dup litn - i) A>) @, FINDCOMPILER m) A>) *, A) &) +, exit, ;

: compile] pushlr, ] ;
: :> here# compile] ;
: code> here# ;
: inline code> over execute exit, compiling ;
: inline, dup inline code i) br, ;
' dup, ' dup compiling dup
' drop, inline drop
' nip, inline nip
:> PSP) @!, ; inline, swap swap,
:> PSP) $4 +) @, $8 ps+, ; inline, 2drop 2drop,
:> dup, PSP) $4 +) @, ; inline, over over,
:> dup, PSP) $8 +) @, ; inline, oover oover,
:> PSP) @!, PSP) $4 +) @!, ; inline, rot rot,
:> PSP) $4 +) @!, PSP) @!, ; inline, rot> rot>,
:> PSP) S>) @, PSP) !, PSP) S>) -!, ; inline, tuck tuck,
:> $fffffff8 ps+, PSP) $4 +) !, PSP) $8 +) S>) @, PSP) S>) !, ;
inline, 2dup 2dup,

: HERE@, curalloc) A>) @, A) S>) @, ;
code , HERE@, S) !, drop, $4 A) +n, exit,

here# ' 32b) ,
: _ code m) !n, exit, ;
' 32b) over _ 32b
' 16b) over _ 16b
' 8b) over _ 8b
code MOD) m) br,

: execute, i) brr, ;
: _ ( xt xtsel -- xt )
  2dup code> pushlr, swap execute, poplr, swap bbr, ( xt xtsel xtcomp )
  code> rot execute rot execute exit, ( xtcomp xtreg ) compiling ;
: multiwidth :> dup [ ' 32b litn ] _ dup [ ' 16b litn ] _ [ ' 8b litn ] _ ;

multiwidth W) MOD) @, ; @ w@ c@
multiwidth PSP) S>) @, W) S>) MOD) !, 2drop, ; ! w! c!
multiwidth S) &) !, drop, S) MOD) @!, ; @! w@! c@!
multiwidth W) S>) MOD) @+, dup, S) &) @, ; @+ w@+ c@+
multiwidth PSP) S>) @, W) S>) MOD) !+, nip, ; !+ w!+ c!+
code +! PSP) S>) @, W) S>) dir) +, 2drop, exit,

code ?dup $0 i) <>) if, dup, then exit,
: ?wnf dup not if (wnf) then ;
: e>xtsel dup e>xt over e>wlen c@ $40 and if execute then nip ;
: find findentry dup if e>xtsel then ;
: find# find ?wnf ;
: 'e word sysdict findentry ?wnf ;
: ' word sysdict find# ;
: ['] ' litn ; immediate
: compile ' litn ['] execute, execute, ; immediate
: exit popexit, ; immediate

: compsel $1 FINDCOMPILER ! ;
: [compile] compsel ' execute, ; immediate
: [rcompile] ' execute, ; immediate
: until dropz, ?brz, ; immediate
: _ code PSP) swap bool, nip, exit, ;
=) _ =   <>) _ <>  >) _ <    <) _ >    >=) _ <=  <=) _ >=

: realias HERE @! swap bbr, HERE ! ;

code nan nip, $0 i) @, exit,
code parsechar
  ' nan $3 i) <>) ?br,
  PSP) A>) @, A) 8b) @+, ' nan $27 ( ' ) i) <>) ?br,
  A) $1 +) 8b) @, ' nan $27 i) <>) ?br,
  A) 8b) @, PSP) !, $1 i) @, exit,

code parsedec
  PSP) A>) @, A) 8b) S>) @,
  $2d ( - ) i) S>) =) if,
    $1 i) -, $1 PSP) +n,
    pushlr, ' parsedec execute, poplr, ( n? f )
    W) &) testz, ifnz, $0 i) S>) @, PSP) dir) S>) swap-, then exit, then
  ' nan $0 i) =) ?br,
  PSP) !, $0 i) @, begin ( u res A=a S=c )
    A) 8b) S>) @+, $30 ( 0 ) i) S>) -,
    ' nan $a i) S>) >=) ?br,
    $a i) *, S) &) +, $1 PSP) -n, ?brnz,
  PSP) !, $1 i) @, exit,

' parse
code parse
  dup, W) 8b) @, ' nan $0 i) =) ?br,
  $1 PSP) +n, PSP) A>) @, A) A>) 8b) @,
  ' parsechar $27 i) A>) =) ?br,
  ' parsedec $24 ( $ ) i) A>) <>) ?br,
  $1 PSP) +n, $1 i) -, ' parsehex bbr,
current swap realias

:> PSP) +, nip, ; inline +
code - PSP) swap-, nip, exit,
code swap- PSP) -, nip, exit,
: adder code> pushlr, over litn compile i) compile +, popexit,
        code> rot i) +, exit, compiling ;
1 adder 1+ -1 adder 1-
2 adder 2+ -2 adder 2-
4 adder 4+ -4 adder 4-
8 adder 8+ -8 adder 8-

code 0< 31 i) >>, exit,
: 0>= 0< not ;
code * PSP) *, nip, exit,
code /mod PSP) @!, PSP) /mod, PSP) S>) !, exit,
: / /mod nip ;
: /+ /mod swap bool + ;
: mod /mod drop ;
code lrot PSP) dir) lrot, PSP) @+, exit,
code rrot PSP) dir) rrot, PSP) @+, exit,
code ?swap ( n n -- l h ) ' swap PSP) <) ?br, exit,
: min ?swap drop ; : max ?swap nip ;
code max0 0 i) signed) <) if, 0 i) !, then exit,
: within? ( n l h -- f ) over - rot> - >= ;
code neg 0 i) swap-, exit,
code inv -1 i) ^, exit,
: align ( a n -- a ) 1- over neg and + ;
: align4 4 align ;
: alignheren here swap align HERE ! ;

: rs+, dup [rcnt] +! RSP) &) +n, ;
: rdrop 4 rs+, ; immediate
: 2rdrop 8 rs+, ; immediate
: r! RSP) -!, -4 [rcnt] +! ; immediate
: r@ dup, RSP) @, ; immediate
: r> dup, RSP) @+, 4 [rcnt] +! ; immediate
: >r [compile] r! drop, ; immediate
: 2>r -8 rs+, PSP) S>) @, RSP) S>) 4 +) !, RSP) !, 2drop, ; immediate
: 2r> -8 ps+, PSP) 4 +) !, RSP) 4 +) @, PSP) !, RSP) @, 8 rs+, ; immediate

: case 0 [compile] r! ; immediate
: of [compile] if ; immediate
: endof [compile] else [compile] r@ ; immediate
: endcase begin ?dup while [compile] then repeat [compile] rdrop ; immediate

: entrylen e>wlen c@ 31 and ;
: entrytag dup entrylen 5 + align4 - @ ;
: tag, alignhere , ;
: entryalias
  dup e>wlen c@ $c0 and
  over entrytag tag, code swap e>xt bbr,
  sysdict e>wlen A! c@ or @Ac! ;
: alias 'e entryalias ;
: @alias code m) br, ;
: const $434e5354 ( CNST ) tag, code litn exit, ;

$60 SYSOFF !
$100 const SYSVARSSZ
alias noop sys#
: sysallot ( -- off ) sys# SYSOFF @ 4 SYSOFF +! A! SYSVARS A+ 0 @A! ;
: sysvar sysallot _sv ;
: sysalias sysallot dup _sv code sys) br, ;

sysvar scrying) SCRYING
: ?scry, scrying) testz, ifnz, swap litn 1 scrying) -n, exit, [compile] then ;
: bind> ( n xt -- xt ) code> rot litn swap bbr, ;
: bind over swap bind> code findselector swap ?scry, litn exit, ;
:> popret bind ;
: does> [ litn ] execute, pushlr, ; immediate

variable _
: :~ here# _ ! compile] ;
: ~ _ @ ; findselector

: _<< i) <<, ; : _>> i) >>, ;
:~ dup ['] _<< bind> inline ['] _>> bind> inline ;
1 ~ 2* 2/
2 ~ 4* 4/
3 ~ 8* 8/
4 ~ 16* 16/
8 ~ 256* 256/

: scnt [ dup, PSP) &) @, ] PSORIGIN @ swap- 4/ 1- ;
: rcnt [ dup, RSP) &) @, ] RSORIGIN @ swap- 4/ ;

sysvar broke) BROKE
: broke? BROKE @ ;
variable _breaklbl
: break 1 i) A>) @, fbr, _breaklbl ! ; immediate

: do [compile] 2>r [compile] ahead here ; immediate
:~ ( ahead brtgt cond -- )
  rot [compile] then RSP) 4 +) S>) @, RSP) S>) swap ?br,
  0 _breaklbl @! ?dup if 0 i) A>) @, [compile] then broke) A>) !, then
  [compile] 2rdrop ;
: loop 1 RSP) +n, >) ~ ; immediate
: -loop RSP) dir) -, drop, <) ~ ; immediate
: +loop RSP) dir) +, drop, >) ~ ; immediate

: i dup, RSP) @, ; immediate
: j dup, RSP) 8 +) @, ; immediate

: _, ( a u n -- ) PSP) S>) @, PSP) 4 +) A>) @, 8 ps+, execute drop, exit, ;
code fill ' fill, _,
code cfill ' cfill, _,

: HEREMAX CURALLOC @ 4+ ;
: NEWHERE CURALLOC @ 8+ ;
here $100000 + HEREMAX ! \ wild guess
alias noop oom
current NEWHERE !
: newhere ( -- a u ) NEWHERE @ execute ;
variable PREVRESERVE
:> ( n -- )
  newhere rot over > if oom then ( a u )
  over HERE ! over PREVRESERVE ! + HEREMAX ! ;
code reserve ( n -- )
  HERE@,
  PREVRESERVE m) S>) !,
  W) &) S>) +,
  ' oom A) ( HERE ) S>) <) ?br,
  ( :> ) A) 4 +) ( HEREMAX ) S>) >) ?br,
  drop, exit,
: allot dup reserve HERE +! ;
: allot@ dup reserve here swap allot ;
: allot0 dup allot@ swap 0 cfill ;
: , 4 allot@ ! ;
: w, 2 allot@ w! ;
: c, 1 allot@ c! ;
: n,@ alignhere dup 4* allot@ r! swap 0 do !+ loop drop r> ;

code le@ W) le@, exit,
code wle@ W) 16b) le@, exit,
code le! PSP) S>) @+, W) S>) le!, drop, exit,
code wle! PSP) S>) @+, W) 16b) S>) le!, drop, exit,
: le, 4 allot@ le! ;
: wle, 2 allot@ wle! ;
code be@ W) be@, exit,
code wbe@ W) 16b) be@, exit,
code be! PSP) S>) @+, W) S>) be!, drop, exit,
code wbe! PSP) S>) @+, W) 16b) S>) be!, drop, exit,
: be, 4 allot@ be! ;
: wbe, 2 allot@ wbe! ;

:~ S) &) !, PSP) A>) @, nip, PSP) @!, ( orig-u n )
   execute ifz, PSP) S>) !, 1 i) @, exit, [compile] then nip, 0 i) @, exit, ;
code idx ' idx, ~
code cidx ' cidx, ~

code move S) &) !, PSP) A>) @, 2drop, move, drop, exit,
code cmove
  PSP) A>) @+, S) &) !, swap,
  W) &) S>) |, A) &) S>) |, 3 i) S>) &,
  ifz, PSP) S>) @, 2 i) S>) >>, move, else PSP) S>) @, cmove, then
  2drop, exit,

$20 const SPC $0d const CR $0a const LF $08 const BS $1b const ESC
sysalias rtype) RTYPE rtype
variable CONSOLETYPE
: console! dup CONSOLETYPE ! RTYPE ! ;
' 2drop console!
create _ 1 allot
: c[] _ tuck c! 1 ;
: emit c[] rtype ;
: nl> LF emit ; : spc> SPC emit ;
: stype c@+ rtype ;
create _ 'n' c, 'r' c, '0' c, LF c, CR c, 0 c,
: str< begin in< [
  '"' i) =) if, 0 i) @, popexit, then
  '\' i) =) if,
    ] in< nip [
    LF i) =) if, drop, rot bbr, then
    _ i) A>) @, 3 i) S>) @, cidx,
    ifz, S) _ 3 + +) 8b) @, then then
  ] 1 ;

: cmoveallot dup allot@ swap cmove ;
: strmove over c@ 1+ cmove ;
: s, dup c@ 1+ cmoveallot ;

$100 const STR_MAXSZ
create pool( $1000 allot here const )pool
create ptr pool( ,
variable curstr

alias noop strovfl
: ?err ( n -- n ) dup STR_MAXSZ > if pool( ptr ! strovfl then ;
:~ ( n -- ) ptr @ + )pool >= if pool( ptr ! then ;
: strallot dup ~ ptr @ swap ptr +! ;
: newstr STR_MAXSZ ~ ptr @ dup curstr ! 1+ ;
: endstr dup ptr ! curstr @ A! - ?err 1- @Ac! A> ;
:~ ( -- str ) newstr begin str< while swap c!+ repeat endstr ;
: ," ~ c@+ cmoveallot ;
:> fbr, here ~ s, swap alignhere [compile] then litn ;
' ~ compiling "
:> [compile] " compile stype ;
:> ~ stype ;
compiling ."
:~ 0 begin str< while swap 256* or repeat ;
:> ~ litn ;
' ~ compiling n"

: idle noop ;
: .. ." ." idle ;
:> ." boot failure" begin idle again ;
create MAINLOOP ' main ,
create ABORTPTR ,
ABORTPTR @alias abort
create QUITHOOK ' noop ,
QUITHOOK @alias quithook
:~ here# QUITHOOK @! pushlr, execute, ;
: :quithook ~ ] ;
: addquithook ~ poplr, bbr, ;
code quit
  0 COMPILING m) !n,
  SYSALLOC curalloc) !n,
  CONSOLETYPE m) @, rtype) !,
  ' quithook execute, MAINLOOP m) br,
code (abort) PSORIGIN m) @, PSP) &) !, ' quit bbr,
: abort" [compile] ." compile abort ; immediate
: ?abort" [compile] if [compile] abort" [compile] then ; immediate

:~ abort" stack out of bounds" ;
here# PSORIGIN m) A>) @, PSP) &) A>) -,
  ' ~ PSSZ m) A>) >=) ?br, exit,
( a ) ' stack? realias

: boolz, ifz, 1 i) @, exit, [compile] then 0 i) @, exit, ;
:~ S) &) !, PSP) A>) @, 2drop, execute boolz, ;
code []= ' []=, ~
code c[]= ' c[]=, ~

code s=
  PSP) A>) @+, W) 8b) S>) @, 1 i) S>) +, c[]=, boolz,

: [if] not if " [then]" begin word over s= until drop then ;
alias noop [then]

create buf( 11 allot here const )buf
: formatdecu ( n -- a u )
  )buf >A begin 10 /mod ( r q A=a )
    swap '0' + -1 A+ @Ac! ?dup not until ( )
  A> )buf A> - ;
: formatdec
  dup 0< if neg formatdecu 1+ '-' rot 1- tuck c! swap else formatdecu then ;
: . formatdec rtype ;
create _ ," 0123456789abcdef"
:~ ( n sz -- a u )
  r! buf( 1- tuck + do dup $f and _ + c@ i c! 16/ 1 -loop ( n )
  drop buf( r> ;
: formathex1 2 ~ ;
: formathex2 4 ~ ;
: formathex 8 ~ ;
: .x1 formathex1 rtype ;
: .x2 formathex2 rtype ;
: .x formathex rtype ;

sysvar insz) INSZ
-1 INSZ !
-1 const EOF
: endofstream 0 ;
create NEXTIN< ' endofstream , ( -- ?a u-or-0 )
NEXTIN< @alias nextin<
variable echoin variable stepped?
code _ ( -- c )
  insz) testz, ifz,
    pushlr, NEXTIN< m) brr, poplr, 0 i) =) if,
      inptr) !, EOF i) @, 1 stepped? m) +n, exit, then
    insz) !, drop, inptr) !, drop, current bbr, then
  1 insz) -n, dup, inptr) @, W) 8b) @, 1 inptr) +n,
  0 i) S>) @, stepped? m) S>) @!, 0 i) S>) =) if,
    echoin m) testz, ifnz, dup, ' emit bbr, then then
  exit,
current ' in< realias
: in<# in< dup 0< ?abort" unexpected EOF" ;
: stepback 1 stepped? @! not if 1 INSZ +! -1 INPTR +! then ;
: peekback stepped? @ if 0 else INPTR @ 1- c@ then ;
: \s begin in< EOF = until ;

: align# mod ?abort" alignment error" ;

: []>r
  dup, compile align4 4 i) +, ( src u n )
  RSP) &) dir) -, PSP) S>) @, RSP) S>) !, 2drop,  ( src )
  RSP) &) 4 +) A>) @, cmove, drop, ; immediate
: str>r compile c@+ [compile] []>r ; immediate
: r>[]
  dup, RSP) @, 1 i) +, compile strallot 1 i) +, ( W=a )
  dup, A) &) !, RSP) S>) @+, PSP) S>) -!, ( a u W=A A=a )
  RSP) &) @, cmove, ( a u W=src-end )
  compile align4 RSP) &) !, drop, ; immediate
: r>str [compile] r>[] A) &) !, drop, 1 i) -, W) 8b) A>) !, ; immediate

variable NEXTWORD
variable CURWORD
: toword? begin in< A! SPC > until A> EOF <> stepback ;
: toword# toword? not ?abort" word expected" ;
: ?charlit ( a c -- a c )
  over curstr @ A! - 3 = if
    A> 1+ c@ ''' = if
      in< ''' = if ''' rot c!+ swap else stepback then
    then then ;

' word
: word
  0 NEXTWORD @! ?dup if dup CURWORD ! exit then
  newstr toword# ( a )
  0 begin ( a c ) in< nip [
    SPC i) signed) >) if, \ why signed? for EOF
    PSP) A>) @, A) 8b) !, 1 PSP) +n,
    swap '"' i) <>) ?br, ( exitjmp )
    ' ?charlit execute, then ] ( a c )
  drop endstr dup CURWORD ! ;

current swap realias
\ we now have fancy double-quotes
: wordorquote ( -- str )
  toword# in< '"' <> if stepback word else
    [rcompile] " dup CURWORD ! then ;
: eol?
  peekback begin
    LF <> while
    in< dup '\' = if drop in< drop in< then
    dup SPC <= while repeat
    EOF = stepback else 1 then ;

: interpret begin toword? while run1 repeat ;
: word" [compile] " NEXTWORD m) !, drop, ; immediate

: exec[] ( xt a u -- )
  ['] endofstream NEXTIN< @! >r INSZ @! >r INPTR @! >r ( xt )
  execute r> INPTR ! r> INSZ ! r> NEXTIN< ! ;
: interpret[] ( a u -- ) ['] interpret rot> exec[] ;

: entry c@+ entry[] ;
: entryname[] A! entrylen A> e>wlen over - swap ;
: :realias ' >r :> r> realias ;
:realias (wnf) FINDSTR @ stype abort" word not found" ;
:realias (div0) abort"divide by zero" ;
:realias oom abort"out of memory" ;
:realias strovfl abort"string overflow" ;
:realias sys# SYSOFF @ SYSVARSSZ >= ?abort"out of SYSVARS" ;
:realias halerr abort"HAL error" ;

: tagged# over @ <> ?abort"wrong tag" 4+ ;
: findtagged
  findentry dup if dup entrytag rot <> if drop 0 then else nip then ;

: n< scnt >r begin run1 scnt r@ > until ( ... )
     scnt r> - 1 <> ?abort"n< overflow" ( n ) ;
: consts begin n< const eol? until ;
: enum 0 begin dup const 1+ eol? until drop ;
enum ZERO ONE
: map< ' >r begin n< r@ execute eol? until rdrop ;

: ?scryentry 1 SCRYING ! e>xtsel 0 SCRYING @! dup if nip then not ;
: scryentry# ?scryentry not ?abort"inscrutable!" ;
: scryfind# sysdict findentry ?wnf scryentry# ;
: scry word scryfind# ;

variable to?
: to?, 0 i) S>) @, to? m) S>) @!, S) &) testz, ;
: to# 0 to? @! ?abort"invalid to usage" ;
: to' 1 to? ! ' to# ;
:> compsel to' execute ; :> to' execute ; compiling to
: getset, ( sxt gxt axt scry tag -- )
  2>r
  code> >r pushlr, dup execute, over execute, popexit,
  code> >r pushlr, dup execute rot execute popexit,
  code> >r pushlr, dup execute, over execute, popexit,
  code> >r pushlr, execute swap execute popexit, ( )
  2r> 2r> 2r> ?dup if tag, then
  code findselector immediate
  ?dup if ?scry, then
  dup, FINDCOMPILER m) A>) @, to?, ifz, ( sc s gc g jmp )
    over i) @, rot> - i) S>) @, ( sc s jmp )
    [compile] else
    over i) @, rot> - i) S>) @,
    [compile] then A) &) S>) *, S) &) +, exit, ;

: do@, dup, @, ;
: do!, !, drop, ;
: value
  1 n,@ >r ['] do!, ['] do@,
  r@ ['] m) bind>
  r> n"VALU" getset, ;

:~ [rcnt] @ - RSP) swap +) ;
: _
  neg 4- >r ['] do!, ['] do@,
  r@ ['] ~ bind>
  r> n"LVAR" getset, ;
0 _ V1 4 _ V2 8 _ V3 12 _ V4

: llend begin dup @ ?dup while nip repeat ;
: lladd here# swap llend ! 0 , ;
code llfind
  0 i) S>) @, PSP) S>) @!, begin \ W=ll S=tofind PSP+0=idx
    0 i) <>) if,
    1 PSP) +n,
    W) S>) <>) if,
    W) @, rot bbr, then then exit,

sysdict llend const KERNELSTART
'e HERESTART @ e>xt const KERNELEND

alias execute | immediate
: bi dup, ['] swap, ; immediate
: dupbi dup, ['] over, ; immediate
: tri dup, ['] rot, ['] over, ; immediate
: dip [compile] >r ['] r> ; immediate

:~ dup entrytag n"FILD" = if [compile] r> then
  1 to? ! compsel e>xtsel to# execute ;
: doto
  'e dup entrytag n"FILD" = if [compile] r! then
  dup compsel e>xtsel execute ['] ~ ; immediate

4 adder unitbottom
8 adder unittop
12 adder unitname
variable currentunit
: loadunit ( str -- ) abort"fs/core not loaded" ;
: ensureunit ( str -- ) dup sysdict findentry if drop else loadunit then ;
: bubbleup ( bottom top -- )
  ?dup if dup SYSDICT @! ( ulow utop oldsys )
    tuck llfind nip ( ulow oldsys graftsrc )
    ?dup if >A swap @! @A! else 2drop then
    else ( ulow ) drop then ;
: endunit ( -- )
  currentunit @ ?dup not if exit then
  dup unittop @ if drop exit then
  sysdict over unittop ! ( unit )
  dup unitbottom @ sysdict llfind nip ( unit prev )
  ?dup not ?abort"broken unit"
  swap unitbottom ! ;
:~ ( e dict' -- e xt ) nip @ word swap findentry ?wnf ( e ) dup e>xtsel ;
: mkunit ( unit -- ) code findselector dup ?scry, unittop litn ['] ~ bbr, ;
: unit
  endunit here# dup currentunit @! , 0 , 0 , ( unit )
  word dup NEXTWORD ! s, dup mkunit ( unit )
  sysdict swap unitbottom ! ;
unit xcomp/boot
: bump bi unitbottom @ | unittop @ bubbleup ;
: needs
  endunit 0 >r ( sentinel )
  begin word dup str>r eol? >r ensureunit r> until
  0 begin r@ while r>str scryfind# swap 1+ repeat ( ... n )
  rdrop 0 do bump loop ;
: needs" [compile] " compile scryfind# compile bump ; immediate
endunit
SYSDICT llend currentunit @ unitbottom !
