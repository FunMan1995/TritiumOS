# m68k Mac target

This targets any Mac with an m68k CPU. We build a `floppy.img` image containing
a HFS boot sector that boots into Dusk under a gr/grid.

## Tested models

While this theoretically can run on any Macintosh with a m68k CPU, this has only
been tested on those models so far:

* Powerbook 520

## Usage

Running `make` will yield `floppy.img`, which you can `dd` into a floppy. Shove
that into your Mac, it will boot to a Dusk OS prompt.
