DUSK = $(DUSKDIR)/dusk

.PHONY: all
all: 6502.img

$(DUSK):
	$(MAKE) -C $(DUSKDIR) dusk

blkpack: blkpack.c
	$(CC) blkpack.c -Wall -o $@

cos.blk: core.fs blkpack
	./blkpack < core.fs > $@

6502.img: $(DUSK) 6502b.fs cos.blk
	cat compat.fs 6502b.fs | $(DUSK) -e -c 'stdio interpretstream' > $@

.PHONY: run
run: $(DUSK) cos.blk 6502r.fs 6502.img
	$(DUSK) -f 6502r.fs
