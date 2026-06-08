: @+, dup @, dup (sz swap -dir) HALDIRECT or clrb +n, ;

: -@, $0 over (sz -
      over -dir) HALDIRECT or
      clrb +n, @, ;

: popret PSP) dir) -@, $e8294858 , ; immediate
