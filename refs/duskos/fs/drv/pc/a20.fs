needs asm/x86
unit drv/pc/a20

\ Is A20 line enabled? We're using boot sector's $aa55 signature as reference
: a20? $7dfe @ $107dfe @ <> ;

code _wait pc al $64 imm) in, al 2 imm) test, abs>rel jnz, ret,
code _wait2 pc al $64 imm) in, al 1 imm) test, abs>rel jz, ret,
: _cmd ( n -- ) ['] _wait abs>rel call, al swap imm) mov, al $64 imm) out, ;
code _unlockThroughPS2
  ax push, cli, $ad _cmd $d0 _cmd
  ' _wait2 abs>rel call,
  al $60 imm) in, ax push,
  $d1 _cmd
  ' _wait abs>rel call,
  ax pop, al 2 imm) or, al $60 imm) out,
  $ae _cmd
  ' _wait abs>rel call,
  sti, ax pop, ret,

32 $400 $400 * * const MEMTOP \ 32 MB ought to be enough for anybody

: a20$
  a20? not if _unlockThroughPS2 then
  a20? not if abort"A20 line can't be activated" then
  $100000 HERE ! MEMTOP HEREMAX ! ;
