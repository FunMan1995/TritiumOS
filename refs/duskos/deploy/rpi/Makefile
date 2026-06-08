TARGETS = rpifat.img
TOCLEAN = rpikernel1.img rpikernel2.img
include ../common.mk

start.elf:
	curl -LO https://github.com/raspberrypi/firmware/raw/refs/tags/1.20241126/boot/start.elf

bootcode.bin:
	curl -LO https://github.com/raspberrypi/firmware/raw/refs/tags/1.20241126/boot/bootcode.bin

rpikernel1.img: $(DUSK)
	$(DUSK) -e -c 'f<< xcomp/arm/rpi/deploy.fs makekernel1 stdio spitkernel' > $@

rpikernel2.img: $(DUSK)
	$(DUSK) -e -c 'f<< xcomp/arm/rpi/deploy.fs makekernel2 stdio spitkernel' > $@

rpifat.img: $(DUSK) start.elf bootcode.bin rpikernel1.img rpikernel2.img init.fs init.txt
	../chk.sh
	dd if=/dev/zero of=$@ bs=512 count=65536 # 32M
	$(DUSK) -c '1 4 "$@"' -f $(DUSKDIR)/makefat.fs
	$(DUSK) -c '"$@"' -f $(DUSKDIR)/syncfs.fs
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.fs' < init.fs
	$(DUSK) -f $(DUSKDIR)/fatcp.fs -c '"$@" mountImage cp init.txt' < init.txt
	# These below require mtools because the RPi needs long FAT filenames
	mcopy -sQ -i $@ rpikernel1.img ::kernel.img
	mcopy -sQ -i $@ rpikernel2.img ::kernel7.img
	mcopy -sQ -i $@ start.elf bootcode.bin ::

.PHONY: emul1
emul1: rpifat.img rpikernel1.img
	qemu-system-arm -M raspi1ap \
		-device loader,addr=0x8000,file=rpikernel1.img \
		-drive file=rpifat.img,if=sd,format=raw \
		-device usb-kbd -device usb-mouse

.PHONY: emul2
emul2: rpifat.img rpikernel2.img
	qemu-system-arm -M raspi2b \
		-device loader,addr=0x8000,file=rpikernel2.img \
		-drive file=rpifat.img,if=sd,format=raw \
		-device usb-kbd -device usb-mouse
