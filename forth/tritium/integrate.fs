\ tritium-integrate stub — TritiumOS.txt §5a.2 / Phase 8 / docs/INTEGRATE.md
\ Words: tritium-integrate  tritium-integrate-demo
\ Host/CLI SoT does real scaffold under evolve/integrate/; Forth mirrors markers + slot gate.

variable integrate-slots-used   0 integrate-slots-used !
variable integrate-max-slots   10 integrate-max-slots !
variable integrate-demo-force   0 integrate-demo-force !
variable integrate-last-ok      0 integrate-last-ok !

: integrate-free-slot? ( -- flag )
  integrate-slots-used @ integrate-max-slots @ < ;

\ Refuse when 10/10 (slot 11) — cite §5a.4 / LICENSE.md
: integrate-require-slot ( -- flag )
  integrate-free-slot? if -1 else
    ." [tritium-integrate] refuse — no free license slot (§5a.4)" cr
    ." [tritium-integrate] refuse — LICENSE.md / TritiumOS.txt §5a.4 (10/10 or slot 11)" cr
    0
  then ;

\ Platform name is fixed in Forth smoke (host/CLI take <platform> arg).
: tritium-integrate ( -- flag )
  integrate-require-slot dup 0= if
    0 integrate-last-ok !
    exit
  then drop
  ." [tritium-integrate] platform=stub from=_template" cr
  ." [tritium-integrate] scaffold → evolve/integrate/stub/ (§5a.2 / Phase 8)" cr
  integrate-slots-used @ 1+ integrate-slots-used !
  ." [tritium-integrate] OK — scaffolded stub" cr
  -1 integrate-last-ok !
  -1 ;

: tritium-integrate-demo ( -- )
  ." [tritium-integrate-demo] force free-slot path → platform=demo (§5a.2 / Phase 8)" cr
  0 integrate-slots-used !          \ force free-slot path
  -1 integrate-demo-force !
  integrate-require-slot 0= if
    ." [tritium-integrate-demo] FAIL" cr
    0 integrate-demo-force !
    exit
  then drop
  ." [tritium-integrate] platform=demo from=_template" cr
  ." [tritium-integrate] scaffold → evolve/integrate/demo/ (§5a.2 / Phase 8)" cr
  integrate-slots-used @ 1+ integrate-slots-used !
  ." [tritium-integrate] OK — scaffolded demo" cr
  ." [tritium-integrate-demo] OK" cr
  -1 integrate-last-ok !
  0 integrate-demo-force ! ;
