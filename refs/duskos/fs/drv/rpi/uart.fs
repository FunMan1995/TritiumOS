needs drv/rpi/gpio drv/rpi/intr lib/ival io/stream mem/roll asm/arm
unit drv/rpi/uart

$201000 const UART_BASE
MMIO_BASE UART_BASE + absvalmap {
  uint DR RSRECR ;
  +$18 uint FR ;
  +$20 uint ILPR IBRD FBRD LCRH MCR IFLS IMSC RIS MIS ICR DMACR ;
  +$80 uint ITCR ITIP ITOP TDR ;
}

: uart0$
  0 to MCR \ Disable UART0
  GPIOPullNone $c000 gpiopull0 \ Disable pull up/down for pins 14,15
  fsel1 $fffc0fff and $00024000 or to fsel1 \ GPIO 14,15 to ALT0
  $7ff to ICR \ Clear pending interrupts.
  \ Set integer & fractional part of baud rate.
  \ UART_CLOCK on rpi1 is 48Mhz
  \ Divider = UART_CLOCK/(16 * Baud)
  \ Fraction part register = 64th of the unit
  \ Baud = 115200.
  \ Divider = 48000000 / (16 * 115200) = 26.042
  26 to IBRD
  3 to FBRD
  $70 to LCRH \ Enable FIFO & 8N1
  0 to IMSC
  $301 to MCR ; \ Enable UART0, receive & transfer part of UART.

$10 const RXMIS
: uart0irq$
  RXMIS to IMSC
  $02000000 to enable2 ; \ Enable IRQ57 (uart_int)

$2000 const RXBUFSZ
RXBUFSZ newrollingbuffer const rxbuf
variable missedrx

: uartbase@, ( reg -- )
  mov) over rd) MMIO_BASE imm) ,)
  add) over rdn) $200000 imm) ,) \ GPIO_BASE
  add) swap rdn) $1000 imm) ,) ;

: dorx,
  rxbuf i) A>) @,
  [compile] begin
    r1 uartbase@,
    ldr) r0 rd) r1 rn) $18 +i) ,) \ FR
    and) r0 rdn) $10 imm) f) ,)
    ifz, ( br )
    ldr) rW rd) r1 rn) ,)
    and) rW rdn) $ff imm) ,)
    fastputc, [compile] else 1 missedrx m) +n, [compile] then
  swap [compile] again [compile] then ;

: uart0isr, ( -- handled-br )
  r1 uartbase@,
  ldr) r0 rd) r1 rn) $40 +i) ,) \ MIS
  and) r0 rdn) RXMIS imm) f) ,)
  ifnz,
    rA push, rW push, rS push,
    str) r0 rd) r1 rn) $44 +i) ,) \ ICR
    dorx,
    rS pop, rW pop, rA pop,
  [compile] else ;

: _readbuf ( n uart -- a? read-n ) [ dorx, ] drop rxbuf readbuf ;
: _writebuf ( a n uart -- written-n )
  begin FR $20 and not until
  2drop c@ to DR 1 ;

' _readbuf ' _writebuf newstream const uart0
