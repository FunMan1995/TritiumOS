needs lib/str gr/buf gr/rdwr
unit tests/grh

create _ ,"0123456789abcdef"
: .1 $f and _ + c@ emit ;
: .pix ( pb -- )
  dup doto flipY 0 | 2>r \ V1=pb V2=flipY
  0 V1 height do V1 width 0 do i j 1- V1 pixel@ .1 loop nl> 1 -loop
  2r> swap to flipY ;
: .pixflip ( pb -- )
  r! height 0 do V1 width 0 do i j V1 pixel@ .1 loop nl> loop
  rdrop ;

: p< ( -- n )
  0 begin drop in< dup SPC > until
  c[] parsehex not ?abort"wrong pix" ;

: expectpix ( w h pb -- )
  >r 0 swap do dup 0 do ( w )
    i j 1- V1 pixel@ p< <> if
      V1 .pix abort"unexpected pixbuf\n" then
  loop 1 -loop drop rdrop ;

: expectpixflip ( w h pb -- )
  >r 0 do dup 0 do ( w )
    i j V1 pixel@ p< <> if
      V1 .pixflip abort"unexpected pixbuf\n" then
  loop loop drop rdrop ;
