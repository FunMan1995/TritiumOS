variable sec
variable buf

:~ ( -- a u ) 1 sec +! sec @ buf @ bootsec@ buf @ BOOTSECSZ ;
: secload ( fromsec -- )
  here# buf ! BOOTSECSZ allot
  1- sec ! ['] ~ NEXTIN< ! 0 INSZ ! ;
