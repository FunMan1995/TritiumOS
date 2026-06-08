needs lib/type lib/str
unit lib/struct

$14 adder szfield
$18 adder structfields
: structsz szfield @ ;
: struct? typeszxt ['] structsz = ;
:~ ( ll -- )
  dup @ ?dup if ~ .", " then
  dup entryname[] rtype spc> e>xt dup 4+ @ ."+" . spc> @ .type ;
variable lvl
: .struct
  lvl @ if drop ."{...}" exit then
  ."{" structfields @ ?dup if 1 lvl +! ~ -1 lvl +! then ."}" ;
: newstructure ( extends -- type )
  >r ['] @, ['] structsz ['] .struct 3 newtype ( type V1=extends )
  r> dup if dup , szfield 8 cmoveallot else , 0 , 0 , then ;

variable cur
variable ext
NULLSTR value curstructname
: cur# cur @ ?dup not ?abort"no active struct" ;
: curoff cur# szfield ;
: extends run1 ext ! ;
: containsstruct? ( s tgt -- f )
  swap begin ?dup while 2dup <> while reftype repeat drop 1 else 0 then nip ;
: findfield structfields find dup if @+ swap @ 1 then ;
: ?placeholder ( name -- f )
  findtype ?dup if dup cur ! bi struct? | typesz not and else 0 then ;
: _struct
  word dup to curstructname ?placeholder if exit then
  repeatword 0 ext @! newstructure dup cur ! addtype ;
: addfield ( type -- field )
  cur# structfields word entry ( type )
  dup typesz curoff dup @ rot> +! ( type off )
  here rot , swap , ;
: addalignedfield ( type -- field ) curoff @ over typealign curoff ! addfield ;
create _ leint , leshort , beint , beshort ,
: ?typeal# ( type -- type )
  curoff @ over case
    array? of r@ reftype typesz 1 max align# endof
    struct? of 4 align# endof
    _ 4 idx of 2drop endof
    typesz 1 max align# endcase ;
: genprefixedalias
  curstructname "." strcat CURWORD @ strcat NEXTWORD ! sysdict entryalias ;
: fieldsel, repeatword getset, genprefixedalias ;
: addr, @+ W) swap type) swap @ +) ;
: do!, PSP) S>) @+, dip S>) | type!, drop, ;
: fieldt ( type -- )
  ?typeal# dup addfield >r ( type V1=field )
  dup ['] do!, bind>
  swap ['] type@, bind>
  r@ ['] addr, bind>
  r> n"FILD" fieldsel, ;

: offsetof
  word sysdict findentry ?wnf
  dup entrytag bi n"FILD" <> | n"IVAL" <> and ?abort"not a FILD"
  scryentry# 4+ @
  COMPILING @ if litn then ; immediate

: ?} ( -- f ) toword# in< dup '\' = if drop [compile] \ ?} else '}' = then ;
: ?+ ( -- ) in< '+' <> if stepback else n< curoff ! then ;
: parsefields ( -- )
  toword# in< '{' <> ?abort"{ expected"
  begin ?} not while stepback
    ?+ type< begin word ";" s= not while repeatword dup fieldt repeat
    drop repeat ;

: struct _struct parsefields 0 cur ! ;
