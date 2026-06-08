needs asm/label
unit asm/6502

\ output: n n-is-2b opoff
: # ( n ) 0 $09 ; \ Immediate
: <0+> ( n ) 0 $05 ; \ ZeroPage
: <X+> ( n ) 0 $15 ; \ ZeroPage+X
: <Y+> ( n ) 0 $15 ; \ Only for LDX
: () ( n ) 1 $0d ; \ Absolute
: (X+) ( n ) 1 $1d ; \ Absolute+X
: (Y+) ( n ) 1 $19 ; \ Absolute+Y
: [X+] ( n ) 0 $01 ; \ Indirect+X
: []Y+ ( n ) 0 $11 ; \ Indirect+Y
: ?, ( n n-is-2b -- ) if wle, else c, then ;

: OPG1 does> or c, ?, ;
$60 OPG1 ADC,  $20 OPG1 AND,  $c0 OPG1 CMP,  $40 OPG1 EOR,
$a0 OPG1 LDA,  $00 OPG1 ORA,  $e0 OPG1 SBC,  $80 OPG1 STA,

: _09repl dup $09 = if drop 1 then ;
: OPG2 does> swap _09repl or 1+ c, ?, ;
$00 OPG2 ASL,  $c0 OPG2 DEC,  $e0 OPG2 INC,  $a0 OPG2 LDX,
$40 OPG2 LSR,  $20 OPG2 ROL,  $60 OPG2 ROR,  $80 OPG2 STX,

: OPG3 does> swap _09repl or 1- c, ?, ;
$20 OPG3 BIT,  $e0 OPG3 CPX,  $c0 OPG3 CPY,  $a0 OPG3 LDY,
$80 OPG3 STY,

: OP does> c, ;
$0a OP ASLA, $00 OP BRK,  $18 OP CLC,  $d8 OP CLD,  $58 OP CLI,
$b8 OP CLV,  $ca OP DEX,  $88 OP DEY,  $e8 OP INX,  $c8 OP INY,
$4a OP LSRA, $ea OP NOP,  $48 OP PHA,  $08 OP PHP,  $68 OP PLA,
$28 OP PLP,  $2a OP ROLA, $6a OP RORA, $40 OP RTI,  $60 OP RTS,
$38 OP SEC,  $f8 OP SED,  $78 OP SEI,  $aa OP TAX,  $a8 OP TAY,
$98 OP TYA,  $ba OP TSX,  $8a OP TXA,  $9a OP TXS,

: _bchk dup $80 + $ff > ?abort"br ovfl" ;
: OPBR does> c, 2 - _bchk c, ;
$90 OPBR BCC, $b0 OPBR BCS, $f0 OPBR BEQ, $30 OPBR BMI,
$d0 OPBR BNE, $10 OPBR BPL, $50 OPBR BVC, $70 OPBR BVS,

: OPBR2 does> c, wle, ;
$20 OPBR2 JSR, $4c OPBR2 JMP, $6c OPBR2 JMP[],

alias JMP, jmp, alias JMP[], @jmp, alias JSR, call,
: jr! ( off a -- )
  dup c@ $b8 = if ( CLV ) 1+ swap 1- swap then
  swap 2 - _bchk swap 1+ c! ;
: jr, CLV, BVC, ; \ no BRA!
alias BEQ, jrz, alias BNE, jrnz,
alias BCS, jrc, alias BCC, jrnc,
: i>, DEX, DEX, dup # LDA, 0 <X+> STA, 8 rshift # LDA, 1 <X+> STA, ;
: i@>,
  DEX, DEX, dup () LDA, 0 <X+> STA, 1+ () LDA, 1 <X+> STA, ;

\ ZP assignments
$06 value 'A   $08 value 'N
0 value IPL    2 value INDJ
: IPH IPL 1+ ; : INDL INDJ 1+ ; : INDH INDL 1+ ;
