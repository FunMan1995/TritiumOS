needs drv/arm/sccp
unit drv/arm/cache

: icacheenabled? armctl@ $1000 and bool ;
: dcacheenabled? armctl@ 4 and bool ;
: writebufferenabled? armctl@ 8 and bool ;

: .enabled ( f -- ) if ."Enabled" else ."Disabled" then ;

\ b11=P b10=0 b9:6=size b5:3=assoc b2=M b1:0=len
: .cachelen ( n -- )
  ."LLen " 1 over 3 and 3 + lshift . spc> ( n )
  dup 4/ 1 and 2+ ( n mul )
  ."Assoc "
  over 3 rshift 7 and ?dup if
    1- over swap lshift . ."-way " else ."- " then ( n mul )
  ."Size "
  swap 6 rshift $f and 8+ lshift . ;

: .caches cachetype@ dup .x nl> ( n )
  ."ctype " dup 25 rshift $f and .x1 nl>
  dup $1000000 and if ."Not " then ."Unified" nl>
  ."DCache " dup 12 rshift $7ff and .cachelen spc>
  dcacheenabled? .enabled nl>
  ."ICache " $7ff and .cachelen spc>
  icacheenabled? .enabled nl>
  ."WriteBuffer " writebufferenabled? .enabled nl> ;

: enableicache armctl@ $1000 or armctl! ;
: enabledcache armctl@ 4 or armctl! ;
: enablewritebuffer armctl@ 8 or armctl! ;
: disableicache armctl@ $1000 invand armctl! ;
: disabledcache armctl@ 4 invand armctl! ;
: disablewritebuffer armctl@ 8 invand armctl! ;
