needs lib/bit io/stream fs/core gr/font gr/font/bit gr/font/uf1
unit gr/font/uf2

create buf $2100 allot

: cmax[] ( a u -- n ) 0 rot> do[] i c@ max loop ;
: uf2width ( -- n ) buf $100 cmax[] ;
: uf2height ( -- n )
  buf $100 + 'g' 32 * + 8+ 8 0 rot> cidx if 8+ else 16 then ;

: loaduf2 ( path -- font )
  openpath >r buf $2100 r@ read# r> close ( )
  uf2width uf2height 256 newbitfont ( font )
  buf over widths $100 cmove
  r! pixbuf Pixbuf.buffer ( dst ) \ V1=font
  256 0 do ( dst )
    i 32 * buf + $100 + swap V1 Font.height 0 do ( src dst )
      over V1 Font.height + i - ( src dst src+ )
      dup c@ swapbits8 ( src dst src+ row16 )
      swap 16 + c@ swapbits8 8 lshift or ( src dst row16 )
      over ! 4+ loop ( src dst )
    nip loop
  drop r> ;

".uf2" current addfontloader