needs io/kbd
unit drv/efi/kbd

create map map< c,
  0 ArrowUp ArrowDown ArrowRight ArrowLeft Home End Insert \
  Delete PageUp PageDown F1 F2 F3 F4 F5 \
  F6 F7 F8 F9 F10 F11 F12 ESC

: ?specialkey ( inputkey -- inputkey-or-nkc f )
  dup c@ 1- $17 < if c@ map + c@ 1 else 0 then ;
: inputkey>nkc ( inputkey -- nkc )
  ?specialkey not if 2+ c@ Passthrough or then ;
:> ( kbd -- ?nkc event-type ) drop ReadKeyStroke dup if inputkey>nkc BOTH then ;
: newefikbd [ litn ] newkbd ;
