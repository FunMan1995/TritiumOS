\ Collapse OS compatibility layer
needs lib/str fs/core fs/memfile io/drvstr mem/endian

$400 const BLKSZ
$40 const LNSZ

"cos.blk" mountImage const blks

create blk( BLKSZ allot
here const blk)
blk( BLKSZ newmemfile const blk
: blk@ << dup blk( blks readsector 1+ blk( $200 + blks readsector ;
: blk! << dup blk( blks writesector 1+ blk( $200 + blks writesector ;
create _buf LNSZ allot LF c,
: _next< ( -- ?a u )
  blk eof? if 0 else _buf LNSZ blk read# _buf LNSZ 1+ then ;
: load ( n -- )
  blk@ blk rewind
  ['] _next< NEXTIN< @! >r @{'INPTR} >r 0 insz @! >r
  interpret r> insz ! r> !{'INPTR} r> NEXTIN< ! ;

0 value blk>
:override load dup to blk> ~ ;
: loadr 1+ for2 i . spc> i load next ;
0 value BLKDTY
: blk!! 1 to BLKDTY ;
: flush 0 @!{>BLKDTY} if blk! then ;

create _ 0 ,
: doer here# _ ! ;
: does> r> _ @ bind ;
: imm? w>e e>wlen c@ $80 and bool ;
: word! NEXTWORD ! ;
: '? word SYSDICT find ;
: ?: '? if ";" begin word over s= until drop
     else curword NEXTWORD ! : then ;
\ COS doesn't have Dusk's special '"' behavior, we need to consume the following
\ whitespace.
: S" in< drop [rcompile] " ;
: S" in< drop [compile] " ; compileonly immediate
:override ," in< drop ~ ;
alias find _find
: find SYSDICT _find ?dup bool ;
: xtcomp [compile] ] ;
: values for 0 value next ;
: l, wle, ; : m, wbe, ;
: r~ [compile] rdrop ; immediate
: in<? in< dup bi 0< | LF = or if drop 0 then ;
create COMPILING 0 c,
: _ 0 COMPILING c! [compile] [ ;
alias _ [ immediate
: _ 0 COMPILING c! [compile] ; ;
alias _ ; immediate
: .x .x1 ;
: .X .x2 ;
alias swap- -^

: move cmove ;
: []= c[]= ;
: fill cfill ;
: move, cmoveallot ;

\ Our compatibility layer is complete, now let's load a few blocks from core
\ words to complete out vocabulary with COS-specific words
: <<8 $ff and 8 lshift ;
: >>8 8 rshift $ff and ;
0 value A
: A> A ; : >A to A ; : A+ +{>A !#1} ; : A- -{>A !#1} ;
312 load \ bit shifting, A register, leave l|m +!
\ dummy jmpi! and calli! to avoid bogus compilation in b314
: jmpi! ; : calli! ;
314 load \ crc16
323 load \ s= waitw [if] _bchk
332 load \ lnlen emitln list index
334 load \ Application loaders
