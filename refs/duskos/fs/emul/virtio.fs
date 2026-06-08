needs lib/wordtbl fs/core io/kbd emul/cpu
unit emul/virtio

extends Stream struct VirtIO {
  uint cpu cmdptr out storage secsz secptr shouldexit ;
}

: mem cpu mem( ;

: err abort"VirtIO error" ;
: # not if err then ;
: hostcmd' ( virtio -- a ) cmdptr 4+ ;
: cansend? ( virtio -- f ) hostcmd' c@ not ;
: _readbuf out readbuf ;
wordtbl[ ( arg virtio -- )
  :> out putc ;
  :> tuck mem + swap to secptr ;
  :> to secsz ;
  : _seek ( secno virtio -- ) tuck secsz * swap storage dup # seek ;
  :> tuck _seek tri secptr | secsz | storage read# ;
  :> tuck _seek tri secptr | secsz | storage write# ;
  :> 1 swap to shouldexit drop ;
]wordtbl cmds

: process ( virtio -- )
  r! cmdptr c@ ?dup if ( cmdidx ) \ V1=virtio
    r@ cmdptr 1+ le@ $ffffff and r@ ( idx arg virtio )
    rot cmds swap 1- wexec ( )
    0 r> cmdptr !
    else rdrop then ;

: runvm ( virtio -- )
  begin dup cpu step dup process dup cpu halted? until drop ;

: _writebuf ( a n virtio -- written-n )
  >r r! 0 do ( a V1=vio V2=n )
    V1 shouldexit if break then
    begin V1 cansend? not while V1 runvm repeat
    c@+ 1 V1 hostcmd' c!+ c! loop ( a )
  drop r> r> runvm ;

: enter ( virtio -- )
  0 over to shouldexit
  begin dup cpu run key over putc dup shouldexit until drop ;

: newvirtio ( storage out cmd cpu -- virtio )
  ['] _readbuf ['] _writebuf newstream >r \ V1=virtio
  ( cpu ) , ( cmd ) , ( out ) , ( storage ) , 512 , 0 , 0 ,
  r@ cmdptr 8 0 cfill r> ;
