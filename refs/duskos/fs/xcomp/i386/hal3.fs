: RSP) $104 ;
: sys) bank) $185 or ;

: _
  dup $c0 and dup $40 - if
    $80 - if
      dup $c7 and $05 - if drop else hslot@ , then else hslot@ , then
    else drop hslot@ c, then ;
: modrm,
  dup c, dup b2:0 $4 - if _ else
    dup $c0 and $c0 - if $24 c, then _ then ;

: deref!
  dup (&? if
    -&) dup $c0 and if
      dup $7 reg! $8d c, modrm, $3 $7 modrm! else $c0 or then then ;

: op, deref! ?66, dup opc, modrm, ;
: op0f, deref! ?66, $0f c, dup opc, modrm, ;

: @,
  dup HALIMM and if
    dup >>3 b2:0 $b8 or c, hslot@ ,
    else dup (dir? if $8a opc! op, else
      dup HAL16B HAL8B or and HAL8B - if
        HAL16B invand dup (signed? if $be else $b6 then opc! op0f,
        else $8a opc! op, then then then ;

: ?mem+, ;
: rmrex ;
