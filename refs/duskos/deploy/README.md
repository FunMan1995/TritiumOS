# Dusk Deployments

Dusk OS is fully self-hosting, which means that a Dusk OS deployment has all the
tools needed to create deployment media for all its target platforms. As a Dusk
operator, all you need are those tools.

As a Dusk newcomer who has nothing but a UNIX system, a kind of launching pad
is needed. This directory contains those launching pads for all platforms that
Dusk OS targets.

They essentially wrap Dusk tooling and provide an example `init.fs`. They don't
cover every possible deployment scenarios, but they are meant to give good
examples of how to get started with your own deployments.

## Quality assurance

When we deal with software that needs to be tested on actual hardware, a problem
arises: not all developers have access to every piece of hardware supported by
the software.

To avoid testing queues to bog down the development process, we allow
deployments to "fall behind". When someone has the time to test the deployment,
they do so and then mark the deployment as "tested".

This procedure takes the form of a Test Matrix, which is at the bottom of this
very page. This lists every deployment, the last version of Dusk it was tested
on and the person who tested it. We only do such tests on tagged versions.

**TL;DR: It's not because a deployment is present here that it works. Refer to
the Test Matrix below to determine the last version a particular deployment was
tested on. Do a checkout of that tag to build a working deployment.**

### The Context column

The "Context" column of the test matrix has a bit of a fuzzy meaning. It exists
because some deployments can be ran in more than one context. For example, the
`pc` deployment has multiple configurations such as ATA, floppy, AHCI. The
`rpi` deployment's main config can run on multiple RPi models and some models
fail their QA tests when others don't.

Whenever there's a situation where the result of the QA can change under a
different context, we add a discriminating line in the test matrix.

Common sense has to be used. It doesn't make sense to add a line for every
possible PC models because there are too many, but it does make sense to do so
for every RPi model.

### Testing procedure

To be able to "stamp" a deployment as tested, this is the procedure:

1. Deploy the built media to **actual** hardware. QEMU, unless it's the only
   available target, doesn't cut it.
2. Get to prompt.
3. If the target has graphical capabilities, run `f<< bench/gr.fs grall`.
4. If the target has graphical capabilities, run
   `f<< oberon/sys.fs oberon` then quit through System.Quit. On systems without
   a 3-buttons mouse, you might have to quit through breaking [doc/break] or
   through powering the machine off.
5. Run `f<< tests/all.fs`.

### Small media testing procedure

Some target media, such as floppy disks, aren't big enough to hold all of Dusk
in them. Those deployments don't include tests. QA for them consists of
reaching prompt.

## Firmware

Some deployment targets such as the Raspberry Pi need opaque binary blobs on
their deployment media. The Dusk repository does not contain them. However, it
contains download scripts for them, which are implicitly called by makefiles.

## Requirements

Same as Dusk: Make and a C compiler.

For deployments needing firmware downloads, you need `curl`.

## Build

Go in the directory corresponding to the platform you want to deploy on and
refer to its `README.md`. In general, `make clean all` will produce some kind of
image that you can `dd` into a media.

Some deployments have a QEMU wrapper which you can generally call with
`make emul`. Deployments having a QEMU wrapper are marked with `(QEMU)` in the
deployment list.

## Deployment list by architecture

### i386

* `efi`: Any EFI-compliant system (QEMU)
* `pc`: BIOS-based systems (QEMU)

### AMD64

* `efi`: Any EFI-compliant system (QEMU)

### ARM

* `rpi`: Raspberry Pi (models 1, 2, 3) (QEMU)
* `sunxi`: Pine A64 (WIP)

### RISC-V

* `riscv`: A dummy virtual QEMU machine

### m68k

* `m68kvirt`: A dummy virtual machine made for running Dusk's m68k port.
* `mac68k`: Theoretically any Macintosh with a m68k CPU, but in reality, a
Powerbook 520.

## Test Matrix

Alphabetic order, Deployment then Context.

| Deployment    | Context       | Version | Tested By         |
| ------------- | ------------- | ------- | ----------------- |
| efi           | amd64         | v31     | Virgil Dupras     |
| efi           | i386+uga      | v31     | Virgil Dupras     |
| m68kvirt      |               | v31     | Virgil Dupras     |
| mac68k        | Powerbook 520 | v31     | Virgil Dupras     |
| pc            | ahci+vesa     | v31     | Virgil Dupras     |
| pc            | alix          | v31     | Virgil Dupras     |
| pc            | ata+vesa      | v31     | Virgil Dupras     |
| pc            | floppy        | v31     | Virgil Dupras     |
| riscv         |               | v31     | Virgil Dupras     |
| rpi           | RPi 0W        | v20     | Tyler Quiring     |
| rpi           | RPi 02W       | v20     | Tyler Quiring     |
| rpi           | RPi 1b        | v31     | Virgil Dupras     |
| rpi           | RPi 3         | v31     | Virgil Dupras     |
| sunxi         | Pine A64 LTS  | v31     | Virgil Dupras     |
