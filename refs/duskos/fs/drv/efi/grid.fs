needs drv/efi io/grid
unit drv/efi/grid


: mode@ ( -- a64 ) ConOut [q+@] 0 9 ;
: nummodes mode@ q@ noq ;
: curmode mode@ [q+@] 4 0 noq ;

qvariable rows
qvariable cols
: QueryMode ( modenum -- ?rows cols-or-0 )
  argstart arg1! cols absaddr arg2! rows absaddr arg3!
  ConOut arg0k! [q+@] 0 3 efiexec if 0 else rows @ cols @ then ;

: SetMode ( modenum -- ) argstart arg1! ConOut arg0k! [q+@] 0 4 efiexec# ;
: SetAttribute ( attr -- ) argstart arg1! ConOut arg0k! [q+@] 0 5 efiexec# ;

: SetCursorPosition ( row col -- )
  argstart arg1! arg2! ConOut arg0k! [q+@] 0 7 efiexec# ;

\ Some firmware yield an error here. We just ignore it. It will just mean we
\ have a "glitchy" grid where the cursor moves a lot.
: EnableCursor ( f -- ) argstart arg1! ConOut arg0k! [q+@] 0 8 efiexec drop ;

: .conmodes ( -- )
  nummodes 0 do
    i QueryMode ?dup if i . .": " . ."x" . nl> then loop ;

create _ 4 allot0
: _cell! ( color c pos -- )
  dup grid size 1- >= if 2drop drop exit then
  0 EnableCursor
  COLS /mod swap SetCursorPosition ( col c )
  swap $7f and SetAttribute ( c )
  _ c! _ absaddr OutputString ;
\ : _cursor! ( pos -- ) COLS /mod swap SetCursorPosition 1 EnableCursor ;

: efigrid$ ( -- ) curmode QueryMode swap grid resize ;
: refreshgrid ( -- )
  0 begin
    grid getnextchange while ( color char pos )
    r! _cell! r> repeat ;
: refreshcaret ( -- )
  refreshgrid grid posxy swap SetCursorPosition 1 EnableCursor ;

: conmode! ( modenum -- ) SetMode efigrid$ ;
