needs lib/ival io/mouse mem/roll drv/pc/ioport drv/pc/pic asm/x86
unit drv/pc/ps28042

$60 ioportb ps2data
$64 ioportb ps2cmd
$fa const ACK

\ Keyboard buffer. we want to keep a history of pressed keys
$20 newrollingbuffer const keybuf

pc to L1 \ break!
  piceoi, popa,
  sp 0 d) ' quit imm) mov,
  iret,

code isrIRQ1
  pusha,
  al addrof ps2data imm) in,
  al $e1 imm) cmp,
  L1 abs>rel jz,
  bx keybuf imm) mov,
  fastputc, then
  piceoi, popa, iret,

: 8042kbd@? ( -- keycode? f )
  keybuf getc dup EOF = if drop 0 else 1 then ;

\ For the mouse, we don't want to buffer all events. Instead, we want to
\ accumulate information for the next 8042mouse@ call.
create dx 0 ,
create dy 0 ,
\ indicate which buttons are *currently* pressed.
create btnflags 0 ,

\ first byte is a packet counter (we get 3 or 4 packets before completing the
\ "transaction". second byte is the value of the first packet (which has the
\ flags)
create pkt 0 ,

pc to L1 \ exit
  pkt abs) bx mov,
  piceoi2,
  bx pop, ax pop,
  iret,
pc \ jz=packet 1, the dx packet
  bh $10 imm) test,
  forward8 jz,
    ax $ffffff00 imm) or,
  forward!
  dx abs) ax add,
  bl inc,
  L1 abs>rel jmp,
pc \ ja=packet 2, the dy packet
  bh $20 imm) test,
  forward8 jz,
    ax $ffffff00 imm) or,
  forward!
  dy abs) ax add,
  bl bl xor,
  L1 abs>rel jmp,
code isrIRQc
  ax push, bx push,
  ax ax xor,
  al addrof ps2data imm) in,
  bx pkt abs) mov,
  bl 1 imm) cmp,
  ( pc ) abs>rel ja,
  ( pc ) abs>rel jz,
  \ packet=0, the flags packet
  \ check if it's a header packet. If not, return early to sync again.
  al $08 imm) test, L1 abs>rel jz,
  bl inc,
  bh al mov,
  al $7 imm) and, \ the 3 buttons flags
  btnflags abs) ax mov,
  L1 abs>rel jmp,

: canread? ps2cmd 1 and ;
: canwrite? ps2cmd 2 and not ;
: waitread 10000 0 do canread? if break then loop ;
: waitreadbig 100 0 do waitread canread? if break then loop ;
: waitwrite 10000 0 do canwrite? if break then loop ;
: cmd! waitwrite to ps2cmd ;
: data! waitwrite to ps2data ;
: data@ waitread ps2data ;
: checkAA ( -- f )
  data@ $aa = ?dup not if
    waitreadbig canread? if checkAA else 0 then then ;
: draindata begin waitread canread? while data@ drop repeat ;
: ack@# data@ ACK <> ?abort"ACK expected" ;
: writeport2 $d4 cmd! data! ;
: interruptoff $60 cmd! $04 data! ;
: interrupton $60 cmd! $07 data! ;

2 value 8042scancodeset

\ You need to remap the PIC before calling this
: 8042ps2$
  \ Disable PS/2 during initialization
  $ad cmd! $a7 cmd!
  ps2data drop \ make sure the port is empty
  interruptoff
  $aa cmd! data@ $55 <> ?abort"8042 self-test failed"
  $ab cmd! data@ $00 <> ?abort"8042 port1 self-test failed"
  $a9 cmd! data@ $00 <> ?abort"8042 port2 self-test failed"
  $ae cmd! \ enable 1st port
  $a8 cmd! \ enable 2nd port
  nullstream keybuf spit
  ['] isrIRQ1 $21 setISR 1 pic1unmask
  $ff data! \ send reset cmd to keyboard
  checkAA not if abort"Keyboard self-test failed" then
  $f0 data! ack@# 0 data! ack@# data@ ( n )
  1- 1 min 1+ to 8042scancodeset
  interrupton ;

: 8042mouse$
  \ Try to initialize the mouse. it might not be there.
  interruptoff
  $ff writeport2 checkAA if \ we have a mouse
    draindata \ the mouse can send all kind of garbage on init
    0 dx ! 0 dy ! 0 btnflags ! 0 pkt ! ['] isrIRQc $2c setISR 4 pic2unmask
    \ Enable straming
    $f4 writeport2 data@ ACK <> ?abort"mouse init failed"
  then interrupton ;

:> ( mouse -- )
  >r [ cli, ] 0 dx @! 0 dy @! btnflags @ [ sti, ] ( dx dy buttons )
  $ff and r@ to buttons ( dx dy )
  \ The PS/2 mouse give a positive dy when going up. We want the opposite.
  neg r> moveby ;
: new8042mouse ( -- mouse ) [ litn ] newmouse ;

\ Reboot using the 8042 interface to the reset pin
: reboot $fe $64 pc! ;
