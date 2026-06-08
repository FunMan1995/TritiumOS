# PC target

This Dusk target is the "old school" PC, with a BIOS. If your PC is more modern,
you probably want to look at `deploy/efi`.

It will boot from any media that the BIOS supports and use the BIOS to read from
that media.

While this target will work fine on most PCs, you will probably want to tweak
your `init.fs` with proper mass storage drivers. Look at `ahci` and `ata` images
for such example deployments.

The default one, `drv/pc/int13hl.fs` calls the BIOS every time a sector is
read or written, which involves jumping in and out of real mode, which can
cause glitches.

There are other images with alternate configuration. They are not built by
default. To build them, you call make with them as a target, with a `.img`
suffix. For example, `make bare.img`.

## Graphics mode

The default PC target has a switcher between VGA text mode and graphics mode.
Typing "graphicsmode" will enable the VESA driver and switch to a [gr/grid],
thus enabling all graphical applications. "consolemode" will disable VESA and
go back to the regular VGA text mode.

## The "pcpbr" image

This is like the main "pc" image except it is for putting in an MBR partition.
There is a patched FreeDOS MBR sector with suitable partition table entry,
which is used for `make emulpbr`.

## The "bare" image

While the default image targets a very low common denominator and should work
on any PC, it might make incorrect assumptions about it an initialization. If
it doesn't boot, try the "bare" image instead, which gets you to prompt in the
barest possible way. This one will boot. Then, you can debug.

## The "ata" image

The `ata.img` image is like the bare one but it demonstrates ATA
initialization for mass storage.

## The "ahci" image

The `ahci.img` image is like the bare one but it demonstrates AHCI
initialization for mass storage.

## The "floppy" image

Dusk OS easily fits on a 1.44M floppy, but not its documentation. To have Dusk
OS boot from a floppy, a special documentation-less version of it has to be
built. The FAT metadata also needs to flip some switches to have it boot
correctly from floppy.

## The "alix" image

This configuration is specific to [PC Engine's Alix board][alix] with a console
on its RS-232 connector.

Remember that by default, the COM port on that machine is configured to 38400
bauds.

[alix]: https://www.pcengines.ch/alix.htm

## Usage

Running `make` will yield `pc.img`, `pcpbr.img`, `bare.img`, and `floppy.img`.
You can `dd` one of those images on the target media and then boot from it.

Running `make emul` will launch QEMU with the `pc.img` image. There's also
`make emulpbr`, `make emulbare` and `make emulfloppy` if you feel so inclined.
