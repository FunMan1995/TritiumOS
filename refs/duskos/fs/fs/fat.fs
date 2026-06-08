needs lib/struct lib/str lib/ival lib/time io/stream io/blk io/part fs/core
unit fs/fat

addrof curfs offsetof walkcontext ivalmapfrom {
  uint widx wcl _pad _pad ;
  uint fatpart lastfreecluster ;
  [void,0] fatheader ;
}
addrof curfs offsetof fatheader ivalmapfrom {
  +$0b leshort fatsecsz ;
  +$0d uchar   secpercluster ;
  +$0e leshort reservedseccnt ;
  +$10 uchar   FATcnt ;
  +$11 leshort rootentcnt ;
  +$13 leshort fatseccnt ;
  +$16 leshort FATsz ;
}

$20 const EXTRASZ

: fatstorage! to curfs dup to storage fatpart to tgtblk ;
: fatflush storage flush fatpart flush ;

$ffff const EOC

\ These words have the same sig: ( -- n )
: RootDirSectors rootentcnt 32 * fatsecsz /+ ;
: totsec FATcnt FATsz * ;
: FirstRootDirSecNum totsec reservedseccnt + ;
: RootDirOff FirstRootDirSecNum fatsecsz * ;
: FirstDataSector FirstRootDirSecNum RootDirSectors + ;
: DataSec FirstDataSector fatseccnt swap- ;
: CountOfClusters DataSec secpercluster / ;
: ClusterSize secpercluster fatsecsz * ;
: FAT12? CountOfClusters 4085 < ;
: MaxClusters FAT12? if 341 else 256 then fatsecsz * FATsz * ;

: FAT12@ ( cluster -- entry )
  fatpart over dup 2/ + ( cl blk offset )
  over seek bi getc | getc ( cl lsb msb )
  256* or ( cl entry )
  swap 1 and if 16/ else $fff and then ;
: FAT12! ( entry cluster -- )
  fatpart over dup 2/ + rot 1 and if ( e blk off )
    over seek r! getc ( e n )
    $f and swap 16* or ( e )
    r@ rewind1 dup r@ putc ( e )
    256/ r> putc
  else ( e blk off )
    over seek >r dup r@ putc ( e )
    r@ getc ( e n )
    $f0 and swap 256/ $f and or ( e )
    r@ rewind1 r> putc then ;

:~ ( cl -- fatpart ) 2* fatpart tuck seek ;
: FAT16@ ( cl -- entry ) ~ bi getc | getc 256* or ;
: FAT16! ( entry cl -- ) ~ 2dup putc dip 256/ | putc ;
: FAT@ ( cl -- entry )
  dup 2 < if drop EOC else FAT12? if FAT12@ else FAT16@ then then ;
: FAT! ( entry cl -- ) FAT12? if FAT12! else FAT16! then ;

: EOC? ( cl -- f ) FAT12? if $ff8 else $fff8 then tuck and = ;

: cl# ( n -- ) not ?abort"cluster out of range" ;
: clusterpos ( cl -- off )
  dup MaxClusters < cl#
  2- secpercluster * FirstDataSector + fatsecsz * ;
: zerocluster ( cl -- )
  clusterpos ClusterSize begin ?dup while ( off n )
    over 1 storage window dup cl# ( off n a u )
    >r over r> min tuck 0 cfill ( off n minn )
    tuck - rot> + swap repeat drop ;
\ find a free cluster in the FAT
: findfreecluster ( -- cl )
  lastfreecluster begin 1+ dup FAT@ not until ( cl )
  dup to lastfreecluster ;
\ Find a free cluster, and mark it as EOC.
: allocatecluster ( -- cl ) findfreecluster EOC over FAT! ;
\ Allocate a free cluster and fill its contents with zeroes
: allocatecluster0 ( -- cl ) allocatecluster dup zerocluster ;

\ Get next FAT entry and if it's EOC, allocate a new one
: FAT@+ ( cl -- entry )
  dup FAT@ ( cl ncl ) dup EOC? if
    drop allocatecluster ( cl ncl ) tuck swap FAT! ( cl ncl )
    else nip then ;

\ Directory logic
consts 32 DIRENTRYSZ 11 NAMESZ 8 EXTIDX 3 EXTSZ $02 ATTR_HIDDEN $10 ATTR_DIR

\ Last iterated entry
create dirent DIRENTRYSZ allot
dirent absvalmap {
  [uchar,NAMESZ] ename ;
  uchar eattr ;
  +22 leint emtime ;
  +26 leshort ecluster ;
  +28 leint efilesize ;
}

: lastentry? ename c@ not ;
: valid? ename c@ bi bool | $e5 <> and ;
: dir? eattr ATTR_DIR and bool ;
: hidden? eattr ATTR_HIDDEN and bool ;
: iterable? valid? hidden? not and ename c@ '.' <> and ;
: makedir $10 to eattr ;

: fatts>ts ( ts -- ts )
  dup 25 rshift 1980 + 2000 max to year
  dup 21 rshift $f and 1 max 12 min to month
  dup 16 rshift $1f and 1 max 31 min to day
  dup 11 rshift $1f and 23 min to hour
  dup 5 rshift $3f and 59 min to minute
  $3f and 2* 59 min to second compose ;

: ts>fatts ( ts -- ts )
  decompose second 2/
  minute 5 lshift or
  hour 11 lshift or
  day 16 lshift or
  month 21 lshift or
  year 1980 - 25 lshift or ;

: entryname! ( name -- )
  ename NAMESZ SPC cfill
  dup "." s= over ".." s= or if
    c@+ ( a len ) ename swap cmove exit then
  ename swap c@+ 0 do ( dst a )
    c@+ dup '.' = if
      drop nip ename EXTIDX + else upcase rot c!+ then ( a dst+1 )
    dup ename - NAMESZ = if break then swap loop 2drop ;

: entryname ( -- name )
  SPC ename tuck EXTIDX cidx not if EXTIDX then ( src u )
  newstr swap cmove+ ( dst )
  SPC ename EXTIDX + tuck EXTSZ cidx not if EXTSZ then ( dst src u )
  ?dup if rot '.' swap c!+ swap cmove+ else drop then ( dst )
  endstr ;

: DirentPerCluster ClusterSize DIRENTRYSZ / ;
: woff ( -- off-or-0 )
  wcl if
    widx DirentPerCluster >= if
      wcl FAT@ dup EOC? if
        drop else to wcl doto widx DirentPerCluster - | then then
    widx DirentPerCluster < if
      widx DIRENTRYSZ * wcl clusterpos + else 0 then
  else
    widx rootentcnt < if widx DIRENTRYSZ * RootDirOff + else 0 then then ;
: ewindow# ( wr? -- a )
  woff dup not ?abort"bad ewindow offset"
  swap storage window DIRENTRYSZ < ?abort"bad FAT entry id" ;
: readentry ( -- ) 0 ewindow# dirent DIRENTRYSZ cmove ;
: writeentry ( -- ) 1 ewindow# dirent swap DIRENTRYSZ cmove ;

: iterentry ( -- f ) doto widx 1+ | woff bool dup if readentry then ;

: :gotoroot ( -- ) -1 to widx 0 to wcl ;

: setwalkinfo ( -- )
  entryname walkname strmove
  dir? to walkdir?
  efilesize to walksize
  emtime fatts>ts to walkmtime ;

: :gotonext ( -- f )
  begin
    iterentry while
    readentry iterable? if setwalkinfo 1 exit then
    lastentry? not while repeat then 0 ;

: :enterdir ( -- )
  wcl 0 = widx -1 = and if exit then
  walkdir# walkpathcat
  readentry ecluster to wcl -1 to widx ;

$e5 const DIRFREE
\ find free dir entry in current walked cluster
: findfreedirentry ( -- )
  -1 to widx begin
    iterentry while
    dirent c@ bi DIRFREE <> | bool and while
    repeat else
    \ nothing found, we have to extend the chain
    allocatecluster0 wcl FAT! then ;

: createdirentry ( cl -- )
  doto wcl swap | >r doto widx 0 | >r ( V1=parentcl V2=parentidx )
  readentry
  ename NAMESZ SPC cfill '.' ename c! makedir
  wcl to ecluster writeentry
  doto widx 1+ |
  '.' ename 1+ c!
  V1 to ecluster writeentry
  r> to widx r> to wcl
  fatflush readentry ;

: :addfsnode ( name dir? -- )
  findfreedirentry
  dirent DIRENTRYSZ 0 cfill
  if makedir allocatecluster0 to ecluster then
  entryname!
  now ts>fatts to emtime
  setwalkinfo writeentry
  ecluster ?dup if createdirentry then ;

\ TODO: deallocate the chain before clearing the entry
: :removefsnode ( -- ) readentry DIRFREE dirent c! writeentry ;

: :writefsnode
  walkname entryname!
  walksize to efilesize
  walkmtime ts>fatts to emtime
  writeentry ;

extends Stream struct FATFile {
  uint cl0 clidx curcl adirty fcl fidx fat ;
}

: filewindow ( wr? file -- ?a n )
  r! fat doto curfs swap | >r ( wr? V1=file V2=oldfs )
  V1 pos ClusterSize /mod ( wr? subpos clidx )
  V1 clidx 2dup = if 2drop else ( wr? subpos tgtidx curidx )
    over V1 to clidx 2dup > if
      - V1 curcl else drop V1 cl0 then ( wr? subpos loopn cl )
    swap 0 do FAT@ loop V1 to curcl then ( wr? subpos )
  V1 curcl clusterpos ( wr? subpos off )
  + swap storage window
  r> to curfs rdrop ;
: :readbuf ( n file -- a? n )
  tuck maxn min ( file n )
  dup not if nip else ( file n )
    over 0 swap filewindow ?dup not ?ioerr rot min ( file a n )
    rot incposk then ;
: :writebuf ( buf n file -- n )
  r! pos over + r@ ?grow ( buf n V1=file )
  1 r@ filewindow dup if ( buf n a n )
    rot min r! cmove ( V2=written-n )
    r> r@ incposk
    else ( buf n 0 ) nip nip then
  1 r> to adirty ;

: :flush ( file -- ) fat doto curfs swap | fatflush to curfs ;
: :close ( file -- )
  r! fat doto curfs swap | >r ( V1=file V2=oldfs )
  V1 doto adirty 0 | if
    V1 fcl to wcl V1 fidx to widx
    readentry
    V1 size to efilesize
    V1 cl0 to ecluster
    now ts>fatts to emtime
    writeentry then
  fatflush
  r> to curfs r> closecursor ;

: grow ( file -- ) \ entry for file is already read
  r! fat doto curfs swap | >r ( V1=file V2=oldfs )
  \ special case: if cluster0 is zero, we have an empty file. We need to
  \ update its direntry to record the file's first cluster.
  V1 cl0 ?dup not if
    allocatecluster
    dup V1 to curcl dup V1 to cl0 then ( cluster0 )
  V1 size ClusterSize / ( cl0 n )
  0 do ( cluster ) FAT@+ loop ( cluster ) drop
  r> to curfs rdrop ;

\ TODO: deallocate truncated FATs if appropriate
: :resize ( sz file -- )
  2dup size = if 2drop exit then
  A! size over A> to size ( sz oldsz )
  A> rot> > if dup grow then ( file )
  dupbi pos | seek ( file )
  1 swap to adirty ;

: :initfilestruct ( -- )
  ['] :readbuf ['] :writebuf newstream
  ['] :resize over to resize
  ['] :close over to close
  ['] :flush swap to flush
  curfs 0 0 0 0 0 0 7 n,@ drop ;

: :openfile ( file -- )
  readentry
  >A wcl A> to fcl
  widx A> to fidx
  efilesize A> to Stream.size
  ecluster dup A> to cl0 A> to curcl
  0 A> to clidx 0 A> to pos 0 A> to adirty ;

: newfatfs ( blk -- fs )
  >r ['] :writefsnode ['] :removefsnode ['] :addfsnode
  ['] :openfile ['] :initfilestruct
  ['] :enterdir ['] :gotonext ['] :gotoroot
  1 r> newfs to curfs
  EXTRASZ allot0
  0 fatheader storage readblk
  \ Verify that the header makes sense
  fatsecsz storage blksz <> ?abort"invalid FAT sector size" ( )
  1 to lastfreecluster
  reservedseccnt FATsz storage newpart to fatpart
  curfs ;
