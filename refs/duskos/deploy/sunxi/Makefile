TARGETS = dusk.img
TOCLEAN = kernel.bin
include ../common.mk

kernel.bin: $(DUSK)
	$(DUSK) -e -n xcomp/arm/sunxi/deploy -c 'makekernel stdio spitkernel' > $@

dusk.img: kernel.bin toaarch32.bin sunxi-spl.bin
	dd if=/dev/zero of=$@ bs=512 count=16384 # 8M
	dd if=sunxi-spl.bin of=$@ bs=512 seek=16 conv=notrunc
	dd if=toaarch32.bin of=$@ bs=512 seek=80 conv=notrunc
	dd if=kernel.bin of=$@ bs=256 seek=161 conv=notrunc
	$(DUSK) -c '220 4 "$@"' -f $(DUSKDIR)/makefat.fs
	$(DUSK) -c '"$@"' -f $(DUSKDIR)/syncfs.fs
