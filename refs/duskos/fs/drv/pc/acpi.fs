needs drv/pc/ioport
unit drv/pc/acpi

\ For now, it's just a place to send a hardcoded shutdown for QEMU...
$604 ioportw acpi_ctl

: bye $2000 to acpi_ctl ;
