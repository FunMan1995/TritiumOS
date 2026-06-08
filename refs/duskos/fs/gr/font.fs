needs lib/struct lib/str fs/core mem/ll gr/buf
unit gr/font

struct Font {
  uint maxwidth height tgtbuf tgtx tgty bgcolor fgcolor ;
  xt (drawglyph) ;
  [uchar,$100] widths ;
}

: fonttarget! ( x y pixbuf font -- ) A! to tgtbuf A> to tgty A> to tgtx ;
: fontcolors! ( bg fg font -- ) A! to fgcolor A> to bgcolor ;

: fontinvalidate ( font -- )
  A! tgtx A> tgty A> maxwidth A> height A> tgtbuf invalidate ;

: newfont ( width height draw -- font )
  here# >r rot , swap , 0 , 0 , 0 , 0 , 0 , , $100 allot0 r> ;

: glyphwidth, A) offsetof widths +) &) +, W) 8b) @, ;
code glyphwidth A) &) !, drop, glyphwidth, exit,

: drawglyph 2dup (drawglyph) tuck glyphwidth swap doto tgtx + | ;

variable loaders  \ LL of: +0 xt +4 ext string
variable registry \ LL of: +0 font +4 name

: addfontloader ( ext xt -- ) loaders lladd , s, ;
: registerfont ( name font -- ) registry lladd , s, ;

: findfont ( name -- font-or-0 )
  registry @ begin dup while ( name ll )
    2dup 8+ s= not while @ repeat
    ( name ll ) 4+ @ then ( name font-or-0 ) nip ;

: _loadfont ( name ll -- font-or-0 )
  over "data/font/" swap strcat ( ll path )
  dup lookup if swap 4+ @ execute tuck registerfont else 2drop 0 then ;
: getfont ( name -- font-or-0 )
  dup findfont ?dup if nip exit then
  loaders @ begin dup while ( name ll )
    2dup 8+ swap endswith? not while ( name ll ) @ repeat
    _loadfont else ( path 0 ) nip then ;
: getfont# getfont dup not ?abort"font not found" ;
