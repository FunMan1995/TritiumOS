: +) dup if over hslot@ + hslot! else drop then ;

: src>A0, A0 over src@ r@, $8 src! ;
: ?src>A0, dup HALIMM and if else
  dup hslot@ $8000 + $ffff invand if
    dup src@ $8 - if src>A0, then
    $d1fc w, dup hslot@ , clrbank
    else dup src@ A? if else src>A0, then then then ;

: rm<>mr dup b2:0 <<3 swap $3 rshift or ;
: mkrm dup $0 - if <<3 swap b2:0 or else drop then ;
: clr, $4280 or w, ;
: ?clrDn, dup $7 invand if drop else clr, then ;
: move, rot dup $2 - if rot dup ?clrDn, rot else rot rot then
        swap rm<>mr <<6 or swap <<12 or w, ;
: lea, $41c0 or w, ;
: rm@ dup dst@ swap src@ rot mkrm ;
: sz1 dup HAL8B and if drop $1 else HAL16B and if $3 else $2 then then ;
: sz2 dup HAL8B and if drop $0 else HAL16B and if $1 else $2 then then ;
: imm, $203c rot $0 mkrm rm<>mr <<6 or w, , ;

: @mode,
  swap dup sz1 rot rot dup (dir? if
    rm@ swap move, else rm@ move, then ;

: i)@, ( op -- )
  dup (&? if
    dup dst@ swap hslot@ imm,
    else dup $9 src! $7 @mode, hslot@ , then ;
: @off, ( op -- )
  ?src>A0, dup (&? if
    dup src@ $5 mkrm lea, dup hslot@ w, dst@ A0 r@,
    else dup $5 @mode, hslot@ w, then ;
: @direct, ( op -- )
  dup (&? if
    dup (dir? if
      dup src@ swap dst@ r@, else dup dst@ swap src@ r@, then
    else ?src>A0, $2 @mode, then ;
: ?signext ( op -- )
  dup (signed? over HAL816B and bool and over (dir? not and if
    $48c0 over HAL8B and or swap dst@ or w,
    else drop then ;
: @,
  dup dup HALIMM and if i)@, else
    dup hslot@ if @off, else @direct, then then ?signext ;

: _ over src@ A? if @mode, else over ?src>A0, swap @mode, src@ A0 r@, then ;
: @+, $3 _ ; : -@, $4 _ ;

: be@, @, ;

: _swap16 $4840 or w, ;
: _swap8 $e058 or w, ;
: le@,
  dup HAL8B and if @, else
    dup (dir? if
      dup HAL16B and if
        dup dst@ _swap8 @,
        else dup dst@ dup _swap8 dup _swap16 _swap8 @, then
  else
    dup @, dup HAL16B and if
      dst@ _swap8
      else dst@ dup _swap8 dup _swap16 _swap8 then then then ;
: ale@, le@, ;
: abe@, @, ;
: u@, @, ;

: @!, dup D0 dst! @, dup dir) @, dst@ D0 r@, ;

: D0@, dup D0 dst! -dir) @, $f0 and &) ;

: instr,
  dup w, dup $ffff invand if
    dup hslot@ swap $100000 and if , else w, then else drop then ;

: eaop,
  over (&? if
    over HALIMM and if
      $3c or instr, hslot@ ,
    else
      over hslot@ if
        swap D0@, drop instr, else swap src@ or instr, then then
  else
    over HALIMM and if
      $39 or instr, hslot@ ,
    else
      over hslot@ dup if
        rot ?src>A0, src@ $5 mkrm rot or instr, w,
        else drop swap ?src>A0, b2:0 $10 or or instr, then then then ;

: andneq? tuck and - ;
: +n,
  dup HALDIRECT $8 or andneq? if
    $100600 over HAL16B and $b lshift xor
    over HAL8B and <<12 xor
    rot hbank! hslot< or
    over sz2 <<6 or eaop,
    else b2:0 <<9 $d1c0 or swap i) swap eaop, then ;
: testz, $0 swap +n, ;

( TODO: use a single instruction instead )
: !n, dup HALDIRECT and if
    src@ swap imm,
    else D0 rot imm, D0 dst! dir) @,
    then ;

: _ over HALINV and $4 rshift or
    over dst@ <<9 or
    over sz2 <<6 or eaop, ;
: _upscale swap D0@, swap _ ;
: ari,
  over HALINV HALDIRECT or andneq? if else
    swap src<>dst -dir) swap then
  over HAL816B HALINV HALDIRECT or or and
  dup HAL8B - if
    HAL16B - if _ else _upscale then else drop _upscale then ;

: +, $d000 ari, ;
: -,
  dup HALDIRECT HALINV $8 or or andneq? if $9000 ari, else
    $91c0 over b2:0 <<9 or swap dst@ or w, then ;
: compare,
  dup (dir? if D0@, dst@ $b080 or w, else $b000 ari, then ;
: &, $c000 ari, ;
: |, $8000 ari, ;
: ^,
  dup (dir? if $b000 ari, else
    dup D0 dst! @, dst@ $b180 or w, then ;

: ?signed
  over (signed? if
    dup $1 invand $6 - if
      dup $1 invand $4 - if $c else $8 then + then then ;
: ?br, ?signed swap compare, bbrc, ;
: if, ?signed swap compare, invcond fbrc, ;
: bool, ?signed swap compare, $8 lshift $50c7 or w, $0287 w, $1 , ;

: neg, $4480 eaop, ;
: swap-, dup (dir? if dup else dup dst@ &) then neg, +, ;

: _ dup dst@ <<12 hbank! hslot< $4c00 or eaop, ;
: *,
  dup (dir? if
    dup (&? if src<>dst _ else
      dup D0@, src<>dst _ D0 dst! @, then
    else dup HAL816B and if D0@, then _ then ;

: _ $5 over dst@ <<12 or over HALSIGNED and $2 rshift or
    hbank! hslot< $4c40 or eaop, ;
: /mod,
  dup (dir? if
    dup (&? if src<>dst _ else
      dup D0 dst! -dir) @,
      dup dup dst@ src! D0 dst! clrbank &) _
      D0 dst! @, then
  else _ then ;

: _
  $20 or over D0 dst! -dir) @, over (dir? if
    over dst@ <<9 or $c0 invand over sz2 <<6 or
    w, D0 dst! @, else swap dst@ or w, then ;
: shift,
  over HALIMM and if
    over (&? if
      over dst@ or swap hslot@ dup $7 invand if
        D0 swap imm, $20 or w,
        else dup if <<9 or w, else drop drop then then
    else _ then
  else _ then ;
: <<, $e188 shift, ;
: >>, $e088 over (signed? if $8 - then shift, ;
: lrot, $e198 shift, ;
: rrot, $e098 shift, ;

: _ over HALIMM and if
  over (&? if swap hslot@ swap bri, $0 else $1 then else $1 then ;
: br, $6000 _ if drop A0 dst! @, $4ed0 w, then ;
: brr, $6100 _ if drop A0 dst! @, $4e90 w, then ;

: popret PSP) dir) -@, RSP) @+, ; immediate

: _ $2046 w, $5385 w, $00c7 or w, $51cd w, $fffc w, $2c08 w, ;
: fill, $2000 _ ;
: wfill, $3000 _ ;
: cfill, $1000 _ ;

: _ $2046 w, $2005 w, $7aff w, $5380 w, $6506 w,
    $5285 w, $be18 or w, $66f6 w, ;
: idx, $80 _ ;
: widx, $40 _ ;
: cidx, $00 _ ;

: _ $2047 w, $2246 w, $5385 w, $650c w,
    dup $0018 or w, $02c0 or w,
    $51cd w, $fffa w, $2e08 w, $2c09 w, ;
: move, $2000 _ ;
: wmove, $3000 _ ;
: cmove, $1000 _ ;

: _ $2047 w, $2246 w, $4a85 w, $670a w, $5385 w,
    $0018 or w, $b019 or w,
    $56cd w, $fffa w, ;
: []=, $80 $2000 _ ;
: w[]=, $40 $3000 _ ;
: c[]=, $00 $1000 _ ;
