needs hal/instr hal/vreg lib/ival lib/bit lib/psrs \
      gr/color gr/damage gr/blt gr/buf
unit gr/varvara

4BPPL PALETTE $2a8 $172 newpixbuf const vscreen

:> ( pb -- )
  dup computelinesz 4+ over to linesz
  dupbi linesz | height * over bufsz > if
    alignhere 0 , allotbuf else drop then ;
vscreen to ?allotbuf
vscreen ?allotbuf

variable vdamage

here# 16 4* allot0 vscreen to Pixbuf.palette

0 value sprite
0 value X
0 value Y

: setvpalette ( c0 c1 c2 c3 -- )
  vscreen Pixbuf.palette A! >r \ V1=a A=a
  dup A> 12 + ! A> 48 + !
  dup A> 8+ ! A> 32 + !
  dup A> 4+ ! A> 16 + !
  A> ! ( )
  V1 4+ dup 16 + 3 move
  V1 4+ dup 32 + 3 move
  r> 4+ dup 48 + 3 move ;

$ff6622 >screencolor
$77ddbb >screencolor
$000000 >screencolor
$ffffff >screencolor setvpalette

\ 16=transparent
create blends map< c,
  16 0 1 2  0 1 2 3   0 2 3 1   0 3 1 2 \
  1 0 1 2   16 1 2 3  1 2 3 1   1 3 1 2 \
  2 0 1 2   2 1 2 3   16 2 3 1  2 3 1 2 \
  3 0 1 2   3 1 2 3   3 2 3 1   16 3 1 2

: flipX) $10 or ;
: flipY) $20 or ;
: fg) $40 or ;
: bg) $40 invand ;
: 2bpp) $80 or ;
: 1bpp) $80 invand ;
: fill) $80 or ;
: X+) $100 or ;
: Y+) $200 or ;
: addr+) $400 or ;
: times) ( opts n -- opts ) 1- 12 lshift or ;

: updatedamage ( -- ) \ R0=x R1=y A=w S=h
  [ dup, -12 ps+, PSP) 8 +) R0>) !, PSP) 4 +) R1>) !,
  PSP) A>) !, S) &) @,
  ' damage execute, dup, vdamage m) @,
  ' damage+ execute, vdamage m) !, drop, ] ;

code updatedamagexy ( -- ) \ S=w/h
  A) &) S>) !, addrof X m) R0>) @, addrof Y m) R1>) @, ' updatedamage bbr,

: ?autoinc, ( n -- ) \ W=opts
  >r $100 W) &) &nf, ifnz,
    $10 W) &) &nf, ifnz, r@ neg addrof X m) +n,
    [compile] else r@ addrof X m) +n, [compile] then [compile] then
  $200 W) &) &nf, ifnz,
    $20 W) &) &nf, ifnz, r@ neg addrof Y m) +n,
    [compile] else r> addrof Y m) +n, [compile] then [compile] then ;

: opts) PSP) 12 +) ;
: ycnt) PSP) 8 +) ;
: srca) PSP) 4 +) ;
: srcdiff) PSP) ;

: setmask, \ R2=pixmask R1=clobbered
  $33333333 i) R1>) @,
  $40 opts) &nf, ifnz, 2 i) R1>) <<, [compile] then
  R2) R1>) !, ;

: writeStoR0,
  R0) R1>) @, R2) R1>) &,
  R1) &) S>) |, R0) S>) !, setmask, 0 i) R1>) @, ;

: 8pix, ( -- )
  8 0 do
    R3) @,
    1 7 i - 8+ lshift A) &) &nf, ifnz, 16 i) >>, [compile] then
    1 7 i - lshift A) &) &nf, ifnz, 8 i) >>, [compile] then
    $f0 W) &) &nf, ifnz, \ transparent
      $f i) @, 2 i) R1>) <<, R1) &) <<, 2 i) R1>) >>, R2) dir) |,
      0 i) @, [compile] then
    $f i) &, wr4l,
    1 i) R1>) +, 7 i) R1>) &, ifz,
      writeStoR0, 0 i) S>) @, 4 i) R0>) +, [compile] then
  loop ;

: ?swapbits,
  $10 opts) &nf, ifnz, PSP) S>) -!, swapbits8, PSP) S>) @+, [compile] then ;

: postlude, 12 ps+, drop, exit, ;

code inner ( opts -- opts )
  dup, 8 i) @, ( opts ycnt )
  dup, addrof sprite m) @, dup, 1 i) @, ( opts ycnt srca srcdiff )
  $20 PSP) 8 +) &nf, ifnz, 7 PSP) +n, -1 i) @, then
  dup, ( opts ycnt srca srcdiff junk )
  vscreen i) A>) @, addrof Y m) @,
  0 i) signed) <) if,
    -8 i) >) if,
      ycnt) dir) +, srcdiff) *, srca) dir) -, 0 i) @, then then
  A) offsetof Pixbuf.height +) S>) @, 8 i) S>) -, S) &) >) if,
    W) &) S>) swap-, 8 i) S>) <) if,
      ycnt) dir) S>) -, then then
  yaddr, 0 i) =) if, postlude, then
  addrof X m) S>) @, A) offsetof Pixbuf.width +) R0>) @,
  8 i) R0>) +, 8 i) S>) +,
  R0) &) S>) >=) if, postlude, then
  8 i) S>) -, 1 i) S>) signed) >>, S) &) +, 3 inv i) &,
  R0) &) !, ( opts ycnt srca srcdiff junk R0=dstaddr )
  opts) @, $f i) &, 2 i) <<, blends i) +, W) ale@,
  $40 opts) &nf, ifz, 2 i) <<, then R3) !, \ R3=blend
  setmask, begin
    srca) A>) @, 0 i) @, \ A=srca
    $80 opts) &nf, ifnz, A) 8 +) 8b) @, ?swapbits, 8 i) <<, then
    A) 8b) |, srcdiff) A>) +, srca) A>) !,
    ?swapbits,
    A) &) !, \ A=input buffer
    addrof X m) R1>) @, R0) ?trim4l, \ S=output buffer R1=dstidx
    8pix,
    7 i) R1>) &, ifnz,
      -1 i) @, 2 i) R1>) <<, R1) &) <<, R2) dir) |,
      32 i) R1>) swap-, R1) &) S>) >>,
      writeStoR0, then
    vscreen i) @, W) offsetof linesz +) @, 4 i) -, W) &) R0>) +,
    -1 ycnt) +n, ?brnz,
  postlude,

: once inner [ 8 i) S>) @, ] updatedamagexy [ 8 ?autoinc, ] drop ;

code drawsprite ( opts -- )
  $f000 W) &) &nf, ' once ?brz,
  $0300 W) &) &nf, ' once ?brz,
  $1000 i) +,
  pushlr, addrof X m) A>) @, RSP) A>) -!, addrof Y m) A>) @, RSP) A>) -!, begin
    ' inner execute,
    8 i) S>) @, ' updatedamagexy execute,
    $100 W) &) &nf, ifnz, 8 addrof Y m) +n, then
    $200 W) &) &nf, ifnz, 8 addrof X m) +n, then
    $400 W) &) &nf, ifnz,
      8 addrof sprite m) +n,
      $80 W) &) &nf, ifnz, 8 addrof sprite m) +n, then then
    $1000 i) -, $f000 W) &) &nf, ?brnz,
  RSP) A>) @+, addrof Y m) A>) !, RSP) A>) @+, addrof X m) A>) !,
  8 ?autoinc, drop, popexit,

: writeStoA,
  A) R0>) @,
  $40 PSP) 8 +) &nf, ifz,
    $33333333 i) R0>) &, [compile] else $cccccccc i) R0>) &, [compile] then
  R0) &) S>) |, A) S>) !+, 0 i) S>) @, ;

: fillpixel ( opts -- )
  $80 invand
  dup $10 and if X 1+ 0 else vscreen Pixbuf.width X then ( opts xmax xmin )
  2dup = if 2drop drop exit then
  [ vscreen i) A>) @,
    A) offsetof Pixbuf.height +) S>) @, A) offsetof Pixbuf.width +) A>) @,
    0 i) R0>) @, 0 i) R1>) @, ] updatedamage
  oover $20 and if Y 1+ 0 else vscreen Pixbuf.height Y then ( ... ymax ymin )
  do ( opts xmax xmin ) [
    dup, S) &) !, RSP) @, vscreen i) A>) @, xyaddr, 3 inv i) &, A) &) !,
    PSP) R1>) @, R3) R1>) !, A) ?trim4l, \ S=buf A=a R1=idx
    PSP) 8 +) @, $40 W) &) &nf, ifz, 2 i) <<, then $f i) &, R2) !, \ R2=color
    begin
      R2) @, wr4l, 1 R3) +n, R3) R1>) @,
      PSP) 4 +) R1>) <>) if, ( loopbr exitbr )
      7 i) R1>) &, ifz, writeStoA, then swap bbr, then ( )
    7 i) R1>) &, ifnz, A) @, mix4l, then writeStoA, drop,
    ] loop 2drop drop ;

code drawpixel ( opts -- )
  vscreen i) A>) @, \ A=pb
  addrof Y m) S>) @, A) offsetof Pixbuf.height +) S>) >=) if, drop, exit, then
  addrof X m) S>) @, A) offsetof Pixbuf.width +) S>) >=) if, drop, exit, then
  $80 W) &) &nf, ' fillpixel ?brnz,
  dup, addrof Y m) @, xyaddr, 3 inv i) &, ( opts a S=x )
  PSP) S>) @, 3 i) S>) &, 3 i) R0>) @, \ S=color R0=mask
  addrof X m) R1>) @, 7 i) R1>) &, 2 i) R1>) <<, \ R1=shift
  $40 PSP) &nf, ifz, 2 i) R1>) +, then
  R1) &) S>) <<, R1) &) R0>) <<, W) A>) @, \ A=8pix
  -1 i) R0>) ^, R0) &) A>) &, S) &) A>) |,
  W) A>) !, drop,
  pushlr, 1 i) S>) @, ' updatedamagexy execute, poplr,
  1 ?autoinc, drop, exit,

: refreshscreen
  0 vdamage @! ?dup if
    1 vscreen to flipY
    damagerect vscreen fliprect
    dupbi width | height screen centerrect ( x y w h src x y dst )
    [ PSP) 24 +) S>) @, PSP) 4 +) dir) S>) +,
      PSP) 20 +) S>) @, PSP) dir) S>) +, ]
    movepixels
    0 vscreen to flipY then ;
