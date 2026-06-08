\ RISC-V UART (https://www.lammertbies.nl/comm/info/serial-uart)
needs lib/ival
unit drv/riscv/uart

variable curaddr
curaddr ivalmap {
  uchar BUF IER FCR ;
  +2 uchar IIR LCR MCR LSR MSR SCR ;
}

$7 const DLAB \ 7-bit of UART_LCR (unused)
$3 const 8BITS \ Set 8b width

\ 00000011b => 8 data bits, one stop bit, no parity
$3 const DEFAULT_LCR
$0 const DEFAULT_FCR

: uart$ ( baseaddr -- )
  curaddr !
  DEFAULT_LCR to LCR
  DEFAULT_FCR to FCR ;
: uart! begin LSR $20 and until to BUF ;
: uart@ begin LSR $1 and until BUF ;
: uart@? LSR $1 and dup if BUF swap then ;
