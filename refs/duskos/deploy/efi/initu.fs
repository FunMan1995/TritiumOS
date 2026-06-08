needs drv/efi/kbd app/prompt lib/diag
newefikbd to keyboard
prompt$

needs drv/efi/uga
\ 1 uga!
uga$

needs gr/font/uf2 io/grid lib/coop io/kbd gr/grid
"data/font/term12.uf2" loaduf2 grgrid$
:> grid write# ; console!
' refreshcaret ' promptforkey realias
createapplication RefreshGrid
' refreshgrid IDLE RefreshGrid sethandler
RefreshGrid newcontext launchcontext

."Dusk OS\n" .free nl>
DisableWatchdog
createapplication EFIIdle
:> 100 Stall ; IDLE EFIIdle sethandler
EFIIdle newcontext launchcontext
