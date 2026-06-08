xcomp 6502a

$ff80 value SYSVARS
SYSVARS $409 - value BLK_MEM
$78 value VIOOUT
$7c value VIOIN
6502m xcompc
alignhere 0 xstart
$200 allot \ pages 0 and 1
6502c corel
code bye 6 # LDA, VIOOUT <> STA, BRK, ;code
pc to L1 \ no character
  BRK, 2DEX, 0 # LDA, 0 A>PS, 1 A>PS, ;code
code (key?) ( -- ?c f )
  VIOIN <> LDA, L1 br BEQ,
  2DEX, 2DEX,
  VIOIN 1+ <> LDA, 2 A>PS,
  0 # LDA, VIOIN <> STA, 1 A>PS, 3 A>PS,
  1 # LDA, 0 A>PS, ;code
code (emit) ( c -- )
  0 PS>A, VIOOUT 1+ <> STA, 2INX,
  1 # LDA, VIOOUT <> STA, ;code
: (blk@) [ VIOOUT 1+ litn ] ! 2 [ VIOOUT litn ] c!
         [ VIOOUT 1+ litn ] ! 4 [ VIOOUT litn ] c! ;
: (blk!) [ VIOOUT 1+ litn ] ! 2 [ VIOOUT litn ] c!
         [ VIOOUT 1+ litn ] ! 5 [ VIOOUT litn ] c! ;
: vio$ 1024 [ VIOOUT 1+ litn ] ! 3 [ VIOOUT litn ] c! ;
blksub
: init vio$ blk$ ;
nl> xwrap
xorg here over - stdio write# bye
