# Dusk OS changelog

Because Dusk OS is designed to mainly be used after civilizational collapse, the
idea of having versions for it doesn't make much sense. After all, the only
version the operator will ever need is the latest she manages to have before she
makes it hers.

With the advent of [Usermode Dusk][usermode], there's a possibility of using
Dusk as an applicative platform that could become a champion for simplicity and
efficiency in the disgusting world of modern software. With some dexterity,
that could be done without jeopardizing original design goals. That's an
interesting prospect.

[usermode]: usermode/README.md

Targeting git commits is awkward as you're always on a moving target, so for
this applicative platform to work, it's better if there's some kind of
versioning in Dusk.

This versioning is not what we commonly call "semantic versioning". It's a
simple integer version being bumped up at semi regular intervals at moments
where things are relatively stable. Something like a release per month sounds
good.

There's never a sub-numbered version. If a particular version turns out to have
a big bug in it, we simply release a new version quickly.

There is no particular effort to maintain backward compatibility of the API. We
don't change it gratuitously, but we don't maintain compatibility layers either.
If you look the the history of the project, you'll see that some APIs have been
there for longer than others, indicating the likeliness of their future change.

The changelog below isn't meant to be a mirror of the git commit. Minor bug
fixing isn't indicated. The goal of this changelog is to facilitate transition
of dependent code to a subsequent version. It therefore lists API addition,
change and removal.

A good way to know more about the context of a particular changelog entry is to
use `git blame` and look the the commit that added this entry. Dusk's git commit
messages often contains the context and rationale to a particular change.

Each version has a PGP-signed tag (example `v42`) in the git repository.

## v1 - 2024/01/03

* Initial release.

## v2 - 2024/01/25

* Remove RPi build scripts (but keep kernels). These now live in
  `duskos-deployments`.
* [lib/bit][]: add `bitflag`.
* `asm/halo`: Introducing the HAL Overdrive
* Add `to&` `to|` and `to^` as well as their companion `and!` `or!` and `xor!`.
* Remove DuskBSD. I hit significant roadblocks in its development and the amount
  work is required to make it work outweighs the value of the tool. I give up.
* pc
    * add [drv/pc/int10h.fs], a new driver for emitting through the BIOS.
    * Change approach to PIC preservation during int13h/int10h calls. Instead of
      doing a $08/$70 <--> $20/$28 remapping on each call (very bug prone,
      especially if the PIT is activated), we copy entries for the IVT during
      setup and only map to $20/$28 once. See [doc/hw/i386/pc/kernel].
    * Set the PIT on mode 2 instead of mode 0.
* Make range words binary modulable, thus changing their semantics. See
  [doc/usage/lit]. To keep previous semantics, those changes have to be made:
    * `move` --> `cmove`
    * `fill` --> `cfill`
    * `[]=` --> `c[]=`
* usermode:
    * Separate "kernel" from "payload".
	* Replace the `callback` struct with a `interopzone` struct which lives in
      Dusk's memory space.
    * The wrapper now has the ability to supply any payload address through boot
      args.
    * Memory size for Dusk is now dynamic, supplied through boot args.
    * Hand over `main()` control to Dusk packages, giving them much more
      flexibility.
    * `common_main()` has been split in two: `common_setup()` and `common_exec()`.
    * There are now 256 `cbfuncs` slots.
	* It's now possible to "freeze" executable for faster startup time.
    * Add `StdIO` struct to common API.

## v3 - 2024/02/23

* core:
    * add `ll>data`.
    * add `cells/`.
* [hal][]:
    * add `s>>,` operator.
    * `*,` is now guaranteed to preserve the S register.
* [asm/dis][]: new unit
* `asm/halm`: Rename "HAL Overdrive" to "HAL Macros", along with lessened
  ambitions. The most ambitious ideas I had for HALO turned out to be
  problematic and the name "Overdrive" becomes a misnomer. It's now only a
  collection of useful HAL-related macros.
* [comp/c][]:
    * Complete overhaul of the expression resolution mechanism and `CDecl` (now
      `CType` and `Symbol`) structure. Regressions might pop up here and there,
      but otherwise it's a drop-in replacement.
    * Typecasting is now stricter and needs to be done more explicitly. For
      example, if "c" is a char, "c << 8" will always yield 0. "(int)(c << 8)"
      needs to be typed. Comparison operators need matching types.
    * Allow creation of incomplete structs, thus allowing circular references
      in structs.
    * Reverse the order of arguments in calling convention. Previously, last
      argument was top of PS, now, first argument is top of PS. This affects
      only calls from C to Forth and from Forth to C. The API for a C function
      calling a C function stays the same.
    * Change semantics of `fread()` and `fwrite()` to target `IO :read` and
      `IO :write` rather than `IO :read#` and `IO :write#`. In effect, it
      transforms their `void` return type into `int` and they don't abort
      anymore on partial read/write.
    * The ">>" operator on signed integers now does an arithmetic shift right.
    * Add the `#const`, `#forth` and `#include` pre-processor directives.
    * Replace `calias` with `#calias`, `#calias16`, `#calias8` and `#caliasns`.
    * Change lifetime rules of types, symbols and constants. They are now
      entirely cleared before compiling a C unit. This brings DuskCC closer to
      the "traditional header #include" pattern. See commit message for
      rationale.
* [comp/c/lib][]:
    * add `uint allot(uint n)`
    * add `uint allot0(uint n)`
    * add `void memcpy(void *dst, void* src, uint n)`
* [comp/tok][]: Change semantics of "tonws<". When no WS char is consumed before
  yielding `c`, `lastws` is `LF` instead of `0`.
* `lib/accel`: Move the "Core words accelerator" part of the late `asm/halo`
  into a more aptly named unit.
* `lib/exec`: new unit
* [lib/fmt][]: add `sprintf`.
* [lib/math][num/math]: add `log2mod`.
* [lib/psrs][]: new unit.
* [lib/str][]: add `startswith?` and `endswith?`.
* `lib/tree`: remove
* `/mem`: new top level directory, with these units moved to it:
    * `lib/alloc`
    * `lib/arena`
    * `lib/array`
    * `lib/dict`
    * `lib/here`
    * `lib/ll`
    * `lib/malloc`
    * `lib/scratch`
    * `lib/stack`
* [mem/alloc][]: add `Allocator :allot0`
* `posix/fd`
    * Change the `DataIO` into a parametrizable `FDIO` and create a new `stdio`
      structbind that wraps FDs 0 and 1.
    * Have this new wrapper replace error (`-1`) return values from `fdread` and
      `fdwrite` to zero.
* `mem/endian`: new unit.
* [sys/file][fs/core]: add `Path :exec<` and `exec<<`.
* [sys/io][io/stream]: Change the semantics of `:read` and `:write`. To keep
  previous semantics, those changes have to be made:
    * `:read` --> `:read#`
    * `:write` --> `:write#`
* usermode
    * Remove the newly added `stdio` structbind in favor of the one now included
      in `posix/fd` (which usermode already includes).
    * Expose the `wantstofreeze` global variable in `common.h`.
    * Have `ABORTPTR` hook to `bye` by default. This doesn't affect interactive
      mode, which uses `sys/rdln` which rehooks `ABORTPTR`.

## v4 - 2024/03/21

* core: add `showprogress` and `?progress>`.
* [hal][]: add "field+)"
* [asm/arm][]
    * Add a big bunch of convenience macros.
    * Add `mcrr)`.
* [comp/c][]
    * Enforce strict type matching all binary operators, return statement and
      function call arguments.
    * Bring Forth's immediate superpowers to C with `#immediate`!
    * Perform sign extension when typecasting a signed type to a larger type.
    * Add `#if/#else/#endif` directives.
* [comp/c/lib][]
    * Add `strequal()` and `assert()`.
    * Move formerly built-in `pspush()` and `pspop()` into stdlib.
    * Add immediate function `nbelem()`.
    * Add `DSTYPE/DPRINTF/DCPRINTF` macros.
* [comp/tok][]: add `n>tok`.
* [drv/rpi/break][]: new unit (and concept, see doc/break)
* [drv/rpi/emmc][]: add write support
* [drv/rpi/gpio][]: new unit
* [drv/rpi/intr][]: new unit
* [drv/rpi/pwr][]: new unit
* [drv/rpi/screen][]: new unit
* [drv/usb][]: port USB stack from Plan 9.
* [drv/usb/kbd][]: replace old implementation with port from Plan 9.
* [drv/usb/uhci][]: temporarily remove. Will soon replace with Plan 9 port.
* [lib/macro][]: add the `%-` argument placeholder type.
* [lib/math][num/math]
    * Add `log2#`, `roundpow2`, `sex8` and `sex16`.
    * Add `/lib/math.h` to expose some words to C.
* [mem/alloc][]: add `:alignto`, allowing allocators to align to something else
  than 4 bytes.

## v5 - 2024/04/10

* core:
	* Make `INSZ` cell into `insz` value and add `inptr` @value to `INPTR`.
	* Lower kernel requirements for `in<` and `word`.
	* Change `fstructbind` signature from `( "structname" "bindname" -- )` to
      `( "fieldname" "structname" "bindname" -- )`.
    * Make `ARCH` into an integer constant described in `doc/kernel`.
    * Guarantee the 3 bytes before the dictionary length field to be stable,
      that is, zero-padded if the name is shorter than 3 characters.
    * Move `alignhere` from `xcomp/boot` to kernels.
* [hal][]:
    * add `popexit,`.
    * change arguments of `branch! ( tgt br -- )` to `branch! ( br tgt -- )`.
* [drv/arm/exc][]: new unit.
* [drv/arm/psr][]: new unit.
* [drv/arm/sccp][]: new unit.
* [emul/oberon][]: [new unit][dusk-oberon]
* [fs/fat][]
    * Merge with `fs/fatlo`, which disappears.
    * Move `newFAT` and related words to the new `fs/fatt`.
* [fs/fatt][]: new unit.
* [gr/color][]: add `rgb888>rgb565 ( n -- n )`.
* `gr/plane`: make `blt`, `bltfill` and `bltpixel` into proper methods.
* [lib/arch][arch/core]: new unit.
* `lib/drivelo`: merge contents with `sys/file`.
* [sys/file][fs/core]
    * Simplify the boot process of systems involving filesystems.
    * Lower the API requirements for structs extending `File`.
    * Replace `Filesystem :iter` with a simpler `Filesystem :children`.
    * Replace `Path :iter` with `Path :children`.
    * Add `Path :bi`.
* [sys/kbd][io/kbd]: complete rewrite. See `doc/sys/kbd`.
* `sys/ps2`: move to [drv/ps2][] and have it follow the new `sys/kbd` API.
* `wasm`: Remove and put in [its own repository][dusk-wasm]
* `xcomp/boot`: Move `IO` struct to `sys/io` and `File`, `Filesystem` and
  `FSInfo` to `sys/file`. `xcomp/boot` is now IO-less and FS-less.

[dusk-wasm]: https://git.sr.ht/~vdupras/dusk-wasm
[dusk-oberon]: oberon/README.md

## v6 - 2024/05/13

* big changes:
    * add [special treatment][usage/io] for double quotes (`"`) and mustache
      (`{`) in word parsing logic. This means, for example, that the string
      literal previously constructed as `" foobar"` is now constructed as
      `"foobar"`.
    * remove "to" semantics and replace it with Big Moustache.
    * remove binary width modulation in favor of moustaches.
* core:
    * added words:
        * `consts`
        * `wordorquote`
        * `variable`
    * removed words:
        * all `field` words
        * `nscompile`
        * `structbind` and `fstructbind`
        * `rebind'` and `[rebind']`
        * `[rallot]` and `[rfree]`
        * `struct[` and `]struct`
        * `@@+` and `@!+`
        * `and!`, `or!` and `xor!`
        * `1+!` and `1-!`
        * All `to` words except the straight `to`
        * `'"` and `-cmove,`
    * add `PSSZ` sysvar and have `stack?` check for PS overflow.
    * move `HEREMAX` out of `SYSVARS` and into a regular cell in `xcomp/boot`.
    * move `scnt` and `rcnt` from [lib/diag][] to `xcomp/boot`.
    * add the concept of "quit hooks". See [usage/flow][].
    * replace `"<` `litrepl` and `litrepl?` with `str<`.
    * rename `S"` to `"` and `,S"` to `,str"`.
    * move `STR_MAXSZ` and `strmove` from [lib/str][] to `xcomp/boot`.
    * replace `MAXWORDSZ` with `STR_MAXSZ`.
    * move endian-ness related words to `mem/endian`.
    * change `echoin` and `'~` values into variables.
* [hal][]:
    * add `ifz,` and `ifnz,` macros.
    * remove `[@+],`, `[!+],` and `(split`.
* [comp/c][]:
    * macros arguments can now optionally be placed inside parentheses.
    * remove `CType :export`.
* [drv/pc/ioport][]:
    * add the `p@{}` and `p!{}` moustache actions.
    * remove the "to-obeying" `ioport` in favor of the new moustaches.
* [drv/usb][]:
    * Rename a few C structs and members:
        * `Usbdev --> IDev`
        * `Ep --> IEp`
        * `KEp --> Ep`
        * `Dev.usb --> Dev.info`
* [fs/fatt][]: replace `fatops` structbind with `FatOpts` namespace containing
  moustache fields.
* `lib/bm`: new Big Moustache unit.
* `lib/exec`: move contents to `xcomp/boot`.
* [lib/drive][io/blk]: remove unused `DriveIO`.
* [lib/macro][]:
    * now uses `wordorquote` for argument parsing.
    * rename `macro"` to `macro`.
* [lib/math][num/math]:
    * replace `roundpow2 ( n pow -- n )` by the more general
      `roundup ( n div -- n )`.
    * add `rounddown ( n div -- n )`.
    * add `abs ( n -- n )`.
* [lib/memfile][mem/stream]: new unit
* [lib/psrs][]:
    * add `ps[] ( ... n -- ... a u )`
    * add `ndrop ( ... n -- )`
    * remove `PS1` to `PS4` in favor of their moustache counterparts.
* [lib/secld][io/secld]: don't depend on the `Drive` struct.
* [lib/str][]:
    * add `bounds[] ( a u -- lo hi )`.
    * rename `intersect[]` to `cintersect[]` without changing semantics.
    * add `swap[] ( a u -- )`.
* [lib/wordtbl][]:
    * replace `wordtbl`, `:w` and `'w` semantics with `wordtbl[`, `]wordtbl` and
      `:>`.
    * add `lookuptbl[`, `]lookuptbl` and `?lexec`.
* [sys/file][fs/core]:
    * remove the "path drive letter" concept.
    * remove the `Path` struct.
    * add `Filesystem finddir` and `Filesystem :exec<`.
    * plug `f<<` and `?f<<` on `Filesystem :exec<` thus allowing dependency
      declarations in units to use relative paths.
    * move to `Drive` struct from `xcomp/boot` to here.
    * remove `FSInfo` and change `Filesystem :info` semantics.
    * decouple `File` from `SectorWindow`.
    * move `SectorWindow` (back!) to [lib/drive][].
* [sys/io][io/stream]:
    * remove unused `SumIO` and `SerialIO`.
    * remove `MemIO` in favor of [lib/memfile][mem/stream].
    * `IO` structs are not seekable anymore, nor do they have a size. It is now
      a `File` property.
    * remove unused `IO :readall`.
* [sys/kbd][io/kbd]:
    * add `:melt ( nkc self -- nkc )`
* [text/gedx][app/edx]: new unit

## v7 - 2024/06/03

* big changes:
    * Add the concept of [unit][usage/unit].
    * "de-namespace", that is, remove the namespaces around the structures, in
      the new spirit of "unit bubbling", for the following units:
        * [ar/tar][]
        * [emul/cpu][]
        * [emul/6502][]
        * [emul/virtio][]
        * [fs/core][]
        * [fs/tar][]
        * [fs/fat][]
        * [fs/fatt][]
        * [fs/memfile][mem/stream]
        * [io/grid][]
        * [io/secwin][]
        * [io/stream][]
        * [text/ed][]
* core:
    * add `readline<` and `readline<#`
    * add `[]>r` `r>[]` `str>r` `r>str`
* usermode:
    * add the "raw" flavor
* [hal][]:
    * add `dropf,`
* [asm/armd][]: new unit
* [io/drive][io/blk]: new unit with contents extracted from [fs/core][]
* `lib/bm`:
    * add `=` `<>` `<` `>` `<=` and `>=` actions
    * remove `smethod`
    * add `k` (keep) modifier
* `lib/context`: remove. `needs` rendered it useless
* [lib/drive][io/secwin]: rename to `io/secwin`
* [lib/memfile][mem/stream]: rename to `fs/memfile`
* [lib/secld][io/secld]: rename to `io/secld`
* [lib/str][]:
    * replace `rmatch` and `rfind` with `rmatch"` and `rfind"`
* [mem/dict][]:
    * change `forget` semantics
    * add `unitofentry`
* [sys/file][fs/core]:
    * rename to [fs/core][]
    * remove "floaded" mechanism and replace `?f<<` with `needs`
    * remove `file` bind
    * add `closeall`
    * remove unused `drv` and `flags` filesystem fields
    * replace `findfreecursor` with `?reusefile`
    * remove unused `newcursor` filesystem method
    * add `lookuprel`
    * remove `curdir`, `finddir` and `fsexec`
* [sys/grid][io/grid]: rename to `io/grid`
* [sys/io][io/stream]:
    * rename to `io/stream`
    * add `exec1<`
    * change `readline` semantics to fit `readline<`
    * add `readline#`
* [sys/kbd][io/kbd]:
    * add control character support to the `:melt` process
* [text/ed][]:
    * replace `edload` with `edload<<` and add `edsaveto`
    * add multiple buffers support
    * add `wordundercursor`
* `text/help`: new unit
* [text/ts][]: new unit

## v8 - 2024/06/26

* core
    * add `addquithook` `scry` `?abort"`
    * change semantics of `'~` `bubbleup` `cmove"`
    * change booting sequence so that it doesn't end with `init`, but `quit`
    * remove `llfindprev` in favor of `llfind` moved from [mem/ll][]
    * move `s,` from [lib/str][] to core
    * allow [units][usage/unit] to be nested
    * move `readline<` up to [io/stream][]
    * rename `-^` to `swap-`
    * replace `:compiling` with [compileonly][usage/imm]
    * remove entry metadata system
* hal
    * add `swap-,`
    * remove `-W,`
    * add `if*` (example `if=`) and `while*` macros
* [fs/core][]
    * now depends on [lib/str][]
* [gr/cursor][]: De-namespace `Cursor`
* `gr/plane`: De-namespace `Plane`
* `gr/rect`: De-namespace `Rect`
* [io/stream][]
    * change `readline<` semantics slightly
* `lib/bm`
    * add `n`, `s` `~` and `@` modifiers
    * add `[]` action
    * add `){`
    * rename `-^` action to `swap-`
    * remove the `@Ainc` action
    * restrict interpret-mode support to only `@` `!` and `@!` actions
    * formalize "low level" API better for better mix-and-match of moustaches
      and HAL code
* [lib/match][]
    * new unit extracting `rmatch"` and `rfind"` from [lib/str][]
    * add `=anyof"`
* [lib/str][]
    * replace static `strbuf` with the concept of "string pool"
    * remove unused `strmove"`
    * add `str>pool`
* `mem/endian`
    * preserve the A register throughout the unit
    * add `lib/bm` actions `le@{}` `le!{}` `be@{}` and `be!{}`
* [mem/ll][]
    * change `llitern` semantics
    * move `llfind` to core, also changing its semantics
* [io/stream][]
    * now depends on [lib/str][]
* [sys/kbd][io/kbd]
    * rename to `io/kbd`
    * De-namespace `Keyboard` and `Keys`
* [sys/mouse][io/mouse]
    * rename to `io/mouse`
    * De-namespace `Mouse`
* [sys/rdln][app/prompt]: rename to `io/prompt` and change semantics
* `sys/screen`
    * rename to `io/screen`
    * De-namespace `Screen`

## v9 - 2024/07/31

* core
    * replace `findentrycomp` with the `FINDMASK` sysvar
    * replace `runword` with `run1`
    * add `entry[]` and `samecode`
    * add [setters][usage/word] mechanism
    * remove unused `compword`
    * add [NEWHERE reserve CURALLOC][usage/mem]
    * replace `SYSCONTEXT` and `CURCONTEXT` with `SYSDICT`
* posix+usermode:
    * add the `-e` `-c` `-f` command line flags
    * remove the `dataio` binding
* [hal][]
    * have `&)` implicitly call `32b)`
    * rename `branch,` to `br,`, `branchC,` to `brc,`, `branchR,` to `brr,` and
      change semantics
    * remove `branchA,` which is replaced by `A) &) br,`
    * add `fbr,` `fbrc,` `bbr,` `bbrc,` and `execute,` macros
* [drv/pc/ioport][]
    * add `ioport{}`
    * remove runtime ability of `p@{}` and `p!{}`
* [drv/usb/kbd][]
    * replace Plan 9 port with a homegrown Forth driver.
* [drv/usb/uhci][]
    * re-add unit removed in v4. It's not from Plan 9 after all, it's homegrown.
* [fs/core][]
    * add `?child` and `?lookup`
    * change `?newfile` and `?newdir` semantics
* [io/drvstr][io/blk]: new unit
* [io/grid][]
    * make all words implicitly target the `grid` global value.
    * the grid struct is no longer a stream
    * add `gridstream`
    * remove `spitpos` `spiton` `spitoff`
* [io/kbd][]
    * move keyboard layouts to new data dir `data/kbdl`
* [io/secwin][]
    * make sector windows comply to [io/drive][io/blk] semantics
    * move the "buffer" part of the unit to [io/drvstr][io/blk]
* [lib/bit][]
    * replace `&bit>` and `&bit<` with `bitfield`
* `lib/bm`
    * have `++` copy over indirection field
    * remove `to` implementation in favor of the new "setters" in core
    * remove `bind{}`
    * add `}value`
* `lib/ns`: remove unit
* [mem/alloc][] [mem/scratch][] [mem/arena][]
    * complete rewrite around `CURALLOC`
* [mem/array][]
    * de-namespace
* [mem/dict][]
    * move `e>wlen` `entrylen` and `entryname[]` to core
* `mem/endian`
    * add `littleendian{}` and `bigendian{}`
    * remove runtime ability of `le@{}` `le!{}` `be@{}` and `be!{}`
* [sys/timer][drv/timer]
    * add `?timeoutus"` and `?timeoutms"`

## v10 - 2024/08/21

*If Forth and Lisp had a baby, would they name it Alia?*

* [hal][]
    * add `C)` `NC)` `ifc,` `ifnc,`
    * have `<<,` `>>,` `s>>,` set the C and Z flags
* [comp/lisp][]: new unit
* [comp/tok][]
    * add `isChar?`
* [drv/sunxi/smhc][]: new unit
* [io/grid][]
    * add `pcell!+`
* [lib/psrs][]
    * add `rs[]`
* [mem/cons][]: new unit

## v11 - 2024/09/24

* core
    * move string pool from [lib/str][] to core
    * remove `,[` `,]` `,str"`
    * add `newstr` `endstr` `strallot`
    * rename `cmove,` to `cmoveallot`
* [hal][]
    * add `fill,` `move,` `[]=,` `idx,`
* [drv/pc/vesa][]
    * remove VESA 1.2 implementation
* [fs/core][]
    * replace `?lookup` with `ensurepath`
    * change semantics of `?newfile` and `?newdir`
* [gr/color][]
    * add `colorwriter` `colorbytes`
* [gr/cursor][]
    * add `cursor` `handlemouse` `showmouse` `hidemouse`
* [gr/damage][]: new unit
* `gr/font/cos`: new unit
* [gr/pix][gr/buf]: new unit
* `gr/plane`: remove, superseded by [gr/pix][gr/buf]
* `gr/pmap`: new unit
* `gr/rect`: remove unit
* [gr/varvara][]: new unit
* [io/grid][]
    * make the grid buffered
    * add `scroll` and `rawcell!`
* `lib/bm`
    * add `offset{`
* [lib/wordtbl][]:
    * add `code>`

## v12 - 2024/10/21

* core
    * add Dynamic SYSVARS concept
    * replace "compileonly" and "setter" with "findselector"
* `asm/halm`: split to [hal/opq][] and [hal/muldiv][]
* [drv/arm/cache][]: new unit
* [drv/pc/vesa][]
    * change API so it fits well with `io/screen` mode management
* [fs/core][]
    * add `lookuprel#` and `loadpath`
* [fs/memfile][mem/stream]
    * add `writtenrange`
* [fs/search][]: new unit
* `gr/font/cos`: remove unit
* [gr/font][]: new unit
* `gr/font/ufx`: new unit
* `gr/icn`: new unit
* [gr/pix][gr/buf]
    * replace `damageptr` with `invalidate`
    * add `bufsz` `pixsrcdst!` `drawnrect` `?resize` and `resize#`
    * change semantics of of `mappixels` and `newpixbuf`
* `gr/pmap`: remove unit
* [hal][]
    * add `sys)`
* `hal/bwr`: new unit
* [hal/opq][]: new unit
* [hal/ops][hal/instr]: new unit
* [hal/muldiv][]: new unit
* `hal/range`: new unit
* [hal/vreg][]: new unit
* [io/fbgrid][gr/grid]
    * use [gr/font][] to draw any font, not just 8x8 cells
* [io/drvstr][io/blk]
    * rename `seek` to `window`
* `io/screen`
    * add "Mode management" concept
* [io/stream][]
    * add `nullstream`
* [lib/arch][arch/core]
    * add `familyname`
* `lib/bm`
    * move the `[]` action to [hal/muldiv][]
* [lib/str][]
    * extract range-related words to [mem/range][]
* [mem/range][]: new unit
* `sys/timer`:
    * rename to [drv/timer][]
    * add `ifelapsedus` and `ifelapsedms`
* [text/ed][]
    * the convenience layer doesn't implicitly call `s` anymore
* `text/help`
    * change `helppath` semantics
* [xcomp/i386/pc/deploy][]: new unit
* [xcomp/arm/rpi/deploy][]: new unit

## v13 - 2024/11/09

* core
    * have kernels implement `parsehex` instead of `parsedec`
    * rename `<<` `>>` `cells` `cells/` to `2*` `2/` `4*` `4/`
    * remove `cell`
    * add `2+` `2-` `4+` `4-` `8+` `8-` `8*` `8/`
    * rename `^` to `inv`
    * add `invand`
    * remove ability for units to nest
* posix VM
	* Complete rewrite. Instead of having its own frankenstein bytecode, it now
	  partially emulates ARMv4 and its HAL generates the same code as ARM
      kernels.
    * Add "curses" and "sdl" flavors. Exactly like in usermode, but easier to
      build, albeit slower.
* [asm/arm][]
    * rename `rTOP` to `rW`
* [bin/gbe][app/bed]: new unit
* [comp/c][]
    * change `#include` semantics
    * move `comp/c/cc.fs` to `comp/c.fs`
* `drv/ata`: remove NetBSD ATA driver port
* [drv/pc/ata][]: bring back from the dead
* [drv/timer][]
    * add `snooze`
* [drv/usb][]
    * move `drv/usb/usb.fs` to `drv/usb.fs`
    * remove sub-unit usage in `drv/usb/struct.fs`
* [gr/color][]
    * add `rgbwhite`, `rgbblue` ... constants
    * add `rgbdarker` `rgblighter` `colormaker`
* [io/fbgrid][gr/grid]
    * change `fbgrid$` semantics
* [io/grid][]
    * de-structify the grid. `newgrid` becomes `grid$`
    * stop implicitly calling `cursor!` on `gridemit`
    * add `defcursor!` `clear` `at-xy`
    * remove `gridstream` `pcell!` `pcell!+` `highlight!` `grid` `grid#` `pos`
      `linefeed`
    * add the concept of per-cell colors
* [io/prompt][app/prompt]
    * add `waitkey`
* `io/screen`
    * de-structify screen
    * change `newscreen` into `screen$`
    * add `>screencolor`

## v14 - 2024/11/28

* core
    * add [RISC-V][hw/riscv] support!
    * add [AMD64][hw/amd64] support!
    * Bring back [deployments](deploy/README.md) in Dusk itself
* [asm/i386][asm/x86]
    * rename to `asm/x86`
    * add 64-bit mode support
    * add SIB support as `r+)` `2r+)` `4r+)` and `8r+)`
    * split `jmp,` into `jmp,/jmpr,`
    * split `call,` into `call,/callr,`
* [asm/i386d][asm/x86d]
    * rename to `asm/x86d`
* [asm/riscv][]: new unit
* [hal][]
    * remove `C)` `NC)` `ifc,` `ifnc,`
    * rename `pushret,` to `pushlr,` and `popret,` to `poplr,`
    * add `popret`
* [hal/bit][lib/bit]: new unit
* `hal/shim`: new unit

## v15 - 2024/12/27

* core
    * add `align`
    * rename `PSTOP` to `PSORIGIN`
    * rename `RSTOP` to `RSORIGIN`
* [deploy/efi][]: new deployment
* [drv/efi][]: new unit
* [drv/efi/blkio][]: new unit
* [drv/efi/devpath][]: new unit
* [drv/efi/gop][]: new unit
* [drv/efi/grid][]: new unit
* [drv/efi/image][]: new unit
* [drv/efi/kbd][]: new unit
* [drv/efi/uga][]: new unit
* [gr/color][]
    * rename `COLOR_RGB888` to `COLOR_RGB24`, `>rgb888` to `>rgb24`,
      `rgb888>rgb565` to `rgb24>rgb565`
    * add `COLOR_RGB32`
* [io/drive][io/blk]
    * add alignment requirements for src/dst buffers
* [io/stream][]
    * add `herestream`
    * add `spitclose`
* [mem/alloc][]
    * rename `alignn` to `alignheren` and move it to core

## v16 - 2025/01/20

* Remove Collapse OS which now lives in its own repository
* core
    * items below can be summarized as introducing the concept of "rest of line"
    * replace `PREVWS` logic with `peekback`
    * add `eol?` (see [usage/io][])
    * change `r>str` semantics
    * add `sysdict`
    * change `consts` semantics
    * add `enum`
    * remove `nc,`
    * add `n<` and `map<`
* [asm/6502][]: copy from Collapse OS
* [asm/arm][]
    * rename `8b)` to `byte)` to avoid shadowing HAL words
* [asm/x86][]
    * renames to avoid shadowing HAL words:
        * `i)` to `imm)`
        * `m)` to `abs)`
        * `8b)` to `byte)`
        * `16b)` to `word)`
        * `32b)` to `dword)`
* [fs/core][]
    * change semantics of `lookup` `lookup#` `openpath` `loadpath` `p"`
    * add `mapfs` and `pathfs`
* [hal/ops][hal/instr]
    * rename to [hal/instr][]
* [deploy/mac68k][]: new unit (WIP)
* [deploy/pc][]
    * rename from `deploy/pc-piix` and merge with `deploy/pc-bios`
    * add a `floppy.img` target for booting from 1.44M floppy
* [drv/efi][]
    * add `reboot`
* [drv/efi/kbdex][]: new unit
* [drv/efi/mouse][]: new unit
* [drv/efi/timer][]: new unit
* [drv/pc/fdc][]: full rewrite, essentially a new unit
* [lib/str][]
    * change `stringlist` semantics
    * remove unused `expectchar` and `toword`
* [io/grid][]
    * add `poscnt`
* [io/kbd][]
    * remove `KBD_EVENT_` prefix to event constants
* [io/stream][]
    * change `readline<` semantics
* `lib/bm`
    * add support for the `k` flag to the `@!{}` action
    * custom actions are now also applied during `@{}` and `!{}`
    * change `}value` semantics

## v17 - 2025/03/01

* [m68k][hw/m68k] port complete!
* core
    * unit words are no longer immediates
    * remove `and?` `or?`
    * rename `=><=` to `within?`
    * rename `e>w` `w>e` to `e>xt` `xt>e`
    * add `formatdec` `formathex` `formathex2` `formathex1`
    * move `create` from `xcomp/boot` to kernels
    * remove `for` `for2` `next`
    * add `do` `loop` `+loop` `-loop`
* [ar/tar][]
    * remove the "record navigating" part of the API, keeping only the record
      parsing part.
* [ar/tarp][]: new unit
* [asm/m68k][]: new unit
* [com/xmodem][]: new unit
* [deploy/mac68k][]: still a WIP, but has prompt and grid!
* [drv/pc/com][]
    * rename `com>?` to `?com>` and change semantics
* [fs/core][]
    * change `child` `?child` `newfile` `newdir` `ensurepath` semantics
    * rename `?newfile` `?newdir` to `ensurefile` `ensuredir` and change
      semantics
* [hal][]
    * remove `ifXX` and `whileXX` immediate macros
* [io/prompt][app/prompt]
    * change semantics slightly to allow usage on the barest of systems
* [io/stream][]
    * add `spitcloseboth` and `spitn`
* `lib/bm`
    * remove `offset{}`
    * remove `create{}`
    * remove `method{}`
    * remove `@` `++` `>>` `'@` and `~` modifiers
* [lib/crc][num/crc]
    * add support for `CRC16-XMODEM`
* [lib/str][]
    * add `lowcase` `strings<` `z[]` and `c[]`
    * remove `rfor`
    * add `do[]`
* [lib/struct][]: new unit (well, resuscitated)
* `mem/endian`
    * remove `littleendian{}` and `bigendian{}`
    * add `fieldle` `fieldwle` `fieldbe` `fieldwbe`
* `sys/loop`: rename to `lib/loop`
* `sys/replay`: rename to `io/replay`
* [text/clip][]
    * add `clipset`
* [xcomp/tools][]
    * add `orgifydictbe`

## v18 - 2025/04/04

* core
    * new concept: [tagged addresses][usage/tag]
    * change `EMIT` mechanics for `RTYPE` ones
    * remove unused `ztype`
    * rename `bi+` to `dupbi`
    * change "scry" semantics
    * move `le@` `wle@` `be@` `wbe@` `le!` `wle!` `be!` `wbe!` `le,` `wle,`
      `be,` `wbe,` from `mem/endian`
    * rename `insz` to `INSZ`
    * move [setter semantics][usage/to] from `lib/bm` to core
    * move `value` from `lib/bm` to core
    * add `FINDSTR` [sysvar][data]
    * remove `curword` shortcut word. `CURWORD` is still there
* [asm/uxntal][]
    * add the `~ bin` directive.
* [comp/c][]
    * have types and signatures live in permanent memory
    * this means removing `#include` and adding `needsh`
    * replace sequenced expressions system with an AST using [mem/cons][]
    * remove the "immediate" flag for C function signatures
    * have forward declarations be done with `#forward` instead of `static`
* [comp/c/lib][]
    * remove `scanf` `fscanf` `sscanf`
    * remove `pspush()` and `pspop()`
* `comp/infix`: remove unused unit
* [comp/oberon][]: new (WIP) unit
* [comp/sig][]: new unit
* [comp/sym][]: new unit
* [comp/tok][]
    * add `peektok<`
* [drv/pc/ioport][]
    * remove `ioport{}` `p@{}` `p!{}`
    * add `ioport` `ioportw` `ioportb`
* [emul/uxn][]
    * add `[dev@]` `[dev2@]` `[dev!]` `[dev2!]` `[devk!]` `[dev2k!]`
* [hal][]
    * add `le@,` `be@,` and `u@,`
* [hal/muldiv][]
    * add `[*n]` `[*n+]`
    * remove `[]{}`
* [hal/vreg][]
    * add `?saveR0,` `?saveR1,` `?restoreR0,` `?restoreR1,`
* [io/stream][]
    * remove `console`
* `lib/bm`
    * remove custom actions mechanism
    * remove the `s` and `n` modifiers. use [lib/struct][]'s `array` instead.
    * remove `value{}`
    * move `le@{}` `be@{}` `le!{}` `be!{}` from `mem/endian`
    * de-document all "internal" words. `lib/bm` is not meant to be used as a
      "moustache library" anymore.
* [lib/diag][]
    * add `squarespit`
* [lib/str][]
    * add `slistlen`
* [lib/struct][]
    * Structures are now Types from [lib/type][]
    * change `method` semantics
    * move `fieldle` `fieldwle` `fieldbe` `fieldwbe` from `mem/endian`
    * `fieldle` `fieldwle` `fieldbe` and `fieldwbe` are no longer moustache
      fields, but regular getters/setters ("to" semantics).
* [lib/tagl][]: new unit
* [lib/type][]: new unit
* [mem/cons][]
    * rename `decons` `?decons` to `carcdr` `?carcdr`
    * add `cdrcar` `?cdrcar`
* [mem/dict][]
    * add `entrytag` `findtagged` `extractdict` `reserveentry`
* `mem/endian`: remove unit
* [mem/range][]
    * rename `rtrimleft` to `ltrim[]` and change signature
    * rename `rtrimright` to `rtrim[]` and change signature
* [oberon][]: Porting started!

## v19 - 2025/05/01

* [deploy/pc][]
    * merge `pc-alix` into it.
* [comp/oberon][]:
    * Not a WIP anymore. It's still rough around the edges, but all the parts are
      there.
* [comp/tok][]
    * add `err"` and `?err"`
* [comp/w][]: new unit
* [drv/pc/int13h][]:
    * split into two units, `int13hl` (LBA) and `int13hc` (CHS)
* [hal/opq][]
    * add `(&?` `(src` `(dst` `src)` `dst)` `nb)` `.hal`
* [hal/muldiv][]
    * add `modorand,`
* [io/stream][]
    * re-add `console`, but write-only
    * add `gets`
* [lib/arch][arch/core]
    * rename `FAMILY_x86` to `FAMILY_i386`
    * add `isx86?`
* [lib/struct][]
    * add `containsstruct?` `findfield`
* [lib/tagl][]
    * add `createtag` and `settag`
* [lib/type][]
    * add `typealign`
* [lib/wordtbl][]
    * rename `lookuptbl[` `]lookuptbl` `?lexec` to `kvtbl[` `]kvtbl` `?kvexec`
      and move them to [mem/kv][]
* [mem/mark][]: new unit
* [mem/kv][]: new unit
* [mem/pool][]: new unit
* [mem/sort][]: new unit
* [oberon][]: Still a WIP...
* `text/pager`
    * slightly change semantics
* [xcomp/i386/pc/pbr][]: new bootloader

## v20 - 2025/06/14

* core
    * remove unused `:override` 
    * make `~` into a proper findselector
    * remove `'~` which became spurious now that we can do `' ~`
    * add `formatdecu`
    * add `CONSOLETYPE` and `console!`
* [asm/riscvd][]: new unit
* [bin/gbe][app/bed]
    * have the address column display relative offsets
* [hal][]
    * add `testz,`
    * tighten "32-bit arithmetic on 8b) and 16b)" logic
    * rename `<>)` to `dir)`
    * rename `Z)` and `NZ)` to `=)` and `<>)`
    * add `invcond`
    * remove `compare,`
    * replace `brc,` with `?br,`
    * replace `C>W,` with `bool,`
    * add `if,` `?brz,` `?brnz,`
    * replace `dropf,` with `dropz,`
    * `8b)` `16b)` or `32b)` applied after `&)` is now defined behavior
* [hal/opq][]
    * remove unused `nb)`
* [io/grid][]
    * have `clear` also reset position
* `lib/bm`
    * move [local variables][usage/to] to `xcomp/boot`
* [lib/match][]
    * remove unused `=anyof"`
* [lib/type][]
    * add `type)`
* [mem/ll][]
    * remove `ll>data` and `llnext`
* [oberon][]
    * Almost looks like a working system, but still a WIP
* [text/ed][]
    * remove `nextws`
    * add `prevword`
* [text/ged][app/ed]
    * remove `s` keybinding
    * rebind `H` to `C+h` `L` to `C+l` and `w` to `L`
    * add `H` `J` `K` `g` `W` `Z` and `C+q` keybindings
    * add a gutter

## v21 - 2025/07/03

* core
    * slightly tweak string escaping rules
    * add `align#` `e>xtsel` `?scryentry` `scryentry#`
* [comp/c][]: almost a complete rewrite
    * types and structs fully integrated with [lib/type][] and [lib/struct][]
    * can reference any word annotated through [comp/sig][]
    * all functions and symbols are added to sysdict, directly accessible
      through regular Forth semantics
    * add backtick escape for referencing non-ident names
    * `static` is gone
* `comp/c/lib`: remove unit
* [comp/sig][]
    * add `varargs?` `varargs!` `fixedargs!`
    * add `annotate@` `annotate<`
* [comp/sym][]
    * replace `tagglobalsymbol` with `addglobalsymbol`
* [comp/w][]
    * add `?PSP+4` and `?2>W`
* [deploy/sunxi][]: new Pine64 deployment
* [drv/timer][]
    * change initialization semantics
* [hal][]
    * make `+)` non-destructive for HAL bank slots
    * remove `hbank+`
    * remove `wordmark,`
* [hal/opq][]
    * add `signedcond` `swappedcond` `(dir?`
    * rename `hslot` to `(slot`
    * add `slot)` `(bank` `bank)`
* [io/part/mbr][]: new unit
* [lib/type][]
    * add `untyped` `forwardtype` `?addtype`

## v22 - 2025/07/31

* core
    * add `entryalias`
* [gr/color][]
    * change color constants numbering scheme
    * remove `COLOR_` prefix from color constants
    * remove unused `colorwriter`
    * add `colordepthidx` `idx>bpp`
* `gr/cursor`: remove unused unit
* [gr/damage][]: complete rewrite
* [gr/font][]: significantly change semantics
* [gr/font/bit][]: new unit
* `gr/font/ufx`: replace with [gr/font/uf1][] and [gr/font/uf2][]
* `gr/icn`: remove unused unit
* [gr/pix][gr/buf]: full rewrite, rename to [gr/buf][]
* [gr/rdwr][]: new unit
* [hal][]:
    * add `ale@,` and `abe@,`
* `hal/bwr`: remove unused unit
* `io/fbgrid`: rename to [gr/grid][] and change semantics
* `io/screen`: remove unused unit
* [lib/bit][]
    * add `swapbits8`
* `lib/bm`
    * add `){`
* [lib/struct][]
    * all fields now come with a "Prefixed Alias"
* [mem/dict][]
    * remove `dictlink`

## v23 - 2025/09/02

* core
    * add `bind>`
* [asm/uxntal][]
    * add `uxntallen` `findLabel` `findLabel#` `findunused`
* [drv/arm/mmu][]: new unit
* [drv/mac68k/mouse][]: new unit
* [drv/mac68k/screen][]: new unit
* [drv/mac68k/timer][]: new unit
* [drv/pc/com][]
    * rewrite receiving logic to an interrupt-based one
    * remove `>com` `com>` `?com>`
* [drv/rpi/uart][]
    * remove `uart!` `uart@` `uart@?`
    * add `uart0` stream
    * add ability to handle IRQs on receiving data
* [drv/rpi/vcore][]
    * add `getclock` `getmaxclock` `setclock`
* [drv/timer][]
    * add `elapsedus` `elapsedms`
* [gr/buf][]
    * replace `Pixbuf.bpp` with `Pixbuf.depth`
    * add `depth>idx` `depth>bpp` and all the `XBPP` constants
    * change `newpixbuf` and `configurebuf` signature to add `depth`
* [gr/color][]: pretty much a complete rewrite
* [gr/rdwr][]
    * change semantics of the compiler API
* [gr/turye][app/turye]: new unit
* [io/grid][]
    * change how it emits ASCII control characters
    * add `insertlines` `deletelines`
* [io/mouse][]
    * remove `mouse@` and `mouseabs@` methods
    * make `update` into a method
* [io/key][io/kbd]: new unit
* [io/ride][]: new unit
* [io/roll][mem/roll]: new unit
* [io/wait][]: new unit
* [mem/alloc][]
    * rename `activate` to `alloc[`
    * rename `bindactivate` to `bindalloc[`
    * add `]alloc` `sys[`
* [mem/here][]
    * make into a proper allocator [mem/alloc][]
    * remove `here$` `lockhere` `unlockhere` `herefree#`
* [mem/range][]
    * add `split[]`
* [text/ansi][]: new unit

## v24 - 2025/10/03

* core
    * remove unused `align2#` `align4#`
* [comp/oberon][]
    * add `obcode` `:ob`
    * add the backtick operator
    * automatically create Forth type entries for Oberon types
    * automatically create Forth words for Oberon procedures
    * remove `obtype` `obalias`
* [gr/font][]
    * add `getfont` `getfont#` `registerfont` `findfont` `addfontloader`
      `glyphwidth` `glyphwidth,` `widths`
    * rename `Font.width` to `Font.maxwidth`
* [hal][]
    * add `signed)`, working with `?br,` `if,` `bool,` `>>,` `/mod,`
    * remove `s<)` `s>)` `s<=)` `s>=)` `s>>,`
* [hal/opq][]
    * remove `signedcond`
* [lib/struct][]
    * add `fieldt` `fieldst` `fieldtref`
    * `struct` now automatically resolves forward references
* [lib/type][]
    * change `?addtype` semantics
* [oberon/arg][]: new module
* [oberon/system][]
    * add `CompileModule`

## v25 - 2025/11/09

* core
    * add `n,@`
    * remove "curses" POSIX/Usermode flavors
* [comp/oberon][]
    * make `IMPORT` map to lowercase file names
* [comp/sym][]
    * add `savesymstate` `restoresymstate`
* [emul/cpu][]
    * remove `CPU.unhalt` `CPU.hook`
* [emul/virtio][]
    * significantly change how the whole thing runs
* [io/ride][]
    * stop riding when target stream spits ASCII EOT
* [lib/double][num/double]: new unit
* `lib/loop`: rename to `lib/idle`
* [mem/pool][]
    * remove unused `syspool`
* [mem/range][]
    * add `map[]` `wmap[]` `cmap[]`
* [oberon/clipboar][]: new unit
* [oberon/system][]
    * add `RunForth` `RunForthLine`
* [oberon/textfram][]
    * move global selection concept to [oberon/texts][]
* [oberon/texts][]
    * add concept of "global selection"
    * add `AsStream` `SelectionAsStream` `ReadBackwards`

## v26 - 2025/12/10

* core
    * change `r>[]` semantics
    * remove `wmove` `showprogress` `?progress>`
    * make `cmove` "smart" (`cmove,` stays dumb)
* [asm/label][]
    * add `resetasm`
* [asm/m68k][]
    * remove `cpush,`
    * add `movec,`
* [bench/fdc][]: new unit
* [comp/w][]
    * add `?PSP+n`
    * remove `?PSP+4`
* [drv/mac68k/qd][]
    * "downgrade" into a much lower level unit
* [drv/mac68k/xpram][]: new unit
* [drv/efi/grid][]
    * semantics changed due to [io/grid][] rewrite.
* [drv/pc/vga][]
    * semantics changed due to [io/grid][] rewrite.
* [drv/pc/fdc][]
    * add a whole bunch of words to open up the API to FDC's innards
* [fs/core][]
    * move the following fields/methods from `File` to `Stream` [io/stream][]:
      * `pos`
      * `size`
      * `resize`
      * `flush`
    * move `seek` `maxn` `truncate` to [io/stream][]
    * remove `File` structure. Files are Streams now
    * rename `FS.open` to `FS.openfile` ans slightly change its semantics.
      `open` becomes a regular word with the same semantics as before.
    * add `FS.initfilestruct` method
    * add `ensurepathrel`
* [fs/fat][]
    * add `fatstorage!`
* [fs/memfile][mem/stream]
    * rename to [mem/stream][]
    * have it extend `Stream` [io/stream][] instead of `File`
* `hal/range`: remove unused unit
* [io/blk][]: new unit
* `io/drive`: remove unit in favor of [io/blk][]
* `io/drvstr`: remove unit in favor of [io/blk][]
* [io/grid][]: complete rewrite
* [io/secwin][io/part]: rename to [io/part][]
* [io/stream][]
    * downgrade the lifetime guarantees of `readbuf`'s result
    * add `?grow` `fillstream` `seekrel` `rewind1`
* [io/memdrive][mem/blk]: rename to [mem/blk][]
* `lib/idle`
    * add `..`
* [lib/str][]
    * add `32SPCS`
* [mem/malloc][mem/reuse]: move to [mem/reuse][] and change semantics
* [mem/range][]
    * remove unused `bounds[]`
    * rename `cintersect[]` to `intersect[]`
    * add `glue[]`
* [oberon/convertp][]: new unit
* [oberon/draw][]: new unit
* [oberon/dumpfont][]: new unit
* [oberon/fontsubs][]: new unit
* [oberon/growfont][]: new unit
* [oberon/optimize][]: new unit
* [text/ansi][]
    * complete rewrite
    * add ability to encode ANSI escape codes (previously was decode-only)
* [text/clip][]
    * add `clipensure`
* [xcomp/deploy][]
    * replace `copyall1440` with `copylist`
* [xcomp/i386/pc/deploy][]
    * add `copyfloppy`
* [xcomp/m68k/cpusel][]: new unit
* [xcomp/m68k/mac/bootdbg][]: new unit
* [xcomp/tools][]
    * remove `kernel` `kernellen`
    * add `kernel[]` `xcompbegin` `xcompend` `xcomp[]`

## v27 - 2025/12/28

* core
    * slightly change [unit semantics][usage/unit]
    * remove `unitalias` `SYSTEMUNIT`
    * add `needs"`
    * rename `system` unit to `xcomp/boot`
* [app/gcon][]: new unit
* [bench/mouse][]: new unit
* [bin/gbe][app/bed]: rename to [app/bed][]
* [fs/core][]
    * move `copyfile` `copydir` and `listdir` to new [fs/sh][] unit
* [fs/sh][]: new unit
* [fs/utag][]: new unit
* [gr/turye][app/turye]: rename to [app/turye][]
* [io/grid][]
    * add `Grid.pixw` `Grid.pixh`
    * add `xyptr` `xypix` `xypixpos` `line[]` `?showmarker`
* [io/kbd][]
    * change `loadkbdl` semantics
* [io/key][io/kbd]: merge contents into [io/kbd][]
* [io/mouse][]
    * add `xy` `evxy` `Ldown?` `Rdown?` `Mdown?` `Lup?` `Rup?` `Mup?`
      `drawmousecursor`
* `io/replay`: remove unused unit
* [io/search][]: new unit
* [lib/coop][]: new unit
* `lib/idle`
    * change semantics of `idle`
* [lib/str][]
    * add `[]>pool`
* [mem/range][]
    * add `?[]`
* [oberon][]
    * rename `oberonloop` to `oberon`
* [text/ed][]
    * add `bounds`
* [text/ged][app/ed]
    * rename to [app/ed][]
    * add `textsel[]` `zoom`
* [text/gedx][app/edx]: rename to [app/edx][]
* `text/help`: remove unit, moved most logic to [fs/utag][]
* `text/pager` remove unused unit

## v28 - 2026/01/30

* core
    * add `MAINLOOP` `doto` `oover` `compsel` `to'`
    * add `A>` `>A` `A!` `@Ac@` `@A@` `@Ac!` `@A!`
    * replace `getsetter,` with `getset,`, see [usage/to][]
    * remove unused `align2` `wfill` `widx` `w[]=`
    * remove [special treatment][usage/io] of the `{` character in `word`
* [bench/fat][]: new unit
* [drv/pc/ioport][]
    * add `PORTX` `IQUAL` entries for [lib/ival][]
* [drv/pci][]
    * change how initialization is done
    * add `pcifilter` `pcifilter1`
* [drv/sunxi/timer][]: new unit
* [drv/sunxi/usb][]: new unit
* [drv/usb/blk][]: new unit
* [drv/usb/ehci][]: new unit
* [drv/usb/uhci][]
    * support systems with more than one UHCI controller
* [hal][]
    * add `-n,`
* [io/kbd][]
    * slightly change the meaning of the `Passthrough` NKC flag
* `lib/bm`: remove unit
* [lib/crc][num/crc]
    * change `crc16` and `crc32` semantics
* [lib/diag][]
    * add `dumpn`
* [lib/ival][]: new unit
* [lib/psrs][]
    * change `roll` and `roll>` semantics
    * add `rollk` and `rollk>`
* [lib/str][]
    * add `consume[]`
* [lib/struct][]
    * remove `absstruct`, replaced by [lib/ival][]
    * add `offsetof`
* [mem/ll][]
    * remove unused `llappend` `llprepend`
* [text/ed][]
    * add `words[]` `wordunder`

## v29 - 2026/03/07

* core
    * remove `immdoes>`
    * add `16*` `16/` `256*` `256/`
* [asm/x86][]
    * change mode changing semantics
    * add `64bmode` `livemode`
    * change `abs)` behavior under `64bmode`
    * add `?d+bp)` `?bp+,` `?bp-,`
* [asm/x87][]: new unit
* [bench/udp][]: new unit
* [com/arp][]: new unit
* [com/ether][]: new unit
* [com/ip4][]: new unit
* [com/link][]: new unit
* [com/net][]: new unit
* [com/slip][]: new unit
* [com/udp][]: new unit
* [drv/pci][]
    * add `busdescendants` `.pciall` `pcifiltervendor` `pcifilterdevice`
* [drv/nic/rtl8169][]: new unit
* [fs/core][]
    * add `parent`
    * move `ensurepath` `ensurepathrel` `ensurefile` `ensuredir` `p"` `f"` to
      [fs/sh][]
* [fs/fat][]
    * change FSID encoding
* [fs/sh][]
    * introduce the concept of global source and destination
    * change semantics of `copyfile` and `copydir`
    * add `path` `filesonly` `dirsonly` `ncopy` `pf"` `pd"`
* [hal/common][]: new unit
* [hal/float][]: new unit
* `hal/shim`: remove unused unit
* [mem/kv][]
    * replace `kv@,` with `kv',`
    * add `kv!` and `kvreplace`
* [num/float][]: new unit
* [num/xxhash][]: new unit
* [lib/bit][]
    * add `lrot` `rrot`
* `lib/crc`: rename to [num/crc][]
* `lib/double`: rename to [num/double][]
* `lib/math`: rename to [num/math][]
* [lib/psrs][]
    * add `nconcat` `nfirst` `nsame`
* [lib/str][]
    * add `rcidx`

## v30 - 2026/04/04

* core
    * add `arch` [top-level directory][dirs]
    * change `bind` and `bind>` signature
* [app/gmux][]: new unit
* [app/prompt][]: new unit
* [app/udc][]: new unit
* [bench/coop][]: new unit
* [bench/uxn][]: new unit merging `bunny` and `uxn*` benches
* [com/arp][]
    * add `arprequest`
* [com/link][]
    * change `recvlink` to `curlink`
* [fs/core][]
    * huge refactoring: FSIDs are gone, replaced by the concept of walking
* [fs/sh][]
    * complete rewrite following [fs/core][]'s refactoring.
* `hal/bit`: remove, merging into [lib/bit][]
* [hal/opq][]
    * add `(signed?` `-&)` `-dir)` `-signed)`
* [hal/vreg][]
    * add `REGR0` `REGR1`
* [io/grid][]
    * add `alldirty`
* `io/prompt`: remove unit in favor of [app/prompt][]
* [io/typeln][]: new unit
* `lib/arch`:
    * rename to [arch/core][]
    * add `archlookup` `arch<<` `needsasm`
* [lib/coop][]
    * replace the `Event` and `Context` structures with [ivalmaps][lib/ival]
    * remove the `Application` structure
    * change event handle signature from `( event -- )` to `( -- )`
    * add the concept of "background application", along with `.bg` and
      `bgdispatch`
* `lib/idle`: remove unit, having [lib/coop][] take care of realiasing `idle`
* [lib/ival][]
    * add the `XT` qualifier
* [lib/str][]
    * add `string` `[]strmove` `upstr` `lowstr`
* [num/chacha][]: new unit

## v31 - 2026/04/30

* core:
    * change signature of findselectors
    * move `not` and `bool` to kernels
* [asm/6502][]
    * rename `<>` to `<0+>`
* [bench/gr][]: new unit replacing the `tests/manual` directory
* [com/dhcp][]: new unit
* [deploy/m68k-virt][]: new deployment
* [drv/pc/rtc][]: rewrite for [lib/time][]
* [drv/pc/ps28042][]
    * add Scan Code Set auto-detection through `8042scancodeset`
* [drv/ps2][]
    * replace `newps2set1kbd` and `newps2set2kbd` with `newps2kbd`
* [emul/cpu][]
    * rename `run1` to `step` and `runN` to `stepN`
* [hal/opq][]
    * move `(&?` `-&)` `(dir?` `-dir)` `(signed?` `-signed)` to core HAL
* [lib/ival][]
    * change semantics of `ivalue` `ivalmap` `absvalmap`
    * add `ivalmapfrom`
* [lib/struct][]
    * Change structure declaration syntax
* [lib/time][]: new unit
* [lib/type][]
    * adopt [comp/c][] type names, that is, replace `DWord` `Word` and `Byte`
      with `uint` `ushort` and `uchar`, and add `int` `short` `char`
    * change the way `.type` prints its types
    * remove the principles of type categories. Types are now flat structures.
    * move the signature type from [comp/sig][] to this unit
    * add `type<`
* [mem/range][]
    * add `rslide-`

## v32 - Brewing...

* core:
    * add `ZERO` `ONE`
    * add `lrot` `rrot`
* hal:
    * add `lrot,` `rrot,` `!n,`
* posix:
    * implement [lib/time][]'s `now`
* [app/ed][]
    * Improve usability with the introduction of the
      Normal/Insert/InsertLine/Replace modes
* `app/edx`: merge into [app/ed][]
* [com/link][]
    * change `readframe` and `beginframe` semantics
    * add `frametype` global value
    * add frame logging mechanism
* [com/net][]
    * add `.frame`
* [drv/nic/loop][]: new unit
* [fs/core][]
    * add `writefsnode` `walkmtime` `walkdepth`
* [fs/sh][]
    * remove `copyfiles` `copydirs`
    * add `copyfile.` `.walk` `walkdo` `walkdoboth`
* [hal/instr][]
    * add multibyte operations
    * add `d*,`
* [io/roll][mem/roll]
    * rename to [mem/roll][]
    * add `newroller` `reset` `writeahead` `advancewindow`
* [io/stream][]
    * remove unused `readline` words
    * remove unused `capture`
    * add `incposk`
* [lib/coop][]
    * add `context[` `]context`
    * add background tasks control words
* [lib/psrs][]
    * add `ndup`
* [lib/time][]
    * add `minutes` `hours` `days` `ago`
* [text/ed][]
    * Transform `Edbuf` struct into an ivalmap [lib/ival], thus modifying the
      API of the whole unit. Now, all words implicitly target the active edbuf.
    * Rather than extending `Stream` like the former `Edbuf` did, we provide
      a separate `edstream` that wraps the active edbuf into stream semantics.

[deploy/efi]: deploy/efi/README.md
[deploy/pc]: deploy/pc/README.md
[deploy/mac68k]: deploy/mac68k/README.md
[deploy/m68k-virt]: deploy/m68k-virt/README.md
[deploy/sunxi]: deploy/sunxi/README.md

[comment]: <> (links below generated with "./doclinks.sh fs/")

[app/bed]: fs/doc/app/bed.txt
[app/ed]: fs/doc/app/ed.txt
[app/gcon]: fs/doc/app/gcon.txt
[app/gmux]: fs/doc/app/gmux.txt
[app/prompt]: fs/doc/app/prompt.txt
[app/turye]: fs/doc/app/turye.txt
[app/udc]: fs/doc/app/udc.txt
[arch/core]: fs/doc/arch/core.txt
[ar/tar]: fs/doc/ar/tar.txt
[ar/tarp]: fs/doc/ar/tarp.txt
[asm/6502]: fs/doc/asm/6502.txt
[asm/armd]: fs/doc/asm/armd.txt
[asm/arm]: fs/doc/asm/arm.txt
[asm/dis]: fs/doc/asm/dis.txt
[asm]: fs/doc/asm.txt
[asm/label]: fs/doc/asm/label.txt
[asm/m68k]: fs/doc/asm/m68k.txt
[asm/riscvd]: fs/doc/asm/riscvd.txt
[asm/riscv]: fs/doc/asm/riscv.txt
[asm/uxntal]: fs/doc/asm/uxntal.txt
[asm/x86d]: fs/doc/asm/x86d.txt
[asm/x86]: fs/doc/asm/x86.txt
[asm/x87]: fs/doc/asm/x87.txt
[bench/coop]: fs/doc/bench/coop.txt
[bench/fat]: fs/doc/bench/fat.txt
[bench/fdc]: fs/doc/bench/fdc.txt
[bench/gr]: fs/doc/bench/gr.txt
[bench/kbd]: fs/doc/bench/kbd.txt
[bench/mem]: fs/doc/bench/mem.txt
[bench/mouse]: fs/doc/bench/mouse.txt
[bench/udp]: fs/doc/bench/udp.txt
[bench/uxn]: fs/doc/bench/uxn.txt
[boot]: fs/doc/boot.txt
[break]: fs/doc/break.txt
[code]: fs/doc/code.txt
[com/arp]: fs/doc/com/arp.txt
[com/dhcp]: fs/doc/com/dhcp.txt
[com/ether]: fs/doc/com/ether.txt
[com/ip4]: fs/doc/com/ip4.txt
[com/link]: fs/doc/com/link.txt
[com/net]: fs/doc/com/net.txt
[comp/c/ast]: fs/doc/comp/c/ast.txt
[comp/c/expr]: fs/doc/comp/c/expr.txt
[comp/c]: fs/doc/comp/c.txt
[comp/c/stmt]: fs/doc/comp/c/stmt.txt
[comp/c/type]: fs/doc/comp/c/type.txt
[comp/lisp]: fs/doc/comp/lisp.txt
[comp/oberon/ast]: fs/doc/comp/oberon/ast.txt
[comp/oberon]: fs/doc/comp/oberon.txt
[comp/oberon/gc]: fs/doc/comp/oberon/gc.txt
[comp/oberon/gen]: fs/doc/comp/oberon/gen.txt
[comp/oberon/mem]: fs/doc/comp/oberon/mem.txt
[comp/oberon/module]: fs/doc/comp/oberon/module.txt
[comp/oberon/tok]: fs/doc/comp/oberon/tok.txt
[comp/oberon/type]: fs/doc/comp/oberon/type.txt
[comp/sig]: fs/doc/comp/sig.txt
[comp/sym]: fs/doc/comp/sym.txt
[comp/tok]: fs/doc/comp/tok.txt
[comp/w]: fs/doc/comp/w.txt
[com/slip]: fs/doc/com/slip.txt
[com/tcp]: fs/doc/com/tcp.txt
[com/udp]: fs/doc/com/udp.txt
[com/xmodem]: fs/doc/com/xmodem.txt
[data]: fs/doc/data.txt
[deploy]: fs/doc/deploy.txt
[design/async]: fs/doc/design/async.txt
[design]: fs/doc/design.txt
[design/limits]: fs/doc/design/limits.txt
[design/port]: fs/doc/design/port.txt
[design/purpose]: fs/doc/design/purpose.txt
[design/shell]: fs/doc/design/shell.txt
[design/simple]: fs/doc/design/simple.txt
[design/speed]: fs/doc/design/speed.txt
[design/test]: fs/doc/design/test.txt
[dict]: fs/doc/dict.txt
[dirs]: fs/doc/dirs.txt
[drv/arm/cache]: fs/doc/drv/arm/cache.txt
[drv/arm/exc]: fs/doc/drv/arm/exc.txt
[drv/arm/mmu]: fs/doc/drv/arm/mmu.txt
[drv/arm/psr]: fs/doc/drv/arm/psr.txt
[drv/arm/sccp]: fs/doc/drv/arm/sccp.txt
[drv/efi/blkio]: fs/doc/drv/efi/blkio.txt
[drv/efi/devpath]: fs/doc/drv/efi/devpath.txt
[drv/efi]: fs/doc/drv/efi.txt
[drv/efi/gop]: fs/doc/drv/efi/gop.txt
[drv/efi/grid]: fs/doc/drv/efi/grid.txt
[drv/efi/image]: fs/doc/drv/efi/image.txt
[drv/efi/kbdex]: fs/doc/drv/efi/kbdex.txt
[drv/efi/kbd]: fs/doc/drv/efi/kbd.txt
[drv/efi/mouse]: fs/doc/drv/efi/mouse.txt
[drv/efi/timer]: fs/doc/drv/efi/timer.txt
[drv/efi/uga]: fs/doc/drv/efi/uga.txt
[drv/mac68k/mouse]: fs/doc/drv/mac68k/mouse.txt
[drv/mac68k/qd]: fs/doc/drv/mac68k/qd.txt
[drv/mac68k/screen]: fs/doc/drv/mac68k/screen.txt
[drv/mac68k/serial]: fs/doc/drv/mac68k/serial.txt
[drv/mac68k/timer]: fs/doc/drv/mac68k/timer.txt
[drv/mac68k/xpram]: fs/doc/drv/mac68k/xpram.txt
[drv/nic/loop]: fs/doc/drv/nic/loop.txt
[drv/nic/rtl8169]: fs/doc/drv/nic/rtl8169.txt
[drv/pc/ahci]: fs/doc/drv/pc/ahci.txt
[drv/pc/ata]: fs/doc/drv/pc/ata.txt
[drv/pc/bios13]: fs/doc/drv/pc/bios13.txt
[drv/pc/com]: fs/doc/drv/pc/com.txt
[drv/pc/fdc]: fs/doc/drv/pc/fdc.txt
[drv/pci]: fs/doc/drv/pci.txt
[drv/pc/int10h]: fs/doc/drv/pc/int10h.txt
[drv/pc/int13h]: fs/doc/drv/pc/int13h.txt
[drv/pc/ioport]: fs/doc/drv/pc/ioport.txt
[drv/pc/pci]: fs/doc/drv/pc/pci.txt
[drv/pc/pit]: fs/doc/drv/pc/pit.txt
[drv/pc/ps28042]: fs/doc/drv/pc/ps28042.txt
[drv/pc/rtc]: fs/doc/drv/pc/rtc.txt
[drv/pc/vesa]: fs/doc/drv/pc/vesa.txt
[drv/pc/vga]: fs/doc/drv/pc/vga.txt
[drv/ps2]: fs/doc/drv/ps2.txt
[drv/rpi/break]: fs/doc/drv/rpi/break.txt
[drv/rpi/dwc]: fs/doc/drv/rpi/dwc.txt
[drv/rpi/emmc]: fs/doc/drv/rpi/emmc.txt
[drv/rpi/gpio]: fs/doc/drv/rpi/gpio.txt
[drv/rpi/intr]: fs/doc/drv/rpi/intr.txt
[drv/rpi/pwr]: fs/doc/drv/rpi/pwr.txt
[drv/rpi/timer]: fs/doc/drv/rpi/timer.txt
[drv/rpi/uart]: fs/doc/drv/rpi/uart.txt
[drv/rpi/vcore]: fs/doc/drv/rpi/vcore.txt
[drv/sunxi/smhc]: fs/doc/drv/sunxi/smhc.txt
[drv/sunxi/timer]: fs/doc/drv/sunxi/timer.txt
[drv/sunxi/usb]: fs/doc/drv/sunxi/usb.txt
[drv/timer]: fs/doc/drv/timer.txt
[drv/usb/blk]: fs/doc/drv/usb/blk.txt
[drv/usb/ehci]: fs/doc/drv/usb/ehci.txt
[drv/usb]: fs/doc/drv/usb.txt
[drv/usb/kbd]: fs/doc/drv/usb/kbd.txt
[drv/usb/mouse]: fs/doc/drv/usb/mouse.txt
[drv/usb/uhci]: fs/doc/drv/usb/uhci.txt
[emul/6502]: fs/doc/emul/6502.txt
[emul/cpu]: fs/doc/emul/cpu.txt
[emul/oberon]: fs/doc/emul/oberon.txt
[emul/uxn]: fs/doc/emul/uxn.txt
[emul/varvara]: fs/doc/emul/varvara.txt
[emul/virtio]: fs/doc/emul/virtio.txt
[fs/core]: fs/doc/fs/core.txt
[fs/fat]: fs/doc/fs/fat.txt
[fs/fatt]: fs/doc/fs/fatt.txt
[fs/hfs]: fs/doc/fs/hfs.txt
[fs/search]: fs/doc/fs/search.txt
[fs/sh]: fs/doc/fs/sh.txt
[fs/tar]: fs/doc/fs/tar.txt
[fs/utag]: fs/doc/fs/utag.txt
[gr/blt]: fs/doc/gr/blt.txt
[gr/buf]: fs/doc/gr/buf.txt
[gr/color]: fs/doc/gr/color.txt
[gr/damage]: fs/doc/gr/damage.txt
[gr/font/bit]: fs/doc/gr/font/bit.txt
[gr/font]: fs/doc/gr/font.txt
[gr/font/uf1]: fs/doc/gr/font/uf1.txt
[gr/font/uf2]: fs/doc/gr/font/uf2.txt
[gr/grid]: fs/doc/gr/grid.txt
[gr/rdwr]: fs/doc/gr/rdwr.txt
[gr/varvara]: fs/doc/gr/varvara.txt
[hal/common]: fs/doc/hal/common.txt
[hal/float]: fs/doc/hal/float.txt
[hal]: fs/doc/hal.txt
[hal/instr]: fs/doc/hal/instr.txt
[hal/muldiv]: fs/doc/hal/muldiv.txt
[hal/opq]: fs/doc/hal/opq.txt
[hal/vreg]: fs/doc/hal/vreg.txt
[howto/asmrings]: fs/doc/howto/asmrings.txt
[howto/dev]: fs/doc/howto/dev.txt
[howto/net]: fs/doc/howto/net.txt
[howto/port]: fs/doc/howto/port.txt
[howto/unixtty]: fs/doc/howto/unixtty.txt
[hw/amd64]: fs/doc/hw/amd64.txt
[hw/arm]: fs/doc/hw/arm.txt
[hw/arm/rpi]: fs/doc/hw/arm/rpi.txt
[hw/arm/sunxi]: fs/doc/hw/arm/sunxi.txt
[hw/efi]: fs/doc/hw/efi.txt
[hw/i386]: fs/doc/hw/i386.txt
[hw/i386/hpmini]: fs/doc/hw/i386/hpmini.txt
[hw/i386/mac]: fs/doc/hw/i386/mac.txt
[hw/i386/pc]: fs/doc/hw/i386/pc.txt
[hw/m68k]: fs/doc/hw/m68k.txt
[hw/m68k/mac]: fs/doc/hw/m68k/mac.txt
[hw/riscv]: fs/doc/hw/riscv.txt
[index]: fs/doc/index.txt
[intr]: fs/doc/intr.txt
[io/blk]: fs/doc/io/blk.txt
[io/grid]: fs/doc/io/grid.txt
[io/kbd]: fs/doc/io/kbd.txt
[io/mouse]: fs/doc/io/mouse.txt
[io/part]: fs/doc/io/part.txt
[io/part/mbr]: fs/doc/io/part/mbr.txt
[io/ride]: fs/doc/io/ride.txt
[io/search]: fs/doc/io/search.txt
[io/secld]: fs/doc/io/secld.txt
[io/stream]: fs/doc/io/stream.txt
[io/typeln]: fs/doc/io/typeln.txt
[io/wait]: fs/doc/io/wait.txt
[kernel]: fs/doc/kernel.txt
[lib/bit]: fs/doc/lib/bit.txt
[lib/coop]: fs/doc/lib/coop.txt
[lib/diag]: fs/doc/lib/diag.txt
[lib/fmt]: fs/doc/lib/fmt.txt
[lib/ival]: fs/doc/lib/ival.txt
[lib/macro]: fs/doc/lib/macro.txt
[lib/match]: fs/doc/lib/match.txt
[lib/psrs]: fs/doc/lib/psrs.txt
[lib/str]: fs/doc/lib/str.txt
[lib/struct]: fs/doc/lib/struct.txt
[lib/tagl]: fs/doc/lib/tagl.txt
[lib/time]: fs/doc/lib/time.txt
[lib/type]: fs/doc/lib/type.txt
[lib/wordtbl]: fs/doc/lib/wordtbl.txt
[mem/alloc]: fs/doc/mem/alloc.txt
[mem/arena]: fs/doc/mem/arena.txt
[mem/array]: fs/doc/mem/array.txt
[mem/blk]: fs/doc/mem/blk.txt
[mem/cons]: fs/doc/mem/cons.txt
[mem/dict]: fs/doc/mem/dict.txt
[mem/here]: fs/doc/mem/here.txt
[mem/kv]: fs/doc/mem/kv.txt
[mem/ll]: fs/doc/mem/ll.txt
[mem/mark]: fs/doc/mem/mark.txt
[mem/pool]: fs/doc/mem/pool.txt
[mem/range]: fs/doc/mem/range.txt
[mem/reuse]: fs/doc/mem/reuse.txt
[mem/roll]: fs/doc/mem/roll.txt
[mem/scratch]: fs/doc/mem/scratch.txt
[mem/sort]: fs/doc/mem/sort.txt
[mem/stack]: fs/doc/mem/stack.txt
[mem/stream]: fs/doc/mem/stream.txt
[num/chacha]: fs/doc/num/chacha.txt
[num/crc]: fs/doc/num/crc.txt
[num/double]: fs/doc/num/double.txt
[num/float]: fs/doc/num/float.txt
[num/math]: fs/doc/num/math.txt
[num/xxhash]: fs/doc/num/xxhash.txt
[oberon/arg]: fs/doc/oberon/arg.txt
[oberon/clipboar]: fs/doc/oberon/clipboar.txt
[oberon/convertp]: fs/doc/oberon/convertp.txt
[oberon/display]: fs/doc/oberon/display.txt
[oberon/draw]: fs/doc/oberon/draw.txt
[oberon/dumpfont]: fs/doc/oberon/dumpfont.txt
[oberon/files]: fs/doc/oberon/files.txt
[oberon/fonts]: fs/doc/oberon/fonts.txt
[oberon/fontsubs]: fs/doc/oberon/fontsubs.txt
[oberon]: fs/doc/oberon.txt
[oberon/growfont]: fs/doc/oberon/growfont.txt
[oberon/input]: fs/doc/oberon/input.txt
[oberon/menuview]: fs/doc/oberon/menuview.txt
[oberon/oberon]: fs/doc/oberon/oberon.txt
[oberon/optimize]: fs/doc/oberon/optimize.txt
[oberon/system]: fs/doc/oberon/system.txt
[oberon/textfram]: fs/doc/oberon/textfram.txt
[oberon/texts]: fs/doc/oberon/texts.txt
[oberon/usage]: fs/doc/oberon/usage.txt
[oberon/viewers]: fs/doc/oberon/viewers.txt
[operator]: fs/doc/operator.txt
[qa]: fs/doc/qa.txt
[sig]: fs/doc/sig.txt
[terms]: fs/doc/terms.txt
[tests]: fs/doc/tests.txt
[text/ansi]: fs/doc/text/ansi.txt
[text/clip]: fs/doc/text/clip.txt
[text/ed]: fs/doc/text/ed.txt
[text/ts]: fs/doc/text/ts.txt
[tips]: fs/doc/tips.txt
[tour]: fs/doc/tour.txt
[usage/file]: fs/doc/usage/file.txt
[usage/flow]: fs/doc/usage/flow.txt
[usage]: fs/doc/usage.txt
[usage/io]: fs/doc/usage/io.txt
[usage/lit]: fs/doc/usage/lit.txt
[usage/mem]: fs/doc/usage/mem.txt
[usage/tag]: fs/doc/usage/tag.txt
[usage/to]: fs/doc/usage/to.txt
[usage/unit]: fs/doc/usage/unit.txt
[usage/word]: fs/doc/usage/word.txt
[xcomp/amd64/efi/deploy]: fs/doc/xcomp/amd64/efi/deploy.txt
[xcomp/amd64/hal]: fs/doc/xcomp/amd64/hal.txt
[xcomp/arm/hal]: fs/doc/xcomp/arm/hal.txt
[xcomp/arm/rpi/deploy]: fs/doc/xcomp/arm/rpi/deploy.txt
[xcomp/deploy]: fs/doc/xcomp/deploy.txt
[xcomp/efi]: fs/doc/xcomp/efi.txt
[xcomp/hallo]: fs/doc/xcomp/hallo.txt
[xcomp/i386/efi/deploy]: fs/doc/xcomp/i386/efi/deploy.txt
[xcomp/i386/hal]: fs/doc/xcomp/i386/hal.txt
[xcomp/i386/pc/deploy]: fs/doc/xcomp/i386/pc/deploy.txt
[xcomp/i386/pc/mbr]: fs/doc/xcomp/i386/pc/mbr.txt
[xcomp/i386/pc/mkfat]: fs/doc/xcomp/i386/pc/mkfat.txt
[xcomp/i386/pc/pbr]: fs/doc/xcomp/i386/pc/pbr.txt
[xcomp/lo]: fs/doc/xcomp/lo.txt
[xcomp/m68k/cpusel]: fs/doc/xcomp/m68k/cpusel.txt
[xcomp/m68k/hal]: fs/doc/xcomp/m68k/hal.txt
[xcomp/m68k/mac/bootdbg]: fs/doc/xcomp/m68k/mac/bootdbg.txt
[xcomp/m68k/mac/deploy]: fs/doc/xcomp/m68k/mac/deploy.txt
[xcomp/riscv/hal]: fs/doc/xcomp/riscv/hal.txt
[xcomp/tools]: fs/doc/xcomp/tools.txt
[xcomp/x86/hal]: fs/doc/xcomp/x86/hal.txt
