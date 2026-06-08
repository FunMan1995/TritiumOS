needs tests/harness asm/m68k
testbegin

0 value c
0 value h
: _#eq 2dup = if 2drop else ."instr: " h here over - 2/ wspit[] #eq then ;
: t[ here to h scnt to c ;
: ]t ( ... ) scnt c - here h - 2/ over _#eq ( ... n )
  here swap 0 do 2- dup wbe@ rot _#eq loop drop ;

t[ $4281 D1 clr, ]t
t[ $4201 D1 byte) clr, ]t
t[ $4241 D1 word) clr, ]t
t[ $2441 A2 D1 move, ]t
t[ $2478 $0824 A2 $824 abs) move, ]t
t[ $2212 D1 A2 [An]) move, ]t
t[ $2481 A2 [An]) D1 move, ]t
t[ $24c1 A2 [An]+) D1 move, ]t
t[ $0481 $0000 $002a D1 42 subi, ]t
t[ $243c $0000 $1234 D2 $1234 imm) move, ]t
t[ $742a D2 42 moveq, ]t
t[ $2f3c $0000 $1234 RSP -[An]) $1234 imm) move, ]t
t[ $2f0e RSP -[An]) A6 move, ]t
t[ $487a $0028 42 [PC,d]) pea, ]t
t[ $357c $0001 $002c A2 44 [An,d]) word) 1 imm) move, ]t
t[ $4ef9 $0010 $3000 $103000 abs) jmp, ]t
t[ $207c $0010 $2000 A0 $102000 imm) move, ]t
t[ $43fa $003e A1 64 [PC,d]) lea, ]t
t[ $e79a D2 3 rol#, ]t
t[ $e18d D5 8 lsl#, ]t
t[ $e6d1 A1 [An]) ror, ]t
t[ $1036 $0800 D0 A6 [An]) D0 Xn]) byte) move, ]t
t[ $223b $2800 D1 0 [PC,d]) D2 Xn]) move, ]t
t[ $0280 $0000 $000f D0 $f andi, ]t
t[ $d493 D2 A3 [An]) add, ]t
t[ $9593 A3 [An]) D2 sub, ]t
t[ $b493 D2 A3 [An]) cmp, ]t
t[ $b593 A3 [An]) D2 eor, ]t
t[ $43f3 $4801 A1 A3 1 [An,d]) D4 Xn]) lea, ]t
t[ $0c2a $0024 $0001 A2 1 [An,d]) byte) '$' cmpi, ]t
t[ $c342 D1 D2 exg, ]t
t[ $c38a D1 A2 exg, ]t
t[ $c38a A2 D1 exg, ]t
t[ $c34a A1 A2 exg, ]t
\ Used to be $8107 but is *not* valid.
\ In the inverted form, mode 0 for EA is invalid.
t[ $8e00 W byte) D0 or, ]t
t[ $9dc3 A6 D3 suba, ]t

testend
