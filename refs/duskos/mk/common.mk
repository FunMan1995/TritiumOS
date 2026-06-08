include mk/plat-$(PLAT).mk
include mk/arch-$(ARCH).mk
PRELUDE = fs/xcomp/lo.fs fs/xcomp/arm/hallo.fs fs/xcomp/hallo.fs \
	fs/xcomp/arm/hal.fs fs/xcomp/boot.fs
BOOTFS_SRC = fs/lib/str.fs fs/lib/type.fs fs/lib/struct.fs fs/lib/ival.fs \
	fs/io/stream.fs fs/fs/core.fs fs/io/blk.fs fs/ar/tar.fs fs/fs/tar.fs \
	posix/fd.fs posix/bootdrv.fs
BOOT_STREAM = $(BOOTFS_SRC) posix/base.fs
BOOT_GRID = $(BOOTFS_SRC) posix/base.fs posix/grid.fs
BOOT_GRAPHIC = $(BOOTFS_SRC) posix/base.fs posix/graphic.fs
CSRC = posix/vm.c posix/opts.c posix/tardrv.c posix/io.c posix/fd.c posix/time.c
STREAMCSRC = posix/dusk-stream.c $(CSRC)
GRIDCSRC = posix/dusk-grid.c posix/grid.c $(CSRC)
SDLCSRC = posix/dusk-sdl.c posix/sdl.c $(CSRC)
SDL2_CONFIG ?= sdl2-config
all: dusk

posix/fs.tar: $(ALLSRCS) posix/init.txt
	tar -cf $@ -C fs $(TAR_FLAGS) .
	tar -rf $@ -C posix ./init.txt

posix/fstar.h: posix/fs.tar
	./embedh.sh fstar < posix/fs.tar > $@

posix/boot-stream.h: $(PRELUDE) $(BOOT_STREAM)
	cat $(PRELUDE) $(BOOT_STREAM) | ./embedh.sh bootstring > $@

dusk: $(STREAMCSRC) posix/boot-stream.h posix/fstar.h
	$(CC) $(STREAMCSRC) -Wall -o $@

posix/boot-grid.h: $(PRELUDE) $(BOOT_GRID)
	cat $(PRELUDE) $(BOOT_GRID) | ./embedh.sh bootstring > $@

dusk-grid: $(GRIDCSRC) posix/boot-grid.h posix/fstar.h
	$(CC) $(GRIDCSRC) -Wall -o $@

posix/boot-graphic.h: $(PRELUDE) $(BOOT_GRAPHIC)
	cat $(PRELUDE) $(BOOT_GRAPHIC) | ./embedh.sh bootstring > $@

dusk-sdl: $(SDLCSRC) posix/boot-graphic.h posix/fstar.h
	$(CC) $(SDLCSRC) -Wall -o $@ $(SDL2_CFLAGS)

kbdconv: posix/kbdconv.c
	$(CC) -Wall $< -o $@

.PHONY: testum
testum:
	$(MAKE) -C usermode dusk
	./usermode/dusk -c "f<< tests/all.fs"

.PHONY: run
run: dusk
	stty -icanon -echo min 0; ./dusk ; stty icanon echo

.PHONY: rungrid
rungrid: dusk-grid
	stty -icanon -echo min 0; ./dusk-grid 2>/dev/null ; stty icanon echo

# testing a subfolder: make TESTDIR=comp test
.PHONY: test
test: dusk
	if [ -z "$(TESTDIR)" ]; then \
	(./dusk -f fs/tests/all.fs) \
	else \
	(./dusk -n tests/harness -c "rundir<< tests/$(TESTDIR)") \
	fi

.PHONY: clean
clean:
	rm -f dusk dusk.o posix/boot*.h posix/fs.tar posix/fstar.h memdump kbdconv
