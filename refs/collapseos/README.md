# Collapse OS

This repository contains the "core" of Collapse OS, that is, the code that is
common to all Collapse OS ports. It also contains documentation, in `doc/`,
common to all ports. Begin with `doc/intro.txt`.

By itself, this repository doesn't do much. Its default `make run` target
builds a 6502 version of Collapse OS targeting a virtual machine designed to be
ran inside [Dusk OS][dusk]' 6502 emulator, but that's not very exciting.

The real meat is in the ports, which all use this common code.

[dusk]: http://duskos.org

## Ports

Collapse OS is deployed to actual machines through *ports*, separate
repositories that contain build procedures and hardware driver allowing
Collapse OS to run on specific machines. This is the list of known ports,
organized by CPU:

* Z80
	* [RC2014](https://git.sr.ht/~vdupras/collapseos-rc2014)
	* [TI-84+](https://git.sr.ht/~vdupras/collapseos-ti84)
	* [Z80-MBC2](https://git.sr.ht/~vdupras/collapseos-z80mbc2)
	* [TRS-80 Model 4P](https://git.sr.ht/~vdupras/collapseos-trs804p)
	* [Sega Master System](https://git.sr.ht/~vdupras/collapseos-sms)
* 8086
	* [PC/AT](https://git.sr.ht/~vdupras/collapseos-pc)
* 6502
	* [Apple IIe](https://git.sr.ht/~vdupras/collapseos-appleiie)
	* [步步高朗文4980](https://codeberg.org/iyzsong/duskos/src/branch/master/cos/ports/bbk-a4980)
* 6809
	* [TRS-80 Color Computer 2](https://git.sr.ht/~vdupras/collapseos-coco2)

## Getting started

The best place to start is with the RC2014 port of Collapse OS. It's
the "canonical" machine-specific port of the project so it's the best
maintained. The port has a built-in emulator so trying Collapse OS is one
"make emul" away.

One thing that the RC2014 port doesn't have, however, is a Grid subsystem, so
you can't use it to try applications like the Visual Editor. To do so, you can
use the PC/AT port of Collapse OS. Emulation of that port is done with QEMU.

Documentation is in text files in Dusk's `fs/doc/cos` directory. Begin with
`intro.txt`. Alternatively, James Stanley hosts an [online Collapse OS
documentation browser](https://incoherency.co.uk/collapseos/). However, at the
time of this writing, this documentation was still generated from the "old"
Collapse OS, so it's outdated.

Another interesting alternative for documentation is
[Michael Schierl's PDF export of it](https://schierlm.github.io/CollapseOS-Web-Emulator/documentation.html)
([code that generates it](https://github.com/schierlm/CollapseOS-Web-Emulator/tree/master/pdf)).
It's a great way to print the whole thing at once. However, at the time of
this writing, this documentation was still generated from the "old" Collapse
OS, so it's outdated.

You can also try Collapse OS directly on your browser with [Michael Schierl's
JS Collapse OS emulator](https://schierlm.github.io/CollapseOS-Web-Emulator/)
which is awesome but it isn't always up to date. The "Javascript Forth" version
is especially awesome: it's not a z80 emulator, but a *javascript port of
Collapse OS*!

## Relation with Dusk

If you run `make run`, you'll notice that it download Dusk OS and runs Collapse
OS through it. That's because to bootstrap itself, Collapse OS either needs
itself or a Forth that looks like it.

To bootstrap itself from a POSIX platform, the best option is Dusk, which is
what we use here.

## History

Collapse OS moved around a lot. It began in 2019 as an OS written entirely in
Z80 assembler. Then, in 2020 I rewrote it in Forth. This early development of
Collapse OS is available in its [old repository][old].

[old]: https://git.sr.ht/~vdupras/collapseos-old

Then, in 2021, I decided I wanted to downsize my tooling and began using RCS
instead of git. I published Collapse OS updates as tarballs until 2023. Those
tarballs are available [here](http://collapseos.org/files/).

Until then, Collapse OS bootstrapped itself from a virtual Forth machine
written in C. But because of the peculiarity of Collapse OS cross compilation
process (It is designed to be able to run from ROM, so unlike Dusk, the kernel
doesn't try to be minimal, but maximal. That is, at boot time, the kernel is
already ready to roll. No boot interpretation needed), it needed an opaque
binary blob to start itself. That blob was built over time, from previous
versions.

That blob irked some bootstrapping purists, including me. Bootstrapping from a
Forth like Dusk provided the opportunity to get rid of opaque binary blobs.

So in 2023, Collapse OS became part of the Dusk project.

But after a while, a problem became apparent: improvement in Dusk often broke
Collapse OS without me noticing. But the thing is that Collapse OS only need a
tiny part of Dusk's feature set. It can target an ancient Dusk version and be
happy.

So in 2025, it broke of from Dusk while still using it for bootstrapping
purposes. It's this repository here. We use an old Dusk version, but it doesn't
matter.
