needs io/kbd emul/uxn emul/varvara/core
unit emul/varvara/ctrl

\ When computing the ASCII char to send down, we ignore ctrl/alt/gui
$ff03ffff const NKCMASK

: ?handlecontroller ( -- )
  curnkc NKCMASK and ?dup if
    keyboard nkc>char dup $1b = if [devk!] $0f then ( key )
    [dev!] $83 [dev2@] $80 ?callvector 0 [dev!] $83 then
  0 ArrowRight keyboard pressed? if $80 or then ( buttons )
  ArrowLeft keyboard pressed? if $40 or then
  ArrowDown keyboard pressed? if $20 or then
  ArrowUp keyboard pressed? if $10 or then ( buttons )
  keyboard mods tuck RShift and if $08 or then ( mods buttons )
  over LShift and if $04 or then
  over Alt and if $02 or then
  swap Control and if $01 or then ( button )
  [dev@] $82 over <> if
    [dev!] $82 [dev2@] $80 ?callvector else drop then ;
