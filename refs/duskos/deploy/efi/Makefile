TARGETS = disk.img diskt.img disku.img
TOCLEAN = dusk*.efi OVMF*.fd common.img
include ../common.mk
QEMU_FLAGS = -accel kvm -accel tcg -rtc base=utc
OVMFDIR = /usr/share/OVMF

duskx64.efi: $(DUSK)
	$(DUSK) -e -c 'f<< xcomp/amd64/efi/deploy.fs stdio spitefi' > $@

duskia32.efi: $(DUSK)
	$(DUSK) -e -c 'f<< xcomp/i386/efi/deploy.fs stdio spitefi' > $@

common.img: duskx64.efi duskia32.efi
	dd if=/dev/zero of=$@ bs=1024 count=8192 # 8M
	$(DUSK) -c '1 4 "$@"' -f $(DUSKDIR)/makefat.fs
	$(DUSK) -c '"$@"' -f $(DUSKDIR)/syncfs.fs
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp efi/boot/bootx64.efi' < duskx64.efi
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp efi/boot/bootia32.efi' < duskia32.efi

diskt.img: common.img initt.fs init.txt
	cp common.img $@
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.fs' < initt.fs
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.txt' < init.txt

disk.img: common.img init.fs init.txt
	cp common.img $@
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.fs' < init.fs
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.txt' < init.txt

disku.img: common.img
	cp common.img $@
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.fs' < initu.fs

OVMF_CODE_4M.fd: $(OVMFDIR)/OVMF_CODE_4M.fd
	cp $(OVMFDIR)/OVMF_CODE_4M.fd $@

OVMF32_CODE_4M.fd: $(OVMFDIR)/OVMF32_CODE_4M.fd
	cp $(OVMFDIR)/OVMF32_CODE_4M.fd $@

.PHONY: emul
emul: disk.img OVMF_CODE_4M.fd
	qemu-system-x86_64 $(QEMU_FLAGS) \
		-net none -pflash OVMF_CODE_4M.fd \
		-drive file=disk.img,if=ide,format=raw

.PHONY: emult
emult: diskt.img OVMF_CODE_4M.fd
	qemu-system-x86_64 $(QEMU_FLAGS) \
		-net none -pflash OVMF_CODE_4M.fd \
		-drive file=diskt.img,if=ide,format=raw

.PHONY: emulia32
emulia32: disk.img OVMF32_CODE_4M.fd
	qemu-system-i386 $(QEMU_FLAGS) \
		-net none -pflash OVMF32_CODE_4M.fd \
		-drive file=disk.img,if=ide,format=raw
