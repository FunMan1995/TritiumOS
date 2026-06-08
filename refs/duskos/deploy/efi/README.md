# EFI deployments

This deployment target is for EFI-compatible systems. The default `init.fs`
initializes a graphics-based console built on `gr/grid` that runs a `app/gcon`.

This deployment produces a media that contains both `X64` and `IA32` boot
executables. The same media can thus be used to boot on both 32-bit and
64-bit Intel machines.

## Emulation requirements

* QEMU
* OVMF. Change `OVMFDIR` in Makefile if needed.

## Build and run

`make emul` will launch QEMU+OVMF and boot EFI Dusk OS.

You can also run `make emulia32` to launch the `IA32` version of EFI Dusk.
You'll need the 32-bit version of OVMF to be installed.

### Running on real machine

`make` will yield `disk.img`, a UEFI Dusk OS image. You can `dd` the
`disk.img` image onto a USB stick to have a bootable EFI Dusk media.

### Text Console

Pretty much all UEFI machines support graphics, but sadly, many don't support a
text-based console (*à la* VGA text mode), which is why graphics is the
default. Even sadder, many of those UEFI machines that do support text consoles
have a horrible and slow implementation, making the graphics version preferable.

But some UEFI implementations are good, and on those machines, you might want
to deploy a superior option. That's why you can build `diskt.img`, which is a
text console. You can also emulate it with `make emult`.

### UGA Console option

That's not all. Older Intel Macs don't implement the GOP, but the older UGA
protocol. These macs are as crippled as their more recent counterpart and don't
have a text console. This is the case, for example, of the 2006 Intel iMac.

For these, you'll need `disku.img`, which is like `diskg.img` but uses
`drv/efi/uga` instead of using `drv/efi/gop`.

Those older Macs don't implement UEFI, but a baztardized in-between version of
EFI and UEFI. Unfortunately, this means that they don't have `drv/efi/kbdex`,
which means that their keyboard input too basic to properly run a Grid Console.

Therefore, this deployment boots to `app/prompt`. When the day comes when this
deployment gains USB support, then we will be able to have a full Grid Console
driven through `drv/usb/kbd`.

### Troubleshooting

For troubleshooting, refer to `doc/hw/efi.txt`.
