# Dusk OS

Dusk OS is a 32-bit Forth and big brother to [Collapse OS][collapseos]. Its
[primary purpose][design/purpose] is to be maximally useful during the [first
stage of civilizational collapse][coswhy], that is, when we can't produce
modern computers anymore but that there's still many modern computers around.

It does so by aggressively prioritizing [simplicity][design/simple] at the
cost of [unorthodox constraints][design/limits], while also aiming to make
[operators happy][design/shell].

Dusk OS innovates by having an ["almost C" compiler][comp/c] allowing it to
piggy-back on UNIX C code, through a modest [porting effort][design/port], to
reach its goals and stay true to its design constraints with a minimal effort.

This is Dusk OS' source code and the rest of the README assumes that you want to
run it. To read more about why this OS exists, see its [website][website].

[website]: http://duskos.org
[collapseos]: http://collapseos.org
[coswhy]: http://collapseos.org/why.html
[design/purpose]: fs/doc/design/purpose.txt
[design/simple]: fs/doc/design/simple.txt
[design/limits]: fs/doc/design/limits.txt
[design/shell]: fs/doc/design/shell.txt
[design/port]: fs/doc/design/port.txt
[comp/c]: fs/doc/comp/c.txt

## Build and run Dusk

Dusk is designed to run on bare metal and to build itself from itself.
However, it's also possible to build Dusk from any POSIX platform using
Dusk's [POSIX VM][posixvm] from `posix/vm.c`. This VM implements a Forth
that can interpret the whole of Dusk's Forth code in a CPU-agnostic manner.

That is enough to generate bare metal images for any of its target platforms,
so that's why it exists. To build this VM, you need:

* Make (GNU or BSD)
* A C compiler
* tar

Running `make` will yield a `./dusk` binary which if executed, provides an
interactive prompt.

Documentation lives in `fs/doc/`. You can begin with [doc/index][docs].

Type `bye` to quit.

Dusk OS expects a non-canonical raw input. With a regular TTY, your input will
be buffered and echoed twice and reads to it will be blocking. We don't want
that. To avoid that, you can invoke it like this:

    (stty -icanon -echo min 0; ./dusk; stty icanon echo)

`make run` does this for you.

[posixvm]: posix/README.md
[docs]: fs/doc/index.txt

## Running the "SDL" flavor

The basic POSIX Dusk deals with streams only. You can't use the grid or
graphics from within there. However, it's possible to build a "SDL" version of
Dusk that has these capabilities. See the [POSIX VM README][posixvm] for
details.

## Running on Usermode

Graphics from within SDL, cool right? It gets even better: Usermode Dusk allows
you to run these things at *native speed*. See [Usermode README][usermode] for
details.

[usermode]: usermode/README.md

## Running on bare metal

Even if Dusk is super powerful even within the constraints of a POSIX
environment, it unleashes its full powers on bare metal.

Deploying Dusk on a real machine is a bit more involving than running the
POSIX VM and you should read [doc/deploy.txt][]. There's a [collection of
deployment configurations][deployments] in the `deploy` directory to help you
get started on deploying Dusk OS to your machine.

This directory contains a few targets with convenient QEMU launchers, so this
can be a good way to quickly see a fully featured Dusk OS in action.

[doc/deploy.txt]: fs/doc/deploy.txt
[deployments]: deploy/README.md

## Trusting this code

Release tags in this repository are signed with my old Gentoo developer key.
This key is present in this repository under the file `signkey`. Of course, you
can't trust this very text telling you that this key is indeed used by the
author of this project, but at least this simple scheme allows a "trust on first
use" pattern. Once you've made the leap of faith importing this key, you can
trust that the person signing the repo doesn't change.

Moreover, I've used my old Gentoo developer key for a reason: it allows you to
make that leap of faith more easily. You can use the same key to verify commits
made by me in the Gentoo repository.

To verify commits or release tag, it goes thus:

	cd duskos
	gpg --import signkey
	git verify-commit master
	git verify-tag v10
