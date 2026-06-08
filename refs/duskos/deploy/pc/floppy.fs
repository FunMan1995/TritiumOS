needs io/kbd io/grid lib/coop drv/pc/acpi drv/pc/vga
vgagrid$
:> grid write# ; console!
' refreshcaret ' promptforkey realias
createapplication RefreshGrid
' refreshgrid IDLE RefreshGrid sethandler
RefreshGrid newcontext launchcontext

..
needs drv/pc/pci drv/pc/pic drv/pc/pit drv/pc/a20
pic$ idt$ pit$ a20$

..
needs drv/pc/fdc
fdc$ init
floppy newfatfs bootfs!

..
needs io/kbd drv/pc/ps28042p drv/ps2

' 8042kbd@? ' (ps2@?) realias
1 newps2kbd to keyboard

..
needs lib/coop app/gcon
coop$ initgcon

needs lib/diag
.free nl>
zsel edload<< init.txt
