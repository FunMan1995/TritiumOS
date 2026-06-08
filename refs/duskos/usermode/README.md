# Usermode Dusk

Usermode Dusk is a wrapper around Dusk OS allowing it to be used as an
applicative platform on modern OSes. This wrapper allows the code to be ran at
*native speed* (that's crazy fast) on the host OS. This wrapper is known to be
able to compile on:

* NetBSD
* FreeBSD
* Linux
* Hurd
* Windows (MinGW)
* Windows (Cygwin)

Because we're talking about native speed, this means that this can only be
ran on CPUs for which there's a support in Dusk OS and for which there is a
usermode kernel. This means:

* `i386`
* `amd64`
* `arm`

`riscv` doesn't have a usermode kernel. `aarch64` could theoretically run the
`arm` kernel with some fiddling, but it hasn't been done yet.

Theoretically, all combinations of hosts/CPU listed above work, but they haven't
been all tested. Moreover, the Makefile is likely broken for some of those
combinations. If you stumble on such combinations, please send patches to fix
the Makefile.

## Build and run

It's as simple as `make <flavor>`, which will create a binary executable named
`./flavor`. The available flavors are:

* `dusk`: raw dusk, run directly in the terminal with relatively poor UI
  support (i.e. no `help` command, among others).

* `dusk-sdl`: dusk with full graphics support. The closest thing to "full dusk"
  that you can get in usermode. Requres SDL2.

Then you just invoke those executables like any other.

Unlike the POSIX VM dusk-sdl, the "sdl" flavors of Usermode Dusk comes with more
interactivity features. Rather than having a read-only in-memory tar FS, they
have a read/write FAT FS living in "disk.img". Its kernel and boot payloads,
instead of being embedded in the executable, are read from files "kernel-XXX"
and "payload-XXX".

> Refer to `posix/README.md` for more details about flavors.

### Forcing i386

It's possible that want to build a i386 usermode Dusk on a amd64 machine. In
that case, you need to override some make variables:

	make ARCH=i386 MACHINE_FLAGS=-m32 clean all

There aren't many reasons to do so other than for development purposes. Maybe
for some workloads the i386 kernel is slightly faster.

In any case, you should know that this executable needs to be built with GCC
"multilib" capability. You also need the i386 versions of curses and SDL2
installed. This might involve significant fiddlings depending on your host
system. In other words, you have to know what you're doing.

## Packages

While Usermode Dusk can be used in "regular" interactive mode, one exciting
possibility that it allows is to package application in compact, fast and
standalone executables for the host OS. These applications are called "Dusk
packages".

To create a package, what you need is a "payload", that is, a stream of Forth
source that will intepreted by the kernel. A Dusk kernel starts from nothing but
the HAL, so that payload is likely to begin with the contents of
`/xcomp/boot.fs`. But afterwards, you place what you want.

In interactive Usermode Dusk, both the kernel and payload live in regular files
named `kernel` and `payload_<flavor>` in the same directory as the executable.
This allows you to fiddle with them for maximum debuggability.

Dusk packages will typically embed both their kernel and payload in the
executable themselves. The most straightforward and portable way to do so is to
generate ".h" file with hardcoded contents in it.  You'll need to generate a
`kernel.h` and a `payload.h` from `usermode/kernel` and from your generated
payload. For this, you can use `embedh.sh` supplied by Dusk.

Then, you wrap all this in a `main()` function that will call `common_init()`
and `common_exec()`. Your C source might look like this:

	#include "duskos/usermode/common.h"
	#include "kernel.h"
	#include "payload.h"

	int main(int argc, char *argv[]) {
		size_t memsz = 4*1024*1024; /* 4MB of memory */,
		void* mem = common_init(argc, argv, memsz);
		if (!mem) return 1;
		memcpy(mem, kernel, sizeof(kernel)); /* supplied by kernel.h */
		void* ppayload = mem + memsz - MAXPAYLOADSZ;
		memcpy(ppayload, payload, sizeof(payload)); /* supplied by payload.h */
		common_exec();
		return 0;
	}

This will result in a standalone executable that runs your payload!

Confused? The best way to figure out how to build your package is to look at
examples. Take a look at existing Dusk packages:

* [Dusk Examples](https://git.sr.ht/~vdupras/dusk-examples)
* [Dusk Invoice](https://git.sr.ht/~vdupras/dusk-invoice)
* [Dusk Gopher Daemon](https://git.sr.ht/~vdupras/dusk-gopherd)
* [Dusk inet server boilerplate](https://git.sr.ht/~vdupras/dusk-inet)

## Theory of operation

The ability of the Usermode wrapper to run Dusk natively rests on Dusk kernels'
ability to auto-relocate themselves. They aren't quite position independent
because links in the system dictionary are absolute addresses, but the kernels
are built in a way that it's possible (trivial even) for it, at boot time, to
examine itself, know where it's ran from, then modify itself to run properly
from this location.

From that point on, it reads the "boot arguments" supplied by the Usermode
wrapper which gives it enough information to bootstrap itself into whatever its
final purpose is.

That's why the wrapper job at boot time is relatively simple:

1. Create a memory area that is readable, writable and executable.
2. Load the kernel at its first address.
3. Place boot arguments at a fixed address in that memory.
4. Call first address of that memory area.

### interopzone

The Usermode wrapper and Dusk communicate through a structure defined in
`common.h` called `interopzone`. It's through this struct that boot arguments
are passed, but it's also through there that API functions receive their
arguments and yield their results.

This structure lives in Dusk memory at a pre-defined address, a constant we call
`BOOTZONESZ`, which has a value of 8KB. That zone represents the maximum size
that a Dusk kernel (which is quite small) can have. This doesn't include the
payload, which lives outside Dusk memory and is read-only.

The `interopzone` struct lives at the very end of `BOOTZONESZ`, which means that
the actual maximum size of a kernel is rather `BOOTZONESZ-sizeof(interopzone)`.

With such a predefined constant, Dusk Usermode kernels know where to look for
boot arguments, which allows them to bootstrap themselves properly.

### Calling an API function

The Usermode wrapper does more than merely booting Dusk, it also provides it
with an API to the Host system. As previously mentioned, this is done through
the `interopzone` struct. `common.c` exposes a global pointer to it as the `iz`
variable.

`iz->funcs` is a pointer to an array of function pointers, which all have a
`void (*)(void)` signature. Its those pointers that the wrapper API words
described below call.

When doing a syscall, Dusk saves its PSP and W registers into the IZ, allowing
the C code to push and pop arguments from PS. Then, when the syscall ends, Dusk
restores the possibly modified registers into the CPU.

## Usermode API

The API that Usermode exposes is the same as the one exposed in the POSIX VM,
that is, stream I/Os ("emit", "?getnkc" etc..) and enough to get the grid or
graphics (with keyboard and mouse) going.

### Pseudo terminal

On Linux, Usermode can also allocate a pseudo terminal that can be used, for
example, to run the ansi terminal emulator inside the SDL2 flavor.

To do so, compile with `-DOPENPTY` flag. When starting, it will allocate a
pseudo terminal, open a non-blocking file descriptor for it, and print both
the terminal name and the descriptor number to stderr.

You can then, on the host, run a command like

	TERM=ansi setsid bash </dev/pts/7 >/dev/pts/7 2>&1

to run a shell on that terminal. Within dusk, you can then run this:

	6 6 newfdio value pty
	needs io/grid io/ride text/ansi
	COLS LINES newansigrid to grid
	grid ttycfgcmd pty puts
	pty ride


### Extending the API

It's sometimes useful to have your own API calls, for example if you want to
wrap a library on the host OS. You can do so by creating a new C function with
a `void (*)(void)` signature and assign it to a `cbfuncs[]` slot. There are
`APIFUNCCNT` (256) available slots and the first `APIRESERVEDCNT` (32) ones are
reserved for Dusk itself, the rest is yours.

As explained above, arguments are passed through the saved PSP and W
registers.  You can access them through helper functions defined in
`posix/mem.h` and implemented in `usermode/common.c`. Here's an example:

	void myadder() { // ( a b -- n )
		dword b = ppop();
		dword a = ppop();
		ppush(a + b);
	}
	// ... Later in setup code
	cbfuncs[42] = myadder;

On the Forth side, those APIs have to be wrapped with the `syscallback` You
supply it with the index of the function. For example, let's wrap `myadder`:

	42 syscallback myadder
	12 23 myadder . \ Prints "35"
