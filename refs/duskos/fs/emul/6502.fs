needs lib/psrs lib/fmt lib/wordtbl lib/macro asm/6502d emul/cpu
unit emul/6502

extends CPU struct CPU6502 {
  uchar A X Y S P _pad ;
  ushort PC ;
}

: )mem mem( $10000 + ;
: exec( mem( $200 + ;

variable ea \ effective addr in *target*.

\ utility words for accessing emulated memory/registers
: 8b# $ff and ;
: 16b# $ffff and ;
: mem+ ( pc cpu -- a ) mem( + ;
: ea' ( cpu -- a ) mem( ea @ + ;
: mc@ ( pc cpu -- c ) mem+ c@ ;
: mc@+ ( pc cpu -- pc+1 c ) dipdup mem+ c@ dip 1+ | ;
: mc! ( c pc cpu -- ) mem+ c! ;
: m@ ( pc cpu -- n ) mem+ wle@ ;
: m@+ ( pc cpu -- pc+1 n ) dipdup mem+ wle@ dip 2+ | ;

\ Words that sets effective address based on instruction type
\ The signature is ( pc -- cpu pc+? ) which means that it sets EA and
\ advances current PC by 0, 1 or 2 bytes depending on the mode.
: inh ( pc cpu -- pc+? ) drop 0 ea ! ;
alias inh acc
: zp mc@+ ea ! ;
: imm drop dup ea ! 1+ ;
: abs m@+ ea ! ;
: ind tuck m@+ rot m@ ea ! ;
: zp,X tuck mc@+ rot X + 8b# ea ! ;
: zp,Y tuck mc@+ rot Y + 8b# ea ! ;
: abs,X tuck m@+ rot X + 16b# ea ! ;
: abs,Y tuck m@+ rot Y + 16b# ea ! ;
: ind,X r! mc@+ r@ X + r> m@ ea ! ;
: ind,Y r! mc@+ r@ m@ r> Y + ea ! ;
13 wordrefs _ inh imm acc zp zp,X zp,Y abs abs,X abs,Y ind ind,X ind,Y imm
: eard ( pc opcode cpu -- pc+? ) swap modeid _ swap wexec ;

\ Flags manipulation
: p! ( n mask cpu -- ) A! P and or A> to P ;
: carry! ( n cpu -- n ) over 8 rshift 1 and $fe rot p! ;
: carry? ( cpu -- f ) P 1 and  ;
: nz! ( n cpu -- ) swap 8b# bi not 2* | $80 and or $7d rot p! ;
: v! ( n cpu -- ) swap $80 and over A $80 and xor 2/ $bf rot p! ;
: a!nz ( n cpu -- ) 2dup to A nz! ;
: a!nzv ( n cpu -- ) 2dup v! a!nz ;

\ Instructions ( cpu -- )
: ora ea @ swap r! mc@ r@ A or r> a!nz ;
: _and ea @ swap r! mc@ r@ A and r> a!nz ;
: eor ea @ swap r! mc@ r@ A xor r> a!nz ;
: adc ea @ swap r! mc@ r@ carry? + r@ A + r@ carry! r> a!nzv ;
: sbc
  r! A ea @ r@ mc@ r@ carry? not + - $100 xor r@ carry! r> a!nzv ;
: asl r! ea' dup c@ 2* r@ carry! dup r> nz! swap c! ;
: rol r! ea' dup c@ 2* r@ carry? or r@ carry! dup r> nz! swap c! ;
: lsr r! ea' dup c@ dup 1 and $fe r@ p! 2/ dup r> nz! swap c! ;
: ror
  r! ea' dup c@ r@ carry? 8 lshift or dup 1 and $fe r@ p! ( a n )
  2/ dup r> nz! swap c! ;
: _ ( c cpu -- ) ea' c! ;
: sta bi A | _ ; : stx bi X | _ ; : sty bi Y | _ ;
: _ ( cpu -- c cpu ) ea @ over mc@ swap 2dup nz! ;
: lda _ to A ; : ldx _ to X ; : ldy _ to Y ;
: _ ( c cpu -- )
  ea @ swap r! mc@ - $100 xor r@ carry! dup r@ v! r> nz! ;
: cmp bi A | _ ; : cpx bi X | _ ; : cpy bi Y | _ ;
: pc+ea ( cpu -- )
  ea @ swap r! mc@ dup $80 and if $ff00 or then r> doto PC + | ;
:~ ( cpu mask -- ) over P and if pc+ea else drop then ;
: bcs $01 ~ ; : beq $02 ~ ; : bvs $40 ~ ; : bmi $80 ~ ;
:~ ( cpu mask -- ) over P and not if pc+ea else drop then ;
: bcc $01 ~ ; : bne $02 ~ ; : bvc $40 ~ ; : bpl $80 ~ ;
: _ does> over P or swap to P ;
$01 _ sec $04 _ sei $08 _ sed $10 _ brk
: _ does> over P invand swap to P ;
$01 _ clc $04 _ cli $08 _ cld $10 _ clv
: pull ( cpu -- c ) r! S $100 or r@ mc@ r> doto S 1+ | ;
: push ( c cpu -- ) r! doto S 1- | r@ S $100 or r> mc! ;
: pla bi pull | to A ;
: plp bi pull | to P ;
: pha bi A | push ;
: php bi P | push ;
: rti r! plp r@ pull r@ pull 8 lshift or r> to PC ;
: rts r! pull r@ pull 8 lshift or 1+ r> to PC ;
: jmp ea @ swap to PC ;
: jsr r! PC 1- dup 8 rshift r@ push r@ push r> jmp ;
: bit ea @ swap r! mc@ dup r@ A and not 2* or $cd r> p! ;
: inc ea @ swap r! mc@ 1+ dup r@ nz! ea @ r> mc! ;
: dec ea @ swap r! mc@ 1- dup r@ nz! ea @ r> mc! ;
4 times": %< r! A! doto %< %< | A> %1 r> nz! ; "
  dex X 1-
  dey Y 1-
  inx X 1+
  iny Y 1+
6 times": %< A! %< A> to %< ; "
  txa X A
  tax A X
  tya Y A
  tay A Y
  txs X S
  tsx S X
: nop drop ;

OPCNT wordrefs _ ora _and eor adc sta lda cmp sbc asl rol lsr ror
  stx ldx dec inc bit jmp sty ldy cpy cpx brk bpl jsr bmi rti bvc
  rts bvs bcc ldy bcs cpy bne cpx beq php clc plp sec pha cli pla
  sei dey tya tay clv iny cld inx sed txa txs tax tsx dex nop
: nulop ( op -- ) .x abort" invalid opcode" ;
: oprun ( opcode cpu -- ) swap opid dup OPCNT < if
  _ swap wexec else nulop then ;

: .cpu ( cpu -- )
  A! PC A> P A> S A> Y A> X A> A
  "A %b X %b Y %b S %b P %b PC %w\n" printf ;
0 value verbose

: :halted? P $10 and bool ;
: :step ( cpu -- )
  r! doto P $ef and | \ V1=cpu
  r@ PC r@ mc@+ tuck r@ eard r@ to PC r@ oprun
  verbose if r@ .cpu then rdrop ;
: reset ( cpu -- ) 0 over to P $200 swap to PC ;

: new6502 ( mem( -- cpu )
  ['] :halted? ['] :step rot newcpu
  CPU6502 typesz CPU typesz - allot dup reset ;
