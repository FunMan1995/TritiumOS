needs lib/str fs/core io/stream asm/uxntal emul/varvara
unit app/turye

varvara#
rstvec uxntal<< app/turye.tal
create rom( $100 allot0 rstvec uxntallen cmoveallot
here const )rom
: rom[] rom( )rom over - ;
"src/buf" findLabel# const src/buf
"data/widths" findLabel# const data/widths
: turye[] data/widths uxn + $2100 ;
: turyeload ( path -- )
  dup str>zstr src/buf uxn + zmove drop ( path )
  openpath dup turye[] rot read# close ;

: turyesave ( -- )
  src/buf uxn + z[] []>str ( path )
  openpath dup turye[] rot write# close ;

: turye<< rom[] uxn swap cmove word turyeload uxnctx launchcontext ;
: turye word"data/font/sans12.uf2" turye<< ;
