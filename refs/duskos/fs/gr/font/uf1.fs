needs lib/bit io/stream fs/core gr/font gr/font/bit
unit gr/font/uf1

create buf $900 allot

: loaduf1 ( path -- font )
  openpath >r buf $900 r@ read# r> close ( )
  8 8 256 newbitfont ( font )
  buf over widths $100 cmove
  r! pixbuf Pixbuf.buffer ( dst ) \ V1=font
  buf $100 + swap 256 0 do 8 0 do ( src dst )
    over 7 + i - c@ swapbits8 over ! 4+ loop swap 8+ swap loop
  2drop r> ;

".uf1" current addfontloader