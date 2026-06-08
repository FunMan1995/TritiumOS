needs io/stream fs/sh
unit xcomp/efi

$200 const PESZ
$14c const PEI386
$8664 const PEAMD64
$1000 const SECTIONALIGN
create pe PESZ allot
f"data/pe.bin" dup pe PESZ rot read# close

: spitpe ( machineid codesz -- a u )
  dup SECTIONALIGN align
  dup pe $190 + le!
  PESZ + SECTIONALIGN align pe $d0 + le! ( mach codesz )
  dup pe $9c + le!
  pe $198 + le! ( mach )
  pe $84 + wle! ( )
  pe PESZ ;
