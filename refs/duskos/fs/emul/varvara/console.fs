needs lib/str io/stream emul/uxn emul/varvara/core
unit emul/varvara/console

0 value uxnstream

:> [dev@] $18 emit ; $18 setdeo
:> [dev@] $19 emit ; $19 setdeo

: checkargs ( -- ) eol? not [dev!] $17 ;
: handlearguments ( -- )
  [dev2@] $10 not eol? or if exit then
  begin
    in< 2 over ws? + over LF = + [dev!] $17 ( c )
    dup [dev!] $12 [callvector] $10 LF = until ;
: ?handleconsole ( -- )
  [dev2@] $10 not if exit then
  uxnstream bool [devk!] $17 if
    uxnstream getc [devk!] $12
    EOF = if 0 to uxnstream 0 [dev2!] $10 exit then
    [callvector] $10
  \ It's weird to check for controller vector, but the official implementation
  \ works very differently in graphical and CLI mode. This condition recreates
  \ this behavior, that is: if we have a controller vector, it has the keyboard
  \ to the expense of the console.
  else [dev2@] $80 not if doto curnkc 0 | keyboard nkc>char ?dup if
      [dev!] $12 [callvector] $10 then then then ;
