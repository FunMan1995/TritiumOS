needs asm/arm
unit drv/arm/exc

$20 const ARMEXCTBL

code noopisr reti,
code resetisr ' abort i) @, mov) f) rPC rd) rW rm) ,)
:~ abort"undefined instruction" ;
code undefisr ' ~ i) @, mov) f) rPC rd) rW rm) ,)
:~ abort"SWI? weird..." ;
code swiisr ' ~ i) @, mov) f) rPC rd) rW rm) ,)
:~ abort"prefetch abort" ;
code prefetchisr ' ~ i) @, mov) f) rPC rd) rW rm) ,)
:~ abort"data abort" ;
code dataisr ' ~ i) @, mov) f) rPC rd) rW rm) ,)

create _ ' resetisr , ' undefisr , ' swiisr , ' prefetchisr ,
         ' dataisr , ' noopisr dup , dup , ,
: armexc$
  0 8 ldr) rPC rdn) $18 +i) fill
  _ ARMEXCTBL 8 move ;
