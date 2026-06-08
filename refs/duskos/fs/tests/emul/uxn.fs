needs tests/harness asm/uxntal emul/uxn
testbegin

: wchk ( ... n -- ) wstcnt over #eq wptr swap 0 do c@+ rot #eq loop drop ;
: rchk ( ... n -- ) rstcnt over #eq rptr swap 0 do c@+ rot #eq loop drop ;
: init" rstvec uxntal" ;
create myhandlers $100 4* allot0
myhandlers newuxn

\test
init"#2a INCk INC ADDk BRK"
uxn$ poweron
42 44 86 3 wchk

\test
init"#01fe #0103 ADD2 BRK SWP BRK SUBk BRK POP2 BRK"
uxn$ rstvec runat
3 1 2 wchk
runat
1 3 2 wchk
runat
1 3 $fe 3 wchk
runat drop
1 1 wchk

\test
init"LITr 12 #022a STHk INCr BRK ADDkr MULk ORAkr BRK"
uxn$ rstvec runat
2 42 2 wchk
18 43 2 rchk
runat drop
2 42 84 3 wchk
18 43 61 63 4 rchk

\test read null-terminated string in ZP into stack
init"#00 @loop LDZk SWP INC OVR ,loop JCN POP2 BRK"
uxn$ "foo\0" c@+ uxn swap cmove
poweron
'f' 'o' 'o' 3 wchk

\test
init"#2a @loop INC BRK loop"
uxn$ rstvec runat
43 1 wchk
0 rchk
runat
44 1 wchk
1 7 2 rchk
runat drop
45 1 wchk
1 7 1 7 4 rchk

\test
init"#2a @loop INC BRK !loop"
uxn$ rstvec runat
43 1 wchk
0 rchk
runat
44 1 wchk
0 rchk
runat drop
45 1 wchk
0 rchk

\test
init"#2a @loop INC BRK #2c OVR SUB ?loop #1234 BRK"
uxn$ rstvec runat
$2b 1 wchk
0 rchk
runat
$2c 1 wchk
0 rchk
runat drop
$2c $12 $34 3 wchk
0 rchk

\test
init";{ ;{ } #1234 } BRK"
uxn$ poweron
$01 $09 $01 $06 $12 $34 6 wchk

\test
init"#0f DEI #80 DEI2 #81 DEI #80 DEI #54cc #20 DEO2 #cc #81 DEO BRK"
uxn$
:~ 42 [dev!] $f $1234 [dev2!] $80 ; ~
0 value called
:> doto called 1+ | ; $21 4* myhandlers + !
poweron
devices $20 + wbe@ $54cc #eq
devices $80 + wbe@ $12cc #eq
called 1 #eq
42 $12 $34 $34 $12 5 wchk

\test
init"#12 #00 DIV #2a #03 DIV BRK"
uxn$ poweron
0 14 2 wchk

\test opctest
create buf $80 allot0
variable cnt
:> [dev@] $18 buf cnt @ + c! 1 cnt +! ; $18 4* myhandlers + !
rstvec uxntal<< tests/emul/uxnopc.tal
uxn$ poweron
"Ok1\nOk2\nOk3\nOk4\nOk5\nOk6\nOpcodes passed!\nResult: passed!\n\n"
buf cnt @ s[]= not [if]
  ."opctests failed. Output:\n"
  buf cnt @ rtype not #true [then]
testend
