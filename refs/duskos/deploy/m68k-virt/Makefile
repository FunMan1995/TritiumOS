TARGETS = kernel.img disk.img
include ../common.mk
M68KVIRT_PATH = ../../../m68k-virt/m68k-virt

disk.img: $(DUSK) init.fs
	dd if=/dev/zero of=$@ bs=512 count=8192
	$(DUSK) -c '1 4 "$@"' -f ../../makefat.fs
	$(DUSK) -c '"$@"' -f ../../syncfs.fs
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.fs' < init.fs

kernel.img: $(DUSK)
	$(DUSK) -e -n xcomp/m68k/virt/deploy -c 'stdio install' > $@

emul: kernel.img disk.img
	stty -icanon -echo; $(M68KVIRT_PATH) kernel.img disk.img; stty icanon echo
