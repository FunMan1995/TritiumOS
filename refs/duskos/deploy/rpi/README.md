# Raspberry Pi

This subfolder produces images that boot Dusk OS on a Raspberry Pi. This image
is known to work on models:

* 1
* 2
* 3
* Zero W
* Zero 2 W

The console outputs to the HDMI screen and takes its input from a USB keyboard
that has to be connected to one of the USB ports at boot.

If a USB mouse is connected at boot time, it will be picked up by the
configuration and hooked to io/mouse.

Detailed documentation is at Dusk's `doc/hw/rpi.txt`.

## Requirements

Unlike other deployments, this one requires [mtools][]. It seems that the RPi
needs FAT long filenames entries for it to find its boot files, something that
Dusk's FAT doesn't have yet.

[mtools]: https://www.gnu.org/software/mtools/

## Usage

Running `make` produces `rpifat.img`. This image is compatible with RPi models 1
2 and 3 (it contains both the model 1 kernel as `kernel.img` and the model 2/3
kernel as `kernel7.img`).

`make emul1` will launch QEMU for the model 1, `make emul2` will do so for the
model 2.

The image contains everything it needs to be bootable by an actual RPi.
All you need to do is to copy (`dd`) the raw image to a SD card. It should boot!

The default `init.fs` sets up the framebuffer linked to the HDMI output as the
console's output and expects a USB keyboard to be plugged to one of the USB
ports. That keyboard is the input. For now, the Boot Protocol is used for
communication with the keyboard, so it's possible that some models of USB
keyboards don't work. The keyboard has to be plugged before powering on the Pi.

## UART version

There is a `uartinit.fs` file in this directory that is an alternative boot
configuration if, instead of having a USB keyboard and framebuffer grid as a
console, you use a UART plugged on GPIO pins 14 and 15.

To use it, copy this file over the `init.fs` file in the resulting `rpifat.img`
image.
