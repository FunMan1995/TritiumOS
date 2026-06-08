# PC Engine Alix boards

[PC Engine's Alix boards](https://www.pcengines.ch/alix.htm) are awesome little
machines that have the advantage of having a rather well-behaved BIOS and in
general very regular PC stuff, but all of this on a small board with no heatsink
or fan. On top of that, there's the old school RS-232 connector plugged into the
very regular COM1 (at 38400 bauds by default, that's a bit weird). And of course
the 3 ethernet ports, making it an ideal little router.

Of course, Dusk runs on it.

* CPU: 500 MHz (LX800) AMD Geode LX CPU
* Memory: 256 MB DDR SDRAM (333 or 400 MHz clock)
* Storage: CompactFlash card connector hooked into ATA primary.
* No VGA
* Comm: RS-232 connector plugged to COM1

## Deployment

1. Run `make` which yields `alix.img`.
2. Copy this image to a CompactFlash card.
3. Put the card in the machine, set yourself up for a 38400 baud 8N1 serial
   connection.
4. Power up the machine, you should get a Dusk OS prompt through the serial
   link.
