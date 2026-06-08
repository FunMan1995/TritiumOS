needs lib/ival io/blk
unit fs/fatt

create tmpl
( jmp )  $eb c, $3c c, $90 c, ( OEMName ) ,"DuskFAT " ( BytsPerSec ) 0 wle,
( SecPerClus ) 0 c, ( RsvdSecCnt ) 0 wle, ( NumFAT ) 1 c,
( RootEntCnt ) 0 wle, ( TotSec16 ) 0 wle, ( Media ) $f0 c,
( FATsz16 ) 0 wle, ( SecPerTrk ) $3f wle, ( NumHeads ) $0f wle,
( HiddenSec ) 0 le, ( TotSec32 ) 0 le, ( DrvNum ) $80 c, ( Reserved1 ) 0 c,
( BootSig ) $29 c, ( VolID ) 4 allot0 ( VolLabel ) ,"NONAME     "
( FilSysType ) ,"FAT     "

tmpl absvalmap {
  +13 uchar   secperclus ;
  +14 leshort rsvdsec ;
  +17 leshort rootentcnt ;
  +24 leshort secpertrk ;
  +26 leshort numheads ;
  +36 uchar   drvnum ;
}

: fatopts$ 1 to secperclus 1 to rsvdsec 64 to rootentcnt
           $3f to secpertrk 16 to numheads $80 to drvnum ;
fatopts$

32 const DIRENTRYSZ
0 value rootentsec \ number of sectors for root entries
: newFAT ( blk -- )
  dup blksz here# over 0 cfill ( blk blksz )
  rootentcnt DIRENTRYSZ * over /+ to rootentsec
  tmpl here $3e cmove
  here $0b + wle! ( blk )
  dup blkcnt dup here over $ffff > if
    $20 + ! else $13 + wle! then ( blk totsec )
  rsvdsec - rootentsec -
  secperclus / ( blk clusters )
  dup $ffff > ?abort"FAT32 not supported"
  dup 4085 < r! if 341 else 256 then / 1+ ( blk fatseccnt ) \ V1=fat12?
  r! here $16 + wle! ( blk ) \ V2=fatseccnt
  $aa55 here $1fe + wle!
  dup 0 here rot writeblk ( blk )
  \ header done. Now, zero-out all FAT and root dir entries
  here over blksz 0 cfill ( blk )
  rsvdsec 1+ ( blk sec )
  rootentsec r> ( fatseccnt ) + 1- 0 do ( blk sec )
    2dup here rot writeblk 1+ loop ( blk sec ) drop
  \ finally, initialize the first FAT sector
  r> ( fat12? ) if $fffff0 else $fffffff0 then here le!
  ( blk ) rsvdsec here rot writeblk ;
