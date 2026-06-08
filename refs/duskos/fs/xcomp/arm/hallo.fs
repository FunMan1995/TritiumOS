: HALBASE $e4009000 ;
: HALMASK $0f800000 ;
: HALBMASK $0000003f ;
: HALDIRECT $01000000 ;
: HALIMM $02000000 ;
: HALINV $00800000 ;
: HALMEM $00000020 ;
: HAL16B $04000000 ;
: HAL8B $00400000 ;
: HALSIGNED $08000000 ;
: i) hbank! HALBASE or HALIMM or ;
: Rn< $10 lshift ;
: Rd< $c lshift ;
: Rn! Rn< swap $f0000 invand or ;
: Rd! Rd< swap $f000 invand or ;
: Rn@ $10 rshift $f and ;
: Rd@ $c rshift $f and ;
: REGW $9 ; : REGA $b ; : REGS $8 ;
: REGSYS $7 ; : REGPSP $a ; : REGRSP $d ;
: _ HALBASE swap Rn< or ;
: W) REGW _ ; : A) REGA _ ; : S) REGS _ ; : PSP) REGPSP _ ; : RSP) REGRSP _ ;
: _ $1c lshift ;
: =) $0 ; : <>) $1 _ ;
: >=) $2 _ ; : <) $3 _ ; : >) $8 _ ; : <=) $9 _ ;
: invcond $1 _ xor ;
: m) hbank! HALBASE HALMEM or or ;
: sys) hbank! HALBASE or REGSYS Rn! ;
: W>) REGW Rd! ; : A>) REGA Rd! ; : S>) REGS Rd! ;

: (slot $f and ;
: slot) swap $f invand or ;

: br! over - $2 rshift $2 - $ffffff and
      over @ $ff000000 and or swap ! clearicache ;

: dropz, $e2999000 , $e49a9004 , ;

: bbl, swap here - $2 rshift $2 - $ffffff and or , clearicache ;
: bbr, $ea000000 bbl, ;
: bl, $eb000000 bbl, ;
: bbrc, $0a000000 or bbl, ;

: ?brz, =) bbrc, ;
: ?brnz, <>) bbrc, ;
: ifz, here $0 ?brnz, ;
: ifnz, here $0 ?brz, ;
: ifW, dropz, ifnz, ;
