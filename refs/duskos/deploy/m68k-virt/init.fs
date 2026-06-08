:> 0 do c@+ $fffffffe c! loop drop ; console!
: bye 0 $ffffffff c! ;
$200000 HEREMAX ! \ we have 2 MB on that machine

needs io/kbd
:> 2drop $fffffffd c@ c[] ;
' ioerr
newstream newstreamkbd to keyboard

needs lib/diag app/prompt
prompt$
."Dusk OS\n" .free
