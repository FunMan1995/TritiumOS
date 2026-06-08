TARGET = alix.img
TARGETS = $(TARGET)
include ../common.mk

$(TARGET): $(DUSK) init.fs
	dd if=/dev/zero of=$@ bs=512 count=16384 # 8M
	$(DUSK) -c '"$@" mountImage' -f mkfat.fs
	$(DUSK) -c 'f<< xcomp/i386/pc/deploy.fs "$@" mountImage install'
	$(DUSK) -c '"$@"' -f $(DUSKDIR)/syncfs.fs
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.fs' < init.fs
