# m68k-virt target

This targets the [m68k-virt][] machine made especially for testing Dusk OS on
the m68k CPU.

[m68k-virt]: https://git.sr.ht/~vdupras/m68k-virt

## Usage

Running `make` will yield `kernel.img` and `disk.img` which can then be ran on
`m68k-virt` with:

	stty -icanon -echo; m68k-virt kernel.img disk.img; stty icanon echo

If you've clones and build the m68k-virt project alongside dusk, `make emul`
will be a shortcut command for building the Dusk port and running it as
described above.
