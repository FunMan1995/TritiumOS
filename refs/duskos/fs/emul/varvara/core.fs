needs emul/uxn
unit emul/varvara/core

create varvarahandlers $100 4* allot0
: setdeo ( w port -- ) 4* varvarahandlers + ! ;
: callvector ( pc -- ) uxn + runat drop ;
: [callvector] [compile] [dev2@] compile callvector ; immediate
: ?callvector ?dup if callvector then ;
: [?callvector] [compile] [dev2@] compile ?callvector ; immediate
0 value curnkc \ set in "continue?", used in controller and console
