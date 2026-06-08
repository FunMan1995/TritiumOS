needs io/kbd io/grid lib/coop drv/pc/acpi drv/pc/vga
vgagrid$
:> grid write# ; console!
' refreshcaret ' promptforkey realias
createapplication RefreshGrid
' refreshgrid IDLE RefreshGrid sethandler
RefreshGrid newcontext launchcontext

needs drv/pc/pic drv/pc/pit drv/pc/a20
pic$ idt$ pit$ a20$

needs drv/pc/pci drv/pc/ahci
pcpci$ ahci$
0 newahcidrive dup enable bootfs fatstorage!

needs io/kbd drv/pc/ps28042p drv/ps2

' 8042kbd@? ' (ps2@?) realias
8042scancodeset newps2kbd to keyboard

needs app/prompt
prompt$

needs lib/fmt lib/diag
."Dusk OS\n" .free
