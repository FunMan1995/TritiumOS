unit mem/ll

: llinsert ( elem ll -- ) over swap @! swap ! ;
code llcnt ( ll -- count )
  A) &) !, 0 i) @, begin
    0 i) A>) =) if, exit, then
    1 i) +, A) A>) @, again

code llitern ( n ll -- prev elem-or-0 )
  PSP) A>) @, 1 i) A>) +, begin \ W=ll A=n PSP+0=prev
    1 i) A>) -, ifz, exit, then
    PSP) !, W) @,
    0 i) =) if, exit, then again
