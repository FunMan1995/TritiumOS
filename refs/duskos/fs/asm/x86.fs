needs arch/core lib/psrs asm/label
unit asm/x86

: regid@ 7 and ;
: mod@ 6 rshift 3 and ;
: mod! ( opmod mod -- opmod ) 6 lshift swap $c0 invand or ;
: modrm@ $c7 and ;
: bankid@ 20 rshift $f and ;
: newbankedop ( n -- opmod ) hbank! 20 lshift ;
: sib@ 24 rshift ;
: imm? $40000 and bool ;
: 8b? bi $100 and not | $ff00 and $8300 = or ;
: has66hprefix? $20000 and bool ;
: special? $38 and bool ;

0 value _imm?
0 value _imm
0 value _has66h
0 value rex
1 value mode \ 0=realmode 1=32bmode 2=64bmode
: realmode 0 to mode ; : realmode? mode 0 = ;
: 32bmode 1 to mode ;  : 32bmode? mode 1 = ;
: 64bmode 2 to mode ;  : 64bmode? mode 2 = ;
: livemode FAMILY_amd64 instrfamily? 1+ to mode ;
livemode

: sib? bi mod@ 3 < | regid@ 4 = and realmode? not and ;
: memmodrm realmode? $05 + ;
: ismem? modrm@ memmodrm = ;
: indirect? bi mod@ 3 <> | ismem? or ;

: asm$ 0 to _has66h 0 to _imm? 0 to rex ;
: _err asm$ abort"asm error" ;
: _assert not if _err then ;

: _ does> $c0 or ; map< _
  0 al    1 cl    2 dl    3 bl    4 ah    5 ch    6 dh    7 bh \
  8 es    9 cs    10 ss   11 ds   12 fs   13 gs \
  $10 cr0 $12 cr2 $13 cr3 \
  $18 dr0 $19 dr1 $1a dr2 $1b dr3 $1c dr4 $1d dr5 $1e dr6 $1f dr7 \
  $26 tr6 $27 tr7
: _ does> $1c0 or ; map< _
  0 ax    1 cx    2 dx    3 bx    4 sp    5 bp    6 si    7 di \
  0 r8    1 r9    2 r10   3 r11   4 r12   5 r13   6 r14   7 r15

\ disp32=2 disp8=1 none=0 bp/sp=special
: dispmod ( disp -- mod ) dup if $100 >= 1+ then ;
: notreal# ( -- ) realmode? not _assert ;
: real# ( -- ) realmode? _assert ;
: setb8 ( opmod -- opmod ) $100 or ;

: _d) ( opmod n -- opmod ) tuck dispmod mod! $ff0fffff and swap newbankedop or ;
: _ ( reg -- ) does> ( n reg ) real# setb8 swap _d) ;
0 _ bx+si) 1 _ bx+di) 2 _ bp+si) 3 _ bp+di) 4 _ si+) 5 _ di+) 6 _ _bp+) 7 _ bx+)
: bp+) ?dup if _bp+) else 1 _bp+) 0 over bankid@ hbank' ! then ;

: imm) newbankedop $40000 or setb8 ;

: d) notreal# _d)
     dup ismem? if ( bp+0 ) $40 + then
     dup sib? if ( sp+n ) dup 24 rshift not if $24000000 or then then ;
: abs) 64bmode? if bp swap d) else newbankedop memmodrm setb8 or then ;

:~  ( opmod reg ss -- opmod )
  >r r! regid@ 4 = ?abort"can't use r+) with sp" ( opmod ) \ V1=ss V2=reg
  dup mod@ 3 = if 0 d) then
  bi $fffff8 and | regid@ 24 lshift or 4 or \ move regid to SIB's base
  r> 27 lshift or r> 30 lshift or ;
: r+) 0 ~ ; : 2r+) 1 ~ ; : 4r+) 2 ~ ; : 8r+) 3 ~ ;

: _remsz $fffffeff and ;
alias _remsz byte)
: _set66h $20000 or ;
: _clear66h $fffdffff and ;
: word) realmode? if _clear66h else _set66h then setb8 ;
: dword) realmode? if _set66h else _clear66h then setb8 ;

: rex.w doto rex $48 or | ;
: rex.r doto rex $44 or | ;
: rex.x doto rex $42 or | ;
: rex.b doto rex $41 or | ;

: _addr, realmode? if wle, else le, then ;
: _data, realmode? _has66h xor if wle, else le, then ;
: ?disp, ( opmod -- ) dup mod@ case ( opmod )
    0 = of dup ismem? if bankid@ hbank@ _addr, else drop then endof
    1 = of bankid@ hbank@ c, endof
    2 = of bankid@ hbank@ _addr, endof
    2drop endcase ;
: ?8b, ( n f -- ) if c, else _data, then ;
: ?imm, ( opmod -- ) _imm? if 8b? _imm swap ?8b, else drop then ;
: ?sib, ( opmod -- ) dup sib? if sib@ c, else drop then ;
: checkimm ( arg -- f ) dup bankid@ hbank@ to _imm imm? dup to _imm? ;
: check66h ( arg -- ) has66hprefix? if 1 to _has66h then ;
: notimm# ( opmod -- ) checkimm not _assert ;
: op, ( opcode -- )
  _has66h if $66 c, then
  rex ?dup if c, then
  dup $100 and if $0f c, then c, ;
: modrm, ( opmod -- ) dup c, dup ?sib, dup ?disp, ?imm, asm$ ;
: opmod, ( opmod -- ) dup check66h dup 256/ op, modrm, ;

\ Inherent
: op ( opcode -- ) does> op, asm$ ; map< op
  $c3 ret,        $90 nop,         $f4 hlt, \
  $fa cli,        $fb sti,         $fc cld,         $fd std, \
  $ac lodsb,      $aa stosb, \
  $a6 cmpsb, \
  $a4 movsb,      $ae scasb, \
  $6c insb,       $6e outsb, \
  $f3 repz,       $f2 repnz,       $f3 rep, \
  $60 pusha,      $61 popa, \
  $9c pushf,      $9d popf,        $cf iret,

\ Inherent 32-bit
: op ( opcode -- ) does> realmode? if $66 c, then op, asm$ ; map< op
  $ad lods,      $ab stos,       $a7 cmps,       $a5 movs, \
  $af scas,      $6d ins,        $6f outs,

\ Inherent 16-bit
: op ( opcode -- ) does> realmode? not if $66 c, then op, asm$ ; map< op
  $ad lodsw,      $ab stosw,       $a7 cmpsw,       $a5 movsw, \
  $af scasw,      $6d insw,        $6f outsw,

\ Single operand
: _single, over notimm# or opmod, ;
: op ( opmod -- ) does> ( arg opmod -- ) _single, ; map< op
  $f618 neg,      $f610 not,       $f620 mul,       $f630 div, \
  $fe08 dec,      $fe00 inc,       $f628 imul,      $f638 idiv,

: op ( opmod -- ) does> ( arg opmod -- ) dip _remsz | _single, ; map< op
  $19f00 setg,    $19c00 setl,     $19700 seta,     $19200 setb, \
  $19d00 setge,   $19e00 setle,    $19300 setae,    $19600 setbe, \
  $19c00 setnge,  $19f00 setnle,   $19200 setnae,   $19700 setnbe, \
  $19400 setz,    $19500 setnz,    $19200 setc,     $19300 setnc, \
  $19000 seto,    $19100 setno,    $19800 sets,     $19900 setns, \
  $19a00 setp,    $19b00 setnp,    $19a00 setpe,    $19b00 setpo, \
  $19400 sete,    $19500 setne, \
  $10110 lgdt,    $10118 lidt,

: AX? modrm@ $c0 = rex not and ;
: ?signext ( opmod -- opmod )
  dup 8b? not if _imm $80 + $100 < if $300 or doto _imm $ff and | then then ;
: _ax, ( ax opmod -- ) or dup 256/ op, ?imm, asm$ ;
: _modrm, ( regarg modarg opmod -- )
  or over 8b? if byte) then swap dup check66h regid@ 8* or opmod, ;
: _regular, ( dst src opmod -- )
  oover indirect? if dipswap else $200 or then _modrm, ;
: op ( idx ) does> 8* ( dst src idx*8 )
  over checkimm if
    nip over AX? if 4 or 256* _ax, else swap ?signext or $8000 or opmod, then
    else 256* _regular, then ;
map< op 0 add, 1 or, 2 adc, 3 sbb, 4 and, 5 sub, 6 xor, 7 cmp,

: _swappable ( dst src opcode -- ) oover indirect? if dipswap then _modrm, ;
: test, ( dst src -- )
  dup checkimm if
    drop dup AX? if $a800 _ax, else $f600 or opmod, then
    else $8400 _swappable then ;

: _1bmerge, ( arg opcode -- ) swap dup check66h regid@ or op, asm$ ;
: xchg, ( dst src -- )
  dup notimm# over AX? if swap then
  over indirect? not over AX? and over 8b? not and if
    drop $90 _1bmerge, else $8600 _swappable then ;

: op ( idx ) does> ( dst src idx )
  over notimm# 11 lshift $1b600 or _modrm, ;
0 op movzx,        1 op movsx,

: ismem# ( arg -- ) ismem? _assert ;
: pop, ( arg -- )
  dup notimm# dup indirect? if
    dup ismem# $8f00 or opmod, else $58 _1bmerge, then ;

: isbyte? ( n -- f ) $100 < ;
: push, ( arg -- )
  dup checkimm if
    drop _imm dup isbyte? $68 over 2* + op, ?8b, asm$
    else dup indirect? if
      dup ismem# $ff00 or opmod, else $50 _1bmerge, then then ;

: op does> ( ax arg idx )
  dip over has66hprefix? if $66 c, then over AX? _assert | over checkimm if
    nip $e4 + swap 256/ or op,
    _imm dup $100 >= ?abort"bad imm range for in/out" c, asm$
    else swap $1c2 ( DX ) = _assert swap 8b? - $ed + op, asm$ then ;
0 op in,           2 op out,

: op ( idx ) does> 8* ( arg narg opmod )
  rot dup check66h or over checkimm if
    0 to _imm? nip _imm 1 = if
      $d000 or opmod, else $c000 or opmod, _imm c, then
    else swap $c1 = _assert ( cl ) $d200 or opmod, then ;
map< op 0 rol, 1 ror, 2 rcl, 3 rcr, 4 sal, 7 sar, 4 shl, 5 shr,

create _tbl map< , $8c $120 $121 $124
: _special, ( reg special dir -- )
  dip dip _remsz | bi modrm@ | 4/ $c and _tbl + @ | ( reg spec opcode dir )
  or 256* dipswap _modrm, ;
: _shortimm ( opmod -- )
  dup check66h bi 8b? | regid@
  over 8* $b8 swap- or op, _imm swap ?8b, asm$ ;
: mov, ( dst src -- )
  dup special? if 0 _special, exit then
  over special? if swap 2 _special, exit then
  dup checkimm if
    drop dup indirect? if $c600 or opmod, else _shortimm then
    else $8800 _regular, then ;

: lea, swap $8d00 _regular, ;
: int, ( n -- ) $cd c, c, ;

\ Jumps and relative addresses
\ i386 jumps and calls in their immediate modes are relative. We keep it that
\ way. However, those relative addresses are inconvenient to use because it's
\ relative to the *end* of the op, which can be 2, 3 or 5 bytes in size. This
\ makes it very inconvenient to use reliably because the caller has to be aware
\ of the size of its relative offset. To that end, we auto adjust that relative
\ address to the size of the op. Therefore, "0 jmp," is an infinite loop encoded
\ as EB FE.
: jrel8? ( rel -- f ) $80 + 2- isbyte? ;
: jrel8, 2- c, ;
: jrel32, ( rel32-or-16 ) realmode? if 3 - wle, else 5 - le, then ;

\ Conditional jumps
: op ( opcode -- ) does> ( rel opcode -- )
  over jrel8? if $70 or op, jrel8, else $180 or op, 1- jrel32, then ;
map< op $0 jo,   $1 jno, \
        $2 jb,   $3 jnb,  $6 jbe,  $3 jnbe, \
        $2 jc,   $3 jnc, \
        $4 je,   $5 jne,  $4 jz,   $5 jnz,  \
        $7 ja,   $6 jna,  $3 jae,  $2 jnae, \
        $8 js,   $9 jns, \
        $a jp,   $b jnp,  $a jpe,  $b jpo,  \
        $c jl,   $d jnl,  $e jle,  $f jnle, \
        $f jg,   $e jng,  $d jge,  $c jnge,

: op ( opcode -- ) does> ( rel opcode -- ) over jrel8? _assert op, jrel8, ;
$e2 op loop,     $e1 op loopz,   $e0 op loopnz,

: jmpr, $ff20 or opmod, asm$ ;
: jmp, dup jrel8? if $eb op, jrel8, else $e9 op, jrel32, then asm$ ;
: callr, $ff10 or opmod, asm$ ;
: call, $e8 op, jrel32, asm$ ;

: jmpfar, ( seg16 absaddr ) $ea op, _addr, wle, asm$ ;
: callfar, ( seg16 absaddr ) $9a op, _addr, wle, asm$ ;

: _jmpop@ ( a -- a+n is8? )
  c@+ dup $0f = if \ 16 bit jcc
    drop 1+ 0 else ( a op )
    dup $eb = swap $f0 and $70 = or then ( a is8? ) ;

: forward! ( jmpaddr -- )
  _jmpop@ ( a is8? ) swap here over - ( is8? a rel )
  rot if 1- swap c! else realmode? if
    2- swap wle! else 4- swap le! then then ;

\ Disabled in realmode because it's broken for now
: ?movzx, ( dst src -- ) notreal# case
    imm? of r@ mov, endof
    8b? of r@ movzx, endof
    has66hprefix? of r@ dword) movzx, endof
    mov, endcase ;

: ?d+bp) d) 64bmode? if bp r+) then ;
: ?bp+, ( opmod -- ) 64bmode? if rex.w bp add, else drop then ;
: ?bp-, ( opmod -- ) 64bmode? if rex.w bp sub, else drop then ;
