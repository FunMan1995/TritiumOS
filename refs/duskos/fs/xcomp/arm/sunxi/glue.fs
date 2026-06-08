:> 0 do c@+ uart! loop drop ; console!
SMHC0 newsmhcdrive newfatfs bootfs!

needs io/kbd
:> 2drop uart@? dup if drop c[] then ;
' ioerr
newstream newstreamkbd to keyboard

needs app/prompt lib/diag
prompt$

needs drv/sunxi/timer
sunxitimer$

$100000 HEREMAX +!
."Dusk OS\n" .free
quit
