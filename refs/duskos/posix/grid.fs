needs io/kbd
:> ( kbd -- ?nkc event-type ) drop (?key) if Passthrough or BOTH else 0 then ;
newkbd to keyboard

needs io/grid text/ansi lib/coop io/kbd
termsz grid resize
:> grid write# ; console!
' refreshcaret ' promptforkey realias
createapplication RefreshGrid
' refreshgrid IDLE RefreshGrid sethandler
RefreshGrid newcontext launchcontext
