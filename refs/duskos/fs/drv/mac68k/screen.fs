needs gr/color gr/buf
unit drv/mac68k/screen

$824 @ const SCRBUF
2BPPH GRAY2 640 480 newpixbuf to screen
screen computelinesz SCRBUF screen setbuf
1 screen to flipY