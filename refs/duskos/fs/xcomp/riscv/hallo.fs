: HALBASE $240 ;
: HALOFF $1 ;
: HALMEM $2 ;
: HAL8B $4 ;
: HALINV $8 ;
: HALDIRECT $10 ;
: HALIMM $20 ;
: HAL16B $40 ;
: HALSIGNED $2000000 ;
: HALRDMASK $f80 ;
: HALRS1MASK $f8000 ;
: HALRS2MASK $1f00000 ;
: HALBMASK $f8000000 ;
: HALMASK $200007f ;
: W) $20240 ;
: A) $28240 ;
: S) $30240 ;
: PSP) $18240 ;
: RSP) $10240 ;
: =) $0 ; : <>) $1 ;
: <) $6 ; : >=) $7 ;
: >) $e ; : <=) $f ;
: invcond $1 xor ;
: (slot $1c rshift ;
: slot) $1c lshift swap $f0000000 invand or ;

: rd@ $7 rshift $1f and ;
: rd! $7 lshift swap HALRDMASK invand or ;
: rs1@ $f rshift $1f and ;
: rs2@ $14 rshift $1f and ;
: rs1! $f lshift swap HALRS1MASK invand or ;
: rs2! $14 lshift swap HALRS2MASK invand or ;
: rdrs1! tuck rd! swap rs1! ;
: rs1<>rs2 dup rs1@ over rs2@ rot swap rs1! swap rs2! ;
: rd<>rs1 dup rs1@ over rd@ rot swap rs1! swap rd! ;
: rd<>rs2 dup rs2@ over rd@ rot swap rs2! swap rd! ;
: funct3! $c lshift or ;
: Bimm! tuck $1e and $7 lshift or
        over $7e0 and $14 lshift or
        over $800 and $4 rshift or
        swap $1000 and $13 lshift or ;
: Simm! tuck $1f and $7 lshift or
        swap $fe0 and $14 lshift or ;

: W>) $4 rd! ;
: A>) $5 rd! ;
: S>) $6 rd! ;

: bbr, here - $0a swap relcall, ;
: _ $00020593 , $0001a203 , $00418193 , ;
: dropz, _ $000585b3 , ;
: ifW, _ here $00059663 , $00000517 , $ffc50567 , ;

code ?>12b $40b25213 , $00120213 , $ffe27213 , exit,

: hibits dup $0c rshift swap $0b rshift $1 and + ;
: lobits $fff and ;
: Uimm $0c lshift swap $fff and or ;
: Iimm $14 lshift swap $fffff and or ;

: br1!
  over - over @ over hibits Uimm rot tuck !
  $4 + tuck @ swap lobits Iimm swap ! ;

: if ifW, ; immediate
: then $4 + here br1! ; immediate
: br! over @ $7f and $17 - if swap $4 + swap then
      over @ $7f and $17 - if halerr then br1! ;
