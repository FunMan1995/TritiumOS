needs io/blk drv/efi drv/efi/devpath drv/efi/image
unit drv/efi/blkio

0 value blkiocnt
0 value blkios \ 64-bit handles

efiguid blkioguid
  21 5b 4e 96 59 64 d2 11 8e 39 00 a0 c9 69 72 3b

:~ blkioguid LocateProtocol ?dup not ?err ( a u )
   dup q/ to blkiocnt here# to blkios cmoveallot ; ~

: bootblkidx ( -- blkidx )
  BootDeviceHandle blkios blkiocnt q* 4/ idx not ?err qsz 4/ / ;
: blkio@ ( blkidx -- a64 ) q* blkios + aq@ ;
: .blkio ( blkidx -- ) dup . .": " 1 1 rot blkio@ .devpath nl> ;

: lsblk ( -- ) blkiocnt 0 do i .blkio loop ;

: blkprotocol ( blkid -- a64 ) blkioguid swap blkio@ HandleProtocol# ;
: blkmedia ( blkid -- a64 ) blkprotocol [q+@] 8 0 ;
: blkmediaid blkmedia q@ 0 drop ;

$200 const BLKSZ
16 alignheren BLKSZ allot@ const buf
: Reset ( extverif blkid -- )
  argstart swap arg1! blkprotocol arg0k! [q+@] 8 1 efiexec ?err ;

\ ReadBlocks/WriteBlocks are weird in the sense that the LBA argument is 64 bit
\ and, under IA32, uses up two arg slots. The simplest solution I could find to
\ this tricky discrepancy is this list of aliases. Remember, with argstart,
\ everything must happen as the same RS level.
alias drop lbahi!
alias arg3! blksz!
alias arg4! bufaddr!
1 q* 4 = [if]
alias arg3! lbahi!
alias arg4! blksz!
alias arg5! bufaddr!
[then]
: ReadBlock ( sec dst blkid -- )
  argstart dup blkmediaid arg1! buf absaddr bufaddr! rot arg2! 0 lbahi!
  BLKSZ blksz! ( dst blkid )
  blkprotocol arg0k! [q+@] 8 2 efiexec# ( dst )
  buf swap BLKSZ 4/ move ;
: WriteBlock ( sec src blkid -- )
  swap buf BLKSZ 4/ move ( sec blkid )
  argstart dup blkmediaid arg1! buf absaddr bufaddr! swap arg2! 0 lbahi!
  BLKSZ blksz! ( blkid )
  blkprotocol arg0k! [q+@] 8 3 efiexec# ;
: FlushBlocks ( blkid -- )
  argstart blkprotocol arg0k! [q+@] 8 4 efiexec# ;

extends Blk struct EFIBlk { uint blkid ; }

:> blkid r! WriteBlock r> FlushBlocks ;
:> blkid ReadBlock ;
: newefiblk ( blkid -- drv )
  0 over Reset [ litn litn ] BLKSZ -1 newblk swap , ;
