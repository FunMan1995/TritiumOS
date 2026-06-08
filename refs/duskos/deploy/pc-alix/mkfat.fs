\ This is a copy of xcomp/i386/mkfat with $ff numheads added
( tgtdrv -- )
needs xcomp/tools fs/fatt

const tgtdrv

."Compiling MBR\n"
f<< /xcomp/i386/pc/mbr.fs
here org - value mbrlen
org value mbr

: setupMBR ( drv -- )
  ."Putting MBR in place\n"
  0 here tgtdrv readsector
  mbr here $3e + mbrlen cmove
  0 here tgtdrv writesector ;

fatopts$
4 to secperclus
120 to rsvdsec
$ff to numheads \ $10 doesn't work, it seems we're always in LBA mode.
tgtdrv newFAT
tgtdrv setupMBR
