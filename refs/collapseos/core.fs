( ----- 000 )
MASTER INDEX

001 Useful little words       010 Block editor
025 Memory Editor             030-099 unused
100 Z80 assembler
110 8086 assembler            120 6502 assembler/disassembler
130 6809 assembler            140 AVR assembler
150-199 unused
200 Z80 boot code             215 8086 boot code
225 6502 boot code            240 6809 boot code
250-299 unused
300 Cross compilation
310 Core words                330 BLK subsystem
340 Grid subsystem            345 RX/TX subsystem
350-399 unused                400+ Machine-specific content
( ----- 001 )
\ Useful little words. CRC16[] MOVE-
'? crc16[] [if] \S [then]
\ Compute CRC16 over a memory range
: crc16[] ( a u -- c ) swap >A 0 swap for Ac@+ crc16 next ;
: move- ( a1 a2 u -- ) \ *A* move starting from the end
  ?dup if >r over - ( a1 diff ) swap r@ + >A
    r> for ( diff ) A- A> over + Ac@ swap c! next drop
  else 2drop then ;
( ----- 002 )
\ Useful little words. MEM>BLK BLK>MEM
\ *A* Copy an area of memory into blocks.
: mem>blk ( addr blkno blkcnt )
  for ( a blk )
    dup blk@ 1+ swap dup blk( $400 move blk!! $400 + swap next
  drop flush ;
\ *A* Copy subsequent blocks in an area of memory
: blk>mem ( blkno blkcnt addr )
  rot> for ( a blk )
    dup blk@ 1+ swap blk( over $400 move $400 + swap next
  drop ;
( ----- 003 )
\ Context. Allows multiple concurrent dictionaries.
\ See doc/usage.txt

0 value saveto \ where to save CURRENT in next switch
: context doer current , does> ( a -- )
  saveto if current to saveto then ( a )
  dup to saveto ( a )
  @ current ! ;
( ----- 004 )
\ string manipulation.
'? >s [if] \s [then]
2 values sa sl
: >s ( sa sl -- ) to sl to sa ; : s> sa sl ;
: cutr ( n -- ) sl -^ dup 0< if drop 0 then to sl ;
: cutl ( n -- ) sl swap cutr sl - sa + to sa ;
: prefix? ( sa sl -- f )
  dup sl > if 2drop 0 exit then sa rot> []= ;
: suffix? ( sa sl -- f )
  dup sl > if 2drop 0 exit then sl over - ( sa sl off ) sa +
  ( sa sl sa2 ) swap []= ;
( ----- 005 )
\ Word table. See doc/wordtbl
: wordtbl ( n -- a ) create here swap cells allot0 1 here c! ;
: w+ ( a -- a+2? ) cell + dup @ if drop then ;
: :w ( a -- a+2? ) here xtcomp over ! w+ ;
: 'w ( a -- a+2? ) ' over ! w+ ;
: wexec ( tbl idx -- ) cells + @ execute ;
( ----- 006 )
\ Pager. See doc/pager
4 values ''EMIT ''KEY? chrcnt lncnt
20 value PGSZ
: realKEY begin ''KEY? execute until ;
: back ''EMIT EMIT ! ''KEY? KEY? ! ;
: _emit ( c -- )
  chrcnt 1+ to chrcnt
  dup CR = chrcnt LNSZ = or if
   0 to chrcnt lncnt 1+ to lncnt then
  ''EMIT execute lncnt PGSZ = if
    0 to lncnt nl> ." Press q to quit, any key otherwise" nl>
    realKEY 'q' = if back quit then then ;
: _key? back key? ;
: page EMIT @ to ''EMIT KEY? @ to ''KEY?
  ['] _emit EMIT ! ['] _key? KEY? ! ;
( ----- 007 )
\ Flow words
'? pc not [if] alias here pc [then]
'? pc2a not [if] : pc2a ; [then]
alias pc begin,
: br ( tgt -- rel ) pc - ;
: fjr begin, 0 ;
: ifz, fjr jrnz, ; : ifnz, fjr jrz, ;
: ifc, fjr jrnc, ; : ifnc, fjr jrc, ;
: fjr! ( tgt -- ) dup pc -^ swap pc2a jr! ;
: then, fjr! ; : else, fjr jr, swap fjr! ;
( ----- 010 )
\ Block editor. see doc/ed.
\ Cursor position in buffer. EDPOS/64 is line number
0 value edpos
create ibuf LNSZ 1+ allot0 \ counted string, first byte is len
create fbuf LNSZ 1+ allot0
: l blk> ." Block " dup . nl> list ;
: b blk> 1- blk@ l ; : n blk> 1+ blk@ l ;
: ibuf+ ibuf 1+ ; : fbuf+ fbuf 1+ ;
: ilen ibuf c@ ; : flen fbuf c@ ;
: edpos! to edpos ; : edpos+! edpos + edpos! ;
: 'pos ( pos -- a, addr of pos in memory ) blk( + ;
: 'edpos edpos 'pos ;
( ----- 011 )
\ Block editor, private helpers
: _lpos ( ln -- a ) LNSZ * 'pos ;
: _pln ( ln -- ) \ print line no ln with pos caret
  dup _lpos dup >A lnlen 1 max for ( lno )
    A> 'edpos = if '^' emit then
    Ac@+ SPC max emit next ( lno ) spc> 1+ . ;
: _zline ( a -- ) LNSZ SPC fill ; \ zero-out a line
: _type ( buf -- ) \ *A* type into buf until end of INBUF
  in<? ?dup not if drop exit then over 1+ dup _zline >A begin
    ( buf c ) Ac!+ in<? ?dup not until ( buf )
  A> over - 1- ( buf len ) swap c! ;
( ----- 012 )
\ Block editor, T P U
\ user-facing lines are 1-based
: t 1- dup LNSZ * edpos! _pln ;
: p ibuf _type ibuf+ 'edpos LNSZ move blk!! ;
: _mvln+ ( ln -- move ln 1 line down )
    dup 14 > if drop exit then
    _lpos dup LNSZ + LNSZ move ;
: _u ( U without P, used in VE )
  15 edpos LNSZ / - ?dup if
    14 swap for dup _mvln+ 1- next drop then ;
: u _u p ;
( ----- 013 )
\ Block editor, F i
: _f ( F without _type and _pln. used in VE )
  'edpos 1+ begin ( a )
    fbuf+ c@ over blk) over - ( a c a u ) cidx
    not if drop exit then ( a idx ) + ( a )
    dup fbuf+ flen []= if blk( - edpos! exit then 1+ again ;
: f fbuf _type _f edpos LNSZ / _pln ;
: _rbufsz ( size of linebuf to the right of curpos )
  edpos LNSZ mod LNSZ -^ ;
: _i ( i without _pln and _type. used in VE )
  _rbufsz ilen over < if ( rsize )
    ilen - ( chars-to-move )
    'edpos dup ilen + rot ( a a+ilen ctm ) move- ilen
  then ( len-to-insert )
  ibuf+ 'edpos rot move ( ilen ) blk!! ;
: i ibuf _type _i edpos LNSZ / _pln ;
( ----- 014 )
\ Block editor, X E Y
: icpy ( n -- copy n chars from cursor to IBUF )
  dup ibuf c! ibuf+ _zline 'edpos ibuf+ ( n a buf ) rot move ;
: _del ( n -- ) ?dup not if exit then _rbufsz min
  'edpos 2dup + ( n a1 a1+n ) swap _rbufsz move ( n )
  \ get to next line - n
  dup edpos $ffc0 and $40 + -^ 'pos ( n a )
  swap SPC fill blk!! ;
: _x ( n -- ) ?dup not if exit then _rbufsz min dup icpy _del ;
: x _x edpos LNSZ / _pln ;
: _e flen _x ;
: e flen x ;
: y fbuf ibuf LNSZ 1+ move ;
( ----- 015 )
\ Visual text editor. VALUEs, lg? width pos@ mode! ...
3 values PREVPOS xoff ACC
LNSZ 3 + value MAXW
10 value MARKCNT
create MARKS MARKCNT << cells allot0 \ 4b: blk/edpos
: nspcs ( pos n ) SPC fillc ;
: lg? COLS MAXW > ; : col- MAXW COLS min -^ ;
: width lg? if LNSZ else COLS then ;
: acc@ ACC 1 max ; : pos@ ( x y -- ) edpos LNSZ /mod ;
: num ( c -- ) \ c is in range 0-9
  '0' - ACC 10 * + to ACC ;
: mode! ( c -- ) 4 col- cell! ;
( ----- 016 )
\ VE, rfshln contents selblk pos! xoff? setpos
: _ ( ln -- ) \ refresh line ln
  dup _lpos xoff + swap 3 + COLS * lg? if 3 + then
  width cells! ;
: rfshln pos@ nip _ ; \ refresh active line
: contents 16 0 swap for dup _ 1+ next drop ;
: selblk blk@ contents ;
: pos! ( newpos -- ) edpos to PREVPOS
    dup 0< if drop 0 then 1023 min edpos! ;
: xoff? pos@ drop ( x )
  xoff ?dup if < if 0 to xoff contents then else
    width >= if LNSZ COLS - to xoff contents then then ;
: setpos ( -- ) pos@ 3 + ( header ) swap ( y x ) xoff -
  lg? if 3 + ( gutter ) then swap at-xy ;
: 'mark ( -- a ) ACC MARKCNT mod << cells MARKS + ;
( ----- 017 )
\ VE, cmv buftype bufprint bufs
: cmv ( n -- , char movement ) acc@ * edpos + pos! ;
: buftype ( buf ln -- ) \ type into buf at ln
  3 over at-xy key dup SPC < if 2drop drop exit then ( b ln c )
  swap COLS * 3 + 3 col- nspcs ( buf c )
  in( swap lntype drop begin ( buf a ) key lntype until
  in( - ( buf len ) swap c!+ in( swap LNSZ move in$ ;
: _ ( buf s pos ) tuck stypec ( buf pos ) 3 + stypec ;
: bufs ( -- ) \ refresh I and F lines
  ibuf S" I: " COLS _ fbuf S" F: " COLS 2 * _ ;
: insl _u edpos $3c0 and dup pos! 'pos _zline blk!! contents ;
( ----- 018 )
\ VE cmds
31 value cmdcnt
create cmdl ," G[]IFnNYEXChlkjHLg@!wWb&mtfROoD"
cmdcnt wordtbl cmds
:w ( G ) ACC selblk ;
:w ( [ ) blk> acc@ - selblk ; :w ( ] ) blk> acc@ + selblk ;
: insert 'I' mode! ibuf 1 buftype _i bufs rfshln ;
'w insert ( I )
:w ( F ) 'F' mode! fbuf 2 buftype _f bufs setpos ;
:w ( n ) _f setpos ;
:w ( N ) edpos _f edpos = if 0 edpos! acc@ for
    blk> 1+ blk@ _f edpos if leave then next
    contents setpos then ;
:w ( Y ) y bufs ; :w ( E ) _e bufs rfshln ;
:w ( X ) acc@ _x bufs rfshln ;
:w ( C ) flen _del rfshln insert ;
( ----- 019 )
\ VE cmds
:w ( h ) -1 cmv ; :w ( l ) 1 cmv ;
:w ( k ) -64 cmv ; :w ( j ) 64 cmv ;
: bol edpos $3c0 and pos! ;
'w bol ( H )
:w ( L ) edpos dup $3f or 2dup = if 2drop exit then swap begin
    ( res p ) 1+ dup 'pos c@ ws? not if nip dup 1+ swap then
    dup $3f and $3f = until drop pos! ;
:w ( g ) ACC 1 max 1- 64 * pos! ;
:w ( @ ) blk> blk( (blk@) 0 BLKDTY ! contents ;
:w ( ! ) blk> flush BLK> ! ;
( ----- 020 )
\ VE cmds
: c@- dup 1- swap c@ ;
: word>> begin c@+ ws? until ;
: ws>> begin c@+ ws? not until ;
: word<< begin c@- ws? until ;
: ws<< begin c@- ws? not until ;
: bpos! blk( - pos! ;
:w ( w ) 'edpos acc@ for word>> ws>> next 1- bpos! ;
:w ( W ) 'edpos acc@ for ws>> word>> next 1- bpos! ;
:w ( b ) 'edpos acc@ for 1- ws<< word<< next 1+ 1+ bpos! ;
:w ( & ) wipe contents ;
:w ( m ) blk> 'mark ! edpos 'mark 1+ 1+ ! ;
:w ( t ) 'mark 1+ 1+ @ pos! 'mark @ selblk ;
( ----- 021 )
\ VE cmds
:w ( f ) edpos PREVPOS 2dup = if 2drop exit then
  2dup > if dup pos! swap then
  ( p1 p2, p1 < p2 ) over - LNSZ min ( pos len ) dup fbuf c!
  fbuf+ _zline swap 'pos fbuf+ ( len src dst ) rot move bufs ;
:w ( R ) 'R' mode! begin
  setpos key dup bs? if -1 edpos+! drop 0 then
  dup SPC >= if
  dup emit 'edpos c! 1 edpos+! blk!! 0 then until ;
'w insl ( O )
:w ( o ) edpos $3c0 < if edpos 64 + edpos! insl then ;
:w ( D ) bol LNSZ icpy acc@ LNSZ * ( delsz ) blk) 'edpos - min
  >r 'edpos r@ + 'edpos ( src dst )
  blk) over - move blk) r@ - r> SPC fill blk!! bufs contents ;
( ----- 022 )
\ VE final: status nums gutter handle VE
: status 0 $20 nspcs 0 0 at-xy ." BLK" spc> blk> . spc> ACC .
  spc> pos@ 1+ . ',' emit . xoff if '>' emit then spc>
  BLKDTY @ if '*' emit then SPC mode! ;
: nums 16 for r@ here fmtd r@ 2 + COLS * stypec next ;
: gutter lg? if 19 for
  '|' r@ 1- COLS * MAXW + cell! next then ;
: handle ( c -- f )
  dup '0' '9' =><= if num 0 exit then
  dup cmdl cmdcnt cidx if cmds swap wexec then
  0 to ACC 'q' = ;
: ve blk> 0< if 0 blk@ then
  clrscr 0 to ACC 0 to PREVPOS
  nums bufs contents gutter
  begin xoff? status setpos key handle until 0 19 at-xy ;
( ----- 025 )
\ Memory Editor. See doc/me
create CMD 2 c, '#' c, 0 c,
\ POS is relative to ADDR
4 values ADDR POS HALT? ASCII?
16 value AWIDTH
LINES 2 - value AHEIGHT
AHEIGHT AWIDTH * value PAGE
COLS 33 < [if] 8 to AWIDTH [then]
: addr ADDR POS + ;
create _ ," 0123456789abcdef"
: hex! ( c pos -- )
  over 16 / _ + c@ over cell! ( c pos )
  1+ swap $f and _ + c@ swap cell! ;
: bottom 0 LINES 1- at-xy ;
( ----- 026 )
\ Memory Editor, line rfshln contents showpos
: line ( ln -- )
  dup AWIDTH * ADDR + >A 1+ COLS * ( pos )
  ':' over cell! A> <<8 >>8 over 1+ hex! 4 + ( pos+4 )
  AWIDTH >> A> rot> for ( a-old pos )
    Ac@+ ( a-old pos c ) over hex! ( a-old pos )
    1+ 1+ Ac@+ over hex! 3 + ( a-old pos+5 ) next
  swap >A AWIDTH for ( pos )
    Ac@+ dup SPC - $5e > if drop '.' then over cell! 1+ next
  drop ;
: rfshln POS AWIDTH / line ;
: contents LINES 2 - for r@ 1- line next ;
: showpos
  POS AWIDTH /mod ( r q ) 1+ swap ( y r ) ASCII? if
  AWIDTH >> 5 * + else dup 1 and << swap >> 5 * + then
  4 + ( y x ) swap at-xy ;
( ----- 027 )
\ Memory Editor, addr! pos! status type typep
: addr! $fff0 and to ADDR contents ;
: pos! dup 0< if PAGE + then dup PAGE >= if PAGE - then
  to POS showpos ;
: status 0 COLS SPC fillc
  0 0 at-xy ." A: " ADDR .X spc> ." C: " POS .X spc> ." S: "
  psdump POS pos! ;
create _buf 0 c, '$' c, 4 allot \ always hex
: type ( cnt -- s ) _buf 2 + >A for
  key dup SPC < if drop leave else dup emit Ac!+ then next
  A> _buf - 1- _buf c! _buf ;
: typep ( cnt -- n? f )
  type dup c@ if parse else drop 0 then ;
( ----- 028 )
\ Memory Editor, almost all actions
: #] ADDR PAGE + addr! ; : #[ ADDR PAGE - addr! ;
: #J ADDR $10 + addr! POS $10 - pos! ;
: #K ADDR $10 - addr! POS $10 + pos! ;
: #l POS 1+ pos! ; : #h POS 1- pos! ;
: #j POS AWIDTH + pos! ; : #k POS AWIDTH - pos! ;
: #m addr ; : #@ addr @ ; : #! addr ! contents ;
: #g scnt if dup ADDR - PAGE < if
  ADDR - pos! else dup addr! $f and pos! then then ;
: #G bottom 4 typep if #g then ;
: #a ASCII? not to ASCII? showpos ;
: #f #@ #g ; : #e #m #f ;
: _h spc> showpos 2 typep ;
: _a showpos key dup SPC < if drop 0 else dup emit 1 then ;
: #R begin spc> ASCII? if _a else _h then ( n? f ) if
    addr c! rfshln #l 0 else 1 then until rfshln ;
( ----- 029 )
\ Memory Editor, #q handle ME
: #q 1 to HALT? ;
: handle ( c -- f ) CMD 2 + c! CMD find ?dup if execute then ;
: me 0 to HALT? clrscr contents 0 pos! begin
    status key handle HALT? until bottom ;
( ----- 100 )
\ Z80 Assembler. Operands. See doc/asm. Requires B5
: >>3 >> >> >> ; : <<3 << << << ; : <<4 <<3 << ;
: opreg 7 and ; : optype >>3 3 and ;
create nbank 8 cells allot
0 value nbank>
: nbank@ ( op -- n ) opreg cells nbank + @ ;
: nbank! ( n -- idx )
  nbank> tuck cells nbank + ! dup 1+ opreg to nbank> ;
28 consts
  $00 B  $01 C  $02 D  $03 E  $04 H  $05 L  $06 (HL)  $07 A
  $08 BC $09 DE $0a HL $0b AF $0b SP
  $20 (BC) $21 (DE) $22 (SP) $23 AF' $24 I $25 R $26 (C)
  $00 CNZ $01 CZ $02 CNC $03 CC $04 CPO $05 CPE $06 CP $07 CM
: i) nbank! $10 or ;         : m) nbank! $18 or ;
: ix, $dd c, ; : iy, $fd c, ; : IX ix, HL ; : IY iy, HL ;
: _ <<8 (HL) or $40 or ; : ix+) ix, _ ; : iy+) iy, _ ;
( ----- 101 )
\ Z80 Assembler. Checks, asserts, util
: err abort" argument error" ;
: # ( f -- ) not if err then ;
: HL# HL = # ; : A# A = # ;
: 8b? optype 0 = ; : 16b? optype 1 = ; : ixy+? $40 and ;
: special? $20 and ;
: 8b# 8b? # ;
: opexec ( op tbl -- ) swap optype wexec ;
: opcode, ( opcode -- ) dup >>8 ?dup if c, then c, ;
: ?ixy+, ( op -- ) dup ixy+? if >>8 c, else drop then ;
( ----- 102 )
\ Z80 Assembler. sub, and, or, xor, cp,
: _reg8, over opreg or opcode, ?ixy+, ;
: _imm, $46 or opcode, nbank@ c, ;
4 wordtbl _ ( op code -- )
  'w _reg8, 'w err 'w _imm, 'w err
: 8bari, ( A op code -- ) rot A# over _ opexec ;
: op doer , does> ( A op 'code -- ) @ 8bari, ;
$a0 op and,               $b8 op cp,
$b0 op or,                $90 op sub,
$a8 op xor,
( ----- 103 )
\ Z80 Assembler. rl, rr, rlc, rrc, sla, srl, bit, set, res,
4 wordtbl _ ( op code -- )
'w _reg8, 'w err 'w err 'w err
: op doer , does> ( op 'code ) @ over _ opexec ;
$cb10 op rl,   $cb18 op rr,   $cb00 op rlc,  $cb08 op rrc,
$cb20 op sla,  $cb38 op srl,
: op doer , does> ( op b 'code ) @ swap <<3 or over _ opexec ;
$cbc0 op set,      $cb80 op res,     $cb40 op bit,
( ----- 104 )
\ Z80 Assembler. inc, dec, add, adc, sbc,
: _reg8<<, @ over opreg <<3 or c, ?ixy+, ;
: _reg16<<, cell + @ swap opreg <<4 or opcode, ;
: _ixy+<<, c, (HL) swap _reg8<<, nbank@ c, ;
4 wordtbl _ ( op 'codes -- )
  'w _reg8<<, 'w _reg16<<, 'w err 'w err
: op doer ( 8b ) , ( 16b ) , does> ( op codes ) over _ opexec ;
$03 04 op inc,      $0b 05 op dec,
: op doer ( 8b ) , ( 16b ) , does> ( dst src 'codes -- )
  over 16b? if rot HL# _reg16<<, else @ 8bari, then ;
$09 $80 op add,   $ed4a $88 op adc,   $ed42 $98 op sbc,
( ----- 105 )
\ Z80 Assembler. push, pop, in, out, rst,
4 wordtbl _ ( op 'codes -- )
'w err 'w _reg16<<, 'w err 'w err
: op doer 0 , , does> ( op 'code -- ) over _ opexec ;
$c5 op push,                $c1 op pop,
: _A, ( n in? ) <<3 $d3 or c, nbank@ c, ;
: _C, ( reg in? ) not $ed40 or swap <<3 or opcode, ;
: _inout, ( op n-or-C in? )
  over (C) = if nip _C, else rot drop _A, then ;
: in, 1 _inout, ;  : out, swap 0 _inout, ;
: rst, ( n ) $c7 or c, ;
create _ 9 nc, AF DE (SP) AF' HL HL $08 $eb $e3
: ex, ( op1 op2 -- ) swap _ 3 cidx #
  3 + _ + dup c@ rot = # 3 + c@ c, ;
( ----- 106 )
\ Z80 Assembler. Inherent ops
: op doer , does> @ opcode, ;
$f3 op di,     $fb op ei,     $d9 op exx,    $76 op halt,
$00 op nop,    $37 op scf,    $3f op ccf,    $c9 op ret,
$17 op rla,    $07 op rlca,   $1f op rra,    $0f op rrca,
$eda1 op cpi,  $edb1 op cpir, $eda9 op cpd,  $edb9 op cpdr,
$ed46 op im0,  $ed56 op im1,  $ed5e op im2,  $eda0 op ldi,
$edb0 op ldir, $eda8 op ldd,  $edb8 op lddr, $ed44 op neg,
$ed4d op reti, $ed45 op retn, $eda2 op ini,  $edaa op ind,
$eda3 op outi,
( ----- 107 )
\ Z80 Assembler. ld,
create _s1 $0a , $1a , 0 , 0 , $ed57 , $ed5f , 0 , 0 ,
create _s2 $02 , $12 , 0 , 0 , $ed47 , $ed4f , 0 , 0 ,
: _r8 over opreg <<3 over opreg or $40 or c, or ?ixy+, ;
: _sp dup special? if nip _s1 else drop _s2 then
      swap opreg cells + @ opcode, ;
: _n ( dst src -- i mask 16b? )
  nbank@ swap dup 16b? if opreg <<4 1 else opreg <<3 0 then ;
4 wordtbl _ ( dst src -- ) \ sel on src. dst should be a reg
:w 2dup or special? if _sp else _r8 then ;
:w HL# SP = # $f9 c, ;
:w _n if $01 or c, l, else $06 or c, c, then ;
:w 2dup < <<3 rot> ?swap _n if
    dup $20 = if $02 else $ed43 then or rot or
    else $38 = # swap $32 or then opcode, l, ;
: ld, ( dst src -- ) over optype over optype max _ swap wexec ;
( ----- 108 )
\ Z80 Assembler. Macros
: clrA, A A xor, ;
: subHL, A A or, HL swap sbc, ;
: pushA, B 0 i) ld, C A ld, BC push, ;
: HLZ, A H ld, A L or, ;
: DEZ, A D ld, A E or, ;
: BCZ, A B ld, A C or, ;
: ldDE(HL), E (HL) ld, HL inc, D (HL) ld, ;
: ldBC(HL), C (HL) ld, HL inc, B (HL) ld, ;
: ldHL(HL), A (HL) ld, HL inc, H (HL) ld, L A ld, ;
: outHL, A H ld, dup A out, A L ld, A out, ;
: outDE, A D ld, dup A out, A E ld, A out, ;
: HL>BC, B H ld, C L ld, ;
: BC>HL, H B ld, L C ld, ;
: A>BC, C A ld, B 0 i) ld, ;
: A>HL, L A ld, H 0 i) ld, ;
( ----- 109 )
\ Z80 Assembler. Jumps, calls and HAL
: cond ( cond opcode -- opcode ) swap <<3 or ;
: jr! ( off a -- ) swap 2 - _bchk swap 1+ c! ;
: j8, ( n opcode -- ) here swap l, jr! ;
: jr, $18 j8, ; : djnz, $10 j8, ; : _ $20 cond j8, ;
: jrz, CZ _ ; : jrnz, CNZ _ ; : jrc, CC _ ; : jrnc, CNC _ ;
: j16, ( n opcode -- ) c, l, ;
: jp, $c3 j16, ;
: call, dup $38 and over = if rst, else $cd j16, then ;
: jpc, $c2 cond j16, ;           : callc, $c4 cond j16, ;
: retc, $c0 cond c, ;            : jp(HL), $e9 c, ;
: jp(IX), IX drop jp(HL), ;      : jp(IY), IY drop jp(HL), ;
: @jmp, m) HL swap ld, jp(HL), ; alias jp, jmp,
: i>, BC push, i) BC swap ld, ; : i@>, BC push, m) BC swap ld, ;
( ----- 110 )
\ 8086 assembler. See doc/asm
28 consts 0 AL 1 CL 2 DL 3 BL
          4 AH 5 CH 6 DH 7 BH
          0 AX 1 CX 2 DX 3 BX
          4 SP 5 BP 6 SI 7 DI
          0 ES 1 CS 2 SS 3 DS
          0 [BX+SI] 1 [BX+DI] 2 [BP+SI] 3 [BP+DI]
          4 [SI] 5 [DI] 6 [BP] 7 [BX]
: <<3 << << << ;
( ----- 111 )
: OP1 doer c, does> c@ c, ;
$c3 OP1 RET,        $fa OP1 CLI,       $fb OP1 STI,
$f4 OP1 HLT,        $fc OP1 CLD,       $fd OP1 STD,
$90 OP1 NOP,        $98 OP1 CBW,
$f3 OP1 REPZ,       $f2 OP1 REPNZ,     $ac OP1 LODSB,
$ad OP1 LODSW,      $a6 OP1 CMPSB,     $a7 OP1 CMPSW,
$a4 OP1 MOVSB,      $a5 OP1 MOVSW,     $ae OP1 SCASB,
$af OP1 SCASW,      $aa OP1 STOSB,     $ab OP1 STOSW,

: OP1r doer c, does> c@ + c, ;
$40 OP1r INCx,      $48 OP1r DECx,
$58 OP1r POPx,      $50 OP1r PUSHx,
( ----- 112 )
: OPr0 ( reg op ) doer c, c, does>
    c@+ c, c@ <<3 or $c0 or c, ;
0 $d0 OPr0 ROLr1,   0 $d1 OPr0 ROLx1,  4 $f6 OPr0 MULr,
1 $d0 OPr0 RORr1,   1 $d1 OPr0 RORx1,  4 $f7 OPr0 MULx,
4 $d0 OPr0 SHLr1,   4 $d1 OPr0 SHLx1,  6 $f6 OPr0 DIVr,
5 $d0 OPr0 SHRr1,   5 $d1 OPr0 SHRx1,  6 $f7 OPr0 DIVx,
0 $d2 OPr0 ROLrCL,  0 $d3 OPr0 ROLxCL, 1 $fe OPr0 DECr,
1 $d2 OPr0 RORrCL,  1 $d3 OPr0 RORxCL, 0 $fe OPr0 INCr,
4 $d2 OPr0 SHLrCL,  4 $d3 OPr0 SHLxCL,
5 $d2 OPr0 SHRrCL,  5 $d3 OPr0 SHRxCL,
( ----- 113 )
: OPrr doer c, does> c@ c, <<3 or $c0 or c, ;
$31 OPrr XORxx,     $30 OPrr XORrr,
$88 OPrr MOVrr,     $89 OPrr MOVxx,    $28 OPrr SUBrr,
$29 OPrr SUBxx,     $08 OPrr ORrr,     $09 OPrr ORxx,
$38 OPrr CMPrr,     $39 OPrr CMPxx,    $00 OPrr ADDrr,
$01 OPrr ADDxx,     $12 OPrr ADCrr,    $13 OPrr ADCxx,
$20 OPrr ANDrr,     $21 OPrr ANDxx,
( ----- 114 )
4 wordtbl mods 'w noop 'w c, 'w l, 'w noop
: modrm ( disp? modrm -- )
 dup c, dup $c7 and 6 = if drop $80 then 64 / mods swap wexec ;
: OP[] ( opbase+modrmbase ) doer , does>
  @ l|m ( disp? modrm opoff modrmbase op ) rot + c, + modrm ;
( -- disp? modrm opoff )
: [b] ( r/m ) 0 ; : [w] ( r/m ) 1 ;
: [m] ( a ) 6 0 ; : [M] [m] 1+ ;
: [r] ( r ) $c0 or 0 ; : [x] [r] 1+ ;
: [b]+ ( r/m disp8 ) swap $40 or 0 ; : [w]+ [b]+ 1+ ;
: r[] ( r r/m ) swap <<3 or 2 ; : x[] r[] 1+ ;
: []r ( r/m r ) <<3 or 0 ; : []x []r 1+ ;
: r[]+ ( r r/m disp8 )
    rot <<3 rot or $40 or 2 ; : x[]+ r[]+ 1+ ;
: []+r ( r/m disp8 r ) <<3 rot or $40 or 0 ; : []+x []+r 1+ ;
( ----- 115 )
$fe00 OP[] INC[],        $fe08 OP[] DEC[],
$fe30 OP[] PUSH[],       $8e00 OP[] POP[],
$8800 OP[] MOV[],        $3800 OP[] CMP[],

: OP[]i ( opbase+modrmbase ) doer , does> swap >r ( i )
  swap ( opoff ) dup if r@ >>8 not if 2 + then then >r
  @ l|m ( disp? modrm modrmbase op )
  r@ + c, + modrm r> 1 = if r> l, else r> c, then ;
$8000 OP[]i ADD[]i,      $8010 OP[]i ADC[]i,
$8038 OP[]i CMP[]i,      $8028 OP[]i SUB[]i,

: OPI doer c, does> c@ c, l, ;
$05 OPI ADDAXI,     $15 OPI ADCALI,    $25 OPI ANDAXI,
$2d OPI SUBAXI,     $a1 OPI MOVAXm,    $a3 OPI MOVmAX,
( ----- 116 )
: OPi doer c, does> c@ c, c, ;
$04 OPi ADDALi,     $14 OPi ADCALi,    $24 OPi ANDALi,
$2c OPi SUBALi,     $cd OPi INT,
$a0 OPi MOVALm,     $a2 OPi MOVmAL,
: MOVri, swap $b0 or c, c, ; : MOVxI, swap $b8 or c, l, ;
: MOVsx, $8e c, swap <<3 or $c0 or c, ;
: MOVrm, $8a c, swap <<3 $6 or c, l, ;
: MOVxm, $8b c, swap <<3 $6 or c, l, ;
: MOVmr, $88 c, <<3 $6 or c, l, ;
: MOVmx, $89 c, <<3 $6 or c, l, ;
: PUSHs, <<3 $06 or c, ; : POPs, <<3 $07 or c, ;
: JMPr, $ff c, 7 and $e0 or c, ;
: JMPf, ( seg off ) $ea c, l, l, ;
( ----- 117 )
: jr! ( off a -- ) swap 2 - _bchk swap 1+ c! ;
: j8, ( n opcode -- ) here swap l, jr! ;
: jr, $eb j8, ; : jrz, $74 j8, ; : jrnz, $75 j8, ;
: jrc, $72 j8, ; : jrnc, $73 j8, ;
: jmp, $e9 c, ( jmp near ) pc - 2 - l, ;
: call, $e8 c, ( jmp near ) pc - 2 - l, ;
: i>, BX PUSHx, BX swap MOVxI, ;
: @jmp, MOVAXm, AX JMPr, ;
: i@>, BX PUSHx, BX swap MOVxm, ;
( ----- 120 )
\ 6502 assembler, Addressing modes.
\ output: n n-is-2b opoff
: # ( n ) 0 $09 ; \ Immediate
: <> ( n ) 0 $05 ; \ ZeroPage
: <X+> ( n ) 0 $15 ; \ ZeroPage+X
: <Y+> ( n ) 0 $15 ; \ Only for LDX
: () ( n ) 1 $0d ; \ Absolute
: (X+) ( n ) 1 $1d ; \ Absolute+X
: (Y+) ( n ) 1 $19 ; \ Absolute+Y
: [X+] ( n ) 0 $01 ; \ Indirect+X
: []Y+ ( n ) 0 $11 ; \ Indirect+Y
: ?, ( n n-is-2b -- ) if l, else c, then ;
( ----- 121 )
\ 6502 asm, Groups 1 and 2, 3-with-AM
: OPG1 doer c, does> c@ or c, ?, ;
$60 OPG1 ADC,  $20 OPG1 AND,  $c0 OPG1 CMP,  $40 OPG1 EOR,
$a0 OPG1 LDA,  $00 OPG1 ORA,  $e0 OPG1 SBC,  $80 OPG1 STA,

: _09repl dup $09 = if drop 1 then ;
: OPG2 doer c, does> c@ swap _09repl or 1+ c, ?, ;
$00 OPG2 ASL,  $c0 OPG2 DEC,  $e0 OPG2 INC,  $a0 OPG2 LDX,
$40 OPG2 LSR,  $20 OPG2 ROL,  $60 OPG2 ROR,  $80 OPG2 STX,

: OPG3 doer c, does> c@ swap _09repl or 1- c, ?, ;
$20 OPG3 BIT,  $e0 OPG3 CPX,  $c0 OPG3 CPY,  $a0 OPG3 LDY,
$80 OPG3 STY,
( ----- 122 )
\ 6502 asm, implied, branching
: OP doer c, does> c@ c, ;
$0a OP ASLA, $00 OP BRK,  $18 OP CLC,  $d8 OP CLD,  $58 OP CLI,
$b8 OP CLV,  $ca OP DEX,  $88 OP DEY,  $e8 OP INX,  $c8 OP INY,
$4a OP LSRA, $ea OP NOP,  $48 OP PHA,  $08 OP PHP,  $68 OP PLA,
$28 OP PLP,  $2a OP ROLA, $6a OP RORA, $40 OP RTI,  $60 OP RTS,
$38 OP SEC,  $f8 OP SED,  $78 OP SEI,  $aa OP TAX,  $a8 OP TAY,
$98 OP TYA,  $ba OP TSX,  $8a OP TXA,  $9a OP TXS,

: OPBR doer c, does> c@ c, 2 - _bchk c, ;
$90 OPBR BCC, $b0 OPBR BCS, $f0 OPBR BEQ, $30 OPBR BMI,
$d0 OPBR BNE, $10 OPBR BPL, $50 OPBR BVC, $70 OPBR BVS,

: OPBR2 doer c, does> c@ c, l, ;
$20 OPBR2 JSR, $4c OPBR2 JMP, $6c OPBR2 JMP[],
( ----- 123 )
\ 6502 Flow
alias JMP, jmp, alias JMP[], @jmp, alias JSR, call,
: jr! ( off a -- )
  dup c@ $b8 = if ( CLV ) 1+ swap 1- swap then
  swap 2 - _bchk swap 1+ c! ;
: jr, CLV, BVC, ; \ no BRA!
alias BEQ, jrz, alias BNE, jrnz,
alias BCS, jrc, alias BCC, jrnc,
: i>, DEX, DEX, dup # LDA, 0 <X+> STA, >>8 # LDA, 1 <X+> STA, ;
: i@>,
  DEX, DEX, dup () LDA, 0 <X+> STA, 1+ () LDA, 1 <X+> STA, ;
\ ZP assignments
$06 value 'A   $08 value 'N
0 value IPL    2 value INDJ
: IPH IPL 1+ ; : INDL INDJ 1+ ; : INDH INDL 1+ ;
( ----- 125 )
\ 6502 disassembler
\ order below represent "opid", also used in emulator
create OPNAME ," ORAANDEORADCSTALDACMPSBC" \ 1/5/9/d x8
," ASLROLLSRRORSTXLDXDECINC" \ 6/a/e x8
," BITJMPSTYLDYCPYCPX" \ 4/c x6
," BRKBPLJSRBMIRTIBVCRTSBVSBCCLDYBCSCPYBNECPXBEQ" \ 0 x15
," PHPCLCPLPSECPHACLIPLASEIDEYTYATAYCLVINYCLDINXSED" \ 8 x16
," TXATXSTAXTSXDEXNOP" \ a x6
59 value OPCNT $ff value NUL 20 value DISCNT
: >>4 >> >> >> >> ;
: opid. dup OPCNT < if
  3 * OPNAME + 3 rtype else drop ." ???" then ;
: words, ( n -- ) create for ' , next ;
: spcs ( n -- ) for spc> next ;
( ----- 126 )
: id159d ( opcode -- opid )
  dup $89 = if drop NUL else >>4 >> then ;
create _ 24 nc, $c $c $d $d $e $e $f $f
                $35 $36 $37 $38 $39 NUL $3a NUL
                $c NUL $d $d $e $e $f $f
: id6ae dup $80 < if ( ASL/ROL/LSR/ROR )
    dup $1f and $1a = if drop NUL exit then >>4 >> 8 + exit then
  dup >> >> 1- 3 and 8 * _ + ( op tbl ) swap >>4 7 and + c@ ;
create _ 32 nc,
NUL NUL $10 NUL NUL NUL NUL NUL $12 $12 $13 $13 $14 $14 $15 NUL
NUL NUL $10 NUL $11 NUL $11 NUL $12 NUL $13 $13 $14 NUL $15 NUL
: id4c _ over $8 and if $10 + then swap >>4 + c@ ;
: idnul drop NUL ;
: id0 >>4 dup 8 = if drop NUL exit then
  dup 8 > if 1- then 22 + ;
: id8 >>4 37 + ;
( ----- 127 )
: id2 $a2 = if $0d else NUL then ;
16 words, _ id0 id159d id2 idnul id4c
  id159d id6ae idnul id8 id159d
  id6ae idnul id4c id159d id6ae idnul
: opid dup $f and cells _ + @ execute ;
\ 0=inh 1=imm 2=acc 3=zp 4=zp,X 5=zp,Y 6=abs 7=abs,X 8=abs,Y
\ 9=ind 10=ind,X 11=ind,Y 12=rel
create _ $40 nc, 0  10 1 0 3 3 3 0 0 1 2 0 6 6 6 0
                 12 11 0 0 4 4 4 0 0 8 0 0 7 7 7 0
                 1  10 1 0 3 3 3 0 0 1 0 0 6 6 6 0
                 12 11 0 0 4 4 4 0 0 8 0 0 7 7 7 0
: modeid ( opcode -- id )
  dup $20 = if drop 6 exit then dup $6c = if drop 9 exit then
  dup $be = if drop 8 exit then
  dup $80 and >> >> swap $1f and or _ + c@ ;
( ----- 128 )
: inh. ( a -- a ) 7 spcs ; : byte. c@+ .x ;
: $. '$' emit byte. ; : zp. $. 4 spcs ; alias zp. rel.
: imm. '#' emit byte. 4 spcs ;
: $$. '$' emit c@+ swap c@+ .x swap .x ; : abs. $$. 2 spcs ;
: ind. '(' emit $$. ')' emit ;
: acc. 'A' emit 6 spcs ;
: ,X. ',' emit 'X' emit ;
: ,Y. ',' emit 'Y' emit ;
: zp,X. $. ,X. 2 spcs ; : zp,Y. $. ,Y. 2 spcs ;
: abs,X. $$. ,X. ; : abs,Y. $$. ,Y. ;
: ind,X. '(' emit $. ,X. ')' emit ;
: ind,Y. '(' emit $. ')' emit ,Y. ;
13 words, _ inh. imm. acc. zp. zp,X. zp,Y. abs. abs,X. abs,Y.
  ind. ind,X. ind,Y. rel.
( ----- 129 )
: mode. ( a opcode -- a ) modeid cells _ + @ execute ;
: op. ( a -- a ) c@+ dup opid dup opid. spc>
  OPCNT < if mode. else drop then ;
: _dump ( a u -- ) for c@+ .x spc> next drop ;
: offset here pc - ;
: dis ( a -- ) DISCNT for
  dup offset - .X spc> dup op. spc>
  tuck over - _dump nl> next drop ;
( ----- 130 )
\ 6809 assembler. See doc/asm.txt.
: <<3 << << << ; : <<4 <<3 << ;
\ For TFR/EXG
10 consts 0 D 1 X 2 Y 3 U 4 S 5 PCR 8 A 9 B 10 CCR 11 DPR
\ Addressing modes. output: n3? n2? n1 nc opoff
: # ( n ) 1 0 ; \ Immediate
: <> ( n ) 1 $10 ; \ Direct
: () ( n ) l|m 2 $30 ; \ Extended
: [] ( n ) l|m $9f 3 $20 ; \ Extended Indirect
\ Offset Indexed. We auto-detect 0, 5-bit, 8-bit, 16-bit
: _0? ?dup if 1 else $84 1 0 then ;
: _5? dup $10 + $1f > if 1 else $1f and 1 0 then ;
: _8? dup $80 + $ff > if 1 else <<8 >>8 $88 2 0 then ;
: _16 l|m $89 3 ;
( ----- 131 )
: R+N doer c, does> c@ ( roff ) >r
    _0? if _5? if _8? if _16 then then then
    swap r> ( roff ) or swap $20 ;
: R+K doer c, does> c@ 1 $20 ;
: PCR+N ( n ) _8? if _16 then swap $8c or swap $20 ;
: [R+N] doer c, does> c@ $10 or ( roff ) >r
    _0? if _8? if _16 then then swap r> or swap $20 ;
: [PCR+N] ( n ) _8? if _16 then swap $9c or swap $20 ;
0 R+N X+N   $20 R+N Y+N  $40 R+N U+N   $60 R+N S+N
: X+0 0 X+N ; : Y+0 0 Y+N ; : S+0 0 S+N ; : U+0 0 S+N ;
0 [R+N] [X+N] $20 [R+N] [Y+N]
$40 [R+N] [U+N] $60 [R+N] [S+N]
: [X+0] 0 [X+N] ; : [Y+0] 0 [Y+N] ;
: [S+0] 0 [S+N] ; : [U+0] 0 [U+N] ;
( ----- 132 )
$86 R+K X+A   $85 R+K X+B   $8b R+K X+D
$a6 R+K Y+A   $a5 R+K Y+B   $ab R+K Y+D
$c6 R+K U+A   $c5 R+K U+B   $cb R+K U+D
$e6 R+K S+A   $e5 R+K S+B   $eb R+K S+D
$96 R+K [X+A] $95 R+K [X+B] $9b R+K [X+D]
$b6 R+K [Y+A] $b5 R+K [Y+B] $bb R+K [Y+D]
$d6 R+K [U+A] $d5 R+K [U+B] $db R+K [U+D]
$f6 R+K [S+A] $f5 R+K [S+B] $fb R+K [S+D]
$80 R+K X+  $81 R+K X++  $82 R+K -X  $83 R+K --X
$a0 R+K Y+  $a1 R+K Y++  $a2 R+K -Y  $a3 R+K --Y
$c0 R+K U+  $c1 R+K U++  $c2 R+K -U  $c3 R+K --U
$e0 R+K S+  $e1 R+K S++  $e2 R+K -S  $e3 R+K --S
$91 R+K [X++] $93 R+K [--X] $b1 R+K [Y++] $b3 R+K [--Y]
$d1 R+K [U++] $d3 R+K [--U] $f1 R+K [S++] $f3 R+K [--S]
( ----- 133 )
: ,? dup $ff > if m, else c, then ;
: ,n ( cnt ) for c, next ;
: OPINH ( inherent ) doer , does> @ ,? ;
( Targets A or B )
: OP1 doer , does> @ ( n2? n1 nc opoff op ) + ,? ,n ;
( Targets D/X/Y/S/U. Same as OP1, but spit 2b immediate )
: OP2 doer , does> @ over + ,? if ,n else drop m, then ;
( Targets memory only. opoff scheme is different than OP1/2 )
: OPMT doer , does> @
    swap $10 - ?dup if $50 + + then ,? ,n ;
( Targets 2 regs )
: OPRR ( src tgt -- ) doer c, does> c@ c, swap <<4 or c, ;
: OPBR ( op1 -- ) doer c, does> ( off -- ) c@ c, 2 - c, ;
: OPLBR ( op? -- ) doer , does> ( off -- ) @ ,? m, ;
( ----- 134 )
$89 OP1 ADCA,        $c9 OP1 ADCB,
$8b OP1 ADDA,        $cb OP1 ADDB,      $c3 OP2 ADDD,
$84 OP1 ANDA,        $c4 OP1 ANDB,      $1c OP1 ANDCC,
$48 OPINH ASLA,      $58 OPINH ASLB,    $08 OPMT ASL,
$47 OPINH ASRA,      $57 OPINH ASRB,    $07 OPMT ASR,
$4f OPINH CLRA,      $5f OPINH CLRB,    $0f OPMT CLR,
$81 OP1 CMPA,        $c1 OP1 CMPB,      $1083 OP2 CMPD,
$118c OP2 CMPS,      $1183 OP2 CMPU,    $8c OP2 CMPX,
$108c OP2 CMPY,
$43 OPINH COMA,      $53 OPINH COMB,    $03 OPMT COM,
$3c OP1 CWAI,        $19 OPINH DAA,
$4a OPINH DECA,      $5a OPINH DECB,    $0a OPMT DEC,
$88 OP1 EORA,        $c8 OP1 EORB,      $1e OPRR EXG,
$4c OPINH INCA,      $5c OPINH INCB,    $0c OPMT INC,
$0e OPMT JMP,        $8d OP1 JSR,
( ----- 135 )
$86 OP1 LDA,         $c6 OP1 LDB,       $cc OP2 LDD,
$10ce OP2 LDS,       $ce OP2 LDU,       $8e OP2 LDX,
$108e OP2 LDY,
$12 OP1 LEAS,        $13 OP1 LEAU,      $10 OP1 LEAX,
$11 OP1 LEAY,
$48 OPINH LSLA,      $58 OPINH LSLB,    $08 OPMT LSL,
$44 OPINH LSRA,      $54 OPINH LSRB,    $04 OPMT LSR,
$3d OPINH MUL,
$40 OPINH NEGA,      $50 OPINH NEGB,    $00 OPMT NEG,
$12 OPINH NOP,
$8a OP1 ORA,         $ca OP1 ORB,       $1a OP1 ORCC,
$49 OPINH ROLA,      $59 OPINH ROLB,    $09 OPMT ROL,
$46 OPINH RORA,      $56 OPINH RORB,    $06 OPMT ROR,
$3b OPINH RTI,       $39 OPINH RTS,
$82 OP1 SBCA,        $c2 OP1 SBCB,
$1d OPINH SEX,
( ----- 136 )
$87 OP1 STA,         $c7 OP1 STB,       $cd OP2 STD,
$10cf OP2 STS,       $cf OP2 STU,       $8f OP2 STX,
$108f OP2 STY,
$80 OP1 SUBA,        $c0 OP1 SUBB,      $83 OP2 SUBD,
$3f OPINH SWI,       $103f OPINH SWI2,  $113f OPINH SWI3,
$13 OPINH SYNC,      $1f OPRR TFR,
$4d OPINH TSTA,      $5d OPINH TSTB,    $0d OPMT TST,
\ TODO: make LBR take abs addr. they're currently broken.
$24 OPBR BCC,        $1024 OPLBR LBCC,  $25 OPBR BCS,
$1025 OPLBR LBCS,    $27 OPBR BEQ,      $1027 OPLBR LBEQ,
$2c OPBR BGE,        $102c OPLBR LBGE,  $2e OPBR BGT,
$102e OPLBR LBGT,    $22 OPBR BHI,      $1022 OPLBR LBHI,
$24 OPBR BHS,        $1024 OPLBR LBHS,  $2f OPBR BLE,
$102f OPLBR LBLE,    $25 OPBR BLO,      $1025 OPLBR LBLO,
$23 OPBR BLS,        $1023 OPLBR LBLS,  $2d OPBR BLT,
$102d OPLBR LBLT,    $2b OPBR BMI,      $102b OPLBR LBMI,
( ----- 137 )
$26 OPBR BNE,        $1026 OPLBR LBNE,  $2a OPBR BPL,
$102a OPLBR LBPL,    $20 OPBR BRA,      $16 OPLBR LBRA,
$21 OPBR BRN,        $1021 OPLBR LBRN,  $8d OPBR BSR,
$17 OPLBR LBSR,      $28 OPBR BVC,      $1028 OPLBR LBVC,
$29 OPBR BVS,        $1029 OPLBR LBVS,

: _ ( r c cref mask -- r c ) rot> over = ( r mask c f )
  if rot> or swap else nip then ;
: OPP doer c, does> c@ c, 0 toword begin ( r c )
    '$' $80 _ 'S' $40 _ 'U' $40 _ 'Y' $20 _ 'X' $10 _
    '%' $08 _ 'B' $04 _ 'A' $02 _ 'C' $01 _ 'D' $06 _
    '@' $ff _ drop in< dup ws? until drop c, ;
$34 OPP PSHS, $36 OPP PSHU, $35 OPP PULS, $37 OPP PULU,
( ----- 138 )
\ 6809 flow words
: jr! ( off a -- ) swap 2 - _bchk swap 1+ c! ;
: jmp, () JMP, ; : call, () JSR, ; : @jmp, [] JMP, ;
alias BRA, jr,
alias BEQ, jrz, alias BNE, jrnz,
alias BCS, jrc, alias BCC, jrnc,
: i>, # LDD, $3406 m, ( pshs d ) ;
: i@>, () LDD, $3406 m, ( pshs d ) ;
( ----- 140 )
\ AVR assembler. See doc/asm/avr.
\ We divide by 2 because each PC represents a word.
: pc16 pc >> ;
: <<3 << << << ; : <<4 <<3 << ;
: >>3 >> >> >> ; : >>4 >>3 >> ;
: _oor
  ." arg out of range: " .X spc> ." PC " pc16 .X nl> abort ;
: _r8c dup 7 > if _oor then ;
: _r32c dup 31 > if _oor then ;
: _r16+c _r32c dup 16 < if _oor then ;
: _r64c dup 63 > if _oor then ;
: _r256c dup 255 > if _oor then ;
: _Rdp ( op rd -- op', place Rd ) <<4 or ;
( ----- 141 )
\ 0000 000d dddd 0000
: OPRd doer , does> @ swap _r32c _Rdp l, ;
$9405 OPRd ASR,   $9400 OPRd COM,
$940a OPRd DEC,   $9403 OPRd INC,
$9206 OPRd LAC,   $9205 OPRd LAS,
$9207 OPRd LAT,
$9406 OPRd LSR,   $9401 OPRd NEG,
$900f OPRd POP,   $920f OPRd PUSH,
$9407 OPRd ROR,   $9402 OPRd SWAP,
$9204 OPRd XCH,

$9200 OPRd _ : STS, ( k16 rd ) _ l, ;
$9000 OPRd _ : LDS, ( rd k16 ) swap _ l, ;
( ----- 142 )
\ 0000 00rd dddd rrrr
: OPRdRr doer c, does> c@ ( rd rr op )
  over _r32c $10 and >>3 or ( rd rr op' )
  <<8 or $ff0f and ( rd op' )
  swap _r32c _Rdp l, ;
$1c OPRdRr ADC,   $0c OPRdRr ADD,    $20 OPRdRr AND,
$14 OPRdRr CP,    $04 OPRdRr CPC,    $10 OPRdRr CPSE,
$24 OPRdRr EOR,   $2c OPRdRr MOV,    $9c OPRdRr MUL,
$28 OPRdRr OR,    $08 OPRdRr SBC,    $18 OPRdRr SUB,

\ 0000 0AAd dddd AAAA
: OPRdA doer c, does> c@ ( rd A op )
  over _r64c $30 and >>3 or ( rd A op' )
  <<8 or $ff0f and ( rd op' ) swap _r32c _Rdp l, ;
$b0 OPRdA IN,     $b8 OPRdA _ : OUT, swap _ ;
( ----- 143 )
\ 0000 KKKK dddd KKKK
: OPRdK doer c, does> c@ ( rd K op )
  over _r256c $f0 and >>4 or ( rd K op' )
  rot _r16+c <<4 rot $0f and or ( op' rdK ) c, c, ;
$70 OPRdK ANDI,   $30 OPRdK CPI,     $e0 OPRdK LDI,
$60 OPRdK ORI,    $40 OPRdK SBCI,    $60 OPRdK SBR,
$50 OPRdK SUBI,

\ 0000 0000 AAAA Abbb
: OPAb doer c, does> c@ ( A b op )
  rot _r32c <<3 rot _r8c or c, c, ;
$98 OPAb CBI,     $9a OPAb SBI,      $99 OPAb SBIC,
$9b OPAb SBIS,
( ----- 144 )
: OPNA doer , does> @ l, ;
$9598 OPNA BREAK, $9488 OPNA CLC,    $94d8 OPNA CLH,
$94f8 OPNA CLI,   $94a8 OPNA CLN,    $94c8 OPNA CLS,
$94e8 OPNA CLT,   $94b8 OPNA CLV,    $9498 OPNA CLZ,
$9419 OPNA EIJMP, $9509 OPNA ICALL,  $9519 OPNA EICALL,
$9409 OPNA IJMP,  $0000 OPNA NOP,    $9508 OPNA RET,
$9518 OPNA RETI,  $9408 OPNA SEC,    $9458 OPNA SEH,
$9478 OPNA SEI,   $9428 OPNA SEN,    $9448 OPNA SES,
$9468 OPNA SET,   $9438 OPNA SEV,    $9418 OPNA SEZ,
$9588 OPNA SLEEP, $95a8 OPNA WDR,
( ----- 145 )
\ 0000 0000 0sss 0000
: OPb doer , does> @ ( b op )
  swap _r8c _Rdp l, ;
$9488 OPb BCLR,   $9408 OPb BSET,

\ 0000 000d dddd 0bbb
: OPRdb doer , does> @ ( rd b op )
  rot _r32c _Rdp swap _r8c or l, ;
$f800 OPRdb BLD,  $fa00 OPRdb BST,
$fc00 OPRdb SBRC, $fe00 OPRdb SBRS,

( special cases )
: CLR, dup EOR, ;  : TST, dup AND, ; : LSL, dup ADD, ;
( ----- 146 )
( a -- k12, absolute addr a, relative to PC in a k12 addr )
: _r7ffc dup $7ff > if _oor then ;
: _raddr12
    pc16 - dup 0< if $800 + _r7ffc $800 or else _r7ffc then ;
: RJMP _raddr12 $c000 or ;
: RCALL _raddr12 $d000 or ;
: RJMP, RJMP l, ; : RCALL, RCALL l, ;
( ----- 147 )
( a -- k7, absolute addr a, relative to PC in a k7 addr )
: _r3fc dup $3f > if _oor then ;
: _raddr7
    pc16 - dup 0< if $40 + _r3fc $40 or else _r3fc then ;
: _brbx ( a b op -- a ) or swap _raddr7 <<3 or ;
: BRBC $f400 _brbx ; : BRBS $f000 _brbx ; : BRCC 0 BRBC ;
: BRCS 0 BRBS ; : BREQ 1 BRBS ; : BRNE 1 BRBC ; : BRGE 4 BRBC ;
: BRHC 5 BRBC ; : BRHS 5 BRBS ; : BRID 7 BRBC ; : BRIE 7 BRBS ;
: BRLO BRCS ; : BRLT 4 BRBS ; : BRMI 2 BRBS ; : BRPL 2 BRBC ;
: BRSH BRCC ; : BRTC 6 BRBC ; : BRTS 6 BRBS ; : BRVC 3 BRBC ;
: BRVS 3 BRBS ;
( ----- 148 )
9 consts $100c X  $0008 Y  $0000 Z
         $100d X+ $1009 Y+ $1001 Z+
         $100e -X $100a -Y $1002 -Z
: _ ( Rd XYZ op ) or ( Rd op' ) swap _Rdp l, ;
: LD, $8000 _ ; : ST, swap $8200 _ ; : LPM, $9004 _ ;
( ----- 149 )
\ pc16 to L1 .. L1 ' RJMP LBL,
: LBL, ( opw pc -- ) 1- swap execute l, ;
: SKIP, pc16 0 l, ;
: TO, ( opw pc )
  \ warning: pc is a PC offset, not a mem addr!
  << xorg + pc16 1- here ( opw addr tgt hbkp )
  rot HERE ! ( opw tgt hbkp )
  swap rot execute l, ( hbkp ) HERE ! ;
\ pc16 to L1 FLBL, .. ' RJMP L1 TO,
: FLBL, 0 l, ;
: BEGIN, pc16 ; : AGAIN?, ( pc op ) swap LBL, ;
: AGAIN, ['] RJMP AGAIN?, ;
: IF, ['] BREQ SKIP, ; : THEN, TO, ;
( ----- 150 )
\ Constant common to all AVR models
38 consts 0 R0 1 R1 2 R2 3 R3 4 R4 5 R5 6 R6 7 R7 8 R8 9 R9
  10 R10 11 R11 12 R12 13 R13 14 R14 15 R15 16 R16 17 R17
  18 R18 19 R19 20 R20 21 R21 22 R22 23 R23 24 R24 25 R25
  26 R26 27 R27 28 R28 29 R29 30 R30 31 R31
  26 XL 27 XH 28 YL 29 YH 30 ZL 31 ZH
( ----- 200 )
\ Z80 port, core routines
fjr jr, to L1 $10 oallot pc to lblxt ( RST 10 )
  IX inc, IX inc, 0 ix+) E ld, 1 ix+) D ld,
  HL pop, ldDE(HL), HL inc, DE HL ex, jp(HL), \ 17 bytes
$28 oallot pc to lblcell ( RST 28 )
  HL pop, BC push, HL>BC, fjr jr, to L2 ( next ) $30 oallot
pc to lblval ( RST 30 ) A SYSVARS $18 + m) ld, A A or,
  fjr jrz, to L3 ( read ) fjr jr, to L4 ( write ) \ 8 bytes
0 jp, ( RST 38 ) $66 oallot retn,
L1 fjr!
di, SP PS_ADDR i) ld, IX RS_ADDR i) ld, 0 jp, pc 2 - to lblboot
L3 fjr! ( val read ) HL pop, BC push, ldBC(HL), \ to lblnext
pc to lblnext L2 fjr!
DE HL ex, pc to L1 ldDE(HL), HL inc, DE HL ex, jp(HL),
L4 fjr! ( val write ) clrA, SYSVARS $18 + m) A ld, HL pop,
  (HL) C ld, HL inc, (HL) B ld, BC pop, lblnext br jr,
( ----- 201 )
\ Z80 port, lbldoes exit quit abort bye rcnt scnt
pc to lbldoes HL pop, BC push, HL>BC, BC inc, BC inc, ldHL(HL),
  jp(HL),
code exit \ put new IP in HL instead of DE for speed
  L 0 ix+) ld, H 1 ix+) ld, IX dec, IX dec, L1 jp,
code quit pc to L1 \ used in ABORT
  IX RS_ADDR i) ld, 0 jp, pc 2 - to lblmain
code abort SP PS_ADDR i) ld, L1 br jr,
code bye halt,
code rcnt BC push, IX push, HL pop, BC RS_ADDR i) ld,
  BC subHL, HL>BC, ;code
code scnt HL 0 i) ld, HL SP add, BC push, HL>BC,
  HL PS_ADDR i) ld, BC subHL, HL>BC, ;code
( ----- 202 )
\ Z80 port, pc! pc@ []= [c]? (im1)
code pc! HL pop, (C) L out, BC pop, ;code
code pc@ C (C) in, B 0 i) ld, ;code
code []= BC push, exx, ( protect DE ) BC pop, DE pop, HL pop,
  pc to L1 ( loop )
    A (DE) ld, DE inc, cpi,
    ifnz, exx, BC 0 i) ld, ;code then,
    L1 CPE jpc, ( BC not zero? loop )
  exx, BC 1 i) ld, ;code
code cidx BCZ, ifz, HL pop, HL pop, ;code then,
  BC push, exx, BC pop, HL pop, DE pop, A E ld, D H ld,
  E L ld, \ HL=a DE=a BC=u A=c
  cpir, ifz, DE subHL, HL dec, HL push, exx, BC 1 i) ld,
  else, exx, BC 0 i) ld, then, ;code
code (im1) im1, ei, ;code
( ----- 203 )
\ Z80 port, /mod *
code * HL pop, DE push, DE HL ex, ( DE * BC -> HL )
  HL 0 i) ld, A $10 i) ld, begin,
    HL HL add, E rl, D rl,
    ifc, HL BC add, then,
    A dec, br jrnz,
  HL>BC, DE pop, ;code
\ Divides AC by DE. quotient in AC remainder in HL
code /mod BC>HL, BC pop, DE push, DE HL ex,
  A B ld, B 16 i) ld, HL 0 i) ld, begin,
    scf, C rl, rla, HL HL adc, HL DE sbc,
    ifc, HL DE add, C dec, then,
  br djnz,
  DE pop, HL push, B A ld, ;code
( ----- 204 )
\ Z80 port, find
code find ( s -- w-or-0 ) BC>HL, C (HL) ld, B 0 i) ld, HL inc,
  HL BC add, \ HL points to after last char of s
  'N m) HL ld, HL SYSVARS $02 ( CURRENT ) + m) ld, begin,
    HL dec, A (HL) ld, A $7f i) and, ( imm ) A C cp, ifz,
      HL push, DE push, BC push, DE 'N m) ld,
      HL dec, HL dec, HL dec, \ Skip prev field
      pc to L1 ( loop )
        DE dec, A (DE) ld, cpd, ifz, to L2 ( break! )
      L1 CPE jpc, ( BC not zero? loop ) L2 fjr!
      BC pop, DE pop, HL pop, then,
    ifz, ( match ) HL inc, HL>BC, ;code then,
    \ no match, go to prev and continue
    HL dec, A (HL) ld, HL dec, L (HL) ld, H A ld,
    A L or, ifz, ( end of dict ) BC 0 i) ld, ;code then,
  br jr,
( ----- 205 )
\ Z80 port, (b) (n) (br) (?br) (next)
code (b) ( -- c ) BC push, A (DE) ld, A>BC, DE inc, ;code
code (n) ( -- n ) BC push,
  DE HL ex, ldBC(HL), HL inc, DE HL ex, ;code
code (br) pc to L1 ( used in ?br and next )
  A (DE) ld, ( sign extend A into HL )
  L A ld, A A add, ( sign in carry ) A A sbc, ( FF if neg )
  H A ld, HL DE add, ( HL --> new IP ) DE HL ex, ;code
code (?br) BCZ, BC pop, L1 br jrz, DE inc, ;code
code (next)
  0 ix+) dec, ifnz,
    A $ff i) ld, A 0 ix+) cp, ifz, 1 ix+) dec, then,
    L1 br jr, then,
  A A xor, A 1 ix+) cp, L1 br jrnz,
  IX dec, IX dec, DE inc, ;code
( ----- 206 )
\ Z80 port, >R I C@ @ C! ! 1+ 1- + -
code >r IX inc, IX inc, 0 ix+) C ld, 1 ix+) B ld, BC pop, ;code
code r@ BC push, C 0 ix+) ld, B 1 ix+) ld, ;code
code r~ IX dec, IX dec, ;code
code r> BC push, C 0 ix+) ld, B 1 ix+) ld,
  IX dec, IX dec, ;code
code c@ A (BC) ld, A>BC, ;code
code @ BC>HL, ldBC(HL), ;code
code c! BC>HL, BC pop, (HL) C ld, BC pop, ;code
code ! BC>HL, BC pop,
  (HL) C ld, HL inc, (HL) B ld, BC pop, ;code
code 1+ BC inc, ;code
code 1- BC dec, ;code
code + HL pop, HL BC add, HL>BC, ;code
code - HL pop, BC subHL, HL>BC, ;code
( ----- 207 )
\ Z80 port, AND OR XOR >> << >>8 <<8
code and HL pop,
  A C ld, A L and, C A ld, A B ld, A H and, B A ld, ;code
code or HL pop,
  A C ld, A L or, C A ld, A B ld, A H or, B A ld, ;code
code xor HL pop,
  A C ld, A L xor, C A ld, A B ld, A H xor, B A ld, ;code
code not BCZ, BC 0 i) ld, ifz, C inc, then, ;code
code >> B srl, C rr, ;code
code << C sla, B rl, ;code
code >>8 C B ld, B 0 i) ld, ;code
code <<8 B C ld, C 0 i) ld, ;code
( ----- 208 )
\ Z80 port, rot rot> dup drop swap over execute
code rot ( a b c -- b c a ) ( BC=c )
 HL pop, ( b ) (SP) HL ex, ( a<>b ) BC push, ( c ) HL>BC, ;code
code rot> ( a b c -- c a b ) ( BC=c )
  BC>HL, BC pop, ( b ) (SP) HL ex, ( a<>c ) HL push, ;code
code dup ( a -- a a ) pc to L1 BC push, ;code
code ?dup BCZ, L1 br jrnz, ;code
code drop ( a -- ) BC pop, ;code
code swap ( a b -- b a ) HL pop, BC push, HL>BC, ;code
code over ( a b -- a b a )
  HL pop, HL push, BC push, HL>BC, ;code
code execute BC>HL, BC pop, jp(HL),
( ----- 209 )
\ Z80 port, JMPi! CALLi!
code jmpi! ( pc a -- len ) BC>HL, BC pop,
  A $c3 i) ld, pc to L1 (HL) A ld, HL inc,
  (HL) C ld, HL inc, (HL) B ld, BC 3 i) ld, ;code
code calli! ( pc a -- len ) BC>HL, BC pop,
  A B ld, A A or, ifz, A C ld, A $c7 i) and, ifz, \ RST
   A C ld, A $c7 i) or, (HL) A ld, BC 1 i) ld, ;code then, then,
  ( not RST ) A $cd i) ld, L1 br jr,
( ----- 210 )
\ Z80 port speedups
code tuck ( a b -- b a b ) HL pop, BC push, HL push, ;code
code nip ( a b -- b ) HL pop, ;code
code +! ( n a -- ) BC>HL, ldBC(HL), HL dec, (SP) HL ex,
  HL BC add, HL>BC, HL pop, (HL) C ld, HL inc, (HL) B ld,
  BC pop, ;code
code A> BC push, IY push, BC pop, ;code
code >A BC push, IY pop, BC pop, ;code
code A>r IY push, HL pop,
  IX inc, IX inc, 0 ix+) L ld, 1 ix+) H ld, ;code
code r>A L 0 ix+) ld, H 1 ix+) ld, IX dec, IX dec,
  HL push, IY pop, ;code
code A+ IY inc, ;code
code A- IY dec, ;code
code Ac@ BC push, C 0 iy+) ld, B 0 i) ld, ;code
code Ac! 0 iy+) C ld, BC pop, ;code
( ----- 211 )
\ Z80 port speedups
code move ( src dst u -- ) HL pop, DE HL ex, (SP) HL ex,
  BCZ, ifnz, ldir, then, DE pop, BC pop, ;code
code = HL pop, BC subHL, BC 0 i) ld, ifz, BC inc, then, ;code
code < HL pop, BC subHL, BC 0 i) ld, ifc, BC inc, then, ;code
code crc16 ( c n -- c ) BC push, exx, ( protect DE )
  HL pop, ( n ) DE pop, ( c ) A L ld, A D xor, D A ld,
  B 8 i) ld, begin,
    E sla, D rl, ifc, ( msb is set, apply polynomial )
      A D ld, A $10 i) xor, D A ld,
      A E ld, A $21 i) xor, E A ld, then,
  br djnz,
  DE push, exx, ( unprotect DE ) BC pop, ;code
( ----- 215 )
\ 8086 boot code. PS=SP, RS=BP, IP=DX, TOS=BX
fjr jr, to L1 ( main ) 4 oallot ( 3=boot driveno )
pc to lblboot 2 allot0 pc to lblmain 2 allot0
L1 fjr! ( main ) DX POPx, ( boot drive no ) $03 DL MOVmr,
  SP PS_ADDR MOVxI, BP RS_ADDR MOVxI,
  DI $04 ( BOOT ) MOVxm, DI JMPr,
pc to lblval AL SYSVARS $18 ( TO? ) + MOVrm, AL AL ORrr, ifz,
  DI POPx, BX PUSHx, BX [DI] x[] MOV[], else,
  AL AL XORrr, SYSVARS $18 + AL MOVmr, DI POPx,
  [DI] BX []x MOV[], BX POPx, then, \ to next
pc to lblnext DI DX MOVxx, ( <-- IP ) DX INCx, DX INCx,
  DI [DI] x[] MOV[], DI JMPr,
pc to lblcell AX POPx, BX PUSHx, BX AX MOVxx, lblnext br jr,
pc to lblxt BP INCx, BP INCx, [BP] 0 DX []+x MOV[], ( pushRS )
  DX POPx, lblnext br jr,
( ----- 216 )
pc to lbldoes DI POPx, BX PUSHx, BX DI MOVxx, BX INCx, BX INCx,
  DI [DI] x[] MOV[], DI JMPr,
code exit DX [BP] 0 x[]+ MOV[], BP DECx, BP DECx, ;code
code []= ( a1 a2 u -- f ) CX BX MOVxx, SI POPx, DI POPx,
  CLD, REPZ, CMPSB, BX 0 MOVxI, ifz, BX INCx, then, ;code
code cidx ( c a u -- ?i f ) CX BX MOVxx, DI POPx, AX POPx,
  CLD, REPNZ, SCASB, ifnz, BX BX XORxx, else,
    BX CX SUBxx, BX DECx, BX PUSHx, BX 1 MOVxI, then, ;code
code quit pc to L1 ( used in ABORT )
  BP RS_ADDR MOVxI, DI $06 ( main ) MOVxm, DI JMPr,
code abort SP PS_ADDR MOVxI, L1 br jr,
code bye HLT, begin, br jr,
( ----- 217 )
code findX ( s -- w-or-0 ) CX BX MOVxx, SI POPx, \ TODO
  DI SYSVARS $2 ( CURRENT ) + MOVxm,
  begin, ( loop )
    AL [DI] -1 r[]+ MOV[], $7f ANDALi, ( strlen )
    CL AL CMPrr, ifz, ( same len )
      SI PUSHx, DI PUSHx, CX PUSHx, ( --> )
        3 ADDALi, ( header ) AH AH XORrr, DI AX SUBxx,
        CLD, REPZ, CMPSB,
      CX POPx, DI POPx, SI POPx, ( <-- )
      ifz, DI PUSHx, BX 1 MOVxI, ;code then,
    then,
    DI [x] 3 SUB[]i, DI [DI] x[] MOV[], ( prev ) DI DI ORxx,
  br jrnz, ( loop ) BX BX XORxx, ;code
( ----- 218 )
code * AX POPx, DX PUSHx, ( protect from MUL ) BX MULx, DX POPx,
  BX AX MOVxx, ;code
code /mod AX POPx, DX PUSHx, ( protect )
  DX DX XORxx, BX DIVx,
  BX DX MOVxx, DX POPx, ( unprotect )
  BX PUSHx, ( modulo ) BX AX MOVxx, ( division ) ;code
code rcnt
  BX PUSHx, BX BP MOVxx, AX RS_ADDR MOVxI, BX AX SUBxx, ;code
code scnt
  AX PS_ADDR MOVxI, AX SP SUBxx, BX PUSHx, BX AX MOVxx, ;code
( ----- 219 )
code (n)
  BX PUSHx, DI DX MOVxx, BX [DI] x[] MOV[],
  DX INCx, DX INCx, ;code
code (b)
  BX PUSHx, DI DX MOVxx, BH BH XORrr, BL [DI] r[] MOV[],
  DX INCx, ;code
code (br) pc to L1 ( used in ?br )
  DI DX MOVxx, AL [DI] r[] MOV[], AH AH XORrr, CBW,
  DX AX ADDxx, ;code
code (?br)
  BX BX ORxx, BX POPx, L1 br jrz, DX INCx, ;code
code (next)
  [BP] 0 [w]+ DEC[], L1 br jrnz,
  BP DECx, BP DECx, DX INCx, ;code
( ----- 220 )
code + AX POPx, BX AX ADDxx, ;code
code - AX POPx, AX BX SUBxx, BX AX MOVxx, ;code
code < AX POPx, CX CX XORxx, AX BX CMPxx, ifc, CX INCx, then,
  BX CX MOVxx, ;code
code 1+ BX INCx, ;code
code 1- BX DECx, ;code
code and AX POPx, BX AX ANDxx, ;code
code or AX POPx, BX AX ORxx, ;code
code xor AX POPx, BX AX XORxx, ;code
code not BX BX ORxx, BX 0 MOVxI, ifz, BX INCx, then, ;code
code >> BX SHRx1, ;code
code << BX SHLx1, ;code
code >>8 BL BH MOVrr, BH BH XORrr, ;code
code <<8 BH BL MOVrr, BL BL XORrr, ;code
( ----- 221 )
code r@ BX PUSHx, BX [BP] 0 x[]+ MOV[], ;code
code r~ BP DECx, BP DECx, ;code
code r> BX PUSHx, BX [BP] 0 x[]+ MOV[], BP DECx, BP DECx, ;code
code >r BP INCx, BP INCx, [BP] 0 BX []+x MOV[], BX POPx, ;code
code rot ( a b c -- b c a ) ( BX=c ) CX POPx, ( b ) AX POPx, \ a
  CX PUSHx, BX PUSHx, BX AX MOVxx, ;code
code rot> ( a b c -- c a b ) CX POPx, AX POPx,
  BX PUSHx, AX PUSHx, BX CX MOVxx, ;code
code dup pc to L1 BX PUSHx, ;code
code ?dup AX BX MOVxx, AX AX ORxx, L1 br jrnz, ;code
code over ( a b -- a b a )
  AX POPx, AX PUSHx, BX PUSHx, BX AX MOVxx, ;code
code swap AX BX MOVxx, BX POPx, AX PUSHx, ;code
code drop BX POPx, ;code
code execute AX BX MOVxx, BX POPx, AX JMPr,
( ----- 222 )
code c@ DI BX MOVxx, BH BH XORrr, BL [DI] r[] MOV[], ;code
code @ DI BX MOVxx, BX [DI] x[] MOV[], ;code
code c! DI BX MOVxx, CX POPx, [DI] CL []r MOV[], BX POPx, ;code
code ! DI BX MOVxx, CX POPx, [DI] CX []x MOV[], BX POPx, ;code
code jmpi! ( pc a -- len ) DI BX MOVxx, AX POPx,
  CL $e9 MOVri, pc to L1 [DI] CL []r MOV[],
  CX SYSVARS $4 ( HOME ) + MOVxm, AX CX SUBxx, AX DECx, AX DECx,
  AX DECx, [DI] 1 AX []+x MOV[], BX 3 MOVxI, ;code
code calli! ( pc a -- len ) DI BX MOVxx, AX POPx,
  CL $e8 MOVri, L1 br jr,
( ----- 225 )
\ 6502 port macros
: PS<>, ( src dst ) swap <X+> LDA, <X+> STA, ;
: PSCLR16, 0 # LDA, dup <X+> STA, 1+ <X+> STA, ;
: A>IND+, INDL []Y+ STA, INY, ;
: PS>A, <X+> LDA, ;
: A>PS, <X+> STA, ;
: PSINC, 0 <X+> INC, ifz, 1 <X+> INC, then, ;
: <>INC16, dup <> INC, ifz, swap 1+ <> INC, then, ;
: IP+, IPL <> INC, ifz, IPH <> INC, then, ;
: 2DEX, DEX, DEX, ; : 2INX, INX, INX, ;
: JMP[[]], INDJ JMP, ;
( ----- 226 )
\ 6502 boot code PS=X RS=S
$6c # LDA, INDJ <> STA, $ff # LDX, TXS,
0 JMP, pc 2 - to lblboot
pc to lblcell 2DEX, PLA, 0 A>PS, PLA, 1 A>PS, PSINC, \ next
pc to lblnext IPH <> LDY, IPL <> LDA, INDH <> STY, INDL <> STA,
pc to L1 CLC, 2 # ADC, ifc, INY, then, IPL <> STA, IPH <> STY,
  JMP[[]],
pc to lblxt PLA, INDL <> STA, PLA, INDH <> STA,
  IPH <> LDA, PHA, IPL <> LDA, PHA,
  INDL <> INC, ifz, INDH <> INC, then,
  INDL <> LDA, INDH <> LDY, L1 JMP,
pc to lbldoes CLC, PLA, TAY, PLA, INY, ifz, 1 # ADC, then,
  INDL <> STY, INDH <> STA, 2DEX, 1 <X+> STA, TYA, 2 # ADC,
  ifc, 1 <X+> INC, then, 0 <X+> STA, JMP[[]],
( ----- 227 )
code bye BRK, ;code
code quit
  TXA, $ff # LDX, TXS, TAX, 0 JMP, pc 2 - to lblmain
code abort $ff # LDX, x' quit br BNE,
code exit PLA, IPL <> STA, PLA, IPH <> STA, ;code
code execute 0 <X+> LDA, INDL <> STA, 1 <X+> LDA, INDH <> STA,
  2INX, INDL JMP[],
code scnt INDL <> STX, 2DEX, 0 # LDA, 1 <X+> STA,
  $ff # LDA, SEC, INDL <> SBC, 0 <X+> STA,
  fjr BPL, 1 <X+> DEC, then, ;code
code rcnt TXA, TSX, INDL <> STX, TAX, 2DEX, 0 # LDA,
  1 <X+> STA, $ff # LDA, SEC, INDL <> SBC, 0 <X+> STA, ;code
( ----- 228 )
code (b) 0 # LDY, IPL []Y+ LDA, 2DEX, 0 A>PS, 0 # LDA,
  1 A>PS, IP+, ;code
code (n) 0 # LDY, IPL []Y+ LDA, 2DEX, 0 A>PS, INY,
  IPL []Y+ LDA, 1 A>PS, IP+, IP+, ;code
code (br) 0 # LDY, IPL []Y+ LDA, fjr BPL, IPH <> DEC, then,
  CLC, IPL <> ADC, ifc, IPH <> INC, then, IPL <> STA, ;code
code (?br) 0 <X+> LDA, 1 <X+> ORA, 2INX,
  0 # ORA, x' (br) br BEQ, IP+, ;code
pc to L1 ( ovfl, always branch, C is clear )
  PLA, 0 # SBC, PHA, $ff # LDA, PHA, x' (br) JMP,
pc to L2 PHA, 0 # LDA, PHA, x' (br) JMP,
code (next) PLA, SEC, 1 # SBC, L1 br BCC,
  PHA, x' (br) br BNE, \ branch if nonzero
  PLA, PLA, L2 br BNE, ( finished ) IP+, ;code
( ----- 229 )
code c@ 0 [X+] LDA, 0 <X+> STA, 0 # LDA, 1 <X+> STA, ;code
code @ pc to L1 0 [X+] LDA, TAY, PSINC, 0 [X+] LDA,
  0 <X+> STY, 1 <X+> STA, ;code
pc to lblval 2DEX, PLA, 0 A>PS, PLA, 1 A>PS, PSINC, L1 JMP,
code c! 2 <X+> LDA, 0 [X+] STA, 2INX, 2INX, ;code
code ! 2 <X+> LDA, 0 [X+] STA, PSINC, 3 <X+> LDA, 0 [X+] STA,
  2INX, 2INX, ;code
code 1+ PSINC, ;code
code 1- 0 <X+> LDA, ifz, 1 <X+> DEC, then, 0 <X+> DEC, ;code
code + CLC, 2 <X+> LDA, 0 <X+> ADC, 2 <X+> STA, 3 <X+> LDA,
  1 <X+> ADC, 3 <X+> STA, 2INX, ;code
code - 2 <X+> LDA, SEC, 0 <X+> SBC, 2 <X+> STA, 3 <X+> LDA,
  1 <X+> SBC, 3 <X+> STA, 2INX, ;code
code < 3 PS>A, 1 <X+> CMP, ifz, 2 PS>A, 0 <X+> CMP, then,
  2INX, 0 # LDA, 1 A>PS, 0 # ADC, 1 # EOR, 0 A>PS, ;code
( ----- 230 )
code << 0 <X+> ASL, 1 <X+> ROL, ;code
code >> 1 <X+> LSR, 0 <X+> ROR, ;code
code <<8 0 1 PS<>, 0 # LDA, 0 <X+> STA, ;code
code >>8 1 0 PS<>, 0 # LDA, 1 <X+> STA, ;code
code and 0 <X+> LDA, 2 <X+> AND, 2 <X+> STA, 1 <X+> LDA,
  3 <X+> AND, 3 <X+> STA, 2INX, ;code
code or 0 <X+> LDA, 2 <X+> ORA, 2 <X+> STA, 1 <X+> LDA,
  3 <X+> ORA, 3 <X+> STA, 2INX, ;code
code xor 0 <X+> LDA, 2 <X+> EOR, 2 <X+> STA, 1 <X+> LDA,
  3 <X+> EOR, 3 <X+> STA, 2INX, ;code
code not 0 # LDY, 0 <X+> LDA, 1 <X+> ORA, 1 <X+> STY,
  ifz, INY, then, 0 <X+> STY, ;code
( ----- 231 )
code * 2DEX, 16 # LDY, 0 PSCLR16,
  begin, 0 <X+> ASL, 1 <X+> ROL, 4 <X+> ASL, 5 <X+> ROL,
    ifc, CLC, 2 <X+> LDA, 0 <X+> ADC, 0 <X+> STA, 3 <X+> LDA,
      1 <X+> ADC, 1 <X+> STA, then, DEY, br BNE,
  0 4 PS<>, 1 5 PS<>, 2INX, 2INX, ;code
code /mod \ a b -- r q
  2DEX, DEX, 16 # LDA, 0 <X+> STA, ( cnt )
  1 PSCLR16, ( remaining )
  \ 3-4 = divisor 5-6 = dividend
  begin, 5 <X+> ASL, 6 <X+> ROL, 1 <X+> ROL, 2 <X+> ROL,
    1 <X+> LDA, SEC, 3 <X+> SBC, TAY, 2 <X+> LDA, 4 <X+> SBC,
    ifc, 2 <X+> STA, 1 <X+> STY, 5 <X+> INC, then,
    0 <X+> DEC, br BNE,
  5 3 PS<>, 6 4 PS<>, 1 5 PS<>, 2 6 PS<>, 2INX, INX, ;code
( ----- 232 )
code dup pc to L1 2DEX, 2 0 PS<>, 3 1 PS<>, ;code
code ?dup 0 <X+> LDA, 1 <X+> ORA, L1 br BNE, ;code
code drop 2INX, ;code
code swap 0 <X+> LDA, 2 <X+> LDY, 0 <X+> STY, 2 <X+> STA,
  1 <X+> LDA, 3 <X+> LDY, 1 <X+> STY, 3 <X+> STA, ;code
code over 2DEX, 4 0 PS<>, 5 1 PS<>, ;code
code rot ( a b c -- b c a ) 5 <X+> LDY, 3 5 PS<>, 1 3 PS<>,
  1 <X+> STY, 4 <X+> LDY, 2 4 PS<>, 0 2 PS<>, 0 <X+> STY, ;code
code ROT> ( a b c -- c a b ) 1 <X+> LDY, 3 1 PS<>, 5 3 PS<>,
  5 <X+> STY, 0 <X+> LDY, 2 0 PS<>, 4 2 PS<>, 4 <X+> STY, ;code
code r@ 2DEX, PLA, 0 <X+> STA, TAY, PLA, 1 <X+> STA, PHA,
  TYA, PHA, ;code
code >r 1 <X+> LDA, PHA, 0 <X+> LDA, PHA, 2INX, ;code
code r> 2DEX, PLA, 0 <X+> STA, PLA, 1 <X+> STA, ;code
code r~ PLA, PLA, ;code
( ----- 233 )
code move ( src dst u -- )
  4 PS>A, INDL <> STA, 5 PS>A, INDH <> STA, 2 PS>A, 'N <> STA,
  3 PS>A, 'N 1+ <> STA, 1 PS>A, 5 A>PS, 0 PS>A, 4 A>PS, 2INX,
  2INX, 1 <X+> INC, 0 # LDY, fjr BEQ, to L1 begin,
    INDL []Y+ LDA, 'N []Y+ STA, INY,
    ifz, INDH <> INC, 'N 1+ <> INC, then,
    L1 fjr! ( entry ) TYA, 0 <X+> CMP,
    dup br BNE, 1 <X+> DEC, br BNE,
  2INX, ;code
( ----- 234 )
pc to L1 \ cmp strs at [IND] and ['N] with cnt <X+0>
  0 # LDY, begin,
    INDL []Y+ LDA, 'N []Y+ CMP, ifnz, RTS, then,
    INY, 0 <X+> DEC, br BNE, RTS,
code []= ( a1 a2 u -- f )
  2 <X+> LDA, INDL <> STA, 3 <X+> LDA, INDH <> STA,
  4 <X+> LDA, 'N <> STA, 5 <X+> LDA, 'N 1+ <> STA,
  0 4 PS<>, 1 <X+> LDY, INY, 5 <X+> STY, 2INX, 2INX,
  begin,
    L1 JSR, ifnz, ( fail ) 0 PSCLR16, ;code then,
    1 <X+> DEC, br BNE,
  ( success ) 0 <X+> INC, ;code
( ----- 235 )
code find ( s -- w-or-0 ) \ 0=cnt 1=sl IND=curword N=sa
  0 PS>A, 'N <> STA, 1 PS>A, 'N 1+ <> STA, 0 # LDY, 'N []Y+ LDA,
  1 A>PS, 'N <>INC16, SYSVARS $02 + dup () LDA, INDL <> STA,
  1+ () LDA, INDH <> STA, begin, ( loop )
    INDH <> LDA, PHA, INDL <> LDA, PHA, SEC, 3 # SBC,
    ifnc, INDH <> DEC, then, INDL <> STA, 0 # LDY,
    INDL []Y+ LDA, PHA, INY, INDL []Y+ LDA, PHA, INY, \ prev
    INDL []Y+ LDA, $7f # AND, 1 <X+> CMP, ifz, \ same cnt
      0 <X+> STA, INDL <> LDA, SEC, 0 <X+> SBC, INDL <> STA,
      ifnc, INDH <> DEC, then, L1 JSR, ifz, \ match
        PLA, PLA, PLA, 0 A>PS, PLA, 1 A>PS, ;code then, then,
    PLA, INDH <> STA, PLA, INDL <> STA, INDH <> ORA, ifz, \ end
      0 A>PS, 1 A>PS, PLA, PLA, ;code then, PLA, PLA,
  ( loop ) JMP,
( ----- 236 )
code cidx ( c a u -- ?i f ) \ [IND]=a 0=uL 1=uH+1 5=iH [N]=c
  4 PS>A, 'N <> STA, 2 PS>A, INDL <> STA, 3 PS>A, INDH <> STA,
  0 # LDA, 5 A>PS, 0 PS>A, 1 <X+> ORA, ifnz, \ u!= 0
    1 <X+> INC, ( 1=uH+1 ) begin,
     'N <> LDA, 0 # LDY, begin,
       INDL []Y+ CMP, ifz, \ match!
         2INX, 2 <X+> STY,
         0 # LDY, 1 <X+> STY, INY, 0 <X+> STY, ;code then,
       INY, 0 <X+> DEC, br BNE,
     5 <X+> INC, INDH <> INC, 1 <X+> DEC, br BNE, then,
  ( no match ) 2INX, 2INX, 0 # LDA, 0 A>PS, 1 A>PS, ;code
( ----- 237 )
code A> 2DEX, 'A <> LDA, 0 A>PS, 'A 1+ <> LDA, 1 A>PS, ;code
code >A 0 PS>A, 'A <> STA, 1 PS>A, 'A 1+ <> STA, 2INX, ;code
code A>r 'A 1+ <> LDA, PHA, 'A <> LDA, PHA, ;code
code r>A PLA, 'A <> STA, PLA, 'A 1+ <> STA, ;code
code A+ 'A <> INC, ifz, 'A 1+ <> INC, then, ;code
code A- 'A <> LDA, ifz, 'A 1+ <> DEC, then, 'A <> DEC, ;code
code Ac@
  2DEX, 0 # LDY, 1 <X+> STY, 'A []Y+ LDA, 0 A>PS, ;code
code Ac!  0 # LDY, 0 PS>A, 'A []Y+ STA, 2INX, ;code
( ----- 240 )
\ 6809 Boot code. IP=Y, PS=S, RS=U
PS_ADDR # LDS, RS_ADDR # LDU, 0 () JMP, pc 2 - to lblboot
pc to lblval SYSVARS $18 ( TO? ) + () TST, fjr BNE, to L1
  ( val rd ) [S+0] LDD, S+0 STD, \ to next
pc to lblcell pc to lblnext Y++ LDX, X+0 JMP,
L1 fjr! ( val wr ) SYSVARS $18 + () CLR, 2 S+N LDD,
  [S++] STD, S++ TST, lblnext br BRA,
pc to lblxt U++ STY, ( IP->RS ) PULS, Y lblnext br jr,
pc to lbldoes [S+0] LDX, 2 # LDD, S+0 ADDD, S+0 STD, X+0 JMP,
code quit pc to L1 ( for ABORT )
  RS_ADDR # LDU, 0 () JMP, pc 2 - to lblmain
code abort PS_ADDR # LDS, L1 br jr,
code bye 0 jr,
code exit --U LDY, ;code
code execute PULS, X X+0 JMP,
( ----- 241 )
code scnt PS_ADDR # LDD, 0 <> STS, 0 <> SUBD, PSHS, D ;code
code rcnt
  RS_ADDR # LDD, 0 <> STD, U D TFR, 0 <> SUBD, PSHS, D ;code
code @ [S+0] LDD, S+0 STD, ;code
code c@ [S+0] LDB, CLRA, S+0 STD, ;code
code ! PULS, X PULS, D X+0 STD, ;code
code c! PULS, X PULS, D X+0 STB, ;code
pc to L1 ( PUSH Z ) CCR B TFR, LSRB, LSRB,
  1 # ANDB, CLRA, S+0 STD, ;code
code = PULS, D S+0 CMPD, L1 br BRA, ( PUSH Z )
code not S+0 LDB, 1 S+N ORB, L1 br BRA, ( PUSH Z )
code <
  2 S+N LDD, S++ CMPD, CCR B TFR,
  1 # ANDB, CLRA, S+0 STD, ;code
( ----- 242 )
code /mod ( a b -- a/b a%b )
  16 # LDA, 0 <> STA, CLRA, CLRB, ( D=running rem ) begin,
    1 # ORCC, 3 S+N ROL, ( a lsb ) 2 S+N ROL, ( a msb )
    ROLB, ROLA, S+0 SUBD,
    fjr BHS, ( if < ) S+0 ADDD, 3 S+N DEC, ( a lsb ) then,
  0 <> DEC, br jrnz,
  2 S+N LDX, 2 S+N STD, ( rem ) S+0 STX, ( quotient ) ;code
code * ( a b -- a*b )
  S+0 ( bm ) LDA, 3 S+N ( al ) LDB, MUL, S+0 ( bm ) STB,
  2 S+N ( am ) LDA, 1 S+N ( bl ) LDB, MUL,
    S+0 ( bm ) ADDB, S+0 STB,
  1 S+N ( al ) LDA, 3 S+N ( bl ) LDB, MUL,
  S++ ADDA, S+0 STD, ;code
( ----- 243 )
pc to L1 ( X=s1 Y=s2 B=cnt ) begin,
  X+ LDA, Y+ CMPA, ifnz, RTS, then, DECB, br jrnz, RTS,
code []= ( a1 a2 u -- f TODO: allow u>$ff )
  0 <> STY, PULS, DXY ( B=u, X=a2, Y=a1 ) L1 () JSR,
  ifz, 1 # LDD, else, CLRA, CLRB, then, PSHS, D 0 <> LDY, ;code
code FIND ( sa sl -- w? f ) \ TODO: change to new semantics
  SYSVARS $02 + ( CURRENT ) () LDX,
  0 <> STY, PULS, D 2 <> STB, begin,
    -X LDB, $7f # ANDB, --X TST, 2 <> CMPB, ifz,
      3 <> STX, S+0 LDY, NEGB, X+B LEAX, NEGB, L1 () JSR,
      ifz, ( match ) 0 <> LDY, 3 <> LDD, 3 # ADDD, S+0 STD,
        1 # LDD, PSHS, D ;code then,
      3 <> LDX, then, \ nomatch, X=prev
  X+0 LDX, br jrnz, \ not zero, loop
  ( end of dict ) 0 <> LDY, S+0 STX, ( X=0 ) ;code
( ----- 244 )
code and PULS, D S+0 ANDA, 1 S+N ANDB, S+0 STD, ;code
code or PULS, D S+0 ORA, 1 S+N ORB, S+0 STD, ;code
code xor PULS, D S+0 EORA, 1 S+N EORB, S+0 STD, ;code
code + PULS, D S+0 ADDD, S+0 STD, ;code
code - 2 S+N LDD, S++ SUBD, S+0 STD, ;code
code 1+ 1 S+N INC, ifz, S+0 INC, then, ;code
code 1- 1 S+N TST, ifz, S+0 DEC, then, 1 S+N DEC, ;code
code << 1 S+N LSL, S+0 ROL, ;code
code >> S+0 LSR, 1 S+N ROR, ;code
code <<8 1 S+N LDA, S+0 STA, 1 S+N CLR, ;code
code >>8 S+0 LDA, 1 S+N STA, S+0 CLR, ;code
( ----- 245 )
code r@ -2 U+N LDD, PSHS, D ;code
code r~ --U TST, ;code
code r> --U LDD, PSHS, D ;code
code >r PULS, D U++ STD, ;code
code drop 2 S+N LEAS, ;code
code dup ( a -- a a ) S+0 LDD, PSHS, D ;code
code ?dup ( a -- a? a ) S+0 LDD, ifnz, PSHS, D then, ;code
code swap ( a b -- b a )
  S+0 LDD, 2 S+N LDX, S+0 STX, 2 S+N STD, ;code
code over ( a b -- a b a ) 2 S+N LDD, PSHS, D ;code
code rot ( a b c -- b c a )
  4 S+N LDX, ( a ) 2 S+N LDD, ( b ) 4 S+N STD, S+0 LDD, ( c )
  2 S+N STD, S+0 STX, ;code
code rot> ( a b c -- c a b )
  S+0 LDX, ( c ) 2 S+N LDD, ( b ) S+0 STD, 4 S+N LDD, ( a )
  2 S+N STD, 4 S+N STX, ;code
( ----- 246 )
code (b) Y+ LDB, CLRA, PSHS, D ;code
code (n) Y++ LDD, PSHS, D ;code
code (br) pc to L1 Y+0 LDA, Y+A LEAY, ;code
code (?br) S+ LDA, S+ ORA, L1 br jrz, Y+ TST, ;code
code (next) --U LDD, 1 # SUBD, ifnz,
  U++ STD, L1 br jr, then, Y+ TST, ;code
( ----- 300 )
\ Cross compilation program, generic part. See doc/cross
0 value bin( \ binary start in target's addr
0 value xorg \ binary start address in host's addr
0 value bigend? \ is target big-endian?
4 values L1 L2 L3 L4
: pc here xorg - bin( + ;
: pc2a ( pc -- a ) here pc - ( org ) + ;
: xstart ( bin( -- ) to bin( here to xorg ;
: oallot ( oa -- ) xorg + here - allot0 ;
: |t l|m bigend? not if swap then ;
: t! ( n a -- ) swap |t rot c!+ c! ;
: t, ( n -- ) |t c, c, ;
: t@ c@+ swap c@ bigend? if swap then <<8 or ;
( ----- 301 )
\ Cross compilation program. COS-specific. See doc/cross
: corel 310 324 loadr ; : coreh 325 329 loadr ;
: blksub 330 334 loadr ; : gridsub 340 341 loadr ;
: rxtxsub 345 load ; : ps2sub 350 352 loadr ;
'? HERESTART not [if] 0 value HERESTART [then]
0 value xcurrent \ CURRENT in target system, in target's addr
8 values lblnext lblcell lbldoes lblxt lblval
  lblhere lblmain lblboot
'? 'A not [if] SYSVARS $06 + value 'A [then]
'? 'N not [if] SYSVARS $08 + value 'N [then]
: variables for create 0 , next ;
7 variables (n)* (b)* (br)* (?br)* exit* (next)* >r*
create '~ cell allot
( ----- 302 )
\ Cross compilation program
: _xoff ( a -- a ) xorg bin( - ;
: _wl ( w -- len ) 1- c@ $7f and ;
: _ws ( w len -- sa ) - 3 - ;
: _xfind ( s -- w? f ) c@+ >r >A xcurrent begin ( w R:sl )
  _xoff + dup _wl r@ = if ( w ) dup r@ _ws A> r@ ( w a1 a2 u )
  []= if ( w ) _xoff - r~ 1 exit then then
  3 - ( prev field ) t@ ?dup not until r~ 0 ( not found ) ;
: xfind ( s -- w ) _xfind not if (wnf) then ;
: x' word xfind ;
: entry
  word c@+ tuck move, xcurrent t, c, here _xoff - to xcurrent ;
( ----- 303 )
\ Cross compilation program
: ;code lblnext jmp, ;
alias alias _alias \ for ":" and ";" at the end of xcomp
: alias x' entry jmp, ; : *alias entry @jmp, ;
: constant entry i>, ;code ;
: consts for run1 constant next ;
: consts+ ( off n -- )
  for run1 over + constant next drop ;
: *value entry i@>, ;code ; : create entry lblcell call, ;
: _ ( lbl str -- )
  curword s= if xcurrent swap ! else drop then ;
: code entry exit* S" exit" _ (b)* S" (b)" _
  (n)* S" (n)" _ (br)* S" (br)" _ >r* S" >r" _
  (?br)* S" (?br)" _ (next)* S" (next)" _ ;
: litn dup $ff > if (n)* @ t, t, else (b)* @ t, c, then ;
: ximm? ( w -- f ) _xoff + 1- c@ $80 and ;
( ----- 304 )
\ Cross compilation program
: _ curword word! ' dup imm? if execute else (wnf) then ;
: ] 1 COMPILING c! begin
  word parse if litn else
    curword _xfind if
      dup ximm? if drop _ else t, then else _ then then
  COMPILING c@ not until ;
: :~ here _xoff - '~ ! lblxt call, ] ;
: ~ '~ @ t, ; immediate
: _: code lblxt call, ] ; \ : can't have its name now
: _; exit* @ t, [compile] [ ; immediate \ ; neither
: '? word _xfind not if 0 then ;
: ?: '? if S" ;" waitw else curword word! _: then ;
: ~doer entry lbldoes call, [compile] ~ ;
: xwrap coreh xcurrent lblhere pc2a t!
  HERESTART ?dup not if pc then lblhere pc2a 1+ 1+ t! ;
( ----- 305 )
\ Cross compilation program
: ['] word xfind litn ; immediate
: compile [compile] ['] S" ," xfind t, ; immediate
: then dup here -^ _bchk swap c! ; immediate
: if (?br)* @ t, here 1 allot ; immediate
: else (br)* @ t, 1 allot [compile] then here 1- ; immediate
: again (br)* @ t, here - c, ; immediate
: until (?br)* @ t, here - c, ; immediate
: for >r* @ t, here ; immediate
: next (next)* @ t, here - c, ; immediate
: S" (br)* @ t, here 0 c, here 0 c, ," here over - 1- over c!
  swap [compile] then _xoff - litn ; immediate
: [compile] word xfind t, ; immediate
: _immediate xcurrent _xoff + 1- dup c@ $80 or swap c! ;
_alias _: : _alias _; ; immediate _alias _immediate immediate
0 xstart
( ----- 310 )
\ Core Forth words. See doc/cross. SYSVARS cell cells noop
SYSVARS 14 consts+
  $00 IOERR $02 CURRENT $04 HERE     $0a NL     $0c LN<
  $0e EMIT  $10 KEY?    $12 NEXTWORD $16 WNF    $19 COMPILING
  $1c IN(   $1e IN>     $20 INBUF    $60 CURWORD
SYSVARS $02 + *value current    SYSVARS $04 + *value here
SYSVARS $0e + *alias emit       SYSVARS $10 + *alias key?
SYSVARS $1c + *value in(        SYSVARS $1e + *value in>
$40 constant LNSZ
2 constant cell
: cells << ;
code noop ;code
( ----- 311 )
\ Core words, basic arithmetic and stack management
?: = - not ;
?: > swap < ;
?: 0< $7fff > ; ?: 0>= $8000 < ; ?: >= < not ; ?: <= > not ;
?: 1+ 1 + ; ?: 1- 1 - ;
?: 2drop drop drop ;
?: 2dup over over ;
?: nip swap drop ;
?: tuck swap over ;
?: rot> rot rot ;
?: =><= ( n l h -- f ) over - rot> ( h n l ) - >= ;
: / /mod nip ; : mod /mod drop ;
?: ?swap ( n n -- l h ) 2dup > if swap then ;
?: min ?swap drop ; ?: max ?swap nip ; ?: -^ swap - ;
( ----- 312 )
\ Core words, bit shifting, A register, leave l|m +!
?: << 2 * ;     ?: >> 2 / ;
?: <<8 $100 * ; ?: >>8 $100 / ;
?: rshift ?dup if for >> next then ;
?: lshift ?dup if for << next then ;
?: l|m dup <<8 >>8 swap >>8 ;
?: +! ( n a -- ) tuck @ + swap ! ;
?: A> [ 'A litn ] @ ;    ?: >A [ 'A litn ] ! ;
?: A>r r> A> >r >r ;     ?: r>A r> r> >A >r ;
?: A+ 1 [ 'A litn ] +! ; ?: A- -1 [ 'A litn ] +! ;
?: Ac@ A> c@ ;           ?: Ac! A> c! ;
: Ac@+ Ac@ A+ ;          : Ac!+ Ac! A+ ;
: leave r> r~ 1 >r >r ;
?: to 1 [ SYSVARS $18 + litn ] c! ;
( ----- 313 )
\ Core words, c@+ allot fill immediate , l, m, move move, ..
?: c@+ dup 1+ swap c@ ;
?: c!+ tuck c! 1+ ;
: allot HERE +! ;
?: fill ( a u b -- ) \ *A*
  rot >A swap for dup Ac!+ next drop ;
: allot0 ( u -- ) here over 0 fill allot ;
: immediate current 1- dup c@ $80 or swap c! ;
: , here ! 2 allot ; : c, here c! 1 allot ;
: l, dup c, >>8 c, ; : m, dup >>8 c, c, ;
?: move ( src dst u -- ) ?dup if
  swap >A for ( src ) c@+ Ac!+ next drop then ;
: move, ( a u -- ) here over allot swap move ;
( ----- 314 )
\ Core words, cidx crc16 []= jmpi! calli!
?: jmpi! [ x' noop pc2a c@ ( jmp op ) litn ] swap c!+ ! 3 ;
?: calli! [ x' move, pc2a c@ ( call op ) litn ] swap c!+ ! 3 ;
?: cidx ( c a u -- ?i f ) \ Guards A
  ?dup not if 2drop 0 exit then A>r swap dup >r >A ( c u )
  for dup Ac@+ = if leave then next ( c )
  A- Ac@ = if A> r> - ( i ) 1 else r~ 0 then r>A ;
?: []= ( a1 a2 u -- f ) \ Guards A
  ?dup not if 2drop 1 exit then A>r swap >A ( a1 u )
  for Ac@+ over c@ = not if r~ r>A drop 0 exit then 1+ next
  drop r>A 1 ;
?: crc16 ( c n -- c )
  <<8 xor 8 for ( c )
    dup 0< if << $1021 xor else << then next ;
( ----- 315 )
\ Core words, stype spc> nl> stack? litn
: rtype swap >A for Ac@+ emit next ;
: stype c@+ rtype ;
5 consts $04 EOT $08 BS $0a LF $0d CR $20 SPC
: spc> SPC emit ;
: nl> NL @ l|m ?dup if emit then emit ;
: stack? scnt 0< if S" stack underflow" stype abort then ;
: litn dup >>8 if compile (n) , else compile (b) c, then ;
( ----- 316 )
\ Core words, number formatting
: fmtd ( n a -- s ) \ *A*
  7 + >A A>r dup >r dup 0< if 0 -^ then begin ( n )
    10 /mod ( d q ) A- swap '0' + Ac! ?dup not until
  r> 0< if A- '-' Ac! then r> A> - A- Ac! A> ;
pc to L1 ," 0123456789abcdef"
:~ ( n -- c n>>4 )
  dup $f and [ L1 litn ] + c@ swap 4 rshift ;
: fmtx ( n a -- s ) \ *A*
  >A 2 Ac!+ ~ ~ drop Ac!+ Ac! A> 2 - ;
: fmtX ( n a -- s ) \ *A*
  >A 4 Ac!+ ~ ~ ~ ~ drop Ac!+ Ac!+ Ac!+ Ac! A> 4 - ;
:~ ( n 'w -- s ) @ A>r here swap execute stype r>A ;
~doer . x' fmtd t,
~doer .x x' fmtx t,
~doer .X x' fmtX t,
( ----- 317 )
\ Core words, literal parsing
:~ ( sl -- n? f ) \ parse unsigned decimal
  0 swap for ( r )
    10 * Ac@+ ( r c ) '0' - dup 9 > if
      2drop r~ 0 exit then + next ( r ) 1 ;
: parse ( s -- n? f ) \ *A*
  c@+ over c@ ''' = if ( sa sl )
    3 = if 1+ dup 1+ c@ ''' = if c@ 1 exit then then
    drop 0 exit then ( sa sl )
  over c@ '$' = if ( sa sl ) 1- swap 1+ >A 0 swap for ( r )
    16 * Ac@+ ( r c ) $20 or [ L1 litn ] ( B216 ) $10 cidx
    not if drop r~ 0 exit then + next ( r ) 1 exit then
  swap >A dup 1 > Ac@ '-' = and if ( sl )
    A+ 1- ~ if 0 -^ 1 else 0 then else ~ then ;
( ----- 318 )
\ Core words, input buffer
: key begin key? until ;
: in) in( LNSZ + ;
pc BS c, $7f ( DEL ) c,
: bs? [ ( PC ) litn ] 2 cidx dup if nip then ;
: ws? SPC <= ;
\ type c into ptr inside INBUF. f=true if typing should stop
: lntype ( ptr c -- ptr+-1 f )
  dup bs? if ( ptr c )
    drop dup in( > if 1- BS emit then spc> BS emit 0
  else ( ptr c ) \ non-BS
    dup SPC < if drop dup in) over - 0 fill 1 else
      tuck emit c!+ dup in) = then then ;
( ----- 319 )
\ Core words, input buffer, ,"
: rdln ( -- ) \ Read 1 line in IN(
  S"  ok" stype nl> in( begin key lntype until drop nl> ;
: in<? ( -- c-or-0 )
  in> in) < if in> c@+ swap IN> ! else 0 then ;
: in< ( -- c ) in<? ?dup not if
    LN< @ execute in( IN> ! SPC then ;
: in$ ['] rdln LN< ! INBUF IN( ! in) IN> ! ;
: ," begin in< dup '"' = if drop exit then c, again ;
( ----- 320 )
\ Core words, word parsing
: toword ( -- c ) 0 begin drop in< dup ws? not until ;
: curword ( -- s ) CURWORD ;
: word! NEXTWORD ! ;
: word ( -- s )
  NEXTWORD @ ?dup if 0 word! else
    0 CURWORD 1+ toword begin ( len a c )
      swap c!+ swap 1+ swap ( len+1 a+1 )
      in<? dup ws? until 2drop ( len )
    CURWORD tuck c! then ;
( ----- 321 )
\ Core words, find (wnf) run1 interpret nc,
?: find ( s -- w-or-0 ) \ Guards A
  A>r c@+ >r >A current begin ( w R:sl )
  dup 1- c@ $7f and ( wlen ) r@ = if ( w )
    dup r@ - 3 - A> r@ ( w a1 a2 u )
    []= if ( w ) r~ r>A exit then then
  3 - ( prev field ) @ ?dup not until r~ 0 r>A ( not found ) ;
: (wnf) curword stype S"  word not found" stype abort ;
: run1 ( -- ) \ interpret next word
  word parse not if
    curword find ?dup not if WNF @ then execute stack? then ;
: interpret begin run1 again ;
: nc, ( n -- ) for run1 c, next ;
( ----- 322 )
\ Core words, code '? ' to forget
: code word c@+ tuck move, ( len )
  current , c, \ write prev value and size
  here CURRENT ! ;
: '? word find ;
: ' word find ?dup not if (wnf) then ;
: forget
  ' dup ( w w )
  \ here must be at the end of prev's word, that is, at the
  \ beginning of w.
  dup 1- c@ ( len ) $7f and ( rm immediate )
  3 + ( fixed header len ) - HERE ! ( w )
  ( get prev addr ) 3 - @ CURRENT ! ;
( ----- 323 )
\ Core words, s= waitw [if] _bchk
: s= ( s1 s2 -- f ) over c@ 1+ []= ;
: waitw ( s -- ) begin dup word s= until drop ;
: [if] not if S" [then]" waitw then ;
alias noop [then]
: _bchk dup $80 + $ff > if S" br ovfl" stype abort then ;
( ----- 324 )
\ Core words, DUMP .S
: dump ( n a -- ) \ *A*
  >A 8 /mod swap if 1+ then for
    ':' emit A> dup .x spc> ( a )
    4 for Ac@+ .x Ac@+ .x spc> next ( a ) >A
    8 for Ac@+ dup SPC - $5e > if drop '.' then emit next
  nl> next ;
: psdump scnt not if exit then
  scnt >A begin dup .X spc> >r scnt not until
  begin r> scnt A> = until ;
: .S ( -- )
  S" SP " stype scnt .x spc> S" RS " stype rcnt .x spc>
  S" -- " stype stack? psdump ;
( ----- 325 )
\ Core high, create doer does> code alias value
: ;code [ lblnext litn ] here jmpi! allot ;
: create code [ lblcell litn ] here calli! allot ;
: doer code [ lbldoes litn ] here calli! 1+ 1+ allot ;
: _ r> current 3 + ! ; \ Popping RS makes us EXIT from parent
: does> compile _ [ lblxt litn ] here calli! allot ; immediate
: alias ' code here jmpi! allot ;
: value code [ lblval litn ] here calli! allot , ;
: values for 0 value next ;
: consts for run1 value next ;
( ----- 326 )
\ Core high, boot
:~ in$ interpret bye ;
'~ @ lblmain pc2a t! \ set jump in QUIT
pc to lblhere 4 allot \ CURRENT, HERESTART
: boot [ lblhere litn ] CURRENT 4 move
  ['] (emit) EMIT ! ['] (key?) KEY? ! ['] (wnf) WNF !
  0 word! 0 IOERR ! $0d0a ( CR/LF ) NL !
  0 [ SYSVARS $18 ( TO? ) + litn ] c!
  init S" Collapse OS" stype abort ;
xcurrent lblboot pc2a t! \ initial jump to BOOT
( ----- 327 )
\ Core high, :
: imm? ( w -- f ) 1- c@ $80 and ;
: ] 1 COMPILING c! begin
    word parse if litn else curword find ?dup if
      dup imm? if execute else , then
    else WNF @ execute then then
  COMPILING c@ not until ;
: xtcomp [ lblxt litn ] here calli! allot ] ;
: : code xtcomp ;
: [ 0 COMPILING c! ; immediate
: _ compile exit [compile] [ ; immediate
';' xcurrent _xoff + 4 - c!
( ----- 328 )
\ Core high, if..else..then ( \
: if ( -- a | a: br cell addr )
  compile (?br) here 1 allot ( br cell allot ) ; immediate
: then ( a -- | a: br cell addr )
  dup here -^ _bchk swap ( a-H a ) c! ; immediate
: else ( a1 -- a2 | a1: IF cell a2: ELSE cell )
  compile (br) 1 allot [compile] then
  here 1- ( push a. 1- for allot offset ) ; immediate
: ( S" )" waitw ; immediate
: \ in) IN> ! ; immediate
: S"
  compile (br) here 0 c, here 0 c, ," here over - 1- over c!
  swap [compile] then litn ; immediate
( ----- 329 )
\ Core high, .", abort", begin..again..until, many others.
: ." [compile] S" compile stype ; immediate
: abort" [compile] ." compile abort ; immediate
: begin here ; immediate
: again compile (br) here - _bchk c, ; immediate
: until compile (?br) here - _bchk c, ; immediate
: next compile (next) here - _bchk c, ; immediate
: for compile >r here ; immediate
: compile ' litn ['] , , ; immediate
: [compile] ' , ; immediate
: ['] ' litn ; immediate
( ----- 330 )
\ BLK subsystem. See doc/blk
BLK_MEM constant blk( \ $400 + "\s "
BLK_MEM $400 + constant blk)
\ Current blk pointer -1 means "invalid"
BLK_MEM $403 + dup constant BLK> *value blk>
\ Whether buffer is dirty
BLK_MEM $405 + constant BLKDTY
BLK_MEM $407 + constant BLKIN>
create _ '\' c, 's' c, SPC c,
: blk$ 0 BLKDTY ! -1 BLK> ! _ blk) 3 move ;
( ----- 331 )
: blk! ( -- ) blk> blk( (blk!) 0 BLKDTY ! ;
: flush BLKDTY @ if blk! then -1 BLK> ! ;
: blk@ ( n -- )
  dup blk> = if drop exit then
  flush dup BLK> ! blk( (blk@) ;
: blk!! 1 BLKDTY ! ;
: wipe blk( 1024 SPC fill blk!! ;
: copy ( src dst -- ) flush swap blk@ BLK> ! blk! ;
( ----- 332 )
: lnlen ( a -- len ) \ len based on last visible char in line
  1- LNSZ for
    dup r@ + c@ SPC > if drop r> exit then next drop 0 ;
: emitln ( a -- ) \ emit LNSZ chars from a or stop at CR
  dup lnlen ?dup if rtype else drop then nl> ;
: list ( n -- ) \ print contents of BLK n
  blk@ 0 16 for ( n )
    dup 1+ dup 10 < if spc> then . spc>
    dup LNSZ * blk( + emitln 1+ next drop ;
: index ( b1 b2 -- ) \ print first line of blocks b1 through b2
  over - 1+ for
    dup . spc> dup blk@ blk( emitln 1+ next drop ;
( ----- 333 )
: \s blk) IN( ! in( IN> ! ;
:~ ( -- ) in) IN( ! ;
: load
  in> BLKIN> ! [ '~ @ litn ] LN< ! blk@ blk( IN( ! in( IN> !
  begin run1 in( blk) = until in$ BLKIN> @ IN> ! ;
\ >R R> around LOAD is to avoid bad blocks messing PS up
: loadr over - 1+ for
  dup . spc> dup >r load r> 1+ next drop ;
( ----- 334 )
\ Application loader, to include in boot binary
: ed 1 load ( move- ) 10 14 loadr ;
: ve 5 load ( wordtbl ) ed 15 22 loadr ;
: me 25 29 loadr ;
: z80a 5 load ( wordtbl ) 100 109 loadr 7 load ( Flow ) ;
: z80c 200 211 loadr ;
: 8086a 5 load 110 117 loadr 7 load ; : 8086c 215 222 loadr ;
: 6502a 120 123 loadr 7 load ; : 6502d 125 129 loadr ;
: 6502m 225 load ; : 6502c 226 237 loadr ;
: 6809a 130 138 loadr 7 load ; : 6809c 240 246 loadr ;
: avra 140 150 loadr ;
: xcomp 300 load ; : xcompc 301 305 loadr ;
( ----- 340 )
\ Grid subsystem. See doc/grid.
GRID_MEM dup constant XYPOS *value xypos
?: cursor! 2drop ;
: xypos! COLS LINES * mod dup xypos cursor! XYPOS ! ;
: at-xy ( x y -- ) COLS * + xypos! ;
?: newln ( oldln -- newln )
  1+ LINES mod dup COLS * ( pos )
  COLS for SPC over cell! 1+ next drop ;
?: cells! ( a pos u -- )
  ?dup if rot >A for ( pos ) Ac@+ over cell! 1+ next
    else drop then drop ;
: stypec ( s pos -- ) swap c@+ swap rot> cells! ;
?: fillc ( pos n c )
  rot> for ( c pos ) 2dup cell! 1+ next 2drop ;
: clrscr 0 COLS LINES * SPC fillc 0 xypos! ;
( ----- 341 )
:~ ( line feed ) xypos COLS / newln COLS * xypos! ;
?: (emit)
  dup bs? if
    drop SPC xypos tuck cell! ( pos ) 1- xypos! exit then
  dup CR = if drop SPC xypos cell! ~ exit then
  dup SPC < if drop exit then
  xypos cell!
  xypos 1+ dup COLS mod if xypos! else drop ~ then ;
: grid$ 0 XYPOS ! ;
( ----- 345 )
\ RX/TX subsystem. See doc/rxtx
RXTX_MEM constant _emit
RXTX_MEM 2 + constant _key
: rx< begin rx<? until ;
: rx<< 0 begin drop rx<? not until ;
: tx[ EMIT @ _emit ! ['] tx> EMIT ! ;
: ]tx _emit @ EMIT ! ;
: rx[ KEY? @ _key ! ['] rx<? KEY? ! ;
: ]rx _key @ KEY? ! ;
( ----- 350 )
: PS2_SHIFT [ PS2_MEM litn ] ; : ps2$ 0 PS2_SHIFT c! ;
\ A list of the values associated with the $80 possible scan
\ codes of the set 2 of the PS/2 keyboard specs. 0 means no
\ value. That value is a character that can be read in (key?)
\ No make code in the PS/2 set 2 reaches $80.
\ TODO: I don't know why, but the key 2 is sent as $1f by 2 of
\ my keyboards. Is it a timing problem on the ATtiny?
create PS2_CODES $80 nc,
0   0   0   0   0   0   0   0 0 0   0   0   0   9   '`' 0
0   0   0   0   0   'q' '1' 0 0 0   'z' 's' 'a' 'w' '2' '2'
0   'c' 'x' 'd' 'e' '4' '3' 0 0 32  'v' 'f' 't' 'r' '5' 0
0   'n' 'b' 'h' 'g' 'y' '6' 0 0 0   'm' 'j' 'u' '7' '8' 0
0   ',' 'k' 'i' 'o' '0' '9' 0 0 '.' '/' 'l' ';' 'p' '-' 0
0   0   ''' 0   '[' '=' 0   0 0 0   13  ']' 0   '\' 0   0
0   0   0   0   0   0   8   0 0 '1' 0   '4' '7' 0   0   0
'0' '.' '2' '5' '6' '8' 27  0 0 0   '3' 0   0   '9' 0   0
( ----- 351 )
( Same values, but shifted ) $80 nc,
0   0   0   0   0   0   0   0 0 0   0   0   0   9   '~' 0
0   0   0   0   0   'Q' '!' 0 0 0   'Z' 'S' 'A' 'W' '@' '@'
0   'C' 'X' 'D' 'E' '$' '#' 0 0 32  'V' 'F' 'T' 'R' '%' 0
0   'N' 'B' 'H' 'G' 'Y' '^' 0 0 0   'M' 'J' 'U' '&' '*' 0
0   '<' 'K' 'I' 'O' ')' '(' 0 0 '>' '?' 'L' ':' 'P' '_' 0
0   0   '"' 0   '{' '+' 0   0 0 0   13  '}' 0   '|' 0   0
0   0   0   0   0   0   8   0 0 0   0   0   0   0   0   0
0   0   0   0   0   0   27  0 0 0   0   0   0   0   0   0
( ----- 352 )
: _shift? ( kc -- f ) dup $12 = swap $59 = or ;
: (key?) ( -- c? f )
  (ps2kc) dup not if exit then ( kc )
  dup $e0 ( extended ) = if ( ignore ) drop 0 exit then
  dup $f0 ( break ) = if drop ( )
    ( get next kc and see if it's a shift )
    begin (ps2kc) ?dup until ( kc )
    _shift? if ( drop shift ) 0 PS2_SHIFT c! then
    ( whether we had a shift or not, we return the next )
    0 exit then
  dup $7f > if drop 0 exit then
  dup _shift? if drop 1 PS2_SHIFT c! 0 exit then
  ( ah, finally, we have a gentle run-of-the-mill KC )
  PS2_CODES PS2_SHIFT c@ if $80 + then + c@ ( c, maybe 0 )
  ?dup ( c? f ) ;
