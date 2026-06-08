needs fs/core mem/stack io/search
unit fs/search

$10 newstack const dirids \ FSIDs of parent dirs
bootfs value fs
0 value searchstr

: .name ( fsid -- ) fs info nip nip stype ;
: .dirs ( -- ) dirids stack[] 0 do @+ .name ."/" loop drop ;
: infile? ( fsid -- f ) fs open searchstr over search swap close ;
:~ ( fsid -- )
  dup fs info 2drop if
    dup dirids push
    fs children 0 do ~ loop
    dirids pop drop
    else dup infile? if .dirs .name nl> else drop then then ;
: search ( str fsid fs -- ) dirids empty to fs swap to searchstr ~ ;
