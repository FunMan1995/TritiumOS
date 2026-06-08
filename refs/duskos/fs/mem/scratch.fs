needs mem/alloc
unit mem/scratch

: newscratchpad ( size -- )
  here# over allot ( size a )
  here# >r dup litn over litn exit, ( size a )
  swap r> rot> newallocator ;
