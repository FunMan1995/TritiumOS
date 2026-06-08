# Pine A64 LTS

This is a work in progress. For now, this produces a Dusk OS prompt on UART0,
mounting its FAT from the SD card. It drives:

* UART
* Timer
* EHCI

## How to build and run

For convenience, binaries needing complicated tools have binaries included here.
See below for instructions on how to rebuild them, but this isn't necessary.

For these instructions, all you need is Make and curl.

1. Run `make` which will yield `dusk.img`
2. Insert a destination sd card.
3. `dd if=dusk.img of=/dev/sdX` where `/dev/sdX` is your SD card device.
4. Put the SD card in the Pine A64 LTS
5. Wire UART0 to something, at 115200 bauds. You can use the EXP connector.
   GND=pin9 TX=pin7 RX=pin8
6. Power it up.
7. You should have a Dusk OS prompt on UART0.

## Running on the PinePhone

The PinePhone (regular, not pro), is the same computer as the Pine A64. The same
SD card created with the steps above can be used in the PinePhone, it's going to
work. Here are the steps.

1. Remove the back cover
2. Disable (switch pointing down) privacy switch 6 (headphone) to enable UART.
3. Plug the special [PinePhone serial cable][serial] in the headphone jack and
the other end on a computer.
4. That USB service is a serial link and can be opened with programs like
"minicom". 115200 bauds. On most linuxes, that device will show up as something
like `/dev/ttyUSB0`. Make sure you disable hardware control flow in your
minicom-like program.
5. Remove the battery.
6. Insert the SD card you've programmed through the steps in the previous
section.
7. Replace the battery (you can also not do it and power through USB instead).
8. With minicom running, hold the power button for at least 2 seconds.
9. You should have a Dusk OS prompt after less than 3 seconds.

[serial]: https://pine64.com/product/pinebook-pinephone-pinetab-serial-console/

## Rebuilding sunxi-spl.bin

U-boot is hardwired to Linux. You need Linux to build this.

You need the aarch64 toolchain. For example, `aarch64-linux-gnu-gcc` needs to
exist on your machine.

    $ git clone https://github.com/u-boot/u-boot.git
    $ cd u-boot
    $ git checkout v2026.01
    $ export CROSS_COMPILE=aarch64-linux-gnu-
    $ make pine64-lts_defconfig
    $ make spl/sunxi-spl.bin

## Rebuilding toaarch32.bin

You need the aarch64 toolchain.

    $ aarch64-linux-gnu-as -march=armv8-a+crc -o toaarch32.o toaarch32.S
    $ aarch64-linux-gnu-objcopy -O binary -j .text toaarch32.o toaarch32.bin
