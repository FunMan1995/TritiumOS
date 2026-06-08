: HAL8B $100 ;
: HAL16B $200 ;
: HAL816B $300 ;
: HALDIRECT $400 ;
: HALIMM $800 ;
: HALINV $1000 ;
: HALSIGNED $2000 ;
: A0 $8 ; : D0 $0 ;
: src@ b3:0 ;
: src! swap $f invand or ;
: dst@ $4 rshift b3:0 ;
: dst! $4 lshift swap $f0 invand or ;
: src<>dst dup src@ over dst@ rot swap src! swap dst! ;
: hslot< $10 lshift ;
: (slot $10 rshift $f and ;
: clrbank $f0000 invand ;
: slot) hslot< swap clrbank or ;
: <<3 $3 lshift ;
: <<6 $6 lshift ;
: <<9 $9 lshift ;
: <<12 $c lshift ;

: 32b) HALDIRECT invand HAL816B invand ;
: 8b) 32b) HAL8B or ; : 16b) 32b) HAL16B or ;
: &) 32b) HALDIRECT or ;
: W) $77 ; : A) $76 ; : S) $75 ;
: PSP) $7e ; : RSP) $7f ;
: sys) $100 + hbank! hslot< $7d or ;
: m) hbank! hslot< $870 or ;
: i) m) &) ;
: W>) $7 dst! ; : A>) $6 dst! ; : S>) $5 dst! ;
: =) $07 ; : <>) $06 ;
: >) $2 ; : <=) $3 ; : <) $5 ; : >=) $4 ;
: s>) $e ; : s<=) $f ; : s<) $d ; : s>=) $c ;
: invcond $1 xor ;
: r@, over $7 and <<9 or swap $8 and <<3 or $2000 or w, ;
: A? $3 rshift $1 and ;
: b2:0 $7 and ; : b5:3 $38 and ;
: popexit, exit, ;

: bbr, $6000 bri, ;
: bbrc, $8 lshift $6000 or bri, ;
: fbrc, here dup $200 + rot bbrc, ;
: ?brz, =) bbrc, ;
: ?brnz, <>) bbrc, ;
: ifz, <>) fbrc, ;
: ifnz, =) fbrc, ;

: dropf, $4cde0080 , ;
: dropz, $4a87 w, dropf, ;
: ifW, dropz, ifnz, ;

: br! over $2 + - swap $2 + w! ;
: if ifW, ; immediate
: then here br! ; immediate
: fbr, here dup $200 + bbr, ;
: else fbr, swap here br! ; immediate

: br!
  over $2 + - over $1 + c@ dup if
    $ff - if
      swap $1 + c! else swap $2 + ! then
    else drop swap $2 + w! then
  clearicache ;
