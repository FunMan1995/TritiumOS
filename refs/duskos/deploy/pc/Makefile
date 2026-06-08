TOCLEAN = *.img
TARGETS = pc.img
include ../common.mk

QEMU_FLAGS = -accel kvm -accel tcg \
	-chardev stdio,id=char0 \
	-serial chardev:char0 \
	-rtc base=utc
QEMU_DRVOPTS = format=raw,index=0,if=ide

common.img: $(DUSK)
	dd if=/dev/zero of=$@ bs=512 count=16384 # 8M
	$(DUSK) -n xcomp/i386/pc/mkfat -c '"$@" mountImage doit'
	$(DUSK) -c 'f<< xcomp/i386/pc/deploy.fs "$@" mountImage install'
	$(DUSK) -c '"$@"' -f $(DUSKDIR)/syncfs.fs

pc.img: common.img init.fs init.txt
	cp common.img $@
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.fs' < init.fs
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.txt' < init.txt

ata.img: common.img ata.fs
	cp common.img $@
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.fs' < ata.fs
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.txt' < init.txt

ahci.img: common.img ahci.fs
	cp common.img $@
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.fs' < ahci.fs

bare.img: common.img bare.fs
	cp common.img $@
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.fs' < bare.fs

commonpbr.img: $(DUSK)
	dd if=/dev/zero of=$@ bs=512 count=16384 # 8M
	$(DUSK) -n xcomp/i386/pc/mkfat -c '"$@" mountImage doitpbr'
	$(DUSK) -c 'f<< xcomp/i386/pc/deploy.fs "$@" mountImage install'
	$(DUSK) -c '"$@"' -f $(DUSKDIR)/syncfs.fs

pcpbr.img: commonpbr.img
	cp commonpbr.img $@
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.fs' < init.fs

# Emulation only. Note that fdosmbr.txt has a partition entry near the end that
# must agree with pcpbr.img size.
disk.img: fdosmbr.txt
	rm -f disk.img
	base64 -d fdosmbr.txt | dd of=disk.img bs=512 count=1

floppy.img: $(DUSK) floppy.fs floppy.txt
	dd if=/dev/zero of=$@ bs=512 count=2880 # 1.44M
	$(DUSK) -n xcomp/i386/pc/mkfat -c '"$@" mountImage dofloppy'
	$(DUSK) -n xcomp/i386/pc/deploy -c '"$@" mountImage installchs'
	$(DUSK) -n 'fs/fat fs/sh xcomp/i386/pc/deploy' \
		-c 'walkdst "$@" mountImage newfatfs walk copyfloppy'
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.fs' < floppy.fs
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.txt' < floppy.txt

alix.img: common.img alix.fs
	cp common.img $@
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.fs' < alix.fs

.PHONY: emul
emul: pc.img
	qemu-system-i386 $(QEMU_FLAGS) -drive file=pc.img,$(QEMU_DRVOPTS)

.PHONY: emulata
emulata: ata.img
	qemu-system-i386 $(QEMU_FLAGS) -drive file=ata.img,$(QEMU_DRVOPTS)

.PHONY: emulahci
emulahci: ahci.img
	qemu-system-i386 $(QEMU_FLAGS) -machine q35 \
		-drive file=ahci.img,$(QEMU_DRVOPTS)

.PHONY: emulpbr
emulpbr: pcpbr.img disk.img
	dd if=pcpbr.img of=disk.img seek=2048 bs=512
	qemu-system-i386 $(QEMU_FLAGS) -drive file=disk.img,$(QEMU_DRVOPTS)

.PHONY: emulbare
emulbare: bare.img
	qemu-system-i386 $(QEMU_FLAGS) -drive file=bare.img,$(QEMU_DRVOPTS)

.PHONY: emulbarepbr
emulbarepbr: barepbr.img disk.img
	dd if=barepbr.img of=disk.img seek=2048 bs=512
	qemu-system-i386 $(QEMU_FLAGS) -drive file=disk.img,$(QEMU_DRVOPTS)

.PHONY: emulfloppy
emulfloppy: floppy.img
	qemu-system-i386 $(QEMU_FLAGS) -fda floppy.img
