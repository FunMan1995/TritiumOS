: @+,
  dup $104 - if
    dup $106 - if
      dup @, dup (sz swap -dir) HALDIRECT or clrb +n,
      else $ad c, drop then
    else $58 c, drop then ;

: -@,
  dup $80104 - if
    $0 over (sz -
    over -dir) HALDIRECT or
    clrb +n, @,
    else $50 c, drop then ;

: popret PSP) dir) -@, RSP) @+, ; immediate
