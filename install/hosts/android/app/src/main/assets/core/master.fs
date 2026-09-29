\ tritium-master stub — TritiumOS.txt §5a.5 / docs/MASTER.md
\ Words: master-mint-license  master-mint-worker  master-verify  master-demo
\ Host/CLI SoT does real GUID mint + format check; Forth mirrors markers (§5a.5).
\ No real crypto / master-root / fleet sync.

variable master-slots-default   10 master-slots-default !
variable master-last-ok         0 master-last-ok !
variable master-verify-flag     0 master-verify-flag !

\ Scaffold license key print (fixed demo hex; host/CLI use random)
: master-mint-license ( slots -- )
  dup 0<= if drop master-slots-default @ then
  ." [master] mint-license slots=" . ." key=TRIT-0A1B2C3D4E5F6789-DRACO" cr
  -1 master-last-ok ! ;

\ device-id as cell; 0 = empty → refuse
: master-mint-worker ( device-id -- )
  dup 0= if
    drop
    ." [master] mint-worker refuse empty id (§5a.5 scaffold)" cr
    0 master-last-ok !
    exit
  then
  ." [master] mint-worker device=" . ." key=TRIT-W-FEEDFACECAFEBEEF-DRACO" cr
  -1 master-last-ok ! ;

\ flag: -1 = format-shaped scaffold accept (host does real check); 0 = refuse
\ Forth smoke: non-zero cell = accept path; 0 = malformed
: master-verify ( shaped? -- flag )
  if
    ." [master] verify OK format-only (§5a.5 scaffold)" cr
    -1 master-verify-flag !
    -1
  else
    ." [master] verify FAIL format-only (§5a.5 scaffold)" cr
    0 master-verify-flag !
    0
  then ;

: master-demo ( -- )
  master-slots-default @ master-mint-license
  1 master-mint-worker          \ non-empty device-id
  -1 master-verify drop         \ shaped license
  -1 master-verify drop         \ shaped worker
  0 master-verify drop          \ malformed → FAIL line expected
  ." [master-demo] OK" cr
  -1 master-last-ok ! ;
