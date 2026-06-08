: _ swap hslot! ;
: i) [ HALBASE HALIMM or litn ] _ ;
: m) [ HALBASE HALMEM or litn ] _ ;
: sys) [ HALBASE HALOFF or litn ] _ $7 rs1! ;
: ind HALDIRECT invand dup HALIMM and if HALIMM invand HALMEM or then ;
: 32b) ind HAL8B invand HAL16B or ;
: 8b) 32b) HAL8B or ;
: 16b) 32b) HAL16B invand ;
: &) 32b) dup HALMEM and if HALMEM invand HALIMM else HALDIRECT then or ;

: +)
  dup if over [ HALMEM HALOFF or litn ] and if
    over hslot@ + hslot!
    else hslot! HALOFF or then
    else drop then ;

: x10 $a ; : x11 $b ; : xS $6 ; : x12 $c ;

: op, [ HALMASK HALBMASK or litn ] invand or , ;

: ?inv dup (dir? if rd<>rs2 swap $20 or swap then ;

: sz!
  dup HAL8B and if $4 else dup HAL16B and if $2 else $5 then then
  over dup (dir? swap (signed? or if $4 invand then $c lshift rot or swap ;

: addrr, $33 swap rs2! swap rdrs1! , ;

: ?imm,
  dup [ HALMEM HALIMM or litn ] and if
    x10 over hslot@ imm, x10 rs1!
    [ HALBMASK HALMEM HALIMM or or litn ] invand then ;

: mvreg,
  dup (dir? if rd<>rs1 then
  dup HALOFF and if
    dup hslot@ dup ?>12b if
      x10 swap imm,
      x10 rs2! $33
    else $13 swap Iimm! then
  else $13 then swap op, ;

: loadstore,
  tuck (dir? if
    rot swap Simm! swap op,
    else rot swap Iimm! swap op, then ;
: @,
  dup HALIMM and if
    dup rd@ swap hslot@ imm,
  else
    dup (&? if mvreg, else
      ?imm, $3 swap ?inv sz!
      dup HALOFF and if
        dup hslot@ swap HALBMASK invand over 12bovfl if
          x11 rot imm, x11 over rs1@ addrr, x11 rs1! op, else loadstore, then
        else $0 swap loadstore, then then then ;

: swp, dup rs1@ rs2! dup rd@ rs1! $00004033 swap op, ;
: @!,
  ?imm, dup (&? if
    dup swp, dup rd<>rs1 swp, swp,
  else
    dup x11 rd! @, dup dir) @,
    HALRDMASK and $00058013 or , then ;

: ?rs1>x10
  dup HALOFF and if
    x10 over hslot@ loadimm,
    [ HALBMASK HALOFF or litn ] invand
    x10 over rs1@ addrr, x10 rs1!
  else
    dup rd@ over rs1@ - if else
      $13 x10 rd! over rs1@ rs1! , x10 rs1! then then ;

: _srl8, rd@ $00805013 over rs1! swap rd! , ;
: _le!
  dup 8b) dup @, dup _srl8, dup $1 +) @, swap HAL16B and if
    dup _srl8, dup $2 +) @, dup _srl8, $3 +) @, else drop then ;

: _be!
  dup HAL16B and if
    8b) dup $3 +) @, dup _srl8, dup $2 +) @,
    dup _srl8, dup $1 +) @, dup _srl8, @,
    else 8b) dup $1 +) @, dup _srl8, @, then ;

: _x11+@, +) x11 rd! @, ;
: _orr, $14 lshift $00001013 or x11 rs1! x11 rd! ,
        $00b06033 over rs1! swap rd! , ;
: _16 8b) dup @, dup $1 _x11+@, dup rd@ $8 _orr, ;
: le@,
  dup HAL8B and if @, else
    ?imm, ?rs1>x10
    dup (dir? if _le! else
      dup HAL16B and if
        _16 dup $2 _x11+@, dup rd@ $10 _orr,
        dup $3 _x11+@, rd@ $18 _orr,
        else _16 drop then then then ;

: be@,
  dup HAL8B and if @, else
    ?imm, ?rs1>x10
    dup (dir? if _be! else
      dup HAL16B and if
        8b) dup $3 +) @, dup $2 _x11+@, dup rd@ $8 _orr,
        dup $1 _x11+@, dup rd@ $10 _orr,
        dup $0 _x11+@, rd@ $18 _orr,
        else 8b) dup $1 +) @, dup x11 rd! @, rd@ $8 _orr, then then then ;
: ale@, @, ;
: abe@, be@, ;
: u@, le@, ;

: brr,
  dup HALIMM and if hslot@ call, else $0b rd! @, $000580e7 , then ;
: br,
  dup HALIMM and if hslot@ bbr, else $0b rd! @, $00058067 , then ;

: ?rs1<>rs2 over $8 and if rs1<>rs2 then ;
: cond! $7 and funct3! ;
: ?signed over (signed? if $2 invand then ;

: lastinstr here $4 - @ ;
: regZ dup $2 rshift $7 and if
    rd@ else dup $20 and if rs2@ else rd@ then then ;

: ?src>x11,
  dup (&? not over HALOFF and or
  if dup x11 rd! -dir) @, x11 rs2! dup rd@ rs1!
  else dup rd@ rs2! rs1<>rs2 then ;
: bcc12,
  ?signed invcond swap ?src>x11, dup (dir? if rs1<>rs2 then
  [ HALRS1MASK HALRS2MASK or litn ] and $663 or ?rs1<>rs2 swap cond! , ;
( TODO: I think we can rewrite ?br, so that we don't always jump 12 forward )
( This will also need a br! rewrite )
: ?br, bcc12, bbr, ;
: if, invcond here $200 + rot rot ?br, here $8 - ;
: Zop $50 lastinstr regZ rd! ;
: ifz, Zop =) if, ;
: ifnz, Zop <>) if, ;
: ?brz, Zop =) ?br, ;
: ?brnz, Zop <>) ?br, ;

: bool, invcond bcc12, $0 i) @, $0080006f , $1 i) @, ;

: ?x11>src, ( op1 -- )
  dup (dir? if x11 rd! @, else drop then ;

: FUNCT7MASK $fe000000 ;
: preari, ( op1 instr -- op1 instr op2 )
  over dup HALIMM and if
    over FUNCT7MASK and over hslot@ ?>12b or if
      dup x10 rd! @, dup rd@ rs1! x10 rs2!
      else dup rd@ rs1! tuck hslot@ Iimm! $20 invand swap then
    HALIMM invand
  else
    ?imm, dup (dir? if
      -dir) dup x11 rd! @,
      rd<>rs1 x11 rd! dup rs1@ rs2! x11 rs1!
    else ?src>x11, then then HALDIRECT or ;
: ari, preari, op, ?x11>src, ;

: +, $00000033 ari, ;
: &, $00007033 ari, ;
: |, $00006033 ari, ;
: ^, $00004033 ari, ;
: swap-, $40000033 preari, rs1<>rs2 op, ?x11>src, ;
: *, $02000033 ari, ;
: neg $0 swap - ;
: -,
  dup HALIMM and if
    dup hslot@ neg ?>12b if $40000033 ari, else
      dup hslot@ neg hslot! +, then
    else $40000033 ari, then ;
: shift,
  swap dup HALIMM and if
    tuck hslot@ $1f and over $14 rshift or
    rot swap hslot! then swap ari, ;
: <<, $00001033 shift, ;
: >>, $00005033 over (signed? if $40000000 or then shift, ;

: +n,
  dup (&? if swap i) swap rs1@ rd! +, else
    -dir) tuck x12 rd! @,
    dup if i) x12 rd! +, x12 rd! dir) @, else
    drop drop then then ;
: testz, $0 swap +n, ;

: !n,
  dup HALDIRECT and if rs1@ swap imm,
  else x11 rot imm, x11 rd! dir) @, then ;

: remdiv, swap
  dup (signed? not if $1000 or then
  dup (dir? if rs1<>rs2 then op, ;
: /mod, dup
  ?src>x11, dup x12 rd! $02004033 remdiv, xS rd! $02006033 remdiv,
  dup (dir? if x12 rd! else x12 rs1!
  [ HALMEM HALIMM HALBMASK or or litn ] invand
  &) then @, ;

: sz dup HAL8B and if drop $1 else HAL16B and if $4 else $2 then then ;
: @+, dup @, dup sz swap &) +n, ;
: -@, dup sz neg over &) +n, @, ;
: popret PSP) dir) -@, RSP) @+, ; immediate

: srcreg ( op -- reg ) dup (dir? if rd@ else rs2@ then ;
: dstreg ( op -- reg ) dup (dir? if rs2@ else rd@ then ;
: dstsz ( op -- n ) dup (dir? if sz $3 lshift else drop $20 then ;
: lrot, ( op -- )
  -signed) dup HALIMM and if
    dup hslot@ $1f and hslot!
    $00005593 over hslot@ $20 swap - Iimm! over rd@ rs1! ,
    dup <<, HALIMM invand x11 rs1! &) |,
  else
    dup ?src>x11, $00006513 over dstsz neg Iimm! over srcreg rs1! , $40a00533 ,
    $00a05633 over dstreg rs1! , $00001033 over dstreg rdrs1! over srcreg rs2! ,
    $00c06033 swap dstreg rdrs1! , dup (dir? if x11 rd! @, else drop then
  then ;
: rrot, ( op -- )
  -signed) dup HALIMM and if
    dup hslot@ $1f and hslot!
    $00001593 over hslot@ $20 swap - Iimm! over rd@ rs1! ,
    dup >>, HALIMM invand x11 rs1! &) |,
  else
    dup ?src>x11, $00006513 over dstsz neg Iimm! over srcreg rs1! , $40a00533 ,
    $00a01633 over dstreg rs1! , $00005033 over dstreg rdrs1! over srcreg rs2! ,
    $00c06033 swap dstreg rdrs1! , dup (dir? if x11 rd! @, else drop then
  then ;

: pre, $00030e63 , ;
: post, $fff30313 , $fe0316e3 , ;
: move, pre, $00022983 , $0132a023 , $00420213 , $00428293 , post, ;
: wmove, pre, $00025983 , $01329023 , $00220213 , $00228293 , post, ;
: cmove, pre, $00024983 , $01328023 , $00120213 , $00128293 , post, ;
: pre, $00030a63 , ;
: post, $fff30313 , $fe031ae3 , ;
: fill, pre, $0042a023 , $00428293 , post, ;
: wfill, pre, $00429023 , $00228293 , post, ;
: cfill, pre, $00428023 , $00128293 , post, ;
: pre, $00000613 , $02030063 , ;
: post, $00a64633 , $fff30313 , $fe0602e3 , $00060613 , ;
: c[]=, pre, $0002c503 , $00024603 , $00128293 , $00120213 , post, ;
: w[]=, pre, $0002d503 , $00025603 ,  $00228293 , $00220213 , post, ;
: []=, pre, $0002a503 , $00022603 , $00428293 , $00420213 , post, ;
: pre, $fff24513 , $00030613 , $00060a63 , ;
: post, $fff60613 , $fe4518e3 , $40c30333 , $fff30313 , $00454533 , ;
: cidx, pre, $0002c503 , $00128293 , post, ;
: widx, pre, $0002d503 , $00228293 , post, ;
: idx, pre, $0002a503 , $00428293 , post, ;
