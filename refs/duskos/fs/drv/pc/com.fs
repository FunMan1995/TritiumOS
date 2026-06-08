needs io/stream mem/roll drv/pc/pic drv/pc/ioport asm/x86
unit drv/pc/com

\ Hardcoded to COM1, 115200 bauds, 8N1

512 const BUFSZ
BUFSZ newrollingbuffer const rxbuf
variable missedrx

$3f8 const COMPORT

\ TODO: the save of EDI below is hazardous. I should do like in ARM and have
\ saveISR, which would push all registers that the HAL uses as tmp.
code isrIRQ4
  ax push, bx push, dx push, di push, \ EDI is used in fastputc,
  dx COMPORT 5 + imm) mov, al dx in, \ Line Status
  ax 1 imm) and, ifnz,
    dx COMPORT imm) mov, al dx in, \ Receive data
    bx rxbuf imm) mov, \ A=rxbuf
    fastputc, else missedrx abs) inc, then then
  piceoi, di pop, dx pop, bx pop, ax pop, iret,

: _readbuf ( n com1 -- a? read-n ) drop rxbuf readbuf ;
: _writebuf ( a n com1 -- written-n )
  begin COMPORT 5 + pc@ $20 and until
  2drop c@ COMPORT pc! 1 ;

' _readbuf ' _writebuf newstream const com1

: comirq$ ['] isrIRQ4 $24 setISR 4 pic1unmask ;
: com$ ( -- )
  $01 COMPORT 1+ pc!  \ Interrupt on RECV
  $80 COMPORT 3 + pc! \ DLAB on
  1 COMPORT pc!       \ DLAB lo, 115200 bauds divided by 1
  0 COMPORT 1+ pc!    \ DLAB hi
  $03 COMPORT 3 + pc! \ 8 bits no parity 1 stop bit, DLAB off
  $07 COMPORT 2+ pc!  \ enable FIFO, clear it, 1-byte threshold
  $0b COMPORT 4+ pc!  \ IRQ enabled RTS/DSR set
  comirq$ ;
