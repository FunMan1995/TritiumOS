needs hal/instr io/stream
unit num/crc

code reflect ( n -- n )
  W) &) A>) @, 0 i) @, 32 i) S>) @, begin
    1 i) <<,
    1 A) &) &nf, ifnz, 1 i) |, then
    1 i) A>) >>,
    1 i) S>) -, ?brnz,
  exit,

: crc, ( poly -- )
  >r \ V1=poly
  PSP) ^, nip, 8 i) S>) @, [compile] begin \ counter in S
    A) &) !, 1 i) >>,
    1 i) A>) &, ifnz, r> ( poly ) i) ^, [compile] then
    1 i) S>) -, ?brnz, ;

: crcrev, ( shift poly -- )
  >r r! ?dup if i) <<, then \ V1=poly V2=shift
  PSP) ^, nip, 8 i) S>) @, [compile] begin \ counter in S
    A) &) !, 1 i) <<,
    $80 r> ( shift ) lshift i) A>) &,
    ifnz, r> ( poly ) i) ^, [compile] then
    1 i) S>) -, ?brnz, ;

code acc32 ( crc c -- crc ) $04c11db7 reflect crc, exit,
code acc16 ( crc c -- crc ) 8 $1021 crcrev, $ffff i) &, exit,
sysalias crcword) CRCWORD crc
: crc32$ ( -- init ) ['] acc32 CRCWORD ! -1 ;
: crc16$ ( -- init ) ['] acc16 CRCWORD ! 0 ;

: crc[] ( init a u -- crc )
  swap >r 0 do ( n ) doto V1 dup 1+ | c@ crc loop rdrop ;
: crc32[] crc32$ rot> crc[] inv ;
: crc16[] crc16$ rot> crc[] $ffff and ;

: crc ( stream init -- crc )
  swap >r begin ( crc ) \ V1=file
    -1 V1 readbuf ?dup while ( crc a u ) crc[] repeat ( crc )
  rdrop ;
: crc16 crc16$ crc ;
: crc32 crc32$ crc ;

: crc<< ( init -- crc ) word openpath swap crc ;
: crc32<< crc32$ crc<< ;
: crc16<< crc16$ crc<< ;
