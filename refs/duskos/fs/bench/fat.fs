needs io/stream fs/fat lib/diag
unit bench/fat

: .fatentry ( id fat -- )
  over .x spc> r! readentry ( V1=fat )
  ename NAMESZ rtype spc> eattr .x1 spc> efilesize . nl>
  ecluster begin dup V1 EOC? not while dup .x2 spc> V1 FAT@ repeat
  nl> rdrop ;

create buf 512 allot
: .cluster ( cl fat -- )
  r! clusterpos V1 storage seek 
  V1 ClusterSize 512 / 0 do
    buf 512 V1 storage read# buf 512 dumpn loop
  rdrop ;