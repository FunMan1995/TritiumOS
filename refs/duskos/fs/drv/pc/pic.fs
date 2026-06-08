\ Programmable Interrupt Controller 8259 driver
needs drv/pc/ioport drv/pc/idt lib/bit asm/x86
unit drv/pc/pic

$20 const PIC1CMD
$21 const PIC1DATA
$a0 const PIC2CMD
$a1 const PIC2DATA

: picisr@ ( -- 16bitisr )
  $0b PIC1CMD pc! PIC1CMD pc@ $0b PIC2CMD pc! PIC2CMD pc@ 8 lshift or ;

: picirr@ ( -- 16bitisr )
  $0a PIC1CMD pc! PIC1CMD pc@ $0a PIC2CMD pc! PIC2CMD pc@ 8 lshift or ;

\ Clear out any pending ISR on the PICs
: picreset
  begin picisr@ ?dup while
    8 rshift if $20 PIC2CMD pc! else $20 PIC1CMD pc! then repeat ;

: picmasks! ( masks -- ) dup $ff and PIC1DATA pc! 8 rshift PIC2DATA pc! ;
: updatemasks PICMASKS @ picmasks! ;

: pic1unmask ( irq0-7 -- ) PICMASKS tuck c@ swap bit0! swap c! updatemasks ;
: pic2unmask ( irq8-f -- ) PICMASKS 1+ tuck c@ swap bit0! swap c! updatemasks ;

\ piceoi and piceoi2 are already defined in the kernel
: piceoi, al $20 imm) mov, al PIC1CMD imm) out, ;
: piceoi2, al $20 imm) mov, al PIC2CMD imm) out, al PIC1CMD imm) out, ;

: pic$
  \ Re-initialize master and slave PIC chips
  \ Mask everything except IRQ2 for cascading
  $11 PIC1CMD pc! $11 PIC2CMD pc! \ INIT + ICW4
  $20 PIC1DATA pc! $28 PIC2DATA pc! \ set 20/28 offsets
  $04 PIC1DATA pc! $02 PIC2DATA pc! \ Cascade on IRQ2
  $01 PIC1DATA pc! $01 PIC2DATA pc! \ 8086/8088 mode
  \ Copy the 08/70 IVTs to 20/28
  $08 4* $20 4* 8 move
  $70 4* $28 4* 8 move
  $fffb PICMASKS ! updatemasks picreset ;
