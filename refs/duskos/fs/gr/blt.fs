needs hal/vreg lib/struct mem/kv gr/buf gr/rdwr
unit gr/blt

: x) PSP) 28 +) ;
: y) PSP) 24 +) ;
: dx) PSP) 20 +) ;
: dy) PSP) 16 +) ;
: w) PSP) 12 +) ;
: h) PSP) 8 +) ;
: dstpb) PSP) 4 +) ;
: srcpb) PSP) ;

\ Subroutines signature
( x y dx dy w h dstbp srcpb -- same )
code blt4l>2h
  dup, W) offsetof palette +) @, R2) !, \ R2=palette
  -16 rs+, 0 i) @, RSP) !, begin \ R+0=yidx R+4=R+8=R+12=junk
  ( x y dx dy w h dst src junk )
    dx) S>) @, dy) @, RSP) +, dstpb) A>) @, xyaddr,
    3 inv i) &, RSP) 4 +) !, \ R+4=dsta
    x) S>) @, y) @, RSP) +, srcpb) A>) @, xyaddr,
    3 inv i) &, W) A>) @+, RSP) 8 +) !, \ A=input buffer R+8=srca
    x) R1>) @, ?skip4l, R3) R1>) !, \ R3=srcidx
    dx) R1>) @, RSP) 4 +) @, W) ?trim2h, \ R1=dstidx S=output buffer
    w) @, RSP) 12 +) !, begin \ R+12=xcnt
      rd4l, \ W=palette index
      2 i) <<, R2) +, W) @, \ W=color
      wr2h, \ W=junk
      1 i) R1>) +, $f i) R1>) &, ifz,
        RSP) 4 +) @, W) S>) !+, RSP) 4 +) !, then
      1 R3) +n, 7 R3) &nf, ifz,
        RSP) 8 +) @, W) A>) @+, RSP) 8 +) !, then
      -1 RSP) 12 +) +n, ?brnz,
    $f i) R1>) &, ifnz,
      RSP) 4 +) A>) @, A) @, mix2h, A) S>) !, then
    1 RSP) +n, RSP) @, h) <) ?br, 16 rs+,
  drop, exit,

: blt4l>16/32, ( widthxt -- )
  >r dup, W) offsetof palette +) @, R2) !, \ R2=palette
  0 i) @, RSP) -!, [compile] begin
  ( x y dx dy w h dst src junk )
    dx) S>) @, dy) @, RSP) +, dstpb) A>) @, xyaddr, R0) &) !, \ R0=dsta
    x) S>) @, y) @, RSP) +, srcpb) A>) @, xyaddr, 3 inv i) &, S) &) !,
    S) A>) @+, \ A=input buffer S=srca
    x) R1>) @, ?skip4l, \ R1=srcidx
    w) @, R3) !, [compile] begin \ R3=xcnt
      rd4l, \ W=palette index
      2 i) <<, R2) +, W) @, \ W=color
      1 i) R1>) +, 7 i) R1>) &, ifz, S) A>) @+, [compile] then
      R0) r> execute !+, -1 R3) +n, ?brnz,
    1 RSP) +n, RSP) @, h) <) ?br, 4 rs+,
  drop, exit, ;

code blt4l>16 ' 16b) blt4l>16/32,
code blt4l>32 ' noop blt4l>16/32,

: p ( depth depth -- n ) 16 lshift or ;
kvtbl[
  4BPPL 2BPPH p ' blt4l>2h
  4BPPL 16BPP p ' blt4l>16
  4BPPL 32BPP p ' blt4l>32
]kvtbl mytbl

: movepixels ( x y w h src x y dst -- )
  2>r >r r! bounds ( x y w h V1=dy V2=dst V3=dx V4=src )
  V3 rot> V1 rot> V2 bounds ( x y dx dy w h )
  dup not if [ 20 ps+, drop, 20 rs+, ] exit then
  r> rdrop r> rdrop swap ( x y dx dy w h dst src )
  over depth over depth swap p mytbl swap ?kvexec
  not ?abort"TODO: unsupported encoding pair in movepixels"
  drop invalidate 2drop ;
