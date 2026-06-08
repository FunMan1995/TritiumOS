needs xcomp/tools io/stream io/blk fs/fatt
unit xcomp/i386/pc/mkfat

: setupMBR ( blk -- )
  ."Putting MBR in place\n"
  "xcomp/i386/pc/mbr.fs" loadpath
  >r $3e r@ seek
  xcomp[] r@ write# r> flush ;

: setupPBR ( blk -- )
  ."Putting PBR in place\n"
  "xcomp/i386/pc/pbr.fs" loadpath
  >r $3e r@ seek
  xcomp[] r@ write# r> flush ;

fatopts$
4 to secperclus
120 to rsvdsec

: doit ( drv -- )
  dup newFAT setupMBR ;

: doitpbr ( drv -- )
  dup newFAT setupPBR ;

: dofloppy ( drv -- )
  1 to secperclus
  18 to secpertrk
  2 to numheads
  0 to drvnum
  doit ;
