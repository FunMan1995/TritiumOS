TARGETS = riscvkernel.img riscvfat.img
include ../common.mk
KERNELSRC = $(FS)/xcomp/riscv/virt/kernel.fs

riscvkernel.img: $(DUSK)
	$(DUSK) -e -c 'f<< xcomp/riscv/virt/deploy.fs makekernel stdio spitkernel' > $@

riscvfat.img: $(DUSK) init.fs
	dd if=/dev/zero of=$@ bs=512 count=16384 # 8M
	$(DUSK) -c '1 4 "$@"' -f $(DUSKDIR)/makefat.fs
	$(DUSK) -c '"$@"' -f $(DUSKDIR)/syncfs.fs
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.fs' < init.fs

emul: $(TARGETS)
	qemu-system-riscv32 -M virt -bios none -serial mon:stdio -display none \
		-device loader,addr=0x80000000,file=riscvkernel.img \
			-device loader,addr=0x83000000,file=riscvfat.img
