needs mem/arena
unit comp/oberon/mem

newarena dup bindalloc[ parena[ dup bindrun1 parena1 bindrun1@ parena1@
newarena dup bindalloc[ tarena[ dup bindrun1 tarena1 bindrun1@ tarena1@

: obmemreserve tarena1 ensurenext parena1 ensurenext ;
: obmemclear tarena1 reset ;
