TARGETS = floppy.img
include ../common.mk

floppy.img: $(DUSK)
	dd if=/dev/zero of=$@ bs=512 count=2880
	$(DUSK) -n fs/fat -n xcomp/m68k/mac/deploy -f deploy.fs -c '"$@" mountImage deploy'
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.fs' < init.fs
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.txt' < init.txt
