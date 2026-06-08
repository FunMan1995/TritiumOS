needs drv/pc/pci drv/pc/pic drv/pc/pit drv/pc/a20
pic$ idt$ pit$ a20$

needs drv/pc/com
\ Don't initialize it, keep it as BIOS-initialized
comirq$
:> com1 write# ; console!

needs io/kbd
com1 newstreamkbd to keyboard

needs lib/diag app/prompt
prompt$

."Dusk OS\n" .free
