: bankrel, hslot@ rel, ;
: br, dup HALIMM and if $e9 c, bankrel, else ?mem+, $ff20 or op, then ;
: brr, dup HALIMM and if $e8 c, bankrel, else  ?mem+, $ff10 or op, then ;

: brc,
  over HALIMM and if swap hslot@ swap bbrc, else
    $1 xor $70 or c,
    here $0 , swap br, here over - $1 - swap ! then ;

: le@, @, ;

: xchgLH, dup $3 invand if halerr then $86 c, dup $3 lshift or $e0 or c, ;
: rol16, $c1 c, $c0 or c, $10 c, ;

: dst@ >>3 b2:0 ;
: be@,
  dup HAL8B and if
    dup (dir? if
      dup HAL16B and if
        dup dst@ xchgLH, @,
        else dup dst@ dup xchgLH, dup rol16, xchgLH, @, then
    else
      dup HAL16B and if
        dup @, dst@ xchgLH,
        else dup @, dst@ dup xchgLH, dup rol16, xchgLH, then then
  else @, then ;
: ale@, @, ;
: abe@, be@, ;
: u@, @, ;

: @!, $86 opc! op, ;

: ?,
  dup HAL16B and if
    drop dup c, $8 rshift c,
    else HAL8B and if , else c, then then ;

: arii, or dup op, ?, ;
: +n,
  $0 reg! dup HAL8B and if
    over $80 + $ffffff00 and if $8000 arii, else $83 opc! op, c, then
    else $8000 arii, then ;
: testz, $0 swap +n, ;

: !n, dup $c6 opc! $0 reg! op, ?, ;

: cx $1 ;
: _reg $b lshift $200 + or op, ;
: ari,
  over HALIMM and if
    single! dup hslot@
    dup $80 + $ffffff00 and if $81 c, swap c, , else $83 c, swap c, c, then
  else
    over (dir? if _reg else
      over HAL16B HAL8B or and HAL8B - if
        swap dup cx reg! @,
        $ff and $3 cx modrm! $300 or swap $b lshift or op,
        else _reg then then then ;

: +, $0 ari, ;
: -, $5 ari, ;
: &, $4 ari, ;
: |, $1 ari, ;
: ^, $6 ari, ;
: compare, $7 ari, ;

: ?signed
  over (signed? if
    dup $a and if
    dup $4 and if $8 else $a then + then then ;
: ?br, ?signed swap compare, bbrc, ;
: if, ?signed swap compare, invcond here dup rot bbrc, ;
( ax 0 i) mov, al setXX, )
: bool, ?signed swap compare, $b8 , $f00 w, $c090 or w, ;

: _<>0+,
  dup HALIMM and if
    dup hslot@ if +, else drop then
  else +, then ;
: _neg+ $f6 opc! rmrex op, _<>0+, ;
: swap-,
  dup dup (dir? if
    -dir) $3 reg!
    else -&) $3 single! clrb then _neg+ ;

: _<>cx!, dup b2:0 cx xor if cx reg! @, else drop then ;
: shift,
  over HALIMM and if
    single! $c1 c, dup c, hslot@ c,
  else
    over (dir? if
      <<3 swap dup >>3 b2:0 cx xor if dup $3 cx modrm! clrb @, then
      -dir) $0 reg! or $d3 over (sz $1 and if $1 invand then opc! op,
      else over _<>cx!, single! $d3 c, c, then then ;
: <<, $4 shift, ;
: >>, $5 over (signed? if $2 + then shift, ;
: lrot, $0 shift, ;
: rrot, $1 shift, ;

: normalize!
  dup (&? if dup (dir? if
    dup >>3 b2:0 over b2:0 <<3 or
    swap $3f invand or -dir)
  then then ;
: ?src>CX,
  dup (&? not if dup -dir) cx reg! @,
  $0 cx modrm! $f $1c lshift invand HALDIRECT or then ;
: _, c, dup dst@ single! dup modrm, hslot@ ;
: *,
  dup HALIMM and if
    dup hslot@ $ff invand if $69 _, , else $6b _, c, then
  else
    dup dup (sz $3 and over (dir? or if ?src>CX, then
    $af opc! normalize! op0f,
    dup (dir? if cx reg! @, else drop then
  then ;

: ?immDI! dup HALIMM and if $bf c, dup hslot@ , HALIMM invand $c7 or then ;
: ?swapAX, dup $38 and if dup >>3 b2:0 $90 or c, $0 reg! then ;
: __
  dup ?swapAX,
  $f630 or dup (signed? if
    $99 ( CDQ ) c, $8 or op,
    else $d231 ( dx dx xor, ) w, op, then
  ?swapAX, drop ;
: _
  deref! ?immDI! dup $7 and $2 - if
    __ else $d189 ( cx dx mov, ) w, $1 - __ then ;
: /mod, dup (dir? if -dir) dup @!, dup _ @!, else _ then ;

( cx dx mov,
  di bx xchg,
  si ax xchg, )
: pre $fb87d189 , $96 c, ?di+, ?si+, ;
( si ax xchg,
  di dx xchg, )
: post ?di-, ?si-, $96 c, $fb87 w, ;
( rep, movs, )
: move, pre $a5f3 w, post ;
: wmove, pre $f3 c, $a566 w, post ;
: cmove, pre $a4f3 w, post ;

( cx dx mov,
  di dx xchg,
  si ax xchg,
  ax ax cmp, (set Z for zero guard) )
: pre $fb87d189 , $96 c, ?di+, ?si+, $c039 w, ;
: post $96 c, $fb87 w, ;
( repz, cmps, )
: []=, pre $a7f3 w, post ;
: w[]=, pre $f3 c, $a766 w, post ;
: c[]=, pre $a6f3 w, post ;

( cx dx mov,
  di bx mov, )
: pre $df89d189 , ?di+, ;
( jnz(
  cx inc,
  dx cx sub,
  ax ax cmp, )
: post $0675 w, $c1ff w, $c039ca29 , ;
( repnz, scas, )
: idx, pre $aff2 w, post ;
: widx, pre $f2 c, $af66 w, post ;
: cidx, pre $aef2 w, post ;

( di bx xchg,
  cx dx mov, )
: pre $fb87d189 , ?di+, ;
( di bx xchg, )
: post ?di-, $fb87 w, ;
( rep, stos, )
: fill, pre $abf3 w, post ;
: wfill, pre $f3 c, $ab66 w, post ;
: cfill, pre $aaf3 w, post ;
