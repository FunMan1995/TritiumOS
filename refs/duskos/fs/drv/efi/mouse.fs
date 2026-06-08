needs lib/struct lib/ival drv/efi drv/efi/devpath io/mouse
unit drv/efi/mouse

0 value mousecnt
0 value mouses

efiguid simplepointerguid
  87 8c 87 31 75 0b d5 11 9a 4f 00 90 27 3f c1 4d

:~ simplepointerguid LocateProtocol ?dup not ?err ( a u )
   dup q/ to mousecnt here# to mouses cmoveallot ; ~
: mouse@ ( mouseid -- a64 ) q* mouses + aq@ ;
: .mouse ( mouseid -- ) dup . .": " 1 1 rot mouse@ .devpath nl> ;
: lsmouse mousecnt 0 do i .mouse loop ;
: mouseprotocol ( mouseid -- a64 )
  simplepointerguid swap mouse@ HandleProtocol# ;

: Reset ( extverif mouseid -- )
  argstart swap arg1! mouseprotocol arg0k! [q+@] 0 0 efiexec# ;

create state 16 allot
state absvalmap { uint relx rely relz ; uchar leftbtn rightbtn ; }

: GetState ( mouseid -- ?dx ?dy ?flags f )
  argstart state absaddr arg1!
  mouseprotocol arg0k! [q+@] 0 1 efiexec
  if 0 else relx rely leftbtn rightbtn 2* or 1 then ;

extends Mouse struct EFIMouse { uint mouseid ; }

:> r! mouseid GetState if r@ to buttons r@ moveby then rdrop ;
: newefimouse ( mouseid -- mouse )
  0 over Reset [ litn ] newmouse swap , ;
