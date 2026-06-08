# Dusk's POSIX VM

The POSIX VM is an implementation of a Dusk kernel written in C that can be
compiled and ran on any POSIX platform. It's slow because it's emulated, but
can run on any POSIX environment.

Because every Dusk kernel must implement a HAL and because the purpose of this
VM is to run on any POSIX platform, it partially emulates ARM and its HAL
generates the same code as the ARM kernel.

This is the main "gateway" to Dusk OS as this VM is used as a launching platform
to build Dusk binaries to actual targets.

Its filesystem is a `tar` snapshot of the `fs` directory that is taken at
compile time and embedded in its binary.

## Requirements

To build the POSIX VM, you needs a POSIX system with a C compiler, Make and tar.

## Build and run

Running `make` at the root of this project (that is, not in the `posix/`
directory, in its parent) yields a `dusk` executable. If you run it, you get
an interactive Dusk console.

Dusk OS expects a non-canonical raw input. With a regular TTY, your input will
be buffered and echoed twice and reads to it will be blocking. We don't want
that. To avoid that, you can invoke it like this:

    (stty -icanon -echo min 0; ./dusk; stty icanon echo)

`make run` does this for you.

### The "grid" flavor

Running `make dusk-grid` produces an executable that sets up a [io/grid][]
and hooks it to stdout using [text/ansi][] encoder, allowing you to use Dusk's
grid applications in a UNIX terminal.

There's a `make rungrid` target that serves the same purpose as `make run`.

[io/grid]: ../fs/doc/io/grid.txt
[text/ansi]: ../fs/doc/text/ansi.txt

### The "SDL" flavor

If you have [SDL2][sdl2] installed, you can run `make dusk-sdl`. This will yield
a `dusk-sdl` binary where SDL is used to implement a graphical screen, giving
you full graphical capabilities.

[sdl2]: https://www.libsdl.org/

## Command line options

A naked `./dusk` brings you the default interactive prompt.

However, the POSIX VM can also be used as a scripting tool through the `-c` `-f`
and `-n` flags.

Those flags basically do the same thing: they construct a command for Dusk to
run.

* `-c` appends a literal command
* `-f` reads a file and appends that contents to the command
* `-n` wraps the specified string between a `needs` and a `\n`

Order of these commands matter. The leftmost argument comes first. For example:

    ./dusk -c '."hello"' -f somescript.fs -c '."goodbye"'

will print `hello`, then will execute the contents of `somescript.fs`, which
is a path to the **host** filesystem, then will print `goodbye`.

When any of those three flags are used, Dusk doesn't go to prompt anymore. It
calls `bye` to quit the VM.

Then comes the boolean flags which are used alone (no string argument) and for
which order doesn't matter.

The `-e` flag will make `emit` spit to FD 2 (stderr) instead of FD 1 (stdout).

The `-p` flag will cause a prompt to be shown even when the `-c/-f/-n` options
are used.

Only the "raw" flavors of Dusk support those flags above. `dusk-sdl` cannot
take options.

## The stdio stream

The POSIX VM has a special `stdio` I/O stream that wrap file descriptors 0 and 1
(stdin and stdout).

This binding ignores the `-e` command line flag. This allows "stdio" to serve
as a "data ouput" channel where emitted stuff don't corrupt it.

This wrapper replaces `-1` (error) from `fdread` and `fdwrite` with zero. It's
not quite right, but there's no straightforward ways to place that error
condition into the IO subsystem in a way that makes it recoverable. So, hum, for
now it works...

## API

The POSIX VM exposes an API to interact with the host OS, mostly to interact
with files and streams.

	fdopen ( strpath write? -- ?size fd-or-0 )
		Run `open(2)` on `strpath` and yield its file descriptor or 0 if there's
		an error. If `write?` is nonzero, open it in R/W mode.

		If `fd` is nonzero, the size in bytes of the opened file is yielded as
		`?size`.

	fdclose ( fd -- )
		Run `close(2)` on `fd`.

	fdread ( a u fd -- n )
		Run `read(2)` on `fd`, reading `u` bytes at destination address `a`.

	fdwrite ( a u fd -- n )
		Run `write(2)` on `fd`, writing `u` bytes from source address `a`.

	fdseek ( off fd -- )
		Run `lseek(2)` on `fd` with `whence=SEEK_SET` and offset `off`.

## Keyboard layouts

When you have a POSIX system, you can use `kbdconv` to convert GRUB keyboard
layout files to Dusk keyboard layouts. GRUB keyboard layouts can be created
by `grub-mklayout` from Linux `loadkeys` keyboard layouts. Run `make kbdconv`
followed by `./kbdconv xx.gkb fs/data/kbdl/xx`.

For example, on a Debian system, you could do:

	apt install console-setup
	ckbcomp ca | grub-mklayout > ca.gkb
	./kbdconv ca.gkb fs/data/kbdl/ca
