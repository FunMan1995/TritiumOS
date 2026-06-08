needs num/math asm/label
unit asm/arm

\ RSP=rSP (r13)
\ PSP=r10
\ PS Top of Stack=r9

: ,) ( op -- ) le, ;

: cond does> 28 lshift swap $0fffffff and or ;
0 cond eq)         1 cond ne)          2 cond cs)          3 cond cc)
0 cond z)          1 cond nz)          2 cond hs)          3 cond lo)
4 cond mi)         5 cond pl)          6 cond vs)          7 cond vc)
8 cond hi)         9 cond ls)          10 cond ge)         11 cond lt)
12 cond gt)        13 cond le)         14 cond al)

: op does> 20 lshift al) ;
$00 op and)        $02 op eor)         $04 op sub)         $06 op rsb)
$08 op add)        $0a op adc)         $0c op sbc)         $0e op rsc)
$11 op tst)        $13 op teq)         $15 op cmp)         $17 op cmn)
$18 op orr)        $1a op mov)         $1c op bic)         $1e op mvn)

: ari# ( op -- op )
  dup $0c000000 and if
    abort"modifier can only go in arithmetic op" then ;

: f) ari# $00100000 or ;

: immsplit ( n -- immop rest )
  r! dup if log2 then 7 - max0 dup 1 and if 1+ then ( shift ) \ V1=n
  r@ over rshift ( shift b )
  2dup swap lshift r> swap- ( shift b rest )
  rot neg $1e and 7 lshift rot or ( rest imm ) swap ;

: imm) ( op n -- op )
  immsplit if abort"invalid immediate value" then or $02000000 or ari# ;

\ Single Data Transfer (LDR, STR, SWP)
$05000000 al) const str)     str) $00100000 or const ldr)
$01000090 al) const swp)

: ldrstr# ( op -- op )
  dup $0c000000 and $04000000 <> if
    abort"modifier can only go in regular ldr) or str)" then ;

: pre) ldrstr# $01000000 or ;
: post) ldrstr# $feffffff and ;
: 12b# ( n -- ) $fffff000 and if abort"invalid offset" then ;
: +i) ( op n -- op ) dup 12b# or $00800000 or ldrstr# ;
: -i) ( op n -- op ) dup 12b# or $ff7fffff and ldrstr# ;
: +r) ( op r -- op ) +i) $02000000 or ;
: -r) ( op r -- op ) -i) $02000000 or ;
: byte) ldrstr# $00400000 or ;
: !) ldrstr# $00200000 or ;

\ Multiply
$90 al) const mul)
: ismul? ( op -- f ) $0fcf00f0 and $90 = ;
: acc) ( op r -- op ) 12 lshift or $00200000 or ;

\ MRC/MCR
$0e000010 al) const mcr)
$0e100010 al) const mrc)
$0c400000 al) const mcrr)
: ismcrr? ( op -- f ) $0ff00000 and $0c400000 = ;
: cpop) ( op r -- op ) over ismcrr? if 4 else 21 then lshift or ;
: cp#) ( op r -- op ) 8 lshift or ;
: cpi) ( op r -- op ) 5 lshift or ;

\ MSR/MRS
$010f0000 al) const mrs)
$0129f000 al) const msr)
: cpsr) ;
: spsr) $00400000 or ;

\ Parameters
0 const r0         1 const r1          2 const r2          3 const r3
4 const r4         5 const r5          6 const r6          7 const r7
8 const r8         9 const r9          10 const r10        11 const r11
12 const r12       13 const r13        14 const r14        15 const r15
8 const rS         9 const rW          10 const rPSP       11 const rA
13 const rSP       14 const rLR        15 const rPC
: rn) ( op r -- op ) 16 lshift or ;
: rd) ( op r -- op ) over ismul? 4* 12 + lshift or ;
: rs) ( op r -- op ) 8 lshift or ;
: rm) ( op r -- op ) or ;
: rdn) tuck rd) swap rn) ;

\ immediate shift operations ( op n -- op )
: _ ( op n type -- op ) dip 3 lshift | 2* or 4 lshift or ;
: lsl) 0 _ ; : lsr) 1 _ ; : asr) 2 _ ; : ror) 3 _ ;
\ register shift operations ( op r -- op )
: _ ( op r type -- op ) dip 4 lshift | 2* 1+ or 4 lshift or ;
: rlsl) 0 _ ; : rlsr) 1 _ ; : rasr) 2 _ ; : rror) 3 _ ;

\ Branching
: _rel>off 4/ 2- $ffffff and ;
: b) ( rel32 -- op ) _rel>off $ea000000 or ;
: bl) b) $01000000 or ;
: bx) ( r -- op ) $e12fff10 or ;

: forward! ( jmpaddr -- )
  here over - _rel>off over le@ $ff000000 and or swap le! ;

\ Macros

: push, ( r -- ) str) swap rd) rSP rn) 4 -i) pre) !) ,) ;
: pop, ( r -- ) ldr) swap rd) rSP rn) 4 +i) post) ,) ;
: ppush, ( r -- ) str) swap rd) rPSP rn) 4 -i) pre) !) ,) ;
: ppop, ( r -- ) ldr) swap rd) rPSP rn) 4 +i) post) ,) ;

: ret, rLR bx) ,) ;
: reti, sub) rPC rd) rLR rn) 4 imm) f) ,) ;
: absb, abs>rel b) ,) ;
: absbl, abs>rel bl) ,) ;
: return) ( -- op ) mov) rPC rd) rLR rm) ;

:~ ( mask -- ) mrs) r0 rd) ,) orr) r0 rdn) swap imm) ,) msr) r0 rm) ,) ;
: fiqdis, $40 ~ ; : irqdis, $80 ~ ;
:~ ( mask -- ) mrs) r0 rd) ,) bic) r0 rdn) swap imm) ,) msr) r0 rm) ,) ;
: fiqena, $40 ~ ; : irqena, $80 ~ ;
