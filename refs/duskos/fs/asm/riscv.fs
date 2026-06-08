\ RISCV 32-bit assembler (specifically RV32GC/RV64GC)
needs num/math asm/label
unit asm/riscv

\ ________________________________________________________
\ Registers:
\ x0 => hard-wired to 0
\ x31:x1 => general-purpose registers
\ pc => program counter
\ ________________________________________________________


\ Parameters | Register naming
0 const x0         1 const x1          2 const x2          3 const x3
4 const x4         5 const x5          6 const x6          7 const x7
8 const x8         9 const x9          10 const x10        11 const x11
12 const x12       13 const x13        14 const x14        15 const x15
16 const x16       17 const x17        18 const x18        19 const x19
20 const x20       21 const x21        22 const x22        23 const x23
24 const x24       25 const x25        26 const x26        27 const x27
28 const x28       29 const x29        30 const x30        31 const x31
\ Usual calling conventions
0 const xZERO      1 const xRA          2 const xSP         3 const xGP

\ DuskOS register maping
\ Allocated registers: x2-x7 (see below)
\ Scratch registers: x10-20
\ Flags registers : x28-29 (see below)
\ Forbidden use of: x30 | used by the "tail" instruction
x2 const xRSP       x3 const xPSP        x4 const xW
x5 const xA         x6 const xS          x7 const xSYSVARS

x28 const xAcmp     x29 const xBcmp

\ Instr type encoding and immediates
\ I = 0 ; S = 1 ; SB = 2 ; U = 3 ; UJ = 4 ; RI = 5 ; R = 6
0 value instr-type

\ I instr format: imm[11:0] placed at instr[31:20]
\ S instr format: imm[11;5] @ instr[31:25] and imm[4:0] @ instr [11:7]
\ SB instr format: imm[12] @ instr[31] & imm[10:5] @ instr[30:25] &
\                  imm[4:1] @ instr[11:8] & imm[11] @ instr[7]
\ U instr format:  imm[31:12] @ instr[31:12]
\ UJ instr format: imm[20] @ instr[31] & imm[10:1] @ instr[30:21]
\                  imm[11] @ instr[20] & imm[19:12] @ instr[19:12]
\ RI instr format: special format for SRAI instr where rs2 is an immediate
\ UJR instr format: special format for jump using register contained value
\ Similar to Iimm) (so using RS1 too)
: _Iinstr 0 to instr-type ;          : _Sinstr 1 to instr-type ;
: _SBinstr 2 to instr-type ;         : _Uinstr 3 to instr-type ;
: _UJinstr 4 to instr-type ;         : _UJRinstr 5 to instr-type ;
: _RIinstr 6 to instr-type ;         : _Rinstr 7 to instr-type ;

\ Macros and options
0 value _reverse?
0 value _twoinstr? \ set to 1 when we are assembling two instructions in the same time
0 value _samedst? \ apply same rd to the top instr, and also apply rd as rs1 to first top instr
                  \ if _32offset? is set
0 value _32offset? \ apply twice imm) on split offset (31:12 and 11:0)

0 value _neg? \ negate immediate if set

0 value _nochecking? \ doing forwarding manipulation
0 value _signed?  \ If set, signed checking needed, if not unsigned
: _signed 1 to _signed? ;               : _unsigned 0 to _signed? ;

: _$rst 0 to _twoinstr? 0 to _samedst? 0 to _32offset?
    0 to _reverse? 0 to _neg? 0 to _signed? 0 to _nochecking? ;

: ,) ( op -- ) le, _twoinstr? if le, then _$rst ;


\ Assembly macros
: _hi ( n -- n:31-12>>12 ) dup $fffff000 and 12 rshift swap $00000800 and if 1 + then ;
: _lo ( n -- n:11-0 ) $fff and ;

\ Register and functions encoding
: _checkrs1 instr-type dup 3 = swap 4 = or
    if abort" Instruction format error, this type isn't using rs1" then ;

: _checkrs2 instr-type dup dup 1 = swap 2 = or swap 7 = or not
    if abort" Instruction format error, this type isn't using rs2" then ;

: _rs1) 15 lshift swap $fff07fff and or ;
: _rs2) 20 lshift swap $fe0fffff and or ;

: rs1) _reverse? if _checkrs2 _rs2) else _checkrs1 _rs1) then ;
: rs2) _reverse? if _checkrs1 _rs1) else _checkrs2 _rs2) then ;

: _rd) 7 lshift swap $fffff07f and or ;

: rd) _samedst? if dup rot> _rd) rot>
	dup rot> _rd) swap _32offset? if _rs1) else drop then swap else _rd) then ;

: rdrs1) tuck rd) swap rs1) ;

\ macros for load/store data processing instr
: base) rs1) ;
: src) rs2) ;
: dst) rd) ;

: funct3) 12 lshift swap $ffff8fff and or ;
: funct7) 25 lshift swap $0effffff and or ;

\ Immediate checking system
0 const unsigned             1 const signed

: bitsz ( bitsize -- sz ) 1 swap lshift ;

: _smaxsz ( bitsize -- max ) 1 swap 1- lshift 1- ;
: _sminsz ( bitsize -- min ) 1 swap 1- lshift 0 swap - ;

\ if returning 1 need to abort
: _ucheckimmsz ( imm requested-size -- imm f ) bitsz over <= ;

: abs dup 0< if neg then ;
\ if returning 1 need to abort
: _scheckimmsz ( imm requested-size -- imm f ) bitsz 2/ over abs <= ;

: _checkimmsz ( imm requested-size -- imm )
    _twoinstr? _nochecking? or not
    if dup 32 =
	if drop else
	    _signed? if _scheckimmsz else _ucheckimmsz then
	    if .x abort" wrong immediate size" then then
    else drop then ;

: Iimm) 12 _checkimmsz
    $fff and 20 lshift swap $000fffff and or ;

: Simm) 12 _checkimmsz
    dup $fe0 and 20 lshift swap $01f and 7 lshift or
    swap $01fff07f and or ;

: SBimm) 13 _checkimmsz ( check that this is a multiplied by 2 offset ? )
    1 rshift dup $800 and 20 lshift swap dup $3f0 and 21 lshift rot or swap
    dup $00f and 8 lshift swap $400 and 3 rshift or or
    swap $01fff07f and or ;

\ the assembly syntax does not represent the lower 12 bits of the U-
\ immediate, which are always zero
: Uimm) 32 _checkimmsz ( check that this is a 31:12 offset??? )
    ( 12 lshift ) $fffff and 12 lshift swap $00000fff and or ;

: RIimm) 5 _checkimmsz \ should it be unsigned ou signed, not sure
    $fff and 20 lshift swap $fe0fffff and or ;

: UJimm) 21 _checkimmsz ( check that this is a multiplied by 2 offset ? )
    1 rshift dup $80000 and 12 lshift swap dup $7f800 and 1 lshift rot or swap
    dup $00400 and 10 lshift swap $3ff and 21 lshift or or
    swap $00000fff and or ; \ [TBA]

: UJRimm) Iimm) ;

: _imm) instr-type case
	0 = of Iimm) endof
	1 = of Simm) endof
	2 = of SBimm) endof
	3 = of Uimm) endof
	4 = of UJimm) endof
	5 = of UJRimm) endof
	6 = of RIimm) endof
	7 > of abort" trying to use immediate on wrong instruction" endof endcase ;

\ the 3 pseudo-instructions generating 2 base instructions use addi or jalr as first instr
\ which is of type Iimm)
: imm) _32offset? if dup rot> _hi _imm) rot> _lo Iimm) swap
    else _neg? if neg then _imm) then ;

\ Opcodes and instructions formatting (op-32 and op-imm32 are for the RV-64I ISA)
: load) _Iinstr $03 ;              : store) _Sinstr $23 ;
: op-imm-32) _Iinstr $1b ;         : op-32) _Rinstr $3b ;
: op-imm) _Iinstr $13 ;            : op) _Rinstr $33 ;
: _jal) _UJinstr $6f ;             : _jalr) _UJRinstr $67 ;
: branch) _SBinstr $63 ;

: _auipc) _Uinstr $17 ;            : _lui) _Uinstr $37 ;

\ Not currently used
: load-fp) $07 ;    : misc-mem) $0f ; : store-fp) $27 ;   : amo) $2f ;
: madd) $43 ;       : msub) $47 ;     : nmadd) $4f ;      : op-fp) $53 ;
: system) $73 ;     : nmsub) $4a ;

\ Load Instructions
: lb) load) 0 funct3) _signed ;            : lh) load) 1 funct3) _signed ;
: lw) load) 2 funct3) _signed ;            : lbu) load) 4 funct3) _signed ;
: lhu) load) 5 funct3) _signed ;

\ Stores Instructions
: sb) store) 0 funct3) _signed ;           : sh) store) 1 funct3) _signed ;
: sw) store) 2 funct3) _signed ;

\ Shifts Instructions
: sll) op) 1 funct3) $00 funct7) ;           : slli) op-imm) 1 funct3) $00 funct7) _RIinstr _unsigned ;
: srl) op) 5 funct3) $00 funct7) ;           : srli) op-imm) 5 funct3) $00 funct7) _RIinstr _unsigned ;
: sra) op) 5 funct3) $20 funct7) ;           : srai) op-imm) 5 funct3) $20 funct7) _RIinstr _unsigned ;

\ Arithmetic Instructions
: add) op) 0 funct3) $00 funct7) ;           : addi) op-imm) 0 funct3) _signed  ;
: sub) op) 0 funct3) $20 funct7) ;           : lui) _lui) _unsigned ;
: auipc) _auipc) _unsigned ;

: subi) addi) 1 to _neg? ; \ Macro

\ Logicial Instructions
: xor) op) 4 funct3) $00 funct7) ;           : xori) op-imm) 4 funct3) 1 to _nochecking? ;
: or) op) 6 funct3) $00 funct7) ;            : ori) op-imm) 6 funct3) 1 to _nochecking? ;
: and) op) 7 funct3) $00 funct7) ;           : andi) op-imm) 7 funct3) 1 to _nochecking? ;

\ Compare Instructions
: slt) op) 2 funct3) $00 funct7) ;           : slti) op-imm) 2 funct3) _signed  ;
: sltu) op) 3 funct3) $00 funct7) ;          : sltiu) op-imm) 3 funct3) _unsigned ;

\ Branches and jump utilities
\ [TBD] Should I let them as "rforward" or should I rename them "forward" and make them erase
\ previous forward definitions ?
\
\ Rather than storing info about instruction that is forwarding, when doing rforward!
\ we read the instruction and determine its type
\ Allow the use of forwarding for two sequence instr (e.g. call) and allow two kind of use of
\ forward labels:

\ Jump system need to be totally rethinked
: rforward _twoinstr? if here rot> $10000 else $10000 here rot> then ;
: rforward16 _twoinstr? if here rot> $100 else $100 here rot> then ;
: rforward8 _twoinstr? if here rot> 0 else 0 here rot> then ;

\ Near jump scenario
: _rforward! ( jmpaddr -- )
    here over - over le@ dup
	$7f and case
	    $63 = of swap SBimm) endof
	    $6f = of swap UJimm) endof
	    $67 = of swap UJRimm) endof \ should not happen
	    drop of 2drop abort" Trying to use forward in a non control flow instruction" endof
	endcase swap le! ;

\ Far jump scenario
: _2rforward! ( jmpaddr -- )
    1 to _nochecking?
    here over - ( jmpaddr rel ) 2dup swap ( jmpaddr rel rel jmpaddr )
    dup le@ rot _hi Uimm) swap le! swap 4 + swap ( writing in auipc instr ) ( jmpaddr rel )
    _lo over le@ dup ( jmpaddr rel[lo] instr instr )
	$7f and case \ this case might not be necessary, it might always be UJRimm)
	    $63 = of swap SBimm) endof \ never used?
	    $6f = of swap UJimm) endof \ never used?
	    $67 = of swap UJRimm) endof
	    drop of 2drop abort" Trying to use forward in a non control flow instruction" endof
	endcase swap le! 0 to _nochecking? ;


\ $17 being auipc) opcode => if jumpaddr is an auipc instr we know that this is a 2-sequence
\ instr, on which the upper instr treat the _hi of the immediate, and the lower instr the _lo imm
: rforward!
    dup le@ $7f and $17 = if _2rforward! else _rforward! then ;

\ Branches Instructions
: beq) branch) 0 funct3) _signed ;           : bne) branch) 1 funct3) _signed ;
: blt) branch) 4 funct3) _signed ;           : bge) branch) 5 funct3) _signed ;
: bltu) branch) 6 funct3) _signed ;        : bgeu) branch) 7 funct3) _signed ;

\ Jump Instructions
: jal) _jal) _signed ;      : jalr) _jalr) 0 funct3) _signed ;

\ M extensions instructions : multiply and divide
\ Both of R-instruction format and using op-32) opcode
: mul) op) 0 funct3) 1 funct7) ;     : mulh) op) 1 funct3) 1 funct7) ;
: mulhu) op) 3 funct3) 1 funct7) ;   : mulhsu) op) 2 funct3) 1 funct7) ;
: mulw) op-32) 0 funct3) 1 funct7) ;

: div) op) 4 funct3) 1 funct7) ;     : divu) op) 5 funct3) 1 funct7) ;
: rem) op) 6 funct3) 1 funct7) ;     : remu) op) 7 funct3) 1 funct7) ;
: divw) op-32) 4 funct3) 1 funct7) ; : divuw) op-32) 5 funct3) 1 funct7) ;
: remw) op-32) 6 funct3) 1 funct7) ; : remuw) op-32) 7 funct3) 1 funct7) ;

\ Pseudo instructions
: li12) addi) xZERO rs1) ;
: li) addi) lui) 1 to _twoinstr? 1 to _32offset? 1 to _samedst? ;

\ shortcut macro choosing between both
\ used as an optimization tool for non familiar user of the system
: li, ( reg imm -- )
    12 _scheckimmsz ( reg imm 0or1? )
    if li) rot imm) rot rd)
    else li12) rot rd) swap imm) then ,) ;

: la) addi) auipc) 1 to _twoinstr? 1 to _32offset? 1 to _samedst? ;

: mv) addi) 0 imm) ;

: not) xori) 1 to _nochecking? -1 imm) ;
: neg) sub) 1 to _reverse? xZERO rs2) ;

: b) beq) xZERO rs1) xZERO rs2) ; \ better to use this according to doc
: j) jal) xZERO rd) ;

: bgt) blt) 1 to _reverse? ;         : ble) bge) 1 to _reverse? ;
: bgtu) bltu) 1 to _reverse? ;       : bleu) bgeu) 1 to _reverse? ;
: beqz) beq) xZERO rs2) ;            : bnez) bne) xZERO rs2) ;
: bgez) bge) xZERO rs2) ;            : blez) bge) 1 to _reverse? xZERO rs2) ;
: bgtz) blt) 1 to _reverse? xZERO rs2) ;

: call12) jal) xRA rd) ;
: call) jalr) auipc) 1 to _twoinstr? 1 to _32offset? 1 to _samedst? xRA rd) ;

\ previously was using xRA as scratch reg => very wrong
\ now using x30 as scratch, which should be noted as a register that 'shouldn't be trust'
: tail12) jal) x0 rd) ;
: tail) jalr) x0 rd) x30 rs1) auipc) 1 to _twoinstr? 1 to _32offset? 0 to _samedst? x30 rd) ;

: ret) jalr) xZERO rd) xRA base) 0 imm) ;
: ret, ret) ,) ;

: nop) addi) xZERO rd) xZERO rs1) 0 imm) ;
: nop, nop) ,) ;

\ Macros
\ Stack manipulation macros
: push, ( r -- )
    addi) xRSP rd) xRSP rs1) -4 imm) ,)
    sw) swap src) xRSP base) 0 imm) ,) ;

: pop, ( r -- )
    lw) swap rd) xRSP base) 0 imm) ,)
    addi) xRSP rd) xRSP rs1) 4 imm) ,) ;

: ppush, ( r -- )
    addi) xPSP rd) xPSP rs1) -4 imm) ,)
    sw) swap src) xPSP base) 0 imm) ,) ;

: ppop, ( r -- )
    lw) swap rd) xPSP base) 0 imm) ,)
    addi) xPSP rd) xPSP rs1) 4 imm) ,) ;

\ Control flow macros
\ do not forget: tail), call), etc.. put TWO instr on stack

\ Relative addressing
: tail, ( rel@ -- )
    dup abs $fffff800 and if tail) rot imm) ,) else tail12) swap imm) ,) then ;
: call, ( rel@ -- )
    dup abs $fffff800 and if call) rot imm) ,) else call12) swap imm) ,) then ;

\ Absolute addressing
: absb, ( abs@ -- )
    abs>rel tail, ;
: absbl, ( abs@ -- )
    abs>rel call, ;

\ Macros
: pc>reg, ( r -- ) \ get PC value and put it in register
    auipc) swap rd) 0 imm) ,) ; \ AUIPC return PC in register when using 0 as immediate

\ Debug Macros
: loop, j) 0 imm) ,) ;
