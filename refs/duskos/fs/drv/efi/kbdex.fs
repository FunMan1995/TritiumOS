needs drv/efi drv/efi/devpath io/kbd drv/efi/kbd
unit drv/efi/kbdex

0 value kbdexcnt
0 value kbdexs \ 64-bit handles

efiguid textinputexguid
  34 75 9e dd 62 77 98 46 8c 14 f5 85 17 a6 25 aa

:~ textinputexguid LocateProtocol ?dup not ?err ( a u )
   dup q/ to kbdexcnt here# to kbdexs cmoveallot ; ~
: bootkbd ( -- kbdid )
  SystemTable [q+@] $18 2 ( ConsoleInHandle )
  kbdexs kbdexcnt q* 4/ idx not ?err qsz 4/ / ;
: kbdex@ ( kbdid -- a64 ) q* kbdexs + aq@ ;
: .kbdex ( kbdid -- ) dup . .": " 1 1 rot kbdex@ .devpath nl> ;
: lskbdex kbdexcnt 0 do i .kbdex loop ;

: kbdexprotocol ( kbdid -- a64 )
  textinputexguid swap kbdex@ HandleProtocol# ;
: Reset ( extverif kbdid -- )
  argstart swap arg1! kbdexprotocol arg0k! [q+@] 0 0 efiexec# ;

create keydata $10 allot
: ReadKeyStrokeEx ( kbdid -- struct-or-0 )
  argstart keydata absaddr arg1!
  kbdexprotocol arg0k! [q+@] 0 1 efiexec
  if 0 else keydata then ;

: SetState ( state kbdid -- )
  swap keydata c! argstart keydata absaddr arg1! ( kbdid )
  kbdexprotocol arg0k! [q+@] 0 3 efiexec# ;

extends Keyboard struct EFIKeyboard { uint kbdid ; }

: ?mods! ( kbd -- )
  keydata 4+ @ dup $80000000 and not if 2drop exit then ( kbd shiftstate )
  bi $55 and 17 lshift | $aa and 15 lshift or ( kbd mods )
  swap to mods ;

:> ( kbd -- ?nkc event-type )
  r! kbdid ReadKeyStrokeEx r@ ?mods! ?dup if
    inputkey>nkc dup $ff and if r@ mods or BOTH else drop NONE then
    else NONE then rdrop ;
: newefikbdex ( kbdid -- kbd )
  $c0 ( EFI_KEY_STATE_EXPOSED ) over SetState
  [ litn ] newkbd swap , ;
