MKPREFIX = ../
include ../mk/plat-$(PLAT).mk
include ../mk/arch-$(ARCH).mk
TARGETS = dusk dusk-sdl
SDL2_CONFIG ?= sdl2-config
KERNEL = kernel-$(ARCH)
PAYLOAD = payload-$(ARCH)
KERNELSRC = $(KERNEL).fs
CFLAGS += -DARCH_$(ARCH) -DPLAT_$(PLAT) -DKERNELNAME=\"kernel-$(ARCH)\"
CCCMD = $(CC) $(CFLAGS) $(MACHINE_FLAGS) $(LDFLAGS)
FS = ../fs
CSRC = ../posix/opts.c ../posix/io.c ../posix/fd.c ../posix/time.c common.c
RAWCSRC = dusk-raw.c ../posix/tardrv.c $(CSRC)
SDLCSRC = dusk-sdl.c ../posix/sdl.c $(CSRC)

.PHONY: all clean run
all: $(TARGETS)

../dusk: $(ALLSRCS)
	$(MAKE) -C .. dusk

$(KERNEL): ../dusk $(KERNELSRC)
	../dusk -e -c "stdio interpretstream kernel kernellen stdio write#" \
		< $(KERNELSRC) > $@

kernel.h: $(KERNEL)
	../embedh.sh kernel < $(KERNEL) > $@

# payload-core and payload-tarfs targets are used in Dusk Packages makefiles.
$(PAYLOAD)-core: ../dusk ../posix/fstar.h
	../dusk -e -n xcomp/deploy -c ' stdio "$(ARCH)" spitboot ' \
		| cat - api.fs > $@

$(PAYLOAD)-tarfs: $(PAYLOAD)-core api.fs
	../dusk -n xcomp/deploy \
		-c 'stdio dup fsUnits spitunits tarUnits spitunits' | \
		cat $(PAYLOAD)-core - ../posix/fd.fs ../posix/bootdrv.fs > $@

payload_raw.h: $(PAYLOAD)-tarfs
	cat $(PAYLOAD)-tarfs glue.fs | ../embedh.sh payload > $@

disk.img: dusk ../posix/fstar.h
	dd if=/dev/zero of=$@ bs=512 count=16384 # 8M
	./dusk -c '1 4 "$@"' -f ../makefat.fs
	./dusk -c '"$@"' -f ../syncfs.fs

$(PAYLOAD)-fat: dusk $(PAYLOAD)-core disk.img api.fs fatboot.fs glue.fs
	../dusk -n xcomp/deploy \
		-c 'stdio dup fsUnits spitunits fatUnits spitunits' | \
		cat $(PAYLOAD)-core - ../posix/fd.fs fatboot.fs glue.fs > $@

$(PAYLOAD)-graphic: $(PAYLOAD)-fat ../posix/graphic.fs
	cat $(PAYLOAD)-fat ../posix/graphic.fs > $@

dusk: ../dusk $(RAWCSRC) ../posix/fstar.h kernel.h payload_raw.h
	$(CCCMD) $(RAWCSRC) -o $@
	$(POSTCMD)

dusk-sdl: $(SDLCSRC) $(KERNEL) $(PAYLOAD)-graphic
	$(CCCMD) -DPAYLOADNAME=\"$(PAYLOAD)-graphic\" \
		$(SDLCSRC) -o $@ $(SDL2_CFLAGS)
	$(POSTCMD)

clean:
	$(MAKE) -C .. clean
	rm -f $(TARGETS) dusk-conio *.o *.exe disk.img \
		$(KERNEL) kernel.h payload*

run: dusk
	stty -icanon -echo min 0; ./dusk ; stty icanon echo
