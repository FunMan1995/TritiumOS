needs com/link
unit drv/nic/loop

1500 const MTU
create frame MTU allot
0 value recvsz

: >frame frame dup to frameptr to payloadptr ;
: :sendframe ( link -- ) drop payloadsz to recvsz ;
: :beginframe ( link -- ) drop 0 to recvsz >frame MTU to payloadsz ;
: :readframe ( link -- f ) drop >frame doto recvsz 0 | dup to payloadsz bool ;

' drop ' :sendframe ' drop ' :beginframe ' :readframe newlink const loopback
