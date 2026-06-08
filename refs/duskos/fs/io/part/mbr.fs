needs lib/struct text/ts mem/kv
unit io/part/mbr
\ MBR partition table accessors

create SECBUF 512 allot0

struct PTblEntry {
  uchar     bootind ;
  [uchar,3] startchs ;
  uchar     systemid ;
  [uchar,3] endchs ;
  leint lbastart lbasize ;
}

$1be const P1OFFSET
PTblEntry typesz const ENTRYSZ

: partnum? ( pn -- f ) 1 4 within? ;
: mbrentry ( pn -- entry ) dup partnum? not ?abort"bad part num"
  1- ENTRYSZ * P1OFFSET + SECBUF + ;
: used? ( entry -- f ) dup systemid swap lbasize or bool ;

: _chs@ ( a -- c h s ) c@+ swap c@+ ( h a+2 cs )
  dup $3f and swap $c0 and 4* ( h a+2 s c_hi )
  rot c@ or rot> ;
: _chs! ( c h s a -- ) rot swap c!+ swap $3f and rot ( a+1 s* c )
  dup 4/ $c0 and rot or rot c!+ ( c a+2 ) swap $ff and swap c! ;
: startchs@ ( entry -- c h s ) startchs _chs@ ;
: startchs! ( c h s entry -- ) startchs _chs! ;
: endchs@ ( entry -- c h s ) endchs _chs@ ;
: endchs! ( c h s entry -- ) endchs _chs! ;
: .chs ( c h s -- ) rot ."C:" . swap ." H:" . ." S:" . ;

kvtbl[
  $00 :> ."<Empty>" ;
  $04 :> ."FAT16 <32MB" ;
  $05 :> ."Extended CHS" ;
  $06 :> ."FAT16 >32MB" ;
  $07 :> ."NTFS/exFAT" ;
  $0b :> ."FAT32 CHS" ;
  $0c :> ."FAT32 LBA" ;
  $0e :> ."FAT16 >32MB LBA" ;
  $0f :> ."Extended LBA" ;
  $39 :> ."Plan 9" ;
  $42 :> ."Win2K dyn part" ;
  $4c :> ."Oberon" ;
  $4f over
  $4d :> ."QNX" ;
  $4e over
  $52 :> ."CP/M" ;
  $82 :> ."Linux Swap" ;
  $83 :> ."Linux" ;
  $85 :> ."Linux Extended" ;
  $8e :> ."Linux LVM" ;
  $a5 :> ."FreeBSD" ;
  $a6 :> ."OpenBSD" ;
  $a8 :> ."macOS" ;
  $a9 :> ."NetBSD" ;
  $ab :> ."macOS boot" ;
  $be :> ."Solaris" ;
  $bf over
  $eb :> ."BeOS" ;
  $ee :> ."<Fake,GPT>" ; \ protective
  $ef :> ."EFI(ESP)" ; \ normally uses GPT though
  $fd :> ."FreeDOS" ;
]kvtbl sysidtbl

: .systemid ( entry --- )
  systemid sysidtbl swap ?kvexec ( f ) not if ."<Unknown>" then ;

: setboot ( pn-or-0 -- ) r! ( V1=pn-or-0 )
    if V1 mbrentry
      dup used? not ?abort"entry being set to bootable must be in use"
      $80 swap to bootind
    then
    5 1 do V1 i <> if 0 i mbrentry to bootind then loop rdrop ;

\ print a right-justified unsigned 32-bit integer
: .ur ( n -- ) formatdecu 10 over - nspcs rtype ;

: .mbrentry ( pn -- ) dup . .": "
  mbrentry dup used? not if ."<Empty>" drop exit then
  r! ( V1=entry )
  bootind if ."BOOT " else ."     " then
  ."start: " V1 lbastart .ur
  .", size: " V1 lbasize .ur
  .", type: " V1 .systemid
  rdrop ;

: .mbrentries ( -- ) 5 1 do i .mbrentry nl> loop ;

: bootind? ( bootind -- f ) $7f and not ;

: lookup ( lbastart lbasize -- pn-or-0 )
  >r >r \ V1=lbasize V2=lbastart
  5 1 do i mbrentry ( entry )
    dup used? if
      dup lbastart V2 = swap lbasize V1 = and if
        i break
      then
      else drop then
  loop
  broke? not if 0 then
  2rdrop ;

: partcreate ( lbastart lbasize systemid pn -- )
  mbrentry r! ( V1=entry )
  used? ?abort"part in use"
  V1 ENTRYSZ $ff cfill
  V1 to systemid
  V1 to lbasize
  V1 to lbastart
  0 V1 to bootind
  rdrop ;

: partdelete ( pn -- ) mbrentry ENTRYSZ 0 cfill ;

\ TODO visit partitions in ascending lbastart order

: validate# ( -- )
  \ TODO verify partitions nonoverlapping; needs ordered visit
  0 dup >r >r ( V1=entry, V2=boot-active count )
  5 1 do i mbrentry to V1
    V1 used? if
      V1 bootind dup bool V2 + to V2
      bootind? not ?abort"invalid bootind"
      V1 lbasize not ?abort"zero size used part"
      V1 startchs@ not ?abort"CHS start S<1" 2drop
      V1 endchs@ not ?abort"CHS end S<1" 2drop
    then
  loop
  V2 1 > ?abort"multiple boot active"
  SECBUF 510 + wle@ $aa55 <> ?abort"no boot magic"
  2rdrop ;
