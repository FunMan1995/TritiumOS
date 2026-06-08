needs drv/riscv/uart
$10000000 uart$
:> do[] i c@ uart! loop ; console!

\ Bump memory to 16mb
here $1000000 + HEREMAX !

:> $5555 $100000 ! ; ' bye realias
: reboot $7777 $100000 ! ;

needs io/kbd
:> 2drop uart@? dup if drop c[] then ;
' ioerr
newstream newstreamkbd to keyboard

needs lib/diag app/prompt
prompt$
."Dusk OS\n" .free
