needs io/kbd io/grid lib/coop drv/pc/acpi drv/pc/vga
vgagrid$
:> grid write# ; console!
' refreshcaret ' promptforkey realias
createapplication RefreshGrid
' refreshgrid IDLE RefreshGrid sethandler
RefreshGrid newcontext launchcontext

needs io/kbd drv/pc/ps28042p drv/ps2

' 8042kbd@? ' (ps2@?) realias
1 newps2kbd to keyboard

needs app/prompt
prompt$

needs drv/pc/a20
a20$

needs lib/fmt lib/diag
."Dusk OS\n" .free
