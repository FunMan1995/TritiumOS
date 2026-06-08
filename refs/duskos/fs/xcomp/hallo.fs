: if ifW, ; immediate
: fbr, here dup $200 + bbr, ;
: ahead fbr, ; immediate
: then here br! ; immediate
: else fbr, swap here br! ; immediate
: begin here ; immediate
: while ifW, swap ; immediate
: repeat bbr, here br! ; immediate
: again bbr, ; immediate
: ?read) word dup c@ $1 - swap $1 + c@ $29 - or ;
: ( begin ?read) while repeat ; immediate

: (&? HALDIRECT and bool ;
: -&) HALDIRECT invand ;
: dir) HALINV or ;
: (dir? HALINV and bool ;
: -dir) HALINV invand ;
: signed) HALSIGNED or ;
: (signed? HALSIGNED and bool ;
: -signed) HALSIGNED invand ;
: hslot@ (slot hbank@ ;
: hslot! hbank! slot) ;
