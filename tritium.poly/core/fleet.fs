\ tritium-fleet stub — TritiumOS.txt §§5a.2–5a.4 / docs/FLEET.md
\ Words: fleet-export  fleet-import  fleet-demo
\ Host/CLI SoT does real evolve/fleet/ I/O; Forth mirrors markers + same-key gate.
\ No network / encryption / auto-sync / CRDT / LINEOS branding.

variable fleet-same-key       -1 fleet-same-key !   \ -1 = match; 0 = mismatch
variable fleet-last-ok         0 fleet-last-ok !
variable fleet-context-ok     -1 fleet-context-ok ! \ scaffold: context present

\ Stamp fingerprint from current license context; write evolve/fleet/ markers
: fleet-export ( -- )
  fleet-context-ok @ 0= if
    ." [fleet] FAIL — no license context" cr
    0 fleet-last-ok !
    exit
  then
  ." [fleet] export → evolve/fleet/ key=TRIT-FLEETSTUB00001-DRACO" cr
  -1 fleet-last-ok ! ;

\ Same-key check; refuse on mismatch (§5a.4)
: fleet-import ( -- flag )
  fleet-same-key @ if
    ." [fleet] import OK same-key" cr
    -1 fleet-last-ok !
    -1
  else
    ." [fleet] refuse — key mismatch (§5a.4)" cr
    0 fleet-last-ok !
    0
  then ;

: fleet-demo ( -- )
  -1 fleet-context-ok !
  -1 fleet-same-key !          \ same-key path
  fleet-export
  fleet-import drop
  0 fleet-same-key !           \ wrong fingerprint path
  fleet-import drop            \ expect refuse marker
  ." [fleet-demo] OK" cr
  -1 fleet-last-ok !
  -1 fleet-same-key ! ;
