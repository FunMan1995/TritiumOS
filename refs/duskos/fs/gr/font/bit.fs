needs hal/instr hal/vreg lib/struct lib/wordtbl \
      gr/color gr/buf gr/rdwr gr/font
unit gr/font/bit

extends Font struct BitFont { uint pixbuf ; }

code pre ( g font -- linesz src dst )
  PSP) S>) @, W) offsetof Font.height +) S>) *, ( g font S=topY )
  W) offsetof pixbuf +) A>) @, dup, S) &) @, yaddr, ( g font src )
  swap, W) offsetof tgtbuf +) A>) @, A) offsetof linesz +) S>) @,
  A) offsetof flipY +) testz, ifnz, 0 i) S>) swap-, then
  PSP) 4 +) S>) !, ( linesz src font A=tgtbuf )
  W) offsetof tgtx +) S>) @, W) offsetof tgty +) @, xyaddr, ( linesz src dst )
  exit,

: TODO abort"unsupported depth" ;

code wr1l ( g font -- )
  RSP) -!, pushlr, ' pre execute, poplr, \ R+0=font
  3 inv i) &, swap, dup, \ P+0=src P+4=dst P+8=linesz W=junk
  RSP) @, W) offsetof Font.bgcolor +) R0>) @, 1 i) R0>) <<,
  W) offsetof Font.fgcolor +) R0>) |, 30 i) R0>) <<, \ R0=bg/fg
  W) offsetof tgtx +) S>) @, RSP) S>) !,
  W) offsetof Font.maxwidth +) S>) @, RSP) S>) -!,
  W) offsetof Font.height +) @, RSP) -!, \ R+8=tgtx R+4=width R+0=height
  begin
    RSP) 4 +) @, R2) !, \ R2=xcnt
    PSP) A>) @, A) A>) @, 4 PSP) +n, \ A=input buf P+0=src+
    PSP) 4 +) @, R3) !, \ R3=dstaddr
    RSP) 8 +) R1>) @, W) ?trim1l, \ R1=idx S=output buf
    begin
      R0) &) @, 1 A) &) &nf, ifnz, 1 i) <<, then $80000000 i) &,
      1 i) S>) >>, W) &) S>) |, \ S=modified output
      1 i) A>) >>, \ A=modified input
      1 i) R1>) +, $1f i) R1>) &, ifz,
        R3) @, W) S>) !+, R3) !, then
      -1 R2) +n, ?brnz,
    $1f i) R1>) &, ifnz,
      R3) A>) @, A) @, mix1l, A) S>) !, then
    PSP) 4 +) @, PSP) 8 +) +, R3) !, PSP) 4 +) !, \ P+4=R3=dst+
    -1 RSP) +n, ?brnz,
  12 ps+, drop, 12 rs+, exit,

code wr2h ( g font -- )
  RSP) -!, pushlr, ' pre execute, poplr, \ R+0=font
  3 inv i) &, swap, dup, \ P+0=src P+4=dst P+8=linesz W=junk
  RSP) @, W) offsetof Font.fgcolor +) R0>) @, 2 i) R0>) <<,
  W) offsetof Font.bgcolor +) R0>) |, \ R0=bg/fg
  W) offsetof tgtx +) S>) @, RSP) S>) !,
  W) offsetof Font.maxwidth +) S>) @, RSP) S>) -!,
  W) offsetof Font.height +) @, RSP) -!, \ R+8=tgtx R+4=width R+0=height
  begin
    RSP) 4 +) @, R2) !, \ R2=xcnt
    PSP) A>) @, A) A>) @, 4 PSP) +n, \ A=input buf P+0=src+
    PSP) 4 +) @, R3) !, \ R3=dstaddr
    RSP) 8 +) R1>) @, W) ?trim2h, \ R1=idx S=output buf
    begin
      R0) &) @, 1 A) &) &nf, ifnz, 2 i) >>, then 3 i) &,
      2 i) S>) <<, W) &) S>) |, \ S=modified output
      1 i) A>) >>, \ A=modified input
      1 i) R1>) +, $f i) R1>) &, ifz,
        R3) @, W) S>) !+, R3) !, then
      -1 R2) +n, ?brnz,
    $f i) R1>) &, ifnz,
      R3) A>) @, A) @, mix2h, A) S>) !, then
    PSP) 4 +) @, PSP) 8 +) +, R3) !, PSP) 4 +) !, \ P+4=R3=dst+
    -1 RSP) +n, ?brnz,
  12 ps+, drop, 12 rs+, exit,

: wr16/32, ( widthxt -- ) ( run: c font -- )
  >r RSP) -!, pushlr, ['] pre execute, poplr, \ R+0=font
  swap, RSP) A>) @, A) offsetof Font.bgcolor +) R0>) @,
  A) offsetof Font.fgcolor +) R1>) @,
  A) offsetof Font.maxwidth +) S>) @, RSP) S>) !, \ R+0=width
  A) offsetof Font.height +) S>) @, RSP) S>) -!, \ R+4=width R+0=height
  [compile] begin
    RSP) 4 +) S>) @, R2) S>) !,
    PSP) A>) @, W) S>) @,
    [compile] begin
      ( lsz dst src A=dst S=bits R0=bg R1=fg R2=xcnt )
      1 S) &) &nf, ifz,
        A) R0>) r@ execute !+, [compile] else
        A) R1>) r> execute !+, [compile] then
      1 i) S>) >>, -1 R2) +n, ?brnz,
    4 i) +, PSP) A>) @, PSP) 4 +) A>) +, PSP) A>) !,
    -1 RSP) +n, ?brnz,
  8 ps+, drop, 8 rs+, exit, ;

code wr16 ' 16b) wr16/32,
code wr32 ' noop wr16/32,

10 wordrefs writetbl wr1l TODO TODO wr2h TODO TODO TODO wr16 TODO wr32

: drawbitglyph ( c font -- )
  r! A! writetbl A> tgtbuf Pixbuf.depth depth>idx wexec
  r> fontinvalidate ;

: newbitfont ( w h numglyph -- font )
  rot> 2dup ['] drawbitglyph newfont 0 , >r ( num w h ) \ V1=font
  rot * 2>r 1BPPL PALETTE 2r> newpixbuf dup allotbuf r@ to pixbuf r> ;
