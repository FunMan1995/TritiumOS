$50 const ARCH
:~ ( sec dst blk -- ) drop $fffffff4 !+ ! ;
: :readblk ~ 0 $fffffffc c! ;
: :writeblk ~ 1 $fffffffc c! ;
' :readblk ' :writeblk 512 -1 newblk const myblk
myblk newfatfs bootfs!
:~ "init.fs" loadpath quit ; ~
