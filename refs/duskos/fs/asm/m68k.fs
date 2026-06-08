needs lib/psrs lib/wordtbl asm/label
unit asm/m68k

enum D0 D1 D2 D3 D4 D5 D6 D7 A0 A1 A2 A3 A4 A5 A6 A7
A7 const RSP
A6 const PSP
D7 const W
D6 const A
D5 const S

$03000000 const SZMASK

: err abort"asm error" ;
: 8bit? ( n -- f ) $80 + $100 < ;
: 16bit? ( n -- f ) $8000 + $10000 < ;
: lo3 7 and ;
: <<9 9 lshift ;
: long) SZMASK invand ;
: byte) long) $02000000 or ;
: word) long) $01000000 or ;
: A? ( eas -- f ) 8 and bool ;
: Alo# ( reg -- n ) dup A? not ?abort"An register expected" lo3 ;
: Dlo# ( reg -- n ) dup A? ?abort"Dn register expected" lo3 ;

: [An]) ( reg -- eas ) Alo# $10 or ;
: [An]+) ( reg -- eas ) Alo# $18 or ;
: -[An]) ( reg -- eas ) Alo# $20 or ;
: bd! ( n eas -- eas ) swap hbank! 16 lshift or ;
: [An,d]) ( reg n -- ) $28 bd! swap Alo# or ;
: [PC,d]) ( n -- ) 2- $3a bd! ;
: abs) ( n -- eas ) dup $38 bd! swap 16bit? not or ;
: imm) ( n -- eas ) $3c bd! ;

: ea@ ( eas -- n ) $3f and ;
: mode@ ( eas -- n ) 8/ lo3 ;
: reg@ ( eas -- n ) lo3 ;

: Xn]) ( eas reg -- eas )
  12 lshift or dup mode@ 7 = if 1 or else $30 or 8 invand then ;

: sz@ ( eas -- 0-1-or-2 ) 24 rshift 3 and ;
:~ rshift $f and dup if hbank@ 1 then ;
: ?bd@ ( eas -- ?n f ) 16 ~ ;
: bd@# ?bd@ not ?abort"displacement expected" ;
: ?od@ ( eas -- ?n f ) 20 ~ ;
: samesz ( eas eas -- eas eas ) 2dup or SZMASK and tuck or rot> or swap ;
\ Not all instructions have the same meaning for their "size" field!
\ "map" is a number with b12:8 mapping to byte), b7:4 mapping to word) and
\ b3:0 mapping to long).
: mapsz ( eas map -- n ) swap sz@ 4* rshift 3 and ;

: inherent does> wbe, ; ( n -- )
map< inherent
  $4e71 nop, $4e75 rts,

: rel, ( rel opcode -- )
  dip 2- | over case
    8bit? of swap $ff and or wbe, endof
    16bit? of wbe, wbe, endof
    drop $ff or wbe, be, endcase ;

: br does> rel, ; ( rel n -- )
map< br
  $6000 bra, $6100 bsr, \
  $6200 bhi, $6300 bls, $6400 bcc, $6500 bcs, \
  $6600 bne, $6700 beq, \
  $6800 bvc, $6900 bvs, $6a00 bpl, $6b00 bmi, \
  $6c00 bge, $6d00 blt, $6e00 bgt, $6f00 ble,

: dbcc does> rot Dlo# or wbe, 2- wbe, ; ( reg rel n -- )
map< dbcc
  $51c8 dbra, \
  $52c8 dbhi, $53c8 dbls, $54c8 dbcc, $55c8 dbcs, \
  $56c8 dbne, $57c8 dbeq, \
  $58c8 dbvc, $59c8 dbvs, $5ac8 dbpl, $5bc8 dbmi, \
  $5cc8 dbge, $5dc8 dblt, $5ec8 dbgt, $5fc8 dble,

: indexed, ( eas -- )
  $ff000 and $800 or
  dup ?bd@ if dup 8bit? not ?abort"TODO 16-bit displacement" or then
  wbe, ;
: mode7, ( eas -- )
  dupbi bd@# | reg@ case
    bi 0 = | 2 = or of nip wbe, endof
    1 = of nip be, endof
    3 = of drop $f0000 invand indexed, endof
    4 = of swap sz@ if wbe, else be, then endof
    abort"invalid mode/reg" endcase ;
: eaext, ( eas -- )
  dup mode@ case
    5 = of bd@# wbe, endof
    6 = of indexed, endof
    7 = of mode7, endof
    2drop endcase ;
: reg<>mode@ ( eas -- n ) bi mode@ | reg@ 8* or ;

: ea>b5:0 ( eas op -- eas op ) over ea@ or ;
: oneea does> ea>b5:0 wbe, eaext, ; ( ea n -- )
map< oneea
  $4840 pea, $4ec0 jmp, \
  $52c0 shi, $53c0 sls, $54c0 scc, $55c0 scs, \
  $56c0 sne, $57c0 seq, \
  $5820 svc, $5920 svs, $5a20 spl, $5b20 smi, \
  $5c20 sge, $5d20 slt, $5e20 sgt, $5f20 sle,

: sz>b7:6 ( eas op -- eas op ) over $012 mapsz 6 lshift or ;
: oneeasz does> ea>b5:0 sz>b7:6 wbe, eaext, ; ( ea n -- )
map< oneeasz
  $4200 clr, $4400 neg, $4600 not, $4a00 tst, \
  $e7c0 rol, $e6c0 ror, $e1c0 asl, $e0c0 asr, $e3c0 lsl, $e2c0 rsl,

 ( dst reg-or-n n -- )
: rot# does> swap lo3 <<9 or sz>b7:6 swap Dlo# or wbe, ;
map< rot# $e118 rol#, $e018 ror#, $e138 rol#r, $e038 ror#r, \
          $e100 asl#, $e000 asr#, $e120 asl#r, $e020 asr#r, \
          $e108 lsl#, $e008 lsr#, $e128 lsl#r, $e028 lsr#r,

: ari, ( reg eas opcode -- )
  oover SZMASK and rot or swap \ propagate SZ if in "reg"
  ea>b5:0 sz>b7:6 ( reg eas n )
  rot Dlo# lo3 <<9 or ( eas n )
  wbe, eaext, ;
: ari does> ( dst src n -- ) oover mode@ if dipswap $100 or then ari, ;
map< ari $8000 or, $9000 sub, $c000 and, $d000 add,
: cmp, swap Dlo# swap $b000 ari, ;
: eor, Dlo# swap $b100 ari, ;

: aria, ( reg eas opcode -- )
  ea>b5:0 ( reg eas n )
  rot Alo# lo3 <<9 or ( eas n )
  wbe, eaext, ;
: aria does> ( dst src n -- ) aria, ;
map< aria $91c0 suba, $b1c0 cmpa, $d1c0 adda,

: exg, ( reg reg -- )
  over A? if
    dup A? if swap lo3 <<9 $40 or or else <<9 $80 or or then
    else $40 over A? if 2* then rot <<9 or or then ( instr )
  $c100 or wbe, ;

: lea, ( dst src -- )
  $41c0 ea>b5:0 rot Alo# ( src instr reg )
  <<9 or wbe, eaext, ;

: move, ( dst src -- )
  samesz dup $132 mapsz 12 lshift ( dst src instr )
  oover reg<>mode@ 6 lshift or ( dst src instr )
  ea>b5:0 wbe, eaext, eaext, ;

: moveq, ( dst n -- ) $7000 or swap <<9 or wbe, ;

4 wordrefs tbl wbe, wbe, be, err
: arii does> ( dst n n -- )
  dipswap sz>b7:6 ea>b5:0 wbe, ( n dst )
  tuck $012 mapsz tbl swap wexec eaext, ;
map< arii $0000 ori, $0200 andi, $0400 subi, $0600 addi, $0a00 eori, $0c00 cmpi,

consts $1000 SFC $1001 DFC $1800 USP $1801 VBR \
       $1002 CACR $1802 CAAR $1803 MSP $1804 ISP \
       $1003 TC $1004 ITT0 $1005 ITT1 $1006 DTT0 $1007 DTT1 \
       $1805 MMUSR $1806 URP $1807 SRP \
       $1004 IACR0 $1005 IACR1 $1006 DACR0 $1007 DACR1

: movec, ( dst src -- )
  over $1000 and if
    $4e7b wbe, 12 lshift swap $fff and or wbe,
    else $4e7a wbe, $fff and swap 12 lshift or wbe, then ;

: forward! ( jmpaddr -- )
  here over - 2- swap dup 1+ c@ case
    0 = of 2+ wbe! endof
    $ff = of 2+ be! endof
    drop 1+ c! endcase ;
