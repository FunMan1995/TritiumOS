needs drv/pc/fdc
unit bench/fdc

$cafebabe const FILLER
create mysec SECSZ allot
create dstsec SECSZ allot

0 value sec

: dummysec ( secno -- ) mysec SECSZ 4/ FILLER fill mysec ! ;

: rwsec ( sec -- )
  ."Testing R/W on cyl/head/sec " cyl . spc> head . spc> dup .
  dup to sec dummysec
  .. $45 ( MFM+WRITE ) sec rwcmd
  .. mysec SECSZ writefifo
  .. $46 ( MFM+READ ) sec rwcmd
  .. dstsec SECSZ readfifo
  nl> mysec dstsec SECSZ c[]= not ?abort"rwsec data discrepancy" ;

: rwtrk ( -- ) FDSECPERTRK 0 do i 1+ rwsec loop ;
: rwall ( -- ) FDCYLCNT 0 do i 0 seek# rwtrk i 1 seek# rwtrk loop ;

: formatfloppy ( -- )
  FDCYLCNT 0 do FDHEADS 0 do
    j i seek#
    ."Formatting cyl/head " cyl . spc> head . .. nl>
    formattrack loop loop ;
