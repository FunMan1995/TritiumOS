needs lib/wordtbl mem/dict text/ts
unit asm/armd

0 value dpc
0 value opc
16 value listingsz

: opfield ( mask rshift ) here# rot> , , does> ( 'does )
  opc over @ rshift swap 4+ @ and ;
$f 28 opfield fcond
$f 21 opfield fopid
$f 16 opfield fRn \ for most ops
$f 12 opfield fRd \ for most ops
$f 8  opfield fRs \ same for imm "rotate"
$1f 7 opfield fshiftN
$3 5  opfield fshift
$f 0  opfield fRm
$ff 0 opfield fimm
$fff 0 opfield fimmaddr
$1 20 opfield fS

: .2 ( n -- ) dup . 10 < if spc> then ;
: .name ( tbl idx -- ) 3 * + 3 rtype ;
create names ,"EQ NE CS CC MI PL VS VC HI LS GE LT GT LE    NE "
: .cond ( -- ) names fcond .name ;
: .reg ( n -- ) 'r' emit .2 ;

\ opcodes are separated in 8 big groups from b27:25
\ each group has its own handler
\ group handlers sig: ( -- )
\ GRP0 is the one with the most exceptions
\ The rest is more regular

create names ,"ANDEORSUBRSBADDADCSBCRSC"
             ,"TSTTEQCMPCMNORRMOVBICMVN"
: .ariname ( -- ) fopid names swap .name ;
: .fS ( -- ) fS if ."S" else spc> then ;
: .cpsr ( -- ) opc $00400000 and if 'S' else 'C' then emit ."PSR " ;
: alt ( -- )
  fopid 8- case
    1 = opc $10 and bool and of ."BX   " fRm .reg endof
    bi 1 = | 3 = or of ."MSR  " .cpsr fRm .reg spc> endof
    bi 0 = | 2 = or of ."MRS  " fRd .reg spc> .cpsr endof
    ."???  " endcase ;
: alt? ( -- f ) fS if 0 else fopid 8- 4 < then ;
: ?.fRd ( -- ) fopid 4/ 2 <> if fRd .reg spc> then ;
: ?.fRn ( -- ) fopid $d and $d <> if fRn .reg spc> then ;

create names ,"lsllsrasrror"
: ?.shift ( -- )
  opc $f90 and if
    names fshift .name spc>
    fshiftN opc $10 and if 2/ .reg else .2 then then ;

: reg ( -- )
  alt? if alt exit then
  .ariname .fS spc>
  ?.fRd ?.fRn
  fRm .reg ?.shift ;

create names ,"MUL  MLA  ???  ???  UMULLUMLALSMULLSMLAL"
             ,"SWP  ???  SWPB ???  ???  ???  ???  ???  "
: special dup fopid 5 * names + 5 rtype ;
: grp0 opc $90 and $90 = if special else reg then ;

: .immrot ( imm rot -- )
  ?dup if 16 swap- 2* lshift then .x ;

: grp1 .ariname .fS spc>
  ?.fRd ?.fRn
  fimm fRs .immrot spc> ;

: .x3 dup $ffff > if
    dup 16 rshift .x1 .x2 else dup $ff > if .x2 else .x1 then then ;
create names ,"STRLDR"
: grp2 ( -- )
  names opc 20 rshift 1 and .name
  opc 22 rshift 1 and if ."B" else spc> then spc>
  fRd .reg spc>
  fRn .reg spc>
  fimmaddr ?dup if ( off )
    opc $00800000 and if '+' else '-' then emit .x3 spc> ( )
    opc $00200000 and if '!' emit then
    opc $01000000 and not if ."!p" then then ;

: grp3 opc $10 and if ."???" else names fS .name then ;

create names ,"STMLDM"
: grp4 names opc 20 rshift 1 and .name ;

code sex24 8 i) <<, 8 i) signed) >>, exit,
create names ,"B  BL "
: grp5 ( -- )
  names opc 24 rshift 1 and .name 2 nspcs
  opc sex24 1+ 4* dpc + ( jmpaddr )
  dup ?xt>e ?dup if nip entryname[] rtype else .x then ;

create names ,"STCLDC"
: grp6 ( -- ) names opc 20 rshift 1 and .name ;

: grp7 ( -- )
  opc case
    $01000010 and $10 = of \ MRC/MCR
      $00100000 opc and if ."MRC" else ."MCR" then 2 nspcs
      'p' emit fRs .2
      fopid .2
      fRd .reg
      'c' emit fRn .2
      'c' emit fRm .2
      opc 5 rshift 7 and .2 spc> endof
    drop ."???  " endcase ;
8 wordrefs grps grp0 grp1 grp2 grp3 grp4 grp5 grp6 grp7
: .op ( -- )
  ts[ doto dpc dup 4+ | dup .x spc> @ ( opc )
  to opc .cond
  grps opc 25 rshift 7 and wexec
  36 tsgo opc .x nl> ]ts ;
: dis ( a -- ) to dpc listingsz 0 do .op loop ;
: disn ( -- ) dpc dis ;
