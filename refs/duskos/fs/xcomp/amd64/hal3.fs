: rex.b! $10000000 or ;
: rex.r! $40000000 or ;
: rex.w! $80000000 or ;
: RSP) $107 rex.b! ;
: sys) SYSVARS + m) ;

: ?disp,
  dup $c0 and dup $40 - if
    $80 - if drop else hslot@ , then
    else drop hslot@ c, then ;

: modrm,
  dup $c0 and $c0 - if
    dup $c7 and $05 - if
      dup b2:0 swap $7 invand $4 or dup c,
      swap $28 or c, ?disp,
      else $80 or dup c, ?disp, then
    else c, then ;

: ?rex, dup $1c rshift if dup $1c rshift $40 or c, then ;

: rmrex $f0000000 invand ;
: deref!
  dup (&? if
    -&) dup $c0 and if
      dup $6 reg! rex.r! $ff00 invand $80008d00 or
      ?rex, dup opc, dup c, ?disp,
      rmrex $3 $6 modrm! rex.b!
      else $c0 or then then ;

: op, deref! ?66, ?rex, dup opc, modrm, ;
: op0f, deref! ?66, ?rex, $0f c, dup opc, modrm, ;

: @,
  dup HALIMM and if
    dup >>3 b2:0 $b8 or c, hslot@ ,
    else dup (dir? if $8a opc! op, else
      dup HAL16B HAL8B or and HAL8B - if
        HAL16B invand dup (signed? if $be else $b6 then opc! op0f,
        else $8a opc! op, then then then ;

: ?mem+,
  dup (&? not if $6 reg! rex.r! @, $106 &) rex.b! then
  rex.w! ?rex, $01 c, dup $e8 or c, ;
