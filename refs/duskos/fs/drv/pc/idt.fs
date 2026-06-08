\ Interrupt Descriptor Table
unit drv/pc/idt

: setISR ( w idx -- ) \ set interrupt "idx" to word w
  over 16 rshift rot> 8 * IDT + tuck w! 6 + w! ;

: _isr00 abort"Divide by zero" ;
: _isr05 abort"Bound range exceeded" ;
: _isr06 abort"Invalid opcode" ;
: _isr07 abort"Device not available" ;
\ To avoid triple faults, we don't do anything on double fault and go straight
\ to (abort) on ISR08
: _isr0a r> .x spc> abort"Invalid TSS" ;
: _isr0b r> .x spc> abort"Segment not present" ;
: _isr0c r> .x spc> abort"Stack segment fault" ;
: _isr0d r> .x spc> abort"General protection fault" ;
: _isr0e r> .x spc> abort"Page fault" ;
: _isr10 abort"x87 floating-point exception" ;
: _isr11 r> .x spc> abort"Alignment check" ;
: _isr12 abort"Machine check" ;
: _isr13 abort"SIMD floating-point exception" ;
: _isr14 abort"Virtualization exception" ;
: _isr15 r> .x spc> abort"Control protection exception" ;

\ Only call this after you've moved PIC base vectors around!
: idt$ ['] _isr00 $00 setISR
       ['] _isr05 $05 setISR
       ['] _isr06 $06 setISR
       ['] _isr07 $07 setISR
       ['] (abort) $08 setISR
       ['] _isr0a $0a setISR
       ['] _isr0b $0b setISR
       ['] _isr0c $0c setISR
       ['] _isr0d $0d setISR
       ['] _isr0e $0e setISR
       ['] _isr10 $10 setISR
       ['] _isr11 $11 setISR
       ['] _isr12 $12 setISR
       ['] _isr13 $13 setISR
       ['] _isr14 $14 setISR
       ['] _isr15 $15 setISR ;
