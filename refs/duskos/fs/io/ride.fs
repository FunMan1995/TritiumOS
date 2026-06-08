needs lib/str io/kbd io/stream mem/range
unit io/ride

0 value autoLF
0 value stop
4 const EOT

: spliton ( a u c -- ?a ?u a u f )
  oover oover cidx if split[] 1 consume[] 1 else 0 then ;
: contype ( a u -- )
  EOT spliton if 2drop 1 to stop then
  autoLF not if rtype exit then
  CR spliton if 2>r rtype LF emit CR emit 2r> contype else rtype then ;
$11 const QUITKEY \ CTRL+Q
: ride ( stream -- )
  0 to stop begin
    -1 over readbuf ?dup if contype promptforkey else idle then
    ?key if dup QUITKEY = if drop 1 to stop else over putc then then
    stop until
  drop ."The ride is over!\n" ;
