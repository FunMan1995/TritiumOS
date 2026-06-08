: not&# dup (&? if halerr then ;
: ind -&) dup HALIMM and if HALIMM invand HALMEM or then ;
: 32b) ind HAL16B or HAL8B invand ;
: 8b) 32b) HAL8B or ;
: (8b? HAL8B and bool ;
: 16b) 32b) HAL16B invand ;
: (16b? HAL16B and not ;
: &) 32b) dup HALMEM and if HALMEM invand HALIMM else HALDIRECT then or ;
: +) dup if over hslot@ + hslot! else drop then ;

: op, ( op instr -- ) swap HALMASK invand or , ;

: r0 $0 ; : r1 $1 ;
: hbank$ HALBMASK invand ;
: ?mem>rn ( op -- op )
  dup HALMEM and if
    dup hslot@ r1 swap imm,
    hbank$ HALMEM invand r1 Rn! then ;

: >ldrh ( op instr -- op instr )
  dup $02000000 and not if
    dup $f00 and if r1 over $fff and imm, $fff invand r1 or else
      dup $f0 and $4 lshift or $f0 invand $00400000 or then then
  $b0 or $06000000 invand
  over (signed? if $40 or over (8b? if $20 invand then then ;

: ?off, ( op instr -- op instr )
  over (slot if
    over (slot hbank@ rot hbank$ rot rot
    dup $80000000 and if
      $0 swap - swap $00800000 invand swap then ( op instr off )
    dup $fff invand if swap $2000001 or r1 rot imm, else or then then ;

: 8bldrh? dup (8b? over (signed? and swap (dir? not and ;
: ldrstr, ( op instr -- )
  swap ?mem>rn dup HALINV and $3 rshift rot xor
  ?off, over dup (16b? swap 8bldrh? or if >ldrh then op, ;

: _deref
  HALDIRECT HALINV or invand
  $e1a00000 over Rd@ Rd! over Rn@ or ,
  dup (slot if
    dup hslot@ dup if
      swap Rd@ swap addlit,
      else drop drop then
    else drop then ;
: @,
  dup (dir? if -signed) then
  dup HALIMM and if
    dup Rd@ swap hslot@ imm,
  else
    dup (&? if
      dup (dir? if
        dup Rd@ over Rn@ rot swap Rd! swap Rn! then
      _deref
      else $e5900000 ldrstr, then then ;

: (sz dup (16b? if drop $2 else (8b? if $1 else $4 then then ;
: _ over (sz or ldrstr, ;
: @+, $e4900000 _ ;
: -@, $e5300000 _ ;

: rn>r1, dup &) -dir) r1 Rd! @, r1 Rn! $0 slot) ;
: ?rn>r1 dup Rd@ over Rn@ - not if rn>r1, then ;
: ?bigoff>r1 dup hslot@ $fff invand if rn>r1, then ;

: _lsr8, ( op -- ) Rd@ $e1a00420 over Rd! or , ;
: _le!
  ?bigoff>r1 dup 8b) dup @, dup _lsr8, dup $1 +) @,
  swap HAL16B and if
    dup _lsr8, dup $2 +) @, dup _lsr8, $3 +) @, else drop then ;

: _be!
  ?bigoff>r1 dup HAL16B and if
    8b) dup $3 +) @, dup _lsr8, dup $2 +) @,
    dup _lsr8, dup $1 +) @, dup _lsr8, @,
    else 8b) dup $1 +) @, dup _lsr8, @, then ;

: _r0+@, ( op off -- ) +) r0 Rd! @, ;
: _orr, ( reg shift -- ) $7 lshift $e1800000 or over Rn! swap Rd! , ;
: _16 ( op -- op ) 8b) dup @, dup $1 _r0+@, dup Rd@ $8 _orr, ;
: le@,
  not&# dup HAL8B and if @, else
    ?mem>rn ?rn>r1
    dup HALINV and if _le! else
      dup HAL16B and if
        _16 dup $2 _r0+@, dup Rd@ $10 _orr,
        dup $3 _r0+@, Rd@ $18 _orr, else _16 drop then then then ;

: be@,
  not&# dup HAL8B and if @, else
    ?mem>rn ?rn>r1
    dup HALINV and if _be! else
      dup HAL16B and if
        8b) dup $3 +) @, dup $2 _r0+@, dup Rd@ $8 _orr,
        dup $1 _r0+@, dup Rd@ $10 _orr,
        dup $0 _r0+@, Rd@ $18 _orr,
      else
        8b) dup $1 +) @, dup $0 Rd! @, Rd@ $8 _orr, then then then ;
: ale@, @, ;
: abe@, be@, ;
: u@, le@, ;

: ?r0>src, ( op -- ) dup (dir? if r0 Rd! @, else drop then ;

: ariimm ( instr op -- instr op )
  tuck hslot@ dup immrot if drop r0 swap imm, else swap drop or HALIMM or then
  swap hbank$ tuck Rd@ Rn! swap ;
: ariregular ( instr op -- instr op )
  dup (&? over HALBMASK and not and if
    dup Rn@ or else dup r0 Rd! @, hbank$ then
  dup Rd@ Rn! ;
: aridir ( instr op -- instr op )
  -dir) dup Rd@ swap r0 Rd! dup @, ( instr rd op )
  hbank$ or r0 Rn! ;
: preari ( op instr -- origop instr op )
  swap ?mem>rn tuck dup (dir? if aridir else
    dup HALIMM and if ariimm else ariregular then then
  HAL8B invand ;
: ari, ( op instr -- ) preari swap op, ?r0>src, ;

: +, $00900000 ari, ;
: -, $00500000 ari, ;
: swap-, $00600000 ari, ;
: &, $00100000 ari, ;
: |, $01900000 ari, ;
: ^, $00300000 ari, ;
: compare, $01500000 preari r0 Rd! swap op, drop ;
: ?imm>r0
  dup HALIMM and if r0 over hslot@ imm, HALIMM invand hbank$ r0 Rn! &) then ;
: *, ?imm>r0 $00000090 preari dup Rd@ $8 lshift or r0 Rd! swap op, ?r0>src, ;

: ?signed
  over (signed? if
    dup $e0000000 and if
    dup $80000000 and if $40000000 else $80000000 then + then then ;
: bool,
  ?signed swap compare,
  $e3a09000 ,      ( mov) rW rd) 0 imm) )
  $02899001 or , ; ( add) z) r@ rdn) 1 imm) )
: ?br, ?signed swap compare, bbrc, ;
: if, ?signed swap compare, here $0 rot invcond bbrc, ;

: br,
  dup HALIMM and if hslot@ bbr, else
    $0 Rd! @, $e1a0f000 ( mov) rPC rd) r0 rm) ) , then ;

: brr,
  dup HALIMM and if hslot@ bl, else
    $0 Rd! @, $e1a0e00f ( mov) rLR rd) rPC rm) ) , $e1a0f000 , then ;

: eor, ( rd rm -- ) over Rd< or swap Rn< or $e0200000 or , ;
: @!,
  ?mem>rn dup (&? if
    dup Rd@ swap Rn@
    over over eor,
    over over swap eor, eor,
  else
    dup dup $0 Rd! @, dir) @,
    Rd@ $e1a00000 ( mov) dst rd) r0 rm) ) swap Rd! , then ;

: shift, ( op mask -- )
  dup rot dup (dir? if
    -dir) dup @!, dup rot shift, @!, drop
  else
    ?mem>rn dup Rd@ $e1b00000 over Rd! or rot or
    over HALIMM and if
      swap hslot@ dup if $7 lshift or , else drop drop then drop
      else swap $0 Rd! @, swap $70 xor not if $e2600020 , then
      $10 or , then then ;

: <<, $0 shift, ;
: >>, $20 over (signed? if $20 + then shift, ;
: rrot, dup (dir? bool over (sz $3 and bool and if
    dup -dir) $0 Rd! @, $e1a01030 over Rd@ $8 lshift or ,
    $e2605000 over (sz $3 lshift or over Rd@ Rn! , $e1810510 , $0 Rd! @,
  else $60 shift, then ;
: lrot, dup (dir? bool over (sz $3 and bool and if
    dup -dir) $0 Rd! @, $e1a01010 over Rd@ $8 lshift or ,
    $e2605000 over (sz $3 lshift or over Rd@ Rn! , $e1810530 , $0 Rd! @,
  else dup HALIMM and if dup (slot hbank@ $20 swap - hslot! $60
  else $70 then shift, then ;

: +n,
  ?mem>rn dup (&? if
    Rn@ swap addlit,
  else
    $0 Rd! dup @,
    over $0 swap addlit,
    swap if dir) @, else drop then then ;
: testz, $0 swap +n, ;

: !n, ( n op -- )
  dup HALDIRECT and if Rn@ swap imm,
  else r0 rot imm, r0 Rd! dir) @, then ;

: _ dup REGS Rd! @,
    dup Rd@ dup $e1a00000 ( mov) ) or ,
    pushlr,
    swap (signed? if [ ' (s/mod) litn ] else [ ' (/mod) litn ] then
    i) brr,
    $e1a00000 swap Rd! , poplr, ;
: /mod, dup (dir? if -dir) dup dup @!, _ @!, else _ then ;

: popret PSP) dir) -@, RSP) @+, ; immediate

( cmp) rS rn) 0 imm) )
: pre $e3580000 , ;
( sub) ne) rS rdn) 1 imm) f)
  -12 b) ne) )
: post $12588001 , $1afffffb , ;
( ldr) ne) r0 rd) rW rn) 4 +i) post)
  str) ne) r0 rd) rA rn) 4 +i) post) )
: move, pre $14990004 , $148b0004 , post ;
( ldrh/strh )
: wmove, pre $10d900b2 , $10cb00b2 , post ;
: cmove, pre $14d90001 , $14cb0001 , post ;
( sub) ne) rS rdn) 1 imm) f)
  -8 b) ne) )
: post $12588001 , $1afffffc , ;
( str) ne) rW rd) rA rn) 4 +i) post) )
: fill, pre $148b9004 , post ;
( strh )
: wfill, pre $10cb90b2 , post ;
: cfill, pre $14cb9001 , post ;
( cmp) rS rn) 0 imm)
  24 b) eq) )
: pre $e3580000 , $0a000004 , ;
( sub) rS rdn) 1 imm)
  cmp) r0 rn) r1 rm)
  -24 b) eq) )
: post $e2488001 , $e1500001 , $0afffff8 , ;
( ldr) r0 rd) rW rn) 4 +i) post)
  ldr) r1 rd) rA rn) 4 +i) post) )
: []=, pre $e4990004 , $e49b1004 , post ;
( ldrh )
: w[]=, pre $e0d900b2 , $e0db10b2 , post ;
: c[]=, pre $e4d90001 , $e4db1001 , post ;
( add) r1 rd) rS rn) 1 imm)
  sub) r1 rdn) 1 imm) f)
  add) eq) r0 rd) rW rn) 1 imm) (never matches) )
: pre $e2881001 , $e2511001 , $02890001 , ;
( cmp) ne) r0 rn) rW rm)
  -16 b) ne)
  cmp) r0 rn) rW rm)
  sub) rS rdn) r1 rm) )
: post $11500009 , $1afffffa , $e1500009 , $e0488001 , ;
( ldr) ne) r0 rd) rA rn) 4 +i) post) )
: idx, pre $149b0004 , post ;
( ldrh )
: widx, pre $10db00b2 , post ;
: cidx, pre $14db0001 , post ;
