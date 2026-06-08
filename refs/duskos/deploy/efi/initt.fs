needs io/kbd io/grid lib/coop drv/efi/grid
efigrid$
:> grid write# ; console!
' refreshcaret ' promptforkey realias
createapplication RefreshGrid
' refreshgrid IDLE RefreshGrid sethandler
RefreshGrid newcontext launchcontext

needs io/kbd drv/efi/kbdex
bootkbd newefikbdex to keyboard

needs drv/efi/gop
gop$

needs drv/efi/timer lib/coop
createapplication EFIIdle
' efiidle IDLE EFIIdle sethandler
EFIIdle newcontext launchcontext

needs lib/coop app/gcon lib/diag
coop$ initgcon

.free nl>
zsel edload<< init.txt
DisableWatchdog
